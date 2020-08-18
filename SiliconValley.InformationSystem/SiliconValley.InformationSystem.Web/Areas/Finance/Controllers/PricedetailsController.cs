using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business.FinaceBusines;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Business;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.EnrollmentBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Base_SysManage;

namespace SiliconValley.InformationSystem.Web.Areas.Finance.Controllers
{
   
    [CheckLogin]
    public class PricedetailsController : Controller
    {

        private readonly StudentFeeStandardBusinsess dbtext;
        public PricedetailsController()
        {
         
               dbtext = new StudentFeeStandardBusinsess();
        }
        // GET: Teachingquality/Pricedetails
        //专业
        SpecialtyBusiness Techarcontext = new SpecialtyBusiness();
        //阶段
        GrandBusiness Grandcontext = new GrandBusiness();
        //明目类型
        BaseBusiness<CostitemsX> costitemssX = new BaseBusiness<CostitemsX>();

        //学员费用明目
        CostitemsBusiness costitemsBusiness = new CostitemsBusiness();
        //预入费业务类
        BaseBusiness<Preentryfee> Preentryfeebusenn = new BaseBusiness<Preentryfee>();
        //本科管理
        EnrollmentBusinesse enrollmentBusinesse = new EnrollmentBusinesse();
        //学员费用明目页面
        public ActionResult Studentfees()
        {
            //阶段
            ViewBag.grade_Id = Grandcontext.GetList().Select(a => new SelectListItem { Text = a.GrandName, Value = a.Id.ToString() });
            //商品类别
            ViewBag.Typex = costitemssX.GetList().Where(a=>a.IsDelete==false&&a.id!= costitemssX.GetList().Where(z => z.Name == "其它" && z.IsDelete == false).FirstOrDefault().id).Select(a => new SelectListItem { Text = a.Name, Value = a.id.ToString() });
            return View();
        }
    
        //所有学生费用录入明目
        [HttpGet]
        public ActionResult Costitems()
        {
            //阶段
            ViewBag.Grand_id = Grandcontext.GetList().Select(a => new SelectListItem { Text = a.GrandName, Value = a.Id.ToString() });
            ViewBag.TypeDate = costitemsBusiness.TypeDate();
            return View();
        }
        [HttpPost]
        //学生费用名目录入数据操作
        public ActionResult Costitems(Costitems costitems)
        {
            return Json(costitemsBusiness.AddCostitems(costitems), JsonRequestBehavior.AllowGet);
        }
            //查询名目名称是否重复
        public int costiBoolName(string id)
        {
            int ids = Convert.ToInt32(id == "undefined" ? null : id);
            string Name = Request.QueryString["Name"];
          return  costitemsBusiness.BoolName(ids, Name);
        }
        //查询所有名目数据
        public ActionResult DateCostitems(int page,int limit,string grade_Id,string Typex)
        {
            return Json(costitemsBusiness.DateCostitems(page, limit,grade_Id,Typex), JsonRequestBehavior.AllowGet);
        }

        //学费明目类型
        [HttpGet]
        public ActionResult Typeeyesight()
        {
            return View();
        }
        //查看类别名称是否重复
        [HttpPost]
        public int TypeName(string id)
        {
            return costitemsBusiness.TypeName(id);
        }
        //添加明目类型数据操作
        public ActionResult Typeeyesight(CostitemsX costitemsX)
        {
            return Json(costitemsBusiness.AddType(costitemsX), JsonRequestBehavior.AllowGet);
        }
        //验证明目表是否有重复的数据
        public int BoolCostitems(Costitems costitems)
        {
          return  costitemsBusiness.BoolCostitems(costitems);
        }
        //学员缴费操作页面
        public ActionResult Studentpayment()
        {
         
            return View();
        }
        //获取学员信息
        public ActionResult GetDate(int page, int limit, string Name, string Sex, string StudentNumber, string identitydocument)
        {
            return Json(dbtext.GetDate(page, limit, Name, Sex, StudentNumber, identitydocument),JsonRequestBehavior.AllowGet);
        }
       
        //学员学费数据加载
        public ActionResult Singlecostitems(int? Grand_id, int TypeID)
        {
            var data = dbtext.Singlecostitems(Grand_id, TypeID);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
      

        //自考本科费用表单录入
        public ActionResult Undergraduatefee(string id)
        {
            //学号
            ViewBag.Stuid = id;
            // ViewBag.Costitemsid
            int Typeid = int.Parse(Request.QueryString["Typeid"]);
            //明目类型id
            ViewBag.Typeid = Typeid;
            //名目名称
            ViewBag.Costitemsid= enrollmentBusinesse.Costlist(id,Typeid).Select(a => new SelectListItem { Text = a.Name, Value = a.id.ToString() }).ToList();
            return View();
        }
        //验证本科费用是否交齐
        public ActionResult BoolEnroll(string id)
        {
            int Typeid = int.Parse(Request.QueryString["Typeid"]);
            return Json(enrollmentBusinesse.Costlist(id, Typeid).Count(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        //获取没有阶段的明目费用
        public ActionResult Otherexpensese(string Costitemsid)
        {
            return Json( dbtext.Otherexpenses(Costitemsid),JsonRequestBehavior.AllowGet);
        }
       
        //接收自考本科表单数据
        [HttpPost]
        public ActionResult Tuitionandfees(string StudenID,string Remarks,string Costitemsid)
        {
          
      
            return Json(dbtext.Tuitionandfees(StudenID, Remarks, Costitemsid),JsonRequestBehavior.AllowGet);
        }
        //学员缴费页面
        [HttpGet]
        public ActionResult StudentPrice(string id)
        {
            decimal Amountofmoney = 0;
            //学员信息
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
            var Student = dbtext.StudentFind(id);
            //阶段
            ViewBag.Stage = Grandcontext.GetList().Where(a => a.IsDelete == false).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.GrandName }).ToList();
            ViewBag.student = JsonConvert.SerializeObject(Student);
            var Preentry= Preentryfeebusenn.GetList().Where(a => a.identitydocument == studentInformationBusiness.GetEntity(id).identitydocument && a.Refundornot == null).ToList();

            ViewBag.Amountofmoney = dbtext.PreentryfeeFinet(id);


            return View();
        }
        [HttpPost]
        //学员缴费数据操作
        public ActionResult StudentPrices(string person, string Remarks,int? Stage)
        {
            SessionHelper.Session["Stage"] = Stage;
            //引入序列化
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //序列化
            var list = serializer.Deserialize<List<StudentFeeRecord>>(person);
            return Json(dbtext.StudentPrices(list, Remarks), JsonRequestBehavior.AllowGet);
        }
            //树形菜单明目类别
        public ActionResult Tree()
        {     
            var list = costitemssX.GetList().Where(a => a.IsDelete == false).ToList();
            List<TreeClass> listtree = new List<TreeClass>();
            foreach (var item in list)
            {
                if (item.Name== "自考本科费用"|| item.Name == "其它")
                {
                    TreeClass seclass = new TreeClass();
                    seclass.title = item.Name;
                    seclass.id = item.id.ToString();
                    listtree.Add(seclass);
                }
                else if (item.Name == "驾校费")
                {
                    TreeClass seclass = new TreeClass();
                    seclass.title = item.Name;
                    seclass.id = item.id.ToString();
                    listtree.Add(seclass);
                }
            }
            TreeClass saea = new TreeClass();
            saea.title = "阶段费用缴纳";
            saea.id = "";
            listtree.Add(saea);
          


            return Json(listtree, JsonRequestBehavior.AllowGet);
        }

        //学员费用缴纳查询阶段费用
        public ActionResult Studentfeepayment(int Grand_id,string studentid)
        {
            return Json(dbtext.Studentfeepayment(Grand_id, studentid), JsonRequestBehavior.AllowGet);
         
        }
       /// <summary>
       /// 验证当前是否有功能操作权限
       /// </summary>
       /// <returns></returns>
        public ActionResult IsFinanc()
        {
            return Json(dbtext.IsFinanc(), JsonRequestBehavior.AllowGet);
        }

        //费用收据发票数据
        public ActionResult Receipt()
        {
            //学员费用
            BaseBusiness<Payview> studentfee = new BaseBusiness<Payview>();
            var personlist = SessionHelper.Session["person"] as List<Payview>;
     
            if (personlist[0].Costitemsid>0)
            {
                //var Amonet = dbtext.PreentryfeeFinet(personlist[0].StudenID);
                //if (Amonet > 0)
                //{
                //    foreach (var item in personlist)
                //    {
                //        var costit = costitemsBusiness.GetEntity(item.Costitemsid);
                //        if (costit.Rategory == 8)
                //        {
                //            item.Amountofmoney = item.Amountofmoney - Amonet;
                //        }
                //    }
                //}
                string Invoicenumber = "";

                ViewBag.Invoicenumber = Invoicenumber;
                // 引入序列化
                //JavaScriptSerializer serializer = new JavaScriptSerializer();
                // string person = Request.QueryString["person"];
                //序列化
                // var  personlist = serializer.Deserialize<List<StudentFeeRecord>>(person);

                ViewBag.student = JsonConvert.SerializeObject(dbtext.StudentFind(personlist.FirstOrDefault().StudenID));
                ViewBag.Receiptdata = JsonConvert.SerializeObject(dbtext.Receiptdata(personlist));
            }
            else
            {
                var GrandName = "";
                var stuid = personlist[0].StudenID.Split(',');
              
                    GrandName = stuid[2];
            
                //班级业务类
                ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
                var stu = stuDataKeepAndRecordBusiness.findId(stuid[0]);
            
                var student = new
                {
                    Name = stu.StuName,
                    identitydocument = personlist[0].Remarks,
                    classa = stuid[1],
                    GrandName = GrandName

                };
                List<object> objlist = new List<object>();
                
                var obj = new
                {

                    personlist[0].Amountofmoney,
                    ostitemsName = "预录学费",
                    GrandName = GrandName,
                    Rategory = "预入费",
                    Remarks = "",
                    personlist[0].AddDate
                };
                objlist.Add(obj);
             ViewBag.student= JsonConvert.SerializeObject(student);
                ViewBag.Receiptdata = JsonConvert.SerializeObject(objlist);
            }
           
            //ViewBag.Remarks = personlist.FirstOrDefault().Remarks;
            return View();
        }
        //查看缴费记录
        public ActionResult Printrecord()
        {
            string student = Request.QueryString["student"];
            ViewBag.vier = dbtext.FienPrice(student);//查询缴费记录
            ViewBag.Tuitionrefund = dbtext.FienTuitionrefund(dbtext.FienPrice(student));
            ViewBag.StudentPrentryfeeDate = dbtext.StudentPrentryfeeDate(student);
            return View();
        }
        [HttpGet]
        public ActionResult Otherexpenses(string id)
        {
            ViewBag.Typeid = Request.QueryString["Typeid"];
            //学号
            ViewBag.Stuid = id;
            return View();
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
        public ActionResult Otherconsumption(string StudenID, string Consumptionname, decimal Amountofmoney, string Remarks, int Typeid)
        {
            return Json(dbtext.Otherconsumption(StudenID, Consumptionname, Amountofmoney, Remarks, Typeid), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 费用报表
        /// </summary>
        /// <returns></returns>
        public ActionResult FeeReport()
        {
            ViewBag.TypeID = costitemssX.GetList().Where(a=>a.id!=10).Select(a => new SelectListItem { Text = a.Name, Value = a.id.ToString() });
           // 
              var cost = costitemsBusiness.GetList().Where(a => a.IsDelete == false && a.Rategory == 10).Select(a => a.Name).Distinct().ToList();
            cost.Add("全部学杂");
            ViewBag.CostitemsName = cost.Select(a => new SelectListItem { Text = a, Value = a });

            return View();
        }
        /// <summary>
        /// 获取缴费名目
        /// </summary>
        /// <param name="StudentID">学号</param>
        /// <param name="Name">姓名</param>
        /// <param name="TypeID">类型</param>
        /// <param name="qBeginTime">开始时间</param>
        /// <param name="qEndTime">结束时间</param>
        /// <returns></returns>
        public ActionResult Nominaldata(int page, int limit, string StudentID, string Name, string TypeID, string qBeginTime, string qEndTime,string CostitemsName)
        {
            var x = dbtext.Nominaldata(StudentID, Name, TypeID, qBeginTime, qEndTime, CostitemsName);
            var dataList = x.OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = x.Count,
                data = dataList
            };
            return Json(data, JsonRequestBehavior.AllowGet);
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
        public ActionResult DateTatal(string StudentID, string Name, string TypeID, string qBeginTime, string qEndTime,string CostitemsName)
        {
            return Json(dbtext.DateTatal(StudentID, Name, TypeID, qBeginTime, qEndTime, CostitemsName), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 更改商品状态
        /// </summary>
        /// <param name="id">商品id</param>
        /// <param name="Isdele">正常或者禁用</param>
        /// <returns></returns>
        public ActionResult PaymentcommodityIsdele(int id, bool Isdele)
        {
            return Json(dbtext.PaymentcommodityIsdele(id, Isdele), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 费用入账
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Expenseentry()
        {
            return View();
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
        public ActionResult Expenseentrys(int page, int limit,string StudentID,string Name,string IsaDopt,string OddNumbers)
        {
            return Json(dbtext.Expenseentry(page, limit, StudentID,Name,IsaDopt,OddNumbers), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 审核是否入账页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Entryreview(int id)
        {
            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            EmployeesInfoManage employeesInfoManage = new EmployeesInfoManage();
            //学员信息
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
            string studentid = Request.QueryString["studentid"];
            ViewBag.student = studentInformationBusiness.GetEntity(studentid);
            ViewBag.id = id;
            ViewBag.vier = dbtext.FienPricesa(id);
            ViewBag.OddNumbers = Request.QueryString["OddNumbers"];
            ViewBag.Passornot = Request.QueryString["Passornot"];
            ViewBag.paymentmethod= Request.QueryString["paymentmethod"];
            //岗位数据
            var positon = employeesInfoManage.GetPositionByEmpid(user.EmpNumber);
           ViewBag.postName=   positon.PositionName.Contains("会计") == true ? 1 : 0;
       
            return View();
        }
        /// <summary>
        /// 审核入账是否成功
        /// </summary>
        /// <param name="id">核对缴费是否成功编号</param>
        /// <param name="whether">是否入账</param>
        /// <param name="OddNumbers">单号</param>
        /// <returns></returns>
        public ActionResult Tuitionentry(int id, string whether, string OddNumbers,string paymentmethod)
        {
            return Json(dbtext.Tuitionentry(id, whether, OddNumbers, paymentmethod), JsonRequestBehavior.AllowGet);
        }

        StudentDataKeepAndRecordBusiness stuDataKeepAndRecordBusiness = new StudentDataKeepAndRecordBusiness();
        /// <summary>
        /// 缴纳预入费页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Prepayments()
        {
            return View();
        }
        /// <summary>
        /// 预入费缴纳操作数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="Name">学员姓名</param>
        /// <returns></returns>
        public ActionResult PrepaymentsDate(int page, int limit,string Name)
        {
           var costlist= stuDataKeepAndRecordBusiness.GetSudentDataAll();
            if (!string.IsNullOrEmpty(Name))
            {
                costlist = costlist.Where(a => a.StuName.Contains(Name)).ToList();
            }
            var dataList = costlist.OrderByDescending(a => a.Id).Skip((page - 1) * limit).Take(limit).ToList();
            //  var x = dbtext.GetList();
            var data = new
            {
                code = "",
                msg = "",
                count = costlist.Count,
                data = dataList
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public class PreentryfeeView
        {
            public string Name { get; set; }
            public string gradeName { get; set; }
        }
        public ActionResult Paytheadvancefee(int id)
        {
            //班级业务类
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            List<PreentryfeeView> preentryfeeViews = new List<PreentryfeeView>();
            PreentryfeeView preentryfeeView = new PreentryfeeView();
            preentryfeeView.Name = "初中生待定";
            preentryfeeView.gradeName = "Y1";
            preentryfeeViews.Add(preentryfeeView);
            preentryfeeView = new PreentryfeeView();
            preentryfeeView.gradeName = "S1";
            preentryfeeView.Name = "高中生待定";
            preentryfeeViews.Add(preentryfeeView);

          var cls=  classScheduleBusiness.GetList().Where(a => a.IsDelete == false && a.ClassStatus == false && a.ClassstatusID == null && a.grade_Id == 1 || a.grade_Id == 1002).OrderByDescending(a=>a.grade_Id).Select(a=>new PreentryfeeView { gradeName= classScheduleBusiness.GetClassGrand(a.id,2), Name=a.ClassNumber }).ToList();
            preentryfeeViews.AddRange(cls);
            ViewBag.preentryfeeViews= preentryfeeViews.Select(a => new SelectListItem { Value = a.Name+","+a.gradeName, Text = a.Name });
            ViewBag.ExportStudentBeanData = stuDataKeepAndRecordBusiness.findId(id.ToString());
         
            return View();
        }
        /// <summary>
        /// 预入费缴纳
        /// </summary>
        /// <param name="preentryfee"></param>
        /// <returns></returns>
        public ActionResult PaytheadvancefeeAdd(Preentryfee preentryfee)
        {
          return Json(dbtext.PaytheadvancefeeAdd(preentryfee));
        }
        /// <summary>
        /// 获取已交预入费的学员页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PreentryfeeDate()
        {
            return View();
        }
        /// <summary>
        /// 获取已交预入费的学员数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult PreentryfeeDates(int page, int limit,string StuName,string identitydocument,string qBeginTime,string qEndTime)
        {
            return Json(dbtext.PreentryfeeDates(page, limit,StuName,identitydocument,qBeginTime,qEndTime), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 预入费统计
        /// </summary>
        /// <param name="StuName"></param>
        /// <param name="identitydocument"></param>
        /// <param name="qBeginTime"></param>
        /// <param name="qEndTime"></param>
        /// <returns></returns>
        public ActionResult PreentryStatistics(string StuName, string identitydocument, string qBeginTime, string qEndTime)
        {
            return null;
        }
        /// <summary>
        /// 退预入费页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preentryfeerefund(int id)
        {
            //班级业务类
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            var x = Preentryfeebusenn.GetEntity(id);
            ViewBag.ExportStudentBeanData = stuDataKeepAndRecordBusiness.GetSudentDataAll().Where(a => a.Id == x.keeponrecordid).FirstOrDefault();
            ViewBag.obj = x;
            ViewBag.ClassNumber = x.ClassID;
            return View();
        }
        /// <summary>
        /// 退预入费数据业务操作
        /// </summary>
        /// <param name="refund">数据对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Preentryfeerefund(Refund refund)
        {
            return Json(dbtext.Preentryfeerefund(refund), JsonRequestBehavior.AllowGet);
       
        }
        /// <summary>
        /// 预入费作废
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Preentryfezuofei(int id)
        {
            return Json(dbtext.Preentryfezuofei(id), JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 学员退费操作数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Tuitionrefund()
        {
            return View();
        }
        [HttpGet]
        /// <summary>
        /// 退费页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult TuitionreHome(string id)
        {
            //学员信息
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
            //学员班级
            ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
            ViewBag.studentid= id;
            ViewBag.Stage = Grandcontext.GetList().Where(a => a.IsDelete == false).Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.GrandName }).ToList();
            var x = studentInformationBusiness.GetEntity(id);
            x.Education = scheduleForTraineesBusiness.SutdentCLassName(id) == null ? "暂无" : scheduleForTraineesBusiness.SutdentCLassName(id).ClassID;
           
            return View(x);
        }

        public ActionResult TuitionreStage(int Grand_id,string StudentID)
        {
           
            return Json(dbtext.TuitionreStage(StudentID, Grand_id),JsonRequestBehavior.AllowGet);
        }

    
        [HttpPost]
        public ActionResult TuitionreHomes(string Tuitionrefunds)
        {
            //引入序列化
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            //序列化
            var list = serializer.Deserialize<List<TuitionrefundView>>(Tuitionrefunds);

            return Json(dbtext.TuitionreHomes(list), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取退费数据
        /// </summary>
        /// <returns></returns>
      
        public ActionResult RefunditemsDates(int page, int limit)
        {
            return Json(dbtext.Refunditems(page, limit), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult RefunditemsDate()
        {
            return View();

        }
        /// <summary>
        /// 驾校费用缴纳
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Drivingschoolpayment(string id)
        {
     
            //学号
            ViewBag.Stuid = id;
            // ViewBag.Costitemsid
            int Typeid = int.Parse(Request.QueryString["Typeid"]);
            //明目类型id
            ViewBag.Typeid = Typeid;
            //名目名称
            ViewBag.Costitemsid = enrollmentBusinesse.Costlist(id, Typeid).Select(a => new SelectListItem { Text = a.Name, Value = a.id.ToString() }).ToList();
          
         
            return View();
        }
        [HttpPost]
        public ActionResult Drivingschoolpayment(Payview payview)
        {
            return Json(dbtext.Drivingschoolpayment(payview), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 预入费单号补入
        /// </summary>
        /// <param name="id"></param>
        /// <param name="OddNumbers"></param>
        /// <param name="typez"></param>
        /// <returns></returns>
        public ActionResult ReentryfeeOddNumbers(int id,string OddNumbers,int typez)
        {
            return Json(dbtext.ReentryfeeOddNumbers(id, OddNumbers, typez), JsonRequestBehavior.AllowGet);
        }

        public class te
        {
            public string x1 { get; set; }
            public string x2 { get; set; }
            public string x3 { get; set; }
            public string x4 { get; set; }
            public string x5 { get; set; }
            public string x6 { get; set; }
            public string x7 { get; set; }
            public string x8 { get; set; }
            public string x9 { get; set; }
        }
        /// <summary>
        /// 学费食宿费缴纳
        /// </summary>
        /// <returns></returns>
        public ActionResult text1()
        {
            //阶段表
            BaseBusiness<Grand> geand = new BaseBusiness<Grand>();
            List<te> telist = new List<te>();
            // List<MyExcelClass> myExcelClasses
            string finame = Server.MapPath(@"\Areas\Finance\images\2020年5月学生学费明细表.xlsx");
            AsposeOfficeHelper asposeOfficeHelper = new AsposeOfficeHelper();
         System.Data.DataTable t = AsposeOfficeHelper.ReadExcel(finame, false);
            for (int i = 2; i < t.Rows.Count; i++)
            {
                te te = new te();

                if (t.Rows[i][1].ToString()!="")
                {
                   
                    if (t.Rows[i][2].ToString() != "")
                    {
                        te.x2 = t.Rows[i][1].ToString();
                        te.x3 = t.Rows[i][2].ToString();
                        te.x4 = t.Rows[i][3].ToString();
                        te.x5 = t.Rows[i][4].ToString();
                        te.x6 = t.Rows[i][5].ToString();
                        te.x7 = t.Rows[i][6].ToString();
                        te.x8 = t.Rows[i][7].ToString();
                        te.x9 = t.Rows[i][12].ToString();
                        telist.Add(te);
                    }
                 
                }
              
              

            }

            List<te> db = new List<te>();
            //学员信息
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
            //foreach (var item in telist)
            //{
            //    if(studentInformationBusiness.GetList().Where(a => a.identitydocument == item.x3).Count() <1)
            //    {
            //        te te = new te();
            //        te.x1 = item.x3;
            //        db.Add(te);
            //    }
            //}
            List<StudentFeeRecord> StudentFeeRecordlist = new List<StudentFeeRecord>();
            foreach (var item in telist)
            {
                //明目类型
                BaseBusiness<Costitems> costitemss = new BaseBusiness<Costitems>();
                var Grand_id = geand.GetList().Where(a => a.GrandName == item.x4).FirstOrDefault().Id;
                StudentFeeRecord studentFeeRecord = new StudentFeeRecord();
                studentFeeRecord.StudenID= studentInformationBusiness.GetEntity(item.x3).StudentNumber;
                studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 8 && a.Grand_id == Grand_id).FirstOrDefault().id;//学费
                studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x6);
                studentFeeRecord.AddDate = Convert.ToDateTime(item.x5);
                studentFeeRecord.Remarks = item.x9;
                studentFeeRecord.IsDelete = false;
                studentFeeRecord.FinanceModelid = 8;
                StudentFeeRecordlist.Add(studentFeeRecord);
                if (item.x7!="")
                {
                    studentFeeRecord = new StudentFeeRecord();
                    studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x3).StudentNumber;
                    studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 12 && a.Grand_id == Grand_id).FirstOrDefault().id;//食宿费
                    studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x7);
                    studentFeeRecord.AddDate = Convert.ToDateTime(item.x5);
                    studentFeeRecord.IsDelete = false;
                    studentFeeRecord.FinanceModelid = 8;
                    StudentFeeRecordlist.Add(studentFeeRecord);
                }
              
            }
            //学员费用
            BaseBusiness<StudentFeeRecord> studentfee = new BaseBusiness<StudentFeeRecord>();
            studentfee.Insert(StudentFeeRecordlist);
            return null;
        }

        //缴学杂费
        public ActionResult text2()
        {
            //学员信息
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
            //阶段表
            BaseBusiness<Grand> geand = new BaseBusiness<Grand>();
            List<te> telist = new List<te>();
            // List<MyExcelClass> myExcelClasses
            string finame = Server.MapPath(@"\Areas\Finance\images\2020年5月学生学费明细表.xlsx");
            AsposeOfficeHelper asposeOfficeHelper = new AsposeOfficeHelper();
            System.Data.DataTable t = AsposeOfficeHelper.ReadExcel(finame, false);
            for (int i = 2; i < t.Rows.Count; i++)
            {
                te te = new te();

                if (t.Rows[i][1].ToString() != "")
                {

                    if (t.Rows[i][2].ToString() != "")
                    {
                        te.x1 = t.Rows[i][1].ToString();
                        te.x2 = t.Rows[i][2].ToString();//身份证
                        te.x3 = t.Rows[i][3].ToString();//阶段
                        te.x4 = t.Rows[i][4].ToString();//缴费日期
                        te.x5 = t.Rows[i][7].ToString();//军训费
                        te.x6 = t.Rows[i][8].ToString();//校服费
                        te.x7 = t.Rows[i][9].ToString();//体检，一卡通
                        te.x8 = t.Rows[i][10].ToString();//宿舍押金
                        te.x9 = t.Rows[i][11].ToString();
                        telist.Add(te);
                    }

                }
            }

            List<StudentFeeRecord> StudentFeeRecordlist = new List<StudentFeeRecord>();
            foreach (var item in telist)
            {
                //明目类型
                BaseBusiness<Costitems> costitemss = new BaseBusiness<Costitems>();
                var Grand_id = geand.GetList().Where(a => a.GrandName == item.x3).FirstOrDefault().Id;
                StudentFeeRecord studentFeeRecord = new StudentFeeRecord();
                if (item.x5 != "")//军训服
                {
                    studentFeeRecord = new StudentFeeRecord();
                    studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x2).StudentNumber;
                    studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 10 && a.Grand_id == Grand_id && a.Name == "军训费").FirstOrDefault().id;//学杂
                    studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x5);
                    studentFeeRecord.AddDate = Convert.ToDateTime(item.x4);
                    studentFeeRecord.IsDelete = false;
                    studentFeeRecord.FinanceModelid = 8;
                    StudentFeeRecordlist.Add(studentFeeRecord);
                }
                if (item.x7 != "")//校服
                {
                    if (Convert.ToDecimal(item.x7) > 50)
                    {
                        studentFeeRecord = new StudentFeeRecord();
                        studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x2).StudentNumber;
                        studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 10 && a.Grand_id == Grand_id && a.Name == "体检费" && a.IsDelete == false).FirstOrDefault().id;//学杂
                        studentFeeRecord.Amountofmoney = 50;
                        studentFeeRecord.AddDate = Convert.ToDateTime(item.x4);
                        studentFeeRecord.IsDelete = false;
                        studentFeeRecord.FinanceModelid = 8;
                        StudentFeeRecordlist.Add(studentFeeRecord);
                        studentFeeRecord = new StudentFeeRecord();
                        studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x2).StudentNumber;
                        studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 10 && a.Grand_id == Grand_id && a.Name == "一卡通" && a.IsDelete == false).FirstOrDefault().id;//学杂
                        studentFeeRecord.Amountofmoney = 50;
                        studentFeeRecord.AddDate = Convert.ToDateTime(item.x4);
                        studentFeeRecord.IsDelete = false;
                        studentFeeRecord.FinanceModelid = 8;
                        StudentFeeRecordlist.Add(studentFeeRecord);
                    }
                    else
                    {
                        studentFeeRecord = new StudentFeeRecord();
                        studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x2).StudentNumber;
                        studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 10 && a.Grand_id == Grand_id && a.Name == "体检费" && a.IsDelete == false).FirstOrDefault().id;//学杂
                        studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x7);
                        studentFeeRecord.AddDate = Convert.ToDateTime(item.x4);
                        studentFeeRecord.IsDelete = false;
                        studentFeeRecord.FinanceModelid = 8;
                        StudentFeeRecordlist.Add(studentFeeRecord);
                    }

                }
                if (item.x6 != "")//体检，一卡通
                {
                    studentFeeRecord = new StudentFeeRecord();
                    studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x2).StudentNumber;
                    studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 10 && a.Grand_id == Grand_id && a.Name.Contains("校服") && a.IsDelete == false).FirstOrDefault().id;//学杂
                    studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x6);
                    studentFeeRecord.AddDate = Convert.ToDateTime(item.x4);
                    studentFeeRecord.IsDelete = false;
                    studentFeeRecord.FinanceModelid = 8;
                    StudentFeeRecordlist.Add(studentFeeRecord);
                }
                if (item.x8 != "")//宿舍押金
                {
                    studentFeeRecord = new StudentFeeRecord();
                    studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x2).StudentNumber;
                    studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 10 && a.Grand_id == Grand_id && a.Name == "宿舍押金" && a.IsDelete == false).FirstOrDefault().id;//学杂
                    studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x8);
                    studentFeeRecord.AddDate = Convert.ToDateTime(item.x4);
                    studentFeeRecord.IsDelete = false;
                    studentFeeRecord.FinanceModelid = 8;
                    StudentFeeRecordlist.Add(studentFeeRecord);
                }
            }
            //学员费用
            BaseBusiness<StudentFeeRecord> studentfee = new BaseBusiness<StudentFeeRecord>();
            studentfee.Insert(StudentFeeRecordlist);
            return null;
        }
        //s3缴费
        public ActionResult text3()
        {
            //学员信息
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
            //阶段表
            BaseBusiness<Grand> geand = new BaseBusiness<Grand>();
            List<te> telist = new List<te>();
            // List<MyExcelClass> myExcelClasses
            string finame = Server.MapPath(@"\Areas\Finance\images\2020年5月学生学费明细表.xlsx");
            AsposeOfficeHelper asposeOfficeHelper = new AsposeOfficeHelper();
            System.Data.DataTable t = AsposeOfficeHelper.ReadExcel(finame, false);
            for (int i = 2; i < t.Rows.Count; i++)
            {
                te te = new te();

                if (t.Rows[i][1].ToString() != "")
                {

                    if (t.Rows[i][2].ToString() != "")
                    {
                        te.x2 = t.Rows[i][1].ToString();
                        te.x3 = t.Rows[i][2].ToString();//身份证
                        te.x4 = t.Rows[i][3].ToString();//阶段
                        te.x5 = t.Rows[i][4].ToString();//缴费日期
                        te.x6 = t.Rows[i][5].ToString();//学费
                    
                        te.x8 = t.Rows[i][7].ToString();//食宿费
                        te.x9 = t.Rows[i][10].ToString();//备注
                        telist.Add(te);
                    }

                }

            }
            List<StudentFeeRecord> StudentFeeRecordlist = new List<StudentFeeRecord>();
            foreach (var item in telist)
            {
                //明目类型
                BaseBusiness<Costitems> costitemss = new BaseBusiness<Costitems>();
                var Grand_id = geand.GetList().Where(a => a.GrandName == item.x4).FirstOrDefault().Id;
                StudentFeeRecord studentFeeRecord = new StudentFeeRecord();
                studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x3).StudentNumber;
                studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 8 && a.Grand_id == Grand_id).FirstOrDefault().id;//学费
                studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x6);
                studentFeeRecord.AddDate = Convert.ToDateTime(item.x5);
                studentFeeRecord.Remarks = item.x9;
                studentFeeRecord.IsDelete = false;
                studentFeeRecord.FinanceModelid = 8;
                StudentFeeRecordlist.Add(studentFeeRecord);
                if (item.x8 != "")
                {
                    studentFeeRecord = new StudentFeeRecord();
                    studentFeeRecord.StudenID = studentInformationBusiness.GetEntity(item.x3).StudentNumber;
                    studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 12 && a.Grand_id == Grand_id).FirstOrDefault().id;//食宿费
                    studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x8);
                    studentFeeRecord.AddDate = Convert.ToDateTime(item.x5);
                    studentFeeRecord.IsDelete = false;
                    studentFeeRecord.FinanceModelid = 8;
                    StudentFeeRecordlist.Add(studentFeeRecord);
                }


            }
            //学员费用
            BaseBusiness<StudentFeeRecord> studentfee = new BaseBusiness<StudentFeeRecord>();
            studentfee.Insert(StudentFeeRecordlist);
            return null;
        }

        public ActionResult text4()
        {
            //学员信息
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
            //阶段表
            BaseBusiness<Grand> geand = new BaseBusiness<Grand>();
            List<te> telist = new List<te>();
            // List<MyExcelClass> myExcelClasses
            string finame = Server.MapPath(@"\Areas\Finance\images\2020年5月学生学费明细表.xlsx");
            AsposeOfficeHelper asposeOfficeHelper = new AsposeOfficeHelper();
            System.Data.DataTable t = AsposeOfficeHelper.ReadExcel(finame, false);
            for (int i = 2; i < t.Rows.Count; i++)
            {
                te te = new te();

                if (t.Rows[i][1].ToString() != "")
                {

                    if (t.Rows[i][2].ToString() != "")
                    {
                        te.x2 = t.Rows[i][1].ToString();
                        te.x3 = t.Rows[i][2].ToString();//身份证
                        te.x4 = t.Rows[i][3].ToString();//阶段
                        te.x5 = t.Rows[i][4].ToString();//缴费日期
                        te.x6 = t.Rows[i][5].ToString();//学费
                        te.x8 = t.Rows[i][6].ToString();//食宿费
                        te.x9 = t.Rows[i][9].ToString();//备注
                        telist.Add(te);
                    }

                }

            }
         
            List<StudentFeeRecord> StudentFeeRecordlist = new List<StudentFeeRecord>();
            foreach (var item in telist)
            {
                //明目类型
                BaseBusiness<Costitems> costitemss = new BaseBusiness<Costitems>();
                var Grand_id = geand.GetList().Where(a => a.GrandName == item.x4).FirstOrDefault().Id;
                StudentFeeRecord studentFeeRecord = new StudentFeeRecord();
                if (item.x6 != "")
                {
                    studentFeeRecord = new StudentFeeRecord();
                    studentFeeRecord.StudenID = studentInformationBusiness.GetList().Where(a => a.identitydocument == item.x3).FirstOrDefault().StudentNumber;
                    studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 8 && a.Grand_id == Grand_id).FirstOrDefault().id;//学费
                    studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x6);
                    studentFeeRecord.AddDate = Convert.ToDateTime(item.x5);
                    studentFeeRecord.Remarks = item.x9;
                    studentFeeRecord.IsDelete = false;
                    studentFeeRecord.FinanceModelid = 8;
                    StudentFeeRecordlist.Add(studentFeeRecord);
                }
                if (item.x8 != "")
                {
                    studentFeeRecord = new StudentFeeRecord();
                    studentFeeRecord.StudenID = studentInformationBusiness.GetList().Where(a => a.identitydocument == item.x3).FirstOrDefault().StudentNumber;
                    studentFeeRecord.Costitemsid = costitemss.GetList().Where(a => a.Rategory == 12 && a.Grand_id == Grand_id).FirstOrDefault().id;//食宿费
                    studentFeeRecord.Amountofmoney = Convert.ToDecimal(item.x8);
                    studentFeeRecord.AddDate = Convert.ToDateTime(item.x5);
                    studentFeeRecord.IsDelete = false;
                    studentFeeRecord.FinanceModelid = 8;
                    StudentFeeRecordlist.Add(studentFeeRecord);
                }


            }
            //学员费用
            BaseBusiness<StudentFeeRecord> studentfee = new BaseBusiness<StudentFeeRecord>();
            studentfee.Insert(StudentFeeRecordlist);
            return null;
        }

        public ActionResult text5() {
            //学员费用
            BaseBusiness<StudentFeeRecord> studentfee = new BaseBusiness<StudentFeeRecord>();
           var x= studentfee.GetList().Where(a => a.Costitemsid == 10 || a.Costitemsid == 11).ToList();
            return null;
        }
        //订单重审
  
        public ActionResult reviewOpen(string check_id,string studentid)
        {
            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            EmployeesInfoManage employeesInfoManage = new EmployeesInfoManage();
            //学员信息
            StudentInformationBusiness studentInformationBusiness = new StudentInformationBusiness();
            ViewBag.student = studentInformationBusiness.GetEntity(studentid);
            ViewBag.id = check_id;
            ViewBag.vier = dbtext.FienPricesa(int.Parse(check_id));
            ViewBag.OddNumbers = Request.QueryString["OddNumbers"];
            ViewBag.Passornot = Request.QueryString["Passornot"];
            ViewBag.paymentmethod = Request.QueryString["paymentmethod"];
            ViewBag.reviewList = pay.GetList().Where(d => d.id == int.Parse(check_id)).ToList();
            //岗位数据
            var positon = employeesInfoManage.GetPositionByEmpid(user.EmpNumber);
            ViewBag.postName = positon.PositionName.Contains("会计") == true ? 1 : 0;
            return PartialView("/Areas/Finance/Views/Shared/Review.cshtml");
        }
        //订单表数据
        BaseBusiness<Paymentverification> pay = new BaseBusiness<Paymentverification>();
        public ActionResult review(string checkID,string OddNumbers,string Paymentmethod)
        {
           
            var iq_reID = pay.GetList().Where(d => d.id == int.Parse(checkID)).SingleOrDefault();
            iq_reID.OddNumbers = OddNumbers;
            iq_reID.Paymentmethod = Paymentmethod;
            pay.Update(iq_reID);
            return Json(new
            {
                code=0,
                msg="修改成功"
            });
            
        }

    } 
}