using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Util;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SiliconValley.InformationSystem.Entity.Base_SysManage;
using SiliconValley.InformationSystem.Web.Common;
using SiliconValley.InformationSystem.Business.Messagenotification_Business;
using System.Configuration;
using SiliconValley.InformationSystem.Business.BaiduAPI_Business;
using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
using SiliconValley.InformationSystem.Business.ipRequestBusiness;

namespace SiliconValley.InformationSystem.Web.Controllers
{
    public class users
    {
        public int ID { get; set; }

        public string name { get; set; }

        public int age  { get; set; }

        public DateTime birthday { get; set; }
    }

   
    //[CheckLogin]
    public class HomeController : Controller
    {

        // /Home/DataSetToExcel
        public ActionResult Index()
        {
            // Business.Base_SysManage.Base_UserBusiness buser = new Business.Base_SysManage.Base_UserBusiness();
            // var list= buser.GetList();
            //return View(list);
            Business.Base_SysManage.Base_SysRoleBusiness brole = new Business.Base_SysManage.Base_SysRoleBusiness();
            var list = brole.GetList();
            return View(list);
        }

        public ActionResult ddd()
        {
            Business.Base_SysManage.Base_SysRoleBusiness brole = new Business.Base_SysManage.Base_SysRoleBusiness();
            var list = brole.GetList();

            errorMsg errorMsg = new errorMsg();
            errorMsg.errorCode = "d";

            return View("Index", errorMsg);
        }

        public ActionResult About()
        {
            

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TEXT()
        {
            return View();
        }

      

        public ActionResult test()
        {
            return View();
        }
        public ActionResult DataSetToExcel()
        {
          // string ss= ConfigHelper.GetConnectionString("BaseDb");

            //SqlConnection con = new SqlConnection("server=.;database=exsil;uid=sa;pwd=123;");
            //string sql = "select * from users";
            //con.Open();
            //SqlDataAdapter adapter = new SqlDataAdapter(sql, con);
            //DataSet ds = new DataSet();
            //adapter.Fill(ds);

           //// DataTable user = ds.Tables[0];

           // List<users> users = new List<users>();
           // users.Add(new users() { ID = 1, name = "唐敏", age = 19, birthday = "2000-08-24".ToDateTime() });
           // users.Add(new users() { ID = 1, name = "唐da敏", age = 19, birthday = "2000-08-24".ToDateTime() });
           // users.Add(new users() { ID = 1, name = "唐xiao敏", age = 19, birthday = "2000-08-24".ToDateTime() });
           // users.Add(new users() { ID = 1, name = "唐mm敏", age = 19, birthday = "2000-08-24".ToDateTime() });
           // DataTable user = users.ToDataTable<users>();
           // var userbaty = AsposeOfficeHelper.DataTableToExcelBytes(user);

           
           // int rowNumber = user.Rows.Count;
           // int columnNumber = user.Columns.Count;
           // int colIndex = 0;
           // if (rowNumber == 0)
           // {

           //     return Content("d"); ;
           // }
            

            //建立Excel对象 
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            
           
            excel.Application.Workbooks.Add(true);
            
            //excel.Application.Workbooks.Open()
            excel.Visible = true;//是否打开该Excel文件 

            //读取用户字段
            string jsonfile = Server.MapPath("/Config/user.json");

            System.IO.StreamReader file = System.IO.File.OpenText(jsonfile);
            //加载问卷
            JsonTextReader reader = new JsonTextReader(file);

            //转化为JObject
            JObject ojb = (JObject)JToken.ReadFrom(reader);

            var jj= ojb["user"].ToString();

            JObject jo = (JObject)JsonConvert.DeserializeObject(jj);
            ////生成字段名称 
            //foreach (DataColumn col in user.Columns)
            //{
            //    colIndex++;
            //    excel.Cells[1, colIndex] = jo[col.ColumnName] ;
            //}

            //填充数据 
            for (int c = 1; c <=3; c++)
            {

                for (int j = 0; j < 3; j++)
                {
                    var ss = excel.Cells[c + 1, j + 1];
                }
            }

            return Content("ds");
        }

        public ActionResult DATATEST()
        {

          

           var  ss= Request.Files[0].FileName;

            Request.Files[0].SaveAs(Server.MapPath("/uploadXLSXfile/"+ss));

            //insert(ss);
            DataTable table = AsposeOfficeHelper.ReadExcel(Server.MapPath(@"\uploadXLSXfile\" + ss));

            //SessionHelper.Session["data"] = table;
            //SessionHelper.Session["url"] = ss;


            List<Base_User> userlist = new List<Base_User>();

            foreach (DataRow item in table.Rows)
            {
                Base_User user = new Base_User();
                user.RealName = item["姓名"].ToString();
                user.Birthday = DateTime.Parse(item["生日"].ToString());

                userlist.Add(user);
            }

            return View("show",userlist);
        }


        public ActionResult show()
        {
            DataTable table = SessionHelper.Session["data"] as DataTable;

            List<Base_User> userlist = new List<Base_User>();

            foreach (DataRow item in table.Rows)
            {
                Base_User user = new Base_User();
                user.RealName = item["Name"].ToString();
                user.Birthday =DateTime.Parse( item["birthday"].ToString());

                userlist.Add(user);
            }


            return View(userlist);


        }

        public ActionResult rtest()
        {
            return View();
        }

        public ActionResult insert()
        {
            
            DataTable table= AsposeOfficeHelper.ReadExcel(Server.MapPath("/uploadXLSXfile/湖南硅谷云教育科技有限公司_考勤报表_20191101-20191130(1).xlsx"));

            SqlConnection con = new SqlConnection("server=.;database=exsil;uid=sa;pwd=123;");

            con.Open();
            SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con);

            List<string> arry = new List<string> {"1","2","3","4","5","六","日","姓名","部门"};

            sqlBulkCopy.DestinationTableName = "users";
            sqlBulkCopy.BatchSize = table.Rows.Count;
            foreach (var item in arry)
            {
                try
                {
                    sqlBulkCopy.ColumnMappings.Add(item, Guid.NewGuid().ToString());
                }
                catch (Exception)
                {
                    
                }
            }
            
            sqlBulkCopy.WriteToServer(table);

            return Content("ok");
        }

        public ActionResult IdCard()
        {

            return View();
        }

        public ActionResult Identification()
        {
            AjaxResult result = new AjaxResult();
            var file = Request.Files[0];
            var appid = ConfigurationManager.AppSettings["AppID"].ToString();
            var API_Key = ConfigurationManager.AppSettings["API_Key"].ToString();
            var Secret_Key = ConfigurationManager.AppSettings["Secret_Key"].ToString();
            //创建请求路由
            var client = new Baidu.Aip.Ocr.Ocr(API_Key, Secret_Key);
            var idCardSide = "front";
            var imageByte = file.InputStream.ReadToBytes();
            try
            {
                var res = client.Idcard(imageByte, idCardSide);
                var root = res.Root.ToString();
                var data = IdentificationBusines.GetFrontInfo(root);
                result.ErrorCode = 200;
                result.Msg = "识别成功";
                result.Data = data;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "异常";
                result.Data = null;
            }
           
             
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Scores(string grandid, string classid)
        {
            ExamScoresBusiness dbscore = new ExamScoresBusiness();
            dbscore.ClassScores(int.Parse(classid), int.Parse(grandid));
            return View();
        }

        public ActionResult stdentScores(string studentnumber)
        {
            ExamScoresBusiness dbscore = new ExamScoresBusiness();
            dbscore.StudentScores(studentnumber);
            return View();
        }

    }
}