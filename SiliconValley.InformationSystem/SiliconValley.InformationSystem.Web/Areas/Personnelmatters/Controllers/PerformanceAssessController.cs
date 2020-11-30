using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using System.IO;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    public class PerformanceAssessController : Controller
    {
        RedisCache rc = new RedisCache();
        MeritsCheckManage mcmanage = new MeritsCheckManage();

        //第一次进入绩效考核页面时加载的年月份的方法
        static string GetFirstTime()
        {
            MeritsCheckManage msrmanage = new MeritsCheckManage();
            string mytime = "";
            if (msrmanage.GetEmpMCData().Where(s => s.IsDel == false).Count() > 0)
            {
                var time = msrmanage.GetEmpMCData().Where(s => s.IsDel == false).LastOrDefault().YearAndMonth;
                mytime = DateTime.Parse(time.ToString()).Year + "-" + DateTime.Parse(time.ToString()).Month;
            }
            else
            {
                mytime = "";
            }
            return mytime;
        }
        static string FirstTime = GetFirstTime();

        //绩效考核统计
        // GET: Personnelmatters/PerformanceAssess
        public ActionResult PerformanceAssessIndex()
        {
            ViewBag.yearandmonth = FirstTime;
            return View();
        }

        /// <summary>
        /// 获取绩效考核的数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult PerformanceAssessShow(int page, int limit, string AppCondition, string time)
        {
            time = FirstTime;
            //var UserName = Base_UserBusiness.GetCurrentUser();//获取当前登录人
            //string eid = UserName.EmpNumber;//为测试，暂时设置的死数据
             
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var mclist = mcmanage.GetEmpMCData().Where(s => s.IsDel == false).ToList();
            //var userdept = emanage.GetDeptByEmpid(eid);
            //var userposition = emanage.GetPositionByEmpid(eid);
            //if (userdept.DeptName!="校办" && (userposition.PositionName.Contains("主任")|| userposition.PositionName.Contains("主任"))) {
            //    mclist = mclist.Where(s=>emanage.GetDeptByEmpid(s.EmployeeId).DeptName==userdept.DeptName).ToList();
            //}
            if (!string.IsNullOrEmpty(time))
            {
                var stime = DateTime.Parse(time);
                mclist = mclist.Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == stime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == stime.Month).ToList();
            }
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string deptname = str[1];
                string pname = str[2];
                string Empstate = str[3];
                mclist = mclist.Where(e => emanage.GetInfoByEmpID(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(deptname))
                {
                    mclist = mclist.Where(e => emanage.GetDeptByEmpid(e.EmployeeId).DeptId == int.Parse(deptname)).ToList();
                }
                if (!string.IsNullOrEmpty(pname))
                {
                    mclist = mclist.Where(e => emanage.GetPositionByEmpid(e.EmployeeId).Pid == int.Parse(pname)).ToList();
                }
                if (!string.IsNullOrEmpty(Empstate))
                {
                    mclist = mclist.Where(e => emanage.GetInfoByEmpID(e.EmployeeId).IsDel == bool.Parse(Empstate)).ToList();
                }

            }
            var newlist = mclist.Skip((page - 1) * limit).Take(limit).ToList();
            var etlist = from e in newlist
                         select new
                         {
                             #region 获取属性值 
                             e.Id,
                             e.EmployeeId,
                             empName = emanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             empDept = emanage.GetDept(emanage.GetInfoByEmpID(e.EmployeeId).PositionId).DeptName,
                             empPosition = emanage.GetPositionByEmpid(e.EmployeeId).PositionName,
                             empIsDel = emanage.GetInfoByEmpID(e.EmployeeId).IsDel,
                             e.YearAndMonth,
                             e.RoutineWork,
                             routineWorkPropotion = e.RoutineWorkPropotion <= 1 && e.RoutineWorkPropotion > 0 ? (e.RoutineWorkPropotion * 100) + "%" : e.RoutineWorkPropotion > 1 ? e.RoutineWorkPropotion + "%" : null,
                             routineWorkFillRate = e.RoutineWorkFillRate <= 1 && e.RoutineWorkFillRate > 0 ? (e.RoutineWorkFillRate * 100) + "%" : e.RoutineWorkFillRate > 1 ? e.RoutineWorkFillRate + "%" : null,
                             e.OtherWork,
                             otherWorkPropotion = e.OtherWorkPropotion <= 1 && e.OtherWorkPropotion > 0 ? (e.OtherWorkPropotion * 100) + "%" : e.OtherWorkPropotion > 1 ? e.OtherWorkPropotion + "%" : null,
                             otherWorkFillRate = e.OtherWorkFillRate <= 1 && e.OtherWorkFillRate > 0 ? (e.OtherWorkFillRate * 100) + "%" : e.OtherWorkFillRate > 1 ? e.OtherWorkFillRate + "%" : null,
                             e.SelfReportedScore,
                             e.SuperiorGrade,
                             e.FinalGrade,
                             e.Remark,
                             e.IsDel,
                             e.IsApproval
                             #endregion
                         };

            var newobj = new
            {
                code = 0,
                msg = "",
                count = mclist.Count(),
                data = etlist
            };

            return Json(newobj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 年月份及应到勤天数的改变
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeMCTime()
        {
            ViewBag.time = FirstTime;
            return View();
        }
        [HttpPost]
        public ActionResult ChangeMCTime(string CurrentTime)
        {
            var AjaxResultxx = new AjaxResult();
            try
            {
                FirstTime = CurrentTime;
                //var attlist = mcmanage.GetEmpMCData().Where(s => s.IsDel == false).ToList();
                //for (int i = 0; i < attlist.Count(); i++)
                //{
                //    attlist[i].YearAndMonth = Convert.ToDateTime(CurrentTime);
                //    mcmanage.Update(attlist[i]);
                //    rc.RemoveCache("InRedisMCData");
                AjaxResultxx = mcmanage.Success();
                //}
            }
            catch (Exception ex)
            {
                AjaxResultxx = mcmanage.Error(ex.Message);
            }

            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑员工绩效考核数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditEmpPFAssess(int id)
        {
            var mc = mcmanage.GetEntity(id);
            ViewBag.id = id;
            return View(mc);
        }
        public ActionResult GetMCByid(int id)
        {
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var mc = mcmanage.GetEntity(id);
            var mcobj = new
            {
                mc.Id,
                mc.EmployeeId,
                empName = empmanage.GetInfoByEmpID(mc.EmployeeId).EmpName,
                empmanage.GetInfoByEmpID(mc.EmployeeId).Sex,
                mc.YearAndMonth,
                mc.RoutineWork,
                mc.RoutineWorkPropotion,
                mc.RoutineWorkFillRate,
                mc.OtherWork,
                mc.OtherWorkPropotion,
                mc.OtherWorkFillRate,
                mc.SelfReportedScore,
                mc.SuperiorGrade,
                mc.FinalGrade,
                mc.Remark,
                mc.IsDel,
                mc.IsApproval
            };
            return Json(mcobj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditEmpPFAssess(MeritsCheck mc)
        {
            var AjaxResultxx = new AjaxResult();
            try
            {
                var m = mcmanage.GetEntity(mc.Id);
                m.FinalGrade = mc.FinalGrade;
                //mc.EmployeeId = m.EmployeeId;
                //mc.YearAndMonth = m.YearAndMonth;
                //mc.IsDel = m.IsDel;
                //mc.IsApproval = m.IsApproval;
                mcmanage.Update(m);
                rc.RemoveCache("InRedisMCData");
                AjaxResultxx = mcmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = mcmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 批量添加绩效数据
        /// </summary>
        /// <returns></returns>
        public ActionResult AddMCEmps()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddMCEmps(string time)
        {
            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                //获取未禁用的员工
                var mclist = esemanage.GetEmpESEData().Where(s => s.IsDel == false).ToList();
                foreach (var item in mclist)
                {
                    var emp = empmanage.GetInfoByEmpID(item.EmployeeId);
                    MeritsCheck mc = new MeritsCheck();
                    mc.EmployeeId = item.EmployeeId;
                    mc.YearAndMonth = DateTime.Parse(time);
                    if (!string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                    {
                        mc.FinalGrade = 100;
                    }
                    mc.IsDel = false;
                    mc.IsApproval = false;
                    mcmanage.Insert(mc);
                    rc.RemoveCache("InRedisMCData");
                }
                AjaxResultxx = mcmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = mcmanage.Error(ex.Message);
            }
            FirstTime = time;
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 判断某月份员工工资是否已确认审批
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsConfirmApproval(string time)
        {
            MeritsCheckManage mcmanage = new MeritsCheckManage();//员工月度工资
            var AjaxResultxx = new AjaxResult();
            try
            {
                var mtime = DateTime.Parse(time);
                var msrlist = mcmanage.GetEmpMCData().Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == mtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == mtime.Month).ToList();
                if (msrlist.FirstOrDefault().IsApproval == true)
                {
                    AjaxResultxx.Data = "该月份员工绩效考核已确认审批！";
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
                AjaxResultxx = mcmanage.Error(ex.Message);
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
                var curlist = mcmanage.GetEmpMCData().Where(s => DateTime.Parse(s.YearAndMonth.ToString()).Year == curtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == curtime.Month).ToList();
                foreach (var item in curlist)
                {
                    item.IsApproval = true;
                    mcmanage.Update(item);
                    rc.RemoveCache("InRedisMCData");
                }
                AjaxResultxx = mcmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = mcmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PermissionManage() {
            return View();
        }

        public ActionResult AssessmentUpload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AssessmentUpload(HttpPostedFileBase excelfile)//当文件类型错误，则excelfile=null
        {
            Stream filestream = excelfile.InputStream;
            MeritsCheckManage meritsCheckManage = new MeritsCheckManage();

            var result = meritsCheckManage.ImportDataFormExcel(filestream, excelfile.ContentType);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 绩效模板下载
        /// </summary>
        /// <returns></returns>
        public FileStreamResult AssessmentDownFile()
        {
            string rr = Server.MapPath("/uploadXLSXfile/Template/绩效模板.xlsx");  //获取下载文件的路径         
            FileStream stream = new FileStream(rr, FileMode.Open);
            return File(stream, "application/octet-stream", Server.UrlEncode("绩效模板.xls"));
        }
        public ActionResult AssessmentTimePeriodQuery(string  empid)
        {
            EmployeesInfoManage manage = new EmployeesInfoManage();
            ViewBag.empid = empid;
            string[] str = empid.Split(',');
            string name = "";
            for (int i = 0; i < str.Length-1; i++)
            {
               name+= manage.GetInfoByEmpID(str[i]).EmpName+",";
            }
            ViewBag.name = name;
            return View();
        }
        [HttpGet]
        public ActionResult TimePeriodQuery(int page, int limit, string time,string empid)
        {
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var mclist = mcmanage.GetEmpMCData().Where(s => s.IsDel == false&& s.EmployeeId == empid).ToList();

            if (!string.IsNullOrEmpty(time))
            {
                string[] str = time.Split(',');
                string time1 = str[0];
                string time2 = str[1];
                if (!string.IsNullOrEmpty(time1)&& string.IsNullOrEmpty(time2))
                {
                    mclist = mclist.Where(e =>e.YearAndMonth>=Convert.ToDateTime(time1)).ToList();
                }
                if (!string.IsNullOrEmpty(time2) && string.IsNullOrEmpty(time1))
                {
                    mclist = mclist.Where(e => e.YearAndMonth<= Convert.ToDateTime(time2)).ToList();
                }
                if (!string.IsNullOrEmpty(time1) && !string.IsNullOrEmpty(time2))
                {
                    mclist = mclist.Where(e => e.YearAndMonth >= Convert.ToDateTime(time1)&&e.YearAndMonth <= Convert.ToDateTime(time2)).ToList();
                }

            }
            var newlist = mclist.Skip((page - 1) * limit).Take(limit).ToList();
            var etlist = from e in newlist
                         select new
                         {
                             #region 获取属性值 
                             e.Id,
                             e.EmployeeId,
                             empName = emanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             empDept = emanage.GetDept(emanage.GetInfoByEmpID(e.EmployeeId).PositionId).DeptName,
                             empPosition = emanage.GetPositionByEmpid(e.EmployeeId).PositionName,
                             empIsDel = emanage.GetInfoByEmpID(e.EmployeeId).IsDel,
                             YearAndMonth=Convert.ToDateTime(e.YearAndMonth.ToString()).ToString("yyyy年MM月"),
                             e.FinalGrade,
                             e.Remark,
                             e.IsDel,
                             e.IsApproval
                             #endregion
                         };

            var newobj = new
            {
                code = 0,
                msg = "",
                count = mclist.Count(),
                data = etlist
            };

            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
    }
}