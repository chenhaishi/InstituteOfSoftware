using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// Staff_Cost_Statistics  课时费统计返回页面集合
    /// </summary>
    public class Staff_CostView
    {
        
        /// <summary>
        /// 员工名称
        /// </summary>
        public string Emp_Name { get; set; }

        /// <summary>
        /// 职务
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 课时费                     
        /// </summary>
        public decimal ?Cost_fee { get; set; }

        /// <summary>
        /// 值班费
        /// </summary>
        public int Duty_fee { get; set; }

        /// <summary>
        /// 监考费
        /// </summary>
        public int Invigilation_fee { get; set; }

        /// <summary>
        /// 阅卷费
        /// </summary>
        public int Marking_fee { get; set; }

        /// <summary>
        /// 超带班
        /// </summary>
        public int Super_class { get; set; }

        /// <summary>
        /// 内训费    S1-S2内训费（55一节）教质部内训费（30一节）
        /// </summary>
        public int Internal_training_fee { get; set; }


        /// <summary>
        /// 研发费
        /// </summary>
        public int RD_fee { get; set; }

        /// <summary>
        /// 总计
        /// </summary>
        public int totalmoney { get; set; }
    }
}
