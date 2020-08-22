using SiliconValley.InformationSystem.Business.EducationalBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Business.PositionBusiness;
using SiliconValley.InformationSystem.Business.DepartmentBusiness;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    [CheckLogin]
    public class ReconcileController : BaseMvcController
    {
        // GET: /Educational/Reconcile/GetMarjioTeacher
        readonly ReconcileManeger Reconcile_Entity = new ReconcileManeger();


        #region 大批量课表安排
        /// <summary>
        /// 系统安排专业排课页面
        /// </summary>
        /// <returns></returns>
        public ActionResult HightStudentAnpaiView() 
        {
            //加载阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;
            //加载校区
            BaseDataEnumManeger baseDataEnum_Entity = new BaseDataEnumManeger();
            List<SelectListItem> schooladdress = baseDataEnum_Entity.GetsameFartherData("校区地址").Select(s => new SelectListItem() { Text = s.Name, Value = s.Id.ToString() }).ToList();
            schooladdress.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.Schooladdress = schooladdress;
            //加载专业老师
            ViewBag.Teacher = Reconcile_Entity.GetTeacherAll().Select(r => new SelectListItem() { Text = r.EmpName, Value = r.EmployeeId }).ToList();
            return View();
        }

        /// <summary>
        /// 排课页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ReconcileIndexViews()
        {

            return View();
        }



        /// <summary>
        /// 通过阶段获取班级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetClassScheduleSelect(int id)
        {
            var c_list = Reconcile_Entity.GetGrandClass(id).ToList();
            return Json(c_list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取课程
        /// </summary>
        /// <param name="id">班级名称</param>
        /// <returns></returns>
        public ActionResult GetClassDate(int id)
        {
            if (id != 0)
            {
                ClassSchedltData new_c = new ClassSchedltData();
                ClassSchedule find_c = Reconcile_Com.ClassSchedule_Entity.GetEntity(id);
                new_c.Name = find_c.ClassNumber;//班级名称
                string marjon = Reconcile_Com.ClassSchedule_Entity.GetClassGrand(find_c.id, 1);//专业
                new_c.marjoiName = marjon;
                string grand = Reconcile_Com.ClassSchedule_Entity.GetClassGrand(find_c.id, 2);//阶段
                new_c.GrandName = grand;
                string time = Reconcile_Com.ClassSchedule_Entity.GetClassTime(find_c.id);//上课时间类型
                new_c.ClassDate = time;
                //获取某个阶段某个专业的所有课程                 
                var find_clist = Reconcile_Entity.GetCurr(find_c.grade_Id, true, Convert.ToInt32(find_c.Major_Id));
                var josndata = new { classData = new_c, c_list = find_clist, stataus = "true" };
                return Json(josndata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var josndata = new { classData = "", c_list = "", stataus = "false" };
                return Json(josndata, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 系统排课业务处理方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PaikeFunction()
        {
            AjaxResult a = new AjaxResult();
            StringBuilder db = new StringBuilder();
            try
            {
                TeacherClassBusiness TeacherClass_Entity = new TeacherClassBusiness();
                int y1_id = Reconcile_Com.GetGrand_Id().Where(c => c.GrandName.Equals("Y1", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Id; //获取Y1阶段Id               
                string grand_Id = Request.Form["mygrand"];//获取班级阶段
                int class_Id = Convert.ToInt32(Request.Form["class_select"]);//获取班级
                int classroom_Id = Convert.ToInt32(Request.Form["myclassroom"]);//获取教室
                int kengcheng = Convert.ToInt32(Request.Form["kecheng"]);//获取课程编号
                string ke = Reconcile_Com.Curriculum_Entity.GetEntity(kengcheng).CourseName;//获取课程名称
                string time = Request.Form["time"];//获取上课时间
                string techar = Request.Form["teachersele"];//获取任课老师
                DateTime startTime = Convert.ToDateTime(Request.Form["startTime"]);//获取排课日期

                //开始排课
                Curriculum find_c = Reconcile_Com.Curriculum_Entity.GetEntity(kengcheng);
                //查看这个课程的课时数
                int Kcount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(find_c.CourseCount / 4.0)));
                //判断该班级这个课程是否已排完课
                int count = Reconcile_Entity.GetList().Where(r => r.Curriculum_Id == ke && r.ClassSchedule_Id == class_Id).ToList().Count;
                if (count > 0)
                {
                    a.Msg = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_Id).ClassNumber + "的" + ke + "已有排课数据，为了避免重复，请删除之后再操作！！";
                    a.Success = false;
                }
                else if (count == Kcount)
                {
                    a.Msg = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_Id).ClassNumber + "的" + ke + "已有排课数据,请安排其他课程！！";
                    a.Success = false;
                }
                else
                {

                    GetYear find_g = Reconcile_Entity.MyGetYear(startTime.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml")); //获取单休双休月份

                    List<Reconcile> new_list = new List<Reconcile>();

                    //if (y1_id.ToString() == grand_Id) //判断是否是Y1的班级
                    //{

                    //    #region 初中生
                    //    string yuwen = Request.Form["yuwen"]; //获取语文老师
                    //    string shuxue = Request.Form["shuxue"]; //获取数学老师
                    //    string yingyu = Request.Form["yingyu"]; //获取英语老师
                    //    List<Reconcile> get_new_data = Reconcile_Entity.MiddleStudentReconcileFunction(find_g, kengcheng, startTime, time, classroom_Id, techar, class_Id, y1_id, yuwen, shuxue, yingyu);
                    //    a = Reconcile_Entity.Inser_list(get_new_data);
                    //    #endregion
                    //}
                    //else
                    //{

                        #region 高中生
                        ClassSchedule find_class = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_Id);
                        int marjoin = Convert.ToInt32(find_class.Major_Id);
                        int grand = find_class.grade_Id;
                        List<Reconcile> get_new_data = Reconcile_Entity.HeghtStudentReconcileFunction(find_g, kengcheng, startTime, time, classroom_Id, techar, class_Id, grand, marjoin);
                        a = Reconcile_Entity.Inser_list(get_new_data);
                        #endregion
                    //}

                }
            }
            catch (Exception ex)
            {
                a.Msg = "系统异常，请刷新重试";
                a.Success = false;
            }

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取某个时间段没有安排上课的空教室
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetEmptyRoom()
        {
            string timeName = Request.Form["timename"];//上课时间
            DateTime Time = Convert.ToDateTime(Request.Form["date"]);//上课日期
            int sid = Convert.ToInt32(Request.Form["schoolId"]);//校区
            List<SelectListItem> c_list = Reconcile_Entity.GetClassrooms(timeName, Time).Where(c => c.BaseData_Id == sid).Select(c => new SelectListItem() { Text = c.ClassroomName, Value = c.Id.ToString() }).ToList();
            c_list = c_list.OrderBy(c => c.Value).ToList();
            return Json(c_list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取某个校区所有有效教室
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSchoolEmptyRoom()
        {
            AjaxResult a = new AjaxResult();
            int belongto_SchoolAdress = Convert.ToInt32(Request.Form["schooldaddressval"]);
            List<SelectListItem> data = Reconcile_Com.Classroom_Entity.GetAddreeClassRoom(belongto_SchoolAdress).Select(c => new SelectListItem() { Text = c.ClassroomName, Value = c.Id.ToString() }).ToList();
            a.Data = data;
            return Json(a, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 根据班级获取专业课程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CaseClassGetCurr(int id)
        {
            ClassSchedule findclass = Reconcile_Com.ClassSchedule_Entity.GetEntity(id);//获取班级的所有数据

            List<SelectListItem> c_list = Reconcile_Entity.GetCurr(findclass.grade_Id, true, Convert.ToInt32(findclass.Major_Id)).Select(c => new SelectListItem() { Text = c.CourseName, Value = c.CurriculumID.ToString() }).ToList();
            c_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            c_list = c_list.OrderBy(c => c.Value).ToList();
            return Json(c_list, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 根据专业课程获取在指定时间没有上课的老师
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTeacher()
        {
            DateTime anpai = Convert.ToDateTime(Request.Form["Time"]);
            int curr_id = Convert.ToInt32(Request.Form["Curr"]);
            string timename = Request.Form["timeName"];
            List<EmployeesInfo> emplist = Reconcile_Entity.GetMarjorTeacher(curr_id, timename, anpai);
            List<SelectListItem> result = emplist.Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  //获取指定日期中有空的非专业课的老师
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetNoMajoiThercher()
        {
            DateTime anpai = Convert.ToDateTime(Request.Form["Time"]);//获取日期
            string currname = Request.Form["Curr"];//获取课程类型
            string timename = Request.Form["timeName"];//获取上课时间
            int class_id = Convert.ToInt32(Request.Form["class_id"]);//获取班级
            List<EmployeesInfo> e_list = new List<EmployeesInfo>();
            PositionManage position = new PositionManage();
            ClassSchedule find_c = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_id);//获取班级数据
            int grand_id = find_c.grade_Id;//获取阶段
            switch (currname)
            {
                case "职素":
                    //获取可以上职素课的班主任
                    //如果是S4的就要获取就业部的老师
                    e_list = Reconcile_Entity.GetMasTeacher(anpai, timename);
                    break;
                case "军事":
                    //获取教官
                    e_list = Reconcile_Entity.GetSir(anpai, timename);
                    break;
                case "英语":
                    //获取阶段                                           
                    Curriculum find_curr = Reconcile_Com.Curriculum_Entity.GetList().Where(c => c.Grand_Id == find_c.grade_Id && c.CourseName == currname).FirstOrDefault();
                    if (find_curr != null)
                    {
                        e_list = Reconcile_Entity.GetMarjorTeacher(find_curr.CurriculumID, timename, anpai);
                    }

                    break;
                case "数学":
                    //获取数学老师
                    Curriculum find_curr1 = Reconcile_Com.Curriculum_Entity.GetList().Where(c => c.Grand_Id == grand_id && c.CourseName == currname).FirstOrDefault();
                    if (find_curr1 != null)
                    {
                        e_list = Reconcile_Entity.GetMarjorTeacher(find_curr1.CurriculumID, timename, anpai);
                    }
                    break;
                case "语文":
                    //获取语文老师
                    Curriculum find_curr2 = Reconcile_Com.Curriculum_Entity.GetList().Where(c => c.Grand_Id == grand_id && c.CourseName == currname).FirstOrDefault();
                    if (find_curr2 != null)
                    {
                        e_list = Reconcile_Entity.GetMarjorTeacher(find_curr2.CurriculumID, timename, anpai);
                    }
                    break;
                case "班会":
                    //获取这个班级的班主任
                    //如果这个班级是S4的话，获取就业部的老师
                    //判断班级是否是S4阶段
                    Grand find_g = Reconcile_Com.GetGrand_Id().Where(g => g.GrandName.Equals("S4", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (find_c.grade_Id == find_g.Id)
                    {
                        EmploymentStaff find_saff = Reconcile_Com.EmploymentStaff_Entity.GetStaffByclassid(find_c.id);
                        bool s1 = Reconcile_Entity.IsHaveClass(find_saff.EmployeesInfo_Id, timename, anpai);
                        if (s1 == false)
                        {
                            e_list.Add(Reconcile_Com.Employees_Entity.GetEntity(find_saff.EmployeesInfo_Id));
                        }
                    }
                    else
                    {
                        EmployeesInfo e = Reconcile_Com.GetZhisuTeacher(class_id).Data as EmployeesInfo;
                        if (e != null)
                        {
                            bool s1 = Reconcile_Entity.IsHaveClass(e.EmployeeId, timename, anpai);
                            if (s1 == false)
                            {
                                e_list.Add(e);
                            }
                        }

                    }
                    break;
                case "开班":
                    //获取这个班级的班主任
                    //如果这个班级是S4的话，获取就业部的老师
                    //判断班级是否是S4阶段
                    Grand find_g1 = Reconcile_Com.GetGrand_Id().Where(g => g.GrandName.Equals("S4", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (find_c.grade_Id == find_g1.Id)
                    {
                        EmploymentStaff find_saff = Reconcile_Com.EmploymentStaff_Entity.GetStaffByclassid(find_c.id);
                        bool s1 = Reconcile_Entity.IsHaveClass(find_saff.EmployeesInfo_Id, timename, anpai);
                        if (s1 == false)
                        {
                            e_list.Add(Reconcile_Com.Employees_Entity.GetEntity(find_saff.EmployeesInfo_Id));
                        }
                    }
                    else
                    {
                        EmployeesInfo e = Reconcile_Com.GetZhisuTeacher(class_id).Data as EmployeesInfo;
                        if (e != null)
                        {
                            bool s1 = Reconcile_Entity.IsHaveClass(e.EmployeeId, timename, anpai);
                            if (s1 == false)
                            {
                                e_list.Add(e);
                            }
                        }

                    }
                    break;
            }
            List<SelectListItem> select = e_list.Select(s => new SelectListItem() { Value = s.EmployeeId, Text = s.EmpName }).ToList();
            return Json(select, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 获取XX班级上课时间段
        /// </summary>
        public ActionResult TimeName(int id)
        {
            ClassSchedule find_c = Reconcile_Com.ClassSchedule_Entity.GetEntity(id);
            string time = Reconcile_Com.ClassSchedule_Entity.GetClassTime(find_c.id);//上课时间类型
            return Json(time, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 单个课程排课页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SingeAnpaiView()
        {
            //加载阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;
            //加载校区
            BaseDataEnumManeger baseDataEnum_Entity = new BaseDataEnumManeger();
            List<SelectListItem> schooladdress = baseDataEnum_Entity.GetsameFartherData("校区地址").Select(s => new SelectListItem() { Text = s.Name, Value = s.Id.ToString() }).ToList();
            schooladdress.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.Schooladdress = schooladdress;
            //加载专业老师
            ViewBag.Teacher = Reconcile_Entity.GetTeacherAll().Select(r => new SelectListItem() { Text = r.EmpName, Value = r.EmployeeId }).ToList();
            return View();
        }

        /// <summary>
        /// 系统单个课程排课方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult PaikeFunction2()
        {
            AjaxResult a = new AjaxResult();
            StringBuilder db = new StringBuilder();
            try
            {
                TeacherClassBusiness TeacherClass_Entity = new TeacherClassBusiness();
                int y1_id = Reconcile_Com.GetGrand_Id().Where(c => c.GrandName.Equals("Y1", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Id; //获取Y1阶段Id               
                string grand_Id = Request.Form["mygrand"];//获取班级阶段
                int class_Id = Convert.ToInt32(Request.Form["class_select"]);//获取班级
                int classroom_Id = Convert.ToInt32(Request.Form["myclassroom"]);//获取教室
                int kengcheng = Convert.ToInt32(Request.Form["kecheng"]);//获取课程编号
                string ke = Reconcile_Com.Curriculum_Entity.GetEntity(kengcheng).CourseName;//获取课程名称
                string time = Request.Form["time"];//获取上课时间
                string techar = Request.Form["teachersele"];//获取任课老师
                DateTime startTime = Convert.ToDateTime(Request.Form["startTime"]);//获取排课日期

                //开始排课
                Curriculum find_c = Reconcile_Com.Curriculum_Entity.GetEntity(kengcheng);
                //查看这个课程的课时数
                int Kcount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(find_c.CourseCount / 4.0)));
                //判断该班级这个课程是否已排完课
                int count = Reconcile_Entity.GetList().Where(r => r.Curriculum_Id == ke && r.ClassSchedule_Id == class_Id).ToList().Count;
                if (count > 0)
                {
                    a.Msg = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_Id).ClassNumber + "的" + ke + "已有排课数据，为了避免重复，请删除之后再操作！！";
                    a.Success = false;
                }
                else if (count == Kcount)
                {
                    a.Msg = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_Id).ClassNumber + "的" + ke + "已有排课数据,请安排其他课程！！";
                    a.Success = false;
                }
                else
                {

                    GetYear find_g = Reconcile_Entity.MyGetYear(startTime.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml")); //获取单休双休月份

                    List<Reconcile> new_list = new List<Reconcile>();

                    //if (y1_id.ToString() == grand_Id) //判断是否是Y1的班级
                    //{

                    //    #region 初中生
                    //    string yuwen = Request.Form["yuwen"]; //获取语文老师
                    //    string shuxue = Request.Form["shuxue"]; //获取数学老师
                    //    string yingyu = Request.Form["yingyu"]; //获取英语老师
                    //    List<Reconcile> get_new_data = Reconcile_Entity.MiddleStudent_SingCurs(find_g, kengcheng, startTime, time, classroom_Id, techar, class_Id, y1_id, yuwen, shuxue, yingyu);
                    //    a = Reconcile_Entity.Inser_list(get_new_data);
                    //    #endregion
                    //}
                    //else
                    //{

                        //#region 高中生
                        ClassSchedule find_class = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_Id);
                        int marjoin = Convert.ToInt32(find_class.Major_Id);
                        int grand = find_class.grade_Id;
                        List<Reconcile> get_new_data = Reconcile_Entity.HeghtStudent_SingCurs(find_g, kengcheng, startTime, time, classroom_Id, techar, class_Id, grand, marjoin);
                        a = Reconcile_Entity.Inser_list(get_new_data);
                        //#endregion
                    //}

                }
            }
            catch (Exception ex)
            {
                a.Msg = "系统异常，请刷新重试";
                a.Success = false;
            }

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 手动排课
        /// <summary>
        /// 手动排课页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ManualReconcileView()
        {
            //加载所有阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.Child_grandlist = g_list;
            //获取课程类型
            List<SelectListItem> t_list = Reconcile_Com.CourseType_Entity.GetCourseTypes().Select(t => new SelectListItem() { Text = t.TypeName, Value = t.TypeName }).ToList();
            t_list.Add(new SelectListItem() { Text = "其他", Value = "0", Selected = true });
            ViewBag.Child_typelist = t_list;
            BaseDataEnumManeger baseDataEnum_Entity = new BaseDataEnumManeger();
            List<SelectListItem> schooladdress = baseDataEnum_Entity.GetsameFartherData("校区地址").Select(s => new SelectListItem() { Text = s.Name, Value = s.Id.ToString() }).ToList();
            schooladdress.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.schooladdress = schooladdress;

            List<SelectListItem> list_t = Reconcile_Entity.GetTeacherAll().Select(s => new SelectListItem() { Text = s.EmpName, Value = s.EmployeeId }).ToList();//获取所有在职的专业老师
            list_t.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            ViewBag.Teacher = list_t.OrderBy(l => l.Value).ToList();
            return View();
        }



        /// <summary>
        /// 手动排课数据提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddHandDataFunction()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                Reconcile new_r = new Reconcile();
                //获取班级
                new_r.ClassSchedule_Id = Convert.ToInt32(Request.Form["child_class"]);
                //获取类型
                string type = Request.Form["childview_Currtype"];
                if (type.Contains("专业"))
                {
                    //获取课程Id                 
                    int cur_id = Convert.ToInt32(Request.Form["childview_currname"]);
                    new_r.Curriculum_Id = Reconcile_Com.Curriculum_Entity.GetEntity(cur_id).CourseName;
                }
                else if (type == "0")
                {
                    new_r.Curriculum_Id = Request.Form["childview_currname"].ToString();
                }
                else
                {
                    new_r.Curriculum_Id = Request.Form["childview_currname"].ToString().Substring(0, 2);
                }

                //获取任课老师
                if (!string.IsNullOrEmpty(Request.Form["teacher_child"]) && Request.Form["teacher_child"] != "0")
                {
                    new_r.EmployeesInfo_Id = Request.Form["teacher_child"];
                }
                else
                {
                    new_r.EmployeesInfo_Id = null;
                }

                //获取上课地点
                string room_Id = Request.Form["childview_room"]; //Convert.ToInt32(Request.Form["childview_room"]);//上课地点可能是室外
                if (!string.IsNullOrEmpty(room_Id))
                {
                    new_r.ClassRoom_Id = Convert.ToInt32(room_Id);
                }
                else
                {
                    new_r.ClassRoom_Id = null;
                }
                //获取上课时间
                new_r.Curse_Id = Request.Form["childview_timetype"];
                new_r.IsDelete = false;
                new_r.NewDate = DateTime.Now;
                //排课的时间
                new_r.AnPaiDate = Convert.ToDateTime(Request.Form["childview_AnpiDate"]);
                bool Is = Reconcile_Entity.IsExcit(new_r, true);
                if (Is == false)
                {
                    bool isture = Reconcile_Entity.AddData(new_r);
                    if (isture)
                    {
                        sb.Append("ok");
                    }
                    else
                    {
                        sb.Append("系统错误，请重试!!!");
                    }

                }
                else
                {
                    sb.Append("该班级在这个日期中已安排了这个课程！！！");
                }

            }
            catch (Exception)
            {
                sb.Append("系统错误，请重试!!!");
            }
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取所有老师
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMarjioTeacher()
        {
            List<SelectListItem> list = Reconcile_Entity.GetTeacherAll().Select(s => new SelectListItem() { Text = s.EmpName, Value = s.EmployeeId }).ToList();//获取所有在职的专业老师
            list.Add(new SelectListItem { Text = "--请选择--", Value = "0" });
            list = list.OrderBy(l => l.Value).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 手动排课页面2
        /// </summary>
        /// <returns></returns>
        public ActionResult ManualReconcileViewTwo()
        {
            //加载所有阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.Child_grandlist = g_list;
            //获取课程类型
            List<SelectListItem> t_list = Reconcile_Com.CourseType_Entity.GetCourseTypes().Select(t => new SelectListItem() { Text = t.TypeName, Value = t.TypeName }).ToList();
            t_list.Add(new SelectListItem() { Text = "其他", Value = "0", Selected = true });
            ViewBag.Child_typelist = t_list;
            BaseDataEnumManeger baseDataEnum_Entity = new BaseDataEnumManeger();
            List<SelectListItem> schooladdress = baseDataEnum_Entity.GetsameFartherData("校区地址").Select(s => new SelectListItem() { Text = s.Name, Value = s.Id.ToString() }).ToList();
            schooladdress.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.schooladdress = schooladdress;

            List<SelectListItem> list_t = Reconcile_Entity.GetTeacherAll().Select(s => new SelectListItem() { Text = s.EmpName, Value = s.EmployeeId }).ToList();//获取所有在职的专业老师
            list_t.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            ViewBag.Teacher = list_t.OrderBy(l => l.Value).ToList();
            return View();
        }
        
        /// <summary>
        /// 获取这个阶段的有效课程
        /// </summary>
        /// <param name="grandid"></param>
        /// <returns></returns>
        public ActionResult GetGrandCurrr(int id)
        {
           List<SelectListItem> curlist= Reconcile_Com.GetGrandCurr(id).Where(c=>c.IsDelete==false).Select(c=>new SelectListItem() { Text=c.CourseName,Value=c.CurriculumID.ToString()}).ToList();
            curlist.Add(new SelectListItem() { Text="其他",Value="0"});

            return Json(curlist,JsonRequestBehavior.AllowGet);
        }
        
        
        #endregion

        #region 非专业课排课
        public ActionResult AnpaiNotMarginView()
        {
            //加载阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.orthergrand = g_list.OrderBy(g => g.Value).ToList();
            CourseTypeBusiness CurseType_Entity = new CourseTypeBusiness();
            BaseDataEnumManeger dataEnum_Entity = new BaseDataEnumManeger();
            //获取所有有效的课程类型
            List<SelectListItem> find_cur_list = CurseType_Entity.GetCourseTypes().Where(c => !c.TypeName.Contains("专业") && !c.TypeName.Contains("语文") && !c.TypeName.Contains("数学")).Select(c => new SelectListItem() { Text = c.TypeName, Value = c.Id.ToString() }).ToList();
            ViewBag.select = find_cur_list;

            //获取有效校区
            List<SelectListItem> schoolAddress = dataEnum_Entity.GetsameFartherData("校区地址").Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList();

            ViewBag.schoolAddress = schoolAddress;

            return View();
        }

        /// <summary>
        /// 获取所有非专业课的老师
        /// </summary>
        /// <returns></returns>
        public ActionResult GetMasterAlldata()
        {
            string type_name = Request.Form["Type"];

            List<SelectListItem> Item = new List<SelectListItem>();
            if (type_name.Contains("职素"))
            {
                Item = Reconcile_Entity.GetMaster_All(false).Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            }
            else if (type_name.Contains("军事"))
            {
                Item = Reconcile_Entity.GetInstructorAll().Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            }
            else if (type_name.Contains("英语"))
            {
                Item = Reconcile_Entity.GetEnglishTeacherAll().Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList(); ;
            }
            else if (type_name.Contains("班会"))
            {
                Item = Reconcile_Entity.GetMaster_All(true).Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            }

            return Json(Item, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EmptyClassRoom()
        {
            int schoold_Id = Convert.ToInt32(Request.Form["address"]);
            List<SelectListItem> classroomlist = Reconcile_Com.Classroom_Entity.GetAddreeClassRoom(schoold_Id).Select(c => new SelectListItem() { Text = c.ClassroomName, Value = c.Id.ToString() }).ToList();
            return Json(classroomlist, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取所有语数英老师
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOrtherTeacher()
        {
            List<GrandClass> data = new List<GrandClass>();
            string[] str = { "语文老师", "数学老师", "英语老师" };
            foreach (string name in str)
            {
                GrandClass new_d = new GrandClass();
                new_d.Grand_Name = name;
                List<simpleDataClass> s = Reconcile_Entity.GetEmployees(name).Select(l => new simpleDataClass() { name = l.EmpName, Id = 0, PID = l.EmployeeId }).ToList();
                new_d.Class_list = s;
                data.Add(new_d);
            }

            return Json(data, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 安排非专业课程
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AnpaiNotMarginFunction()
        {
            DateTime date = Convert.ToDateTime(Request.Form["anpaitime"]);//排课开始日期
            int currrtype = Convert.ToInt32(Request.Form["currtype"]);//获取安排的课程类型
            string timename = Request.Form["timename"];//上课时间           
            int classid = Convert.ToInt32(Request.Form["orther_myclass"]);//获取班级
            int classroomid = Convert.ToInt32(Request.Form["Classroom_input"]);//获取教室
            string teacherid = Request.Form["teacher_id"];//获取老师​
            int grand_id = Convert.ToInt32(Request.Form["ortherGrand"]);//阶段
            int days = Convert.ToInt32(Request.Form["days"]);//星期
            List<Reconcile> list = Reconcile_Entity.AnPaiNoMarginFuntion(date, classid, classroomid, teacherid, timename, currrtype, grand_id, days);
            AjaxResult a = Reconcile_Entity.Inser_list(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetClassTimeName(int id)
        {
            ClassSchedltData new_c = new ClassSchedltData();
            BaseDataEnumManeger basedata_entity = new BaseDataEnumManeger();
            ClassSchedule find_c = Reconcile_Com.ClassSchedule_Entity.GetEntity(id);
            BaseDataEnum find_b = basedata_entity.GetEntity(find_c.BaseDataEnum_Id);
            AjaxResult a = new AjaxResult();
            if (find_b != null)
            {
                a.Success = true;
                a.Data = find_b.Name;
            }
            else
            {
                a.Success = false;
                a.Msg = "数据错误，请重试";
            }
            return Json(a, JsonRequestBehavior.AllowGet);

        }
        #endregion         

        #region 排课表的查询,删除,编辑
        public ActionResult SerachReconcile_Index()
        {
            //加载阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;
            return View();
        }

        public ActionResult GetReconAllData(int page, int limit)
        {
            List<ReconcileView> all = Reconcile_Entity.SQLGetReconcileDate().OrderBy(r => r.AnPaiDate).ToList();//获取所有排课数据                   
            string class_select1 = Request.QueryString["class_select1"];
            string starTime = Request.QueryString["starTime"];
            string endTime = Request.QueryString["endTime"];
            string teachername = Request.QueryString["teachername"];
            string Time2 = Request.QueryString["Time2"];
            string curr_select1 = Request.QueryString["curr_select1"];

            if (!string.IsNullOrEmpty(class_select1))
            {
                int class_id = Convert.ToInt32(class_select1);
                if (class_id != 0)
                {
                    all = all.Where(r => r.ClassSchedule_Id == class_id).ToList();
                }

            }

            if (!string.IsNullOrEmpty(starTime))
            {
                DateTime dd = Convert.ToDateTime(starTime);
                all = all.Where(r => r.AnPaiDate >= dd).ToList();
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                DateTime dd = Convert.ToDateTime(endTime);
                all = all.Where(r => r.AnPaiDate <= dd).ToList();
            }

            if (!string.IsNullOrEmpty(teachername))
            {
                List<EmployeesInfo> fin_lsite = Reconcile_Com.Employees_Entity.GetAll().Where(e => e.EmpName == teachername).ToList();
                if (fin_lsite.Count == 1)
                {
                    all = all.Where(r => r.EmployeesInfo_Id == fin_lsite[0].EmployeeId).ToList();
                }
                else if (fin_lsite.Count == 2)
                {
                    all = all.Where(r => r.EmployeesInfo_Id == fin_lsite[0].EmployeeId || r.EmployeesInfo_Id == fin_lsite[1].EmployeeId).ToList();
                }
            }

            if (!string.IsNullOrEmpty(Time2))
            {
                DateTime dd = Convert.ToDateTime(Time2);
                all = all.Where(r => r.AnPaiDate == dd).ToList();
            }

            if (!string.IsNullOrEmpty(curr_select1))
            {
                if (curr_select1 != "0")
                {
                    all = all.Where(r => r.Curriculum_Id == curr_select1).ToList();
                }
            }
            var mydata = all.Skip((page - 1) * limit).Take(limit).ToList();

            var jsondata = new { code = 0, msg = "", data = mydata, count = all.Count };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            AjaxResult a = Reconcile_Entity.DeleteReconcile(id);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCurrrlistdata(int id)
        {
            List<SelectListItem> listItems = Reconcile_Com.Curriculum_Entity.GetList().Where(c => c.Grand_Id == id && c.CourseName != "英语").Select(c => new SelectListItem() { Text = c.CourseName, Value = c.CourseName }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Edit_rView(int id)
        {
            Reconcile find_r = Reconcile_Entity.GetEntity(id);
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "上午", Value = "上午" });
            list.Add(new SelectListItem() { Text = "下午", Value = "下午" });
            list.Add(new SelectListItem() { Text = "上午1/2节", Value = "上午12节" });
            list.Add(new SelectListItem() { Text = "上午3/4节", Value = "上午34节" });
            list.Add(new SelectListItem() { Text = "下午1/2节", Value = "下午12节" });
            list.Add(new SelectListItem() { Text = "下午3/4节", Value = "下午34节" });
            ViewBag.Time = list;
            ReconView data = new ReconView()
            {
                id = find_r.Id,
                class_name = Reconcile_Com.ClassSchedule_Entity.GetEntity(find_r.ClassSchedule_Id).ClassNumber,
                classroom_name = find_r.ClassRoom_Id == null ? null : Reconcile_Com.Classroom_Entity.GetEntity(find_r.ClassRoom_Id).ClassroomName,
                currname = find_r.Curriculum_Id,
                teachername = find_r.EmployeesInfo_Id == null ? null : Reconcile_Com.Employees_Entity.GetEntity(find_r.EmployeesInfo_Id).EmpName,
                curdname = find_r.Curse_Id,
                anpaidate = find_r.AnPaiDate.Year + "-" + find_r.AnPaiDate.Month + "-" + find_r.AnPaiDate.Day,
                classId = find_r.ClassSchedule_Id,
                ramak = find_r.Rmark
            };
            return View(data);
        }
        [HttpPost]
        public ActionResult EditFunction(ReconView new_r)
        {
            Reconcile find = Reconcile_Entity.GetEntity(new_r.id);
            string classid = Request.Form["Editclass_id"];
            string classroomid = Request.Form["Editclassroom_id"];
            string curid = Request.Form["EditCurr_id"];
            string teacherid = Request.Form["EditTeacher_Id"];
            if (!string.IsNullOrEmpty(classid))
            {
                find.ClassSchedule_Id = Convert.ToInt32(classid);
            }
            if (!string.IsNullOrEmpty(classroomid))
            {
                find.ClassRoom_Id = Convert.ToInt32(classroomid);
            }
            if (!string.IsNullOrEmpty(curid))
            {
                find.Curriculum_Id = curid;
            }
            if (!string.IsNullOrEmpty(teacherid) && teacherid != "-1")
            {
                find.EmployeesInfo_Id = teacherid;
            }
            else if (teacherid == "-1")
            {
                find.EmployeesInfo_Id = null;
            }
            find.AnPaiDate = Convert.ToDateTime(new_r.anpaidate);
            find.Rmark = new_r.ramak;
            find.Curse_Id = new_r.curdname;

            AjaxResult a = Reconcile_Entity.UpdateSingleData(find);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        //编辑数据所选数据页面

        public ActionResult EditOrtherViewData(string id)
        {
            ViewBag.curtype = id.Split(',')[0];
            string name = id.Split(',')[0];
            int class_id = Convert.ToInt32(id.Split(',')[1]);
            ClassSchedule find_class = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_id);
            string major = null;
            if (find_class.Major_Id != null)
            {
                major = find_class.Major_Id.ToString();
            }
            if (name == "课程")
            {//获取所有有效的课程
                List<simpleDataClass> list_Truecur = Reconcile_Entity.GetCurr(find_class.grade_Id, true, Convert.ToInt32(major)).Select(g => new simpleDataClass() { name = g.CourseName, Id = g.CurriculumID }).ToList();
                List<simpleDataClass> list_Falsecur = Reconcile_Entity.GetCurr(find_class.grade_Id, false, Convert.ToInt32(major)).Select(g => new simpleDataClass() { name = g.CourseName, Id = g.CurriculumID }).ToList();
                List<GrandClass> list_curr = new List<GrandClass>();
                list_curr.Add(new GrandClass() { Grand_Name = "专业课程", Class_list = list_Truecur });
                list_curr.Add(new GrandClass() { Grand_Name = "非专业课程", Class_list = list_Falsecur });
                ViewBag.Truecur = list_curr;
            }
            else if (name == "老师")
            { //获取所有在职的老师

                List<SelectListItem> list_Teacher = Reconcile_Entity.GetTeacherAll().Select(t => new SelectListItem() { Text = t.EmpName, Value = t.EmployeeId }).ToList();//专业课老师
                list_Teacher.AddRange(Reconcile_Entity.GetMaster_All(true).Select(t => new SelectListItem() { Text = t.EmpName, Value = t.EmployeeId }).ToList()); //非专业课老师
                list_Teacher.AddRange(Reconcile_Entity.GetInstructorAll().Select(t => new SelectListItem() { Text = t.EmpName, Value = t.EmployeeId }).ToList());
                ViewBag.Teacher = list_Teacher;
            }
            else if (name == "班级")
            {
                //获取所有有效的班级
                List<ClassSchedule> classdata = Reconcile_Com.GetClass();
                List<Grand> granddata = Reconcile_Com.GetGrand_Id();
                List<GrandClass> list_GrandClass = new List<GrandClass>();
                foreach (Grand g in granddata)
                {
                    GrandClass new_data = new GrandClass();
                    new_data.Grand_Name = g.GrandName;
                    new_data.Class_list = classdata.Where(c => c.grade_Id == g.Id).Select(c => new simpleDataClass() { Id = c.id, name = c.ClassNumber }).ToList();
                    list_GrandClass.Add(new_data);
                }

                ViewBag.GrandClass = list_GrandClass;
            }
            else if (name == "教室")
            {//获取所有有效的教室

                BaseDataEnumManeger baseDataEnum_Entity = new BaseDataEnumManeger();
                List<BaseDataEnum> schooladdress = baseDataEnum_Entity.GetsameFartherData("校区地址").ToList();
                List<Classroom> classroom = Reconcile_Com.Classroom_Entity.GetEffectiveClass();
                List<GrandClass> list_GrandClassRoom = new List<GrandClass>();
                foreach (BaseDataEnum s in schooladdress)
                {
                    GrandClass new_data = new GrandClass();
                    new_data.Grand_Name = s.Name;
                    new_data.Class_list = classroom.Where(c => c.BaseData_Id == s.Id).Select(c => new simpleDataClass() { Id = c.Id, name = c.ClassroomName }).ToList();
                    list_GrandClassRoom.Add(new_data);
                }
                ViewBag.GrandClassRoom = list_GrandClassRoom;
            }
            return View();
        }

        /// <summary>
        /// 大批量删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMore()
        {
            string[] str = Request.Form["str"].Split(',');
            List<Reconcile> list = new List<Reconcile>();
            foreach (string item in str)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id = Convert.ToInt32(item);
                    list.Add(Reconcile_Entity.FindId(id));
                }
            }

            AjaxResult a = Reconcile_Entity.Delete_More(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 调课

        /// <summary>
        /// 所有数据调课页面
        /// </summary>
        /// <returns></returns>
        public ActionResult BigDataUpdate()
        {
            return View();
        }
        /// <summary>
        /// 所有数据调课方法
        /// </summary>
        /// <returns></returns>
        public ActionResult BigDataAID()
        {
            DateTime startme = Convert.ToDateTime(Request.Form["oldTime"]);
            DateTime endtime = Convert.ToDateTime(Request.Form["endtime"]);
            var days = endtime.Subtract(startme);
            int count = days.Days;

            GetYear years = Reconcile_Entity.MyGetYear(startme.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));
            List<Reconcile> list = Reconcile_Entity.GetReconcileDate(startme, true);
            bool s = Reconcile_Entity.AidAllData(count, years, list);
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 班级数据大批量调课
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassBigDataAID()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;
            return View();
        }
        [HttpPost]
        public ActionResult ClassBigDataFunction()
        {
            int class_id = Convert.ToInt32(Request.Form["class_select"]);
            DateTime startime = Convert.ToDateTime(Request.Form["starTime"]);
            int count = Convert.ToInt32(Request.Form["endTime"]);           
            GetYear years = Reconcile_Entity.MyGetYear(startime.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));
            bool s = true;
            if (count<=0)
            {
               s= Reconcile_Entity.DescClassData(startime, count, class_id, years);
            }
            else
            {
                 s = Reconcile_Entity.AidClassData(startime, count, class_id, years);
            }
          
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取某个班级上课时间、所在教室
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTimeRoom()
        {
            DateTime date = Convert.ToDateTime(Request.QueryString["date"]);
            int id = Convert.ToInt32(Request.QueryString["id"]);
            ClassSchedule find_c = Reconcile_Com.ClassSchedule_Entity.GetEntity(id);
            ReconcileView find_r = Reconcile_Entity.SQLGetReconcileDate().Where(r => r.ClassSchedule_Id == id && r.AnPaiDate == date).FirstOrDefault();
            int room = find_r == null ? 0 : find_r.ClassRoom_Id;
            int? address = null;
            if (find_r != null)
            {
                BaseDataEnum find_b = Reconcile_Com.Classroom_Entity.GetAddressData(Convert.ToInt32(find_r.ClassRoom_Id)); //获取校区
                if (find_b != null)
                {
                    address = find_b.Id;
                }
            }
            var data = new { room = room, address = address };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改班级上课时间段页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateClassView()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;

            //获取上课时间段
            BaseDataEnumManeger base_Entity = new BaseDataEnumManeger();
            List<SelectListItem> basesataenum = base_Entity.GetsameFartherData("上课时间类型").Select(b => new SelectListItem() { Text = b.Name, Value = b.Name }).ToList();
            ViewBag.baseE = basesataenum;

            //获取所在校区
            List<SelectListItem> addreess = base_Entity.GetsameFartherData("校区地址").Select(b => new SelectListItem() { Text = b.Name, Value = b.Id.ToString() }).ToList();
            ViewBag.baseA = addreess;

            return View();
        }

        public ActionResult UpdateClassFunction()
        {
            int class_id = Convert.ToInt32(Request.Form["class_select"]);
            string time = Request.Form["time"];
            DateTime date1 = Convert.ToDateTime(Request.Form["date"]);
            int room = Convert.ToInt32(Request.Form["room"]);
            BaseDataEnumManeger base_Entity = new BaseDataEnumManeger();
            int time2 = base_Entity.GetsameFartherData("上课时间类型").Where(b => b.Name == time).FirstOrDefault().Id;
            AjaxResult a = Reconcile_Com.ClassSchedule_Entity.Modifyclasstime(class_id, time2);
            if (a.Success == true)
            {
                //修改调课单

                string date = date1.Year + "-" + date1.Month + "-" + date1.Day;
                DateTime dd2 = Convert.ToDateTime(date);
                List<ReconcileView> find_list = Reconcile_Entity.SQLGetReconcileDate().Where(r => r.AnPaiDate >= dd2 && r.ClassSchedule_Id == class_id).ToList();
                AjaxResult a2 = Reconcile_Entity.Update_data2(find_list, time, room);
                if (a2.Success == true && a.Success == true)
                {
                    return Json(a2, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    a2.Msg = "修改排课数据失败，请刷新页面重试";
                    return Json(a2, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(a, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult UpdateClassTeacher()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;

            //获取所有的老师

            return View();
        }
        /// <summary>
        /// 获取所有上专业课老师
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTecherAll(int id)
        {

            List<TeacherData> tlist = new List<TeacherData>();
            List<Teacher> teachers = Reconcile_Com.Teacher_Entity.GetTeachers();
            foreach (Teacher item in teachers)
            {
                EmployeesInfo find_e = Reconcile_Entity.dbemployeesInfo.GetEntity(item.EmployeeId);


                if (find_e != null)
                {
                    string name = Reconcile_Com.PositionBusiness.GetEntity(find_e.PositionId).PositionName;
                    if (name != "英语老师" || name != "数学老师" || name != "语文老师")
                    {
                        TeacherData t = new TeacherData();
                        t.Emp_id = find_e.EmployeeId;
                        t.Name = find_e.EmpName;
                        t.Teacher_id = item.TeacherID;
                        tlist.Add(t);
                    }

                }


            }
            //获取该班级的专业老师

            ClassTeacher find_ct = Reconcile_Entity.TeacherClass_Entity.GetList().Where(t => t.ClassNumber == id && t.IsDel == false).FirstOrDefault();
            int teacher_id = find_ct == null ? 0 : Convert.ToInt32(find_ct.TeacherID);
            var data = new { t_id = teacher_id, list_teacher = tlist };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateClassTeacherFuntion()
        {
            AjaxResult a = new AjaxResult();
            try
            {
                int class_id = Convert.ToInt32(Request.Form["class_select"]);//获取班级
                string teacher_id = Request.Form["teacher_id"];//获取老师姓名
                                                              
                EmployeesInfo find_e = Reconcile_Entity.dbemployeesInfo.FindEmpData(teacher_id, false); //判断这个教员是有
                if (find_e == null)
                {
                    a.Msg = "教员姓名有误！！！";
                    a.Success = false;
                    return Json(a, JsonRequestBehavior.AllowGet);
                }
                DateTime time = Convert.ToDateTime(Request.Form["startime"]);//获取修改的时间
                 
                List<Reconcile> find_list = Reconcile_Entity.Class_Date(class_id, time);//获取属于这个班级的所有排课数据

                List<Reconcile> TureEditData = new List<Reconcile>();//修改专业课的任课老师
                foreach (Reconcile item in find_list)
                {
                    Curriculum find = Reconcile_Com.GetNameGetCur(item.Curriculum_Id);
                    if (find != null)
                    {
                        if (find.CourseType_Id == 1)
                        {
                            item.EmployeesInfo_Id = find_e.EmployeeId;
                            TureEditData.Add(item);
                        }
                    }
                }
               
                
                a = Reconcile_Entity.Update_date(TureEditData);
                if (a.Success == false)
                {
                    a.Msg = "修改排课数据错误！！！";
                }

                return Json(a, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                a.Success = false;
                a.Msg = "系统错误！！！";
                return Json(a, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 全体上课日期调换
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangReconciledata()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangReconcileFunction()
        {
            DateTime startime = Convert.ToDateTime(Request.Form["starTime"]);
            DateTime endtime = Convert.ToDateTime(Request.Form["endTime"]);
            List<ClassSchedule> class_s = Reconcile_Com.GetClass();
            List<ReconcileView> lisr_r = Reconcile_Entity.SQLGetReconcileDate().Where(r => r.AnPaiDate == startime).ToList();
            List<ReconcileView> find_lidt = new List<ReconcileView>();
            foreach (ReconcileView i in lisr_r)
            {
                int count = class_s.Where(s => s.id == i.ClassSchedule_Id).ToList().Count;
                if (count > 0)
                {
                    find_lidt.Add(i);
                }
            }
            AjaxResult a = Reconcile_Entity.update_date4(find_lidt, endtime);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 指定班级上课日期调换
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangClassDateView()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;
            return View();
        }
        [HttpPost]
        public ActionResult ChangClassDataFunction()
        {
            int class_id = Convert.ToInt32(Request.Form["class_select"]);
            DateTime startime = Convert.ToDateTime(Request.Form["starTime"]);
            DateTime endtime = Convert.ToDateTime(Request.Form["endTime"]);
            List<ReconcileView> find_list = Reconcile_Entity.SQLGetReconcileDate().Where(r => r.ClassSchedule_Id == class_id && r.AnPaiDate == startime).ToList();
            AjaxResult a = Reconcile_Entity.update_date4(find_list, endtime);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 指定阶段调课
        /// </summary>
        /// <returns></returns>
        public ActionResult GrandChangDataView()
        {
            //获取所有阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            ViewBag.Mygrandlist = g_list;

            return View();
        }
        [HttpPost]
        public ActionResult GrandChangData()
        {
            string[] grand = Request.Form["grands"].Split(',');

            DateTime de1 = Convert.ToDateTime(Request.Form["starTime"]);

            int de2 = Convert.ToInt32(Request.Form["endTime"]);
                         
            List<ClassSchedule> myclass = new List<ClassSchedule>();

            foreach (string it in grand)
            {
                if (!string.IsNullOrEmpty(it))
                {
                    int grandid = Convert.ToInt32(it);
                    myclass.AddRange(Reconcile_Com.GetClass().Where(c => c.grade_Id == grandid).ToList());
                }
            }

            //获取属于这个日期的这些班级的排课数据

            List<Reconcile> myclist = new List<Reconcile>();
            List<Reconcile> all = Reconcile_Entity.GetList().Where(a => a.AnPaiDate >= de1).ToList();
            foreach (ClassSchedule cla in myclass)
            {
                myclist.AddRange(all.Where(a => a.ClassSchedule_Id == cla.id).ToList());
            }


            GetYear years = Reconcile_Entity.MyGetYear(de1.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));
            bool s = true;
            if (de2 <= 0)
            {
               s= Reconcile_Entity.DescData(de2, years, myclist);
            }
            else
            {
                s = Reconcile_Entity.AidAllData(de2, years, myclist);
            }
           

            return Json(s, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 阶段日期调换
        /// </summary>
        /// <returns></returns>
        public ActionResult GrandUpdatDataView()
        {
            //获取所有阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            ViewBag.Mygrandlist = g_list;
            return View();
        }

        public ActionResult GrandUpdatDataFuntion()
        {
            string[] grand = Request.Form["grands"].Split(',');

            DateTime de1 = Convert.ToDateTime(Request.Form["starTime"]);

            DateTime de2 = Convert.ToDateTime(Request.Form["endTime"]);



            List<ClassSchedule> myclass = new List<ClassSchedule>();

            foreach (string it in grand)
            {
                if (!string.IsNullOrEmpty(it))
                {
                    int grandid = Convert.ToInt32(it);
                    myclass.AddRange(Reconcile_Com.GetClass().Where(c => c.grade_Id == grandid).ToList());
                }
            }

            //获取属于这个日期的这些班级的排课数据

            List<ReconcileView> myclist = new List<ReconcileView>();
            List<ReconcileView> all = Reconcile_Entity.SQLGetReconcileDate().Where(a => a.AnPaiDate == de1).ToList();
            foreach (ClassSchedule cla in myclass)
            {
                myclist.AddRange(all.Where(a => a.ClassSchedule_Id == cla.id).ToList());
            }

            AjaxResult a1 = Reconcile_Entity.update_date4(myclist, de2);


            return Json(a1, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region 自动安排自习课
        [HttpPost]
        public ActionResult SystemMyseltStudey()
        {
            DateTime date = Convert.ToDateTime(Request.Form["date"]);//排课的日期
            string[] grands = Request.Form["grands"].Split(',');//获取要排上机课的阶段
            int address = Convert.ToInt32(Request.Form["xiaoqu"]);//获取校区Id 
            List<Classroom> roomlist = Reconcile_Com.Classroom_Entity.GetAddreeClassRoom(address);//获取XX校区的有效教室

            List<EmtyClassroom> classrooms = Reconcile_Entity.GetEmtyClassroom(address, date, roomlist);//获取所有空教室

            List<EmtyClassroom> one = classrooms.Where(c1 => c1.Time.Contains("上午")).ToList();//获取上午的空教室
            List<EmtyClassroom> two = classrooms.Where(c1 => c1.Time.Contains("下午")).ToList();//获取下午的空教室

            List<ClassSchedule> list = Reconcile_Com.GetClass();
            List<ClassSchedule> cla1 = new List<ClassSchedule>();// 获取上午专业课的班级
            List<ClassSchedule> cla2 = new List<ClassSchedule>();//获取下午专业课的班级

            foreach (string ite in grands)
            {
                if (!string.IsNullOrEmpty(ite))
                {
                    int grand_id = Convert.ToInt32(ite);
                    cla1.AddRange(list.Where(l => l.grade_Id == grand_id && l.BaseDataEnum_Id == 1).ToList());
                    cla2.AddRange(list.Where(l => l.grade_Id == grand_id && l.BaseDataEnum_Id == 2).ToList());
                }
            }

            List<Reconcile> listr = Reconcile_Entity.SysclassStuty(cla2, one, date);
            listr.AddRange(Reconcile_Entity.SysclassStuty(cla1, two, date));
            listr = Reconcile_Entity.DeleteOrrdieData(listr);
            AjaxResult a = Reconcile_Entity.AddData(listr);
            return Json(a, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 安排自习页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SysView()
        {
            //加载阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            ViewBag.grandlist = g_list;
            //加载校区
            BaseDataEnumManeger baseDataEnum_Entity = new BaseDataEnumManeger();
            List<SelectListItem> schooladdress = baseDataEnum_Entity.GetsameFartherData("校区地址").Select(s => new SelectListItem() { Text = s.Name, Value = s.Id.ToString() }).ToList();
            ViewBag.Schooladdress = schooladdress;
            return View();
        }
        #endregion

        #region 加课


        #endregion

    }
}