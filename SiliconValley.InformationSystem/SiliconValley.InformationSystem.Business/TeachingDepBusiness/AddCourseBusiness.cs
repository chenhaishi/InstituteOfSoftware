using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.TeachingDepBusiness
{

    /// <summary>
    /// 加课业务类
    /// </summary>
    public class AddCourseBusiness:BaseBusiness<AddCourse>
    {
        /// <summary>
        /// 课程业务类实例
        /// </summary>
        private readonly CourseBusiness db_course;

        public AddCourseBusiness()
        {

            db_course = new CourseBusiness();
        }

        public List<AddCourse> addCourses()
        {
            return this.GetList().ToList();
        }

        public AddCourseView ConvertToView(AddCourse addCourse)
        {
            AddCourseView view = new AddCourseView();

            view.ApplyDate = addCourse.ApplyDate;

            ClassScheduleBusiness dbclass = new ClassScheduleBusiness();

            var classobj = dbclass.GetList().Where(d => d.id == addCourse.ClassNumber).FirstOrDefault();

            view.ClassNumber = classobj;
            view.Count = addCourse.Count;

            CourseBusiness dbcourse = new CourseBusiness();
            var course = dbcourse.Curriculas().Where(d => d.CurriculumID == addCourse.Course).FirstOrDefault();

            view.Course = course;
            view.ID = addCourse.ID;
            view.Isdel = addCourse.Isdel;
            view.reason = addCourse.reason;
            view.SpecDate = addCourse.SpecDate;
            view.TeachDate = addCourse.TeachDate;

            TeacherBusiness dbteacher = new TeacherBusiness();
            var teacher = dbteacher.GetTeachers(IsNeedDimission: true).Where(d=> d.TeacherID == addCourse.TeacherID).FirstOrDefault();

            EmployeesInfoManage dbemp = new EmployeesInfoManage();
            var emp = dbemp.GetInfoByEmpID(teacher.EmployeeId);

            view.Teacher = emp;


            return view;

        }

        /// <summary>
        /// 获取某个员工的加课记录
        /// </summary>
        /// <returns></returns>
        public List<AddCourse> TeacherAddCourses(string EmpNo)
        {
            TeacherBusiness dbteacher = new TeacherBusiness();

            var teacher = dbteacher.GetTeachers(IsNeedDimission: true).Where(d=>d.EmployeeId == EmpNo).FirstOrDefault();

            if (teacher == null)
            {
                return new List<AddCourse>();
            }

            return this.addCourses().Where(d => d.TeacherID == teacher.TeacherID).ToList();
        }

    }
}
