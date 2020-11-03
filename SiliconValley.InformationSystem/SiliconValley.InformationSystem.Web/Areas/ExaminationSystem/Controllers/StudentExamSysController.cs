using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
namespace SiliconValley.InformationSystem.Web.Areas.ExaminationSystem.Controllers
{
    using BaiduBce.Services.Bos.Model;
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.StudentBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Entity.MyEntity;
   

  
    public class StudentExamSysController : Controller
    {

        private readonly ExaminationBusiness db_exam;

        private readonly StudentExamBusiness db_stuExam;

        private readonly ChoiceQuestionBusiness db_choiceQuestion;
        private readonly ExamScoresBusiness db_examScores;
        private readonly AnswerQuestionBusiness db_answerQuestion;
        private readonly CandidateInfoBusiness db_candidateinfo;

        public StudentExamSysController()
        {
            db_exam = new ExaminationBusiness();
            db_stuExam = new StudentExamBusiness();
            db_choiceQuestion = new ChoiceQuestionBusiness();
            db_examScores = new ExamScoresBusiness();
            db_answerQuestion = new AnswerQuestionBusiness();
            db_candidateinfo =new CandidateInfoBusiness();
        }
        // GET: ExaminationSystem/StudentExamSys
        public ActionResult StuExamIndex()
        {
            //获取学员最近的考试

          
            return View();
        }
        public ActionResult Brushthetopic()
        {
            //提供数据 当前登录的学员、阶段、考试类型 难度级别

            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();

            var studentNumber = SessionHelper.Session["studentnumber"].ToString();
            var student = studentInformationBusiness.StudentList().Where(d => d.StudentNumber == studentNumber).FirstOrDefault();
            ViewBag.student = student;

            GrandBusiness grandBusiness = new GrandBusiness();
            var grandlist = grandBusiness.AllGrand();
            ViewBag.grandlist = grandlist;

            List<ExamType> list = db_exam.allExamType();

            List<ExamTypeView> viewlist = new List<ExamTypeView>();
            //转换类型
            foreach (var item in list)
            {
                var tempobj = db_exam.ConvertToExamTypeView(item);

                if (tempobj != null)
                    viewlist.Add(tempobj);
            }

            ViewBag.examtypelist = viewlist;

            //提供课程数据
            CourseBusiness db_course = new CourseBusiness();

            ViewBag.Courselist = db_course.GetCurriculas();

            var levellist = db_exam.AllQuestionLevel();
            ViewBag.levellist = levellist;
            //提供刷题类型,选择题/机试题

            return View();
        }
        /// <summary>
        /// 如果中途关闭了机试页,再次进入考场直接跳转到机试页
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public ActionResult ContinueThExam(int examid)
        {
            AjaxResult result = new AjaxResult();
            //获取当前登录学员
            var studentNumber = SessionHelper.Session["studentnumber"].ToString();

            var candidateinfo = db_candidateinfo.CandidateInfoList().Where(d => d.Examination == examid && d.StudentID == studentNumber).SingleOrDefault();
            if (candidateinfo.ComputerPaper == null)
            {
                result.Data = "0";
                result.Msg = "成功";
                result.ErrorCode = 400;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///获取学员最近的一次考试
        /// </summary>
        /// <returns></returns>
        public ActionResult GetStuSooExam()
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var studentNumber = SessionHelper.Session["studentnumber"].ToString();
                var exam = db_stuExam.StudetnSoonExam(studentNumber.ToString()).OrderByDescending(d => d.BeginDate).FirstOrDefault();
                var examview = db_exam.ConvertToExaminationView(exam);

                //在判断是否考完了

                if (examview != null)
                {
                    var scores = db_examScores.StuExamScores(examview.ID, studentNumber);

                    if (scores.ChooseScore != null)
                    {
                        result.Data = "0";
                        result.Msg = "成功";
                        result.ErrorCode = 400;
                    }
                    else
                    {
                        result.Data = examview;
                        result.Msg = "成功";
                        result.ErrorCode = 200;
                    }
                }
                else
                {
                    result.Data = "0";
                    result.Msg = "成功";
                    result.ErrorCode = 400;
                }



            }
            catch (Exception ex)
            {

                result.Data = null;
                result.Msg = "失败";
                result.ErrorCode = 500;

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 学员答题机试页面
        /// </summary>
        /// <returns></returns>
         public ActionResult MachineTest(int examid)
        {
            var exam = db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
            var EXAMVIEW = db_exam.ConvertToExaminationView(exam);
            ViewBag.EXAMVIEW = EXAMVIEW;
                //~获取答卷信息.
                var studentNumber = SessionHelper.Session["studentnumber"].ToString();
            //获取当前登录学员
            var answerSheetInfo = db_stuExam.AnswerSheetInfos(examid, studentNumber);
            
            ViewBag.AnswerSheetInfo = answerSheetInfo;
            
                return View();
        }
    
        /// <summary>
        /// 学员答题笔试页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AnswerSheet(int examid)
        {
            var exam = db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
            var EXAMVIEW = db_exam.ConvertToExaminationView(exam);
            ViewBag.EXAMVIEW = EXAMVIEW;

            //~获取答卷信息.
            var studentNumber = SessionHelper.Session["studentnumber"].ToString();
            //获取当前登录学员
            var answerSheetInfo = db_stuExam.AnswerSheetInfos(examid, studentNumber);

            ViewBag.AnswerSheetInfo = answerSheetInfo;

            return View();
        }

        /// <summary>
        /// 选择题题目数据
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public ActionResult ChoiceQuestionData(int examid)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                //获取考试的类型
                

                var exam = db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();

                var examveiw  = db_exam.ConvertToExaminationView(exam);

                List<ChoiceQuestionTableView> data = new List<ChoiceQuestionTableView>();
                //判断考试类型
                if (examveiw.ExamType.ExamTypeID == 1)
                {
                    data = db_stuExam.ProductChoiceQuestion(exam, 0);
                }

                if (examveiw.ExamType.ExamTypeID == 2)
                {

                    //需要获取课程

                     XmlElement xmlelm  = db_exam.ExamCourseConfigRead(examid);

                    int courseid =int.Parse( xmlelm.FirstChild.Attributes["id"].Value);

                    data = db_stuExam.ProductChoiceQuestion(exam, courseid);
                }

                
                var scores = db_stuExam.distributionScores(data);

                var res = new {

                    data = data,
                    scores = scores

                };

                result.Data = res;
                result.Msg = "成功";
                result.ErrorCode = 200;
            }
            catch (Exception ex) 
            {

                result.Data = null;
                result.Msg = "失败";
                result.ErrorCode = 500;
            }
           

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        

        public ActionResult answerQuestionData(int examid)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                var exam = db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
                var examveiw = db_exam.ConvertToExaminationView(exam);
                List<AnswerQuestionView> data = new List<AnswerQuestionView>();
                //判断考试类型
                if (examveiw.ExamType.ExamTypeID == 1)
                {
                    data = db_stuExam.productAnswerQuestion(exam, 0);
                }

                if (examveiw.ExamType.ExamTypeID == 2)
                {

                    //需要获取课程

                    XmlElement xmlelm = db_exam.ExamCourseConfigRead(examid);

                    int courseid = int.Parse(xmlelm.FirstChild.Attributes["id"].Value);

                    data = db_stuExam.productAnswerQuestion(exam, courseid);
                }

                var scores = db_stuExam.distributionScores(data);

                var res = new
                {

                    data = data,
                    scores = scores

                };
                result.Data = res;
                result.Msg = "成功";
                result.ErrorCode = 200;
            }
            catch (Exception ex)
            {

                result.Data = null;
                result.Msg = "失败";
                result.ErrorCode = 500;
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 机试题下载
        /// </summary>
        /// <returns></returns>

        public ActionResult ComputerQuestionUpload(int examid)
        {
            var exam = db_exam.AllExamination().Where(d=>d.ID == examid).FirstOrDefault();
            var examview = db_exam.ConvertToExaminationView(exam);

            //随机选择一个机试题
            var studentNumber = SessionHelper.Session["studentnumber"].ToString();
            //首先查看是否已经随机获取到了一个

            var candidateInfo =  db_exam.AllCandidateInfo(examid).Where(d=>d.StudentID == studentNumber).FirstOrDefault();
            ComputerTestQuestionsView computer = null;
            if (candidateInfo.ComputerPaper == null)
            {
                //第一次

                //判断考试类型
                if (examview.ExamType.ExamTypeID == 1)
                {
                    computer = db_stuExam.productComputerQuestion(exam,0);

                    candidateInfo.ComputerPaper = computer.ID.ToString() + ",";

                    db_exam.UpdateCandidateInfo(candidateInfo);
                }

                if (examview.ExamType.ExamTypeID == 2)
                {

                    //需要获取课程

                    XmlElement xmlelm = db_exam.ExamCourseConfigRead(examid);

                    int courseid = int.Parse(xmlelm.FirstChild.Attributes["id"].Value);
                    computer = db_stuExam.productComputerQuestion(exam, courseid);

                    candidateInfo.ComputerPaper = computer.ID.ToString() + ",";

                    db_exam.UpdateCandidateInfo(candidateInfo);
                }           
            }


            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            var ar = candidateInfo.ComputerPaper.Split(',');

           var com = db_exam.AllComputerTestQuestion(IsNeedProposition : false).Where(d => d.ID ==int.Parse( ar[0])).FirstOrDefault();

            var filename = Path.GetFileName(com.SaveURL);

            //var path = Server.MapPath("/uploadXLSXfile/ComputerTestQuestionsWord/" + filename);

            var fileData = client.GetObject("xinxihua", $"/ExaminationSystem/ComputerTestQuestionsWord/{filename}");

            //FileStream fileStream = new FileStream(path, FileMode.Open);

            return File(fileData.ObjectContent, "application/octet-stream", Server.UrlEncode(filename));
            
        }
        /// <summary>
        /// 获取选择题答案
        /// </summary>
        /// <returns></returns>
        public ActionResult ChoiceQuestionAnswer(string questions)
        {

            var ary1 = questions.Split(',');

            var list = ary1.ToList();
            list.RemoveAt(ary1.Length - 1);


            AjaxResult result = new AjaxResult();

            try
            {

                var questionlist = db_choiceQuestion.AllChoiceQuestionData();

                List<object> objlist = new List<object>();
                foreach (var item in list)
                {
                    var tempobj = questionlist.Where(d => d.Id == int.Parse(item)).FirstOrDefault();

                    if (tempobj != null)
                    {
                        var obj = new
                        {

                            questionid = tempobj.Id,
                            answer = tempobj.Answer
                        };

                        objlist.Add(obj);
                    }
                }
                
                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = objlist;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 获取题目个数
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQuestionCount()
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/questionLevelConfig.xml"));

            var xmlRoot = xmlDocument.DocumentElement;

            //获取我需要的配置		foreach	error CS1525: 表达式项“foreach”无效	

            //

            var choxml = (XmlElement)xmlRoot.GetElementsByTagName("choicequestion")[0];
            var answer = (XmlElement)xmlRoot.GetElementsByTagName("answerQuestion")[0];
            int choiceCount  = int.Parse(choxml.GetElementsByTagName("total")[0].InnerText);
            int answerCount = int.Parse(answer.GetElementsByTagName("total")[0].InnerText);

            return Json((choiceCount+ answerCount).ToString(),JsonRequestBehavior.AllowGet);
        }

        //学员提交机试
        public ActionResult MachineTestCommit(int examid)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                CloudstorageBusiness Bos = new CloudstorageBusiness();
                var client = Bos.BosClient();

                var studentNumber = SessionHelper.Session["studentnumber"].ToString();
                string direName = $"/ExaminationSystem/AnswerSheet/{studentNumber + examid}/";
                //2 将机试题保存到文件夹 
                string computerfielnaem = "computerfielnaem";
                var computerfile = Request.Files["rarfile"];

                //保存的机试题路径
                string computerUrl = "";
                if (computerfile != null)
                {
                    //获取文件拓展名称 
                    var exait = Path.GetExtension(computerfile.FileName);
                    //computerUrl = Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + direName + "/" + computerfielnaem + exait);
                    computerUrl = $"{direName}{computerfielnaem + exait}";
                    //computerfile.SaveAs(computerUrl);
                    Bos.Savefile("xinxihua", direName, computerfielnaem + exait, computerfile.InputStream);
                }
                db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
                var Candidateinfo = db_exam.AllCandidateInfo(examid).Where(d => d.Examination == examid && d.StudentID == studentNumber).FirstOrDefault();
                Candidateinfo.ComputerPaper = computerUrl;
                //获取需要替换的字符串路径

                if (Candidateinfo.ComputerPaper != null)
                {
                    var old = Candidateinfo.ComputerPaper.Substring(Candidateinfo.ComputerPaper.IndexOf(',') + 1);

                    if (old.Length == 0)
                    {
                        Candidateinfo.ComputerPaper = Candidateinfo.ComputerPaper + computerUrl;
                    }
                    else

                    {
                        Candidateinfo.ComputerPaper = Candidateinfo.ComputerPaper.Replace(old, computerUrl);
                    }
                }

                db_exam.UpdateCandidateInfo(Candidateinfo);
                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }
            return Json(result);
        }


        /// <summary>
        /// 学员提交答卷 
        /// </summary>
        /// <param name="ChoiceScores">选择题得分</param>
        /// <param name="AnswerCommit">解答题答卷</param>
        /// <returns></returns>
        public ActionResult AnswerSheetCommit(float ChoiceScores, string AnswerCommit,int examid)
        {
            AjaxResult result = new AjaxResult();

            try
            {

                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();

                var studentNumber = SessionHelper.Session["studentnumber"].ToString();
                //1.将解答题答案存入文件
                //2 将机试题文件放入AnswerSheet文件夹
                //3.修改数据库值（选择题分数,解答题答案路径,机试题路径）
                //4.记录选择题分数

                //1 将解答题答案存入文件 首先新建学生答卷文件夹 名称规则 学号加上考试ID

                string direName = $"/ExaminationSystem/AnswerSheet/{studentNumber + examid}/";
                //写文件
                 string answerfilename = "AnswerSheet.txt";

                PutObjectResponse putObjectResponseFromString = client.PutObject("xinxihua",$"{direName}{answerfilename}", AnswerCommit);

                //2 将机试题保存到文件夹 
                //string computerfielnaem = "computerfielnaem";
                //var computerfile = Request.Files["rarfile"];

                //保存的机试题路径
                string computerUrl = "";
                //if (computerfile != null)
                //{
                //获取文件拓展名称 
                //var exait = Path.GetExtension(computerfile.FileName);
                //computerUrl = Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + direName + "/" + computerfielnaem + exait);
                //computerUrl = $"{direName}{computerfielnaem + exait}";
                //computerfile.SaveAs(computerUrl);
                //Bos.Savefile("xinxihua", direName, computerfielnaem + exait, computerfile.InputStream);
                //}

                //3.修改数据库值（选择题分数，解答题答案路径，机试题路径）
                db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
                var Candidateinfo = db_exam.AllCandidateInfo(examid).Where(d => d.Examination == examid && d.StudentID == studentNumber).FirstOrDefault();
                //Candidateinfo.Paper = Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + direName + "/" + answerfilename);
                Candidateinfo.Paper = $"{direName}{answerfilename}";

                //获取需要替换的字符串路径

                if (Candidateinfo.ComputerPaper != null)
                {
                    var old = Candidateinfo.ComputerPaper.Substring(Candidateinfo.ComputerPaper.IndexOf(',') + 1);

                    if (old.Length == 0)
                    {
                        Candidateinfo.ComputerPaper = Candidateinfo.ComputerPaper + computerUrl;
                    }
                    else

                    {
                        Candidateinfo.ComputerPaper = Candidateinfo.ComputerPaper.Replace(old, computerUrl);
                    }
                }

                db_exam.UpdateCandidateInfo(Candidateinfo);

                //4.记录选择题分数
                //获取考生
                var stuScores = db_examScores.StuExamScores(examid, studentNumber);

                if (stuScores == null)
                {
                    TestScore testScore = new TestScore();
                    testScore.CandidateInfo = Candidateinfo.CandidateNumber;
                    testScore.ChooseScore = ChoiceScores;
                    testScore.Examination = examid;

                    db_examScores.Insert(testScore);
                }

                else
                {
                    stuScores.ChooseScore = ChoiceScores;
                    db_examScores.Update(stuScores);
                }
              

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            } 
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }
            return Json(result);
        }
        public ActionResult MockExaminationView()
        {
            //提供数据 当前登录的学员、阶段、考试类型 难度级别

            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();

            var studentNumber = SessionHelper.Session["studentnumber"].ToString();
            var student = studentInformationBusiness.StudentList().Where(d => d.StudentNumber == studentNumber).FirstOrDefault();
            ViewBag.student = student;


            GrandBusiness grandBusiness = new GrandBusiness();
            var grandlist = grandBusiness.AllGrand();
            ViewBag.grandlist = grandlist;


            List<ExamType> list = db_exam.allExamType();

            List<ExamTypeView> viewlist = new List<ExamTypeView>();
            //转换类型
            foreach (var item in list)
            {
                var tempobj = db_exam.ConvertToExamTypeView(item);

                if (tempobj != null)
                    viewlist.Add(tempobj);
            }

            ViewBag.examtypelist = viewlist;

            //提供课程数据
            CourseBusiness db_course = new CourseBusiness();

            ViewBag.Courselist = db_course.GetCurriculas();

            var levellist = db_exam.AllQuestionLevel();
            ViewBag.levellist = levellist;

            return View();
        }
        /// <summary>
        /// 刷题选择题数据
        /// </summary>
        /// <param name="examType">阶段</param>
        /// <param name="kecheng">课程</param>
        /// <param name="level">难度等级</param>
        /// <returns></returns>
        public ActionResult BrushAlltheQestion(string examType, string kecheng)
        {
            AjaxResult result = new AjaxResult();

            Examination examination = new Examination();


            examination.ExamType = int.Parse(examType);
            examination.ID = 0;
            List<MultipleChoiceQuestion> multipleChoicelist = db_choiceQuestion.AllChoiceQuestionData().Where(d=>d.Course==int.Parse(kecheng)).ToList();
 
            try
            {
                var res = new
                {

                    data = multipleChoicelist,

                };


                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = res;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 刷题解答题数据
        /// </summary>
        /// <param name="examType">阶段</param>
        /// <param name="kecheng">课程</param>
        /// <param name="level">难度等级</param>
        /// <returns></returns>
        public ActionResult BrushAlltheQuestion(string examType, string kecheng)
        {
            AjaxResult result = new AjaxResult();

            Examination examination = new Examination();

            examination.ExamType = int.Parse(examType);
            examination.ID = 0;
            List<AnswerQuestionBank> list = db_answerQuestion.AllAnswerQuestion().Where(d => d.Course==int.Parse(kecheng)).ToList();
            try
            {
                var res = new
                {

                    data = list,

                };
                result.Data = res;
                result.Msg = "成功";
                result.ErrorCode = 200;
            }
            catch (Exception ex)
            {

                throw;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 模拟选择题数据
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult MockChoiceqestiondata(string examType, string kecheng, string level)
        {
            AjaxResult result = new AjaxResult();

            Examination examination = new Examination();

            examination.ExamType = int.Parse(examType);
            examination.ID = 0;
            examination.PaperLevel = int.Parse(level);

            try
            {
                var questiondata = db_stuExam.ProductChoiceQuestion(examination, int.Parse(kecheng));


                var scores = db_stuExam.distributionScores(questiondata);

                var res = new
                {

                    data = questiondata,
                    scores = scores

                };


                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = res;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //模拟解答题数据
        public ActionResult MockAnswerquestiondata(string examType, string kecheng, string level)
        {
            AjaxResult result = new AjaxResult();

            Examination examination = new Examination();

            examination.ExamType = int.Parse(examType);
            examination.ID = 0;
            examination.PaperLevel = int.Parse(level);

            try
            {
                var questiondata = db_stuExam.productAnswerQuestion(examination, int.Parse(kecheng));

                var scores = db_stuExam.distributionScores(questiondata);

                var res = new
                {

                    data = questiondata,
                    scores = scores

                };
                result.Data = res;
                result.Msg = "成功";
                result.ErrorCode = 200;
            }
            catch (Exception ex)
            {

                throw;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MockComputerquestion(string examType, string kecheng, string level)
        {

            AjaxResult result = new AjaxResult();

            Examination examination = new Examination();

            examination.ExamType = int.Parse(examType);
            examination.ID = 0;
            examination.PaperLevel = int.Parse(level);

            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            var questiondata = db_stuExam.productComputerQuestion(examination, int.Parse(kecheng));

                var filename = Path.GetFileName(questiondata.SaveURL);

                // var filename = Path.GetFileName(com.SaveURL);

            var fileData = client.GetObject("xinxihua", $"/ExaminationSystem/ComputerTestQuestionsWord/{filename}");

            //FileStream fileStream = new FileStream(path, FileMode.Open);


            return File(fileData.ObjectContent, "application/octet-stream", Server.UrlEncode(filename));

        }

        public ActionResult GetExamEndDate(string examid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var exam = db_exam.GetEntity(int.Parse(examid));

                result.Data = exam.BeginDate.AddHours(exam.TimeLimit);
                result.ErrorCode = 200;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {

                result.Data = null;
                result.ErrorCode = 500;
                result.Msg = "失败";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}