using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;

namespace SiliconValley.InformationSystem.Web.Areas.Teaching.Controllers
{
    public class AddCourseController :BaseController
    {

        private AddCourseBusiness db_addCourse;

        public AddCourseController()
        {
            db_addCourse = new AddCourseBusiness();
        }
        // GET: Teaching/AddCourse

        /// <summary>
        /// 加课列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCouseIndex()
        {
            return View();
        }

        /// <summary>
        /// 加课数据
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult AddCourseData(int limit, int page)
        {
            var currentUser = Base_UserBusiness.GetCurrentUser();

            var list = db_addCourse.TeacherAddCourses(currentUser.EmpNumber);

            var skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();

            List<AddCourseView> viewlist = new List<AddCourseView>();

            skiplist.ForEach(d=>
            {
                var tempobj = db_addCourse.ConvertToView(d);
                if (tempobj != null) viewlist.Add(tempobj);
            });

            var obj = new {

                code = 0,
                msg = "",
                count = list.Count,
                data = viewlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }
    }
}