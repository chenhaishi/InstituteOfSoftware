using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.DormitoryMaintenance.Controllers
{
    public class StuApartmentController : Controller
    {
        // GET: DormitoryMaintenance/StuApartment
        //学生公寓控制器
        public ActionResult Index()
        {
            return View();
        }
    }
}