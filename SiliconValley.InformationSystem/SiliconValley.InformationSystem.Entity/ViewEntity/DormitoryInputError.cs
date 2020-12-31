using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// 维修数据excel导入错误视图
    /// </summary>
    public class DormitoryInputError
    {
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StuName { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        public string HeadMaster { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorInfo { get; set; }
    }
}
