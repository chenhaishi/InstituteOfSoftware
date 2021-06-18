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
        /// <summary>
        /// 统计在校学生总人数
        /// </summary>
        public object StudentCount()
        {
            return StudentInformationBusiness.StudentList().Count();
        }


    }
}
