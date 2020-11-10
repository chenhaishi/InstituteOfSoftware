using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.SocialSecurityDetail_Business
{
   public class SocialSecurityDetailManage: BaseBusiness<SocialSecurityDetail>
    {
        RedisCache rc;
        public List<SocialSecurityDetail> GetSocialSecurityData()
        {
            rc = new RedisCache();
            rc.RemoveCache("InRedisSocialSecurityData");
            List<SocialSecurityDetail>  soclist = new List<SocialSecurityDetail>();
            if (soclist == null || soclist.Count() == 0)
            {
                soclist = this.GetIQueryable().ToList();
                rc.SetCache("InRedisSocialSecurityData", soclist);
            }
            soclist = rc.GetCache<List<SocialSecurityDetail>>("InRedisSocialSecurityData");
            return soclist;
        }
    }
}
