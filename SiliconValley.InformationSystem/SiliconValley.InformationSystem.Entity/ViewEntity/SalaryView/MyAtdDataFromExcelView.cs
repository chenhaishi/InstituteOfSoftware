using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView
{
    public class MyAtdDataFromExcelView
    {     
        /// <summary>
        /// 年月份时间 
        /// </summary>
        public Nullable<System.DateTime> YearAndMonth { get; set; }

        /// <summary>
        /// 应到勤天数
        /// </summary>
        public Nullable<decimal> DeserveToRegularDays { get; set; }

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmpName { get; set; }

        /// <summary>
        /// 员工钉钉号
        /// </summary>
        public int EmpDDid { get; set; }

        /// <summary>
        /// 到勤天数
        /// </summary>
        public Nullable<decimal> ToRegularDays { get; set; }

        /// <summary>
        /// 请假天数
        /// </summary>
        public Nullable<decimal> LeaveDays { get; set; }

        /// <summary>
        /// 上班缺卡次数
        /// </summary>
        public Nullable<int> WorkAbsentNum { get; set; }

        /// <summary>
        /// 上班缺卡记录 
        /// </summary>
        public string WorkAbsentRecord { get; set; }

        /// <summary>
        /// 下班缺卡次数
        /// </summary>
        public Nullable<int> OffDutyAbsentNum { get; set; }

        /// <summary>
        /// 下班缺卡记录
        /// </summary>
        public string OffDutyAbsentRecord { get; set; }

        /// <summary>
        ///    迟到次数
        /// </summary>
        public Nullable<int> TardyNum { get; set; }

        /// <summary>
        /// 迟到记录
        /// </summary>
        public string TardyRecord { get; set; }

        /// <summary>
        /// 早退次数
        /// </summary>
        public Nullable<int> LeaveEarlyNum { get; set; }

        /// <summary>
        /// 早退记录
        /// </summary>
        public string LeaveEarlyRecord { get; set; }

        /// <summary>
        /// 迟到扣费
        /// </summary>
        public Nullable<decimal> TardyWithhold { get; set; }

        /// <summary>
        /// 早退扣费
        /// </summary>
        public Nullable<decimal> LeaveWithhold { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }



        /// <summary>
        /// 请假记录 
        /// </summary>
        public string LeaveRecord { get; set; }

        
        public Nullable<decimal> OvertTimeDuration { get; set; }//加班时长

        public string OvertTimeRecord { get; set; }//加班记录
        //public Nullable<decimal> OvertimeCharges { get; set; }//加班费用

        public Nullable<decimal> DaysoffDuration { get; set; }//调休时长
        public string DaysoffRecord { get; set; }//调休记录
        public Nullable<decimal> AbsenteeismDays { get; set; }//旷工天数
        // public Nullable<decimal> AbsenteeismWithhold { get; set; }//旷工扣费
        public string AbsenteeismRecord { get; set; }//旷工记录
    }
}
