using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Util;
using System.Data;
using System.Text;
using System.IO;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.SchoolAttendanceManagementBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    public class AttendanceStatisticsController : Controller
    {
        AttendanceInfoManage atdmanage = new AttendanceInfoManage();
        EmployeesInfoManage empmanage = new EmployeesInfoManage();
        RedisCache rc = new RedisCache();

        //第一次进入月度工资表页面时加载的年月份的方法
        static string GetFirstTime()
        {
            AttendanceInfoManage atdmanage = new AttendanceInfoManage();//员工月度工资
            string mytime = "";
            if (atdmanage.GetADInfoData().Where(s => s.IsDel == false).Count() > 0)
            {
                var time = atdmanage.GetADInfoData().Where(s => s.IsDel == false).LastOrDefault().YearAndMonth;
                mytime = DateTime.Parse(time.ToString()).Year + "-" + DateTime.Parse(time.ToString()).Month;
            }
            else {
                mytime = "";
            }
            return mytime;
        }

        static string FirstTime = GetFirstTime(); 
    
        //考勤统计
        // GET: Personnelmatters/AttendanceStatistics
        public ActionResult AttendanceStatisticsIndex()
        {
            ViewBag.yearandmonth = FirstTime;
            return View();
        }
        //获取考勤数据
        public ActionResult GetCheckingInData(int page, int limit, string AppCondition)
        {
          
          string   ymtime = FirstTime;
            var attlist = atdmanage.GetADInfoData().Where(s => s.IsDel == false).ToList();
            if (!string.IsNullOrEmpty( ymtime)) {
            var time = DateTime.Parse(ymtime);
             attlist = attlist.Where(s =>DateTime.Parse(s.YearAndMonth.ToString()).Year==time.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == time.Month).ToList();
            }
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string deptname = str[1];
                string pname = str[2];
                string empstate = str[3];
               
                attlist = attlist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(deptname))
                {
                    attlist = attlist.Where(e => empmanage.GetDeptByEmpid(e.EmployeeId).DeptId == int.Parse(deptname)).ToList();
                }
                if (!string.IsNullOrEmpty(pname))
                {
                    attlist = attlist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).PositionId == int.Parse(pname)).ToList();
                }
                if (!string.IsNullOrEmpty(empstate))
                {
                    attlist = attlist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).IsDel == bool.Parse(empstate)).ToList();
                }

            }
            var newlist = attlist.OrderBy(s => s.AttendanceId).Skip((page - 1) * limit).Take(limit).ToList();
            var mylist = from e in newlist
                         select new
                         {
                             #region 获取值
                             e.AttendanceId,
                             e.EmployeeId,
                             empName = empmanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             empDept = empmanage.GetDeptByEmpid(e.EmployeeId).DeptName,
                             empPosition = empmanage.GetPositionByEmpid(e.EmployeeId).PositionName,
                             empIsDel = empmanage.GetInfoByEmpID(e.EmployeeId).IsDel,
                             e.YearAndMonth,
                             e.DeserveToRegularDays,
                             e.ToRegularDays,
                             e.LeaveDays,
                             e.LeaveRecord,
                             e.WorkAbsentNum,
                             e.WorkAbsentRecord,
                             e.OffDutyAbsentNum,
                             e.OffDutyAbsentRecord,
                             NoClockTotalNum = e.WorkAbsentNum + e.OffDutyAbsentNum+e.NoonAbsentNum,
                             e.TardyNum,
                             e.TardyRecord,
                             e.LeaveEarlyNum,
                             e.LeaveEarlyRecord,
                             e.Remark,
                             e.IsDel,
                             e.IsApproval,
                             e.TardyAndLeaveWithhold,
                             e.AbsentNumWithhold,
                            // e.LeaveWithhold,
                            // e.OvertTimeDuration,
                            // e.OvertTimeRecord,
                             e.OvertimeCharges,
                             e.DaysoffDuration,
                             e.DaysoffRecord,
                             e.AbsenteeismDays,
                             e.AbsenteeismRecord,
                             e.AbsenteeismWithhold,
                             e.NonPersonalLeaveNum,
                             e.NonPersonalLeaveRecord
                             #endregion
                         };
                 
            var newobj = new
            {
                code = 0,
                msg = "",
                count = attlist.Count(),
                data = mylist
            }; 
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 月度全勤统计
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="AppCondition"></param>
        /// <returns></returns>
        public ActionResult GetFullCheckingInData(int page, int limit, string AppCondition)
        {
            string ymtime = FirstTime;
            var attlist = atdmanage.GetListBySql<AttendanceInfo>(@"select *from  AttendanceInfo where DeserveToRegularDays=ToRegularDays and (TardyNum is null or TardyNum = 0)  and
                (LeaveEarlyNum is null or LeaveEarlyNum = 0)  and(AbsenteeismDays is null or AbsenteeismDays = 0)and
                (WorkAbsentNum is null or WorkAbsentNum = 0) and (OffDutyAbsentNum is null or OffDutyAbsentNum=0) and (NoonAbsentNum is null or NoonAbsentNum=0)");
             attlist = attlist.Where(s => s.IsDel == false).ToList();
            if (!string.IsNullOrEmpty(ymtime))
            {
                var time = DateTime.Parse(ymtime);
                attlist = attlist.Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == time.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == time.Month).ToList();
            }
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string deptname = str[1];
                string pname = str[2];
                string empstate = str[3];
                attlist = attlist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(deptname))
                {
                    attlist = attlist.Where(e => empmanage.GetDeptByEmpid(e.EmployeeId).DeptId == int.Parse(deptname)).ToList();
                }
                if (!string.IsNullOrEmpty(pname))
                {
                    attlist = attlist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).PositionId == int.Parse(pname)).ToList();
                }
                if (!string.IsNullOrEmpty(empstate))
                {
                    attlist = attlist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).IsDel == bool.Parse(empstate)).ToList();
                }

            }
            var newlist = attlist.OrderBy(s => s.AttendanceId).Skip((page - 1) * limit).Take(limit).ToList();
            var mylist = from e in newlist
                         select new
                         {
                             #region 获取值
                             e.AttendanceId,
                             e.EmployeeId,
                             empName = empmanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             empDept = empmanage.GetDeptByEmpid(e.EmployeeId).DeptName,
                             empPosition = empmanage.GetPositionByEmpid(e.EmployeeId).PositionName,
                             empIsDel = empmanage.GetInfoByEmpID(e.EmployeeId).IsDel,
                             e.YearAndMonth,
                             e.DeserveToRegularDays,
                             e.ToRegularDays,
                             e.LeaveDays,
                             e.LeaveRecord,
                             e.WorkAbsentNum,
                             e.WorkAbsentRecord,
                             e.OffDutyAbsentNum,
                             e.OffDutyAbsentRecord,
                             NoClockTotalNum = e.WorkAbsentNum + e.OffDutyAbsentNum + e.NoonAbsentNum,
                             e.TardyNum,
                             e.TardyRecord,
                             e.LeaveEarlyNum,
                             e.LeaveEarlyRecord,
                             e.Remark,
                             e.IsDel,
                             e.IsApproval,
                             e.TardyAndLeaveWithhold,
                             e.OvertimeCharges,
                             e.DaysoffDuration,
                             e.DaysoffRecord,
                             e.AbsenteeismDays,
                             e.AbsenteeismRecord,
                             e.AbsenteeismWithhold
                             #endregion
                         };

            var newobj = new
            {
                code = 0,
                msg = "",
                count = attlist.Count(),
                data = mylist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 年月份及应到勤天数的改变（查看记录）
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeTimeandDays()
        {
            if (atdmanage.GetADInfoData().Where(s => s.IsDel == false).Count() > 0)
            {
                ViewBag.time = FirstTime; 
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangeTimeandDays(string CurrentTime)
        {
            var AjaxResultxx = new AjaxResult();
            try
            {
                var stime = DateTime.Parse(CurrentTime);
                var attlist = atdmanage.GetADInfoData().Where(s => s.IsDel == false && DateTime.Parse(s.YearAndMonth.ToString()).Year == stime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == stime.Month).ToList();
                if (attlist.Count > 0)
                {
                    FirstTime = CurrentTime;
                }
                AjaxResultxx.Success = true;
                AjaxResultxx.Data = attlist.Count();
            }
            catch (Exception ex)
            {
                AjaxResultxx.Success = false;
                AjaxResultxx = atdmanage.Error(ex.Message);
            }


            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 考勤数据编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditAttendanceInfo(int id)
        {
            var att = atdmanage.GetEntity(id);
            return View(att);
        }
        public ActionResult GetAttById(int id)
        {
            var att = atdmanage.GetEntity(id);
            var newobj = new
            {
                #region 考勤表赋值
                att.AttendanceId,
                att.EmployeeId,
                empName = empmanage.GetInfoByEmpID(att.EmployeeId).EmpName,
                esex = empmanage.GetInfoByEmpID(att.EmployeeId).Sex,
                dname = empmanage.GetDeptByEmpid(att.EmployeeId).DeptName,
                pname = empmanage.GetPositionByEmpid(att.EmployeeId).PositionName,
                att.ToRegularDays,
                att.LeaveDays,
                att.WorkAbsentNum,
                att.WorkAbsentRecord,
                att.OffDutyAbsentNum,
                att.OffDutyAbsentRecord,
                att.TardyNum,
                att.TardyRecord,
                att.LeaveEarlyNum,
                att.LeaveEarlyRecord,
                att.Remark,
                att.YearAndMonth,
                att.DeserveToRegularDays,
                att.IsDel,
                att.IsApproval
                #endregion
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditAttendanceInfo(AttendanceInfo att)
        {
            var AjaxResultxx = new AjaxResult();
            try
            {
                var a = atdmanage.GetEntity(att.AttendanceId);
                att.YearAndMonth = a.YearAndMonth;
                att.DeserveToRegularDays = a.DeserveToRegularDays;
                att.IsDel = a.IsDel;
                att.IsApproval = a.IsApproval;
                att.EmployeeId = a.EmployeeId;
                atdmanage.Update(att);
                rc.RemoveCache("InRedisATDData");
                AjaxResultxx = atdmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = atdmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        #region 批量导入导出
      
        public ActionResult BatchImport()
        {
            return View();
        }
        /// <summary>
        /// 批量录入(excel导入)考勤数据
        /// </summary>
        /// <param name="excelfile"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult BatchImport(HttpPostedFileBase excelfile)
        {
            Stream filestream = excelfile.InputStream;
            AttendanceInfoManage atdmanage = new AttendanceInfoManage();
            var result =  atdmanage.ImportDataFormExcel(filestream, excelfile.ContentType);
            if (result.Success) {
                if (atdmanage.GetAtdBySql().Count()>0) {
                    DateTime year_month = (DateTime)atdmanage.GetList().FirstOrDefault().YearAndMonth;
                    var mytime = DateTime.Parse(year_month.ToString()).Year + "-" + DateTime.Parse(year_month.ToString()).Month;
                    FirstTime = mytime;
                }            
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }    
        /// <summary>
        /// 考勤模板下载 
        /// </summary>
        /// <returns></returns>        
        public FileStreamResult DownFile()
        {
            string rr = Server.MapPath("/uploadXLSXfile/Template/AttendenceTemplate.xls");  //获取下载文件的路径         
            FileStream stream = new FileStream(rr, FileMode.Open);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("ExcleTemplate.xlsx"));
        }

        /// <summary>
        /// 加班记录导入
        /// </summary>
        /// <returns></returns>
        public ActionResult OvertimeRecordImport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OvertimeRecordImport(HttpPostedFileBase excelfile) {
            Stream filestream = excelfile.InputStream;
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            var result = otrmanage.ImportDataFormExcel(filestream, excelfile.ContentType);
        
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 加班记录模板下载
        /// </summary>
        /// <returns></returns>
        public FileStreamResult OvertimeDownFile()
        {
            string rr = Server.MapPath("/uploadXLSXfile/Template/OvertimeTemplate.xls");  //获取下载文件的路径         
            FileStream stream = new FileStream(rr, FileMode.Open);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", Server.UrlEncode("OvertimeTemplate.xlsx"));
        }

        #endregion

        /// <summary>
        /// 判断某月份员工工资是否已确认审批
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsConfirmApproval(string time)
        {
            AttendanceInfoManage atdmanage = new AttendanceInfoManage();//员工月度工资
            var AjaxResultxx = new AjaxResult();
            try
            {
                var mtime = DateTime.Parse(time);
                var msrlist = atdmanage.GetADInfoData().Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == mtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == mtime.Month).ToList();
                if (msrlist.FirstOrDefault().IsApproval == true)
                {
                    AjaxResultxx.Data = "该月份员工考勤数据已确认审批！";
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
                AjaxResultxx = atdmanage.Error(ex.Message);
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
            try
            {
                var curtime = DateTime.Parse(time);
                var curlist = atdmanage.GetADInfoData().Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == curtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == curtime.Month).ToList();
                foreach (var item in curlist)
                {
                    item.IsApproval = true;
                    atdmanage.Update(item);
                    rc.RemoveCache("InRedisATDData");
                }
                AjaxResultxx = atdmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = atdmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 判断是否审批
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult JudgeIsApproval(int id)
        {
            AttendanceInfoManage atdmanage = new AttendanceInfoManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                var msr = atdmanage.GetEntity(id);
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
                AjaxResultxx = atdmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OTRDetail(int id) {
            ViewBag.atdid = id;
            return View();
        }

       /// <summary>
       /// 加班详情
       /// </summary>
       /// <param name="id"></param>
       /// <param name="page"></param>
       /// <param name="limit"></param>
       /// <returns></returns>
        public ActionResult GetOvertimeData(int id,int page, int limit) {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
            var otrlist = otrmanage.GetOTRDataByAtdid(id).Where(i=>i.IsPass==false);
            //List<OvertimeRecordView> mylist = new List<OvertimeRecordView>();
            //foreach (var e in otrlist)
            //{
            //    OvertimeRecordView item = new OvertimeRecordView();
            //    item.Id = e.Id;
            //    item.EmployeeId = e.EmployeeId;
            //    item.DDAppId = empmanage.GetInfoByEmpID(e.EmployeeId).DDAppId;
            //    item.empName = empmanage.GetInfoByEmpID(e.EmployeeId).EmpName;
            //    item.empDept = empmanage.GetDeptByEmpid(e.EmployeeId).DeptName;
            //    item.empPosition = empmanage.GetPositionByEmpid(e.EmployeeId).PositionName;
            //    item.IsApproval = monthly.GetAttendanceInfoByEmpid(e.EmployeeId, (DateTime)e.YearAndMonth).IsApproval;
            //    item.YearAndMonth=e.YearAndMonth;
            //    item.StartTime = e.StartTime;
            //    item.EndTime = e.EndTime;
            //    item.Duration = e.Duration;
            //    item.Remark = e.Remark;
            //    item.OvertimeReason = e.OvertimeReason;
            //    item.IsNoDaysOff = e.IsNoDaysOff;
            //    item.OvertimeTypeId = e.OvertimeTypeId;
            //    item.IsPass= e.IsPass;
            //    mylist.Add(item);
            //}
            var mylist = from e in otrlist
                         select new
                         {
                             #region 获取值
                             e.Id,
                             e.EmployeeId,
                             DDAppId = empmanage.GetInfoByEmpID(e.EmployeeId).DDAppId,
                             empName = empmanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             empDept = empmanage.GetDeptByEmpid(e.EmployeeId).DeptName,
                             empPosition = empmanage.GetPositionByEmpid(e.EmployeeId).PositionName,
                             IsApproval = monthly.GetAttendanceInfoByEmpid(e.EmployeeId, (DateTime)e.YearAndMonth).IsApproval,
                             e.YearAndMonth,
                             e.StartTime,
                             e.EndTime,
                             e.Duration,
                             e.Remark,
                             e.OvertimeReason,
                             e.IsNoDaysOff,
                             e.OvertimeTypeId,
                             e.IsPass,
                             #endregion
                         };
            var newlist = mylist.OrderByDescending(s => s.Id).Skip((page - 1) * limit).Take(limit).ToList();
            var newobj = new
            {
                code = 0,
                msg = "",
                count = mylist.Count(),
                data = newlist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 加班详情修改页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult OvertimeEdit(int  id)
        {
            OvertimeRecordManage overtime = new OvertimeRecordManage();
            EmployeesInfoManage manage = new EmployeesInfoManage();

            var s = overtime.GetEntity(id);
            ViewBag.EmpName = manage.GetEntity(s.EmployeeId).EmpName;
            return View(s);
        }
        /// <summary>
        /// 加班详情修改
        /// </summary>
        /// <param name="over"></param>
        /// <param name="type"></param>
        /// <param name="Duration"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OvertimeEdit(OvertimeRecord over)
        {
            AjaxResult result = new AjaxResult();
            OvertimeRecordManage overtime = new OvertimeRecordManage();
            MonthlySalaryRecordManage monthly = new MonthlySalaryRecordManage();
            AttendanceInfoManage attendance = new AttendanceInfoManage();
        

            var s = overtime.GetEntity(over.Id);
            over.EmployeeId = s.EmployeeId;
            over.YearAndMonth = s.YearAndMonth;

            var oldOvertime = overtime.OvertimeWithhold(s.OvertimeTypeId, Convert.ToDecimal(s.Duration));
            
         var att=monthly.GetAttendanceInfoByEmpid(over.EmployeeId,Convert.ToDateTime(over.YearAndMonth));
            var OvertimeWithhold= overtime.OvertimeWithhold(over.OvertimeTypeId, (dynamic)over.Duration);
            if ( (bool)over.IsPass)
            {
                OvertimeWithhold = 0;
            }

            att.OvertimeCharges = (att.OvertimeCharges - oldOvertime )+ OvertimeWithhold;
            attendance.Update(att);
            overtime.Update(over);


            try
            {
                result.Msg = "修改成功";
                result.Success = true;
                result.ErrorCode = 200;
            }
            catch (Exception e)
            {
                result.Msg = "修改失败";
                result.Success = true;
                result.ErrorCode = 200;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}