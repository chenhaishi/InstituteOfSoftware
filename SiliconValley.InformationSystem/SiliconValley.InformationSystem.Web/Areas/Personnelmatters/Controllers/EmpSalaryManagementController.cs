using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    public class EmpSalaryManagementController : Controller
    {
        RedisCache rc = new RedisCache();

        //第一次进入月度工资表页面时加载的年月份的方法
        static string GetFirstTime()
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();//员工月度工资
            string mytime = "";
            if (msrmanage.GetEmpMsrData().Where(s => s.IsDel == false).Count() > 0)
            {
                var time = msrmanage.GetEmpMsrData().Where(s => s.IsDel == false).LastOrDefault().YearAndMonth;
                mytime = DateTime.Parse(time.ToString()).Year + "-" + DateTime.Parse(time.ToString()).Month;
            }
            else
            {
                mytime = "";
            }
            return mytime;
        }
        static string FirstTime = GetFirstTime();
        //员工工资管理页面
        // GET: Personnelmatters/EmpSalaryManagement
        public ActionResult SalaryManageIndex()
        {
            ViewBag.yearandmonth = FirstTime;
            return View();
        }
        //工资表数据加载
        public ActionResult EmpSalaryData(int page, int limit, string AppCondition, string ymtime)
        {
            ymtime = FirstTime;
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();//员工月度工资
            EmplSalaryEmbodyManage empsemanage = new EmplSalaryEmbodyManage();//员工工资体系表     
            List<MonthlySalaryRecord> eselist = new List<MonthlySalaryRecord>();
            List<MySalaryObjView> result = new List<MySalaryObjView>();
            EmployeesInfoManage empmanage = new EmployeesInfoManage();//员工信息表
            eselist = msrmanage.GetEmpMsrData().Where(s => s.IsDel == false).ToList();
            if (!string.IsNullOrEmpty(ymtime))
            {
                var time = DateTime.Parse(ymtime);
                eselist = eselist.Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == time.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == time.Month).ToList();
            }
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string deptname = str[1];
                string pname = str[2];
                string Empstate = str[3];
                eselist = eselist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(deptname))
                {
                    eselist = eselist.Where(e => empmanage.GetDeptByEmpid(e.EmployeeId).DeptId == int.Parse(deptname)).ToList();
                }
                if (!string.IsNullOrEmpty(pname))
                {
                    eselist = eselist.Where(e => empmanage.GetPositionByEmpid(e.EmployeeId).Pid == int.Parse(pname)).ToList();
                }
                if (!string.IsNullOrEmpty(Empstate))
                {
                    eselist = eselist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).IsDel == bool.Parse(Empstate)).ToList();
                }

            }
            var newlist = eselist.OrderBy(s => s.Id).Skip((page - 1) * limit).Take(limit).ToList();

            foreach (var item in newlist)
            {
                MySalaryObjView view = new MySalaryObjView();
                view.Id = item.Id;//工资编号
                view.EmployeeId = item.EmployeeId;//员工编号
                view.empName = empmanage.GetEntity(item.EmployeeId).EmpName;//员工姓名
                view.Depart = empmanage.GetDeptByEmpid(item.EmployeeId).DeptName;//所属部门
                view.Position = empmanage.GetPositionByEmpid(item.EmployeeId).PositionName;//所属岗位
                view.EmpState = empmanage.GetEntity(item.EmployeeId).IsDel;
                //拿到该员工工资体系对象
                var eseobj = msrmanage.GetEmpsalaryByEmpid(item.EmployeeId);
                view.baseSalary = eseobj.BaseSalary;//基本工资
                view.positionSalary = eseobj.PositionSalary;//岗位工资
                if (msrmanage.GetMCByEmpid(item.EmployeeId, (DateTime)item.YearAndMonth) == null)
                {
                    view.finalGrade = null;//绩效分
                }
                else
                {
                    view.finalGrade = item.FinalGrade;
                }
                if (view.finalGrade == null)
                {
                    view.PerformanceSalary = null;//绩效工资
                }
                else
                {
                    view.PerformanceSalary = msrmanage.GetempPerformanceSalary(item.FinalGrade, eseobj.PerformancePay);
                }

                view.netbookSubsidy = eseobj.NetbookSubsidy;//笔记本补助
                view.socialSecuritySubsidy = eseobj.SocialSecuritySubsidy;//社保补贴
                #region 应发工资1赋值
                var one = view.baseSalary + view.positionSalary;

                view.SalaryOne = msrmanage.GetSalaryone(one, view.PerformanceSalary, view.netbookSubsidy, view.socialSecuritySubsidy);
                #endregion
                //考勤表对象
                var attendobj = msrmanage.GetAttendanceInfoByEmpid(item.EmployeeId, (DateTime)item.YearAndMonth);
                if (attendobj == null)
                {
                    view.toRegularDays = null;//到勤天数
                    view.leavedays = null;//请假天数
                }
                else
                {
                    view.toRegularDays = attendobj.ToRegularDays;
                    view.leavedays = attendobj.LeaveDays;
                    if (view.leavedays > 0)
                    {
                        view.LeaveDeductions = msrmanage.GetLeaveDeductions(view.Id, one, view.PerformanceSalary, attendobj.DeserveToRegularDays, view.leavedays);//请假扣款
                    }
                    else
                    {
                        view.LeaveDeductions = null;
                    }
                    view.TardyAndLeaveWithhold = attendobj.TardyAndLeaveWithhold;//迟到扣款
                                                                                 //  view.LeaveWithhold = attendobj.LeaveWithhold;//早退扣款
                    var NoClocknum = attendobj.WorkAbsentNum + attendobj.OffDutyAbsentNum;
                    //if (NoClocknum > 3)
                    //{
                    //    view.NoClockWithhold = attendobj.AbsentNumWithhold; /*msrmanage.GetNoClockWithhold(view.Id, one, view.PerformanceSalary, attendobj.DeserveToRegularDays);//缺卡扣款*/
                    //}
                    //else
                    //{
                    //    view.NoClockWithhold = null;
                    //}
                    view.NoClockWithhold = attendobj.AbsentNumWithhold;
                    view.AbsentNumWithhold = attendobj.AbsenteeismWithhold;
                }

                view.OvertimeCharges = item.OvertimeCharges;//加班费用
                view.Bonus = item.Bonus;//奖金

                view.OtherDeductions = item.OtherDeductions;//其他扣款

                #region 应发工资2赋值
                view.SalaryTwo = msrmanage.GetSalarytwo(view.SalaryOne, view.OvertimeCharges, view.Bonus, view.LeaveDeductions, view.TardyAndLeaveWithhold/*, view.LeaveWithhold*/, view.NoClockWithhold, view.OtherDeductions);
                #endregion
                view.PersonalSocialSecurity = eseobj.PersonalSocialSecurity;//个人社保
                view.PersonalIncomeTax = eseobj.PersonalIncomeTax;//个税
                item.Total = msrmanage.GetTotal(view.Id, view.SalaryTwo, view.PersonalSocialSecurity, view.PersonalIncomeTax);
                view.Total = item.Total;//合计
                view.PayCardSalary = msrmanage.GetPaycardSalary(view.Id, view.Total, view.PersonalSocialSecurity, eseobj.ContributionBase);//工资卡工资
                view.CashSalary = msrmanage.GetCashSalary(view.Id, view.Total, view.PayCardSalary);//现金工资
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
                result.Add(view);
            }

            var newobj = new
            {
                code = 0,
                msg = "",
                count = eselist.Count(),
                data = result
            };

            return Json(newobj, JsonRequestBehavior.AllowGet);
        }

        #region 年月份改变于员工月度工资表的变化

        //当切换年月份时，循环所有月度工资表数据的月份是否有和选择的月份相匹配的数据，
        //有的话则进行查询功能，若没有则将所有未禁用的员工添加一次该月份工资，即新月份工资表生成，且月份为选择的月份

        /// <summary>
        /// 年月份改变
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateTime()
        {
            //string names = "";
            //EmployeesInfoManage empmanage = new EmployeesInfoManage();
            //EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            //List<EmployeesInfo> employeeslist = empmanage.GetList().Where(i=>i.IsDel==false).ToList();
            //var emplist = esemanage.GetEmpESEData().Where(s => s.IsDel == false).OrderBy(i => i.Id).ToList();
            //foreach (var item in employeeslist)
            //{
            //    var s = emplist.Where(i => i.EmployeeId == item.EmployeeId).FirstOrDefault();
            //    if (s == null)
            //    {
            //        names += item.EmpName+";";
            //    }
            //}
            ViewBag.time = FirstTime;
            return View();
        }
        /// <summary>
        /// 判断某月份员工工资是否已确认审批
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsConfirmApproval(string time)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();//员工月度工资
            var AjaxResultxx = new AjaxResult();
            try
            {
                var mtime = DateTime.Parse(time);
                var msrlist = msrmanage.GetEmpMsrData().Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == mtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == mtime.Month).ToList();
                if (msrlist.FirstOrDefault().IsApproval == true)
                {
                    AjaxResultxx.Data = "该月份员工工资已确认审批！";
                    AjaxResultxx.Success = false;
                }
                else
                {
                    AjaxResultxx.Success = true;
                }
                AjaxResultxx.ErrorCode = 200;
            }
            catch (Exception ex)
            {
                AjaxResultxx.ErrorCode = 500;
                AjaxResultxx = msrmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateTime(string CurrentTime)
        {
            var AjaxResultxx = new AjaxResult();
            var newobj = new object();
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();//员工月度工资
            var msrlist = msrmanage.GetEmpMsrData().Where(s => s.IsDel == false).ToList();
            var nowtime = DateTime.Parse(CurrentTime);
            //匹配是否有该月（选择的年月即传过来的参数）的月度工资数据
            var matchlist = msrlist.Where(m => DateTime.Parse(m.YearAndMonth.ToString()).Year == nowtime.Year && DateTime.Parse(m.YearAndMonth.ToString()).Month == nowtime.Month).ToList();
            AjaxResultxx.Data = matchlist.Count();
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 年月份改变后工资表刷新
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SalarytableRefresh(string time)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();

            AjaxResult result = new AjaxResult();
            result = msrmanage.CreateSalTab(time);

            FirstTime = time;

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        #endregion

        /// <summary>
        /// 工资表中员工禁用
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSalaryManageEmp(string list)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();//员工月度工资
            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            AttendanceInfoManage admanage = new AttendanceInfoManage();
            MeritsCheckManage mcmanage = new MeritsCheckManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                string[] ids = list.Split(',');
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    int id = Convert.ToInt32(ids[i]);
                    AjaxResultxx.Success = msrmanage.EditEmpMS(id);
                    var ad = msrmanage.GetEntity(id);
                    if (AjaxResultxx.Success)
                    {
                        bool e = esemanage.EditEmpSalaryState(ad.EmployeeId);//员工体系表禁用该员工
                        AjaxResultxx.Success = e;
                    }
                    if (AjaxResultxx.Success)
                    {
                        bool a = admanage.EditEmpStateToAds(ad.EmployeeId, ad.YearAndMonth.ToString());//员工考勤表禁用该员工
                        AjaxResultxx.Success = a;
                    }
                    if (AjaxResultxx.Success)
                    {
                        bool e = mcmanage.EditEmpStateToMC(ad.EmployeeId, ad.YearAndMonth.ToString());//员工绩效表禁用该员工
                        AjaxResultxx.Success = e;
                    }
                }
            }
            catch (Exception ex)
            {
                AjaxResultxx = msrmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 确认审批
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult JudgeIsApproval(int id)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                var msr = msrmanage.GetEntity(id);
                if (msr.IsApproval == true)
                {
                    AjaxResultxx.Success = true;

                }
                else
                {
                    AjaxResultxx.Success = false;
                }
                AjaxResultxx.ErrorCode = 200;
            }
            catch (Exception ex)
            {
                AjaxResultxx.ErrorCode = 500;
                AjaxResultxx = msrmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditEmpSalary(int id)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();
            var msr = msrmanage.GetEntity(id);
            //ViewBag.IsFinancialAudit = msr.IsFinancialAudit;
            ViewBag.id = id;
            return View(msr);
        }
        public ActionResult GetMSRById(int id)
        {
            MonthlySalaryRecordManage esemanage = new MonthlySalaryRecordManage();
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var ese = esemanage.GetEntity(id);
            var newobj = new
            {
                ese.Id,
                ese.EmployeeId,
                empName = empmanage.GetEntity(ese.EmployeeId).EmpName,
                deptName = empmanage.GetDeptByEmpid(ese.EmployeeId).DeptName,
                pName = empmanage.GetPositionByEmpid(ese.EmployeeId).PositionName,
                ese.Bonus,
                ese.OvertimeCharges,
                ese.OtherDeductions,
                ese.IsDel,
                ese.IsApproval
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查看选择编辑的数据是否已审核发放
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetIsApprovalState(int id)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                var isapproval = msrmanage.GetEntity(id).IsApproval;
                AjaxResultxx.Data = isapproval;
            }
            catch (Exception ex)
            {

                AjaxResultxx = msrmanage.Error(ex.Message);
            }

            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditEmpSalary(MonthlySalaryRecord msr)
        {
            var AjaxResultxx = new AjaxResult();
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();
            try
            {
                var mobj = msrmanage.GetEntity(msr.Id);
                mobj.OvertimeCharges = msr.OvertimeCharges;
                mobj.Bonus = msr.Bonus;
                mobj.OtherDeductions = msr.OtherDeductions;
                msrmanage.Update(mobj);
                rc.RemoveCache("InRedisMSRData");
                AjaxResultxx = msrmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = msrmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 确认审批（确认审批过的数据不可再编辑）
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmApproval(string time)
        {
            var AjaxResultxx = new AjaxResult();
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();
            try
            {
                var curtime = DateTime.Parse(time);
                var curlist = msrmanage.GetList().Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == curtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == curtime.Month).ToList();
                foreach (var item in curlist)
                {
                    item.IsApproval = true;
                    msrmanage.Update(item);
                    rc.RemoveCache("InRedisMSRData");
                }
                AjaxResultxx = msrmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = msrmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public decimal GetPerformancePay(DateTime year_month, string dname, string pname) {
            var resultsalary = 0;
            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            var month = year_month.Month;
            if (month % 3 == 0)
            {
                if (dname == "校办")
                {
                    if (pname == "校长")
                    {//指杨校和黄主任
                        resultsalary = 24000;//他俩的月度绩效额度为3000
                    }
                    else if (pname == "教学副校长")
                    {
                        resultsalary = 20000;
                    }
                    else if (pname == "新校区副校长")
                    {
                        resultsalary = 15000;
                    }
                }

            }
            else {
                esemanage.GetPerformancePay(dname, pname);
            }
            return resultsalary;
        }
        [HttpPost]
        public ActionResult PaySlipExcel(string time)
        {
            AjaxResult result = new AjaxResult();
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
            List<PaySlipExcelError> error = new List<PaySlipExcelError>();
            try
            {
                List<MonthlySalaryRecord> salary = monthly.GetEmpMsrData().Where(i => i.IsDel == false && Convert.ToDateTime(i.YearAndMonth.ToString().Substring(0, 7)) == Convert.ToDateTime(time.Substring(0, 7))&&i.SendingStatus==false).ToList();
                EmployeesInfoManage manage = new EmployeesInfoManage();

               
                //发件人邮箱
                string FromMail = "feihongos@163.com";
                //发件人邮箱授权码
                string AuthorizationCode = "QRSNTQRISGFLTXXS";
                int num = 0;
                foreach (var i in salary)
                {
                    PaySlipExcelError paySlip = new PaySlipExcelError();
                    if (!(bool)i.IsApproval)
                    {
                        paySlip.empname = manage.GetInfoByEmpID(i.EmployeeId).EmpName;
                        paySlip.errorExplain = "工资未审核";
                        error.Add(paySlip);
                    }
                    else
                    {
                        result = monthly.WagesDataToEmail(FromMail, "2651396164@qq.com", AuthorizationCode, i);
                        if (!result.Success)
                        {
                           
                            paySlip.empname = manage.GetInfoByEmpID(i.EmployeeId).EmpName;
                            paySlip.errorExplain = result.Msg;
                            error.Add(paySlip);
                        }
                        else
                        {
                            i.SendingStatus = true;
                            monthly.Update(i);
                            num++;
                        }
                    }

                }
                if (error.Count()!=0)
                {
                    result.Success = true;
                    result.Msg = (salary.Count() - error.Count()).ToString();
                    result.Data = error;
                    result.ErrorCode = 200;
                }
                else if(error.Count==0)
                {
                    result.Msg = salary.ToString();
                    result.ErrorCode = 100;
                    result.Success = true;
                    result.Data = error;
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
                result.Data = error;
            }
           
            return Json(result, JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult ModifyAbnormalData(int id)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();//员工月度工资
            EmployeesInfoManage manage = new EmployeesInfoManage();
            var s = msrmanage.GetEntity(id);
            ViewBag.EmpName = manage.GetEntity(s.EmployeeId).EmpName;
            return View(s);
        }
        [HttpPost]
        public ActionResult ModifyAbnormalData(MonthlySalaryRecord m)
        {
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
            EmplSalaryEmbodyManage  empl = new EmplSalaryEmbodyManage();

            AjaxResult result = new AjaxResult();

            try
            {
                var monlies = monthly.GetEntity(m.Id);
                var emplies = empl.GetEseByEmpid(monlies.EmployeeId);
                emplies.BaseSalary = m.BaseSalary;
                emplies.PositionSalary = m.PositionSalary;
                emplies.SocialSecuritySubsidy = m.SocialSecuritySubsidy;
                emplies.PersonalSocialSecurity = m.PersonalSocialSecurity;
                emplies.NetbookSubsidy = m.NetbookSubsidy;
                emplies.PersonalIncomeTax = m.PersonalIncomeTax;
                empl.Update(emplies);
                rc.RemoveCache("InRedisESEData");

                //monlies.IsFinancialAudit = 0;
                monlies.BaseSalary = m.BaseSalary;
                monlies.PositionSalary = m.PositionSalary;
                monlies.FinalGrade = m.FinalGrade;
                monlies.NetbookSubsidy = m.NetbookSubsidy;
                monlies.SocialSecuritySubsidy = m.SocialSecuritySubsidy;
                monlies.PersonalSocialSecurity = m.PersonalSocialSecurity;
                monlies.OvertimeCharges = m.OvertimeCharges;
                monlies.Bonus = m.Bonus;
                monlies.AbsentNumWithhold = m.AbsentNumWithhold;
                monlies.AbsenteeismWithhold = m.AbsenteeismWithhold;
                monlies.OtherDeductions = m.OtherDeductions;
                monlies.PersonalIncomeTax = m.PersonalIncomeTax;
                monlies.MonthPerformancePay = m.MonthPerformancePay;
                monlies.Total = m.Total;

                monthly.Update(monlies);
                rc.RemoveCache("InRedisMSRData");
                result = monthly.Success();
                result.ErrorCode = 200;
            }
            catch (Exception ex)
            {
                result = monthly.Error(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult MonthlySalaryExport(string time)
        //{
        //    MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
        //    AjaxResult result = new AjaxResult();
        //  var data=  monthly.GetEmpMsrData().Where(i=>Convert.ToDateTime(i.YearAndMonth).Year==Convert.ToDateTime(time).Year&& Convert.ToDateTime(i.YearAndMonth).Month == Convert.ToDateTime(time).Month).ToList();

        //    result = monthly.Month(data);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public FileStreamResult MonthlySalaryExport(string time)
        {

            var ajaxresult = new AjaxResult();
            MemoryStream bookStream = new MemoryStream();
            var workbook = new HSSFWorkbook();
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
            EmployeesInfoManage manage = new EmployeesInfoManage();
            AttendanceInfoManage attendance = new AttendanceInfoManage();
            var data=  monthly.GetEmpMsrData().Where(i=>Convert.ToDateTime(i.YearAndMonth).Year==Convert.ToDateTime(time).Year&& Convert.ToDateTime(i.YearAndMonth).Month == Convert.ToDateTime(time).Month).ToList();
            //创建工作区
            var sheet = workbook.CreateSheet();
            string Detailfilename = Convert.ToDateTime(time).ToString("yyyy年MM月") + "员工工资" + ".xls"; ;
            #region 表头样式

            HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

            HeadercellStyle.Alignment = HorizontalAlignment.Center;
            HeadercellStyle.VerticalAlignment = VerticalAlignment.Center;
            HeadercellFont.IsBold = true;

            HeadercellStyle.SetFont(HeadercellFont);

            #endregion


            HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

            ContentcellStyle.Alignment = HorizontalAlignment.Center;

            CreateHeader();

            int num = 2;
            var YearAndMonth = "";
            data.ForEach(d =>
            {
                var row = (HSSFRow)sheet.CreateRow(num);
                YearAndMonth = d.YearAndMonth.ToString();

                var Salaryone = monthly.GetSalaryone(d.BaseSalary + d.PositionSalary, d.MonthPerformancePay, d.NetbookSubsidy, d.SocialSecuritySubsidy);
                var Salarytwo = monthly.GetSalarytwo(Salaryone, d.OvertimeCharges, d.Bonus, d.LeaveDeductions, d.TardyAndLeaveWithhold, d.AbsentNumWithhold, d.OtherDeductions);
                var PaycardSalary = monthly.GetPaycardSalary(d.Id, d.Total, d.PersonalSocialSecurity, d.ContributionBase);

                CreateCell(row, ContentcellStyle, 0, d.EmployeeId);//员工编号
                CreateCell(row, ContentcellStyle, 1, manage.GetEntity(d.EmployeeId).EmpName);//员工姓名
                CreateCell(row, ContentcellStyle, 2, manage.GetDeptByEmpid(d.EmployeeId).DeptName);//所属部门
                CreateCell(row, ContentcellStyle, 3, manage.GetPositionByEmpid(d.EmployeeId).PositionName);//所属岗位
                CreateCell(row, ContentcellStyle, 4, d.BaseSalary.ToString());//基本工资
                CreateCell(row, ContentcellStyle, 5, d.PositionSalary.ToString());//岗位工资
                CreateCell(row, ContentcellStyle, 6, d.FinalGrade.ToString());//绩效分
                CreateCell(row, ContentcellStyle, 7, d.MonthPerformancePay.ToString());//绩效工资
                CreateCell(row, ContentcellStyle, 8, d.NetbookSubsidy.ToString());//笔记本补助
                CreateCell(row, ContentcellStyle, 9, d.SocialSecuritySubsidy.ToString());//社保补贴
                CreateCell(row, ContentcellStyle, 10, Salaryone.ToString());//应发工资1
                CreateCell(row, ContentcellStyle, 11, d.OvertimeCharges.ToString());//加班费用
                CreateCell(row, ContentcellStyle, 12, d.Bonus.ToString());//奖金(元)
                CreateCell(row, ContentcellStyle, 13, monthly.GetAttendanceInfoByEmpid(d.EmployeeId, (DateTime)d.YearAndMonth).LeaveDays.ToString());//请假天数
                CreateCell(row, ContentcellStyle, 14, d.LeaveDeductions.ToString());//请假扣款(元)
                CreateCell(row, ContentcellStyle, 15, d.TardyAndLeaveWithhold.ToString());//迟到/早退扣款(元)
                CreateCell(row, ContentcellStyle, 16, d.AbsentNumWithhold.ToString());//缺卡扣款(元)
                CreateCell(row, ContentcellStyle, 17, d.AbsenteeismWithhold.ToString());//旷工扣款(元)
                CreateCell(row, ContentcellStyle, 18, d.OtherDeductions.ToString());//其他扣款(元)
                CreateCell(row, ContentcellStyle, 19, Salarytwo.ToString());//应发工资2
                CreateCell(row, ContentcellStyle, 20, d.PersonalSocialSecurity.ToString());//个人社保
                CreateCell(row, ContentcellStyle, 21, d.PersonalIncomeTax.ToString());//个税
                CreateCell(row, ContentcellStyle, 22, PaycardSalary.ToString());//实发工资(工资卡)
                CreateCell(row, ContentcellStyle, 23, monthly.GetCashSalary(d.Id, d.Total, PaycardSalary).ToString());//实发工资(现金)
                num++;

            });

           
            try
            {
                workbook.Write(bookStream);
                bookStream.Seek(0, SeekOrigin.Begin);

            }
            catch (Exception ex)
            {
                ajaxresult.ErrorCode = 100;
                ajaxresult.Msg = "导出失败，" + ex.Message;

            }
            return File(bookStream, "application / vnd.ms - excel", Detailfilename);
            void CreateHeader()
            {
                HSSFRow Header = (HSSFRow)sheet.CreateRow(0);

                for (int i = 0; i < 24; i++)
                {
                    if (i < 15 || i > 17)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(0, 1, i, i));
                    }
                }
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 15, 17));

                CreateCell(Header, HeadercellStyle, 0, "员工编号");
                CreateCell(Header, HeadercellStyle, 1, "员工姓名");
                CreateCell(Header, HeadercellStyle, 2, "所属部门");
                CreateCell(Header, HeadercellStyle, 3, "所属岗位");
                CreateCell(Header, HeadercellStyle, 8, "出勤天数");
                CreateCell(Header, HeadercellStyle, 4, "基本工资");
                CreateCell(Header, HeadercellStyle, 5, "岗位工资");
                CreateCell(Header, HeadercellStyle, 6, "绩效分");
                CreateCell(Header, HeadercellStyle, 7, "绩效工资");
                CreateCell(Header, HeadercellStyle, 8, "笔记本补助");
                CreateCell(Header, HeadercellStyle, 9, "社保补贴");
                CreateCell(Header, HeadercellStyle, 10, "应发工资1");
                CreateCell(Header, HeadercellStyle, 11, "加班费用");
                CreateCell(Header, HeadercellStyle, 12, "奖金(元)");
                CreateCell(Header, HeadercellStyle, 13, "请假天数");
                CreateCell(Header, HeadercellStyle, 14, "请假扣款(元)");
                CreateCell(Header, HeadercellStyle, 15, "考勤扣款");
                CreateCell(Header, HeadercellStyle, 18, "其他扣款(元)");
                CreateCell(Header, HeadercellStyle, 19, "应发工资2");
                CreateCell(Header, HeadercellStyle, 20, "个人社保");
                CreateCell(Header, HeadercellStyle, 21, "个税");
                CreateCell(Header, HeadercellStyle, 22, "实发工资(工资卡)");
                CreateCell(Header, HeadercellStyle, 23, "实发工资(现金)");
                HSSFRow Header2 = (HSSFRow)sheet.CreateRow(1);
                CreateCell(Header2, HeadercellStyle, 15, "迟到/早退扣款(元)");
                CreateCell(Header2, HeadercellStyle, 16, "缺卡扣款(元)");
                CreateCell(Header2, HeadercellStyle, 17, "旷工扣款(元)");

            }

            void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
            {
                HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                Header_Name.SetCellValue(value);

                Header_Name.CellStyle = TcellStyle;
            }
        }
        [HttpPost]
        public ActionResult JudgmentOfSalaryDetails()
        {
            AjaxResult result = new AjaxResult();
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
            try
            {
                string time = Convert.ToDateTime(FirstTime).ToString("yyyy年MM月");
                List<MonthlySalaryRecord> salary = monthly.GetEmpMsrData().Where(i => i.IsDel == false && Convert.ToDateTime(i.YearAndMonth.ToString().Substring(0, 7)) == Convert.ToDateTime(FirstTime.Substring(0, 7))).ToList();
                 int count = salary.Where(i=>i.SendingStatus==false).Count();
                if (count == 0)
                {
                    result.Data = "该月份员工工资详情已发送！";
                    result.Success = false;
                }else if (salary.Count() == count)
                {
                    result.Data = "是否要向所有员工发送"+ time+"的工资详情";
                    result.Success = true;
                }
                else
                {
                    result.Data = "还有"+count+"位员工未发送工资详情是否为这"+count+"位员工发送"+time+"的工资详情";
                    result.Success = true;
                }
                result.ErrorCode = 200;
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result = monthly.Error(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult MonthlySalaryExport()
        //{
        //    return View();
        //}

    } 
}