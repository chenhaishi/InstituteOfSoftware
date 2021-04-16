using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Controllers
{
    public class StudentController : BaseController
    {
        // GET: Student
        public ActionResult Index()
        {
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();

            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();
            //获取当前登陆学生的学号
            var studentNumber = Request.Cookies["StudentNumber"].Value.ToString();
            var student = studentInformationBusiness.StudentList().Where(d => d.StudentNumber == studentNumber).FirstOrDefault();
            ViewBag.student = student;
            return View();
        }

        public ActionResult Text()
        {
            RedisCache redis = new RedisCache();
            redis.SetCache("student", "唐敏");

           string str= redis.GetCache("student").ToString();
            return null;
        }
    }
}