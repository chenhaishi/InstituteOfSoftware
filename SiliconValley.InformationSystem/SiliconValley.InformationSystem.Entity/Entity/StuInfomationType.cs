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
    /// 数据备案信息来源
    /// </summary>
    [Table(name: "StuInfomationType")]
    public partial class StuInfomationType
    {
       
        /// <summary>
        /// 备案数据信息编号
        /// </summary>
    [Key]
        public int Id { get; set; }
        /// <summary>
        /// 备案数据信息来源
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Rmark { get; set; }
        /// <summary>
        /// 是否禁用（false--否，true--禁用）
        /// </summary>
        public Nullable<bool> IsDelete { get; set; }
    
        
    }
}
