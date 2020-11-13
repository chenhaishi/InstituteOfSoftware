using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
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
                    view.NoClockWithhold = attendobj.NoonAbsentNum;
                    view.AbsentNumWithhold = attendobj.AbsentNumWithhold;
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
                view.IsFinancialAudit = item.IsFinancialAudit;
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
            bool result = true;
            if (msrmanage.CreateSalTab(time))
            {
                result = true;
            }
            else
            {
                result = false;
            }
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
            ViewBag.IsFinancialAudit = msr.IsFinancialAudit;
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
                var curlist = msrmanage.GetEmpMsrData().Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == curtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == curtime.Month).ToList();
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
        public ActionResult PaySlipExcel(string time)
        {
            AjaxResult result = new AjaxResult();
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
            List<MonthlySalaryRecord> salary = monthly.GetEmpMsrData().Where(i => i.IsDel == false && i.IsApproval == true && Convert.ToDateTime(i.YearAndMonth.ToString().Substring(0, 7)) ==Convert.ToDateTime(time)).ToList();
            EmployeesInfoManage manage = new EmployeesInfoManage();

            //发件人邮箱
            string FromMail = "2651396164@qq.com";
            //发件人邮箱授权码
            string AuthorizationCode = "xuobhcbwwnewecde";

            foreach (var i in salary)
            {

                result = monthly.WagesDataToEmail(FromMail,"", AuthorizationCode, i);
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

                monlies.IsFinancialAudit = 0;
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
        [HttpPost]
        public ActionResult MonthlySalaryExport(string time)
        {
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
            AjaxResult result = new AjaxResult();
          var data=  monthly.GetEmpMsrData().Where(i=>Convert.ToDateTime(i.YearAndMonth).Year==Convert.ToDateTime(time).Year&& Convert.ToDateTime(i.YearAndMonth).Month == Convert.ToDateTime(time).Month).ToList();

            result = monthly.Month(data);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MonthlySalaryExport()
        {
            return View();
        }
    } 
}