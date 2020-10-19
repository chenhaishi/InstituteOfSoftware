using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Newtonsoft.Json;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Entity.Entity;
using BaiduBce.Services.Bos;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos.Model;
using SiliconValley.InformationSystem.Entity.ViewEntity;

namespace SiliconValley.InformationSystem.Web.Areas.Teachingquality.Controllers
{  //学员信息模块
    [CheckLogin]
    [CheckUrlPermission]
    public class StudentInformationController : Controller
    {
        //拆班记录
        BaseBusiness<RemovalRecords> Dismantle = new BaseBusiness<RemovalRecords>();
        //备案提供方法
        StudentDataKeepAndRecordBusiness dataKeepAndRecordBusiness = new StudentDataKeepAndRecordBusiness();
        //班级表
        ClassScheduleBusiness classschedu = new ClassScheduleBusiness();
        //学员班级表
        ScheduleForTraineesBusiness Stuclass = new ScheduleForTraineesBusiness();
        //班级阶段
        BaseBusiness<Grand> MyGrand = new BaseBusiness<Grand>();
        //专业
        SpecialtyBusiness Techarcontext = new SpecialtyBusiness();

        private static int NameKeysid = 0;
        public class Student { }

        private readonly StudentInformationBusiness dbtext;
        public StudentInformationController()
        {
            dbtext = new StudentInformationBusiness();

        }
        // GET: Teachingquality/StudentInformation
        public ActionResult Index()
        {
            return View();
        }
        //获取网络时间
        public string Date()
        {

            WebRequest request = null;
            WebResponse response = null;
            WebHeaderCollection headerCollection = null;

            string datetime = string.Empty;
            try
            {
                request = WebRequest.Create("https://www.baidu.com");
                request.Timeout = 3000;
                request.Credentials = CredentialCache.DefaultCredentials;
                response = (WebResponse)request.GetResponse();
                headerCollection = response.Headers;
                foreach (var h in headerCollection.AllKeys)
                { if (h == "Date") { datetime = headerCollection[h]; } }
                return datetime;
            }

            catch (Exception) { return datetime; }
            finally
            {
                if (request != null)
                { request.Abort(); }
                if (response != null)
                { response.Close(); }
                if (headerCollection != null)
                { headerCollection.Clear(); }
            }
        }
        //月份前面加个零
        public string Month(int a)
        {

            if (a < 10)
            {
                return "0" + a;
            }
            string c = a.ToString();
            return c;
        }

        //图片
        public JsonResult img()
        {
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase f = Request.Files["file"];//最简单的获取方法
                string name = "2017.jpg";

                f.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "Areas/Teachingquality/StuImages/" + name);//保存图片

                //这下面是返回json给前端 
                var data1 = new
                {
                    src = AppDomain.CurrentDomain.BaseDirectory + "Areas/Teachingquality/StuImages/" + name,//服务器储存路径
                };
                var Person = new
                {
                    code = 0,//0表示成功
                    msg = "",//这个是失败返回的错误
                    data = data1
                };
                return Json(Person);//格式化为json
            }
            else
            {
                return null;
            }
        }
        //生成学号
        public string StudentID(string IDnumber)

        {

            string mingci = string.Empty;
            DateTime date = Convert.ToDateTime(Date());
            //当前年份
            string n = date.Year.ToString().Substring(2);//获取年份

            //学员总数Mylist("StudentInformation")
            var laststr = dbtext.GetList().Where(a => Convert.ToDateTime(a.InsitDate).Year.ToString().Substring(2).ToString() == n).Count() + 1;
            string sfz = IDnumber.Substring(6, 8);

            string y = Month(Convert.ToInt32(date.Month)).ToString();
            // string count = Count().ToString();
            string count = laststr.ToString();
            if (count.Length < 2)
                mingci = "0000" + count;
            else if (count.Length < 3)
                mingci = "000" + count;
            else if (count.Length < 4)
                mingci = "00" + count;
            else if (count.Length < 5)
                mingci = "0" + count;
            else mingci = count;

            string xuehao = n + y + sfz + mingci;
            return xuehao;
        }

        //复杂日期截取
        public string Datetimes(DateTime InsitDate)
        {
            string[] dat = InsitDate.ToString().Split('/');
            string c = Month(int.Parse(dat[1]));
            string ri = dat[2];
            string[] ri1 = ri.Split(' ');
            string riqi = Month(int.Parse(ri1[0]));
            string data = dat[0] + "/" + c + "/" + riqi;
            return data;
        }


        //学员注册编辑
        public ActionResult Registeredtrainees()
        {

          
            string studentid = Request.QueryString["studentid"];


    
     
            ViewBag.Name = "编辑学员信息";
             
                return View(dbtext.GetEntity(studentid));

        }
        RedisCache redis = new RedisCache();
        //获取所有数据
        public ActionResult GetDate(int page, int limit, string Name, string Sex, string StudentNumber, string identitydocument,string Stu,string ClassName)
        {
            List<StudentInformation> listx = new List<StudentInformation>();
            //List<StudentInformation> list = dbtext.GetList().Where(a=>a.IsDelete!=true).ToList();
            List<StudentInformation> list = classschedu.HeadteStudent();
            foreach (var item in list)
            {
                //list
                if (item.BirthDate==null)
                {
                    var ident = item.identitydocument;
                   var x= ident.Substring(6).Substring(0,8);
                    var n = x.Substring(0, 4);
                    var y = x.Substring(4, 2);
                    var r = x.Substring(6, 2);
                   var  da=    n + "-" + y + "-" + r;
                    item.BirthDate = Convert.ToDateTime(da);
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(ClassName))
                {
                    listx = new List<StudentInformation>();
                  var classid=  classschedu.GetList().Where(a => a.ClassNumber == ClassName.ToUpper()&&a.ClassstatusID==null).FirstOrDefault();
                    if (classid!=null)
                    {
                      var student=  Stuclass.GetList().Where(a => a.CurrentClass == true && a.ID_ClassName == classid.id).ToList();
                        foreach (var item in student)
                        {
                            var x = list.Where(a => a.StudentNumber == item.StudentID).FirstOrDefault();
                            if (x!=null)
                            {
                                listx.Add(x);
                            }
                         
                        }
                        list = listx;
                    }
                    
                }
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
                if (!string.IsNullOrEmpty(Stu))
                {
                    listx = new List<StudentInformation>();
                    //Trim
                    string[] st = Stu.Split('-');
                    string kai = st[0] + st[1];
                    string jie = st[2] + st[3];

                    if (kai.Trim()==jie.Trim())
                    {
                        foreach (var item in list)
                        {
                            if (item.StudentNumber.Substring(0, 4) == kai.Substring(2).Trim())
                            {
                                listx.Add(item);
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in list)
                        {
                            if (int.Parse(item.StudentNumber.Substring(0, 4))>=int.Parse(kai.Substring(2).Trim())&& int.Parse(item.StudentNumber.Substring(0, 4))<= int.Parse(jie.Substring(2).Trim()))
                            {
                                listx.Add(item);
                            }

                            
                        }
                    }
                    list = listx;
                }

            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.加载数据);
            }
            var dataList = list.OrderBy(a => a.StudentNumber).Skip((page - 1) * limit).Take(limit).ToList();
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

        //学员备案查询赋值信息
        [HttpGet]
        public ActionResult DataKey()
        {
            ViewBag.Name = Request.QueryString["Name"];
            StudentDataKeepAndRecordBusiness dbctext = new StudentDataKeepAndRecordBusiness();

            return View();
        }
        /// <summary>
        /// 根据名字获取备案数据
        /// </summary>
        /// <param name="Name">姓名</param>
        /// <returns></returns>
        public List<StudentPutOnRecord> ListStudentPutOnRecord(string Name)
        {
            StudentDataKeepAndRecordBusiness dbctexta = new StudentDataKeepAndRecordBusiness();
            List<StudentPutOnRecord> x = null;
            x = dbctexta.GetList().Where(a => a.StuName.Trim() == Name).ToList();

            var studentlist = dbtext.StudentList();
            for (int i = x.Count - 1; i >= 0; i--)
            {
                if (x.Count > 0)
                {
                    foreach (var item in studentlist)
                    {
                        if (x.Count > 0)
                        {
                            if (x[i].Id == item.StudentPutOnRecord_Id && item.State == null)
                            {
                                x.Remove(x[i]);
                            }
                        }

                    }
                }
            }
            return x;
        }
        /// <summary>
        /// 验证该名字是否已经注册了
        /// </summary>
        /// <returns></returns>
        public ActionResult BoolStudentPut()
        {
            string Name = Request.QueryString["Name"];
            return Json(ListStudentPutOnRecord(Name).Count, JsonRequestBehavior.AllowGet);
        }
        //备案查询
        public ActionResult DataKeys()
        {
            string Name = Request.QueryString["Name"];
            List<StudentPutOnRecord> x = null;
            try
            {
                x = ListStudentPutOnRecord(Name);


            }
            catch (Exception ex)
            {

                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.加载数据);
            }

            var data = new
            {
                code = "",
                msg = "",
                count = x.Count,
                data = x
            };
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        //查询赋值
        public ActionResult NameKeys(int id)
        {
            NameKeysid = id;
            StudentDataKeepAndRecordBusiness dbctext = new StudentDataKeepAndRecordBusiness();
            try
            {
                var x = dbctext.GetList().Where(a => a.Id == id).FirstOrDefault();

                return Json(x, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.加载数据);
                return Json("加载数据异常", JsonRequestBehavior.AllowGet);
            }

        }
        //以身份证查询是否有重复学员
        public bool Isidentitydocument(string identitydocument)
        {

            var x = dbtext.GetList().Where(a => a.identitydocument == identitydocument && a.IsDelete != true).ToList();
            if (x.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        //注册学员编辑学员
        public ActionResult Enti(StudentInformation studentInformation, int? List)
        {
            redis.RemoveCache("StudentInformation");
            AjaxResult result = null;
            if (studentInformation.StudentNumber == null)
            {
                if (Isidentitydocument(studentInformation.identitydocument))
                {
                    try
                    {
                        studentInformation.StudentNumber = StudentID(studentInformation.identitydocument);
                        studentInformation.InsitDate = DateTime.Now;
                        studentInformation.Password = "000000";
                        studentInformation.StudentPutOnRecord_Id = NameKeysid;
                        studentInformation.IsDelete = false;
                        //dataKeepAndRecordBusiness.ChangeStudentState(NameKeysid);
                        dbtext.Insert(studentInformation);

                        ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                        scheduleForTrainees.ClassID = classschedu.GetEntity(List).ClassNumber;//班级名称
                        scheduleForTrainees.ID_ClassName = (int)List;//班级编号
                        scheduleForTrainees.CurrentClass = true;
                        scheduleForTrainees.StudentID = studentInformation.StudentNumber;
                        scheduleForTrainees.AddDate = DateTime.Now;
                        scheduleForTrainees.IsGraduating = false;
                        Stuclass.Insert(scheduleForTrainees);
                        redis.RemoveCache("StudentInformation");
                        // Stuclass.Remove("ScheduleForTrainees");
                        result = new SuccessResult();
                        result.Msg = "注册成功";
                        result.Success = true;
                        result.Data = studentInformation.StudentNumber;
                        //dbtext.Remove("StudentInformation");
                        BusHelper.WriteSysLog("注册学员成功", Entity.Base_SysManage.EnumType.LogType.添加数据);

                    }
                    catch (Exception ex)
                    {
                        result = new ErrorResult();
                        result.ErrorCode = 500;
                        result.Msg = "服务器错误1";

                        BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);

                    }
                }
                else
                {
                    result = new SuccessResult();
                    result.Success = false;
                    result.Msg = "身份证重复";
                }
            }
            else
            {
                try
                {
                    StudentInformation x = Finds(studentInformation.StudentNumber);
                    studentInformation.Password = x.Password;
                    //studentInformation.StudentPutOnRecord_Id = x.StudentPutOnRecord_Id;
                    studentInformation.InsitDate = x.InsitDate;
                    studentInformation.IsDelete = false;
                    dbtext.Update(studentInformation);
                    dbtext.GetList();
                    result = new SuccessResult();
                    result.Msg = "修改成功";
                    result.Success = true;
                    //   dbtext.Remove("StudentInformation");
                    BusHelper.WriteSysLog("修改学员信息成功", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                }
                catch (Exception ex)
                {
                    result = new ErrorResult();
                    result.ErrorCode = 500;
                    result.Msg = "服务器错误";

                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);

                }
            }



            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //按学号查询单个学员 find
        public StudentInformation Finds(string id)
        {
            var x = dbtext.GetList().Where(a => a.StudentNumber == id && a.IsDelete != true).FirstOrDefault();
            return x;
        }
        //按学号查询单个学员返回json格式
        public ActionResult Select()
        {
            CloudstorageBusiness db_Bos = new CloudstorageBusiness();
            string stuid = Request.QueryString["stuid"];
            var a = Finds(stuid);//Mylist("ScheduleForTrainees")
            var ClassID = Stuclass.GetList().Where(c => c.StudentID == a.StudentNumber).First().ClassID;
            var x = new
            {
                a.Familyphone,
                StudentNumber = a.StudentNumber,//学号
                Name = a.Name,//姓名
                InsitDate = a.InsitDate,//入校时间
                Education = a.Education,//学历
                Telephone = a.Telephone,//电话
                Familyaddress = a.Familyaddress,//家庭住址
                WeChat = a.WeChat,//微信
                qq = a.qq,//qq
                BirthDate = a.BirthDate,//出生日期
                identitydocument = a.identitydocument,//身份证号码
                Reack = a.Reack,//备注
                Traine = a.Traine,//学员性格
                Hobby = a.Hobby,//兴趣爱好
                Nation = a.Nation,//民族
                Sex = a.Sex,//性别,
                Guardian = a.Guardian,//亲属Mylist("ScheduleForTrainees").
                AddDate = Stuclass.GetList().Where(c => c.StudentID == a.StudentNumber).First().AddDate,//入班时间
                classa = Stuclass.ClassNames(stuid),//班级号
                Images = db_Bos.ImagesFine("xinxihua", "StudentImage",a.Picture,5)
                //a => a.IsDelete == false && a.ClassStatus == false
            };
          
            //classab = classschedu.GetList().Where(w => w.IsDelete == false&&w.ClassNumber== Stuclass.GetList().Where(c => c.StudentID == a.StudentNumber && c.CurrentClass == false).First().ClassID && w.ClassStatus == false).FirstOrDefault().ClassNumber //班级名称
            //a => a.IsDelete == false && a.ClassStatus == false
            return Json(x, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Viewdetails()
        {
            string stuid = Request.QueryString["id"];
            ViewBag.stuid = stuid;
            return View();
        }
        //修改密码视图
        [HttpGet]
        public ActionResult Password()
        {
            string stuid = Request.QueryString["stuid"];
            ViewBag.Password = stuid;
            return View();
        }
        //修改密码方法
        [HttpPost]
        public ActionResult Password(StudentInformation student)
        {
            var x = dbtext.GetList().Where(a => a.StudentNumber == student.StudentNumber && a.IsDelete != true).FirstOrDefault();
            x.Password = student.Password;
            AjaxResult result = null;
            try
            {
                dbtext.Update(x);
                result = new SuccessResult();
                result.Success = true;
                result.Msg = "修改成功";

                BusHelper.WriteSysLog("修改学员密码成功", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {

                result = new ErrorResult();
                result.ErrorCode = 500;
                result.Msg = "服务器错误";
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //查询备案是否存在
        public ActionResult Beian()
        {
            string Name = Request.QueryString["Name"];
            StudentDataKeepAndRecordBusiness dbctexta = new StudentDataKeepAndRecordBusiness();

            var x = dbctexta.GetList().Where(a => a.StuName.Trim() == Name).ToList();
            return Json(x, JsonRequestBehavior.AllowGet);
        }
        //根据阶段查找专业
        public ActionResult Stage(int id)
        {


            return Json(dbtext.Stage(id), JsonRequestBehavior.AllowGet);

        }
        //学费赋值
        public ActionResult GetFeestandard(int Stage, int Major_Id)
        {
            //int Stage = Convert.ToInt32(Request.QueryString["Stage"]);
            //int Major_Id = Convert.ToInt32(Request.QueryString["Major_Id"]);
            return Json(dbtext.GetFeestandard(Stage, Major_Id), JsonRequestBehavior.AllowGet);
        }
        //迟交信息
        public ActionResult Latetuitionfee(int id)
        {
            return Json(dbtext.Latetuitionfee(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Studenttuitionfeestandard(int id)
        {
            return Json(dbtext.Studenttuitionfeestandard(id), JsonRequestBehavior.AllowGet);
        }
        //对象存储业务类
        CloudstorageBusiness cloudstorage_Business = new CloudstorageBusiness();
        //添加学员照片规格宽144高192
        [HttpGet]
        public ActionResult AddStudentimg()
        {
            string studentid = Request.QueryString["studentid"]; ;
            ViewBag.studentid = studentid;
            ViewBag.student = JsonConvert.SerializeObject(dbtext.StuClass(studentid));
            ViewBag.Picture = dbtext.GetEntity(studentid).Picture == null ? "" : cloudstorage_Business.ImagesFine("xinxihua", "StudentImage", dbtext.GetEntity(studentid).Picture,5);
            return View();
        }
        //添加学员照片规格宽144高192
        [HttpPost]
        public ActionResult AddStudentimg(string id)
        {
            //    string studentid = Request.QueryString["studentid"];
            // Bitmap bitmap = new Bitmap(144,192);
            AjaxResult result = new AjaxResult();
            try
            {
                

                var fien = Request.Files[0];
                string filename = fien.FileName;
                string Extension = Path.GetExtension(filename);
                string newfilename = id + Extension;
              
                if (dbtext.StudentAddImg(id, newfilename) == true)
                {

                    result = new SuccessResult();
                    result.ErrorCode = 200;

                 var client=   cloudstorage_Business.BosClient();
                    // 以数据流形式上传Object
                   

                     cloudstorage_Business.PutObject("xinxihua", "StudentImage", newfilename,fien.InputStream);



                    //  bitmap.Save(path);
                }
            }
            catch (Exception ex)
            {

                result = new SuccessResult();
                result.ErrorCode = 300;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //验证图片是否已经有了
        public int boolImg(string studentid)
        {
            return dbtext.boolImg(studentid);
        }
        /// <summary>
        /// 获取班级
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentclassView()
        {
            //阶段
            GrandBusiness Grandcontext = new GrandBusiness();
            var Techarco = Techarcontext.GetList();

            Specialty specialty = new Specialty();
            specialty.SpecialtyName = "无";


            Techarco.Add(specialty);
            //专业
            ViewBag.Major_Id = Techarco.OrderBy(a => a.Id).Select(a => new SelectListItem { Text = a.SpecialtyName, Value = a.Id.ToString() });
            //阶段
            ViewBag.Stage = Grandcontext.GetList().Where(a => a.IsDelete == false).Select(a => new SelectListItem { Text = a.GrandName, Value = a.Id.ToString() });
            return View();

        }

        /// <summary>
        /// 获取班级
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ActionResult Classlist(int page, int limit,string ClassName, string Stage_id, string Major_Id)
        {
            //阶段
            GrandBusiness Grandcontext = new GrandBusiness();

            //班级学员业务类
            BaseBusiness<ScheduleForTraineesview> ScheduleForTraineesviewBusiness = new BaseBusiness<ScheduleForTraineesview>();

            var MyClass = classschedu.GetListBySql<ClassSchedule>("select *from ClassSchedule").Where(a => a.ClassstatusID == null).ToList();


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
            if (!string.IsNullOrEmpty(ClassName))
            {
                MyClass = MyClass.Where(a => a.ClassNumber.Contains(ClassName)).ToList();
            }
            var dataList = MyClass.Select(a => new
            {
                //  a.BaseDataEnum_Id,
                a.id,
                ClassNumber = a.ClassNumber,
                grade_Id = Grandcontext.GetEntity(a.grade_Id).GrandName, //阶段id
                Major_Id = a.Major_Id == null ? "暂无专业" : Techarcontext.GetEntity(a.Major_Id).SpecialtyName,//专业
                stuclasss = ScheduleForTraineesviewBusiness.GetListBySql<ScheduleForTraineesview>("select * from ScheduleForTraineesview where Classid=" + a.id).Count()//班级人数
            }).OrderByDescending(a => a.id).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = MyClass.Count,
                data = dataList
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IDcardphoto()
        {
            string studentid = Request.QueryString["studentid"];
            ViewBag.studentid = studentid;
            var student = dbtext.GetEntity(studentid);
            student.Identitybackimg = cloudstorage_Business.ImagesFine("xinxihua", "IDcardphotoImg/Identitybackimg", student.Identitybackimg, 5);
            student.Identityjustimg = cloudstorage_Business.ImagesFine("xinxihua", "IDcardphotoImg/Identityjustimg", student.Identityjustimg, 5);
            return View(student);
        }
        /// <summary>
        /// 身份证正面
        /// </summary>
        /// <returns></returns>
        public ActionResult Identityjustimg(string id)
        {
            AjaxResult result = new AjaxResult();
            var fien = Request.Files[0];
            string filename = fien.FileName;
            string Extension = Path.GetExtension(filename);
            string newfilename = id + "just" + Extension;
            try
            {
                if (dbtext.StudentIdentityImg(id, 1, newfilename) == true)
                {
                    result = new SuccessResult();
                    result.ErrorCode = 200;
                    cloudstorage_Business.PutObject("xinxihua", "IDcardphotoImg/Identityjustimg", newfilename, fien.InputStream);
                 
                }
            }
            catch (Exception ex)
            {
                result = new SuccessResult();
                result.ErrorCode = 300;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 身份证反面
        /// </summary>
        /// <returns></returns>
        public ActionResult Identitybackimg(string id)
        {
            AjaxResult result = new AjaxResult();
            var fien = Request.Files[0];
            string filename = fien.FileName;
            string Extension = Path.GetExtension(filename);
            string newfilename = id + "back" + Extension;
            //    StudentIdentityImg
            try
            {
                if (dbtext.StudentIdentityImg(id, 2, newfilename) == true)
                {
                    result = new SuccessResult();
                    result.ErrorCode = 200;
                    cloudstorage_Business.PutObject("xinxihua", "IDcardphotoImg/Identitybackimg", newfilename, fien.InputStream);
                 
                  
                }
            }
            catch (Exception ex)
            {
                result = new SuccessResult();
                result.ErrorCode = 300;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 学员基本资料编辑
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult StudentEnll()
        {
            List<StudentInformation> studentlist = new List<StudentInformation>();
            string studentid = Request.QueryString["studentid"];
         
            string[] stuid=  studentid.Split(',');
            foreach (var item in stuid)
            {
                if (item!="")
                {
                    studentlist.Add(dbtext.GetEntity(item));
                }
                
            }
            ViewBag.studentlist = studentlist;
            return View();
        }
        /// <summary>
        /// 学员基本信息修改
        /// </summary>
        /// <param name="studentInformation"></param>
        /// <returns></returns>
        public ActionResult StudentEnll(StudentInformation studentInformation)
        {
            AjaxResult result = null;
            try
            {
                StudentInformation x = dbtext.GetEntity(studentInformation.StudentNumber);
                studentInformation.Password = x.Password;
                studentInformation.StudentPutOnRecord_Id = x.StudentPutOnRecord_Id;
                studentInformation.InsitDate = x.InsitDate;
                studentInformation.IsDelete = false;
                dbtext.Update(studentInformation);
               
                result = new SuccessResult();
                result.Msg = "修改成功";
                result.Success = true;
                //   dbtext.Remove("StudentInformation");
                BusHelper.WriteSysLog("修改学员信息成功", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {

                result = new ErrorResult();
                result.ErrorCode = 500;
                result.Msg = "服务器错误";

                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return Json(result,JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 修改学生学号准确时间
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ye"></param>
        public void StudentXueGai(string id,bool ye)
        {
            List<StudentInformation> students = new List<StudentInformation>();
            var classname = classschedu.GetList().Where(a => a.ClassNumber == id).FirstOrDefault();
            //var student = Stuclass.ClassStudent(classname.id);
            var da = "";
            if (ye==true)
            {
               da= id.Substring(0, id.Length - 1);
            }
            else
            {
                da = id.Substring(2, 6);
            }
            var n = da.Substring(0, 2);
            var y = da.Substring(2, 2);
            var r = da.Substring(4, 2);
            //foreach (var item in student)
            //{
            //  item.StudentNumber=n+y+  item.StudentNumber.Substring(4, item.StudentNumber.Length - 4);
            //    item.InsitDate = Convert.ToDateTime("19" + n + "-" + y + "-" + r);
            //    students.Add(item);
            //}
            //    dbtext.Update(students);


            var x = Stuclass.GetList().Where(a => a.ID_ClassName == classname.id).ToList();
            foreach (var item in x)
            {
                var stuid = item.StudentID;
                var stu = item.StudentID.Substring(4, stuid.Length - 4);
                item.StudentID = n + y + stu;

            }
            Stuclass.Update(x);

        }
        /// <summary>
        /// 修改班级学号
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ye"></param>
        public void Classstu(string id, bool ye)
        {
            var da = "";
            var classname = classschedu.GetList().Where(a => a.ClassNumber == id).FirstOrDefault();
            var x = Stuclass.GetList().Where(a => a.ID_ClassName == classname.id&&a.AddDate> Convert.ToDateTime("2020-06-07")).ToList();

            if (ye == true)
            {
                da = id.Substring(0, id.Length - 1);
            }
            else
            {
                da = id.Substring(2, 6);
            }
            var n = da.Substring(0, 2);
            var y = da.Substring(2, 2);
            var r = da.Substring(4, 2);
            foreach (var item in x)
            {
                var stuid = item.StudentID;
                var stu = item.StudentID.Substring(4, stuid.Length - 4);
                item.StudentID = n + y + stu;

            }
            Stuclass.Update(x);

        }
        /// <summary>
        /// 删除重复的学生/删除前要修改key唯一键
        /// </summary>
        /// <returns></returns>
        public ActionResult MyStudenttext()
        {
            List<StudentInformation> stu= new List<StudentInformation>();
            List<StudentInformation> studentInformation = new List<StudentInformation>();
          var x=  dbtext.GetList();
            foreach (var item in x)
            {
              
                foreach (var item1 in studentInformation)
                {
                    if (item.identitydocument==item1.identitydocument)
                    {
                        stu.Add(item);
                    }
                }
                studentInformation.Add(item);

            }

            foreach (var item in stu)
            {
                Stuclass.Delete(Stuclass.GetList().Where(a => a.StudentID == item.StudentNumber).FirstOrDefault());
                dbtext.Delete(item);
            }
            return null;
        }
        /// <summary>
        /// 修改班级学号及学生学号
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentClassID()
        {
          var x=  classschedu.GetList();
            foreach (var item in x)
            {
                bool ye = item.grade_Id == 1 ? true : false;
                StudentXueGai(item.ClassNumber, ye);

            }
            return null;
        }

        //修改进班日期和注册日期
        public ActionResult Lubandate()
        {
            List<ScheduleForTrainees> scheduleForTrainees = new List<ScheduleForTrainees>();
            List<StudentInformation> studentInformation = new List<StudentInformation>();
            var da = "";
            var x = classschedu.GetList();
            foreach (var item in x)
            {
                if (item.grade_Id==1)
                {
                    da = item.ClassNumber.Substring(0, item.ClassNumber.Length - 1);
                }
                else if(item.grade_Id == 1002)
                {
                    da = item.ClassNumber.Substring(2, 6);
                }
                var n = da.Substring(0, 2);
                var y = da.Substring(2, 2);
                var r = da.Substring(4, 2);
                var dat = "20" + n + "-" + y + "-" + r;
                var student = Stuclass.GetList().Where(a => a.ID_ClassName == item.id).ToList();
                foreach (var item1 in student)
                {
                    item1.AddDate = Convert.ToDateTime(dat);
                    var stu = dbtext.GetEntity(item1.StudentID);
                    stu.InsitDate = Convert.ToDateTime(dat);
                    studentInformation.Add(stu);
                    scheduleForTrainees.Add(item1);
                }
            }
            Stuclass.Update(scheduleForTrainees);
            dbtext.Update(studentInformation);
            // var student = Stuclass.GetList();
            return null;
        }
        //删除班级数据（没有学生的数据）
        public ActionResult ClassStus()
        {
            List<ScheduleForTrainees> scheduleForTrainees = new List<ScheduleForTrainees>();
            var x = classschedu.GetList();
            foreach (var item in x)
            {
                var clssstu = Stuclass.GetList().Where(a => a.ID_ClassName == item.id&& a.AddDate > Convert.ToDateTime("2020-06-07")).ToList();
                foreach (var item1 in clssstu)
                {
                   if(dbtext.GetList().Where(a => a.StudentNumber == item1.StudentID).FirstOrDefault() == null)
                    {
                        scheduleForTrainees.Add(item1);
                    }
                }
            }
            Stuclass.Delete(scheduleForTrainees);
            return null;
        }
        //删除重复学生
        public ActionResult StuClass()
        {
            List<StudentInformation> studentInformation = new List<StudentInformation>();
           var x= dbtext.GetList();
            foreach (var item in x)
            {
                if (Stuclass.GetList().Where(a => a.StudentID == item.StudentNumber).FirstOrDefault()==null)
                {
                    studentInformation.Add(item);
                }
            }
            dbtext.Delete(studentInformation);
            return null;
        }
        //GetDate StudentClassView
        public ActionResult Classlists()
        {
            //阶段
            GrandBusiness Grandcontext = new GrandBusiness();
            var Techarco = Techarcontext.GetList();

            Specialty specialty = new Specialty();
            specialty.SpecialtyName = "无";


            Techarco.Add(specialty);
          
            //阶段
            ViewBag.Stage = Grandcontext.GetList().Where(a => a.IsDelete == false).Select(a => new SelectListItem { Text = a.GrandName, Value = a.Id.ToString() });
            return View();

        }

        public ActionResult ClassGetDate(int page, int limit, string ClassName, string Stage_id)
        {
            //班级状态表
            BaseBusiness<Classstatus> classtatus = new BaseBusiness<Classstatus>();
            HeadmasterBusiness Hadmst = new HeadmasterBusiness();
            GrandBusiness Grandcontext = new GrandBusiness();
            var EmpNumber = Base_UserBusiness.GetCurrentUser().EmpNumber;
            var id = Hadmst.GetList().Where(a => a.informatiees_Id == EmpNumber && a.IsDelete == false).FirstOrDefault().ID;

            var dataList = Hadmst.EmpClass(id,true).Select(a => new
            {
                //  a.BaseDataEnum_Id,
                a.id,
                ClassNumber = a.ClassNumber,
                ClassRemarks = a.ClassRemarks,
             
                grade_Id = Grandcontext.GetEntity(a.grade_Id).Id, //阶段id
                grade_Name = Grandcontext.GetEntity(a.grade_Id).GrandName, //阶段id
                stuclasss = Stuclass.GetList().Where(c => c.ID_ClassName == a.id && c.CurrentClass == true).Count()//班级人数
            }).ToList();
            if (!string.IsNullOrEmpty(ClassName))
            {
                dataList = dataList.Where(a => a.ClassNumber.Contains(ClassName)).ToList();
            }
            if (!string.IsNullOrEmpty(Stage_id))
            {
                int stage = int.Parse(Stage_id);

                dataList = dataList.Where(a => a.grade_Id == stage).ToList();
            }
            var dt=dataList.OrderBy(a => a.id).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = dataList.Count,
                data = dt
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 学号顺序优化
        /// </summary>
        /// <returns></returns>
        public ActionResult StuidPaixu()
        {
            BaseBusiness<StudentInformationview> stuview = new BaseBusiness<StudentInformationview>();
            List<StudentInformationview> studentInformationviews = new List<StudentInformationview>();
            
            List<StudentInformation> studentInformation = new List<StudentInformation>();
            var mingci = string.Empty;
            var x = dbtext.GetList() ;
            var count = 0;
            foreach (var item in x)
            {
                StudentInformationview z = new StudentInformationview();
                
                z.State = item.State;
                z.Sex = item.Sex;
                z.Reack = item.Picture;
                z.Password = item.Password;
                z.Nation = item.Nation;
                z.Name = item.Name;
                z.IsDelete = item.IsDelete;
                z.InsitDate = item.InsitDate;
                z.Identityjustimg = item.Identityjustimg;
                z.identitydocument = item.identitydocument;
                z.Identitybackimg = item.Identitybackimg;
                z.Hobby = item.Hobby;
                z.Guardian = item.Guardian;
                z.Familyphone = item.Familyphone;
                z.Familyaddress = item.Familyaddress;
                z.Education = item.Education;
                z.BirthDate = item.BirthDate;
                z.WeChat = item.WeChat;
                z.Traine = item.Traine;
                z.Telephone = item.Telephone;
                z.StudentPutOnRecord_Id = item.StudentPutOnRecord_Id;
                count++;
                if (count.ToString().Length < 2)
                    mingci = "0000" + count;
                else if (count.ToString().Length < 3)
                    mingci = "000" + count;
                else if (count.ToString().Length < 4)
                    mingci = "00" + count;
                else if (count.ToString().Length < 5)
                    mingci = "0" + count;
                else mingci = count.ToString();
               var stuid= item.StudentNumber.Substring(0, item.StudentNumber.Length - 5);
                z.StudentNumber  = stuid + mingci;
                studentInformationviews.Add(z);
            }

            stuview.BulkInsert(studentInformationviews);
            return null;
        }

         public ActionResult studenttext()
        {
            BaseBusiness<StudentInformationview> stuview = new BaseBusiness<StudentInformationview>();
            List<StudentInformation> studentInformation = new List<StudentInformation>();
            var x= stuview.GetList();
            foreach (var item in x)
            {
                StudentInformation z = new StudentInformation();
                z.StudentNumber = item.StudentNumber;
                z.State = item.State;
                z.Sex = item.Sex;
                z.Reack = item.Picture;
                z.Password = item.Password;
                z.Nation = item.Nation;
                z.Name = item.Name;
                z.IsDelete = item.IsDelete;
                z.InsitDate = item.InsitDate;
                z.Identityjustimg = item.Identityjustimg;
                z.identitydocument = item.identitydocument;
                z.Identitybackimg = item.Identitybackimg;
                z.Hobby = item.Hobby;
                z.Guardian = item.Guardian;
                z.Familyphone = item.Familyphone;
                z.Familyaddress = item.Familyaddress;
                z.Education = item.Education;
                z.BirthDate =item.BirthDate;
                z.WeChat = item.WeChat;
                z.Traine = item.Traine;
                z.Telephone = item.Telephone;
                z.StudentPutOnRecord_Id = item.StudentPutOnRecord_Id;
                studentInformation.Add(z);
            }

            dbtext.Insert(studentInformation);
            return null;
        }
        //修改班级身份证
        public ActionResult classidnti()
        {
            List<ScheduleForTrainees> scheduleForTrainees = new List<ScheduleForTrainees>();
            var classstu = Stuclass.GetList();
            
                foreach (var item in classstu)
                {
                item.identitydocument = dbtext.GetEntity(item.StudentID).identitydocument;
                scheduleForTrainees.Add(item);
                }
            Stuclass.Update(scheduleForTrainees);
            return null;
           }

        public ActionResult text2()
        {
            List<ScheduleForTrainees> scheduleForTrainees = new List<ScheduleForTrainees>();
            var classstu = Stuclass.GetList();
            var stu = dbtext.GetList();
            foreach (var item in classstu)
            {
                item.StudentID = stu.Where(a => a.identitydocument == item.identitydocument).FirstOrDefault().StudentNumber;
                scheduleForTrainees.Add(item);
            }
            Stuclass.Update(scheduleForTrainees);
            return null;
        }

        public ActionResult text3()
        {
            List<ScheduleForTrainees> scheduleForTrainees = new List<ScheduleForTrainees>();
            List<ScheduleForTrainees> s2 = new List<ScheduleForTrainees>();
            var classstu = Stuclass.GetList();
            foreach (var item in classstu)
            {
               
                foreach (var item1 in scheduleForTrainees)
                {
                    if (item.StudentID==item1.StudentID)
                    {
                        s2.Add(item);
                    }
                }
                scheduleForTrainees.Add(item);
            }
            return null;
        }

        public ActionResult text4()
        {
            List<StudentInformation> studentInformation = new List<StudentInformation>();
            List<StudentInformation> s2 = new List<StudentInformation>();
            var x=   dbtext.GetList();
            foreach (var item in x)
            {
              var st=  Stuclass.GetList().Where(a => a.StudentID == item.StudentNumber).FirstOrDefault();
                if (st==null)
                {
                    studentInformation.Add(item);
                }
            }
            return null;
        }

        public ActionResult  Text5()
        {
            List<ScheduleForTrainees> scheduleForTrainees = new List<ScheduleForTrainees>();
            List<ScheduleForTrainees> x1 = new List<ScheduleForTrainees>();
            var x = Stuclass.GetList().Where(a => a.ID_ClassName == 48).ToList();
            foreach (var item in x)
            {
                foreach (var item1 in scheduleForTrainees)
                {
                    if (item.StudentID==item1.StudentID)
                    {
                        x1.Add(item);
                    }
                }
                scheduleForTrainees.Add(item);
            }
            Stuclass.Delete(x1);



            return null;
        }

        public ActionResult text6()
        {
            // CloudstorageBusiness cloudstorage_Business = new CloudstorageBusiness();
            //var z= cloudstorage_Business.Listfiles("xinxihua", "StudentImage");
            //var client = cloudstorage_Business.BosClient();

            // var list = client.ListObjects("xinxihua", "Stu");

            // var imageStream = client.GetObject("xinxihua", "StudentImage/19042000091400212.jpg");
            // // 获取ObjectMeta
            // ObjectMetadata meta = imageStream.ObjectMetadata;

            // // 获取Object的输入流
            // Stream objectContent = imageStream.ObjectContent;
            // var img = Image.FromStream(objectContent);
            // return null;
           var x= dbtext.GetList().Where(a => a.InsitDate < Convert.ToDateTime("2020-01-01")).ToList().OrderByDescending(a => a.StudentNumber).ToList();
            return null;
           
        }
        /// <summary>
        /// 改学号
        /// </summary>
        /// <returns></returns>
        public ActionResult Text7()
        {
            List<StudentInformation> studentInformation = new List<StudentInformation>();
            List<ScheduleForTrainees> scheduleForTrainees = new List<ScheduleForTrainees>();
          var x=  dbtext.GetListBySql<StudentInformation>("select * from StudentInformation").ToList();
            foreach (var item in x)
            {
                scheduleForTrainees.Add(Stuclass.GetListBySql<ScheduleForTrainees>("select * from ScheduleForTrainees where StudentID='" + item.StudentNumber + "'").OrderBy(a => a.ID).FirstOrDefault());
            }

            foreach (var item in scheduleForTrainees)
            {
                var GrandName = classschedu.ClassGrand(item.ID_ClassName).GrandName;
                var student = dbtext.GetListBySql<StudentInformation>("select * from StudentInformation where StudentNumber='" + item.StudentID+"'").FirstOrDefault();
                if (GrandName == "S1")
                {
                    GrandName = "2";
                }
                else if (GrandName == "S2")
                {
                    GrandName = "3";
                }
                else if (GrandName == "S3")
                {
                    GrandName = "4";
                }
                else if (GrandName == "Y1")
                {
                    GrandName = "1";
                }

              var nyr=  student.StudentNumber.Substring(0, student.StudentNumber.Length - 5);
                var wei = student.StudentNumber.Substring(student.StudentNumber.Length - 4);
                student.StudentNumber = nyr + GrandName + wei;
                studentInformation.Add(student);
            }
            // dbtext.Update(studentInformation);
            dbtext.Update(studentInformation);
            x = studentInformation;
            scheduleForTrainees = new List<ScheduleForTrainees>();
            foreach (var item in x)
            {
              var z=  Stuclass.GetListBySql<ScheduleForTrainees>("select * from ScheduleForTrainees where identitydocument='" + item.identitydocument + "'").ToList();
                foreach (var item1 in z)
                {
                    item1.StudentID = item.StudentNumber;
                    scheduleForTrainees.Add(item1);
                }
            }

         
            Stuclass.Update(scheduleForTrainees);
            return null;
           
        }
        public ActionResult Text8()
        {
            BaseBusiness<DetailedStudentIn> a1 = new BaseBusiness<DetailedStudentIn>();
            BaseBusiness<ClassMembers> a2 = new BaseBusiness<ClassMembers>();
            BaseBusiness<ClassDynamics> a3 = new BaseBusiness<ClassDynamics>();
            BaseBusiness<ApplicationDropout> a4 = new BaseBusiness<ApplicationDropout>();
            BaseBusiness<ApplicationRepair> a5 = new BaseBusiness<ApplicationRepair>();
            BaseBusiness<Expels> a6 = new BaseBusiness<Expels>();
            BaseBusiness<Transfer> a7 = new BaseBusiness<Transfer>();
            BaseBusiness<StudentFeeRecord> a8 = new BaseBusiness<StudentFeeRecord>();
            BaseBusiness<InterviewStudents> a9 = new BaseBusiness<InterviewStudents>();
            BaseBusiness<Payview> a10 = new BaseBusiness<Payview>();
            var x = dbtext.GetListBySql<StudentInformation>("select * from StudentInformation").ToList();
            foreach (var item in x)
            {
                var nyr = item.StudentNumber.Substring(0, item.StudentNumber.Length - 5);
                var wei = item.StudentNumber.Substring(item.StudentNumber.Length - 4);
               string stuid = nyr + "0" + wei;
                var x1 = a1.GetList().Where(a => a.StudentID == stuid).ToList();
                foreach (var item1 in x1)
                {
                    item1.StudentID = item.StudentNumber;
                    a1.Update(item1);
                }
                var x2 = a2.GetList().Where(a => a.Studentnumber == stuid).ToList();
                foreach (var item1 in x2)
                {
                    item1.Studentnumber = item.StudentNumber;
                    a2.Update(item1);
                }
                var x3 = a3.GetList().Where(a => a.Studentnumber == stuid).ToList();
                foreach (var item1 in x3)
                {
                    item1.Studentnumber = item.StudentNumber;
                    a3.Update(item1);
                }
                var x4 = a4.GetList().Where(a => a.Studentnumber == stuid).ToList();
                foreach (var item1 in x4)
                {
                    item1.Studentnumber = item.StudentNumber;
                    a4.Update(item1);
                }
                var x5 = a5.GetList().Where(a => a.StudentID == stuid).ToList();
                foreach (var item1 in x5)
                {
                    item1.StudentID = item.StudentNumber;
                    a5.Update(item1);
                }
                var x6 = a6.GetList().Where(a => a.Studentnumber == stuid).ToList();
                foreach (var item1 in x6)
                {
                    item1.Studentnumber = item.StudentNumber;
                    a6.Update(item1);
                }
                var x7 = a7.GetList().Where(a => a.Studentnumber == stuid).ToList();
                foreach (var item1 in x7)
                {
                    item1.Studentnumber = item.StudentNumber;
                    a7.Update(item1);
                }
                var x8 = a8.GetList().Where(a => a.StudenID == stuid).ToList();
                foreach (var item1 in x8)
                {
                    item1.StudenID = item.StudentNumber;
                    a8.Update(item1);
                }
                var x9 = a9.GetList().Where(a => a.StudentNumberID == stuid).ToList();
                foreach (var item1 in x9)
                {
                    item1.StudentNumberID = item.StudentNumber;
                    a9.Update(item1);
                }
                var x10 = a10.GetList().Where(a => a.StudenID == stuid).ToList();
                foreach (var item1 in x10)
                {
                    item1.StudenID = item.StudentNumber;
                    a10.Update(item1);
                }
            }
            return null;
        }

        public void GetObject(BosClient client, String bucketName, String objectKey)
        {

            // 获取Object，返回结果为BosObject对象
            BosObject bosObject = client.GetObject(bucketName, objectKey);

            // 获取ObjectMeta
            ObjectMetadata meta = bosObject.ObjectMetadata;

            // 获取Object的输入流
            Stream objectContent = bosObject.ObjectContent;

    // 处理Object

    // 关闭流
    objectContent.Close();
        }

    }
}