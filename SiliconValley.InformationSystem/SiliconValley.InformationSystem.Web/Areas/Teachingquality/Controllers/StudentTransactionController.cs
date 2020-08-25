using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.ClassDynamics_Business;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Teachingquality.Controllers
{
    /// <summary>
    /// 学员异动处理
    /// </summary>
       [CheckLogin]
    public class StudentTransactionController : Controller
    {
        private readonly ClassScheduleBusiness dbtext;
        public StudentTransactionController()
        {
            dbtext = new ClassScheduleBusiness();

        }
        //拆班记录
        BaseBusiness<RemovalRecords> Dismantle = new BaseBusiness<RemovalRecords>();
        //学员班级表
        ScheduleForTraineesBusiness Stuclass = new ScheduleForTraineesBusiness();
        //学员异动类
        ClassDynamicsBusiness classDynamicsBusiness = new ClassDynamicsBusiness();
        //学员状态基础数据
        BaseBusiness<Basicdat> BasicdatBusiness = new BaseBusiness<Basicdat>();
        //阶段
        GrandBusiness Grandcontext = new GrandBusiness();
        //专业
        SpecialtyBusiness Techarcontext = new SpecialtyBusiness();
        // GET: Teachingquality/StudentTransaction
        [HttpPost]
        public ActionResult ClassEnd(int ClassID)
        {
            return Json(dbtext.ClassEnd(ClassID), JsonRequestBehavior.AllowGet);
        }
       
        /// <summary>
        /// 异动申请处理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult transactionprocessing()
        {
            ViewBag.ClassID = Request.QueryString["ClassID"];
            ViewBag.Types = BasicdatBusiness.GetList().Where(a => a.IsDetele == false&&a.Name!="毕业").Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });
            return View();
        }
        /// <summary>
        /// 通过班级获取该班级所有异动数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult TransactionDate(int page, int limit,string TypeName,string StudentID,string Name,string IsaDopt)
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);

            return Json(dbtext.TransactionDate(page, limit, ClassID,TypeName,StudentID,Name, IsaDopt), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取已审核的异动数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="TypeName"></param>
        /// <param name="StudentID"></param>
        /// <param name="Name"></param>
        /// <param name="IsaDopt"></param>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public ActionResult TransactionDatePro(int page, int limit, string TypeName,  string Name, string IsaDopt, string ClassID)
        {
            return Json(dbtext.TransactionDatepro(page, limit, TypeName, Name, IsaDopt, ClassID), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 转班详细数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Transactiondetails()
        {
            int ID = int.Parse(Request.QueryString["ID"]);
            return View(dbtext.Transactiondetails(ID));
        }
        /// <summary>
        /// 转班操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Secss">同意或者拒绝</param>
        /// <returns></returns>
        public ActionResult Shiftworkoperation(int ID,string Secss)
        {
            return Json(dbtext.Shiftworkoperation(ID, Secss), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 重修申请表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Rebuild()
        {
            //学号
            string StudentID = Request.QueryString["StudentID"];
            //是否打印
            string Types = Request.QueryString["Types"];
            if (Types=="1")
            {
                ViewBag.Types = Types;
                int ID = int.Parse(Request.QueryString["ID"]);
                var Classview = dbtext.Transactiondetails(ID);
                Classview.NowCLassName = dbtext.GetEntity(Classview.NowCLass).ClassNumber;
                return View(Classview);
            }
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            var Dismantl = Dismantle.GetList().Where(a => a.IsDelete == false).ToList();
            var List = dbtext.RebuildStage(dbtext.FintClassSchedule(ClassID).grade_Id);
            foreach (var item in Dismantl)
            {
                List = List.Where(a => a.id != item.FormerClass).ToList();
            }
            ViewBag.List = List.Where(a => a.id != ClassID).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ClassNumber }).ToList();
            var x = dbtext.ShiftworkFine(StudentID);
            x.OriginalClass = ClassID;
            return View(x);
        }
        /// <summary>
        /// 重修表单数据提交
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Rebuild(TransactionView transactionView)
        {
            return Json(dbtext.RebuildAdd(transactionView), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 重修详细数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Rebuilddetailed()
        {
            int ID = int.Parse(Request.QueryString["ID"]);
           
            return View(dbtext.Transactiondetails(ID));
        }
        /// <summary>
        /// 重修数据操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Secss">同意或者拒绝</param>
        /// <returns></returns>
        public ActionResult Rebuildkoperation(int ID, string Secss)
        {
            return Json(dbtext.Rebuildkoperation(ID, Secss), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 休学申请表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Suspensionofschool()
        {
            string Types = Request.QueryString["Types"];
            if (Types == "1")
            {
                ViewBag.Types = Types;
                int ID = int.Parse(Request.QueryString["ID"]);
                var Classview = dbtext.Transactiondetails(ID);
                return View(Classview);
            }
            //学号
            string StudentID = Request.QueryString["StudentID"];
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            var Dismantl = Dismantle.GetList().Where(a => a.IsDelete == false).ToList();
            var List = dbtext.RebuildStage(dbtext.FintClassSchedule(ClassID).grade_Id);

            foreach (var item in Dismantl)
            {
                List = List.Where(a => a.id != item.FormerClass).ToList();
            }
            ViewBag.List = List.Where(a => a.id != ClassID).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ClassNumber }).ToList();
            return View(dbtext.ShiftworkFine(StudentID));
        }
        /// <summary>
        /// 休学申请提交数据
        /// </summary>
        /// <param name="transactionView"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Suspensionofschool(TransactionView transactionView)
        {
            return Json(dbtext.SuspensionofschoolAdd(transactionView), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 休学详细数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Suspensionofschooldetailed()
        {
            int ID = int.Parse(Request.QueryString["ID"]);
            return View(dbtext.Transactiondetails(ID));
        }
        /// <summary>
        /// 休学数据操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Seccs">通过或者拒绝</param>
        /// <returns></returns>
        public ActionResult SuspensionofschoolAdd(int ID,string Secss)
        {
            return Json(dbtext.SuspensionofschoolAdd(ID, Secss), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 复学申请表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Resumptionofstudy()
        {
            string Types = Request.QueryString["Types"];
            ViewBag.Types = Types;
            if (Types == "1")
            {

                int ID = int.Parse(Request.QueryString["ID"]);
                var Classview = dbtext.Transactiondetails(ID);
                return View(Classview);
            }
            //学号
            string StudentID = Request.QueryString["StudentID"];
           var  ClassID= classDynamicsBusiness.GetList().Where(a => a.Studentnumber == StudentID && a.IsaDopt != null).OrderByDescending(a => a.ID).FirstOrDefault().FormerClass;
                 var List = dbtext.ListGradeidenticals(dbtext.FintClassSchedule(ClassID).grade_Id);
            ViewBag.List = List.Where(a => a.id != ClassID).Select(a => new SelectListItem { Value = a.id.ToString(), Text = a.ClassNumber }).ToList();
            var x = dbtext.ShiftworkFine(StudentID);
            x.OriginalClass = ClassID;
          
            return View(x);
        }
        /// <summary>
        /// 复学表单数据提交
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Resumptionofstudy(TransactionView transactionView)
        {
            return Json(dbtext.ResumptionofstudyAdd(transactionView), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 复学详细数据Resumption operation
        /// </summary>
        /// <returns></returns>
        public ActionResult Resumptionodetailed()
        {
            int ID = int.Parse(Request.QueryString["ID"]);
        
            return View(dbtext.Transactiondetails(ID));
        }

        /// <summary>
        /// 复学数据操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Secss">通过或者拒绝</param>
        /// <returns></returns>
        public ActionResult Resumptionoperation(int ID, string Secss)
        {
            return Json(dbtext.Resumptionoperation(ID, Secss), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 学员开除申请表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Expel()
        {
            //学号
            string StudentID = Request.QueryString["StudentID"];
            //是否打印
            string Types = Request.QueryString["Types"];
            if (Types == "1")
            {
                ViewBag.Types = Types;
                int ID = int.Parse(Request.QueryString["ID"]);
                var Classview = dbtext.Transactiondetails(ID);
              
                return View(Classview);
            }
            //班级id
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
       
            var x = dbtext.ShiftworkFine(StudentID);
            x.OriginalClass = ClassID;
            return View(x);
        }
        /// <summary>
        /// 申请开除学员数据提交
        /// </summary>
        /// <param name="transactionView">数据对象</param>
        /// <returns></returns>
        public ActionResult Expel(TransactionView transactionView)
        {
            return Json(dbtext.ExpelAdd(transactionView), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取申请开除数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Exoeldetailed()
        {
            int ID = int.Parse(Request.QueryString["ID"]);
          
            return View(dbtext.Transactiondetails(ID));
        }
        /// <summary>
        /// 开除数据操作
        /// </summary>
        /// <param name="ID">异动id</param>
        /// <param name="Secss">通过或者拒绝</param>
        /// <returns></returns>
        public ActionResult Expelperation(int ID, string Secss,string State)
        {
            return Json(dbtext.Expelperation(ID, Secss, State), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 退学申请页面表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Dropoutofschools()
        {
            string Types = Request.QueryString["Types"];
            ViewBag.Types = Types;
            if (Types == "1")
            {

                int ID = int.Parse(Request.QueryString["ID"]);
                var Classview = dbtext.Transactiondetails(ID);
                return View(Classview);
            }
            //学号
            string StudentID = Request.QueryString["StudentID"];
           var x = dbtext.ShiftworkFine(StudentID);
           
            return View(x);
        }
        /// <summary>
        /// 退学表单数据提交
        /// </summary>
        /// <param name="transactionView"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Dropoutofschools(TransactionView transactionView)
        {
            return Json(dbtext.DropoutofschoolsAdd(transactionView), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 班级名称
        /// </summary>
        /// <returns></returns>
        public ActionResult Classinquiry()
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
            ViewBag.ClassID = ClassID;
            var Techarco = Techarcontext.GetList();

            Specialty specialty = new Specialty();
            specialty.SpecialtyName = "无";
        

            Techarco.Add(specialty);
            //专业
            ViewBag.Major_Id = Techarco.OrderBy(a=>a.Id).Select(a => new SelectListItem { Text = a.SpecialtyName, Value = a.Id.ToString() });
            //阶段
            ViewBag.Stage= dbtext.Grandbehind(dbtext.FintClassSchedule(ClassID).grade_Id).Select(a => new SelectListItem { Text = a.GrandName, Value = a.Id.ToString() });
            return View();
        }
        /// <summary>
        /// 获取班级
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult Classlist(int page, int limit,string ClassName,string Stage_id,string Major_Id)
        {
            int ClassID = int.Parse(Request.QueryString["ClassID"]);
          
            var MyClass = dbtext.RebuildStage(dbtext.FintClassSchedule(ClassID).grade_Id).Where(a=>a.id!=ClassID).ToList();

            if (!string.IsNullOrEmpty(ClassName))
            {
                MyClass = MyClass.Where(a => a.ClassNumber.Contains(ClassName)).ToList();
            }
            if (!string.IsNullOrEmpty(Stage_id))
            {
                int stage = int.Parse(Stage_id);

                MyClass = MyClass.Where(a => a.grade_Id == stage).ToList();
            }
            if (!string.IsNullOrEmpty(Major_Id))
            {
                if (Major_Id == "0")
                {
                    MyClass = MyClass.Where(a => a.Major_Id == null).ToList();
                }
                else
                {
                    int major = int.Parse(Major_Id);
                    MyClass = MyClass.Where(a => a.Major_Id == major).ToList();
                }
               
            }
            var dataList = MyClass.Select(a => new
            {
                //  a.BaseDataEnum_Id,
                a.id,
                ClassNumber = a.ClassNumber,
                grade_Id = Grandcontext.GetEntity(a.grade_Id).GrandName, //阶段id
                Major_Id = a.Major_Id == null ? "暂无专业" : Techarcontext.GetEntity(a.Major_Id).SpecialtyName,//专业
                stuclasss = Stuclass.GetList().Where(c => c.ID_ClassName == a.id && c.CurrentClass == true).Count()//班级人数
            }).OrderBy(a => a.id).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = MyClass.Count,
                data = dataList
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 退学详细
        /// </summary>
        /// <returns></returns>
        public ActionResult Dropoutofschoolsdetailed()
        {
            int ID = int.Parse(Request.QueryString["ID"]);

            return View(dbtext.Transactiondetails(ID));
        }
        //异动数据页面
        public ActionResult Changedata()
        {
            ViewBag.Types = BasicdatBusiness.GetList().Where(a => a.IsDetele == false && a.Name != "毕业").Select(a => new SelectListItem { Value = a.ID.ToString(), Text = a.Name });

            return View();
        }
        public ActionResult ClassProDate()
        {
            string ClassName = Request.QueryString["ClassName"];
            return Json(dbtext.ClassProDate(ClassName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult errorClass(string stuid)
        {
            BaseBusiness<StudentInformation> stu = new BaseBusiness<StudentInformation>();
            BaseBusiness<ScheduleForTrainees> sch = new BaseBusiness<ScheduleForTrainees>();
            BaseBusiness<ClassSchedule> cs = new BaseBusiness<ClassSchedule>();
            var id = stu.GetList().Where(d => d.StudentNumber == stuid).SingleOrDefault();//获取学生名称
            var class_error = sch.GetList().Where(d => d.StudentID == id.StudentNumber).SingleOrDefault();//获取学生当前所在班级
            var error = cs.GetList().ToList();//获取所有班级
            ViewBag.id = id.Name;
            ViewBag.class_error = class_error.ClassID;
            ViewBag.error = error;
            return  PartialView("/Areas/Teachingquality/Views/Shared/errorClass.cshtml");
        }
        /// <summary>
        /// 班级错误修改
        /// </summary>
        /// <param name="stuid">学员id</param>
        /// <param name="classid">班级名称</param>
        /// <param name="className">班级id</param>
        /// <returns></returns>
        public ActionResult error_class(string stuid,string classid,string className)
        {
            BaseBusiness<StudentInformation> stu = new BaseBusiness<StudentInformation>();
            BaseBusiness<ScheduleForTrainees> sch = new BaseBusiness<ScheduleForTrainees>();
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            EmployeesInfoManage empid = new EmployeesInfoManage();//员工业务类
            var dept = empid.GetDeptByEmpid(UserName.EmpNumber);//根据员工编号获取员工所属部门
            var posi = empid.GetPositionByEmpid(UserName.EmpNumber);//根据员工编号所属岗位
            if (dept.DeptName == "s1、s2教质部" && posi.PositionName.Contains("主任"))
            {


                var id = stu.GetList().Where(d => d.StudentNumber == stuid).SingleOrDefault();
                var class_error = sch.GetList().Where(d => d.StudentID == id.StudentNumber).SingleOrDefault();
                if (class_error == null)
                {
                    return Json(new
                    {
                        code = -1,
                        msg = "查询数据为空"
                    });
                }
                class_error.ClassID = classid;
                class_error.ID_ClassName = int.Parse(className);
                sch.Update(class_error);
                return Json(new
                {
                    code = 0,
                    msg = "修改成功"
                });
            }
            else
            {
                return Json(new
                {
                    code = -1,
                    msg = "你没有该权限"
                });
            }

        }


    }
}