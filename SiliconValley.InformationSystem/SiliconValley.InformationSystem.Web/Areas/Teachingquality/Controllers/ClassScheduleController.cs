using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.StageGrade_Business;
using SiliconValley.InformationSystem.Business.BaseDataEnum_Business;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using System.Web.Script.Serialization;
using SiliconValley.InformationSystem.Business.EnroExamination_Business;
using SiliconValley.InformationSystem.Depository.CellPhoneSMS;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Business.EducationalBusiness;
using SiliconValley.InformationSystem.Business.Shortmessage_Business;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using System.IO;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;

//班级管理
namespace SiliconValley.InformationSystem.Web.Areas.Teachingquality.Controllers
{

    [CheckLogin]
    public class ClassScheduleController : Controller
    {


        public static int counts = 0;
        private readonly ClassScheduleBusiness dbtext;
        public ClassScheduleController()
        {
            
            dbtext = new ClassScheduleBusiness();

        }
        //升学阶段
        BaseBusiness<GotoschoolStage> GotoschoolStageBusiness = new BaseBusiness<GotoschoolStage>();
        //拆班记录
        BaseBusiness<RemovalRecords> Dismantle = new BaseBusiness<RemovalRecords>();
        //学员信息
        StudentInformationBusiness student = new StudentInformationBusiness();

        //学员班级表
        ScheduleForTraineesBusiness Stuclass = new ScheduleForTraineesBusiness();
        //班主任带班
        BaseBusiness<HeadClass> HeadClassEnti = new BaseBusiness<HeadClass>();
        //班主任
        HeadmasterBusiness Hadmst = new HeadmasterBusiness();
        //成考管理
        EnroExaminationBusiness enroExaminationBusiness = new EnroExaminationBusiness();
        //短信模板
        ShortmessageBusiness shortmessageBusiness = new ShortmessageBusiness();
        //班级状态表
        BaseBusiness<Classstatus> classtatus = new BaseBusiness<Classstatus>();
        // GET: Teachingquality/ClassSchedule
        //主页面
        public ActionResult Index()
        {
            //公共类库
            BaseDataEnumManeger baseDataEnumManeger = new BaseDataEnumManeger();
            //专业课时段
            ViewBag.BaseDataEnum_Id = baseDataEnumManeger.GetsameFartherData("上课时间类型").Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() });
            //专业
            ViewBag.Major_Id = Techarcontext.GetList().Select(a => new SelectListItem { Text = a.SpecialtyName, Value = a.Id.ToString() });
            //阶段
            ViewBag.grade_Id = Grandcontext.GetList().Select(a => new SelectListItem { Text = a.GrandName, Value = a.Id.ToString() });
            
            var States = classtatus.GetList().Where(a => a.IsDelete == false).ToList();
          
            Classstatus classstatus = new Classstatus();
            classstatus.TypeName = "正常";
            classstatus.id = null;
            States.Add(classstatus);
            //班级状态
            ViewBag.ClassState = States.Select(a => new SelectListItem { Text = a.TypeName, Value = a.id==null? "null":a.id.ToString(),Selected=(a.id==null) });
            return View();
        }
        public ActionResult OddNumberMyclass()
        {
            return View();
        }
        //基础数据枚举数据
        BaseDataEnumBusiness BanseDatea = new BaseDataEnumBusiness();
        //当前登陆人
        Base_UserModel user = Base_UserBusiness.GetCurrentUser();
        //专业
        SpecialtyBusiness Techarcontext = new SpecialtyBusiness();
        //阶段
        GrandBusiness Grandcontext = new GrandBusiness();
       [HttpGet]
        //获取数据
        public ActionResult GetDate(int page ,int limit,string ClassNumber,string Major_Id,string grade_Id,string BaseDataEnum_Id,string ClassstatusID)
        {
            BaseBusiness<ScheduleForTraineesview> ScheduleForTraineesviewBusiness = new BaseBusiness<ScheduleForTraineesview>();
            try
            {
                EmployeesInfoManage employeesInfoManage = new EmployeesInfoManage();
                //岗位数据
                var positon = employeesInfoManage.GetPositionByEmpid(user.EmpNumber);
                List<ClassSchedule> list = new List<ClassSchedule>();
                List<ClassSchedule> list1 = new List<ClassSchedule>();
               var dbclass= dbtext.GetListBySql<ClassSchedule>("select *from ClassSchedule").ToList();
                if (Hadmst.GetList().Where(a => a.informatiees_Id == user.EmpNumber).FirstOrDefault() == null)
                {
                    list = dbclass.Where(a => a.IsDelete == false).ToList();
                }
                else
                {
                    
                    if (positon.PositionName.Contains("教质主任")|| positon.PositionName.Contains("教质副主任"))
                    {
                        //部门数据
                        var dept = employeesInfoManage.GetDept(positon.Pid);
                       var Grandlist= Grandcontext.GetList();
                        List<ClassSchedule> mylist = new List<ClassSchedule>();
                        if (dept.DeptName.Contains("s1"))
                        {
                          var x=  Grandlist.Where(a => a.GrandName=="S1"|| a.GrandName=="S2").ToList();
                            foreach (var item in x)
                            {
                                mylist.AddRange(dbclass.Where(a => a.grade_Id == item.Id).ToList());
                            }
                        }
                        else
                        {
                            var x = Grandlist.Where(a => a.GrandName == "S3").ToList();
                            foreach (var item in x)
                            {
                                mylist.AddRange(dbclass.Where(a => a.grade_Id == item.Id).ToList());
                            }
                        }
                        list = mylist;
                    }
                    else {
                        var HadnID = Hadmst.GetList().Where(c => c.informatiees_Id == user.EmpNumber && c.IsDelete == false).FirstOrDefault();
                        if (HadnID != null)
                        {
                            var x = HeadClassEnti.GetList().Where(a => a.IsDelete == false && a.LeaderID == HadnID.ID).ToList();
                            foreach (var item in x)
                            {
                                list.Add(dbclass.Where(a => a.IsDelete == false && a.id == item.ClassID).FirstOrDefault());
                            }
                        }
                    }

                }

                list1 = list;
                if (ClassstatusID=="")
                {
                    list1 = list1.Where(a => a.IsDelete == false).ToList();
                }
                else 
                {
                    if (!string.IsNullOrEmpty(ClassstatusID))
                    {
                        if (ClassstatusID!="null")
                        {
                            list1 = list.Where(a =>  a.IsDelete == false && a.ClassstatusID == int.Parse(ClassstatusID)).ToList();
                        }
                        else
                        {
                            list1 = list.Where(a => a.IsDelete == false && a.ClassstatusID ==null).ToList();
                        }
                    }
                        
                 
                }
                if (!string.IsNullOrEmpty(ClassNumber))
               {
                    list1 = list1.Where(a => a.ClassNumber.Contains(ClassNumber)).ToList();
                }
          
                if (!string.IsNullOrEmpty(Major_Id))
               {
                int maid = int.Parse(Major_Id);
                    list1 = list1.Where(a => a.Major_Id== maid).ToList();
               }
            if (!string.IsNullOrEmpty(grade_Id))
            {
                int maid = int.Parse(grade_Id);
                    list1 = list1.Where(a => a.grade_Id == maid).ToList();
            }
            if (!string.IsNullOrEmpty(BaseDataEnum_Id))
            {
                int maid = int.Parse(BaseDataEnum_Id);
                    list1 = list1.Where(a => a.BaseDataEnum_Id == maid).ToList();
            }

                var dataList = list1.Select(a => new
                {
                    //  a.BaseDataEnum_Id,
                    a.id,
                    ClassNumber = a.ClassNumber,
                    ClassRemarks = a.ClassRemarks,
                    ClassStatus = a.ClassStatus,
                    IsDelete = a.IsDelete,
                    grade_Id = Grandcontext.GetEntity(a.grade_Id).GrandName, //阶段id
                    BaseDataEnum_Id = BanseDatea.GetEntity(a.BaseDataEnum_Id).Name,//专业课时间
                    Major_Id = a.Major_Id == null ? "暂无专业" : Techarcontext.GetEntity(a.Major_Id).SpecialtyName,//专业

                    //  HeadmasterName = classtatus.GetList().Where(c => c.IsDelete == false && c.id == a.ClassstatusID).FirstOrDefault() != null?"班级已升学或毕业":dbtext.HeadSraffFine(a.id).EmployeeId==null?"未设置老师": dbtext.HeadSraffFine(a.id).EmpName,
                    //Hadmst.HeadmastaerClassFine(a.id)==null?"未设置班主任": Hadmst.ClassHeadmaster(a.id).EmpName,
                    IsBool = classtatus.GetList().Where(c => c.IsDelete == false && c.id == a.ClassstatusID).FirstOrDefault() == null ? "正常" : classtatus.GetList().Where(c => c.IsDelete == false && c.id == a.ClassstatusID).FirstOrDefault().TypeName,
                    stuclasss = ScheduleForTraineesviewBusiness.GetListBySql<ScheduleForTraineesview>("select * from ScheduleForTraineesview where Classid=" + a.id).Count()
              }).OrderByDescending(a => a.id).Skip((page - 1) * limit).Take(limit).ToList();
           
            //  var x = dbtext.GetList();
            var data = new
            {
                code = "",
                msg = "",
                count = list1.Count,
                data = dataList
            }; return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.加载数据);
                return Json("数据有误", JsonRequestBehavior.AllowGet);
            }
        }

         //开设班级页面
         [HttpGet]
        public ActionResult AddClassSchedule()
        {
            //公共类库
            BaseDataEnumManeger baseDataEnumManeger = new BaseDataEnumManeger();
            //GetsameFartherData
            //专业课时段
            ViewBag.BaseDataEnum_Id = baseDataEnumManeger.GetsameFartherData("上课时间类型").Select(a => new SelectListItem { Text = a.Name, Value = a.Id.ToString() });
            //专业
            ViewBag.Major_Id = Techarcontext.GetList().Select(a => new SelectListItem { Text = a.SpecialtyName, Value = a.Id.ToString() });
                //阶段
          ViewBag.grade_Id = Grandcontext.GetList().Select(a => new SelectListItem { Text = a.GrandName, Value = a.Id.ToString() });
            return View();
        }
        //添加班级数据
        [HttpPost]
        public ActionResult AddClassSchedule(ClassSchedule classSchedule)
        {
          AjaxResult  retus = null;
           var cou= dbtext.GetList().Where(a => a.ClassNumber == classSchedule.ClassNumber && a.IsDelete == false &&a.ClassStatus==false).Count();
            if (cou>0)
            {
                retus = new ErrorResult();
                retus.Msg = "班级名称重复";

                retus.Success = false;
                retus.ErrorCode = 300;
            }
            else
            {
                try
                {
                    classSchedule.ClassStatus = false;
                    classSchedule.IsDelete = false;
                    classSchedule.ClassImage = "ClassImages.jpg";
                    dbtext.Insert(classSchedule);
                  
                    retus = new SuccessResult();
                    retus.Success = true;
                    retus.Msg = "开设成功";
                    BusHelper.WriteSysLog("班级开设成功", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
                catch (Exception ex)
                {
                    retus = new ErrorResult();
                    retus.Msg = "服务器错误";

                    retus.Success = false;
                    retus.ErrorCode = 500;
                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
               

                }
            }
            
            return Json(retus, JsonRequestBehavior.AllowGet);
        }
        //查看班级的学员
        public ActionResult ClassStudent()
        {
            ReconcileManeger reconcileManeger = new ReconcileManeger();
           var classNumberss =int.Parse( Request.QueryString["ClassNumber"]);
            ViewBag.ClassName =dbtext.GetEntity(classNumberss).ClassNumber;
            ViewBag.GrandName= Grandcontext.GetEntity(dbtext.GetEntity(classNumberss).grade_Id).GrandName;
            ViewBag.ClassdetailsView = dbtext.Listdatails((int)classNumberss);
            ViewBag.ClassID = classNumberss;
            ViewBag.Members = dbtext.MembersList();
            ViewBag.Stage = dbtext.GetClassGrand((int)classNumberss, 234);
            ViewBag.Status= dbtext.GetEntity(classNumberss).ClassstatusID;
            ViewBag.Shiftchange = reconcileManeger.Further_education(classNumberss, "开学典礼");
            return View();
        }
        /// <summary>
        /// 获取班级学员的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassStuDate()
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            var x = dbtext.ClassStudentneViewList(ClassID);
            return Json(x, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取班级正常学员
        /// </summary>
        /// <returns></returns>
        public ActionResult ISClassStuDate(int page, int limit)
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            var x = dbtext.ClassStudentneViewList(ClassID).Where(a=>a.Statusname==null).ToList();
           var mystudent = x.OrderBy(a => a.StuNameID).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = x.Count,
                data = mystudent
            }; return Json(data, JsonRequestBehavior.AllowGet);

           
        }
        public ActionResult ListStudent()
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            var x = dbtext.ClassStudentneViewList(ClassID).Where(a => a.Statusname == null).Select(a=>new { a.StuNameID,a.Name,a.identitydocument,Sex=a.Sex==true?"男":"女"}).ToList();
            return Json(x, JsonRequestBehavior.AllowGet);
        }
        //群号操作页面
        public ActionResult Groupnumber()
        {
            int ClassNumber =int.Parse( Request.QueryString["ClassNumber"]);
            ViewBag.ClassNumber = ClassNumber;
            ViewBag.ClassName = dbtext.GetEntity(ClassNumber).ClassNumber;
            return View(dbtext.Grouselect(ClassNumber));
        }
        //群号数据操作
        public ActionResult GroupEntity(GroupManagement groupManagement)
        {
            return Json(dbtext.GroupAdd(groupManagement), JsonRequestBehavior.AllowGet);
        }
        public ActionResult text()
        {
            return View();
        }
        //获取班委
        public ActionResult DateMembers()
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            return Json(dbtext.ClassStudentneList(ClassID), JsonRequestBehavior.AllowGet);
        }
        //班委数据操作
        public ActionResult AddMembers()
        {
            //学员学号
            string Stuid = Request.QueryString["Stuid"];
            //班委名称
            string MenName = Request.QueryString["MenName"];
            //数据操作
            string Entity = Request.QueryString["Entity"];
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
         return Json(dbtext.Entityembers(Stuid, MenName, ClassID, Entity),JsonRequestBehavior.AllowGet);

        }
        //班会表单页面
        [HttpGet]
        public ActionResult AssmeetingsEntity()
        {
            //undefined
            //string ClassName = Request.QueryString["ClassName"];
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            string uid = Request.QueryString["id"];
            Assmeetings assmeetings = new Assmeetings();
            if (uid!= "undefined")
            {
                ViewBag.Name = "添加班会记录";
                assmeetings =  dbtext.AssmeetingsSelect(int.Parse(uid));
                ViewBag.ClassName = dbtext.GetEntity(assmeetings.ClassNumber).ClassNumber;
                return View(assmeetings);
            }
            else
            {
                ViewBag.Name = "编辑班会记录";
                assmeetings.ClassNumber =ClassID;
                ViewBag.ClassName = dbtext.GetEntity(assmeetings.ClassNumber).ClassNumber;
                return View(assmeetings);
            }
         
        }
        //班会数据操作方法
        [HttpPost]
        public ActionResult AssmeetingsEntity(Assmeetings assmeetings)
        {
            return Json(dbtext.EntityAssmeetings(assmeetings), JsonRequestBehavior.AllowGet);
        }

        //查看是否有相同的班委
        public ActionResult AssmeetingsBool()
        {
            //班委名称
            string MenName = Request.QueryString["MenName"];
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            //学号
            string Stuid = Request.QueryString["Stuid"];
            return Json(dbtext.AssmeetingsBool(MenName, ClassID, Stuid), JsonRequestBehavior.AllowGet);
        }
        //班级班会数据
        public ActionResult AssmeetingsGetDate(int page, int limit,string Title,string qBeginTime,string qEndTime)
        {
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);

            var dataList = dbtext.AssmeetingsList(ClassID) ;
            if (!string.IsNullOrEmpty(Title))
            {
                dataList = dataList.Where(a => a.Title.Contains(Title)).ToList();
            }
       
            if (!string.IsNullOrEmpty(qBeginTime))
            {
                dataList = dataList.Where(a => a.Classmeetingdate >= Convert.ToDateTime(qBeginTime)).ToList();
            }
            if (!string.IsNullOrEmpty(qEndTime))
            {
                dataList = dataList.Where(a => a.Classmeetingdate <= Convert.ToDateTime(qEndTime)).ToList();
            }
            dataList= dataList.OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = dataList.Count,
                data = dataList
            }; return Json(data, JsonRequestBehavior.AllowGet);
        
          
       
        }

        //转班业务
        public ActionResult Transferoftraunees()
        {
            StudentUnionBusiness SS = new StudentUnionBusiness();
            var x= SS.StudentList("aa");
            return View();
        }
        //拆班页面
        [HttpGet]
        public ActionResult Dismantleclasses()
        {
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);

            string studentID = Request.QueryString["StudentID"];
           string[] studentIDs= studentID.Split(',');
            var Dismantl=  Dismantle.GetList().Where(a => a.IsDelete == false).ToList();
            //获取当前班级数据对象
            var x = dbtext.FintClassSchedule(Stuclass.SutdentCLassName(studentIDs[0]).ID_ClassName);
          var Grdeid=  GotoschoolStageBusiness.GetList().Where(a => a.CurrentStageID == x.grade_Id).FirstOrDefault().NextStageID;
            var List = dbtext.ListGradeidenticals(Grdeid);
            foreach (var item in Dismantl)
            {
                List = List.Where(a => a.id != item.FormerClass).ToList();
            }
            ViewBag.List= List.Where(a=>a.id!= ClassID).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ClassNumber }).ToList();
            studentID = studentID.Substring(0, studentID.Length - 1);
            string[] stu = studentID.Split(',');
            List<StudentInformation> list = new List<StudentInformation>();
            foreach (var item in stu)
            {
                list.Add(student.GetEntity(item));
            }

            ViewBag.StudentID = studentID;
            ViewBag.ClassName = dbtext.FintClassSchedule(ClassID).ClassNumber;
            ViewBag.ClassID= ClassID;
            ViewBag.Mylist = list;


            return View();
        }
        //拆班数据操作
        [HttpPost]
        public ActionResult Dismantleclasses(string Addtime,int FormerClass,int List,string Reasong,string Remarks,string StudentID)
        {
           return Json(dbtext. Dismantleclasses(Addtime, FormerClass, List, Reasong, Remarks, StudentID),JsonRequestBehavior.AllowGet);

        }

        /// <summary>
       /// 验证当前班级是否有下一个阶段
       /// </summary>
       /// <param name="ClassID">班级编号</param>
       /// <returns></returns>
        public ActionResult PayMones(int ClassID)
        {
            return Json(dbtext.PayMones(ClassID), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 验证成考学员是否重复
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public ActionResult BoollistEnroExamination(string student)
        {
            //引入序列化
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //序列化
            var list = serializer.Deserialize<List<StudentInformation>>(student);

            return Json(enroExaminationBusiness.BoollistEnroExamination(list), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 验证班级名称是否重复
        /// </summary>
        /// <param name="id">班级名称</param>
        /// <returns></returns>
        public ActionResult ClassNameCount(string id)
        {
            return Json(dbtext.ClassNameCount(id), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 升学缴费情况
        /// </summary>
        /// <returns></returns>
        public ActionResult FindTuition(string id)
        {
            ViewBag.ClassName = id;
            return View();
        }
        /// <summary>
        /// 根据班级查询班上学生升学缴费情况
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="id">班级号</param>
        /// <returns></returns>
        public ActionResult listTuiton(int page, int limit,int id,string Studentid,string Name,string Isitinturn)
        {
            StudentFeeStandardBusinsess studentFeeStandardBusinsess = new StudentFeeStandardBusinsess();
            studentFeeStandardBusinsess.TuitionFine(id);
           var x= dbtext.listTuiton(id);
            if (!string.IsNullOrEmpty(Studentid))
            {
                x = x.Where(a => a.Stidentid == Studentid).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                x = x.Where(a => a.Name.Contains(Name)).ToList();
            }
            if (!string.IsNullOrEmpty(Isitinturn))
            {
                x = x.Where(a => a.Isitinturn== Isitinturn).ToList();
            }
            var da=  x.OrderBy(a => a.Stidentid).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
               count=x.Count,
                data = da
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult SMScharging()
        {
            var x = shortmessageBusiness.FineShortmessage("学费催费");
            ViewBag.horetem = x == null ? "" : x.content;
            return View();
        }
        // <summary>
        // 短信模板
        // </summary>
        // <param name = "Datailedcost" > 内容 </ param >
        // < returns ></ returns >
         [HttpPost]
       [ValidateInput(false)]
        public ActionResult SMScharging(string Datailedcost)
        {
            // 引入序列化
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //   string str = Datailedcost.Substring(3, Datailedcost.Length - 7);
            //Replace("Name", "替换").Replace("NextStageID","S2");
            // 序列化
            // var  personlist = serializer.Deserialize<List<DetailedcostView>>(Datailedcost);
            //return dbtext.SMScharging(personlist);
            return Json(dbtext.EntiShortmessage(Datailedcost), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 短信催费
        /// </summary>
        /// <param name="Datailedcost">字符串集合数据</param>
        /// <returns></returns>
        public ActionResult SendoutSMScharging(string Datailedcost)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var personlist = serializer.Deserialize<List<DetailedcostView>>(Datailedcost);
            return Json(dbtext.SMScharging(personlist), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// S2,S3升学
        /// </summary>
        /// <returns></returns>
        public ActionResult EntitAddClassSchedule(int ClassName)
        {
       
            return Json(dbtext.EntitAddClassSchedule(ClassName), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// S4班级毕业
        /// </summary>
        /// <param name="ClassID">班级编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ClassEnd(int ClassID)
        {
            return Json(dbtext.ClassEnd(ClassID), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 转班申请数据表单及打印
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Shiftwork()
        {
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            //是否打印
            string Types = Request.QueryString["Types"];
            if (Types == "1")
            {
                ViewBag.Types = Types;
                int ID = int.Parse(Request.QueryString["ID"]);
                var Classview = dbtext.Transactiondetails(ID);
                Classview.NowCLassName = dbtext.GetEntity(Classview.NowCLass).ClassNumber;
                return View(Classview);
            }
            string StudentID = Request.QueryString["StudentID"];
            var Dismantl = Dismantle.GetList().Where(a => a.IsDelete == false).ToList();
            //var List = dbtext.ListGradeidentical(Stuclass.SutdentCLassName(StudentID).ID_ClassName);
         
            //foreach (var item in Dismantl)
            //{
            //    List = List.Where(a => a.id != item.FormerClass).ToList();
            //}
            //ViewBag.List = List.Where(a => a.id != ClassID).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ClassNumber }).ToList();
           return View(dbtext.ShiftworkFine(StudentID));
        }
        /// <summary>
        /// 转班申请数据提交
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Shiftwork(TransactionView transactionView)
        {
            return Json(dbtext.Shiftwork(transactionView), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 通过班级获取该班级所有异动数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult TransactionDate(int page, int limit, string TypeName, string StudentID, string Name,string IsaDopt)
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
           
            return Json(dbtext.TransactionDate(page,limit,ClassID,TypeName,StudentID,Name, IsaDopt),JsonRequestBehavior.AllowGet);
        }
        /// <summary>
       /// 学员保险
       /// </summary>
       /// <returns></returns>
        public ActionResult Insurance()
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            ViewBag.ClassID = ClassID;
            return View();
        }
        /// <summary>
        /// 获取保险数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult premiumGetdate(int page, int limit)
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            var list = dbtext.premiumGetdate(ClassID).Select(a => new InsuranceView
            {
                 ID=a.ID,
                ClassNumber = dbtext.GetEntity(ClassID).ClassNumber,
                Dateofbirth = (DateTime)student.GetEntity(a.StudentID).BirthDate,
                Duedate = a.Endtime,
                Guardiansphone = student.GetEntity(a.StudentID).Familyphone,
                IDcardNo = student.GetEntity(a.StudentID).identitydocument,
                NameofGuardian = student.GetEntity(a.StudentID).Guardian.Split(',')[0],
                Nameofinsurer = student.GetEntity(a.StudentID).Name,
                premium = a.InsurancePremium,
                Sex = student.GetEntity(a.StudentID).Sex,
                Startdate = a.Starttime,
            
                Telephonenumber = student.GetEntity(a.StudentID).Telephone
            }).ToList();
                
            var Mylist=  list .OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = Mylist
            }; return Json(data, JsonRequestBehavior.AllowGet);
        
         
       }
        /// <summary>
        /// 获取保险及班级所以学员
        /// </summary>
        /// <param name="ClassID">班级id</param>
        /// <returns></returns>
        public ActionResult InsuranceStudent(int ClassID)
        {
            return Json(dbtext.InsuranceStudent(ClassID), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加保险页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult InsuranceAdd()
        {

            List<StudentInformation> studentlist = new List<StudentInformation>();
           string Students= Request.QueryString["StudentID"];
            
            string StuID = Students.Substring(0, Students.Length - 1);
            ViewBag.Student = StuID;
           
          string [] stu= StuID.Split(',');
            InsuranceView insuranceView = new InsuranceView();
            insuranceView.ClassNumber = Stuclass.SutdentCLassName(stu[0]).ClassID;
            foreach (var item in stu)
            {
                StudentInformation information = new StudentInformation();
                information.Name = student.GetEntity(item).Name;
                studentlist.Add(information);
            }
            ViewBag.StudentList = studentlist;
            return View(insuranceView);
        }
        //学员保险业务
        private BaseBusiness<DetailedStudentIn> detailedstudentinBusiness;
        /// <summary>
        /// 保险数据添加
        /// </summary>
        /// <param name="insuranceView">数据对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult InsuranceAdd(InsuranceView insuranceView)
        {
            return Json(dbtext.InsuranceAdd(insuranceView), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
          /// 保险数据信息补充表单
          /// </summary>
          /// <returns></returns>
         
        [HttpGet]
        public ActionResult SupplementInsuran()
        {
            detailedstudentinBusiness= new BaseBusiness<DetailedStudentIn>();
            List<StudentInformation> studentlist = new List<StudentInformation>();
            string Students = Request.QueryString["ID"];

            string StuID = Students.Substring(0, Students.Length - 1);
            ViewBag.Student = StuID;

            string[] stu = StuID.Split(',');
            InsuranceView insuranceView = new InsuranceView();
            
            foreach (var item in stu)
            {
              var StuIDs =  detailedstudentinBusiness.GetEntity(int.Parse(item));
                StudentInformation information = new StudentInformation();
                insuranceView.ClassNumber = Stuclass.SutdentCLassName(StuIDs.StudentID).ClassID;
                information.Name = student.GetEntity(StuIDs.StudentID).Name;
                studentlist.Add(information);
            }
            ViewBag.StudentList = studentlist;
            return View(insuranceView);
        }
        /// <summary>
        /// 添加保险
        /// </summary>
        /// <param name="insuranceView"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SupplementInsuran(InsuranceView insuranceView)
        {
            return Json(dbtext.SupplementInsuran(insuranceView), JsonRequestBehavior.AllowGet);
        }
        public ActionResult PhoneSMS()
        {
            string str = "<p>湖南硅谷高科软件学员逾期缴费学员通知：</p><p>{Name}家长：您好！经财务核查，您孩子{NextStageID}阶段升学费用逾期未缴，应交{ShouldJiao}元，欠费{Surplus}元。请您于本周内经财务办理缴费手续，逾期不缴教务处将根据学员管理规定予以听课处理。感谢您的配合与理解！</p><p>班主任：{HeadmasterName}&nbsp; 电话：{Phone}</p><p><br/></p>";
             dbtext.PhoneSMS("15073315702",str);
            return null;
        }
        /// <summary>
        /// 学员欠费页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Studententrancefee()
        {
            ViewBag.Stages = Grandcontext.GetList().Select(a => new SelectListItem { Text = a.GrandName, Value = a.Id.ToString() });
            ViewBag.ClassID = Request.QueryString["ClassID"];
            return View();
        }
        /// <summary>
        /// 获取学员未交阶段的欠费
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult Tuitionfeeforentrancestudy(int page, int limit,string Stidentid,string Name,string StagesID)
        {
            StudentFeeStandardBusinsess studentFeeStandardBusinsess = new StudentFeeStandardBusinsess();
            var ClassID = int.Parse(Request.QueryString["ClassID"]);
            var list = studentFeeStandardBusinsess.TuitionFine_list(ClassID);
            if (!string.IsNullOrEmpty(Stidentid))
            {
                list = list.Where(a => a.Stidentid == Stidentid).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name)).ToList();
            }
            if (!string.IsNullOrEmpty(StagesID))
            {
                int staid = int.Parse(StagesID);
                list = list.Where(a => a.StagesID == staid).ToList();
            }
            var Myx = list.OrderBy(a => a.Stidentid).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = Myx
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取学员所有欠费
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult Arrearge(int page, int limit, string Stidentid, string Name, string StagesID,string ClassName)
        {
            StudentFeeStandardBusinsess studentFeeStandardBusinsess = new StudentFeeStandardBusinsess();

            var list = studentFeeStandardBusinsess.StudentArreargeList();
            //List<DetailedcostView> list = new List<DetailedcostView>();
            if (!string.IsNullOrEmpty(Stidentid))
            {
                list = list.Where(a => a.Stidentid == Stidentid).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name)).ToList();
            }
            if (!string.IsNullOrEmpty(StagesID))
            {
                int staid = int.Parse(StagesID);
                list = list.Where(a => a.StagesID == staid).ToList();
            }
            if (!string.IsNullOrEmpty(ClassName))
            {
                list = list.Where(a => a.ClassName == ClassName).ToList();
            }
            var Myx = list.OrderBy(a => a.Stidentid).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = Myx
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 升学成绩页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Entranceexaminationresults()
        {
            ViewBag.ClassID = Request.QueryString["ClassID"];
            return View();
        }
        public ActionResult EntranceexaminationresultsDate(int page, int limit,string Totalscore,string number,string Name)
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            int grade_Id = dbtext.GetEntity(ClassID).grade_Id;
            ExamScoresBusiness examScoresBusiness = new ExamScoresBusiness();
       
            var x = examScoresBusiness.ClassScores(ClassID, grade_Id).Select(a => new {

                a.StudentNumber,
                a.StudentName,
                student.GetEntity(a.StudentNumber).identitydocument,
                Sex = student.GetEntity(a.StudentNumber).Sex == false ? "女" : "男",
                a.Score.OnBoard,
                Writtenexamination = a.Score.ChooseScore + a.Score.TextQuestionScore,
                Totalscore = a.Score.OnBoard + a.Score.ChooseScore + a.Score.TextQuestionScore
          }).ToList();
            if (!string.IsNullOrEmpty(Totalscore))
            {
                int totalscore = int.Parse(Totalscore);
                x = x.Where(a => a.Totalscore >= totalscore).ToList();
            }
            if (!string.IsNullOrEmpty(number))
            {
                x = x.Where(a => a.StudentNumber == number).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                x = x.Where(a => a.StudentName.Contains(Name)).ToList();
            }
            var Myx = x.OrderBy(a => a.StudentNumber).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = x.Count,
                data = Myx
            };
            return Json(data, JsonRequestBehavior.AllowGet);
           
        }

        //对象存储boss
        CloudstorageBusiness cloudstorageBusiness = new CloudstorageBusiness();

        //添加活动视图
        public ActionResult AddClassactivities()
        {
            return View();
        }
        /// <summary>
        /// 我的班级
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Myclass()
        {
            return View(dbtext.Headteacherdetails());
        }
        /// <summary>
        /// 获取当前登陆人的班级
        /// </summary>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EmpClass(string EndTime)
        {
           bool  End=   EndTime == null ? true : false;
            var EmpNumber = Base_UserBusiness.GetCurrentUser().EmpNumber;
            var id = Hadmst.GetList().Where(a => a.informatiees_Id == EmpNumber && a.IsDelete == false).FirstOrDefault().ID;

            var dataList = Hadmst.EmpClass(id, End).Select(a => new
            {
                //  a.BaseDataEnum_Id,
                a.id,
                ClassNumber = a.ClassNumber,
                ClassRemarks = a.ClassRemarks,
                ClassStatus = a.ClassStatus,
                IsDelete = a.IsDelete,
                ClassImage= cloudstorageBusiness.ImagesFine("xinxihua", "ClassImages", a.ClassImage,2),
                grade_Id = Grandcontext.GetEntity(a.grade_Id).GrandName, //阶段id
                
            //Hadmst.HeadmastaerClassFine(a.id)==null?"未设置班主任": Hadmst.ClassHeadmaster(a.id).EmpName,
                IsBool = classtatus.GetList().Where(c => c.IsDelete == false && c.id == a.ClassstatusID).FirstOrDefault() == null ? "正常" : classtatus.GetList().Where(c => c.IsDelete == false && c.id == a.ClassstatusID).FirstOrDefault().TypeName,
                stuclasss = Stuclass.GetList().Where(c => c.ID_ClassName == a.id && c.CurrentClass == true).Count()//班级人数
            }).ToList();
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 更改班级照片页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassImages(int id)
        {
            var a = dbtext.GetEntity(id);
           a.ClassImage= cloudstorageBusiness.ImagesFine("xinxihua", "ClassImages", a.ClassImage, 2);
            return View(a);
        }
        /// <summary>
        /// 更新班级照片
        /// </summary>
        /// <param name="id">班级id</param>
        /// <returns></returns>
        public ActionResult AddClassImages(int id)
        {
            var cl = dbtext.GetEntity(id);

            AjaxResult result = new AjaxResult();
            var fien = Request.Files[0];
            string filename = fien.FileName;
            string Extension = Path.GetExtension(filename);
            string newfilename = id  + Extension;
            try
            {
                    cl.ClassImage = newfilename;
                    dbtext.Update(cl);
                    result = new SuccessResult();
                    result.ErrorCode = 200;
                  cloudstorageBusiness.PutObject("xinxihua", "ClassImages", newfilename, fien.InputStream);
                    }
            catch (Exception ex)
            {
                result = new SuccessResult();
                result.ErrorCode = 300;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
            
        }
        /// <summary>
        /// 获取班主任带班历史记录
        /// </summary>
        /// <returns></returns>
        public ActionResult EmpHearClass()
        {
            var EmpNumber = Base_UserBusiness.GetCurrentUser().EmpNumber;
            var id = Hadmst.GetList().Where(a => a.informatiees_Id == EmpNumber && a.IsDelete == false).FirstOrDefault().ID;
            var c = Hadmst.EmpClass(id, true);
            var x = Hadmst.EmpClass(id, false);
            foreach (var item in c)
            {
               var x1 = x.Where(a => a.id == item.id).ToList();
                foreach (var item1 in x1)
                {
                    x.Remove(item1);
                }
            }
         x .Select(a=>new {
                ClassNumber=a.ClassNumber+"(" +Grandcontext.GetEntity(a.grade_Id).GrandName+")",
                a.id
            }).Distinct().ToList();
           
         
            return Json(x,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 合班
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Combinedclasses()
        {
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);

            string studentID = Request.QueryString["StudentID"];
            string[] studentIDs = studentID.Split(',');
            var Dismantl = Dismantle.GetList().Where(a => a.IsDelete == false).ToList();
            //获取当前班级数据对象
            var x = dbtext.FintClassSchedule(Stuclass.SutdentCLassName(studentIDs[0]).ID_ClassName);
          
            var List = dbtext.ListGradeidenticals(x.grade_Id);
            foreach (var item in Dismantl)
            {
                List = List.Where(a => a.id != item.FormerClass).ToList();
            }
            ViewBag.List = List.Where(a => a.id != ClassID).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ClassNumber }).ToList();
            studentID = studentID.Substring(0, studentID.Length - 1);
            string[] stu = studentID.Split(',');
            List<StudentInformation> list = new List<StudentInformation>();
            foreach (var item in stu)
            {
                list.Add(student.GetEntity(item));
            }

            ViewBag.StudentID = studentID;
            ViewBag.ClassName = dbtext.FintClassSchedule(ClassID).ClassNumber;
            ViewBag.ClassID = ClassID;
            ViewBag.Mylist = list;
            return View();
        }
        /// <summary>
        /// 合班业务
        /// </summary>
        /// <param name="Addtime"></param>
        /// <param name="FormerClass"></param>
        /// <param name="List"></param>
        /// <param name="Reasong"></param>
        /// <param name="Remarks"></param>
        /// <param name="StudentID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Combinedclasses(string Addtime, int FormerClass, int List, string Reasong, string Remarks, string StudentID)
        {
            return Json(dbtext.Combinedclasses(Addtime, FormerClass, List, Reasong, Remarks, StudentID), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 转班
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Shift()
        {

            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);

            string studentID = Request.QueryString["StudentID"];
            string[] studentIDs = studentID.Split(',');
            var Dismantl = Dismantle.GetList().Where(a => a.IsDelete == false).ToList();
            //获取当前班级数据对象
            var x = dbtext.FintClassSchedule(Stuclass.SutdentCLassName(studentIDs[0]).ID_ClassName);

            var List = dbtext.ListGradeidenticals(x.grade_Id);
            foreach (var item in Dismantl)
            {
                List = List.Where(a => a.id != item.FormerClass).ToList();
            }
            ViewBag.List = List.Where(a => a.id != ClassID).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ClassNumber }).ToList();
            studentID = studentID.Substring(0, studentID.Length - 1);
            string[] stu = studentID.Split(',');
            List<StudentInformation> list = new List<StudentInformation>();
            foreach (var item in stu)
            {
                list.Add(student.GetEntity(item));
            }

            ViewBag.StudentID = studentID;
            ViewBag.ClassName = dbtext.FintClassSchedule(ClassID).ClassNumber;
            ViewBag.ClassID = ClassID;
            ViewBag.Mylist = list;
            return View();
        }
        [HttpPost]
        public ActionResult Shift(int FormerClass, int List,string StudentID)
        {
            return Json(dbtext.Shift(FormerClass,List, StudentID), JsonRequestBehavior.AllowGet);

        }
        //public ActionResult xxx()
        //{
        //    EmployeesInfoManage employeesInfoManage = new EmployeesInfoManage();
        //    //岗位数据
        //    var positon = employeesInfoManage.GetPositionByEmpid(user.EmpNumber);
        //    if (positon.PositionName.Contains("教质主任") || positon.PositionName.Contains("教质副主任"))
        //    {
        //        //部门数据
        //        var dept = employeesInfoManage.GetDept(positon.Pid);
        //        var Grandlist = Grandcontext.GetList();
        //        List<ClassSchedule> mylist = new List<ClassSchedule>();
        //        if (dept.DeptName.Contains("s1"))
        //        {
        //            var x = Grandlist.Where(a => a.GrandName == "S1" || a.GrandName == "S2").ToList();
        //            foreach (var item in x)
        //            {
        //                mylist.AddRange(dbclass.Where(a => a.grade_Id == item.Id).ToList());
        //            }
        //        }
        //        else
        //        {
        //            var x = Grandlist.Where(a => a.GrandName == "S3").ToList();
        //            foreach (var item in x)
        //            {
        //                mylist.AddRange(dbclass.Where(a => a.grade_Id == item.Id).ToList());
        //            }
        //        }
        //        list = mylist;
        //    }
        //    else
        //    {
        //        var HadnID = Hadmst.GetList().Where(c => c.informatiees_Id == user.EmpNumber && c.IsDelete == false).FirstOrDefault();
        //        if (HadnID != null)
        //        {
        //            var x = HeadClassEnti.GetList().Where(a => a.IsDelete == false && a.LeaderID == HadnID.ID).ToList();
        //            foreach (var item in x)
        //            {
        //                list.Add(dbclass.Where(a => a.ClassStatus == false && a.IsDelete == false && a.id == item.ClassID).FirstOrDefault());
        //            }
        //        }
        //    }
        //}
    }
}