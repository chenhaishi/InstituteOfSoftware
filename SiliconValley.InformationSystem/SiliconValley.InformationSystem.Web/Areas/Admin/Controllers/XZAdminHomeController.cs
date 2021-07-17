using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Admin;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Admin.Controllers
{
    public class XZAdminHomeController : Controller
    {
        XYKAdminHome_Business AdminBind = new XYKAdminHome_Business();
        // GET: Admin/XZAdminHome
        [HttpGet]
        public ActionResult AdminHomeIndex()
        {
            ViewBag.StudentCount = AdminBind.StudentCount();//学校在校人
            ViewBag.StudentExpel = AdminBind.StudentExpel();//统计开除总人数
            ViewBag.StudentDropout = AdminBind.StudentDropout();//退学统计
            ViewBag.StudentSuspensionof = AdminBind.StudentSuspensionof();//休学统计
            ViewBag.StudentClassSum = AdminBind.StudentClassSum();//获取所有班级
            return View();
        }
        /// <summary>
        /// 升学视图绑定方法
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>

        public ActionResult GetCharts(string date)
        {
            BaseBusiness<Entity.ViewEntity.View_StudentAvg> view_StudentAvg = new BaseBusiness<Entity.ViewEntity.View_StudentAvg>();
            ArrayList sAxisData = new ArrayList();//升学总人数
            ArrayList yAxisData = new ArrayList();//阶段
            ArrayList xAxisData = new ArrayList();//未升学率总人数
            ArrayList sum_xAxisData = new ArrayList();//升学率
            var grand = AdminBind.StudentGrand();
            foreach (var item in grand)
            {
                yAxisData.Add(item.GrandName);
            }

            if (date == null)
            {
                var sql1 = GetDataTable("select grade_Id,sum(班级人数) as countsum from (select *, (select count(studentId) from ScheduleForTrainees where ClassID = cs.ClassNumber and CurrentClass = 0) as 班级人数 from ClassSchedule cs) as temp where ClassstatusID = 4 and grade_Id!=1002 group by grade_Id ");//获取本机年份
                for (int i = 0; i < sql1.Rows.Count; i++)
                {
                    sAxisData.Add(sql1.Rows[i].ItemArray[1]);
                    
                }
            }
            else
            {
                var tabel = GetDataTable("select grade_Id,sum(班级人数) as countsum from (select *, (select count(studentId) from ScheduleForTrainees where ClassID = cs.ClassNumber and CurrentClass = 0 and AddDate like '%" + date + "%') as 班级人数 from ClassSchedule cs) as temp where ClassstatusID = 4 and grade_Id!=1002 group by grade_Id ");//筛选时间
                for (int i = 0; i < tabel.Rows.Count; i++)
                {
                    sAxisData.Add(tabel.Rows[i].ItemArray[1]);
                }
            }
            if (date == null)
            {
                var sql2 = GetDataTable("select c.grade_Id,count(*) from StudentInformation  S inner JOIN ScheduleForTrainees sh  on s.StudentNumber = sh.StudentID inner join ClassSchedule  c on c.id = sh.ID_ClassName where   s.State is not null and s.State != 8 and s.State != 9 AND s.State != 3 and s.State != 2 and s.State != 7 and  CurrentClass = 0 and grade_Id!=1002 and not exists(select  1  from ScheduleForTrainees AS SC where sh.StudentID = sc.StudentID and  sh.AddDate < sc.AddDate)group by c.grade_Id");//获取本机年份
                for (int i = 0; i < sql2.Rows.Count; i++)
                {
                    xAxisData.Add(sql2.Rows[i].ItemArray[1]);
                }
            }
            else
            {
                var tabel_1 = GetDataTable("select c.grade_Id,count(*) from StudentInformation  S inner JOIN ScheduleForTrainees sh  on s.StudentNumber = sh.StudentID inner join ClassSchedule  c on c.id = sh.ID_ClassName where   s.State is not null and s.State != 8 and s.State != 9 AND s.State != 3 and s.State != 2 and s.State != 7 and  CurrentClass = 0 and grade_Id!=1002 and AddDate like '%" + date + "%' and not exists(select  1  from ScheduleForTrainees AS SC where sh.StudentID = sc.StudentID and  sh.AddDate < sc.AddDate)group by c.grade_Id");//筛选时间
                for (int i = 0; i < tabel_1.Rows.Count; i++)
                {
                    xAxisData.Add(tabel_1.Rows[i].ItemArray[1]);
                }
            }
           
        
            var result = new
            {
                name = yAxisData,
                sum = xAxisData,
                count= sAxisData
            };
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        //public class sAxisData
        //{
        //    public string Sum_S { get; set; }
        //}
        //public class xAxisData
        //{
        //    public int Sum_X { get; set; }
        //}

        /// <summary>
        /// Data数据库连接方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private DataTable GetDataTable(string sql)
        {
            //连接数据库
            string config = @"Data Source=106.13.104.179;Initial Catalog=Coldairarrow.Fx.Net.Easyui.GitHub;User ID=sa;Password=tangdan2020@";
            try
            {
                using (SqlConnection conn = new SqlConnection(config))
                {
                    //打开数据库连接
                    conn.Open();

                }
            }
            catch (Exception ex)
            {
                this.Response.Write("<script language='javascript'>alert('连接失败！')</script>");
            }
            //创建SqlDataAdapter对象并执行sql语句
            SqlDataAdapter sda = new SqlDataAdapter(sql, config);
            DataTable dt = new DataTable();
            //将数据填充到数据表中
            sda.Fill(dt);
            dt.Dispose();
            return dt;
        }
    }
}