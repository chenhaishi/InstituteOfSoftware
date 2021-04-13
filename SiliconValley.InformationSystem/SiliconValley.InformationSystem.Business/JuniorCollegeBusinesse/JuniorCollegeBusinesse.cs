using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.JuniorCollegeBusinesse
{
   public  class JuniorCollegeBusinesse:BaseBusiness<JuniorCollege>
    {
        //学员信息
        StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
        //学员班级
        ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
        //报考学校
        BaseBusiness<Undergraduateschool> UndergraduateschoolBusiness = new BaseBusiness<Undergraduateschool>();
        //本科专业
        BaseBusiness<Undergraduatemajor> UbderfgerBunsiness = new BaseBusiness<Undergraduatemajor>();
        //班主任表
        HeadmasterBusiness headmasters = new HeadmasterBusiness();
        NotRegisteredForJuniorCollegeBuiness NotRegisteredForJunior = new NotRegisteredForJuniorCollegeBuiness();
        public object GetData(int page, int limit,string AppCondition)
        {
            var list = this.GetListBySql<JuniorCollege>("select *from JuniorCollege where IsDelete=0 ");
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
            var dz = dataList.Select(a => new JuniorCollegeView
            {
                ID=a.ID,
                StudentNumber = a.StudentNumber,
                ChengkaoStudentNumber = a.ChengkaoStudentNumber,
                Name = a.Name,
                identitydocument = a.identitydocument,
                Sex =a.Sex==true?"男":"女",
                Telephone = a.Telephone,
                Familyphone = a.Familyphone,
                EducationLevel = a.EducationLevel,
                School = this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().SchoolId == null ? "请补充完整信息" : UndergraduateschoolBusiness.GetEntity(this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().SchoolId).SchoolName,
                Major = this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().MajorID == null ? "请补充完整信息" : UbderfgerBunsiness.GetEntity(this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().MajorID).ProfessionalName,
                ClassName = scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber) == null ? "暂无" : scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber).ClassID,
                Headmasters = headmasters.Listheadmasters(a.StudentNumber) == null ? "暂无" : headmasters.Listheadmasters(a.StudentNumber).EmpName,
            }).ToList();

            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = dz
            };
            return data;

        }
        public object GetNotRegisteredForJuniorCollegeData(int page, int limit, string AppCondition)
        {
            var list = studentInformationBusiness.GetListBySql<StudentInformation>("select *from StudentInformation where Education='大专'");
            #region 查询
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string name = str[0];
                string className = str[1];
                string studentNumber = str[2];
                string identitydocument = str[3];
                string state = str[4];
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
            var dz = dataList.Select(a => new JuniorCollegeView
            {
                StudentNumber = a.StudentNumber,
                Name = a.Name,
                identitydocument = a.identitydocument,
                ClassName = scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber) == null ? "暂无" : scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber).ClassID,
                Headmasters = headmasters.Listheadmasters(a.StudentNumber) == null ? "暂无" : headmasters.Listheadmasters(a.StudentNumber).EmpName,
            }).ToList();

            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = dz
            };
            return data;

        }
        public AjaxResult JuniorCollegeEdit(JuniorCollege junior)
        {
            AjaxResult result = null;
            try
            {
                result = new SuccessResult();
                var x = this.GetList().Where(a => a.StudentNumber == junior.StudentNumber && a.IsDelete == false).FirstOrDefault();
                x.MajorID = junior.MajorID;
                x.ChengkaoStudentNumber = junior.ChengkaoStudentNumber;
                x.SchoolId = junior.SchoolId;
                x.EducationLevel = junior.EducationLevel;
                this.Update(x);
                result.Msg = "信息补充成功";
                BusHelper.WriteSysLog("大专学员信息编辑", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {

                result = new ErrorResult();
                result.Msg = "服务器错误！";
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;
        }

        public AjaxResult Generate()
        {
           var result =new AjaxResult();
            try
            {
                var list = studentInformationBusiness.GetListBySql<StudentInformation>("select *from StudentInformation where Education='大专'");

                list.ForEach(i =>
                {
                    InsertJuniorCollege(i.StudentNumber);
                });
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
            }
            return result;
        }

        public AjaxResult InsertJuniorCollege(string id)
        {
            var result = new AjaxResult();
            try
            {
                var i = studentInformationBusiness.GetListBySql<StudentInformation>("select *from StudentInformation where StudentNumber='" + id + "'").FirstOrDefault();
                if (i != null)
                {
                    JuniorCollege junior = new JuniorCollege();
                    junior.StudentNumber = i.StudentNumber;
                    junior.Sex = i.Sex;
                    junior.Name = i.Name;
                    junior.identitydocument = i.identitydocument;
                    junior.Telephone = i.Telephone;
                    junior.Familyphone = i.Familyphone;
                    junior.IsDelete = false;
                    junior.EducationLevel = i.Education;
                    this.Insert(junior);
                    result = Success();
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
            }
            return result;
        }
        

        public AjaxResult UpdateJuniorCollege(string id,string Education)
        {
            var result = new AjaxResult();
            try
            {
                var jun = this.GetListBySql<JuniorCollege>("select *from JuniorCollege where StudentNumber='" + id + "'").FirstOrDefault();
                var notjun = this.GetListBySql<NotRegisteredForJuniorCollege>("select *from NotRegisteredForJuniorCollege where StudentNumber='" + id + "'").FirstOrDefault();
                if (Education == "大专")
                {
                    if (notjun != null)
                    {
                        notjun.IsDelete = true;
                        NotRegisteredForJunior.Update(notjun);

                    }
                    if (jun != null)
                    {
                        jun.IsDelete = false;
                        this.Update(jun);
                    }
                    else
                    {
                        InsertJuniorCollege(id);
                    }
                }
                else
                {
                    if (jun != null)
                    {
                        jun.IsDelete = true;
                       this.Update(jun);
                    }
                    if (notjun != null)
                    {
                        notjun.IsDelete = false;
                        NotRegisteredForJunior.Update(notjun);

                    }
                }
                result = Success();
            }
            catch (Exception e)
            {
                result = Error(e.Message);
            }
            return result;
        }


    }
}
