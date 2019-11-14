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
    [Table(name: "Reconcile")]
    public partial class Reconcile
    {
        [Key]
        public int Id { get; set; }
        public Nullable<int> ClassRoom_Id { get; set; }
        public string Curriculum_Id { get; set; }
        /// <summary>
        /// 课程时间字段
        /// </summary>
        public string Curse_Id { get; set; }
        public string ClassSchedule_Id { get; set; }
        /// <summary>
        /// 系统创建时间
        /// </summary>
        public Nullable<System.DateTime> NewDate { get; set; }
        public string Rmark { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        /// <summary>
        /// 排课时间
        /// </summary>
        public Nullable<System.DateTime> AnPaiDate { get; set; }
        /// <summary>
        /// 老师编号
        /// </summary>
        public string EmployeesInfo_Id { get; set; }

    }
}
