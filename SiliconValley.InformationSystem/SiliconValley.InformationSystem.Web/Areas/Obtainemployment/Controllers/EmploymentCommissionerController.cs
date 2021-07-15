using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Employment;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Obtainemployment.Controllers
{


    [CheckLogin]
    /// <summary>
    /// 就业专员管理
    /// </summary>
    public class EmploymentCommissionerController : Controller
    {
        /// <summary>
        /// 就业专员业务类
        /// </summary>
        private EmploymentStaffBusiness MrDEmployStaffBus;
        /// <summary>
        /// 就业区域业务类
        /// </summary>
        private EmploymentAreasBusiness MrDEmployAreBus;
        /// <summary>
        /// 管理就业专员首页
        /// </summary>
        /// <returns></returns>
        public ActionResult EmploymentCommissionerIndex()
        {
            MrDEmployAreBus = new EmploymentAreasBusiness();
            ViewBag.arealist= MrDEmployAreBus.GetAll();
            return View();
        }
        /// <summary>
        /// 显示就业专员的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchData(int page, int limit, int? areaid, string name, string phone)
        {
            MrDEmployStaffBus = new EmploymentStaffBusiness();
            MrDEmployAreBus = new EmploymentAreasBusiness();
            //根据部门查出没离职的就业专员
            var MrDEmpInfoData = MrDEmployStaffBus.GetALl().OrderByDescending(a => a.ID).ToList();
            if (areaid>0)
            {
                MrDEmpInfoData= MrDEmpInfoData.Where(a => a.AreaID == areaid).ToList();

            }
            var returnlist = new List<MrDEmployStaffView>();
             
            foreach (var item in MrDEmpInfoData)
            {
                EmployeesInfo info = MrDEmployStaffBus.GetEmployeesInfoByID(item.EmployeesInfo_Id);
                var obj = new MrDEmployStaffView();
                obj.EmploymentStaffID = item.ID;
                obj.EmpName = info.EmpName;
                obj.Phone = info.Phone;
                obj.EmployeeId = item.EmployeesInfo_Id;
                obj.Sex = info.Sex;
                obj.AreaName =MrDEmployAreBus.GetObjByID(item.AreaID).AreaName;
                returnlist.Add(obj);
            }

            if (!string.IsNullOrEmpty(name))
            {
                returnlist = returnlist.Where(a => a.EmpName.Contains(name)).ToList();
            }
            if (!string.IsNullOrEmpty(phone))
            {
                returnlist = returnlist.Where(a => a.Phone.Contains(phone)).ToList();
            }
            var bnewdata = returnlist.Skip((page - 1) * limit).Take(limit).ToList();
            var returnObj = new
            {
                code = 0,
                msg = "",
                count = returnlist.Count(),
                data = bnewdata
            };

            return Json(returnObj, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 显示详细页面
        /// </summary>
        /// <param name="id">就业专员ID</param>
        /// <returns></returns>
        public ActionResult EmployDetailView(int id)
        {

            MrDEmployStaffBus = new EmploymentStaffBusiness();
            EmployStaffDetailView result = MrDEmployStaffBus.GetStaffDetailView(id);
            return View(result);
        }

        [HttpGet]
        /// <summary>
        /// get 请求的编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditEmployStaff(int id)
        {
            MrDEmployStaffBus = new EmploymentStaffBusiness();
            var empstaff = MrDEmployStaffBus.GetEmploymentByID(id);
            var empinfo = MrDEmployStaffBus.GetEmployeesInfoByID(empstaff.EmployeesInfo_Id);
            ViewBag.empinfo = empinfo;
            MrDEmployAreBus = new EmploymentAreasBusiness();
            var AreaData = MrDEmployAreBus.GetAll().Select(s1 => new SelectListItem() { Text = s1.AreaName, Value = s1.ID.ToString() }).ToList();
            SelectListItem s = new SelectListItem() { Text = "---待定---", Value = "-1", Selected = true };
            AreaData.Add(s);
            ViewBag.Area = AreaData;
            return View(empstaff);
        }

        /// <summary>
        /// post 请求修改数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditEmployStaff(EmploymentStaff employment)
        {
            AjaxResult result = null;

            MrDEmployStaffBus = new EmploymentStaffBusiness();
            var bubu = MrDEmployStaffBus.GetEmploymentByID(employment.ID);
            bubu.AttendClassStyle = employment.AttendClassStyle.Trim();
            bubu.EmployExperience = employment.EmployExperience.Trim();
            bubu.WorkExperience = employment.WorkExperience.Trim();
            bubu.Remark = employment.Remark.Trim();
            if (employment.AreaID > 0)
            {
                bubu.AreaID = employment.AreaID;
            }
            else
            {
                bubu.AreaID = null;
            }
            try
            {
                MrDEmployStaffBus.Update(bubu);
                result = new SuccessResult();
                result.Msg = "修改成功";
                result.Success = true;

            }
            catch (Exception)
            {
                result = new ErrorResult();
                result.ErrorCode = 500;
                result.Msg = "服务器错误1";

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}