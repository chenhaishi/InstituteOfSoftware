using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// 升学视图
    /// </summary>
    //[Table("View_StudentAvg")]
   public class View_StudentAvg
    {
        public int grade_Id { get; set; }
        public int countsum { get; set; }
    }
}
