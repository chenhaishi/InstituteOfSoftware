using SiliconValley.InformationSystem.Business.EducationalBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.SchoolAttendanceManagementBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    public class EmpOverTimeRecordController : Controller
    {
        // GET: Personnelmatters/EmpOverTimeRecord
        public ActionResult EmpOverTimeRecordIndex()
        {
            return View();
        }

        /// <summary>
        /// 获取未审批的加班记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetOverTimeData(int page, int limit)
        {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            BeOnDutyManeger bodmanage = new BeOnDutyManeger();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var list = otrmanage.GetList()/*.Where(s => s.IsApproval == false)*/.ToList();

            var newlist = list.OrderByDescending(s => s.Id).Skip((page - 1) * limit).Take(limit).ToList();
            var etlist = from e in newlist
                         select new
                         {
                             #region 获取属性值 
                             e.Id,
                             e.EmployeeId,
                             empName = emanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             e.StartTime,
                             e.EndTime,
                             e.Duration,
                             e.OvertimeReason,
                             typename = bodmanage.GetSingleBeOnButy(e.OvertimeTypeId.ToString(), true).TypeName,
                             e.Remark,
                             e.IsNoDaysOff,
                           //  e.IsPassYear,
                            // e.IsApproval,
                             e.IsPass
                             #endregion
                         };
            var newobj = new
            {
                code = 0,
                msg = "",
                count = list.Count(),
                data = etlist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取已审批且未过年限（及当下年份）的加班记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetOverTimeApprovedData(int page, int limit, string AppCondition)
        {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            BeOnDutyManeger bodmanage = new BeOnDutyManeger();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var list = otrmanage.GetList()/*.Where(s => s.IsApproval == true && s.IsPassYear == false)*/.ToList();
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string IsNoDaysOff = str[1];
                string YearSelect = str[2];
                string MonthSelect = str[3];
                list = list.Where(e => emanage.GetEntity(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(IsNoDaysOff))
                {
                    list = list.Where(e => e.IsNoDaysOff == bool.Parse(IsNoDaysOff)).ToList();
                }
                if (!string.IsNullOrEmpty(YearSelect))
                {
                    list = list.Where(e => DateTime.Parse(e.StartTime.ToString()).Year == int.Parse(YearSelect)).ToList();
                }
                if (!string.IsNullOrEmpty(MonthSelect))
                {
                    var year = DateTime.Parse(MonthSelect).Year;
                    var month = DateTime.Parse(MonthSelect).Month;
                    list = list.Where(e => DateTime.Parse(e.StartTime.ToString()).Year == year && DateTime.Parse(e.StartTime.ToString()).Month == month).ToList();
                }
            }
            var newlist = list.OrderByDescending(s => s.Id).Skip((page - 1) * limit).Take(limit).ToList();
            var etlist = from e in newlist
                         select new
                         {
                             #region 获取属性值 
                             e.Id,
                             e.EmployeeId,
                             empName = emanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             e.StartTime,
                             e.EndTime,
                             e.Duration,
                             e.OvertimeReason,
                             typename = bodmanage.GetSingleBeOnButy(e.OvertimeTypeId.ToString(), true).TypeName,
                             e.Remark,
                             e.IsNoDaysOff,
                          //   e.IsPassYear,
                           //  e.IsApproval,
                             e.IsPass
                             #endregion
                         };
            var newobj = new
            {
                code = 0,
                msg = "",
                count = list.Count(),
                data = etlist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        //审批加班申请
        [HttpPost]
        public ActionResult overtimeIsPassed(int id, bool state)
        {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            var ajaxresult = new AjaxResult();
            try
            {
                var otr = otrmanage.GetEntity(id);
                //otr.IsApproval = true;
                otr.IsPass = state;
                otrmanage.Update(otr);
                ajaxresult = otrmanage.Success();
            }
            catch (Exception ex)
            {
                ajaxresult = otrmanage.Error(ex.Message);
            }
            return Json(ajaxresult, JsonRequestBehavior.AllowGet);
        }
        //根据编号获取加班记录对象
        public ActionResult GetOvertimeById(int id)
        {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            BeOnDutyManeger bodmanage = new BeOnDutyManeger();
            var otobj = otrmanage.GetEntity(id);
            var empobj = new
            {
                #region 获取属性值 
                otobj.Id,
                otobj.EmployeeId,
                empName = emanage.GetInfoByEmpID(otobj.EmployeeId).EmpName,
                otobj.StartTime,
                otobj.EndTime,
                otobj.Duration,
                otobj.OvertimeReason,
                otobj.OvertimeTypeId,
                otobj.Remark,
                otobj.IsNoDaysOff,
               // otobj.IsPassYear,
               // otobj.IsApproval,
                otobj.IsPass
                #endregion
            };
            return Json(empobj, JsonRequestBehavior.AllowGet);
        }
        //加班编辑页面
        public ActionResult OvertimeEdit(int id)
        {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            var otobj = otrmanage.GetEntity(id);
            ViewBag.Id = id;
            return View(otobj);
        }
        //加班编辑的提交
        [HttpPost]
        public ActionResult OvertimeEdit(OvertimeRecord otr)
        {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            var ajaxresult = new AjaxResult();
            try
            {
                var myotr = otrmanage.GetEntity(otr.Id);
                otr.IsPass = myotr.IsPass;
              //  otr.IsApproval = myotr.IsApproval;
              //  otr.IsPassYear = myotr.IsPassYear;
                otrmanage.Update(otr);
                ajaxresult = otrmanage.Success();
            }
            catch (Exception ex)
            {
                ajaxresult = otrmanage.Error(ex.Message);
            }

            return Json(ajaxresult, JsonRequestBehavior.AllowGet);
        }
        //修改加班的过了年限的数据，将 是否过了年限 属性进行修改
        [HttpPost]
        public ActionResult EditIsPassYear(string list)
        {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            var ajaxresult = new AjaxResult();
            string[] arr = list.Split(',');
            for (int i = 0; i < arr.Length - 1; i++)
            {
                try
                {
                    int id = int.Parse(arr[i]);
                    var otr = otrmanage.GetEntity(id);
                   // otr.IsPassYear = true;
                    otrmanage.Update(otr);
                    ajaxresult = otrmanage.Success();

                }
                catch (Exception ex)
                {
                    ajaxresult = otrmanage.Error(ex.Message);
                }
            }

            return Json(ajaxresult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取未审批的调休记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetDaysOffData(int page, int limit)
        {
            DaysOffManage dfmanage = new DaysOffManage();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var list = dfmanage.GetList().Where(s => s.IsApproval == false).ToList();
            var newlist = list.OrderByDescending(s => s.Id).Skip((page - 1) * limit).Take(limit).ToList();
            var etlist = from e in newlist
                         select new
                         {
                             #region 获取属性值 
                             e.Id,
                             e.EmployeeId,
                             empName = emanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             e.StartTime,
                             e.EndTime,
                             e.Duration,
                             e.LeaveReason,
                             e.IsPassYear,
                             e.IsApproval,
                             e.IsPass
                             #endregion
                         };
            var newobj = new
            {
                code = 0,
                msg = "",
                count = list.Count(),
                data = etlist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取已审批且未过年限（及当下年份）的调休记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetDaysOffApprovedData(int page, int limit, string AppCondition)
        {
            DaysOffManage dfmanage = new DaysOffManage();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var list = dfmanage.GetList().Where(s => s.IsApproval == true && s.IsPassYear == false).ToList();
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string IsPass = str[1];
                string YearSelect = str[2];
                string MonthSelect = str[3];
                list = list.Where(e => emanage.GetEntity(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(IsPass))
                {
                    list = list.Where(e => e.IsPass == bool.Parse(IsPass)).ToList();
                }
                if (!string.IsNullOrEmpty(YearSelect))
                {
                    list = list.Where(e => DateTime.Parse(e.StartTime.ToString()).Year == int.Parse(YearSelect)).ToList();
                }
                if (!string.IsNullOrEmpty(MonthSelect))
                {
                    var year = DateTime.Parse(MonthSelect).Year;
                    var month = DateTime.Parse(MonthSelect).Month;
                    list = list.Where(e => DateTime.Parse(e.StartTime.ToString()).Year == year && DateTime.Parse(e.StartTime.ToString()).Month == month).ToList();
                }
            }
            var newlist = list.OrderByDescending(s => s.Id).Skip((page - 1) * limit).Take(limit).ToList();
            var etlist = from e in newlist
                         select new
                         {
                             #region 获取属性值 
                             e.Id,
                             e.EmployeeId,
                             empName = emanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             e.StartTime,
                             e.EndTime,
                             e.Duration,
                             e.LeaveReason,
                             e.IsPassYear,
                             e.IsApproval,
                             e.IsPass
                             #endregion
                         };
            var newobj = new
            {
                code = 0,
                msg = "",
                count = list.Count(),
                data = etlist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        //根据编号获取调休申请对象
        public ActionResult GetDaysOffById(int id)
        {
            DaysOffManage dfmanage = new DaysOffManage();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var mydf = dfmanage.GetEntity(id);
            var empobj = new
            {
                #region 获取属性值 
                mydf.Id,
                mydf.EmployeeId,
                empName = emanage.GetInfoByEmpID(mydf.EmployeeId).EmpName,
                mydf.StartTime,
                mydf.EndTime,
                mydf.Duration,
                mydf.LeaveReason,
                Image = mydf.Image,
                mydf.IsPassYear,
                mydf.IsApproval,
                mydf.IsPass
                #endregion
            };
            return Json(empobj, JsonRequestBehavior.AllowGet);
        }
        //审批调休申请
        [HttpPost]
        public ActionResult DaysoffIsPassed(int id, bool state)
        {
            DaysOffManage dfmanage = new DaysOffManage();
            var ajaxresult = new AjaxResult();
            try
            {
                var otr = dfmanage.GetEntity(id);
                otr.IsApproval = true;
                otr.IsPass = state;
                dfmanage.Update(otr);
                ajaxresult = dfmanage.Success();
            }
            catch (Exception ex)
            {
                ajaxresult = dfmanage.Error(ex.Message);
            }
            return Json(ajaxresult, JsonRequestBehavior.AllowGet);
        }
        //调休申请的详情页
        public ActionResult DaysOffDetail(int id)
        {
            DaysOffManage dfmanage = new DaysOffManage();
            var mydf = dfmanage.GetEntity(id);
            ViewBag.Id = id;
            return View(mydf);
        }
        //已审批的调休申请的编辑
        public ActionResult DaysOffEdit(int id)
        {
            DaysOffManage dfmanage = new DaysOffManage();
            var mydf = dfmanage.GetEntity(id);
            ViewBag.Id = id;
            return View(mydf);
        }
        [HttpPost]
        public ActionResult DaysOffEdit(DaysOff df)
        {
            DaysOffManage dfmanage = new DaysOffManage();
            var ajaxresult = new AjaxResult();
            try
            {
                var myotr = dfmanage.GetEntity(df.Id);
                myotr.Duration = df.Duration;
                if (df.Image != "undefined")
                {
                    myotr.Image = ImageUpload();
                }
               
                dfmanage.Update(myotr);
                ajaxresult = dfmanage.Success();
            }
            catch (Exception ex)
            {
                ajaxresult = dfmanage.Error(ex.Message);
            }
            return Json(ajaxresult, JsonRequestBehavior.AllowGet);
        }
        //修改调休的过了年限的数据，将 是否过了年限 属性进行修改
        public ActionResult EditDaysOffIsPassYear(string list)
        {
            DaysOffManage dfmanage = new DaysOffManage();
            var ajaxresult = new AjaxResult();
            string[] arr = list.Split(',');
            for (int i = 0; i < arr.Length - 1; i++)
            {
                try
                {
                    int id = int.Parse(arr[i]);
                    var otr = dfmanage.GetEntity(id);
                    otr.IsPassYear = true;
                    dfmanage.Update(otr);
                    ajaxresult = dfmanage.Success();
                }
                catch (Exception ex)
                {
                    ajaxresult = dfmanage.Error(ex.Message);
                }
            }

            return Json(ajaxresult, JsonRequestBehavior.AllowGet);
        }

        //将作为调休的加班申请统计
        public bool Check(List<MyStaticsData> mydata, OvertimeRecord otr)
        {
            foreach (var item in mydata)
            {
                if (item.EmployeeId == otr.EmployeeId)
                {
                    item.OvertimeTotaltime += otr.Duration;//可调休总时间
                    item.ResidueDaysoffTime = item.OvertimeTotaltime - item.DaysoffTotaltime;
                    return true;
                }

            }
            return false;

        }
        //将已调休完的统计
        public bool Check2(List<MyStaticsData> mydata, DaysOff dff)
        {
            foreach (var item in mydata)
            {
                if (item.EmployeeId == dff.EmployeeId)
                {
                    item.DaysoffTotaltime += dff.Duration;//已调休总时间
                    item.ResidueDaysoffTime = item.OvertimeTotaltime - item.DaysoffTotaltime;
                    return true;
                }

            }

            return false;
        }
        /// <summary>
        ///获取加班及调休的统计数据 (首先按年份分别找到每个人的加班总时长和调休总时长)
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStatisticsTimeData(int page, int limit)
        {
            OvertimeRecordManage otrmanage = new OvertimeRecordManage();
            DaysOffManage dfmanage = new DaysOffManage();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            List<MyStaticsData> Statisticslist = new List<MyStaticsData>();
            //获取可调休的总时长（已审批通过的作为调休的加班总时间）
            var otrlist = otrmanage.GetList().Where(s => s.IsNoDaysOff == false && /*s.IsPassYear == false &&*/ s.IsPass == true).ToList();
            var dflist = dfmanage.GetList().Where(s => s.IsPassYear == false && s.IsPass == true).ToList();
            foreach (var item in otrlist)
            {
                if (!Check(Statisticslist, item))
                {
                    MyStaticsData msd = new MyStaticsData();
                    msd.EmployeeId = item.EmployeeId;
                    msd.YearTime = DateTime.Parse(item.EndTime.ToString()).Year;
                    msd.OvertimeTotaltime = item.Duration;
                    msd.DaysoffTotaltime = 0;
                    msd.ResidueDaysoffTime = msd.OvertimeTotaltime - msd.DaysoffTotaltime;
                    Statisticslist.Add(msd);
                }

            }

            foreach (var item in dflist)
            {
                if (!Check2(Statisticslist, item))
                {
                    MyStaticsData msd = new MyStaticsData();
                    msd.EmployeeId = item.EmployeeId;
                    msd.YearTime = DateTime.Parse(item.EndTime.ToString()).Year;
                    msd.OvertimeTotaltime = 0;
                    msd.DaysoffTotaltime = item.Duration;
                    msd.ResidueDaysoffTime = msd.OvertimeTotaltime - msd.DaysoffTotaltime;
                    Statisticslist.Add(msd);
                }
            }
            var newlist = Statisticslist.OrderByDescending(s => s.YearTime).Skip((page - 1) * limit).Take(limit).ToList();

            var newobj = from ss in newlist
                         select new
                         {
                             empName = emanage.GetEntity(ss.EmployeeId).EmpName,
                             ss.EmployeeId,
                             ss.OvertimeTotaltime,
                             ss.DaysoffTotaltime,
                             ss.ResidueDaysoffTime,
                             ss.YearTime
                         };

            var obj = new
            {
                code = 0,
                msg = "",
                count = Statisticslist.Count(),
                data = newobj
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        // 图片上传
        public string ImageUpload()
        {

            StringBuilder ProName = new StringBuilder();
            HttpPostedFileBase file = Request.Files["Image"];
            string fname = file.FileName; //获取上传文件名称（包含扩展名）
            string f = Path.GetFileNameWithoutExtension(fname);//获取文件名称
            string name = Path.GetExtension(fname);//获取扩展名
            string pfilename = AppDomain.CurrentDomain.BaseDirectory + "uploadXLSXfile/EmpImage/";//获取当前程序集下面的uploads文件夹中的文件夹目录
            string completefilePath = DateTime.Now.ToString("yyyyMMddhhmmss") + name;//将上传的文件名称转变为当前项目名称
            ProName.Append(Path.Combine(pfilename, completefilePath));//合并成一个完整的路径;
            file.SaveAs(ProName.ToString());//上传文件   

            return completefilePath;
        }
    }
}