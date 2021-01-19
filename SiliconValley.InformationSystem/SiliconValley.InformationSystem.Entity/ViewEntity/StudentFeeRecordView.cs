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
    /// 学员缴费模型类
    /// </summary>
   [Table("StudentFeeRecordView")]
   public class StudentFeeRecordView
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
      public string   Name { get; set; }
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
        /// 班级名称
        /// </summary>
      public string ClassName { get; set; }
        /// <summary>
        /// 名目名称
        /// </summary>
      public string CostitemsName { get; set; }
        /// <summary>
        /// 名目阶段
        /// </summary>
      public string StageName { get; set; }

      public bool IsDelete { get; set; }
    }


    public class StudentClassName
    {
      
        /// <summary>
        /// 名目阶段
        /// </summary>
        public string StageName { get; set; }

    }
}
