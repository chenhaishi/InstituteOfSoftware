using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// 学员缴费记录查询
    /// </summary>
    public class vierprice
    {
        public vierprice()
        {
            this.Chicked = new List<vierprice>();
        }
        public string Date
        {
            get; set;
        }
        public decimal Amountofmoney { get; set; }
        public string CostitemName { get; set; }//名目
        public List<vierprice> Chicked { get; set; }
        public string GrandName { get; set; }
        public int id { get; set; }
        public string Rategory { get; set; }
    }
}
