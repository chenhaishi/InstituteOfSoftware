using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.ExaminationSystemBusiness
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
    using SiliconValley.InformationSystem.Util;
    using System.IO;
    public class CandidateInfoBusiness:BaseBusiness<CandidateInfo>
    {
        public List<CandidateInfo> CandidateInfoList()
        {
            return this.GetList();
        }
    }
}
