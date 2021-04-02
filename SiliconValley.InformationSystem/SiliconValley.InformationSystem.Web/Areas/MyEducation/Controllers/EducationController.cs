using Newtonsoft.Json;
using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.EnrollmentBusiness;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;
using SiliconValley.InformationSystem.Entity;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SiliconValley.InformationSystem.Web.Areas.MyEducation.Controllers
{
    //本科管理
    [CheckLogin]
    public class EducationController : Controller
    {  //课程类别
        BaseBusiness<Undergraduatemajor> UndergraduatemajorYBusiness = new BaseBusiness<Undergraduatemajor>();
        //报考学校
        BaseBusiness<Undergraduateschool> UndergraduateschoolBunsiness = new BaseBusiness<Undergraduateschool>();
        //班级表
        ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
        private readonly EnrollmentBusinesse dbtext;
        public EducationController()
        {
            dbtext = new EnrollmentBusinesse();
        }
        // GET: MyEducation/Education
        //主页面
        public ActionResult Undergraduate()
        {
            ViewBag.ClassName = classScheduleBusiness.GetList().Where(a => a.IsDelete == false).Select(a => new SelectListItem { Text = a.ClassNumber, Value = a.ClassNumber }).ToList();
            return View();
        }

        //获取已报名本科学员
        public ActionResult GetDate(int page, int limit, string Name, string StudentNumber, string identitydocument,string ClassName,string Alreadypassed)
        {
            return Json(dbtext.GetDate(page, limit, Name, StudentNumber, identitydocument, ClassName, Alreadypassed), JsonRequestBehavior.AllowGet);
        }
        //本科专业添加表单
        [HttpGet]
        public ActionResult AddUndergraduatemajor()
        {
            return View();
        }
        //本科专业数据操作添加
        [HttpPost]
        public ActionResult AddUndergraduatemajor(Undergraduatemajor undergraduatemajor)
        {

            return Json(dbtext.AddUndergraduatemajor(undergraduatemajor));
        }
        //添加本科课程类别大
        [HttpGet]
        public ActionResult AddCoursecategory()
        {
            ViewBag.Major = UndergraduatemajorYBusiness.GetList().Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ProfessionalName}).ToList();
            return View();
        }
        //添加课程类别大
        public ActionResult AddCoursecategory(CoursecategoryY coursecategoryY)
        {
            return Json(dbtext.AddCoursecategory(coursecategoryY), JsonRequestBehavior.AllowGet);
        }
        //验证专业名称是否重复
        public ActionResult BoolUndergraduatemajor(string id)
        {
            return Json(dbtext.BoolUndergraduatemajor(id), JsonRequestBehavior.AllowGet);
        }
        //验证本科课程类别是否有重复大
        public ActionResult BoolCoursecategory(int id)
        {
            string name = Request.QueryString["name"];
            return Json(dbtext.BoolCoursecategory(name,id), JsonRequestBehavior.AllowGet);
        }
        //添加子及课程类别
        [HttpGet]
        public ActionResult AddCoursecategoryX()
        {
            ViewBag.Major = UndergraduatemajorYBusiness.GetList().Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ProfessionalName }).ToList();
            return View();
        }
        //加载课程类别子及数据
        public ActionResult SelectCoursecaregoryX(int MajorID)
        {
            return Json(dbtext.SelectCoursecaregoryX(MajorID), JsonRequestBehavior.AllowGet);
        }
        //添加子及课程类别数据操作
        [HttpPost]
        public ActionResult AddCoursecategoryX(CoursecategoryX coursecategoryX)
        {
            return Json(dbtext.AddCoursecategoryX(coursecategoryX), JsonRequestBehavior.AllowGet);
        }
         //验证下级类别名称是否重复
        public ActionResult BoolCoursecategoryX(int CoursecategoryYID,string Coursetitle)
        {
            return Json(dbtext.BoolCoursecategoryX(CoursecategoryYID, Coursetitle), JsonRequestBehavior.AllowGet);
        }
        //本科学员信息修改
        [HttpGet]
        public ActionResult UpdaEnrollment(string id)
        {
            ViewBag.student = dbtext.FineStudent(id);
            ViewBag.Major = UndergraduatemajorYBusiness.GetList().Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ProfessionalName }).ToList();
            ViewBag.Registeredbatch= UndergraduateschoolBunsiness.GetList().Where(a => a.IsDelete == false).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.SchoolName }).ToList();
            return View(dbtext.GetList().Where(a=>a.IsDelete==false&&a.StudentNumber==id).FirstOrDefault());
        }
        //本科报考学校
        [HttpGet]
        public ActionResult AddUndergraduateschool()
        {
            return View();
        }
        //验证报考学校名字是否重复
        public ActionResult BoolUndergraduateschool(string id)
        {
            return Json(dbtext.BoolUndergraduateschool(id), JsonRequestBehavior.AllowGet);
        }
        //添加报考学校数据操作
        [HttpPost]
        public ActionResult AddUndergraduateschool(Undergraduateschool undergraduateschool)
        {
            return Json(dbtext.AddUndergraduateschool(undergraduateschool), JsonRequestBehavior.AllowGet);
        }

        //补充本科学员信息
        [HttpPost]
        public ActionResult UpdaEnrollment(Enrollment enrollment)
        {
            return Json(dbtext.UpdaEnrollment(enrollment), JsonRequestBehavior.AllowGet);
        }
        //课程添加
        public ActionResult curriculum()
        {
            ViewBag.Major = UndergraduatemajorYBusiness.GetList().Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ProfessionalName }).ToList();
            return View();
        }
        //下级下拉框赋值
        public ActionResult SelectCoursecaregory(int CoursecategoryYID)
        {
            return Json(dbtext.SelectCoursecaregory(CoursecategoryYID),JsonRequestBehavior.AllowGet);
        }
        //验证本科课程名称是否重复
        public ActionResult BoolUndergraduatecourse(int CoursecategoryXid,string Coursetitle)
        {
            return Json(dbtext.BoolUndergraduatecourse(CoursecategoryXid, Coursetitle), JsonRequestBehavior.AllowGet);
        }
        //本科课程添加
        [HttpPost]
        public ActionResult Addcurriculum(Undergraduatecourse undergraduatecourse)
        {
            return Json(dbtext.Addcurriculum(undergraduatecourse), JsonRequestBehavior.AllowGet);
        }
        //报考课程
        public ActionResult Registerforexamination(string id)
        {
            ViewBag.student = dbtext.FineRegisterforexamination(id);
          var enrid=  dbtext.GetList().Where(a => a.IsDelete == false && a.StudentNumber == id).FirstOrDefault();
            ViewBag.CoursecategoryY = dbtext.SelectCoursecaregoryXView((int)enrid.MajorID).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.Coursetitle }).ToList();
            return View();
        }
        //加载复选框课程名称
        public ActionResult Checkboxatecourse(int CoursecategoryXid,int EnrollID)
        {
           
            return Json(dbtext.Checkboxatecourse(CoursecategoryXid, EnrollID), JsonRequestBehavior.AllowGet);
        }
        //学员报考课程记录
        [HttpPost]
        public ActionResult Registerforexamination(Registerforexamination registerforexamination,string UndergraduatecourseIDs)
        {
           
            return Json(dbtext.AddRegisterforexamination(registerforexamination, UndergraduatecourseIDs),JsonRequestBehavior.AllowGet);

        }
        StudentFeeStandardBusinsess studentFeeStandard = new StudentFeeStandardBusinsess();
        //报考费收据单
        public ActionResult Applicationfeereceipt()
        {
            string studentid = SessionHelper.Session["Appstudentid"].ToString();
            ViewBag.student = JsonConvert.SerializeObject(studentFeeStandard.StudentFind(studentid));
            ViewBag.Receiptdata = JsonConvert.SerializeObject(SessionHelper.Session["Applicationfeereceipt"]);
            return View();
        }
        // //Examinationperiod
        //本科成绩录入
        [HttpGet]
        public ActionResult AddUndergraduateachievement(string id)
        {
         ViewBag.student = dbtext.FineRegisterforexamination(id);

         ViewBag.Examinationperiod = dbtext.SelectExaminationperiod(id).Select(a=>new SelectListItem {  Text=a,Value=a});
            return View();
        }
        //加载已报考科目
        public ActionResult ChedeckdUndergraduateachievement(int EnrollID,string Examinationperiod)
        {
            return Json(dbtext.ChedeckdUndergraduateachievement(EnrollID,Examinationperiod),JsonRequestBehavior.AllowGet);
        }
        //成绩录入系统
        [HttpPost]
        public ActionResult AddUndergraduateachievemenS(string Examinationperiod)
        {
            //引入序列化
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //序列化
            var list = serializer.Deserialize<List<Undergraduateachievement>>(Examinationperiod);
            return Json(dbtext.AddUndergraduateachievemenS(list), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 成绩查询
        /// </summary>
        /// <param name="id">学号</param>
        /// <returns></returns>
        public ActionResult Achievementduate(string id)
        {
            ViewBag.studentid = id;
            return View(dbtext.FindEssntiali(id));
        }
        /// <summary>
        /// 加载已经报考的课程
        /// </summary>
        /// <param name="id">学号</param>
        /// <returns></returns>
        public ActionResult GetDateRegisterforexamination(string id, int page, int limit)
        {
            return Json(dbtext.GetDateRegisterforexamination(id,  page, limit), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 加载已经合格的课程
        /// </summary>
        /// <param name="id">学号</param>
        /// <returns></returns>
        public ActionResult GetDatechievement(string id, int page, int limit)
        {
            return Json(dbtext.GetDatechievement(id,  page,  limit), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据学号获取是否报名本科
        /// </summary>
        /// <param name="Studentid">学号</param>
        /// <returns></returns>
        public ActionResult StudentUndergraduatecount(string Studentid)
        {
            return Json(dbtext.StudentUndergraduatecount(Studentid), JsonRequestBehavior.AllowGet);
        }
        public ActionResult AchievementImport()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AchievementImport(HttpPostedFileBase excelfile)
        {
            Stream filestream = excelfile.InputStream;

            var result = dbtext.ImportDataFormExcel(filestream, excelfile.ContentType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AchievementDetails(string stunumber)
        {
            ViewBag.stunumber = stunumber;
            return View();
        }
        public ActionResult GetDetails(int page,int limit,string stunumber)
        {
           return Json(dbtext.GetUndergraduateachievement(page,limit,stunumber),JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDetailspass(int page, int limit, string stunumber)
        {
            return Json(dbtext.Getpass(page, limit, stunumber), JsonRequestBehavior.AllowGet);
        }
    }
}