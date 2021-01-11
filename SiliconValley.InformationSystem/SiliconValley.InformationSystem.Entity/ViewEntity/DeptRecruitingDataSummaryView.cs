using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public class DeptRecruitingDataSummaryView
    {
        //public int Id { get; set; }
        public string  DeptName { get; set; }//部门
        public Nullable<int> PlanRecruitNum { get; set; }//计划招聘人数
        public Nullable<int> ResumeSum { get; set; }//简历总数
        public Nullable<int> OutboundCallSum { get; set; }//联系的总数
        public Nullable<int> InstantInviteSum { get; set; }//当月邀约总数
        public Nullable<int> InstantToFacesSum { get; set; }//当月到面总数
        public Nullable<int> InstantRetestSum { get; set; }//当月复试总数
        public Nullable<int> InstantRetestPassSum { get; set; }//当月复试通过总数
        public Nullable<int> InstantEntryNum { get; set; }//当月入职人数
        public Nullable<decimal> InstantToFacesRate { get; set; }//当月到面率
        public Nullable<decimal> InstantInviteRate { get; set; }//当月邀约率
        public Nullable<decimal> InstantRetestPassrate { get; set; }//当月复试通过率
        public Nullable<decimal> EntryRate { get; set; }//入职率
        public Nullable<decimal> RecruitPercentage { get; set; }//招聘完成率
        //public string Remark { get; set; }// 备注
        //public Nullable<bool> IsDel { get; set; }
        public Nullable<System.DateTime> YearAndMonth { get; set; }//年月份
    }
}
