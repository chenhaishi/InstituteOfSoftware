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
    [Table(name: "MonthlySalaryRecord")]
    //月度工资表
    public partial class MonthlySalaryRecord
    { 
        [Key]
        public int Id { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<System.DateTime> YearAndMonth { get; set; }//年月份
       
        public Nullable<decimal> OvertimeCharges { get; set; }//加班费用
        public Nullable<decimal> Bonus { get; set; }  //奖金/元
        public Nullable<decimal> LeaveDeductions { get; set; }//（请假）扣款
        //public Nullable<decimal> NoClockWithhold { get; set; }//缺卡
        public Nullable<decimal> OtherDeductions { get; set; }//其他扣款

        public Nullable<decimal> Total { get; set; }//合计
        public Nullable<decimal> PayCardSalary { get; set; }//工资卡工资
        public Nullable<decimal> CashSalary { get; set; }//现金工资
        public Nullable<bool> IsDel { get; set; }
        public Nullable<bool> IsApproval { get; set; }//是否已审批
        public Nullable<decimal> BaseSalary { get; set; }//基本工资 
        public Nullable<decimal> PositionSalary { get; set; }//岗位工资
        public Nullable<decimal> MonthPerformancePay { get; set; }//绩效工资
        public Nullable<decimal> PersonalSocialSecurity { get; set; }//个人社保
        public Nullable<decimal> SocialSecuritySubsidy { get; set; }//社保补贴
        public Nullable<decimal> NetbookSubsidy { get; set; }//笔记本补助
        public Nullable<int> ContributionBase { get; set; }//社保缴费基数
        public Nullable<decimal> PersonalIncomeTax { get; set; }//个税

        public Nullable<decimal> PerformancePay { get; set; }//绩效额度
        public Nullable<decimal> FinalGrade { get; set; }//绩效分

        public Nullable<decimal> TardyAndLeaveWithhold { get; set; }//迟到/早退扣费
        public Nullable<decimal> AbsenteeismWithhold { get; set; }//旷工扣费
        public Nullable<decimal> AbsentNumWithhold { get; set; }//缺卡扣费
        /// <summary>
        /// 财务审核状态 0-未审核 1-已审核 2-已驳回
        /// </summary>
        public int IsFinancialAudit { get; set; }
        /// <summary>
        /// 财务备注
        /// </summary>
        public string FinancialRemarks  {get; set; }
        /// <summary>
        /// 人事备注
        /// </summary>
        public string Remarks { get; set; }

    }
}
