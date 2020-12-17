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
    /// 学生单号缴费模型
    /// </summary>
    [Table("StudentFeeRecordListView")]
    public class StudentFeeRecordListView
    {
        /// <summary>
        /// 学号
        /// </summary>
        public string StudenID { get; set; }
        /// <summary>
        /// 缴费时间
        /// </summary>
        public DateTime AddDate { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        public string FinancialstaffName { get; set; }
        /// <summary>
        /// 缴费主键
        /// </summary>
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amountofmoney { get; set; }
        /// <summary>
        /// 名目id
        /// </summary>
        public int Costitemsid { get; set; }

        /// <summary>
        /// 名目名称
        /// </summary>
        public string CostitemsName { get; set; }
        /// <summary>
        /// 名目阶段
        /// </summary>
        public string StageName { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 入账时间
        /// </summary>
        public Nullable<System.DateTime> AddTime { get; set; }
        /// <summary>
        /// 入账状态 1 入账 2作废 3撤销
        /// </summary>
        public string Passornot { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string Paymentmethod { get; set; }
        //单号
        public string OddNumbers { get; set; }
        //身份证
        public string identitydocument { get; set; }


    }
}
