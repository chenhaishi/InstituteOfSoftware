
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
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
using SiliconValley.InformationSystem.Business.CourseSchedulingSysBusiness;
using SiliconValley.InformationSystem.Business;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using System.Xml;
using SiliconValley.InformationSystem.Business.Coursewaremaking_Business;
using SiliconValley.InformationSystem.Business.Base_SysManage;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    [CheckLogin]
    public class Staff_Cost_StatisticsController : Controller
    {
        //员工费用统计业务类
        private Staff_Cost_StatisticssBusiness db_staf_Cost = new Staff_Cost_StatisticssBusiness();
        //员工业务类
        public EmployeesInfoManage EmployeesInfoManage_Entity = new EmployeesInfoManage();
        //排课业务类
        public ReconcileManeger Reconcile_Entity = new ReconcileManeger();
        //教员业务类
        public TeacherBusiness TeacherBusiness_Entity = new TeacherBusiness();

        public CurriculumBusiness curriculum_Entity = new CurriculumBusiness();

        public BaseBusiness<Position> Position_Entity = new BaseBusiness<Position>();

        public BaseBusiness<Department> Department_Entity = new BaseBusiness<Department>();

        public ClassScheduleBusiness ClassSchedule_Entity = new ClassScheduleBusiness();

        public GrandBusiness Grand_Entity = new GrandBusiness();

        public ScheduleForTraineesBusiness ScheduleForTrainees_Entity = new ScheduleForTraineesBusiness();

        public BaseBusiness<TeacherAddorBeonDutyView> teacherAddorBeonDutyView_Entity = new BaseBusiness<TeacherAddorBeonDutyView>();

        public ExaminationBusiness Examination_Entity = new ExaminationBusiness();

        public ExaminationRoomBusiness ExaminationRoom_Entity = new ExaminationRoomBusiness();

        public BaseBusiness<MarkingArrange> Marking_Entity = new BaseBusiness<MarkingArrange>();

        public CandidateInfoBusiness Candi_Entity = new CandidateInfoBusiness();

        public CoursewaremakingBusiness Courseware_Entity = new CoursewaremakingBusiness();

        public BaseBusiness<HeadClass> HeadClass_Entity = new BaseBusiness<HeadClass>();

        public HeadmasterBusiness Headmaster_Entity = new HeadmasterBusiness();

        public ClassTimeBusiness ClassTime_Entity = new ClassTimeBusiness();

        public Staff_Cost_StatisticsController()
        {
            db_staf_Cost = new Staff_Cost_StatisticssBusiness();
        }

        // GET: Educational/Staff_Cost_Statistics

        public ActionResult GetPersonByid(int departid)
        {
            List<EmployeesInfo> Emp = EmployeesInfoManage_Entity.GetEmpsByDeptid(departid);
            List<SelectListItem> select_list = new List<SelectListItem>();
            for (int i = 0; i < Emp.Count; i++)
            {
                SelectListItem select = new SelectListItem();
                select.Text = Emp[i].EmpName;
                select.Value = Emp[i].EmployeeId;
                select_list.Add(select);
            }
            return Json(select_list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 管理底课时页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageClassTime()
        {
            var deps = db_staf_Cost.GetDepartmentbyjiaoxue();

            ViewBag.deptlist = deps;

            ViewBag.NOSetCount = GetNOSetClassTime();

            return View();
        }


        public ActionResult GetClassTime(int limit, int page, string dept = null)
        {

            string sql = "select * from ManageClassTime where ClassTimeState != 0";
            List<ManageClassTime> list = ClassTime_Entity.GetListBySql<ManageClassTime>(sql);

            if (dept != "0" && dept != null)
            {
                List<EmployeesInfo> emp_list = EmployeesInfoManage_Entity.GetEmpsByDeptid(Convert.ToInt32(dept));
                List<ManageClassTime> templist = new List<ManageClassTime>();
                for (int i = 0; i < emp_list.Count; i++)
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        if (emp_list[i].EmployeeId == list[k].Emp_ID)
                        {
                            templist.Add(list[k]);
                            break;
                        }

                    }

                }
                var resData = templist.Select(
                a => new
                {
                    ID = a.ID,
                    Emp_Name = EmployeesInfoManage_Entity.GetEntity(a.Emp_ID).EmpName,
                    classTime = a.ClassTime,
                    Dept_Name = EmployeesInfoManage_Entity.GetDeptByEmpid(a.Emp_ID).DeptName
                });
                var temp = new
                {
                    code = 0,
                    count = resData.Count(),
                    msg = "",
                    data = resData.Skip((page - 1) * limit).Take(limit).ToList()
                };
                return Json(temp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var resData = list.Select(
                a => new
                {
                    ID = a.ID,
                    Emp_Name = EmployeesInfoManage_Entity.GetEntity(a.Emp_ID).EmpName,
                    classTime = a.ClassTime,
                    Dept_Name = EmployeesInfoManage_Entity.GetDeptByEmpid(a.Emp_ID).DeptName
                });
                var temp = new
                {
                    code = 0,
                    count = resData.Count(),
                    msg = "",
                    data = resData.Skip((page - 1) * limit).Take(limit).ToList()
                };
                return Json(temp, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        /// <summary>
        /// 查询有多少老师是没有设置底课时
        /// </summary>
        /// <returns></returns>
        public int GetNOSetClassTime()
        {
            //List<Department> deptlist = db_staf_Cost.GetDepartmentbyjiaoxue();
            //List<EmployeesInfo> Emp_List = new List<EmployeesInfo>();
            //for (int i = 0; i < deptlist.Count; i++)
            //{
            //    Emp_List.AddRange(EmployeesInfoManage_Entity.GetEmpsByDeptid(deptlist[i].DeptId);
            //}

            Department dept = EmployeesInfoManage_Entity.GetDeptByDname("s1、s2教学部");
            Department dept1 = EmployeesInfoManage_Entity.GetDeptByDname("s3教学部");
            Department dept2 = EmployeesInfoManage_Entity.GetDeptByDname("s4教学部");

            List<EmployeesInfo> Emp_List = EmployeesInfoManage_Entity.GetEmpsByDeptid(dept.DeptId);
            Emp_List.AddRange(EmployeesInfoManage_Entity.GetEmpsByDeptid(dept1.DeptId));
            Emp_List.AddRange(EmployeesInfoManage_Entity.GetEmpsByDeptid(dept2.DeptId));

            string sql = "select * from ManageClassTime where ClassTimeState =1";
            List<ManageClassTime> ClassTime_List = ClassTime_Entity.GetListBySql<ManageClassTime>(sql);

            foreach (var item in ClassTime_List)
            {
                foreach (var it in Emp_List)
                {
                    if (item.Emp_ID == it.EmployeeId)
                    {
                        Emp_List.Remove(it);
                        break;
                    }
                }
            }
            return Emp_List.Count();
        }

        /// <summary>
        /// 设置未设置底课时的教学老师
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddClassTime_Emp()
        {
            Department dept = EmployeesInfoManage_Entity.GetDeptByDname("s1、s2教学部");
            Department dept1 = EmployeesInfoManage_Entity.GetDeptByDname("s3教学部");
            Department dept2 = EmployeesInfoManage_Entity.GetDeptByDname("s4教学部");

            List<EmployeesInfo> Emp_List = EmployeesInfoManage_Entity.GetEmpsByDeptid(dept.DeptId);
            Emp_List.AddRange(EmployeesInfoManage_Entity.GetEmpsByDeptid(dept1.DeptId));
            Emp_List.AddRange(EmployeesInfoManage_Entity.GetEmpsByDeptid(dept2.DeptId));

            string sql = "select * from ManageClassTime where ClassTimeState =1";
            List<ManageClassTime> ClassTime_List = ClassTime_Entity.GetListBySql<ManageClassTime>(sql);

            foreach (var item in ClassTime_List)
            {
                foreach (var it in Emp_List)
                {
                    if (item.Emp_ID == it.EmployeeId)
                    {
                        Emp_List.Remove(it);
                        break;
                    }
                }
            }

            List<ManageClassTime> time = new List<ManageClassTime>();
            for (int i = 0; i < Emp_List.Count; i++)
            {
                ManageClassTime classtime = new ManageClassTime();
                classtime.ID = Guid.NewGuid().ToString();

                classtime.ClassTime = 50;
                classtime.Emp_ID = Emp_List[i].EmployeeId;
                classtime.ClassTimeState = 1;
                time.Add(classtime);
            }
            ClassTime_Entity.Insert(time);

            return Json(Emp_List.Count(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量修改底课时方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateClassTime(string[] str)
        {
            int classtimecount = Convert.ToInt16(Request.Form["ClassTime"]);
            List<ManageClassTime> class_List = new List<ManageClassTime>();
            for (int i = 0; i < str.Length; i++)
            {
                ManageClassTime classtime = ClassTime_Entity.GetEntity(str[i]);
                classtime.ClassTime = classtimecount;
                class_List.Add(classtime);
            }
            if (str.Length == class_List.Count())
            {
                ClassTime_Entity.Update(class_List);
            }

            return Json(class_List.Count(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateSingleClassTime()
        {
            string ID = Request.Form["ID"];
            int classcount = Convert.ToInt32(Request.Form["classTime"]);
            ManageClassTime classtime = ClassTime_Entity.GetEntity(ID);
            classtime.ClassTime = classcount;
            ClassTime_Entity.Update(classtime);
            return Json(JsonRequestBehavior.AllowGet);
        }

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

        public ActionResult Search_PersonClass()
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

                    var data = db_staf_Cost.Staff_CostData(item.EmployeeId, DateTime.Parse(date), workingDays);

                    var obj = db_staf_Cost.Statistics_Cost(data);

                    result.Add(obj);

                    detaillist.Add(data);
                }

                string Detailfilename = DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month + "费用统计明细表";

                db_staf_Cost.SaveStaff_CostData(detaillist, result, Detailfilename);
                //保存到文件 
                string filename = DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month + "费用统计表";

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
        public ActionResult DownloadCostStatics(string date = null, string Dfilename = null)
        {
            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            string filename = Dfilename == null ? DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month + "费用统计表.xls" : Dfilename;

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

        [HttpGet]
        public ActionResult PersonalCostStatics1(string empid, string date)
        {


            AjaxResult result = new AjaxResult();
            DateTime dt = Convert.ToDateTime(date.Substring(0, 4) + "-" + date.Substring(5, 2));

            //int WorkDay = 0;
            //if (IsDanxiu == "0")
            //{
            //    WorkDay = WorkDaysOfyearmonth(dt.Year, dt.Month, true);
            //}
            //else
            //{
            //    WorkDay = WorkDaysOfyearmonth(dt.Year, dt.Month, false);
            //}

            //节假日天数 >0  用工作日天数减去节假日天数
            //if (jiejiari > 0)
            //{
            //    WorkDay = WorkDay - jiejiari;
            //}
            string sql = "select * from EmployeesInfo where EmployeeId = " + empid + "";
            List<EmployeesInfo> Emp_List = EmployeesInfoManage_Entity.GetListBySql<EmployeesInfo>(sql);
            List<Staff_CostView> staff_list = db_staf_Cost.CostTimeFee(Emp_List, dt);
            var data = new
            {
                code = 0,
                count = staff_list.Count,
                data = staff_list
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 历史纪录
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoryCost()
        {
            return View();
        }

        public ActionResult UpExcel_Bos(int deptid, string date)
        {
            AjaxResult ajaxResult = new AjaxResult();
            List<Staff_CostView> list = SessionHelper.Session["Cost_Emp_list"] as List<Staff_CostView>;


            if (list != null)
            {
                var workbook = new HSSFWorkbook();
                var sheet = workbook.CreateSheet("课时费统计");

                #region 表头样式

                HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

                HeadercellStyle.Alignment = HorizontalAlignment.Center;
                HeadercellFont.IsBold = true;

                HeadercellStyle.SetFont(HeadercellFont);

                #endregion

                #region 内容样式
                HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
                HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

                ContentcellStyle.Alignment = HorizontalAlignment.Center;
                #endregion

                #region 创建表头

                HSSFRow Header = (HSSFRow)sheet.CreateRow(0);
                Header.HeightInPoints = 40;

                CreateCell(Header, HeadercellStyle, 0, "姓名");
                CreateCell(Header, HeadercellStyle, 1, "职务");
                CreateCell(Header, HeadercellStyle, 2, "课时费");
                CreateCell(Header, HeadercellStyle, 3, "值班费");
                CreateCell(Header, HeadercellStyle, 4, "监考费");
                CreateCell(Header, HeadercellStyle, 5, "阅卷费");
                CreateCell(Header, HeadercellStyle, 6, "超带班");
                CreateCell(Header, HeadercellStyle, 7, "内训费");
                CreateCell(Header, HeadercellStyle, 8, "研发费");
                CreateCell(Header, HeadercellStyle, 9, "合计");
                #endregion

                int num = 1;
                list.ForEach(d =>
                {
                    var row = (HSSFRow)sheet.CreateRow(num);

                    CreateCell(row, ContentcellStyle, 0, d.Emp_Name);
                    CreateCell(row, ContentcellStyle, 1, d.RoleName);
                    CreateCell(row, ContentcellStyle, 2, d.Cost_fee.ToString());
                    CreateCell(row, ContentcellStyle, 3, d.Duty_fee.ToString());
                    CreateCell(row, ContentcellStyle, 4, d.Invigilation_fee.ToString());
                    CreateCell(row, ContentcellStyle, 5, d.Marking_fee.ToString());
                    CreateCell(row, ContentcellStyle, 6, d.Super_class.ToString());
                    CreateCell(row, ContentcellStyle, 7, d.Internal_training_fee.ToString());
                    CreateCell(row, ContentcellStyle, 8, d.RD_fee.ToString());
                    CreateCell(row, ContentcellStyle, 9, d.totalmoney.ToString());
                    num++;

                });

                void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
                {
                    HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                    Header_Name.SetCellValue(value);

                    Header_Name.CellStyle = TcellStyle;
                }
                Department department = null;
                string filename = null;
                if (deptid != 0)
                {
                    department = Department_Entity.GetEntity(deptid);
                    filename = DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month + department.DeptName + "课时费统计表";
                }
                else
                {
                    filename = DateTime.Parse(date).Year + "-" + DateTime.Parse(date).Month + "课时费统计表";
                }

                string LOACpathName = System.Web.HttpContext.Current.Server.MapPath("/Areas/Educational/CostHistoryFiles/" + filename + ".xls");

                string pathName = $"/CostHistoryFiles/{filename}.xls";

                FileStream stream = new FileStream(LOACpathName, FileMode.Create, FileAccess.ReadWrite);

                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();

                workbook.Write(stream);

                stream.Close();

                var FILEINFO = new FileInfo(LOACpathName);

                client.PutObject("xinxihua", pathName, FILEINFO);

                workbook.Close();

                ajaxResult.ErrorCode = 200;
                ajaxResult.Msg = "上传成功！";

            }
            else
            {
                ajaxResult.ErrorCode = 404;
                ajaxResult.Msg = "网络异常";
            }
            return Json(ajaxResult, JsonRequestBehavior.AllowGet);

        }

        public ActionResult HistoryCostFileData(int page, int limit)
        {

            //List<FileInfo> list = db_staf_Cost.HistoryCostFileData().OrderBy(d=>d.LastWriteTime).ToList();

            //List<FileInfo> skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();

            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            var list = client.ListObjects("xinxihua", "CostHistoryFiles").Contents.OrderByDescending(d => d.LastModified).ToList();

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

            var obj = new
            {

                code = 0,
                msg = "",
                count = list.Count,
                data = dataObj
            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Emp_Cost_Statististics(string data)
        {
            return View();
        }

        /// <summary>
        /// 计算课时费
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="DeptID">部门</param>
        /// <param name="IsDanxiu">单休 --> 0   双休-->1  </param>
        /// <param name="jiejiari">节假日天数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TempFunction(string date, int[] DeptID)
        {
            DateTime dt = Convert.ToDateTime(date.Substring(0, 4) + "-" + date.Substring(5, 2));

            //int WorkDay = 0;
            //if (IsDanxiu == "0")
            //{
            //    WorkDay = WorkDaysOfyearmonth(dt.Year, dt.Month, true);
            //}
            //else
            //{
            //    WorkDay = WorkDaysOfyearmonth(dt.Year, dt.Month, false);
            //}

            ////节假日天数 >0  用工作日天数减去节假日天数
            //if (jiejiari > 0)
            //{
            //    WorkDay = WorkDay - jiejiari;
            //}

            List<EmployeesInfo> Emp_List = new List<EmployeesInfo>();

            for (int i = 0; i < DeptID.Length; i++)
            {
                Emp_List.AddRange(EmployeesInfoManage_Entity.GetEmpsByDeptid(DeptID[i]));
            }

            List<Staff_CostView> staff_list = db_staf_Cost.CostTimeFee(Emp_List, dt);

            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.Msg = "统计完成";
            SessionHelper.Session["Cost_Emp_list"] = staff_list;

            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 课时费统计    写入Excel
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CostDataToExcel()
        {

            List<Staff_CostView> list = SessionHelper.Session["Cost_Emp_list"] as List<Staff_CostView>;

            var ajaxresult = new { data = list };
            return Json(ajaxresult, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 课时费统计页面
        /// </summary>
        /// <returns></returns>
        public ActionResult teaching_hour()
        {
            var deps = db_staf_Cost.GetDepartments();

            ViewBag.deps = deps;
            return View();
        }

        /// <summary>
        /// 计算当月工作日天数
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="IsDanxiu">是否为单休  true 是   false 否</param>
        /// <returns></returns>
        private int WorkDaysOfyearmonth(int year, int month, bool IsDanxiu)
        {
            int alldays = DateTime.DaysInMonth(year, month);

            int workday = alldays;
            DateTime indata;
            for (int i = 1; i <= alldays; i++)
            {
                if (IsDanxiu)
                {
                    indata = Convert.ToDateTime(year.ToString() + "/" + month.ToString() + "/" + i.ToString());
                    if (indata.DayOfWeek == DayOfWeek.Sunday)
                    {
                        workday--;
                    }
                }
                else
                {
                    indata = Convert.ToDateTime(year.ToString() + "/" + month.ToString() + "/" + i.ToString());
                    if (indata.DayOfWeek == DayOfWeek.Sunday || indata.DayOfWeek == DayOfWeek.Saturday)
                    {
                        workday--;
                    }
                }


            }
            return workday;

        }
    }
}