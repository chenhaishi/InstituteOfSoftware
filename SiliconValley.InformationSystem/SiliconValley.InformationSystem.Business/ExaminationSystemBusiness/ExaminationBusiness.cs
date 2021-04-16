using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Depository.CellPhoneSMS;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;

namespace SiliconValley.InformationSystem.Business.ExaminationSystemBusiness
{
    /// <summary>
    /// 考试信息业务类
    /// </summary>
    public class ExaminationBusiness : BaseBusiness<Examination>
    {

        private readonly BaseBusiness<ExamType> db_examtype;

        private readonly BaseBusiness<ExamTypeName> db_examtypename;

        private readonly QuestionLevelBusiness db_questionlevel;
        /// <summary>
        /// 班主任带班业务实例
        /// </summary>
        private readonly BaseBusiness<HeadClass> db_headclass;

        private readonly BaseBusiness<CandidateInfo> db_candidateinfo;

        private readonly ExaminationRoomBusiness db_examroom;

        private readonly EmployeesInfoManage db_emp;

        private readonly ComputerTestQuestionsBusiness db_computerQuestion;

        private readonly GrandBusiness db_grand;
        /// <summary>
        /// 排课业务实例
        /// </summary>

        private readonly BaseBusiness<Reconcile> db_reconicle;

        private readonly BaseBusiness<ExamRoomDistributed> db_examroomDistributed;

        public ExaminationBusiness()
        {
            db_examtype = new BaseBusiness<ExamType>();
            db_questionlevel = new QuestionLevelBusiness();
            db_headclass = new BaseBusiness<HeadClass>();
            db_candidateinfo = new BaseBusiness<CandidateInfo>();
            db_examroom = new ExaminationRoomBusiness();
            db_examroomDistributed = new BaseBusiness<ExamRoomDistributed>();
            db_emp = new EmployeesInfoManage();

            db_reconicle = new BaseBusiness<Reconcile>();
            db_computerQuestion = new ComputerTestQuestionsBusiness();
            db_grand = new GrandBusiness();
            db_examtypename = new BaseBusiness<ExamTypeName>();
        }

        public List<Examination> AllExamination()
        {
            return this.GetList().ToList();
        }

        public List<ExamType> allExamType()
        {
            return db_examtype.GetList();
        }

        /// <summary>
        /// 获取考试违纪情况数据
        /// </summary>
        /// <returns></returns>
        public List<ExamBreach> AllExamBreach()
        {
            BaseBusiness<ExamBreach> dbexambreach = new BaseBusiness<ExamBreach>();

            return dbexambreach.GetIQueryable().ToList();
        }

        /// <summary>
        /// 转换模型视图
        /// </summary>
        /// <returns></returns>
        public ExaminationView ConvertToExaminationView(Examination examination)
        {
            ExaminationView view = new ExaminationView();
            view.ID = examination.ID;
            view.BeginDate = examination.BeginDate;
            view.ExamNo = examination.ExamNo;
            view.ExamType = db_examtype.GetList().Where(d => d.ID == examination.ExamType).FirstOrDefault();
            view.Remark = examination.Remark;
            view.TimeLimit = examination.TimeLimit;
            view.Title = examination.Title;
            view.PaperLevel = db_questionlevel.GetList().Where(d => d.LevelID == examination.PaperLevel).FirstOrDefault();

            return view;
        }

        public List<QuestionLevel> AllQuestionLevel()
        {
            return db_questionlevel.GetList();
        }
        
        /// <summary>
        /// 生成考试编号
        /// </summary>
        /// <returns></returns>
        public string ProductExamNumber(Examination examination)
        {
            string defaultstr = "guigu";


            string year = examination.BeginDate.Year.ToString();


            string mouth = examination.BeginDate.Month.ToString();

            if (mouth.ToString().Length == 1)
            {
                mouth = "0" + mouth;
            }
            string day = examination.BeginDate.Day.ToString();

            if (day.Length == 1)
            {
                day = "0" + day;
            }

            Random rad = new Random();
            int value = rad.Next(1000, 10000);
            string suijishu = value.ToString();

            return defaultstr + year + mouth + day + suijishu;

        }


        public List<ExamTypeName> allExamTypeName()
        {
            return db_examtypename.GetList();
        }

        /// <summary>
        /// 判断考试是否已经结束
        /// </summary>
        /// <returns></returns>

        public bool IsEnd(Examination examination)
        {
            DateTime datetime1 = examination.BeginDate;

            DateTime nowdate = DateTime.Now;

            var d = (float)(nowdate - datetime1).TotalHours;

            if (d > examination.TimeLimit)
            {
                return true;
            }

            return false;

        }


        /// <summary>
        /// 班主任获取自己带班的学生 
        /// </summary>
        /// <returns></returns>
        public List<StudentInformation> GetMyStudentData()
        {
            //获取当前登陆的班主任
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            BaseBusiness<Headmaster> dbheadmaster = new BaseBusiness<Headmaster>();
            ///获取到班主任
            var headmaster = dbheadmaster.GetList().Where(d => d.IsDelete == false).Where(d => d.informatiees_Id == user.EmpNumber).FirstOrDefault();

            //获取班主任的班级
            var classs = db_headclass.GetList().Where(d => d.IsDelete == false && d.LeaderID == headmaster.ID).ToList();

            List<StudentInformation> studentlist = new List<StudentInformation>();
            ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();

            foreach (var item in classs)
            {
                var templist = scheduleForTraineesBusiness.ClassStudent((int)item.ClassID);

                if (templist != null)
                {
                    studentlist.AddRange(templist);
                }
            }

            return studentlist;

        }
        /// <summary>
        /// 获取所有考场信息
        /// </summary>
        /// <returns></returns>
        public List<ExaminationRoom> AllExaminationRoom()
        {
            BaseBusiness<ExaminationRoom> exroom = new BaseBusiness<ExaminationRoom>();
            return exroom.GetList();
        }
        /// <summary>
        /// 删除历史考试数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult DeleteKaoShi(int id)
        //{
        //    AjaxResult result = new AjaxResult();
        //}
        public List<Examination> AllNoEndExamination()
        {
            List<Examination> result = new List<Examination>();
            List<Examination> examinations = this.AllExamination();

            foreach (var item in examinations)
            {
                if (!this.IsEnd(item))
                {
                    result.Add(item);
                }
            }

            return result;

        }


        /// <summary>
        /// 根据考试获取考场
        /// </summary>
        /// <returns></returns>
        public List<ExaminationRoom> ExaminationRoomByExamID(int examid)
        {
            List<ExaminationRoom> resultlist = new List<ExaminationRoom>();

            List<ExaminationRoom> allexaminationroom = this.AllExaminationRoom();

            foreach (var item in allexaminationroom)
            {
                if (item.Examination == examid)
                {
                    resultlist.Add(item);
                }
            }

            return resultlist;
        }


        /// <summary>
        /// 转化视图模型
        /// </summary>
        /// <returns></returns>
        public ExaminationRoomView ConvertToExaminationView(ExaminationRoom examinationRoom)
        {
            BaseBusiness<Classroom> db_classroom = new BaseBusiness<Classroom>();
            BaseBusiness<Headmaster> db_headmaster = new BaseBusiness<Headmaster>();

            ExaminationRoomView view = new ExaminationRoomView();

            view.Classroom = db_classroom.GetList().Where(d => d.IsDelete == false && d.Id == examinationRoom.Classroom_Id).FirstOrDefault();
            view.Examination = this.AllExamination().Where(d => d.ID == examinationRoom.Examination).FirstOrDefault();
            view.ID = examinationRoom.ID;

            if (examinationRoom.Invigilator1 != null)
            {
              
                view.Invigilator1 = db_emp.GetInfoByEmpID(examinationRoom.Invigilator1);
            }
            else
            {
                view.Invigilator1 = null;
            }


            if (examinationRoom.Invigilator2 != null)
            {
               
                view.Invigilator2 = db_emp.GetInfoByEmpID(examinationRoom.Invigilator2);
            }
            else
            {
                view.Invigilator2 = null;
            }



            view.Remark = examinationRoom.Remark;

            return view;
        }

        /// <summary>
        /// 获取未使用的教室
        /// </summary>
        /// <param name="dateTime">时间</param>
        /// <returns></returns>
        public List<Classroom> NotUsedClassroom(DateTime dateTime)
        {
            return null;
        }

        /// <summary>
        /// 获取还未开始的考试
        /// </summary>
        /// <returns></returns>
        public List<Examination> NoEndExamination()
        {
            var list = this.AllExamination();

            List<Examination> resultlist = new List<Examination>();

            foreach (var item in list)
            {
                if (!IsEnd(item))
                {
                    resultlist.Add(item);
                }
            }

            return resultlist;

        }

        /// <summary>
        /// 考生信息
        /// </summary>
        /// <returns></returns>
        public List<CandidateInfo> AllCandidateInfo(int examid)
        {
            return db_candidateinfo.GetList().Where(d => d.Examination == examid).ToList();
        }

        public List<CandidateInfo> AllCandidateInfo()
        {
            return db_candidateinfo.GetList();
        }



        /// <summary>
        /// 学生报名考试
        /// </summary>
        /// <param name="studentlist"></param>
        /// <param name="examid"></param>

        public void subscribeExam(Dictionary<string, bool> studentlist, int examid)
        {
            TeachingDepBusiness.TeacherClassBusiness dbteacherclass = new TeacherClassBusiness();

            Examination examination = this.AllExamination().Where(d => d.ID == examid).FirstOrDefault();

            foreach (var item in studentlist)
            {

                var tempstu = AllCandidateInfo(examid).Where(d => d.StudentID == item.Key).FirstOrDefault();

                if (tempstu == null)
                {
                    CandidateInfo candidateInfo = new CandidateInfo();
                    candidateInfo.CandidateNumber = Guid.NewGuid().ToString();
                    candidateInfo.ComputerPaper = null;
                    candidateInfo.Examination = examid;
                    candidateInfo.IsReExam = item.Value;
                    candidateInfo.Paper = null;
                    candidateInfo.StudentID = item.Key;
                    candidateInfo.ClassId = dbteacherclass.GetScheduleByStudent(item.Key).id;

                    db_candidateinfo.Insert(candidateInfo);
                }

            }

        }

        public void UpdateCandidateInfo(CandidateInfo candidateInfo)
        {
            db_candidateinfo.Update(candidateInfo);
        }


        /// <summary>
        /// 取消报名
        /// </summary>
        /// <param name="students"></param>
        /// <param name="examid"></param>
        public void cancelsubscribeExam(List<string> students, int examid)
        {

            foreach (var item in students)
            {
                var tempexamobj = this.AllCandidateInfo(examid).Where(d => d.StudentID == item).FirstOrDefault();

                if (tempexamobj != null)
                {
                    db_candidateinfo.Delete(tempexamobj);

                    //删除成绩单


                }

            }

        }


        /// <summary>
        /// 安排监考员
        /// </summary>
        /// <param name="examid">考试号</param>
        /// <param name="examroomid">考场id</param>
        /// <param name="proctorList">监考员</param>
        public void ArrangeTheInvigilator(int examid, int examroomid, List<string> proctorList)
        {
            var exammroom = this.ExaminationRoomByExamID(examid).Where(d => d.Classroom_Id == examroomid).FirstOrDefault();
            var exam = this.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
            BaseBusiness<Headmaster> dbheadmaster = new BaseBusiness<Headmaster>();
            BaseBusiness<Classroom> dbclassroom = new BaseBusiness<Classroom>();
            //首先先删除
            exammroom.Invigilator1 = null;
            exammroom.Invigilator2 = null;

            db_examroom.Update(exammroom);
            if (proctorList.Count == 0)
            {
                return;
            }

            if (proctorList[0] != null)
            {
                var headmaster = db_emp.GetAll().Where(d => d.EmployeeId == proctorList[0]).FirstOrDefault();

                exammroom.Invigilator1 = headmaster.EmployeeId;

                //发送短信 
                var classroom = dbclassroom.GetEntity(exammroom.Classroom_Id);
                string smgText = "监考通知: "+ exam.Title +" ， 教室："+classroom.ClassroomName + "，时间："+ exam.BeginDate;

                PhoneMsgHelper.SendMsg(headmaster.Phone, smgText);
            }

            if (proctorList.Count > 1)
            {
                if (proctorList[1] != null)
                {
                    var headmaster1 = db_emp.GetAll().Where(d => d.EmployeeId == proctorList[1]).FirstOrDefault();
                    exammroom.Invigilator2 = headmaster1.EmployeeId;

                    //发送短信
                    var classroom = dbclassroom.GetEntity(exammroom.Classroom_Id);
                    string smgText = "监考通知: " + exam.Title + " ， 教室：" + classroom.ClassroomName + "，时间：" + exam.BeginDate;

                    PhoneMsgHelper.SendMsg(headmaster1.Phone, smgText);


                }
            }

            db_examroom.Update(exammroom);

        }


        /// <summary>
        /// 获取监考员
        /// </summary>      
        /// <param name="examid"></param>
        /// <param name="examroomid"></param>
        /// <returns></returns>
        public List<EmployeesInfo> GetProtorData(int examid, int examroomid)
        {
            var examroom = this.ExaminationRoomByExamID(examid).Where(d => d.Classroom_Id == examroomid).FirstOrDefault();

            List<EmployeesInfo> list = new List<EmployeesInfo>();

            BaseBusiness<Headmaster> dbheadmaster = new BaseBusiness<Headmaster>();

            if (examroom.Invigilator1 != null)
            {
              

                var emp1 = db_emp.GetInfoByEmpID(examroom.Invigilator1);

                if (emp1 != null)
                {
                    list.Add(emp1);
                }
            }

            if (examroom.Invigilator2 != null)
            {
              
                var emp2 = db_emp.GetInfoByEmpID(examroom.Invigilator2);

                if (emp2 != null)
                {
                    list.Add(emp2);
                }
            }



            return list;
        }

        //public List<HeadMasterSortView> GetHeadmastersByGrand(int grandid)
        //{
        //    BaseBusiness<HeadMasterGrand> db_headmastergrand = new BaseBusiness<HeadMasterGrand>();

        //    //获取所有考试
        //    var examlist = this.AllExamination();

        //    //获取未开始的考试
        //    List<Examination> noendexamlist = new List<Examination>();
        //    foreach (var item in examlist)
        //    {
        //        if (!this.IsEnd(item))
        //        {
        //            noendexamlist.Add(item);
        //        }
        //    }

        //    List<int> headmasterlist = new List<int>();

        //    foreach (var item in noendexamlist)
        //    {
        //        var templist = this.ExaminationRoomByExamID(item.ID).ToList();

        //        foreach (var item1 in templist)
        //        {
        //            if (item1.Invigilator1 != null)
        //            {
        //                headmasterlist.Add(item1.Invigilator1);
        //            }

        //            if (item1.Invigilator2 != null)
        //            {
        //                headmasterlist.Add(item1.Invigilator2);
        //            }
        //        }
        //    }


        //    var list1 = db_headmastergrand.GetList().Where(d => d.GrandID == grandid).ToList();

        //    //排除 


        //    var list = new List<HeadMasterGrand>();

        //    foreach (var item in list1)
        //    {
        //        if (!IsCoaint(headmasterlist, item))
        //        {
        //            list.Add(item);
        //        }
        //    }



        //    EmployeesInfoManage empdb = new EmployeesInfoManage();
        //    BaseBusiness<Headmaster> db_headmaster = new BaseBusiness<Headmaster>();
        //    //转型
        //    List<HeadMasterSortView> resultlist = new List<HeadMasterSortView>();
        //    foreach (var item in list)
        //    {
        //        HeadMasterSortView headMasterSortView = new HeadMasterSortView();
        //        var headmaster = db_headmaster.GetList().Where(d => d.IsDelete == false && d.ID == item.HeadMasterID).FirstOrDefault();
        //        headMasterSortView.emp = empdb.GetInfoByEmpID(headmaster.informatiees_Id);
        //        headMasterSortView.ID = item.ID;
        //        resultlist.Add(headMasterSortView);
        //    }

        //    return resultlist;

        //}

        public bool IsCoaint(List<int> sources, HeadMasterGrand headMasterGrand)
        {
            foreach (var item in sources)
            {
                if (item == headMasterGrand.HeadMasterID)
                {
                    return true;
                }
            }

            return false;


        }


        /// <summary>
        /// 考场人员分布
        /// </summary>
        /// <returns></returns>
        public List<ExamRoomDistributed> AllExamroomDistributed(int examid)
        {
            List<ExamRoomDistributed> list = db_examroomDistributed.GetList().Where(d => d.ExamID == examid).ToList();

            return list;
        }

        public ComputerTestQuestionsView AllComputerTestQuestion(int PaperLevel, int courseid,bool IsNeedProposition = true)
        {
            List<ComputerTestQuestionsView> resutlist = new List<ComputerTestQuestionsView>();
            //List<ComputerTestQuestionsView> resutlist2 = new List<ComputerTestQuestionsView>();
            var list = db_computerQuestion.AllComputerTestQuestion().Where(d=>d.Course == courseid && d.Level == PaperLevel);
            foreach (var item in list)
            {
                var obj = db_computerQuestion.ConvertToComputerTestQuestionsView(item, true);

                if (obj != null)
                {
                    resutlist.Add(obj);
                }
            }
            Random rendom = new Random();

            var reslist = resutlist.ToList();
            int rdom = rendom.Next(0, reslist.Count);
            return reslist[rdom];
        }


        /// <summary>
        /// 转换模型视图
        /// </summary>
        /// <returns></returns>
        public ExamTypeView ConvertToExamTypeView(ExamType examType)
        {
            ExamTypeView view = new ExamTypeView();

            BaseBusiness<ExamTypeName> db_examtypenaem = new BaseBusiness<ExamTypeName>();

            view.ID = examType.ID;
            view.GrandID = db_grand.GetGrandByID((int)examType.GrandID);
            view.TypeName = db_examtypenaem.GetList().Where(d => d.ID == examType.ExamTypeID).FirstOrDefault();

            return view;
        }


        public XmlElement ExamCouresConfigAdd(int examid, int courseid)
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/ExamCourseconfig.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            //添加节点
            var examElemet = xmlDocument.CreateElement("exam");
            examElemet.SetAttribute("id", examid.ToString());

            var courseElemt = xmlDocument.CreateElement("course");  
            courseElemt.SetAttribute("id", courseid.ToString());

            examElemet.AppendChild(courseElemt);

            xmlRoot.AppendChild(examElemet);

            xmlDocument.Save(System.Web.HttpContext.Current.Server.MapPath("/Config/ExamCourseconfig.xml"));

            return examElemet;

        }

        public XmlElement ExamCourseConfigRead(int examid)
        {

            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/ExamCourseconfig.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            var list = xmlRoot.ChildNodes;

            foreach (XmlElement item in list)
            {
                var id = item.Attributes["id"];

                if (id.Value == examid.ToString())
                {
                    return item;
                }
            }

            return null;
        }

        public XmlElement ExamCourseConfigRemove(int examid)
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/ExamCourseconfig.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            var list = xmlRoot.ChildNodes;

            foreach (XmlElement item in list)
            {
                var id = item.Attributes["id"];

                if (id.Value == examid.ToString())
                {
                    xmlRoot.RemoveChild(item);
                    xmlDocument.Save(System.Web.HttpContext.Current.Server.MapPath("/Config/ExamCourseconfig.xml"));
                    return item;
                }
            }
            return null;

        }



        /// <summary>
        /// 考试座位安排  返回值结构 : 匿名对象，属性> classroom(教室),seat(座位表)   seat(匿名对象) 属性> student(学生) seat(座位)
        /// </summary>
        /// <param name="exam">考试ID</param>
        /// <returns></returns>
        public List<object> GenerateSeatTable(int exam)
        {
            TeacherClassBusiness teacherClass = new TeacherClassBusiness();

            List<object> resultlist = new List<object>();

            BaseBusiness<Classroom> db_classRoom = new BaseBusiness<Classroom>();


            //获取这次考试的考场

            List<ExaminationRoom> examroomlist = this.AllExaminationRoom().Where(d => d.Examination == exam).ToList();

            //获取参加考试的人数  
            var candidateinfolist = this.AllCandidateInfo(exam).OrderBy(d => d.CandidateNumber).ToList();


            foreach (var item in examroomlist)
            {
                //座位表
                List<string> seatlist = new List<string>();

                var classroom = db_classRoom.GetList().Where(d => d.Id == item.Classroom_Id).FirstOrDefault();


                ////班级能容纳的人数
                //var classroomcount = (int)classroom.Count;


                //var stulist = candidateinfolist.Take(classroomcount).ToList();


                //将这些考生分配到教室

                foreach (var item2 in candidateinfolist)
                {
                    var tempbuted = this.AllExamroomDistributed(exam).Where(d => d.CandidateNumber == item2.CandidateNumber).FirstOrDefault();

                    if (tempbuted == null)
                    {
                        ExamRoomDistributed distributed = new ExamRoomDistributed();
                        distributed.CandidateNumber = item2.CandidateNumber;
                        distributed.ExamID = exam;
                        distributed.ExaminationRoom = item.ID;
                        db_examroomDistributed.Insert(distributed);
                    }
                    else
                    {
                        tempbuted.ExaminationRoom = item.ID;

                        db_examroomDistributed.Update(tempbuted);
                    }
                }


                //创建随机数
                Random r = new Random();

                var stulist = candidateinfolist.ToList();
                for (int i = 0; i < stulist.Count; i++) 
                {
                    //生成随机数
                    int randomNum = r.Next(1, stulist.Count + 1);

                    string Snum = seatlist.Where(d => d == randomNum.ToString()).FirstOrDefault();

                    if (Snum == null)
                    {
                        seatlist.Add(randomNum.ToString());
                        //
                    }
                    else
                    {
                        i--;
                    }


                }

                List<object> seat = new List<object>();

                for (int i = 0; i < seatlist.Count; i++)
                {

                    BaseBusiness<StudentInformation> db_student = new BaseBusiness<StudentInformation>();

                    var stu = db_student.GetList().Where(d => d.StudentNumber == stulist[i].StudentID).FirstOrDefault();

                    var stuView = teacherClass.GetStudetentDetailView(stu);

                    if (stu != null)
                    {
                        var seatd = new
                        {
                            student = stuView,
                            seat = seatlist[i]

                        };

                        seat.Add(seatd);


                    }


                }

                //组装
                var obj = new
                {

                    classroom = classroom,
                    seat = seat
                };
                resultlist.Add(obj);


            }

            return resultlist;
        }


        /// <summary>
        /// 获取可以监考的人员
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetSureInvigilator()
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/ExaminationSystem/config/InvigilatorConfig.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            var depNodelist = xmlRoot.GetElementsByTagName("dep");

            List<EmployeesInfo> emplist = new List<EmployeesInfo>();

            foreach (XmlElement item in depNodelist)
            {
                //获取部门ID
                var depid = item.Attributes["depid"].Value;

                var tempemplist = db_emp.GetEmpsByDeptid(int.Parse(depid));

                emplist.AddRange(tempemplist);
            }

            return emplist;


        }


        /// <summary>
        /// 获取可以成为监考员的部门
        /// </summary>
        /// <returns></returns>
        public List<Department> GetSureInvigilatorDep()
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/ExaminationSystem/config/InvigilatorConfig.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            var depNodelist = xmlRoot.GetElementsByTagName("dep");

            List<Department> Deplist = new List<Department>();

            foreach (XmlElement item in depNodelist)
            {
                //获取部门ID
                var depid = item.Attributes["depid"].Value;

                var dep = db_emp.GetDeptById(int.Parse(depid));

                if (dep != null)
                {
                    Deplist.Add(dep);
                }
            }

            return Deplist;
        }

    }
}
