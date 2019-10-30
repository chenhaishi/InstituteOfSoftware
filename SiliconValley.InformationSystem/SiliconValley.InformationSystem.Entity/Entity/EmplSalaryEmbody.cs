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

    [Table(name: "EmplSalaryEmbody")]
    public partial class EmplSalaryEmbody
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<decimal> BaseSalary { get; set; }//基本工资
        public Nullable<decimal> PositionSalary { get; set; }//岗位工资
        public Nullable<decimal> PerformancePay { get; set; }//基本绩效工资
        public Nullable<decimal> PersonalSocialSecurity { get; set; }//个人社保
        public Nullable<decimal> SocialSecuritySubsidy { get; set; }//社保补贴
        public Nullable<decimal> NetbookSubsidy { get; set; }//笔记本补助
        public Nullable<int> ContributionBase { get; set; }//社保缴费基数
        public string Remark { get; set; }//备注
        public Nullable<bool> IsDel { get; set; }
        public Nullable<decimal> PayCardSalarySum { get; set; }//工资卡总应发工资
        public Nullable<decimal> PersonalIncomeTax { get; set; }//个税
    }
}
