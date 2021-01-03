﻿using System;
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

        public ActionResult Search_PersonClass(string empid)
        {
            ViewBag.EmpID = empid;
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

                    try
                    {
                        var data = db_staf_Cost.Staff_CostData(item.EmployeeId, DateTime.Parse(date), workingDays);

                        var obj = db_staf_Cost.Statistics_Cost(data);

                        result.Add(obj);

                        detaillist.Add(data);
                    }
                    catch (Exception ex)
                    {
                    }

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
        public ActionResult TempFunction(string date, int DeptID, string IsDanxiu,int jiejiari)
        {
            string datetime = date.Substring(0, 4);
            DateTime dt = Convert.ToDateTime(date.Substring(0, 4) + "-" + date.Substring(5, 2));

            int WorkDay = 0;
            if (IsDanxiu == "0")
            {
                WorkDay = WorkDaysOfyearmonth(dt.Year, dt.Month, true);
            }
            else {
                WorkDay = WorkDaysOfyearmonth(dt.Year, dt.Month, false);
            }

            //节假日天数 >0  用工作日天数减去节假日天数
            if (jiejiari >0) {
                WorkDay = WorkDay - jiejiari;
            }
            
            
            List<EmployeesInfo> Emp_List = null;
            if (DeptID == 0)
            {
                Emp_List = EmployeesInfoManage_Entity.GetEmpByDeptName();
            }
            else
            {
                Emp_List = EmployeesInfoManage_Entity.GetEmpsByDeptid(DeptID);
            }

            List<Staff_CostView> staff_list = new List<Staff_CostView>();

            decimal? Cost_fee = 0;
            int QuanDay = 0;//全天课天数
            int ClassTime = 50;//底课时
            int Duty_fee = 0;//值班费
            int Invigilation_fee = 0;//监考费
            int Marking_fee = 0;//阅卷费
            int Super_class = 0;//超带班
            int Internal_training_fee = 0;//内训费
            int RD_fee = 0;//研发费

            for (int i = 0; i < Emp_List.Count; i++)
            {
                Staff_CostView staff = new Staff_CostView();

                DateTime NowTime = DateTime.Now;

                if ((NowTime.Year - Emp_List[i].EntryTime.Year)>=2) {
                    ClassTime = 40;
                }

                
                string sqlstr = $"select * from Reconcile  where Year(AnPaiDate)={dt.Year} and Month(AnPaiDate) = {dt.Month} and EmployeesInfo_Id ={ Emp_List[i].EmployeeId }";
                List<Reconcile> mydata = Reconcile_Entity.GetListBySql<Reconcile>(sqlstr).ToList();

                //筛选出前预科的数据
                var qianyuke = mydata.Where(a => a.Curriculum_Id == "前预科").ToList();
                //根据时间分组
                var AnPaiGroup = (
                    from m in mydata
                    group m by m.AnPaiDate into list
                    select list
                    ).ToList();

                //根据课程分组
                var ClassGroup1 = (
                    from m in mydata
                    group m by m.Curriculum_Id into list
                    select list).ToList();

                //根据班级id分组   作用：前预科：根据班级iD看班级有多少人
                var ClassScheduleGroup = (
                    from m in qianyuke
                    group m by m.ClassSchedule_Id into list
                    select list).ToList();

                //班级id分组   带班数量
                var ClassScheduleCount = (
                    from m in mydata
                    group m by m.ClassSchedule_Id into list
                    select list).ToList();

                
                var ClassGroup = ClassGroup1.Where
                    (a => a.Key != "复习" ).ToList();//&& a.Key != "项目答辩"&& !a.Key.Contains("职素") && !a.Key.Contains("班")
                        

                //计算全天课天数&& a.Curriculum_Id != "项目答辩"
                for (int k = 0; k < AnPaiGroup.Count; k++)
                {
                    QuanDay += Cost_EndClass(AnPaiGroup[k].Key, Emp_List[i].EmployeeId);
                }
                //ClassTime = 40 * QuanDay / WorkDay;//底课时   40*全天课天数/工作日天数

                if (EmployeesInfoManage_Entity.GetPositionByEmpid(Emp_List[i].EmployeeId).PositionName == "教学主任")
                {
                    ClassTime = ClassTime * QuanDay / WorkDay;
                }

                if (!EmployeesInfoManage_Entity.GetDeptByEmpid(Emp_List[i].EmployeeId).DeptName.Contains("s1、s2教学部")) {
                    ClassTime = 0;
                }

                int FirstStage = 0;//第一阶段  预科，S1,S2
                int SecondStage = 0;//第二阶段 S3,S4
                int OtherStage = 0;//其他  语，数，英，职素，班会，军事
                for (int j = 0; j < ClassGroup.Count; j++)
                {
                    //判断是否为“前预科”
                    if (ClassGroup[j].Key == "前预科")
                    {
                        for (int q = 0; q < ClassScheduleGroup.Count; q++)
                        {
                            ClassSchedule schedule = ClassSchedule_Entity.GetEntity(ClassScheduleGroup[q].Key);
                            string sql = $"select * from ScheduleForTrainees where ClassID='{schedule.ClassNumber}' and CurrentClass=1";
                            List<ScheduleForTrainees> Trainees_List = ScheduleForTrainees_Entity.GetListBySql<ScheduleForTrainees>(sql);
                            if (Trainees_List.Count < 10)
                            {
                                FirstStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, "前预科", false);
                            }
                            else
                            {
                                FirstStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, "前预科", true);
                            }
                        }
                    }
                    else
                    {
                        //根据课程名称获取第一条数据 && && a.Curriculum_Id != ""
                        Reconcile reconcile = mydata.Where(a => a.Curriculum_Id == ClassGroup[j].Key).FirstOrDefault();
                        //根据班级id查询单条数据
                        ClassSchedule classSchedule = ClassSchedule_Entity.GetEntity(reconcile.ClassSchedule_Id);
                        //根据课程名称以及阶段id筛选
                        Curriculum curriculum = curriculum_Entity.GetList().FirstOrDefault(a => a.CourseName == reconcile.Curriculum_Id && a.Grand_Id == classSchedule.grade_Id);
                        //去除语文课之类的
                        Curriculum curriculum1 = curriculum_Entity.GetList()
                            .Where(a => !a.CourseName.Contains("语文") &&
                            !a.CourseName.Contains("数学") &&
                            !a.CourseName.Contains("英语") &&
                            !a.CourseName.Contains("职素") &&
                            !a.CourseName.Contains("班会") &&
                            !a.CourseName.Contains("军事"))
                            .FirstOrDefault(a => a.CourseName == reconcile.Curriculum_Id && a.Grand_Id == classSchedule.grade_Id);

                        if (curriculum.CourseName.Contains("语文") ||
                            curriculum.CourseName.Contains("数学") ||
                            curriculum.CourseName.Contains("英语") ||
                            curriculum.CourseName.Contains("班会") ||
                            curriculum.CourseName.Contains("军事"))
                        {
                            OtherStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                        }
                        else if (curriculum.CourseName.Contains("职素"))
                        {

                            int zhisuClassTime = Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                            int totalClassTime = (zhisuClassTime / 4) + zhisuClassTime;
                            OtherStage += zhisuClassTime;
                        }
                        else
                        {
                            Grand grand = Grand_Entity.GetEntity(curriculum1.Grand_Id);
                            if (grand.GrandName.Contains("S1") || grand.GrandName.Contains("S2") || grand.GrandName.Contains("Y1"))
                            {
                                FirstStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                            }
                            else if (grand.GrandName.Contains("S3") || grand.GrandName.Contains("S4"))
                            {
                                SecondStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                            }
                        }
                    }

                }
                //课时费计算总额
                if (FirstStage > 0)
                {
                    Cost_fee += FirstStage * 55;
                }

                if (SecondStage>0)
                {
                    Cost_fee += SecondStage * 65;
                }

                if (OtherStage>0)
                {
                    Cost_fee += OtherStage * 30;
                }

                
                //计算值班费
                //查询当年 年月的值班数据
                string TeacherAddsql = @"select * from TeacherAddorBeonDutyView  where YEAR(Anpaidate)=" + dt.Year + "" +
                    " and Month(Anpaidate)=" + dt.Month + "";
                List<TeacherAddorBeonDutyView> TearcherAdd_List = teacherAddorBeonDutyView_Entity.GetListBySql<TeacherAddorBeonDutyView>(TeacherAddsql);
                //根据时间分组
                var TeacherAddGroup = (
                    from m in TearcherAdd_List
                    group m by m.Anpaidate into list
                    select list).ToList();

                //一节  50    两节   80
                for (int g = 0; g < TeacherAddGroup.Count; g++)
                {
                    string sql = @"select * from TeacherAddorBeonDutyView  where YEAR(Anpaidate)=" + TeacherAddGroup[g].Key.Year + "" +
                    " and Month(Anpaidate)=" + TeacherAddGroup[g].Key.Month + " and Day(Anpaidate)="+ TeacherAddGroup[g].Key.Day + " and Tearcher_Id="+Emp_List[i].EmployeeId+"";
                    List<TeacherAddorBeonDutyView> TearcherAdd = teacherAddorBeonDutyView_Entity.GetListBySql<TeacherAddorBeonDutyView>(sql);
                    if (TearcherAdd.Count == 1) {
                        Duty_fee += 50;
                    } else if (TearcherAdd.Count==2) {
                        Duty_fee += 80;
                    }
                }

                //计算监考费
                int InvigilationCount = 0;
                string ExaminationRoomSql = "select * from ExaminationRoom where Invigilator1 = '"+Emp_List[i].EmployeeId+ "' or Invigilator2='" + Emp_List[i].EmployeeId + "'";
                List<ExaminationRoom> ExamRoomList = ExaminationRoom_Entity.GetListBySql<ExaminationRoom>(ExaminationRoomSql);
                for (int t = 0; t < ExamRoomList.Count; t++)
                {
                    string ExamDateSql = "select * from Examination where year(BeginDate) = '" + dt.Year + "' and Month(BeginDate)='" + dt.Month + "' and ID ="+ExamRoomList[i].Examination+"";
                    
                    if (Examination_Entity.GetListBySql<Examination>(ExamDateSql).FirstOrDefault()!=null) {
                        InvigilationCount += 1;
                    }
                }
                Invigilation_fee += InvigilationCount * 20;

                //计算阅卷费
                int StuCount = 0;
                string MarkingSql = "select * from MarkingArrange where MarkingTeacher="+Emp_List[i].EmployeeId+"";
                List<MarkingArrange> MarkingList = Marking_Entity.GetListBySql<MarkingArrange>(MarkingSql);
                for (int k = 0; k < MarkingList.Count; k++)
                {
                    string ExaminationSql = "select * from Examination where year(BeginDate) = '"+dt.Year+"' and Month(BeginDate)='"+dt.Month+"' and ID ="+ MarkingList[k].ExamID + "";
                    List<Examination> ExamList = Examination_Entity.GetListBySql<Examination>(ExaminationSql);
                    if (ExamList.Count > 0) {
                        string CandiSql = "select * from CandidateInfo where Examination ="+ExamList[0].ID+"";
                        List<CandidateInfo> CandiList = Candi_Entity.GetListBySql<CandidateInfo>(CandiSql);
                        StuCount += CandiList.Count;
                    }
                }
                //读取配置文件
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/Cost.xml"));

                //根节点
                var xmlRoot = xmlDocument.DocumentElement;

                //获取标准费用
                var cost = xmlRoot.GetElementsByTagName("MarkingCost")[0].InnerText;
                Marking_fee = int.Parse(cost) * StuCount;

                staff.totalmoney =Convert.ToInt32(Cost_fee) + Duty_fee + Invigilation_fee + Marking_fee + Super_class + Internal_training_fee + RD_fee;
                staff.Cost_fee = Cost_fee;
                staff.Duty_fee = Duty_fee;
                staff.Emp_Name = Emp_List[i].EmpName; 
                staff.RoleName = EmployeesInfoManage_Entity.GetPositionByEmpid(Emp_List[i].EmployeeId).PositionName;
                staff_list.Add(staff);

                Cost_fee = 0;
                QuanDay = 0;
                ClassTime = 50;
                Duty_fee = 0;//值班费
                Invigilation_fee = 0;//监考费
                Marking_fee = 0;//阅卷费
                Super_class = 0;//超带班
                Internal_training_fee = 0;//内训费
                RD_fee = 0;//研发费

            }

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
        /// 计算全天课天数
        /// </summary>
        /// <returns></returns>
        public int Cost_EndClass(DateTime time, string EmpID)
        {
            string sql = $"select * from Reconcile where YEAR(AnPaiDate)='{time.Year}' and MONTH(AnPaiDate)='{time.Month}' and Day(AnPaiDate)='{time.Day}' and EmployeesInfo_Id='{EmpID}'";
            List<Reconcile> mydata = Reconcile_Entity.GetListBySql<Reconcile>(sql).ToList();
            int count = 0;//一个月上的天数

            if (mydata.Count == 2)
            {
                int mycount = 0;
                for (int i = 0; i < mydata.Count; i++)
                {
                    if (mydata[0].Curse_Id == "上午" || mydata[0].Curse_Id == "下午")
                    {
                        mycount++;
                    }
                }
                if (mycount == mydata.Count)
                {
                    count++;
                }
            }
            else if (mydata.Count == 4)
            {
                int mycount = 0;
                for (int i = 0; i < mydata.Count; i++)
                {
                    if (mydata[i].Curse_Id.Contains("12"))
                    {
                        mycount += 1;
                    }
                    else if (mydata[i].Curse_Id.Contains("34"))
                    {
                        mycount += 1;
                    }
                }

                if (mycount == mydata.Count)
                {
                    count++;
                }
            }
            else if (mydata.Count == 3)
            {
                int mycount = 0;
                for (int i = 0; i < mydata.Count; i++)
                {
                    if (mydata[i].Curse_Id.Contains("12"))
                    {
                        mycount += 1;
                    }
                    else if (mydata[i].Curse_Id.Contains("34"))
                    {
                        mycount += 1;
                    }
                    else if (mydata[i].Curse_Id.Contains("上午"))
                    {
                        mycount += 1;
                    }
                    else if (mydata[i].Curse_Id.Contains("下午"))
                    {
                        mycount += 1;
                    }
                }
                if (mycount == mydata.Count)
                {
                    count++;
                }
            }


            return count;
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
                else {
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