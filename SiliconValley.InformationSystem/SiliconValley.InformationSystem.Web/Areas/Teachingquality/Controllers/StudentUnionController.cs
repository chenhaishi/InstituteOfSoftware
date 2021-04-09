using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Teachingquality.Controllers
{
    //学生会
    [CheckLogin]
    public class StudentUnionController : Controller
    {
        public static string UnName = null;
        private readonly StudentUnionBusiness dbtext;
        public StudentUnionController()
        {
            dbtext = new StudentUnionBusiness();
        }
        //学生会部门
        BaseBusiness<StudentUnionDepartment> UnionDepart = new BaseBusiness<StudentUnionDepartment>();
        // GET: Teachingquality/StudentUnion
        public ActionResult Index()
        {
            var x = UnionDepart.GetList().Where(a => a.Dateofregistration == false).ToList();
            List<StudentUnionDepartmentView> list = new List<StudentUnionDepartmentView>();
            foreach (var item in x)
            {
                StudentUnionDepartmentView studentUnionDepartmentView = new StudentUnionDepartmentView();
                studentUnionDepartmentView.Name = item.Departmentname;
                studentUnionDepartmentView.count = dbtext.GetList().Where(a => a.Dateofregistration == false && a.Departuretime == null && a.department == item.ID).Count();
                list.Add(studentUnionDepartmentView);

            }

            ViewBag.list = list;
            return View();
        }
        //查询这个部门有多少人
        public int count()
        {
            string Name = Request.QueryString["name"];
            int id = UnionDepart.GetList().Where(a => a.Dateofregistration == false && a.Departmentname == Name).FirstOrDefault().ID;
            return dbtext.GetList().Where(a => a.Dateofregistration == false && a.Departuretime == null && a.department == id).Count();
        }
        //学生会部门
        [HttpGet]
        public ActionResult Department()
        {
            
          
            return View();
        }
        //查询部门是否有重复
        public int SelectDepa()
        {
            string Name = Request.QueryString["Name"];
           return dbtext.SelectDepa(Name);
        }
        //添加学生会部门数据操作
        [HttpPost]
        public bool Department(StudentUnionDepartment studentUnionDepartment)
        {
            studentUnionDepartment.Addtime = DateTime.Now;
            studentUnionDepartment.Dateofregistration = false;
            return dbtext.AddDepa(studentUnionDepartment);
       
        }
        //撤销部门
        public bool UodateDepa()
        {
            string Name = Request.QueryString["name"];
            return dbtext.UodateDepa(Name);
        }
        //学生会成员
        public ActionResult StudentUnionMemberss(string id)
        {

            UnName = id;
            return View();
        }

        //获取学生会成员
        public ActionResult MebersGetDate(int page,int limit, string StuName, string qEndTime, string qBeginTime, string quiz1, string sex)
        {
      
            var dataList = dbtext.UnionMembersList(UnName, page, limit, StuName, qEndTime, qBeginTime, quiz1, sex);
            //  var x = dbtext.GetList();
            return Json( dataList, JsonRequestBehavior.AllowGet);
        }
        //添加学生会成员页面
        [HttpGet]
        public ActionResult UnionMemberAdd()
        {
            ViewBag.position = dbtext.UnionPositionList().Select(a => new SelectListItem { Text = a.Jobtitle, Value = a.ID.ToString() }).ToList();
          
            ViewBag.department = UnName;
            return View();
        }
        public ActionResult Activty()
        {
            return View();
        }
        
        CloudstorageBusiness cloudstorage_Business = new CloudstorageBusiness();
        [HttpPost]
        public ActionResult ActivtyStudentimg(string id)
        {
            //    string studentid = Request.QueryString["studentid"];
            // Bitmap bitmap = new Bitmap(144,192);
            AjaxResult result = new AjaxResult();
            try
            {
                var fien = Request.Files[0];
                string filename = fien.FileName;
                string Extension = Path.GetExtension(filename);
                string newfilename = "15055"+ Extension;

                //if (dbtext.StudentAddImg(id, newfilename) == true)
                //{

                    result = new SuccessResult();
                    result.ErrorCode = 200;
                result.Data = newfilename;
                    var client = cloudstorage_Business.BosClient();
                    // 以数据流形式上传Object


                    cloudstorage_Business.PutObject("xinxihua", "Activty", newfilename, fien.InputStream);



                    //  bitmap.Save(path);
                //}
            }
            catch (Exception ex)
            {

                result = new SuccessResult();
                result.ErrorCode = 300;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //添加学生会数据操作
        [HttpPost] 
        public ActionResult UnionMemberAdd(StudentUnionMembers studentUnionMembers)
        {

            studentUnionMembers.department = UnionDepart.GetList().Where(a => a.Dateofregistration == false && a.Departmentname == UnName).FirstOrDefault().ID;
            string Studentid = Request.QueryString["StudentID"];
            return Json( dbtext.UnionMembersEntity(studentUnionMembers, Studentid),JsonRequestBehavior.AllowGet);
        }
        //学生会成员离职
        [HttpGet]
        public ActionResult StudentunionCheng()
        {
            string UnID = Request.QueryString["UnID"];
         var x= dbtext.GetList().Where(a => a.Dateofregistration == false && a.Departuretime == null && a.ID == int.Parse(UnID)).FirstOrDefault();
            ViewBag.Studentnumber = x.Studentnumber;
          ViewBag.department = UnName;
         
            ViewBag.Union_id = x.ID;
         
            return View();
        }
        //学生会成员离职数据操作
        [HttpPost]
        public ActionResult StudentunionCheng(StudentUnionLeaves studentUnionLeaves)
        {
            return Json(dbtext.StudentunionCheng(studentUnionLeaves), JsonRequestBehavior.AllowGet);
        }

        //学生会详细
        [HttpGet]
        public ActionResult Detailed()
        {
            ViewBag.stuid = Request.QueryString["StudentID"];
            return View();
        }
        public ActionResult StudentUnionMembersDetailed()
        {
            var stuid = Request.QueryString["stuid"];
            return Json( dbtext.StudentUnionMembersDetailed(stuid),JsonRequestBehavior.AllowGet);
        }
    }
}