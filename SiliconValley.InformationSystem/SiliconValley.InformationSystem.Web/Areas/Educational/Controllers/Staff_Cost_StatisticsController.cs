using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business.EducationalBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using System.IO;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    public class Staff_Cost_StatisticsController : Controller
    {
        //员工费用统计业务类
        private Staff_Cost_StatisticssBusiness db_staf_Cost =new Staff_Cost_StatisticssBusiness ();
        //员工业务类
        public EmployeesInfoManage EmployeesInfoManage_Entity;
        //排课业务类
        public ReconcileManeger Reconcile_Entity = new ReconcileManeger ();
        //教员业务类
        public TeacherBusiness TeacherBusiness_Entity = new TeacherBusiness();

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

            ViewBag.deps = deps;

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
            var deps = db_staf_Cost.GetDepartments();

            ViewBag.deps = deps;
            return View();
        }

        /// <summary>
        /// 费用统计
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CostStatistics(string date, int DeptID)
        {

            AjaxResult resultObj = new AjaxResult();

            try
            {
                DateTime dt = Convert.ToDateTime(date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2));

                //根据部门生成费用
                List<EmployeesInfo> Emp_List = EmployeesInfoManage_Entity.GetEmpsByDeptid(DeptID);

                for (int i = 0; i < Emp_List.Count; i++)
                {
                    //带班数量
                    string sqlstr = "select ClassSchedule_Id from Reconcile  where Year(AnPaiDate)='"+dt.Year+"' " +
                        " and Month(AnPaiDate) = '"+dt.Month+"' and EmployeesInfo_Id = '"+Emp_List[i].EmployeeId+"'" +
                        " group by ClassSchedule_Id";
                    List<Reconcile> concile_List = Reconcile_Entity.GetListBySql<Reconcile>(sqlstr);
                    Position position = db_staf_Cost.GetPositionByEmp(Emp_List[i].EmployeeId);
                    if (concile_List.Count == 1 && position.PositionName=="教学主任")
                    {
                        //int ClassCount = Reconcile_Entity.GetTeacherJieshu(dt.Year,dt.Month,Emp_List[i].EmployeeId);

                        Teacher teacher = TeacherBusiness_Entity.GetList().FirstOrDefault(s=>s.EmployeeId==Emp_List[i].EmployeeId);
                    }

                }

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


        public ActionResult Emp_Cost_Statististics(string data)
        {
            return View();
        }
    }
}