﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{

    [Table(name: "InternalTrainingCost")]
    /// <summary>
    /// 内训表
    /// </summary>
    public partial class InternalTrainingCost
    {
        /// <summary>
        /// 内训id
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 阶段
        /// </summary>
        public int grandId { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsUsing { get; set; }
        /// <summary>
        /// 内训标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内训内容
        /// </summary>
        public string Contents { get; set; }
        /// <summary>
        /// 内训时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 内训部门id
        /// </summary>
        public int Department { get; set; }
        /// <summary>
        /// 培训人id
        /// </summary>
        public string Trainer { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 课时次数
        /// </summary>
        public int ClassHours { get; set; }
        /// <summary>
        /// 课程id
        /// </summary>
        public string Curriculum { get; set; }
    }
}
