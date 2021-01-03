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
        public string studentID { get; set; }
       
        //学生名字
        public string className { get; set; }
        //学生身份证
        public string identity { get; set; }
        //班级
        //public string class_id { get; set; }
        //缴费金额
        public decimal Amountofmoney { get; set; }
        //缴费名目
        public string CostitemsName { get; set; }
        //缴费单号
        public string OddNumbers { get; set; }
        //阶段名字
        public string GrandName { get; set; }
        //收费方式
        public string Paymentmethod { get; set; }
        //入账时间
        public string AddTime { get; set; }
        //缴费时间
        public string AddDate { get; set; }
        //经办人
        public string FinanceModelName { get; set; }



    }
}
