using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.ExaminationSystemBusiness
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.XSSF.UserModel;
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Depository.CellPhoneSMS;
    using SiliconValley.InformationSystem.Entity.Entity;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
    using SiliconValley.InformationSystem.Util;
    using System.IO;

    //考试成绩业务类
    public class ExamScoresBusiness : BaseBusiness<TestScore>
    {
        /// <summary>
        /// 考试业务类实例
        /// </summary>
        private readonly ExaminationBusiness db_exam;
        /// <summary>
        /// 阅卷安排业务类实例
        /// </summary>
        private readonly BaseBusiness<MarkingArrange> db_markingArrange;


        /// <summary>
        /// 员工业务类实例
        /// </summary>
        private readonly EmployeesInfoManage db_emp;

        public ExamScoresBusiness()
        {
            db_exam = new ExaminationBusiness();
            db_markingArrange = new BaseBusiness<MarkingArrange>();
            db_emp = new EmployeesInfoManage();
        }

        public List<TestScore> AllExamScores()
        {
            return this.GetList().ToList();
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

            if (contentType == "application/vnd.ms-excel")
            {
                workbook = new HSSFWorkbook(stream);
            }

            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(0);
            var result = ExcelImportAtdSql(sheet);
            stream.Close();
            stream.Dispose();
            workbook.Close();

            return result;
        }

        private AjaxResult ExcelImportAtdSql(ISheet sheet)
        {
            var ajaxresult = new AjaxResult();
            try
            {
                while (true)
                {

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

        /// <summary>
        /// 获取考试成绩
        /// </summary>
        /// <returns></returns>
        public List<TestScore> ExamScores(int examid)
        {
            return this.AllExamScores().Where(d => d.Examination == examid).ToList();
        }


        /// <summary>
        /// 获取学员个人考试成绩
        /// </summary>
        /// <param name="examid">考试ID</param>
        /// <param name="studentNo">学号</param>
        /// <returns></returns>
        public TestScore StuExamScores(int examid, string studentNo)
        {
            //获取学员考号
            var candidateinfo = db_exam.AllCandidateInfo(examid).Where(d => d.StudentID == studentNo).FirstOrDefault();

            return this.ExamScores(examid).Where(d => d.CandidateInfo == candidateinfo.CandidateNumber).FirstOrDefault();
        }


        /// <summary>
        /// 获取所有阅卷安排
        /// </summary>
        /// <returns></returns>
        public List<MarkingArrange> AllMarkingArrange()
        {
            return db_markingArrange.GetList();
        }

        /// <summary>
        /// 转换模型
        /// </summary>
        /// <returns></returns>
        public MarkingArrangeView ConvertToMarkingArrangeView(MarkingArrange markingArrange)
        {
             
            BaseBusiness<Classroom> dbclassroom = new BaseBusiness<Classroom>();
           
            MarkingArrangeView view = new MarkingArrangeView();

           var examroom = db_exam.AllExaminationRoom().Where(d => d.ID == markingArrange.ExamRoom).FirstOrDefault();
            List<Examination> list= db_exam.AllExamination();
            view.ExamID = db_exam.AllExamination().Where(d => d.ID == markingArrange.ExamID).FirstOrDefault();
            view.ExamRoom = db_exam.AllExaminationRoom().Where(d => d.ID == markingArrange.ExamRoom).FirstOrDefault();
            view.ID = markingArrange.ID;
            view.IsFinsh = markingArrange.IsFinsh;
            view.MarkingTeacher = db_emp.GetInfoByEmpID(markingArrange.MarkingTeacher);
            
            view.classroom = dbclassroom.GetList().Where(d => d.Id == examroom.Classroom_Id).FirstOrDefault();

            return view;


        }

        /// <summary>
        /// 获取老师未完成阅卷任务
        /// </summary>
        /// <returns></returns>
        public List<MarkingArrange> MarkingArrangeByEmpID(string empNumber)
        {
            return this.AllMarkingArrange().Where(d => d.IsFinsh == false && d.MarkingTeacher == empNumber).ToList();
        }


        /// <summary>
        ///  获取这个考场未完成的成绩录入
        /// </summary>
        /// <param name="examid">考试ID</param>
        /// <param name="examroom">教室ID</param>
        /// <returns></returns>
        public List<TestScore> NoMarkeArrangeScore(int examid, int examroom)
        {

            //获取考场ID
            var examroomobj = db_exam.AllExaminationRoom().Where(d => d.Examination == examid && d.Classroom_Id == examroom).FirstOrDefault();

            var clist = db_exam.AllExamroomDistributed(examid).Where(d => d.ExaminationRoom == examroomobj.ID).ToList();


            //考场的成绩表
            List<TestScore> list = new List<TestScore>();
            foreach (var item in clist)
            {
                var scores = this.AllExamScores().Where(d => d.Examination == examid && d.CandidateInfo == item.CandidateNumber).FirstOrDefault();

                if (scores != null)
                {
                    list.Add(scores);
                }
            }

            //解答题分数 或者 机试题分数为 NULL 即视为为阅卷
            List<TestScore> resultlist = new List<TestScore>();
            foreach (var item in list)
            {
                if (item.OnBoard == null || item.TextQuestionScore == null)
                {
                    resultlist.Add(item);
                }
            }

            return resultlist;





        }


        /// <summary>
        /// 获取考场考生
        /// </summary>
        /// <param name="examid">考试ID</param>
        /// <param name="examroom">考场（教室ID）</param>
        /// <returns></returns>
        public List<CandidateInfo> CandidateinfosByExamroom(int examid, int examroom)
        {

            List<CandidateInfo> resultlist = new List<CandidateInfo>();

            var list = db_exam.AllExamroomDistributed(examid);

            foreach (var item in list)
            {
                var tempexamroom = db_exam.AllExaminationRoom().Where(d => d.ID == item.ExaminationRoom).FirstOrDefault();

                if (tempexamroom.Classroom_Id == examroom)
                {
                    var tempobj = db_exam.AllCandidateInfo(examid).Where(d => d.CandidateNumber == item.CandidateNumber).FirstOrDefault();

                    if (tempobj != null)
                        resultlist.Add(tempobj);
                }
            }

            return resultlist;

        }



        /// <summary>
        /// 是否完成了阅卷
        /// </summary>
        /// <param name="EmpNo"></param>
        /// <param name="examid"></param>
        /// <param name="examroom">教室ID</param>
        /// <returns></returns>
        public bool IsFinshMarking(string EmpNo, int examid, int examroom)
        {


            //获取考场考生

            var candidinfolist = this.CandidateinfosByExamroom(examid, examroom);

            foreach (var item in candidinfolist)
            {
                //获取每个人成绩表
                var stuScore = this.StuExamScores(examid, item.StudentID);

                if (stuScore.OnBoard == null || stuScore.TextQuestionScore == null)
                {
                    return false;
                }
            }

            return true;


        }

        public void UpdateMarkingArrange(MarkingArrange markingArrange)

        {
            db_markingArrange.Update(markingArrange);
        }


        /// <summary>
        /// 初始化成绩单
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="StuExamNumber">考生号</param>
        public void InitExamScores(int examid, string StuExamNumber)
        {
            TestScore score = new TestScore();

            score.CandidateInfo = StuExamNumber;
            score.Examination = examid;
            score.ChooseScore = null;
            score.OnBoard = null;
            score.TextQuestionScore = null;

            this.Insert(score);
        }

        /// <summary>
        /// 删除成绩单
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="StuExamNumber"></param>
        public void RemoveExamScores(int examid, string StuExamNumber)
        {
            var score = this.ExamScores(examid).Where(d => d.CandidateInfo == StuExamNumber).FirstOrDefault();

            this.Delete(score);
        }


        /// <summary>
        /// 设置阅卷老师
        /// </summary>
        public void SetMarkingTeacher(int examid, int examroomid, string empid)
        {
            var emp = db_emp.GetInfoByEmpID(empid);
            var temp = this.AllMarkingArrange().Where(d => d.ExamID == examid && d.ExamRoom == examroomid &&d.MarkingTeacher == empid).FirstOrDefault();

            if (temp == null)
            {
                MarkingArrange markingArrange = new MarkingArrange();
                markingArrange.ExamID = examid;
                markingArrange.ExamRoom = examroomid;
                markingArrange.IsFinsh = false;
                markingArrange.MarkingTeacher = empid;

                db_markingArrange.Insert(markingArrange);

                return;
            }

            temp.MarkingTeacher = empid;

            db_markingArrange.Update(temp);

            var smgText = "硅谷高科阅卷通知 ：收到一份阅卷通知  请及时查看";

            PhoneMsgHelper.SendMsg(emp.Phone, smgText);
        }


        public StudentExamScoreView ConvertToStudentExamScoreView(TestScore testScore)
        {
            StudentExamScoreView view = new StudentExamScoreView();

            GrandBusiness db_grand = new GrandBusiness();

            var allExem = db_exam.AllExamination();


            TeachingDepBusiness.TeacherClassBusiness dbteacherclass = new TeachingDepBusiness.TeacherClassBusiness();

            var candidInfo = db_exam.AllCandidateInfo().Where(d => d.CandidateNumber == testScore.CandidateInfo).FirstOrDefault();
            BaseBusiness<StudentInformation> dbstudentg = new BaseBusiness<StudentInformation>();
            //阅卷老师
            TeachingDepBusiness.TeacherBusiness dbteacher = new TeachingDepBusiness.TeacherBusiness();

            if (testScore.Reviewer == null)
            {
                view.MarkingTeacherName = "无";
            }
            else
            {
                var markingteacher = db_emp.GetInfoByEmpID(dbteacher.GetTeacherByID(testScore.Reviewer).EmployeeId);
                view.MarkingTeacherName = markingteacher.EmpName;
            }
          
            view.Score = testScore;
            BaseBusiness<ClassSchedule> dbclass = new BaseBusiness<ClassSchedule>();
            view.StudentClass = dbclass.GetIQueryable().Where(d => d.id == candidInfo.ClassId).FirstOrDefault().ClassNumber;
            view.StudentName = dbstudentg.GetIQueryable().Where(d => d.StudentNumber == candidInfo.StudentID).FirstOrDefault().Name;
            view.ExamTitle = db_exam.AllExamination().Where(d => d.ID == testScore.Examination).FirstOrDefault().Title;
            view.StudentNumber = candidInfo.StudentID;

            //获取类型
            var exam = allExem.Where(d => d.ID == testScore.Examination).FirstOrDefault();
            var examtype = db_exam.allExamType().Where(d => d.ID == exam.ExamType).FirstOrDefault();

            var examtypename = db_exam.allExamTypeName().Where(d => d.ID == examtype.ExamTypeID).FirstOrDefault();

            view.Grand = db_grand.AllGrand().Where(d => d.Id == examtype.GrandID).FirstOrDefault();

            if (examtypename.TypeName.Contains("升学"))
            {
                view.Curriculum = null;
            }
            else
            {
                // 去获取课程ID

               var curxml = db_exam.ExamCourseConfigRead((int)testScore.Examination);
                var coursexml = curxml.GetElementsByTagName("course")[0];

                var couserid = coursexml.Attributes["id"].Value;

                CourseBusiness dbcourse = new CourseBusiness();

                var cu = dbcourse.Curriculas().Where(d => d.CurriculumID == int.Parse(couserid)).FirstOrDefault();

                view.Curriculum = cu;

            }

            return view;
        }


        /// <summary>
        /// 获取参加考试班级
        /// </summary>
        /// <returns></returns>
        public List<ClassSchedule> GetExamJoinClass(int examid)
        {
            TeachingDepBusiness.TeacherClassBusiness dbteacherclass = new TeachingDepBusiness.TeacherClassBusiness();
            List<ClassSchedule> classlist = new List<ClassSchedule>();

           var lsit = db_exam.AllCandidateInfo(examid);

            foreach (var item in lsit)
            {
               var tempclass = dbteacherclass.AllClassSchedule().Where(d => d.id == item.ClassId).FirstOrDefault();

                if (!dbteacherclass.IsContains(classlist, tempclass))
                {
                    //如果不存在
                    classlist.Add(tempclass);
                }
            }


            return classlist;
        }


        /// <summary>
        /// 获取班级升学成绩
        /// </summary>
        /// <param name="classid">班级ID</param>
        /// <param name="grandId">阶段ID</param>
        /// <returns></returns>
        public List<StudentExamScoreView> ClassScores(int classid, int grandId)
        {

            var list =db_exam.AllExamination();

            //筛选出 升学考试
            List<ExaminationView> examviewlist = new List<ExaminationView>();

            foreach (var item in list)
            {
                //var exam = db_exam.AllExamination().Where(d => d.ID == item.Examination).FirstOrDefault();

                var examview = db_exam.ConvertToExaminationView(item);

                if (examview.ExamType.GrandID == grandId && examview.ExamType.ExamTypeID == 1)
                {

                    examviewlist.Add(examview);
                }
               
            }

            ///筛选出班级学生

            List<CandidateInfo> candidateifnolist = new List<CandidateInfo>();


            foreach (var item in examviewlist)
            {
               var candidlist = db_exam.AllCandidateInfo(item.ID).Where(d=>d.ClassId == classid).ToList();

                if (candidlist != null)
                {
                    candidateifnolist.AddRange(candidlist);
                }
            }

            //获取成绩
            List<StudentExamScoreView> scorelist = new List<StudentExamScoreView>();

            foreach (var item in candidateifnolist)
            {
                var score = this.AllExamScores().Where(d=>d.CandidateInfo == item.CandidateNumber).FirstOrDefault();

                if (score != null)
                {
                   var tempobj = this.ConvertToStudentExamScoreView(score);

                    if (tempobj != null)
                    {
                        scorelist.Add(tempobj);
                    }
                }
            }


            return scorelist;
        }

        /// <summary>
        /// 获取学生成绩
        /// </summary>
        /// <param name="studentNumber"></param>
        /// <param name="type">kecheng, shengxue</param>
        /// <returns></returns>
        public List<StudentExamScoreView> StudentScores(string studentNumber)
        {
            List<StudentExamScoreView> examScoreViews = new List<StudentExamScoreView>();

            List<TestScore> allScores = this.AllExamScores();

            List<CandidateInfo> allCadidateInfo = db_exam.AllCandidateInfo();

            List<TestScore> resultlist = new List<TestScore>();

            var didInfolist = allCadidateInfo.Where(x => x.StudentID == studentNumber).ToList();

            didInfolist.ForEach(d=>
            {
               var templist = allScores.Where(t => t.CandidateInfo == d.CandidateNumber).ToList();
                if (templist != null)
                {
                    resultlist.AddRange(templist);
                }
            });

            resultlist.ForEach(d=> {

                StudentExamScoreView examScoreView = ConvertToStudentExamScoreView(d);

                if (examScoreView != null)
                {
                    examScoreViews.Add(examScoreView);
                }
            });

            return examScoreViews;
        }

    }
}
