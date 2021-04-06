using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView
{
    /// <summary>
    /// 员工工资对象
    /// </summary>
    public class MySalaryObjView
    {
        /// <summary>
        /// 月度工资表
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmployeeId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string empName { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Depart { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 到勤天数
        /// </summary>
        public Nullable<decimal> toRegularDays { get; set; }
        /// <summary>
        /// 基本工资
        /// </summary>
        public Nullable<decimal> baseSalary { get; set; }
        /// <summary>
        /// 岗位工资
        /// </summary>
        public Nullable<decimal> positionSalary { get; set; }
        /// <summary>
        /// 绩效分
        /// </summary>
        public Nullable<decimal> finalGrade { get; set; }
        /// <summary>
        /// 绩效工资
        /// </summary>
        public Nullable<decimal> PerformanceSalary { get; set; }
        /// <summary>
        /// 笔记本补助
        /// </summary>
        public Nullable<decimal> netbookSubsidy { get; set; }
        /// <summary>
        /// 社保补贴
        /// </summary>
        public Nullable<decimal> socialSecuritySubsidy { get; set; }
        /// <summary>
        /// 应发工资1
        /// </summary>
        public Nullable<decimal> SalaryOne { get; set; }
        /// <summary>
        /// 加班费用
        /// </summary>
        public Nullable<decimal> OvertimeCharges { get; set; }
        /// <summary>
        /// 奖金/元
        /// </summary>
        public Nullable<decimal> Bonus { get; set; }
        /// <summary>
        /// 请假天数
        /// </summary>
        public Nullable<decimal> leavedays { get; set; }
        /// <summary>
        /// 请假扣款
        /// </summary>
        public Nullable<decimal> LeaveDeductions { get; set; }
        /// <summary>
        /// 其他扣款/元
        /// </summary>
        public Nullable<decimal> OtherDeductions { get; set; }
        /// <summary>
        /// 应发工资2
        /// </summary>
        public Nullable<decimal> SalaryTwo { get; set; }
        /// <summary>
        /// 个人社保
        /// </summary>
        public Nullable<decimal> PersonalSocialSecurity { get; set; }
        /// <summary>
        /// 个税
        /// </summary>
        public Nullable<decimal> PersonalIncomeTax { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        public Nullable<decimal> Total { get; set; }
        /// <summary>
        /// 工资卡工资
        /// </summary>
        public Nullable<decimal> PayCardSalary { get; set; }

        /// <summary>
        /// 现金工资
        /// </summary>
        public Nullable<decimal> CashSalary { get; set; }

        /// <summary>
        /// 迟到/早退扣款
        /// </summary>
        public Nullable<decimal> TardyAndLeaveWithhold { get; set; }

        /// <summary>
        /// (早退)请假扣款
        /// </summary>
      //  public Nullable<decimal> LeaveWithhold { get; set; }
        /// <summary>
        /// 缺卡扣款
        /// </summary>
        public Nullable<decimal> NoClockWithhold { get; set; }

        /// <summary>
        /// 员工在职状态
        /// </summary>
        public Nullable<bool> EmpState { get; set; }


        /// <summary>
        /// 旷工扣款
        /// </summary>
        public Nullable<decimal> AbsentNumWithhold { get; set; }
        /// <summary>
        /// 财务审核状态 0-未审核 1-已审核 2-已驳回
        /// </summary>
        public int IsFinancialAudit { get; set; }
        /// <summary>
        /// 财务备注
        /// </summary>
        public string FinancialRemarks { get; set; }
        /// <summary>
        /// 人事备注
        /// </summary>
        public string Remarks { get; set; }
        public Nullable<bool> SendingStatus { get; set; }
        public Nullable<bool> IsApproval { get; set; }//是否已审批
    }
}
