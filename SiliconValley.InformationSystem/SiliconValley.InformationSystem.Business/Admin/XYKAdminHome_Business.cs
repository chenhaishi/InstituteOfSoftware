using SiliconValley.InformationSystem.Business.ClassDynamics_Business;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.Admin
{
   public class XYKAdminHome_Business:BaseBusiness<StudentInformation>
    {
        StudentInformationBusiness StudentInformationBusiness = new StudentInformationBusiness();
        ClassDynamicsBusiness ClassDynamicsBusiness = new ClassDynamicsBusiness();
        ClassScheduleBusiness ClassScheduleBusiness = new ClassScheduleBusiness();
        BaseBusiness<Grand> Grand = new BaseBusiness<Grand>();
       
        /// <summary>
        /// 统计在校学生总人数
        /// </summary>
        public object StudentCount()
        {
            return StudentInformationBusiness.StudentList().Count();
        }
        /// <summary>
        /// 统计开除总人数
        /// </summary>
        /// <returns></returns>
        public object StudentExpel()
        {
            return ClassDynamicsBusiness.ClassBindKC();
        }
        /// <summary>
        /// 统计退学总人数
        /// </summary>
        /// <returns></returns>
        public object StudentDropout()
        {
            return ClassDynamicsBusiness.ClassBindTX();
        }
        /// <summary>
        /// 统计休学总人数
        /// </summary>
        /// <returns></returns>
        public object StudentSuspensionof()
        {
            return ClassDynamicsBusiness.ClassBindXX();
        }
        /// <summary>
        /// 统计在校所有班级总数
        /// </summary>
        /// <returns></returns>
        public object StudentClassSum()
        {   
            return ClassScheduleBusiness.ClassList().Count();
        }
        /// <summary>
        /// 获取所有阶段数据（除去Y2）
        /// </summary>
        /// <returns></returns>
        public object StudentGrand()
        {
            return Grand.GetList().Where(d => d.Id != 1006).ToList();
        }
        /// <summary>
        /// 算每个阶段升学率
        /// </summary>
        /// <returns></returns>
        //public object StudentAvgGrand()//创建一个实体类，往里面加数据
        //{
        //    List<Entity.ViewEntity.XYK_Data.AvgGrand> avgGrands = new List<Entity.ViewEntity.XYK_Data.AvgGrand>();
            
        //}


}
}
