using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public  class MeritsCheckView
    {
        public int Id { get; set; }
        //员工编号
        public string EmployeeId { get; set; }
        //年月份
        public Nullable<System.DateTime> YearAndMonth { get; set; }
        public string RoutineWork { get; set; }//日常工作内容
        public Nullable<decimal> RoutineWorkPropotion { get; set; }//日常工作权重占比
        public Nullable<decimal> RoutineWorkFillRate { get; set; }//日常工作完成率
        public string OtherWork { get; set; }//其他或领导临时指派任务
        public Nullable<decimal> OtherWorkPropotion { get; set; }//其他工作权重占比
        public Nullable<decimal> OtherWorkFillRate { get; set; }//其他工作完成率
        public Nullable<decimal> SelfReportedScore { get; set; }//自评得分
        public Nullable<decimal> SuperiorGrade { get; set; }//上级评分
        public Nullable<decimal> FinalGrade { get; set; }//最终绩效分
        public string Remark { get; set; }//备注
        public Nullable<bool> IsDel { get; set; }
        public Nullable<bool> IsApproval { get; set; }
        public Nullable<bool> IsManager { get; set; }//是否为管理者（即评分人，true为评分人；评分人才能有进入绩效考核页面的权限）
        public string Superior { get; set; }//上级（即给该员工评分的领导）
    }
}
