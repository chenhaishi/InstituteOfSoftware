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
        BaseBusiness<Entity.ViewEntity.View_StudentAvg> view_StudentAvg = new BaseBusiness<Entity.ViewEntity.View_StudentAvg>();
       
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
        public object StudentAvgGrand(DateTime date)//创建一个实体类，往里面加数据
        {
            List<Entity.ViewEntity.XYK_Data.AvgGrand> avgGrands = new List<Entity.ViewEntity.XYK_Data.AvgGrand>();
            var time = DateTime.Now.Year.ToString();
            var avgSql = "";
            if (date == null)
            {
                 avgSql = "select grade_Id,sum(班级人数) as countsum from (select *, (select count(studentId) from ScheduleForTrainees where ClassID = cs.ClassNumber and CurrentClass = 0 and AddDate like '%" + time + "%') as 班级人数 from ClassSchedule cs) as temp where ClassstatusID = 4 group by grade_Id ";//获取当前年份
            }
            else
            {
                avgSql = "select grade_Id,sum(班级人数) as countsum from (select *, (select count(studentId) from ScheduleForTrainees where ClassID = cs.ClassNumber and CurrentClass = 0 and AddDate like '%" + date + "%') as 班级人数 from ClassSchedule cs) as temp where ClassstatusID = 4 group by grade_Id ";//当前时间
            }
             
            var avg = view_StudentAvg.GetListBySql<ScheduleForTrainees>(avgSql).ToList();

            return avg;


        }


    }
}
