using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Entity.Base_SysManage;
using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.UserManeger;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Depository.CellPhoneSMS;

using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Web.Common;
using System.Drawing;
using System.IO;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Web.App_Start.IPHander;

namespace SiliconValley.InformationSystem.Web.Controllers
{
    //  /Login/LoginIndex
    [IgnoreLogin]
    [IgnoreUrlPermissionAttribute]
    public class LoginController : BaseMvcController
    {
        UsersInfoManeger userinfo = new UsersInfoManeger();
        EmployeesInfoManage empmanage = new EmployeesInfoManage();
        // GET: Login
        //登录页面
        [IPDisablePolicyFilter]
        public ActionResult LoginIndex()
        {
            return View();
        }
        [IPDisablePolicyFilter]
        //登录方法
        public ActionResult LoginFunction(Base_User u, string loginType, string mobile, string smsCaptcha, string code)
        {
            ErrorResult err = new ErrorResult();
            try
            {
                //账号密码登录
                if (loginType == "account")
                {
                    string pwd = Util.Extention.ToMD5String(u.Password);
                    Base_User findu = userinfo.GetList().Where(find => find.UserName == u.UserName && find.Password == pwd).FirstOrDefault();

                    if (findu != null && findu.State!=0)
                    {
                        SessionHelper sessionHelper = new SessionHelper();
                        //判断该员工是否离职
                        var emp= empmanage.FindEmpData(findu.EmpNumber,true);
                         
                            SessionHelper.Session["UserId"] = findu.UserId;
                            err.Success = true;
                            err.Msg = "登陆成功!";
                            err.Data = "/Base_SysManage/Base_SysMenu/Index";

                            //获取权限

                            var permisslist = PermissionManage.GetOperatorPermissionValues();

                            SessionHelper.Session["OperatorPermission"] = permisslist;
                            return Json(err, JsonRequestBehavior.AllowGet);
                         
                    }
                    else
                    {                        
                        err.Msg = "用户名或密码错误";
                    }

                }

                //手机短信验证码登录
                if (loginType == "phone")
                {
                    EmployeesInfo emp = empmanage.GetList().Where(e => e.Phone == mobile).FirstOrDefault();
                    if (emp.IsDel!=true)
                    {
                        if (emp != null && SessionHelper.Session["code"].ToString() == smsCaptcha)
                        {

                            Base_User myuser = userinfo.GetList().Where(user => user.EmpNumber == emp.EmployeeId).FirstOrDefault();

                            SessionHelper.Session["UserId"] = myuser.UserId;
                            err.Success = true;
                            err.Msg = "登陆成功!";
                            err.Data = "/Base_SysManage/Base_SysMenu/Index";

                            //获取权限

                            var permisslist = PermissionManage.GetOperatorPermissionValues();

                            SessionHelper.Session["OperatorPermission"] = permisslist;


                            return Json(err, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            err.Msg = "用户名或密码错误";
                        }
                    }
                    else
                    {
                        err.Msg = "员工已离职，账号不可用！";
                    }
                     
                }
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return Json(err, JsonRequestBehavior.AllowGet);
        }

        //手机短信实例
        public ActionResult PhoneSMS()
        {
            string number = "13723860463";
            string smsText = "达磊，下雨了！！！";
            string t = PhoneMsgHelper.SendMsg(number, smsText);
            return View();
        }

        /// <summary>
        /// 产生手机登录验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCode()
        {
            string code = LoginHelper.Code(4);
            //将验证码存储到Session中
            SessionHelper.Session["code"] = code;
            Bitmap image = LoginHelper.CreateCodeImage(code);
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] bytes = ms.ToArray();
            return File(bytes, "image/Jpeg");
        }

        public ActionResult GetPhoneCode(string mobile)
        {
            string code = LoginHelper.PhoneCode(4);
            //将手机校验码存储到Session中
            SessionHelper.Session["phonecode"] = code;
            //将校验码发送到指定的手机
            string msg = string.Format("您在{0}使用手机登录方式登录湖南硅谷高科软件学院信息平台，为了您的账户安全！请使用：{1} 验证码完成登录", DateTime.Now, code);
            PhoneMsgHelper.SendMsg(mobile, msg);
            ErrorResult er = new ErrorResult();
            er.Success = true;
            er.Msg = code;
            return Json(er, JsonRequestBehavior.AllowGet);
        }

        [IPDisablePolicyFilter]
        public ActionResult SLogin()
        {
            return View();
        }
        [IPDisablePolicyFilter]
        public ActionResult LoginAction(string username, string password)
        {
            //20052000082300013

            BaseBusiness<StudentInformation> dbst = new BaseBusiness<StudentInformation>();
            ErrorResult err = new ErrorResult();
            var student = dbst.GetEntity(username);

            if (student != null)
            {
                if (student.Password == password)
                {
                    SessionHelper.Session["studentnumber"] = student.StudentNumber;
                    SessionHelper.Session["UserId"] = student.StudentNumber;
                    err.Success = true;
                    err.Msg = "登陆成功!";
                    err.Data = "/student/index";
                    HttpCookie cookie = new HttpCookie("StudentNumber");
                    cookie.Value = student.StudentNumber;

                    Response.AppendCookie(cookie);
                    return Json(err, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    err.Success = false;
                    err.Msg = "用户名或密码错误！";
                    err.Data = null;

                    return Json(err, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                err.Success = false;
                err.Msg = "用户名或密码错误！";
                err.Data = null;

                return Json(err, JsonRequestBehavior.AllowGet);
            }


        }
    }
}