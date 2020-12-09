using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.XYK_Data
{

    //财务入账导出实体类
   public class PriceDC
    {
        //学生学号
        public int studentID { get; set; }
        //学生名字
        public string className { get; set; }
        //学生身份证
        public string identity { get; set; }
        //缴费金额
        public decimal Amountofmoney { get; set; }
        //缴费单号
        public string OddNumbers { get; set; }
        //阶段名字
        public string GrandName { get; set; }
        //收费方式
        public string Paymentmethod { get; set; }

    
    }
}
