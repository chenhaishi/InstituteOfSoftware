using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data
{
    /// <summary>
    /// 一个迟到记录表
    /// </summary>
    [Table("Laterecord")]
    public class Laterecord
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 班级编号
        /// </summary>
        public int Class_Id { get; set; }
        /// <summary>
        /// 班主任是否到场
        /// </summary>
        public bool IsHavaHeadMaster { get; set; }
        /// <summary>
        /// 任课老师是否到场
        /// </summary>
        public bool IsHavaTeacher { get; set; }
        /// <summary>
        /// PPT是否在讲
        /// </summary>
        public bool IshavaPPT { get; set; }
        /// <summary>
        /// 应到场人数
        /// </summary>
        public int PersonCount { get; set; }
        /// <summary>
        /// 实到场人数
        /// </summary>
        public int PctualCout { get; set; }
        /// <summary>
        /// 其他说明
        /// </summary>
        public string Reak { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Createdate { get; set; }

        /// <summary>
        /// 创建人(员工编号)
        /// </summary>
        public string CreateUser { get; set; }
    }
}
