using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{
    [Table("ManageClassTime")]
    public class ManageClassTime
    {
        [Key]
        public string ID { get; set; }

        /// <summary>
        /// 员工ID
        /// </summary>
        public string Emp_ID { get; set; }
        
        /// <summary>
        /// 底课时
        /// </summary>
        public int ClassTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int ClassTimeState { get; set; }
    }
}
