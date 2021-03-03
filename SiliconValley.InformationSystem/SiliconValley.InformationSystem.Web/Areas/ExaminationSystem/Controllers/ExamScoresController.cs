using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.ExaminationSystem.Controllers
{
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Xml;
    using Newtonsoft.Json;
    using NPOI.SS.UserModel;
    using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.StudentBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Entity.ViewEntity.zhongyike;
    using Spire.Doc;
    using Spire.Doc.Documents;

    /// <summary>
    /// 学员成绩控制器
    /// </summary>
    /// 
    [CheckLogin]
    public class ExamScoresController : Controller
    {
        // GET: ExaminationSystem/ExamScores



        private readonly ExaminationBusiness db_exam;

        private readonly ExamScoresBusiness db_examScores;

        private readonly StudentExamBusiness db_stuExam;

        private readonly AnswerQuestionBusiness db_answerQuestion;

        private readonly StudentInformationBusiness db_student;
        private readonly CourseBusiness db_course;
        private readonly CandidateInfoBusiness db_candidate;
        private readonly MachTestQuesBankBusiness db_machtest;
        private readonly ComputerTestQuestionsBusiness db_computerTestQuestion;

        public ExamScoresController()
        {
            db_exam = new ExaminationBusiness();
            db_examScores = new ExamScoresBusiness();
            db_stuExam = new StudentExamBusiness();
            db_answerQuestion = new AnswerQuestionBusiness();
            db_student = new StudentInformationBusiness();
            db_course = new CourseBusiness();
            db_candidate = new CandidateInfoBusiness();
           db_machtest =  new MachTestQuesBankBusiness();
            db_computerTestQuestion = new ComputerTestQuestionsBusiness();
        }

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 加载这个学生的选择题解答题及机试题和总分
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadingScore()
        {
            return null;
        }
        /// <summary>
        /// 手动确认阅卷完毕
        /// </summary>
        /// <returns></returns>
        public ActionResult Endofmarking(int examid, int examroom)
        {
            AjaxResult result = new AjaxResult();

            try
            {

                var examroomobj = db_exam.AllExaminationRoom().Where(d => d.Classroom_Id == examroom && d.Examination == examid).FirstOrDefault();


                Base_UserModel user = Base_UserBusiness.GetCurrentUser();
                var markingArrange = db_examScores.AllMarkingArrange().Where(d => d.MarkingTeacher == user.EmpNumber && examid == d.ExamID && d.ExamRoom == examroomobj.ID).FirstOrDefault();

                markingArrange.IsFinsh = true;
                db_examScores.UpdateMarkingArrange(markingArrange);

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

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 学生个人成绩查询页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StuPersonalScores()
        {
            var studentNumber = SessionHelper.Session["studentnumber"].ToString();
            //学员的考试
            List<Examination> examlist = db_stuExam.StuExaminationEnd(studentNumber);

            ViewBag.ExamList = examlist;

            return View();

        }

        //获取学生考试成绩 
        [HttpPost]
        public ActionResult StuPersonalScores(int examid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var studentNumber = SessionHelper.Session["studentnumber"].ToString();
                var examscores = db_examScores.StuExamScores(examid, studentNumber);
                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = examscores;
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
        /// 阅卷页面1
        /// </summary>
        /// <returns></returns>
        public ActionResult Marking()
        {
            return View();
        }
        /// <summary>
        /// 选择xls上传分数
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DocumentUpload(HttpPostedFileBase excelfile)
        {
            Stream filestream = excelfile.InputStream;
            var textscore = db_examScores.ImportDataFormExcel(filestream, excelfile.ContentType);

            return Json(textscore, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 阅卷页面2
        /// </summary>
        /// <returns></returns>
        public ActionResult MarkingPapers()
        {
            return View();
        }
        [HttpPost]
        /// <summary>
        /// 阅卷数据
        /// </summary>
        /// <returns></returns>
        public ActionResult MarkeData()
       {
            AjaxResult result = new AjaxResult();

            try
            {

                //获取当前登录的老师

                Base_UserModel user = Base_UserBusiness.GetCurrentUser();

                var markingArrangeList = db_examScores.MarkingArrangeByEmpID(user.EmpNumber);

                List<object> objlist =  new List<object>();

                foreach (var item in markingArrangeList)
                {
                    var tempView = db_examScores.ConvertToMarkingArrangeView(item);

                    if (tempView != null)
                    {

                        var tempobj = new
                        {

                            markingView = tempView,
                            ExamIsEnd = db_exam.IsEnd(tempView.ExamID)
                        };


                        objlist.Add(tempobj);
                    }

                }

                result.Data = objlist;
                result.ErrorCode = 200;
                result.Msg = "成功";
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
        /// 考试是否结束
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamIsEnd(int examid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var exam = db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
                bool isEnd = db_exam.IsEnd(exam);

                result.ErrorCode = 200;
                result.Data = isEnd;
                result.Msg = "成功";

            }
            catch (Exception ex)
            {

                result.Msg = "失败";
                result.Data = null;
                result.ErrorCode = 500;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
           
            

        }
        /// <summary>
        /// 考试信息 考场信息 试卷数量 以阅卷数量 
        /// </summary>
        /// <param name="examid">考试ID</param>
        /// <param name="examroom">考场（教室编号）</param>
        /// <returns></returns>
        public ActionResult MarkeDatax(int examid, int examroom)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                //考试信息
                var exam = db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();

                //考场信息
                var examroomObj = db_exam.AllExaminationRoom().Where(d => d.Examination == examid && d.Classroom_Id == examroom).FirstOrDefault();

                //试卷数量  从成绩表中获取
                var scoreslist = db_examScores.AllExamScores().Where(d => d.Examination == examid).ToList();

                //考场人员分布
                var distributlist = db_exam.AllExamroomDistributed(examid).Where(d=>d.ExaminationRoom == examroomObj.ID).ToList();

                List<TestScore> scorelist = new List<TestScore>();

                foreach (var item in distributlist)
                {
                     var empobj1 =  scoreslist.Where(d => d.CandidateInfo == item.CandidateNumber).FirstOrDefault();

                    if (empobj1 != null)
                        scorelist.Add(empobj1);
                }

                //未阅卷数量
               var NoScoreslist = db_examScores.NoMarkeArrangeScore(examid, examroom);

                var obj = new {
                    exam = exam,
                    examroom= examroomObj,
                    total = scoreslist,
                    NoScoresCount = NoScoreslist


                };

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = obj;




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
        /// 弹出层解答题答卷页面显示学生的答卷
        /// </summary>
        /// <returns></returns>
        public ActionResult AnswerPage(int examid,string kaohao)
        {
            //var answerSheet = db_exam.AllCandidateInfo(examid).Where(d => d.StudentID == kaohao).FirstOrDefault().Paper;
            //List<object> objlist = new List<object>(); 
            //CloudstorageBusiness Bos = new CloudstorageBusiness();

            //var client = Bos.BosClient();
            //var filedata = client.GetObject("xinxihua", answerSheet);
            ////解答题答卷
            //MemoryStream stream = new MemoryStream();
            //filedata.ObjectContent.CopyTo(stream);

            //string SheetStr = Encoding.UTF8.GetString(stream.ReadToBytes());

            //var list = JsonConvert.DeserializeObject<List<AnswerSheetHelp>>(SheetStr);

            //foreach (var item in list)
            //{
            //    //根据问题ID 获取题目
            //    var question = db_answerQuestion.AllAnswerQuestion().Where(d => d.ID == item.questionid).FirstOrDefault();

            //    var obj = new
            //    {
            //        question = item,
            //        questionTitle = question,
            //    };

            //    objlist.Add(obj);
            //}
            // ViewBag.Data = objlist;
            var name = db_student.GetList().Where(d => d.StudentNumber == kaohao).FirstOrDefault().Name;
            ViewBag.Name = name;
            return View();
        }

        /// <summary>
        /// 提供阅卷的答卷数据
        /// </summary>
        /// <param name="examid">考试ID</param>
        /// <param name="examroom"></param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        public ActionResult MarkeAnswerSheetData(int examid, int examroom,int index ,string StudentNumber )
        {

            AjaxResult result = new AjaxResult();
            try
            {

                CandidateInfo candidateInfo = new CandidateInfo();
                ///考生
                var candidinfolist = db_examScores.CandidateinfosByExamroom(examid, examroom).OrderBy(d => d.CandidateNumber).ToList();
                if (StudentNumber != null)
                {
                    candidateInfo = candidinfolist.Where(d => d.StudentID == StudentNumber).FirstOrDefault();
                }
                else
                {
                    candidateInfo = candidinfolist.Skip(index - 1).Take(1).FirstOrDefault();
                }
                //·········获取这个考生的考卷···················

                // 1. 获取文件夹名称   文件夹名称为 学号_考试ID   解答题文件名称为 AnswerSheet文本文件   机试题文件名为 computerfielnaem.rar 压缩包

                //var stuDirName = candidinfo.StudentID + '_' + examid.ToString();

                //解答题答卷路径
                var answerSheet = candidateInfo.Paper;

                List<object> objlist = new List<object>();

                if (answerSheet == null)
                {
                    var obj = new
                    {
                        question = "",
                        questionTitle = "",
                        candidinfo = candidateInfo,
                        isEnd = ""
                    };

                    objlist.Add(obj);
                }
                else
                {
                    CloudstorageBusiness Bos = new CloudstorageBusiness();

                    var client = Bos.BosClient();

                    //Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + stuDirName + "AnswerSheet.txt");

                    //FileStream fileStream = new FileStream(answerSheet, FileMode.Open, FileAccess.Read);

                    var filedata = client.GetObject("xinxihua", answerSheet);


                    //解答题答卷
                    MemoryStream stream = new MemoryStream();
                    filedata.ObjectContent.CopyTo(stream);

                    string SheetStr =Encoding.UTF8.GetString(stream.ReadToBytes());

                    var list = JsonConvert.DeserializeObject<List<AnswerSheetHelp>>(SheetStr);

                    //判断是否为最后一个 
                    var IsEnd = index == candidinfolist.Count() ? true : false;


                    foreach (var item in list)
                    {
                        //根据问题ID 获取题目
                        var question = db_answerQuestion.AllAnswerQuestion().Where(d => d.ID == item.questionid).FirstOrDefault();

                        var obj = new
                        {
                            question = item,
                            questionTitle = question,
                            candidinfo = candidateInfo,
                            isEnd = IsEnd
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
        /// 下载模板
        /// </summary>
        /// <returns></returns>
        public ActionResult DownloadModule(int examid)
        {
            var xuesheng = db_candidate.GetList().Where(d => d.Examination == examid).ToList();
            List<string> mingzi = new List<string>();
            foreach (var item in xuesheng)
            {
                var name = db_student.GetList().Where(d => d.StudentNumber == item.StudentID).FirstOrDefault().Name;
                mingzi.Add(name);
            }
            var ajaxresult = new { data = mingzi };
            return Json(ajaxresult, JsonRequestBehavior.AllowGet);
            //string tr = Server.MapPath("/uploadXLSXfile/Template/Scoretemplate.xls");
            //FileStream stream = new FileStream(tr, FileMode.Open);
            //return File(stream, "application/octet-stream", Server.UrlEncode("Template.xls"));
        }
        /// <summary>
        /// 一键下载解答题
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public Document Exporter(int examid)
        {
            Document doc = new Document();

            //添加section
            Section s = doc.AddSection();

            //查询这堂考试学生的解答题地址
            var cand = db_candidate.GetList().Where(d => d.Examination == examid).ToList();
            
            Paragraph para2 = s.AddParagraph();

            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            foreach (var item in cand)
            {
                if (item.Paper == null)
                {
                    continue;
                }
                var filedata = client.GetObject("xinxihua",item.Paper);

                MemoryStream stream = new MemoryStream();
                filedata.ObjectContent.CopyTo(stream);

                string SheetStr = Encoding.UTF8.GetString(stream.ReadToBytes());

                var list = JsonConvert.DeserializeObject<List<AnswerSheetHelp>>(SheetStr);
               
                foreach (var itemes in list)
                {
                    //根据问题ID 获取题目
                    var name = db_student.GetList().Where(d => d.StudentNumber == item.StudentID).FirstOrDefault().Name;
                    var question = db_answerQuestion.AllAnswerQuestion().Where(d => d.ID == itemes.questionid).FirstOrDefault().Title;
                    var questiones = db_answerQuestion.AllAnswerQuestion().Where(d => d.ID == itemes.questionid).FirstOrDefault().ReferenceAnswer;
                    para2.AppendText(name + "---" + question + "(" + itemes.questionScores + "分)" +"正确答案:"+itemes.answer+ "\r\n" + itemes.answer + "\r\n");

                }

            }

            //创建段落样式2
            ParagraphStyle style2 = new ParagraphStyle(doc);
            style2.Name = "paraStyle";
            style2.CharacterFormat.FontName = "宋体";
            style2.CharacterFormat.FontSize = 8;

            doc.Styles.Add(style2);
            para2.ApplyStyle("paraStyle");

            //设置段落对齐方式
            para2.Format.HorizontalAlignment = Spire.Doc.Documents.HorizontalAlignment.Justify;

            //设置段落缩进
            para2.Format.FirstLineIndent = 30;
            para2.Format.AfterSpacing = 10;

            //保存文档
            doc.SaveToFile("C:\\解答题" + examid + ".docx", FileFormat.Docx2013);

            return doc;
        }
        /// <summary>
        /// 下载考生考试的试卷
        /// </summary>
        /// <param name="kaohao"></param>
        /// <param name="examid"></param>
        /// <returns></returns>
        public ActionResult xiazaijishi(string DownloadContent)
        {
            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            var obj = db_computerTestQuestion.AllComputerTestQuestion().Where(d => d.Title == DownloadContent).FirstOrDefault();

            var filename = Path.GetFileName(obj.SaveURL);

            //var path = Server.MapPath("/uploadXLSXfile/ComputerTestQuestionsWord/"+filename);

            var path = "/ExaminationSystem/ComputerTestQuestionsWord/" + filename;

            var fliedata = client.GetObject("xinxihua", path);

            //FileStream fileStream = new FileStream(path, FileMode.Open);

            return File(fliedata.ObjectContent, "application/octet-stream", Server.UrlEncode(filename));

        }
        /// <summary>
        /// 考生机试题答卷下载
        /// </summary>
        /// <param name="kaohao">学员考号</param>
        /// <returns></returns>       
        public ActionResult DownloadComputerSheet(string kaohao, int examid)
        {
            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            var candidateinfo = db_exam.AllCandidateInfo(examid).Where(d=>d.StudentID == kaohao).FirstOrDefault();

            //获取答卷路径

            if (candidateinfo.ComputerPaper == null || candidateinfo.ComputerPaper == "1" || candidateinfo.ComputerPaper == "")
            {
                return Json("404", JsonRequestBehavior.AllowGet);
            }

            //var computerPath = candidateinfo.ComputerPaper.Split(',')[1];
            var computerPath = candidateinfo.ComputerPaper;
            //FileStream fileStream = new FileStream(computerPath, FileMode.Open);

            var filedata = client.GetObject("xinxihua", computerPath);

            var filename = Path.GetFileName(computerPath);

            return File(filedata.ObjectContent, "application/octet-stream", Server.UrlEncode(filename));

        }

        public ActionResult checkHaveComputerPaper(string kaohao, int examid)
        {
            AjaxResult result = new AjaxResult();

            try
            {

                var candidateinfo = db_exam.AllCandidateInfo(examid).Where(d => d.CandidateNumber == kaohao).FirstOrDefault();

                //获取答卷路径

                if (candidateinfo.ComputerPaper == null || candidateinfo.ComputerPaper == "1" || candidateinfo.ComputerPaper == "")
                {
                    result.ErrorCode = 200;
                    result.Data = "0";
                    result.Msg = "成功";
                }
                else
                {
                    result.ErrorCode = 200;
                    result.Data = "1";
                    result.Msg = "成功";
                }
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Data = "0";
                result.Msg = "失败";
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 阅卷提交分数
        /// </summary>
        /// <returns></returns>
        public ActionResult CommitScores(int examid, string StuExamNumber, string answerScores, string computerScores, string remark)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                var candiInfo = db_candidate.GetList().Where(d => d.StudentID == StuExamNumber && d.Examination == examid).FirstOrDefault().CandidateNumber;
                var score = db_examScores.AllExamScores().Where(d => d.Examination == examid && d.CandidateInfo == candiInfo).FirstOrDefault();
                var candiInfoes = db_candidate.GetList().Where(d => d.StudentID == StuExamNumber && d.Examination == examid).FirstOrDefault();
                //修改 解答题分数 机试题分数
                if (answerScores == "")

                    score.OnBoard = null;

                else

                    score.OnBoard = float.Parse(computerScores);

                if (answerScores == "")
                    score.TextQuestionScore = null;
                else
                    score.TextQuestionScore = float.Parse(answerScores);


                Base_UserModel user = Base_UserBusiness.GetCurrentUser();

                TeacherBusiness teacherdb = new TeacherBusiness();


                score.Reviewer = teacherdb.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault().TeacherID;
                score.CreateTime = DateTime.Now;
                score.Remark = remark;
                candiInfoes.IsReExam = true;
                db_candidate.Update(candiInfoes);
                db_examScores.Update(score);

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = score;
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
        /// 获取学员成绩
        /// </summary>
        /// <param name="examid">考试ID</param>
        /// <param name="StuExamNumber">考号</param>
        /// <returns></returns>
        public ActionResult StuScores(int examid, string StuExamNumber)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                var candidInfo = db_exam.AllCandidateInfo(examid).Where(d => d.StudentID == StuExamNumber).FirstOrDefault();

                var testscore = db_examScores.StuExamScores(examid, candidInfo.StudentID);

                result.Data = testscore;
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
        /// 获取阅卷名单
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="examroom"></param>
        /// <returns></returns>
        public ActionResult MakeStuData(int examid, int examroom)
        {


            AjaxResult result = new AjaxResult();

            try
            {
                //首先考场考生
                var tempstulist = db_examScores.CandidateinfosByExamroom(examid, examroom);


                List<object> stulist = new List<object>();
                //转换为学员
                foreach (var item in tempstulist)
                {
                    var tempstu = db_student.GetList().Where(d => d.StudentNumber == item.StudentID).FirstOrDefault();

                    //查看这个学员的成绩是否已经被录入

                    var stuscore = db_examScores.StuExamScores(examid, tempstu.StudentNumber);

                    var IsMark = true;

                    if (stuscore.TextQuestionScore == null || stuscore.OnBoard == null)
                    {
                        IsMark = false;
                    }

                    var obj = new
                    {

                        student = tempstu,
                        IsMark = IsMark


                    };
                    stulist.Add(obj);
                }

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = stulist;
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
        /// 是否完成了阅卷
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="examroom">教室ID</param>
        /// <returns></returns>
        public ActionResult IsFinshMake(int examid, int examroom)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                Base_UserModel user = Base_UserBusiness.GetCurrentUser();
                bool b =  db_examScores.IsFinshMarking(user.EmpNumber, examid, examroom);

                if (b)
                {
                    result.ErrorCode = 200;
                    result.Msg = "成功";
                    result.Data = "1";
                }
                else
                {
                    result.ErrorCode = 200;
                    result.Msg = "成功";
                    result.Data = "0";
                }

               
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = "0";
            }

            return Json(result, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 完成阅卷 
        /// </summary>
        /// <returns></returns>
        public ActionResult FinshMake(int examid, int examroom)
        {
            AjaxResult result = new AjaxResult();

            try
            {

                var examroomobj = db_exam.AllExaminationRoom().Where(d => d.Classroom_Id == examroom && d.Examination == examid).FirstOrDefault();


                Base_UserModel user = Base_UserBusiness.GetCurrentUser();
                var markingArrange = db_examScores.AllMarkingArrange().Where(d => d.MarkingTeacher == user.EmpNumber && examid == d.ExamID && d.ExamRoom == examroomobj.ID).FirstOrDefault();

                markingArrange.IsFinsh = true;
                db_examScores.UpdateMarkingArrange(markingArrange);

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

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 考试成绩查询
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamScoreSearch()
        {
            //获取所有考试
            //var allExam = db_exam.AllExamination();

            //ViewBag.Examlist = allExam;

            //获取这堂考试的阶段


            return View();
        }
        /// <summary>
        /// 计算考试合格率
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExamPassRate(string Exam, string FenShu)
        {
            AjaxResult result = new AjaxResult();
            string[] ary1 = FenShu.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);
            try
            {
                //获取参加这场考试的重修人员
                var student = db_student.GetIQueryable().Where(d=>d.State == 2).FirstOrDefault().StudentNumber;
                //List<StudentExamScoreView> scorelist = new List<StudentExamScoreView>();
                var list1 = new List<TestScore>();
                //查出这场考试的所有考生的分数
                list1 = db_examScores.GetIQueryable().ToList().Where(d => d.Examination == int.Parse(Exam)).ToList();
                var RenShu = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if (Convert.ToInt32(Convert.ToDouble(list[i])) >= 60)
                    {
                        RenShu++;
                    }
                }
                 var HeGe = ((decimal)RenShu / (decimal)list.Count() * 100).ToString();
                //var HeGes = Math.Round(HeGe, 2);
                var HeGes = HeGe.Substring(0, 4).ToString() + "%";
               // var HeGe = 22.00 / 31.00;
                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = HeGes;
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
        /// 学生名单数据, string classiD
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentList(int examid)
        {
            List<StudentExamView> scorelist = new List<StudentExamView>();
            List<CandidateInfo> multipleChoicelist = db_candidate.GetList().Where(d =>d.Examination==examid).ToList();
            
            for (int i = 0; i < multipleChoicelist.Count; i++)
            {
                StudentExamView examView = new StudentExamView();
                examView.StudentID = multipleChoicelist[i].StudentID;
                examView.StudentName = db_student.GetEntity(multipleChoicelist[i].StudentID).Name;
                if (multipleChoicelist[i].DownloadContent != null)
                {
                    var jishi = multipleChoicelist[i].DownloadContent.ToInt();
                    examView.DownloadContent = db_machtest.GetList().Where(d => d.ID == jishi).FirstOrDefault().Title;
                }
                else if(multipleChoicelist[i].DownloadContent == null)
                {
                    examView.DownloadContent = "没有下载";
                }
                examView.IsReExam = multipleChoicelist[i].IsReExam;
                examView.Paper = multipleChoicelist[i].Paper;
                examView.ComputerPaper = multipleChoicelist[i].ComputerPaper;
                scorelist.Add(examView);
            }
            
            var obj = new
            {

                code = 0,
                msg = "",
                count = scorelist.Count,
                data = scorelist


            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查看考试数据详情
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="examid"></param>
        /// <param name="classiD"></param>
        /// <returns></returns>
        public ActionResult ExamScoreDataes(int page, int limit, int examid)
        {
            List<StudentExamView> scorelist = new List<StudentExamView>();
            List<CandidateInfo> multipleChoicelist = db_candidate.GetList().Where(d => d.Examination == examid).ToList();

            for (int i = 0; i < multipleChoicelist.Count; i++)
            {
                StudentExamView examView = new StudentExamView();
                examView.StudentID = multipleChoicelist[i].StudentID;
                examView.StudentName = db_student.GetEntity(multipleChoicelist[i].StudentID).Name;
                examView.PaperTime = multipleChoicelist[i].PaperTime;
                examView.ComputerPaperTime = multipleChoicelist[i].ComputerPaperTime;
                scorelist.Add(examView);
            }

            var obj = new
            {

                code = 0,
                msg = "",
                count = scorelist.Count,
                data = scorelist


            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 成绩数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamScoreData(int page, int limit, string examid, string classiD)
        {
            List<StudentExamScoreView> scorelist = new List<StudentExamScoreView>();
            var list1 = new List<TestScore>();

            if (classiD == "0")
            {
                list1 = db_examScores.GetIQueryable().ToList().Where(d => d.Examination == int.Parse(examid)).ToList();
            }
            else
            {
                //帅选班级
               var templist = db_examScores.GetIQueryable().ToList().Where(d => d.Examination == int.Parse(examid)).ToList();

                foreach (var item in templist)
                {
                    var tempobj1 = db_exam.AllCandidateInfo(int.Parse(examid)).Where(d => d.CandidateNumber == item.CandidateInfo).FirstOrDefault();

                    if (tempobj1 != null)
                    {
                        if (tempobj1.ClassId == int.Parse(classiD))
                        {
                            list1.Add(item);
                        }
                    }

                }
            }

            var skiplist = list1.ToList();

            foreach (var item in skiplist)
            {
                var temp = db_examScores.ConvertToStudentExamScoreView(item);

                if (temp != null)
                {
                    scorelist.Add(temp);
                }
            }

            var obj = new {

                code = 0,
                msg = "",
                count = list1.Count,
                data = scorelist

            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 获取这个时间段参加考试的考试
        /// </summary>
        /// <returns>riqi</returns>日期
        public ActionResult ExamJoinClass(string riqi)
        {
            AjaxResult result = new AjaxResult();
            DateTime dt = DateTime.Parse(riqi);
            string yys = dt.Year.ToString();
            string mms = dt.Month.ToString();
            string nianyue = yys + "-" + mms;
            List<MyExamCurren> mylist = new List<MyExamCurren>();
            MyExamCurren mydata = new MyExamCurren();
            try
            {
               
                var exam = db_exam.GetList().ToList();
                
                foreach (Examination item in exam)
                {
                   
                   int year= item.BeginDate.Year;
                   int month = item.BeginDate.Month;
                   XmlElement xmlelm = db_exam.ExamCourseConfigRead(item.ID);
                   int courseid = int.Parse(xmlelm.FirstChild.Attributes["id"].Value);
                    string mm = year + "-" + month;
                    if (courseid == 0)
                    {
                        if (nianyue == mm)
                        {
                            
                            mydata.CurreName = "升学";
                            mydata.Title = item.Title;
                            mydata.ID = item.ID;
                            mylist.Add(mydata);
                        }
                    }
                    else
                    {
                        var KeCheng = db_course.GetCurriculas().Where(d => d.CurriculumID == courseid).SingleOrDefault().CourseName;

                        if (nianyue == mm)
                        {
                         
                            mydata.CurreName = KeCheng;
                            mydata.Title = item.Title;
                            mydata.ID = item.ID;
                            mylist.Add(mydata);
                        }
                    }

                }
                result.ErrorCode = 200;
                result.Data = mylist;
                result.Msg = "";
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Data = null;
                result.Msg = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        ///  一键下载这堂考试的所有机试
        /// </summary>
        /// <returns></returns>
        public ActionResult OneClickDownload(int examid)
        {
            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();
            List<FileStreamResult> zhi = new List<FileStreamResult>() ;
            var candidateinfo = db_exam.AllCandidateInfo(examid).ToList();
                foreach (var item in candidateinfo)
                {
                    if (!(item.ComputerPaper == null || item.ComputerPaper == "1" || item.ComputerPaper == ""))
                    {
                        //var computerPath = candidateinfo.ComputerPaper.Split(',')[1];
                        var computerPath = item.ComputerPaper;
                        //FileStream fileStream = new FileStream(computerPath, FileMode.Open);

                        var filedata = client.GetObject("xinxihua", computerPath);

                        var filename = Path.GetFileName(computerPath);

                       //return File(filedata.ObjectContent, "application/octet-stream", Server.UrlEncode(filename));
                        zhi.Add(File(filedata.ObjectContent, "application/octet-stream", Server.UrlEncode(filename)));
                    }

                }

            var obj = new { data=zhi};
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
    }
}