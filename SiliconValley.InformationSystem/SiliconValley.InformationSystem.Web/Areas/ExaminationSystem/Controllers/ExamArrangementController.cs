using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SiliconValley.InformationSystem.Web.Areas.ExaminationSystem.Controllers
{
    //考试安排控制器
    [CheckLogin]
    public class ExamArrangementController : Controller
    {

        private readonly ExaminationPaperBusiness db_Paper;

        private readonly ChoiceQuestionBusiness db_choiceQuestion;

        private readonly AnswerQuestionBusiness db_answerQuestion;

        private readonly ComputerTestQuestionsBusiness db_computerTestQuestion;

        private readonly ExaminationBusiness db_examination;

        private readonly ExaminationRoomBusiness db_examinationRoom;

        private readonly BaseBusiness<HeadClass> db_headclass;

        private readonly GrandBusiness db_grand;

        private readonly CourseBusiness db_course;

        private readonly ExamScoresBusiness db_scores;
        private readonly ClassScheduleBusiness db_classsched;
        private readonly ScheduleForTraineesBusiness db_schedule;
        private readonly CandidateInfoBusiness db_candidateinfo;

        public ExamArrangementController()
        {
            db_Paper = new ExaminationPaperBusiness();

            db_choiceQuestion = new ChoiceQuestionBusiness();

            db_answerQuestion = new AnswerQuestionBusiness();

            db_computerTestQuestion = new ComputerTestQuestionsBusiness();
            db_examination = new ExaminationBusiness();

            db_examinationRoom = new ExaminationRoomBusiness();

            db_headclass = new BaseBusiness<HeadClass>();
            db_grand = new GrandBusiness();
            db_course = new CourseBusiness();
            db_scores = new ExamScoresBusiness();
           db_classsched =  new ClassScheduleBusiness();
          db_schedule =  new ScheduleForTraineesBusiness();
            db_candidateinfo=  new CandidateInfoBusiness();
        }

        // GET: ExaminationSystem/ExamArrangement
        public ActionResult ExamArrangement()
        {
            SpecialtyBusiness specialtyBusiness = new SpecialtyBusiness();
            ViewBag.Major = specialtyBusiness.GetSpecialties();
            //提供试卷类型数据

            ViewBag.ExamTypes = db_Paper.AllexamTypes();


            return View();
        }
        /// <summary>
        /// 考场学生操作
        /// </summary>
        /// <returns></returns>
        public ActionResult Examinationroomoperation()
        {
            return View();
        }
        /// <summary>
        /// 一键录入班级人员名单
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public ActionResult ExaminationOnekey(int className, int examid)
        {
            Business.TeachingDepBusiness.TeacherClassBusiness dbteacherclass = new TeacherClassBusiness();
            AjaxResult result = new AjaxResult();
            try
            {
                var exam = db_examination.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
                if (db_examination.IsEnd(exam))
                {
                    result.ErrorCode = 400;
                    result.Msg = "失败";
                    result.Data = null;
                }
                else
                {
                    //根据className查出这个班级的人员
                    var classsch = db_classsched.GetList().Where(d => d.id == className).FirstOrDefault().ClassNumber;
                    var renyuan = db_schedule.GetList().Where(b => b.ClassID == classsch && b.CurrentClass == true).ToList();
                    foreach (var item in renyuan)
                    {

                        var tempstu = db_examination.AllCandidateInfo(examid).Where(d => d.StudentID == item.StudentID).FirstOrDefault();

                        if (tempstu == null)
                        {
                            CandidateInfo candidateInfo = new CandidateInfo();
                            candidateInfo.CandidateNumber = Guid.NewGuid().ToString();
                            candidateInfo.ComputerPaper = null;
                            candidateInfo.Examination = examid;
                            candidateInfo.IsReExam = false;
                            candidateInfo.Paper = null;
                            candidateInfo.StudentID = item.StudentID;
                            candidateInfo.ClassId = dbteacherclass.GetScheduleByStudent(item.StudentID).id;

                            db_candidateinfo.Insert(candidateInfo);
                        }

                    }

                    BaseBusiness<CandidateInfo> dbcnad = new BaseBusiness<CandidateInfo>();



                    renyuan.ForEach(s =>
                    {
                        var stu = dbcnad.GetIQueryable().Where(c => c.Examination == examid && c.StudentID == s.StudentID).FirstOrDefault();
                        //初始化成绩单
                        db_scores.InitExamScores(examid, stu.CandidateNumber);
                    });


                    result.ErrorCode = 200;
                    result.Msg = "成功";
                    result.Data = null;
                }
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
        /// 考试发布编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ReleaseTestEditor(int ids)
        {
            var list = db_examination.AllExamination().Where(d => d.ID == ids).SingleOrDefault();

            ViewBag.zhi = list;

            return View();
        }
        /// <summary>
        /// 考试编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult ReleaseTestEditorbianji()
        {
            return null;
        }
        /// <summary>
        /// 选择题数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult ChoiceQuestionTableData(int page, int limit)
        {
            List<MultipleChoiceQuestion> multipleChoicelist = db_choiceQuestion.AllChoiceQuestionData().Where(d => d.IsUsing == true).ToList();

            List<MultipleChoiceQuestion> skiplist = multipleChoicelist.Skip((page - 1) * limit).Take(limit).ToList();

            //转换对象

            List<ChoiceQuestionTableView> resultlist = new List<ChoiceQuestionTableView>();

            foreach (var item in skiplist)
            {
                var tempobj = db_choiceQuestion.ConvertToChoiceQuestionTableView(item);

                resultlist.Add(tempobj);
            }

            var obj = new
            {

                code = 0,
                msg = "",
                count = multipleChoicelist.Count,
                data = resultlist


            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        
        public ActionResult GetChoiceQuestionByID(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                var tempobj = db_choiceQuestion.AllChoiceQuestionData().Where(d => d.Id == id && d.IsUsing == true).FirstOrDefault();
                var obj = db_choiceQuestion.ConvertToChoiceQuestionTableView(tempobj);

                result.Data = obj;
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
        /// 获取解答题
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetAnsweruestionByID(int id)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                var tempobj = db_answerQuestion.AllAnswerQuestion().Where(d => d.ID == id && d.IsUsing == true).FirstOrDefault();
                var obj = db_answerQuestion.ConvertToAnswerQuestionView(tempobj, true);

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

        public ActionResult AnswerQuestionData(int page, int limit)
        {
            var list1 = db_answerQuestion.AllAnswerQuestion().Where(d => d.IsUsing == true);

            var list = list1.Skip((page - 1) * limit).Take(limit).ToList();

            List<AnswerQuestionView> resultlist = new List<AnswerQuestionView>();
            foreach (var item in list)
            {
                var tempobj = db_answerQuestion.ConvertToAnswerQuestionView(item, true);
                resultlist.Add(tempobj);
            }
            var obj = new {

                code = 0,
                msg = "",
                count = list1.Count(),
                data = resultlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ComputerTestQuestionData(int page, int limit)
        {
            var list1 = db_computerTestQuestion.AllComputerTestQuestion().Where(d => d.IsUsing == true).ToList();

            var list = list1.Skip((page - 1) * limit).Take(limit).ToList();

            List<ComputerTestQuestionsView> resultlist = new List<ComputerTestQuestionsView>();

            foreach (var item in list)
            {
                var objtemp = db_computerTestQuestion.ConvertToComputerTestQuestionsView(item,true);

                resultlist.Add(objtemp);
            }

            var obj = new {
                code = 0,
                msg = "",
                count = list1.Count,
                data = resultlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetConputertionByID(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                var tempobj = db_computerTestQuestion.AllComputerTestQuestion().Where(d => d.IsUsing == true).FirstOrDefault();

                var obj = db_computerTestQuestion.ConvertToComputerTestQuestionsView(tempobj, true);

                result.ErrorCode = 200;
                result.Data = obj;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// 发布考试信息页面
        /// </summary>
        public ActionResult ReleaseExamination()
        {
            return View();
           
        }

        /// <summary>
        /// 考试历史的分布视图
        /// </summary>
        /// <returns></returns>
        public ActionResult ExaminationStoryView()
        {
            return PartialView();
        }
        /// <summary>
        /// 删除历史考试数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DeleteExaminationStoryData(int id)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                var list = db_examination.AllExamination().OrderByDescending(d => d.ID == id).FirstOrDefault();
                //获取考试是否结束
                bool Isend = db_examination.IsEnd(db_examination.AllExamination().Where(d => d.ID == id).FirstOrDefault());
                //判断考试是否结束，结束就不能删除这趟考试 true结束 flase未结束（进行中）
                if (Isend == true)
                {
                    result.ErrorCode = 300;
                }
                else
                {
                    db_examination.Delete(list);
                    result.ErrorCode = 200;
                }   
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
            }
            
            return Json(result);
        }
        /// <summary>
        /// 历史考试数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ExaminationStoryData(int page, int limit)
        {

            var list = db_examination.AllExamination().OrderByDescending(d=>d.BeginDate).ToList();

            var skplist = list.Skip((page - 1) * limit).Take(limit);


            var resultlist = new List<ExaminationView>();
            foreach (var item in skplist)
            {
                var tempobj = db_examination.ConvertToExaminationView(item);

                if (tempobj != null)
                {
                    resultlist.Add(tempobj);
                }
            }

            List<object> returnlist = new List<object>();

            foreach (var item in resultlist)
            {
                bool Isend = db_examination.IsEnd(db_examination.AllExamination().Where(d => d.ID == item.ID).FirstOrDefault());

               var examtypeview = db_examination.ConvertToExamTypeView(item.ExamType);

                var grand = db_grand.AllGrand().Where(d => d.Id == examtypeview.GrandID.Id).FirstOrDefault();

                var temobj1 = new {
                    ID = item.ID,
                    Title = item.Title,
                    TypeName = examtypeview.TypeName.TypeName,
                    grand = grand.GrandName,
                    BeginDate = item.BeginDate,
                    TimeLimit = item.TimeLimit,
                    Remark = item.Remark,
                    IsEnd = Isend,
                    //== true ? "结束" : "未结束"
                    
                };
                returnlist.Add(temobj1);

            }
            var obj = new {
                code = 0,
                msg = "",
                count = list.Count,
                data = returnlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取阶段课程
        /// </summary>
        /// <returns></returns>

        public ActionResult GetCourseByGrand(int grandid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
               var COURSE = db_course.GetCurriculas().Where(d => d.Grand_Id == grandid).ToList();
                result.ErrorCode = 200;
                result.Msg = "";
                result.Data = COURSE;
            }
            catch (Exception EX)
            {

                result.ErrorCode = 500;
                result.Msg = "";
                result.Data = null;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
           
        }

        /// <summary>
        /// 发布考试信息
        /// </summary>
        /// <param name="examination"></param>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult ReleaseExaminationView()
        {
            List<ExamType> list = db_Paper.AllexamTypes();

            List<ExamTypeView> viewlist = new List<ExamTypeView>();
            //转换类型
            foreach (var item in list)
            {
                var tempobj = db_examination.ConvertToExamTypeView(item);

                if (tempobj != null)
                    viewlist.Add(tempobj);
            }
            ViewBag.ExamTypes = viewlist;

            //提供课程数据

            ViewBag.Courselist = db_course.GetCurriculas();


            List<QuestionLevel> questionLevels = db_examination.AllQuestionLevel();
            ViewBag.QuestionLevels = questionLevels;
            return View();
        }

        /// <summary>
        /// 发布考试信息
        /// </summary>
        /// <param name="examination"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ReleaseExamination(Examination examination,int course)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                examination.ExamNo = db_examination.ProductExamNumber(examination);

                var examview = db_examination.ConvertToExaminationView(examination);

                db_examination.Insert(examination);
                //2是阶段，1是升学
                if (examview.ExamType.ExamTypeID == 2)
                {
                    //记录课程
                    db_examination.ExamCouresConfigAdd(examination.ID, course);

                } else if (examview.ExamType.ExamTypeID == 1) {

                    //记录课程
                    db_examination.ExamCouresConfigAdd(examination.ID, 0);
                }

                //将考试 题目存储到redis缓存
                RedisCache redisCache = new RedisCache();

                redisCache.RemoveCache("MultipleChoiceQuestion");
                redisCache.SetCache("MultipleChoiceQuestion", db_choiceQuestion.AllChoiceQuestionData());

                redisCache.RemoveCache("AnswerQuestionBank");
                redisCache.SetCache("AnswerQuestionBank", db_answerQuestion.AllAnswerQuestion());

                redisCache.RemoveCache("MachTestQuesBank");
                redisCache.SetCache("MachTestQuesBank", db_computerTestQuestion.AllComputerTestQuestion());

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
        /// 安排考场
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public ActionResult ArrangeExamRoom(int id)
        {

            //提供考场数据


            var examinroomlist = db_examination.ExaminationRoomByExamID(id);
            List<ExaminationRoomView> list = new List<ExaminationRoomView>();

            foreach (var item in examinroomlist)
            {
                var obj = db_examination.ConvertToExaminationView(item);

                list.Add(obj);
            }

            //阶段数据、
            var grands = db_grand.GetList().Where(d => d.IsDelete == false).ToList();




            ViewBag.ExaminationRooms = list;
            ViewBag.ExamintionID = id;
            ViewBag.Grands = grands;
            return View();
        }

        public ActionResult GetExamByid(int id)
        {
            var obj = db_examination.AllExamination().Where(d => d.ID == id).FirstOrDefault();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取考场数据
        /// </summary>
        /// <param name="examintionid"></param>
        /// <returns></returns>
        public ActionResult ExamRoomData(int examintionid)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                List<ExaminationRoom> roomlist = db_examination.ExaminationRoomByExamID(examintionid);

                List<ExaminationRoomView> resultlist = new List<ExaminationRoomView>();

                foreach (var item in roomlist)
                {
                    var obj = db_examination.ConvertToExaminationView(item);
                    if (obj != null)
                    {
                        resultlist.Add(obj);
                    }
                }

                result.ErrorCode = 200;
                result.Data = resultlist;
                result.Msg = "成功";


            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Data = null;
                result.Msg = "失败";

            }

            return Json(result, JsonRequestBehavior.AllowGet); ;
        }

        public ActionResult ChoiceClassroom()
        {

            //获取已经安排的成为考场的教室

            var allllist = db_examination.AllExamination();

            var usedroomlist = new List<ExaminationRoom>();
            foreach (var item in allllist)
            {
                if (!db_examination.IsEnd(item))
                {
                    var obj = db_examination.ExaminationRoomByExamID(item.ID);
                    usedroomlist.AddRange(obj);
                }
            }


            BaseBusiness<Classroom> classroomdb = new BaseBusiness<Classroom>();

            var allclassroom = classroomdb.GetList().Where(d => d.IsDelete == false).ToList();

            //去掉已经使用 的


            foreach (var item1 in usedroomlist)
            {
                allclassroom.RemoveAll(d => d.Id == item1.Classroom_Id);
            }


            ViewBag.Classrooms = allclassroom;


            return View();
        }

        /// <summary>
        /// 移除考场
        /// </summary>
        /// <param name="examintionid">考试ID</param>
        /// <param name="classroom">教室Id</param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult RemoveExamRoom(int examintionid, int classroom)

        {
            AjaxResult result = new AjaxResult();

            try
            {
                var examroom = db_examination.AllExaminationRoom().Where(d => d.Examination == examintionid && d.Classroom_Id == classroom).FirstOrDefault();

                db_examinationRoom.Delete(examroom);


                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception x)
            {
                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);



        }


        /// <summary>
        /// 添加考场
        /// </summary>
        /// <param name="examintionid">考试ID</param>
        /// <param name="classroom">教室ID</param>
        /// <returns></returns>

        public ActionResult AddExamRoom(int examintionid, string classroom)
        {
            AjaxResult result = new AjaxResult();

            var arry1 = classroom.Split(',');

            var arry = arry1.ToList();
            arry.RemoveAt(arry1.Length - 1);

            try
            {
                foreach (var item in arry)
                {
                    ExaminationRoom examinationRoom = new ExaminationRoom();
                    examinationRoom.Classroom_Id = int.Parse(item);
                    examinationRoom.Examination = examintionid;
                    db_examinationRoom.Insert(examinationRoom);
                }



                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception ex)
            {

                result.Data = null;
                result.ErrorCode = 500;
                result.Msg = "失败";


            }


            return Json(result, JsonRequestBehavior.AllowGet);



        }


        /// <summary>
        /// 班主任录入参加考试的学生视图  //教务使用
        /// </summary> 
        /// <returns></returns>
        public ActionResult AppointmentExamination()
        {

            ////获取当前登陆的班主任
            //Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            BaseBusiness<ClassSchedule> dbclass = new BaseBusiness<ClassSchedule>();
            //BaseBusiness<Headmaster> dbheadmaster = new BaseBusiness<Headmaster>();
            ///获取到班主任
            //var headmaster = dbheadmaster.GetList().Where(d => d.IsDelete == false).Where(d => d.informatiees_Id == user.EmpNumber).FirstOrDefault();

            //获取班主任的班级
            //var classs = db_headclass.GetList().Where(d => d.IsDelete == false && d.LeaderID == headmaster.ID).ToList();

            var classs = dbclass.GetIQueryable().ToList().Where(d => d.ClassstatusID == null).ToList();

            
           

            //List<ClassSchedule> list = new List<ClassSchedule>();

            //foreach (var item in classs)
            //{
            //   var temp = dbclass.GetIQueryable().Where(d => d.id == item.ClassID).FirstOrDefault();
            //    if (temp != null)
            //        list.Add(temp);

            //}
            ViewBag.myClass = classs;
            return View();
        }

        public ActionResult StudentData(int page, int limit, int classid)
        {
           // var list = db_examination.GetMyStudentData();

            TeacherClassBusiness dbteacherclass = new TeacherClassBusiness();
            ClassScheduleBusiness dbclass = new ClassScheduleBusiness();
            //获取班级学员
            var list = dbclass.ClassListStudent(classid);
            

            var skiplist = list.Skip((page - 1) * limit).Take(limit).ToList();
            //将学生转换为详细模型
            List<StudentDetailView> studentlist = new List<StudentDetailView>();
            TeacherClassBusiness teacherclas = new TeacherClassBusiness();
            foreach (var item in skiplist)
            {
                var tempobj = teacherclas.GetStudetentDetailView(item);

                if (tempobj != null)
                {
                    studentlist.Add(tempobj);
                }
            }

            var obj = new {
                code = 0,
                msg = "",
                count = list.Count,
                data = studentlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取还没开始的考试
        /// </summary>
        /// <returns></returns>
        public ActionResult GetNoEndExamination()
        {

            AjaxResult result = new AjaxResult();
            List<ExaminationView> resultlist = new List<ExaminationView>();
            try
            {
                var list = db_examination.NoEndExamination();

                foreach (var item in list)
                {
                    var tempobj = db_examination.ConvertToExaminationView(item);
                    if (tempobj != null)
                    {
                        resultlist.Add(tempobj);
                    }
                }

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = resultlist;
            }
            catch (Exception xe)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取学生是否预约了考试
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentIsInExam(string studentnumber, int examid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                bool IsIn = false;
                var list = db_examination.AllCandidateInfo(examid);

                foreach (var item in list)
                {
                    if (item.StudentID == studentnumber)
                    {
                        IsIn = true;
                        break;
                    }
                }

                result.ErrorCode = 200;
                result.Msg = "成功";
                if (IsIn)
                {
                    result.Data = "1";
                }
                else
                {
                    result.Data = "0";
                }
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
        /// 报名考试
        /// </summary>
        /// <param name="studentnumbers"></param>
        /// <param name="examid"></param>
        /// <returns></returns>
        public ActionResult subscribeExam(string JsonStr, int examid)
        {
            AjaxResult result = new AjaxResult();

            //{ "钟伦攀":"正常","19081997072400002":"false","19081997072400003":"false","19081997072400008":"false","19081997072400009":"false","19081997072400010":"false"}

            //首先去掉首尾括号

            var json = JsonStr.Substring(1, JsonStr.Length - 2);

            var str1 = json.Split(',');



            Dictionary<string, bool> list = new Dictionary<string, bool>();
            foreach (var item in str1)
            {

                var tempbool = item.Split(':')[1].Replace("\"", "").Replace("\\", "") == "true" ? true : false;

                var dd = item.Split(':')[0].Replace("\"", "");

                list.Add(dd, tempbool);
            }

            try
            {
                var exam = db_examination.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
                if (db_examination.IsEnd(exam))
                {
                    result.ErrorCode = 400;
                    result.Msg = "失败";
                    result.Data = null;
                }
                else
                {
                    db_examination.subscribeExam(list, examid);

                    BaseBusiness<CandidateInfo> dbcnad = new BaseBusiness<CandidateInfo>();

                  

                    list.ForEach(s=>
                    {
                        var stu = dbcnad.GetIQueryable().Where(d => d.Examination == examid && d.StudentID == s.Key).FirstOrDefault();
                        //初始化成绩单
                        db_scores.InitExamScores(examid, stu.CandidateNumber);
                    });


                    result.ErrorCode = 200;
                    result.Msg = "成功";
                    result.Data = null;
                }


            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }
            //调用方法


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取消报名
        /// </summary>
        /// <param name="studentIDs"></param>
        /// <param name="examid"></param>
        /// <returns></returns>
        public ActionResult cancelsubscribeExam(string studentIDs, int examid)
        {
            AjaxResult result = new AjaxResult();

            var list1 = studentIDs.Split(',');

            var studentlist = list1.ToList();
            studentlist.RemoveAt(list1.Length - 1);

            try
            {
                var exam = db_examination.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
                if (db_examination.IsEnd(exam))
                {
                    result.ErrorCode = 400;
                    result.Msg = "失败";
                    result.Data = null;
                }
                else
                {
                    foreach (var item in studentlist)
                    {
                        var ss = db_examination.AllCandidateInfo(examid);
                        var candidinfo = db_examination.AllCandidateInfo(examid).Where(d => d.StudentID == item).FirstOrDefault();

                        db_scores.RemoveExamScores(examid, candidinfo.CandidateNumber);

                    }

                    db_examination.cancelsubscribeExam(studentlist, examid);

                    
                    

                    result.ErrorCode = 200;
                    result.Msg = "成功";
                    result.Data = null;
                }



            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = ex.Message;
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SearchStudentData(int classnumber, string studentName, int page, int limit)
        {
            ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();

            List<StudentInformation> resultlist = new List<StudentInformation>();

            //if (classnumber == "0" && studentName == "")
            //{
            //    //获取全部
            //    resultlist = db_examination.GetMyStudentData();


            //}
            //else if (classnumber != "0" && studentName == "")
            //{
            //    resultlist = scheduleForTraineesBusiness.ClassStudent(classnumber);
            //}
            //else if (classnumber == "0" && studentName != "")
            //{
            //    resultlist = db_examination.GetMyStudentData().Where(d => d.Name.Contains(studentName)).ToList();
            //}
            //else
            //{
            //    resultlist = scheduleForTraineesBusiness.ClassStudent(classnumber).Where(d => d.Name.Contains(studentName)).ToList();
            //}

            var skiplist = resultlist.Skip((page - 1) * limit).Take(limit).ToList();

            //将学生转换为详细模型
            List<StudentDetailView> studentlist = new List<StudentDetailView>();
            TeacherClassBusiness teacherclas = new TeacherClassBusiness();
            foreach (var item in skiplist)
            {
                var tempobj = teacherclas.GetStudetentDetailView(item);

                if (tempobj != null)
                {
                    studentlist.Add(tempobj);
                }
            }

            var obj = new {
                code = 0,
                msg = "",
                count = resultlist.Count,
                data = studentlist
            };





            return Json(obj, JsonRequestBehavior.AllowGet);


        }



        public ActionResult SetInvigilator(int examroom, int examid)
        {
            BaseBusiness<Classroom> dbclassroom = new BaseBusiness<Classroom>();
                 
            ViewBag.examid = examid;
            ViewBag.classroom = dbclassroom.GetIQueryable().ToList().Where(d => d.Id == examroom).FirstOrDefault();


            //提供部门数据
           List<Department> deplist = db_examination.GetSureInvigilatorDep();

            ViewBag.Deplist = deplist;

            return View();
        }


        /// <summary>
        /// 获取监考员
        /// </summary>
        /// <returns></returns>
        public ActionResult GetExamInvigilator(int examid, int classroomid)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var emplist = db_examination.GetProtorData(examid, classroomid);
                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = emplist;
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
        /// 员工数据
        /// </summary>
        /// <returns></returns>
        public ActionResult EmpData(int page, int limit)
        {
            EmployeesInfoManage dbemp = new EmployeesInfoManage();
            TeacherBusiness dbteacher = new TeacherBusiness();


            var emplist = db_examination.GetSureInvigilator();
            var skiplist = emplist.Skip((page - 1) * limit).Take(limit).ToList();

            List<EmpDetailView> viewlist = new List<EmpDetailView>();

            foreach (var item in skiplist)
            {
              var temp =   dbteacher.ConvertToEmpDetailView(item);

                if (temp != null)
                {
                    viewlist.Add(temp);
                }
            }

            var obj = new {

                code = 0,
                msg="",
                count = emplist.Count,
                data=viewlist

            };


            return Json(obj, JsonRequestBehavior.AllowGet);


        }


        /// <summary>
        /// 安排监考员
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="examroomid"></param>
        /// <param name="Invigilatorids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ArrangeTheInvigilator(int examid, int examroomid, string Invigilatorids)
        {

            AjaxResult result = new AjaxResult();

            var list1 = Invigilatorids.Split(',');
            var list = list1.ToList();
            list.RemoveAt(list1.Length - 1);


            try
            {
                db_examination.ArrangeTheInvigilator(examid, examroomid, list);

                result.Data = null;
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

        public ActionResult GetProtorData(int examid, int examroom)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                var list = db_examination.GetProtorData(examid, examroom);

                result.Data = list;
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


        /// <summary>
        /// 详细页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamDetailView(int examid)
        {

            Examination exam = db_examination.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
            var examview = db_examination.ConvertToExaminationView(exam);
            //ViewBag.Examination = examview;

           var examroomlist = db_examination.ExaminationRoomByExamID(examid);

            List<ExaminationRoomView> roomlist = new List<ExaminationRoomView>();

            foreach (var item in examroomlist)
            {
               var tempobj = db_examination.ConvertToExaminationView(item);

                if (tempobj != null)
                {
                    roomlist.Add(tempobj);
                }
            }
            BaseBusiness<ExamTypeName> dbexamtypename = new BaseBusiness<ExamTypeName>();
            ViewBag.examtype = dbexamtypename.GetList().Where(d => d.ID == examview.ExamType.ExamTypeID).FirstOrDefault();

            ViewBag.IsEnd = db_examination.IsEnd(exam) ? "已结束" : "未结束";

            var tatal = 0;
          List<CandidateInfo> candidatelist =  db_examination.AllCandidateInfo(examid);

            //总人数
            ViewBag.total = candidatelist.Count;

            //重考人数Re exam
            ViewBag.ReExam = candidatelist.Where(d => d.IsReExam).ToList().Count;
            ViewBag.examroomlist = roomlist;

            return View(examview);
        }


        /// <summary>
        /// 详细数据
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public ActionResult ExamDateData(int examid,int classroomid)
        {
            Examination exam = db_examination.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
           
            //考场人数
            //重考人数
            //监考员
            var examroom = db_examination.AllExaminationRoom().Where(d => d.Examination == examid && d.Classroom_Id == classroomid).FirstOrDefault();
            var examview = db_examination.ConvertToExaminationView(examroom);

            var list = db_examination.AllExamroomDistributed(examid).Where(d => d.ExaminationRoom == examroom.ID).ToList();

            List<EmployeesInfo> emplist = new List<EmployeesInfo>();

            if (examview.Invigilator1 != null)
            {
                emplist.Add(examview.Invigilator1);
            }

            if (examview.Invigilator2 != null)
            {
                emplist.Add(examview.Invigilator2);
            }

            var obj = new
            {
                tatal = list.Count,
                emplist = emplist,


            };
            
            return Json(obj, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// 安排座位表
        /// </summary>
        /// <returns></returns>
        public ActionResult ArrangeSeatingChart()
        {

            //提供考试数据

            List<Examination> examinations = db_examination.NoEndExamination();

            ViewBag.Exams = examinations;

            return View();
        }


        /// <summary>
        /// 安排座位表
        /// </summary>
        /// <returns></returns>   
        public ActionResult ArrangeSeatingChartData(int examid)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                //生成座位表
                var seatobj = db_examination.GenerateSeatTable(examid);

                result.Data = seatobj;
                result.Msg = "成功";
                result.ErrorCode = 200;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Msg = "失败";
                result.ErrorCode = 500;
                throw;
            }



            return Json(result, JsonRequestBehavior.AllowGet);



        }


        /// <summary>
        /// 考试阅卷安排 //教务使用
        /// </summary>
        /// <returns></returns>
        public ActionResult MarkingArrangement()
        {
            return View();
        }


        /// <summary>
        /// 查看考试的阅卷人
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckMarkingArrangemtn(int examid)
        {

            AjaxResult result = new AjaxResult();

            try
            {
                //获取阅卷安排
                List<MarkingArrangeView> viewlist = new List<MarkingArrangeView>();

                ExamScoresBusiness dbexamscores = new ExamScoresBusiness();

                BaseBusiness<Classroom> dbclassroom = new BaseBusiness<Classroom>();

                var markingarrangelist = dbexamscores.AllMarkingArrange().Where(d => d.ExamID == examid).ToList();

                if (markingarrangelist == null || markingarrangelist.Count ==0)
                {
                    var examroomlist = db_examination.AllExaminationRoom().Where(d => d.Examination == examid).ToList();

                    if (examroomlist != null)
                    {
                        foreach (var item in examroomlist)
                        {
                            MarkingArrangeView view = new MarkingArrangeView();
                            view.classroom = dbclassroom.GetIQueryable().ToList().Where(d => d.Id == item.Classroom_Id).FirstOrDefault();
                            view.ExamID = db_examination.AllExamination().Where(d => d.ID == item.Examination).FirstOrDefault();
                            view.IsFinsh = false;
                            view.MarkingTeacher = null;


                            viewlist.Add(view);
                        }
                    }
                }
                else
                {
                    foreach (var item in markingarrangelist)
                    {
                        var temobj = dbexamscores.ConvertToMarkingArrangeView(item);

                        if (temobj != null)
                        {
                            viewlist.Add(temobj);
                        }
                    }
                }


               

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = viewlist;
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
        /// 获取阶段数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GrandData()
        {
            AjaxResult result = new AjaxResult();
            try
            {
               var granlist = db_grand.AllGrand();

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = granlist;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTeacher_Grand(int grand)
        {
            AjaxResult result = new AjaxResult();


            try
            {


                TeacherBusiness dbteacher = new TeacherBusiness();

               var teacherlist = dbteacher.BrushSelectionByGrand(grand);


                List<Teacher> resultlist = new List<Teacher>();
                //筛选掉离职的

                foreach (var item in teacherlist)
                {
                    if ( !dbteacher.IsDimission(item.TeacherID))
                    {
                        resultlist.Add(item);
                    }
                }

                List<EmployeesInfo> emplist = new List<EmployeesInfo>();
                foreach (var item in resultlist)
                {
                   var tempemp =  dbteacher.GetEmpByEmpNo(item.EmployeeId);

                    if (tempemp != null)
                        emplist.Add(tempemp);
                }

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = emplist;
            }
            catch (Exception ex)
            {
                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
           

        }

        /// <summary>
        /// 设置阅卷老师
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <param name="classroomid">教室id</param>
        /// <param name="teacher">阅卷老师编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetMarkingTeacher(int examid, int classroomid, string teacher)
        {
            AjaxResult result = new AjaxResult();

            try
            {

                //根据classroomid 获取考场id
               var examroom = db_examination.AllExaminationRoom().Where(d => d.Classroom_Id == classroomid && d.Examination == examid).FirstOrDefault();

                //获取当前的阅卷老师
                var markingteacher = db_scores.AllMarkingArrange().Where(d => d.ExamID == examid && d.ExamRoom == examroom.ID).FirstOrDefault();

                if (markingteacher == null)
                {

                    db_scores.SetMarkingTeacher(examid, examroom.ID, teacher);

                    result.ErrorCode = 200;
                    result.Msg = "设置成功";
                    result.Data = null;

                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                TeacherBusiness dbteacher = new TeacherBusiness();
                 var teacherobj = dbteacher.GetTeachers().Where(d => d.EmployeeId == markingteacher.MarkingTeacher).FirstOrDefault();


                var isBiginMarking = false;

                //获取考场学员
                var stulist = db_scores.CandidateinfosByExamroom(examid, classroomid);

                foreach (var item in stulist)
                {
                   var temp = db_scores.AllExamScores().Where(d => d.Examination == examid && d.CandidateInfo == item.CandidateNumber && d.Reviewer == teacherobj.TeacherID).FirstOrDefault();

                    if (temp != null)
                    {
                        isBiginMarking = true;
                        break;
                    }
                }

                if (markingteacher != null && isBiginMarking)
                {
                    result.ErrorCode = 502;
                    result.Msg = "阅卷人修改失败!  原因:当前阅卷人已经开始阅卷";
                    result.Data = null;

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    db_scores.SetMarkingTeacher(examid, examroom.ID, teacher);

                    result.ErrorCode = 200;
                    result.Msg = "设置成功";
                    result.Data = null;
                }
  
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "服务器错误";
                result.Data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 部门人员查询
        /// </summary>
        /// <returns></returns>
        public ActionResult DepEmpData(string depid, string empname, int page, int limit)
        {

            EmployeesInfoManage dbemp = new EmployeesInfoManage();

            List<EmployeesInfo> emplist = new List<EmployeesInfo>();

            if (depid == null)
            {
                //根据名字查询
               var templist = dbemp.GetAll().Where(d=>d.EmpName.Contains(empname)).ToList();

                emplist.AddRange(templist);
            }

            if (empname == "" && depid!=null)
            {
                //获取部门所有人员

               var templist = dbemp.GetEmpsByDeptid(int.Parse(depid));

                emplist.AddRange(templist);
            }

            if (empname != "" && depid != null)
            {
                var templist = dbemp.GetEmpsByDeptid(int.Parse(depid)).Where(d=>d.EmpName.Contains(empname));

                emplist.AddRange(templist);
            }

            var skplist = emplist.Skip((page - 1) * limit).Take(limit).ToList();

            List<EmpDetailView> viewlist = new List<EmpDetailView>();

            TeacherBusiness dbteacher = new TeacherBusiness();
            foreach (var item in skplist)
            {
               var temp = dbteacher.ConvertToEmpDetailView(item);

                if (temp != null)
                {
                    viewlist.Add(temp);
                }
            } 


            var obj = new {

                code = 0,
                msg = "",
                count = emplist.Count,
                data = viewlist
            };

            return Json(obj, JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// 考试违纪情况视图
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamBreachView()
        {
            //提供考试

            var examlist = db_examination.AllExamination().OrderByDescending(d => d.BeginDate).ToList();

            ViewBag.Examlist = examlist;
            return View();

        }


        /// <summary>
        /// 考试违纪数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamBreachData(int limit, int page, int examid)
        {
            List<ExamBreach> examBreaches = new List<ExamBreach>();

            if (examid == 0)
            {
                //获取全部数据
                examBreaches = db_examination.AllExamBreach();
            }
            else
            {

                examBreaches = db_examination.AllExamBreach().Where(d=>d.Exam == examid).ToList();

            }

            BaseBusiness<StudentInformation> dbstudent = new BaseBusiness<StudentInformation>();

            var skiplist = examBreaches.Skip((page - 1) * limit).Take(limit);

            List<object> objlist = new List<object>();

            foreach (var item in skiplist)
            {
                

                var obj = new {

                    ID = item.ID,
                    ExamTID = item.Exam,
                    ExamTitle = db_examination.AllExamination().Where(d => d.ID == item.Exam).FirstOrDefault().Title,
                    StudentNumber = item.StudentNumber,
                    StudentName = dbstudent.GetEntity(item.StudentNumber).Name,
                    Breach = item.Breach,
                    CreateDate = item.CreateDate


                };

                objlist.Add(obj);
            }


            var resultobj = new {

                code = 0,
                msg = "",
                count = examBreaches.Count,
                data = objlist

            };

            return Json(resultobj, JsonRequestBehavior.AllowGet);
        }

        
        /// <summary>
        /// 添加考试违纪情况
        /// </summary>
        /// <returns></returns>
        public ActionResult AddExamBreachView()
        {
            return View();
        }


        /// <summary>
        /// 选择考试数据
        /// </summary>
        /// <returns></returns>
        public ActionResult selectExam()
        {
            return View();
        }

        /// <summary>
        /// 选择考生数据
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>

        public ActionResult selectcandidInfo(int examid)
        {

            ViewBag.Examid = examid;
            return View();

        }

        /// <summary>
        /// 考生数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ExamStuData(int examid, int limit, int page)
        {
            BaseBusiness<StudentInformation> dbstudent = new BaseBusiness<StudentInformation>();

            List<StudentInformation> stulit = new List<StudentInformation>();

            var candidifnolist = db_examination.AllCandidateInfo(examid);

            var skiplist = candidifnolist.Skip((page - 1) * limit).Take(limit).ToList();

            foreach (var item in skiplist)
            {
               var tempobj = dbstudent.GetEntity(item.StudentID);

                if (tempobj != null)
                {
                    stulit.Add(tempobj);
                }
            }

            var obj = new {

                code = 0,
                msg ="",
                count = candidifnolist.Count,
                data = stulit

            };

            return Json(obj, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// 提交考试违纪记录
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult ExamBreach(int examid, string stumber, string breach)
        {
            AjaxResult result = new AjaxResult();

            try
            {
                BaseBusiness<ExamBreach> dbexambreach = new BaseBusiness<ExamBreach>();

                ExamBreach data = new ExamBreach();
                data.Breach = breach;
                data.CreateDate = DateTime.Now;
                data.Exam = examid;
                data.StudentNumber = stumber;

                dbexambreach.Insert(data);

                result.ErrorCode = 200;
                result.Msg = "";
                result.Data = null;
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Msg = "";
                result.Data = null;

               
            }


            return Json(result, JsonRequestBehavior.AllowGet);

        }
    }
}