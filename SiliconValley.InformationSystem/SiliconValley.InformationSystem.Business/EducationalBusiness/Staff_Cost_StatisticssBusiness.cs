using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
////////////////////////////////////////
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using Newtonsoft.Json.Linq;
using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using System.Xml;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
    using SiliconValley.InformationSystem.Business.CourseSchedulingSysBusiness;
    using SiliconValley.InformationSystem.Business.Coursewaremaking_Business;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
    using System.IO;
    using System.Net;

    /// <summary>
    ///员工费用统计--教务处
    /// </summary>
    public class Staff_Cost_StatisticssBusiness
    {
        /// <summary>
        /// 员工业务类实例
        /// </summary>
        private EmployeesInfoManage db_emp;

        public BaseBusiness<HeadClass> HeadClass_Entity = new BaseBusiness<HeadClass>();

        public HeadmasterBusiness Headmaster_Entity = new HeadmasterBusiness();

        public ReconcileManeger Reconcile_Entity = new ReconcileManeger();

        public EmployeesInfoManage EmployeesInfoManage_Entity = new EmployeesInfoManage();

        public ClassScheduleBusiness ClassSchedule_Entity = new ClassScheduleBusiness();
        public ScheduleForTraineesBusiness ScheduleForTrainees_Entity = new ScheduleForTraineesBusiness();
        public CurriculumBusiness curriculum_Entity = new CurriculumBusiness();
        public GrandBusiness Grand_Entity = new GrandBusiness();
        public BaseBusiness<TeacherAddorBeonDutyView> teacherAddorBeonDutyView_Entity = new BaseBusiness<TeacherAddorBeonDutyView>();
        public ExaminationRoomBusiness ExaminationRoom_Entity = new ExaminationRoomBusiness();
        public ExaminationBusiness Examination_Entity = new ExaminationBusiness();
        public BaseBusiness<MarkingArrange> Marking_Entity = new BaseBusiness<MarkingArrange>();
        public CandidateInfoBusiness Candi_Entity = new CandidateInfoBusiness();
        public CoursewaremakingBusiness Courseware_Entity = new CoursewaremakingBusiness();

        public Staff_Cost_StatisticssBusiness()
        {
            db_emp = new EmployeesInfoManage();
        }

        /// <summary>
        /// 返回所有员工
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> Emplist()
        {
            return db_emp.GetAll(); //没有离职的员工

        }

        /// <summary>
        /// 统计课时费
        /// </summary>
        /// <param name="Emp_List">员工集合</param>
        /// <param name="dt">日期</param>
        /// <param name="WorkDay">工作日</param>
        /// <returns></returns>
        public List<Staff_CostView> CostTimeFee(List<EmployeesInfo> Emp_List,DateTime dt, int WorkDay)
        {
            List<Staff_CostView> staff_list = new List<Staff_CostView>();

            decimal? Cost_fee = 0;
            int QuanDay = 0;//全天课天数
            int ClassTime = 50;//底课时
            int Duty_fee = 0;//值班费
            int Invigilation_fee = 0;//监考费
            int Marking_fee = 0;//阅卷费
            int Super_class = 0;//超带班
            int Internal_training_fee = 0;//内训费
            int RD_fee = 0;//研发费
            

            for (int i = 0; i < Emp_List.Count; i++)
            {
                Staff_CostView staff = new Staff_CostView();

                DateTime NowTime = DateTime.Now;

                if ((NowTime.Year - Emp_List[i].EntryTime.Year) >= 2)
                {
                    ClassTime = 40;
                }
                
                string sqlstr = $"select * from Reconcile  where Year(AnPaiDate)={dt.Year} and Month(AnPaiDate) = {dt.Month} and EmployeesInfo_Id ={ Emp_List[i].EmployeeId }";
                List<Reconcile> mydata = Reconcile_Entity.GetListBySql<Reconcile>(sqlstr).ToList();

                //筛选出前预科的数据
                var qianyuke = mydata.Where(a => a.Curriculum_Id == "前预科").ToList();
                //根据时间分组
                var AnPaiGroup = (
                    from m in mydata
                    group m by m.AnPaiDate into list
                    select list
                    ).ToList();

                //根据课程分组
                var ClassGroup1 = (
                    from m in mydata
                    group m by m.Curriculum_Id into list
                    select list).ToList();

                //根据班级id分组   作用：前预科：根据班级iD看班级有多少人
                var ClassScheduleGroup = (
                    from m in qianyuke
                    group m by m.ClassSchedule_Id into list
                    select list).ToList();

                //班级id分组   带班数量
                var ClassScheduleCount = (
                    from m in mydata
                    group m by m.ClassSchedule_Id into list
                    select list).ToList();


                var ClassGroup = ClassGroup1.Where
                    (a => a.Key != "复习").ToList();//&& a.Key != "项目答辩"&& !a.Key.Contains("职素") && !a.Key.Contains("班")


                //计算全天课天数&& a.Curriculum_Id != "项目答辩"
                for (int k = 0; k < AnPaiGroup.Count; k++)
                {
                    QuanDay += Cost_EndClass(AnPaiGroup[k].Key, Emp_List[i].EmployeeId);
                }

                if (EmployeesInfoManage_Entity.GetPositionByEmpid(Emp_List[i].EmployeeId).PositionName == "教学主任")
                {
                    ClassTime = ClassTime * QuanDay / WorkDay;
                }

                if (!EmployeesInfoManage_Entity.GetDeptByEmpid(Emp_List[i].EmployeeId).DeptName.Contains("s1、s2教学部"))
                {
                    ClassTime = 0;
                }

                int FirstStage = 0;//第一阶段  预科，S1,S2 ---55
                int SecondStage = 0;//第二阶段 S3,   ----65
                int ThreeStage = 0;//S4 ----70   测试和ui ----65    班级名称中UI和CUI为UI班，TA为测试班
                int OtherStage = 0;//其他  语，数，英，职素，班会，军事
                for (int j = 0; j < ClassGroup.Count; j++)
                {
                    //判断是否为“前预科”
                    if (ClassGroup[j].Key == "前预科")
                    {
                        for (int q = 0; q < ClassScheduleGroup.Count; q++)
                        {
                            ClassSchedule schedule = ClassSchedule_Entity.GetEntity(ClassScheduleGroup[q].Key);
                            string sql = $"select * from ScheduleForTrainees where ClassID='{schedule.ClassNumber}' and CurrentClass=1";
                            List<ScheduleForTrainees> Trainees_List = ScheduleForTrainees_Entity.GetListBySql<ScheduleForTrainees>(sql);
                            if (Trainees_List.Count < 10)
                            {
                                FirstStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, "前预科", false);
                            }
                            else
                            {
                                FirstStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, "前预科", true);
                            }
                        }
                    }
                    else
                    {
                        //根据课程名称获取第一条数据 && && a.Curriculum_Id != ""
                        Reconcile reconcile = mydata.Where(a => a.Curriculum_Id == ClassGroup[j].Key).FirstOrDefault();
                        //根据班级id查询单条数据
                        ClassSchedule classSchedule = ClassSchedule_Entity.GetEntity(reconcile.ClassSchedule_Id);
                        //根据课程名称以及阶段id筛选
                        Curriculum curriculum = curriculum_Entity?.GetList()?.FirstOrDefault(a => a?.CourseName == reconcile?.Curriculum_Id && a?.Grand_Id == classSchedule?.grade_Id);
                        //去除语文课之类的
                        Curriculum curriculum1 = curriculum_Entity.GetList()
                            .Where(a => !a.CourseName.Contains("语文") &&
                            !a.CourseName.Contains("数学") &&
                            !a.CourseName.Contains("英语") &&
                            !a.CourseName.Contains("职素") &&
                            !a.CourseName.Contains("班会") &&
                            !a.CourseName.Contains("军事"))
                            .FirstOrDefault(a => a.CourseName == reconcile.Curriculum_Id && a.Grand_Id == classSchedule.grade_Id);

                        if (curriculum != null) {
                            
                        if (curriculum.CourseName.Contains("语文") ||
                            curriculum.CourseName.Contains("数学") ||
                            curriculum.CourseName.Contains("英语") ||
                            curriculum.CourseName.Contains("班会") ||
                            curriculum.CourseName.Contains("军事"))
                        {
                            OtherStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                        }
                        else if (curriculum.CourseName.Contains("职素"))
                        {

                            int zhisuClassTime = Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                            int totalClassTime = (zhisuClassTime / 4) + zhisuClassTime;
                            OtherStage += zhisuClassTime;
                        }
                        else
                        {
                            Grand grand = Grand_Entity.GetEntity(curriculum1.Grand_Id);
                            if (grand.GrandName.Contains("S1") || grand.GrandName.Contains("S2") || grand.GrandName.Contains("Y1"))
                            {
                                FirstStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                            }
                            else if (grand.GrandName.Contains("S3"))
                            {
                                SecondStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                            }
                            else if (grand.GrandName.Contains("S4"))
                            {
                                    if (classSchedule.ClassNumber.Contains("UI") || classSchedule.ClassNumber.Contains("CUI") || classSchedule.ClassNumber.Contains("TA"))
                                    {
                                        SecondStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                                    }
                                    else {
                                         ThreeStage += Reconcile_Entity.GetTeacherClassCount(dt.Year, dt.Month, Emp_List[i].EmployeeId, curriculum.CourseName, true);
                                    }
                            }
                        }
                    }
                    }
                }
                //课时费计算总额    //底课时

                if (FirstStage > 0)
                {

                    FirstStage = FirstStage - ClassTime;
                    Cost_fee += FirstStage * 55;
                }

                if (SecondStage > 0)
                {
                    SecondStage = SecondStage - ClassTime;
                    Cost_fee += SecondStage * 65;
                }

                if (ThreeStage > 0)
                {
                    ThreeStage = ThreeStage - ClassTime;
                    Cost_fee += ThreeStage * 70;
                }

                if (OtherStage > 0)
                {
                    if (EmployeesInfoManage_Entity.GetDeptByEmpid(Emp_List[i].EmployeeId).DeptName.Contains("教学部"))
                    {
                        OtherStage = OtherStage - ClassTime;
                    }

                    Cost_fee += OtherStage * 30;
                }


                //计算值班费
                //查询当年 年月的值班数据
                string TeacherAddsql = @"select * from TeacherAddorBeonDutyView  where YEAR(Anpaidate)=" + dt.Year + "" +
                    " and Month(Anpaidate)=" + dt.Month + "";
                List<TeacherAddorBeonDutyView> TearcherAdd_List = teacherAddorBeonDutyView_Entity.GetListBySql<TeacherAddorBeonDutyView>(TeacherAddsql);
                //根据时间分组
                var TeacherAddGroup = (
                    from m in TearcherAdd_List
                    group m by m.Anpaidate into list
                    select list).ToList();

                //一节  50    两节   80
                for (int g = 0; g < TeacherAddGroup.Count; g++)
                {
                    string sql = @"select * from TeacherAddorBeonDutyView  where YEAR(Anpaidate)=" + TeacherAddGroup[g].Key.Year + "" +
                    " and Month(Anpaidate)=" + TeacherAddGroup[g].Key.Month + " and Day(Anpaidate)=" + TeacherAddGroup[g].Key.Day + " and Tearcher_Id=" + Emp_List[i].EmployeeId + "";
                    List<TeacherAddorBeonDutyView> TearcherAdd = teacherAddorBeonDutyView_Entity.GetListBySql<TeacherAddorBeonDutyView>(sql);
                    if (TearcherAdd.Count == 1)
                    {
                        Duty_fee += 50;
                    }
                    else if (TearcherAdd.Count == 2)
                    {
                        Duty_fee += 80;
                    }
                }

                //计算监考费
                int InvigilationCount = 0;
                string ExaminationRoomSql = "select * from ExaminationRoom where Invigilator1 = '" + Emp_List[i].EmployeeId + "' or Invigilator2='" + Emp_List[i].EmployeeId + "'";
                List<ExaminationRoom> ExamRoomList = ExaminationRoom_Entity.GetListBySql<ExaminationRoom>(ExaminationRoomSql);
                for (int t = 0; t < ExamRoomList.Count; t++)
                {
                    string ExamDateSql = "select * from Examination where year(BeginDate) = '" + dt.Year + "' and Month(BeginDate)='" + dt.Month + "' and ID =" + ExamRoomList[t].Examination + "";

                    if (Examination_Entity.GetListBySql<Examination>(ExamDateSql).FirstOrDefault() != null)
                    {
                        InvigilationCount += 1;
                    }
                }
                Invigilation_fee += InvigilationCount * 20;

                //计算阅卷费
                int StuCount = 0;
                string MarkingSql = "select * from MarkingArrange where MarkingTeacher=" + Emp_List[i].EmployeeId + "";
                List<MarkingArrange> MarkingList = Marking_Entity.GetListBySql<MarkingArrange>(MarkingSql);
                for (int k = 0; k < MarkingList.Count; k++)
                {
                    string ExaminationSql = "select * from Examination where year(BeginDate) = '" + dt.Year + "' and Month(BeginDate)='" + dt.Month + "' and ID =" + MarkingList[k].ExamID + "";
                    List<Examination> ExamList = Examination_Entity.GetListBySql<Examination>(ExaminationSql);
                    if (ExamList.Count > 0)
                    {
                        string CandiSql = "select * from CandidateInfo where Examination =" + ExamList[0].ID + "";
                        List<CandidateInfo> CandiList = Candi_Entity.GetListBySql<CandidateInfo>(CandiSql);
                        StuCount += CandiList.Count;
                    }
                }
                //读取配置文件
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/Cost.xml"));

                //根节点
                var xmlRoot = xmlDocument.DocumentElement;

                //获取标准费用
                var cost = xmlRoot.GetElementsByTagName("MarkingCost")[0].InnerText;
                Marking_fee = int.Parse(cost) * StuCount;

                //计算教材研发费用
                var PPT_NodeCost = xmlRoot.GetElementsByTagName("PPT_NodeCost")[0].InnerText;
                var TextBook_NodeCost = xmlRoot.GetElementsByTagName("TextBook_NodeCost")[0].InnerText;
                string CoursewareSql = "select * from Coursewaremaking where year(submissiontime)=" + dt.Year + " and month(submissiontime)=" + dt.Year + " and RampdpersonID =" + Emp_List[i].EmployeeId + "";
                List<Coursewaremaking> CoursewareList = Courseware_Entity.GetListBySql<Coursewaremaking>(CoursewareSql);
                for (int h = 0; h < CoursewareList.Count; h++)
                {
                    if (CoursewareList[h].MakingType == "Word")
                    {
                        RD_fee += Convert.ToInt32(TextBook_NodeCost);
                    }
                    else if (CoursewareList[h].MakingType == "PPT")
                    {
                        RD_fee += Convert.ToInt32(PPT_NodeCost);
                    }
                }

                //计算班主任超带班   7-15天  200    15-30天   300  
                int jibenban = 3;  //主任带班2个   班主任带班3个
                if (EmployeesInfoManage_Entity.GetDeptByEmpid(Emp_List[i].EmployeeId).DeptName.Contains("教质部"))
                {

                    //根据用户id查出班主任带班id
                    string HeadMasterSql = "select * from Headmaster where informatiees_Id=" + Emp_List[i].EmployeeId + " and IsDelete=0";
                    Headmaster header = Headmaster_Entity.GetListBySql<Headmaster>(HeadMasterSql).FirstOrDefault();

                    string Super_classSql = @"select * from Headclass where (Year(EndingTime)=" + dt.Year + " " +
                        "and Month(Endingtime)>=" + dt.Month + " and Year(LeadTime)<=" + dt.Year + " " +
                        "and Month(LeadTime)<=" + dt.Month + " or (EndingTime is null ))and " +
                        " LeaderID =" + header.ID + " order by LeadTime";
                    List<HeadClass> HeadClassList = HeadClass_Entity.GetListBySql<HeadClass>(Super_classSql).ToList();
                    if (EmployeesInfoManage_Entity.GetPositionByEmpid(Emp_List[i].EmployeeId).PositionName == "教质主任")
                    {
                        jibenban = 2;
                    }

                    if (HeadClassList.Count > jibenban)
                    {
                        //根据带班时间分组
                        for (int y = 0; y < HeadClassList.Count - jibenban; y++)
                        {
                            DateTime datetime1 = DateTime.Parse(HeadClassList[HeadClassList.Count - y - 1].LeadTime.ToString());
                            if (HeadClassList[HeadClassList.Count - y - 1].EndingTime == null)
                            {
                                Super_class += 300;
                            }
                            else
                            {
                                int days = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(dt.Year, dt.Month);
                                //结束时间中的天
                                int daibanday = DateTime.Parse(HeadClassList[HeadClassList.Count - y - 1].EndingTime.ToString()).Day;
                                if (daibanday >= 7 && daibanday <= 15)
                                {
                                    Super_class += 200;
                                }
                                else if (daibanday >= 15 && daibanday <= days)
                                {
                                    Super_class += 300;
                                }
                            }


                        }
                    }
                }

                staff.ClassTime = FirstStage + SecondStage + ThreeStage + OtherStage;
                staff.totalmoney = Convert.ToInt32(Cost_fee) + Duty_fee + Invigilation_fee + Marking_fee + Super_class + Internal_training_fee + RD_fee;
                staff.Cost_fee = Cost_fee;
                staff.Duty_fee = Duty_fee;
                staff.Invigilation_fee = Invigilation_fee;
                staff.Marking_fee = Marking_fee;
                staff.RD_fee = RD_fee;
                staff.Super_class = Super_class;
                staff.Emp_Name = Emp_List[i].EmpName;
                staff.RoleName = EmployeesInfoManage_Entity.GetPositionByEmpid(Emp_List[i].EmployeeId).PositionName;
                staff_list.Add(staff);

                Cost_fee = 0;
                QuanDay = 0;
                ClassTime = 50;
                Duty_fee = 0;//值班费
                Invigilation_fee = 0;//监考费
                Marking_fee = 0;//阅卷费
                Super_class = 0;//超带班
                Internal_training_fee = 0;//内训费
                RD_fee = 0;//研发费

            }
            return staff_list;
        }

        /// <summary>
        /// 筛选员工
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> ScreenEmp(string EmpName = null, string DepId = null)
        {
            List<EmployeesInfo> result = new List<EmployeesInfo>();

            if (!string.IsNullOrEmpty(EmpName))
            {
                //全局所搜名字

                var templist = this.Emplist();

                result = templist.Where(d => d.EmpName.Contains(EmpName)).ToList();
            }

            if (!string.IsNullOrEmpty(DepId))
            {
                var templist = this.Emplist();

                if (DepId == "0")
                {
                    result = db_emp.GetEmpByDeptName();
                }
                else
                {
                    result = db_emp.GetEmpsByDeptid(int.Parse(DepId));
                }

            }


            return result;

        }


        /// <summary>
        /// 获取员工部门
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public Department GetDeparmentByEmp(string empid)
        {
            //获取岗位对象
            var po = db_emp.GetPositionByEmpid(empid);

            //获取部门对象
            return db_emp.GetDeptByPid(po.Pid);

        }

        /// <summary>
        /// 获取员工岗位对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public Position GetPositionByEmp(string empid)
        {
            var po = db_emp.GetPositionByEmpid(empid);

            return po = db_emp.GetPosition(po.Pid);
        }


        /// <summary>
        /// 获取部门集合
        /// </summary>
        /// <returns></returns>
        public List<Department> GetDepartments()
        {
            DepartmentBusiness.DepartmentManage tempdb_dep = new DepartmentBusiness.DepartmentManage();

            return tempdb_dep.GetDepartments().Where(
                s => s.DeptName.Contains("教学") 
                || s.DeptName.Contains("教质") 
                || s.DeptName.Contains("教导大队")
                || s.DeptName.Contains("信息")
                ).ToList();
        }


        /// <summary>
        /// 获取统计需要的数据  
        /// </summary>
        /// <returns></returns>

        public Staff_Cost_StatisticesDetailView Staff_CostData(string empid, DateTime date, int workingDays)
        {
            //获取这位员工

            var empObj = db_emp.GetInfoByEmpID(empid);
            //获取这位员工的职位

            var dep = this.GetDeparmentByEmp(empObj.EmployeeId);

            Staff_Cost_StatisticesDetailView resultObj = new Staff_Cost_StatisticesDetailView(); //返回值

            resultObj.emp = empObj;
         
            resultObj.teachingitems = teachingitems(empObj, date);
            
            //职业素养课，语数外 体育 课时
            resultObj.otherTeaccher_count = teachingitems(empObj, date, "other");
                
            //内训课时
            resultObj.InternalTraining_Count = InternalTraining_Count(empObj);

            //获取底课时
            resultObj.BottomClassHour = CalculationsBottomClassHour(empObj, date, workingDays, resultObj.teachingitems);

            //获取满意度调查分数
            resultObj.SatisfactionScore = CalculationSeurev(emp: empObj, tempdate: date);

            //获取阅卷份数
            resultObj.Marking_item = MarkingNumber();

            //获取监考次数
            resultObj.Invigilate_Count = InvigilateNumber();

            //获取值班次数
            resultObj.Duty_Count = Duty_Count(empObj.EmployeeId, date);

            ///研发教材章数
            resultObj.TeachingMaterial_Node = TeachingMaterial_Node(empObj.EmployeeId, date);

            resultObj.PPT_Node = PPT_Node(empObj.EmployeeId, date);

            #region 获取内训课时

            List<grand_number> InternalTraining_Count(EmployeesInfo emp)
            {

                List<grand_number> retulist = new List<grand_number>();

                if (dep.DeptName.Contains("教质"))
                {
                    BaseBusiness<Professionala> ProfessionalaBusiness = new BaseBusiness<Professionala>();

                    //按照时间筛选出培训记录
                    var templist = ProfessionalaBusiness.GetList().Where(d => ((DateTime)d.AddTime).Year == date.Year && ((DateTime)d.AddTime).Month == date.Month).ToList();

                    //获取员工班主任ID

                    HeadmasterBusiness temphead_db = new HeadmasterBusiness();
                    var headmaster = temphead_db.GetList().Where(d => d.informatiees_Id == emp.EmployeeId).FirstOrDefault();

                    if (headmaster != null)
                    {
                        //根据员工筛选数据

                        var resultlist = templist.Where(d => d.Trainee == headmaster.ID).ToList();


                        if (resultlist != null)
                        {

                            //在进行阶段分类

                            resultlist.ForEach(d => {

                                var item = retulist.Where(x => x.grand == d.Grand).FirstOrDefault();
                                if (item == null)
                                {
                                    grand_number grand_Number = new grand_number();
                                    grand_Number.grand = d.Grand;
                                    grand_Number.number = 1;

                                    retulist.Add(grand_Number);
                                }
                                else
                                {
                                    retulist.ForEach(c => {

                                        if (c.grand == d.Grand)
                                        {
                                            c.number += 1;
                                        }
                                    });
                                }

                            });
                        }

                    }


                }

                if (dep.DeptName.Contains("教学"))
                {
                    

                    BaseBusiness<Teachingtraining> temptedb_achtran = new BaseBusiness<Teachingtraining>();

                    //按照时间筛选出培训记录
                    var templist = temptedb_achtran.GetList().Where(d => ((DateTime)d.AddTime).Year == date.Year && ((DateTime)d.AddTime).Month == date.Month).ToList();


                    //获取员工 教员ID
                    TeacherBusiness tempdb_teacher = new TeacherBusiness();

                    var teacher = tempdb_teacher.GetTeachers(isContains_Jiaowu: false, IsNeedDimission: true).Where(d => d.EmployeeId == emp.EmployeeId).FirstOrDefault();

                 
                    var resultlist = templist.Where(d => d.Trainee == teacher.TeacherID).ToList();
                    //在进行阶段分类

                    resultlist.ForEach(d=> {

                       var item = retulist.Where(x => x.grand == d.Grand).FirstOrDefault();
                        if (item == null)
                        {
                            grand_number grand_Number = new grand_number();
                            grand_Number.grand = d.Grand;
                            grand_Number.number = 1;

                            retulist.Add(grand_Number);
                        }
                        else
                        {
                            retulist.ForEach(c => {

                                if (c.grand == d.Grand)
                                {
                                    c.number += 1;
                                }
                            });
                        }

                    });
                  

                }

                return retulist;


            }

            #endregion             

            #region 获取职业素养课，语数外 体育 课时
            int otherTeaccher_count()
            {
                return ScreenReconcile(empObj.EmployeeId, date, type: "other").Count;
            }
            #endregion

            
            #region 阅卷份数
            int MarkingNumber()
            {
                int result = 0;

                result = this.ScreenTestScore(empObj, date).Count;

                return result;
            }
            #endregion

            int InvigilateNumber()
            {
                int result = 0;

                ExaminationBusiness tempdb_exam = new ExaminationBusiness();

                var examlist = tempdb_exam.AllExamination().Where(d => d.BeginDate.Year == date.Year && d.BeginDate.Month == date.Month).ToList();

                //获取到具体考场

                List<ExaminationRoom> temproomlist = new List<ExaminationRoom>();

                foreach (var item in examlist)
                {
                    var templist = tempdb_exam.AllExaminationRoom().Where(d => d.Examination == item.ID).ToList();

                    if (templist != null)

                        temproomlist.AddRange(templist);

                }

                result = GetCount();

                return result;

                #region 得到份数数
                int GetCount()
                {
                    int tempresult1 = 0;

                    foreach (var item in temproomlist)
                    {
                        if (item.Invigilator1 == empObj.EmployeeId || item.Invigilator2 == empObj.EmployeeId)
                            tempresult1++;
                    }

                    return tempresult1;
                }

                #endregion

            }

            return resultObj;
        }


        /// <summary>
        /// 排课筛选
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="date"></param>
        /// <param name="type">type:0 = 所有；type: skill = 专业；type: other = 其他</param>
        /// <returns></returns>
        public List<ReconcileView> ScreenReconcile(string empid, DateTime date, string type = "0")
        {
            ReconcileManeger tempdb_reconcile = new ReconcileManeger();
            CourseBusiness db_course = new CourseBusiness();

            //获取当期日期
            var currentData = DateTime.Now;

            var list = tempdb_reconcile.SQLGetReconcileDate().Where(d => d.AnPaiDate.Month == date.Month && d.AnPaiDate.Year == date.Year && d.EmployeesInfo_Id == empid).ToList();

            //定义返回值
            List<ReconcileView> result = new List<ReconcileView>();

            switch (type)
            {
                case "0":
                    result = list;
                    break;

                case "skill":
                    //获取专业课

                    foreach (var item in list)
                    {

                        var coustype = db_course.CurseType(item.Curriculum_Id);

                        if (coustype.Id == 1)
                        {
                            result.Add(item);
                        }
                    }


                    break;
                case "other":

                    foreach (var item in list)
                    {

                        var coustype = db_course.CurseType(item.Curriculum_Id);

                        if (coustype.Id != 1)
                        {
                            result.Add(item);
                        }
                    }

                    break;
            }

            return result;

        }

        public bool IsContains(List<ReconcileView> sourcs, ReconcileView r)
        {
            foreach (var item in sourcs)
            {
                if (item.ClassSchedule_Id == r.ClassSchedule_Id)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 判断一个日期是否为节假日
        /// </summary>
        /// <returns></returns>
        public  bool iSHoliday(string date)
        {


            bool isHoliday = false;
            System.Net.WebClient WebClientObj = new System.Net.WebClient();
            System.Collections.Specialized.NameValueCollection PostVars = new System.Collections.Specialized.NameValueCollection();
            PostVars.Add("d", DateTime.Parse(date).ToShortDateString());//参数
            try
            {
                byte[] byRemoteInfo =  WebClientObj.UploadValues("http://easybots.cn/api/holiday.php", "POST", PostVars);//请求地址,传参方式,参数集合
                string sRemoteInfo = System.Text.Encoding.UTF8.GetString(byRemoteInfo);//获取返回值
                string result = JObject.Parse(sRemoteInfo)[date].ToString();
                if (result == "0")
                {
                    isHoliday = false;
                }
                else if (result == "1" || result == "2")
                {
                    isHoliday = true;
                }
            }
            catch
            {
                isHoliday = false;
            }
            //string backMsg = "";
            //WebRequest request = HttpWebRequest.Create("http://www.easybots.cn/api/holiday.php");

            //request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            //byte[] dataArray = System.Text.Encoding.UTF8.GetBytes("d="+date);

            //System.Net.WebResponse response = request.GetResponse();
            //System.IO.Stream responseStream = response.GetResponseStream();
            //System.IO.StreamReader reader = new System.IO.StreamReader(responseStream, System.Text.Encoding.UTF8);
            //backMsg = reader.ReadToEnd();

            //reader.Close();
            //reader.Dispose();

            //responseStream.Close();
            //responseStream.Dispose();
            return isHoliday;
        }
        
        /// <summary>
        /// 获取工作日的天数
        /// </summary>
        /// <returns></returns>q
        public int WorkingDate(DateTime date)
        {
            int result = 0;

            ReconcileManeger tempdbR = new ReconcileManeger();

            GetYear find_g = tempdbR.MyGetYear(date.Year.ToString(), System.Web.HttpContext.Current.Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml"));

            //判断是否为休息日

            if (date.Month >= find_g.StartmonthName && date.Month <= find_g.EndmonthName)
            {
                //单休
                result = SingleCeaseWorkingDay(date, type: "单休");
            }
            else
            {
                //双休
                result = SingleCeaseWorkingDay(date, type: "双休");
            }

            return result;
        }

        /// <summary>
        ///提供老师月阅卷数量
        /// </summary>
        /// <returns></returns>
        public List<TestScore> ScreenTestScore(EmployeesInfo emp, DateTime date)
        {

            List<TestScore> resultlist = new List<TestScore>();

            ExamScoresBusiness db_exscore = new ExamScoresBusiness();

            Teacher t = teacher();

            if (t == null)
            {
                return resultlist;
            }

            resultlist = db_exscore.AllExamScores().Where(d=>d.CreateTime!=null).ToList().

                Where(d => ((DateTime)d.CreateTime).Year == date.Year && ((DateTime)d.CreateTime).Month == date.Month).

                ToList().Where(d => d.Reviewer == t.TeacherID).ToList();

            return resultlist;


            Teacher teacher()
            {
                Teacher result = new Teacher();

                TeacherBusiness tempdb = new TeacherBusiness();

                var list = tempdb.GetTeachers(IsNeedDimission: true);

                result = list.Where(d => d.EmployeeId == emp.EmployeeId).FirstOrDefault();

                return result;
            }

        }

        /// <summary>
        /// 计算工作日天数
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public int SingleCeaseWorkingDay(DateTime date, string type = "单休")
        {
            int result = System.Threading.Thread.CurrentThread.CurrentUICulture.Calendar.GetDaysInMonth(date.Year, date.Month); //获取这个月的总天数

            DateTime dtt = new DateTime(date.Year, date.Month, 1); //初始化时间

            //得出结果
            return result - CalculationDayNumber();

            #region 计算天数和
            int CalculationDayNumber()
            {
                int num = 0;

                while (dtt.Month == date.Month)
                {
                    //是否法定节假日
                    var isHoliday = this.iSHoliday(dtt.ToString());

                    if (!isHoliday)
                    {
                        num++;

                        dtt = dtt.AddDays(1);
                        continue;
                        
                        
                    }

                    if (type == "单休")
                    {
                        if (dtt.DayOfWeek == DayOfWeek.Sunday)
                        {
                            dtt = dtt.AddDays(1);
                            num++;
                            continue;
                            
                        }

                        
                    }

                    if (type == "双休")
                    {
                        if (dtt.DayOfWeek == DayOfWeek.Sunday || dtt.DayOfWeek == DayOfWeek.Saturday)
                        {
                            dtt = dtt.AddDays(1);
                            num++;
                            continue;
                        }
   
                    }

                    dtt = dtt.AddDays(1);
                }

                return num;
            }
            #endregion



        }
        
        /// <summary>
        /// 获取上专业课的课时
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<TeachingItem> teachingitems(EmployeesInfo emp, DateTime date, string type="skill")
        {
            //从排课数据里面获取到这个老师上的专业课
            var templist = ScreenReconcile(emp.EmployeeId, date, type: type);

            return GetteachingNum();
            
            List<TeachingItem> GetteachingNum()
            {
                CourseBusiness tempdb_course = new CourseBusiness();

                List<TeachingItem> result = new List<TeachingItem>();

                foreach (var item in templist)
                {
                    var course = tempdb_course.GetCurriculas().Where(d => d.CourseName == item.Curriculum_Id).FirstOrDefault();

                    if (IsContains(result, course.CurriculumID))
                    {
                        foreach (var item1 in result)
                        {
                            if (item1.Course == course.CurriculumID)
                            {
                                if (type == "skill")
                                {
                                    

                                    if (item.Curse_Id.Contains("12") || item.Curse_Id.Contains("34")) item1.NodeNumber += 2;

                                    else item1.NodeNumber += 4;


                                }
                                else
                                {
                                    item1.NodeNumber += 2;


                                } 
                            }
                        }
                    }

                    else
                    {
                        TeachingItem teachingItem = new TeachingItem();
                        teachingItem.Course = course.CurriculumID;
                        

                        if (type == "skill")
                        {

                            if (item.Curse_Id.Contains("12") || item.Curse_Id.Contains("34")) teachingItem.NodeNumber += 2;

                            else teachingItem.NodeNumber += 4;

                        }
                        else
                        {
                            teachingItem.NodeNumber += 2;
                        }
                        //创建新的
                       

                        result.Add(teachingItem);
                    }

                }

                return result;
            }
            
        }

        public bool IsContains(List<TeachingItem> teachingItems, int course)
        {

            foreach (var item in teachingItems)
            {
                if (item.Course == course)
                    return true;
            }

            return false;
        }


        #region 将排课数据按照阶段分组 获取课时
        int ReconcileGroupByGrand(List<Reconcile> data, Grand grand)
        {

            int result = 0;

            foreach (var item in data)
            {
                CourseBusiness tempdb_course = new CourseBusiness();

                //获取课程的阶段

                var course = tempdb_course.GetCurriculas().Where(d => d.CourseName == item.Curriculum_Id).FirstOrDefault();

                if (course.Grand_Id == grand.Id)
                {
                    result += 4;
                }

            }

            return result;
        }

        #endregion

        /// <summary>
        /// 计算满意度分数
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="tempdate"></param>
        /// <returns></returns>
        public float CalculationSeurev(EmployeesInfo emp, DateTime tempdate)
        {

            float result = 0;

            SatisfactionSurveyBusiness tempdb_datis = new SatisfactionSurveyBusiness();//满意度业务类实例

            List<SatisfactionSurveyDetailView> datalist = tempdb_datis.SurveyResult_Cost(emp.EmployeeId, tempdate);

            //开始计算 

            float totalScore = 0; //获取达到总分
            foreach (var item in datalist)
            {
                totalScore += item.TotalScore;
            }

            result = totalScore / datalist.Count;


            return result;

        }
        
        /// <summary>
        /// 计算底课时
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="teachingItems"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public float CalculationsBottomClassHour(EmployeesInfo emp, DateTime date, int workingDays, List<TeachingItem> teachingItems = null)
        {

            #region 计算规则
            /* S1S2教学经理，S3教学经理: 如果带1个班底课时为0，如果带2个班则按照标准底课时计算(需要把
            底课时分摊到每一个工作日，然后在看上两个班级的工作日是多少，得出底课时，如果有小数 向上取整)
            */
            #endregion

            if (teachingItems == null)
            {
                teachingItems = teachingitems(emp, date);
            }

            float result = 0;
            //获取员工岗位
            var poo = db_emp.GetPositionByEmpid(emp.EmployeeId);

            //判断这个月所带班情况

            TeacherClassBusiness tempdb_teacherclass = new TeacherClassBusiness();

            //获取部门
            var dep = this.GetDeparmentByEmp(emp.EmployeeId);

            //获取员工 教员ID
            TeacherBusiness tempdb_teacher = new TeacherBusiness();

            var teacher = tempdb_teacher.GetTeachers(IsNeedDimission: true, isContains_Jiaowu: false).Where(d => d.EmployeeId == emp.EmployeeId).FirstOrDefault();

            ///获取到这个月教员所带班级个数
            ///
            List<ReconcileView> tempcount = new List<ReconcileView>(); //记录带班个数

            var reconlielist = this.ScreenReconcile(emp.EmployeeId, date, type: "skill");

            //去掉重复项
            foreach (var item in reconlielist)
            {
                if (!IsContains(tempcount, item))
                {
                    tempcount.Add(item);
                }
            }

            //*************************S1S2教学经理，S3教学经理 的底课时计算方式*********************************
            if ((dep.DeptName == "s1、s2教学部" && poo.PositionName == "教学主任") || (dep.DeptName == "s3教学部" && poo.PositionName == "教学主任"))
            {
                result = bottmTeacherHours_jingli();
            }


            if(dep.DeptName.Contains("教学部")  && !db_emp.GetPobjById( emp.PositionId).PositionName.Contains("教务")) //****************************教员的底课时计算方式******************************
            {
                result = bottomTeacherHours_teacher();

            }

            return result;



            #region 教学经理底课时计算方式
            float bottmTeacherHours_jingli()
            {
                float tempresult = 0;

                if (tempcount.Count > 1)
                {
                    //如果带2个班则按照标准底课时计算(需要把底课时分摊到每一个工作日，然后在看上两个班级的工作日是多少，得出底课时，如果有小数 向上取整)

                    //获取到工作日

                    

                    int workingdateCount = string.IsNullOrEmpty(workingDays.ToString()) ? this.WorkingDate(date):workingDays;

                    //获取标准底课时
                    var MinimumCourseHours = (float)teacher.MinimumCourseHours;

                    //计算结果
                    tempresult = MinimumCourseHours / workingdateCount;


                }
                else
                {
                    //底课时为0
                    tempresult = 0;
                }


                return tempresult;
            }

            #endregion

            #region 教员底课时计算方式

            float bottomTeacherHours_teacher()
            {
                return (float)teacher.MinimumCourseHours;
            }


            #endregion

        }
        
        //保存详细数据
        public void SaveStaff_CostData(List<Staff_Cost_StatisticesDetailView> data, List<Cose_StatisticsItems> Cost, string filename)
        {

            if (data == null)
            {
                return;

            }
            var workbook = new HSSFWorkbook();
            
            //创建工作区
            var sheet = workbook.CreateSheet("课时费费用详细");

            #region 表头样式

            HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

            HeadercellStyle.Alignment = HorizontalAlignment.Center;
            HeadercellFont.IsBold = true;

            HeadercellStyle.SetFont(HeadercellFont);

            #endregion


          
            HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

            ContentcellStyle.Alignment = HorizontalAlignment.Center;

            CreateHeader();

            int num = 1;

            CourseBusiness dbcourse = new CourseBusiness();

            GrandBusiness dbgrand = new GrandBusiness();

            data.ForEach(d =>
            {

                var row = (HSSFRow)sheet.CreateRow(num);

                CreateCell(row, ContentcellStyle, 0, d.emp.EmpName);
                CreateCell(row, ContentcellStyle, 1, db_emp.GetPobjById(d.emp.PositionId).PositionName);

                List<TeachingItem> teachingItems = d.teachingitems;

                float S1 = 0;
                float S2 = 0;
                float S3 = 0;
                float S4 = 0;
                float Y1 = 0;

                teachingItems.ForEach(x =>
                {

                    var course = dbcourse.GetCurriculas().Where(c => c.CurriculumID == x.Course).FirstOrDefault();

                    var grand = dbgrand.AllGrand().Where(g => g.Id == course.Grand_Id).FirstOrDefault();

                    if (grand.GrandName.Contains("S1"))
                    {
                        S1 += x.NodeNumber;
                    }

                    if (grand.GrandName.Contains("S2"))
                    {
                        S2 += x.NodeNumber;
                    }

                    if (grand.GrandName.Contains("S3"))
                    {
                        S3 += x.NodeNumber;
                    }

                    if (grand.GrandName.Contains("S4"))
                    {
                        S4 += x.NodeNumber;
                    }

                    if (grand.GrandName.Contains("Y1"))
                    {
                        Y1 += x.NodeNumber;
                    }

                });

                List<grand_number> leixunlist = d.InternalTraining_Count;

                int lS1 = 0;
                int lS2 = 0;
                int Ls3 = 0;
                int lS4 = 0;
                int lY1 = 0;

                leixunlist.ForEach(x =>
                {
                    var grand = dbgrand.AllGrand().Where(c=>c.Id == x.grand).FirstOrDefault();


                    if (grand.GrandName.Contains("S1"))
                    {
                        lS1 += x.number;
                    }

                    if (grand.GrandName.Contains("S2"))
                    {
                        lS2 += x.number;
                    }

                    if (grand.GrandName.Contains("S3"))
                    {
                        Ls3 += x.number;
                    }

                    if (grand.GrandName.Contains("S4"))
                    {
                        lS4 += x.number;
                    }

                    if (grand.GrandName.Contains("Y1"))
                    {
                        lY1 += x.number;
                    }

                });
                //内训次数



                CreateCell(row, ContentcellStyle, 2, Y1.ToString());
                CreateCell(row, ContentcellStyle, 3, S1.ToString());
                CreateCell(row, ContentcellStyle, 4, S2.ToString());
                CreateCell(row, ContentcellStyle, 5, S3.ToString());
                CreateCell(row, ContentcellStyle, 6, S4.ToString());


                CreateCell(row, ContentcellStyle, 7, lS1.ToString());
                CreateCell(row, ContentcellStyle, 8, lS2.ToString());
                CreateCell(row, ContentcellStyle, 9, Ls3.ToString());
                CreateCell(row, ContentcellStyle, 10, lS4.ToString());
                CreateCell(row, ContentcellStyle, 11, lY1.ToString());

                float otherTeaccher_count = 0;

                d.otherTeaccher_count.ForEach(o=>
                {
                    otherTeaccher_count += o.NodeNumber;
                });
                
                CreateCell(row, ContentcellStyle, 12, otherTeaccher_count.ToString());
                CreateCell(row, ContentcellStyle, 13, d.BottomClassHour.ToString());
                CreateCell(row, ContentcellStyle, 14, d.SatisfactionScore.ToString());

                var cost = Cost.Where(c => c.Emp.EmployeeId == d.emp.EmployeeId).FirstOrDefault();

                CreateCell(row, ContentcellStyle, 15, cost.EachingHourCost.ToString());

                CreateCell(row, ContentcellStyle, 16, cost.MarkingCost.ToString());
                CreateCell(row, ContentcellStyle, 17, cost.InvigilateCost.ToString());
                CreateCell(row, ContentcellStyle, 18, cost.CurriculumDevelopmentCost.ToString());

                num++;

            });

            string pathName = System.Web.HttpContext.Current.Server.MapPath("/Areas/Educational/CostHistoryFiles/" + filename + ".xls");

            var stream = new FileStream(pathName, FileMode.Create, FileAccess.ReadWrite);

            workbook.Write(stream);

            stream.Close();
            FileInfo fileinfo = new FileInfo(pathName);
            
            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            client.PutObject("xinxihua", $"/CostHistoryFiles/{filename}.xls", fileinfo);

            workbook.Close();


            void CreateHeader()
            {
                HSSFRow Header = (HSSFRow)sheet.CreateRow(0);
                Header.HeightInPoints = 40;

                CreateCell(Header, HeadercellStyle, 0, "姓名");

                CreateCell(Header, HeadercellStyle, 1, "职务");

                CreateCell(Header, HeadercellStyle, 2, "Y1课时");

                CreateCell(Header, HeadercellStyle, 3, "S1课时");

                CreateCell(Header, HeadercellStyle, 4, "S2课时");

                CreateCell(Header, HeadercellStyle, 5, "S3课时");

                CreateCell(Header, HeadercellStyle, 6, "S4课时");

                CreateCell(Header, HeadercellStyle, 7, "S1内训课时");

                CreateCell(Header, HeadercellStyle, 8, "S2内训课时");

                CreateCell(Header, HeadercellStyle, 9, "S3内训课时");

                CreateCell(Header, HeadercellStyle, 10, "S4内训课时");

                CreateCell(Header, HeadercellStyle, 11, "Y1内训课时");

                CreateCell(Header, HeadercellStyle, 12, "质素，语数外，体育课时");

                CreateCell(Header, HeadercellStyle, 13, "底课时");

                CreateCell(Header, HeadercellStyle, 14, "满意度分数");

                CreateCell(Header, HeadercellStyle, 15, "课时费");

                CreateCell(Header, HeadercellStyle, 16, "阅卷费");

                CreateCell(Header, HeadercellStyle, 17, "监考费");

                CreateCell(Header, HeadercellStyle, 18, "课程研发费");
            }

            void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
            {
                HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                Header_Name.SetCellValue(value);

                Header_Name.CellStyle = TcellStyle;
            }

        }
        
        /// <summary>
        /// 费用统计
        /// </summary>
        /// <param name="empid">员工</param>
        /// <param name="date">日期</param>
        public Cose_StatisticsItems Statistics_Cost(Staff_Cost_StatisticesDetailView data)
        {
           
            Cose_StatisticsItems result = new Cose_StatisticsItems();

            result.Emp = data.emp;

            //var data = this.Staff_CostData(empid, DateTime.Parse(date), workingDays);

            ///课时费
            result.EachingHourCost = (decimal)this.EachingHourCost(data.teachingitems, data.BottomClassHour, data.SatisfactionScore);

            result.OtherTeachingCost = OtherTeachingCost(data.otherTeaccher_count , data.BottomClassHour);

            ///值班费
            result.DutyCost = Duty_Cost(data.Duty_Count);

            /// 监考费
            result.InvigilateCost = this.InvigilateCost(data.Invigilate_Count);

            ///阅卷费
            result.MarkingCost = MarkingCost(data.Marking_item);

            ///教材研发费用
            result.CurriculumDevelopmentCost = CurriculumDevelopmentCost(data.TeachingMaterial_Node) + PPTDevelopmentCost(data.PPT_Node);

            ///内训费
            ///
            result.InternalTrainingCost = this.InternalTrainingCost(data.InternalTraining_Count, result.Emp);

              
            return result;
        }

        /// <summary>
        /// 课时费
        /// </summary>
        /// <param name="teachingItems">总课时</param>
        /// <param name="bottomTeacherHours">底课时</param>
        /// <param name="SurveyCost">满意度分数</param>
        /// <returns></returns> 

        public float EachingHourCost(List<TeachingItem> teachingItems, float bottomTeacherHours, float SurveyScores)
        {
            float result = 0;

            //得到课时
            List<TeachingItem> teacherHours = TeacherHours(teachingItems, bottomTeacherHours);

            foreach (var item in teacherHours)
            {
                CourseBusiness tempdb_course = new CourseBusiness();

                Curriculum course = tempdb_course.GetCurriculas().Where(d => d.CurriculumID == item.Course).FirstOrDefault();
                // 获取到对应的课时费

                //ji算
                result += item.NodeNumber * ((float)course.PeriodMoney - 5);

            }

            // 加上5元的满意度系数

            float SurveyCost = GetSurveyCost();

            return result + SurveyCost;



            // 获取满意度费用
            float GetSurveyCost()
            {
                float tempresult = 0;

                SatisfactionBusiness tempdb_satis = new SatisfactionBusiness();

                var satislist = tempdb_satis.satisfacctions();

                foreach (var item in satislist)
                {
                    if ((float)item.MaxValue >= SurveyScores && SurveyScores >= (float)item.MinValue)
                    {
                        tempresult = (float)item.AddMoney;

                        break;
                    }
                }

                return tempresult;
            }



            /*
             
            //总课时-底课时*(对应阶段的课时费-5元的满意度系数);


             教员的课时(已减去底课时)

            计算：

            //测试数据
            S1------10节课
            S3------2节课

            10*S1阶段课时费+2*S3阶段课时费

            /////////   【 对应阶段的课时费：】
            ///
             ((10*S1阶段课时费-5）+ (2*S3阶段课时费-5) ) + 5元满意度系数
             
             
             */



        }


        /// <summary>
        /// 计算课时   总课时-底课时
        /// </summary>
        /// <param name="teachingItems"></param>
        /// <param name="bottomTeacherHours"></param>
        /// <returns></returns>
        public List<TeachingItem> TeacherHours(List<TeachingItem> teachingItems, float bottomTeacherHours)
        {
            CourseBusiness tempdb_course = new CourseBusiness();
            GrandBusiness tempdb_grand = new GrandBusiness();

            foreach (var item in teachingItems)
            {

                var course = tempdb_course.GetCurriculas().Where(d=>d.CurriculumID == item.Course).FirstOrDefault();

                var grand = tempdb_grand.AllGrand().Where(d=>d.Id == course.Grand_Id).FirstOrDefault();

                if (bottomTeacherHours == 0)
                    break;

                if (grand.GrandName == "S1" && item.NodeNumber > 0)
                {
                    item.NodeNumber = jisuan(item.NodeNumber);
                }

                if (grand.GrandName == "S2" && item.NodeNumber > 0)
                {
                    item.NodeNumber = jisuan(item.NodeNumber);

                }

                if (grand.GrandName == "S3" && item.NodeNumber > 0)
                {

                    item.NodeNumber = jisuan(item.NodeNumber);

                }

                if (grand.GrandName == "S4" && item.NodeNumber > 0)
                {

                    item.NodeNumber = jisuan(item.NodeNumber);

                }

            }

            float jisuan(float NodeNumber)
            {
                float tempresult = 0;

                if (NodeNumber - bottomTeacherHours >= 0)
                {
                    tempresult = NodeNumber - bottomTeacherHours;

                    //记录底课时

                    bottomTeacherHours = 0;
                }
                else
                {
                    tempresult = 0;
                    //记录底课时

                    bottomTeacherHours -= NodeNumber;
                }

                return tempresult;

            }

            return teachingItems;



        }


        /// <summary>
        /// 获取值班次数
        /// </summary>
        /// <returns></returns>
        public List<DutyCount> Duty_Count(string empid, DateTime date)
        {
            List<DutyCount> result = new List<DutyCount>();

            TeacherNightManeger tempddb_teachernight = new TeacherNightManeger();

            var DutyRecordList = tempddb_teachernight.GetAllTeacherNight().Where(d => d.OrwatchDate.Year == date.Year && d.OrwatchDate.Month == date.Month).ToList().Where(d => d.Tearcher_Id == empid).ToList();

            BeOnDutyManeger tempdb_duty = new BeOnDutyManeger();

            var dytytypelist = tempdb_duty.GetList().Where(d => d.IsDelete == true).ToList();

            foreach (var item in dytytypelist)
            {
                var countlist = DutyRecordList.Where(d => d.BeOnDuty_Id == item.Id).ToList();

                DutyCount dutyCount = new DutyCount();

                dutyCount.Count = countlist.Count;

                dutyCount.DutyType = item;


                result.Add(dutyCount);
            }

            return result;

        }


        /// <summary>
        /// 值班费计算
        /// </summary>
        /// <param name="dutyCounts"></param>
        /// <returns></returns>
        public decimal Duty_Cost(List<DutyCount> dutyCounts)
        {
            decimal result = 0;

            foreach (var item in dutyCounts)
            {
                result += (decimal)item.Count * item.DutyType.Cost;
            }

            return result;
        }


        /// <summary>
        /// 监考费
        /// </summary>
        /// <param name="Invigilate_Count"></param>
        /// <returns></returns>
        public decimal InvigilateCost(int Invigilate_Count)
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/Cost.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            //获取标准费用
            var cost = xmlRoot.GetElementsByTagName("InvigilateCost")[0].InnerText;

            return Invigilate_Count * int.Parse(cost);

        }

        /// <summary>
        /// 阅卷费
        /// </summary>
        /// <param name="Marking_Count"></param>
        /// <returns></returns>
        public decimal MarkingCost(int Marking_Count)
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/Cost.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            //获取标准费用
            var cost = xmlRoot.GetElementsByTagName("MarkingCost")[0].InnerText;

            return Marking_Count * int.Parse(cost);
        }

        /// <summary>
        /// 内训费用
        /// </summary>
        /// <returns></returns>
        public decimal InternalTrainingCost(List<grand_number> list, EmployeesInfo emp)
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/Cost.xml"));
            //根节点
            var xmlRoot = xmlDocument.DocumentElement;
            var dep = this.GetDeparmentByEmp(emp.EmployeeId);

            decimal result = 0;

            if (emp.EmpName.Contains("教质"))
            {
                int num = 0;

                list.ForEach(d=>
                {
                    num += d.number;
                });

                //获取标准费用
                var cost = xmlRoot.GetElementsByTagName("OtherTeachingCose")[0].InnerText;

                result = num * int.Parse(cost);
            }

            if (emp.EmpName.Contains("教学"))
            {
                decimal currentresult = 0;

                //获取标准费用
                var Professionala = (XmlElement)xmlRoot.GetElementsByTagName("Professionala")[0];

                list.ForEach(d =>
                {
          
                    var nodelist = Professionala.ChildNodes;

                    foreach (XmlElement item in nodelist)
                    {
                        var grand = item.GetAttribute("grand");

                        if (d.grand == int.Parse(grand))
                        {
                            var cost = item.InnerText;

                            currentresult += d.number * int.Parse(cost);

                            break;
                        }
                    }

                });

                result = currentresult;
            }

            return result;
        }


        /// <summary>
        /// 研发教材章数
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int TeachingMaterial_Node(string empid, DateTime date)
        {
            
            BaseBusiness<Coursewaremaking> tempdbb = new BaseBusiness<Coursewaremaking>();

            var templist = tempdbb.GetList().Where(d=>d.Submissiontime.Year == date.Year && d.Submissiontime.Month == date.Month).ToList().Where(d=>d.RampDpersonID == empid && d.MakingType=="Word").ToList();

            return templist == null ? 0 : templist.Count;
            
        }

        /// <summary>
        /// ppt数量
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public int PPT_Node(string empid, DateTime date)
        {

            BaseBusiness<Coursewaremaking> tempdbb = new BaseBusiness<Coursewaremaking>();

            var templist = tempdbb.GetList().Where(d => d.Submissiontime.Year == date.Year && d.Submissiontime.Month == date.Month).ToList().Where(d => d.RampDpersonID == empid && d.MakingType == "PPT").ToList();

            return templist == null ? 0 : templist.Count;

        }

        /// <summary>
        /// 教材研发费用
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public decimal CurriculumDevelopmentCost(int count)
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/Cost.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            //获取标准费用
            var cost = xmlRoot.GetElementsByTagName("TextBook_NodeCost")[0].InnerText;

            return count * int.Parse(cost);

        }
        /// <summary>
        /// PPT研发费用
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public decimal PPTDevelopmentCost(int count)
        {
            //读取配置文件
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Config/Cost.xml"));

            //根节点
            var xmlRoot = xmlDocument.DocumentElement;

            //获取标准费用
            var cost = xmlRoot.GetElementsByTagName("PPT_NodeCost")[0].InnerText;

            return count * int.Parse(cost);
        }

        public decimal OtherTeachingCost(List<TeachingItem> items, float buttonHours = 0)
        {
            decimal result = 0;

            CourseBusiness tempdb_course = new CourseBusiness();

            items.ForEach(d=> {

                //获取课程费用
             
                Curriculum course = tempdb_course.GetCurriculas().Where(x=>x.CurriculumID == d.Course).FirstOrDefault();

                decimal nodenumber = 0;

                var temp1 = d.NodeNumber - buttonHours;
                if (temp1 >= 0)
                {
                    nodenumber = (decimal)temp1;

                    buttonHours = 0;
                }
                else
                {
                    nodenumber = (decimal)buttonHours - (decimal)d.NodeNumber;

                    buttonHours = buttonHours - d.NodeNumber;
                }

                result += (decimal)course.PeriodMoney * (decimal)nodenumber;

            });

            return result;
        }


        /// <summary>
        /// 保存到文件
        /// </summary>
        /// <returns></returns>
        public string SaveToExcel(List<Cose_StatisticsItems> data, string fileName)
        {

            if (data == null)
            {
                return null;
            }

            
            var workbook = new HSSFWorkbook();

            //创建工作区
            var sheet = workbook.CreateSheet("费用统计");
   
            #region 表头样式

            HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

            HeadercellStyle.Alignment = HorizontalAlignment.Center;
            HeadercellFont.IsBold = true;
  
            HeadercellStyle.SetFont(HeadercellFont);

            #endregion


            #region 内容样式
            HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

            ContentcellStyle.Alignment = HorizontalAlignment.Center;          
            #endregion

            #region 创建表头

            HSSFRow Header = (HSSFRow)sheet.CreateRow(0);
            Header.HeightInPoints = 40;

            CreateCell(Header, HeadercellStyle, 0, "姓名");

            CreateCell(Header, HeadercellStyle, 1, "职务");

            CreateCell(Header, HeadercellStyle, 2, "课时费");

            CreateCell(Header, HeadercellStyle, 3, "值班费");

            CreateCell(Header, HeadercellStyle, 4, "监考费");

            CreateCell(Header, HeadercellStyle, 5, "阅卷费");

            CreateCell(Header, HeadercellStyle, 6, "内训费");

            CreateCell(Header, HeadercellStyle, 7, "教材研发费");

            CreateCell(Header, HeadercellStyle, 8, "合计");
            #endregion

            int count = 1;

            int cellCount = typeof(Cose_StatisticsItems).GetType().GetProperties().Count();

            foreach (var item in data)
            {
                 //填充数据

                 //创建行
                 HSSFRow contentRow = (HSSFRow)sheet.CreateRow(count);
                contentRow.HeightInPoints = 35;

                //填充单元格

                CreateCell(contentRow, ContentcellStyle, 0, item.Emp.EmpName);

                CreateCell(contentRow,ContentcellStyle, 1, db_emp.GetPobjById(item.Emp.PositionId).PositionName);

                CreateCell(contentRow, ContentcellStyle, 2, item.EachingHourCost.ToString());

                CreateCell(contentRow, ContentcellStyle, 3, item.DutyCost.ToString());

                CreateCell(contentRow, ContentcellStyle, 4, item.InvigilateCost.ToString());

                CreateCell(contentRow, ContentcellStyle, 5, item.MarkingCost.ToString());

                CreateCell(contentRow, ContentcellStyle, 6, item.InternalTrainingCost.ToString());

                CreateCell(contentRow, ContentcellStyle, 7, item.CurriculumDevelopmentCost.ToString());

                var total = item.EachingHourCost + item.DutyCost + item.InvigilateCost + item.MarkingCost + item.InternalTrainingCost + item.CurriculumDevelopmentCost;

                CreateCell(contentRow, ContentcellStyle, 8, total.ToString());

                count++;
            }

            string LOACpathName = System.Web.HttpContext.Current.Server.MapPath("/Areas/Educational/CostHistoryFiles/" + fileName+".xls");

            string pathName = $"/CostHistoryFiles/{fileName}.xls";

            FileStream stream = new FileStream(LOACpathName, FileMode.Create,FileAccess.ReadWrite);

            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();

            workbook.Write(stream);

            stream.Close();

            var FILEINFO = new FileInfo(LOACpathName);

            client.PutObject("xinxihua", pathName, FILEINFO);
           
            workbook.Close();

            return pathName;


            void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
            {
                HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                Header_Name.SetCellValue(value);

                Header_Name.CellStyle = TcellStyle;
            }
        }

        /// <summary>
        /// 统计记录——文件
        /// </summary>path
        /// <returns>键值对: key=文件名; value = 更新时间</returns>
        public List<FileInfo> HistoryCostFileData()
        {
            List<FileInfo> result = new List<FileInfo>();

            string path = System.Web.HttpContext.Current.Server.MapPath("/Areas/Educational/CostHistoryFiles/");

            DirectoryInfo directory = new DirectoryInfo(path);

            //获取文件夹下所有文件
            FileInfo [] files = directory.GetFiles();

            
            foreach (var file in files)
            {
                result.Add(file);
            }
            
            return result;


        }


        /// <summary>
        /// 对质素课数据可进行处理
        /// </summary>
        /// <returns></returns>
        public List<TeachingItem> Qualityclass(List<TeachingItem> datas)
        {
            CourseBusiness dbcourse = new CourseBusiness();
            datas.ForEach(d =>
                {
                    var courseobj = dbcourse.GetEntity(d.Course);

                    if (courseobj.CourseName.Contains("职素"))
                    {
                        //if(d.)
                    }
                });
            return null;
        }

        public List<HeadClass> SuperClass(List<HeadClass> datas)
        {
            return null;
        }

        /// <summary>
        /// 计算全天课天数
        /// </summary>
        /// <returns></returns>
        public int Cost_EndClass(DateTime time, string EmpID)
        {
            string sql = $"select * from Reconcile where YEAR(AnPaiDate)='{time.Year}' and MONTH(AnPaiDate)='{time.Month}' and Day(AnPaiDate)='{time.Day}' and EmployeesInfo_Id='{EmpID}'";
            List<Reconcile> mydata = Reconcile_Entity.GetListBySql<Reconcile>(sql).ToList();
            int count = 0;//一个月上的天数

            if (mydata.Count == 2)
            {
                int mycount = 0;
                for (int i = 0; i < mydata.Count; i++)
                {
                    if (mydata[0].Curse_Id == "上午" || mydata[0].Curse_Id == "下午")
                    {
                        mycount++;
                    }
                }
                if (mycount == mydata.Count)
                {
                    count++;
                }
            }
            else if (mydata.Count == 4)
            {
                int mycount = 0;
                for (int i = 0; i < mydata.Count; i++)
                {
                    if (mydata[i].Curse_Id.Contains("12"))
                    {
                        mycount += 1;
                    }
                    else if (mydata[i].Curse_Id.Contains("34"))
                    {
                        mycount += 1;
                    }
                }

                if (mycount == mydata.Count)
                {
                    count++;
                }
            }
            else if (mydata.Count == 3)
            {
                int mycount = 0;
                for (int i = 0; i < mydata.Count; i++)
                {
                    if (mydata[i].Curse_Id.Contains("12"))
                    {
                        mycount += 1;
                    }
                    else if (mydata[i].Curse_Id.Contains("34"))
                    {
                        mycount += 1;
                    }
                    else if (mydata[i].Curse_Id.Contains("上午"))
                    {
                        mycount += 1;
                    }
                    else if (mydata[i].Curse_Id.Contains("下午"))
                    {
                        mycount += 1;
                    }
                }
                if (mycount == mydata.Count)
                {
                    count++;
                }
            }


            return count;
        }
    }
}