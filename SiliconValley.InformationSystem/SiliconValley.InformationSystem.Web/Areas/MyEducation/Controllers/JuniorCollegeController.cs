using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.JuniorCollegeBusinesse;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.MyEducation.Controllers
{
    public class JuniorCollegeController : Controller
    {
        //班级表
        ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
        //学员信息
        StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
        //报考学校
        BaseBusiness<Undergraduateschool> UndergraduateschoolBunsiness = new BaseBusiness<Undergraduateschool>();
        //课程类别
        BaseBusiness<Undergraduatemajor> UndergraduatemajorYBusiness = new BaseBusiness<Undergraduatemajor>();
        NotRegisteredForJuniorCollegeBuiness notRegisteredForJuniorCollegeBuiness = new NotRegisteredForJuniorCollegeBuiness();
        private readonly JuniorCollegeBusinesse dbtext;
        public JuniorCollegeController()
        {
            dbtext = new JuniorCollegeBusinesse();
        }
        // GET: MyEducation/JuniorCollege
        public ActionResult Index()
        {
            //dbtext.Generate();
            ViewBag.ClassName = classScheduleBusiness.GetList().Where(a => a.IsDelete == false).Select(a => new SelectListItem { Text = a.ClassNumber, Value = a.ClassNumber }).ToList();
            return View();
        }
        public ActionResult GetData(int page, int limit, string AppCondition)
        {
           
            return Json(dbtext.GetData(page, limit, AppCondition), JsonRequestBehavior.AllowGet);
        }
        public ActionResult JuniorCollegeEdit(string id)
        {
            ViewBag.Major = UndergraduatemajorYBusiness.GetList().Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ProfessionalName }).ToList();
            ViewBag.Registeredbatch = UndergraduateschoolBunsiness.GetList().Where(a => a.IsDelete == false).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.SchoolName }).ToList();
            var data = dbtext.GetListBySql<JuniorCollege>("select *from JuniorCollege where StudentNumber='"+id+"'").FirstOrDefault();
            ViewBag.data = data;
            return View();
        }

        [HttpPost]
        public ActionResult JuniorCollegeEdit(JuniorCollege junior)
        {
            return Json(dbtext.JuniorCollegeEdit(junior), JsonRequestBehavior.AllowGet);
        }
        public ActionResult InsertNotRegisteredForJuniorCollege(string id)
        {
            ViewBag.StudentNumber = id;
            ViewBag.Name = studentInformationBusiness.GetEntity(id).Name;
            ViewBag.Sex = studentInformationBusiness.GetEntity(id).Sex==true?"男":"女";
            return View();
        }
        [HttpPost]
        public ActionResult InsertNotRegisteredForJuniorCollege(NotRegisteredForJuniorCollege junior)
        {
            return Json(notRegisteredForJuniorCollegeBuiness.InsertNotRegisteredForJuniorCollege(junior), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ISJuniorCollege(string id)
        {
            AjaxResult result = new AjaxResult();

            var s = studentInformationBusiness.GetListBySql<StudentInformation>("select *from StudentInformation where StudentNumber='" + id + "' and Education='大专'");
            if (s.Count==0)
            {
                result.Success = true;
            }
            else
            {
                result.Success = false;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetNotJuniorCollegeData(int page, int limit, string AppCondition)
        {
            return Json(notRegisteredForJuniorCollegeBuiness.GetNotJuniorCollegeData(page, limit, AppCondition), JsonRequestBehavior.AllowGet);
        }
    }
}
