using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.ExaminationSystem.Controllers
{
   
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
    using SiliconValley.InformationSystem.Util;
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    /// <summary>
    /// 考试题库控制器
    /// </summary>
    /// 
    [CheckLogin]
    public class QuestionsBankController : Controller
    {

        private readonly ChoiceQuestionBusiness db_choiceQuestion;

        private readonly QuestionLevelBusiness db_questionLevel;

        private readonly TeacherBusiness db_teacher;

        private readonly AnswerQuestionBusiness db_answerQuestion;

        private readonly ComputerTestQuestionsBusiness db_computerTestQuestion;

        /// <summary>
        /// 构造函数
        /// </summary>
        public QuestionsBankController()
        {

            //业务类初始化
            db_choiceQuestion = new ChoiceQuestionBusiness();
            db_questionLevel = new QuestionLevelBusiness();
            db_teacher = new TeacherBusiness();
            db_answerQuestion = new AnswerQuestionBusiness();
            db_computerTestQuestion = new ComputerTestQuestionsBusiness();

        }


        // GET: ExaminationSystem/QuestionsBank
        public ActionResult QuestionsBankIndex()
        {
            GrandBusiness GrandBusiness = new GrandBusiness();
            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            ViewBag.grand = GrandBusiness.AllGrand();
            //StudentExamBusiness studentExamBusiness = new StudentExamBusiness();

            //ExaminationBusiness examinationBusiness = new ExaminationBusiness();

            //var e = examinationBusiness.GetList().Where(d => d.ID == 3012).FirstOrDefault();

            //studentExamBusiness.productAnswerQuestion(e, 10);

            return View();
        }
        /// <summary>
        /// 选择题去重分布视图
        /// </summary>
        /// <returns></returns>
        public ActionResult DuplicateRemovalXZ()
        {
            GrandBusiness GrandBusiness = new GrandBusiness();
            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            ViewBag.grand = GrandBusiness.AllGrand();
            return View();
        }
        /// <summary>
        /// 解答题去重分布视图
        /// </summary>
        /// <returns></returns>
        public ActionResult DuplicateRemoval()
        {
            GrandBusiness GrandBusiness = new GrandBusiness();
            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            ViewBag.grand = GrandBusiness.AllGrand();
            return View();
        }
        /// <summary>
        /// 批量修改选择题难度
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        public ActionResult NanDuXuanZe(string ids, int Level)
        {
            AjaxResult result = new AjaxResult();

            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);
            try
            {
                db_choiceQuestion.PiLiangXiuGaiND(list, Level);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
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
        /// 批量更改解答题难度
        /// </summary>
        /// <returns></returns>
        public ActionResult NanDuJieDa(string ids,int Level)
        {
            AjaxResult result = new AjaxResult();

            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);

            try
            {
                db_answerQuestion.PiliangXiuGaiND(list, Level);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
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
        /// 批量更改解答题课程
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeJieDa(string ids,int courseS)
        {
            AjaxResult result = new AjaxResult();

            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);

            try
            {
                db_answerQuestion.PiliangXiuGai(list, courseS);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
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
        /// 批量更改选择题课程
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeXuanZe(string ids,int courseS)
        {
            AjaxResult result = new AjaxResult();

            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);
            try
            {
                db_choiceQuestion.PiLiangXiuGai(list,courseS);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Data = null;
                result.Msg = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult XianZeXianShi(int page, int limit)
        {
            List<MultipleChoiceQuestion> resultlist = new List<MultipleChoiceQuestion>();
            List<ChoiceQuestionTableView> objList = new List<ChoiceQuestionTableView>();
            resultlist = db_choiceQuestion.AllChoiceQuestionData().ToList();
            List<string> listone = new List<string>();
            List<string> listtwo = new List<string>();
            for (int i = 0; i < resultlist.Count; i++)
            {
                if (listone.Contains(resultlist[i].Title))
                {
                    listtwo.Add(resultlist[i].Title);
                }
                else
                {
                    listone.Add(resultlist[i].Title);
                }
            }
            if (listtwo.Count > 0)
            {
                for (int i = 0; i <1; i++)
                {
                     resultlist = db_choiceQuestion.AllChoiceQuestionData().Where(d => d.Title == listtwo[i]).ToList();

                    foreach (var item in resultlist)
                    {
                        var tempobj = db_choiceQuestion.ConvertToChoiceQuestionTableView(item);
                        objList.Add(tempobj);
                    }
                }
            }
            var obj = new
            {
                code = 0,
                msg = "查询成功",
                count = objList.Count,
                data = objList.Skip((page - 1) * limit).Take(limit)
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult JieDaXianShi(int page, int limit)
        {
            List<AnswerQuestionBank> resultlist = new List<AnswerQuestionBank>();
            List<AnswerQuestionView> objList = new List<AnswerQuestionView>();
            resultlist = db_answerQuestion.AllAnswerQuestion().ToList();
            List<string> listone = new List<string>();
            List<string> listtwo = new List<string>();
            //循环找出重复题
            for (int i = 0; i < resultlist.Count; i++)
            {
                if (listone.Contains(resultlist[i].Title))
                {
                    listtwo.Add(resultlist[i].Title);
                }
                else
                {
                    listone.Add(resultlist[i].Title);
                }
            }
            //把重复题查询出来，保存
            if (listtwo.Count > 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    resultlist = db_answerQuestion.AllAnswerQuestion().Where(d => d.Title == listtwo[i]).ToList();
                    foreach (var item in resultlist)
                    {
                        var tempobj = db_answerQuestion.ConvertToAnswerQuestionView(item, true);
                        objList.Add(tempobj);
                    }
                }
            }
            var obj = new
            {
                code = 0,
                msg = "查询相同成功!",
                count = objList.Count,
                data = objList.Skip((page - 1) * limit).Take(limit)
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 解答题查询相同题 
        /// </summary>
        /// <returns></returns>
        public ActionResult JieDaSelect(int courseS, int page, int limit)
        {
            AjaxResult result = new AjaxResult();
            List<AnswerQuestionBank> resultlist = new List<AnswerQuestionBank>();
            List<AnswerQuestionView> objList = new List<AnswerQuestionView>();
            resultlist = db_answerQuestion.AllAnswerQuestion().ToList();
            List<string> listone = new List<string>();
            List<string> listtwo = new List<string>();
            //循环找出重复题
            for (int i = 0; i < resultlist.Count; i++)
            {
                if (listone.Contains(resultlist[i].Title))
                {
                    listtwo.Add(resultlist[i].Title);
                }
                else {
                    listone.Add(resultlist[i].Title);
                }
            }
            //把重复题查询出来，保存
            if (listtwo.Count>0) {
                for (int i = 0; i < 1; i++)
                {
                    resultlist = db_answerQuestion.AllAnswerQuestion().Where(d => d.Title == listtwo[i] && d.Course == courseS).ToList();
                    foreach (var item in resultlist)
                    {
                        var tempobj = db_answerQuestion.ConvertToAnswerQuestionView(item, true);
                        objList.Add(tempobj);
                    }
                }
            }
            var obj = new
            {
                code = 0,
                msg = "查询相同成功!",
                count = objList.Count,
                data = objList.Skip((page - 1) * limit).Take(limit)
            };           
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 解答题去重删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult JieDaDelete(string ids)
        {
            AjaxResult result = new AjaxResult();
            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);

            try
            {
                db_answerQuestion.PiLiangShanChu(list);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
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
        /// 选择题的去重删除
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult XianZeDelete(string ids)
        {
            AjaxResult result = new AjaxResult();
            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);
            try
            {
                db_choiceQuestion.PiLiangShanChu(list);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
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
        /// 选择题查询相同题
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult XianZeSelect(int courseS, int page, int limit)
        {
            List<MultipleChoiceQuestion> resultlist = new List<MultipleChoiceQuestion>();
            List<ChoiceQuestionTableView> objList = new List<ChoiceQuestionTableView>();
            resultlist = db_choiceQuestion.AllChoiceQuestionData().ToList();
            List<string> listone = new List<string>();
            List<string> listtwo = new List<string>();
            for (int i = 0; i < resultlist.Count; i++)
            {
                if (listone.Contains(resultlist[i].Title))
                {
                    listtwo.Add(resultlist[i].Title);
                }
                else
                {
                    listone.Add(resultlist[i].Title);
                }
            }
            if (listtwo.Count > 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    resultlist = db_choiceQuestion.AllChoiceQuestionData().Where(d => d.Title == listtwo[i]&&d.Course== courseS).ToList();

                    foreach (var item in resultlist)
                    {
                        var tempobj = db_choiceQuestion.ConvertToChoiceQuestionTableView(item);
                        objList.Add(tempobj);
                    }
                }
            }
            var obj = new
            {
                code = 0,
                msg = "查询成功",
                count = objList.Count,
                data = objList.Skip((page - 1) * limit).Take(limit)
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 选择题题库页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ChoiceQuestionIndex()
        {
            GrandBusiness GrandBusiness = new GrandBusiness();
            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            ViewBag.grand = GrandBusiness.AllGrand();
            return View();

        }

        /// <summary>
        /// 批量录入
        /// </summary>
        /// <param name="excelFile"></param>
        /// <returns></returns>
        public ActionResult ChoiceQuestionBatchEntry(string QuestionType)
        {
            GrandBusiness grandBusiness = new GrandBusiness();
            //提供专业数据
            ViewBag.Grand = grandBusiness.AllGrand();

            ViewBag.QuestionType = QuestionType;

            return View();

        }
        /// <summary>
        /// 下载选择题模板
        /// </summary>
        /// <returns></returns>
        public ActionResult DownLoadChoiceQuestionTemplate(string templateType, int init = 1)
        {

            string path = "";
            

            if (templateType == "选择题")
            {
                path = Server.MapPath("/uploadXLSXfile/ExamtionQuestionBankTemplate/ChoiceQuestionTemplate.xls");

                //初始化文件
                db_choiceQuestion.InitQuestionTemplate(init, path);
            }

            if (templateType == "解答题")
            {
                path = Server.MapPath("/uploadXLSXfile/ExamtionQuestionBankTemplate/ClearQuestionTemplate.xls");

                db_answerQuestion.InitQuestionTemplate(init, path);
            }

            FileStream filestream = new FileStream(path, FileMode.Open, FileAccess.Read);

            return File(filestream, "application/vnd.ms-excel", "题库模板.xls");
     

        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="excelfile"></param>
       /// <param name="course"></param>
       /// <param name="QuestionType">['选择题、解答题']</param>
       /// <returns></returns>
        [HttpPost]

        public ActionResult ChoiceQuestionBatchEntry(HttpPostedFileBase excelfile, string course, string QuestionType, string grand)
        {

   
            AjaxResult result = new AjaxResult();

            try
            {
                

                Stream filestream = excelfile.InputStream;

                int count = 0;

                if (QuestionType == "选择题")
                {
                    count = InsertChoiceQuestion(filestream);
                }

                else
                {
                    count = InsertAnswerQuestion(filestream);
                }
                

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = count;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = "0";
            }


            #region 插入选择题
            int InsertChoiceQuestion(Stream stream)
            {
                List<MultipleChoiceQuestion> list = db_choiceQuestion.ReadQuestionForExcel(stream, excelfile.ContentType);

                Base_UserModel user = Base_UserBusiness.GetCurrentUser();

                var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault();

                foreach (var item in list)
                {
                    item.Course = int.Parse(course);
                    item.CreateTime = DateTime.Now;
                    item.IsUsing = true;
                    item.Grand = int.Parse(grand);
                    item.Proposition = teacher.TeacherID;
                }

                db_choiceQuestion.Insert(list);

                return list.Count;
            }
            #endregion

            #region 插入解答题
            int InsertAnswerQuestion(Stream stream)
            {
                AnswerQuestionBusiness tempdb_answer = new AnswerQuestionBusiness();

                List<AnswerQuestionBank> list = tempdb_answer.ReadQuestionForExcel(stream, excelfile.ContentType);
                Base_UserModel user = Base_UserBusiness.GetCurrentUser();


                var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault();

                foreach (var item in list)
                {
                    item.Course = int.Parse(course);
                    item.PropositionDate = DateTime.Now;
                    item.IsUsing = true;
                    item.Grand = int.Parse(grand);
                    item.Proposition = teacher.TeacherID;
                    
                }

                tempdb_answer.Insert(list);

                return list.Count;
            }
            #endregion



            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取选择题数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ChoiceQuestionTableData(int limit, int page)
        {

            List<MultipleChoiceQuestion> multipleChoicelist = db_choiceQuestion.AllChoiceQuestionData().OrderByDescending(d=>d.CreateTime).ToList();

            List<MultipleChoiceQuestion> skiplist = multipleChoicelist.Skip((page - 1) * limit).Take(limit).ToList();

            //转换对象

            List<ChoiceQuestionTableView> resultlist = new List<ChoiceQuestionTableView>();

            foreach (var item in skiplist)
            {
                var tempobj = db_choiceQuestion.ConvertToChoiceQuestionTableView(item);

                resultlist.Add(tempobj);
            }
 
            var obj = new {

                code = 0,
                msg = "",
                count = multipleChoicelist.Count,
                data = resultlist


            };

            return Json(obj, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// 命题视图
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult Proposition()
        {
            GrandBusiness grandBusiness = new GrandBusiness();

            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            //提供阶段数据
            ViewBag.Grand = grandBusiness.AllGrand();

            return View();
        }
         

        public ActionResult ModifyChoice(int choiceId)
        {
            GrandBusiness grandBusiness = new GrandBusiness();

            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            //提供阶段数据
            ViewBag.Grand = grandBusiness.AllGrand();

            var question = db_choiceQuestion.GetEntity(choiceId);

            CourseBusiness dbcourse = new CourseBusiness();
            var course = dbcourse.GetEntity(question.Course);
            ViewBag.course = course;
            return View(question);
        }
        [HttpPost]
        public ActionResult ModifyChoice(MultipleChoiceQuestion question)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                MultipleChoiceQuestion newquestion = db_choiceQuestion.GetEntity(question.Id);
                newquestion.Answer = question.Answer;
                newquestion.Course = question.Course;
                newquestion.Grand = question.Grand;
                newquestion.IsRadio = question.IsRadio;
                newquestion.Level = question.Level;
                newquestion.OptionA = question.OptionA;
                newquestion.OptionB = question.OptionB;
                newquestion.OptionC = question.OptionC;
                newquestion.OptionD = question.OptionD;

                newquestion.Remark = question.OptionD;
                newquestion.Title = question.Title;

                db_choiceQuestion.Update(newquestion);
                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = newquestion;

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
        /// 获取课程
        /// </summary>
        /// <param name="majorid">专业</param>
        /// <returns></returns>
        public ActionResult CourseData(int grandid)
        {
            CourseBusiness courseBusiness = new CourseBusiness();

            List<Curriculum> curricullist = new List<Curriculum>();

           var list = courseBusiness.GetCurriculas().Where(d => d.Grand_Id == grandid);

            if (list != null)
            {
                curricullist.AddRange(list);
            }
            // 转化类型

            List<CourseView> resultlist = new List<CourseView>();

            foreach (var item in curricullist)
            {
                try
                {
                    var tempobj = courseBusiness.ToCourseView(item);

                    if (tempobj != null)
                    {
                        resultlist.Add(tempobj);
                    }
                }
                catch (Exception)
                {

                    
                }
                
            }

            return Json(resultlist, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CourseDataBYGrind(int grandid)
        {
            CourseBusiness courseBusiness = new CourseBusiness();

            List<Curriculum> curricullist = new List<Curriculum>();

            var list = courseBusiness.GetCurriculas().Where(d => d.Grand_Id == grandid);

            if (list != null)
            {
                curricullist.AddRange(list);
            }
            // 转化类型

            List<CourseView> resultlist = new List<CourseView>();

            foreach (var item in curricullist)
            {
                try
                {
                    var tempobj = courseBusiness.ToCourseView(item);

                    if (tempobj != null)
                    {
                        resultlist.Add(tempobj);
                    }
                }
                catch (Exception)
                {


                }

            }

            return Json(resultlist, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 添加选择题题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddProposition(MultipleChoiceQuestion multipleChoice)
        {
            AjaxResult result = new AjaxResult();

            //获取当前登录的老师

            Base_UserModel user = Base_UserBusiness.GetCurrentUser();

            var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault();


            multipleChoice.CreateTime = DateTime.Now;
            multipleChoice.IsUsing = true;
            multipleChoice.Proposition = teacher.TeacherID;


            try
            {
                db_choiceQuestion.Insert(multipleChoice);

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = ex.Message;
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet); ;
        }

        /// <summary>
        /// 选择题详细
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DetailChoiceQuestion(int id)
        {
            var ChoiceQuestion = db_choiceQuestion.AllChoiceQuestionData().Where(d => d.Id == id).FirstOrDefault();

            var obj  = db_choiceQuestion.ConvertToChoiceQuestionTableView(ChoiceQuestion);

            return View(obj);
        }


        /// <summary>
        /// 禁用或启用选择题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChoiceQuestionIsUsing(string  ids)
        {

            AjaxResult result = new AjaxResult();

            string [] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);
            try
            {
                db_choiceQuestion.DisableOrEnable(list.ToArray());

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
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
        /// 删除选择题目
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public ActionResult DeleteChoiceQuestion(int id)
        {

           var result = db_choiceQuestion.RemoveChoiceQuestion(id);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 查询选择题目
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult Search(string title, int course,int limit, int page)
        {
            List<MultipleChoiceQuestion> resultlist = new List<MultipleChoiceQuestion>();

            if (course == 0 && title=="")
            {
                resultlist = db_choiceQuestion.AllChoiceQuestionData().ToList();
            }
            else
            if (course != 0 && title == null)
            {
                resultlist = db_choiceQuestion.AllChoiceQuestionData().Where(d => d.Course == course).ToList();
            }
            else
            if (course != 0 && title =="")
            {
                resultlist = db_choiceQuestion.AllChoiceQuestionData().Where(d => d.Course == course ).ToList();
            }
            else
            if (title != "" && course != 0)
            {
                resultlist = db_choiceQuestion.AllChoiceQuestionData().Where(d => d.Course == course && d.Title.Contains(title)).ToList();
            }
            else
            if (title != "" && course == 0)
            {
                resultlist = db_choiceQuestion.AllChoiceQuestionData().Where(d=>d.Title.Contains(title)).ToList();
            }

            


            //转换对象

            List<ChoiceQuestionTableView> returnlist = new List<ChoiceQuestionTableView>();

            foreach (var item in resultlist)
            {
                var tempobj = db_choiceQuestion.ConvertToChoiceQuestionTableView(item);

                returnlist.Add(tempobj);
            }


            var obj = new {

                code = 0,
                msg = "",
                count = returnlist.Count,
                data = returnlist.Skip((page - 1) * limit).Take(limit)
            };

            return Json(obj, JsonRequestBehavior.AllowGet);

            

        }



        //////////////////////////////////////////////////////////////////////////////解答题//////////////////////////////////////////////////////////////////



        /// <summary>
        /// 解答题题库页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearlyQuestionIndex()
        {
            GrandBusiness GrandBusiness = new GrandBusiness();
            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            ViewBag.grand = GrandBusiness.AllGrand();


            return View();
        }

        public ActionResult ClearlyQuestionBatchEntry()
        {
            SpecialtyBusiness specialtyBusiness = new SpecialtyBusiness();
            //提供专业数据
            ViewBag.Major = specialtyBusiness.GetSpecialties();
            return View();

        }
        /// <summary>
        /// 解答题表格数据
        /// </summary>
        /// <returns></returns>
        public ActionResult AnswerQuestionTableData(int limit, int page)
        {
            List<AnswerQuestionBank> list = db_answerQuestion.AllAnswerQuestion().OrderByDescending(d=>d.PropositionDate).ToList();

            List<AnswerQuestionBank> skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();

            List<AnswerQuestionView> resultlist = new List<AnswerQuestionView>();

            foreach (var item in skiplist)
            {
               var tempobj = db_answerQuestion.ConvertToAnswerQuestionView(item, true);

                resultlist.Add(tempobj);
            }

            var obj = new {
                code = 0,
                msg = "",
                count = list.Count,
                data=resultlist

            };

            return Json(obj, JsonRequestBehavior.AllowGet);



        }


        /// <summary>
        /// 命题-解答题页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PropositionAnswerQuestion()
        {
            GrandBusiness grandBusiness = new GrandBusiness();

            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            //提供专业数据
            ViewBag.Grand = grandBusiness.AllGrand();
            return View();
        }

        /// <summary>
        /// 编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult ModifyAnswerQuestion(string questionid)
        {
            GrandBusiness grandBusiness = new GrandBusiness();

            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            //提供阶段数据
            ViewBag.Grand = grandBusiness.AllGrand();

            var question = db_answerQuestion.GetEntity(int.Parse(questionid));

            CourseBusiness dbcourse = new CourseBusiness();
            var course = dbcourse.GetEntity(question.Course);

            ViewBag.course = course;
            return View(question);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ModifyAnswerQuestion(AnswerQuestionBank In_question)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var question = db_answerQuestion.GetEntity(In_question.ID);
                question.Course = In_question.Course;
                question.Grand = In_question.Grand;
                question.Level = In_question.Level;
                question.ReferenceAnswer = In_question.ReferenceAnswer;
                question.Remark = In_question.Remark;
                question.Title = In_question.Title;

                db_answerQuestion.Update(question);

                result.ErrorCode = 200;
                result.Msg = "成功"; 
                result.Data = question;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            return Json(result,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 命题-解答题
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PropositionAnswerQuestion(AnswerQuestionBank answerQuestionBank)
        {
            AjaxResult result = new AjaxResult();

            answerQuestionBank.IsUsing = true;

            Base_UserModel user = Base_UserBusiness.GetCurrentUser();

            var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault();


            answerQuestionBank.Proposition = teacher.TeacherID;
            answerQuestionBank.PropositionDate = DateTime.Now;


            try
            {
                db_answerQuestion.Insert(answerQuestionBank);

                result.Data = null;
                result.ErrorCode = 200;
                result.Msg = "成功";
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.ErrorCode = 500;
                result.Msg = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);



        }


        /// <summary>
        /// 启用或者禁用选择题题目
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult AnswerQuestionIsUsing(string ids)
        {
            AjaxResult result = new AjaxResult();

            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);

            try
            {
                db_answerQuestion.DisableOrEnable(list);

                result.ErrorCode = 200;
                result.Data = null;
                result.Msg = "成功";
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
        /// 选择题题目详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DetailAnswerQuestion(int id)
        {


           var tempobj = db_answerQuestion.AllAnswerQuestion().Where(d => d.ID == id).FirstOrDefault();

            var obj = db_answerQuestion.ConvertToAnswerQuestionView(tempobj, true);


            return View(obj);


        }


        /// <summary>
        /// 删除解答题题目
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult DeleteAnswerQuestion(int id)
        {
           var result = db_answerQuestion.Remove(id);

            return Json(result);
        }



        /// <summary>
        /// 查询解答题题目
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchAnswerQuestion(string Title, int course, int page, int limit)
        {
            List<AnswerQuestionBank> resultlist = new List<AnswerQuestionBank>();
            if (course == 0 && Title == "")
            {
                resultlist = db_answerQuestion.AllAnswerQuestion().ToList();
            }
            else
            if (course != 0 && Title == null)
            {
                resultlist = db_answerQuestion.AllAnswerQuestion().Where(d => d.Course == course).ToList();
            }
            else
            if (course != 0 && Title == "")
            {
                resultlist = db_answerQuestion.AllAnswerQuestion().Where(d => d.Course == course).ToList();
            }
            else
            if (Title != "" && course != 0)
            {
                resultlist = db_answerQuestion.AllAnswerQuestion().Where(d => d.Course == course && d.Title.Contains(Title)).ToList();
            }
            else
            if (Title != "" && course == 0)
            {
                resultlist = db_answerQuestion.AllAnswerQuestion().Where(d => d.Title.Contains(Title)).ToList();
            }

            //转换对象

            List<AnswerQuestionView> returnlist = new List<AnswerQuestionView>();
            //转换对象

            foreach (var item in resultlist)
            {

               var tempobj = db_answerQuestion.ConvertToAnswerQuestionView(item, true);
                returnlist.Add(tempobj);

            }

           var obj = new
            {

                code = 0,
                msg = "",
                count = returnlist.Count,
                data = returnlist.Skip((page - 1) * limit).Take(limit)
            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }


        //////////////////////////////////////////////////////////////机试题题库////////////////////////////////////////////////////////////////////////////////////////////////



            /// <summary>
            /// 机试题题库主页
            /// </summary>
            /// <returns></returns>
        public ActionResult ComputerTestQuestionsIndex()
        {
            GrandBusiness GrandBusiness = new GrandBusiness();

            ViewBag.grand = GrandBusiness.AllGrand();

            return View();
        }


        /// <summary>
        /// 机试题数据表格数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ComputerTestQuestionTableData(int page, int limit)
        {
       
            List<MachTestQuesBank> list = db_computerTestQuestion.AllComputerTestQuestion().OrderByDescending(d=>d.CreateDate).ToList();

            List<MachTestQuesBank> skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();

            List<ComputerTestQuestionsView> resultlist = new List<ComputerTestQuestionsView>();

            foreach (var item in skiplist)
            {

                  var tempobj = db_computerTestQuestion.ConvertToComputerTestQuestionsView(item, true);

                resultlist.Add(tempobj);

            }

            var ojb = new {

                code=0,
                msg="",
                count= list.Count,
                data=resultlist

            };

            return Json(ojb, JsonRequestBehavior.AllowGet);



        }


        /// <summary>
        /// 添加机试题视图
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult PropositionComputerTestQuestion()
        {
            //提供难度级别数据

            GrandBusiness grandBusiness = new GrandBusiness();

            //提供难度级别数据

            ViewBag.QuestionLevel = db_questionLevel.AllQuestionLevel();

            //提供阶段数据
            ViewBag.Grand = grandBusiness.AllGrand();

            return View();
        }


        /// <summary>
        /// 机试题提交
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult PropositionComputerTestQuestion(MachTestQuesBank machTestQuesBank)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                CloudstorageBusiness Bos = new CloudstorageBusiness();

                var client = Bos.BosClient();

                //判断是否重名
                var isExit = db_computerTestQuestion.AllComputerTestQuestion().Where(d => d.Title == machTestQuesBank.Title).FirstOrDefault();

                if (isExit != null)
                {
                    result.ErrorCode = 444;

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var word = Request.Files[0];

                string filename = word.FileName;
                string Extension = Path.GetExtension(filename);

                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);

                 var Utc = Convert.ToInt64(ts.TotalSeconds).ToString();

                string newfilename = Utc + Extension;

                //string path = Server.MapPath("~/uploadXLSXfile/ComputerTestQuestionsWord/" + newfilename);

                string path = $"/ExaminationSystem/ComputerTestQuestionsWord/{newfilename}";

                machTestQuesBank.CreateDate = DateTime.Now;
                machTestQuesBank.IsUsing = true;
                Base_UserModel user = Base_UserBusiness.GetCurrentUser();

                var teacher = db_teacher.GetTeachers().Where(d => d.EmployeeId == user.EmpNumber).FirstOrDefault();

                machTestQuesBank.Proposition = teacher.TeacherID;
                machTestQuesBank.SaveURL = path;

                db_computerTestQuestion.Insert(machTestQuesBank);

                //word.SaveAs(path);

                client.PutObject("xinxihua", path, word.InputStream);

                result.ErrorCode = 200;


            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
            }


           


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 机试题详细页面
        /// </summary>
        /// <param name="ids">机试题Id 以逗号分割</param>
        /// <returns></returns>
        
        public ActionResult ComputerTestQuestionDetailList(string ids)
        {
            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);

            var obj = db_computerTestQuestion.AllComputerTestQuestion().Where(d=>d.ID==int.Parse( list[0])).FirstOrDefault();

            var result = db_computerTestQuestion.ConvertToComputerTestQuestionsView(obj, true);

            ViewBag.ComputerTestQuestionIds = ids;

            ////读取文件
            //byte[] s = System.IO.File.ReadAllBytes(obj.SaveURL);

            ViewBag.ids = ids;

            return View(result);


        }


        /// <summary>
        /// 启用或禁用
        /// </summary>
        /// <returns></returns>
        public ActionResult ComputerTestQuestionIsUsing(string ids)
        {
            string[] ary1 = ids.Split(',');

            var list = ary1.ToList();

            list.RemoveAt(ary1.Length - 1);
            AjaxResult result = new AjaxResult();

            try
            {
                db_computerTestQuestion.DisableOrEnable(list);
                result.ErrorCode=200;
            }
            catch (Exception ex)
            {

                result.ErrorCode=500;
            }

            return Json(result,JsonRequestBehavior.AllowGet);

        }

        public ActionResult RemoveComputerTestQuestion(int id)
        {
            var result = db_computerTestQuestion.Delete(id);

            return Json(result);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <returns></returns>
        public ActionResult Dowle(int id)
        {

            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            var obj = db_computerTestQuestion.AllComputerTestQuestion().Where(d => d.ID == id).FirstOrDefault();

            var filename = Path.GetFileName(obj.SaveURL);

            //var path = Server.MapPath("/uploadXLSXfile/ComputerTestQuestionsWord/"+filename);

            var path = "/ExaminationSystem/ComputerTestQuestionsWord/" + filename;

            var fliedata = client.GetObject("xinxihua", path);

            //FileStream fileStream = new FileStream(path, FileMode.Open);

            return File(fliedata.ObjectContent, "application/octet-stream",Server.UrlEncode(filename));


        }
        [ValidateInput(false)]
        public ActionResult SearchComputerTestQuestion(string Title, int course, int page, int limit)
        {

            var list = db_computerTestQuestion.Search(Title, course);

            var skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();

            //转换对象
            List<ComputerTestQuestionsView> resultlist = new List<ComputerTestQuestionsView>();

            foreach (var item in skiplist)
            {

                var tempobj = db_computerTestQuestion.ConvertToComputerTestQuestionsView(item, true);
                resultlist.Add(tempobj);

            }

            var obj = new
            {
                code = 0,
                msg = "",
                count = resultlist.Count,
                data = resultlist

            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }

      

    }
}