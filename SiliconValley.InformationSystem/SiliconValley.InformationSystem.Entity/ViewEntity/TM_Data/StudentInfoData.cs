using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data
{
    /// <summary>
    /// 存放学生的姓名，学号，目前所在班级，阶段，班主任
    /// </summary>
   public class StudentInfoData
    {
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string Stuname { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        public string StudentNumber { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassID { get; set; }

        /// <summary>
        /// 阶段名称
        /// </summary>
        public string GrandName { get; set; }

        /// <summary>
        /// 班级编号
        /// </summary>
        public int ID_ClassName { get; set; }
    }
}
