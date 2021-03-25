using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{

    [Table(name: "AnalysisoftheChoiceQuestion")]
    //选择题解析表
   public partial class AnalysisoftheChoiceQuestion
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 考场id
        /// </summary>
        public int Examid { get; set; }
        /// <summary>
        /// 考生id
        /// </summary>
        public string Studentid { get; set; }
        /// <summary>
        /// 题目id
        /// </summary>
        public int Subject { get; set; }
        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }
    }
}
