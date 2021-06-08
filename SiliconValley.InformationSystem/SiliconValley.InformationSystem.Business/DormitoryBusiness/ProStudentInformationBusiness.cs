using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Employment;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System.Collections.Generic;
using System.Linq;

namespace SiliconValley.InformationSystem.Business.DormitoryBusiness
{
    /// <summary>
    /// 我自己的学生业务类
    /// </summary>
    public class ProStudentInformationBusiness : BaseBusiness<StudentInformation>
    {

        private EmployeesInfoManage dbemp;
        private ProScheduleForTrainees dbproScheduleForTrainees;
        private ProHeadClass dbproHeadClass;
        private ProHeadmaster dbheadmaster;
        private EmploymentStaffBusiness dbemploymentStaff;
        private EmpClassBusiness dbempClass;
        /// <summary>
        /// 获取所有的在校生
        /// </summary>
        /// <returns></returns>
        public List<StudentInformation> GetStudentInSchoolData() {
            //
          return  this.GetListBySql<StudentInformation>("select * from StudentInformation where IsDelete=0 and State is NULL").ToList();
        }

        /// <summary>
        /// 获取所有的在校生
        /// </summary>
        /// <returns></returns>
        public List<StudentInformation> GetStudentInSchoolData1()
        {

            return this.GetListBySql<StudentInformation>("select * from StudentInformation where State =8 or State is null and IsDelete=0").ToList();
           // return this.GetList().Where(a => a.);
        }

        /// <summary>
        /// 根据学生编号获取学生对象
        /// </summary>
        /// <param name="StudentNumber"></param>
        /// <returns></returns>
        public StudentInformation GetStudent(string StudentNumber)
        {
            return this.GetStudentInSchoolData1().Where(a => a.StudentNumber == StudentNumber).FirstOrDefault();
        }

        /// <summary>
        /// 根据学会编号获取班主任
        /// </summary>
        /// <param name="StudentNumber"></param>
        /// <returns></returns>
        public EmployeesInfo GetEmpinfoByStudentNumber(string StudentNumber)
        {
            dbemp = new EmployeesInfoManage();
            dbproScheduleForTrainees = new ProScheduleForTrainees();
            dbproHeadClass = new ProHeadClass();
            dbheadmaster = new ProHeadmaster();
            var obj0 = dbproScheduleForTrainees.GetTraineesByStudentNumber(StudentNumber);
            var obj1 = dbproHeadClass.GetClassByClassid(obj0.ID_ClassName);
            if (obj1!=null)
            {
                if (dbheadmaster.GetEmployeesInfoByHeadID(obj1.LeaderID)==null){
                    EmployeesInfo emppp = new EmployeesInfo();
                    emppp.EmpName = "该班级暂未有班主任";

                    return emppp;
                }
                 
                    return dbheadmaster.GetEmployeesInfoByHeadID(obj1.LeaderID);
                

            }
            else
            {
                EmployeesInfo emppp = new EmployeesInfo();
                emppp.EmpName = "该班级暂未有班主任";
                return emppp;
            }
          
        }
        /// <summary>
        /// 根据学会编号获取就业班主任班主任
        /// </summary>
        /// <param name="StudentNumber"></param>
        /// <returns></returns>
        public EmployeesInfo GetEEmpinfoByStudentNumber(string StudentNumber)
        {
            dbemp = new EmployeesInfoManage();
            dbemploymentStaff = new EmploymentStaffBusiness();
            dbempClass = new EmpClassBusiness();
            dbproScheduleForTrainees = new ProScheduleForTrainees();
             var obj0 = dbproScheduleForTrainees.GetTraineesByStudentNumber(StudentNumber);

            var obj1 = dbempClass.GetStaffByClassid(obj0.ID_ClassName);
            if (obj1 != null)
            {
                return dbemploymentStaff.GetEmpInfoByEmpID(obj1.ID);
            }
            else
            {
                EmployeesInfo emppp = new EmployeesInfo();
                emppp.EmpName = "该班级暂未有班主任";
                return emppp;
            }

        }
    }
}
