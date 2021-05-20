using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.TeachingDepBusiness
{
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Business.ClassesBusiness;
    using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
    using SiliconValley.InformationSystem.Business.DormitoryBusiness;
    using SiliconValley.InformationSystem.Entity.Entity;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity;
    using System.Xml;

    public class TeacherClassBusiness : BaseBusiness<ClassTeacher>
    {


        BaseBusiness<StudentInformation> db_student = new BaseBusiness<StudentInformation>();

        BaseBusiness<ClassSchedule> db_class = new BaseBusiness<ClassSchedule>();

        BaseBusiness<ClassMembers> db_stuposi = new BaseBusiness<ClassMembers>();

        BaseBusiness<Members> db_members = new BaseBusiness<Members>();

        SpecialtyBusiness db_major = new SpecialtyBusiness();
        GrandBusiness db_grand = new GrandBusiness();

        public TeacherBusiness db_teacher = new TeacherBusiness();
        BaseBusiness<Curriculum> Currculum_Entity = new BaseBusiness<Curriculum>();

        /// <summary>
        /// 学员所在班级
        /// </summary>
        BaseBusiness<ScheduleForTrainees> db_studentclass = new BaseBusiness<ScheduleForTrainees>();

        public List<ClassTeacher> GetClassTeachers()
        {
            return this.GetList().Where(d => d.IsDel == false).ToList();
        }

        /// <summary>
        /// 获取我的班级
        /// </summary>
        /// <param name="teacherid">老师ID</param>
        /// <returns>班级集合</returns>

        public List<ClassSchedule> GetCrrentMyClass(int teacherid)
        {

            var templist = this.GetClassTeachers().Where(d => d.TeacherID == teacherid).ToList();

            BaseBusiness<ClassSchedule> classdb = new BaseBusiness<ClassSchedule>();

            var classlisttemp = classdb.GetList().Where(d => d.IsDelete == false && d.ClassStatus == false && d.ClassstatusID ==null).ToList();

            List<ClassSchedule> resultlist = new List<ClassSchedule>();

            foreach (var item in templist)
            {

                var obj = classlisttemp.Where(d => d.id == item.ClassNumber).FirstOrDefault();

                if (obj != null)
                {
                    resultlist.Add(obj);
                }

            }

            return resultlist;

        }


        /// <summary>
        /// 获取学员
        /// </summary>
        /// <param name="studentnumber">学员编号</param>
        /// <returns></returns>
        public StudentInformation GetStudentByNumber(string studentnumber)
        {

            var student = db_student.GetList().Where(d => d.IsDelete == false && d.StudentNumber == studentnumber).FirstOrDefault();

            return student;
        }

        /// <summary>
        /// 获取所有班级对象
        /// </summary>
        /// <returns></returns>
        public List<ClassSchedule> AllClassSchedule()
        {
            return db_class.GetIQueryable().ToList().Where(d => d.IsDelete == false).ToList();
        }
        public StudentDetailView GetStudetentDetailView(StudentInformation student)
        {
            StudentDetailView detailView = new StudentDetailView();
            detailView.BirthDate = student.BirthDate;
            detailView.Education = student.Education;
            detailView.Familyaddress = student.Familyaddress;
            detailView.Name = student.Name;
            detailView.Nation = student.Nation;

            //获取图片路径

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/studentConfig.xml"));

            var xmlRoot = xmlDocument.DocumentElement;

            var Avatar = (XmlElement)xmlRoot.GetElementsByTagName("Avatar")[0];

            //头像路径 
            var avatarUrl = Avatar.Attributes["url"].Value;
            if (student.Picture == null || student.Picture == "")
            {
                //默认头像
                var defaultImg = Avatar.GetElementsByTagName("default")[0];
                detailView.Avatar = avatarUrl + defaultImg.Attributes["img"].Value;
            }
            else
            {
                detailView.Avatar = avatarUrl + student.Picture;
            }
            detailView.qq = student.qq;
            detailView.Sex = (bool)student.Sex ? "男" : "女";
            detailView.State = student.State;
            detailView.StudentNumber = student.StudentNumber;
            detailView.Telephone = student.Telephone;
            detailView.WeChat = student.WeChat;
            detailView.IdCard = student.identitydocument;

            //获取这个学员的当前班级

            var temp = db_studentclass.GetList().Where(d => d.CurrentClass == true && d.StudentID == student.StudentNumber).FirstOrDefault();
            ClassSchedule myclass = db_class.GetList().Where(d => d.IsDelete == false && d.ClassNumber == temp.ClassID).FirstOrDefault();
            var classmenber = db_stuposi.GetList().Where(d => d.IsDelete == false && d.ClassNumber == myclass.id && d.Studentnumber == student.StudentNumber).FirstOrDefault();


            if (classmenber != null)
            {
                var posti = db_members.GetList().Where(d => d.IsDelete == false && d.ID == classmenber.Typeofposition).FirstOrDefault();
                detailView.PositionName = posti.Nameofmembers;

            }
            detailView.ClassName = myclass.ClassNumber;
            var grand = db_grand.GetGrandByID((int)myclass.grade_Id);

            if (grand != null)
            {
                detailView.GrandName = grand.GrandName;
            }
            else
            {
                detailView.GrandName = "";
            }


            if (myclass.Major_Id != null)
            {
                var marjor = db_major.GetSpecialtyByID((int)myclass.Major_Id);
                if (marjor != null)
                {

                    detailView.MajorName = marjor.SpecialtyName;
                }
                else
                {
                    detailView.MajorName = "";
                }
            }

            return detailView;
        }

        /// <summary>
        /// 获取班级的表格类型数据
        /// </summary>
        /// <returns></returns>
        public ClassTableView GetClassTableView(ClassSchedule classSchedule)
        {
            ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
            ClassTableView classTableView = new ClassTableView();
            BaseBusiness<HeadClass> headerclass = new BaseBusiness<HeadClass>();
            BaseBusiness<Headmaster> headermaster = new BaseBusiness<Headmaster>();
            BaseBusiness<EmployeesInfo> emp = new BaseBusiness<EmployeesInfo>();

            BaseBusiness<GroupManagement> classgroup = new BaseBusiness<GroupManagement>();

            classTableView.classid = classSchedule.id;
            classTableView.ClassNumber = classSchedule.ClassNumber;
            classTableView.ClassRemarks = classSchedule.ClassRemarks;
            classTableView.ClassStatus = classSchedule.ClassStatus;
            classTableView.GradeName = db_grand.GetGrandByID((int)classSchedule.grade_Id).GrandName;
            classTableView.IsDelete = classSchedule.IsDelete;

            if (classSchedule.Major_Id != null)
            {
                classTableView.MajorName = db_major.GetSpecialtyByID((int)classSchedule.Major_Id).SpecialtyName;
            }
            else
            {
                classTableView.MajorName = "";

            }
            classTableView.ClassSize = this.GetStudentByClass(classSchedule.id).Count;//班级人数
            //学员班级     
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();

            try
            {

                var master = headerclass.GetList().Where(d => d.IsDelete == false && d.ClassID == classScheduleBusiness.GetEntity(classSchedule.id).id).FirstOrDefault();

                var temp1 = headermaster.GetList().Where(d => d.IsDelete == false && d.ID == master.LeaderID).FirstOrDefault();//获取到班主任

                classTableView.Headmaster = emp.GetList().Where(d => d.IsDel == false && d.EmployeeId == temp1.informatiees_Id).FirstOrDefault().EmpName;

            }
            catch (Exception ex)
            {

                classTableView.Headmaster = "暂无";
            }
            try
            {
                classTableView.qqGroup = classgroup.GetList().Where(d => d.IsDelete == false && d.ClassNumber == classSchedule.id).FirstOrDefault().QQGroupnumber;
            }
            catch (Exception ex)
            {

                classTableView.qqGroup = "暂无";
            }



            return classTableView;

        }


        /// <summary>
        /// 获取班级
        /// </summary>
        /// <param name="classnumber">班级id</param>
        /// <returns></returns>
        public ClassSchedule GetClassByClassNumber(string classnumber)
        {
            return db_class.GetList().Where(d => d.IsDelete == false && d.id == int.Parse(classnumber)).FirstOrDefault();

        }


        /// <summary>
        /// 获取班级班干部
        /// </summary>
        /// <param name="classnumber"></param>
        /// <returns></returns>
        public Dictionary<string, StudentInformation> GetClassCadres(int classnumber)
        {

            Dictionary<string, StudentInformation> result = new Dictionary<string, StudentInformation>();

            //获取班干部名称
            BaseBusiness<Members> db_members = new BaseBusiness<Members>();
            var memberlist = db_members.GetList().Where(d => d.IsDelete == false).ToList();


            var temp = db_stuposi.GetList().Where(d => d.IsDelete == false && d.ClassNumber == classnumber).ToList();

            foreach (var item in memberlist)
            {
                var obj = temp.Where(d => d.Typeofposition == item.ID).FirstOrDefault();

                if (obj != null)
                {
                    //判断学员的班级是否是当前的班级
                    var classss = GetScheduleByStudent(obj.Studentnumber);

                    if (classss.id == classnumber)
                    {
                        if (obj != null)
                        {
                            result[item.Nameofmembers] = db_student.GetList().Where(d => d.IsDelete == false && d.StudentNumber == obj.Studentnumber).FirstOrDefault();


                        }
                        else
                        {
                            result[item.Nameofmembers] = null;
                        }
                    }
                }


            }

            return result;



        }


        /// <summary>
        /// 获取学员的技术老师
        /// </summary>
        /// <param name="studentnumber">学员编号</param>
        /// <returns></returns>
        public Teacher GetTeacherByStudent(string studentnumber)
        {
            //获取学员班级

            var dd = db_studentclass.GetList().Where(d => d.StudentID == studentnumber && d.CurrentClass == true).FirstOrDefault();

            TeacherBusiness db = new TeacherBusiness();

            return db.GetTeachers(IsNeedDimission: true).Where(x => x.TeacherID == this.GetList().Where(d => d.ClassNumber == dd.ID_ClassName && d.IsDel == false).FirstOrDefault().TeacherID).FirstOrDefault();



        }
        /// <summary>
        /// 获取学员的技术老师
        /// </summary>
        /// <param name="studentnumber">学员编号</param>
        /// <returns></returns>
        public EmployeesInfo GetTeacherByStudent1(string studentnumber)
        {

            ProStudentInformationBusiness proStudentInformationBusiness = new ProStudentInformationBusiness();
            EmployeesInfo t = new EmployeesInfo();
            var query = proStudentInformationBusiness.GetEntity(studentnumber);
            if (query.State == 2)
            {
                t.EmpName = "";
                return t;
            }
            else
            {
                //获取学员班级

                var dd = db_studentclass.GetList().Where(d => d.StudentID == studentnumber && d.CurrentClass == true).FirstOrDefault();


                if (dd == null)
                {
                    t.EmpName = "";
                    return t;
                }
                else
                {
                    TeacherBusiness db = new TeacherBusiness();


                    var teachclass = this.GetList().Where(d => d.ClassNumber == dd.ID_ClassName && d.IsDel == false).FirstOrDefault();

                    if (teachclass == null)
                    {
                        t.EmpName = "";
                        return t;
                    }

                    ///获取老师对性能
                    var teacher = db.GetTeachers(IsNeedDimission: true).Where(d => d.TeacherID == teachclass.TeacherID).FirstOrDefault();

                    return db_teacher.GetEmpByEmpNo(teacher.EmployeeId);
                }

            }



        }


        /// <summary>
        /// 获取班级教学老师
        /// </summary>
        /// <returns></returns>
        public EmployeesInfo ClassTeacher(string classNumber)
        {
            var tempobj = this.GetList().Where(d => d.ClassNumber == int.Parse(classNumber)).FirstOrDefault();
            if (tempobj != null)
            {
                var tempobj1 = db_teacher.GetTeachers(IsNeedDimission: true).Where(d => d.TeacherID == tempobj.TeacherID).FirstOrDefault();
                if (tempobj1 != null)
                {
                    BaseBusiness<EmployeesInfo> empmanage = new BaseBusiness<EmployeesInfo>();

                    return empmanage.GetList().Where(d => d.EmployeeId == tempobj1.EmployeeId).FirstOrDefault();
                }
                else
                {
                    return new EmployeesInfo();
                }
            }
            else
            {
                return new EmployeesInfo();
            }

        }

        /// <summary>
        /// 获取全部学员
        /// </summary>
        /// <returns></returns>
        public List<StudentInformation> AllStudent()
        {
            return db_student.GetIQueryable().ToList();
        }

        public List<ScheduleForTrainees> AllScheduleForTrainees()
        {
            BaseBusiness<ScheduleForTrainees> sdb = new BaseBusiness<ScheduleForTrainees>();
            return sdb.GetIQueryable().ToList();
        }


        /// <summary>
        /// 获取班级学员
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public List<StudentInformation> GetStudentByClass(int classId)
        {
            List<StudentInformation> result = new List<StudentInformation>();

            var templist = this.AllScheduleForTrainees().Where(d => d.ID_ClassName == classId && d.CurrentClass == true).ToList();

            foreach (var item in templist)
            {
                var student = this.GetStudentByNumber(item.StudentID);

                if (student != null)
                    result.Add(student);
            }

            return result;
        }

        /// <summary>
        /// 获取学员班级
        /// </summary>
        /// <returns></returns>
        public ClassSchedule GetScheduleByStudent(string studentnumber)
        {
            var tempobj = db_studentclass.GetIQueryable().Where(d => d.CurrentClass == true && d.StudentID == studentnumber).FirstOrDefault();

            return this.AllClassSchedule().Where(d => d.id == tempobj.ID_ClassName).FirstOrDefault();
        }

        /// <summary>
        /// 获取班级专业
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        public Specialty GetClass_Major(int classid)
        {
            var classschu = this.GetClassByClassNumber(classid.ToString());

            return db_major.GetSpecialtyByID(classschu.Major_Id);

        }



        /// <summary>
        /// 获取教员带班记录
        /// </summary>
        /// <returns></returns>
        public List<ClassTeacher> TeacherArrangementRecord(int teacherid)
        {
            return this.GetIQueryable().Where(d => d.TeacherID == teacherid).ToList();

        }

        public List<ClassSchedule> GrandClassByUser(Base_UserModel user)
        {
            //获取账号所有的角色

            var userRoles = user.RoleIdList;


            List<ClassSchedule> emplist = new List<ClassSchedule>();

            //循环获取每个角色的权限

            foreach (var role in userRoles)
            {
                // 权限id 权限名称 ,可查看的部门 


                //var permissions = PermissionManage.GetRolePermissionModules(role);  //获取角色所拥有的的权限
                BaseBusiness<OtherRoleMapPermissionValue> db_permissrole = new BaseBusiness<OtherRoleMapPermissionValue>();

                var permissions = db_permissrole.GetIQueryable().Where(d => d.RoleId == role).ToList();

                foreach (var permission in permissions)
                {
                    //根据权限到 配置文件中去匹配
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/empmanageConfig.xml"));

                    var xmlRoot = xmlDocument.DocumentElement;

                    var permissionConfig = (XmlElement)xmlRoot.GetElementsByTagName("grandteacherPermissions")[0];

                    //获取配置文件中的权限
                    XmlNodeList permissNmaes = permissionConfig.ChildNodes;

                    foreach (XmlElement item in permissNmaes)
                    {
                        if (item.Attributes["permissionid"].Value == permission.PermissionValue)
                        {
                            //获取部门
                            var grandStr = item.Attributes["grand"].Value.Split(',');
                            List<string> deplist = grandStr.ToList();
                            if (grandStr[grandStr.Length - 1] == "")
                            {
                                deplist.RemoveAt(grandStr.Length - 1);
                            }
                            //阶段教员

                            foreach (var depItem in deplist)
                            {
                                emplist.AddRange(GetClassScheduleByGrand(int.Parse(depItem)));
                            }
                        }
                    }
                }

            }

            List<ClassSchedule> resultlist = new List<ClassSchedule>();

            foreach (var item in emplist)
            {
                if (!IsContains(resultlist, item))
                {
                    resultlist.Add(item);
                }
            }
            return resultlist;

        }

        public List<ClassSchedule> GetClassScheduleByGrand(int grandid)
        {
            return db_class.GetIQueryable().Where(d => d.grade_Id == grandid && d.IsDelete == false).ToList();
        }

        public bool IsContains(List<ClassSchedule> scours, ClassSchedule classSchedule)
        {
            foreach (var item in scours)
            {
                if (item.id == classSchedule.id)
                {
                    return true;
                }
            }

            return false;
        }


        public List<ClassTeacher> TeacherArrangementRecord(string classid)
        {
            return this.GetIQueryable().ToList().Where(d => d.ClassNumber == int.Parse(classid)).ToList();
        }
        /// <summary>
        ///  获取班级正在上的课程
        /// </summary>
        /// <param name="class_id">班级编号</param>
        /// <returns></returns>
        public Curriculum GetClassOnCurr(int class_id)
        {
            ClassTeacher find = this.GetClassTeachers().Where(t => t.ClassNumber == class_id).FirstOrDefault();
            if (find != null)
            {
                return Currculum_Entity.GetEntity(find.Skill);
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取教员每个月的上课班的记录 插取排课记录
        /// </summary>
        /// <returns></returns>
        public List<ClassTeacher> TeacherClassCount(int teacherId, DateTime date)
        {
           var teacherclass = this.GetIQueryable().Where(d=>d.TeacherID == teacherId && d.BeginDate.Year == date.Year && d.BeginDate.Month == date.Month).ToList();

            var resultlist = new List<ClassTeacher>();

            foreach (var item in teacherclass)
            {
                if (!IsContains(item.ClassNumber, resultlist))
                {
                    resultlist.Add(item);
                }
            }

            return resultlist;

            #region 去掉重复班级
            bool IsContains(int classid, List<ClassTeacher> list)
            {
                foreach (var item in list)
                {
                    if (item.ClassNumber == classid)
                    {
                        return true;
                    }
                }

                return false;
            }
            #endregion

        }

    }
}
