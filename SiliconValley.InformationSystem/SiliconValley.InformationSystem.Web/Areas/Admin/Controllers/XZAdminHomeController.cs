using SiliconValley.InformationSystem.Business.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Admin.Controllers
{
    public class XZAdminHomeController : Controller
    {
        XYKAdminHome_Business AdminBind = new XYKAdminHome_Business();
        // GET: Admin/XZAdminHome
        [HttpGet]
        public ActionResult AdminHomeIndex()
        {
            ViewBag.StudentCount = AdminBind.StudentCount();//学校在校人
            ViewBag.StudentExpel = AdminBind.StudentExpel();//统计开除总人数
            ViewBag.StudentDropout = AdminBind.StudentDropout();//退学统计
            ViewBag.StudentSuspensionof = AdminBind.StudentSuspensionof();//休学统计
            ViewBag.StudentClassSum = AdminBind.StudentClassSum();//获取所有班级
            return View();
        }
    }
}