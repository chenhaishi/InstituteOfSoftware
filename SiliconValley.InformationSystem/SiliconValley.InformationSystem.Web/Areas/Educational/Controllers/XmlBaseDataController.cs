using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    using SiliconValley.InformationSystem.Business.EducationalBusiness;
    using SiliconValley.InformationSystem.Util;
    [CheckLogin]
    public class XmlBaseDataController : Controller
    {
        XmlManeger Xml_Entity = new XmlManeger();
        // GET: /Educational/XmlBaseData/UpdateFuntion
        public ActionResult XmlDataIndex()
        {
            return View();
        }

        public ActionResult GetData(int limit,int page)
        {
            string path = Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml");

            List<XmlEntity> list=  Xml_Entity.Getlist(path);

            var data = list.Skip((page - 1) * limit).Take(limit).ToList();

            var jsondata = new { code=0,msg="",count=list.Count,data=data};

            return Json(jsondata,JsonRequestBehavior.AllowGet) ;
        }


        public ActionResult AddView()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddFunction(XmlEntity entity)
        {
            string path = Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml");
            AjaxResult a = new AjaxResult();
            int count= Xml_Entity.Getlist(path).Where(l => l.Year == entity.Year).Count();
            if (count<=0)
            {
                a.Success = Xml_Entity.AddData(entity, path);
                a.Msg = a.Success == true ? "操作成功！！！" : "操作失败，请刷新重试！！";
            }
            else
            {
                a.Msg = "已有该数据！";
            }
             

            return Json(a,JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateView(string id)
        {
            string path = Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml");
            XmlEntity find=  Xml_Entity.Getlist(path).Where(l => l.Year == id).FirstOrDefault();
            return View(find);
        }

        [HttpPost]
        public ActionResult UpdateFuntion(XmlEntity entity)
        {
            string path = Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml");
            XmlEntity find = Xml_Entity.Getlist(path).Where(l => l.Year == entity.Year).FirstOrDefault();
            AjaxResult a = new AjaxResult();
            if (find!=null)
            {
                find.EndMonth = entity.EndMonth;
                find.StartMonth = entity.StartMonth;
                a.Success= Xml_Entity.EditData(entity, path);
            }
            else
            {
                a.Success = Xml_Entity.AddData(entity, path);
            }

            a.Msg = a.Success == false ? "操作失败！请刷新重试！！":"操作成功";

            return Json(a,JsonRequestBehavior.AllowGet);
        }

    }
}