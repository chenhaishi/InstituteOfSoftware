using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
    /// <summary>
    /// 答题卡信息类
    /// </summary>
    public class AnswerSheetInfosView
    {
        /// <summary>
        /// 答题人
        /// </summary>
        public StudentInformation AnswerPerson { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public Classroom Classroom { get; set; }


        /// <summary>
        /// 考试时间
        /// </summary>
        public DateTime BeginDate { get; set; }


        /// <summary>
        /// 考试时限 单位 h
        /// </summary>
        public decimal TimeLimit { get; set; }


        /// <summary>
        /// 笔试题总分
        /// </summary>
        public float WrittenQuestionScores { get; set; }

        /// <summary>
        /// 机试题总分
        /// </summary>
        public float ComputerQuestionScores { get; set; }


    }
}
