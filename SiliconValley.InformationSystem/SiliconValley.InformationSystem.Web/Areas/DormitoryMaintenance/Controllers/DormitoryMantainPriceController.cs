using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.DormitoryMaintenance.Controllers
{
    using SiliconValley.InformationSystem.Business.DormitoryMantainBusiness;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Util;
    [CheckLogin]
    public class DormitoryMantainPriceController : Controller
    {
        PricedormitoryarticlesManeger PricedorGood_Entity = new PricedormitoryarticlesManeger();

        // GET: /DormitoryMaintenance/DormitoryMantainPrice/DormitoryMantainPriceIndex
        public ActionResult DormitoryMantainPriceIndex()
        {
            return View();
        }


        public ActionResult Tabledata(int limit,int page)
        {
            List<Pricedormitoryarticles> list= PricedorGood_Entity.GetList(true);

            var data = list.Skip((page - 1) * limit).Take(limit);

            var jsondata = new {count=list.Count,code=0,msg="",data=data};

            return Json(jsondata,JsonRequestBehavior.AllowGet); ;
        }

        /// <summary>
        /// 添加页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddDormitoryMantainPriceView()
        {
            return View();
        }

        /// <summary>
        /// 添加方法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFunction(Pricedormitoryarticles data)
        {
            AjaxResult result = new AjaxResult();
            data.Addtime = DateTime.Now;
            data.Dateofregistration = true;
            result.Success= PricedorGood_Entity.AddData(data);
            result.Msg = result.Success == true ? "操作成功！" : "操作失败！";

            return Json(result,JsonRequestBehavior.AllowGet);
        }


    }
}