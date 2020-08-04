using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.DepartmentBusiness;
using SiliconValley.InformationSystem.Business.EducationalBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.PositionBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    [CheckLogin]
    public class TeacherNightController : Controller
    {
        // GET: /Educational/TeacherNight/TeacherSerch

        TeacherNightManeger TeacherNight_Entity = new TeacherNightManeger();
        TeacherBusiness Teacher_Entity;
        //ClassroomManeger Classoom_Entity;
        BeOnDutyManeger beOnDuty_Entity = new BeOnDutyManeger(); //获取教员晚自习
        #region  教员晚自习值班
        public ActionResult TeacherNightViewIndex()
        {
            //获取所有老师
            Teacher_Entity = new TeacherBusiness();
            List<SelectListItem> teacherlist = Teacher_Entity.GetTeacherEmps().Select(e=>new SelectListItem() { Text=e.EmpName,Value=e.EmployeeId}).ToList();
            teacherlist.Add(new SelectListItem() { Text="--请选择--",Value="0"});
            teacherlist = teacherlist.OrderBy(t => t.Value).ToList();
            ViewBag.teacher = teacherlist;
            return View();
        }

        public ActionResult TeacherTableData(int page, int limit)
        {
            int id = beOnDuty_Entity.GetSingleBeOnButy("教员晚自习", false).Id;
            TeacherNight_Entity = new TeacherNightManeger();
            List<TeacherNightView> getall = new List<TeacherNightView>();
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            int forginKay = TeacherNight_Entity.IsShowData(UserName.EmpNumber);
            if (forginKay==0)
            {
                getall = TeacherNight_Entity.AccordingtoEmpGetData(false, "",id);//获取所有数据
            }else if (forginKay==1 || forginKay == 2 || forginKay == 3)
            {
                //获取登录人所在部门
               List<EmployeesInfo> find= TeacherNight_Entity.AccordingtoEmplyess(UserName.EmpNumber);
                getall= TeacherNight_Entity.AccordingtoDepartMentData(find, id);
            }else
            {
                getall= TeacherNight_Entity.AccordingtoEmpGetData(true, UserName.EmpNumber,id);
            }
            string tid= Request.QueryString["tid"];
            string old = Request.QueryString["olddate"];
            string news = Request.QueryString["newdate"];
            if (!string.IsNullOrEmpty(tid) && tid!="0")
            {
                getall = getall.Where(g => g.Tearcher_Id == tid).ToList();
            }

            if (!string.IsNullOrEmpty(old))
            {
                DateTime date = Convert.ToDateTime(old);
                getall = getall.Where(g => g.OrwatchDate >= date).ToList();
            }

            if (!string.IsNullOrEmpty(news))
            {
                DateTime date = Convert.ToDateTime(news);
                getall = getall.Where(g => g.OrwatchDate <= date).ToList();
            }
            var data = getall.OrderByDescending(t => t.Id).Skip((page - 1) * limit).Take(limit).ToList();
            var jsondata = new { count = getall.Count, code = 0, msg = "", data = data };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AnpaiTeacherNight()
        {
            return null;
        }

        /// <summary>
        /// 模糊查询教员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TeacherSerch()
        {
            AjaxResult a = new AjaxResult();
            Teacher_Entity = new TeacherBusiness();
            string teachername= Request.Form["teachername"];

            List<SelectListItem> teacherlist = Teacher_Entity.GetTeacherEmps().Where(t => t.EmpName.Contains(teachername)).Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            a.Success = true;
            a.Data = teacherlist;

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddDataView()
        {            
            List<SelectListItem> sle_grand = Reconcile_Com.GetGrand_Id().Select(cl => new SelectListItem() { Text = cl.GrandName, Value = cl.Id.ToString() ,Selected=false}).ToList();  //获取阶段
            sle_grand.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.myclass = sle_grand;
 
            return View();
        }

        #region 系统安排晚自习

        /// <summary>
        /// 系统自动安排晚自习方法
        /// </summary>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult SystemAnpaiFunction()
        //{
        //    TeacherNight_Entity = new TeacherNightManeger();
        //    string[] times = Request.Form["times"].Split('到');
        //    DateTime start = Convert.ToDateTime(times[0]);
        //    DateTime end = Convert.ToDateTime(times[1]);
        //    string[] grand_id = Request.Form["str"].Split(',');
        //    //获取这个阶段的班级
        //    List<ClassSchedule> grand_class_list = new List<ClassSchedule>();
        //    foreach (string item in grand_id)
        //    {
        //        if (!string.IsNullOrEmpty(item))
        //        {
        //            int gid = Convert.ToInt32(item);
        //            grand_class_list.AddRange( Reconcile_Com.GetClass().Where(c=>c.grade_Id==gid));
        //        }
        //    }

        //    AjaxResult a = TeacherNight_Entity.AnpaiNight(start, end,grand_class_list);
        //    return Json(a, JsonRequestBehavior.AllowGet);
        //}
        #endregion
       
        

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DeleteFunction(string id)
        {
            List<TeacherNight> DELE = new List<TeacherNight>();
            AjaxResult a = new AjaxResult();
            string[] ids= id.Split(',');
            foreach (string item in ids)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int tid = Convert.ToInt32(item);
                    DELE.Add(TeacherNight_Entity.GetEntity(tid));
                }
            }
             
            a = TeacherNight_Entity.My_Delete(DELE);
            if (a.Success)
            {
                //修改晚自习数据
                List<EvningSelfStudy> updateTeacher = new List<EvningSelfStudy>();
                foreach (TeacherNight item in DELE)
                {
                   List<EvningSelfStudy> findlist = TeacherNight_Entity.EvningSelfStudent_Entity.GetSQLDat("select * from EvningSelfStudy where ClassSchedule_id=" + item.ClassSchedule_Id + " and Anpaidate='" + item.OrwatchDate + "'");

                    //findlist[0].emp_id = null;
                    updateTeacher.Add(findlist[0]);
                }
              a=  TeacherNight_Entity.EvningSelfStudent_Entity.Update_Data(updateTeacher);
            }
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EditView(int id)
        {
            
            TeacherNight_Entity = new TeacherNightManeger();//获取数据
            TeacherNight find_data = TeacherNight_Entity.GetEntity(id);
            ViewBag.time = find_data.OrwatchDate.ToString("yyyy-MM-dd");
            Teacher_Entity = new TeacherBusiness();
            List<SelectListItem> teacherlist = Teacher_Entity.GetTeachers().Select(t => new SelectListItem() { Text = Reconcile_Com.GetEmpName(t.EmployeeId), Value = t.EmployeeId, Selected = find_data.Tearcher_Id == t.EmployeeId ? true : false }).ToList();

            ViewBag.teacherlist = teacherlist;
            ViewBag.className = Reconcile_Com.GetClassName(Convert.ToInt32(find_data.ClassSchedule_Id));
            ViewBag.classroom = Reconcile_Com.Classroom_Entity.GetEntity(find_data.ClassRoom_id).ClassroomName;
            return View(find_data);
        }

        [HttpPost]
        public ActionResult EditFunction(TeacherNight t)
        {
            TeacherNight find = TeacherNight_Entity.GetEntity(t.Id);
            t.ClassSchedule_Id = find.ClassSchedule_Id;
            t.BeOnDuty_Id = find.BeOnDuty_Id;
            t.AttendDate = find.AttendDate;
            t.ClassRoom_id = find.ClassRoom_id;
            AjaxResult a = new AjaxResult();
            bool istrue= TeacherNightandEvningStudet.IsUpdateTeacherNightData(t.OrwatchDate,Convert.ToInt32(t.ClassSchedule_Id));
            if (istrue==true)
            {
                 a = TeacherNight_Entity.Edit_Data(t);
                if (a.Success)
                {
                    //学生晚自习数据
                   a=  TeacherNightandEvningStudet.SetEvningStudentData(t.OrwatchDate,Convert.ToInt32( t.ClassSchedule_Id), t.Tearcher_Id);                     
                }
            }
            else
            {
                a.Success = false;
                a.Msg = "该日期没有安排该班级的晚自习！，请重新选择日期或去安排班级在这个日期的晚自习";
            }

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetEmtpyClassroom()
        {
            int s_id = Convert.ToInt32(Request.Form["schooladdress"]);
            List<TreeClass> tree =Reconcile_Com.Classroom_Entity.GetAddreeClassRoom(s_id).Select(c => new TreeClass() { id = c.Id.ToString(), title = c.ClassroomName }).ToList();
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取班级晚自习安排数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetClassData()
        {
            int classid=Convert.ToInt32(Request.Form["classid"]);
            DateTime date = Convert.ToDateTime(Request.Form["date"]);
            //获取班级在这个日期中的晚自习安排数据
           List<EvningSelfStudy> findata= TeacherNightandEvningStudet.GetEvningData(date, classid);
            AjaxResult a = new AjaxResult();
            if (findata.Count>0)
            {
               var data= findata.Select(l => new { id = l.id, curname = l.curd_name }).ToList();
                a.Success = true;
                a.Data = data;
            }
            else
            {
                a.Success = false;
                a.Msg = "该班级在此日期中没有安排晚自习";
            }

            return Json(a,JsonRequestBehavior.AllowGet);
        }

        #region 调课或上课日期更换
        public ActionResult ClassadjustmentView(bool id)
        {
            ViewBag.Is = id;//如果是0--》只需要调换日期1-->日期往前推迟或往后推迟
            return View();
        }

        public ActionResult ClassadjustmentFunction()
        {
            int id = beOnDuty_Entity.GetSingleBeOnButy("教员晚自习", false).Id;
            DateTime old = Convert.ToDateTime(Request.Form["oldtime"]);
            DateTime new_old = Convert.ToDateTime(Request.Form["newtime"]);
            bool Whychangedate = Convert.ToBoolean(Request.Form["mybool"]);
            List<TeacherNight> list = TeacherNight_Entity.GetAllTeacherNight();
            int count = (new_old-old).Days;
            if (Whychangedate)//调课
            {
                list = list.Where(t => t.OrwatchDate >= old && t.BeOnDuty_Id == id).ToList();
            }
            else //日期更改
            {
                list = list.Where(t => t.OrwatchDate == old && t.BeOnDuty_Id == id).ToList();
            }
            AjaxResult a = TeacherNight_Entity.Update_Date(Whychangedate, list, count, new_old);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        #endregion

         
        #endregion

        #region 班主任晚自习值班
        public ActionResult ClassMasterIndex()
        {
            //获取所有班主任跟S4就业部的老师
            List<SelectListItem>  list=TeacherNight_Entity.GEThEADmASTER().Select(t => new SelectListItem { Text = t.EmpName, Value = t.EmployeeId }).ToList();
            list.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            list = list.OrderBy(l => l.Value).ToList();

            ViewBag.master = list;
            return View();
        }

        public ActionResult GetClassMasterFunction(int page, int limit)
        {
            EmployeesInfoManage e = new EmployeesInfoManage();
            int id1 = beOnDuty_Entity.GetSingleBeOnButy("周末值班", false).Id;
            int id2 = beOnDuty_Entity.GetSingleBeOnButy("班主任晚自习", false).Id;
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            List<TeacherNightView> getall = new List<TeacherNightView>();
            if (TeacherNight_Entity.IsShowData(UserName.EmpNumber)==0)
            {
                getall = TeacherNight_Entity.GetHeadMasterAll(id1,id2).OrderBy(l=>l.OrwatchDate).ToList();
            }
            else if (TeacherNight_Entity.IsShowData(UserName.EmpNumber)==2 || TeacherNight_Entity.IsShowData(UserName.EmpNumber) == 3)
            {
                List<EmployeesInfo> li = TeacherNight_Entity.AccordingtoEmplyess(UserName.EmpNumber);
                getall= TeacherNight_Entity.AccordingtoDepartMentData(li, id1, id2).OrderBy(l=>l.OrwatchDate).ToList();
            }
             //TeacherNight_Entity.GetAllTeacherNight().Where(t => t.BeOnDuty_Id == id1 || t.BeOnDuty_Id == id2).OrderByDescending(t => t.Id).ToList();
            string mid = Request.QueryString["tid"];
            string old = Request.QueryString["olddate"];
            string news = Request.QueryString["newdate"];
            if (!string.IsNullOrEmpty(mid) && mid != "0")
            {
                getall = getall.Where(g => g.Tearcher_Id == mid).ToList();
            }

            if (!string.IsNullOrEmpty(old))
            {
                DateTime date = Convert.ToDateTime(old);
                getall = getall.Where(g => g.OrwatchDate >= date).ToList();
            }

            if (!string.IsNullOrEmpty(news))
            {
                DateTime date = Convert.ToDateTime(news);
                getall = getall.Where(g => g.OrwatchDate <= date).ToList();
            }

            List<HeadmasterView> list_View = new List<HeadmasterView>();

            for (int i = 0; i < getall.Count;)
            {
                List<TeacherNightView> sametime= getall.Where(g => g.OrwatchDate == getall[i].OrwatchDate && g.BeOnDuty_Id==getall[i].BeOnDuty_Id && e.GetDeptByEmpid(g.Tearcher_Id).DeptId== e.GetDeptByEmpid(getall[i].Tearcher_Id).DeptId).ToList();
                HeadmasterView headmaster = new HeadmasterView();
                headmaster.Time = getall[i].OrwatchDate;
                headmaster.Types = getall[i].timename;
                string ids = null;
                string teachers = null;
                int index = 1;
                if (sametime.Count<=0)
                {
                    headmaster.Teachers = e.GetEntity(getall[i].Tearcher_Id).EmpName;

                    headmaster.Id = getall[i].Id.ToString();

                    list_View.Add(headmaster);
                }
                else
                {
                    sametime.ForEach(s => {
                        if (index == sametime.Count)
                        {
                            ids = ids + s.Id;
                            teachers = teachers + e.GetEntity(s.Tearcher_Id).EmpName;
                        }
                        else
                        {
                            ids = ids + s.Id + ",";
                            teachers = teachers + e.GetEntity(s.Tearcher_Id).EmpName + ",";
                        }
                        index++;

                        getall.Remove(s);

                        headmaster.Teachers = teachers;

                        headmaster.Id = ids;


                    });

                    list_View.Add(headmaster);
                    i=0;
                }
                
            }

            var data = list_View.OrderBy(l=>l.Time).Skip((page - 1) * limit).Take(limit).ToList();
 
            var jsondata = new { count = list_View.Count, code = 0, msg = "", data = list_View };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MasterAddView()
        {
            return View();
        }

        /// <summary>
        /// 添加值班数据
        /// </summary>
        /// <returns></returns>
        public ActionResult WeekenddutyView()
        {
            DepartmentManage Deparment_Entity = new DepartmentManage();
            List<SelectListItem> list = Deparment_Entity.GetDepartments().Where(d => d.DeptName.Contains("教质部") || d.DeptName.Contains("就业部")).Select(d => new SelectListItem() { Text = d.DeptName, Value = d.DeptId.ToString() }).ToList();//获取所有有效的部门
            list.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            ViewBag.dep = list.OrderBy(d => d.Value).ToList();
            return View();
        }
        [HttpPost]
        public ActionResult GetDepEmp()
        {
            int did = Convert.ToInt32(Request.Form["depid"]);//获取部门id
            EmployeesInfoManage Employeesinfo_Entity = new EmployeesInfoManage();
            List<SelectListItem> list = Employeesinfo_Entity.GetEmpsByDeptid(did).Where(e => e.IsDel == false).Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();//获取所属部门的所有未辞职的员工

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult WeekEnddutyFunction()
        {
            bool types = Convert.ToBoolean(Request.Form["Type"]);
            int typeid1 = beOnDuty_Entity.GetSingleBeOnButy("周末值班", false).Id;
            int typeid2 = beOnDuty_Entity.GetSingleBeOnButy("班主任晚自习", false).Id;
            string[] tid = Request.Form["tid"].Split(',');//值班老师员工编号
            string[] date = Request.Form["time"].Split(',');//值班日期
            List<TeacherNight> night = new List<TeacherNight>();
            foreach (string mydate in date)
            {
                if (!string.IsNullOrEmpty(mydate))
                {
                    DateTime time = Convert.ToDateTime(mydate);
                    foreach (string id in tid)
                    {
                        if (!string.IsNullOrEmpty(id))
                        {
                            TeacherNight new_t = new TeacherNight();
                            new_t.AttendDate = DateTime.Now;
                            new_t.BeOnDuty_Id = types == true ? typeid2 : typeid1;
                            new_t.IsDelete = false;
                            new_t.OrwatchDate = time;
                            new_t.Tearcher_Id = id;
                            new_t.timename = types == true ? "晚自习值班" : "周末值班";
                            night.Add(new_t);
                        }
                    }
                }
            }
            

            AjaxResult a = TeacherNight_Entity.Add_masterdata(night);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditMasterView(string id)
        {
            //根据Ids获取值班数据
            string[] ids = id.Split(',');
            DateTime? time = null;
            List<string> findlist = new List<string>();
            EmployeesInfoManage e = new EmployeesInfoManage();
            foreach (string item in ids)
            {                 
                if (!string.IsNullOrEmpty(item))
                {
                    int intid = Convert.ToInt32(item);
                    time = TeacherNight_Entity.GetEntity(intid).OrwatchDate;
                    findlist.Add(e.GetEntity(TeacherNight_Entity.GetEntity(intid).Tearcher_Id).EmpName);
                }
            }

            ViewBag.time = time;
            ViewBag.teachers = findlist;
            ViewBag.ids = id;
            return View();
        }
        [HttpPost]
        public ActionResult EditMasterFunction()
        {
            string[] ids = Request.Form["ids"].Split(',');
            string[] teachers = Request.Form["Teachers"].Split(',');
            DateTime time =Convert.ToDateTime(Request.Form["time"]);
            List<TeacherNight> oldlist = new List<TeacherNight>();
            EmployeesInfoManage e = new EmployeesInfoManage();
            foreach (string item in ids)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id= Convert.ToInt32(item);
                    TeacherNight find= TeacherNight_Entity.GetEntity(id);
                    var name= teachers.Where(t => t == e.GetEntity(find.Tearcher_Id).EmpName).FirstOrDefault();
                    if (name==null)
                    {
                        TeacherNight_Entity.Delete(find);
                    }
                    else
                    {
                        if (find.OrwatchDate != time)
                        {
                            find.OrwatchDate = time;
                            oldlist.Add(find);
                        }
                    }
                }
            }            
            AjaxResult a = TeacherNight_Entity.Edit_Data(oldlist);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 日期调换
        /// </summary>
        /// <returns></returns>
        public ActionResult EditDateView()
        {
            return View();
        }
        [HttpPost]
        public ActionResult EditDateFuntion()
        {
            DateTime time = Convert.ToDateTime(Request.Form["time"]);
            string idlist = Request.Form["ids"];
            bool isAll = Convert.ToBoolean(Request.Form["IsAll"]);
            AjaxResult a = new AjaxResult();
            List<TeacherNight> list = new List<TeacherNight>();
            if (isAll) //值班数据全部改为某日期
            {
                int id1 = beOnDuty_Entity.GetSingleBeOnButy("周末值班", false).Id;
                int id2 = beOnDuty_Entity.GetSingleBeOnButy("班主任晚自习", false).Id;
                List<TeacherNight> getall = TeacherNight_Entity.GetAllTeacherNight().Where(t => t.BeOnDuty_Id == id1 || t.BeOnDuty_Id == id2).OrderByDescending(t => t.Id).ToList();
                a = TeacherNight_Entity.Update_Date(false, getall, 0, time);
            }
            else
            {
                string[] list_id = idlist.Split(',');
                foreach (string id in list_id)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        int myid = Convert.ToInt32(id);
                        list.Add(TeacherNight_Entity.GetEntity(myid));
                    }
                }
               a= TeacherNight_Entity.Update_Date(false, list, 0, time);
            }
            return Json(a,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 延迟、提前值班日期页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EditDateChangeView()
        {
            return View();
        }

        public ActionResult EditDateChangeFuntion()
        {           
            DateTime newtime = Convert.ToDateTime(Request.Form["newtime"]);
            string idlist = Request.Form["ids"];
            List<TeacherNight> Tnightdata = TeacherNight_Entity.GetIQueryable().ToList();
            AjaxResult a = new AjaxResult();
            List<TeacherNight> list = new List<TeacherNight>();
            
            int count = 0;
                string[] list_id = idlist.Split(',');
                foreach (string id in list_id)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        int myid = Convert.ToInt32(id);
                        TeacherNight find = TeacherNight_Entity.GetEntity(myid);
                        list.Add(find);
                        list.AddRange(Tnightdata.Where(t => t.OrwatchDate >= find.OrwatchDate && t.Tearcher_Id == find.Tearcher_Id).ToList());
                    }
                }
           
            count = (newtime - list[0].OrwatchDate).Days;
            a = TeacherNight_Entity.Update_Date(true, list, count, newtime);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 班主任值班数据
        /// </summary>
        /// <returns></returns>
        public ActionResult EmpZhibanData()
        {
            return View();
        }
        
        public ActionResult TaableData(int limit,int page)
        {
            int id1 = beOnDuty_Entity.GetSingleBeOnButy("周末值班", false).Id;
            int id2 = beOnDuty_Entity.GetSingleBeOnButy("班主任晚自习", false).Id;
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            
            List<TeacherNightView> list = TeacherNight_Entity.AccordingtoEmpGetData(UserName.EmpNumber, id1, id2).OrderBy(l=>l.OrwatchDate).ToList();
            string da1 = Request.QueryString["d1"];

            if (!string.IsNullOrEmpty(da1))
            {
                DateTime d1 = Convert.ToDateTime(da1);
                list = list.Where(l => l.OrwatchDate >= d1).ToList();
            }
            string da2 = Request.QueryString["d2"];

            if (!string.IsNullOrEmpty(da2))
            {
                DateTime d2 = Convert.ToDateTime(da2);
                list = list.Where(l => l.OrwatchDate <= d2).ToList();
            }
            var data = list.Skip((page - 1) * limit).Take(limit).ToList();

            var jsondata = new { count = list.Count, code = 0, msg = "", data = data };
            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }
       
        #endregion
    }
}