using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.FinaceBusines;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Entity;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.EnrollmentBusiness
{
    //学员学历本科管理
  public  class EnrollmentBusinesse:BaseBusiness<Enrollment>
    {
        //本科费用名目
        BaseBusiness<DetailsofCharges> DetailsofBusiness = new BaseBusiness<DetailsofCharges>();
        //费用明目
        CostitemsBusiness costitemsBusiness = new CostitemsBusiness();
        //学员信息
        StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
        //学员班级
        ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
        //班主任表
        HeadmasterBusiness headmasters = new HeadmasterBusiness();
        //本科专业
        BaseBusiness<Undergraduatemajor> UbderfgerBunsiness = new BaseBusiness<Undergraduatemajor>();
        //课程类别大
        BaseBusiness<CoursecategoryY> CoursecategoryYBunsiness = new BaseBusiness<CoursecategoryY>();
        //报考学校
        BaseBusiness<Undergraduateschool> UndergraduateschoolBunsiness = new BaseBusiness<Undergraduateschool>();
        //课程类别下级
        BaseBusiness<CoursecategoryX> CoursecategoryXBunsiness = new BaseBusiness<CoursecategoryX>();
        //报考学校
        BaseBusiness<Undergraduateschool> UndergraduateschoolBusiness = new BaseBusiness<Undergraduateschool>();
        //自考本科课程
        BaseBusiness<Undergraduatecourse> UndergraduatecourseBusiness = new BaseBusiness<Undergraduatecourse>();
        //学员报考课程
        BaseBusiness<Registerforexamination> RegisterforexaminationBusiness = new BaseBusiness<Registerforexamination>();
        //本科成绩
        BaseBusiness<Undergraduateachievement> UndergraduateachievementBusines = new BaseBusiness<Undergraduateachievement>();
        /// <summary>
        /// 添加本科学员信息
        /// </summary>
        /// <param name="enrollment">本科数据对象</param>
        /// <param name="Costitemsid">缴费明目</param>
        /// <returns></returns>
        public AjaxResult AddEnro(Enrollment enrollment,int Costitemsid)
        {
            AjaxResult retus = null;
            DetailsofCharges detailsofCharges = new DetailsofCharges();
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                enrollment.IsDelete = false;
                enrollment.Datestration = DateTime.Now;
                var enrid=  this.GetList().Where(a => a.IsDelete == false && a.StudentNumber == enrollment.StudentNumber).FirstOrDefault();
                if (enrid==null)
                {
                    this.Insert(enrollment);
                    var Enrollid = this.GetList().Where(a => a.IsDelete == false && a.StudentNumber == enrollment.StudentNumber).FirstOrDefault().ID;
                    detailsofCharges.Enrollmentid = Enrollid;
                    detailsofCharges.Costitemsid = Costitemsid;
                    DetailsofBusiness.Insert(detailsofCharges);
                    retus.ErrorCode = 200;
                 
                }
                else
                {
                  

                    detailsofCharges.Enrollmentid = enrid.ID;
                    detailsofCharges.Costitemsid = Costitemsid;
                    DetailsofBusiness.Insert(detailsofCharges);
                    retus.ErrorCode = 200;
                  
                }
                BusHelper.WriteSysLog("注册自考本科学员", Entity.Base_SysManage.EnumType.LogType.添加数据);

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
        /// 获取还没有缴费的年期本科
        /// </summary>
        /// <param name="Studentid">学号</param>
        /// <param name="Typeid">明目类型</param>
        /// <returns></returns>
        public List<Costitems> Costlist(string Studentid,int Typeid)
        {
           
           var lsit= this.GetList().Where(a => a.StudentNumber == Studentid&&a.IsDelete==false).ToList();
            List<DetailsofCharges> mylist = new List<DetailsofCharges>();
            foreach (var item in lsit)
            {
                mylist.AddRange(DetailsofBusiness.GetList().Where(x => x.Enrollmentid == item.ID).ToList());
            }
            List<Costitems> costlist = new List<Costitems>();
            foreach (var item in mylist)
            {
                costlist.Add(costitemsBusiness.GetEntity(item.Costitemsid));
            }
           
            var listtitme = costitemsBusiness.costitemslist().Where(a => a.Rategory == Typeid&&a.IsDelete==false).ToList();
            if (costlist.Count>0)
            {
                //for (int i = listtitme.Count-1; i >=0; i--)
                //{
                //    foreach (var item in costlist)
                //    {
                //        if (listtitme.Count>0)
                //        {
                //            if (listtitme[i].Name == item.Name)
                //            {
                //                listtitme.Remove(listtitme[i]);
                //            }
                //        }
                //        else
                //        {
                //            break;
                //        }

                //    }
                //}
                foreach (var item in costlist)
                {
                    if (listtitme.Count>0)
                    {
                        for (int i = listtitme.Count - 1; i >= 0; i--)
                        {
                            if (listtitme.Count > 0)
                            {
                                if (listtitme[i].Name == item.Name)
                                {
                                    listtitme.Remove(listtitme[i]);
                                }
                            }
                        }
                    }
                  
                }
            }
            return listtitme; ;

        }
        /// <summary>
        /// 获取已报名本科学员
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="Name">姓名</param>
        /// <param name="StudentNumber">学号</param>
        /// <param name="identitydocument">身份证号</param>
        /// <returns></returns>
        public object GetDate(int page, int limit, string Name,  string StudentNumber, string identitydocument,string ClassName,string Alreadypassed)
        {
            //班主任带班
            BaseBusiness<HeadClass> Hoadclass = new BaseBusiness<HeadClass>();
            var mylist = this.GetList().Where(a => a.IsDelete == false).ToList();
            List<StudentInformation> list = new List<StudentInformation>();
            foreach (var item in mylist)
            {
                list.Add(studentInformationBusiness.GetEntity(item.StudentNumber)); ;
            }

            if (!string.IsNullOrEmpty(ClassName))
            {
                int classid = int.Parse(ClassName);
                var studentlist = scheduleForTraineesBusiness.ClassStudent(classid);
                List<StudentInformation> Mylist = new List<StudentInformation>();
                foreach (var item in studentlist)
                {
                    var x = list.Where(a => a.StudentNumber == item.StudentNumber).FirstOrDefault();
                    if (x!=null)
                    {
                        Mylist.Add(x);
                    }
                  
                }
                list = Mylist;
            }
            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name)).ToList();
            }
         
            if (!string.IsNullOrEmpty(StudentNumber))
            {
                list = list.Where(a => a.StudentNumber.Contains(StudentNumber)).ToList();
            }
            if (!string.IsNullOrEmpty(identitydocument))
            {
                list = list.Where(a => a.identitydocument.Contains(identitydocument)).ToList();
            }
            var xz = list.Select(a => new EnrollmentStudentView
            {
             StudentNumber=  a.StudentNumber,
              Name=  a.Name,
              identitydocument=    a.identitydocument,
                ClassName = scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber)==null?"暂无":scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber).ClassID,
                Headmasters = headmasters.Listheadmasters(a.StudentNumber)==null?"暂无" : headmasters.Listheadmasters(a.StudentNumber).EmpName,
                School =this.GetList().Where(x=>x.IsDelete==false&&x.StudentNumber==a.StudentNumber).FirstOrDefault().School==null?"请补充完整信息": UndergraduateschoolBusiness.GetEntity( this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().School).SchoolName,
                Registeredbatch = this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().Registeredbatch==null?"请补充完整信息": this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().Registeredbatch,
                PassNumber= this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().PassNumber==null?"请补充完整信息" : this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().PassNumber,
                MajorID= this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().MajorID==null?"请补充完整信息": UbderfgerBunsiness.GetEntity(this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == a.StudentNumber).FirstOrDefault().MajorID).ProfessionalName,
                Alreadypassed=UndergraduateachievementBusines.GetList().Where(c=>c.EnrollID== this.GetList().Where(w=>w.IsDelete==false&&w.StudentNumber==a.StudentNumber).FirstOrDefault().ID).ToList().Count,
                
            }).ToList();
            if (!string.IsNullOrEmpty(Alreadypassed))
            {
                int Alread = int.Parse(Alreadypassed);
                xz = xz.Where(a => a.Alreadypassed == Alread).ToList();
            }
            var dataList = xz.OrderBy(a => a.StudentNumber).Skip((page - 1) * limit).Take(limit).ToList();
            //  var x = dbtext.GetList();
            var data = new
            {
                code = "",
                msg = "",
                count = xz.Count,
                data = dataList
            };
            return data;

        }
        /// <summary>
          /// 获取本科查询成绩基本信息
          /// </summary>
          /// <param name="StudentNumber">学号</param>
          /// <returns></returns>
        public EssentialinformationEnucView FindEssntiali(string StudentNumber)
        {
          var find=  studentInformationBusiness.GetEntity(StudentNumber);
            EssentialinformationEnucView enucView = new EssentialinformationEnucView();
            enucView.MajorID = UbderfgerBunsiness.GetEntity(this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == find.StudentNumber).FirstOrDefault().MajorID).ProfessionalName;
            enucView.Name = find.Name;
            enucView.identitydocument = find.identitydocument;
            enucView.PassNumber = this.GetList().Where(x => x.IsDelete == false && x.StudentNumber == find.StudentNumber).FirstOrDefault().PassNumber;
            return enucView;
           }
        /// <summary>
        /// 添加本科专业数据
        /// </summary>
        /// <param name="undergraduatemajor">专业对象数据</param>
        /// <returns></returns>
        public AjaxResult AddUndergraduatemajor(Undergraduatemajor undergraduatemajor)
        {
            AjaxResult result = null;
            try
            {
                undergraduatemajor.IsDelete = false;
                UbderfgerBunsiness.Insert(undergraduatemajor);
                result = new SuccessResult();
                result.Msg = "添加成功";
                result.Success = true;
                BusHelper.WriteSysLog("添加本科专业数据成功", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误!";
                result.Success = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return result;
        }
        /// <summary>
        /// 验证专业名称是否重复，返回0则未重复
        /// </summary>
        /// <param name="name">专业名称</param>
        /// <returns></returns>
        public int BoolUndergraduatemajor(string name)
        {
           return UbderfgerBunsiness.GetList().Where(a => a.IsDelete == false && a.ProfessionalName == name).ToList().Count();
        }
        /// <summary>
       /// 验证课程类别是否有重复
       /// </summary>
       /// <param name="name">类别名称</param>
       /// <param name="MajorID">专业</param>
       /// <returns></returns>
        public int BoolCoursecategory(string name,int MajorID)
        {
          return CoursecategoryYBunsiness.GetList().Where(a => a.MajorID == MajorID && a.Coursetitle == name).ToList().Count();
        }
        /// <summary>
        /// 添加本科类别大
        /// </summary>
        /// <param name="coursecategoryY">数据对象</param>
        /// <returns></returns>
        public AjaxResult AddCoursecategory(CoursecategoryY coursecategoryY)
        {
            AjaxResult result = null;
            try
            {
                CoursecategoryYBunsiness.Insert(coursecategoryY);
                result = new SuccessResult();
                result.Msg = "添加成功";
                result.Success = true;
                BusHelper.WriteSysLog("添加本科课程类别（大）", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误！";
                     result.Success = false ;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);

            }
            return result;
        
        }
        /// <summary>
        /// 下拉框课程类别加载父级数据
        /// </summary>
        /// <param name="MajorID">专业id</param>
        /// <returns></returns>
        public object SelectCoursecaregoryX(int MajorID)
        {
            return CoursecategoryYBunsiness.GetList().Where(a => a.MajorID == MajorID).Select(a => new { name = a.Coursetitle, a.id });
        }
        /// <summary>
        /// 获取本科课程数据
        /// </summary>
        /// <param name="MajorID">专业id</param>
        /// <returns></returns>
        public List<CoursecategoryY> SelectCoursecaregoryXView(int MajorID)
        {
            return CoursecategoryYBunsiness.GetList().Where(a => a.MajorID == MajorID).ToList();
        }
        /// <summary>
        /// 添加子级名称
        /// </summary>
        /// <param name="coursecategoryX">数据对象</param>
        /// <returns></returns>
        public AjaxResult AddCoursecategoryX(CoursecategoryX coursecategoryX)
        {
            AjaxResult result = null;
            try
            {

                
                CoursecategoryXBunsiness.Insert(coursecategoryX);
                BusHelper.WriteSysLog("添加本科课程类别子级", Entity.Base_SysManage.EnumType.LogType.添加数据);
                result = new SuccessResult();
                result.Msg = "添加成功";
                result.Success = true;
            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "添加失败";
                result.Success = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return result;
        }
        /// <summary>
        /// 验证下级类别是否重复
        /// </summary>
        /// <param name="CoursecategoryYID">上级id</param>
        /// <param name="Coursetitle">名称</param>
        /// <returns></returns>
        public int BoolCoursecategoryX(int CoursecategoryYID, string Coursetitle)
        {
         return   CoursecategoryXBunsiness.GetList().Where(a => a.CoursecategoryYID == CoursecategoryYID && a.Coursetitle == Coursetitle).Count();
        }
        /// <summary>
        /// 添加报考学校
        /// </summary>
        /// <param name="undergraduateschool">数据对象</param>
        /// <returns></returns>
        public AjaxResult AddUndergraduateschool(Undergraduateschool undergraduateschool)
        {
            AjaxResult result = null;
            try
            {
                undergraduateschool.IsDelete = false;
                UndergraduateschoolBunsiness.Insert(undergraduateschool);
                result = new SuccessResult();
                result.Success = true;
                BusHelper.WriteSysLog("添加报考学校", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                result = new SuccessResult();
                result.Success = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return result;
        }
        /// <summary>
        /// 验证报考学校是否有重复
        /// </summary>
        /// <param name="name">学校名称</param>
        /// <returns></returns>
        public int BoolUndergraduateschool(string name){
          return  UndergraduateschoolBunsiness.GetList().Where(a => a.IsDelete == false && a.SchoolName == name).ToList().Count();
        }
        /// <summary>
        /// 根据学号获取学员信息
        /// </summary>
        /// <param name="studentid">学号</param>
        /// <returns></returns>
        public StudentInformation FineStudent(string studentid)
        {
          return  studentInformationBusiness.GetEntity(studentid);
        }
        /// <summary>
        /// 本科信息补充
        /// </summary>
        /// <param name="enrollment">数据对象</param>
        /// <returns></returns>
        public AjaxResult UpdaEnrollment(Enrollment enrollment)
        {
            AjaxResult result = null;
            try
            {
                result = new SuccessResult();
               var x= this.GetList().Where(a => a.StudentNumber == enrollment.StudentNumber && a.IsDelete == false).FirstOrDefault();
                x.MajorID = enrollment.MajorID;
                x.Registeredbatch = enrollment.Registeredbatch;
                x.School = enrollment.School;
                x.PassNumber = enrollment.PassNumber;
                this.Update(x);
                result.Msg = "信息补充成功";
                BusHelper.WriteSysLog("本科学员信息编辑", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {

                result = new ErrorResult();
                result.Msg = "服务器错误！";
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;
        }
        /// <summary>
        /// 下拉框课程类别加载下级数据
        /// </summary>
        /// <param name="MajorID">父级id</param>
        /// <returns></returns>
        public object SelectCoursecaregory(int CoursecategoryYID)
        {
            return CoursecategoryXBunsiness.GetList().Where(a => a.CoursecategoryYID == CoursecategoryYID).Select(a => new { name = a.Coursetitle, a.id });
        }
        /// <summary>
         /// 验证一个时间距离当前时间大于多少
         /// </summary>
         /// <param name="date">时间</param>
         /// <param name="count">天数</param>
         /// <returns></returns>
        private bool check(string date,int count)
        {
            DateTime dt = DateTime.Now;
            DateTime tmp1;
            TimeSpan ts1;
            if (!"".Equals(date))
            {
                tmp1 = DateTime.Parse(date);
                ts1 = dt.Subtract(tmp1);
                if (ts1.Days > count)
                {
                    //this.Alert("不能超过3天！");
                    //message = "不能超过3天！";//这个其实没必要传，反正都是【不能超过3天！】
                    return false;
                }

            }
            else
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 下拉框课程类别加载多选框数据课程名称
        /// </summary>
        /// <param name="MajorID">父级id</param>
        /// <returns></returns>
        public object Checkboxatecourse(int CoursecategoryXid,int EnrollID)
        {

            var list = RegisterforexaminationBusiness.GetList().Where(a => a.EnrollID == EnrollID).ToList();
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (!check(list[i].Dateofapplication.ToString(), 130))
                    {
                        list.Remove(list[i]);
                    }
                }
            }


            var mylist = UndergraduatecourseBusiness.GetList().Where(a => a.CoursecategoryXid == CoursecategoryXid).ToList();
            
            //获取已经过了的成绩
          var achievement =UndergraduateachievementBusines.GetList().Where(a => a.EnrollID == EnrollID&&a.Fraction>=60).ToList();
            for (int i = mylist.Count - 1; i >= 0; i--)
            {
                foreach (var item in achievement)
                {
                    if (mylist.Count > 0)
                    {
                        if (mylist[i].id == item.Subjectid)
                        {
                            mylist.Remove(mylist[i]);
                            break;
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                for (int i = mylist.Count - 1; i >= 0; i--)
                {
                    foreach (var item in list)
                    {
                        if (mylist.Count > 0)
                        {
                            if (mylist[i].id == item.UndergraduatecourseID)
                            {
                                mylist.Remove(mylist[i]);
                                break;
                            }
                        }

                    }
                }
            }
            return  mylist.Select(a => new { name = a.Coursetitle, a.id,a.Cost });
        }
        /// <summary>
        /// 验证本科课程名称是否有重复
        /// </summary>
        /// <param name="CoursecategoryXid">课程类别id下级</param>
        /// <param name="Coursetitle">课程名称</param>
        /// <returns></returns>
        public int BoolUndergraduatecourse(int CoursecategoryXid,string Coursetitle)
        {
         return UndergraduatecourseBusiness.GetList().Where(a => a.CoursecategoryXid == CoursecategoryXid && a.Coursetitle == Coursetitle).Count();
        }
        /// <summary>
        /// 本科课程数据提交
        /// </summary>
        /// <param name="undergraduatecourse">数据对象</param>
        /// <returns></returns>
        public AjaxResult Addcurriculum(Undergraduatecourse undergraduatecourse)
        {
            AjaxResult result = null;
            try
            {
                undergraduatecourse.IsDelete = false;
                UndergraduatecourseBusiness.Insert(undergraduatecourse);
                result = new SuccessResult();
                result.Success = true;
                BusHelper.WriteSysLog("添加本科课程名称", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                result = new ErrorResult();
                result.Success = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return result;
        }
        /// <summary>
        /// 根据学号获取本科基本信息
        /// </summary>
        /// <param name="Studentid">学号</param>
        /// <returns></returns>
        public EnrollmentView FineRegisterforexamination(string Studentid)
        {
          var x=  this.GetList().Where(a => a.StudentNumber == Studentid && a.IsDelete == false).FirstOrDefault();
         var student=  studentInformationBusiness.StudentList().Where(a => a.StudentNumber == Studentid).FirstOrDefault();
            EnrollmentView enrollmentView = new EnrollmentView();
            enrollmentView.Name = student.Name;
            enrollmentView.ClassName=scheduleForTraineesBusiness.SutdentCLassName(student.StudentNumber).ClassID;
            enrollmentView.PassNumber = x.PassNumber;
            enrollmentView.id = x.ID;
            enrollmentView.identitydocument = student.identitydocument;
            enrollmentView.StudentNumber = student.StudentNumber;

            return enrollmentView;
        }
        /// <summary>
        /// 报考课程学员
        /// </summary>
        /// <param name="registerforexamination">对象数据</param>
        /// <param name="UndergraduatecourseIDs">课程id(字符串数组)</param>
        /// <returns></returns>
        public AjaxResult AddRegisterforexamination(Registerforexamination registerforexamination, string UndergraduatecourseIDs)
        {
            AjaxResult result = null;
            try
            {

                string[] UndergraduatecourseID = UndergraduatecourseIDs.Split(',');
                List<Undergraduatecourse> ListUndergraduatecourse = new List<Undergraduatecourse>();
                List<Registerforexamination> Listrengister = new List<Registerforexamination>();
                registerforexamination.Dateofapplication = DateTime.Now;
                foreach (var item in UndergraduatecourseID)
                {
                    if (registerforexamination.Whether==true)
                    {
                        ListUndergraduatecourse.Add(UndergraduatecourseBusiness.GetEntity(int.Parse(item)));
                    }
                   
                    Registerforexamination x = new Registerforexamination();
                    x.Whether = registerforexamination.Whether;
                    x.Examinationperiod = registerforexamination.Examinationperiod;
                    x.UndergraduatecourseID = int.Parse(item);
                    x.Dateofapplication = DateTime.Now;
                    x.EnrollID = registerforexamination.id;
                    x.Remarks = registerforexamination.Remarks;
                    Listrengister.Add(x);
                }
                if (registerforexamination.Whether == true)
                {
                    SessionHelper.Session["Appstudentid"] = this.GetEntity(registerforexamination.id).StudentNumber;
                    SessionHelper.Session["Applicationfeereceipt"] = ListUndergraduatecourse.Select(a => new { Amountofmoney = a.Cost, Remarks = registerforexamination.Remarks, title = "本科报考费", AddDate = registerforexamination.Dateofapplication, Name = a.Coursetitle }).ToList();
                   // SessionHelper.Session["person"] as List<StudentFeeRecord>;
                }
                RegisterforexaminationBusiness.Insert(Listrengister);
                result = new SuccessResult();
                result.Msg = "报考成功";
                BusHelper.WriteSysLog("学员本科报考", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误！";
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return result;
        }
        /// <summary>
        /// 加载学员报考本科年期
        /// </summary>
        /// <param name="id">本科信息id</param>
        /// <returns></returns>
        public List<string> SelectExaminationperiod(string id)
        {
         var EnroId= this.GetList().Where(a => a.IsDelete == false && a.StudentNumber == id).FirstOrDefault().ID;
            var x = RegisterforexaminationBusiness.GetList().Where(a => a.EnrollID == EnroId).Select(a => a.Examinationperiod).ToList();
            return x.Distinct().ToList(); 

        }
        /// <summary>
        /// 加载已经报考的课程
        /// </summary>
        /// <param name="EnrollID">本科信息id</param>
        /// <param name="Examinationperiod">年期</param>
        /// <returns></returns>
        public object ChedeckdUndergraduateachievement(int EnrollID, string Examinationperiod)
        {
           var listchievemen= UndergraduateachievementBusines.GetList().Where(a => a.EnrollID == EnrollID).ToList();
          var x= RegisterforexaminationBusiness.GetList().Where(a => a.EnrollID == EnrollID && a.Examinationperiod == Examinationperiod).ToList();
            //删除已经及格的科目
           
            List<Undergraduatecourse> undergraduatecourses = new List<Undergraduatecourse>();
            foreach (var item in x)
            {
                undergraduatecourses.Add(UndergraduatecourseBusiness.GetEntity(item.UndergraduatecourseID));
            }

            for (int i = undergraduatecourses.Count-1; i >= 0; i--)
            {
               
                 foreach (var item in listchievemen)
                  {
                    if (undergraduatecourses.Count > 0)
                    {
                        if (undergraduatecourses[i].id == item.Subjectid)
                        {
                            undergraduatecourses.Remove(undergraduatecourses[i]);
                            break;
                        }
                    }
                }
            }
            return undergraduatecourses.Select(a=>new { name=a.Coursetitle,a.id}).ToList();
        }
        /// <summary>
        /// 本科成绩录入
        /// </summary>
        /// <param name="undergraduateachievements">集合对象数据</param>
        /// <returns></returns>
        public AjaxResult AddUndergraduateachievemenS(List<Undergraduateachievement> undergraduateachievements)
        {
            AjaxResult result = null;
            try
            {
                UndergraduateachievementBusines.Insert(undergraduateachievements);
                result = new SuccessResult();
                result.Msg = "成绩录入成功";
                BusHelper.WriteSysLog("学员本科成绩录入", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误！";
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return result;
        }
        /// <summary>
        /// 查询已经报考的课程
        /// </summary>
        /// <param name="Studentid">学号</param>
        /// <returns></returns>
        public object GetDateRegisterforexamination(string Studentid, int page, int limit)
        {
           var EnroID= this.GetList().Where(a => a.IsDelete == false && a.StudentNumber == Studentid).FirstOrDefault().ID;
            List<AchievementView> achievementViews = new List<AchievementView>();
            var list = RegisterforexaminationBusiness.GetList().Where(a => a.EnrollID == EnroID).ToList();
            foreach (var item in list)
            {
                AchievementView view = new AchievementView();
                var Undergrdua = UndergraduatecourseBusiness.GetEntity(item.UndergraduatecourseID);
                view.Coursecode = Undergrdua.Coursecode;
                view.Coursetitle = Undergrdua.Coursetitle;
                view.Examinationperiod = item.Examinationperiod;
                achievementViews.Add(view);
            }
            var dataList = achievementViews.OrderBy(a => a.Examinationperiod).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = achievementViews.Count,
                data = dataList
            };
            return data;
        }
        /// <summary>
        /// 查询已经过了的课程
        /// </summary>
        /// <param name="Studentid">学号</param>
        /// <returns></returns>
        public object GetDatechievement(string Studentid,int page,int limit)
        {
            var EnroID = this.GetList().Where(a => a.IsDelete == false && a.StudentNumber == Studentid).FirstOrDefault().ID;
            List<AchievementView> achievementViews = new List<AchievementView>();
            var list = UndergraduateachievementBusines.GetList().Where(a => a.EnrollID == EnroID&&a.Fraction>=60).ToList();
            foreach (var item in list)
            {
                AchievementView view = new AchievementView();
                var Undergrdua = UndergraduatecourseBusiness.GetEntity(item.Subjectid);
                view.Coursecode = Undergrdua.Coursecode;
                view.Coursetitle = Undergrdua.Coursetitle;
                view.Examinationperiod = item.Examinationperiod;
                view.Fraction = item.Fraction;
                achievementViews.Add(view);
            }
            var dataList = achievementViews.OrderBy(a => a.Examinationperiod).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = achievementViews.Count,
                data = dataList
            };
            return data;
         
        }
        /// <summary>
        /// 根据学号获取是否报名本科
        /// </summary>
        /// <param name="Studentid">学号</param>
        /// <returns></returns>
        public int StudentUndergraduatecount(string Studentid)
        {
            return this.GetList().Where(a => a.StudentNumber == Studentid && a.IsDelete == false&&a.PassNumber!=null).Count();
        }
        /// <summary>
        /// 拿到考勤excel表中的第一个单元
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public AjaxResult ImportDataFormExcel(Stream stream, string contentType)
        {
            IWorkbook workbook = null;
            AjaxResult result = new AjaxResult();
            try
            {
                if (contentType == "application/vnd.ms-excel")
                {
                    workbook = new HSSFWorkbook(stream);
                }

                if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    workbook = new XSSFWorkbook(stream);
                }

                ISheet sheet = workbook.GetSheetAt(0);
                result = ExcelImportAtdSql(sheet);
                stream.Close();
                stream.Dispose();
                workbook.Close();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
            }


            return result;
        }

        /// <summary>
        /// 将excel数据类的数据存入到数据库的考勤表中
        /// </summary>
        /// <returns></returns>
        public AjaxResult ExcelImportAtdSql(ISheet sheet)
        {
            var ajaxresult = new AjaxResult();
            //StudentInformationBusiness student = new StudentInformationBusiness();

            int num = 3;
            try
            {
                var under = UbderfgerBunsiness.GetList();
                var schoollist = UndergraduateschoolBunsiness.GetList();
                while (true)
                {
                    num++;
                    var getrow = sheet.GetRow(num);
                    if (getrow == null)
                    {
                        break;
                    }
                    string School = string.IsNullOrEmpty(Convert.ToString(sheet.GetRow(0).GetCell(0))) ? null : sheet.GetRow(0).GetCell(0).ToString();
                    string passNumber = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(1))) ? null : getrow.GetCell(1).ToString();
                    string name = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(2))) ? null : getrow.GetCell(2).ToString();
                    string identitydocument = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(3))) ? null : getrow.GetCell(3).ToString();
                    string professionalName = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(4))) ? null : getrow.GetCell(4).ToString();
                    string Datestration = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(5))) ? null : getrow.GetCell(5).ToString();

                    var student = studentInformationBusiness.GetListBySql<StudentInformation>("select * from StudentInformation where identitydocument='" + identitydocument + "'").FirstOrDefault();
                    if (student == null)
                    {
                        StudentInformation stu = new StudentInformation();
                        stu.StudentNumber = num.ToString();
                        stu.Name = name;
                        stu.identitydocument = identitydocument;
                        studentInformationBusiness.Insert(stu);

                    }
                    var year = Datestration.Substring(0, 4);
                    var month = Datestration.Substring(4, 2);
                    Enrollment e = new Enrollment();
                    e.PassNumber = passNumber;
                    e.StudentNumber = student.StudentNumber;
                    e.Datestration = Convert.ToDateTime(year + "-" + month + "-01");
                    e.MajorID = under.Where(i => i.ProfessionalName == professionalName).FirstOrDefault().id;
                    e.School = schoollist.Where(i => i.SchoolName == School).FirstOrDefault().id;
                    e.IsDelete = false;
                    Insert(e);

                }
            }
            catch (Exception ex)
            {
                ajaxresult.Success = false;
                ajaxresult.ErrorCode = 500;
                ajaxresult.Msg = ex.Message;
                ajaxresult.Data = "0";
            }
            return ajaxresult;
        }
    }
}
