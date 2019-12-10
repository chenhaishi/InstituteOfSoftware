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
    [Table(name: "Restudy")]
    public partial class Restudy
    {

        [Key]
        public int ID { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 复学班级
        /// </summary>
        public int ClassID { get; set; }
        /// <summary>
        /// 耽误学业原因
        /// </summary>
        public string Reasonsfordelay { get; set; }
        /// <summary>
        /// 申请原因
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public Nullable<bool> IsDelete { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public Nullable<System.DateTime> Applicationtime { get; set; }
        /// <summary>
        /// 是否需要领书
        /// </summary>
        public bool IsBookcollection { get; set; }

    }
}
