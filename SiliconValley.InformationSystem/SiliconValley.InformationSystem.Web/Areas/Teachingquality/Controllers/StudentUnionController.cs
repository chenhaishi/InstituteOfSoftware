using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public ActionResult MebersGetDate(int page, int limit, string StuName, string qEndTime, string qBeginTime, string quiz1, string sex)
        {

            var dataList = dbtext.UnionMembersList(UnName, page, limit, StuName, qEndTime, qBeginTime, quiz1, sex);
            //  var x = dbtext.GetList();
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }
        //添加学生会成员页面
        [HttpGet]
        public ActionResult UnionMemberAdd()
        {
            ViewBag.position = dbtext.UnionPositionList().Select(a => new SelectListItem { Text = a.Jobtitle, Value = a.ID.ToString() }).ToList();

            ViewBag.department = UnName;
            return View();
        }
        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="AID">文件id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteActivty(string AID)
        {

            try
            {
                var AList = list.GetList().Where(d => d.AID == AID).SingleOrDefault();
                AList.IsDelete = 0;
                list.Update(AList);
                return Json(new { code = 0,msg="删除成功" });
            }
            catch (Exception ex)
            {

                return Json(new { code = -1,msg="服务器异常" });
            }
        }
        public ActionResult Activty()
        {
            return View();
        }

        public ActionResult openActivty()
        {
            return View();
        }
        [HttpPost]
        //学生会文件活动上传方法
        public ActionResult Acticty(string title, DateTime date)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                CloudstorageBusiness Bos = new CloudstorageBusiness();
                var client = Bos.BosClient();

                string direName = $"/Acticty/Dox/{title}/";
                //保存到文件夹 
                string computerfielnaem = "Actictyfielnaem";
                var computerfile = Request.Files["rarfile"];

                //保存的文件路径
                string computerUrl = "";
                if (computerfile != null)
                {
                    //获取文件拓展名称 
                    var exait = Path.GetExtension(computerfile.FileName);
                    //computerUrl = Server.MapPath("/Areas/ExaminationSystem/Files/AnswerSheet/" + direName + "/" + computerfielnaem + exait);
                    computerUrl = $"{direName}{computerfielnaem + exait}";
                    //
                    Bos.Savefile("xinxihua", direName, computerfielnaem + exait, computerfile.InputStream);
                }
                BaseBusiness<StudentActivity> Activity_list = new BaseBusiness<StudentActivity>();
                if (computerUrl != "")
                {
                    StudentActivity list = new StudentActivity()
                    {
                        AddTime = date,
                        ADox = computerUrl,
                        AID = Guid.NewGuid().ToString(),
                        AState = 1,
                        IsDelete = 1,
                        Reamk = null,
                        Title = title
                    };
                    //string sql = "insert into StudentActivity values ('" + Guid.NewGuid().ToString() + "','"+ title + "','"+date+"',"+"'22'"+","+1+","+1+",'"+ computerUrl + "')";
                    //Activity_list.ExecuteSql(sql);
                    Activity_list.Insert(list);
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
            return Json(result);
        }
        //学生会下载
        public ActionResult DownloadComputerSheet(string Aid)
        {
            CloudstorageBusiness Bos = new CloudstorageBusiness();

            var client = Bos.BosClient();
            
            var candidateinfo = list.GetList().Where(d => d.AID == Aid).FirstOrDefault();

            //获取答卷路径

            //var computerPath = candidateinfo.ComputerPaper.Split(',')[1];
            var computerPath = candidateinfo.ADox;
            //FileStream fileStream = new FileStream(computerPath, FileMode.Open);

            var filedata = client.GetObject("xinxihua", computerPath);

            var filename = Path.GetFileName(computerPath);

            return File(filedata.ObjectContent, "application/octet-stream", Server.UrlEncode(filename));

        }
        BaseBusiness<StudentActivity> list = new BaseBusiness<StudentActivity>();
        //学生会列表显示
        public ActionResult Acticty_List(int page, int limit, string StuTitle)
        {

            var sql = "";

            if (StuTitle == null)
            {
                sql = "select* from StudentActivity where IsDelete='1'";
            }
            else
            {
                sql = "select * from StudentActivity where Title='" + StuTitle + "'and IsDelete='1'";
            }
            var Alist = list.GetListBySql<StudentActivity>(sql).Select(d => new {AID=d.AID, AddTime = d.AddTime.ToString("yyyy-MM-dd"), ADox = d.ADox, Title = d.Title }).ToList();


            var dataList = Alist.OrderBy(a => a.AddTime).Skip((page - 1) * limit).Take(limit).ToList();
            return Json(new
            {
                code = 0,
                data = Alist,
                count = dataList.Count()
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UnionMemberAdd(StudentUnionMembers studentUnionMembers)
        {

            studentUnionMembers.department = UnionDepart.GetList().Where(a => a.Dateofregistration == false && a.Departmentname == UnName).FirstOrDefault().ID;
            string Studentid = Request.QueryString["StudentID"];
            return Json(dbtext.UnionMembersEntity(studentUnionMembers, Studentid), JsonRequestBehavior.AllowGet);
        }
        //学生会成员离职
        [HttpGet]
        public ActionResult StudentunionCheng()
        {
            string UnID = Request.QueryString["UnID"];
            var x = dbtext.GetList().Where(a => a.Dateofregistration == false && a.Departuretime == null && a.ID == int.Parse(UnID)).FirstOrDefault();
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
            return Json(dbtext.StudentUnionMembersDetailed(stuid), JsonRequestBehavior.AllowGet);
        }
    }
}