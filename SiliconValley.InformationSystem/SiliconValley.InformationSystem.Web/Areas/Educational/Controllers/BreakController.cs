using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Business.EducationalBusiness;
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Util;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;

    public static class MyEntity{
        public static readonly BreakManeger Break_Entity = new BreakManeger();
     }
    [CheckLogin]
    public class BreakController : Controller
    {
        // GET: /Educational/Break/BreakIndexView
        public ActionResult BreakIndexView()
        {
            //加载阶段
            List<SelectListItem> g_list = MyEntity.Break_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString(),Selected=false }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.s_grandlist = g_list;
             
            //获取上课违纪类型
            BaseDataEnum find_dataV = BreakManeger.BaseDataEnum_Entity.GetSingData("上课违纪类型", false);
            List<SelectListItem> typelist = BreakManeger.BaseDataEnum_Entity.GetChildData(find_dataV.Id).Select(b => new SelectListItem { Text = b.Name, Value = b.Id.ToString(),Selected=false }).ToList();
            typelist.Add(new SelectListItem() { Text="--请选择--" ,Value="0" ,Selected=true});
            ViewBag.Selct_V = typelist;
            return View();
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetBrakData(int limit,int page)
        {
            try
            {
                string Name = Request.QueryString["s_class"];
                List<BaseDataView> break_list = MyEntity.Break_Entity.ALL_DATA().Where(b => b.IsDelete == false).OrderByDescending(b => b.Id).ToList();
                if (Name != null)
                {
                    int classid = Convert.ToInt32(Name);
                   break_list = break_list.Where(b => b.ClassSchedule_Id == classid).ToList();
                }
                string type = Request.QueryString["typeV"];
                if (type!="0" && type!=null)
                {
                    int typeid = Convert.ToInt32(type);
                    break_list = break_list.Where(b => b.Violationofdiscipline_Id== typeid).ToList();
                }
                string time = Request.QueryString["s_time"];
                if (time!="0" && time!=null)
                {
                    break_list= break_list.Where(b => b.BaseDataTime==time).ToList();
                }
                string starDate = Request.QueryString["stateTime"];
                if (!string.IsNullOrEmpty(starDate))
                {

                    break_list = break_list.Where(b => b.RecodeDate >= Convert.ToDateTime(starDate)).ToList();
                }

                string endDate = Request.QueryString["endTime"];
                if (!string.IsNullOrEmpty(endDate))
                {
                    break_list = break_list.Where(b => b.RecodeDate <= Convert.ToDateTime(endDate)).ToList();
                }
                var mydata = break_list.Skip((page - 1) * limit).Take(limit);

                var datajson = new
                {
                    code = 0,
                    msg = "",
                    count = break_list.Count,
                    data = mydata
                };
                return Json(datajson, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                string st = ex.Message;
            }

            return null;
        }

        public ActionResult AddorEditView(int? Id)
        {
            //加载阶段
            List<SelectListItem> g_list = MyEntity.Break_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString(), Selected = false }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.Add_grandlist = g_list;

            //获取上课违纪类型
            BaseDataEnum find_dataV = BreakManeger.BaseDataEnum_Entity.GetSingData("上课违纪类型", false);
            ViewBag.Add_V = BreakManeger.BaseDataEnum_Entity.GetChildData(find_dataV.Id).Select(b => new SelectListItem { Text = b.Name, Value = b.Id.ToString() }).ToList();

            

            if (Id==null)
            {
                //添加
                return View();
            }
            else
            {
                //编辑
               MyBreak find_b= MyEntity.Break_Entity.GetEntity(Id);                 
                    return View(find_b);                            
            }
            
        }
        [HttpPost]
        public ActionResult AddorEditFunction()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            string Count = Request.Form["Count"];//违纪类型与人数

            
            List<MyBreak> blist = new List<MyBreak>();

            AjaxResult a = new AjaxResult();
            try
            {
                string[] Counts= Count.Split(',');
                foreach (string item in Counts)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        int ClassSchedule_Id = Convert.ToInt32(Request.Form["ClassSchedule_Id"]);

                        string BaseDataTime = Request.Form["BaseDataTime"];//时间段

                        string Rmark = Request.Form["Rmark"];
                        string empstring = Request.Form["empstring"];//上课老师

                        DateTime date = Convert.ToDateTime(Request.Form["RecodeDate"]);

                        string[] istr = item.Split(':');
                        MyBreak bre = new MyBreak();
                        bre.Count = Convert.ToInt32(istr[1]);
                        bre.Violationofdiscipline_Id = Convert.ToInt32(istr[0]);
                        bre.Emp_Id = UserName.EmpNumber;
                        bre.ClassSchedule_Id = ClassSchedule_Id;
                        bre.RecodeDate = date;
                        bre.BaseDataTime = BaseDataTime;
                        bre.IsDelete = false;
                        bre.Rmark = Rmark;
                        bre.empstring = empstring;

                        blist.Add(bre);
                    }                                       
                }

               a= MyEntity.Break_Entity.Addlist(blist);


                //if (b.Id > 0)
                //{
                //    //Break fin_b = MyEntity.Break_Entity.GetEntity(b.Id);                    
                //    ////编辑

                //    //fin_b.ClassSchedule_Id = b.ClassSchedule_Id;

                //    //fin_b.BaseDataEnum_Id = b.BaseDataEnum_Id;
                //    //fin_b.Rmark = b.Rmark;
                //    //MyEntity.Break_Entity.Update(fin_b);
                //}
                //else
                //{

                //    //添加                   
                //    MyEntity.Break_Entity.Insert(b);
                //}

                return Json(a, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message,JsonRequestBehavior.AllowGet);
            }
             
             
        }
        
        [HttpPost]
        public ActionResult Deletefunction(int Id)
        {
            MyBreak fin_b = MyEntity.Break_Entity.GetEntity(Id);
             //删除
                fin_b.IsDelete = true;
                MyEntity.Break_Entity.Update(fin_b);
                return Json("ok", JsonRequestBehavior.AllowGet);            
        }

        public ActionResult EditView(int id)
        {
            //根据Id获取要修改的数据
            BaseDataView find= MyEntity.Break_Entity.GetSingData(id);

            //加载阶段
            List<SelectListItem> g_list = MyEntity.Break_Entity.GetEffectiveData().Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString(), Selected = false }).ToList();
            g_list.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.Add_grandlist = g_list;
            return View(find);
        }

        [HttpPost]
        public ActionResult EditFunction(BaseDataView newdata)
        {
            MyBreak find = MyEntity.Break_Entity.GetFindId(newdata.Id);

            find.ClassSchedule_Id = newdata.ClassSchedule_Id;
            find.RecodeDate =Convert.ToDateTime( newdata.RecodeDate);
            find.Count = newdata.count;
            find.Rmark = newdata.Rmark;

           AjaxResult a=  MyEntity.Break_Entity.EditData(find);

            return Json(a,JsonRequestBehavior.AllowGet);
        }
    }
}