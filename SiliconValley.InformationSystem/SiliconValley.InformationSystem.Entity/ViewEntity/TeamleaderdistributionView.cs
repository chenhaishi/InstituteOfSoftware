using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// 班主任带班基础数据
    /// </summary>
  public  class TeamleaderdistributionView
    {
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 班级编号
        /// </summary>
        public int ClassID { get; set; }
        /// <summary>
        /// 班主任姓名
        /// </summary>
        public string HeadmasterName { get; set; }
        /// <summary>
        /// 班主任照片
        /// </summary>
        public string HeadmasterImages { get; set; }
        /// <summary>
        /// 阶段
        /// </summary>
        public string Stage { get; set; }
        /// <summary>
        /// 专业
        /// </summary>
        public string Major { get; set; }
        /// <summary>
        /// 是否毕业 0=不是 1=不是
        /// </summary>
        public Nullable<bool> ClassStatus { get; set; }
    }
}
