
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public class SatisficingConfigDataView
    {
        public int SatisficingConfigId { get; set; }

        public EmployeesInfo Emp { get; set; }
        /// <summary>
        /// 调查时间
        /// </summary>
        public DateTime investigationDate { get; set; }
      
        /// <summary>
        /// 总分
        /// </summary>
        public int TotalScore { get; set; }
       
        /// <summary>
        /// 班级
        /// </summary>
        public ClassSchedule investigationClass { get; set; }
        /// <summary>
        /// 调查的课程
        /// </summary>
        public Curriculum Curriculum { get; set; }

        public double Average { get; set; }


    }
}
