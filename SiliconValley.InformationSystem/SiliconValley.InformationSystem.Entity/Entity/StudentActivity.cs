using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{
    [Table("StudentActivity")]

    public class StudentActivity
    {
            //学生会文件上传
            [Key]
            public string AID { get; set; }
            public string Title { get; set; }
            public DateTime AddTime { get; set; }
            public string Reamk { get; set; }
            public int IsDelete { get; set; }
            public int AState { get; set; }
            public string ADox { get; set; }
        
    }
}
