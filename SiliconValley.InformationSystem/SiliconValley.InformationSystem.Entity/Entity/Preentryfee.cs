using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{
    /// <summary>
    /// 预入费
    /// </summary>
    [Table("Preentryfee")]
    public class Preentryfee
    {
        [Key]
        public int id { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string identitydocument { get; set; }
        /// <summary>
        /// 备案id
        /// </summary>
        public int keeponrecordid { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Amountofmoney { get; set; }
        /// <summary>
        /// 班级(如初中班级，高中班级)
        /// </summary>
        public string ClassID { get; set; }
        /// <summary>
        /// 是否报名 null未报名，true已报名，float已退款
        /// </summary>
        public bool? Refundornot{ get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDit { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public int? FinanceModelid { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string OddNumbers { get; set; }
    }
}
