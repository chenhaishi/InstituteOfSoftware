using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// 企业视图
    /// </summary>
   public class AddEnterpriseView
    {
        public string EntName { get; set; }

        public string EntAddress { get; set; }
        //规模
        public string EntScale { get; set; }
        //性质
        public string EntNature { get; set; }
  
        //福利
        public string EntWelfare { get; set; }
        //企业备注
        public string Remark { get; set; }
        //联系人
        public string EntContacts { get; set; }
        //是否合作
        public bool close { get; set; }
        //联系方式
        public string EntPhone { get; set; }
    }
}
