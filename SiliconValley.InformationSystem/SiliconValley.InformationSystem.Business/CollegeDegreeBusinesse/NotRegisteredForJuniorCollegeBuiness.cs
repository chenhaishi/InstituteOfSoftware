using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.CollegeDegreeBusinesse
{
   public class NotRegisteredForJuniorCollegeBuiness:BaseBusiness<NotRegisteredForJuniorCollege>
    {
        //学员信息
        StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
        //学员班级
        ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
        //班主任表
        HeadmasterBusiness headmasters = new HeadmasterBusiness();

        public AjaxResult InsertNotRegisteredForJuniorCollege(NotRegisteredForJuniorCollege not)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var stu = studentInformationBusiness.GetListBySql<StudentInformation>("select *from StudentInformation where StudentNumber='"+not.StudentNumber+"'").FirstOrDefault();
                not.Sex = stu.Sex;
                not.Name = stu.Name;
                not.identitydocument = stu.identitydocument;
                not.CurrentEducation = stu.Education;
                not.IsDelete = false;
                this.Insert(not);
                result = Success();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
            }

            return result;
        }
        public AjaxResult UpdateNotRegisteredForJuniorCollege(NotRegisteredForJuniorCollege not)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var stu = studentInformationBusiness.GetListBySql<StudentInformation>("select *from StudentInformation where StudentNumber='" + not.StudentNumber + "'").FirstOrDefault();
                not.Sex = stu.Sex;
                not.Name = stu.Name;
                not.identitydocument = stu.identitydocument;
                not.CurrentEducation = stu.Education;
                this.Update(not);
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
            }

            return result;
        }

        public AjaxResult UpdateNotJuniorCollege(string id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                var join=this.GetListBySql<NotRegisteredForJuniorCollege>("select *from NotRegisteredForJuniorCollege where StudentNumber='" + id + "'").FirstOrDefault();
                if (join!=null)
                {
                    join.IsDelete = true;
                    UpdateNotRegisteredForJuniorCollege(join);
                }
                result = Success();
            }
            catch (Exception e)
            {
                result = Error(e.Message);
                throw;
            }
           return  result;
        }

        public object GetNotJuniorCollegeData(int page, int limit, string AppCondition)
        {
            var list = this.GetListBySql<NotRegisteredForJuniorCollege>("select *from NotRegisteredForJuniorCollege where IsDelete=0 ");
            #region 查询
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string name = str[0];
                string className = str[1];
                string studentNumber = str[2];
                string identitydocument = str[3];
                //string state = str[4];
                list = list.Where(e => e.Name.Contains(name)).ToList();
                if (!string.IsNullOrEmpty(className))
                {

                    list = list.Where(e => scheduleForTraineesBusiness.SutdentCLassName(e.StudentNumber).ClassID == className).ToList();
                }
                if (!string.IsNullOrEmpty(studentNumber))
                {
                    list = list.Where(e => e.StudentNumber == studentNumber).ToList();
                }
                if (!string.IsNullOrEmpty(identitydocument))
                {
                    list = list.Where(e => e.identitydocument == identitydocument).ToList();
                }
                //if (!string.IsNullOrEmpty(state))
                //{
                //    list = list.Where(e => e.Sex == sex).ToList();
                //}
            }
            #endregion
            var dataList = list.OrderBy(a => a.StudentNumber).Skip((page - 1) * limit).Take(limit).ToList();
            var dz = from a in dataList
                     select new {
                         a.ID,
                         a.StudentNumber,
                         a.Name,
                         a.identitydocument,
                         a.CurrentEducation,
                         a.Reason,
                         DateTime = a.DateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                Sex = a.Sex == true ? "男" : "女",
                ClassName = scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber) == null ? "暂无" : scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber).ClassID,
                Headmasters = headmasters.Listheadmasters(a.StudentNumber) == null ? "暂无" : headmasters.Listheadmasters(a.StudentNumber).EmpName,
            };

            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = dz
            };
            return data;

        }
    }
}
