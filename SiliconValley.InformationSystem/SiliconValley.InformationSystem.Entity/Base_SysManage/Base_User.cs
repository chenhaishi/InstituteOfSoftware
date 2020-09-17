using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiliconValley.InformationSystem.Entity.Base_SysManage
{
    /// <summary>
    /// 系统，用户表
    /// </summary>
    [Table("Base_User")]
    public class Base_User
    {

        /// <summary>
        /// 代理主键
        /// </summary>
        [Key, Column(Order = 1)]
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
        /// 员工编号
        /// </summary>
        public string EmpNumber { get; set; }


        /// <summary>
        /// 微信标识
        /// </summary>
        public string WX_Unionid { get; set; }

        /// <summary>
        /// 账号状态  （1--代表正常，0--代表禁用）
        /// </summary>
        public int State { get; set; }
    }
}