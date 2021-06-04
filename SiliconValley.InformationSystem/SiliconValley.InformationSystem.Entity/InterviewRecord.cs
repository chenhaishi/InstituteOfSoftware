using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity
{
    [Table("InterviewRecord")]
    public partial class InterviewRecord
    {
        public int ID { get; set; }
        /// <summary>
        /// 记录内容
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 时间类型id
        /// </summary>
        public int TimeTypeID { get; set; }
        /// <summary>
        /// 企业id
        /// </summary>
        public string EnterpriseInfoID { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime ShiJian { get; set; }

    }
}
