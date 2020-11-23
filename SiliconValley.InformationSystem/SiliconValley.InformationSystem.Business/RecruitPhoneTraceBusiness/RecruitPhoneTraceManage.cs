using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
namespace SiliconValley.InformationSystem.Business.RecruitPhoneTraceBusiness
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Util;

    public  class RecruitPhoneTraceManage:BaseBusiness<RecruitPhoneTrace>
    {
        RedisCache rc;
        public List<RecruitPhoneTrace> GetRPTData() {
            rc = new RedisCache();
            rc.RemoveCache("RedisRPTData");
            List<RecruitPhoneTrace> rptlist = new List<RecruitPhoneTrace>();
            if(rptlist==null || rptlist.Count()==0) {
                rptlist = this.GetIQueryable().ToList();
                rc.SetCache("RedisRPTData", rptlist);

            }
            rptlist = rc.GetCache<List<RecruitPhoneTrace>>("RedisRPTData");
            return rptlist;
        }

        public List<RecruitPhoneTrace> GetRptFromSql() {
            List<RecruitPhoneTrace> rptlist = this.GetListBySql<RecruitPhoneTrace>("select * from RecruitPhoneTrace where IsDel='false'");
            return rptlist;
        }

        public RecruitPhoneTraceView GetRptView(int id) { 
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            RecruitPhoneTraceView rptview = new RecruitPhoneTraceView();
            var item = this.GetEntity(id);
            var dept = empmanage.GetDeptByPid((int)item.Pid);
            rptview.Id = item.Id;
            rptview.Name = item.Name;
            rptview.PhoneNumber = item.PhoneNumber;
            rptview.TraceTime = item.TraceTime;
            rptview.Channel = item.Channel;
            rptview.ResumeType = item.ResumeType;
            rptview.PhoneCommunicateResult = item.PhoneCommunicateResult;
            rptview.IsEntry = item.IsEntry;
            rptview.Remark = item.Remark;
            rptview.IsDel = item.IsDel;
            rptview.Pid = item.Pid;
            rptview.Deptid = dept.DeptId;
            rptview.Pname = empmanage.GetPobjById((int)item.Pid).PositionName;
            rptview.Dname = dept.DeptName;
            rptview.ForwardDate = item.ForwardDate;
            return rptview;
        }

        /// <summary>
        /// 获取某条追踪数据的面试记录数据集合
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<RecruitPhoneTraceView> GetRptViewList(int id) {
            List<RecruitPhoneTraceView> rptviewlist = new List<RecruitPhoneTraceView>();
            var rpt = this.GetEntity(id);
            var rptdata = this.GetList().Where(r => r.SonId == rpt.SonId && r.IsDel == true).ToList();
            foreach (var item in rptdata)
            {
                var rptlist = GetRptView(item.Id);
                rptviewlist.Add(rptlist);
            }
            return rptviewlist;
        }

        /// <summary>
        /// 获取最新的预面试时间
        /// </summary>
        /// <param name="sonid"></param>
        /// <returns></returns>
        public DateTime? GetNewestForwardDate(int sonid) {
       
            var rptlist = this.GetListBySql<RecruitPhoneTrace>("select * from RecruitPhoneTrace where SonId="+sonid);
            DateTime? time = rptlist.LastOrDefault().ForwardDate;
            return time;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sonid"></param>
        /// <returns></returns>
        public RecruitPhoneTrace GetNewest(int sonid)
        {
            var rptlist = this.GetListBySql<RecruitPhoneTrace>("select * from RecruitPhoneTrace where SonId=" + sonid).Last();
            return rptlist;
        }
        public AjaxResult UpdNewestForwardDate(int sonid, string forwarddate)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                var list = this.GetList().Where(i => i.SonId == sonid);
                int id = list.LastOrDefault().Id;
                var recruit = this.GetEntity(id);
                recruit.ForwardDate = Convert.ToDateTime(forwarddate);
                this.Update(recruit);   
               result= this.Success();
            }
            catch (Exception e)
            {
                result = this.Error(e.Message);
            }

            return result;
           
        }
        /// <summary>
        /// 获取最新面试结果
        /// </summary>
        /// <param name="sonid"></param>
        /// <returns></returns>
        public bool GetPhoneCommunicateResult(int sonid)
        {
            var rptlist = this.GetListBySql<RecruitPhoneTrace>("select * from RecruitPhoneTrace where SonId="+sonid);
            bool result = (bool)rptlist.LastOrDefault().PhoneCommunicateResult;
            return result;
        }
    }
}
