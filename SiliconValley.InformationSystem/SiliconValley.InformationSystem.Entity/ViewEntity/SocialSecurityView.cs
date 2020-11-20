using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public  class SocialSecurityView
    {
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
        /// 在职状态
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 缴费基数
        /// </summary>
        public Nullable<decimal> PaymentBase { get; set; }
        /// <summary>
        /// 大病保险
        /// </summary>
        public Nullable<decimal> SeriousIllnessInsurance { get; set; }
        /// <summary>
        /// 当前年月
        /// </summary>
        public Nullable<System.DateTime> CurrentYearAndMonth { get; set; }
        /// <summary>
        /// 补缴月份
        /// </summary>
        public Nullable<int> OverPayMonthNum { get; set; }
        /// <summary>
        /// 单位小计
        /// </summary>
        public Nullable<decimal> UnitTotal { get; set; }
        /// <summary>
        /// 个人小计
        /// </summary>
        public Nullable<decimal> PersonalTotal { get; set; }
        /// <summary>
        /// 养老保险
        /// </summary>
        public Nullable<decimal> EndowmentInsurance { get; set; }
        /// <summary>
        /// 医疗保险
        /// </summary>
        public Nullable<decimal> MedicalInsurance { get; set; }
        /// <summary>
        /// 工伤保险
        /// </summary>
        public Nullable<decimal> WorkInjuryInsurance { get; set; }
        /// <summary>
        /// 生育保险
        /// </summary>
        public Nullable<decimal> MaternityInsurance { get; set; }
        /// <summary>
        /// 失业保险
        /// </summary>
        public Nullable<decimal> UnemploymentInsurance { get; set; }
        /// <summary>
        /// 个人养老保险
        /// </summary>
        public Nullable<decimal> PersonalEndowmentInsurance { get; set; }
        /// <summary>
        /// 个人医疗保险
        /// </summary>
        public Nullable<decimal> PersonalMedicalInsurance { get; set; }
        /// <summary>
        /// 个人失业保险
        /// </summary>
        public Nullable<decimal> PersonalUnemploymentInsurance { get; set; }
        /// <summary>
        /// 合计
        /// </summary>
        public Nullable<decimal> Total { get; set; }
        /// <summary>
        /// 单位补缴部分
        /// </summary>
        public Nullable<decimal> UnitSupplementaryPayment { get; set; }
        /// <summary>
        /// 个人补缴部分
        /// </summary>
        public Nullable<decimal> PersonalSupplementaryPayment { get; set; }
        public int Count { get; set; }
    }
}
