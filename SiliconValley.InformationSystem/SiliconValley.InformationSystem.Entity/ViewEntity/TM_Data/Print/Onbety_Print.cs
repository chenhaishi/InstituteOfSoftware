using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.Print
{
    /// <summary>
    /// 值班Excel导出实体
    /// </summary>
    public class Onbety_Print
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime time { get; set; }
        /// <summary>
        /// 内容(格式:值班老师姓名【班级名称/值班时间段】)
        /// </summary>
        public string Content { get; set; }


    }
}
