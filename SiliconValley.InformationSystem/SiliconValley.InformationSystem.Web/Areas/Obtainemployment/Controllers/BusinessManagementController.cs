using Newtonsoft.Json.Linq;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Employment;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business.Common;

namespace SiliconValley.InformationSystem.Web.Areas.Obtainemployment.Controllers
{


    [CheckLogin]
    public class BusinessManagementController : Controller
    {

        /// <summary>
        /// 企业业务类
        /// </summary>
        private EnterpriseInfoBusiness Enter;

        /// <summary>
        /// 就业专员业务类
        /// </summary>
        private EmploymentStaffBusiness EmployStaff;
        /// <summary>
        /// 专业业务类
        /// </summary>
        private SpecialtyBusiness SpecBus;
        /// <summary>
        /// 企业专业业务类
        /// </summary>
        private EntSpeeBusiness EntSpeeBus;
        /// <summary>
        /// 用户业务类
        /// </summary>
        private Base_UserBusiness base_UserBusiness;
        private EmploymentStaffBusiness dbEmploymentStaff;

        /// <summary>
        /// 判断在企业专业表中，专业id是否对应这个一个企业Id
        /// </summary>
        /// <param name="EntId">企业ID</param>
        /// <param name="SpeID">专业ID</param>
        /// <returns></returns>
        public bool Existence(string EntId, int SpeeID)
        {
            EntSpeeBus = new EntSpeeBusiness();
            int newEntId = int.Parse(EntId);
            var EntSpeeList = EntSpeeBus.GetIQueryable().Where(a => a.IsDel == false).ToList();
            var entspee = EntSpeeList.Where(a => a.EntID == newEntId && a.SpeID == SpeeID).FirstOrDefault();
            if (entspee != null)
                return true;
            else
                return false;

        }
        /// <summary>
        /// 根据专业ID以及企业ID查找企业专业对像
        /// </summary>
        /// <param name="SpecID">专业id</param>
        /// <param name="EntID">企业id</param>
        /// <returns></returns>
        public EntSpee Existence(string SpecID, string EntID)
        {
            EntSpeeBus = new EntSpeeBusiness();
            int newSpecID = int.Parse(SpecID);
            int newEntId = int.Parse(EntID);
            var EntSpeeList = EntSpeeBus.GetIQueryable().Where(a => a.IsDel == false).ToList();
            return EntSpeeList.Where(a => a.EntID == newEntId && a.SpeID == newSpecID).FirstOrDefault();

        }
        /// <summary>
        /// 根据专业id查找专业对象
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public Specialty GetSpecialtyById(int Id)
        {
            SpecBus = new SpecialtyBusiness();
            return SpecBus.GetIQueryable().Where(a => a.Id == Id && a.IsDelete == false).FirstOrDefault();
        }
        #region 关于企业信息库管理
        /// <summary>
        /// 显示合作企业信息
        /// </summary>
        /// <returns></returns>
        public ActionResult EnterpriseInfoIndex()
        {
            return View();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="limit">一页多少条</param>
        /// <param name="whyshow">查询条件</param>
        /// <param name="EntName">企业名称</param>
        /// <param name="EntContacts">联系人</param>
        /// <returns></returns>
        public ActionResult SearchEnterpriseInfo(int page, int limit, string whyshow, string EntName, string EntContacts)
        {
            Enter = new EnterpriseInfoBusiness();
            EmployStaff = new EmploymentStaffBusiness();
            //企业信息
            var EnterList = Enter.GetIQueryable().OrderByDescending(a => a.ID).Where(a => a.IsDel == false).ToList();

            switch (whyshow)
            {
                case "ShowCoop":
                    EnterList = Enter.GetCooAll();
                    break;
                case "ShowNoCoop":
                    EnterList = Enter.GetNoCooAll();
                    break;
                default:
                    break;
            }
            var newdata = EnterList.Select(a => new ShowCoopEnterView
            {
                EnterpriseInfoID = a.ID,
                EntName = a.EntName,
                EntAddress = a.EntAddress,
                EntNature = a.EntNature,
                EntWelfare = a.EntWelfare,
                EntScale = a.EntScale,
                EmploymentStaffName = EmployStaff.GetEmployeesInfoByID(EmployStaff.GetEmploymentByID(a.EmpStaffID).EmployeesInfo_Id).EmpName,
                Remark = a.Remark,
                EntContacts = a.EntContacts,
                EntPhone = a.EntPhone,
                EntDate = a.EntDate
            }).ToList();
            if (!string.IsNullOrEmpty(EntName))
            {
                newdata = newdata.Where(a => a.EntName.Contains(EntName)).ToList();
            }
            if (!string.IsNullOrEmpty(EntContacts))
            {
                var dudu = Enter.GetCooAll().Where(a => a.EntContacts.Contains(EntContacts)).ToList();
                newdata = dudu.Select(a => new ShowCoopEnterView
                {
                    EnterpriseInfoID = a.ID,
                    EntName = a.EntName,
                    EntAddress = a.EntAddress,
                    EntNature = a.EntNature,
                    EntWelfare = a.EntWelfare,
                    EntScale = a.EntScale,
                    EmploymentStaffName = EmployStaff.GetEmployeesInfoByID(EmployStaff.GetEmploymentByID(a.EmpStaffID).EmployeesInfo_Id).EmpName,
                    Remark = a.Remark,
                    EntContacts = a.EntContacts,
                    EntPhone = a.EntPhone,
                    EntDate = a.EntDate
                }).ToList();
            }
            var bnewdata = newdata.Skip((page - 1) * limit).Take(limit).ToList();
            var returnObj = new
            {
                code = 0,
                msg = "",
                count = newdata.Count(),
                data = bnewdata
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        /// <summary>
        /// 根据企业id解除合作关系
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DelEnterpriseInfo(int EntID)
        {
            //没有进行验证处理。
            Enter = new EnterpriseInfoBusiness();
            AjaxResult ajaxResult = new AjaxResult();
            var OldEnter = Enter.GetEnterByID(EntID);
            OldEnter.IsCooper = false;
            OldEnter.EntContacts = string.Empty;
            OldEnter.EntPhone = string.Empty;
            try
            {
                Enter.Update(OldEnter);
                ajaxResult.Success = true;
                ajaxResult.Msg = "取消合作";
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
                ajaxResult.Success = false;
                ajaxResult.Msg = ex.Message;
            }

            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddEnterprise()
        {
            SpecBus = new SpecialtyBusiness();
            var Sepclist = SpecBus.GetIQueryable().Where(a => a.IsDelete == false).ToList();
            ViewBag.Sepclist = Sepclist;
            return View();
        }

        /// <summary>
        /// post添加合作企业
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="EntSpeeList"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddEnterprise(AddEnterpriseView jsonStr, string EntSpeeList)
        {
           
            Enter = new EnterpriseInfoBusiness();
            EnterpriseInfo enterpriseInfo = new EnterpriseInfo();
            var AjaxResultss = new AjaxResult();
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            dbEmploymentStaff = new EmploymentStaffBusiness();
            EmploymentStaff staff = dbEmploymentStaff.GetEmploymentByEmpInfoID(user.EmpNumber);
          
            //添加企业基础信息
            enterpriseInfo.EmpStaffID = staff.ID;
            enterpriseInfo.EntAddress = jsonStr.EntAddress;
            enterpriseInfo.EntDate = DateTime.Now;
            enterpriseInfo.EntName = jsonStr.EntName;
            enterpriseInfo.EntNature = jsonStr.EntNature;
            enterpriseInfo.EntScale = jsonStr.EntScale;
            enterpriseInfo.EntWelfare = jsonStr.EntWelfare;
            enterpriseInfo.IsDel = false;
            enterpriseInfo.Remark = jsonStr.Remark;
            enterpriseInfo.CooData = DateTime.Now;
            enterpriseInfo.EntContacts = jsonStr.EntContacts;
            enterpriseInfo.EntPhone = jsonStr.EntPhone;
            enterpriseInfo.IsCooper = true;
            try
            {
                Enter.Insert(enterpriseInfo);
                AjaxResultss.Success = true;
                AjaxResultss.Msg = "添加企业基础信息成功";
                Enter = new EnterpriseInfoBusiness();
                var newEnt = Enter.GetIQueryable().Where(a => a.EntName == jsonStr.EntName).FirstOrDefault();
                var EnterID = newEnt.ID;

                try
                {

                    AjaxResultss.Success = true;
                    AjaxResultss.Msg = "添加合作企业成功";
                    JArray jArray = JArray.Parse(EntSpeeList);
                    foreach (var item in jArray)
                    {
                        JObject jdata = (JObject)item;
                        EntSpee entSpee = new EntSpee();
                        entSpee.EntID = EnterID;
                        entSpee.IsDel = false;
                        entSpee.JobResponsibilities = jdata["JobResponsibilities"].ToString();
                        entSpee.Remark = string.Empty;
                        entSpee.Requirements = jdata["Requirements"].ToString();
                        entSpee.SpeeDate = DateTime.Now;
                        entSpee.SpeID = (int)jdata["SpeID"];
                        entSpee.EntSalary = jdata["EntSalary"].ToString();
                        EntSpeeBus = new EntSpeeBusiness();
                        try
                        {
                            EntSpeeBus.Insert(entSpee);
                            AjaxResultss.Success = true;
                            AjaxResultss.Msg = "添加成功";


                        }
                        catch (Exception ex)
                        {

                            AjaxResultss.Success = false;
                            AjaxResultss.Msg = ex.Message;

                        }
                    }
                }
                catch (Exception ex)
                {

                    AjaxResultss.Success = false;
                    AjaxResultss.Msg = ex.Message;

                }
            }
            catch (Exception ex)
            {
                AjaxResultss.Success = false;
                AjaxResultss.Msg = ex.Message;

            }
            return Json(AjaxResultss, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 编辑合作企业页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditEnterprise(string id)
        {
            //var UserID = SessionHelper.Session["UserId"].ToString();
            //var user= base_UserBusiness.GetIQueryable().Where(a => a.UserId == UserID).FirstOrDefault();
            //var EmpStaffNo = user.EmpNumber;

            Enter = new EnterpriseInfoBusiness();

            //EmployStaff = new EmploymentStaffBusiness();
            //var MrDEmployStaff = EmployStaff.GetIQueryable().Where(a => a.EmployeesInfo_Id == EmpStaffNo).FirstOrDefault();

            int ID = int.Parse(id);
            var entdata = Enter.GetIQueryable().Where(a => a.ID == ID).FirstOrDefault();
            //if (entdata.EmpStaffID== MrDEmployStaff.ID)
            //{

            EntSpeeBus = new EntSpeeBusiness();
            SpecBus = new SpecialtyBusiness();


            var modeldata = new EditEnterpriseView();
            modeldata.EntID = entdata.ID;
            modeldata.EntName = entdata.EntName;
            modeldata.EntAddress = entdata.EntAddress;
            modeldata.EntScale = entdata.EntScale;
            modeldata.EntNature = entdata.EntNature;
            modeldata.EntWelfare = entdata.EntWelfare;
            modeldata.Remark = entdata.Remark;
            modeldata.OperNO = entdata.EntContacts == null ? 0 : 1;
            modeldata.EntContacts = entdata.EntContacts;
            modeldata.EntPhone = entdata.EntPhone;

            ViewBag.EntSpeelist = EntSpeeBus.GetIQueryable().Where(a => a.EntID == entdata.ID).ToList();

            var Sepclist = SpecBus.GetIQueryable().Where(a => a.IsDelete == false).ToList();
            var SepclistView = Sepclist.Select(a => new MrDSpecialtyView
            {
                Id = a.Id,
                SpecialtyName = a.SpecialtyName,
                IsCheack = Existence(id, a.Id)
            }).ToList();
            ViewBag.SepclistView = SepclistView;
            return View(modeldata);
            //}
            //else
            //{
            //   var  AjaxResult = new AjaxResult();
            //    AjaxResult.Success = false;
            //    AjaxResult.Msg = "你没有权限修改其他人的合作企业信息";
            //    return Json(AjaxResult, JsonRequestBehavior.AllowGet);
            //}


        }

        [HttpPost]
        public ActionResult EditEnterprise(EditEnterpriseView editEnterpriseView, string EntSpeeList)
        {

            int OperNo = editEnterpriseView.OperNO;
            var AjaxResultss = new AjaxResult();
            var entid = editEnterpriseView.EntID;
            Enter = new EnterpriseInfoBusiness();
            var data = Enter.GetEnterByID(entid);
            data.EntAddress = editEnterpriseView.EntAddress;
            data.EntName = editEnterpriseView.EntName;
            data.EntNature = editEnterpriseView.EntNature;
            data.EntScale = editEnterpriseView.EntScale;
            data.EntWelfare = editEnterpriseView.EntWelfare;
            data.Remark = editEnterpriseView.Remark;
            if (OperNo > 0)
            {
                data.EntContacts = editEnterpriseView.EntContacts;
                data.EntPhone = editEnterpriseView.EntPhone;
                data.IsCooper = true;
            }
            else
            {
                data.EntContacts = string.Empty;
                data.EntPhone = string.Empty;
                data.IsCooper = false;
            }
            try
            {
                Enter.Update(data);

                var mybool = Operation(entid, EntSpeeList);
                if (mybool)
                {
                    AjaxResultss.Success = true;
                    AjaxResultss.Msg = "修改企业成功";
                }
                else
                {
                    AjaxResultss.Success = false;
                    AjaxResultss.Msg = "修改企业失败";
                }
            }
            catch (Exception ex)
            {
                AjaxResultss.Success = false;
                AjaxResultss.Msg = ex.Message;
            }

            return Json(AjaxResultss, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改企业专业操作
        /// </summary>
        /// <param name="entid"></param>
        /// <param name="EntSpeeList"></param>
        /// <returns></returns>
        private bool Operation(int entid, string EntSpeeList)
        {
            EntSpeeBus = new EntSpeeBusiness();
            bool result = false;
            JArray jArray = JArray.Parse(EntSpeeList);

            List<EntSpee> entspeeList = EntSpeeBus.GetIQueryable().Where(a => a.EntID == entid).ToList();
            try
            {
                EntSpeeBus.Delete(entspeeList);

                List<EntSpee> entSpees = new List<EntSpee>();
                foreach (var item in jArray)
                {
                    JObject jdata = (JObject)item;
                    var entSpee = new EntSpee();
                    entSpee.EntID = entid;
                    entSpee.IsDel = false;
                    entSpee.JobResponsibilities = jdata["JobResponsibilities"].ToString();
                    entSpee.Remark = string.Empty;
                    entSpee.Requirements = jdata["Requirements"].ToString();
                    entSpee.SpeeDate = DateTime.Now;
                    entSpee.SpeID = (int)jdata["SpeID"];
                    entSpee.EntSalary = jdata["EntSalary"].ToString();
                    entSpees.Add(entSpee);
                }
                try
                {
                    EntSpeeBus.Insert(entSpees);
                    result = true;
                }
                catch (Exception ex)
                {
                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        /// <summary>
        /// 用来显示企业专业的信息
        /// </summary>
        /// <param name="SpecID"></param>
        /// <param name="EntID"></param>
        /// <returns></returns>
        public ActionResult GetEntSpeeInfo(string SpecID, string EntID)
        {
            var EntSpee = Existence(SpecID, EntID);
            SpecBus = new SpecialtyBusiness();
            int newSpecID = int.Parse(SpecID);
            var MrDSpec = SpecBus.GetIQueryable().Where(a => a.Id == newSpecID).FirstOrDefault();
            var ResultObj = new
            {
                JobResponsibilities = EntSpee.JobResponsibilities,
                Requirements = EntSpee.Requirements,
                EntSalary = EntSpee.EntSalary,
                SpecialtyName = MrDSpec.SpecialtyName
            };
            AjaxResult ajaxResult = new AjaxResult();
            ajaxResult.Success = true;
            ajaxResult.Msg = "1";
            ajaxResult.Data = ResultObj;
            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 判断公司名称是否唯一
        /// </summary>
        /// <param name="EntName"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EntNameOnly(string EntName, int? EntID)
        {

            Enter = new EnterpriseInfoBusiness();
            var AjaxResultss = new AjaxResult();
            var MrDEnt = Enter.GetEnterByID(EntID);
            if (MrDEnt != null && MrDEnt.EntName == EntName)
            {
                AjaxResultss.Success = true;
                AjaxResultss.ErrorCode = 1;
                AjaxResultss.Msg = "没问题。";
            }
            else
            {
                try
                {
                    var enterobjj = Enter.GetIQueryable().Where(a => a.EntName == EntName && a.IsDel == false).FirstOrDefault();
                    if (enterobjj != null)
                    {
                        AjaxResultss.Success = true;
                        AjaxResultss.ErrorCode = 0;
                        AjaxResultss.Msg = "存在重复的公司名称。";
                    }
                }
                catch (Exception ex)
                {
                    AjaxResultss.Success = false;
                    AjaxResultss.ErrorCode = 1;
                    AjaxResultss.Msg = "请刷新页面。";
                }
            }

            return Json(AjaxResultss, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 判断合作企业的联系人电话是否唯一
        /// </summary>
        /// <param name="EntPhoneOn"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EntPhoneOnly(string EntPhone, int? EntID)
        {


            Enter = new EnterpriseInfoBusiness();
            var AjaxResultss = new AjaxResult();
            var MrDEnt = Enter.GetEnterByID(EntID);
            if (MrDEnt != null && MrDEnt.EntPhone == EntPhone)
            {
                AjaxResultss.Success = true;
                AjaxResultss.ErrorCode = 1;
                AjaxResultss.Msg = "没问题。";
            }
            else
            {
                try
                {
                    var enterobjj = Enter.GetIQueryable().Where(a => a.EntPhone == EntPhone && a.IsDel == false).FirstOrDefault();
                    if (enterobjj != null)
                    {
                        AjaxResultss.Success = true;
                        AjaxResultss.ErrorCode = 0;
                        AjaxResultss.Msg = "存在重复的电话号码。";
                    }
                }
                catch (Exception ex)
                {
                    AjaxResultss.Success = false;
                    AjaxResultss.ErrorCode = 1;
                    AjaxResultss.Msg = "请刷新页面。";
                }
            }

            return Json(AjaxResultss, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 详细页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Enterdetail(int id)
        {

            Enter = new EnterpriseInfoBusiness();
            EntSpeeBus = new EntSpeeBusiness();
            SpecBus = new SpecialtyBusiness();
            var enterobj = Enter.GetEnterByID(id);
            EnterdetailView enterdetailView = new EnterdetailView();
            enterdetailView.EntAddress = enterobj.EntAddress;
            enterdetailView.EntName = enterobj.EntName;
            enterdetailView.EntNature = enterobj.EntNature;
            enterdetailView.EntScale = enterobj.EntScale;
            enterdetailView.EntWelfare = enterobj.EntWelfare;
            enterdetailView.Remark = enterobj.Remark;
            enterdetailView.EntContacts = enterobj.EntContacts;
            enterdetailView.EntPhone = enterobj.EntPhone;

            var ListEntSpee = EntSpeeBus.GetIQueryable().Where(a => a.EntID == id).ToList().Select(a => new MrDDetailSpecView
            {
                JobResponsibilities = a.JobResponsibilities,
                Requirements = a.Requirements,
                EntSalary = a.EntSalary,
                SpecialtyName = GetSpecialtyById(a.SpeID).SpecialtyName
            }).ToList();

            ViewBag.NowSpee = Newtonsoft.Json.JsonConvert.SerializeObject(ListEntSpee);

            return View(enterdetailView);
        }
        #endregion
    }
}