using SiliconValley.InformationSystem.Entity.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.RecruitingDataSummaryBusiness
{
   public  class DeptRecruitingDataSummaryViewManage: BaseBusiness<DeptRecruitingDataSummaryView>
    {
        public List<DeptRecruitingDataSummaryView> Getsqlview()
        {
           var list= this.GetListBySql<DeptRecruitingDataSummaryView>(@"select DeptName,YearAndMonth,SUM(PlanRecruitNum)as'PlanRecruitNum' ,SUM(ResumeSum)as'ResumeSum',SUM(OutboundCallSum)as'OutboundCallSum',
                            SUM(InstantInviteSum)as'InstantInviteSum',SUM(InstantToFacesSum)as'InstantToFacesSum',SUM(InstantRetestSum)as'InstantRetestSum',SUM(InstantRetestPassSum)as'InstantRetestPassSum',
                            SUM(InstantEntryNum)as'InstantEntryNum',SUM(InstantToFacesRate)as'InstantToFacesRate',SUM(InstantInviteRate)as'InstantInviteRate',
                            SUM(InstantRetestPassrate)as'InstantRetestPassrate',SUM(EntryRate)as'EntryRate',SUM(RecruitPercentage)as'RecruitPercentage' from RecruitingDataSummary r
                            inner join Position p on p.Pid=r.Pid
                            inner join Department d on d.DeptId=p.DeptId
                            group by DeptName ,YearAndMonth
                                                ");
            return list;
        }
    }
}
