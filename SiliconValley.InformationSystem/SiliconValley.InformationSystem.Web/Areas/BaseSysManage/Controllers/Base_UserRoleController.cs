using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.Base_SysManage;
using SiliconValley.InformationSystem.Entity.Base_SysManage.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.BaseSysManage.Controllers
{
    using SiliconValley.InformationSystem.Business;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity;
    using SiliconValley.InformationSystem.Util;
    using SiliconValley.InformationSystem.Entity.Entity;

    [CheckLogin]
    public class Base_UserRoleController : Controller
    {


        private readonly Base_UserBusiness db_user;
        private readonly Base_SysRoleBusiness db_role;
        private readonly OtherPermissionBusiness db_otherPermission;
        public Base_UserRoleController()
        {
            db_user = new Base_UserBusiness();
            db_role = new Base_SysRoleBusiness();
            db_otherPermission = new OtherPermissionBusiness();
        }


        // GET: /BaseSysManage/Base_UserRole/ChangeState


        /// <summary>
        /// 角色管理
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleIndex()
        {
            return View();
        }


        /// <summary>
        /// 账号页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AccountIndex()
        {

            return View();
        }

        public ActionResult AccountData(int page, int limit,  string empname, string empnumber)
        {

            List<Base_User> userlist = db_user.GetList();
            List<Base_User> searchlist = new List<Base_User>();
            EmployeesInfoManage dbemp = new EmployeesInfoManage();


            if (!string.IsNullOrEmpty(empname) && string.IsNullOrEmpty(empnumber))
            {
                //按照姓名筛选
                userlist.ForEach(u=>
                {
                    var empobj = dbemp.GetInfoByEmpID(u.EmpNumber);

                    if (empobj != null)
                    {

                        if (empobj.EmpName.Contains(empname))
                        {
                            //从集合中删除
                            searchlist.Add(u);
                        }
                    }

                });
               
            }

            if (string.IsNullOrEmpty(empname) && string.IsNullOrEmpty(empnumber))
            {
                searchlist.AddRange(userlist);
            }

            var skiplist = searchlist.Skip((page - 1) * limit).Take(limit).ToList();

            ///转为模型视图

            List<AccountView> accountlist = new List<AccountView>();

            foreach (var item in skiplist)
            {
                var tempobj = db_user.ConvetToView(item);

                if (tempobj != null)
                {
                    accountlist.Add(tempobj);
                }
            }


            var obj = new
            {

                code = 0,
                msg = "",
                count = searchlist.Count,
                data = accountlist

            };
            return Json(obj, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 创建账号视图
        /// </summary>
        /// <returns></returns>
        public ActionResult createAccount()
        {
            return View();
        }

        public ActionResult selectEmp()
        {
            //提供部门数据
            BaseBusiness<Department> dbdep = new BaseBusiness<Department>();

            ViewBag.Deplist = dbdep.GetList().Where(d => d.IsDel == false).ToList();

            return View();
        }


        /// <summary>
        /// 员工数据
        /// </summary>
        /// <returns></returns>
        public ActionResult empData(int limit, int page, int depid)
        {

            EmployeesInfoManage dbemp = new EmployeesInfoManage();

            List<EmployeesInfo> emplist = new List<EmployeesInfo>();

            if (depid == 0)
            {
                //获取所有员工

                emplist.AddRange(dbemp.GetAll());

            }
            else
            {
                //根据部门帅选
                emplist.AddRange(dbemp.GetEmpsByDeptid(depid));

            }

            var skiplist = emplist.Skip((page - 1) * limit).Take(limit).ToList();

            List<EmpDetailView> viewlist = new List<EmpDetailView>();

            SiliconValley.InformationSystem.Business.TeachingDepBusiness.TeacherBusiness dbteacher = new Business.TeachingDepBusiness.TeacherBusiness();

            foreach (var item in skiplist)
            {
                var tempobj = dbteacher.ConvertToEmpDetailView(item);

                if (tempobj != null)
                    viewlist.Add(tempobj);
            }

            var obj = new
            {

                code = 0,
                msg = "",
                count = emplist.Count,
                data = viewlist

            };

            return Json(obj, JsonRequestBehavior.AllowGet);


        }


        /// <summary>
        /// 创建账号
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="empnumber">员工编号</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult createAccount(string username, string empnumber)
        {
            AjaxResult result = new AjaxResult();


            try
            {
                db_user.createAccount(username, empnumber);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "正常!";
            }
            catch (Exception ex)
            {
                if (ex.Message == "该用户名已存在！")
                {
                    result.ErrorCode = 444;
                    result.Data = null;
                    result.Msg = "用户名重复!";
                }
                else
                {
                    result.ErrorCode = 500;
                    result.Data = null;
                    result.Msg = "服务器异常!";
                }


            }

            return Json(result, JsonRequestBehavior.AllowGet);


        }


        /// <summary>
        /// 角色数据
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleData(int page, int limit, string roleName)
        {
            List<Base_SysRole> rolelist = new List<Base_SysRole>();

            if (roleName == null)
            {
                //获取全部数据
                rolelist.AddRange(db_role.GetList().ToList());

            }
            else
            {
                rolelist.AddRange(db_role.GetList().ToList().Where(d => d.RoleName.Contains(roleName)).ToList());
            }


            var skiplist = rolelist.Skip((page - 1) * limit).Take(limit).ToList();

            var obj = new
            {
                code = 0,
                msg = "",
                count = rolelist.Count,
                data = skiplist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 创建角色 
        /// </summary>
        /// <returns></returns>
        /// []
        /// 
        [HttpGet]
        public ActionResult createRole()
        {
            return View();
        }



        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="roleName">角色名称</param>
        /// <param name="businessName">业务名称</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult createRole(string roleName, string businessName)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                result = db_role.createRole(roleName, businessName);


            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "服务器异常";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 用户角色管理
        /// </summary>
        /// <returns></returns>
        public ActionResult UserRoelManage(string userId)
        {
            //获取用户
            var user = db_user.GetList().Where(d => d.UserId == userId).FirstOrDefault();

            ViewBag.user = user;

            return View();
        }


        /// <summary>
        /// 用户角色数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public ActionResult UserRoleManageData(string userId)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                //获取所有角色
                var allrolelist = db_role.GetList();

                List<object> allrolelist_obj = new List<object>();

                ///转换数据类型
                foreach (var item in allrolelist)
                {
                    var temp = new
                    {

                        value = item.RoleId,
                        title = item.RoleName
                    };

                    allrolelist_obj.Add(temp);
                }

                //获取用户的角色
                var HaveRolelis = Base_UserBusiness.GetTheUser(userId);


                var tempobj = new
                {
                    alllist = allrolelist_obj,
                    havelist = HaveRolelis
                };

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = tempobj;



            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 给账户授予角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIdlist"></param>
        /// <returns></returns>


        [HttpPost]
        public ActionResult SetUserRoles(string userId, List<string> roleIdlist)
        {

            AjaxResult result = new AjaxResult();

            try
            {

                //首先排除掉账号所拥有的角色
                //List<string> WillsetRolelist = new List<string>();

                //var currentHaverolelist = Base_UserBusiness.GetUserRoleIds(userId);

                //foreach (var item in roleIdlist)
                //{
                //    //判断是否已经拥有了该角色
                //    if (!db_user.IsContains(currentHaverolelist, item))
                //    {
                //        WillsetRolelist.Add(item);
                //    }
                //}

                db_user.SetUserRole(userId, roleIdlist);

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 角色菜单配置
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleUrlPermiss(string roleId)
        {
            //获取角色
            var role = db_role.GetList().Where(d => d.RoleId == roleId).FirstOrDefault();
            ViewBag.role = role;
            return View();

        }



        [HttpPost]
        public ActionResult RoleUrlPermissData(string roleId)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                var RoleTempPermisslist = db_role.RolePermission(roleId);
                var allpermisslist = PermissionManage.GetAllPermissionModules();

                List<Common.layuitree> treelist = new List<Common.layuitree>();

                foreach (var item in RoleTempPermisslist)
                {
                    //加载第一层
                    Common.layuitree firsttree = new Common.layuitree();

                    firsttree.field = item.Value;
                    firsttree.title = item.Name;
                    firsttree.id = item.Value;
                    //加载第二ceng
                    foreach (var item1 in item.Items)
                    {
                        Common.layuitree secondtree = new Common.layuitree();

                        secondtree.field = item1.Value;
                        secondtree.title = item1.Name;
                        secondtree.id = item.Value + item1.Value;
                        //判断是否有该权限
                        var ischeck = false;

                        if (item1.IsChecked == true)
                        {
                            ischeck = true;
                            firsttree.spread = true;
                        }


                        secondtree.@checked = ischeck;

                        firsttree.children.Add(secondtree);
                    }

                    treelist.Add(firsttree);
                }


                result.ErrorCode = 200;
                result.Msg = "";
                result.Data = treelist;

            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "";
                result.Data = null;
            }



            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 给角色配置菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult SetPermissToRole(string roleId, List<Common.layuitree> permisslist)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                List<string> WillsetPermisslist = new List<string>();

                foreach (var item in permisslist)
                {
                    foreach (var item1 in item.children)
                    {
                        WillsetPermisslist.Add(item.field + "." + item1.field);
                    }
                }

                db_role.SavePermission(roleId, WillsetPermisslist);
                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 200;
                result.Msg = "";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);


        }


        /// <summary>
        /// 添加其他权限页面
        /// </summary>
        /// <returns></returns>
        public ActionResult setOtherPermissTorole(string role)
        {
            ViewBag.role = role;

            return View();
        }

        public ActionResult OtherPermissionData(string role, int page, int limit)
        {
            //获取这个角色没有的权限

            var alllist = db_otherPermission.AllOtherPermissions(); //所有其他权限

            var rolelist = db_otherPermission.GetPermissionByRole(role);

            //所有角色 减去 拥有角色 = 未拥有角色

            List<OthorPermission> resultlist = new List<OthorPermission>();

            foreach (var item in alllist)
            {
                if (!db_otherPermission.IsContains(rolelist, item))
                {
                    resultlist.Add(item);
                }
            }

            var skiplist = resultlist.Skip((page - 1) * limit).Take(limit).ToList();

            var obj = new
            {
                code = 0,
                msg = "",
                count = resultlist.Count,
                data = skiplist

            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HaveOtherPermissionData(int page, int limit, string role)
        {
            var list = db_otherPermission.GetPermissionByRole(role);
            var skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();


            var obj = new
            {

                code = 0,
                msg = "",
                count = list.Count,
                data = skiplist
            };


            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除其他权限
        /// </summary>
        /// <returns></returns>
        public ActionResult remove(string role, string permissionValues)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var permission_arry = permissionValues.Split(',').ToList();
                permission_arry.RemoveAt(permission_arry.Count - 1);
                db_otherPermission.removeRolePermissions(role, permission_arry);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "";

            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Data = null;
                result.Msg = "";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除其他权限
        /// </summary>
        /// <returns></returns>
        public ActionResult add(string role, string permissionValues)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var permission_arry = permissionValues.Split(',').ToList();
                permission_arry.RemoveAt(permission_arry.Count - 1);
                db_otherPermission.addRolePermissions(role, permission_arry);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "";

            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Data = null;
                result.Msg = "";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult selectMenuPermission()
        {
            return View();
        }
        public ActionResult menuPermissionlist()
        {
            AjaxResult result = new AjaxResult();

            try
            {

                var allpermisslist = PermissionManage.GetAllPermissionModules();

                List<Common.layuitree> treelist = new List<Common.layuitree>();

                foreach (var item in allpermisslist)
                {
                    //加载第一层
                    Common.layuitree firsttree = new Common.layuitree();

                    firsttree.field = item.Value;
                    firsttree.title = item.Name;
                    firsttree.id = item.Value;
                    //加载第二ceng
                    foreach (var item1 in item.Items)
                    {
                        Common.layuitree secondtree = new Common.layuitree();

                        secondtree.field = item.Value + "." + item1.Value;
                        secondtree.title = item1.Name;
                        secondtree.id = item.Value + item1.Value;

                        firsttree.children.Add(secondtree);
                    }

                    treelist.Add(firsttree);
                }


                result.ErrorCode = 200;
                result.Msg = "";
                result.Data = treelist;

            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "";
                result.Data = null;
            }



            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public ActionResult ResetPasswd(string userid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                db_user.ResetPasswd(userid);

                result.ErrorCode = 200;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
            }

            return Json(result);

            
        }

        public ActionResult UpdatePasswd(string userid,string passwd)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                db_user.UpdatePassword(userid, passwd);

                result.ErrorCode = 200;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
            }

            return Json(result);


        }

        public ActionResult SetUp()
        {
            return View();
        }
        /// <summary>
        /// 密码修改
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Translate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Translate(string password)
        {
            try
            {
                var UserId = Base_UserBusiness.GetCurrentUser().UserId;
                db_user.UpdatePassword(UserId, password);
                return Json("更改成功");
            }
            catch (Exception)
            {

                return Json("更改失败");
            }
          
        }
        /// <summary>
        /// 获取用户绑定的微信号信息
        /// </summary>
        /// <returns></returns>
        public ActionResult UserWeixinInfo()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var currentUser = Base_UserBusiness.GetCurrentUser();

                if (currentUser.WX_Unionid != null) result.Data = "1";
                else result.Data = "0";
                result.Success = true;
                result.ErrorCode = 200;
                result.Msg = "成功";
                   
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Data = null;
                result.ErrorCode = 500;
                result.Msg = "失败";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 禁用该账号并修改密码
        /// </summary>
        /// <param name="id">用户代理主键</param>
        /// <returns></returns>
        public ActionResult ChangeState(string id)
        {
            AjaxResult a =  db_user.Change(id,true);
                         
            return Json(a,JsonRequestBehavior.AllowGet);
        }       
                
    }
}