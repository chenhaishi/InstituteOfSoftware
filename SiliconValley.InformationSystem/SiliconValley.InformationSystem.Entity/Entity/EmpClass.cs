//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SiliconValley.InformationSystem.Entity.MyEntity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table(name: "EmpClass")]
    public partial class EmpClass
    {
        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 就业专员ID
        /// </summary>
        public int EmpStaffID { get; set; }
        /// <summary>
        /// 班级id
        /// </summary>
        public int ClassId { get; set; }

        public bool IsDel { get; set; }

        public System.DateTime dirDate { get; set; }
        public string Remark { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public System.DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public Nullable<System.DateTime> EndingTime { get; set; }

    }
}
