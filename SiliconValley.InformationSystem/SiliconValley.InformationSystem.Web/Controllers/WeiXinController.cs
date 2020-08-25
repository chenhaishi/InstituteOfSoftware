using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Entity.Base_SysManage.ViewEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Web.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using SiliconValley.InformationSystem.Entity.Base_SysManage;

namespace SiliconValley.InformationSystem.Web.Controllers
{

    public class WeiXinController : BaseMvcController
    {
        private readonly Base_UserBusiness db_account;

        public WeiXinController()
        {
            db_account = new Base_UserBusiness();
        }
        [IgnoreLogin]
        // GET: WeiXin
        public ActionResult WXLogin(string code)
        {
            Base_SysLogBusiness base_SysLogBusiness = new Base_SysLogBusiness();
            try
            {
                QQ_callback callback = new QQ_callback();

                Weixin_info userinfo = callback.getWeixinUserInfoJSON(code);

                //匹配账号 

                //base_SysLogBusiness.WriteSysLog($"code:{code}");

                //base_SysLogBusiness.WriteSysLog($"用户信息:{JsonConvert.SerializeObject(userinfo)}");

                var account = db_account.GetList().Where(d => d.WX_Unionid == userinfo.Unionid).FirstOrDefault();

                if (account == null)
                {
                    errorMsg errorMsg = new errorMsg();

                    errorMsg.errorCode = "login404";
                    return View(viewName: "WXLoginError", model: errorMsg);
                } 

                //记录登录
                SessionHelper.Session["UserId"] = account.UserId;

                var permisslist = PermissionManage.GetOperatorPermissionValues();

                SessionHelper.Session["OperatorPermission"] = permisslist;

            }
            catch (Exception ex)
            {
                throw;

            }
            return Redirect("/Base_SysManage/Base_SysMenu/Index");
        }

     
        
        [HttpGet]
        public ActionResult BindingWX(string redirectUrl)
        {
            SessionHelper.Session["BindWXRedirectUrl"] = redirectUrl;
           
            return View();
            
        }

        
        public ActionResult DoBindingWX(string code)
        {

            QQ_callback callback = new QQ_callback();

            Weixin_info userinfo = callback.getWeixinUserInfoJSON(code);

            var currentUser = Base_UserBusiness.GetCurrentUser();

            Base_User user = db_account.GetEntity(currentUser.Id);

            //if (!string.IsNullOrEmpty(user.WX_Unionid))
            //{
            //    errorMsg errorMsg = new errorMsg();

            //    errorMsg.errorCode = "binding402";

            //    return View("WXLoginError", errorMsg);
            //}


            //存入信息
            user.WX_Unionid = userinfo.Unionid;

            db_account.Update(user);

            //跳转登录
            var redirectUrl = SessionHelper.Session["BindWXRedirectUrl"].ToString();

            return Redirect(redirectUrl);

        }

        [IgnoreLogin]
        public ActionResult WXLoginError()
        {

            return View();
        }
    }
}