using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Teaching.Controllers
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Util;
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Entity.Base_SysManage;
    using SiliconValley.InformationSystem.Business;
    
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.ClassesBusiness;
    using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
    using System.Xml;
    using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using SiliconValley.InformationSystem.Business.StudentBusiness;

    /// <summary>
    /// 满意度调查控制器
    /// </summary>
    /// 
    [CheckLogin]
    public class SatisfactionSurveyController : Controller
    {

        BaseBusiness<SatisficingResultDetail> db_satisresultdetail = new BaseBusiness<SatisficingResultDetail>();
        // GET: Teaching/SatisfactionSurvey
        BaseBusiness<Department> db_dep = new BaseBusiness<Department>();

        EmployeesInfoManage db_emp = new EmployeesInfoManage();

        BaseBusiness<Headmaster> db_headmaster = new BaseBusiness<Headmaster>();

        Base_UserMapRoeBusinessL db_userrole = new Base_UserMapRoeBusinessL();

        private readonly SatisfactionSurveyBusiness db_survey;

        TeacherClassBusiness db_teacherclass = new TeacherClassBusiness();

        BaseBusiness<ClassSchedule> db_class = new BaseBusiness<ClassSchedule>();
        //学员班级
        ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();

        public EmployeesInfoManage EmployeesInfoManage_Entity = new EmployeesInfoManage();

        private readonly TeacherBusiness db_teacher;
        private readonly CourseBusiness db_course;

        public SatisfactionSurveyController()
        {
            db_survey = new SatisfactionSurveyBusiness();


            db_course = new CourseBusiness();
            db_teacher = new TeacherBusiness();
        }
        public ActionResult SatisfactionIndex()
        {
            var permisslist = PermissionManage.GetOperatorPermissionValues();

            ViewBag.Permisslist = permisslist;

            return View();
        }
        /// <summary>
        /// 食堂导出页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CanteenExportView()
        {
            return View();
        }
        /// <summary>
        /// 我的满意度学生查看
        /// </summary>
        /// <returns></returns>
        public ActionResult MySatisfactionCheck(int surveyResultID)
        {
            //提供 JoinSurveyStudents

            var survey = db_survey.AllsatisficingResults().Where(d => d.ID == surveyResultID).FirstOrDefault();

            var studentlist = db_survey.JoinSurveyStudents(surveyResultID);

            ViewBag.SurveyConfigId = surveyResultID;

            ViewBag.studentlist = studentlist;

            return View();
        }
        /// <summary>
        /// 食堂导出方法
        /// </summary>
        /// <returns></returns>
        public ActionResult CanteenExcel(string date)
        {
            var ajaxresult = new AjaxResult();
            DateTime dt = DateTime.Parse(date);
            string yys = dt.Year.ToString();
            string mms = dt.Month.ToString();
            MemoryStream bookStream = new MemoryStream();
            var workbook = new HSSFWorkbook();
            //表名
            string Detailfilename = "食堂满意度调查表.xls";
            //创建工作区
            var sheet = workbook.CreateSheet();

            #region 表头样式

            HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

            HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            HeadercellFont.IsBold = true;

            HeadercellStyle.SetFont(HeadercellFont);

            #endregion


            HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

            ContentcellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            CreateHeader();

            int num = 1;
            StudentInformationBusiness studet = new StudentInformationBusiness();
            //string sql = "select * from CandidateInfo where Examination ='" + Examid + "'";
            //var list = db_candidate.GetListBySql<CandidateInfo>(sql).ToList();

            var xinxi = db_survey.satisficingConfigs().Where(d => d.Isitacanteen == true && d.CreateTime.Value.Year.ToString() == yys && d.CreateTime.Value.Month.ToString() == mms).ToList();
            xinxi.ForEach(d =>
            {
                var row = (HSSFRow)sheet.CreateRow(num);
                //string sqles = "select * from StudentInformation where StudentNumber = '" + d.StudentID + "'";
                //var name = studet.GetListBySql<StudentInformation>(sqles).FirstOrDefault().Name;
                var Schedule = db_class.GetList().Where(a => a.id == d.ClassNumber).FirstOrDefault().ClassNumber;
                var staresultlist = db_survey.AllsatisficingResults().Where(c => c.SatisficingConfig == d.ID).ToList();
                var total = 0;
                var fenshu = 0;
                staresultlist.ForEach(c =>
                {
                    var templist = db_satisresultdetail.GetList().Where(b => b.SatisficingBill == c.ID).ToList();

                    templist.ForEach(x =>
                    {
                        total += (int)x.Scores;
                    });
                });

                if (staresultlist.Count() == 0)
                {
                    fenshu = total / 1;
                }
                else
                {
                    fenshu = total / staresultlist.Count();
                }
                CreateCell(row, ContentcellStyle, 0, Schedule);//班级名字
                CreateCell(row, ContentcellStyle, 1, "食堂");//调查对象
                CreateCell(row, ContentcellStyle, 2, total.ToString());//总分
                CreateCell(row, ContentcellStyle, 3, fenshu.ToString());//每个班的平均分

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
                ajaxresult.Msg = "导入失败，" + ex.Message;

            }
            return File(bookStream, "application / vnd.ms - excel", Detailfilename);

            void CreateHeader()
            {
                HSSFRow Header = (HSSFRow)sheet.CreateRow(0);
                Header.HeightInPoints = 40;
                CreateCell(Header, HeadercellStyle, 0, "班级");
                CreateCell(Header, HeadercellStyle, 1, "调查对象");
                CreateCell(Header, HeadercellStyle, 2, "总分");
                CreateCell(Header, HeadercellStyle, 3, "平均分");
            }

            void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
            {
                HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                Header_Name.SetCellValue(value);

                Header_Name.CellStyle = TcellStyle;
            }
        }
        /// <summary>
        /// 满意度导出页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SatisfactionDerivationView()
        {
            return View();
        }
        /// <summary>
        /// 满意度导出方法
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherExport(string date)
        {
            var ajaxresult = new AjaxResult();
            DateTime dt = DateTime.Parse(date);
            string yys = dt.Year.ToString();
            string mms = dt.Month.ToString();
            MemoryStream bookStream = new MemoryStream();
            var workbook = new HSSFWorkbook();
            //表名
            string Detailfilename = "班主任教员满意度调查表.xls";
            //创建工作区
            var sheet = workbook.CreateSheet();

            #region 表头样式

            HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

            HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
            HeadercellFont.IsBold = true;

            HeadercellStyle.SetFont(HeadercellFont);

            #endregion


            HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

            ContentcellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;

            CreateHeader();

            int num = 1;
            StudentInformationBusiness studet = new StudentInformationBusiness();
            var xinxi = db_survey.satisficingConfigs().Where(d => d.Isitacanteen == false && d.CreateTime.Value.Year.ToString() == yys && d.CreateTime.Value.Month.ToString() == mms).ToList();
            xinxi.ForEach(d =>
            {
                var row = (HSSFRow)sheet.CreateRow(num);
                var Schedule = db_class.GetList().Where(a => a.id == d.ClassNumber).FirstOrDefault();
                var staresultlist = db_survey.AllsatisficingResults().Where(c => c.SatisficingConfig == d.ID).ToList();
                //通过班级获取班主任
                HeadmasterBusiness business = new HeadmasterBusiness();
                var banzhuren = business.ClassHeadmaster(Schedule.id);
                //获取教员的课程
                var kecheng = db_course.GetList().Where(m => m.CurriculumID == d.CurriculumID).FirstOrDefault();
                 //获取教员名称
                 var jiaoyuanName = db_emp.GetList().Where(l =>l.EmployeeId == d.EmployeeId).FirstOrDefault();
                var total = 0;
                var fenshu = 0;
                staresultlist.ForEach(c =>
                {
                    var templist = db_satisresultdetail.GetList().Where(b => b.SatisficingBill == c.ID).ToList();

                    templist.ForEach(x =>
                    {
                        total += (int)x.Scores;
                    });
                });

                if (staresultlist.Count() == 0)
                {
                    fenshu = total / 1;
                }
                else
                {
                    fenshu = total / staresultlist.Count();
                }
                CreateCell(row, ContentcellStyle, 0, Schedule.ClassNumber);//班级名字
                CreateCell(row, ContentcellStyle, 1, jiaoyuanName.EmpName);//专业老师
                CreateCell(row, ContentcellStyle, 2, kecheng==null?"空": kecheng.CourseName);//专业课程
                CreateCell(row, ContentcellStyle, 3, fenshu.ToString());//总分
                CreateCell(row, ContentcellStyle, 4, fenshu.ToString());//平均分
                //CreateCell(row, ContentcellStyle, 5, banzhuren.EmpName);//班主任
                //CreateCell(row, ContentcellStyle, 6, fenshu.ToString());//总分
                //CreateCell(row, ContentcellStyle, 7, fenshu.ToString());//平均分
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
                ajaxresult.Msg = "导入失败，" + ex.Message;

            }
            return File(bookStream, "application / vnd.ms - excel", Detailfilename);

            void CreateHeader()
            {
                HSSFRow Header = (HSSFRow)sheet.CreateRow(0);
                Header.HeightInPoints = 40;
                CreateCell(Header, HeadercellStyle, 0, "班级");
                CreateCell(Header, HeadercellStyle, 1, "任课老师及班主任");
                CreateCell(Header, HeadercellStyle, 2, "专业课程");
                CreateCell(Header, HeadercellStyle, 3, "总分");
                CreateCell(Header, HeadercellStyle, 4, "平均分");
                //CreateCell(Header, HeadercellStyle, 5, "班主任");
                //CreateCell(Header, HeadercellStyle, 6, "总分");
                //CreateCell(Header, HeadercellStyle, 7, "平均分");
            }

            void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
            {
                HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                Header_Name.SetCellValue(value);

                Header_Name.CellStyle = TcellStyle;
            }
        }
        /// <summary>
        /// 满意度注意事项页面
        /// </summary>
        public ActionResult SatisfactionHomePage()
        {
            return View();
        }
        /// <summary>
        /// 语文老师满意度问卷页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ChineseQuestionnaire(int surveyId)
        {
            var su = db_survey.satisficingConfigs().Where(d => d.ID == surveyId).FirstOrDefault();

            var view = db_survey.ConvertToview(su);

            ViewBag.SurveyConfig = view;
            return View();
        }
        /// <summary>
        /// 数学老师满意度问卷页面
        /// </summary>
        /// <returns></returns>
        public ActionResult MathematicsQuestionnaire(int surveyId)
        {
            var su = db_survey.satisficingConfigs().Where(d => d.ID == surveyId).FirstOrDefault();

            var view = db_survey.ConvertToview(su);

            ViewBag.SurveyConfig = view;
            return View();
        }
        /// <summary>
        /// 英语老师满意度问卷页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EnglishQuestionnaire(int surveyId)
        {
            var su = db_survey.satisficingConfigs().Where(d => d.ID == surveyId).FirstOrDefault();

            var view = db_survey.ConvertToview(su);

            ViewBag.SurveyConfig = view;
            return View();
        }
        /// <summary>
        /// 满意度调查配置文件
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfigSetting()
        {

            return View();
        }

        public ActionResult satisfactionItemSettingView()
        {


            //获取所有满意度调查对象
            ViewBag.Department = db_survey.AllSatisfactionSurveyObject();

            return View();
        }

        public ActionResult satisfactionItemTypeSettingView()
        {

            //获取所有满意度调查对象
            ViewBag.Department = db_survey.AllSatisfactionSurveyObject();



            return View();

        }


        /// <summary>
        /// 获取调查具体项
        /// </summary>
        /// <param name="DepID">部门ID</param>
        /// <param name="itemTypeid">类型ID</param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetSurveyItemData(int DepID, int itemTypeid, int page, int limit)
        {


            List<SatisfactionSurveyView> resultlist = new List<SatisfactionSurveyView>();

            try
            {
                var list = db_survey.Screen(DepID, itemTypeid);

                foreach (var item in list)
                {

                    var viewobj = db_survey.ConvertModelView(item);

                    if (viewobj != null)
                    {
                        resultlist.Add(viewobj);
                    }
                }

            }
            catch (Exception ex)
            {
                Base_UserBusiness.WriteSysLog("查询数据出错了 位置 ：满意度调查GetSurveyData ", EnumType.LogType.加载数据);
            }

            int count = resultlist.Count;

            var obj = new {

                code = 0,
                msg = "",
                count = count,
                data = resultlist.Skip((page - 1) * limit).Take(limit).ToList()

            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// 获取调查项类型数据
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult GetSurveyItemTypeData(string typename, int depid)
        {
            AjaxResult result = new AjaxResult();

            List<SatisficingType> resultlist = new List<SatisficingType>();

            try
            {
                resultlist = db_survey.Screen(typename, depid);

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = resultlist;

            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = ex.Message;
                result.Data = resultlist;
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 添加调查项类型
        /// </summary>
        /// <param name="typename">类名称</param>
        /// <param name="depid">部门</param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult AddSurveyItemType(string typename, int depid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                db_survey.AddSurveyItemType(typename, depid);

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = ex.Message;
                result.Data = null;
            }



            return Json(result, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult GetGetSurveyItemTypeDataByDepid(int depid)
        {
            AjaxResult result = new AjaxResult();

            List<SatisficingType> resultlist = new List<SatisficingType>();

            try
            {

                resultlist = db_survey.Screen(null, depid);

                result.ErrorCode = 200;
                result.Data = resultlist;
                result.Msg = "成功";

            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Data = resultlist;
                result.Msg = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加满意度调查具体项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddSurveyItem()
        {
            //获取所有满意度调查对象


            ViewBag.Dep = db_survey.AllSatisfactionSurveyObject();
          
            return View();
        }

        /// <summary>
        /// 添加满意度调查具体项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSurveyItem(SatisficingItem satisficingItem)
        {
            AjaxResult result = new AjaxResult();

            satisficingItem.IsDel = false;

            try
            {
                db_survey.Insert(satisficingItem);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Data = null;
                result.Msg = ex.Message;
            }


            return Json(result);

        }


        /// <summary>
        /// 调查项内容列表视图
        /// </summary>
        /// <param name="itemtypeid">类型id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SurveylistView(int itemtypeid)
        {

            //提供具体调查项

            ViewBag.itemlist = db_survey.GetAllSatisfactionItems().Where(d => d.ItemType == itemtypeid).ToList();


            return View();

        }


        /// <summary>
        /// 删除调查项类型
        /// </summary>
        /// <returns></returns>
        public ActionResult DelItemType(int typeid)
        {

            AjaxResult result = new AjaxResult();


            try
            {
                db_survey.RemoveItemType(typeid);

                result.Data = null;
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
        /// 删除调查具体项
        /// </summary>
        /// <param name="itemid">具体项ID</param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult delSurveyItem(int itemid)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                var delobj = db_survey.GetAllSatisfactionItems().Where(d => d.ItemID == itemid).FirstOrDefault();

                db_survey.Delete(delobj);

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = ex.Message;
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 满意度调查记录视图
        /// </summary>
        /// <returns></returns>
        public ActionResult SurveyHistoryView()
        {
            //判断登录的角色

            Base_UserModel user = Base_UserBusiness.GetCurrentUser();

            var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault();

            //获取员工的岗位

            var teacherview = db_teacher.GetTeacherView(teacher.TeacherID);

            //获取员工权限

            ViewBag.TeacherView = teacherview;


            var permisslist = PermissionManage.GetOperatorPermissionValues();

            ViewBag.Permisslist = permisslist;



            return View();


        }
        /// <summary>
        /// 获取历史食堂满意度
        /// </summary>
        /// <returns></returns>
        public ActionResult CanteenSelect(int limit,int page)
        {
            var xinxi = db_survey.satisficingConfigs().OrderByDescending(t => t.CreateTime).Where(d => d.Isitacanteen == true).ToList();
            List<SatisficingConfig> skiplist = xinxi.Skip((page - 1) * limit).Take(limit).ToList();
            List<SatisficingConfigDataView> resultlist = new List<SatisficingConfigDataView>();
            foreach (var item in skiplist)
            {
                var tempobj = db_survey.ConvertToSatisficingConfigDataView(item);

                resultlist.Add(tempobj);
            }
            var obj = new
            {

                code = 0,
                msg = "",
                count = xinxi.Count,
                data = resultlist


            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取食堂满意度查询
        /// </summary>
        /// <returns></returns>
        public ActionResult CanteenInquiry(int limit, int page)
        {
            var xinxi = db_survey.satisficingConfigs().OrderByDescending(t => t.CreateTime).Where(d =>   d.Isitacanteen == true).ToList();
            List<SatisficingConfig> skiplist = xinxi.Skip((page - 1) * limit).Take(limit).ToList();
            List<SatisficingConfigDataView> resultlist = new List<SatisficingConfigDataView>();
            foreach (var item in skiplist)
            {
                var tempobj = db_survey.ConvertToSatisficingConfigDataView(item);

                resultlist.Add(tempobj);
            }
            var obj = new
            {

                code = 0,
                msg = "",
                count = xinxi.Count,
                data = resultlist


            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取我的满意度查询
        /// </summary>
        /// <returns></returns>
        public ActionResult Mysatisfaction(int limit, int page)
        {
            //获取当前账号
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            var xinxi = db_survey.satisficingConfigs().OrderByDescending(t=>t.CreateTime).Where(d => d.EmployeeId == user.EmpNumber).ToList();
            List<SatisficingConfig> skiplist = xinxi.Skip((page - 1) * limit).Take(limit).ToList();
            List<SatisficingConfigDataView> resultlist = new List<SatisficingConfigDataView>();
            foreach (var item in skiplist)
            {
                var tempobj = db_survey.ConvertToSatisficingConfigDataView(item);

                resultlist.Add(tempobj);
            }
            var obj = new
            {

                code = 0,
                msg = "",
                count = xinxi.Count,
                data = resultlist


            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取员工的满意度调查记录
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>

        public ActionResult SurveyHistoryData(int limit, int page)
        {
            //Base_UserModel user = Base_UserBusiness.GetCurrentUser();

            //获取这些员工所在的部门
            EmployeesInfoManage yees = new EmployeesInfoManage();
            //List<EmployeesInfo> emplist = db_survey.GetMyDepEmp(user);
            string sql = "select * from EmployeesInfo";
            var emplist = yees.GetListBySql<EmployeesInfo>(sql).ToList();
            var configtempList = db_survey.satisficingConfigs();

            var configList = new List<SatisficingConfig>();

            foreach (var item in emplist)
            {
               var templist = configtempList.Where(d=>d.EmployeeId == item.EmployeeId).ToList();

                if (templist != null)
                {
                    configList.AddRange(templist);
                }
            }

            var skiplist = configList.OrderByDescending(d=>d.CreateTime).Skip((page - 1) * limit).Take(limit).ToList();

            List<SatisficingConfigDataView> detaillist = new List<SatisficingConfigDataView>();


            skiplist.ForEach(d=>
            {
               var temobj = db_survey.ConvertToSatisficingConfigDataView(d);
                if (temobj != null) detaillist.Add(temobj);
            });

            var obj = new {
                code=0,
                msg="",
                count = configList.Count,
                data = detaillist

            };
            return Json(obj, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 获取满意度调查记录
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        public ActionResult SurveyHistoryDataByID(int id)
        {
            AjaxResult result = new AjaxResult();



            try
            {
                //***获取对象的岗位在决定获取那个部门的数据

                var obj1 = db_survey.GetSatisficingResultByID(id);

                var obj2 = db_survey.ConvertToViewModel(obj1);


                EmployeesInfoManage empmanage = new EmployeesInfoManage();
                var dep = empmanage.GetDept(obj2.Emp.PositionId);

                //获取了部门的调查类型

                var templist = db_survey.Screen(null, dep.DeptId);

                var list1 = new List<string>();

                //组装数据
                foreach (var item in templist)
                {

                    list1.Add(item.TypeName);

                }


                List<object> list2 = new List<object>();
                var deatilitemlist = obj2.detailitem;

                foreach (var item in templist)
                {

                    var templist4 = deatilitemlist.Where(d => d.SatisficingItem.ItemID == item.ID).ToList();

                    var score = 0;

                    foreach (var item1 in templist4)
                    {
                        score += (int)item1.Scores;
                    }


                    // 调查类型和分数的对象

                    var itemtypesocreobj = new
                    {

                        value = score,
                        name = item.TypeName

                    };

                    list2.Add(itemtypesocreobj);



                }


                //最后返回的对象
                var obj = new
                {

                    itemTypelist = list1,
                    Data = obj2,
                    itemTypeScores = list2
                };



                result.ErrorCode = 200;
                result.Data = obj;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);



        }
        /// <summary>
        /// 对食堂满意度进行帅选
        /// </summary>
        /// <param name="empnumber"></param>
        /// <param name="date"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult CanteenSelection(string empnumber, string date, int limit, int page)
        {
            var list = db_survey.SurveyData_filters(empnumber, date);

            var skiplist = list.Skip((page - 1) * limit).Take(limit);

            List<SatisfactionSurveyDetailView> resultlist = new List<SatisfactionSurveyDetailView>();

            foreach (var item in skiplist)
            {
                var tempobj = db_survey.AllsatisficingResults().Where(d => d.SatisficingConfig == item.ID).FirstOrDefault();

                if (tempobj != null)
                {
                    var detail = db_survey.ConvertToViewModel(tempobj);

                    if (detail != null)
                        resultlist.Add(detail);
                }
            }

            var obj = new
            {
                code = 0,
                msg = "",
                count = list.Count,
                data = resultlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 对历史满意度调查记录进行查询
        /// </summary>
        /// <param name="empnumber"></param>
        /// <param name="date"></param>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult SurveyDatafilters(string empnumber, string date, int limit, int page)
        {
            var list = db_survey.SurveyDatafilters(empnumber, date);

            var skiplist = list.Skip((page - 1) * limit).Take(limit);

            List<SatisfactionSurveyDetailView> resultlist = new List<SatisfactionSurveyDetailView>();

            foreach (var item in skiplist)
            {
                var tempobj = db_survey.AllsatisficingResults().Where(d => d.SatisficingConfig == item.ID).FirstOrDefault();

                if (tempobj != null)
                {
                    var detail = db_survey.ConvertToViewModel(tempobj);

                    if (detail != null)
                        resultlist.Add(detail);
                }
            }

            var obj = new
            {
                code = 0,
                msg = "",
                count = list.Count,
                data = resultlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 对满意度调查记录进行帅选 
        /// </summary>
        /// <returns></returns>
        public ActionResult SurveyData_filter(string empnumber, string date, int limit, int page)
        {
           
            var list = db_survey.SurveyData_filter(empnumber, date);

            var skiplist = list.Skip((page - 1) * limit).Take(limit);

            List<SatisfactionSurveyDetailView> resultlist = new List<SatisfactionSurveyDetailView>();

            foreach (var item in skiplist)
            {
               var tempobj = db_survey.AllsatisficingResults().Where(d => d.SatisficingConfig == item.ID).FirstOrDefault();

                if (tempobj != null)
                {
                   var detail = db_survey.ConvertToViewModel(tempobj);

                    if (detail != null)
                        resultlist.Add(detail);
                }
            }

            var obj = new {
                code = 0,
                msg="",
                count=list.Count,
                data= resultlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取班级
        /// </summary>
        /// <returns></returns>
        /// 
        public ActionResult GetClassNumber(int page, int limit)
        {

            List<ClassTableView> resultlist = new List<ClassTableView>();
            List<ClassTableView> returnlist = new List<ClassTableView>();
            //当前用户
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault();
            //角色列表
            var list = db_userrole.CurrentUserRoles();

            //教学校长角色roleID
            var roel1 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2d");

            //S1S2教学主任roleid
            var role2 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2e");

            //S1S2教学副主任roleid
            var role3 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2f");

            //S3教学主任Roleid
            var role4 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2g");
            //S3教学副主任roleid 
            var role5 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2h");

            //S4教学主任
            var role6 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2i");

            //S4教学副主任        
            var role7 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2j");

            //班主任
            var role8 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2l");

            //教员
            var role9 = db_userrole.GetSysRoleAtConfig("039e4630ba5a1-31320968-d1dd-4275-ae71-a06da0731a2j");

            foreach (var item in list)
            {

                //教学校长
                if (item.RoleId == roel1.RoleId)
                {

                    //所有班级列表
                    var classlist = db_class.GetList().Where(d => d.IsDelete == false).ToList();

                    foreach (var item1 in classlist)
                    {

                        //转换类型
                        var tempobj = db_teacherclass.GetClassTableView(item1);
                        resultlist.Add(tempobj);

                    }
                }

                //S1S2教学主任 S1S2教学副主任
                if (item.RoleId == role2.RoleId || item.RoleId == role3.RoleId)
                {
                    //获取S1S2班级

                    //所有班级列表
                    var classlist = db_class.GetList().Where(d => d.IsDelete == false).ToList();

                    //筛选出S1S2的班级

                    foreach (var item1 in classlist)
                    {
                         
                        if (item1.grade_Id == 1 || item1.grade_Id == 2)
                        {

                            //转换类型                          
                            var tempobj = db_teacherclass.GetClassTableView(item1);
                            resultlist.Add(tempobj);
                        }

                    }
                }


                //S3教学主任 S3教学副主任
                if (item.RoleId == role4.RoleId || item.RoleId == role5.RoleId)
                {
                    //获取S3班级

                    //所有班级列表
                    var classlist = db_class.GetList().Where(d => d.IsDelete == false).ToList();

                    //筛选出S3的班级

                    foreach (var item1 in classlist)
                    {

                        if (item1.grade_Id == 3)
                        {

                            //转换类型
                            var tempobj = db_teacherclass.GetClassTableView(item1);
                            resultlist.Add(tempobj);
                        }

                    }

                }

                //S4教学主任 S4教学副主任

                if (item.RoleId == role6.RoleId || item.RoleId == role7.RoleId)
                {

                    //获取S4的班级

                    //所有班级列表
                    var classlist = db_class.GetList().Where(d => d.IsDelete == false).ToList();

                    //筛选出S3的班级

                    foreach (var item1 in classlist)
                    {

                        if (item1.grade_Id == 4)
                        {

                            //转换类型
                            var tempobj = db_teacherclass.GetClassTableView(item1);
                            resultlist.Add(tempobj);
                        }

                    }




                }


                //教员
                if (item.RoleId == role9.RoleId)
                {
                    //获取自己的班级


                    var templist = db_teacherclass.GetCrrentMyClass(teacher.TeacherID);

                    foreach (var item1 in templist)
                    {
                        //转换类型
                        var tempobj = db_teacherclass.GetClassTableView(item1);
                        resultlist.Add(tempobj);
                    }
                }

                //班主任
                if (item.RoleId == role8.RoleId)
                {
                    //未完成
                }


            }

            //去掉重复的班级

            foreach (var item in resultlist)
            {
                if (!IsContainClass(returnlist, item))
                {
                    returnlist.Add(item);
                }

            }

            var obj = new {

                code = 0,
                msg = "",
                count = returnlist.Count,
                data = returnlist.Skip((page - 1) * limit).Take(limit).ToList()
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 判断班级列表里面是否存在了一个班级
        /// </summary>
        /// <returns></returns>
        public bool IsContainClass(List<ClassTableView> sorces, ClassTableView classtableview)
        {

            foreach (var item in sorces)
            {
                if (item.ClassNumber == classtableview.ClassNumber)
                {
                    return true;
                }
            }

            return false;

        }


        public ActionResult selectClassView()
        {
            return View();
        }

        /// <summary>
        /// 获取
        /// 
        /// 班级
        /// </summary>
        /// <returns></returns>
        public ActionResult GetClassNumberByEmp(string EmpID, int limit, int page)
        {

            List<ClassTableView> resultlist = new List<ClassTableView>();

            //判断员工是哪个部门的

           var emp = db_emp.GetList().Where(d => d.EmployeeId == EmpID).FirstOrDefault();

            EmployeesInfoManage empmanage = new EmployeesInfoManage();

            var dep = empmanage.GetDept(emp.PositionId);


            //如果是教质部
            if (dep.DeptName.Contains("教质部"))
            {

                var headmaster = db_headmaster.GetList().Where(d => d.informatiees_Id == emp.EmployeeId).FirstOrDefault();

                BaseBusiness<HeadClass> db_masterclass = new BaseBusiness<HeadClass>();

               var templist = db_masterclass.GetList().Where(d => d.LeaderID == headmaster.ID).ToList();

                var classtemplist = new List<ClassSchedule>();

                foreach (var item in templist)
                {
                  
                   var tempobj = db_class.GetList().Where(d => d.ClassNumber ==   classScheduleBusiness.GetEntity(item.ClassID).ClassNumber).FirstOrDefault();

                    resultlist .Add( db_teacherclass.GetClassTableView(tempobj));
                }

            }

            //如果是教学部

            if (dep.DeptName.Contains("教学部"))
            {

                //获取班级

                var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == emp.EmployeeId).FirstOrDefault();

                var templist  = db_teacherclass.GetCrrentMyClass(teacher.TeacherID);

                foreach (var item in templist)
                {
                    resultlist.Add( db_teacherclass.GetClassTableView(item));
                }

            }


            var obj = new {

                code=0,
                msg="",
                count=resultlist.Count,
                data=resultlist.Skip((page-1)*limit).Take(limit).ToList()
            };

            return Json(obj, JsonRequestBehavior.AllowGet);



        }



        /// <summary>
        /// 满意度调查详细视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SurveyDetail(int id)
        {
            var obj = db_survey.GetSatisficingResultByID(id);

           var resultobj = db_survey.ConvertToViewModel(obj);

            ViewBag.SurveyDetail = resultobj;

            return View();
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="EmpID"></param>
        /// <param name="ClassNumber"></param>
        /// <returns></returns>
        public ActionResult GetPieImageData(string EmpID, string ClassNumber, int courseid, string date)
        {

            //首先判断员工部门
            var emp = db_emp.GetList().Where(d => d.EmployeeId == EmpID).FirstOrDefault();

            EmployeesInfoManage empmanage = new EmployeesInfoManage();


           var sss =  db_survey.satisficingConfigs().Where(d=>d.ClassNumber == int.Parse(ClassNumber) && d.CurriculumID==courseid).ToList();

           var bbb = db_survey.SatisficingResults().Where(d => d.SatisficingConfig == sss.FirstOrDefault().ID).ToList();

            var ccc = db_survey.ConvertToViewModel(bbb.FirstOrDefault());

            var dep = empmanage.GetDept(emp.PositionId);


            var list = db_survey.SurveyHistoryData(EmpID, date, courseid, ClassNumber);

            //下面进行组装数据

            //获取调查类型

            List<SatisficingType> temptypelist = new List<SatisficingType>();

            List<string> typelist = new List<string>();


            temptypelist = temptypelist = db_survey.GetSatisficingTypes().Where(d => d.DepartmentID == dep.DeptId).ToList();

            foreach (var item in temptypelist)
            {
                typelist.Add(item.TypeName);
            }


            List<PieServiceHelper> tyepscorelist = new List<PieServiceHelper>();

            //创建帮助类
            foreach (var item in temptypelist)
            {
                PieServiceHelper pieServiceHelper = new PieServiceHelper();
                pieServiceHelper.id = item.ID;
                pieServiceHelper.name = item.TypeName;
                pieServiceHelper.value = 0;

                tyepscorelist.Add(pieServiceHelper);
            }


            List<object> itemscoreObj = new List<object>();

            foreach (var item in list)
            {

                foreach (var item1 in item.detailitem)
                {

                    tyepscorelist.Where(d => d.id == item1.SatisficingItem.ItemType).FirstOrDefault().value += (int)item1.Scores;

                }

            }



            var obj = new {

                typelist = typelist,

                tyepscorelist = tyepscorelist,

                Data= ccc
            };



            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取教员在班级教过的课程
        /// </summary>
        /// <param name="empid">员工ID/param>
        /// <param name="classnumber">班级编号 </param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCursor(string empid, int classnumber)
        {


            AjaxResult result = new AjaxResult();
            List<Curriculum> list = new List<Curriculum>();
            CourseBusiness db_course = new CourseBusiness();
            List<Curriculum> Resultlist = new List<Curriculum>();
            try
            {
                //获取教员
                var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == empid).FirstOrDefault();

                //排课业务类
                BaseBusiness<Reconcile> db_reconile = new BaseBusiness<Reconcile>();

                //排课集合
                var templist = db_reconile.GetList().Where(d => d.ClassSchedule_Id == classnumber).ToList();

              

              

                foreach (var item in templist)
                {
                    list.Add(db_course.GetCurriculas().Where(d => d.CourseName == item.Curse_Id &&d.IsDelete==false).FirstOrDefault());
                }

                //去掉重复项

                foreach (var item in list)
                {
                    if (!db_course.isContain(Resultlist, item))
                    {
                        Resultlist.Add(item);
                    }
                }

                result.ErrorCode = 200;
                result.Data = Resultlist;
                result.Msg = "成功";

            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Data = Resultlist;
                result.Msg = ex.Message;

            }


            return Json(result,JsonRequestBehavior.AllowGet);


        }



        /// <summary>
        /// 学生满意度主页面
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult StudentSurveyIndex()
        {
            return View();
        }
        /// <summary>
        /// 食堂满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult CanteenSatisfaction(int surveyId)
        {
            var su = db_survey.satisficingConfigs().Where(d => d.ID == surveyId).FirstOrDefault();

            var view = db_survey.ConvertToview(su);

            ViewBag.SurveyConfig = view;
            return View();
        }

        /// <summary>
        /// 班主任满意度调查表
        /// </summary>
        /// <returns></returns>
        public ActionResult HeadMasterSatisfactionQuestionnaire(int surveyId)
        {
             
            var su = db_survey.satisficingConfigs().Where(d => d.ID == surveyId).FirstOrDefault();

            var view = db_survey.ConvertToview(su);

            ViewBag.SurveyConfig = view;

            return View();

        }


        /// <summary>
        /// 获取班主任的调查问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSurveyQuectionHeadMaster()
        {
            AjaxResult result = new AjaxResult();

            List<SatisficingItem> resultlist = new List<SatisficingItem>();

            try
            {
               resultlist = db_survey.Screen(2, 0);

                result.Data = resultlist;
                result.ErrorCode = 200;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {

                result.Data = resultlist;
                result.ErrorCode = 500;
                result.Msg =ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);


        }
        
        /// <summary>
        /// 获取食堂的调查问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSurveyQuectionCanteen()
        {
            AjaxResult result = new AjaxResult();

            List<SatisficingItem> resultlist = new List<SatisficingItem>();

            try
            {
                resultlist = db_survey.Screen(3, 0);

                result.Data = resultlist;
                result.ErrorCode = 200;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {

                result.Data = resultlist;
                result.ErrorCode = 500;
                result.Msg = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取教员的调查问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSurveyQuectionTeacher()
        {
            AjaxResult result = new AjaxResult();

            List<SatisficingItem> resultlist = new List<SatisficingItem>();

            try
            {
                resultlist = db_survey.Screen(1, 0);
                result.Data = resultlist;
                result.ErrorCode = 200;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {

                result.Data = resultlist;
                result.ErrorCode = 500;
                result.Msg = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);


        }
        /// <summary>
        /// 提交食堂满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult CanteenSubmission(List<SurveyCommitHelper> list, int configId, string suggest)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();
                //获取当前登陆学生的学号
                var studentnumber = Request.Cookies["StudentNumber"].Value.ToString();
                //1. 添加结果表  2.添加详细表 
                SatisficingResult Surveyresult = new SatisficingResult();
                Surveyresult.Answerer = studentnumber;

                var date = DateTime.Now;

                Surveyresult.CreateDate = date;
                Surveyresult.IsDel = false;
                Surveyresult.SatisficingConfig = configId;
                Surveyresult.Suggest = suggest;

                db_survey.insertSatisfactionResult(Surveyresult);

                BaseBusiness<SatisficingResult> dd = new BaseBusiness<SatisficingResult>();

                var suResult = dd.GetList().Where(d => d.CreateDate.Value.ToString() == date.ToString()).FirstOrDefault();

                //2 添加详细

                List<SatisficingResultDetail> insertlIST = new List<SatisficingResultDetail>();

                foreach (var item in list)
                {
                    SatisficingResultDetail detail = new SatisficingResultDetail();
                    detail.Remark = "";
                    detail.SatisficingBill = suResult.ID;
                    detail.SatisficingItem = item.contentId;
                    detail.Scores = item.scores;

                    insertlIST.Add(detail);

                }

                BaseBusiness<SatisficingResultDetail> bas = new BaseBusiness<SatisficingResultDetail>();

                bas.Insert(insertlIST);


                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;

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
        /// 提交语文满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult LanguageSubmission(List<SurveyCommitHelper> list, int configId, string suggest)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();
                //获取当前登陆学生的学号
                var studentnumber = Request.Cookies["StudentNumber"].Value.ToString();
                //1. 添加结果表  2.添加详细表 
                SatisficingResult Surveyresult = new SatisficingResult();
                Surveyresult.Answerer = studentnumber;

                var date = DateTime.Now;

                Surveyresult.CreateDate = date;
                Surveyresult.IsDel = false;
                Surveyresult.SatisficingConfig = configId;
                Surveyresult.Suggest = suggest;

                db_survey.insertSatisfactionResult(Surveyresult);

                BaseBusiness<SatisficingResult> dd = new BaseBusiness<SatisficingResult>();

                var suResult = dd.GetList().Where(d => d.CreateDate.Value.ToString() == date.ToString()).FirstOrDefault();

                //2 添加详细

                List<SatisficingResultDetail> insertlIST = new List<SatisficingResultDetail>();

                foreach (var item in list)
                {
                    SatisficingResultDetail detail = new SatisficingResultDetail();
                    detail.Remark = "";
                    detail.SatisficingBill = suResult.ID;
                    detail.SatisficingItem = item.contentId;
                    detail.Scores = item.scores;

                    insertlIST.Add(detail);

                }

                BaseBusiness<SatisficingResultDetail> bas = new BaseBusiness<SatisficingResultDetail>();

                bas.Insert(insertlIST);

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;

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
        /// 提交数学满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult MathematicsSubmission(List<SurveyCommitHelper> list, int configId, string suggest)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();
                //获取当前登陆学生的学号
                var studentnumber = Request.Cookies["StudentNumber"].Value.ToString();
                //1. 添加结果表  2.添加详细表 
                SatisficingResult Surveyresult = new SatisficingResult();
                Surveyresult.Answerer = studentnumber;

                var date = DateTime.Now;

                Surveyresult.CreateDate = date;
                Surveyresult.IsDel = false;
                Surveyresult.SatisficingConfig = configId;
                Surveyresult.Suggest = suggest;

                db_survey.insertSatisfactionResult(Surveyresult);

                BaseBusiness<SatisficingResult> dd = new BaseBusiness<SatisficingResult>();

                var suResult = dd.GetList().Where(d => d.CreateDate.Value.ToString() == date.ToString()).FirstOrDefault();

                //2 添加详细

                List<SatisficingResultDetail> insertlIST = new List<SatisficingResultDetail>();

                foreach (var item in list)
                {
                    SatisficingResultDetail detail = new SatisficingResultDetail();
                    detail.Remark = "";
                    detail.SatisficingBill = suResult.ID;
                    detail.SatisficingItem = item.contentId;
                    detail.Scores = item.scores;

                    insertlIST.Add(detail);

                }

                BaseBusiness<SatisficingResultDetail> bas = new BaseBusiness<SatisficingResultDetail>();

                bas.Insert(insertlIST);


                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;

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
        /// 提交英语满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult EnglishSubmission(List<SurveyCommitHelper> list, int configId, string suggest)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();
                //获取当前登陆学生的学号
                var studentnumber = Request.Cookies["StudentNumber"].Value.ToString();
                //1. 添加结果表  2.添加详细表 
                SatisficingResult Surveyresult = new SatisficingResult();
                Surveyresult.Answerer = studentnumber;

                var date = DateTime.Now;

                Surveyresult.CreateDate = date;
                Surveyresult.IsDel = false;
                Surveyresult.SatisficingConfig = configId;
                Surveyresult.Suggest = suggest;

                db_survey.insertSatisfactionResult(Surveyresult);

                BaseBusiness<SatisficingResult> dd = new BaseBusiness<SatisficingResult>();

                var suResult = dd.GetList().Where(d => d.CreateDate.Value.ToString() == date.ToString()).FirstOrDefault();

                //2 添加详细

                List<SatisficingResultDetail> insertlIST = new List<SatisficingResultDetail>();

                foreach (var item in list)
                {
                    SatisficingResultDetail detail = new SatisficingResultDetail();
                    detail.Remark = "";
                    detail.SatisficingBill = suResult.ID;
                    detail.SatisficingItem = item.contentId;
                    detail.Scores = item.scores;

                    insertlIST.Add(detail);

                }

                BaseBusiness<SatisficingResultDetail> bas = new BaseBusiness<SatisficingResultDetail>();

                bas.Insert(insertlIST);


                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;

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
        ///  提交班主任调查问卷
        /// </summary>
        /// <param name="surveyCommit">项分数对象</param>
        /// <param name="headmaster">班主任</param>
        /// <param name="sug">建议</param>
        /// <returns></returns>
        public ActionResult SurveyQuectionCommitHeadMaster(List<SurveyCommitView> surveyCommit, int configId,string suggest)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();
                //获取当前登陆学生的学号
                var studentnumber = Request.Cookies["StudentNumber"].Value.ToString();
                //1. 添加结果表  2.添加详细表 
                SatisficingResult Surveyresult = new SatisficingResult();
                Surveyresult.Answerer = studentnumber;

                var date = DateTime.Now;

                Surveyresult.CreateDate = date;
                Surveyresult.IsDel = false;
                Surveyresult.SatisficingConfig = configId;
                Surveyresult.Suggest = suggest;

                db_survey.insertSatisfactionResult(Surveyresult);

                BaseBusiness<SatisficingResult> dd = new BaseBusiness<SatisficingResult>();

                var suResult = dd.GetList().Where(d => d.CreateDate.Value.ToString() == date.ToString()).FirstOrDefault();

                //2 添加详细

                List<SatisficingResultDetail> insertlIST = new List<SatisficingResultDetail>();

                foreach (var item in surveyCommit)
                {
                    SatisficingResultDetail detail = new SatisficingResultDetail();
                    detail.Remark = "";
                    detail.SatisficingBill = suResult.ID;
                    detail.SatisficingItem = item.SurveyItemId;
                    detail.Scores = item.Score;

                    insertlIST.Add(detail);

                }

                BaseBusiness<SatisficingResultDetail> bas = new BaseBusiness<SatisficingResultDetail>();

                bas.Insert(insertlIST);


                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;

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
        /// 提交教员满意度调查问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult commitTeacherSurvey(List<SurveyCommitHelper> list, string suggest,int configId)
        {
            AjaxResult result = new AjaxResult();

            try
            {

                //1. 添加结果表  2.添加详细表 
                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();
                //获取当前登陆学生的学号
                var studentnumber = Request.Cookies["StudentNumber"].Value.ToString();
                SatisficingResult Surveyresult = new SatisficingResult();
                Surveyresult.Answerer = studentnumber;

                var date = DateTime.Now;

                Surveyresult.CreateDate = date;
                Surveyresult.IsDel = false;
                Surveyresult.SatisficingConfig = configId;
                Surveyresult.Suggest = suggest;

                db_survey.insertSatisfactionResult(Surveyresult);

                BaseBusiness<SatisficingResult> dd = new BaseBusiness<SatisficingResult>();

                var suResult = dd.GetList().Where(d => d.CreateDate.Value.ToString() == date.ToString()).FirstOrDefault();

                //2 添加详细

                List<SatisficingResultDetail> insertlIST = new List<SatisficingResultDetail>();

                foreach (var item in list)
                {
                    SatisficingResultDetail detail = new SatisficingResultDetail();
                    detail.Remark = "";
                    detail.SatisficingBill = suResult.ID;
                    detail.SatisficingItem = item.contentId;
                    detail.Scores = item.scores;

                    insertlIST.Add(detail);

                }

                BaseBusiness<SatisficingResultDetail> bas = new BaseBusiness<SatisficingResultDetail>();

                bas.Insert(insertlIST);


                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;

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
        /// 创建 班主任满意度调查问卷总单
        /// </summary>
        /// <returns></returns>
        /// 

        [HttpGet]
        public ActionResult CreateHeadMasterSurveyConfig()
        {

            //获取班级

            var classlist = db_teacherclass.AllClassSchedule().Where(d => d.IsDelete == false).ToList().Where(d => d.ClassstatusID == null).ToList();


            ViewBag.classlist = classlist;

            return View();

        }
        /// <summary>
        /// 生成班主任的满意度调查
        /// </summary>
        /// <param name="classnumber"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult CreateHeadMasterSurveyConfig(int classnumber)
        {

               AjaxResult result = new AjaxResult();
                BaseBusiness<HeadClass> db_headclass = new BaseBusiness<HeadClass>();
                var headclass  = db_headclass.GetList().Where(d => d.IsDelete == false && d.ClassID == classnumber).FirstOrDefault();
               var master = db_headmaster.GetList().Where(d => d.ID == headclass.LeaderID).FirstOrDefault();
               var user = db_emp.GetInfoByEmpID(master.informatiees_Id);
                 var date = DateTime.Now;
                var templist =  db_survey.satisficingConfigs().Where(d => d.EmployeeId == user.EmployeeId && DateTime.Parse(d.CreateTime.ToString()).Year == date.Year && DateTime.Parse(d.CreateTime.ToString()).Month == date.Month && d.ClassNumber == classnumber).ToList();
                if (templist.Count != 0)
                {
                    result.ErrorCode = 600;
                    result.Data = null;
                    result.Msg = "本月班主任的满意度问卷已存在";

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                SatisficingConfig satisficingConfig = new SatisficingConfig();
                satisficingConfig.ClassNumber = classnumber;
                satisficingConfig.CreateTime = DateTime.Now;
                satisficingConfig.CurriculumID = null;
                satisficingConfig.EmployeeId = user.EmployeeId;
                satisficingConfig.IsDel = false;
                satisficingConfig.IsPastDue = false;
                satisficingConfig.Isitacanteen = false;
                satisficingConfig.Isitayuwen = false;
                satisficingConfig.isitashuxue = false;
                satisficingConfig.isitayingyu = false;
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/empmanageConfig.xml"));
                var xmlRoot = xmlDocument.DocumentElement;
                var defaultTime = ((XmlElement)xmlRoot.GetElementsByTagName("defaultCutOffDate")[0]).Attributes["value"].Value;
                satisficingConfig.CutoffDate = DateTime.Now.AddHours(int.Parse(defaultTime));
                db_survey.AddSatisficingConfig(satisficingConfig);
                return Json(result,JsonRequestBehavior.AllowGet);
        }




        /// <summary>
        /// 教员满意度表单视图
        /// </summary>
        /// <param name="surveyId">满意度单ID</param>
        /// <returns></returns>
        public ActionResult TeacherSatisfactionQuestionnaire(int surveyId)
        {    
            var su = db_survey.satisficingConfigs().Where(d => d.ID == surveyId).FirstOrDefault();

            var view = db_survey.ConvertToview(su);
            ViewBag.SurveyConfig = view;
            return View();
        }
        /// <summary>
        /// 通过班级获取高中班还是初中班
        /// </summary>
        /// <returns></returns>
        public ActionResult HuoQulaoshi(int banjiid)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                ClassScheduleBusiness classs = new ClassScheduleBusiness();
                //获取班级
                var classlist = classs.GetList().Where(d => d.IsDelete == false && d.id == banjiid).FirstOrDefault().ClassNumber;
                
                result.ErrorCode = 200;
                result.Data = classlist.Substring(0,2);
                result.Msg = "成功";
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Data = null;
                result.Msg = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 生成教员满意度调查问卷
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreateTeacherSurveyConfig()
        {
            //提供难度级别数据
            GrandBusiness grandBusiness = new GrandBusiness();
            //获取班级
            var classlist = db_teacherclass.AllClassSchedule().Where(d => d.IsDelete == false).ToList().Where(d => d.ClassstatusID == null).ToList();
            //获取部门
            var getdepartments = db_dep.GetList().Where(s => s.DeptName.Contains("教学")).ToList();
            //获取课程
            ViewBag.Courselist = db_course.GetCurriculas();
            //提供阶段
            ViewBag.Grand = grandBusiness.AllGrand();
            ViewBag.getdepartments = getdepartments;
            ViewBag.classlist = classlist;
            return View();
        }
        /// <summary>
        /// 根据部门获取员工
        /// </summary>
        /// <param name="bumeng"></param>
        /// <returns></returns>
        public ActionResult DepartmentTeacher(int bumeng)
        {
            AjaxResult result = new AjaxResult();
            List<EmployeesInfo> db_info = null;
            try
            {
                db_info = EmployeesInfoManage_Entity.GetEmpsByDeptid(bumeng);

                result.ErrorCode = 200;
                result.Data = db_info;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Data = db_info;
                result.Msg = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取教员在班级上过的课程      
        /// </summary>
        /// <param name="classnumber"></param>
        /// <returns></returns>
        public ActionResult GetCorsueOnReconile(string classnumber)
        {


            AjaxResult result = new AjaxResult();
            List<Curriculum> resultlist = new List<Curriculum>();
            try
            {
                //排课业务类
                BaseBusiness<Reconcile> db_reconile = new BaseBusiness<Reconcile>();

                CourseBusiness db_course = new CourseBusiness();


                Base_UserModel user = Base_UserBusiness.GetCurrentUser();

                var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault();

                //排课筛选之后的数据
                var templist = db_reconile.GetList().Where(d => d.IsDelete == false && d.ClassSchedule_Id == int.Parse(classnumber)).ToList();

                List<Curriculum> list = new List<Curriculum>();

                foreach (var item in templist)
                {
                    var tempobj = db_course.GetList().Where(d => d.CourseName == item.Curriculum_Id).FirstOrDefault();

                    if(tempobj!=null)
                        list.Add(tempobj);


                }

                //去掉重复项

                foreach (var item in list)
                {
                    if (!db_course.isContain(resultlist, item))
                    {
                        resultlist.Add(item);
                    }
                }

                result.ErrorCode = 200;
                result.Data = resultlist;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Data = resultlist;
                result.Msg = ex.Message;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
            


        }
        /// <summary>
        /// 生成教员，班主任，食堂 语数英老师的满意度问卷
        /// </summary>
        /// <param name="classnumber"></param>
        /// <param name="Curriculum"></param>
        /// <param name="laoshi"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult CreateTeacherSurveyConfig(int classnumber, int Curriculum,string laoshi,string yuwen,string shuxue,string yingyu)
        {

            AjaxResult result = new AjaxResult();
            try
            {
                Base_UserModel user = Base_UserBusiness.GetCurrentUser();
                BaseBusiness<ClassSchedule> classdb = new BaseBusiness<ClassSchedule>();
                //查出评价班级
                var classlisttemp = classdb.GetList().Where(d => d.IsDelete == false && d.id == classnumber).FirstOrDefault().ClassNumber;
                var templist = db_survey.satisficingConfigs().Where(d => d.IsDel == false && d.EmployeeId == laoshi && d.ClassNumber == classnumber).ToList();
                 if (templist.Count !=0)
                {
                    //已经存在
                    result.ErrorCode = 300;
                    result.Data = null;
                    result.Msg = "满意度本月已存在";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                SatisficingConfig satisficingConfig = new SatisficingConfig();
                satisficingConfig.ClassNumber = classnumber;
                satisficingConfig.CreateTime = DateTime.Now;
                satisficingConfig.CurriculumID = Curriculum;
                satisficingConfig.EmployeeId = laoshi;
                satisficingConfig.IsDel = false;
                satisficingConfig.IsPastDue = false;
                satisficingConfig.Isitacanteen = false;
                satisficingConfig.Isitayuwen = false;
                satisficingConfig.isitashuxue = false;
                satisficingConfig.isitayingyu = false;
                //设置截止时间 默认截止日期
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/empmanageConfig.xml"));
                var xmlRoot = xmlDocument.DocumentElement;
                var defaultTime =( (XmlElement)xmlRoot.GetElementsByTagName("defaultCutOffDate")[0]).Attributes["value"].Value ;
                satisficingConfig.CutoffDate = DateTime.Now.AddHours(int.Parse(defaultTime));
                db_survey.AddSatisficingConfig(satisficingConfig);
                //生成班主任的满意度调查问卷
                CreateHeadMasterSurveyConfig(classnumber);
                //生成食堂满意度问卷
                CanteenQuestionnaire(classnumber);
                //生成语文老师满意度问卷
                //生成数学老师满意度问卷
                //生成英语老师满意度问卷
                if (yuwen != null && shuxue != null && yingyu != null)
                {
                    ChineseSatisfaction(classnumber,yuwen);
                    MathematicsSatisfaction(classnumber, shuxue);
                    EnglishSatisfaction(classnumber, yingyu);
                }

                result.Data = null;
                result.Msg = "成功";
                result.ErrorCode = 200;

            }
            catch (Exception ex)
            {

                result.Data = null;
                result.Msg = ex.Message;
                result.ErrorCode = 500;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 生成食堂满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult CanteenQuestionnaire(int classnumber)
        {
            AjaxResult result = new AjaxResult();
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            BaseBusiness<ClassSchedule> classdb = new BaseBusiness<ClassSchedule>();
            //查出评价班级
            var classlisttemp = classdb.GetList().Where(d => d.IsDelete == false && d.id == classnumber).FirstOrDefault().ClassNumber;
            //查出这个月这个班的食堂满意度有没有生成
            var templist = db_survey.satisficingConfigs().Where(d => d.IsDel == false && d.EmployeeId == null && d.ClassNumber == classnumber && d.Isitacanteen == true).ToList();
            if (templist.Count != 0)
            {
                //已经存在
                result.ErrorCode = 300;
                result.Data = null;
                result.Msg = "满意度本月已存在";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            SatisficingConfig satisficingConfig = new SatisficingConfig();
            satisficingConfig.ClassNumber = classnumber;
            satisficingConfig.CreateTime = DateTime.Now;
            satisficingConfig.CurriculumID = null;
            satisficingConfig.EmployeeId = null;
            satisficingConfig.IsDel = false;
            satisficingConfig.IsPastDue = false;
            satisficingConfig.Isitacanteen = true;
            satisficingConfig.Isitayuwen = false;
            satisficingConfig.isitashuxue = false;
            satisficingConfig.isitayingyu = false;
            //设置截止时间 默认截止日期
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/empmanageConfig.xml"));
            var xmlRoot = xmlDocument.DocumentElement;
            var defaultTime = ((XmlElement)xmlRoot.GetElementsByTagName("defaultCutOffDate")[0]).Attributes["value"].Value;
            satisficingConfig.CutoffDate = DateTime.Now.AddHours(int.Parse(defaultTime));
            db_survey.AddSatisficingConfig(satisficingConfig);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 语文老师满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult ChineseSatisfaction(int classnumber,string yuwu)
        {
            AjaxResult result = new AjaxResult();
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            BaseBusiness<ClassSchedule> classdb = new BaseBusiness<ClassSchedule>();
            //查出评价班级
            var classlisttemp = classdb.GetList().Where(d => d.IsDelete == false && d.id == classnumber).FirstOrDefault().ClassNumber;
            //查出这个月这个班的食堂满意度有没有生成
            var templist = db_survey.satisficingConfigs().Where(d => d.IsDel == false && d.EmployeeId == null && d.ClassNumber == classnumber && d.Isitayuwen == true).ToList();
            if (templist.Count != 0)
            {
                //已经存在
                result.ErrorCode = 300;
                result.Data = null;
                result.Msg = "满意度本月已存在";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            SatisficingConfig satisficingConfig = new SatisficingConfig();
            satisficingConfig.ClassNumber = classnumber;
            satisficingConfig.CreateTime = DateTime.Now;
            satisficingConfig.CurriculumID = 8;
            satisficingConfig.EmployeeId = yuwu;
            satisficingConfig.IsDel = false;
            satisficingConfig.IsPastDue = false;
            satisficingConfig.Isitacanteen = false;
            satisficingConfig.Isitayuwen = true;
            satisficingConfig.isitashuxue = false;
            satisficingConfig.isitayingyu = false;
            //设置截止时间 默认截止日期
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/empmanageConfig.xml"));
            var xmlRoot = xmlDocument.DocumentElement;
            var defaultTime = ((XmlElement)xmlRoot.GetElementsByTagName("defaultCutOffDate")[0]).Attributes["value"].Value;
            satisficingConfig.CutoffDate = DateTime.Now.AddHours(int.Parse(defaultTime));
            db_survey.AddSatisficingConfig(satisficingConfig);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 数学老师满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult MathematicsSatisfaction(int classnumber,string shuxue)
        {
            AjaxResult result = new AjaxResult();
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            BaseBusiness<ClassSchedule> classdb = new BaseBusiness<ClassSchedule>();
            //查出评价班级
            var classlisttemp = classdb.GetList().Where(d => d.IsDelete == false && d.id == classnumber).FirstOrDefault().ClassNumber;
            //查出这个月这个班的食堂满意度有没有生成
            var templist = db_survey.satisficingConfigs().Where(d => d.IsDel == false && d.EmployeeId == null && d.ClassNumber == classnumber && d.isitashuxue == true).ToList();
            if (templist.Count != 0)
            {
                //已经存在
                result.ErrorCode = 300;
                result.Data = null;
                result.Msg = "满意度本月已存在";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            SatisficingConfig satisficingConfig = new SatisficingConfig();
            satisficingConfig.ClassNumber = classnumber;
            satisficingConfig.CreateTime = DateTime.Now;
            satisficingConfig.CurriculumID = 9;
            satisficingConfig.EmployeeId = shuxue;
            satisficingConfig.IsDel = false;
            satisficingConfig.IsPastDue = false;
            satisficingConfig.Isitacanteen = false;
            satisficingConfig.Isitayuwen = false;
            satisficingConfig.isitashuxue = true;
            satisficingConfig.isitayingyu = false;
            //设置截止时间 默认截止日期
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/empmanageConfig.xml"));
            var xmlRoot = xmlDocument.DocumentElement;
            var defaultTime = ((XmlElement)xmlRoot.GetElementsByTagName("defaultCutOffDate")[0]).Attributes["value"].Value;
            satisficingConfig.CutoffDate = DateTime.Now.AddHours(int.Parse(defaultTime));
            db_survey.AddSatisficingConfig(satisficingConfig);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 英语老师满意度问卷
        /// </summary>
        /// <returns></returns>
        public ActionResult EnglishSatisfaction(int classnumber,string yingyu)
        {
            AjaxResult result = new AjaxResult();
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            BaseBusiness<ClassSchedule> classdb = new BaseBusiness<ClassSchedule>();
            //查出评价班级
            var classlisttemp = classdb.GetList().Where(d => d.IsDelete == false && d.id == classnumber).FirstOrDefault().ClassNumber;
            //查出这个月这个班的食堂满意度有没有生成
            var templist = db_survey.satisficingConfigs().Where(d => d.IsDel == false && d.EmployeeId == null && d.ClassNumber == classnumber && d.isitayingyu == true).ToList();
            if (templist.Count != 0)
            {
                //已经存在
                result.ErrorCode = 300;
                result.Data = null;
                result.Msg = "满意度本月已存在";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            SatisficingConfig satisficingConfig = new SatisficingConfig();
            satisficingConfig.ClassNumber = classnumber;
            satisficingConfig.CreateTime = DateTime.Now;
            satisficingConfig.CurriculumID = 10;
            satisficingConfig.EmployeeId = yingyu;
            satisficingConfig.IsDel = false;
            satisficingConfig.IsPastDue = false;
            satisficingConfig.Isitacanteen = false;
            satisficingConfig.Isitayuwen = false;
            satisficingConfig.isitashuxue = false;
            satisficingConfig.isitayingyu = true;
            //设置截止时间 默认截止日期
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/empmanageConfig.xml"));
            var xmlRoot = xmlDocument.DocumentElement;
            var defaultTime = ((XmlElement)xmlRoot.GetElementsByTagName("defaultCutOffDate")[0]).Attributes["value"].Value;
            satisficingConfig.CutoffDate = DateTime.Now.AddHours(int.Parse(defaultTime));
            db_survey.AddSatisficingConfig(satisficingConfig);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 是否可以进行本月的班主任满意度调查
        /// </summary>
        /// <returns></returns>
        public ActionResult IsOkHeadMasterSurvey()
        {
            return null;
        }


        /// <summary>
        /// 获取当前学员可以填写的满意度调查单
        /// </summary>
        /// <returns></returns>
        public ActionResult IsHaveSatisfaction(string type)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                //SatisficingConfig
                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();
                //获取当前登陆学生的学号
                var studentNumber = Request.Cookies["StudentNumber"].Value.ToString();

                var data = db_survey.GetSatisficingConfigsByStudent(studentNumber, type);
                result.Data = data;
                result.Msg = "成功";
                result.ErrorCode = 200;
            }
            catch (Exception ex)
            {

                result.Data = null;
                result.Msg = "失败";
                result.ErrorCode = 500;
            }
            //获取当前登录的学员

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 历史记录
        /// </summary>
        /// <returns></returns>
        public ActionResult SurveyHistory()
        {
           
            return View();
        }


        /// <summary>
        /// 获取我的部门人员
        /// </summary>
        /// <returns></returns>
        public ActionResult MyDepEmplist()
        {

            //返回的结果
            resultdtree result = new resultdtree();

            //状态
            dtreestatus dtreestatus = new dtreestatus();


            try
            {
                Base_UserModel user = Base_UserBusiness.GetCurrentUser();

                //获取这些员工所在的部门

                List<EmployeesInfo> emplist = db_survey.GetMyDepEmp(user);

                //获取员工部门
                List<dtreeview> childrendtreedata = new List<dtreeview>();
                List<Department> deplist = new List<Department>();

                foreach (var item in emplist)
                {
                    var dep = db_emp.GetDeptByEmpid(item.EmployeeId);

                    if (!db_survey.IsContains(deplist, dep))
                    {
                        deplist.Add(dep);
                    }
                }

                for (int i = 0; i < deplist.Count; i++)
                {
                    //第一层
                    dtreeview seconddtree = new dtreeview();

                    seconddtree.context = deplist[i].DeptName;
                    seconddtree.last = false;
                    seconddtree.level = 0;
                    seconddtree.nodeId = deplist[i].DeptId.ToString();
                    seconddtree.parentId = "0";
                    seconddtree.spread = false;

                    //第二层

                    var tememplist = db_emp.GetEmpsByDeptid(deplist[i].DeptId);
                  
                    if (tememplist.Count >= 0)
                    {

                        List<dtreeview> Quarterlist = new List<dtreeview>();
                        foreach (var item in tememplist)
                        {
                            dtreeview treeemp = new dtreeview();
                            treeemp.nodeId = item.EmployeeId;
                            treeemp.context = item.EmpName;
                            treeemp.last = true;
                            treeemp.parentId = deplist[i].DeptId.ToString();
                            treeemp.level = 1;

                            Quarterlist.Add(treeemp);
                        }

                        seconddtree.children = Quarterlist;


                        childrendtreedata.Add(seconddtree);

                       
                    }
                    else
                    {
                        seconddtree.last = true;
                    }
                    
                }

                result.status = dtreestatus;
                result.data = childrendtreedata;

                dtreestatus.code = "200";
                dtreestatus.message = "操作成功";
            }
            catch (Exception ex)
            {

                dtreestatus.code = "1";
                dtreestatus.message = "操作失败";
            }
          

            return Json(result,JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="surveyResultID">ConfigiD</param>
        /// <returns></returns>

        public ActionResult checkSurveyView(int surveyResultID)
        {

            //提供 JoinSurveyStudents

           var survey = db_survey.AllsatisficingResults().Where(d => d.ID == surveyResultID).FirstOrDefault();

           var studentlist = db_survey.JoinSurveyStudents(surveyResultID);

            ViewBag.SurveyConfigId = surveyResultID;

            ViewBag.studentlist = studentlist;

            return View();
        }


        /// <summary>
        /// 获取满意度详细数据
        /// </summary>
        /// <returns></returns>
        public ActionResult SurveyItemData(string studentnumber, int surveyConfigid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
               var surveyResult = db_survey.AllsatisficingResults().Where(d => d.Answerer == studentnumber && d.SatisficingConfig == surveyConfigid).FirstOrDefault();

               var res = db_survey.ConvertToViewModel(surveyResult);


                result.Data = res;
                result.Msg = "成功";
                result.ErrorCode = 200;

            }
            catch (Exception ex)
            {


                result.Data = null;
                result.Msg = "失败";
                result.ErrorCode = 500;
            }

            return Json(result, JsonRequestBehavior.AllowGet);

           
        }

        public ActionResult EmpSurveyView()
        {
            return View();
        }


        //获取员工个人满意度调查
        public ActionResult EmpSurveyData(int page)
        {
            // 筛选条件员工 日期降序排序
            List<SatisfactionSurveyDetailView> configlist = new List<SatisfactionSurveyDetailView>();
            List<SurveyGroupByDateView> resultlist = new List<SurveyGroupByDateView>();
            int TotalCount = 0;
            try
            {
                //获取当前账号

                Base_UserModel user = Base_UserBusiness.GetCurrentUser();

                var alllist = db_survey.satisficingConfigs().Where(d => d.EmployeeId == user.EmpNumber).OrderByDescending(d => d.CreateTime).ToList();
                TotalCount = alllist.Count;
                var templist = alllist.Skip((page - 1) * 6).Take(6).ToList();

                foreach (var item in templist)
                {
                   var tempResult = db_survey.AllsatisficingResults().Where(d => d.SatisficingConfig == item.ID).FirstOrDefault();

                   var temobj = db_survey.ConvertToViewModel(tempResult);

                    if (temobj != null)
                    {
                        configlist.Add(temobj);

                    }
                }
                foreach (var item in configlist)
                {
                    if (SurveyGroupByDateView.IsContains(resultlist, item.investigationDate))
                    {
                        resultlist.Where(d => d.date.Year == item.investigationDate.Year && d.date.Month == item.investigationDate.Month).FirstOrDefault().data.Add(item);
                    }

                    else {
                        SurveyGroupByDateView surveyGroupByDateView = new SurveyGroupByDateView();
                        surveyGroupByDateView.date = item.investigationDate;
                        surveyGroupByDateView.data.Add(item);
                        resultlist.Add(surveyGroupByDateView);

                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            var objresult = new
            {

                status = 0,
                message = "成功",
                total = TotalCount,
                data = resultlist

            };

            return Json(objresult, JsonRequestBehavior.AllowGet);



        }



    }
} 