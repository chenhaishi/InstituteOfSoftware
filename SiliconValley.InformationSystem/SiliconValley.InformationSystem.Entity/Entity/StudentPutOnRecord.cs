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

    [Table(name: "StudentPutOnRecord")]
    public partial class StudentPutOnRecord
    {
      
        [Key]
        public int Id { get; set; }
        public string StuName { get; set; }
        public Nullable<bool> StuSex { get; set; }
        public Nullable<System.DateTime> StuBirthy { get; set; }
        public string StuPhone { get; set; }
        public string StuSchoolName { get; set; }
        public string StuEducational { get; set; }
        public string StuAddress { get; set; }
        public string StuWeiXin { get; set; }
        public string StuQQ { get; set; }
        public Nullable<int> StuInfomationType_Id { get; set; }
        public Nullable<int> StuStatus_Id { get; set; }
        public Nullable<bool> StuIsGoto { get; set; }
        public Nullable<System.DateTime> StuVisit { get; set; }
        public string EmployeesInfo_Id { get; set; }
        public Nullable<System.DateTime> StuDateTime { get; set; }
        public string StuEntering { get; set; }
        public string Reak { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<int> Region_id { get; set; }

    }
}
