using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity
{
    /// <summary>
    /// 存放学生押金信息
    /// </summary>
   public class StudentDorMoney
    {
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StuName { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        public string StuNumber { get; set; }

        /// <summary>
        /// 应缴费用
        /// </summary>
        public decimal PayMoney { get; set; }

        /// <summary>
        /// 维修费用
        /// </summary>
        public decimal MantainMoney { get; set; }

        /// <summary>
        /// 应退金额
        /// </summary>
        public decimal SumMoney { get; set; }
    }
}
