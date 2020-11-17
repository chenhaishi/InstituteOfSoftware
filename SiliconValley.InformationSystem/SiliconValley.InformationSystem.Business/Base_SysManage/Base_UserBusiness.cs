using SiliconValley.InformationSystem.Business.Cache;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.Base_SysManage;
using SiliconValley.InformationSystem.Entity.Base_SysManage.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using SiliconValley.InformationSystem.Entity.MyEntity;

namespace SiliconValley.InformationSystem.Business.Base_SysManage
{
    public class Base_UserBusiness : BaseBusiness<Base_User>
    {
        static Base_UserModelCache _cache { get; } = new Base_UserModelCache();

        #region 外部接口

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <param name="condition">查询类型</param>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public List<Base_UserModel> GetDataList(string condition, string keyword, Pagination pagination)
        {
            var where = LinqHelper.True<Base_UserModel>();

            Expression<Func<Base_User, Base_UserModel>> selectExpre = a => new Base_UserModel
            {

            };
            selectExpre = selectExpre.BuildExtendSelectExpre();

            var q = from a in GetIQueryable().AsExpandable()
                    select selectExpre.Invoke(a);

            //模糊查询
            if (!condition.IsNullOrEmpty() && !keyword.IsNullOrEmpty())
                q = q.Where($@"{condition}.Contains(@0)", keyword);

            var list = q.Where(where).GetPagination(pagination).ToList();
            SetProperty(list);

            return list;

            void SetProperty(List<Base_UserModel> users)
            {
                //补充用户角色属性
                List<string> userIds = users.Select(x => x.UserId).ToList();
                var userRoles = (from a in Service.GetIQueryable<Base_UserRoleMap>()
                                 join b in Service.GetIQueryable<Base_SysRole>() on a.RoleId equals b.RoleId
                                 where userIds.Contains(a.UserId)
                                 select new
                                 {
                                     a.UserId,
                                     b.RoleId,
                                     b.RoleName
                                 }).ToList();
                users.ForEach(aUser =>
                {
                    aUser.RoleIdList = userRoles.Where(x => x.UserId == aUser.UserId).Select(x => x.RoleId).ToList();
                    aUser.RoleNameList = userRoles.Where(x => x.UserId == aUser.UserId).Select(x => x.RoleName).ToList();
                });
            }
        }

        /// <summary>
        /// 获取指定的单条数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public Base_User GetTheData(string id)
        {
            return GetEntity(id);
        }

        public void AddData(Base_User newData)
        {
            if (GetIQueryable().Any(x => x.UserName == newData.UserName))
                throw new Exception("该用户名已存在！");

            Insert(newData);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public void UpdateData(Base_User theData)
        {
            if (theData.UserId == "Admin" && Operator.UserId != theData.UserId)
                throw new Exception("禁止更改超级管理员！");

            Update(theData);
            _cache.UpdateCache(theData.UserId);
        }

        public void SetUserRole(string userId, List<string> roleIds)
        {
            Service.Delete_Sql<Base_UserRoleMap>(x => x.UserId == userId);
            var insertList = roleIds.Select(x => new Base_UserRoleMap
            {
                Id = GuidHelper.GenerateKey(),
                UserId = userId,
                RoleId = x
            }).ToList();

            Service.Insert(insertList);
            _cache.UpdateCache(userId);
            PermissionManage.UpdateUserPermissionCache(userId);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="theData">删除的数据</param>
        public void DeleteData(List<string> ids)
        {
            var adminUser = GetTheUser("Admin");
            if (ids.Contains(adminUser.Id))
                throw new Exception("超级管理员是内置账号,禁止删除！");
            var userIds = GetIQueryable().Where(x => ids.Contains(x.UserId)).Select(x => x.UserId).ToList();

            Delete(ids);
            _cache.UpdateCache(userIds);
        }

        /// <summary>
        /// 获取当前操作者信息
        /// </summary>
        /// <returns></returns>
        public static Base_UserModel GetCurrentUser()
        {
            return GetTheUser(Operator.UserId);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public static Base_UserModel GetTheUser(string userId)
        {
            return _cache.GetCache(userId);
        }

        public static List<string> GetUserRoleIds(string userId)
        {
            return GetTheUser(userId).RoleIdList;
        }

        /// <summary>
        /// 更改密码
        /// </summary>
        /// <param name="oldPwd">老密码</param>
        /// <param name="newPwd">新密码</param>
        public AjaxResult ChangePwd(string oldPwd, string newPwd)
        {
            AjaxResult res = new AjaxResult() { Success = true };
            string userId = Operator.UserId;
            oldPwd = oldPwd.ToMD5String();
            newPwd = newPwd.ToMD5String();
            var theUser = GetIQueryable().Where(x => x.UserId == userId && x.Password == oldPwd).FirstOrDefault();
            if (theUser == null)
            {
                res.Success = false;
                res.Msg = "原密码不正确！";
            }
            else
            {
                theUser.Password = newPwd;
                Update(theUser);
            }

            _cache.UpdateCache(userId);

            return res;
        }

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="permissions">权限值</param>
        public void SavePermission(string userId, List<string> permissions)
        {
            Service.Delete_Sql<Base_PermissionUser>(x => x.UserId == userId);
            var roleIdList = Service.GetIQueryable<Base_UserRoleMap>().Where(x => x.UserId == userId).Select(x => x.RoleId).ToList();
            var existsPermissions = Service.GetIQueryable<Base_PermissionRole>()
                .Where(x => roleIdList.Contains(x.RoleId) && permissions.Contains(x.PermissionValue))
                .GroupBy(x => x.PermissionValue)
                .Select(x => x.Key)
                .ToList();
            permissions.RemoveAll(x => existsPermissions.Contains(x));

            List<Base_PermissionUser> insertList = new List<Base_PermissionUser>();
            permissions.ForEach(newPermission =>
            {
                insertList.Add(new Base_PermissionUser
                {
                    Id = Guid.NewGuid().ToSequentialGuid(),
                    UserId = userId,
                    PermissionValue = newPermission
                });
            });

            Service.Insert(insertList);
        }

        #endregion

        #region 私有成员

        #endregion

        #region 数据模型

        #endregion

        public AccountView ConvetToView(Base_User user)
        {
            EmployeesInfoManage dbemp = new EmployeesInfoManage();

            AccountView view = new AccountView();
            view.Birthday = user.Birthday;
            view.Emp = dbemp.GetInfoByEmpID(user.EmpNumber);

            view.Id = user.Id;
            view.Password = user.Password;
            view.RealName = user.RealName;
            view.Sex = user.Sex;
            view.UserId = user.UserId;
            view.UserName = user.UserName;
            view.State = user.State;

            return view;
        }


        /// <summary>
        /// 创建账号
        /// </summary>
        public void createAccount(string userName, string empNumber)
        {

            EmployeesInfoManage dbemp = new EmployeesInfoManage();
            //获取员工
            var emp = dbemp.GetInfoByEmpID(empNumber);

            //获取身份证后6位

            var password = emp.IdCardNum.Substring(12);

            //进行MD5加密
            string password_md5 = Extention.ToMD5String(password);

            //*******************************************************************************************//

            Base_User user = new Base_User();
            user.Birthday = null;
            user.EmpNumber = empNumber;
            user.Password = password_md5;
            user.UserName = userName;
            user.UserId = Guid.NewGuid().ToString();
            user.Id = Guid.NewGuid().ToString();
            user.WX_Unionid = null;
            user.State = 1;//默认非禁用
            this.AddData(user);


        }

        public void ResetPasswd(string UserId)
        {
            var account = this.GetList().Where(d => d.UserId == UserId).FirstOrDefault();

            EmployeesInfoManage dbemp = new EmployeesInfoManage();

            var emp = dbemp.GetInfoByEmpID(account.EmpNumber);

            account.Password = Extention.ToMD5String(emp.IdCardNum.Substring(12));

            this.Update(account);

        }

        public void UpdatePassword(string UserId, string Password)
        {
            var account = this.GetList().Where(d => d.UserId == UserId).FirstOrDefault();

            account.Password = Extention.ToMD5String(Password);

            this.Update(account);
        }

        public bool IsContains(List<string> rolelist, string roleid)
        {
            foreach (var item in rolelist)
            {
                if (item == roleid)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 根据员工编号获取用户对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public Base_User GetUserByEmpid(string empid)
        {
            var user = this.GetList().Where(s => s.EmpNumber == empid).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// 根据用户获取员工对象
        /// </summary>
        /// <returns></returns>
        public EmployeesInfo GetEmpByUser() {
            var user = GetCurrentUser();
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            return empmanage.GetInfoByEmpID(user.EmpNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="IsKey">是否根据账号主键修改</param>
        /// <returns></returns>
        public AjaxResult Change(string Value,bool IsKey)
        {
            AjaxResult a = new AjaxResult() { Msg="操作成功",Success=true};

            Base_User find = null;
            try
            {
                if (IsKey)
                {
                    find = this.GetEntity(Value);
                }
                else
                {
                  
                    find = this.GetList().Where(u => u.EmpNumber == Value).FirstOrDefault();
                }
                find.WX_Unionid = null;
               
                if(find.State == 0)
                {
                    find.State = 1;
                }
                else
                {
                    find.State = 0;
                }
                find.Password = Extention.ToMD5String("tangdan2020");
                this.Update(find);
            }
            catch (Exception)
            {
                a.Msg = "操作失败！";
                a.Success = false;
            }

            return a;
        }
    }

    public class Base_UserModel : Base_User
    {
        public string RoleNames { get => string.Join(",", RoleNameList); }

        public List<string> RoleIdList { get; set; }

        public List<string> RoleNameList { get; set; }

        public EnumType.RoleType RoleType
        {
            get
            {
                int type = 0;

                var values = typeof(EnumType.RoleType).GetEnumValues();
                foreach (var aValue in values)
                {
                    if (RoleNames.Contains(aValue.ToString()))
                        type += (int)aValue;
                }

                return (EnumType.RoleType)type;
            }
        }
    }

   
    

}