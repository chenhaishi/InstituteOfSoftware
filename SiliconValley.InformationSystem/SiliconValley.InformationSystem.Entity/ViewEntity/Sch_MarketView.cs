using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// Sch_Market数据导出实体
    /// </summary>
    public class Sch_MarketView
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public string Sex { get; set; }
        public string Phone { get; set; }
        public string Education { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string QQ { get; set; }
        public string School { get; set; }
        public string Inquiry { get; set; }
        public string source { get; set; }
        public string Area { get; set; }
        /// <summary>
        /// 备案人
        /// </summary>
        public string  SalePerson { get; set; }
        /// <summary>
        /// 关系人
        /// </summary>
        public string  RelatedPerson { get; set; }
        public string MarketState { get; set; }
        /// <summary>
        /// 咨询信息
        /// </summary>
        public string Info { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        public string Remark { get; set; }

        public bool IsDel { get; set; }
    }
}
