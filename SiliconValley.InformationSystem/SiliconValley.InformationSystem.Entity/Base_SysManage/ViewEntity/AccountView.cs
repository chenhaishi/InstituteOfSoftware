using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Base_SysManage.ViewEntity
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
    /// <summary>
    /// 账号模型 视图
    /// </summary>
   public class AccountView
    {
      
        public String Id { get; set; }

        /// <summary>
        /// 用户Id,逻辑主键
        /// </summary>
        public String UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public String UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public String RealName { get; set; }

        /// <summary>
        /// 性别(1为男，0为女)
        /// </summary>
        public Int32? Sex { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 员工
        /// </summary>
        public EmployeesInfo Emp { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }
    }
}
