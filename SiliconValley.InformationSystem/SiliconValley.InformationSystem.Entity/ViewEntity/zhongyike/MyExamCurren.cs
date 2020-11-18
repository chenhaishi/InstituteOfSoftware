using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.zhongyike
{
    /// <summary>
    /// 存放考试课程数据
    /// </summary>
   public class MyExamCurren
    {
        /// <summary>
        /// 考场编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 考场名字
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        public string CurreName { get; set; }
    }
}
