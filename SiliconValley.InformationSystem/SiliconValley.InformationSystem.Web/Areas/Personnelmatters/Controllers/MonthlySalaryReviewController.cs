using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    public class MonthlySalaryReviewController : Controller
    {
        // GET: Personnelmatters/MonthlySalaryReview
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
        public ActionResult FinancialAuditSalary()
        {
            ViewBag.yearandmonth = FirstTime;
            return View();
        }
        public ActionResult FinancialEmpSalaryData(int page, int limit, string AppCondition, string ymtime)
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
                eselist = eselist.Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == time.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == time.Month && s.IsFinancialAudit == 0).ToList();
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
        public ActionResult IsConfirmApproval(string time)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();//员工月度工资
            var AjaxResultxx = new AjaxResult();
            try
            {
                var curtime = DateTime.Parse(time);
                var monthlies = msrmanage.GetEmpMsrData().Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == curtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == curtime.Month&&s.IsFinancialAudit==0).ToList();
                foreach (var item in monthlies)
                {
                    item.IsFinancialAudit = 1;
                    msrmanage.Update(item);
                    rc.RemoveCache("InRedisMSRData");
                }
                AjaxResultxx = msrmanage.Success();
                AjaxResultxx.ErrorCode = 200;
                
            }
            catch (Exception ex)
            {
                AjaxResultxx = msrmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SalaryApproval(int id)
        {
            MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();//员工月度工资
            EmployeesInfoManage manage = new EmployeesInfoManage();
            var s = msrmanage.GetEntity(id);
            ViewBag.EmpName = manage.GetEntity(s.EmployeeId).EmpName;
            ViewBag.yearandmonth = FirstTime;
            return View(s);
        }
        public ActionResult SalaryApproved(int id,string remark)
        {
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();

            AjaxResult result = new AjaxResult();

            try
            {
                var monlies = monthly.GetEntity(id);
                monlies.IsFinancialAudit = 1;
                monlies.FinancialRemarks = remark;

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
        public ActionResult SalaryReviewRejected(int id, string remark)
        {
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();

            AjaxResult result = new AjaxResult();

            try
            {
                var monlies = monthly.GetEntity(id);
                monlies.IsFinancialAudit = 2;
                monlies.FinancialRemarks = remark;
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
        public ActionResult OneClickRejection(string id,string remark)
        {
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();

            AjaxResult result = new AjaxResult();
            try
            {
                var monthlies = monthly.GetListBySql<MonthlySalaryRecord>("select *from MonthlySalaryRecord  where Id in("+id+")");
                foreach (var item in monthlies)
                {
                    item.IsFinancialAudit = 2;
                    item.FinancialRemarks = remark;
                    monthly.Update(item);
                    rc.RemoveCache("InRedisMSRData");
                }
                result = monthly.Success();
                result.ErrorCode = 200;

            }
            catch (Exception ex)
            {
                result = monthly.Error(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}