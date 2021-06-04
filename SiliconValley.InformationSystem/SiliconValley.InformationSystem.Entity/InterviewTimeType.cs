using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity
{
    [Table("InterviewTimeType")]
    public partial class InterviewTimeType
    {
        public int ID { get; set; }
        /// <summary>
        /// 时间类型
        /// </summary>
        public string TimeType { get; set; }
    }
}
