using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.CollegeDegreeBusinesse;
using SiliconValley.InformationSystem.Business.EnrollmentBusiness;

namespace SiliconValley.InformationSystem.Business.Admin
{
   public class HSFAdminHome_Businesscs
    {
        JuniorCollegeBusinesse Junior = new JuniorCollegeBusinesse();
        EnrollmentBusinesse enrollment = new EnrollmentBusinesse();
        /// <summary>
        /// 本科人数
        /// </summary>
        /// <returns></returns>
        public int NumberOfUndergraduates()
        {
           return enrollment.NumberOfUndergraduates();
        }
        /// <summary>
        /// 大专人数
        /// </summary>
        /// <returns></returns>
        public int NumberOfJuniorCollegeStudents()
        {
            return Junior.NumberOfJuniorCollegeStudents();
        }
    }
}
