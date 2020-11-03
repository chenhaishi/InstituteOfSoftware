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
        /// 员工编号
        /// </summary>
        public string Emp_ID { get; set; }

        /// <summary>
        /// 员工名称
        /// </summary>
        public string Emp_Name { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal ?summoney { get; set; }

        /// <summary>
        /// 总课时
        /// </summary>
        public int? totalClass { get; set; }

        /// <summary>
        /// 教课数量
        /// </summary>
        public int ClassCount { get; set; }
    }
}
