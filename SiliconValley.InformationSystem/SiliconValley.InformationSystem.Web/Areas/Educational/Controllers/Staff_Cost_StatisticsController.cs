using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
/// <summary>
/// ////////////////////////
/// </summary>
/// 

using SiliconValley.InformationSystem.Business.EducationalBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using System.IO;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    public class Staff_Cost_StatisticsController : Controller
    {

        private Staff_Cost_StatisticssBusiness db_staf_Cost;


        public Staff_Cost_StatisticsController()
        {
            db_staf_Cost = new Staff_Cost_StatisticssBusiness();
        }

        // GET: Educational/Staff_Cost_Statistics

        /// <summary>
        /// 员工费用统计页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Staff_Cost_StatisticsIndex()
        {

            var deps = db_staf_Cost.GetDepartments();

            ViewBag.Deps = deps;

            return View();
        }

        /// <summary>
        /// 员工数据
        /// </summary>
        /// <returns></returns>
        public ActionResult EmpData(int limit, int page, string empName = null, string depId = null)
        {
            //获取筛选之后的员工
            List<EmployeesInfo> emplist = db_staf_Cost.ScreenEmp(EmpName: empName, DepId: depId);

            //分页
            List<EmployeesInfo> skiplist = emplist.Skip((page - 1) * limit).Take(limit).ToList();

            //组装返回对象
            var result = new
            {
                code = 0,
                count = emplist.Count,
                msg = "",
                data = skiplist

            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 返回员工部门
        /// </summary>
        /// <param name="PositionId">员工编号</param>
        /// <returns></returns>
        public ActionResult empDep(string EmployeeId)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                var dep = db_staf_Cost.GetDeparmentByEmp(EmployeeId);

                result.Data = dep;
                result.ErrorCode = 200;
                result.Msg = "";

            }
            catch (Exception)
            {
                result.Data = null;
                result.ErrorCode = 500;
                result.Msg = "";
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 获取员工岗位
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <returns></returns>
        public ActionResult empPosition(string EmployeeId)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                var poo = db_staf_Cost.GetPositionByEmp(EmployeeId);

                result.Data = poo;
                result.ErrorCode = 200;
                result.Msg = "";

            }
            catch (Exception)
            {
                result.Data = null;
                result.ErrorCode = 500;
                result.Msg = "";
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取工作日天数
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkingDays(string date)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                int Days = db_staf_Cost.WorkingDate(DateTime.Parse(date));

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = Days.ToString();
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 生成费用统计数
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateCostStatistics()
        {
            return View();
        }

        /// <summary>
        /// 费用统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CostStatistics(string date, int workingDays)
        {

            AjaxResult resultObj = new AjaxResult();

            try
            {
                EmployeesInfoManage tempdb_emp = new EmployeesInfoManage();
                //获取所以员工信息
                var list = tempdb_emp.GetAll();

                List<Cose_StatisticsItems> result = new List<Cose_StatisticsItems>();

                List<Staff_Cost_StatisticesDetailView> detaillist = new List<Staff_Cost_StatisticesDetailView>();
                foreach (var item in list)
                {
                   
                    try
                    {
                        var data = db_staf_Cost.Staff_CostData(item.EmployeeId, DateTime.Parse(date), workingDays);

                        var obj = db_staf_Cost.Statistics_Cost(data);

                        result.Add(obj);

                        detaillist.Add(data);
                    }
                    catch (Exception ex)
                    {


                        //emp = item.emp;
                    }
                   
                }

                string Detailfilename = DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month + "费用统计明细表";

                db_staf_Cost.SaveStaff_CostData(detaillist, result, Detailfilename);
                //保存到文件 
                string filename = DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month+"费用统计表";

                db_staf_Cost.SaveToExcel(result, filename);

                resultObj.ErrorCode = 200;
                resultObj.Msg = "成功";
                resultObj.Data = result;

            }
            catch (Exception ex)
            {
                resultObj.ErrorCode = 500;
                resultObj.Msg = "失败";
                resultObj.Data = null;

                SessionHelper.Session["CostStatistics"] = null;

            }

            return Json(resultObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下载费用统计文件
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadCostStatics(string date= null, string Dfilename = null)
        {
            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            string filename = Dfilename == null? DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month + "费用统计表.xls" : Dfilename;

            string pathName = "/CostHistoryFiles/" + filename;

            //开始下载
            //FileStream stream = new FileStream(Server.MapPath(pathName), FileMode.Open, FileAccess.Read);
            var filedata = client.GetObject("xinxihua", pathName);

            return File(filedata.ObjectContent, "application/vnd.ms-excel", filename);


        }


        /// <summary>
        /// 个人费用统计
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult PersonalCostStatics(string empid)
        {
            EmployeesInfoManage db_emp = new EmployeesInfoManage();

            ViewBag.Emp = db_emp.GetInfoByEmpID(empid);

            return View();
        }

        [HttpPost]
        public ActionResult PersonalCostStatics(string empid, string date, int workingDays)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var data = db_staf_Cost.Staff_CostData(empid, DateTime.Parse(date), workingDays);
                var costItems = db_staf_Cost.Statistics_Cost(data);

                result.Data = costItems;
                result.ErrorCode = 200;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {

                result.Data = null;
                result.ErrorCode = 500;
                result.Msg = "失败";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 历史纪录
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoryCost()
        {
            return View();
        }

        public ActionResult HistoryCostFileData(int page, int limit)
        {

            //List<FileInfo> list = db_staf_Cost.HistoryCostFileData().OrderBy(d=>d.LastWriteTime).ToList();

            //List<FileInfo> skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();

            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            var list = client.ListObjects("xinxihua", "CostHistoryFiles").Contents.OrderByDescending(d=>d.LastModified).ToList();

            var skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();

            List<object> dataObj = new List<object>();

            foreach (var item in skiplist)
            {
                var filename = Path.GetFileName(item.Key);

                if (!string.IsNullOrEmpty(filename))
                {
                    var tempobj = new
                    {

                        filename = filename,
                        lastupdatetime = item.LastModified

                    };
                    dataObj.Add(tempobj);
                }
            }

            var obj = new {

                code = 0,
                msg ="",
                count = list.Count,
                data = dataObj
            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }
    }
}