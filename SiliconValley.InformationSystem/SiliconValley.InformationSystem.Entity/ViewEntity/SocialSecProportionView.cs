using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public  class SocialSecProportionView
    {
        /// <summary>
        /// 养老保险
        /// </summary>
        public Nullable<decimal> EndowmentInsurance { get; set; }
        /// <summary>
        /// 医疗保险
        /// </summary>
        public Nullable<decimal> MedicalInsurance { get; set; }
        /// <summary>
        /// 工伤保险
        /// </summary>
        public Nullable<decimal> WorkInjuryInsurance { get; set; }
        /// <summary>
        /// 生育保险
        /// </summary>
        public Nullable<decimal> MaternityInsurance { get; set; }
        /// <summary>
        /// 失业保险
        /// </summary>
        public Nullable<decimal> UnemploymentInsurance { get; set; }
        /// <summary>
        /// 个人养老保险
        /// </summary>
        public Nullable<decimal> PersonalEndowmentInsurance { get; set; }
        /// <summary>
        /// 个人医疗保险
        /// </summary>
        public Nullable<decimal> PersonalMedicalInsurance { get; set; }
        /// <summary>
        /// 个人失业保险
        /// </summary>
        public Nullable<decimal> PersonalUnemploymentInsurance { get; set; }
    }
}
