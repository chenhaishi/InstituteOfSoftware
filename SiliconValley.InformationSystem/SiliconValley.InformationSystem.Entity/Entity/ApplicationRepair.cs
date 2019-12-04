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
    /// <summary>
    /// 重修
    /// </summary>
    [Table("ApplicationRepair")]
    public partial class ApplicationRepair
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 重修班级
        /// </summary>
        public int Rehabilit { get; set; }
        /// <summary>
        /// 重修原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 学员学号
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 重修时间
        /// </summary>
        public Nullable<System.DateTime> Repairtime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public Nullable<bool> IsDelete { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public Nullable<System.DateTime> Addtime { get; set; }
    
       
    }
}
