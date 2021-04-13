using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{
    [Table(name: "NotRegisteredForJuniorCollege")]
    public  class NotRegisteredForJuniorCollege
    {

            [Key]
            public int ID { get; set; }
            /// <summary>
            /// 学号
            /// </summary>
            public string StudentNumber { get; set; }
            /// <summary>
            /// 目前学历
            /// </summary>
            public string CurrentEducation { get; set; }
            /// <summary>
            /// 姓名
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 性别
            /// </summary>
            public bool Sex { get; set; }
            /// <summary>
            /// 身份证
            /// </summary>
            public string identitydocument { get; set; }
            /// <summary>
            /// 日期
            /// </summary>
            public DateTime DateTime { get; set; }
            /// <summary>
            /// 原因
            /// </summary>
            public string Reason { get; set; }

            /// <summary>
            /// 备用字段
            /// </summary>
            public string SpareField { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public bool IsDelete { get; set; }


        
    }
}
