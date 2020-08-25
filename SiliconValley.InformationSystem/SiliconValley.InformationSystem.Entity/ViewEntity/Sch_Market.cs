using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// 用于获取远程数据库中的备案数据
    /// </summary>
    [Table(name: "Sch_Market")]
    public class Sch_Market
    {
        [Key]
        public int Id { get; set; } 

        /// <summary>
        /// 市场编号
        /// </summary>
        public string MarketId { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }
        /// <summary>
        /// 学生性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public string CreateUserId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 备案日期
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 做修改处理的用户编号
        /// </summary>
        public string ModifyUserId { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 做修改业务的用户名称
        /// </summary>
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 其他说明
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 学生联系方式
        /// </summary>
        public string Phone { get; set; }

        public string QQ { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        public string School { get; set; }

        /// <summary>
        /// 咨询师名称
        /// </summary>
        public string Inquiry { get; set; }

        /// <summary>
        /// 信息来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 所在区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }

        /// <summary>
        /// 跟踪内容
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 备案人
        /// </summary>
        public string SalePerson { get; set; }

        /// <summary>
        /// 关系人
        /// </summary>
        public string RelatedPerson { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public string Age { get; set; }

        /// <summary>
        /// 备案状态
        /// </summary>
        public string MarketState { get; set; }

        /// <summary>
        /// 市场类别
        /// </summary>
        public string MarketType { get; set; }

        /// <summary>
        /// 数据是否作废  ture--作废，false--不作废
        /// </summary>
        public  bool IsDel { get; set; }
    }
}
