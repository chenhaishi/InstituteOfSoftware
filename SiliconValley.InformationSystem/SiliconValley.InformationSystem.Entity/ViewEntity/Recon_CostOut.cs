using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public class Recon_CostOut
    {
        /// <summary>
        /// 教员姓名
        /// </summary>
        public string  EmpName { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public List<string>  CurriName { get; set; }

        /// <summary>
        /// 课时
        /// </summary>
        public int CostTime { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
    }
}
