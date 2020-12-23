using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.DormitoryMaintenance.Controllers
{
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Business.DormitoryMantainBusiness;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Util;
    [CheckLogin]
    public class DormitoryMantainPriceController : Controller
    {
        PricedormitoryarticlesManeger PricedorGood_Entity = new PricedormitoryarticlesManeger();
        EmployeesInfoManage Employess_Entity = new EmployeesInfoManage();

        // GET: /DormitoryMaintenance/DormitoryMantainPrice/DormitoryMantainPriceIndex
        public ActionResult DormitoryMantainPriceIndex()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            int number = Employess_Entity.GetDeptByEmpid(UserName.EmpNumber).DeptId;
            ViewBag.number = number;
            return View();
        }


        public ActionResult Tabledata(int limit, int page,string name)
        {
            List<Pricedormitoryarticles> list = PricedorGood_Entity.GetList(true);
           
            if (!string.IsNullOrEmpty(name))
            {
                list = list.Where(s=>s.Nameofarticle.Contains(name)).ToList();
            }
            
            var data = list.Skip((page - 1) * limit).Take(limit);

            var jsondata = new { count = list.Count, code = 0, msg = "", data = data };

            return Json(jsondata, JsonRequestBehavior.AllowGet);
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
            result.Success = PricedorGood_Entity.AddData(data);
            result.Msg = result.Success == true ? "操作成功！" : "操作失败！";

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///   修改页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditDormitoryMantainPriceView(int id)
        {
            Pricedormitoryarticles PriceEntity = PricedorGood_Entity.GetEntity(id);
            return View(PriceEntity);

        }

        /// <summary>
        ///   修改宿舍物品信息
        /// </summary>
        /// <param name="pricedormitoryarticles"></param>
        /// <returns></returns>
        public ActionResult EditFunction(Pricedormitoryarticles pricedormitoryarticles)
        {
            var AjaxResult = new AjaxResult();
            try
            {
                Pricedormitoryarticles price = PricedorGood_Entity.GetEntity(pricedormitoryarticles.ID);
                price.Nameofarticle = pricedormitoryarticles.Nameofarticle;
                price.Reentry = pricedormitoryarticles.Reentry;
                price.Remarks = pricedormitoryarticles.Remarks;
                PricedorGood_Entity.Update(price);
                AjaxResult = PricedorGood_Entity.Success();
            }
            catch (Exception ex)
            {
                AjaxResult = PricedorGood_Entity.Error(ex.Message);
            }

            return Json(AjaxResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改物品状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateStatus(int id)
        {
            Pricedormitoryarticles pricedormitoryarticles = PricedorGood_Entity.GetEntity(id);
            if (pricedormitoryarticles.Dateofregistration)
            {
                pricedormitoryarticles.Dateofregistration = false;
            }
            else {
                pricedormitoryarticles.Dateofregistration = true;
            }
            
            PricedorGood_Entity.Update(pricedormitoryarticles);

            var ajaxresult = new AjaxResult();
            ajaxresult = PricedorGood_Entity.Success();
            return Json(ajaxresult,JsonRequestBehavior.AllowGet);
        }

        
        public bool CheckName(string name)
        {
            bool flag = false;
            List<Pricedormitoryarticles> price_list = PricedorGood_Entity.GetList();
            var temp = price_list.Where(s=>s.Nameofarticle == name && s.Dateofregistration==true).Count();
            if (temp>0)
            {
                flag = true;
            }
            return flag;
        }
        
    }
}