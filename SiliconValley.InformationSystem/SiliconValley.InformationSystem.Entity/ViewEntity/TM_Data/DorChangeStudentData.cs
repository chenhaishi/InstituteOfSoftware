using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data
{
    /// <summary>
    /// 存放调寝学员信息
    /// </summary>
   public class DorChangeStudentData
    {
        /// <summary>
        /// 学号
        /// </summary>
        public string StuNumber { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StuName { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        public string TeacherName { get; set; }

        /// <summary>
        /// 寝室名称
        /// </summary>
        public string DorName { get; set; }

        /// <summary>
        /// 床位号
        /// </summary>
        public int ChuangNumber { get; set; }
    }
}
