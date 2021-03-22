using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace SiliconValley.InformationSystem.Web.Areas.Teachingquality.Controllers
{
    [CheckLogin]
    public class InterviewStudentsController : Controller
    {
        // GET: Teachingquality/InterviewStudents
        //学员班级表
        ScheduleForTraineesBusiness Stuclass = new ScheduleForTraineesBusiness();
        //学员表
        StudentInformationBusiness student = new StudentInformationBusiness();
        //员工表
        EmployeesInfoManage infoBusiness = new EmployeesInfoManage();
        //班级表
        ClassScheduleBusiness classschedu = new ClassScheduleBusiness();
        //实例化
        private readonly InterviewStudentsBusiness dbtext;
        public InterviewStudentsController()
        {
            dbtext = new InterviewStudentsBusiness();
        }
        //当前登陆人
        Base_UserModel user = Base_UserBusiness.GetCurrentUser();
        //主页面
        public ActionResult Index()
        {
            return View();
        }
        //对象储存测试
        //public class SQLCommon
        //{
        //    private string connectionStr;

        //    public SQLCommon(string connectionStr)
        //    {
        //        this.connectionStr = connectionStr;
        //    }
        //}
        //private static string connectionStr = "server=.;database=Coldairarrow.Fx.Net.Easyui.GitHub;uid=sa;pwd=123456";
        //SQLCommon SQLC = new SQLCommon(connectionStr);
        //public DataTable GetsSQLData(string MYSQLCODE, SqlParameter[] SQlCMDpas)
        //{
        //    SqlConnection Sqlcon = new SqlConnection(connectionStr);
        //    DataTable dt = new DataTable();
        //    Sqlcon.Open();
        //    SqlCommand com = new SqlCommand(MYSQLCODE, Sqlcon);
        //    com.CommandType = CommandType.StoredProcedure;
        //    com.Parameters.AddRange(SQlCMDpas);
        //    SqlDataReader sdr = com.ExecuteReader();
        //    if (sdr.HasRows)
        //    {
        //        while (sdr.Read())
        //        {
        //            string id = sdr["Id"].ToString();
                    
        //        }
                
        //    }
        //    //SqlDataAdapter sqlda = new SqlDataAdapter(MYSQLCODE, Sqlcon);
        //    //sqlda.SelectCommand.Parameters.AddRange(SQlCMDpas);
        //   // sqlda.SelectCommand.CommandType = CommandType.StoredProcedure;
        //    //sqlda.Fill(dt);
        //    return dt;
        //}
        //主页面获取表格数据
        public ActionResult GetDate(int page, int limit, string Name, string StudentNumberID, string qBeginTime, string InterviewTopicsm, string qEndTime)
        {
            //SqlCommand SQLC = new SqlCommand(connectionStr);
            //SqlParameter[] sqlpar = new SqlParameter[2];
            //sqlpar[0] = new SqlParameter("@StudentNumberID", "19062003122710694");
            //sqlpar[1] = new SqlParameter("@InterviewTopicsm", "202005300054");


            //Console.WriteLine(sqlpar[0]);
            //DataTable dt = GetsSQLData("proc_InterviewStudents", sqlpar);
           

            var list = new List<InterviewStudents>();
            if (user.UserName == "Admin")
            {
                list = dbtext.GetList().Where(a => a.IsDelete == false).ToList();
            }
            else
            {
                list = dbtext.GetList().Where(a => a.IsDelete == false && a.InterviewRecorderID == user.EmpNumber).ToList();
            }

            if (!string.IsNullOrEmpty(Name))
            {
                var x = student.GetList().Where(a => a.IsDelete == false && a.Name.Contains(Name)).ToList();
                List<InterviewStudents> interviewStudents = new List<InterviewStudents>();
                foreach (var item in x)
                {
                    var stu = list.Where(a => a.StudentNumberID == item.StudentNumber).FirstOrDefault();
                    if (stu != null)
                    {
                        interviewStudents.Add(stu);
                    }

                }
                list = interviewStudents;
            }
            if (!string.IsNullOrEmpty(StudentNumberID))
            {

                list = list.Where(a => a.StudentNumberID.Contains(StudentNumberID)).ToList();
            }
            if (!string.IsNullOrEmpty(InterviewTopicsm))
            {
                list = list.Where(a => a.InterviewTopics.Contains(InterviewTopicsm)).ToList();
            }
            if (!string.IsNullOrEmpty(qBeginTime))
            {
                list = list.Where(a => a.Dateofinterview >= Convert.ToDateTime(qBeginTime)).ToList();
            }
            if (!string.IsNullOrEmpty(qEndTime))
            {
                list = list.Where(a => a.Dateofinterview <= Convert.ToDateTime(qEndTime)).ToList();
            }


            var mylist = list.Select(a => new
            {
                ID = a.ID,
                InterviewTopics = a.InterviewTopics, //谈话标题
                Interviewcontent = a.Interviewcontent, //谈话内容
                StudentNumberID = a.StudentNumberID,//谈话学员学号
                StudentNumberName = student.GetEntity(a.StudentNumberID).Name,//谈话学员姓名
                StuClass = Stuclass.GetList().Where(c => c.StudentID == a.StudentNumberID && c.CurrentClass == true).FirstOrDefault().ClassID, //谈话学员班级
                InterviewRecorderID = a.InterviewRecorderID, //记录人编号
                InterviewRecorderName = infoBusiness.GetEntity(a.InterviewRecorderID).EmpName, //姓名
                Dateofinterview = a.Dateofinterview //谈话日期
            }).ToList();
            var dataList = mylist.OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();

            var data = new
            {
                code = "",
                msg = "",
                count = mylist.Count,
                data = dataList
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        //添加 编辑学员谈话记录
        public ActionResult StudIenterEntiy()
        {
            string id = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(id) && id != "undefined")
            {

                ViewBag.Name = "修改学员访谈记录";
                ViewBag.id = id;
                return View();

            }
            ViewBag.id = "";
            ViewBag.Name = "登记学员访谈记录";
            return View();
        }

        //查询数据库是否由重复数据
        public ActionResult Errnt(InterviewStudents interviewStudents)
        {

            string tu = interviewStudents.StudentNumberID.Substring(0, interviewStudents.StudentNumberID.Length);
            //转成数组
            string[] stu = tu.Split(',');
            var list = dbtext.GetList();
            List<InterviewStudents> mylist = new List<InterviewStudents>();
            for (int i = 0; i < stu.Length; i++)
            {

                var count = list.Where(a => a.StudentNumberID == stu[i] && a.InterviewTopics == interviewStudents.InterviewTopics && a.Dateofinterview == interviewStudents.Dateofinterview).ToList();
                if (count.Count > 0)
                {
                    mylist.AddRange(count);
                }
            }

            return Json(mylist.Select(a => new { Name = student.GetEntity(a.StudentNumberID).Name, StudentID = a.StudentNumberID, Dateofinterview = a.Dateofinterview }), JsonRequestBehavior.AllowGet);


        }

        //数据提交到数据库
        [HttpPost]
        public ActionResult StudIenterEntiy(InterviewStudents interviewStudents)
        {
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            string EmpNumber = user.EmpNumber;
            AjaxResult result = null;
            //多个学号组成后面截取逗号
            if (interviewStudents.ID > 0)
            {
                try
                {
                    var x = dbtext.GetEntity(interviewStudents.ID);
                    interviewStudents.InterviewRecorderID = x.InterviewRecorderID;
                    interviewStudents.Dateofinterview = x.Dateofinterview;
                    interviewStudents.IsDelete = x.IsDelete;
                    interviewStudents.StudentNumberID = x.StudentNumberID;
                    dbtext.Update(interviewStudents);
                    result = new SuccessResult();
                    result.Msg = "修改谈话记录成功";
                    result.Success = true;
                    BusHelper.WriteSysLog("修改谈话记录成功", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                }
                catch (Exception ex)
                {


                    result = new ErrorResult();
                    result.ErrorCode = 500;
                    result.Success = false;
                    result.Msg = "服务器错误";
                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            else
            {
                string tu = interviewStudents.StudentNumberID.Substring(0, interviewStudents.StudentNumberID.Length);
                //转成数组
                // Session["EmpNumber"].ToString();

                string[] stu = tu.Split(',');

                try
                {
                    List<InterviewStudents> list = new List<InterviewStudents>();
                    for (int i = 0; i < stu.Length; i++)
                    {
                        InterviewStudents Students = new InterviewStudents();
                        Students.InterviewTopics = interviewStudents.InterviewTopics;
                        Students.Dateofinterview = interviewStudents.Dateofinterview;
                        Students.Interviewcontent = interviewStudents.Interviewcontent;
                        Students.InterviewRecorderID = EmpNumber;
                        Students.StudentNumberID = stu[i];
                        Students.Addtime = DateTime.Now;
                        Students.IsDelete = false;

                        list.Add(Students);
                    }
                    dbtext.BulkInsert(list);
                    result = new SuccessResult();
                    result.Msg = "记录成功";
                    result.Success = true;
                    BusHelper.WriteSysLog("谈话记录成功", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
                catch (Exception ex)
                {

                    result = new ErrorResult();
                    result.ErrorCode = 500;
                    result.Success = false;
                    result.Msg = "服务器错误";
                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);

                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }

        //编辑页面赋值数据
        public ActionResult Select(int id)
        {
            try
            {
                var x = dbtext.GetList().Where(a => a.ID == id && a.IsDelete == false).Select(c => new
                {
                    ID = c.ID,
                    InterviewTopics = c.InterviewTopics,
                    Interviewcontent = c.Interviewcontent,
                    Dateofinterview = c.Dateofinterview,
                    StudentNumberID = student.GetList().Where(w => w.StudentNumber == c.StudentNumberID && w.IsDelete != true).FirstOrDefault().Name


                });
                return Json(x, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
                return Json("数据异常", JsonRequestBehavior.AllowGet);
            }

        }
        /// <summary>
        /// 学员家长访谈记录
        /// </summary>
        /// <returns></returns>
        public ActionResult ParentinterviewHome()
        {
            return View();
        }

        // 学员家长访谈业务类型
        BaseBusiness<InterviewRecordsof> intervirereco = new BaseBusiness<InterviewRecordsof>();
        /// <summary>
        /// 学员家长访谈记录
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="Name">姓名</param>
        /// <param name="StudentNumberID">学号</param>
        /// <param name="qBeginTime">开设时间</param>
        /// <param name="InterviewTopicsm">谈话标题</param>
        /// <param name="qEndTime">结束日期</param>
        /// <returns></returns>
        public ActionResult ParentinterviewGetDate(int page, int limit, string Name, string StudentNumberID, string qBeginTime, string InterviewTopicsm, string qEndTime)
        {
            var list = new List<InterviewRecordsof>();
            if (user.UserName == "Admin")
            {
                list = intervirereco.GetList().Where(a => a.IsDelete == false).ToList();
            }
            else
            {
                list = intervirereco.GetList().Where(a => a.IsDelete == false && a.Empnumber == user.EmpNumber).ToList();
            }
            if (!string.IsNullOrEmpty(Name))
            {
                var x = student.GetList().Where(a => a.IsDelete == false && a.Name.Contains(Name)).ToList();
                List<InterviewRecordsof> interviewStudents = new List<InterviewRecordsof>();
                foreach (var item in x)
                {
                    var stu = list.Where(a => a.Studentnumber == item.StudentNumber).FirstOrDefault();
                    if (stu != null)
                    {
                        interviewStudents.Add(stu);
                    }

                }
                list = interviewStudents;
            }
            if (!string.IsNullOrEmpty(StudentNumberID))
            {

                list = list.Where(a => a.Studentnumber.Contains(StudentNumberID)).ToList();
            }
            if (!string.IsNullOrEmpty(InterviewTopicsm))
            {
                list = list.Where(a => a.InterviewTopics.Contains(InterviewTopicsm)).ToList();
            }
            if (!string.IsNullOrEmpty(qBeginTime))
            {
                list = list.Where(a => a.Interviewtime >= Convert.ToDateTime(qBeginTime)).ToList();
            }
            if (!string.IsNullOrEmpty(qEndTime))
            {
                list = list.Where(a => a.Interviewtime <= Convert.ToDateTime(qEndTime)).ToList();
            }

            var mylist = list.Select(a => new
            {
                ID = a.ID,
                a.InterviewTopics, //谈话标题
                a.Interviewcontent, //谈话内容
                a.Studentnumber,//谈话学员学号
                StudentNumberName = student.GetEntity(a.Studentnumber).Name,//谈话学员姓名
                a.Parent,//学员家长
                a.Empnumber, //记录人编号
                InterviewRecorderName = infoBusiness.GetEntity(a.Empnumber).EmpName, //员工姓名
                a.Interviewtime //谈话日期
            }).ToList();
            var dataList = mylist.OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = dataList.Count,
                data = dataList
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加学员家长访谈页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ParentinterviewEntiy()
        {
            return View();
        }


        [HttpPost]
        /// <summary>
        /// 添加家长访谈数据
        /// </summary>
        /// <param name="interviewRecordsof"></param>
        /// <returns></returns>
        public ActionResult ParentinterviewEntiy(InterviewRecordsof interviewRecordsof)
        {
            Base_UserModel user = Base_UserBusiness.GetCurrentUser();
            string EmpNumber = user.EmpNumber;
            AjaxResult result = null;
            string tu = interviewRecordsof.Studentnumber.Substring(0, interviewRecordsof.Studentnumber.Length);
            //转成数组
            // Session["EmpNumber"].ToString();

            string[] stu = tu.Split(',');

            try
            {
                List<InterviewRecordsof> list = new List<InterviewRecordsof>();
                for (int i = 0; i < stu.Length; i++)
                {
                    InterviewRecordsof Students = new InterviewRecordsof();
                    Students.InterviewTopics = interviewRecordsof.InterviewTopics;
                    Students.Interviewtime = interviewRecordsof.Interviewtime;
                    Students.Interviewcontent = interviewRecordsof.Interviewcontent;
                    Students.Empnumber = EmpNumber;
                    Students.Studentnumber = stu[i];
                    Students.Addtime = DateTime.Now;
                    Students.Parent = interviewRecordsof.Parent;
                    Students.IsDelete = false;

                    list.Add(Students);
                }
                intervirereco.Insert(list);
                result = new SuccessResult();
                result.Msg = "记录成功";
                result.Success = true;
                BusHelper.WriteSysLog("谈话记录成功", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                result = new ErrorResult();
                result.ErrorCode = 500;
                result.Success = false;
                result.Msg = "服务器错误";
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取学员家长谈话
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult ParentinterviewFine(int ID)
        {
            var x = intervirereco.GetEntity(ID);
            var data = new
            {
                x.Studentnumber,
                x.Parent,
                x.InterviewTopics,
                x.Interviewtime,
                x.Interviewcontent,
                student.GetEntity(x.Studentnumber).Name,
                Empnumber = infoBusiness.GetEntity(x.Empnumber).EmpName
            };
            ViewBag.intervirereco = JsonConvert.SerializeObject(data);
            return View();
        }
    }

 
}