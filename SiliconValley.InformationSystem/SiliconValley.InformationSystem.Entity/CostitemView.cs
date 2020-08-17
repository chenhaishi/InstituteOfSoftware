using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity
{
    /// <summary>
    /// 财务学员费用入账视图
    /// </summary>
    [Table("CostitemView")]
  public  class CostitemView
    {
        [Key]
        public int Paymentver { get; set; }
        public string StudentNumber { get; set; }
        public DateTime AddDate { get; set; }
        public string identitydocument { get; set; }
        public string Name { get; set; }
        public decimal Amountofmoney { get; set; }


    }
}
