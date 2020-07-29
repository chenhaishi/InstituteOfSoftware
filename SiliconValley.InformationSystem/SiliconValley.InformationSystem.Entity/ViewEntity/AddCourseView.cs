using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    public class AddCourseView
    {
        public int ID { get; set; }
        /// <summary>
        /// 加课次数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 加课原因
        /// </summary>
        public string reason { get; set; }
        public EmployeesInfo Teacher { get; set; }

        /// <summary>
        /// 班级编号
        /// </summary>
        public ClassSchedule ClassNumber { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public DateTime ApplyDate { get; set; }
        /// <summary>
        /// 加班开始日期
        /// </summary>
        public DateTime TeachDate { get; set; }

        /// <summary>
        /// 具体日期 (上午 下午 晚自习)
        /// </summary>
        public string SpecDate { get; set; }


        /// <summary>
        /// 课程
        /// </summary>
        public Curriculum Course { get; set; }

        public bool Isdel { get; set; }
    }
}
