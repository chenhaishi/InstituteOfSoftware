using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data
{
    /// <summary>
    /// 存放学生维修数据
    /// </summary>
   public class StuSusheData
    {
        /// <summary>
        /// 维修编号
        /// </summary>
        public string DeaID { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string DeastuName { get; set; }

        /// <summary>
        /// 维修日期
        /// </summary>
        public DateTime DeaMaintain { get; set; }

        /// <summary>
        /// 房间编号
        /// </summary>
        public string DeaDorName { get; set; }

        /// <summary>
        /// 维修金额
        /// </summary>
        public decimal DeaGoodPrice { get; set; }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string DeaNameofarticle { get; set; }

        /// <summary>
        /// 是否支付
        /// </summary>
        public string Isdelete { get; set; }
    }


   public class ListStusheData
    {
        /// <summary>
        /// 学生维修详细
        /// </summary>
       public List<StuSusheData> listdata { get; set; }

        /// <summary>
        /// 所缴寝室押金
        /// </summary>

        public decimal SumMantanMoney { get; set; }

        /// <summary>
        /// 退还寝室押金
        /// </summary>
        public decimal GetTuiMoney { get; set; }
    }
}
