﻿using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SiliconValley.InformationSystem.Web.Areas.ExaminationSystem.Controllers
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
   
    public class StudentExamSysController : Controller
    {

        private readonly ExaminationBusiness db_exam;

        private readonly StudentExamBusiness db_stuExam;

        private readonly ChoiceQuestionBusiness db_choiceQuestion;
        private readonly ExamScoresBusiness db_examScores;

        public StudentExamSysController()
        {
            db_exam = new ExaminationBusiness();
            db_stuExam = new StudentExamBusiness();
            db_choiceQuestion = new ChoiceQuestionBusiness();
            db_examScores = new ExamScoresBusiness();
        }
        // GET: ExaminationSystem/StudentExamSys
        public ActionResult StuExamIndex()
        {
            //获取学员最近的考试

          
            return View();
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
                var exam = db_stuExam.StudetnSoonExam("19081997072400002").OrderByDescending(d => d.BeginDate).FirstOrDefault();
                var examview = db_exam.ConvertToExaminationView(exam);

                result.Data = examview;
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
        /// 学员答题页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AnswerSheet(int examid)
        {
            var exam = db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
            var EXAMVIEW = db_exam.ConvertToExaminationView(exam);
            ViewBag.EXAMVIEW = EXAMVIEW;
            
            //学员进入答题

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


                List<ChoiceQuestionTableView> data = new List<ChoiceQuestionTableView>();
                //判断考试类型
                if (exam.ExamType == 1)
                {
                    data = db_stuExam.ProductChoiceQuestion(exam, 0);
                }

                if (exam.ExamType == 2)
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
                List<AnswerQuestionView> data = new List<AnswerQuestionView>();
                //判断考试类型
                if (exam.ExamType == 1)
                {
                    data = db_stuExam.productAnswerQuestion(exam, 0);
                }

                if (exam.ExamType == 2)
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

            //首先查看是否已经随机获取到了一个
            
            var candidateInfo =  db_exam.AllCandidateInfo(examid).Where(d=>d.StudentID == "19081997072400002").FirstOrDefault();
            ComputerTestQuestionsView computer = null;
            if (candidateInfo.ComputerPaper == null)
            {
                //第一次

                computer = db_stuExam.productComputerQuestion(exam);
                candidateInfo.ComputerPaper = computer.ID.ToString()+",";
                db_exam.UpdateCandidateInfo(candidateInfo);

            }

            var ar = candidateInfo.ComputerPaper.Split(',');

           var com = db_exam.AllComputerTestQuestion().Where(d => d.ID ==int.Parse( ar[0])).FirstOrDefault();


            var filename = Path.GetFileName(com.SaveURL);

            var path = Server.MapPath("/uploadXLSXfile/ComputerTestQuestionsWord/" + filename);

            FileStream fileStream = new FileStream(path, FileMode.Open);

            return File(fileStream, "application/octet-stream", Server.UrlEncode(filename));
            
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
                // 1.将解答题答案存入文件  2 将机试题文件放入AnswerSheet文件夹 3.修改数据库值（选择题分数，解答题答案路径，机试题路径）4.记录选择题分数

                //1 将解答题答案存入文件 首先新建学生答卷文件夹
                //名称规则 学号加上考试ID
                string direName = "19081997072400002_" + examid;
                DirectoryInfo directoryInfo = new DirectoryInfo(Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + direName));
                directoryInfo.Create();

                //写文件
                string answerfilename = "AnswerSheet.txt";
                FileInfo fileinfo = new FileInfo(Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + direName + "/" + answerfilename));
                var stream1 = fileinfo.CreateText();
                stream1.Write(AnswerCommit);
                stream1.Flush();
                stream1.Close();

                //2 将机试题保存到文件夹 
                string computerfielnaem = "computerfielnaem";
                var computerfile = Request.Files["rarfile"];

                //保存的机试题路径
                string computerUrl = "";
                if (computerfile != null)
                {
                    //获取文件拓展名称 
                    var exait = Path.GetExtension(computerfile.FileName);
                    computerUrl = Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + direName + "/" + computerfielnaem + exait);
                    computerfile.SaveAs(computerUrl);

                }

                //3.修改数据库值（选择题分数，解答题答案路径，机试题路径）
                db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
                var Candidateinfo = db_exam.AllCandidateInfo(examid).Where(d => d.Examination == examid && d.StudentID == "19081997072400002").FirstOrDefault();
                Candidateinfo.Paper = Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + direName + "/" + answerfilename);

                //获取需要替换的字符串路径
                var old = Candidateinfo.ComputerPaper.Substring(Candidateinfo.ComputerPaper.IndexOf(',') + 1);
                if (old.Length == 0)
                {
                    Candidateinfo.ComputerPaper = Candidateinfo.ComputerPaper + computerUrl;
                }
                else

                {
                    Candidateinfo.ComputerPaper = Candidateinfo.ComputerPaper.Replace(old, computerUrl);
                   
                }
                db_exam.UpdateCandidateInfo(Candidateinfo);

                //4.记录选择题分数

                //获取考生

               var stuScores = db_examScores.StuExamScores(examid, "19081997072400002");

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




    }
}