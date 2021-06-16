using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.DepartmentBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Teachingquality.Controllers
{
    [CheckLogin]
    public class HeadteacherController : Controller
    {
        public static int HeadteID;
        // GET: Teachingquality/Headteacher
        private readonly HeadmasterBusiness dbtext;
        //员工信息
        EmployeesInfoManage employeesInfoManage = new EmployeesInfoManage();
        //员工岗位
        BaseBusiness<Position> business = new BaseBusiness<Position>();
        //人事,部门表
        BaseBusiness<Department> Depa = new BaseBusiness<Department>();
        //班级
        ClassScheduleBusiness ClasHead = new ClassScheduleBusiness();
        //班主任带班
        BaseBusiness<HeadClass> HeadClassEnti = new BaseBusiness<HeadClass>();
        public HeadteacherController()
        {
            dbtext = new HeadmasterBusiness();
        }
        //主页面
        public ActionResult Index()
        {
            return View();
        }
        //班主任数据
        public ActionResult GetDate(int page, int limit)
        {
            //当前登陆人
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            //  int DepaID = Depa.GetList().Where(a => a.DeptName.Contains("教质部") && a.IsDel == false).FirstOrDefault().DeptId;
            var list = dbtext.GetList().Where(d=>d.IsDelete==false&& employeesInfoManage.GetInfoByEmpID(d.informatiees_Id).IsDel == false).ToList();
            if (dbtext.GetList().Where(a => a.informatiees_Id == user.EmpNumber).FirstOrDefault() != null)
            {
                //employeesInfoManage.GetDeptByEmpid(user.EmpNumber).DeptId;//部门
                list= dbtext.GetList().Where(a =>  employeesInfoManage.GetDeptByEmpid(a.informatiees_Id).DeptId == employeesInfoManage.GetDeptByEmpid(user.EmpNumber).DeptId&&a.IsDelete==false ).ToList();
            }
                //    List<EmployeesInfo> EmployeesInfoList = new List<EmployeesInfo>();
                var emp=   employeesInfoManage.GetAll();
        var dataList = list.Select(c=>new { c.informatiees_Id, informatiees_Name = employeesInfoManage.GetEntity(c.informatiees_Id).EmpName,
        informatiees_Sex = emp.Where(a=>a.EmployeeId==c.informatiees_Id).FirstOrDefault().Sex ,
        Name = business.GetList().Where(a=>a.Pid== emp.Where(q=>q.EmployeeId==c.informatiees_Id).FirstOrDefault().PositionId).FirstOrDefault().PositionName,
        EntryTime = emp.Where(a=>a.EmployeeId==c.informatiees_Id).FirstOrDefault().EntryTime,
         DeptName=   employeesInfoManage.GetDeptByEmpid(dbtext.GetEntity(c.ID).informatiees_Id).DeptName,
         ID =c.ID,
         c.IsAttend,
         c.IsDelete
        }).OrderBy(a => a.informatiees_Id).Skip((page - 1) * limit).Take(limit).ToList();
        //  var x = dbtext.GetList();
        var data = new
        {
            code = "",
            msg = "",
            count = list.Count,
            data = dataList
        };
            return Json(data, JsonRequestBehavior.AllowGet);
    }
        //接班页面
        public ActionResult HeadmasterEntity(int ID)
        {
            HeadteID = ID;

            ViewBag.HeadMa = HeadmasreClass();
            ViewBag.Name = employeesInfoManage.GetEntity(dbtext.GetEntity(ID).informatiees_Id).EmpName;
           
            return View();
        }

        //班主任职业素养培训
        BaseBusiness<Professionala> ProfessionalaBusiness = new BaseBusiness<Professionala>();
        //班级数据

        public string HeadmasreClass()
        {
            
            //阶段
            GrandBusiness Grandcontext = new GrandBusiness();
            //拿到该班主任负责班级的阶段
            List<Grand> grands = Grandcontext.GetList();
            var x1s = dbtext.GetEntity(HeadteID);
            //班主任部门名称
            string DeptName = employeesInfoManage.GetDeptByEmpid(x1s.informatiees_Id).DeptName;
            //if (DeptName.ToLower().Contains("s1"))
            //{
            //    grands = Grandcontext.GetList().Where(a => a.IsDelete == false && a.GrandName == "S1" || a.GrandName == "S2" || a.GrandName == "Y1").ToList();
            //}
            //else
            //{
            //    grands = Grandcontext.GetList().Where(a => a.IsDelete == false && a.GrandName == "S3" || a.GrandName == "S4").ToList();
            //}
            //带班人
            object obj = new object();
            //该班主任所有可负责的班级
            List<ClassSchedule> classesList = new List<ClassSchedule>();
            foreach (var item in grands)
            {
                classesList.AddRange(ClasHead.GetList().Where(a => a.ClassstatusID == null && a.grade_Id == item.Id).ToList());
            }
            List<ClassSchedule> ListClass = classesList;
            List<ClassSchedule> MyClass = new List<ClassSchedule>();
           var x=   HeadClassEnti.GetList().Where(a => a.IsDelete == false && a.LeaderID == HeadteID&&a.EndingTime==null).ToList();
            //3
            var x1 = HeadClassEnti.GetList().Where(a => a.IsDelete == false && a.LeaderID != HeadteID&&a.EndingTime==null).ToList();
            //0
            foreach (var item in x)
            {
                var classstudent = classesList.Where(a => a.IsDelete == false && a.ClassStatus == false && a.id == item.ClassID).FirstOrDefault();
                if (classstudent!=null)
                {
                    MyClass.Add(classstudent);
                }
               
            }
            foreach (var item in x1)
            {
                ListClass = ListClass.Where(a => a.IsDelete == false && a.ClassStatus == false && a.id != item.ClassID).ToList();
            }

            var data=new
            {
                d1=ListClass.Select(c => new { value = c.id, title = c.ClassNumber+"("+ ClasHead.GetClassGrand(c.id,222) + ")" }).ToList(),
                d2= MyClass.Select(c => new { title= c.ClassNumber + "(" + ClasHead.GetClassGrand(c.id, 222) + ")", value =c.id}).ToList()
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
            
        }
        public ActionResult HeadmasreEntity()
        {
            string ClassName = Request.QueryString["ClassName"];

            return Json(dbtext.HeadClassEntis(HeadteID.ToString(), ClassName), JsonRequestBehavior.AllowGet); ;
        }


        //登记班主任职业素养课件培训
        [HttpGet]
        public ActionResult AddProfessionala()
        {
            GrandBusiness db_grand = new GrandBusiness();
            
            var gradlist = db_grand.AllGrand().Select(d => new SelectListItem { Text = d.GrandName, Value = d.Id.ToString() });

            ViewBag.grandlist = gradlist;

            return View();
        }
        //班主任职业素养培训数据记录
        [HttpPost]
        public ActionResult AddProfessionala(Professionala professionala)
        {
            return Json(dbtext.AddProfessionala(professionala),JsonRequestBehavior.AllowGet);
        }
        //显示班主任职业素养课件培训
        public ActionResult GetProfessionala()
        {
            ViewBag.professionala = ProfessionalaBusiness.GetList().Where(a => a.Dateofregistration == false).ToList().Count;
            return View();
        }
        /// <summary>
        /// 班主任职业素养培训数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult GetDateProfessionala(int page, int limit)
        {
            return Json(dbtext.GetDateProfessionala(page, limit), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查询单条班主任职业素养培训课件
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public ActionResult FineProfessionala(int id)
        {
            return View(dbtext.FineProfessionala(id));
        }
        /// <summary>
        /// 班主任带班分布图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Teamleaderdistribution()
        {
          // ViewBag.ListTeam= Newtonsoft.Json.JsonConvert.SerializeObject(dbtext.ListTeamleaderdistributionView());
           // ViewBag.ListTeamleaderdistributionView = dbtext.ListTeamleaderdistributionView();
            return View();
        }
        /// <summary>
        /// 获取分布图数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ListTeam()
        {
            return Json(dbtext.ListTeamleaderdistributionView(), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取已毕业班级分布图
        /// </summary>
        /// <returns></returns>
        public ActionResult graduationTeam()
        {
            return Json(dbtext.graduationView(), JsonRequestBehavior.AllowGet);
        }
        //班主任接班记录
        public ActionResult Successionrecord()
        {
            int HeadID = int.Parse(Request.QueryString["HeadID"]);
            ViewBag.HeadID = HeadID;
            var employees = employeesInfoManage.GetInfoByEmpID(dbtext.GetEntity(HeadID).informatiees_Id);
            var department = employeesInfoManage.GetDeptByEmpid(dbtext.GetEntity(HeadID).informatiees_Id);
            var Headyees = new SuccessionrecordView
            {
               Education=  employees.Education,//学历
               EmpName= employees.EmpName,//姓名
               Sex= employees.Sex,//性别
               Phone= employees.Phone,//电话
               DeptName= department.DeptName,//部门
                Images=  employees.Image//图片
            };
            return View(Headyees);
        }
        /// <summary>
        /// 获取班主任带班的数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult SuccessionrecordDate(int page, int limit)
        {
            int HeadID = int.Parse(Request.QueryString["HeadID"]);
            ViewBag.Employees= employeesInfoManage.GetInfoByEmpID(dbtext.GetEntity(HeadID).informatiees_Id);
            return Json(dbtext.SuccessionrecordDate(page, limit, HeadID), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 是否允许或者禁止班主任上质素课
        /// </summary>
        /// <param name="id">班主任表主键id</param>
        /// <param name="Isdele">允许或者拒绝</param>
        /// <returns></returns>
        public ActionResult IsAttend(int id, bool Isdele)
        {
            return Json(dbtext.IsAttend(id, Isdele), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 培训人
        /// </summary>
        /// <returns></returns>
        public ActionResult TraineeContro()
        {
            //部门
            ViewBag.department = Depa.GetList().Where(a => a.DeptName.Contains("教质部")).ToList().Select(a => new SelectListItem { Text = a.DeptName, Value = a.DeptId.ToString() }); ;
            return View();
        }
        /// <summary>
        /// 培训人数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="EmployeeId"员工编号></param>
        /// <param name="EmpName">员工姓名</param>
        /// <param name="department">部门</param>
        /// <returns></returns>
        public ActionResult TraineeDate(int page, int limit, string EmployeeId, string EmpName,string department)
        {

            return Json(dbtext.TraineeDate(page, limit, EmployeeId, EmpName, department), JsonRequestBehavior.AllowGet); 
          
        }

    }
}