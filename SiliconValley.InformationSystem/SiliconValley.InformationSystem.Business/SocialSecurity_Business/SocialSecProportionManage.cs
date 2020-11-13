using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Entity.MyEntity;

namespace SiliconValley.InformationSystem.Business.SocialSecurity_Business
{
   public  class SocialSecProportionManage:BaseBusiness<SocialSecProportion>
    {
        public SocialSecProportion GetbyType(string type)
        {
            return this.GetList().Where(i=>i.PayType==type).FirstOrDefault();
        }
    }
}
