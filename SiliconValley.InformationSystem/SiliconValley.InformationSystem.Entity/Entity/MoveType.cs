using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.MyEntity
{
    [Table("MoveType")]
    public class MoveType
    {
        [Key]
        public int ID { get; set; }

        /// <summary>
        /// 异动类型名称
        /// </summary>
        public string MoveTypeName { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }

        public string Remark { get; set; }

        public Nullable<System.DateTime>  MoveTypeDate { get; set; }
    }
}
