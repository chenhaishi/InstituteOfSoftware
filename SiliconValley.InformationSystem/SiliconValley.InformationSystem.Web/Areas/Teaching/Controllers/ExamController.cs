using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Teaching.Controllers
{
    public class ExamController : BaseController
    {
        // GET: Teaching/Exam
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 项目答辩页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ProjectDefense()
        {
            return View();
        }
        /// <summary>
        /// 考题讲解
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExamquestionExplanation()
        //{
        //    return View();
        //}
        /// <summary>
        /// 考试讲解模块机试页面
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExaminationModule()
        //{
        //    return View();
        //}
    }
}