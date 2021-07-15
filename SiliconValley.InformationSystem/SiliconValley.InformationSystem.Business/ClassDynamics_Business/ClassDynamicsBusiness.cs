using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.ClassDynamics_Business
{
    /// <summary>
    /// 学员异动业务类
    /// </summary>
   public class ClassDynamicsBusiness:BaseBusiness<ClassDynamics>
    {
        /// <summary>
        /// 获取所有开除总人数
        /// </summary>
        /// <returns></returns>
        public object ClassBindKC()
        {
            BaseBusiness<Entity.Entity.Expels> expels = new BaseBusiness<Entity.Entity.Expels>();
            return expels.GetList().Count();
        }
        /// <summary>
        /// 获取所有退学的总人数
        /// </summary>
        /// <returns></returns>
        public object ClassBindTX()
        {
            BaseBusiness<ApplicationDropout> ApplicationDropout = new BaseBusiness<ApplicationDropout>();
            return ApplicationDropout.GetList().Count();
        }
        /// <summary>
        /// 获取所有休学的总人数
        /// </summary>
        /// <returns></returns>
        public object ClassBindXX()
        {
            BaseBusiness<Entity.Entity.Suspensionofschool> Suspensionofschool = new BaseBusiness<Entity.Entity.Suspensionofschool>();
            return Suspensionofschool.GetList().Count();
        }


    }
}
