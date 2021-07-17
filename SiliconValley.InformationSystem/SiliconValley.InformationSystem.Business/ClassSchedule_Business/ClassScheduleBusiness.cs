using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Entity.Base_SysManage;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Business.FinaceBusines;
using SiliconValley.InformationSystem.Depository.CellPhoneSMS;
using SiliconValley.InformationSystem.Business.Shortmessage_Business;
using SiliconValley.InformationSystem.Business.EducationalBusiness;
using SiliconValley.InformationSystem.Business.DormitoryBusiness;
using SiliconValley.InformationSystem.Business.ClassDynamics_Business;
using SiliconValley.InformationSystem.Business.Employment;
using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
using SiliconValley.InformationSystem.Business.DormitoryMantainBusiness;

namespace SiliconValley.InformationSystem.Business.ClassSchedule_Business
{
    public class ClassScheduleBusiness : BaseBusiness<ClassSchedule>
    {
        //时间段
        public BaseDataEnumManeger BaseDataEnum_Entity = new BaseDataEnumManeger();
        //学生委员职位
        BaseBusiness<Members> MemBers = new BaseBusiness<Members>();
        //专业
        SpecialtyBusiness Techarcontext = new SpecialtyBusiness();
        //阶段
        GrandBusiness Grandcontext = new GrandBusiness();
        //学生委员
        BaseBusiness<ClassMembers> Business = new BaseBusiness<ClassMembers>();
        //根据班级号查询出所有学员
        ScheduleForTraineesBusiness ss = new ScheduleForTraineesBusiness();
        //班级群管理
        BaseBusiness<GroupManagement> GetBase = new BaseBusiness<GroupManagement>();
        //班会
        BaseBusiness<Assmeetings> myassmeetings = new BaseBusiness<Assmeetings>();
        //拆班记录
        BaseBusiness<RemovalRecords> Dismantle = new BaseBusiness<RemovalRecords>();
        //班级异动表
        BaseBusiness<ClassDynamics> CLassdynamic = new BaseBusiness<ClassDynamics>();
        //升学阶段
        BaseBusiness<GotoschoolStage> GotoschoolStageBusiness = new BaseBusiness<GotoschoolStage>();
        //费用明目
        CostitemsBusiness costitemsBusiness = new CostitemsBusiness();
        //学员费用
        BaseBusiness<StudentFeeRecord> studentfee = new BaseBusiness<StudentFeeRecord>();
        //班主任
        HeadmasterBusiness Hadmst = new HeadmasterBusiness();
        //学员信息
        StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
        //短信模板
        ShortmessageBusiness shortmessageBusiness = new ShortmessageBusiness();
        //班级状态
        BaseBusiness<Classstatus> classtatus = new BaseBusiness<Classstatus>();
        //转班申请业务类
        BaseBusiness<Transfer> TransferBusiness = new BaseBusiness<Transfer>();
        //学员异动类
        ClassDynamicsBusiness classDynamicsBusiness = new ClassDynamicsBusiness();
        //学员状态基础数据
        BaseBusiness<Basicdat> BasicdatBusiness = new BaseBusiness<Basicdat>();
        //试学业务
        BaseBusiness<Trialapplication> TrialapplicationBusiness = new BaseBusiness<Trialapplication>();
        //复学业务
        BaseBusiness<Restudy> RestudyBusines = new BaseBusiness<Restudy>();
        //退学业务类
        BaseBusiness<ApplicationDropout> ApplicationDropoutBusiness = new BaseBusiness<ApplicationDropout>();
        //重修业务类
        BaseBusiness<ApplicationRepair> ApplicationRepairBusiness = new BaseBusiness<ApplicationRepair>();
        //休学业务类
        BaseBusiness<Suspensionofschool> SuspensionofschoolBusiness = new BaseBusiness<Suspensionofschool>();
        //开除业务类
        BaseBusiness<Expels> ExpelsBusiness = new BaseBusiness<Expels>();
        //员工表
        EmployeesInfoManage employeesInfoManage = new EmployeesInfoManage();
        //班主任带班
        BaseBusiness<HeadClass> Hoadclass = new BaseBusiness<HeadClass>();

        //学生居住信息
        private AccdationinformationBusiness Accdation = new AccdationinformationBusiness ();
        private PricedormitoryarticlesManeger PriceManger = new PricedormitoryarticlesManeger();
        //private DormitoryDepositManeger Dormitory_Entity = new DormitoryDepositManeger();
        BaseBusiness<DormitoryDeposit> Dormitory_Entity = new BaseBusiness<DormitoryDeposit>();
        //public 

        /// <summary>
        /// 根据班级表id 查询班级数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClassSchedule Getgrade_idById(int id)
        {
            return GetList().Where(a => a.id==id && a.ClassStatus==false && a.IsDelete==false).FirstOrDefault();
        }

        /// <summary>
        /// 通过班级名称获取学号，姓名，职位
        /// </summary>
        /// <returns></returns>
        public List<ClassStudentView> ClassStudentneList(int classid)
        {

            List<ClassStudentView> listview = new List<ClassStudentView>();

            var list = this.ClassStudentneViewList(classid);
            foreach (var item in list)
            {
                List<ClassStudentView> Nameofmember = new List<ClassStudentView>();
                ClassStudentView classStudentView = new ClassStudentView();
                if (Business.GetList().Where(a => a.Studentnumber == item.StuNameID && a.IsDelete == false && a.ClassNumber == classid).ToList().Count > 0)
                {
                    var mylist = Business.GetList().Where(a => a.Studentnumber == item.StuNameID && a.IsDelete == false).ToList();
                    foreach (var item1 in mylist)
                    {
                        ClassStudentView view = new ClassStudentView();
                        view.Nameofmembers = MemBers.GetList().Where(c => c.ID == item1.Typeofposition && c.IsDelete == false).FirstOrDefault().Nameofmembers;
                        Nameofmember.Add(view);
                    }
                    classStudentView.Nameofmembers = Nameofmember;
                }

                classStudentView.Name = item.Name;
                classStudentView.Sex = (bool)item.Sex;

                classStudentView.StuNameID = item.StuNameID;
                if (classStudentView != null)
                {
                    listview.Add(classStudentView);
                }
            }
            return listview;



        }
        /// <summary>
        /// 获取所有能正常使用的班级
        /// </summary>
        /// <returns></returns>
        public List<ClassSchedule> ClassList()
        {
          return  this.GetList().Where(a => a.ClassstatusID == null).ToList();
        }
        /// <summary>
        /// 获取已毕业的班级
        /// </summary>
        /// <returns></returns>
        public List<ClassSchedule> ClassListGraduation()
        {
            return this.GetList().Where(a => a.ClassstatusID !=null && a.ClassstatusID==3).ToList();
        }
        /// <summary>
        /// 排除相同数据
        /// </summary>
        /// <param name="scheduleForTrainees"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        public bool show(List<ScheduleForTrainees> scheduleForTrainees, ScheduleForTrainees schedule )
        {
            foreach (var item in scheduleForTrainees)
            {
                if (item.ID_ClassName==schedule.ID_ClassName&&item.StudentID==schedule.StudentID)
                {
                    return true;
                }
            }
            return false;
          
        }
        /// <summary>
        /// 获取班级学员数据+异动学员
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        public List<ClassStudentView> ClassStudentneViewList(int classid)
        {
            //学员班级
            ScheduleForTraineesBusiness scheduleForTraineesBusinessz = new ScheduleForTraineesBusiness();
          var scheduleForTraineesBusiness=  scheduleForTraineesBusinessz.GetListBySql<ScheduleForTrainees>("select * from ScheduleForTrainees");
            //学员信息表
            StudentInformationBusiness student = new StudentInformationBusiness();

            List<ClassStudentView> listview = new List<ClassStudentView>();

            List<ScheduleForTrainees> scheduleFors = new List<ScheduleForTrainees>();
           
            var x = scheduleForTraineesBusiness.Where(a => a.ID_ClassName == classid&&a.CurrentClass==true).ToList();
            
            var y = scheduleForTraineesBusiness.Where(a => a.ID_ClassName == classid && a.CurrentClass == false).ToList();
            foreach (var item in y)
            {
                if (!this.show(scheduleFors, item))
                {
                    scheduleFors.Add(item);
                }
            }
            y = scheduleFors;
            for (int i = y.Count-1; i>=0 ; i--)
            {
                if (y.Count > 0)
                {
                    foreach (var item1 in x)
                    {
                        if (item1.StudentID == y[i].StudentID && item1.ID_ClassName == y[i].ID_ClassName)
                        {
                            y.Remove(y[i]);
                            break;
                        }
                    }
                }
            }

            x.AddRange(y);

            foreach (var item in x)
            {
                var stu = student.GetEntity(item.StudentID);
                ClassStudentView classStudentView = new ClassStudentView();
                classStudentView.Name = stu.Name;
                classStudentView.StuNameID = stu.StudentNumber;
                classStudentView.Sex = stu.Sex;
                classStudentView.identitydocument = stu.identitydocument;
                if (item.CurrentClass == false)
                {
                    var z = scheduleForTraineesBusiness.Where(a => a.StudentID == item.StudentID&&a.CurrentClass==true).FirstOrDefault();
                    
                    if (z!=null)
                    {
                        classStudentView.ClassNameView = z.ClassID;
                    }
                    else
                    {
                     var ClaStudent= scheduleForTraineesBusiness.Where(a => a.CurrentClass == false && a.IsGraduating == true&&a.StudentID==item.StudentID).FirstOrDefault();
                        if (ClaStudent!=null)
                        {
                            classStudentView.Statusname = "毕业";
                            classStudentView.ClassID = ClaStudent.ID_ClassName;
                        }
                        else
                        {
                            var Dyan = classDynamicsBusiness.GetList().Where(a => a.Studentnumber == classStudentView.StuNameID && a.IsaDopt == true).ToList().OrderByDescending(a => a.ID).FirstOrDefault();
                            classStudentView.Statusname = BasicdatBusiness.GetEntity(Dyan.States).Name;
                            classStudentView.ClassID = Dyan.FormerClass;
                        }
                       
                  
                    }
                }
                listview.Add(classStudentView);
            }
            return listview;



        }
        /// <summary>
        /// 通过班级名称获取一个正常班级
        /// </summary>
        /// <param name="ClassName">班级名称</param>
        /// <returns></returns>
        public ClassSchedule FintClassSchedule(int ClassID)
        {
            return this.GetEntity(ClassID) ;
        }
        /// <summary>
        /// 根据班级查询返回出班级人数,微信号,QQ号,班级名称
        /// </summary>
        /// <param name="claassid"></param>
        /// <returns></returns>
        public List<ClassdetailsView> Listdatails(int claassid)
        {
            BaseBusiness<ScheduleForTraineesview> ScheduleForTraineesviewBusiness = new BaseBusiness<ScheduleForTraineesview>();
            int count = ScheduleForTraineesviewBusiness.GetListBySql<ScheduleForTraineesview>("select * from ScheduleForTraineesview where Classid=" + claassid).Count();
            List<ClassdetailsView> list = new List<ClassdetailsView>();
            ClassdetailsView classdetailsView = new ClassdetailsView();
            var x = GetBase.GetList().Where(a => a.ClassNumber == claassid && a.IsDelete == false).FirstOrDefault();
            if (x != null)
            {
                classdetailsView.count = count;
                classdetailsView.QQ = x.QQGroupnumber;
                classdetailsView.WeChat = x.WechatGroupNumber;
                classdetailsView.ClassName =this.GetEntity( claassid).ClassNumber;
            }
            else
            {
                classdetailsView.count = count;
                classdetailsView.QQ = "未记录";
                classdetailsView.WeChat = "未记录";
                classdetailsView.ClassName =this.GetEntity( claassid).ClassNumber;
            }
            list.Add(classdetailsView);
            return list;
        }
        /// <summary>
        /// 根据班级号查询出班级的联系方式
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public GroupManagement Grouselect(int className)
        {
            return GetBase.GetList().Where(a => a.ClassNumber == className && a.IsDelete == false).FirstOrDefault();

        }
        /// <summary>
        /// 添加或者修改班级群号
        /// </summary>
        /// <returns></returns>
        public AjaxResult GroupAdd(GroupManagement groupManagement)
        {
            AjaxResult retus = null;

            try
            {

                if (groupManagement.ID > 0)
                {
                    GroupManagement x = GetBase.GetEntity(groupManagement.ID);
                    groupManagement.Addtime = x.Addtime;
                    groupManagement.IsDelete = false;
                    GetBase.Update(groupManagement);


                    BusHelper.WriteSysLog("数据修改", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                }
                else
                {


                    groupManagement.Addtime = DateTime.Now;
                    groupManagement.IsDelete = false;
                    GetBase.Insert(groupManagement);
                    BusHelper.WriteSysLog("数据添加", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }

                retus = new SuccessResult();
                retus.Msg = "操作成功";
                retus.Success = true;
            }
            catch (Exception ex)
            {
                if (groupManagement.ID > 0)
                {
                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
                }
                else
                {
                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
                retus = new ErrorResult();
                retus.Msg = "服务器错误";

                retus.Success = false;
                retus.ErrorCode = 500;
            }
            return retus;
        }
        /// <summary>
        /// 班委职位
        /// </summary>
        /// <returns></returns>
        public List<Members> MembersList()
        {
            return MemBers.GetListBySql<Members>("select*from Members").ToList();
        }
        /// <summary>
        /// 班级委员操作
        /// </summary>
        /// <param name="Stuid">学员学号</param>
        /// <param name="Typeofposition">班委名称</param>
        /// <param name="ClassNumber">班级号</param>
        /// <param name="Entity">数据操作为空则添加</param>
        /// <returns></returns>
        public AjaxResult Entityembers(string Stuid, string Typeofposition, int ClassNumber, string Entity)
        {
            //获取班委id

            AjaxResult retus = null;

            try
            {
                //如果不为空则进行删除委员
                if (Entity != "undefined")
                {
                    var x = Business.GetList().Where(a => a.IsDelete == false && a.Studentnumber == Stuid).ToList();

                    Business.Delete(x);

                    retus = new SuccessResult();
                    retus.Msg = "班委撤销成功";
                    retus.Success = true;
                    BusHelper.WriteSysLog("数据删除", EnumType.LogType.删除数据);
                }
                else
                {
                    int ID = MemBers.GetList().Where(a => a.Nameofmembers == Typeofposition && a.IsDelete == false).FirstOrDefault().ID;
                    //查询出同个职位有多少个
                    var mylist = Business.GetList().Where(a => a.Typeofposition == ID && a.ClassNumber == ClassNumber && a.IsDelete == false).ToList();
                    if (mylist.Count() > 0)
                    {
                        Business.Delete(mylist);
                    }
                    ClassMembers classMembers = new ClassMembers();
                    classMembers.Typeofposition = ID;
                    classMembers.ClassNumber = ClassNumber;
                    classMembers.Studentnumber = Stuid;
                    classMembers.IsDelete = false;
                    classMembers.Addtime = DateTime.Now;
                    Business.Insert(classMembers);
                    retus = new SuccessResult();
                    retus.Msg = "任命班委成功";
                    retus.Success = true;
                    BusHelper.WriteSysLog("数据添加", EnumType.LogType.添加数据);
                }
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 班会数据操作
        /// </summary>
        /// <param name="assmeetings">对象数据</param>
        /// <returns></returns>
        public AjaxResult EntityAssmeetings(Assmeetings assmeetings)
        {

            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                if (assmeetings.ID > 0)
                {
                    var x = myassmeetings.GetEntity(assmeetings.ID);
                    x.Title = assmeetings.Title;
                    x.Content = assmeetings.Content;
                    x.Classmeetingdate = assmeetings.Classmeetingdate;
                    x.Remarks = assmeetings.Remarks;
                    myassmeetings.Update(x);
                    BusHelper.WriteSysLog("数据修改", EnumType.LogType.编辑数据);
                    retus.Msg = "数据编辑成功";
                }
                else
                {
                    assmeetings.Addtime = DateTime.Now;
                    assmeetings.IsDelete = false;
                    myassmeetings.Insert(assmeetings);
                    BusHelper.WriteSysLog("添加数据", EnumType.LogType.添加数据);

                    retus.Msg = "数据添加成功";

                }
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";

                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 班会根据id查询出一个实体
        /// </summary>
        /// <param name="id">参数id</param>
        /// <returns></returns>
        public Assmeetings AssmeetingsSelect(int id)
        {
            return myassmeetings.GetEntity(id);
        }
        /// <summary>
        /// 通过班级名称拿到所有班会数据
        /// </summary>
        /// <param name="ClassName">班级名称</param>
        /// <returns></returns>
        public List<Assmeetings> AssmeetingsList(int ClassName)
        {
            return myassmeetings.GetList().Where(a => a.IsDelete == false && a.ClassNumber == ClassName).ToList();
        }
        /// <summary>
        /// 查询这个委员是否有重复的
        /// </summary>
        /// <param name="Typeofposition">委员名称</param>
        /// <param name="ClassNumber">班级号</param>
        /// /// <param name="Stuid">学号</param>
        /// <returns></returns>
        public AjaxResult AssmeetingsBool(string Typeofposition, int ClassNumber, string Stuid)
        {
            AjaxResult retus = null;
            retus = new SuccessResult();
            retus.Success = true;
            int ID = MemBers.GetList().Where(a => a.Nameofmembers == Typeofposition && a.IsDelete == false).FirstOrDefault().ID;
            var mylist = Business.GetList().Where(a => a.Typeofposition == ID && a.ClassNumber == ClassNumber && a.IsDelete == false && a.Studentnumber != Stuid).ToList().Count();
            if (mylist > 0)
            {

                retus.Msg = "已有学员为" + Typeofposition + "是否替换";

                retus.Data = "aaa";

            }

            //查询出同个职位有多少个
            var mylist1 = Business.GetList().Where(a => a.Typeofposition == ID && a.ClassNumber == ClassNumber && a.IsDelete == false && a.Studentnumber == Stuid).ToList().Count();
            if (mylist1 > 0)
            {
                retus.Msg = Typeofposition + "已任命该学员,请勿重复任命";

            }

            return retus;
        }
        /// <summary>
        /// 拆班数据操作
        /// </summary>
        /// <param name="Addtime">拆班日期</param>
        /// <param name="FormerClass">原班级</param>
        /// <param name="List">现班级param>
        /// <param name="Reasong">原因</param>
        /// <param name="Remarks">备注</param>
        /// <param name="StudentID">学号</param>
        /// <returns></returns>
        public AjaxResult Dismantleclasses(string Addtime, int FormerClass, int List, string Reasong, string Remarks, string StudentID)
        {
            var x = Dismantle.GetList().Where(a => a.IsDelete == false && a.FormerClass == FormerClass).Count();
            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true; 
                if (x < 1)
                {

                    Hadmst.EndDai(FormerClass);
                     var staid = classtatus.GetList().Where(a => a.IsDelete == false && a.TypeName == "升学").FirstOrDefault().id;
                  var fint=  this.FintClassSchedule(FormerClass);
                    fint.ClassstatusID = staid;
                    this.Update(fint);
                    RemovalRecords removalRecords = new RemovalRecords();
                    removalRecords.Addtime = Convert.ToDateTime(Addtime);
                    removalRecords.FormerClass = FormerClass;
                    removalRecords.Reasong = Reasong;
                    removalRecords.Remarks = Remarks;
                    removalRecords.IsDelete = false;
                    Dismantle.Insert(removalRecords);
                }

                string[] Student = StudentID.Split(',');
           //     List<ClassDynamics> MyDynamise = new List<ClassDynamics>();

                List<ScheduleForTrainees> UpdateScheduleFor = new List<ScheduleForTrainees>();

                List<ScheduleForTrainees> AddScheduleFor = new List<ScheduleForTrainees>();
                foreach (var item in Student)
                {
                    //ClassDynamics classDynamics = new ClassDynamics();
                    //classDynamics.Addtime = Convert.ToDateTime(Addtime);
                    //classDynamics.FormerClass = FormerClass;
                    //classDynamics.CurrentClass = List;
                    //classDynamics.Studentnumber = item;
                    //classDynamics.Remarks = Remarks;
                    //classDynamics.Reason = Reasong;
                    //classDynamics.IsDelete = false;
                    //MyDynamise.Add(classDynamics);
                    var UpdateSche = ss.GetList().Where(a => a.StudentID == item && a.ID_ClassName == FormerClass).FirstOrDefault();
                    if (UpdateSche != null)
                    {
                        UpdateSche.CurrentClass = false;
                        UpdateScheduleFor.Add(UpdateSche);
                    }
                    ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                    scheduleForTrainees.ID_ClassName = List;
                    scheduleForTrainees.StudentID = item;
                    scheduleForTrainees.CurrentClass = true;
                    scheduleForTrainees.AddDate = Convert.ToDateTime(Addtime);
                    scheduleForTrainees.ClassID = this.GetEntity(List).ClassNumber;
                    AddScheduleFor.Add(scheduleForTrainees);
                }
                ss.Update(UpdateScheduleFor);
                ss = new ScheduleForTraineesBusiness();
                ss.Insert(AddScheduleFor);
             //   CLassdynamic.Insert(MyDynamise);

                BusHelper.WriteSysLog("拆班数据添加", EnumType.LogType.添加数据);
                retus.Msg = "拆班成功";
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";

                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
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
        public AjaxResult Combinedclasses(string Addtime, int FormerClass, int List, string Reasong, string Remarks, string StudentID)
        {
            var x = Dismantle.GetList().Where(a => a.IsDelete == false && a.FormerClass == FormerClass).Count();
            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                if (x < 1)
                {

                    Hadmst.EndDai(FormerClass);
                    //var staid = classtatus.GetList().Where(a => a.IsDelete == false && a.TypeName == "升学").FirstOrDefault().id;
                    //var fint = this.FintClassSchedule(FormerClass);
                    //fint.ClassstatusID = staid;
                    //this.Update(fint);
                  
                }

                string[] Student = StudentID.Split(',');
                //     List<ClassDynamics> MyDynamise = new List<ClassDynamics>();

                List<ScheduleForTrainees> UpdateScheduleFor = new List<ScheduleForTrainees>();

                List<ScheduleForTrainees> AddScheduleFor = new List<ScheduleForTrainees>();
                foreach (var item in Student)
                {
                 
                    var UpdateSche = ss.GetList().Where(a => a.StudentID == item && a.ID_ClassName == FormerClass).FirstOrDefault();
                    if (UpdateSche != null)
                    {
                        UpdateSche.CurrentClass = false;
                        UpdateScheduleFor.Add(UpdateSche);
                    }
                    ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                    scheduleForTrainees.ID_ClassName = List;
                    scheduleForTrainees.StudentID = item;
                    scheduleForTrainees.CurrentClass = true;
                    scheduleForTrainees.AddDate = Convert.ToDateTime(Addtime);
                    scheduleForTrainees.ClassID = this.GetEntity(List).ClassNumber;
                    AddScheduleFor.Add(scheduleForTrainees);
                }
                ss.Update(UpdateScheduleFor);
                ss = new ScheduleForTraineesBusiness();
                ss.Insert(AddScheduleFor);
                //   CLassdynamic.Insert(MyDynamise);

                BusHelper.WriteSysLog("合班数据添加", EnumType.LogType.添加数据);
                retus.Msg = "合班成功";
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";

                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 转班业务
        /// </summary>
        /// <param name="FormerClass">原班</param>
        /// <param name="List">分配的班级</param>
        /// <param name="StudentID">学号</param>
        /// <returns></returns>
        public AjaxResult Shift(int FormerClass, int List, string StudentID) {

            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                string[] Student = StudentID.Split(',');
                List<ScheduleForTrainees> UpdateScheduleFor = new List<ScheduleForTrainees>();
                List<ScheduleForTrainees> AddScheduleFor = new List<ScheduleForTrainees>();
                foreach (var item in Student)
                {
                    var UpdateSche = ss.GetList().Where(a => a.StudentID == item && a.ID_ClassName == FormerClass).FirstOrDefault();
                    if (UpdateSche != null)
                    {
                        UpdateSche.CurrentClass = false;
                        UpdateScheduleFor.Add(UpdateSche);
                    }
                    ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                    scheduleForTrainees.ID_ClassName = List;
                    scheduleForTrainees.StudentID = item;
                    scheduleForTrainees.CurrentClass = true;
                    scheduleForTrainees.AddDate = DateTime.Now;
                    scheduleForTrainees.ClassID = this.GetEntity(List).ClassNumber;
                    AddScheduleFor.Add(scheduleForTrainees);
                }
                ss.Update(UpdateScheduleFor);
                ss = new ScheduleForTraineesBusiness();
                ss.Insert(AddScheduleFor);
                retus.Msg = "转班成功";
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";

                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 根据班级名称获取阶段跟专业
        /// </summary>
        /// <param name="ClassNumber">班级名称</param>
        /// // <param name="type">1是专业名称，否则是阶段</param>
        /// <returns></returns>
        public string GetClassGrand(int ClassNumber, int type)
        {
            var CLaaNuma = this.GetEntity(ClassNumber);
            CLaaNuma = CLaaNuma == null ? new ClassSchedule() : CLaaNuma;
            if (type == 1)
            {
                Specialty find_s = Techarcontext.GetEntity(CLaaNuma.Major_Id);
                if (find_s != null)
                {
                    return find_s.SpecialtyName;
                }
                else
                {
                    return "无";
                }

            }
            else
            {
                return CLaaNuma.grade_Id <1?"": Grandcontext.GetEntity(CLaaNuma.grade_Id).GrandName;
            }
        }
        /// <summary>
        /// 获取班级时间段
        /// </summary>
        /// <param name="ClassNumber"></param>
        /// <returns></returns>
        public string GetClassTime(int ClassNumber)
        {
            ClassSchedule CLaaNuma = this.GetEntity(ClassNumber);
            BaseDataEnum find_b= BaseDataEnum_Entity.GetEntity(CLaaNuma.BaseDataEnum_Id);
            if (find_b!=null)
            {
                return find_b.Name;
            }
            else
            {
                return "无";
            }
        }
        /// <summary>
        /// 获取班级学员缴费记录
        /// </summary>
        /// <param name="ClassName">班级名称</param>
        /// <returns></returns>
        public List<DetailedcostView> listTuiton(int ClassName)
        {
            ClassScheduleBusiness classSchedules = new ClassScheduleBusiness();
            var CLaaNuma = this.GetList().Where(a => a.id == ClassName).FirstOrDefault();
            var Mylist = classSchedules.ClassStudentneViewList(ClassName);
            List<DetailedcostView> lisrDetaild = new List<DetailedcostView>();
            foreach (var item in Mylist)
            {

                DetailedcostView detailedcostView = this.GotoschoolTuition(CLaaNuma.grade_Id, item.StuNameID);
                detailedcostView.ClassName =this.GetEntity( ClassName).ClassNumber;
                detailedcostView.Name = item.Name;
                detailedcostView.Stidentid = item.StuNameID;
                detailedcostView.Sex =  studentInformationBusiness.GetEntity(item.StuNameID).Sex== false ? "女" : "男";
                detailedcostView.HeadmasterName = Hadmst.ClassHeadmaster(ClassName)==null?"无": Hadmst.ClassHeadmaster(ClassName).EmpName;
                detailedcostView.Phone = Hadmst.ClassHeadmaster(ClassName)==null?"无": Hadmst.ClassHeadmaster(ClassName).Phone;
                lisrDetaild.Add(detailedcostView);
            }
            
            return lisrDetaild;
        }
        /// <summary>
        /// 获取升学费用
        /// </summary>
        /// 
        /// <param name="Grand"></param>
        /// <param name="StudentID"></param>
        /// <returns></returns>
        public DetailedcostView GotoschoolTuition(int Grand, string StudentID)
        {
            //已交费用
            decimal price = 0;
            //应交费用
            decimal myprice = 0;
            //拿到下一个阶段
            var x = GotoschoolStageBusiness.GetList().Where(a => a.CurrentStageID == Grand).FirstOrDefault();

            var mylist = costitemsBusiness.GetList().Where(a => a.IsDelete == false && a.Grand_id == x.NextStageID).ToList();
            foreach (var item in mylist)
            {
                myprice = myprice + item.Amountofmoney;
                var x1 = studentfee.GetList().Where(a => a.IsDelete == false && a.StudenID == StudentID && a.Costitemsid == item.id).ToList();
                decimal? Amountofmoney = 0;
                foreach (var item1 in x1)
                {
                    Amountofmoney = Amountofmoney + item1.Amountofmoney;
                }

                price = price + (decimal)Amountofmoney;

            }
            DetailedcostView detailedcostView = new DetailedcostView();
            detailedcostView.Amountofmoney = price;
            detailedcostView.Isitinturn = price >= myprice ? "交齐" : "未交齐";
            detailedcostView.ShouldJiao = myprice;
            detailedcostView.Surplus = myprice - price;
            detailedcostView.NextStageID = Grandcontext.GetEntity(x.NextStageID).GrandName;
            detailedcostView.CurrentStageID = Grandcontext.GetEntity(Grand).GrandName;
            return detailedcostView;
        }
        /// <summary>
        /// 验证当前班级是否有下一个阶段
        /// </summary>
        /// <param name="ClassID">班级编号</param>
        /// <returns></returns>
        public bool PayMones(int ClassID)
        {
         var x=   this.GetEntity(ClassID);
            //拿到下一个阶段
            var z = GotoschoolStageBusiness.GetList().Where(a => a.CurrentStageID == x.grade_Id).Count();
            if (z>0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 通过班级名称查询是否有重复名称
        /// </summary>
        /// <param name="ClassName">班级名称</param>
        /// <returns></returns>
        public int ClassNameCount(string ClassName)
        {
            return this.GetList().Where(a => a.ClassNumber == ClassName&&a.ClassstatusID == null).Count();
        }
        /// <summary>
        /// 手机短信实例
        /// </summary>
        /// <param name="numbers">电话</param>
        /// <param name="smsTexts">内容</param>
        /// <returns></returns>
        public string PhoneSMS(string numbers, string smsTexts)
        {
            string number = numbers;
            string smsText = smsTexts;
            string t = PhoneMsgHelper.SendMsg(number, smsText);
            return t;
        }
        /// <summary>
        /// 短信催费
        /// </summary>
        /// <param name="Datailedcost">序列化集合对象</param>
        /// <returns></returns>
        public string SMScharging(List<DetailedcostView> Datailedcost)
        {
            string count = "";
          var fine=  shortmessageBusiness.FineShortmessage("学费催费").content;
            foreach (var item in Datailedcost)
            {
                //电话Familyphone
                var student = studentInformationBusiness.GetEntity(item.Stidentid);
               var msmTexts= fine.Replace("{{Name}}", item.Name).Replace("{{NextStageID}}", item.NextStageID).Replace("{{ShouldJiao}}", item.ShouldJiao.ToString()).Replace("{{Surplus}}", item.Surplus.ToString()).Replace("{{HeadmasterName}}", item.HeadmasterName).
                    Replace("{{Phone}}", item.Phone).Replace("{{Stidentid}}", item.Stidentid).Replace("{{ClassName}}", item.ClassName);
                string strText = System.Text.RegularExpressions.Regex.Replace(msmTexts, "<[^>]+>", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", ""); 
                count = PhoneSMS(student.Familyphone, strText);
            }
            return count;
        }
        /// <summary>
        /// 添加学费催费模板
        /// </summary>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public AjaxResult EntiShortmessage(string content)
        {
            return shortmessageBusiness.EntiShortmessage("学费催费", content);
        }
        private EmpClassBusiness empClassBusiness;
        /// <summary>
        /// S2,S3升学
        /// </summary>
        /// <param name="ClassName">班级名称</param>
        /// <returns></returns>
        public AjaxResult EntitAddClassSchedule(int ClassID)
        {
            empClassBusiness = new EmpClassBusiness();
        
            AjaxResult result = null;
            try
            {
            List<ScheduleForTrainees> UpdateScheduleFor = new List<ScheduleForTrainees>();
            List<ScheduleForTrainees> AddScheduleFor = new List<ScheduleForTrainees>();
                Hadmst.EndDai(ClassID);
                var ClassSchedules = this.GetEntity(ClassID);
            var staid = classtatus.GetList().Where(a => a.IsDelete == false && a.TypeName == "升学").FirstOrDefault().id;
           
            ClassSchedules.ClassstatusID = staid;
               
            //先修改原来班级
            this.Update(ClassSchedules);
            var x=  GotoschoolStageBusiness.GetList().Where(a => a.CurrentStageID == ClassSchedules.grade_Id).FirstOrDefault();
            ClassSchedules.grade_Id = x.NextStageID;
            ClassSchedules.ClassstatusID = null;
         
            result = new SuccessResult();
            result.Success = true;
            //再添加一个新班级
            this.Insert(ClassSchedules);
       
            var ClassStudent=  ss.GetList().Where(a => a.ID_ClassName == ClassID&&a.CurrentClass==true).ToList();
            
            foreach (var item in ClassStudent)
            {
                var UpdateSche = ss.GetList().Where(a => a.StudentID == item.StudentID && a.ID_ClassName == ClassID).FirstOrDefault();
                if (UpdateSche != null)
                {
                    UpdateSche.CurrentClass = false;
                    UpdateScheduleFor.Add(UpdateSche);
                }
                ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                scheduleForTrainees.ClassID = ClassSchedules.ClassNumber;
                scheduleForTrainees.StudentID = item.StudentID;
                scheduleForTrainees.CurrentClass = true;
                scheduleForTrainees.ID_ClassName = ClassSchedules.id;
                scheduleForTrainees.AddDate = DateTime.Now;
                AddScheduleFor.Add(scheduleForTrainees);
            }
             if(this.GetEntity(ClassID).grade_Id== Grandcontext.GetList().Where(a => a.IsDelete == false && a.GrandName == "S3").FirstOrDefault().Id)
             {
                    empClassBusiness.entrance(ClassID, ClassSchedules.id);
             }
            ss.Update(UpdateScheduleFor);
            ss = new ScheduleForTraineesBusiness();
            ss.Insert(AddScheduleFor);
            BusHelper.WriteSysLog("添加班级学员", EnumType.LogType.添加数据);
            result.Msg = "升学成功";

            }
            catch (Exception ex)
            {

                result = new ErrorResult();
                result.Msg = "服务器错误";

                result.Success = false;
                result.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return result;
        }
        /// <summary>
        /// S4毕业操作
        /// </summary>
        /// <param name="ClassID">班级编号</param>
        /// <returns></returns>
        public AjaxResult ClassEnd(int ClassID)
        {
           
            AjaxResult result = null;
            try
            {
                result = new SuccessResult();
                result.Success = true;
                List<ScheduleForTrainees> UpdateScheduleFor = new List<ScheduleForTrainees>();

                Hadmst.EndDai(ClassID);
                var ClassSchedules = this.GetEntity(ClassID);
                var staid = classtatus.GetList().Where(a => a.IsDelete == false && a.TypeName == "毕业").FirstOrDefault().id;
                ClassSchedules.ClassstatusID = staid;
                //先修改原来班级
                this.Update(ClassSchedules);
             
             
                
                var ClassStudent = ss.GetList().Where(a => a.ID_ClassName == ClassID).ToList();

                foreach (var item in ClassStudent)
                {
                    var UpdateSche = ss.GetList().Where(a => a.StudentID == item.StudentID && a.ID_ClassName == ClassID).FirstOrDefault();
                    if (UpdateSche != null)
                    {
                        UpdateSche.CurrentClass = false;
                        UpdateSche.IsGraduating = true;
                       var stu= studentInformationBusiness.GetEntity(UpdateSche.StudentID);
                        //修改学生状态为毕业
                        stu.State = BasicdatBusiness.GetList().Where(a => a.Name == "毕业" && a.IsDetele == false).FirstOrDefault().ID;
                  
                        studentInformationBusiness.Update(stu);
                        //删除宿舍
                        Accdation = new AccdationinformationBusiness();
                        Accdation.delacc(UpdateSche.StudentID);
                        UpdateScheduleFor.Add(UpdateSche);
                    }
                
                   
                }
                ss.Update(UpdateScheduleFor);
           
                BusHelper.WriteSysLog("编辑班级学员", EnumType.LogType.编辑数据);
                result.Msg = "恭喜本班级顺利毕业！";

            }
            catch (Exception ex)
            {

                result = new ErrorResult();
                result.Msg = "服务器错误";

                result.Success = false;
                result.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return result;
        }
        /// <summary>
        /// 获取转班申请表单数据
        /// </summary>
        /// <param name="StudentID">学号</param>
        /// <returns></returns>
        public TransactionView ShiftworkFine(string StudentID)
        {
            //宿舍业务
            AccdationinformationBusiness accdationinformationBusiness = new AccdationinformationBusiness();
            //学员班级
            ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();

            //学生对象数据
          var student=  studentInformationBusiness.GetEntity(StudentID);
            TransactionView transactionView = new TransactionView();
            transactionView.OriginalClass = scheduleForTraineesBusiness.SutdentCLassName(StudentID).ID_ClassName;//班级id
            transactionView.OriginalClassName = scheduleForTraineesBusiness.SutdentCLassName(StudentID).ClassID;//班级名称
            transactionView.Name = student.Name;//姓名
            transactionView.Sex = student.Sex == true ? "男" : "女";
            transactionView.StudentID = StudentID;//学号
            transactionView.Telephone = student.Telephone;//联系电话
            transactionView.Postaladdress = student.Familyaddress;//家庭住址
            //transactionView.NowHeadmaster = Hadmst.ClassHeadmaster(scheduleForTraineesBusiness.SutdentCLassName(StudentID).ID_ClassName).EmpName;//班主任姓名
            transactionView.IDnumber = student.identitydocument;//身份证
            //获取班级对象
            var Stage = this.GetEntity(scheduleForTraineesBusiness.SutdentCLassName(StudentID).ID_ClassName);
            transactionView.Stage =Grandcontext.GetEntity(Stage.grade_Id).GrandName;//阶段
          transactionView.Dormitoryaddress = accdationinformationBusiness.GetDormBystudentno(StudentID)==null?"该学员暂无宿舍地址": accdationinformationBusiness.GetDormBystudentno(StudentID).DormInfoName;//宿舍地址

            return transactionView;
           
        }
        /// <summary>
        /// 根据班级id获取相同的阶段班级并且不能有该班级id
        /// </summary>
        /// <param name="ClassID">班级编号</param>
        /// <returns></returns>
        public List<ClassSchedule> ListGradeidentical(int ClassID) {
            
            var find = this.GetEntity(ClassID);
         return   this.GetList().Where(a => a.id != ClassID && a.grade_Id == find.grade_Id && a.ClassstatusID == null&&a.ClassStatus==false&&a.Major_Id==find.Major_Id).ToList();
        }
        /// <summary>
        /// 根据阶段专业获取班级
        /// </summary>
        /// <param name="grade_Id">阶段</param>
        /// <returns></returns>
        public List<ClassSchedule> ListGradeidenticals(int grade_Id)
        {
            return this.GetList().Where(a => a.grade_Id == grade_Id && a.ClassstatusID == null && a.ClassStatus == false).ToList();
        }
        /// <summary>
        /// 转班业务申请
        /// </summary>
        /// <param name="transactionView">转班数据</param>
        /// <returns></returns>
        public AjaxResult Shiftwork(TransactionView transactionView)
        {
            AjaxResult result = null;
            try
            {
                if (this.Changeprocessing(transactionView.StudentID) < 1)
                {
                    Transfer transfer = new Transfer();
                    transfer.Dateofregistration = false;
                    transfer.Reasonsforshifting = transactionView.Reason;
                    transfer.TransferDate = transactionView.Dateofapplication;
                    transfer.Studentnumber = transactionView.StudentID;
                    transfer.Hopetoransfer = transactionView.NowCLass;
                    var boolTransfer = classDynamicsBusiness.GetList().Where(a => a.IsaDopt == null && a.Studentnumber == transfer.Studentnumber && a.TransferID != null).ToList();
                    result = new SuccessResult();
                    result.Success = true;
                    if (boolTransfer.Count() < 1)
                    {
                        TransferBusiness.Insert(transfer);
                        ClassDynamics classDynamics = new ClassDynamics();
                        classDynamics.IsaDopt = null;
                        classDynamics.Addtime = DateTime.Now;
                        classDynamics.CurrentClass = transactionView.NowCLass;
                        classDynamics.FormerClass = transactionView.OriginalClass;
                        classDynamics.Studentnumber = transactionView.StudentID;
                        var tranID = TransferBusiness.GetList().Where(a => a.Dateofregistration == false && a.Studentnumber == transactionView.StudentID && a.Hopetoransfer == transactionView.NowCLass).ToList().OrderByDescending(a => a.ID).FirstOrDefault();
                        classDynamics.TransferID = tranID.ID;
                        classDynamics.States = this.FineBasicdat("转班").ID;
                        classDynamicsBusiness.Insert(classDynamics);

                        BusHelper.WriteSysLog("添加异动数据", EnumType.LogType.添加数据);
                        result.Msg = "申请成功";
                    }
                    else
                    {

                        result.Msg = "请勿重复申请";
                    }
                }
                else
                {
                    result.Msg = "有异动数据未处理";
                }
            
            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误";
                result.Success = false;
                result.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return result;
        }
        /// <summary>
        /// 根据类型获取基础数据对象
        /// </summary>
        /// <param name="Name">类型名称</param>
        /// <returns></returns>
        public Basicdat FineBasicdat(string Name)
        {
          return  BasicdatBusiness.GetList().Where(a => a.Name == Name && a.IsDetele == false).FirstOrDefault();
        }
        /// <summary>
        /// 通过班级名称获取该班级的所有异动
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public object TransactionDate(int page, int limit, int ClassID, string TypeName, string StudentID, string Name,string IsaDopt)
        {
            var classDyan = classDynamicsBusiness.GetList();
            List<ClassDynamics> listDyanmics = new List<ClassDynamics>();
            foreach (var item in classDyan)
            {
               
                if (item.FormerClass == ClassID)
                {
                    var ClassStu = ss.GetList().Where(a => a.StudentID == item.Studentnumber).FirstOrDefault();
                    if (ClassStu != null)
                    {
                        listDyanmics.Add(item);
                    }
                }
              
            }
            var x = listDyanmics.Select(a => new
            {
                a.ID,
                a.States,
                StatesName = BasicdatBusiness.GetEntity(a.States).Name,//类型
                a.Studentnumber,
                Name = studentInformationBusiness.GetEntity(a.Studentnumber).Name,
                Sex = studentInformationBusiness.GetEntity(a.Studentnumber).Sex == false ? "女" : "男",
                Telephone = studentInformationBusiness.GetEntity(a.Studentnumber).Telephone,
                a.IsaDopt
            }).ToList();
            if (!string.IsNullOrEmpty(TypeName))
            {
                var sta = int.Parse(TypeName);
                x= x.Where(a => a.States == sta).ToList();
            }
            if (!string.IsNullOrEmpty(StudentID))
            {
              
                x = x.Where(a => a.Studentnumber == StudentID).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
               
                x = x.Where(a => a.Name.Contains(Name)).ToList();
            }
            if (!string.IsNullOrEmpty(IsaDopt))
            {
                if (IsaDopt=="null")
                {
                    x = x.Where(a => a.IsaDopt == null).ToList();
                }
                else
                {
                    x = x.Where(a => a.IsaDopt == Convert.ToBoolean(IsaDopt)).ToList();
                }
            }
            var Myx=x.OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = x.Count,
                data = Myx
            };
            return data;
        }

        /// <summary>
        /// 获取所有异动数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="TypeName"></param>
        /// <param name="StudentID"></param>
        /// <param name="Name"></param>
        /// <param name="IsaDopt"></param>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public object TransactionDatepro(int page, int limit,  string TypeName,  string Name, string IsaDopt, string ClassID)
        {
            var classDyan = classDynamicsBusiness.GetList().Where(a=>a.IsaDopt!=null).ToList();
            var x = classDyan.Select(a => new
            {
                a.ID,
                a.States,
                StatesName = BasicdatBusiness.GetEntity(a.States).Name,//类型
                a.Studentnumber,
                Name = studentInformationBusiness.GetEntity(a.Studentnumber).Name,
                Sex = studentInformationBusiness.GetEntity(a.Studentnumber).Sex == false ? "女" : "男",
                Telephone = studentInformationBusiness.GetEntity(a.Studentnumber).Telephone,
                a.IsaDopt,
                a.FormerClass,
                this.GetEntity(a.FormerClass).ClassNumber
            }).ToList();

            if (!string.IsNullOrEmpty(TypeName))
            {
                var sta = int.Parse(TypeName);
                x = x.Where(a => a.States == sta).ToList();
            }
         
            if (!string.IsNullOrEmpty(Name))
            {

                x = x.Where(a => a.Name.Contains(Name)).ToList();
            }
            if (!string.IsNullOrEmpty(IsaDopt))
            {
                if (IsaDopt != "null")
                {
                   
                    x = x.Where(a => a.IsaDopt == Convert.ToBoolean(IsaDopt)).ToList();
                }
            }
            if (!string.IsNullOrEmpty(ClassID))
            {
                int ClassIDpro = int.Parse(ClassID);
                x = x.Where(a => a.FormerClass== ClassIDpro).ToList();
            }
            var Myx = x.OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = x.Count,
                data = Myx
            };
            return data;
        }
        /// <summary>
        /// 获取转班申请单数据
        /// </summary>
        /// <param name="ID">转班类id</param>
        /// <returns></returns>
        public TransactionView TransferFine(int ID)
        {
            var x = TransferBusiness.GetEntity(ID);
            TransactionView transactionView = new TransactionView();
            transactionView.Dateofapplication =(DateTime) x.TransferDate;
            transactionView.Reason = x.Reasonsforshifting;
            transactionView.NowCLassName = this.GetEntity(x.Hopetoransfer).ClassNumber;
            return transactionView;
        }
        /// <summary>
        /// 试学表单数据
        /// </summary>
        /// <param name="ID">试学id</param>
        /// <returns></returns>
        public TransactionView TrialapplicationFine(int ID)
        {
            var x = TrialapplicationBusiness.GetEntity(ID);
            TransactionView transactionView = new TransactionView();
            transactionView.Dateofapplication = (DateTime)x.TrialDate;
            transactionView.Reason = x.ReasonsforTrialStudy;
            transactionView.NowCLassName = this.GetEntity(x.TrialClass).ClassNumber;
            return transactionView;
        }
        /// <summary>
        /// 复学申请表单数据
        /// </summary>
        /// <param name="ID">复学id</param>
        /// <returns></returns>
        public TransactionView RestudyFine(int ID)
        {
            var x = RestudyBusines.GetEntity(ID);
            TransactionView transactionView = new TransactionView();
            transactionView.Dateofapplication = (DateTime)x.Applicationtime;
            transactionView.Reason = x.Reason;
            transactionView.NowCLassName = this.GetEntity(x.ClassID).ClassNumber;
            transactionView.IsBookcollection = x.IsBookcollection;
            transactionView.Reasonsfordelay = x.Reasonsfordelay;
            return transactionView;
        }
        /// <summary>
        /// 休学申请表单数据
        /// </summary>
        /// <param name="ID">休学id</param>
        /// <returns></returns>
        public TransactionView SuspensionofschoolFine(int ID)
        {
            var x = SuspensionofschoolBusiness.GetEntity(ID);
            TransactionView transactionView = new TransactionView();
            transactionView.Dateofapplication = (DateTime)x.Dateofapplication;
            transactionView.Reason = x.Reason;
            transactionView.qBeginTime = x.Startingperiod;
            transactionView.qEndTime = x.Deadline;
            return transactionView;
        }
        /// <summary>
        /// 退学申请表单数据
        /// </summary>
        /// <param name="ID">退学id</param>
        /// <returns></returns>
        public TransactionView ApplicationDropoutFine(int ID)
        {
            var x = ApplicationDropoutBusiness.GetEntity(ID);
            TransactionView transactionView = new TransactionView();
            transactionView.Dateofapplication = (DateTime)x.Addtime;
            transactionView.Reason = x.Reasonofdropout;
            transactionView.NowCLassName = ss.SutdentCLassName(x.Studentnumber).ClassID;
            return transactionView;
            
        }
        /// <summary>
        /// 重修申请表单数据
        /// </summary>
        /// <param name="ID">重修id</param>
        /// <returns></returns>
        public TransactionView ApplicationRepairFine(int ID)
        {
           var x= ApplicationRepairBusiness.GetEntity(ID);
            TransactionView transactionView = new TransactionView();
            transactionView.Dateofapplication = (DateTime)x.Repairtime;
            transactionView.Reason = x.Reason;
            transactionView.NowCLassName = this.GetEntity(x.Rehabilit).ClassNumber;
            return transactionView;
        }
        /// <summary>
        /// 获取开除表单数据
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public TransactionView ExoeldetaFine(int ID)
        {
            var x = ExpelsBusiness.GetEntity(ID);
            TransactionView transactionView = new TransactionView();
            transactionView.Dateofapplication = (DateTime)x.Applicationtime;
            transactionView.Reason = x.Reason;
            return transactionView;
        }
        /// <summary>
        /// 异动数据
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <returns></returns>
        public TransactionView Transactiondetails(int ID)
        {
            TransactionView transactionView = new TransactionView();
            var x = classDynamicsBusiness.GetEntity(ID);
            if (x.ApplicationDropoutID != null){
                
             transactionView = this.ApplicationDropoutFine((int)x.ApplicationDropoutID);
            }
            else if (x.ApplicationRepairID!=null)
            {
                transactionView = this.ApplicationRepairFine((int)x.ApplicationRepairID);
            }
            else if (x.TransferID!=null)
            {
             transactionView= this.TransferFine((int)x.TransferID);
            }
            else if (x.TrialapplctionID!=null)
            {

            }
            else if (x.SuspensionofschoolID!=null)
            {
                
              transactionView = this.SuspensionofschoolFine((int)x.SuspensionofschoolID);
              
            }
            else if (x.RestudyID!=null)
            {
              transactionView = this.RestudyFine((int)x.RestudyID);
            }
            else if (x.ExpelsID!=null)
            {
                transactionView.NowCLassName = this.GetEntity(x.FormerClass).ClassNumber;
                transactionView = this.ExoeldetaFine((int)x.ExpelsID);
            }
           var Student= studentInformationBusiness.GetEntity(x.Studentnumber);
            transactionView.IsaDopt = x.IsaDopt;
            transactionView.Name = Student.Name;
            transactionView.ID = ID;
            transactionView.Sex = Student.Sex==false?"女":"男";
            transactionView.StudentID = Student.StudentNumber;
            transactionView.IDnumber = Student.identitydocument;
            transactionView.Telephone = Student.Telephone;//联系电话
            transactionView.Postaladdress = Student.Familyaddress;//家庭住址
            transactionView.NowHeadmaster = Hadmst.ClassHeadmasterPro(this.GetEntity(x.FormerClass).id).EmpName;//班主任姓名
            transactionView.OriginalClassName = this.GetEntity(x.FormerClass).ClassNumber;
            transactionView.NowCLass = x.CurrentClass;
            transactionView.Dormitoryaddress = x.Dormitoryaddress;

            return transactionView;
        }
        /// <summary>
        /// 转班数据操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Secss">同意或者拒绝</param>
        /// <returns></returns>
        public AjaxResult Shiftworkoperation(int ID,string Secss)
        {
           var x= classDynamicsBusiness.GetEntity(ID);
          
            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                if (Secss == "Yes")
                {
                    x.IsaDopt = true;
                    var StudenClass = ss.GetList().Where(a => a.StudentID == x.Studentnumber && a.CurrentClass == true).FirstOrDefault();
                    StudenClass.CurrentClass = false;
                    ss.Update(StudenClass);
                    CLassdynamic.Update(x);
                    ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                    scheduleForTrainees.ID_ClassName = TransferBusiness.GetEntity(x.TransferID).Hopetoransfer;
                    scheduleForTrainees.ClassID = this.GetEntity(TransferBusiness.GetEntity(x.TransferID).Hopetoransfer).ClassNumber;
                    scheduleForTrainees.StudentID = x.Studentnumber;
                    scheduleForTrainees.CurrentClass = true;
                    scheduleForTrainees.AddDate = Convert.ToDateTime(TransferBusiness.GetEntity(x.TransferID).TransferDate);
                    ss.Insert(scheduleForTrainees);
                    BusHelper.WriteSysLog("转班数据修改及学员数据添加", EnumType.LogType.添加数据);
                    retus.Msg = "转班成功";
                }
                else
                {
                    x.IsaDopt = false;
                    CLassdynamic.Update(x);
                    BusHelper.WriteSysLog("修改异动数据", EnumType.LogType.编辑数据);
                    retus.Msg = "审批失败";
                }
             
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 重修数据操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Secss">同意或者拒绝</param>
        /// <returns></returns>
        public AjaxResult Rebuildkoperation(int ID, string Secss)
        {
            var x = classDynamicsBusiness.GetEntity(ID);

            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                if (Secss == "Yes")
                {
                    x.IsaDopt = true;
                    var StudenClass = ss.GetList().Where(a => a.StudentID == x.Studentnumber && a.CurrentClass == true).FirstOrDefault();
                    StudenClass.CurrentClass = false;
                    ss.Update(StudenClass);
                    CLassdynamic.Update(x);
                    ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                    scheduleForTrainees.ID_ClassName = ApplicationRepairBusiness.GetEntity(x.ApplicationRepairID).Rehabilit;
                    scheduleForTrainees.ClassID = this.GetEntity(ApplicationRepairBusiness.GetEntity(x.ApplicationRepairID).Rehabilit).ClassNumber;
                    scheduleForTrainees.StudentID = x.Studentnumber;
                    scheduleForTrainees.CurrentClass = true;
                    scheduleForTrainees.AddDate = Convert.ToDateTime(ApplicationRepairBusiness.GetEntity(x.ApplicationRepairID).Repairtime);
                    ss.Insert(scheduleForTrainees);
                    BusHelper.WriteSysLog("重修数据修改及学员数据添加", EnumType.LogType.添加数据);
                    retus.Msg = "操作成功";
                }
                else
                {
                    x.IsaDopt = false;
                    CLassdynamic.Update(x);
                    BusHelper.WriteSysLog("修改异动数据", EnumType.LogType.编辑数据);
                    retus.Msg = "审批失败";
                }

            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        List<GotoschoolStage> MylistStage = new List<GotoschoolStage>();
        /// <summary>
        /// 递归根据阶段查找上面可重修的阶段班级
        /// </summary>
        /// <param name="Stage">阶段</param>
        /// <returns></returns>
        public List<GotoschoolStage> RecursionStage(int Stage)
        {
           var x= GotoschoolStageBusiness.GetList().Where(a => a.NextStageID == Stage).FirstOrDefault();
            if (x==null)
            {
                return MylistStage;
            }
            else
            {
                MylistStage.Add(x);
                return RecursionStage(x.CurrentStageID);
            }
           
       
        }
        /// <summary>
        /// 根据阶段获取可重修的班级
        /// </summary>
        /// <param name="Stage">阶段</param>
        /// <returns></returns>
        public List<ClassSchedule> RebuildStage(int Stage)
        {
            List<ClassSchedule> ListClass = new List<ClassSchedule>();
            ListClass.AddRange(this.GetList().Where(a => a.grade_Id == Stage && a.ClassstatusID == null).ToList());
            var ListStage = this.RecursionStage(Stage);
            foreach (var item in ListStage)
            {
                ListClass.AddRange(this.GetList().Where(a => a.grade_Id == item.CurrentStageID&&a.ClassstatusID==null).ToList());
            }
            return ListClass;
        }
        /// <summary>
        /// 根据阶段获取前面阶段
        /// </summary>
        /// <param name="Stage">阶段id</param>
        /// <returns></returns>
        public List<Grand> Grandbehind(int Stage)
        {
            List<Grand> grands = new List<Grand>();
           
              var ListStage = this.RecursionStage(Stage);
            foreach (var item in ListStage)
            {
                grands.Add(Grandcontext.GetEntity(item.CurrentStageID));
            }
            grands.Add(Grandcontext.GetEntity(Stage));
            return grands;
        }
        /// <summary>
        /// 重修数据申请提交
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        public AjaxResult RebuildAdd(TransactionView transactionView)
        {
            AjaxResult result = null;
            try
            {
                result = new SuccessResult();
                result.Success = true;
                if (this.Changeprocessing(transactionView.StudentID) < 1)
                {
                    ApplicationRepair transfer = new ApplicationRepair();
                    transfer.IsDelete = false;
                    transfer.Reason = transactionView.Reason;
                    transfer.Repairtime = transactionView.Dateofapplication;
                    transfer.StudentID = transactionView.StudentID;
                    transfer.Rehabilit = transactionView.NowCLass;
                    transfer.Addtime = DateTime.Now;
                    var boolTransfer = classDynamicsBusiness.GetList().Where(a => a.IsaDopt == null && a.Studentnumber == transfer.StudentID && a.ApplicationRepairID != null).ToList();
                    if (boolTransfer.Count() < 1)
                    {
                        ApplicationRepairBusiness.Insert(transfer);
                        ClassDynamics classDynamics = new ClassDynamics();
                        classDynamics.IsaDopt = null;
                        classDynamics.Addtime = DateTime.Now;
                        classDynamics.CurrentClass = transactionView.NowCLass;
                        classDynamics.FormerClass = transactionView.OriginalClass;
                        classDynamics.Studentnumber = transactionView.StudentID;
                        classDynamics.Dormitoryaddress = transactionView.Dormitoryaddress;
                        var tranID = ApplicationRepairBusiness.GetList().Where(a => a.IsDelete == false && a.StudentID == transactionView.StudentID && a.Rehabilit == transactionView.NowCLass).ToList().OrderByDescending(a => a.Id).FirstOrDefault();
                        classDynamics.ApplicationRepairID = tranID.Id;
                        classDynamics.States = this.FineBasicdat("重修").ID;
                        classDynamicsBusiness.Insert(classDynamics);

                        BusHelper.WriteSysLog("添加异动数据", EnumType.LogType.添加数据);
                        result.Msg = "申请成功";
                    }
                    else
                    {

                        result.Msg = "请勿重复申请";
                    }
                }
                else
                {
                    result.Msg = "有异动数据未处理";
                }
            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误";
                result.Success = false;
                result.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return result;
        }
        /// <summary>
        /// 休学数据操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Secss">同意或者拒绝</param>
        /// <returns></returns>
        public AjaxResult SuspensionofschoolAdd(int ID, string Secss)
        {
            var x = classDynamicsBusiness.GetEntity(ID);

            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                if (Secss == "Yes")
                {
                    //休学的将学生移除宿舍并结算保险柜押金
                    int? Suspensionid = x.SuspensionofschoolID;
                    Suspensionofschool SuspenEntity = SuspensionofschoolBusiness.GetEntity(Suspensionid);
                    string accsql = "select * from Accdationinformation where studentnumber = " + x.Studentnumber + " and EndDate is null";
                    Accdationinformation Acc = Accdation.GetListBySql<Accdationinformation>(accsql).FirstOrDefault();
                    Acc.EndDate = SuspenEntity.Startingperiod;
                    Acc.IsDel = true;
                    Accdation.Update(Acc);

                    //结算押金
                    Accdationinformation accdate = Accdation.GetListBySql<Accdationinformation>("select top 1 * from Accdationinformation where Studentnumber=" + x.Studentnumber + " order by StayDate asc").FirstOrDefault();
                    Accdationinformation accdate1 = Accdation.GetListBySql<Accdationinformation>("select top 1 * from Accdationinformation where Studentnumber=" + x.Studentnumber + " order by StayDate desc").FirstOrDefault();
                    var months = ((SuspenEntity.Startingperiod.Year - accdate.StayDate.Year) * 12) + SuspenEntity.Startingperiod.Month - accdate.StayDate.Month;
                    Pricedormitoryarticles Price_List = PriceManger.GetList().Where(s => s.Nameofarticle.Contains("保险柜每月扣费")).FirstOrDefault();
                    Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
                    DormitoryDeposit dormitory = new DormitoryDeposit()
                    {
                        ID = Guid.NewGuid().ToSequentialGuid(),
                        Maintain = SuspenEntity.Startingperiod,
                        DormId = accdate1.DormId,
                        StuNumber = x.Studentnumber,
                        MaintainGood = Price_List.ID,
                        GoodPrice = months * 10,
                        CompleteTime = DateTime.Now,
                        ChuangNumber= Acc.BedId,
                        MaintainState = 1,
                        CreaDate = DateTime.Now,
                        RepairContent = "每月扣除的保险柜金额",
                        EntryPersonnel = UserName.EmpNumber
                    };
                    Dormitory_Entity.Insert(dormitory);

                    x.IsaDopt = true;
                    var StudenClass = ss.GetList().Where(a => a.StudentID == x.Studentnumber && a.CurrentClass == true).FirstOrDefault();
                    StudenClass.CurrentClass = false;
                    ss.Update(StudenClass);
                    CLassdynamic.Update(x);
                    BusHelper.WriteSysLog("异动数据修改", EnumType.LogType.添加数据);
                    retus.Msg = "操作成功";
                }
                else
                {
                    x.IsaDopt = false;
                    CLassdynamic.Update(x);
                    BusHelper.WriteSysLog("修改异动数据", EnumType.LogType.编辑数据);
                    retus.Msg = "审批失败";
                }

            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 休学申请表单数据提交
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        public AjaxResult SuspensionofschoolAdd(TransactionView transactionView)
        {
            AjaxResult result = null;
            try
            {
                result = new SuccessResult();
                result.Success = true;
                if (this.Changeprocessing(transactionView.StudentID) < 1)
                {
                    Suspensionofschool transfer = new Suspensionofschool();
                    transfer.IsDelete = false;//是否删除
                    transfer.Reason = transactionView.Reason;//原因
                    transfer.Dateofapplication = transactionView.Dateofapplication;//申请日期
                    transfer.Studentnumber = transactionView.StudentID;//学号
                    transfer.Startingperiod = transactionView.qBeginTime;//开始时间
                    transfer.Deadline = transactionView.qEndTime;//结束时间



                    var boolTransfer = classDynamicsBusiness.GetList().Where(a => a.IsaDopt == null && a.Studentnumber == transfer.Studentnumber && a.SuspensionofschoolID != null).ToList();
                   
                    if (boolTransfer.Count() < 1)
                    {
                        SuspensionofschoolBusiness.Insert(transfer);
                        ClassDynamics classDynamics = new ClassDynamics();
                        classDynamics.IsaDopt = null;
                        classDynamics.Addtime = DateTime.Now;
                        classDynamics.FormerClass = transactionView.OriginalClass;
                        classDynamics.Studentnumber = transactionView.StudentID;
                        classDynamics.Dormitoryaddress = transactionView.Dormitoryaddress;
                        var tranID = SuspensionofschoolBusiness.GetList().Where(a => a.IsDelete == false && a.Studentnumber == transactionView.StudentID && a.Dateofapplication == transactionView.Dateofapplication && a.Startingperiod == transactionView.qBeginTime && a.Deadline == transactionView.qEndTime).ToList().OrderByDescending(a => a.id).FirstOrDefault();
                        classDynamics.SuspensionofschoolID = tranID.id;
                        classDynamics.States = this.FineBasicdat("休学").ID;
                        classDynamicsBusiness.Insert(classDynamics);

                        BusHelper.WriteSysLog("添加异动数据", EnumType.LogType.添加数据);
                        result.Msg = "申请成功";
                    }
                    else
                    {

                        result.Msg = "请勿重复申请";
                    }
                }
                else
                {
                    result.Msg = "有异动申请未处理";
                }
            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误";
                result.Success = false;
                result.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return result;
        }
        /// <summary>
        /// 复学页面表单提交
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        public AjaxResult ResumptionofstudyAdd(TransactionView transactionView)
        {
            AjaxResult result = null;
            try
            {
                result = new SuccessResult();
                result.Success = true;
                if (this.Changeprocessing(transactionView.StudentID) < 1)
                {
                    Restudy transfer = new Restudy();
                    transfer.IsDelete = false;//是否删除
                    transfer.Reason = transactionView.Reason;//原因
                    transfer.Applicationtime = transactionView.Dateofapplication;//申请日期
                    transfer.StudentID = transactionView.StudentID;//学号
                    transfer.IsBookcollection = transactionView.IsBookcollection;//是否领书
                    transfer.ClassID = transactionView.NowCLass;//复学班级
                    transfer.Reasonsfordelay = transactionView.Reasonsfordelay;//耽误学业原因
                    var boolTransfer = classDynamicsBusiness.GetList().Where(a => a.IsaDopt == null && a.Studentnumber == transfer.StudentID && a.SuspensionofschoolID != null).ToList();
                  
                    if (boolTransfer.Count() < 1)
                    {
                        RestudyBusines.Insert(transfer);
                        ClassDynamics classDynamics = new ClassDynamics();
                        classDynamics.IsaDopt = null;
                        classDynamics.Addtime = DateTime.Now;
                        classDynamics.FormerClass = transactionView.OriginalClass;
                        classDynamics.CurrentClass = classDynamicsBusiness.GetList().Where(a => a.Studentnumber == transactionView.StudentID && a.IsaDopt != false).OrderByDescending(a => a.ID).FirstOrDefault().FormerClass;
                        classDynamics.Studentnumber = transactionView.StudentID;
                        classDynamics.Dormitoryaddress = transactionView.Dormitoryaddress;
                        var tranID = RestudyBusines.GetList().Where(a => a.IsDelete == false && a.StudentID == transactionView.StudentID && a.Applicationtime == transactionView.Dateofapplication && a.IsBookcollection == transactionView.IsBookcollection).ToList().OrderByDescending(a => a.ID).FirstOrDefault();
                        classDynamics.RestudyID = tranID.ID;
                        classDynamics.States = this.FineBasicdat("复学").ID;
                        classDynamicsBusiness.Insert(classDynamics);

                        BusHelper.WriteSysLog("添加异动数据", EnumType.LogType.添加数据);
                        result.Msg = "申请成功";
                    }
                    else
                    {

                        result.Msg = "请勿重复申请";
                    }
                }
                else
                {
                    result.Msg = "有异动申请未处理";
                }

            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误";
                result.Success = false;
                result.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return result;
        }
        /// <summary>
        /// 复学数据操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Secss">同意或者拒绝</param>
        /// <returns></returns>
        public AjaxResult Resumptionoperation(int ID, string Secss) {
            var x = classDynamicsBusiness.GetEntity(ID);

            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                if (Secss == "Yes")
                {
                    x.IsaDopt = true;
                    CLassdynamic.Update(x);
                    ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                    scheduleForTrainees.ID_ClassName = RestudyBusines.GetEntity(x.RestudyID).ClassID;
                    scheduleForTrainees.ClassID = this.GetEntity(RestudyBusines.GetEntity(x.RestudyID).ClassID).ClassNumber;
                    scheduleForTrainees.StudentID = x.Studentnumber;
                    scheduleForTrainees.CurrentClass = true;
                    scheduleForTrainees.AddDate = DateTime.Now;
                    ss.Insert(scheduleForTrainees);
                    BusHelper.WriteSysLog("异动数据修改", EnumType.LogType.添加数据);
                    retus.Msg = "操作成功";
                }
                else
                {
                    x.IsaDopt = false;
                    CLassdynamic.Update(x);
                    BusHelper.WriteSysLog("修改异动数据", EnumType.LogType.编辑数据);
                    retus.Msg = "审批失败";
                }

            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 开除学员数据申请
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        public AjaxResult ExpelAdd(TransactionView transactionView)
        {
            AjaxResult result = null;
            try
            {
                result = new SuccessResult();
                result.Success = true;
                if (this.Changeprocessing(transactionView.StudentID) < 1) { 
                Expels transfer = new Expels();
                transfer.IsDelete = false;//是否删除
                transfer.Reason = transactionView.Reason;//原因
                transfer.Applicationtime = transactionView.Dateofapplication;//申请日期
                transfer.Studentnumber = transactionView.StudentID;//学号
               
                var boolTransfer = classDynamicsBusiness.GetList().Where(a => a.IsaDopt == null && a.Studentnumber == transfer.Studentnumber && a.ExpelsID != null).ToList();
          
                if (boolTransfer.Count() < 1)
                {
                    ExpelsBusiness.Insert(transfer);
                    ClassDynamics classDynamics = new ClassDynamics();
                    classDynamics.IsaDopt = null;
                    classDynamics.Addtime = DateTime.Now;
                    classDynamics.FormerClass = transactionView.OriginalClass;
                    classDynamics.Studentnumber = transactionView.StudentID;
                    classDynamics.Dormitoryaddress = transactionView.Dormitoryaddress;
                    var tranID = ExpelsBusiness.GetList().Where(a => a.IsDelete == false && a.Studentnumber == transactionView.StudentID && a.Applicationtime == transactionView.Dateofapplication).ToList().OrderByDescending(a => a.id).FirstOrDefault();
                    classDynamics.ExpelsID = tranID.id;
                    classDynamics.States = this.FineBasicdat("开除").ID;
                    classDynamicsBusiness.Insert(classDynamics);

                    BusHelper.WriteSysLog("添加异动数据", EnumType.LogType.添加数据);
                    result.Msg = "申请成功";
                }
                else
                {

                    result.Msg = "请勿重复申请";
                }
                }
                else
                {
                    result.Msg = "有异动申请未处理";
                }

            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误";
                result.Success = false;
                result.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return result;
        }
        /// <summary>
        /// 开除数据操作
        /// </summary>
        /// <param name="ID">异动id</param>Expelperation
        /// <param name="Secss">同意或者拒绝</param>
        /// <returns></returns>
        public AjaxResult Expelperation(int ID, string Secss,string State)
        {
            var x = classDynamicsBusiness.GetEntity(ID);
            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                if (Secss == "Yes")
                {

                    //退学的将学生移除宿舍并结算保险柜押金
                    int? ApplicationDropoutID = x.ApplicationDropoutID;

                    ApplicationDropout ApPEntity = ApplicationDropoutBusiness.GetEntity(ApplicationDropoutID);  
                    Accdationinformation Acc = Accdation.GetListBySql<Accdationinformation>("select * from Accdationinformation where studentnumber = '" + x.Studentnumber + "' and EndDate is null").FirstOrDefault();
                    //删除宿舍
                    if (Acc != null) {
                    Accdation = new AccdationinformationBusiness();
                    Accdation.delacc(x.Studentnumber);
                    if (ApplicationDropoutID !=null) {
                         
                        //结算押金
                        Accdationinformation accdate = Accdation.GetListBySql<Accdationinformation>("select top 1 * from Accdationinformation where Studentnumber=" + x.Studentnumber + " order by StayDate asc").FirstOrDefault();
                        Accdationinformation accdate1 = Accdation.GetListBySql<Accdationinformation>("select top 1 * from Accdationinformation where Studentnumber=" + x.Studentnumber + " order by StayDate desc").FirstOrDefault();
                        var months = ((ApPEntity.Addtime.Value.Year - accdate.StayDate.Year) * 12) + ApPEntity.Addtime.Value.Month - accdate.StayDate.Month;
                        Pricedormitoryarticles Price_List = PriceManger.GetList().Where(s => s.Nameofarticle.Contains("保险柜每月扣费")).FirstOrDefault();
                        Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
                        DormitoryDeposit dormitory = new DormitoryDeposit()
                        {
                            ID = Guid.NewGuid().ToSequentialGuid(),
                            Maintain = ApPEntity.Addtime.Value,
                            DormId = accdate1.DormId,
                            StuNumber = x.Studentnumber,
                            MaintainGood = Price_List.ID,
                            GoodPrice = months * 10,
                            MaintainState = 1,
                            CreaDate = DateTime.Now,
                            CompleteTime = DateTime.Now,
                            ChuangNumber = Acc.BedId,
                            RepairContent = "每月扣除的保险柜金额",
                            EntryPersonnel = UserName.EmpNumber
                        };
                        Dormitory_Entity.Insert(dormitory);
                    }
                    }

                    
                    x.IsaDopt = true;
                    var StudenClass = ss.GetList().Where(a => a.StudentID == x.Studentnumber && a.CurrentClass == true).FirstOrDefault();
                    StudenClass.CurrentClass = false;
                    ss.Update(StudenClass);
                    
                    var Student = studentInformationBusiness.GetEntity(x.Studentnumber);
                    Student.State= this.FineBasicdat(State).ID;
                    studentInformationBusiness.Update(Student);
                    CLassdynamic.Update(x);
                    BusHelper.WriteSysLog("异动数据修改", EnumType.LogType.添加数据);
                    retus.Msg = "操作成功";
                }
                else
                {
                    x.IsaDopt = false;
                    CLassdynamic.Update(x);
                    BusHelper.WriteSysLog("修改异动数据", EnumType.LogType.编辑数据);
                    retus.Msg = "审批失败";
                }

            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 根据学号拿到正在处理的申请
        /// </summary>
        /// <param name="StudentID"></param>
        /// <returns></returns>
        public int Changeprocessing(string StudentID)
        {
           return classDynamicsBusiness.GetList().Where(a => a.Studentnumber == StudentID && a.IsaDopt == null).Count();
        }
        /// <summary>
        /// 退学表单数据提交
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        public AjaxResult DropoutofschoolsAdd(TransactionView transactionView)
        {
            AjaxResult result = null;
            try
            {
                result = new SuccessResult();
                result.Success = true;
                if (this.Changeprocessing(transactionView.StudentID) < 1)
                {
                    
                    ApplicationDropout transfer = new ApplicationDropout();
                    transfer.IsDelete = false;//是否删除
                    transfer.Reasonofdropout = transactionView.Reason;//原因
                    transfer.Addtime = transactionView.Dateofapplication;//申请日期
                    transfer.Studentnumber = transactionView.StudentID;//学号

                    var boolTransfer = classDynamicsBusiness.GetList().Where(a => a.IsaDopt == null && a.Studentnumber == transfer.Studentnumber && a.ExpelsID != null).ToList();

                    if (boolTransfer.Count() < 1)
                    {
                        ApplicationDropoutBusiness.Insert(transfer);
                        ClassDynamics classDynamics = new ClassDynamics();
                        classDynamics.IsaDopt = null;
                        classDynamics.Addtime = DateTime.Now;
                        classDynamics.FormerClass = transactionView.OriginalClass;
                        classDynamics.Studentnumber = transactionView.StudentID;
                        classDynamics.Dormitoryaddress = transactionView.Dormitoryaddress;
                        var tranID = ApplicationDropoutBusiness.GetList().Where(a => a.IsDelete == false && a.Studentnumber == transactionView.StudentID && a.Addtime == transactionView.Dateofapplication).ToList().OrderByDescending(a => a.ID).FirstOrDefault();
                        classDynamics.ApplicationDropoutID = tranID.ID;
                        classDynamics.States = this.FineBasicdat("退学").ID;
                        classDynamicsBusiness.Insert(classDynamics);

                        BusHelper.WriteSysLog("添加异动数据", EnumType.LogType.添加数据);
                        result.Msg = "申请成功";
                    }
                    else
                    {

                        result.Msg = "请勿重复申请";
                    }
                }
                else
                {
                    result.Msg = "有异动申请未处理";
                }

            }
            catch (Exception ex)
            {
                result = new ErrorResult();
                result.Msg = "服务器错误";
                result.Success = false;
                result.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return result;
        }
        //学员保险业务
        private BaseBusiness<DetailedStudentIn> detailedstudentinBusiness;
        /// <summary>
        /// 保险名单去重复
        /// </summary>
        /// <param name="scheduleForTrainees">集合对象</param>
        /// <param name="schedule">数据对象</param>
        /// <returns></returns>
        public bool Insurancerepetition(List<DetailedStudentIn> scheduleForTrainees, DetailedStudentIn schedule)
        {
            foreach (var item in scheduleForTrainees)
            {
                if (item.StudentID == schedule.StudentID)
                {
                    return true;
                }
            }
            return false;

        }
        /// <summary>
        /// 根据班级号获取保险学员
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public List<DetailedStudentIn> premiumGetdate(int ClassID)
        {
            //学员班级
            ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
            var x = scheduleForTraineesBusiness.GetList().Where(a => a.ID_ClassName == ClassID && a.CurrentClass == true).ToList();

            List<DetailedStudentIn> detailedStudentIns = new List<DetailedStudentIn>();

            List<DetailedStudentIn> ins = new List<DetailedStudentIn>();

             List<DetailedStudentIn> detailedList = new List<DetailedStudentIn>();
            detailedstudentinBusiness = new BaseBusiness<DetailedStudentIn>();
            DateTime date = DateTime.Now;
            foreach (var item in x)
            {
             var xm= detailedstudentinBusiness.GetList().Where(a => a.StudentID == item.StudentID && a.Endtime ==null).FirstOrDefault();
                if (xm!=null)
                {
                    detailedStudentIns.Add(xm);
                }
                else
                {
                    var mydeta = detailedstudentinBusiness.GetList().Where(a => a.StudentID == item.StudentID && a.Endtime > date).FirstOrDefault();
                    if (mydeta != null)
                    {
                        detailedStudentIns.Add(mydeta);
                    }
                    else
                    {
                        detailedList.AddRange(detailedstudentinBusiness.GetList().Where(a => a.StudentID == item.StudentID && a.Endtime <= date).ToList());
                    }
                } 
            }
            foreach (var item in detailedList.OrderByDescending(a=>a.ID))
            {
                if (!this.Insurancerepetition(ins, item))
                {
                    ins.Add(item);
                }
            }
            detailedList = ins;

            for (int i = detailedList.Count - 1; i >= 0; i--)
            {
                if (detailedList.Count > 0)
                {
                    foreach (var item1 in detailedStudentIns)
                    {
                        if (item1.StudentID == detailedList[i].StudentID)
                        {
                            detailedList.Remove(detailedList[i]);
                            break;
                        }
                    }
                }
            }
            detailedStudentIns.AddRange(detailedList);
            return detailedStudentIns;
        }
        /// <summary>
        /// 获取保险及班级所有学员
        /// </summary>
        /// <param name="ClassID">班级id</param>
        /// <returns></returns>
        public List<InsuranceStudentView> InsuranceStudent(int ClassID)
        {
            var student = ss.ClassStudent(ClassID);
            List<InsuranceStudentView> insuranceStudentViews = new List<InsuranceStudentView>();

            detailedstudentinBusiness = new BaseBusiness<DetailedStudentIn>();
            foreach (var item in student)
            {
               var x= detailedstudentinBusiness.GetList().Where(a => a.StudentID == item.StudentNumber).OrderByDescending(a => a.ID).FirstOrDefault();
                if (x!=null)
                {
                    InsuranceStudentView view = new InsuranceStudentView();
                    view.StudentID = item.StudentNumber;
                    view.StuID= item.StudentNumber.Substring(item.StudentNumber.Length - 5);
                    view.Name = item.Name;
                    if (x.Endtime==null)
                    {
                        view.Count = 4;
                        view.Context = "数据未更新";
                    }else if(x.Endtime<=DateTime.Now){
                        view.Count = 2;
                        view.Context = x.Endtime.ToString();
                    }
                    else if (x.Endtime>DateTime.Now)
                    {
                        view.Count = 1;
                        view.Context = x.Endtime.ToString();
                    }
                    insuranceStudentViews.Add(view);
                }
                else
                {
                    InsuranceStudentView view = new InsuranceStudentView();
                    view.StudentID = item.StudentNumber;
                    view.StuID = item.StudentNumber.Substring(item.StudentNumber.Length - 5);
                    view.Name = item.Name;
                    view.Count = 3;
                    view.Context = "未购买";
                    insuranceStudentViews.Add(view);
                }
               
            }
            return insuranceStudentViews;
        }
        /// <summary>
        /// 学员保险数据添加
        /// </summary>
        /// <param name="insuranceStudentView">数据对象</param>
        /// <returns></returns>
        public AjaxResult InsuranceAdd(InsuranceView insuranceStudentView)
        {
            AjaxResult retus = null;
            try
            {
                List<DetailedStudentIn> detaileds = new List<DetailedStudentIn>();
                detailedstudentinBusiness = new BaseBusiness<DetailedStudentIn>();
                string[] Student = insuranceStudentView.StudentID.Split(',');
                foreach (var item in Student)
                {
                    DetailedStudentIn studentIn = new DetailedStudentIn();
                    studentIn.StudentID = item;
                    studentIn.Endtime = insuranceStudentView.Duedate;
                    studentIn.Starttime = insuranceStudentView.Startdate;
                    studentIn.Addtime = DateTime.Now;
                    studentIn.InsurancePremium = insuranceStudentView.premium;
                    studentIn.Remarks = insuranceStudentView.Remarks;
                    detaileds.Add(studentIn);
                }
                detailedstudentinBusiness.Insert(detaileds);
                retus = new SuccessResult();
                retus.Success = true;
        
                BusHelper.WriteSysLog("保险数据添加", EnumType.LogType.添加数据);
                retus.Msg = "操作成功";

            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 保险数据补充
        /// </summary>
        /// <param name="insuranceView">数据对象</param>
        /// <returns></returns>
        public AjaxResult SupplementInsuran(InsuranceView  insuranceView)
        {
            AjaxResult retus = null;
            try
            {
                List<DetailedStudentIn> detaileds = new List<DetailedStudentIn>();
                detailedstudentinBusiness = new BaseBusiness<DetailedStudentIn>();
                string[] Student = insuranceView.StudentID.Split(',');
                foreach (var item in Student)
                {
                    DetailedStudentIn  studentIn= detailedstudentinBusiness.GetEntity(int.Parse(item));
                    studentIn.Endtime = insuranceView.Duedate;
                    studentIn.Starttime = insuranceView.Startdate;
                    studentIn.InsurancePremium = insuranceView.premium;
                    studentIn.Remarks = insuranceView.Remarks;
                    detaileds.Add(studentIn);
                }
                detailedstudentinBusiness.Update(detaileds);
                retus = new SuccessResult();
                retus.Success = true;

                BusHelper.WriteSysLog("保险数据修改", EnumType.LogType.编辑数据);
                retus.Msg = "操作成功";
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 修改班级上课时间段
        /// </summary>
        /// <param name="ClassID">班级id</param>
        /// <param name="Types">上午或者下午id</param>
        /// <returns></returns>
        public AjaxResult Modifyclasstime(int ClassID,int Types)
        {
          var x=  this.GetEntity(ClassID);
            AjaxResult retus = null;
            try
            {
                x.BaseDataEnum_Id = Types;
                this.Update(x);
                retus = new SuccessResult();
                retus.Success = true;  

                BusHelper.WriteSysLog("班级上课时间段数据修改成功", EnumType.LogType.编辑数据);
                retus.Msg = "更改成功";
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.系统异常);
            }
            return retus;
        }
        /// <summary>
        /// 获取毕业班级
        /// </summary>
        /// <returns></returns>
        public List<ClassSchedule> Graduatingclass()
        {
            var GradID = classtatus.GetList().Where(a => a.IsDelete == false && a.TypeName == "毕业").FirstOrDefault().id;
           return this.GetList().Where(a => a.IsDelete == false && a.ClassstatusID == GradID).ToList();
        }
        /// <summary>
        /// 获取正常班级
        /// </summary>
        /// <returns></returns>
        public List<ClassSchedule> Normalclass()
        {
            return this.GetList().Where(a => a.IsDelete == false && a.ClassstatusID == null).ToList();
        }
        /// <summary>
        /// 通过班级id获取阶段对象
        /// </summary>
        /// <param name="ClassID">班级id</param>
        /// <returns></returns>
        public Grand ClassGrand(int ClassID)
        {
         return   Grandcontext.GetGrandByID(this.GetEntity(ClassID).grade_Id);
        }
        /// <summary>
        /// 根据班级id获取到带班老师，就业专业及班主任,如果没查到请验证员工编号为空
        /// </summary>
        /// <param name="ClaaID">班主任</param>
        /// <returns></returns>
        public EmployeesInfo HeadSraffFine( int ClaaID)
        {
            //就业专业带班业务类
            EmpClassBusiness empClassBusiness = new EmpClassBusiness();
            //就业专业表
            EmploymentStaffBusiness employmentStaffBusiness = new EmploymentStaffBusiness();

            EmployeesInfo info = new EmployeesInfo();
            //获取到员工对象
         
            if (!this.ClassGrand(ClaaID).GrandName.Contains("S4"))
            {
                info = Hadmst.HeadmastaerClassFine(ClaaID) == null ? new EmployeesInfo() : Hadmst.ClassHeadmaster(ClaaID);
            }
            else
            {
                if (Hadmst.HeadmastaerClassFine(ClaaID)==null)
                {
                    //拿到就业专员带班对象返回就业专员表
                    var empclassid = empClassBusiness.GetStaffByClassids(ClaaID);
                    if (empclassid.ID>0)//验证就业专员带班是否为空
                    {
                        info = employmentStaffBusiness.GetEmpInfoByEmpID(empclassid.ID) == null ? new EmployeesInfo() : employmentStaffBusiness.GetEmpInfoByEmpID(empclassid.ID);
                    }
                    else
                    {
                        info= new EmployeesInfo();
                    }

                 
                }
                else
                {
                    info = Hadmst.ClassHeadmaster(ClaaID);
                }
                   
            }
            return info;
        }
        /// <summary>
        /// 根据班级获取同名班级业务信息
        /// </summary>
        /// <param name="ClassNumber"></param>
        /// <returns></returns>
        public object ClassProDate(string ClassNumber)
        {
         var x= this.GetList().Where(a => a.ClassNumber == ClassNumber.ToUpper()).ToList();
           var z= x.Select(a => new
            {
               a.id,
                a.ClassNumber,
                Grandcontext.GetEntity(a.grade_Id).GrandName,
                TypeName = a.ClassstatusID == null ? "正常" : classtatus.GetEntity(a.ClassstatusID).TypeName
            });
            return z;
        }
        /// <summary>
        /// 通过班级id获取学员
        /// </summary>
        /// <param name="ClassID">班级id</param>
        /// <returns></returns>
        public List<StudentInformation> ClassListStudent(int ClassID)
        {
            List<StudentInformation> studentsmy = new List<StudentInformation>();
           var studentid= ss.GetList().Where(a => a.ID_ClassName == ClassID && a.CurrentClass == true && a.IsGraduating == false).ToList();
          
            if (studentid.Count<1)
            {
                return null;
            }
            foreach (var item in studentid)
            {
                studentsmy.Add(studentInformationBusiness.GetEntity(item.StudentID));
            }
            return studentsmy;
        }
        /// <summary>
        /// 获取当前班主任带班的所有学员数据，主任即可查看全部
        /// </summary>
        /// <returns></returns>
        public List<StudentInformation> HeadteStudent()
        {
            //学员数据
            List<StudentInformation> studentInformation = new List<StudentInformation>();
            var EmpNumber = Base_UserBusiness.GetCurrentUser().EmpNumber;
            //岗位
            var Position = employeesInfoManage.GetPositionByEmpid(EmpNumber);
            //部门
            var Dept = employeesInfoManage.GetDeptByEmpid(EmpNumber);
            if (Hadmst.GetList().Where(a => a.informatiees_Id == EmpNumber).FirstOrDefault() == null)
            {
                studentInformation= studentInformationBusiness.GetList().Where(a => a.IsDelete != true).ToList();
            }
            else if (Position.PositionName.Contains("教质主任")|| Position.PositionName.Contains("教质副主任") &&  Position.DeptId== Dept.DeptId)
            {
                var x = this.Departmentclass(Dept.DeptName);
                foreach (var item in x)
                {
                    if (this.ClassListStudent(item.id)!=null)
                    {
                        studentInformation.AddRange(this.ClassListStudent(item.id));
                    }
                  
                }
               
            }
            else
            {
                //获取班主任
                var heamstEmp = Hadmst.GetList().Where(a => a.informatiees_Id == EmpNumber).FirstOrDefault().ID;
                //获取带班的班级
                var x = Hadmst.EmpClass(heamstEmp,true);
                foreach (var item in x)
                {
                    if (this.ClassListStudent(item.id) != null)
                    {
                        studentInformation.AddRange(this.ClassListStudent(item.id));
                    }
                   
                }

            }
            return studentInformation;
        }
        /// <summary>
        /// 通过部门名称获取部门负责的班级
        /// </summary>
        /// <param name="DeptName"></param>
        /// <returns></returns>
        public List< ClassSchedule> Departmentclass(string DeptName)
        {
            //拿到部分负责班级的阶段
            List<Grand> grands = Grandcontext.GetList();
            if (DeptName.ToLower().Contains("s1"))
            {
                grands = Grandcontext.GetList().Where(a => a.IsDelete == false && a.GrandName == "S1" || a.GrandName == "S2" || a.GrandName == "Y1").ToList();
            }
            else
            {
                grands = Grandcontext.GetList().Where(a => a.IsDelete == false && a.GrandName == "S3" || a.GrandName == "S4").ToList();
            }
            //该班主任所有可负责的班级
            List<ClassSchedule> classesList = new List<ClassSchedule>();
            foreach (var item in grands)
            {
                classesList.AddRange(this.GetList().Where(a => a.ClassstatusID == null && a.grade_Id == item.Id).ToList());
            }
            return classesList;
        }
        //对象存储boss
        CloudstorageBusiness cloudstorageBusiness = new CloudstorageBusiness();
        /// <summary>
        /// 班主任详情
        /// </summary>
        /// <returns></returns>
        public SuccessionrecordView Headteacherdetails()
        {
            var EmpNumber = Base_UserBusiness.GetCurrentUser().EmpNumber;
           
            var employees = employeesInfoManage.GetInfoByEmpID(EmpNumber);
            var department = employeesInfoManage.GetDeptByEmpid(EmpNumber);
            var Headyees = new SuccessionrecordView
            {
                Education = employees.Education,//学历
                EmpName = employees.EmpName,//姓名
                Sex = employees.Sex,//性别
                Phone = employees.Phone,//电话
                DeptName = department.DeptName,//部门
                Images = cloudstorageBusiness.ImagesFine("xinxihua", "EmpImage", employees.Image,20)//图片
            };

            return Headyees;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentID">学号</param>
        /// <param name="date">时间</param>
        /// <param name="">a=1是班级编号 a=2是阶段编号</param>
        /// <returns></returns>
        public int GetStudentDate(string studentID, DateTime date,int a)
        {
            BaseBusiness<ScheduleForTrainees> ScheduleForTrainees = new BaseBusiness<ScheduleForTrainees>();
            BaseBusiness<ClassSchedule> ClassSchedule = new BaseBusiness<ClassSchedule>();
            int i = 0;
            if (a == 1)
            {
                var classID = ScheduleForTrainees.GetList().Where(d => d.StudentID == studentID && d.AddDate == date).SingleOrDefault();
                i = classID.ID_ClassName;
                
            }
            else if(a==2){
                var GreadID = ScheduleForTrainees.GetList().Where(d => d.StudentID == studentID && d.AddDate == date).SingleOrDefault();
                var grandID = ClassSchedule.GetList().Where(d => d.id == GreadID.ID_ClassName).SingleOrDefault();
                i = grandID.grade_Id;
            }
            return i;
        }

    }
}
