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
    /// 存放学生维修的信息
    /// </summary>
   [Table("DormitoryDeposit")]
   public class DormitoryDeposit
    {
        [Key]
       public string ID { get; set; }
        /// <summary>
        ///  维修日期
        /// </summary>
        public DateTime Maintain { get; set; }
        /// <summary>
        ///  寝室号
        /// </summary>
        public int DormId { get; set; }

        public int ChuangNumber { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        public string StuNumber { get; set; }

        /// <summary>
        ///  维修的物品
        /// </summary>
        public int MaintainGood { get; set; }

        /// <summary>
        ///  应该出的维修费用
        /// </summary>
        public decimal GoodPrice { get; set; }
        /// <summary>
        ///  状态(1--未支付，2--已付清)
        /// </summary>
        public int MaintainState { get; set; }
        /// <summary>
        /// 创建的日期
        /// </summary>
       public DateTime CreaDate { get; set; }
        /// <summary>
        /// 录入人员
        /// </summary>
        public string EntryPersonnel { get; set; }
        /// <summary>
        ///  结算人员
        /// </summary>
        public string SettlementStaff { get; set; }

 
    }
}
