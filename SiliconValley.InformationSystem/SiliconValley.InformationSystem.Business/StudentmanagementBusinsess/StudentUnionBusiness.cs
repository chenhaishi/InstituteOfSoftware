using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.StudentmanagementBusinsess
{
    //学生会
  public class StudentUnionBusiness:BaseBusiness<StudentUnionMembers>
    {
    
        //学员信息
        BaseBusiness<StudentInformation> Student = new BaseBusiness<StudentInformation>();
        //班级学员
        BaseBusiness<ScheduleForTrainees> ClassSche = new BaseBusiness<ScheduleForTrainees>();
        //学生会部门
        BaseBusiness<StudentUnionDepartment> UnionDepart = new BaseBusiness<StudentUnionDepartment>();
        //学生会职位
        BaseBusiness<StudentUnionPosition> SUnionPosition = new BaseBusiness<StudentUnionPosition>();
        //班级表
        ClassScheduleBusiness classschedu = new ClassScheduleBusiness();
        //班级学员表 
        ScheduleForTraineesBusiness ClassStudent = new ScheduleForTraineesBusiness();
        //学生会成员撤销表
        BaseBusiness<StudentUnionLeaves> UnLeaves = new BaseBusiness<StudentUnionLeaves>();
        /// <summary>
        /// 获取学生会所有成员
        /// </summary>
        /// <param name="Name">学生会部门名称</param>
        /// <returns></returns>
        public object UnionMembersList(string Name,int page,int limit,string StuName,string qEndTime,string qBeginTime,string quiz1,string sex)
        {
        
          int UnId=  UnionDepart.GetList().Where(a => a.Dateofregistration == false && a.Departmentname == Name).FirstOrDefault().ID;
            var list = this.GetList().Where(a => a.Dateofregistration == false && a.Departuretime == null && a.department == UnId).ToList();
            if (!string.IsNullOrEmpty(quiz1))
            {
                if (quiz1 == "0")
                {
                    list = this.GetList().Where(a => a.Dateofregistration == false && a.department == UnId).ToList();
                }
                else
                if (quiz1 == "2")
                {
                    list = this.GetList().Where(a => a.Dateofregistration == false && a.department == UnId&& a.Departuretime != null).ToList();
                }
            }
            if (!string.IsNullOrEmpty(StuName))
            {
                var x = Student.GetList().Where(a => a.IsDelete == false && a.Name.Contains(StuName)).ToList();
                List<StudentUnionMembers> studentUnions = new List<StudentUnionMembers>();
                foreach (var item in x)
                {
                    var stu=list.Where(a => a.Studentnumber == item.StudentNumber).FirstOrDefault();
                    if (stu!=null)
                    {
                        studentUnions.Add(stu);
                    }
                 
                }
                list = studentUnions;

            }
            if (!string.IsNullOrEmpty(qBeginTime))
            {
                list = list.Where(a => a.Inrtiationtime >= Convert.ToDateTime(qBeginTime)).ToList();
            }
            if (!string.IsNullOrEmpty(qEndTime))
            {
                list = list.Where(a =>Convert.ToDateTime( a.Inrtiationtime.ToString().Split(' ')[0]) <= Convert.ToDateTime(qEndTime)).ToList();
            }
           
            if (!string.IsNullOrEmpty(sex))
            {
                if (sex!="全部")
                {
                    var x = Student.GetList().Where(a => a.IsDelete == false && a.Sex==Convert.ToBoolean(sex)).ToList();
                    List<StudentUnionMembers> studentUnions = new List<StudentUnionMembers>();
                    foreach (var item in x)
                    {
                        var stu = list.Where(a => a.Studentnumber == item.StudentNumber).FirstOrDefault();
                        if (stu!=null)
                        {
                            studentUnions.Add(stu);
                        }
                       
                    }
                    list = studentUnions;
                }
            }
        
         var dataList =  list.Select(
                aa => new
                {
                    aa.ID,
                    aa.Studentnumber,//学号
                    StidentName = Student.GetList().Where(a => a.IsDelete == false && a.State == null && a.StudentNumber == aa.Studentnumber).SingleOrDefault().Name,
                    StudentSex = Student.GetList().Where(a => a.IsDelete == false && a.State == null && a.StudentNumber == aa.Studentnumber).SingleOrDefault().Sex,
                    ClassName = ClassSche.GetList().Where(a => a.CurrentClass == true && a.StudentID == aa.Studentnumber).SingleOrDefault()==null?"暂无": ClassSche.GetList().Where(a => a.CurrentClass == true && a.StudentID == aa.Studentnumber).SingleOrDefault().ClassID,
                    Department= UnionDepart.GetList().Where(a=>a.Dateofregistration==false&&a.ID==aa.department).SingleOrDefault().Departmentname==null?"暂无": UnionDepart.GetList().Where(a => a.Dateofregistration == false && a.ID == aa.department).SingleOrDefault().Departmentname,
                    Position= SUnionPosition.GetList().Where(a=>a.Dateofregistration==false&&a.ID==aa.position).SingleOrDefault().Jobtitle==null?"群众": SUnionPosition.GetList().Where(a => a.Dateofregistration == false && a.ID == aa.position).SingleOrDefault().Jobtitle,
                    aa.Inrtiationtime,
                    aa.Departuretime
                });
            var mydatalist= dataList.OrderByDescending(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = mydatalist.Count,
                data = mydatalist
            };
            return data;
        }
        /// <summary>
        /// 添加或者编辑 当有ID值大于零是就编辑
        /// </summary>
        /// <param name="studentUnionMembers">学生会成员对象</param>
        /// <returns></returns>
        public AjaxResult UnionMembersEntity(StudentUnionMembers studentUnionMembers,string Stuid)
        {
            AjaxResult retus = null;
            
            try
            {
                retus = new SuccessResult();

                retus.Success = true;
                if (studentUnionMembers.ID > 0)
                {
                    retus.Msg = "修改成功";
                    BusHelper.WriteSysLog("编辑数据", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                }
                else
                {
                    Stuid= Stuid.Substring(0, Stuid.Length - 1);
                    string[] Studentid = Stuid.Split(',');
                    List<StudentUnionMembers> list = new List<StudentUnionMembers>();
                    foreach (var item in Studentid)
                    {
                        StudentUnionMembers student = new StudentUnionMembers();
                        student.Inrtiationtime = DateTime.Now;
                        student.Dateofregistration = false;
                        student.department = studentUnionMembers.department;
                        student.Remarks = studentUnionMembers.Remarks;
                        student.position = studentUnionMembers.position;
                        student.Studentnumber = item;
                        list.Add(student);
                    }
                    retus.Msg = "添加成功";
                    
                    this.Insert(list);
                    BusHelper.WriteSysLog("数据添加", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";

                retus.Success = false;
                retus.ErrorCode = 500;
                if (studentUnionMembers.ID > 0)
                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
                else
                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
                throw;
            }
            return retus;


        }
        /// <summary>
        /// 根据班级号查询是否有未进学生会的人员
        /// </summary>
        /// <param name="ClassName">班级号</param>
        /// <returns></returns>
        public List<StudentInformation> StudentList(string ClassName)
        {
         
            //查询班级所有学生
            StudentInformationBusiness student = new StudentInformationBusiness();
            var mylist= this.GetList().Where(a => a.Dateofregistration == false && a.Departuretime == null).ToList();
            List<StudentInformation> list = new List<StudentInformation>();
         
            if (!string.IsNullOrEmpty(ClassName))
            {
                //CurrentClass是否为正常状态 查询使用以参数为班级的学员学号
                var stuid = ClassStudent.GetList().Where(a => a.ClassID == ClassName && a.CurrentClass == true).ToList();
                foreach (var item in stuid)
                {
                    var studentobj = student.GetList().Where(d => d.IsDelete == false && d.StudentNumber == item.StudentID).FirstOrDefault();

                    if (studentobj != null)
                        list.Add(studentobj);
                }
                foreach (var item in mylist)
                {
                    list = list.Where(d => d.StudentNumber != item.Studentnumber).ToList();

                }
            }
            return list;

        }
        /// <summary>
        /// 查询部门名称是否重复。重复则大于0
        /// </summary>
        /// <param name="Name">部门名称</param>
        /// <returns></returns>
        public int SelectDepa(string Name)
        {
           return UnionDepart.GetList().Where(a => a.Dateofregistration == false && a.Departmentname == Name).ToList().Count();
        }
        /// <summary>
        /// 添加学生会部门
        /// </summary>
        /// <param name="studentUnionDepartment">数据对象</param>
        /// <returns></returns>
        public bool AddDepa(StudentUnionDepartment studentUnionDepartment)
        {
            bool str = false;
            try
            {
                str=true;
                UnionDepart.Insert(studentUnionDepartment);
             BusHelper.WriteSysLog("添加学生会部门数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return str;
        }
        /// <summary>
        /// 修改部门状态
        /// </summary>
        /// <param name="Name">部门名称</param>
        /// <returns></returns>
        public bool UodateDepa(string Name)
        {
           var x= UnionDepart.GetList().Where(a => a.Dateofregistration == false && a.Departmentname == Name).FirstOrDefault();
            x.Dateofregistration = true;
            bool str = true;
            try
            {
                UnionDepart.Update(x);
                BusHelper.WriteSysLog("修改学生会部门状态", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
                str = false;
            }
            return str;
        }

        /// <summary>
        ///学生会职位所有数据
        /// </summary>
        /// <returns></returns>
        public List<StudentUnionPosition> UnionPositionList()
        {
          return  SUnionPosition.GetList().Where(a => a.Dateofregistration == false).ToList();
        }

        /// <summary>
        /// 学生会成员撤销
        /// </summary>
        /// <param name="studentUnionLeaves">数据对象</param>
        /// <returns></returns>
        public AjaxResult StudentunionCheng(StudentUnionLeaves studentUnionLeaves)
        {
            AjaxResult retus = null;
            try
            {
                studentUnionLeaves.Datetimes = DateTime.Now;
           
                UnLeaves.Insert(studentUnionLeaves);
                //修改离职时间
                var m = this.GetList().Where(a => a.Dateofregistration == false && a.Departuretime == null && a.ID == studentUnionLeaves.Union_id).FirstOrDefault();
                m.Departuretime = DateTime.Now;
                this.Update(m);
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "操作成功";
                BusHelper.WriteSysLog("数据添加数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
          
        }
        /// <summary>
        /// 学生会详细
        /// </summary>
        /// <param name="Studentnumber">学号</param>
        /// <returns></returns>
        public  object StudentUnionMembersDetailed(string Studentnumber)
        {
            var ClassID = ClassStudent.GetList().Where(c => c.StudentID == Studentnumber && c.CurrentClass == true).First().ClassID;
            var Studentu = Student.GetEntity(Studentnumber);
          

          
            var list = this.GetList().Where(a => a.Dateofregistration == false  && a.Studentnumber == Studentnumber).FirstOrDefault();
            var datalist = new
            {
                StudentNumber = list.Studentnumber,
                classa = classschedu.GetList().Where(q => q.IsDelete == false && q.ClassStatus == false && q.ClassNumber == ClassID).FirstOrDefault().ClassNumber,//班级号
                Name = Studentu.Name,
                Sex = Studentu.Sex,
                Remarks = list.Remarks,
                department = UnionDepart.GetEntity( list.department).Departmentname,
                position = SUnionPosition.GetEntity(list.position).Jobtitle,
                Inrtiationtime = list.Inrtiationtime,
                Departuretime = list.Departuretime
            };
            return datalist;
        }
    }
}
