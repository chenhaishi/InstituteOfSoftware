using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Business.EducationalBusiness;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
    using SiliconValley.InformationSystem.Util;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.Print;
    using System.Text;

    [CheckLogin]
    public class TeacherAddorBeonDutyController : Controller
    {
        // GET: /Educational/TeacherAddorBeonDuty/OutData
        TeacherAddorBeonDutyManager Tb_Entity = new TeacherAddorBeonDutyManager();

        public ActionResult TeacherAddorBeonDutyIndex()
        {
            //判断登录的是否是教务
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            ViewBag.emp= Tb_Entity.Isjiaowu(UserName.EmpNumber);
            ////获取所有老师

            List<SelectListItem> teacherlist = Tb_Entity.Teacher_Entity.GetTeacherEmps().Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            teacherlist.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            teacherlist = teacherlist.OrderBy(t => t.Value).ToList();
            ViewBag.teacher = teacherlist;
            return View();
        }

        public ActionResult Tabledata(int limit, int page)
        {
            List<TeacherAddorBeonDutyView> list = Tb_Entity.GetViewAll().OrderByDescending(l => l.Anpaidate).ToList();
            var jsondata = new { code = 0, Msg = "", count = list.Count, data = list.Skip((page - 1) * limit).Take(limit).ToList() };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TabledataSecher(int limit, int page)
        {
            string tid = Request.QueryString["tid"];
            string startime = Request.QueryString["olddate"];
            string endtime = Request.QueryString["newdate"];

            StringBuilder sb = new StringBuilder("select * from TeacherAddorBeonDutyView where 1=1 ");

            if (!string.IsNullOrEmpty(tid) && tid != "0")
            {
                sb.Append(" and Tearcher_Id='" + tid + "'");
            }


            if (!string.IsNullOrEmpty(startime))
            {
                sb.Append(" and  Anpaidate>='" + startime + "'");
            }

            if (!string.IsNullOrEmpty(endtime))
            {
                sb.Append(" and  Anpaidate<='" + endtime + "'");
            }




            List<TeacherAddorBeonDutyView> list = Tb_Entity.AttendSqlGetData(sb.ToString());
            var jsondata = new { code = 0, Msg = "", count = list.Count, data = list.Skip((page - 1) * limit).Take(limit).ToList() };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 手动安排教员值班、加课数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HandAnpaiFunction()
        {
            AjaxResult a = new AjaxResult();
            string[] evnings = Request.Form["evnings"].Split(',');
            DateTime time = Convert.ToDateTime(Request.Form["mytime"]);
            List<TeacherAddorBeonDuty> list = new List<TeacherAddorBeonDuty>();
            foreach (string item in evnings)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id = Convert.ToInt32(item);
                    TeacherAddorBeonDuty new_t = new TeacherAddorBeonDuty()
                    {
                        Tearcher_Id = Request.Form["Teacherid"],
                        BeOnDuty_Id = Convert.ToInt32(Request.Form["type"]),
                        OnByReak = Request.Form["ramke"],
                        AttendDate = DateTime.Now,
                        evning_Id = id,
                        IsDels=false
                    };

                    bool hava = Tb_Entity.Exits(new_t.evning_Id, time, new_t.Tearcher_Id);
                    if (!hava)
                    {
                        list.Add(new_t);
                    }
                }
            }

            if (list.Count > 0)
            {
                a = Tb_Entity.Add_data(list);
            }
            else
            {
                a.Success = false;
                a.Msg = "数据已重复，请删除数据在进行添加！";
            }

            return Json(a, JsonRequestBehavior.AllowGet); ;
        }

        [HttpPost]
        public ActionResult DeleteFunction()
        {
            string[] ids = Request.Form["ids"].Split(',');
            List<TeacherAddorBeonDuty> list = new List<TeacherAddorBeonDuty>();
            foreach (string item in ids)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id = Convert.ToInt32(item);
                    TeacherAddorBeonDuty find = Tb_Entity.Findid(id);
                    if (find != null && find.IsDels==false)
                    {
                        list.Add(find);
                    }
                }
            }

            AjaxResult a = Tb_Entity.Del_data(list);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        #region 编辑页面

        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult EditFunction(int id)
        //{
        //    TeacherAddorBeonDutyView find= Tb_Entity.FindViewid(id);

        //    return View(find);
        //}
        #endregion
      
        /// <summary>
        /// 教学老师模糊选择视图
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherEACHView()
        {
            return View();
        }


        public ActionResult Shenhe(int id)
        {
            TeacherAddorBeonDuty find = Tb_Entity.Findid(id);

            find.IsDels = true;

            AjaxResult a=  Tb_Entity.Upd_data(find);

            return Json(a, JsonRequestBehavior.AllowGet);
            
        }


        public ActionResult OutData()
        {
            string[] date =  Request.Form["date_Anpaidate"].Split('-');

            int dep =Convert.ToInt32( Request.Form["dep"]);

 
            //获取属于这个部门的员工

            List<EmployeesInfo> emplist= Tb_Entity.EmployeesInfo_Entity.GetEmpsByDeptid(dep);

            List<TeacherAddorBeonDutyView> beonduty_list = new List<TeacherAddorBeonDutyView>();

            foreach (EmployeesInfo item in emplist)
            {
               var find= Tb_Entity.AttendSqlGetData(" select * from TeacherAddorBeonDutyView where YEAR(Anpaidate)='" + date[0] + "' and MONTH(Anpaidate)='" + date[1] + "' and  Tearcher_Id='"+item.EmployeeId+"'");

                beonduty_list.AddRange(find);
            }
            Dictionary<string, Onbety_Print> list = new Dictionary<string, Onbety_Print>();
            var a = beonduty_list.GroupBy(b => b.Anpaidate);
            a.ForEach(b=>
            {
                try
                {
                    b.ForEach(d =>
                    {
                        string dd = d.Anpaidate.ToString();
                        var tempob = new Onbety_Print()
                        {
                            Content = $"{d.EmpName}({d.ClassNumber}/{d.curd_name})"
                        };

                        list.Add(dd, tempob);
                    });
                }
                catch (Exception ex)
                {

                    string mm = ex.Message;
                }
                 
            });
            string title = date[0] + "年" + date[1] + "月值班表";

            string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + ".xls";
            SessionHelper.Session["filename"] = filename;
            string path = "~/uploadXLSXfile/ConsultUploadfile/ExportAll/" + filename;
            string truePath = Server.MapPath(path);

            return null;
        }
    }
}