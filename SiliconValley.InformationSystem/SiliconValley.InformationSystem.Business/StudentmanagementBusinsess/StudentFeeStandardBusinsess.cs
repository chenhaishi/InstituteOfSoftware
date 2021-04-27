using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.FinaceBusines;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.EnrollmentBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness;
using SiliconValley.InformationSystem.Entity;
using SiliconValley.InformationSystem.Entity.ViewEntity.XYK_Data;

namespace SiliconValley.InformationSystem.Business.StudentmanagementBusinsess
{

    public class StudentFeeStandardBusinsess : BaseBusiness<StudentFeeStandard>
    {
        private static int count = 0;

        //专业表
        BaseBusiness<Specialty> special = new BaseBusiness<Specialty>();
        //明目类型
        BaseBusiness<CostitemsX> costitemssX = new BaseBusiness<CostitemsX>();
        //班主任表
        HeadmasterBusiness headmasters = new HeadmasterBusiness();
        //学员班级
        ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
        //阶段专业表
        BaseBusiness<StageGrade> stagegrade = new BaseBusiness<StageGrade>();
        //阶段表
        BaseBusiness<Grand> geand = new BaseBusiness<Grand>();
        //班级表
        ClassScheduleBusiness classschedu = new ClassScheduleBusiness();
        //学费详情
        BaseBusiness<Studenttuitionfeestandard> Feestandard = new BaseBusiness<Studenttuitionfeestandard>();
        //学员信息
        StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
        //费用明目
        CostitemsBusiness costitemsBusiness = new CostitemsBusiness();
        //自考本科
        EnrollmentBusinesse enrollmentBusiness = new EnrollmentBusinesse();
        //升学阶段
        BaseBusiness<GotoschoolStage> GotoschoolStageBusiness = new BaseBusiness<GotoschoolStage>();
        //审核缴费业务类
        BaseBusiness<Payview> PayviewBusiness = new BaseBusiness<Payview>();
        //核对缴费是否成功业务类
        BaseBusiness<Paymentverification> PaymentverificationBusiness = new BaseBusiness<Paymentverification>();
        //审核缴费信息对应业务类
        BaseBusiness<PayviewPaymentver> PayviewPaymentverBusiness = new BaseBusiness<PayviewPaymentver>();
        //预入费退费业务类
        BaseBusiness<Refund> RefundBusiness = new BaseBusiness<Refund>();
        BaseBusiness<StudentFeeRecord> StudentFeeRecord_list_add = new BaseBusiness<StudentFeeRecord>();
        //缴费视图
        BaseBusiness<StudentFeeRecordListView> StudentFeeRecordList = new BaseBusiness<StudentFeeRecordListView>();

        //退费业务类
        BaseBusiness<Tuitionrefund> TuitionrefundBusiness = new BaseBusiness<Tuitionrefund>();
        /// <summary>
        /// 学生班级查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ScheduleForTrainees schFor_Class(string id)
        {
            BaseBusiness<ScheduleForTrainees> classID = new BaseBusiness<ScheduleForTrainees>();
            var sql = "select * from ScheduleForTrainees where StudentID='"+id+ "' and CurrentClass='1' or CurrentClass='0' and StudentID='" + id + "'";
            var sch_classid = classID.GetListBySql<ScheduleForTrainees>(sql).ToList();
            return sch_classid[0];
        }
        /// <summary>
        /// 根据学号获取学生缴费备注
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StudentFeeRecord Remarks(string id)
        {
            var sql = "select * from StudentFeeRecord where StudenID='" + id + "'";
           var fee= StudentFeeRecord_list_add.GetListBySql<StudentFeeRecord>(sql).ToList();
            return fee[0];
        }
        /// <summary>
        /// 获取学员现在所读阶段
        /// </summary>
        /// <param name="id">学员学号</param>
        public Grand StateGrand(string id)
        {
            BaseBusiness<ScheduleForTrainees> ScheduleForTrainees = new BaseBusiness<ScheduleForTrainees>();
            BaseBusiness<ClassSchedule> ClassSchedule = new BaseBusiness<ClassSchedule>();
            BaseBusiness<Grand> Grand = new BaseBusiness<Grand>();
            string sql = "select * from ScheduleForTrainees where StudentID='" + id + "' and CurrentClass='1'";
            var listall = ScheduleForTrainees.GetListBySql<ScheduleForTrainees>(sql).ToList();
            //var list = ScheduleForTrainees.GetList().Where(d => d.StudentID == id && d.CurrentClass == true).SingleOrDefault();
            List<ClassSchedule> schlist = null;
            foreach (var item in listall)
            {
                 schlist = ClassSchedule.GetList().Where(d => d.id == item.ID_ClassName).ToList();
            }
            List<Grand> Grandlist = null;
            foreach (var item in schlist)
            {
                 Grandlist = Grand.GetList().Where(d => d.Id == item.grade_Id).ToList();
            }


            return Grandlist[0];
        }
        /// <summary>
        /// 获取所有学员数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="Name">姓名</param>
        /// <param name="Sex">性别</param>
        /// <param name="StudentNumber">学号</param>
        /// <param name="identitydocument">身份证号码</param>
        /// <returns></returns>
        public object GetDate(int page, int limit, string Name, string Sex, string StudentNumber, string identitydocument)
        {
            BaseBusiness<StudentpaymentView> StudentpaymentViewbusiness = new BaseBusiness<StudentpaymentView>();
            //班主任带班
            BaseBusiness<HeadClass> Hoadclass = new BaseBusiness<HeadClass>();
            //    List<StudentInformation>list=  dbtext.GetPagination(dbtext.GetIQueryable(),page,limit, dbtext)
            // List<StudentInformation> list = studentInformationBusiness.Mylist("StudentInformation").Where(a => a.IsDelete ==false).ToList();
            List<StudentpaymentView> list = StudentpaymentViewbusiness.GetListBySql<StudentpaymentView>("select * from StudentpaymentView");

            if (!string.IsNullOrEmpty(Name))
            {
                list = list.Where(a => a.Name.Contains(Name)).ToList();
            }
            if (!string.IsNullOrEmpty(Sex))
            {
                bool sex = Convert.ToBoolean(Sex);
                list = list.Where(a => a.Sex == sex).ToList();
            }
            if (!string.IsNullOrEmpty(StudentNumber))
            {
                list = list.Where(a => a.StudentNumber.Contains(StudentNumber)).ToList();
            }
            if (!string.IsNullOrEmpty(identitydocument))
            {
                list = list.Where(a => a.identitydocument.Contains(identitydocument)).ToList();
            }

            StudentDataKeepAndRecordBusiness studentDataKeepAndRecord = new StudentDataKeepAndRecordBusiness();
            StudentFeeStandardBusinsess StudentFeeStandardBusinsess = new StudentFeeStandardBusinsess();
            var dataList = list.Select(a => new
            {
                a.ClassName,
                a.StudentNumber,
                a.Name,
                a.identitydocument,
                a.Sex,
                a.Headmasters,
                a.BirthDate,
                StudentFeeStandardBusinsess.StateGrand(a.StudentNumber).GrandName,
                studentDataKeepAndRecord.findId(a.StudentPutOnRecord_Id.ToString()).ConsultTeacher,
                studentDataKeepAndRecord.findId(a.StudentPutOnRecord_Id.ToString()).empName
            }).OrderByDescending(a => a.StudentNumber).Skip((page - 1) * limit).Take(limit).ToList();
            //  var x = dbtext.GetList();
            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = dataList
            };
            return data;
        }
        /// <summary>
        /// 下拉框获取价格
        /// </summary>
        /// <param name="Grand_id">阶段</param>
        /// <param name="Rategory">明目类型</param>
        /// <returns></returns>
        public Costitems Singlecostitems(int? Grand_id, int Rategory)
        {
            var list = costitemsBusiness.costitemslist().Where(a => a.Rategory == Rategory);
            if (Grand_id != null)
            {
                list = list.Where(a => a.Grand_id == Grand_id);
            }
            return list.FirstOrDefault();
        }
        //学员费用
        BaseBusiness<StudentFeeRecord> studentfee = new BaseBusiness<StudentFeeRecord>();
        BaseBusiness<StudentFeeRecordListView> SfeeRecordList = new BaseBusiness<StudentFeeRecordListView>();
        //财务人员
        BaseBusiness<FinanceModel> finacemo = new BaseBusiness<FinanceModel>();
        /// <summary>
        /// 添加学员费用
        /// </summary>
        /// <param name="studentFeeRecord">费用对象数据</param>
        /// <returns></returns>
        public AjaxResult AddstudentFeeRecord(StudentFeeRecord studentFeeRecord)
        {
            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            AjaxResult retus = null;
            try
            {
                var fine = finacemo.GetList().Where(a => a.Financialstaff == user.EmpNumber).FirstOrDefault();
                studentFeeRecord.FinanceModelid = fine.id;
                studentFeeRecord.IsDelete = false;
                studentFeeRecord.AddDate = DateTime.Now;

                studentfee.Insert(studentFeeRecord);
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "录入费用成功";
                BusHelper.WriteSysLog("添加费用费用数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }
        /// <summary>
        /// 自考本科费用录入
        /// </summary>
        /// <param name="studentFeeRecord">数据对象</param>
        /// <param name="Rategorys">多个费用明目id</param>
        /// <returns></returns>
        public AjaxResult Tuitionandfees(string StudenID, string Remarks, string Costitemsid)
        {
            //tring StudenID,string Remarks,string Costitemsid
            List<Payview> listFeeRecord = new List<Payview>();
            BaseBusiness<Enrollment> Enrollment = new BaseBusiness<Enrollment>();
            List<Enrollment> ENlist = new List<Enrollment>();
            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();

            var fine = finacemo.GetList().Where(a => a.Financialstaff == user.EmpNumber).FirstOrDefault();
            AjaxResult retus = null;
            try
            {
                var Costitems = Costitemsid.Substring(0, Costitemsid.Length - 1).Split(',');
                foreach (var item in Costitems)
                {
                    if (Enrollment.GetList().Where(d=>d.StudentNumber==StudenID).Count()==0)
                    {
                        Payview studentFee = new Payview();
                        studentFee.StudenID = StudenID;
                        studentFee.FinanceModelid = fine.id;
                        studentFee.IsDelete = false;
                        studentFee.AddDate = DateTime.Now;
                        studentFee.Costitemsid = int.Parse(item);
                        studentFee.Amountofmoney = costitemsBusiness.GetEntity(int.Parse(item)).Amountofmoney;
                        studentFee.Remarks = Remarks;
                        listFeeRecord.Add(studentFee);
                        //学历添加
                        Enrollment listEnrollment = new Enrollment();
                        listEnrollment.PassNumber = null;
                        listEnrollment.Datestration = null;
                        listEnrollment.School = null;
                        listEnrollment.StudentNumber = StudenID;
                        listEnrollment.Remarks = null;
                        listEnrollment.IsDelete = false;
                        listEnrollment.MajorID = null;
                        listEnrollment.Registeredbatch = null;
                        ENlist.Add(listEnrollment);
                    }
                    else
                    {
                        Payview studentFee = new Payview();
                        studentFee.StudenID = StudenID;
                        studentFee.FinanceModelid = fine.id;
                        studentFee.IsDelete = false;
                        studentFee.AddDate = DateTime.Now;
                        studentFee.Costitemsid = int.Parse(item);
                        studentFee.Amountofmoney = costitemsBusiness.GetEntity(int.Parse(item)).Amountofmoney;
                        studentFee.Remarks = Remarks;
                        listFeeRecord.Add(studentFee);
                    }
                    
                   
                    //this.Studentpayment(studentFee.StudenID, fine.id, 1);
                }
                Enrollment.Insert(ENlist);
                SessionHelper.Session["person"] = listFeeRecord;
                PayviewBusiness.Insert(listFeeRecord);
                //添加核对表
                Paymentverification paymentverification = new Paymentverification();
                paymentverification.Passornot = null;
                paymentverification.OddNumbers = null;
                paymentverification.AddDate = null;

                PaymentverificationBusiness.Insert(paymentverification);
                List<PayviewPaymentver> payviewPaymentverslist = new List<PayviewPaymentver>();
                var FeeRecordlist = PayviewBusiness.GetList().Where(a => a.StudenID == StudenID && a.FinanceModelid == fine.id).OrderByDescending(a => a.ID).Take(listFeeRecord.Count()).ToList();
                foreach (var item in listFeeRecord)
                {
                    PayviewPaymentver payviewPaymentver = new PayviewPaymentver();
                    payviewPaymentver.Paymentver = PaymentverificationBusiness.GetList().OrderByDescending(a => a.id).Take(1).FirstOrDefault().id;
                    payviewPaymentver.Payviewid = item.ID;
                    payviewPaymentverslist.Add(payviewPaymentver);
                }
                PayviewPaymentverBusiness.Insert(payviewPaymentverslist);
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "录入费用成功";
                BusHelper.WriteSysLog("添加费用费用数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;

        }

        public object Drivingschoolpayment(Payview studentFeez)
        {


            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            var fine = finacemo.GetList().Where(a => a.Financialstaff == user.EmpNumber).FirstOrDefault();
            AjaxResult retus = null;
            try
            {
                List<Payview> liststudents = new List<Payview>();
                Payview studentFee = new Payview();
                studentFee.StudenID = studentFeez.StudenID;
                studentFee.Amountofmoney = studentFeez.Amountofmoney;
                studentFee.AddDate = DateTime.Now;
                studentFee.Remarks = studentFeez.Remarks;
                studentFee.IsDelete = false;
                studentFee.Costitemsid = studentFeez.Costitemsid;
                studentFee.FinanceModelid = fine.id;
                PayviewBusiness.Insert(studentFee);
                liststudents.Add(studentFee);
                this.Studentpayment(studentFee.StudenID, fine.id, 1);


                SessionHelper.Session["person"] = liststudents;



                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "录入费用成功";
                BusHelper.WriteSysLog("添加费用费用数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }
        /// <summary>
        /// 没有阶段的费用明目在这获取费用
        /// </summary>
        /// <param name="id">明目id</param>
        /// <returns></returns>
        public int Otherexpenses(string id)
        {

            string[] mingmu = id.Split(',');
            //价格
            int price = 0;
            foreach (var item in mingmu)
            {
                int cid = int.Parse(item);
                price = price + Convert.ToInt32(costitemsBusiness.GetEntity(cid).Amountofmoney);
            }
            return price;

        }
        /// <summary>
        /// 根据名目类型获取明目
        /// </summary>
        /// <param name="Typeid">类型id</param>
        /// <returns></returns>
        public List<Costitems> ListTuitionandfees(int Typeid)
        {
            return costitemsBusiness.costitemslist().Where(a => a.Rategory == Typeid).ToList();
        }
        /// <summary>
        /// 根据学号获取学杂费获取当前没有缴费的名目
        /// </summary>
        /// <param name="StudentID">学号</param>
        /// <param name="Typeid"><明目类型/param>
        /// <returns></returns>
        public List<Costitems> boolTuitionandfees(string StudentID, int Typeid)
        {
            var list = this.ListTuitionandfees(Typeid);
            var FeeRecordsList = this.SinglestudentFeeRecords(StudentID);
            foreach (var item in FeeRecordsList)
            {
                list = list.Where(a => a.id != item.Costitemsid).ToList();
            }
            return list;

        }
        /// <summary>
        /// 获取所有缴费记录
        /// </summary>
        /// <returns></returns>
        public List<StudentFeeRecord> ListstudentFeeRecords()
        {
            return studentfee.GetList().Where(a => a.IsDelete == false).ToList();
        }
        /// <summary>
        /// 获取单个学员所有缴费记录
        /// </summary>
        /// <param name="Studentid">学号</param>
        /// <returns></returns>
        public List<StudentFeeRecord> SinglestudentFeeRecords(string Studentid)
        {
            return this.ListstudentFeeRecords().Where(a => a.StudenID == Studentid).ToList();
        }
        /// <summary>
        /// 学员费用查询复杂
        /// </summary>
        /// <param name="Grand_id">阶段</param>
        /// <returns></returns>
        public object Studentfeepayment(int Grand_id, string studentid)
        {
            int countfee = 0;
            int countfee1 = 0;
            var idcost = costitemssX.GetList().Where(a => a.IsDelete == false && a.Name == "学杂费").FirstOrDefault().id;
            var costitemslist = costitemsBusiness.costitemslist().Where(a => a.Rategory == idcost && a.IsDelete == false && a.Grand_id == Grand_id).ToList();
            List<Costitems> costlist = new List<Costitems>();
            foreach (var item in costitemslist)
            {
                var x = studentfee.GetListBySql<StudentFeeRecord>("select * from StudentFeeRecord where IsDelete='false' and StudenID='" + studentid + "' and Costitemsid=" + item.id).FirstOrDefault();
                // studentfee.GetList().Where(a => a.IsDelete == false && a.StudenID == studentid && a.Costitemsid == item.id).FirstOrDefault();
                // string studentViewSQL = "select * from StudentFeeRecordView where StudenID='" + item.StudentID + "'";//根据学生id查询出缴费数据
                // studentView = StudentFeeRecordView.GetListBySql<StudentFeeRecordView>(studentViewSQL).ToList();
                var SQLP = "select * from StudentFeeRecordListView where IsDelete='false' and StudenID='" + studentid + "' and Passornot='2'";
                var p = StudentFeeRecordList.GetListBySql<StudentFeeRecordListView>(SQLP).ToList();
                if (x != null)
                {
                    if (p.Count() == 0)
                    {
                        costlist.Add(item);
                    }
                    
                }
            }
            foreach (var item in costlist)
            {
                costitemslist.Remove(item);
            }

            List<Costitems> z = new List<Costitems>();




            z.AddRange(costitemsBusiness.GetList().Where(a => a.Grand_id == Grand_id && a.IsDelete == false & a.Rategory != 10).ToList());
            z.AddRange(costitemslist);
            var z1 = z.Select(a => new { a.id, a.Name, a.Amountofmoney, Rategory = costitemssX.GetEntity(a.Rategory).Name, countfee, countfee1 }).ToList();
            return z1;
        }
        /// <summary>
        /// 根据学号获取姓名，性别，班级，身份证，学号
        /// </summary>
        /// <param name="studentid">学号</param>
        /// <returns></returns>
        public object StudentFind(string studentid)
        {

            var a = studentInformationBusiness.GetEntity(studentid);
            var ClassID = scheduleForTraineesBusiness.SutdentCLassName(a.StudentNumber).ID_ClassName;
            var x = new
            {
                StudentNumber = a.StudentNumber,//学号
                Name = a.Name,//姓名
                identitydocument = a.identitydocument,//身份证号码
                Sex = a.Sex,//性别,

                GrandName = classschedu.GetClassGrand(ClassID, 22),//阶段
                classa = classschedu.GetList().Where(q => q.IsDelete == false && q.ClassStatus == false && q.id == ClassID).FirstOrDefault().ClassNumber//班级号                                                                                                                                              //a => a.IsDelete == false && a.ClassStatus == false
            };
            return x;
        }

        /// <summary>
        /// 学员阶段费用录入
        /// </summary>
        /// <param name="studentFeeRecords">集合数据</param>
        /// <returns></returns>
        public AjaxResult StudentPrices(List<StudentFeeRecord> studentFeeRecords, string Remarks, string Help)
        {
            string studentid = "";
            int Costitemsid = 0;
            AjaxResult retus = null;
            List<Payview> listFeeRecord = new List<Payview>();
            List<StudentFeeRecord> StudentFeeRecordss = new List<StudentFeeRecord>();
            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            var fine = finacemo.GetList().Where(a => a.Financialstaff == user.EmpNumber).FirstOrDefault();
            if (fine == null)
            {

                retus = new ErrorResult();
                retus.Msg = "对不起,当前登陆账号不能进行缴费！";
                retus.Success = false;
                retus.ErrorCode = 500;
            }
            else
            {
                Costitemsid = fine.id;
                foreach (var item in studentFeeRecords)
                {
                    studentid = item.StudenID;
                    Payview studentFeeRecord = new Payview();
                    studentFeeRecord.IsDelete = false;
                    studentFeeRecord.AddDate = DateTime.Now;
                    studentFeeRecord.FinanceModelid = fine.id;
                    studentFeeRecord.Costitemsid = item.Costitemsid;
                    studentFeeRecord.Amountofmoney = item.Amountofmoney;
                    studentFeeRecord.StudenID = item.StudenID;
                    studentFeeRecord.Remarks = Remarks + Help;
                    listFeeRecord.Add(studentFeeRecord);
                }
                try
                {
                    SessionHelper.Session["person"] = listFeeRecord;
                    PayviewBusiness.Insert(listFeeRecord);
                    foreach (var item in listFeeRecord)
                    {
                        StudentFeeRecord pay = new StudentFeeRecord();
                        pay.Amountofmoney = item.Amountofmoney;
                        pay.AddTime = null;
                        pay.Costitemsid = item.Costitemsid;
                        pay.AddDate = item.AddDate;
                        pay.StudenID = item.StudenID;
                        pay.FinanceModelid = item.FinanceModelid;
                        pay.Remarks = item.Remarks;
                        pay.IsDelete = item.IsDelete;
                        StudentFeeRecordss.Add(pay);
                    }
                    StudentFeeRecord_list_add.Insert(StudentFeeRecordss);
                    //添加核对表
                    Paymentverification paymentverification = new Paymentverification();
                    paymentverification.Passornot = null;
                    paymentverification.OddNumbers = null;
                    paymentverification.AddDate = null;
                    PaymentverificationBusiness.Insert(paymentverification);
                    List<PayviewPaymentver> payviewPaymentverslist = new List<PayviewPaymentver>();

                    var FeeRecordlist = PayviewBusiness.GetList().Where(a => a.StudenID == studentid && a.FinanceModelid == Costitemsid).OrderByDescending(a => a.ID).Take(listFeeRecord.Count()).ToList();
                    foreach (var item in FeeRecordlist)
                    {
                        PayviewPaymentver payviewPaymentver = new PayviewPaymentver();
                        payviewPaymentver.Paymentver = PaymentverificationBusiness.GetList().OrderByDescending(a => a.id).Take(1).FirstOrDefault().id;
                        payviewPaymentver.Payviewid = item.ID;
                        payviewPaymentverslist.Add(payviewPaymentver);
                    }
                    PayviewPaymentverBusiness.Insert(payviewPaymentverslist);


                    retus = new SuccessResult();
                    retus.Success = true;
                    retus.Msg = "录入费用成功";


                    BusHelper.WriteSysLog("录入费用模拟数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
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
            return retus;
        }
        /// <summary>
        /// 验证当前是否有操作权限
        /// </summary>
        /// <returns></returns>
        public bool IsFinanc()
        {
            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            var fine = finacemo.GetList().Where(a => a.Financialstaff == user.EmpNumber).FirstOrDefault();
            if (fine == null)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 收据项目及费用详情
        /// </summary>
        /// <param name="studentFeeRecords">集合数据</param>
        /// <returns></returns>
        public object Receiptdata(List<Payview> studentFeeRecords)
        {
            int Stage = Convert.ToInt32(SessionHelper.Session["Stage"]);
            return studentFeeRecords.Select(a => new
            {
                a.Amountofmoney,
                ostitemsName = costitemsBusiness.GetEntity(a.Costitemsid).Name,
                GrandName = Stage > 0 ? geand.GetEntity(Stage).GrandName : "",
                Rategory = costitemssX.GetEntity(costitemsBusiness.GetEntity(a.Costitemsid).Rategory).Name,
                Remarks = a.Remarks,
                a.AddDate

            }).ToList();
        }
        /// <summary>
        /// 根据学号查询出所有缴费记录
        /// </summary>
        /// <param name="student">学号</param>
        /// <returns></returns>
        public List<vierprice> FienPrice(string student)
        {
            List<object> students = new List<object>();
            var st = SfeeRecordList.GetList().Where(a => a.IsDelete == false && a.StudenID == student&&a.Passornot=="1"&&a.AddTime!=null).ToList();
            foreach (var item in st)
            {
                students.Add(Convert.ToDateTime(item.AddDate).ToLongDateString().ToString());
            }
            var x = students.Distinct().ToList();
            List<vierprice> listvier = new List<vierprice>();
            // string date = Convert.ToDateTime(item.AddDate).ToLongDateString().ToString();
            foreach (var item1 in x)
            {
                vierprice vierprices = new vierprice();
                vierprices.Date = item1.ToString();
                List<vierprice> view = new List<vierprice>();
                foreach (var item in st)
                {

                    string date = Convert.ToDateTime(item.AddDate).ToLongDateString().ToString();
                    if (item1.ToString() == date)
                    {
                        var costit = costitemsBusiness.GetEntity(item.Costitemsid);
                        vierprice stivier = new vierprice();
                        stivier.Amountofmoney = (decimal)item.Amountofmoney;
                        stivier.CostitemName = costit.Name;
                        stivier.id = item.ID;
                        stivier.GrandName = costit.Grand_id != null ? geand.GetEntity(costit.Grand_id).GrandName : "";
                        stivier.Rategory = costitemssX.GetEntity(costit.Rategory).Name;
                        view.Add(stivier);
                    }
                }
                vierprices.Chicked = view;
                listvier.Add(vierprices);
            }
            return listvier;

        }


        public List<vierprice> FienTuitionrefund(List<vierprice> vierprices2)
        {
            List<vierprice> vierprices1 = new List<vierprice>();
            foreach (var item in vierprices2)
            {
                foreach (var item2 in item.Chicked)
                {
                    vierprices1.Add(item2);
                }
            }
            List<Tuitionrefund> listTuitionrefund = new List<Tuitionrefund>();
            foreach (var item in vierprices1)
            {
                var x1 = TuitionrefundBusiness.GetList().Where(a => a.StudentFeeRecordId == item.id).FirstOrDefault();
                if (x1 != null)
                {
                    listTuitionrefund.Add(x1);
                }
            }
            List<object> students = new List<object>();
            foreach (var item in listTuitionrefund)
            {
                students.Add(Convert.ToDateTime(item.AddDate).ToLongDateString().ToString());
            }
            var x = students.Distinct().ToList();
            List<vierprice> listvier = new List<vierprice>();
            foreach (var item1 in x)
            {
                vierprice vierprices = new vierprice();
                vierprices.Date = item1.ToString();
                List<vierprice> view = new List<vierprice>();
                foreach (var item in listTuitionrefund)
                {

                    string date = Convert.ToDateTime(item.AddDate).ToLongDateString().ToString();
                    if (item1.ToString() == date)
                    {
                        var costit = costitemsBusiness.GetEntity(studentfee.GetEntity(item.StudentFeeRecordId).Costitemsid);
                        vierprice stivier = new vierprice();
                        stivier.Amountofmoney = (decimal)item.Amountofmoney;
                        stivier.CostitemName = costit.Name;

                        stivier.GrandName = costit.Grand_id != null ? geand.GetEntity(costit.Grand_id).GrandName : "";
                        stivier.Rategory = costitemssX.GetEntity(costit.Rategory).Name;
                        view.Add(stivier);
                    }
                }
                vierprices.Chicked = view;
                listvier.Add(vierprices);
            }
            return listvier;

        }
        /// <summary>
        /// 其它缴费数据操作
        /// </summary>
        /// <param name="StudenID">学号</param>
        /// <param name="Consumptionname">名称</param>
        /// <param name="Amountofmoney">金额</param>
        /// <param name="Remarks">备注</param>
        /// <param name="Typeid">名目</param>
        /// <returns></returns>
        public AjaxResult Otherconsumption(string StudenID, string Consumptionname, decimal Amountofmoney, string Remarks, int Typeid, string Consumptionname_su, decimal Amountofmoney_su)
        {
            AjaxResult retus = null;
            try
            {
                //当前登陆人
                Base_UserModel user = Base_UserBusiness.GetCurrentUser();
                var fine = finacemo.GetList().Where(a => a.Financialstaff == user.EmpNumber).FirstOrDefault();
                var x = costitemsBusiness.GetList().Where(a => a.Name == Consumptionname).FirstOrDefault();
                var xu = costitemsBusiness.GetList().Where(d => d.Name == Consumptionname_su).FirstOrDefault();
                if (x == null || xu == null)
                {
                    if (Consumptionname_su != null)
                    {
                        Costitems costitems_list = new Costitems();
                        costitems_list.Amountofmoney = Amountofmoney_su;
                        costitems_list.Name = Consumptionname_su;
                        costitems_list.IsDelete = false;
                        costitems_list.Rategory = Typeid;
                        costitemsBusiness.Insert(costitems_list);
                        xu = costitemsBusiness.GetList().Where(a => a.Name == Consumptionname_su).OrderByDescending(d => d.id).FirstOrDefault();
                    }

                    Costitems costitems = new Costitems();
                    costitems.Amountofmoney = Amountofmoney;
                    costitems.Name = Consumptionname;
                    costitems.IsDelete = false;
                    costitems.Rategory = Typeid;
                    costitemsBusiness.Insert(costitems);
                    x = costitemsBusiness.GetList().Where(a => a.Name == Consumptionname).OrderByDescending(a => a.id).FirstOrDefault();
                }
                List<Payview> liststudents_1 = new List<Payview>();
                if (Amountofmoney == 0)
                {
                    Payview studentFee_2 = new Payview();
                    studentFee_2.StudenID = StudenID;
                    studentFee_2.Amountofmoney = Amountofmoney_su;
                    studentFee_2.AddDate = DateTime.Now;
                    studentFee_2.Remarks = Remarks;
                    studentFee_2.IsDelete = false;
                    studentFee_2.Costitemsid = xu.id;
                    studentFee_2.FinanceModelid = fine.id;
                    PayviewBusiness.Insert(studentFee_2);
                    liststudents_1.Add(studentFee_2);
                    this.Studentpayment(StudenID, fine.id, 1);
                    SessionHelper.Session["person"] = liststudents_1;
                    retus = new SuccessResult();
                    retus.Success = true;
                    retus.Msg = "缴费成功";
                    BusHelper.WriteSysLog("其它缴费数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
                else if (Amountofmoney_su == 0)
                {
                    Payview studentFee_3 = new Payview();
                    studentFee_3.StudenID = StudenID;
                    studentFee_3.Amountofmoney = Amountofmoney;
                    studentFee_3.AddDate = DateTime.Now;
                    studentFee_3.Remarks = Remarks;
                    studentFee_3.IsDelete = false;
                    studentFee_3.Costitemsid = x.id;
                    studentFee_3.FinanceModelid = fine.id;
                    PayviewBusiness.Insert(studentFee_3);
                    liststudents_1.Add(studentFee_3);
                    this.Studentpayment(StudenID, fine.id, 1);
                    SessionHelper.Session["person"] = liststudents_1;
                    retus = new SuccessResult();
                    retus.Success = true;
                    retus.Msg = "缴费成功";
                    BusHelper.WriteSysLog("其它缴费数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
                else
                {
                    if (Consumptionname_su != null)
                    {

                        Payview studentFee_1 = new Payview();
                        studentFee_1.StudenID = StudenID;
                        studentFee_1.Amountofmoney = Amountofmoney_su;
                        studentFee_1.AddDate = DateTime.Now;
                        studentFee_1.Remarks = Remarks;
                        studentFee_1.IsDelete = false;
                        studentFee_1.Costitemsid = xu.id;
                        studentFee_1.FinanceModelid = fine.id;
                        PayviewBusiness.Insert(studentFee_1);
                        liststudents_1.Add(studentFee_1);
                        this.Studentpayment(StudenID, fine.id, 1);
                        SessionHelper.Session["person"] = liststudents_1;
                        retus = new SuccessResult();
                        retus.Success = true;
                        retus.Msg = "缴费成功";
                        BusHelper.WriteSysLog("其它缴费数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                    }

                    Payview studentFee = new Payview();
                    studentFee.StudenID = StudenID;
                    studentFee.Amountofmoney = Amountofmoney;
                    studentFee.AddDate = DateTime.Now;
                    studentFee.Remarks = Remarks;
                    studentFee.IsDelete = false;
                    studentFee.Costitemsid = x.id;
                    studentFee.FinanceModelid = fine.id;
                    PayviewBusiness.Insert(studentFee);
                    liststudents_1.Add(studentFee);
                    this.Studentpayment(StudenID, fine.id, 1);
                    SessionHelper.Session["person"] = liststudents_1;

                    retus = new SuccessResult();
                    retus.Success = true;
                    retus.Msg = "缴费成功";
                    BusHelper.WriteSysLog("其它缴费数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;

        }
        /// <summary>
        /// 添加学员费用方法
        /// </summary>
        /// <param name="studentid">学号</param>
        /// <param name="Costitemsid">操作人</param>
        /// <returns></returns>
        public bool Studentpayment(string studentid, int Costitemsid, int count)
        {
            bool str = false;
            try
            {
                str = true;
                //添加核对表
                Paymentverification paymentverification = new Paymentverification();
                paymentverification.Passornot = null;
                paymentverification.OddNumbers = null;
                paymentverification.AddDate = null;
                PaymentverificationBusiness.Insert(paymentverification);
                List<PayviewPaymentver> payviewPaymentverslist = new List<PayviewPaymentver>();
                //
                var FeeRecordlist = PayviewBusiness.GetList().Where(a => a.StudenID == studentid && a.FinanceModelid == Costitemsid).OrderByDescending(a => a.ID).Take(count).ToList();
                foreach (var item in FeeRecordlist)
                {
                    PayviewPaymentver payviewPaymentver = new PayviewPaymentver();
                    payviewPaymentver.Paymentver = PaymentverificationBusiness.GetList().OrderByDescending(a => a.id).Take(1).FirstOrDefault().id;
                    payviewPaymentver.Payviewid = item.ID;
                    payviewPaymentverslist.Add(payviewPaymentver);
                }
                PayviewPaymentverBusiness.Insert(payviewPaymentverslist);

            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return str;
        }
        /// <summary>
        /// 获取学员缴费数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="StudentID">学号</param>
        /// <param name="Name">姓名</param>
        /// <param name="TypeID">类型</param>
        /// <param name="qBeginTime">开始时间</param>
        /// <param name="qEndTime">结束时间</param>
        /// <returns></returns>
        public List<StudentFeeRecordView> Nominaldata(string StudentID, string Name, string TypeID, string qBeginTime, string qEndTime, string CostitemsName)
        {
            BaseBusiness<StudentFeeRecordView> StudentFeeRecordViewBusiness = new BaseBusiness<StudentFeeRecordView>();
            EmployeesInfoManage employeesInfoManage = new EmployeesInfoManage();
            count++;
            var x = StudentFeeRecordViewBusiness.GetList().Where(a => a.IsDelete == false).ToList();
            //.Mylist("StudentFeeRecord")
            //var x = StudentFeeRecordViewBusiness.GetList().Where(a => a.IsDelete == false).Select(a=>new StudentFeeRecordView{
            //   StudenID=  a.StudenID,
            //    AddDate=a.AddDate,
            //    Name= a.Name,
            //    FinancialstaffName= a.FinancialstaffName,
            //    ID=a.ID,
            //    Amountofmoney= a.Amountofmoney,
            //    Costitemsid= a.Costitemsid,
            //    ClassName=a.ClassName,
            //    CostitemsName=a.CostitemsName,
            //    StageName=a.StageName
            //}).ToList();
            //var x = studentfee.GetList().Where(a=>a.IsDelete==false).Select(a => new StudentFeeRecordView
            //{
            //    StudenID = a.StudenID,
            //    AddDate = (DateTime)a.AddDate,
            //    Name = studentInformationBusiness.GetEntity(a.StudenID).Name,
            //    FinancialstaffName = employeesInfoManage.GetEntity(finacemo.GetEntity(a.FinanceModelid).Financialstaff).EmpName,
            //    ID = a.ID,
            //    Amountofmoney = (decimal)a.Amountofmoney,
            //    Costitemsid = a.Costitemsid,
            //    ClassName = scheduleForTraineesBusiness.SutdentCLassName(a.StudenID) == null ? "暂无班级" : scheduleForTraineesBusiness.SutdentCLassName(a.StudenID).ClassID,
            //    CostitemsName = costitemsBusiness.GetEntity(a.Costitemsid).Name,
            //    StageName= costitemsBusiness.GetEntity(a.Costitemsid).Grand_id==null?"": geand.GetEntity(costitemsBusiness.GetEntity(a.Costitemsid).Grand_id).GrandName
            //}).ToList();
            if (count == 1)
            {
                x = x.Where(a => Convert.ToDateTime(a.AddDate).ToShortDateString() == DateTime.Now.ToShortDateString()).ToList();
            }
            if (!string.IsNullOrEmpty(StudentID))
            {
                x = x.Where(a => a.StudenID == StudentID).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                x = x.Where(a => a.Name.Contains(Name)).ToList();
            }
            if (!string.IsNullOrEmpty(TypeID))
            {
                int typeid = int.Parse(TypeID);

                var mycosti = costitemsBusiness.GetList().Where(a => a.Rategory == typeid).ToList();

                List<StudentFeeRecordView> feereview = new List<StudentFeeRecordView>();
                //往z集合添加数据
                foreach (var item in mycosti)
                {
                    var mx = x.Where(a => a.Costitemsid == item.id).ToList();
                    feereview.AddRange(mx);
                }
                x = feereview;
            }

            if (!string.IsNullOrEmpty(CostitemsName))
            {
                List<Costitems> costli = new List<Costitems>();
                List<StudentFeeRecordView> studentFeeRecordViews = new List<StudentFeeRecordView>();
                if (CostitemsName == "全部学杂")
                {
                    costli = costitemsBusiness.GetList().Where(a => a.IsDelete == false && a.Rategory == 10).ToList();
                }
                else
                {

                    costli = costitemsBusiness.GetList().Where(a => a.Name == CostitemsName && a.IsDelete == false && a.Rategory == 10).ToList();

                }
                foreach (var item in costli)
                {
                    studentFeeRecordViews.AddRange(x.Where(a => a.Costitemsid == item.id).ToList());
                }
                x = studentFeeRecordViews;

            }
            if (!string.IsNullOrEmpty(qBeginTime))
            {
                DateTime time = Convert.ToDateTime(qBeginTime);
                x = x.Where(a => Convert.ToDateTime(a.AddDate).ToShortDateString().ToDateTime() >= time).ToList();
            }
            if (!string.IsNullOrEmpty(qEndTime))
            {
                DateTime time = Convert.ToDateTime(qEndTime);
                x = x.Where(a => Convert.ToDateTime(a.AddDate).ToShortDateString().ToDateTime() <= time).ToList();
            }

            return x;

        }
        /// <summary>
        /// 获取总额统计
        /// </summary>
        /// <param name="StudentID">学号</param>
        /// <param name="Name">姓名</param>
        /// <param name="TypeID">类型</param>
        /// <param name="qBeginTime">开始时间</param>
        /// <param name="qEndTime">结束时间</param>
        /// <returns></returns>
        public List<TotalcostView> DateTatal(string StudentID, string Name, string TypeID, string qBeginTime, string qEndTime, string CostitemsName)
        {
            List<TotalcostView> totalcostViews = new List<TotalcostView>();
            var cost = costitemssX.GetList().Select(a => new TotalcostView { TypeID = a.id, TypeName = a.Name }).ToList();
            foreach (var item in cost)
            {
                var mycosti = costitemsBusiness.GetList().Where(a => a.Rategory == item.TypeID).ToList();
                TotalcostView view = new TotalcostView();
                view.TypeName = item.TypeName;

                foreach (var item1 in mycosti)
                {
                    TotalcostView views = new TotalcostView();
                    views.TypeID = item1.id;
                    view.ListTotalcostView.Add(views);
                    view.ListTotalcostView.Add(views);
                }
                totalcostViews.Add(view);

            }
            var x = this.Nominaldata(StudentID, Name, TypeID, qBeginTime, qEndTime, CostitemsName);
            List<TotalcostView> MyViewList = new List<TotalcostView>();

            foreach (var item1 in totalcostViews)
            {
                TotalcostView totalcost = new TotalcostView();
                totalcost.TypeName = item1.TypeName;
                totalcost.Total = 0;
                foreach (var item2 in item1.ListTotalcostView)
                {
                    foreach (var item in x)
                    {
                        if (item2.TypeID == item.Costitemsid)
                        {
                            totalcost.Total = totalcost.Total + item.Amountofmoney;
                        }
                    }
                }
                MyViewList.Add(totalcost);

            }

            return MyViewList;
        }
        /// <summary>
        /// 阶段应缴模型类
        /// </summary>
        public class ViewGotosch
        {
            /// <summary>
            /// 阶段id
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 应交
            /// </summary>
            public decimal Price { get; set; }
        }
        /// <summary>
        /// 模型类
        /// </summary>
        public class ViewCostit
        {
            public string StudentID { get; set; }
            public int? GrandID { get; set; }
            public decimal? Price { get; set; }
        }
        /// <summary>
        /// 获取学员信息
        /// </summary>
        /// <param name="ClassName">班级号</param>
        /// <param name="StudentID">学号</param>
        /// <returns></returns>
        public DetailedcostView FineDetail(int ClassName, string StudentID)
        {
            var student = studentInformationBusiness.GetEntity(StudentID);
            DetailedcostView detailedcostView = new DetailedcostView();
            detailedcostView.ClassName = classschedu.GetEntity(ClassName).ClassNumber;
            detailedcostView.Name = student.Name;
            detailedcostView.Stidentid = student.StudentNumber;
            detailedcostView.Sex = student.Sex == false ? "女" : "男";
            detailedcostView.HeadmasterName = headmasters.ClassHeadmaster(ClassName) == null ? "无" : headmasters.ClassHeadmaster(ClassName).EmpName;
            detailedcostView.Phone = headmasters.ClassHeadmaster(ClassName) == null ? "无" : headmasters.ClassHeadmaster(ClassName).Phone;
            return detailedcostView;
        }
        /// <summary>
        /// 拿到学生欠费的数据
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public List<DetailedcostView> TuitionFine(int ClassID)
        {
            //拿到学生已经交了费用的
            List<DetailedcostView> listdetailedc = new List<DetailedcostView>();
            //升学阶段
            BaseBusiness<GotoschoolStage> GotoschoolStageBusiness = new BaseBusiness<GotoschoolStage>();
            //阶段应交费用
            List<ViewGotosch> gotosches = new List<ViewGotosch>();
            //获取阶段价格单
            var costit = costitemsBusiness.GetList().Where(a => a.Grand_id != null).ToList();
            //当前阶段
            var ClassGrade = classschedu.GetEntity(ClassID).grade_Id;
            //拿到应该缴费的阶段
            var Gotoschool = classschedu.RecursionStage(ClassGrade);
            //拿到最后一个阶段
            var co = Gotoschool.Where(a => a.CurrentStageID == geand.GetList().Where(w => w.GrandName == "Y1" && w.IsDelete == false).FirstOrDefault().Id).FirstOrDefault();
            Gotoschool.Remove(co);
            Gotoschool.Add(new GotoschoolStage { CurrentStageID = ClassGrade, NextStageID = 0, ID = 0 });

            var StudentClass = classschedu.FintClassSchedule(ClassID);
            //拿到阶段以及应缴的费用
            foreach (var item in Gotoschool)
            {
                ViewGotosch viewGotosch = new ViewGotosch();
                viewGotosch.Price = 0;
                viewGotosch.id = item.CurrentStageID;
                foreach (var item1 in costit)
                {
                    if (item.CurrentStageID == item1.Grand_id)
                    {
                        viewGotosch.Price = viewGotosch.Price + item1.Amountofmoney;
                    }
                }
                gotosches.Add(viewGotosch);
            }

            var student = classschedu.ClassStudentneList(ClassID);
            foreach (var item in student)
            {
                var fee = studentfee.GetList().Where(a => a.StudenID == item.StuNameID).Select(a => new ViewCostit { GrandID = costitemsBusiness.GetEntity(a.Costitemsid).Grand_id, Price = a.Amountofmoney, StudentID = a.StudenID }).ToList();

                foreach (var item1 in Gotoschool)
                {
                    DetailedcostView detailedcostView = new DetailedcostView();
                    detailedcostView.CurrentStageID = geand.GetEntity(item1.CurrentStageID).GrandName;
                    detailedcostView.StagesID = item1.CurrentStageID;
                    detailedcostView.Amountofmoney = 0;
                    if (fee.Count > 0)
                    {
                        foreach (var item2 in fee)
                        {
                            detailedcostView.Stidentid = item2.StudentID;
                            if (item2.GrandID == item1.CurrentStageID)
                            {
                                detailedcostView.Amountofmoney = detailedcostView.Amountofmoney + (decimal)item2.Price;
                            }
                        }
                    }
                    else
                    {
                        detailedcostView.Stidentid = item.StuNameID;
                        detailedcostView.Amountofmoney = 0;
                    }

                    listdetailedc.Add(detailedcostView);
                }
            }
            List<DetailedcostView> listdetailedcs = new List<DetailedcostView>();
            foreach (var item in gotosches)
            {
                foreach (var item1 in listdetailedc)
                {
                    if (item.id == item1.StagesID)
                    {
                        if (item1.Amountofmoney < item.Price)
                        {
                            DetailedcostView detailedcostView = this.FineDetail(ClassID, item1.Stidentid);
                            detailedcostView.Amountofmoney = item1.Amountofmoney;
                            detailedcostView.CurrentStageID = item1.CurrentStageID;
                            detailedcostView.ShouldJiao = item.Price;
                            detailedcostView.Surplus = item.Price - item1.Amountofmoney;
                            detailedcostView.StagesID = item1.StagesID;
                            listdetailedcs.Add(detailedcostView);
                        }
                    }

                }
            }
            return listdetailedcs;
        }

        private List<IGrouping<string, StudentFeeRecordListView>> list;
        /// <summary>
        ///财务查看学生欠费功能
        /// </summary>
        /// <returns></returns>
        public List<DetailedcostView> StudentArreargeList()
        {

            BaseBusiness<ScheduleForTrainees> ScheduleForTrainees = new BaseBusiness<ScheduleForTrainees>();//所有学生
            BaseBusiness<StudentInformation> stuinfomation = new BaseBusiness<StudentInformation>();
            BaseBusiness<StudentFeeRecordListView> StudentFeeRecordListView = new BaseBusiness<StudentFeeRecordListView>();//学员缴费
            List<DetailedcostView> listdetailedcs = new List<DetailedcostView>();//欠费实体类
            BaseBusiness<Grand> Grand = new BaseBusiness<Grand>();//阶段类型
            BaseBusiness<Costitems> Costitems = new BaseBusiness<Costitems>();//阶段类型详情
            var studentView =new List<StudentFeeRecordListView>();
            
            //var student = ScheduleForTrainees.GetList().Where(d => d.CurrentClass == true).ToList();
            string sql = " select * from ScheduleForTrainees where CurrentClass='1' ";
            var student = ScheduleForTrainees.GetListBySql<ScheduleForTrainees>(sql).ToList();//查询班级里所有的学生
            foreach (var item in student)
            {
                 string studentViewSQL = "select * from StudentFeeRecordListView where StudenID='" + item.StudentID + "' and Passornot='1' and AddTime is not null";//根据学生id查询出缴费数据
                 studentView = StudentFeeRecordListView.GetListBySql<StudentFeeRecordListView>(studentViewSQL).ToList();
                 list = (from s in studentView
                            where s.StageName != null//分组对象
                            group s by s.StageName//按什么分组
                            into mylist
                            select mylist).ToList();//返回对象
                foreach (var i in list)
                {
                    StringBuilder sb = new StringBuilder();
                   var stage = i.Key;
                    /*var Grandid = Grand.GetList().Where(d => d.GrandName == stage).SingleOrDefault();*/ //查询阶段费用
                    string GrandSQL = "select * from Grand where GrandName='"+stage+"'";
                    var Grandid = Grand.GetListBySql<Grand>(GrandSQL).SingleOrDefault();
                    if (Grandid == null)
                    {
                        sb.Append("select SUM(c.Amountofmoney) as 'Summonry' from Costitems  as c where c.IsDelete=0");//查询自考本科及其他费用（转换为字符串）
                    }
                    else
                    {
                        sb.Append("select SUM(c.Amountofmoney) as 'Summonry',c.Grand_id from Costitems  as c where Grand_id='" + Grandid.Id + "'and c.IsDelete=0 group by c.Grand_id ");//查询阶段总金额（转换为字符串）
                    }

                    var grand_sum = Costitems.GetListBySql<Grand_SUM>(sb.ToString());
                    decimal sum = 0;//学生每个阶段的总和
                    decimal count = 0;//欠费总和
                    foreach (var m in i)
                    {
                        sum += m.Amountofmoney;//阶段费用 
                    }
                    DetailedcostView deta_list = new DetailedcostView();
                    if (Grandid == null)
                    {
                        count = grand_sum.SingleOrDefault().Summonry - sum;//计算欠费
                        deta_list.Surplus = count;
                        deta_list.Stidentid = item.StudentID;
                        deta_list.Name = stuinfomation.GetEntity(item.StudentID).Name;
                        deta_list.ClassName = item.ClassID;
                        deta_list.CurrentStageID = stage;
                        deta_list.Amountofmoney = sum;
                        deta_list.ShouldJiao = grand_sum.SingleOrDefault().Summonry;
                    }
                    else
                    {
                        deta_list.StagesID = Grandid.Id;
                        count = grand_sum.SingleOrDefault().Summonry - sum;//计算欠费
                        deta_list.Surplus = count;
                        deta_list.Stidentid = item.StudentID;
                        deta_list.Name = stuinfomation.GetEntity(item.StudentID).Name;
                        deta_list.ClassName = item.ClassID;
                        deta_list.CurrentStageID = stage;
                        deta_list.Amountofmoney = sum;
                        deta_list.ShouldJiao = grand_sum.SingleOrDefault().Summonry;
                    }
                    listdetailedcs.Add(deta_list);
                }

            }

            return listdetailedcs;
        }
        /// <summary>
        /// 班级欠费查询
        /// </summary>
        /// <param name="ClassID">班级ID</param>
        /// <returns></returns>
        public List<DetailedcostView> TuitionFine_list(int ClassID)
        {
            BaseBusiness<ScheduleForTrainees> ScheduleForTrainees = new BaseBusiness<ScheduleForTrainees>();//所有学生
            BaseBusiness<StudentInformation> stuinfomation = new BaseBusiness<StudentInformation>();
            BaseBusiness<StudentFeeRecordView> StudentFeeRecordView = new BaseBusiness<StudentFeeRecordView>();//学员缴费
            BaseBusiness<CostitemsX> CostitemsX = new BaseBusiness<CostitemsX>();//商品类型
            List<DetailedcostView> listdetailedcs = new List<DetailedcostView>();//欠费实体类
            BaseBusiness<Grand> Grand = new BaseBusiness<Grand>();//阶段类型
            BaseBusiness<Costitems> Costitems = new BaseBusiness<Costitems>();//阶段类型详情
           /* var Class_id = classschedu.GetList().Where(d => d.id == ClassID).SingleOrDefault();*///查询班级
            string Class_idSQL = "select * from ClassSchedule where id='" + ClassID + "'";
            var Class_id = classschedu.GetListBySql<ClassSchedule>(Class_idSQL).SingleOrDefault();   
            string sql = " select * from ScheduleForTrainees where CurrentClass='1' and ClassID='"+Class_id.ClassNumber+"'";
            var student = ScheduleForTrainees.GetListBySql<ScheduleForTrainees>(sql).ToList();//根据班级查询学生
            /*var student = ScheduleForTrainees.GetList().Where(d => d.ClassID == Class_id.ClassNumber && d.CurrentClass == true).ToList();*///查询班级里所有的学生
            foreach (var item in student)
            {

                /*var studentView = StudentFeeRecordView.GetList().Where(d => d.StudenID == item.StudentID).ToList();*///所有学生的缴费情况
                string studentViewSQL = "select * from StudentFeeRecordListView where StudenID='" + item.StudentID + "'and Passornot='1' and AddTime is not null";//根据学生id查询出缴费数据select * from StudentFeeRecordListView where StudenID='" + item.StudentID + "' and Passornot='1' and AddTime is not null
                var studentView = StudentFeeRecordView.GetListBySql<StudentFeeRecordListView>(studentViewSQL).ToList();
                //分组

                var list = (from s in studentView where s.StageName != null //分组对象
                            group s by s.StageName//按什么分组
                            into mylist
                            select mylist).ToList();//返回对象

                foreach (var i in list)
                {
                    StringBuilder sb = new StringBuilder();
                    var stage = i.Key;//获取阶段
                    //var Grandid = Grand.GetList().Where(d => d.GrandName == stage).SingleOrDefault(); //查询阶段费用
                    string GrandSQL = "select * from Grand where GrandName='" + stage + "'";
                    var Grandid = Grand.GetListBySql<Grand>(GrandSQL).SingleOrDefault();
                    if (Grandid == null)
                    {
                        sb.Append("select SUM(c.Amountofmoney) as 'Summonry' from Costitems  as c where c.IsDelete=0");//查询自考本科及其他费用（转换为字符串）
                    }
                    else
                    {
                        sb.Append("select SUM(c.Amountofmoney) as 'Summonry',c.Grand_id from Costitems  as c where Grand_id='" + Grandid.Id + "'and c.IsDelete=0 group by c.Grand_id ");//查询阶段总金额（转换为字符串）
                    }

                    var grand_sum = Costitems.GetListBySql<Grand_SUM>(sb.ToString());
                    decimal sum = 0;//学生每个阶段的总和
                    decimal count = 0;//欠费总和
                    foreach (var m in i)
                    {
                        sum += m.Amountofmoney;//阶段费用 
                    }
                    DetailedcostView deta_list = new DetailedcostView();
                    if (Grandid == null)
                    {
                        count = grand_sum.SingleOrDefault().Summonry - sum;//计算欠费
                        deta_list.Surplus = count;
                        deta_list.Stidentid = item.StudentID;
                        deta_list.Name = stuinfomation.GetEntity(item.StudentID).Name;
                        deta_list.ClassName = item.ClassID;
                        deta_list.CurrentStageID = stage;
                        deta_list.Amountofmoney = sum;
                        deta_list.ShouldJiao = grand_sum.SingleOrDefault().Summonry;
                    }
                    else
                    {
                        deta_list.StagesID = Grandid.Id;
                        count = grand_sum.SingleOrDefault().Summonry - sum;//计算欠费
                        deta_list.Surplus = count;
                        deta_list.Stidentid = item.StudentID;
                        deta_list.Name = stuinfomation.GetEntity(item.StudentID).Name;
                        deta_list.ClassName = item.ClassID;
                        deta_list.CurrentStageID = stage;
                        deta_list.Amountofmoney = sum;
                        deta_list.ShouldJiao = grand_sum.SingleOrDefault().Summonry;

                    }
                    listdetailedcs.Add(deta_list);
                }

            }

            return listdetailedcs;
        }
        /// <summary>
        /// 更改商品状态
        /// </summary>
        /// <param name="id">商品id</param>
        /// <param name="Isdele">正常或者禁用</param>
        /// <returns></returns>
        public bool PaymentcommodityIsdele(int id, bool Isdele)
        {
            bool bol = true;
            try
            {
                var cost = costitemsBusiness.GetEntity(id);
                var costlist = costitemsBusiness.GetList().Where(a => a.Grand_id == cost.Grand_id && a.Name == cost.Name && a.Rategory == cost.Rategory && a.IsDelete == false).ToList();
                foreach (var item in costlist)
                {
                    item.IsDelete = true;
                }
                costitemsBusiness.Update(costlist);
                cost.IsDelete = Isdele;
                costitemsBusiness.Update(cost);
                BusHelper.WriteSysLog("修改学员学费及其它商品表", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {
                bol = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return bol;

        }

        public class CostitemViews
        {
            public int id { get; set; }
            public string studentid { get; set; }
            public string name { get; set; }
            public string IDnumber { get; set; }
            public decimal Amountofmoney { get; set; }
            public DateTime AddDate { get; set; }
            public string Passornot { get; set; }
            public string Paymentmethod { get; set; }
            //单号
            public string OddNumbers { get; set; }
        }
        /// <summary>
        /// 获取待入账数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="StudentID">学号</param>
        /// <param name="Name">姓名</param>
        /// <param name="IsaDopt">状态</param>
        /// <param name="OddNumbers">单号</param>
        /// <returns></returns> 

        public object Expenseentry(int page, int limit, string StudentID, string Name, string IsaDopt, string OddNumbers)
        {
            BaseBusiness<Feedetails> FeedetailsBusiness = new BaseBusiness<Feedetails>();
            BaseBusiness<StudentFeeRecord> studentFeeRecordslist = new BaseBusiness<StudentFeeRecord>();
            var costlist = FeedetailsBusiness.GetList();

            if (!string.IsNullOrEmpty(StudentID))
            {
                costlist = costlist.Where(a => a.studentid.Contains(StudentID)).ToList();

            }
            if (!string.IsNullOrEmpty(Name))
            {
                costlist = costlist.Where(a => a.name.Contains(Name)).ToList();
            }
            if (!string.IsNullOrEmpty(IsaDopt))
            {
                if (IsaDopt == "null")
                {
                    costlist = costlist.Where(a => a.Passornot == null).ToList();
                }
                else { costlist = costlist.Where(a => a.Passornot == IsaDopt).ToList(); }
            }
            if (!string.IsNullOrEmpty(OddNumbers))
            {
                costlist = costlist.Where(a => a.OddNumbers == OddNumbers).ToList();
            }
            var dataList = costlist.OrderByDescending(a => a.id).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = costlist.Count,
                data = dataList,
            };
            return data;
        }
        /// <summary>
        /// 审批页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<vierprice> FienPricesa(int id)
        {
            List<object> students = new List<object>();
            var pay = PayviewPaymentverBusiness.GetList().Where(z => z.Paymentver == id).ToList();
            List<Payview> st = new List<Payview>();
            foreach (var item in pay)
            {
                var st1 = PayviewBusiness.GetEntity(item.Payviewid);
                st.Add(st1);
                students.Add(Convert.ToDateTime(st1.AddDate).ToLongDateString().ToString());
            }

            var x = students.Distinct().ToList();
            List<vierprice> listvier = new List<vierprice>();
            // string date = Convert.ToDateTime(item.AddDate).ToLongDateString().ToString();
            foreach (var item1 in x)
            {
                vierprice vierprices = new vierprice();
                vierprices.Date = item1.ToString();
                List<vierprice> view = new List<vierprice>();
                foreach (var item in st)
                {

                    string date = Convert.ToDateTime(item.AddDate).ToLongDateString().ToString();
                    if (item1.ToString() == date)
                    {
                        var costit = costitemsBusiness.GetEntity(item.Costitemsid);
                        vierprice stivier = new vierprice();
                        stivier.Amountofmoney = (decimal)item.Amountofmoney;
                        stivier.CostitemName = costit.Name;
                        stivier.GrandName = costit.Grand_id != null ? geand.GetEntity(costit.Grand_id).GrandName : "";
                        stivier.Rategory = costitemssX.GetEntity(costit.Rategory).Name;
                        view.Add(stivier);
                    }
                }
                vierprices.Chicked = view;
                listvier.Add(vierprices);
            }
            return listvier;
        }
        //备案业务类
        StudentDataKeepAndRecordBusiness studentDataKeepAndRecordBusiness = new StudentDataKeepAndRecordBusiness();
        //撤销入账业务类
        BaseBusiness<Cancelreceipt> CancelreceiptBusiness = new BaseBusiness<Cancelreceipt>();
        /// <summary>
        /// 审核入账是否成功
        /// </summary>
        /// <param name="id">核对缴费是否成功编号</param>
        /// <param name="whether">是否入账</param>
        /// <param name="OddNumbers">单号</param>
        /// <param name="paymentmethod">收款方式</param>
        /// <returns></returns>
        public AjaxResult Tuitionentry(int id, string whether, string OddNumbers, string paymentmethod, DateTime? time)
        {
            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "系统故障，请联系开发人员";

                if (whether == "1")
                {

                    var x = PaymentverificationBusiness.GetEntity(id);
                    x.Passornot = "1";
                    x.OddNumbers = OddNumbers;
                    x.Paymentmethod = paymentmethod;
                    x.AddDate = DateTime.Now;
                    PaymentverificationBusiness.Update(x);
                    var pay = PayviewPaymentverBusiness.GetList().Where(a => a.Paymentver == id).ToList();
                    List<StudentFeeRecord> studentFeeRecordslist = new List<StudentFeeRecord>();
                    //BaseBusiness<Feedetails> FeedetailsBusiness_list = new BaseBusiness<Feedetails>();
                    foreach (var item in pay)
                    {
                        var view = PayviewBusiness.GetEntity(item.Payviewid);
                        StudentFeeRecord studentFeeRecord = new StudentFeeRecord();
                        studentFeeRecord.AddDate = view.AddDate;
                        studentFeeRecord.AddTime = time;
                        studentFeeRecord.Amountofmoney = view.Amountofmoney;
                        studentFeeRecord.Costitemsid = view.Costitemsid;
                        studentFeeRecord.FinanceModelid = view.FinanceModelid;
                        studentFeeRecord.IsDelete = view.IsDelete;
                        studentFeeRecord.Remarks = view.Remarks;
                        studentFeeRecord.StudenID = view.StudenID;

                        studentFeeRecordslist.Add(studentFeeRecord);
                    }

                    var x2 = Preentryfeebusenn.GetList().Where(a => a.keeponrecordid == studentInformationBusiness.GetEntity(PayviewBusiness.GetEntity(pay[0].Payviewid).StudenID).StudentPutOnRecord_Id && a.Refundornot == null).ToList();
                    if (x2.Count > 0)
                    {
                        foreach (var item in x2)
                        {
                            item.Refundornot = true;

                        }
                        Preentryfeebusenn.Update(x2);
                    }
                    studentfee.Insert(studentFeeRecordslist);
                    if (studentFeeRecordslist.Count < 2)
                    {
                        if (costitemssX.GetEntity(costitemsBusiness.GetEntity(studentFeeRecordslist[0].Costitemsid).Rategory).Name == "自考本科费用")
                        {
                            var FeeRecords = studentFeeRecordslist[0];
                            Enrollment enrollment = new Enrollment();
                            enrollment.StudentNumber = FeeRecords.StudenID;
                            enrollment.IsDelete = false;
                            enrollmentBusiness.AddEnro(enrollment, FeeRecords.Costitemsid);
                        }

                    }
                    var xz = studentInformationBusiness.GetEntity(studentFeeRecordslist[0].StudenID).StudentPutOnRecord_Id;
                    studentDataKeepAndRecordBusiness.ChangeStudentState((int)xz);
                    List<Cancelreceipt> Cancelreceiptlist = new List<Cancelreceipt>();
                    foreach (var item in studentFeeRecordslist)
                    {
                        Cancelreceipt cancelreceipt = new Cancelreceipt();
                        cancelreceipt.FeeRecordid = studentfee.GetList().Where(a => a.AddDate == item.AddDate && a.Amountofmoney == item.Amountofmoney && a.Costitemsid == item.Costitemsid && a.FinanceModelid == item.FinanceModelid && a.StudenID == item.StudenID).OrderByDescending(a => a.ID).FirstOrDefault().ID;
                        cancelreceipt.Paymentverid = x.id;
                        Cancelreceiptlist.Add(cancelreceipt);
                    }
                    CancelreceiptBusiness.Insert(Cancelreceiptlist);
                    var proes = Preentryfeebusenn.GetList().Where(a => a.keeponrecordid == xz && a.IsDit == false && a.Refundornot == null).ToList();
                    foreach (var item in proes)
                    {
                        item.Refundornot = true;
                    }
                    Preentryfeebusenn.Update(proes);
                    retus.Msg = "入账成功";



                }


                else if (whether == "2")
                {
                    var x = PaymentverificationBusiness.GetEntity(id);
                    //var r = StudentFeeRecord_list_add.GetList().Where(d=>d.ID==id).SingleOrDefault();
                    x.Passornot = "2";
                    x.AddDate = DateTime.Now;
                    x.OddNumbers = OddNumbers;
                    x.Paymentmethod = paymentmethod;
                        //r.IsDelete = true;
                        //StudentFeeRecord_list_add.Update(r);

                    PaymentverificationBusiness.Update(x);
                    retus.Msg = "作废成功";
                }
                else
                {
                    List<StudentFeeRecord> studentFeeRecords = new List<StudentFeeRecord>();
                    var canList = CancelreceiptBusiness.GetList().Where(a => a.Paymentverid == id).ToList();
                    if (canList.Count > 0)
                    {
                        foreach (var item in canList)
                        {
                            studentFeeRecords.Add(studentfee.GetEntity(item.FeeRecordid));
                        }
                        studentfee.Delete(studentFeeRecords);
                        var x = PaymentverificationBusiness.GetEntity(id);
                        x.Passornot = "3";
                        PaymentverificationBusiness.Update(x);
                        retus.Msg = "撤单成功";
                    }
                }

                BusHelper.WriteSysLog("审核缴费数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }
        //预入费业务类
        BaseBusiness<Preentryfee> Preentryfeebusenn = new BaseBusiness<Preentryfee>();

        /// <summary>
        /// 预入费缴纳
        /// </summary>
        /// <param name="preentryfee">数据对象</param>
        /// <returns></returns>
        public object PaytheadvancefeeAdd(Preentryfee preentryfee)
        {
            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            var fine = finacemo.GetList().Where(a => a.Financialstaff == user.EmpNumber).FirstOrDefault();
            AjaxResult retus = null;
            try
            {

                preentryfee.AddDate = DateTime.Now;
                preentryfee.IsDit = false;
                preentryfee.FinanceModelid = fine.id;
                preentryfee.Reamk = preentryfee.Reamk;
                var classid = preentryfee.ClassID.Split(',');
                Payview studentFee = new Payview();
                studentFee.StudenID = preentryfee.keeponrecordid.ToString() + "," + preentryfee.ClassID;
                preentryfee.ClassID = classid[0];
                Preentryfeebusenn.Insert(preentryfee);
                StudentDataKeepAndRecordBusiness studentDataKeepAndRecordBusiness = new StudentDataKeepAndRecordBusiness();
                studentDataKeepAndRecordBusiness.ChangeStudentState(preentryfee.keeponrecordid);
                List<Payview> liststudents = new List<Payview>();

                studentFee.Amountofmoney = preentryfee.Amountofmoney;
                studentFee.AddDate = DateTime.Now;
                studentFee.Remarks = preentryfee.identitydocument;
                studentFee.IsDelete = false;
                studentFee.Costitemsid = 0;
                studentFee.FinanceModelid = fine.id;

                liststudents.Add(studentFee);
                SessionHelper.Session["person"] = liststudents;
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "缴费成功";
                BusHelper.WriteSysLog("审核缴费数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }

        public class Preeviews
        {
            public int id { get; set; }
            public string StuName { get; set; }
            public string Stuphone { get; set; }
            public string StuSex { get; set; }
            public string empName { get; set; }
            public string StuEntering { get; set; }
            public bool? Refundornot { get; set; }
            public decimal Amountofmoney { get; set; }
            public string identitydocument { get; set; }
            public string ClassNumber { get; set; }
            public string OddNumbers { get; set; }
            public DateTime AddDate { get; set; }
        }
        /// <summary>
        /// 获取已缴预入费数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public object PreentryfeeDates(int page, int limit, string StuName, string identitydocument, string qBeginTime, string qEndTime)
        {
            var preenlist = Preentryfeebusenn.GetList().Where(A => A.IsDit == false).ToList();
            List<Preeviews> list = new List<Preeviews>();
            foreach (var item in preenlist)
            {
                var Keep = studentDataKeepAndRecordBusiness.findId(item.keeponrecordid.ToString());
                var x = new Preeviews()
                {
                    id = item.id,
                    StuName = Keep.StuName,
                    Stuphone = Keep.Stuphone,
                    StuSex = Keep.StuSex,
                    empName = Keep.empName,
                    StuEntering = Keep.StuEntering,
                    Refundornot = item.Refundornot,
                    Amountofmoney = Convert.ToDecimal(item.Amountofmoney),
                    identitydocument = item.identitydocument,
                    ClassNumber = item.ClassID,
                    OddNumbers = item.OddNumbers == null ? "请补录" : item.OddNumbers,
                    AddDate = item.AddDate

                };
                list.Add(x);
            }
            if (!string.IsNullOrEmpty(StuName))
            {
                list = list.Where(a => a.StuName == StuName).ToList();
            }
            if (!string.IsNullOrEmpty(identitydocument))
            {
                list = list.Where(a => a.identitydocument == identitydocument).ToList();
            }
            if (!string.IsNullOrEmpty(qBeginTime))
            {
                DateTime time = Convert.ToDateTime(qBeginTime);
                list = list.Where(a => Convert.ToDateTime(a.AddDate).ToShortDateString().ToDateTime() >= time).ToList();
            }
            if (!string.IsNullOrEmpty(qEndTime))
            {
                DateTime time = Convert.ToDateTime(qEndTime);
                list = list.Where(a => Convert.ToDateTime(a.AddDate).ToShortDateString().ToDateTime() <= time).ToList();
            }

            var dataList = list.OrderBy(a => a.id).Skip((page - 1) * limit).Take(limit).ToList();
            //  var x = dbtext.GetList();
            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = dataList
            };
            return data;
        }
        /// <summary>
        /// 根据学号获取预入费
        /// </summary>
        /// <param name="id">学号</param>
        /// <returns></returns>
        public List<vierprice> StudentPrentryfeeDate(string studentid)
        {

            List<vierprice> vierpriceslist = new List<vierprice>();
            var x = Preentryfeebusenn.GetList().Where(a => a.IsDit == false && a.OddNumbers != null && a.keeponrecordid == studentInformationBusiness.GetEntity(studentid).StudentPutOnRecord_Id).ToList();
            var date = x.Select(a => a.AddDate).Distinct().ToList();
            foreach (var item in date)
            {
                vierprice vierprice = new vierprice();
                vierprice.Date = Convert.ToDateTime(item.ToString()).ToLongDateString().ToString();
                foreach (var item1 in x)
                {
                    if (item == item1.AddDate)
                    {
                        vierprice vierprice1 = new vierprice();
                        vierprice1.CostitemName = "预入费";
                        vierprice1.Amountofmoney = Convert.ToDecimal(item1.Amountofmoney);
                        vierprice1.GrandName = item1.ClassID;
                        vierprice1.Rategory = item1.Refundornot.ToString();
                        vierprice.Chicked.Add(vierprice1);
                    }
                }
                vierpriceslist.Add(vierprice);
            }
            return vierpriceslist;

        }
        /// <summary>
        /// 预入费退费
        /// </summary>
        /// <param name="refund">数据对象</param>
        /// <returns></returns>
        public object Preentryfeerefund(Refund refund)
        {
            AjaxResult retus = null;
            try
            {
                retus = new SuccessResult();
                var x = Preentryfeebusenn.GetEntity(refund.Preentid);
                if (x.Refundornot == null)
                {
                    x.Refundornot = false;
                    refund.AddDate = DateTime.Now;
                    Preentryfeebusenn.Update(x);
                    RefundBusiness.Insert(refund);
                    retus.Msg = "退费成功";
                }
                else
                {
                    retus.Msg = "该学员已报名请勿退费";
                }


                retus.Success = true;
                BusHelper.WriteSysLog("退预入费成功", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }
        /// <summary>
        /// 预入费作废
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        public object Preentryfezuofei(int id)
        {
            AjaxResult retus = null;
            try
            {
                var x = Preentryfeebusenn.GetEntity(id);
                retus = new SuccessResult();
                retus.Success = true;
                if (x.Refundornot == null)
                {
                    x.IsDit = true;
                    Preentryfeebusenn.Update(x);
                    retus.Msg = "操作成功";
                }
                else
                {
                    retus.Msg = "该学员已报名，禁止作废！";
                }
                BusHelper.WriteSysLog("退预入费作废", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }

        public class viewTuitionre
        {

            public decimal Amountofmoney { get; set; }
            public int id { get; set; }
            public string Name { get; set; }
            public int Grand_id { get; set; }
        }

        public object TuitionreStage(string StuidetID, int Grand_id)
        {
            var obj = new List<viewTuitionre>();
            var obj1 = new List<viewTuitionre>();
            var costitemslist = costitemsBusiness.GetList().Where(a => a.Grand_id == Grand_id).ToList();
            var studentfeelist = studentfee.GetList().Where(a => a.StudenID == StuidetID).ToList();
            foreach (var item in studentfeelist)
            {

            }

            foreach (var item in studentfeelist)
            {
                if (TuitionrefundBusiness.GetList().Where(a => a.StudentFeeRecordId == item.ID).FirstOrDefault() == null)
                {
                    foreach (var item1 in costitemslist)
                    {
                        if (item1.id == item.Costitemsid)
                        {
                            viewTuitionre viewTuitionre = new viewTuitionre();
                            viewTuitionre.id = item1.id;
                            viewTuitionre.Amountofmoney = (decimal)item.Amountofmoney;
                            viewTuitionre.Name = item1.Name;
                            viewTuitionre.Grand_id = (int)item1.Grand_id;
                            obj.Add(viewTuitionre);
                        }
                    }
                }

            }

            foreach (var item in obj)
            {
                obj1.Add(item);
                var x = obj1.Where(a => a.id == item.id).ToList();
                if (x.Count > 1)
                {
                    viewTuitionre viewTuitionre = new viewTuitionre();

                    foreach (var item1 in x)
                    {
                        viewTuitionre.id = item1.id;
                        viewTuitionre.Grand_id = item1.Grand_id;
                        viewTuitionre.Name = item1.Name;
                        viewTuitionre.Amountofmoney = viewTuitionre.Amountofmoney + item1.Amountofmoney;
                        obj1.Remove(item1);
                    }
                    obj1.Add(viewTuitionre);

                }
            }
            return obj1;
        }
        List<Tuitionrefund> tuitionrefundsz = new List<Tuitionrefund>();
        /// <summary>
        /// 阶段费用退款
        /// </summary>
        /// <param name="tuitionrefunds"></param>
        /// <returns></returns>
        public object TuitionreHomes(List<TuitionrefundView> tuitionrefunds)
        {
            AjaxResult retus = null;
            foreach (var item in tuitionrefunds)
            {
                var studentfeelist = studentfee.GetList().Where(a => a.StudenID == item.StudentID && a.Costitemsid == item.StudentFeeRecordId&&a.AddTime!=null).ToList();
                if (studentfeelist.Count >= 1)
                {
                    TuitionrefundAdds(studentfeelist, item.Amountofmoney);
                }
            }

            var mylist = new List<Tuitionrefund>();
            foreach (var item in tuitionrefundsz)
            {
                mylist.Add(item);
                var x = mylist.Where(a => a.StudentFeeRecordId == item.StudentFeeRecordId).ToList();
                if (x.Count > 1)
                {
                    Tuitionrefund viewTuitionre = new Tuitionrefund();

                    foreach (var item1 in x)
                    {
                        viewTuitionre.id = item1.id;
                        viewTuitionre.StudentFeeRecordId = item1.StudentFeeRecordId;
                        viewTuitionre.AddDate = item1.AddDate;
                        viewTuitionre.Amountofmoney = item1.Amountofmoney;

                        viewTuitionre.Empnumber = user.EmpNumber;
                        mylist.Remove(item1);
                    }
                    mylist.Add(viewTuitionre);

                }


            }
            try
            {
                TuitionrefundBusiness.Insert(mylist);
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "学费退费成功";
                BusHelper.WriteSysLog("退费添加数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }

        //当前登陆人
        Base_UserModel user = Base_UserBusiness.GetCurrentUser();


        /// <summary>
        /// 拿到退费名目项目
        /// </summary>
        /// <param name="studentFeeRecords"></param>
        /// <param name="Amountofmoney"></param>
        public void TuitionrefundAdds(List<StudentFeeRecord> studentFeeRecords, decimal Amountofmoney)
        {
            foreach (var item in studentFeeRecords)
            {
                Tuitionrefund tuitionrefund = new Tuitionrefund();
                //if (item.Amountofmoney - Amountofmoney >= 0)
                //{
                    tuitionrefund.Amountofmoney = Amountofmoney;
                    tuitionrefund.StudentFeeRecordId = item.ID;
                    tuitionrefund.AddDate = DateTime.Now;
                    tuitionrefund.Empnumber = user.EmpNumber;
                    tuitionrefundsz.Add(tuitionrefund);
                //}
                //else
                //{

                //    TuitionrefundAddd(item, Amountofmoney);
                //}
            }
        }
        /// <summary>
        /// 递归循环拿到退费名目
        /// </summary>
        /// <param name="studentFeeRecords"></param>
        /// <param name="Amountofmoney"></param>
        public void TuitionrefundAddd(StudentFeeRecord studentFeeRecords, decimal Amountofmoney)
        {
            Tuitionrefund tuitionrefund = new Tuitionrefund();
            var Amountofmoneys = studentFeeRecords.Amountofmoney - Amountofmoney;
            tuitionrefund.Amountofmoney = (decimal)studentFeeRecords.Amountofmoney;
            tuitionrefund.StudentFeeRecordId = studentFeeRecords.ID;
            tuitionrefund.AddDate = DateTime.Now;
            tuitionrefund.Empnumber = user.EmpNumber;
            tuitionrefundsz.Add(tuitionrefund);
            if (Amountofmoneys < 0)
            {
                if (Math.Abs(Convert.ToInt32(Amountofmoneys)) > 0)
                {
                    var x = studentfee.GetList().Where(a => a.StudenID == studentFeeRecords.StudenID && a.Costitemsid == studentFeeRecords.Costitemsid && a.ID != studentFeeRecords.ID).ToList();

                    foreach (var item in x)
                    {

                        TuitionrefundAddd(item, Convert.ToInt32(Amountofmoneys));
                    }

                }
            }

        }

        public class StudentRefunditems
        {
            public string Name { get; set; }
            public string StudentId { get; set; }
            public string identitydocument { get; set; }
            public string Telephone { get; set; }
            public bool Sex { get; set; }
            public decimal Amountofmoney { get; set; }
        }
        /// <summary>
        /// 获取所有退费数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public object Refunditems(int page, int limit)
        {
            BaseBusiness<Refunditemsview> RefunditemsviewBusiness = new BaseBusiness<Refunditemsview>();

            List<StudentRefunditems> StudentRefunditemslist = new List<StudentRefunditems>();
            var x = RefunditemsviewBusiness.GetList();


            foreach (var item1 in x.Select(a => a.StudenID).Distinct().ToList())
            {
                StudentRefunditems studentRefunditems = new StudentRefunditems();
                var student = studentInformationBusiness.GetEntity(item1);
                studentRefunditems.StudentId = item1;
                studentRefunditems.Name = student.Name;
                studentRefunditems.Telephone = student.Telephone;
                studentRefunditems.Sex = student.Sex;
                studentRefunditems.identitydocument = student.identitydocument;
                foreach (var item in x)
                {
                    if (item1 == item.StudenID)
                    {
                        studentRefunditems.Amountofmoney = studentRefunditems.Amountofmoney + item.Amountofmoney;

                    }

                }
                StudentRefunditemslist.Add(studentRefunditems);
            }
            var dataList = StudentRefunditemslist.OrderBy(a => a.StudentId).Skip((page - 1) * limit).Take(limit).ToList();
            //  var x = dbtext.GetList();
            var data = new
            {
                code = "",
                msg = "",
                count = StudentRefunditemslist.Count,
                data = dataList
            };
            return data;
        }
        /// <summary>
        /// 获取已交的预入费总额且使用
        /// </summary>
        /// <param name="Studentid">学号</param>
        /// <returns></returns>
        public decimal PreentryfeeFinet(string Studentid)
        {
            var x = Preentryfeebusenn.GetList().Where(a => a.keeponrecordid == studentInformationBusiness.GetEntity(Studentid).StudentPutOnRecord_Id && a.IsDit == false && a.Refundornot == null).ToList();
            StudentRefunditems studentRefunditems = new StudentRefunditems();
            foreach (var item in x)
            {
                studentRefunditems.Amountofmoney = studentRefunditems.Amountofmoney + Convert.ToDecimal(item.Amountofmoney);
            }
            return studentRefunditems.Amountofmoney;

        }
        /// <summary>
        /// 通过备案id获取身份证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Identityid(int id)
        {
            var x = Preentryfeebusenn.GetList().Where(a => a.keeponrecordid == id).FirstOrDefault();
            if (x != null)
            {
                return x.identitydocument;
            }
            return "";
        }
        /// <summary>
        /// 通过预入费id去补录单号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OddNumbers">单号</param>
        /// <param name="type">为1则id为备案id，否则为预入费id</param>
        /// <returns></returns>
        public object ReentryfeeOddNumbers(int id, string OddNumbers, int type)
        {
            AjaxResult retus = null;
            try
            {
                if (type == 1)
                {
                    id = Preentryfeebusenn.GetList().Where(a => a.keeponrecordid == id).OrderByDescending(a => a.id).FirstOrDefault().id;
                }
                var preentryfee = Preentryfeebusenn.GetEntity(id);
                preentryfee.OddNumbers = OddNumbers;
                Preentryfeebusenn.Update(preentryfee);
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "单号录入成功";
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return retus;
        }
    }
}
