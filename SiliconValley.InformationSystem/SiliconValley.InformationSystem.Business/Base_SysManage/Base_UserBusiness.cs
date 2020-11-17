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

        #region �ⲿ�ӿ�

        /// <summary>
        /// ��ȡ�����б�
        /// </summary>
        /// <param name="condition">��ѯ����</param>
        /// <param name="keyword">�ؼ���</param>
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

            //ģ����ѯ
            if (!condition.IsNullOrEmpty() && !keyword.IsNullOrEmpty())
                q = q.Where($@"{condition}.Contains(@0)", keyword);

            var list = q.Where(where).GetPagination(pagination).ToList();
            SetProperty(list);

            return list;

            void SetProperty(List<Base_UserModel> users)
            {
                //�����û���ɫ����
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
        /// ��ȡָ���ĵ�������
        /// </summary>
        /// <param name="id">����</param>
        /// <returns></returns>
        public Base_User GetTheData(string id)
        {
            return GetEntity(id);
        }

        public void AddData(Base_User newData)
        {
            if (GetIQueryable().Any(x => x.UserName == newData.UserName))
                throw new Exception("���û����Ѵ��ڣ�");

            Insert(newData);
        }

        /// <summary>
        /// ��������
        /// </summary>
        public void UpdateData(Base_User theData)
        {
            if (theData.UserId == "Admin" && Operator.UserId != theData.UserId)
                throw new Exception("��ֹ���ĳ�������Ա��");

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
        /// ɾ������
        /// </summary>
        /// <param name="theData">ɾ��������</param>
        public void DeleteData(List<string> ids)
        {
            var adminUser = GetTheUser("Admin");
            if (ids.Contains(adminUser.Id))
                throw new Exception("��������Ա�������˺�,��ֹɾ����");
            var userIds = GetIQueryable().Where(x => ids.Contains(x.UserId)).Select(x => x.UserId).ToList();

            Delete(ids);
            _cache.UpdateCache(userIds);
        }

        /// <summary>
        /// ��ȡ��ǰ��������Ϣ
        /// </summary>
        /// <returns></returns>
        public static Base_UserModel GetCurrentUser()
        {
            return GetTheUser(Operator.UserId);
        }

        /// <summary>
        /// ��ȡ�û���Ϣ
        /// </summary>
        /// <param name="userId">�û�Id</param>
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
        /// ��������
        /// </summary>
        /// <param name="oldPwd">������</param>
        /// <param name="newPwd">������</param>
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
                res.Msg = "ԭ���벻��ȷ��";
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
        /// ����Ȩ��
        /// </summary>
        /// <param name="userId">�û�Id</param>
        /// <param name="permissions">Ȩ��ֵ</param>
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

        #region ˽�г�Ա

        #endregion

        #region ����ģ��

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
        /// �����˺�
        /// </summary>
        public void createAccount(string userName, string empNumber)
        {

            EmployeesInfoManage dbemp = new EmployeesInfoManage();
            //��ȡԱ��
            var emp = dbemp.GetInfoByEmpID(empNumber);

            //��ȡ���֤��6λ

            var password = emp.IdCardNum.Substring(12);

            //����MD5����
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
            user.State = 1;//Ĭ�Ϸǽ���
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
        /// ����Ա����Ż�ȡ�û�����
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public Base_User GetUserByEmpid(string empid)
        {
            var user = this.GetList().Where(s => s.EmpNumber == empid).FirstOrDefault();
            return user;
        }

        /// <summary>
        /// �����û���ȡԱ������
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
        /// <param name="IsKey">�Ƿ�����˺������޸�</param>
        /// <returns></returns>
        public AjaxResult Change(string Value,bool IsKey)
        {
            AjaxResult a = new AjaxResult() { Msg="�����ɹ�",Success=true};

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
                a.Msg = "����ʧ�ܣ�";
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