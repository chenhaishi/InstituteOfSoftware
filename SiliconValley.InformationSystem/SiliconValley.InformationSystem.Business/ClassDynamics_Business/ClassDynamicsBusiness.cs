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
            return this.GetList().Where(d => d.ExpelsID != null && d.States == 6 && d.IsaDopt == true).Count();
        }
        /// <summary>
        /// 获取所有退学的总人数
        /// </summary>
        /// <returns></returns>
        public object ClassBindTX()
        {
            return this.GetList().Where(d => d.ApplicationRepairID != null && d.States == 1 && d.IsaDopt == true).Count();
        }
        /// <summary>
        /// 获取所有退学的总人数
        /// </summary>
        /// <returns></returns>
        public object ClassBindXX()
        {
            return this.GetList().Where(d => d.SuspensionofschoolID != null && d.States == 5 && d.IsaDopt == true).Count();
        }


    }
}
