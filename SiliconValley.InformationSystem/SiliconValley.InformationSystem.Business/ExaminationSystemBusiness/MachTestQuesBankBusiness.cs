using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.ExaminationSystemBusiness
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
    public  class MachTestQuesBankBusiness:BaseBusiness<MachTestQuesBank>
    {
        public List<MachTestQuesBank> MachTestQuesBankList()
        {
            return this.GetList();
        }
    }
}
