﻿using SiliconValley.InformationSystem.Business.EducationalBusiness;
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

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    public class ReconcileController : Controller
    {
        // GET: /Educational/Reconcile/GetEmptyRoom
          static readonly ReconcileManeger Reconcile_Entity = new ReconcileManeger();
          static readonly EmployeesInfoManage Employees_Entity = new EmployeesInfoManage();
          static readonly TeacherBusiness Teacher_Entity = new TeacherBusiness();
          static Recon_Login_Data GetBaseData(string Emp)
        {
            Recon_Login_Data new_re = new Recon_Login_Data();
            EmployeesInfo employees= Employees_Entity.GetEntity(Emp);
            //获取部门
            DepartmentManage department = new DepartmentManage();
            Department find_d1= department.GetList().Where(d => d.DeptName == "s1、s2教学部").FirstOrDefault();
            Department find_d2 = department.GetList().Where(d => d.DeptName == "s3教学部").FirstOrDefault();
            //获取岗位
            PositionManage position = new PositionManage();
            Position find_p = position.GetEntity(employees.PositionId);
            if (find_p.PositionName=="教务" && find_p.DeptId==find_d1.DeptId)
            {
                //s1.s2教务
                new_re.ClassRoom_Id = ReconcileManeger.ClassSchedule_Entity.BaseDataEnum_Entity.GetSingData("继善高科校区", false).Id;
                new_re.IsOld = true;
            }
            else
            {
                //s3教务
                new_re.ClassRoom_Id = ReconcileManeger.ClassSchedule_Entity.BaseDataEnum_Entity.GetSingData("达嘉维康校区", false).Id;
                new_re.IsOld = false;
            }
            return new_re;
        }
        //获取当前登录员是哪个校区的教务
          static int base_id = GetBaseData("201911190040").ClassRoom_Id;
          static bool IsOld = GetBaseData("201911190040").IsOld;//确定教务
        #region 高中生课表安排
        public ActionResult ReconcileIndexViews()
        {
            //加载阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData(IsOld).Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;            
            //加载教室             
            ViewBag.classrrrom= Reconcile_Entity.GetEffectioveClassRoom(base_id).Select(c=>new SelectListItem() { Text=c.ClassroomName,Value=c.Id.ToString()}).ToList();
            return View();
        }
        /// <summary>
        /// 通过阶段获取班级
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetClassScheduleSelect(int id)
        {
            var c_list= Reconcile_Entity.GetGrandClass(id).ToList();
            return Json(c_list,JsonRequestBehavior.AllowGet);
        }         
        /// <summary>
        /// 通过班级名称获取班级其他数据
        /// </summary>
        /// <param name="id">班级名称</param>
        /// <returns></returns>
        public ActionResult GetClassDate(string id)
        {
            if (!string.IsNullOrEmpty(id) && id!="0")
            {
                ClassSchedltData new_c = new ClassSchedltData();
                ClassSchedule find_c = ReconcileManeger.ClassSchedule_Entity.GetEntity(id);
                new_c.Name = find_c.ClassNumber;//班级名称
                string marjon = ReconcileManeger.ClassSchedule_Entity.GetClassGrand(find_c.ClassNumber, 1);//专业
                new_c.marjoiName = marjon;
                string grand = ReconcileManeger.ClassSchedule_Entity.GetClassGrand(find_c.ClassNumber, 2);//阶段
                new_c.GrandName = grand;
                string time = ReconcileManeger.ClassSchedule_Entity.GetClassTime(find_c.ClassNumber);//上课时间类型
                new_c.ClassDate = time;
                //获取某个阶段某个专业的所有课程                 
                
                if (marjon=="无")
                {
                    int grand_id = ReconcileManeger.Grand_Entity.FindNameGetData(grand).Id;
                   var find_clist = ReconcileManeger.Curriculum_Entity.GetList().Where(c=>c.Grand_Id== grand_id && c.MajorID==null && c.IsDelete==false).Select(c => new { CourseName = c.CourseName, CurriculumID = c.CurriculumID }).ToList();
                    var josndata = new { classData = new_c, c_list = find_clist, stataus = "true" };
                    return Json(josndata, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var find_clist = ReconcileManeger.Curriculum_Entity.GetRelevantCurricul(ReconcileManeger.Grand_Entity.FindNameGetData(grand).Id, ReconcileManeger.Specialty_Entity.FindNameSame(marjon).Id).Select(c => new { CourseName = c.CourseName, CurriculumID = c.CurriculumID }).ToList();
                    var josndata = new { classData = new_c, c_list = find_clist, stataus = "true" };
                    return Json(josndata, JsonRequestBehavior.AllowGet);
                }                 
            }
            else
            {
                var josndata = new { classData = "", c_list = "", stataus = "false" };
                return Json(josndata, JsonRequestBehavior.AllowGet);
            }
            
        }
        //排课
        [HttpPost]
        public ActionResult PaikeFunction()
        {
            TeacherClassBusiness TeacherClass_Entity = new TeacherClassBusiness();
            
            StringBuilder db = new StringBuilder();
            string grand_Id = Request.Form["mygrand"];
            string class_Id =Request.Form["class_select"];
            int classroom_Id =Convert.ToInt32(Request.Form["myclassroom"]);
            string kengcheng = Request.Form["kecheng"];
            string time = Request.Form["time"];
            DateTime startTime =Convert.ToDateTime( Request.Form["startTime"]);
            //判断该班级这个课程是否已排完课
            int count= Reconcile_Entity.GetList().Where(r => r.Curriculum_Id == kengcheng && r.ClassSchedule_Id == class_Id).ToList().Count;
            if (count>0)
            {
                string coursename= ReconcileManeger.Curriculum_Entity.GetEntity(kengcheng).CourseName;
                db.Append(class_Id + "的" + coursename + "已排好,请选择其他课程");
               
            }
            else
            {
                //判断该课程是否安排了教学老师
                ClassTeacher Ishave= TeacherClass_Entity.GetList().Where(t => t.IsDel == false && t.ClassNumber == class_Id).FirstOrDefault();
                if (Ishave!=null)
                {
                    //开始排课
                    Curriculum find_c = ReconcileManeger.Curriculum_Entity.GetList().Where(c => c.CourseName == kengcheng).FirstOrDefault();
                    //查看这个课程的课时数
                    int Kcount = Convert.ToInt32(find_c.CourseCount) / 4;
                    //获取单休双休月份
                    GetYear find_g = Reconcile_Entity.MyGetYear("2019", Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));
                    List<Reconcile> new_list = new List<Reconcile>();
                    for (int i = 0; i <= Kcount; i++)
                    {
                        Reconcile r = new Reconcile();
                        //判断是否是单休
                        if (startTime.Month >= find_g.StartmonthName && startTime.Month <= find_g.EndmonthName)
                        {
                            //单休
                            if (Reconcile_Entity.IsSaturday(startTime.AddDays(i)) == 2)
                            {
                                //如果是周日
                                r.AnPaiDate = startTime.AddDays(i + 1);
                                i++;
                                Kcount++;
                            }
                            else
                            {
                                r.AnPaiDate = startTime.AddDays(i);
                            }
                        }
                        else
                        {
                            //双休
                            if (Reconcile_Entity.IsSaturday(startTime.AddDays(i)) == 1)
                            {
                                //如果是周六
                                r.AnPaiDate = startTime.AddDays(i + 2);
                                i = i + 2;
                                Kcount = Kcount + 2;
                            }
                            else
                            {
                                r.AnPaiDate = startTime.AddDays(i);
                            }
                        }
                        r.ClassRoom_Id = classroom_Id;
                        r.ClassSchedule_Id = class_Id;
                        r.EmployeesInfo_Id = Teacher_Entity.GetEntity(Ishave.TeacherID).EmployeeId;
                        if (i == Kcount)
                        {
                            //课程考试
                            bool iscurr = Reconcile_Entity.IsEndCurr(kengcheng);
                            if (iscurr)
                            {
                                r.Curriculum_Id = "升学考试";
                            }
                            else
                            {

                            }
                            r.Curriculum_Id = kengcheng + "考试";
                        }
                        else
                        {
                            r.Curriculum_Id = kengcheng;
                        }
                        r.NewDate = DateTime.Now;
                        r.Curse_Id = time;
                        r.IsDelete = false;
                        new_list.Add(r);
                    }
                    if (Reconcile_Entity.IsExcit(new_list[0]))
                    {
                        db.Append(new_list[0].ClassSchedule_Id+"的"+new_list[0].Curriculum_Id+"已安排！！！");
                    }
                    else
                    {
                        if (Reconcile_Entity.Inser_list(new_list))
                        {
                            db.Append("ok");
                        }
                    }
                     
                }
                else
                {
                    db.Append("请联系教学主任安排"+ class_Id+"班级"+"的"+ kengcheng+"课程的教学老师！！！");
                }
                           
            }
            return Json(db.ToString(),JsonRequestBehavior.AllowGet);
        }
        //排课数据
        public ActionResult GetTableData(int limit ,int page)
        {
            string classname = Request.QueryString["classname"];//班级名称
            if (string.IsNullOrEmpty(classname))
            {                   
                return Json(new { code = 0, msg = "", count = 0, data = "" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<Reconcile> lisr_r = Reconcile_Entity.GetList().Where(r => r.ClassSchedule_Id == classname).ToList();
                var mydata = lisr_r.Skip((page - 1) * limit).Take(limit).Select(r => new {
                    Id = r.Id,
                    classname = r.ClassSchedule_Id,//班级名称
                    classroom = r.ClassRoom_Id==null?"无": ReconcileManeger.Classroom_Entity.GetEntity(r.ClassRoom_Id).ClassroomName,//教室
                    curriName = r.Curriculum_Id,//课程
                    Sketime = r.Curse_Id,//课程时间字段
                    ADate = r.AnPaiDate,
                    Teacher= r.EmployeesInfo_Id==null?"无":Employees_Entity.GetEntity(r.EmployeesInfo_Id).EmpName
                });
                var jsondata = new { code = 0, msg = "", count = lisr_r.Count, data = mydata };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
             
        }
        //修改排课数据页面
        public ActionResult EditView(int id)
        {
            //获取有效教室
            int base_id = ReconcileManeger.ClassSchedule_Entity.BaseDataEnum_Entity.GetSingData("继善高科校区", false).Id;
            ViewBag.Editclassrrrom = Reconcile_Entity.GetEffectioveClassRoom(base_id).Select(c => new SelectListItem() { Text = c.ClassroomName, Value = c.Id.ToString() }).ToList();
            Reconcile find_r= Reconcile_Entity.GetEntity(id);
            return View(find_r);
        }

        //生成课表页面
        public ActionResult NewsRexoncileView()
        {
            //Mydate d= Reconcile_Entity.GetMydate(Convert.ToDateTime("2019-11-12"));
            //获取当前登录员是哪个角色是哪个校区的教务
            int base_id = ReconcileManeger.ClassSchedule_Entity.BaseDataEnum_Entity.GetSingData("继善高科校区", false).Id;
            Reconcile_Entity.mmm(Convert.ToDateTime("2019-11-13"), base_id);
            return View();
        }
        //手动排课页面
        public ActionResult ManualReconcileView()
        {
            //获取阶段
            //加载所有阶段
            List<SelectListItem> g_list = Reconcile_Entity.GetEffectiveData(IsOld).Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.Child_grandlist = g_list;
            //获取课程类型
            List<SelectListItem> t_list= ReconcileManeger.CourseType_Entity.GetCourseTypes().Select(t=>new SelectListItem() { Text=t.TypeName,Value=t.TypeName}).ToList();
            t_list.Add(new SelectListItem() { Text="其他",Value="0",Selected=true});
            ViewBag.Child_typelist= t_list;
            return View();
        }
        //获取空教室
        [HttpPost]
        public ActionResult GetEmptyRoom()
        {
            string timeName= Request.Form["timeName"];
            DateTime Time =Convert.ToDateTime( Request.Form["Time"]);

           List<Classroom> c_list= Reconcile_Entity.GetClassrooms(timeName, base_id, Time);
            c_list.Add(new Classroom() {ClassroomName="--请选择--",Id=0});
            List<Classroom> c_list2 = c_list.OrderBy(c => c.Id).ToList();
            return Json(c_list2,JsonRequestBehavior.AllowGet);
        }
        //根据班级获取课程
        public ActionResult CaseClassGetCurr(string id)
        {
            List<Curriculum> c_list = new List<Curriculum>();
            string find_m= ReconcileManeger.ClassSchedule_Entity.GetClassGrand(id, 1);//专业名称

            string fing_g= ReconcileManeger.ClassSchedule_Entity.GetClassGrand(id, 2);//阶段名称
            Grand find_grand= ReconcileManeger.Grand_Entity.FindNameGetData(fing_g);
            if (find_grand!=null)
            {
                if (find_m=="无")
                {
                    c_list= ReconcileManeger.Curriculum_Entity.GetRelevantCurricul(find_grand.Id, null);
                }
                else
                {
                    //获取专业Id
                    Specialty find_s = ReconcileManeger.Specialty_Entity.FindNameSame(find_m);
                    if (find_s != null)
                    {
                        c_list = ReconcileManeger.Curriculum_Entity.GetRelevantCurricul(find_grand.Id, find_s.Id);
                    }
                }
                 
            }
            c_list.Add(new Curriculum() { CourseName = "--请选择--", CurriculumID = 0 });
            c_list=c_list.OrderBy(c => c.CurriculumID).ToList();
            return Json(c_list,JsonRequestBehavior.AllowGet);
        }
        //根据专业课程获取专业老师
        public ActionResult GetTeacher(int id)
        {
            List<Teacher> find_t= ReconcileManeger.GoodSkill_Entity.GetTeachers(id);
            List<EmployeesInfo> all= Employees_Entity.GetList();
            List<EmployeesInfo> find_e = new List<EmployeesInfo>();
            foreach (EmployeesInfo item1 in all)
            {
                foreach (Teacher item2 in  find_t)
                {
                    if (item2.EmployeeId==item1.EmployeeId)
                    {
                        find_e.Add(item1);
                    }
                }
            }
            List<SelectListItem> result= find_e.Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            return Json(result,JsonRequestBehavior.AllowGet); 
        }
        //获取非专业课的老师
        public ActionResult GetNoMajoiThercher(string id)
        {
            List<SelectListItem> select = new List<SelectListItem>();
            switch (id)
            {
                case "职素":
                    //获取可以上职素课的班主任
                    select= Reconcile_Entity.GetMasTeacher().Select(m => new SelectListItem() { Text = Employees_Entity.GetEntity(m.informatiees_Id).EmpName, Value = m.informatiees_Id }).ToList();
                    break;
                case "军事":
                    //获取教官
                    select= Reconcile_Entity.GetSir().Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
                    break;
            }

            return Json(select, JsonRequestBehavior.AllowGet);
        }
        //手动排课数据提交
        [HttpPost]
        public ActionResult AddHandDataFunction()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                Reconcile new_r = new Reconcile();
                //获取班级
                new_r.ClassSchedule_Id = Request.Form["child_class"];
                //获取类型
                string type = Request.Form["childview_Currtype"];
                if (type.Contains("专业"))
                {
                    //获取课程Id                 
                    int cur_id = Convert.ToInt32(Request.Form["childview_currname"]);
                    new_r.Curriculum_Id = ReconcileManeger.Curriculum_Entity.GetEntity(cur_id).CourseName;
                }
                else
                {
                    new_r.Curriculum_Id = Request.Form["childview_currname"];
                }
                
                //获取任课老师
                new_r.EmployeesInfo_Id = Request.Form["teacher_child"];
                //获取上课地点
                int room_Id = Convert.ToInt32(Request.Form["childview_room"]);//上课地点可能是室外
                if (room_Id!=0)
                {
                    new_r.ClassRoom_Id = room_Id;
                }                 
                //获取上课时间
                new_r.Curse_Id = Request.Form["childview_timetype"];                                                          
                new_r.IsDelete = false;
                new_r.NewDate = DateTime.Now;
                //排课的时间
                new_r.AnPaiDate = Convert.ToDateTime(Request.Form["childview_AnpiDate"]);

                Reconcile_Entity.Insert(new_r);

                sb.Append("ok");
            }
            catch (Exception)
            {
                sb.Append("系统错误，请重试!!!");
            }
            return Json(sb.ToString(),JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 生成课表
        public ActionResult GetGenerateTimetable()
        {
            //获取XX校区的所有教室
            List<Classroom> c_list = ReconcileManeger.Classroom_Entity.GetList().Where(c => c.BaseData_Id == base_id).OrderBy(c => c.Id).ToList();
            ViewBag.classroom_all= c_list.Select(c=>new SelectListItem() { Text=c.ClassroomName,Value=c.Id.ToString()}).ToList();
            //获取所有教室的上午班级上课情况
            ViewBag.mongingOne= Reconcile_Entity.GetPaiDatas(Convert.ToDateTime("2019-11-19"), "上午12节", c_list);
            ViewBag.mongingTwo = Reconcile_Entity.GetPaiDatas(Convert.ToDateTime("2019-11-19"), "上午34节", c_list);
            //下午
            ViewBag.afternoonOne = Reconcile_Entity.GetPaiDatas(Convert.ToDateTime("2019-11-19"), "下午12节", c_list);
            ViewBag.afternoonTwo = Reconcile_Entity.GetPaiDatas(Convert.ToDateTime("2019-11-19"), "下午34节", c_list);
            //晚自习
            return View();
        }

        #endregion
    }
}