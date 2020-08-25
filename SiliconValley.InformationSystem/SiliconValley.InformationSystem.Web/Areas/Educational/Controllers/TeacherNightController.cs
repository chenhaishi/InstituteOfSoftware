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
using System.Text;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    [CheckLogin]
    public class TeacherNightController : Controller
    {
        // GET: /Educational/TeacherNight/TeacherSerch

        TeacherNightManeger TeacherNight_Entity = new TeacherNightManeger();
        TeacherBusiness Teacher_Entity;

        BeOnDutyManeger beOnDuty_Entity = new BeOnDutyManeger(); //获取值班类型        
       
        /// <summary>
        /// 添加教员值班页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddDataView()
        {            
            List<SelectListItem> sle_grand = Reconcile_Com.GetGrand_Id().Select(cl => new SelectListItem() { Text = cl.GrandName, Value = cl.Id.ToString() ,Selected=false}).ToList();  //获取阶段
            sle_grand.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.myclass = sle_grand;
 
            return View();
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
            string teachername = Request.Form["teachername"];

            List<SelectListItem> teacherlist = Teacher_Entity.GetTeacherEmps().Where(t => t.EmpName.Contains(teachername)).Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            a.Success = true;
            a.Data = teacherlist;

            return Json(a, JsonRequestBehavior.AllowGet);
        }

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
                    TeacherNight finda = TeacherNight_Entity.GetEntity(tid);
                    if (finda!=null)
                    {
                       if(finda.IsDelete == false)
                        {
                            DELE.Add(finda);
                        }
                       
                    }
                    
                }
            }
             
            a = TeacherNight_Entity.My_Delete(DELE);
             
            return Json(a, JsonRequestBehavior.AllowGet);
        }
       
             
        #region 班主任晚自习值班
        public ActionResult ClassMasterIndex()
        {
            //获取所有班主任跟S4就业部的老师
            List<SelectListItem>  list=TeacherNight_Entity.GEThEADmASTER().Select(t => new SelectListItem { Text = t.EmpName, Value = t.EmployeeId }).ToList();
            list.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            list = list.OrderBy(l => l.Value).ToList();

            ViewBag.master = list;

            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息

            //判断是否是教务
            ViewBag.isjiaowu = TeacherNight_Entity.IsJiaowu(UserName.EmpNumber)==true?1:0;

            return View();
        }

        /// <summary>
        /// 给班主任显示的数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetClassMasterFunction(int page, int limit)
        {
            EmployeesInfoManage e = new EmployeesInfoManage();
            int id1 = 2;//beOnDuty_Entity.GetSingleBeOnButy("周末值班", false).Id;
            int id2 = 6;//beOnDuty_Entity.GetSingleBeOnButy("班主任晚自习", false).Id;
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
           
                List<TeacherNightView> getall = new List<TeacherNightView>();
                if (TeacherNight_Entity.IsShowData(UserName.EmpNumber) == 0)
                {
                    getall = TeacherNight_Entity.GetHeadMasterAll(id1, id2).OrderBy(l => l.OrwatchDate).ToList();
                }
                else if (TeacherNight_Entity.IsShowData(UserName.EmpNumber) == 2 || TeacherNight_Entity.IsShowData(UserName.EmpNumber) == 3)
                {
                    List<EmployeesInfo> li = TeacherNight_Entity.AccordingtoEmplyess(UserName.EmpNumber);
                    getall = TeacherNight_Entity.AccordingtoDepartMentData(li, id1, id2).OrderBy(l => l.OrwatchDate).ToList();
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
                    List<TeacherNightView> sametime = getall.Where(g => g.OrwatchDate == getall[i].OrwatchDate && g.BeOnDuty_Id == getall[i].BeOnDuty_Id && e.GetDeptByEmpid(g.Tearcher_Id).DeptId == e.GetDeptByEmpid(getall[i].Tearcher_Id).DeptId).ToList();
                    HeadmasterView headmaster = new HeadmasterView();
                    headmaster.Time = getall[i].OrwatchDate;
                    //headmaster.Types = getall[i].timename;
                    string ids = null;
                    string teachers = null;
                    int index = 1;
                    if (sametime.Count <= 0)
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
                        i = 0;
                    }

                }

                SessionHelper.Session["data"] = list_View;
                var data = list_View.OrderBy(l => l.Time).Skip((page - 1) * limit).Take(limit).ToList();

                var jsondata = new { count = list_View.Count, code = 0, msg = "", data = data };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
          
            
        }

        /// <summary>
        /// 该数据是显示给教务审核的数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetJiaoMasterFuntion(int page,int limit)
        {
            int id1 = 2;//beOnDuty_Entity.GetSingleBeOnButy("周末值班", false).Id;
            int id2 = 6;//beOnDuty_Entity.GetSingleBeOnButy("班主任晚自习", false).Id;

            List<TeacherNightView> list= TeacherNight_Entity.GetHeadMasterAll(id1, id2).ToList();

            var data = list.OrderBy(l => l.OrwatchDate).Skip((page - 1) * limit).Take(limit).ToList();

            var jsondata = new { count = list.Count, code = 0, msg = "", data = data };
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
                            //new_t.timename = types == true ? "晚自习值班" : "周末值班";
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
                    TeacherNight finda = TeacherNight_Entity.GetEntity(intid);
                    if (finda!=null)
                    {
                       time= finda.OrwatchDate;
                        findlist.Add(e.GetEntity(TeacherNight_Entity.GetEntity(intid).Tearcher_Id).EmpName);
                    }
                  
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
        
        /// <summary>
        /// 给教务显示的数据(第一次数据加载)
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult TaableData(int limit,int page)
        {
            int id1 = 2;//beOnDuty_Entity.GetSingleBeOnButy("周末值班", false).Id;
            int id2 = 6; //beOnDuty_Entity.GetSingleBeOnButy("班主任晚自习", false).Id;
            //Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            
            List<TeacherNightView> list = TeacherNight_Entity.GetHeadMasterAll(id1, id2).OrderBy(l=>l.OrwatchDate).ToList();
           
            var data = list.Skip((page - 1) * limit).Take(limit).ToList();

            var jsondata = new { count = list.Count, code = 0, msg = "", data = data };
            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }
       
        [HttpPost]
        public ActionResult ShenHe()
        {
            string[] strs = Request.Form["strs"].Split(',');
            List<TeacherNight> list = new List<TeacherNight>();
            foreach (string item in strs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                   int id=  Convert.ToInt32(item);

                   if(TeacherNight_Entity.FindId(id)!=null)
                    {
                        TeacherNight FIDNA = TeacherNight_Entity.FindId(id);
                        FIDNA.IsDelete = true;
                        list.Add(FIDNA);
                    }
                }
            }

           AjaxResult a= TeacherNight_Entity.Edit_Data(list);

            return Json(a,JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 用于教务查询值班数据的方法
        /// </summary>
        /// <returns></returns>
        public ActionResult SercherTable(int limit,int page )
        {
            string did = Request.QueryString["Depar_find"];//部门编号
            string startime = Request.QueryString["starTime"];//开始日期
            string endtime = Request.QueryString["endTime"];//结束日期
            string type = Request.QueryString["Type_find"];//值班类型
 
            string teacherid=  Request.QueryString["teacher_Div"];//值班老师
            //int type1 = 6;//类型班主任晚自习
            //int type2 = 2;//类型为周末值班

            

            StringBuilder sb = new StringBuilder("select * from TeacherNightView where 1=1");

            if (!string.IsNullOrEmpty(startime))
            {
                sb.Append(" and OrwatchDate>='"+startime+"'");
            }

            if (!string.IsNullOrEmpty(endtime))
            {
                sb.Append(" and OrwatchDate<='" + endtime + "'");
            }

            if (type!="0")
            {
                sb.Append(" and BeOnDuty_Id="+type);
            }

            List<TeacherNightView> teacherlist = TeacherNight_Entity.GetListBySql<TeacherNightView>(sb.ToString());

            if (teacherid!="0")
            {
                sb.Append(" and Tearcher_Id='"+ teacherid + "'");
                teacherlist = TeacherNight_Entity.GetListBySql<TeacherNightView>(sb.ToString());

                var data = teacherlist.OrderBy(d => d.OrwatchDate).Skip((page - 1) * limit).Take(limit).ToList();

                var jsondata = new { code = 0, mag = "", data = data, count = teacherlist.Count };

                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            else if (did != "0" && teacherid == "0")
            {
                EmployeesInfoManage employeesInfo = new EmployeesInfoManage();
                List<EmployeesInfo> emplist = employeesInfo.GetEmpsByDeptid(Convert.ToInt32(did));//获取属于这个部门点员工

                List<TeacherNightView> list = new List<TeacherNightView>();
                foreach (EmployeesInfo item in emplist)
                {
                    var dd = teacherlist.Where(l => l.Tearcher_Id == item.EmployeeId).ToList();
                    list.AddRange(dd);
                }

                var data = list.OrderBy(d => d.OrwatchDate).Skip((page - 1) * limit).Take(limit).ToList();

                var jsondata = new { code = 0, mag = "", data = data, count = list.Count };

                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = teacherlist.OrderBy(d => d.OrwatchDate).Skip((page - 1) * limit).Take(limit).ToList();

                var jsondata = new { code = 0, mag = "", data = data, count = teacherlist.Count };

                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }

           
        }
       
        #endregion
    }
}