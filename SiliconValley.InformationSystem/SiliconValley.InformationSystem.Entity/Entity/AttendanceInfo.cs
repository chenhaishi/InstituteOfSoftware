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
    [Table(name:"AttendanceInfo")]//考勤表
    public partial class AttendanceInfo 
    { 
        [Key] 
        public int AttendanceId { get; set; }
        public string EmployeeId { get; set; }
        public Nullable<System.DateTime> YearAndMonth { get; set; }//年月份
        public Nullable<decimal> DeserveToRegularDays { get; set; }//应到勤天数
        public Nullable<decimal> ToRegularDays { get; set; }//到勤天数
        public Nullable<decimal> LeaveDays { get; set; }//请假天数
        
        public Nullable<int> WorkAbsentNum { get; set; }//上班缺卡次数
        public string WorkAbsentRecord { get; set; }//上班缺卡记录
        public Nullable<int> OffDutyAbsentNum { get; set; }//下班缺卡次数
        public string OffDutyAbsentRecord { get; set; }//下班缺卡记录
        public Nullable<int> TardyNum { get; set; }//迟到次数
        public string TardyRecord { get; set; }//迟到记录
        public Nullable<int> LeaveEarlyNum { get; set; }//早退次数
        public string LeaveEarlyRecord { get; set; }//早退记录
        public Nullable<decimal> TardyAndLeaveWithhold { get; set; }//迟到/早退扣费
        public string Remark { get; set; }//备注
        public Nullable<bool> IsDel { get; set; }
        public Nullable<bool> IsApproval { get; set; }
        public string LeaveRecord { get; set; }//请假记录（事假）
        public Nullable<decimal> OvertimeCharges { get; set; }//加班费用
        public Nullable<decimal> DaysoffDuration { get; set; }//调休时长
        public string DaysoffRecord { get; set; }//调休记录
        public Nullable<decimal> AbsenteeismDays { get; set; }//旷工天数
        public string AbsenteeismRecord { get; set; }//旷工记录
        public Nullable<decimal> AbsenteeismWithhold { get; set; }//旷工扣费
        public Nullable<int> NoonAbsentNum { get; set; }//中午缺卡次数
        public string NoonAbsentRecord { get; set; }//中午缺卡记录
        public Nullable<int> GoOutNum { get; set; }//外出次数
        public string GoOutRecord { get; set; }//外出记录 
        public Nullable<decimal> EvectionNum { get; set; }//出差天数
        public string EvectionRecord { get; set; }//出差记录

        public Nullable<decimal> AbsentNumWithhold { get; set; }//缺卡扣费
        public Nullable<decimal> NonPersonalLeaveNum { get; set; }//非事假天数
        public string NonPersonalLeaveRecord { get; set; }//非事假记录
    }
} 