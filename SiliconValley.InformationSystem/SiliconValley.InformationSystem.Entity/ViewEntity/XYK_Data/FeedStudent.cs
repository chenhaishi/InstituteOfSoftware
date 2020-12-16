using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.XYK_Data
{
   public class FeedStudent
    {
        public int id { get; set; }
        public string studentid { get; set; }
        public string name { get; set; }
        public string IDnumber { get; set; }
        public decimal Amountofmoney { get; set; }
        public DateTime AddDate { get; set; }
        public string Passornot { get; set; }
        public string Paymentmethod { get; set; }
        //单号
        public string OddNumbers { get; set; }
        //入账时间
        public Nullable<System.DateTime> AddTime { get; set; }
    }
}
