using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    using SiliconValley.InformationSystem.Business.EducationalBusiness;
    [CheckLogin]
    public class XmlBaseDataController : Controller
    {
        XmlManeger Xml_Entity = new XmlManeger();
        // GET: Educational/XmlBaseData/XmlDataIndex
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


    }
}