using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity
{
    [Table("Feedetails")]
  public  class Feedetails
    {
        [Key]
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
    }
}
