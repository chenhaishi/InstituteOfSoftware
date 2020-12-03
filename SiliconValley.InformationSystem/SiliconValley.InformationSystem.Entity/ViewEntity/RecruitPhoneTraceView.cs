using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public class RecruitPhoneTraceView
    {
        public int Id { get; set; }
        public string Name { get; set; }//姓名
        public string PhoneNumber { get; set; } //电话号码
        public Nullable<System.DateTime> TraceTime { get; set; }//追踪时间
        public string Channel { get; set; }//渠道
        public string ResumeType { get; set; }//简历类型
        public Nullable<bool> PhoneCommunicateResult { get; set; }//联系结果
        public Nullable<bool> IsEntry { get; set; }//是否入职
        public string Remark { get; set; }//备注
        public Nullable<bool> IsDel { get; set; }
        public Nullable<int> Pid { get; set; }//岗位
        public Nullable<int> Deptid { get; set; }//部门
        public string Pname { get; set; }//应聘岗位
        public string Dname { get; set; }//应聘部门
        public Nullable<System.DateTime> ForwardDate { get; set; }//预面试时间
        public Nullable<int> SonId { get; set; }//子级
        public Nullable<System.DateTime> forwardDate { get; set; }//（用于显示的最新）预面试时间
        public Nullable<bool> result { get; set; }//用于显示的最新）联系结果
        public string NewesRemark { get; set; }//用于显示的最新）备注
        public Nullable<bool> NewesIsEntry { get; set; }//用于显示的最新）入职状态

    }
}
