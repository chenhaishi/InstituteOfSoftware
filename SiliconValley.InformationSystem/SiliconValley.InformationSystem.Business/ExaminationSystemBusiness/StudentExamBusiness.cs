using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.ExaminationSystemBusiness
{
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.StudentBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using System.Xml;


    public class StudentExamBusiness
    {

        private readonly ExaminationBusiness db_exam;

        private readonly BaseBusiness<CandidateInfo> db_candidateinfo;

        private readonly ChoiceQuestionBusiness db_choiceQuestion;

        private readonly CourseBusiness db_Course;

        private readonly AnswerQuestionBusiness db_answerQuextion;

        private readonly ComputerTestQuestionsBusiness db_computerQuestion;

        private readonly StudentInformationBusiness db_student;
        public StudentExamBusiness()
        {
            db_exam = new ExaminationBusiness();
            db_candidateinfo = new BaseBusiness<CandidateInfo>();
            db_choiceQuestion = new ChoiceQuestionBusiness();
            db_Course = new CourseBusiness();
            db_answerQuextion = new AnswerQuestionBusiness();
            db_computerQuestion = new ComputerTestQuestionsBusiness();
            db_student = new StudentInformationBusiness();
        }
        /// <summary>
        /// 获取学员即将开始的考试
        /// </summary>
        public List<Examination> StudetnSoonExam(string studentNo)
        {
            List<Examination> resultlist = new List<Examination>();

            var examlist = db_exam.AllNoEndExamination();

            var sturdentexanmlist = db_candidateinfo.GetList().Where(d => d.StudentID == studentNo).ToList();

            foreach (var item in sturdentexanmlist)
            {
                var tempobj = examlist.Where(d => d.ID == item.Examination).FirstOrDefault();
                if(tempobj !=null)

                    resultlist.Add(tempobj);

            }
            return resultlist;


        }


        /// <summary>
        /// 随机获取选择题题目  //升学考试使用本函数抽题目
        /// </summary>
        /// <param name="count">个数</param>
        /// <param name="kecheng">课程（当为阶段考试id时候传入该参数）</param>
        /// <returns></returns>

        public List<ChoiceQuestionTableView> ProductChoiceQuestion(Examination examination,int kecheng)
        {            
            TeacherClassBusiness dbteacheraclass = new TeacherClassBusiness();
      
            List<ChoiceQuestionTableView> questionlist = new List<ChoiceQuestionTableView>();

            //获取考试类型
            var examType = examination.ExamType;
            var examview = db_exam.ConvertToExaminationView(examination);


            //从缓存中获取数据
            RedisCache redisCache = new RedisCache();

            var list = new List<MultipleChoiceQuestion>();

            var redisdata = redisCache.GetCache<List<MultipleChoiceQuestion>>("MultipleChoiceQuestion");
            if (redisdata != null)
            {
                list.AddRange(redisdata);
            }

            if (list == null || list.Count == 0)
            {
                list.AddRange(db_choiceQuestion.AllChoiceQuestionData());
            }

            var templist = new List<ChoiceQuestionTableView>();

            //foreach (var item in list)
            //{
            //    templist.Add(db_choiceQuestion.ConvertToChoiceQuestionTableView(item, false));
            //}
            List<Curriculum> sourchlist = new List<Curriculum>();
            if (examview.ExamType.ExamTypeID == 1)
            {
                var tempqulist = list.Where(d => d.Grand == examview.ExamType.GrandID && d.Course == 0).ToList();
                tempqulist.ForEach(d=>
                {
                    var tempobj = db_choiceQuestion.ConvertToChoiceQuestionTableView(d, false);
                    if (tempobj != null) questionlist.Add(tempobj);
                });
            }

            if (examview.ExamType.ExamTypeID == 2)
            {
               //var course = db_Course.GetList().Where(d => d.IsDelete == false && d.CurriculumID == kecheng).FirstOrDefault();

                list = list.Where(d => d.Course != 0 &&d.Course == kecheng).ToList();
                                        
                list.ForEach(d=>
                {
                    var tempobj = db_choiceQuestion.ConvertToChoiceQuestionTableView(d, false);

                    if (tempobj != null) questionlist.Add(tempobj);
                });

                //if (course != null) sourchlist.Add(course);

                //筛选题目
                //foreach (var item in templist)
                //{
                //    foreach (var item1 in sourchlist)
                //    {

                //        if (item.Course.CurriculumID == item1.CurriculumID)
                //        {
                //            //判断是否存在
                //            if (!IsContain(questionlist, item))
                //            {
                //                questionlist.Add(item);
                //            }
                //        }

                //    }
                //}


            }
      
            //筛选难度
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/questionLevelConfig.xml"));

            var xmlRoot = xmlDocument.DocumentElement;

            //获取我需要的配置		foreach	error CS1525: 表达式项“foreach”无效	

            //

            var choxml = (XmlElement)xmlRoot.GetElementsByTagName("choicequestion")[0];

            var levels = choxml.GetElementsByTagName("level");

            XmlElement mylevel = null;

            foreach (XmlElement item in levels)
            {
                var levelid = item.Attributes["levelId"];

                if (int.Parse(levelid.Value) == examview.PaperLevel.LevelID)
                {
                    mylevel = item;
                }

            }

            // 简单题的个数
            float jiandan = 0;
            float putong = 0;
            float kunnan = 0;

            int total = int.Parse(choxml.GetElementsByTagName("total")[0].InnerText);

            var temlist = mylevel.GetElementsByTagName("count");

            foreach (XmlElement item in temlist)
            {
                var levelid = item.Attributes["levelId"];
                if (int.Parse(levelid.Value) == 1)
                {
                    jiandan = int.Parse(item.Attributes["Percentage"].Value);
                    jiandan = total * (jiandan / 100);
                }

                if (int.Parse(levelid.Value) == 2)
                {
                    putong = int.Parse(item.Attributes["Percentage"].Value);
                    putong = total * (putong / 100);
                }

                if (int.Parse(levelid.Value) == 3)
                {
                    kunnan = int.Parse(item.Attributes["Percentage"].Value);
                    kunnan = total * (kunnan / 100);
                }
            }
            List<ChoiceQuestionTableView> resultlist = new List<ChoiceQuestionTableView>();
            Random random = new Random();


            //抽取题目
            var temp1list = questionlist.Where(d => d.Level.LevelID == 1).ToList();
            
            try
            {
                resultlist.AddRange(this.extracChoicequestion(temp1list, (int)jiandan));
            }
            catch (Exception ex)
            {

                string str = ex.Message;
            }

            var temp1list1 = questionlist.Where(d => d.Level.LevelID == 2).ToList();
            resultlist.AddRange(this.extracChoicequestion(temp1list1, (int)putong));

            var temp1list2 = questionlist.Where(d => d.Level.LevelID == 3).ToList();
            resultlist.AddRange(this.extracChoicequestion(temp1list2, (int)kunnan));

            List<ChoiceQuestionTableView> returnlist = new List<ChoiceQuestionTableView>();

            //去掉重复项
            foreach (var item in resultlist)
            {
                if (!IsContain(returnlist, item))
                {
                    returnlist.Add(item);
                }
            }


            return returnlist;
        }


        /// <summary>
        /// 生成考试解答题
        /// </summary>
        /// <param name="examination"></param>
        /// <param name="kecheng">课程(如果考试为阶段考试责需要传入此参数 否则为NULL)</param>
        /// <returns></returns>
        public List<AnswerQuestionView> productAnswerQuestion(Examination examination, int kecheng)
        {

            //获取专业

         
            TeacherClassBusiness dbteacheraclass = new TeacherClassBusiness();
          
            var examview = db_exam.ConvertToExaminationView(examination);

            //从缓存中获取数据

            RedisCache redisCache = new RedisCache();

            var list = new List<AnswerQuestionBank>();

            var redisdata = redisCache.GetCache<List<AnswerQuestionBank>>("AnswerQuestionBank");
            if (redisdata != null)
            {
                list.AddRange(redisdata);
            }
            

            if (list == null || list.Count == 0)
            {
                list.AddRange(db_answerQuextion.AllAnswerQuestion());
            }
            
            var templist = new List<AnswerQuestionView>();
            //foreach (var item in list)
            //{
            //    templist.Add(db_answerQuextion.ConvertToAnswerQuestionView(item, false));
            //}
            //筛选出S2的课程
            List<AnswerQuestionView> questionlist = new List<AnswerQuestionView>();


            List<Curriculum> sourchlist = new List<Curriculum>();
            if (examview.ExamType.ExamTypeID == 1)
            {

                var tempqulist = list.Where(d => d.Grand == examview.ExamType.GrandID && d.Course ==0).ToList();
                tempqulist.ForEach(d=>
                {
                    var tempobj = db_answerQuextion.ConvertToAnswerQuestionView(d, false);

                    if (tempobj != null) questionlist.Add(tempobj);
                });
            }

            if (examview.ExamType.ExamTypeID == 2)
            {

               // var course = db_Course.GetList().Where(d => d.IsDelete == false && d.CurriculumID == kecheng).FirstOrDefault();

                list = list.Where(d => d.Course !=0 && d.Course == kecheng).ToList();

                list.ForEach(d=>
                {
                    var tempobj = db_answerQuextion.ConvertToAnswerQuestionView(d, false);

                    if (tempobj != null) questionlist.Add(tempobj);

                });

                //if (course != null) sourchlist.Add(course);

                //foreach (var item in templist)
                //{
                //    foreach (var item1 in sourchlist)
                //    {

                //        if (item.Course.CurriculumID == item1.CurriculumID)
                //        {
                //            //判断是否存在
                //            if (!IsContain(questionlist, item))
                //            {
                //                questionlist.Add(item);
                //            }
                //        }

                //    }
                //}

            }

      
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/questionLevelConfig.xml"));

            var xmlRoot = xmlDocument.DocumentElement;

            //获取题目总数量
            int total = int.Parse(xmlRoot.GetElementsByTagName("answerQuestion")[0].FirstChild.InnerText);

            XmlElement qnser = (XmlElement)xmlRoot.GetElementsByTagName("answerQuestion")[0];
            var levels = qnser.GetElementsByTagName("level");
            XmlElement mylevel = null;

            foreach (XmlElement item in levels)
            {
                var levelid = item.Attributes["levelId"];

                if (int.Parse(levelid.Value) == examview.PaperLevel.LevelID)
                {
                    mylevel = item;
                }

            }
            // 简单题的个数
            float jiandan = 0;
            float putong = 0;
            float kunnan = 0;
            var temlist = mylevel.GetElementsByTagName("count");

            foreach (XmlElement item in temlist)
            {
                var levelid = item.Attributes["levelId"];
                if (int.Parse(levelid.Value) == 1)
                {
                    jiandan = int.Parse(item.Attributes["Percentage"].Value);
                    jiandan = total * (jiandan / 100);
                }

                if (int.Parse(levelid.Value) == 2)
                {
                    putong = int.Parse(item.Attributes["Percentage"].Value);
                    putong = total * (putong / 100);
                }

                if (int.Parse(levelid.Value) == 3)
                {
                    kunnan = int.Parse(item.Attributes["Percentage"].Value);
                    kunnan = total * (kunnan / 100);
                }
            }
            List<AnswerQuestionView> resultlist = new List<AnswerQuestionView>();
            var temp1list = questionlist.Where(d => d.Level.LevelID == 1).ToList();
            resultlist.AddRange(this.extracAnswerQuestion(temp1list, (int)jiandan));

            var temp1list1 = questionlist.Where(d => d.Level.LevelID == 2).ToList();
            resultlist.AddRange(this.extracAnswerQuestion(temp1list1, (int)putong));

            var temp1list2 = questionlist.Where(d => d.Level.LevelID == 3).ToList();
            resultlist.AddRange(this.extracAnswerQuestion(temp1list2, (int)kunnan));
            List<AnswerQuestionView> returnlist = new List<AnswerQuestionView>();

            //去掉重复项
            foreach (var item in resultlist)
            {
                if (!IsContain(returnlist, item))
                {
                    returnlist.Add(item);
                }
            }
            return returnlist;

        }


        public ComputerTestQuestionsView productComputerQuestion(Examination examination,int kecheng)
        {
            //获取专业

            var examview = db_exam.ConvertToExaminationView(examination);

            //从缓存中获取数据

            RedisCache redisCache = new RedisCache();

            var list = new List<MachTestQuesBank>();

            var redisdata = redisCache.GetCache<List<MachTestQuesBank>>("MachTestQuesBank");
            if (redisdata != null)
            {
                list.AddRange(redisdata);
            }

            if (list == null || list.Count == 0)
            {
                list.AddRange(db_computerQuestion.AllComputerTestQuestion());
            }

            var templist = new List<ComputerTestQuestionsView>();

            List<Curriculum> sourchlist = new List<Curriculum>();

            //foreach (var item in list)
            //{
            //    templist.Add(db_computerQuestion.ConvertToComputerTestQuestionsView(item,false));
            //}

            List<ComputerTestQuestionsView> questionlist = new List<ComputerTestQuestionsView>();

            if (examview.ExamType.ExamTypeID == 1)
            {
                var tempqulist  = list.Where(d => d.Grand == examview.ExamType.GrandID && d.Course == 0).ToList();

                tempqulist.ForEach(d=> {

                   var tempobj = db_computerQuestion.ConvertToComputerTestQuestionsView(d, false);

                    if (tempobj != null) questionlist.Add(tempobj);
                });

            }

            if (examview.ExamType.ExamTypeID == 2)
            {

                //var course = db_Course.GetList().Where(d => d.IsDelete == false && d.CurriculumID == kecheng).FirstOrDefault();

                list = list.Where(d => d.Course !=0 && d.Course == kecheng).ToList();

                list.ForEach(d=> {

                    var tempobj = db_computerQuestion.ConvertToComputerTestQuestionsView(d, false);

                    if (tempobj != null) questionlist.Add(tempobj);
                });


                //if (course != null) sourchlist.Add(course);


                ////获取S2的题目
                //foreach (var item in templist)
                //{
                //    foreach (var item1 in sourchlist)
                //    {
                //        if (!IsContain(questionlist, item))
                //        {
                //            if (item.Course.CurriculumID == item1.CurriculumID)
                //            {
                //                questionlist.Add(item);
                //            }
                //        }
                //    }
                //}


            }
           
            Random rendom = new Random();
           
            var reslist = questionlist.Where(d => d.Level.LevelID == examview.PaperLevel.LevelID).ToList();
            int rdom = rendom.Next(0, reslist.Count);
            return reslist[rdom];


        }
        public bool IsContain(List<AnswerQuestionView> sources, AnswerQuestionView item)
        {
            foreach (var obj in sources)
            {
                if (obj.ID == item.ID)
                {
                    return true;
                }
            }
            return false;

        }
        public bool IsContain(List<ComputerTestQuestionsView> sources, ComputerTestQuestionsView item)
        {
            foreach (var obj in sources)
            {
                if (obj.ID == item.ID)
                {
                    return true;
                }
            }
            return false;

        }

        public bool IsContain(List<ChoiceQuestionTableView> sources, ChoiceQuestionTableView item)
        {
            foreach (var obj in sources)
            {
                if (obj.Id == item.Id)
                {
                    return true;
                }
            }
            return false;

        }


        /// <summary>
        /// 获取XX难度的选择题
        /// </summary>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<ChoiceQuestionTableView> extracChoicequestion(List<ChoiceQuestionTableView> source, int count)
        {
            count = 5;
            if (source == null || source.Count == 0)
            {
                return new List<ChoiceQuestionTableView>();
            }
            Random random = new Random();

            List<ChoiceQuestionTableView> result = new List<ChoiceQuestionTableView>();


            for (int i = 0; i < count; i++)
            {
                int index = random.Next(0, source.Count);

                var question = source[index];

                //判断是否重复
                if (this.IsContain(result, question))
                {
                    //如果已经存在

                    i--;

                }
                else
                {
                    result.Add(question);
                }


            }

            return result;
        }

        public List<AnswerQuestionView> extracAnswerQuestion(List<AnswerQuestionView> source, int count)
        {
            if (source == null || source.Count == 0)
            {
                return new List<AnswerQuestionView>();
            }
            Random random = new Random();
            List<AnswerQuestionView> result = new List<AnswerQuestionView>();



            for (int i = 0; i < count; i++)
            {
                int index = random.Next(0, source.Count);
                var question = source[index];

                //判断是否重复
                if (this.IsContain(result, question))
                {
                    //如果已经存在

                    i--;

                }
                else
                {
                    result.Add(question);
                }

            }

            return result;
        }


        /// <summary>
        /// 选择题分配分数
        /// </summary>
        public List<object> distributionScores(List<ChoiceQuestionTableView> questionlist)
        {
            List<object> resultlist = new List<object>();


            //首先将题目的难以程度分类
            //分为 简单 普通 困难

            var grouplist =  questionlist.GroupBy(d => d.Level.LevelID).ToList();//分类后的题目

            //再读取出配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/questionLevelConfig.xml"));

            //根节点 root
            var xmlRoot = xmlDocument.DocumentElement;
            //拿到分数配置节点
           XmlElement scoreElem =  (XmlElement)xmlRoot.GetElementsByTagName("scores")[0];
            XmlElement choiceelem = (XmlElement)scoreElem.GetElementsByTagName("choiceQuestionScores")[0];

            //获取没有的题目类型

            List<int> NoCantainlist = new List<int>();

            var levellist = db_exam.AllQuestionLevel();

            foreach (var item in levellist)
            {
                try
                {
                    var temp = grouplist.Where(d => d.Key == item.LevelID).First();
                }
                catch (Exception ex)
                {
                    NoCantainlist.Add(item.LevelID);


                }
               
            }

            var temxmllist = choiceelem.GetElementsByTagName("questionLevel");
            float Percentage = 0; ///这是多余的比率额
            foreach (var item in NoCantainlist)
            {
                foreach (XmlElement item1 in temxmllist)
                {

                    var attr = item1.Attributes["levelid"].Value;
                    if (attr == item.ToString())
                    {
                        //获取去对应的分数额

                        Percentage += int.Parse(item1.Attributes["Percentage"].Value);
                    }
                   
                }
            }

            //比率初始化
            Dictionary<int, float> percentageDic = new Dictionary<int, float>();
            List<int> keys = new List<int>();
             foreach (IGrouping<int, ChoiceQuestionTableView> item in grouplist)
            {
                percentageDic.Add(item.Key, 0);
                keys.Add(item.Key);
            }

            //获取选择题总分
            int total =int.Parse( choiceelem.GetElementsByTagName("total")[0].InnerText);



            foreach (var item in keys)
            {
                //获取key

                var temxmllist1 = choiceelem.GetElementsByTagName("questionLevel");

                foreach (XmlElement item1 in temxmllist)
                {
                    var attr = item1.Attributes["levelid"].Value;
                    if (attr == item.ToString())
                    {
                        //获取去对应的分数额

                        var Percentage1 = int.Parse(item1.Attributes["Percentage"].Value);

                        //计算分数 每小题分数等于 题目类型总分*比率/个数

                        percentageDic[item] = Percentage1;

                       
                    }

                }
            }

            //加上多余的比率
            foreach (var item in percentageDic)
            {
                percentageDic[item.Key] += Percentage;

                break;
            }




            foreach (var item in percentageDic)
            {
                int count = 0;

                foreach (var item1 in grouplist)
                {
                    if (item1.Key == item.Key)
                    {
                        count = item1.Count();
                    }
                }

                //计算每个题目分数

                float itemscore = total * ((percentageDic[item.Key] / 100)) / count;
                var tempobj = new
                {

                    level = item.Key,
                    scores = itemscore

                };

                resultlist.Add(tempobj);
            }
            return resultlist;



        }


        /// <summary>
        /// 选择题分配分数
        /// </summary>
        public List<object> distributionScores(List<AnswerQuestionView> questionlist)
        {
            List<object> resultlist = new List<object>();


            //首先将题目的难以程度分类
            //分为 简单 普通 困难

            var grouplist = questionlist.GroupBy(d => d.Level.LevelID).ToList();//分类后的题目

            //再读取出配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/questionLevelConfig.xml"));

            //根节点 root
            var xmlRoot = xmlDocument.DocumentElement;
            //拿到分数配置节点
            XmlElement scoreElem = (XmlElement)xmlRoot.GetElementsByTagName("scores")[0];
            XmlElement choiceelem = (XmlElement)scoreElem.GetElementsByTagName("anwserQuestionScores")[0];

            //获取没有的题目类型

            List<int> NoCantainlist = new List<int>();

            var levellist = db_exam.AllQuestionLevel();

            foreach (var item in levellist)
            {
                try
                {
                    var temp = grouplist.Where(d => d.Key == item.LevelID).First();
                }
                catch (Exception ex)
                {
                    NoCantainlist.Add(item.LevelID);


                }

            }

            var temxmllist = choiceelem.GetElementsByTagName("questionLevel");
            float Percentage = 0; ///这是多余的比率额
            foreach (var item in NoCantainlist)
            {
                foreach (XmlElement item1 in temxmllist)
                {

                    var attr = item1.Attributes["levelid"].Value;
                    if (attr == item.ToString())
                    {
                        //获取去对应的分数额

                        Percentage += int.Parse(item1.Attributes["Percentage"].Value);
                    }

                }
            }

            //比率初始化
            Dictionary<int, float> percentageDic = new Dictionary<int, float>();
            List<int> keys = new List<int>();
            foreach (IGrouping<int, AnswerQuestionView> item in grouplist)
            {
                percentageDic.Add(item.Key, 0);
                keys.Add(item.Key);
            }

            //获取选择题总分
            int total = int.Parse(choiceelem.GetElementsByTagName("total")[0].InnerText);



            foreach (var item in keys)
            {
                //获取key

                var temxmllist1 = choiceelem.GetElementsByTagName("questionLevel");

                foreach (XmlElement item1 in temxmllist)
                {
                    var attr = item1.Attributes["levelid"].Value;
                    if (attr == item.ToString())
                    {
                        //获取去对应的分数额

                        var Percentage1 = int.Parse(item1.Attributes["Percentage"].Value);

                        //计算分数 每小题分数等于 题目类型总分*比率/个数

                        percentageDic[item] = Percentage1;


                    }

                }
            }

            //加上多余的比率
            foreach (var item in percentageDic)
            {
                percentageDic[item.Key] += Percentage;

                break;
            }



            foreach (var item in percentageDic)
            {
                int count = 0;

                foreach (var item1 in grouplist)
                {
                    if (item1.Key == item.Key)
                    {
                        count = item1.Count();
                    }
                }

                //计算每个题目分数

                float itemscore = total * ((percentageDic[item.Key] / 100)) / count;
                var tempobj = new
                {

                    level = item.Key,
                    scores = itemscore

                };

                resultlist.Add(tempobj);
            }
            return resultlist;



        }


        /// <summary>
        /// 获取学员的历史考试数据
        /// </summary>
        /// <param name="studentNo"></param>
        /// <returns></returns>
        public List<Examination> StuExaminationEnd(string studentNo)
        {
            var candlist = db_exam.AllCandidateInfo().Where(d => d.StudentID == studentNo).ToList();

            List<Examination> resultlist = new List<Examination>();

            foreach (var item in candlist)
            {
                var exam = db_exam.AllExamination().Where(d => d.ID == item.Examination).FirstOrDefault();

                if (exam != null)
                    resultlist.Add(exam);
            }

            return resultlist;


        }


        /// <summary>
        /// 答题卡信息
        /// </summary>
        /// <returns></returns>
        public AnswerSheetInfosView AnswerSheetInfos(int examid, string studentnumber)
        {
            AnswerSheetInfosView infosView = new AnswerSheetInfosView();

            //获取考生
            var candidateinfo = db_exam.AllCandidateInfo(examid).Where(d => d.StudentID == studentnumber).FirstOrDefault();

           var student = db_student.StudentList().Where(d => d.StudentNumber == studentnumber).FirstOrDefault();
            infosView.AnswerPerson = student;
            var exam = db_exam.AllExamination().Where(d => d.ID == examid).FirstOrDefault();
            infosView.BeginDate = exam.BeginDate;

            var examroomdistri = db_exam.AllExamroomDistributed(examid).Where(d => d.CandidateNumber == candidateinfo.CandidateNumber).FirstOrDefault();
           var examroom = db_exam.AllExaminationRoom().Where(d => d.Examination == examid && d.ID == examroomdistri.ExaminationRoom).FirstOrDefault();
            BaseBusiness<Classroom> dbclassroom = new BaseBusiness<Classroom>();
            infosView.Classroom = dbclassroom.GetIQueryable().Where(d => d.Id == examroom.Classroom_Id).FirstOrDefault();


            ///读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/questionLevelConfig.xml"));

            var xmlRoot = xmlDocument.DocumentElement;

            //读取分数设置
            var scoresXml = (XmlElement)xmlRoot.GetElementsByTagName("scores")[0];

            var computerSetxml = (XmlElement)scoresXml.GetElementsByTagName("computerQuestionScores")[0];

            var Computertotal = (float) (int.Parse( computerSetxml.GetElementsByTagName("total")[0].InnerText));


            infosView.ComputerQuestionScores = Computertotal;

            //获取选择题总分
            var choiceXmlset = (XmlElement)scoresXml.GetElementsByTagName("choiceQuestionScores")[0];
            var choicetotal = (float)(int.Parse(choiceXmlset.GetElementsByTagName("total")[0].InnerText));

            //获取解答题总分

            var answerXmlset = (XmlElement)scoresXml.GetElementsByTagName("anwserQuestionScores")[0];
            var answertotal = (float)(int.Parse(answerXmlset.GetElementsByTagName("total")[0].InnerText));

            infosView.WrittenQuestionScores = choicetotal + answertotal;

            infosView.TimeLimit = exam.TimeLimit;

            return infosView;
        }

    }
}
