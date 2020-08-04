using SiliconValley.InformationSystem.Business.DepartmentBusiness;
using SiliconValley.InformationSystem.Business.EducationalBusiness;
using SiliconValley.InformationSystem.Business.PositionBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{

    using System.Text;

    [CheckLogin]
    public class EvningSelfStudyController : Controller
    {
        // GET: /Educational/EvningSelfStudy/ChangeGrandADIfuntion

        EvningSelfStudyManeger EvningSelefstudy_Entity = new EvningSelfStudyManeger();//DBCC CHECKIDENT (Reconcile,reseed,5304) 

        /// <summary>
        /// 数据主页显示
        /// </summary>
        /// <returns></returns>
        public ActionResult EvningSelfStudyIndexView()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Com.GetGrand_Id().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grlist = g_list;
            return View();
        }

        public ActionResult EvningTableData(int page, int limit)
        {
            List<EvningSelfStudyView> Evn_list = EvningSelefstudy_Entity.GetAllView().OrderByDescending(e => e.Anpaidate).ToList();//获取所有晚自习排课数据
            List<Grand> glist = Reconcile_Com.GetGrand_Id();

            string c_id = Request.QueryString["class_selectone"];
            string startime = Request.QueryString["onetime"];
            string endtime = Request.QueryString["twotime"];
            string grandid = Request.QueryString["Grand"];
            if (!string.IsNullOrEmpty(c_id) && c_id != "0")
            {
                int class_id = int.Parse(c_id);
                Evn_list = Evn_list.Where(e => e.ClassSchedule_id == class_id).ToList();
            }
            if (!string.IsNullOrEmpty(startime))
            {
                DateTime d1 = Convert.ToDateTime(startime);
                Evn_list = Evn_list.Where(e => e.Anpaidate >= d1).ToList();
            }
            if (!string.IsNullOrEmpty(endtime))
            {
                DateTime d2 = Convert.ToDateTime(endtime);
                Evn_list = Evn_list.Where(e => e.Anpaidate <= d2).ToList();
            }
            if (!string.IsNullOrEmpty(grandid) && grandid != "0")
            {
                int grand_id = Convert.ToInt32(grandid);
                Evn_list = Evn_list.Where(e => Reconcile_Com.ClassSchedule_Entity.GetEntity(e.ClassSchedule_id).grade_Id == grand_id).ToList();
            }
            var data = Evn_list.Skip((page - 1) * limit).Take(limit).ToList();

            var jsonData = new { code = 0, msg = "", data = data, count = Evn_list.Count };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        #region 安排晚自习
        public ActionResult AnPaiEvningSelfStudyView()
        {
            TeacherBusiness Teacher_Entity = new TeacherBusiness();
            BaseDataEnumManeger dataEnum_Entity = new BaseDataEnumManeger();
            //获取阶段
            List<SelectListItem> g_list =Reconcile_Com.GetGrand_Id().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grandlist = g_list;

            List<SelectListItem> teachers = Teacher_Entity.GetTeachers().Select(t => new SelectListItem() { Value = t.EmployeeId, Text = Reconcile_Com.GetEmpName(t.EmployeeId) }).ToList(); //获取所有未辞职的教员
            teachers.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            ViewBag.teacher = teachers.OrderBy(t => t.Value).ToList();
            List<SelectListItem> s_list = dataEnum_Entity.GetsameFartherData("校区地址").Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() }).ToList(); //获取有效校区
            s_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            ViewBag.schoolAddress = s_list.OrderBy(c => c.Value).ToList();
            return View();
        }

        /// <summary>
        /// 系统自动安排晚自习
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AnPaiEvningSelfStudyFunction()
        {
            AjaxResult a = new AjaxResult();
            string timestring = Request.Form["time"];
            DateTime startime = Convert.ToDateTime(timestring.Split('到')[0]);
            DateTime endtime = Convert.ToDateTime(timestring.Split('到')[1]);
            GetYear find_g = EvningSelefstudy_Entity.MyGetYear(startime.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));
            bool doublerest = true;//默认双休
            if (startime.Month >= find_g.StartmonthName && startime.Month <= find_g.EndmonthName)
            {
                doublerest = false;//单休
            }
            a = EvningSelefstudy_Entity.GetNewEvningData(startime, endtime, true, doublerest);
            if (a.Success == false)//有未安排专业课的班级
            {
                return Json(a, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<EvningSelfStudy> list = a.Data as List<EvningSelfStudy>;
                a = EvningSelefstudy_Entity.Add_Data(list, true);
                return Json(a, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 手动安排晚自习
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HnadEvningSelfStudyFunction()
        {
            AjaxResult a = new AjaxResult();
            EvningSelfStudy new_data = new EvningSelfStudy();
            new_data.Anpaidate = Convert.ToDateTime(Request.Form["mytime"]); //自习日期
            new_data.Classroom_id = Convert.ToInt32(Request.Form["classroom"]);//所在教室
            new_data.ClassSchedule_id = Convert.ToInt32(Request.Form["classShdule_sele"]);//班级
            new_data.curd_name = Request.Form["timename"]; //自习时间
            new_data.Rmark = Request.Form["ramke"];//其他说明
            new_data.emp_id = Request.Form["teacher_sele"] == "0" ? null : Request.Form["teacher_sele"];//任课老师
            new_data.Newdate = DateTime.Now;
            new_data.IsDelete = false;

            a = EvningSelefstudy_Entity.Add_Data(new_data);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 阶段安排晚自习
        /// </summary>
        /// <returns></returns>
        public ActionResult GoodEvningSelfStudyView()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Com.GetGrand_Id().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            ViewBag.grlist = g_list;
            return View();
        }

        /// <summary>
        /// 安排一天的晚自习
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GoodEvningSelfStudyFunction()
        {
            string[] class_ids = Request.Form["checkid_Str"].Split(',');
            //根据阶段获取班级
            List<ClassSchedule> list_c = new List<ClassSchedule>();
            foreach (string it in class_ids)
            {
                if (!string.IsNullOrEmpty(it)) { int grandid = Convert.ToInt32(it); list_c.AddRange(Reconcile_Com.GetClass().Where(c => c.grade_Id == grandid).ToList()); }
            }

            DateTime starttime = Convert.ToDateTime(Request.Form["starTime"].Split('到')[0]);
            DateTime endtime = Convert.ToDateTime(Request.Form["starTime"].Split('到')[1]);
            GrandClassAnpaiEvningSelf data = EvningSelefstudy_Entity.GoodsEvningSelfStudyFunction(list_c, starttime, endtime);
            AjaxResult a = EvningSelefstudy_Entity.Add_Data(data.evnlist, false);
            a.Data = data.emplist;
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 安排一天的晚自习
        /// </summary>
        /// <returns></returns>
        public ActionResult GoodEvningSelftSudyFinciton2()
        {
            string[] class_ids = Request.Form["checkid_Str"].Split(',');
            //根据阶段获取班级
            List<ClassSchedule> list_c = new List<ClassSchedule>();
            foreach (string it in class_ids)
            {
                if (!string.IsNullOrEmpty(it)) { int grandid = Convert.ToInt32(it); list_c.AddRange(Reconcile_Com.GetClass().Where(c => c.grade_Id == grandid).ToList()); }
            }
            DateTime starttime = Convert.ToDateTime(Request.Form["starTime"]);

            GrandClassAnpaiEvningSelf data = EvningSelefstudy_Entity.GoodsEvningSelfStudyFunction(list_c, starttime);
            AjaxResult a = EvningSelefstudy_Entity.Add_Data(data.evnlist, true);
            a.Data = data.emplist;
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 对于数据的操作

        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateEvning(int id)
        {

            List<SelectListItem> class_select = Reconcile_Com.GetClass().Select(e => new SelectListItem() { Text = e.ClassNumber, Value = e.id.ToString() }).ToList();   //获取班级
            ViewBag.Classlist = class_select;

            List<SelectListItem> classrooms = Reconcile_Com.Classroom_Entity.GetEffectiveClass().Select(c => new SelectListItem() { Text = c.ClassroomName, Value = c.Id.ToString() }).ToList();     //获取教室
            ViewBag.classroom = classrooms;

            List<SelectListItem> timename_list = new List<SelectListItem>();   //获取上课时间段
            timename_list.Add(new SelectListItem() { Text = "晚一", Value = "晚一" });
            timename_list.Add(new SelectListItem() { Text = "晚二", Value = "晚二" });
            ViewBag.timename = timename_list;
            EvningSelfStudyView find_e = EvningSelefstudy_Entity.FindIdView(id);
            TeacherBusiness Teacher_Entity = new TeacherBusiness();
            List<SelectListItem> teachers = Teacher_Entity.GetTeachers().Select(t => new SelectListItem() { Value = t.EmployeeId, Text = Reconcile_Com.GetEmpName(t.EmployeeId),Selected= Reconcile_Com.GetEmpName(t.EmployeeId)== find_e.EmpName?true:false }).ToList(); //获取所有未辞职的教员
            if (find_e.EmpName==null)
            {
                teachers.Add(new SelectListItem() { Text = "--无--", Value = "0" ,Selected=true});
            }
            else
            {
                teachers.Add(new SelectListItem() { Text = "--无--", Value = "0", Selected = false });
            }
             
            ViewBag.tt = teachers;
            //获取排课日期
            ViewBag.date = find_e.Anpaidate.Year + "-" + find_e.Anpaidate.Month + "-" + find_e.Anpaidate.Day;
            ClassSchedule findclass=  Reconcile_Com.ClassSchedule_Entity.GetEntity(find_e.ClassSchedule_id);
            //获取阶段
            ViewBag.grand=  Reconcile_Com.GetGrand_Id().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString(),Selected=g.Id== findclass.grade_Id?true:false }).ToList();
            //获取校区
            Classroom find_classroom = Reconcile_Com.Classroom_Entity.GetEntity(find_e.Classroom_id);
            ViewBag.address=  EvningSelefstudy_Entity.BaseDataEnum_Entity.GetsameFartherData("校区地址").Select(ress=>new SelectListItem() { Text=ress.Name,Value=ress.Id.ToString(),Selected=ress.Id==find_classroom.BaseData_Id?true:false});
            return View(find_e);
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateFunction(EvningSelfStudyView e)
        {
            EvningSelfStudy evning= EvningSelefstudy_Entity.FindId(e.id);
            evning.ClassSchedule_id = e.ClassSchedule_id;
            evning.Classroom_id = e.Classroom_id;
            evning.curd_name = e.curd_name;
            evning.Anpaidate = e.Anpaidate;
            evning.emp_id = e.EmpName;
            evning.Rmark = e.Rmark;
            AjaxResult a = EvningSelefstudy_Entity.Update_Data(evning);

            if (e.EmpName != "0")
            {
                //判断是否修改了老师
                EvningSelefstudy_Entity.FindIdView(e.id);
            }
             
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            AjaxResult a = EvningSelefstudy_Entity.Delete_Data(id);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region  调课
        //全体调课
        public ActionResult BigDataADIview()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BigDataADIfunction()
        {
            DateTime startime = Convert.ToDateTime(Request.Form["oldTime"]);
            DateTime endtime = Convert.ToDateTime(Request.Form["endtime"]);
            var days = endtime.Subtract(startime);
            int count = days.Days;
            List<EvningSelfStudy> e_list = EvningSelefstudy_Entity.GetEmpClass(startime, false);//获取这个日期之后的所有数据
            GetYear years = EvningSelefstudy_Entity.MyGetYear(startime.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));
            AjaxResult a = EvningSelefstudy_Entity.ALLDataADI(count, e_list, years);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        //班级调课
        public ActionResult ClassDataADIview()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Com.GetGrand_Id().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grlist = g_list;
            return View();
        }

        [HttpPost]
        public ActionResult ClassDataADIfunction()
        {
            DateTime startime = Convert.ToDateTime(Request.Form["starTime"]);
            int count = Convert.ToInt32(Request.Form["endTime"]);
            string[] id_String = Request.Form["checkid_Str"].Split(',');//获取班级id数组
            AjaxResult a = new AjaxResult();
            List<EvningSelfStudy> e_list = new List<EvningSelfStudy>();
            foreach (string id in id_String)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    List<EvningSelfStudy> updatedata = EvningSelefstudy_Entity.AcctoingDate(startime, Convert.ToInt32(id));
                    e_list.AddRange(updatedata);
                }
            }

            GetYear years = EvningSelefstudy_Entity.MyGetYear(startime.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));
            if(count>0)
            {
                a = EvningSelefstudy_Entity.ALLDataADI(count, e_list, years);
            }
            else
            {
                a = EvningSelefstudy_Entity.AccoreDataADI(count, e_list, years);
            }
              
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 阶段延迟、提前页面
        /// </summary>
        /// <returns></returns>
        public ActionResult GrandADIView()
        {
            //获取所有阶段
            List<SelectListItem> g_list = Reconcile_Com.GetGrand_Id().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            ViewBag.Mygrandlist = g_list;
            return View();
        }

        [HttpPost]
        public ActionResult GrandADIfuntion()
        {
            string[] grands= Request.Form["grands"].Split(',');

            DateTime startTime =Convert.ToDateTime( Request.Form["starTime"]);//获取开始的日期

            int count =Convert.ToInt32(Request.Form["endTime"]);//天数

            List<EvningSelfStudy> list = new List<EvningSelfStudy>();
            //获取符合的数据
            foreach (string item in grands)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    StringBuilder sb = new StringBuilder("select * from EvningSelfStudyView where Anpaidate>='"+startTime + "' and grandid="+item);

                    list.AddRange( EvningSelefstudy_Entity.GetSQLDat(sb.ToString()).Select(e=>new EvningSelfStudy() {
                        id=e.id,
                        ClassSchedule_id=e.ClassSchedule_id,
                        Classroom_id=e.Classroom_id,
                        curd_name=e.curd_name,
                        Anpaidate=e.Anpaidate,
                        Newdate=e.Newdate,
                        Rmark=e.Rmark,
                        IsDelete=e.IsDelete,
                        emp_id=e.emp_id
                    }));
                }
            }

            GetYear years = EvningSelefstudy_Entity.MyGetYear(startTime.Year.ToString(), Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));

            AjaxResult a = new AjaxResult();
            if (count > 0)
            {
                a = EvningSelefstudy_Entity.ALLDataADI(count, list, years);
            }
            else
            {
                a = EvningSelefstudy_Entity.AccoreDataADI(count, list, years);
            }
            return Json(a,JsonRequestBehavior.AllowGet);
        }
       
        /// <summary>
        /// 阶段日期调换
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeGrandADIView()
        {
            return View();
        }

        public ActionResult ChangeGrandADIfuntion()
        {
            DateTime stardate =Convert.ToDateTime( Request.Form["starTime"]);

            DateTime enddate = Convert.ToDateTime(Request.Form["endTime"]);

            string[] grands = Request.Form["grand"].Split(',');

            List<EvningSelfStudy> list = new List<EvningSelfStudy>();
            //获取符合的数据
            foreach (string item in grands)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    StringBuilder sb = new StringBuilder("select * from EvningSelfStudyView where Anpaidate>='" + stardate + "' and grandid=" + item);

                    list.AddRange(EvningSelefstudy_Entity.GetSQLDat(sb.ToString()).Select(e => new EvningSelfStudy()
                    {
                        id = e.id,
                        ClassSchedule_id = e.ClassSchedule_id,
                        Classroom_id = e.Classroom_id,
                        curd_name = e.curd_name,
                        Anpaidate = e.Anpaidate,
                        Newdate = e.Newdate,
                        Rmark = e.Rmark,
                        IsDelete = e.IsDelete,
                        emp_id = e.emp_id
                    }));
                }
            }

            AjaxResult a= EvningSelefstudy_Entity.ChangDate(list, enddate);

            return Json(a,JsonRequestBehavior.AllowGet);
        }
        
        #endregion

        #region 日期调换
        //全体上课日期调换
        public ActionResult BigDataChangView()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BigDataChangFunction()
        {
            DateTime startime = Convert.ToDateTime(Request.Form["oldTime"]);//获取原来上课的日期
            DateTime endtime = Convert.ToDateTime(Request.Form["endtime"]);//获取更改的日期


            List<EvningSelfStudy> find_e = EvningSelefstudy_Entity.GetEmpClass(startime, true);//获取原来日期的晚自习安排数据

            AjaxResult a = EvningSelefstudy_Entity.ChangDate(find_e, endtime);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        //班级上课日期调换
        public ActionResult BigDataClassChangView()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Com.GetGrand_Id().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.grlist = g_list;
            return View();
        }

        [HttpPost]
        public ActionResult ClassDataClassChangfunction()
        {
            DateTime startime = Convert.ToDateTime(Request.Form["starTime"]);
            DateTime endtime = Convert.ToDateTime(Request.Form["endTime"]);
            string[] id_String = Request.Form["checkid_Str"].Split(',');//获取班级id数组
            List<EvningSelfStudy> find_e = new List<EvningSelfStudy>();
            foreach (string id in id_String)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    List<EvningSelfStudy> update_data = EvningSelefstudy_Entity.GetEmpClass(startime, true).Where(e => e.ClassSchedule_id == Convert.ToInt32(id)).ToList();
                    find_e.AddRange(update_data);
                }
            }

            AjaxResult a = EvningSelefstudy_Entity.ChangDate(find_e, endtime);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 其他
        /// <summary>
        /// 获取空教室
        /// </summary>
        /// <returns></returns>
        public ActionResult GetEmptyClassroom()
        {
            string timename = Request.Form["timename"];//时间
            DateTime time = Convert.ToDateTime(Request.Form["time"]);//日期
            int shcooldrees_id = Convert.ToInt32(Request.Form["schooladdres"]);//校区编号
            List<Classroom> list = EvningSelefstudy_Entity.GetEmptyClassrooms(timename, time, shcooldrees_id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据登录的老师获取属于他们的班级晚自习数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowTeacher()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteFunction()
        {

            string[] str = Request.Form["str"].Split(',');
            List<EvningSelfStudy> list = new List<EvningSelfStudy>();
            foreach (string item in str)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id = Convert.ToInt32(item);
                    list.Add(EvningSelefstudy_Entity.FindId(id));
                }
            }

            AjaxResult a = EvningSelefstudy_Entity.Delete_Data(list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据选择的阶段删除指定日期的晚自习数据
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteDateView()
        {
            //获取阶段
            List<SelectListItem> g_list = Reconcile_Com.GetGrand_Id().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();
            ViewBag.grlist = g_list;
            return View();
        }

        [HttpPost]
        public ActionResult DeletetDateFunction()
        {
            string[] ids = Request.Form["checkid_Str"].Split(',');//所选阶段
            DateTime time = Convert.ToDateTime(Request.Form["starTime"]);//日期
            List<ClassSchedule> cla_list = new List<ClassSchedule>();
            foreach (string item in ids)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id = Convert.ToInt32(item);
                    cla_list.AddRange(Reconcile_Com.GetClass().Where(c => c.grade_Id == id).ToList());
                }
            }
            List<EvningSelfStudy> evn_list = new List<EvningSelfStudy>();
            foreach (ClassSchedule item in cla_list)
            {
                EvningSelfStudy findata = EvningSelefstudy_Entity.GetEmpClass(time, true).Where(t => t.ClassSchedule_id == item.id).FirstOrDefault();
                  if(findata != null)
                  {
                    evn_list.Add(findata);
                  }
            }

            AjaxResult a = EvningSelefstudy_Entity.Delete_Data(evn_list);
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}