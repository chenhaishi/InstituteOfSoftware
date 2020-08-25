using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{
    /// <summary>
    /// 用于存放作废的备案数据及原因
    /// </summary>
    [Table("Delte_StudentPutinfo")]
    public class Delte_StudentPutinfo
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 备案编号
        /// </summary>
        public int StudentID { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime datetims { get; set; }
        /// <summary>
        /// 操作说明
        /// </summary>
        public string Reamk { get; set; }
    }
}
