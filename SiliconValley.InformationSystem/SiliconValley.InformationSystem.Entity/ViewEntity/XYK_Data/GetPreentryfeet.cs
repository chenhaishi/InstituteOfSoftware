using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.XYK_Data
{
   public class GetPreentryfeet
    {
        //学生名字,学生身份证,班级,预录费金额,状态,缴费单号,缴费时间,经办人
        public string Py_Name { get; set; }
        public string identitydocument { get; set; }
        public string sex { get; set; }
        public string ClassID { get; set; }
        public string Amountofmoney { get; set; }
        public string Refundornot { get; set; }
        public string OddNumbers { get; set; }
        public string AddDtae { get; set; }
    }
}
