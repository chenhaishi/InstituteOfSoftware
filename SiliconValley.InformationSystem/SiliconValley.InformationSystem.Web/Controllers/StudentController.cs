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