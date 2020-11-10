using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SiliconValley.InformationSystem.Web.Areas.Educational.Controllers
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
    using SiliconValley.InformationSystem.Business.EducationalBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.Print;
    using SiliconValley.InformationSystem.Util;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Linq;

    [CheckLogin]
    public class TeacherAddorBeonDutyController : Controller
    {
        // GET: /Educational/TeacherAddorBeonDuty/AddDataView
        TeacherAddorBeonDutyManager Tb_Entity = new TeacherAddorBeonDutyManager();
        ClassScheduleBusiness ClassSchedule_Entity = new ClassScheduleBusiness();
        //值班数据导出

        public ActionResult dutyDataToExcel(string dutyTime)
        {
            DateTime dt = Convert.ToDateTime(dutyTime.Substring(0, 4) + "-" + dutyTime.Substring(5, 2));

            string sql = $"select * from TeacherAddorBeonDutyView where Year(Anpaidate)='"+dt.Year+"' and MONTH(Anpaidate)='"+dt.Month+"'";
            List<TeacherAddorBeonDutyView> list = Tb_Entity.GetListBySql<TeacherAddorBeonDutyView>(sql);
            var ajaxresult = new AjaxResult();
            
            var workbook = new HSSFWorkbook();

            //创建工作区
            var sheet = workbook.CreateSheet("值班统计");

            #region 表头样式

            HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

            HeadercellStyle.Alignment = HorizontalAlignment.Center;
            HeadercellFont.IsBold = true;

            HeadercellStyle.SetFont(HeadercellFont);

            #endregion

            HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

            ContentcellStyle.Alignment = HorizontalAlignment.Center;

            CreateHeader();

            int num = 1;

            GrandBusiness dbgrand = new GrandBusiness();

            list.ForEach(d =>
            {
                var row = (HSSFRow)sheet.CreateRow(num);

                CreateCell(row, ContentcellStyle, 0, d.EmpName);
                CreateCell(row, ContentcellStyle, 1, d.ClassroomName);
                CreateCell(row, ContentcellStyle, 2, d.AttendDate.ToString());
                CreateCell(row, ContentcellStyle, 3, d.ClassNumber);
                num++;

            });

            string path1 = System.AppDomain.CurrentDomain.BaseDirectory.Split('\\')[0];    //获得项目的基目录
            var Path = System.IO.Path.Combine(path1, "\\XinxihuaData\\Excel");
            if (!System.IO.Directory.Exists(Path))     //判断是否有该文件夹
                System.IO.Directory.CreateDirectory(Path); //如果没有在Uploads文件夹下创建文件夹Excel
            string saveFileName = Path + "\\" + "值班统计" + ".xlsx"; //路径+表名+文件类型
            try
            {
                FileStream fs = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);  //写入文件
                workbook.Close();  //关闭
                ajaxresult.ErrorCode = 200;
                ajaxresult.Msg = "导入成功！文件地址：" + saveFileName;
                // ajaxresult.Data = list;

            }
            catch (Exception ex)
            {
                ajaxresult.ErrorCode = 100;
                ajaxresult.Msg = "导入失败，" + ex.Message;

            }
            return Json(ajaxresult, JsonRequestBehavior.AllowGet);

            void CreateHeader()
            {
                HSSFRow Header = (HSSFRow)sheet.CreateRow(0);
                Header.HeightInPoints = 40;

                CreateCell(Header, HeadercellStyle, 0, "员工姓名");

                CreateCell(Header, HeadercellStyle, 1, "教室");

                CreateCell(Header, HeadercellStyle, 2, "值班日期");

                CreateCell(Header, HeadercellStyle, 3, "班级");
                
            }

            void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
            {
                HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                Header_Name.SetCellValue(value);

                Header_Name.CellStyle = TcellStyle;
            }


        }

        public ActionResult TeacherAddorBeonDutyIndex()
        {
            //判断登录的是否是教务
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            ViewBag.emp = Tb_Entity.Isjiaowu(UserName.EmpNumber);
            ////获取
            EmployeesInfo employees = Tb_Entity.EmployeesInfo_Entity.FindEmpData(UserName.EmpNumber, true);

            Position position = Tb_Entity.EmployeesInfo_Entity.GetPosition(employees.PositionId);

            List<EmployeesInfo> listemp = Tb_Entity.EmployeesInfo_Entity.GetEmpsByDeptid(position.DeptId);

            List<SelectListItem> teacherlist = listemp.Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();
            teacherlist.Add(new SelectListItem() { Text = "--请选择--", Value = "0" });
            teacherlist = teacherlist.OrderBy(t => t.Value).ToList();
            ViewBag.teacher = teacherlist;

            List<ClassSchedule> classSchedules =ClassSchedule_Entity.GetList().ToList();
            List<SelectListItem> ClassList = classSchedules.Select(a=>new SelectListItem() { Text = a.ClassNumber,Value=a.ClassNumber }).ToList();
            ClassList = ClassList.OrderBy(t => t.Value).ToList() ;
            ViewBag.ClassName = ClassList;
            return View();
        }

        /// <summary>
        /// 获取选中的班级某个日期的晚自习情况
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetClassData()
        {
            int classid = Convert.ToInt32(Request.Form["classid"]);
            DateTime time = Convert.ToDateTime(Request.Form["date"]);
            AjaxResult a = new AjaxResult();
            var list = Tb_Entity.EvningSelf_Entity.GetSQLDat("select * from EvningSelfStudy where ClassSchedule_id=" + classid + " and Anpaidate='" + time + "'");
            if (list.Count > 0)
            {
                a.Success = true;
                a.Data = list;
                a.Msg = "数据查询成功！";
            }
            else
            {
                a.Success = false;
                a.Msg = "没有安排晚自习请联系教务！";
            }

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 第一次数据加载
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Tabledata(int limit, int page)
        {

            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            if (Tb_Entity.Isjiaowu(UserName.EmpNumber) == 2)
            {
                //加载属于这个部门的值班数据
                EmployeesInfo employees = Tb_Entity.EmployeesInfo_Entity.FindEmpData(UserName.EmpNumber, true);

                Position position = Tb_Entity.EmployeesInfo_Entity.GetPosition(employees.PositionId);

                List<EmployeesInfo> listemp = Tb_Entity.EmployeesInfo_Entity.GetEmpsByDeptid(position.DeptId);

                DateTime dt = DateTime.Now;

                List<TeacherAddorBeonDutyView> list = Tb_Entity.DepData(listemp).OrderByDescending(l => l.Anpaidate).Where(a=>a.Anpaidate.Month==dt.Month-1).ToList();

                var jsondata = new { code = 0, Msg = "", count = list.Count, data = list.Skip((page - 1) * limit).Take(limit).ToList() };

                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            else if (Tb_Entity.Isjiaowu(UserName.EmpNumber) == 3)
            {
                //普通员工查看的数据自己个人值班数据
                List<EmployeesInfo> list_emp = new List<EmployeesInfo>();
                EmployeesInfo employees = Tb_Entity.EmployeesInfo_Entity.FindEmpData(UserName.EmpNumber, true);
                list_emp.Add(employees);
                List<TeacherAddorBeonDutyView> list = Tb_Entity.DepData(list_emp).OrderBy(l => l.Anpaidate).ToList();

                var jsondata = new { code = 0, Msg = "", count = list.Count, data = list.Skip((page - 1) * limit).Take(limit).ToList() };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                //教务查看的数据
                List<TeacherAddorBeonDutyView> list = Tb_Entity.GetViewAll().OrderBy(l => l.EmpName).ToList();
                var jsondata = new { code = 0, Msg = "", count = list.Count, data = list.Skip((page - 1) * limit).Take(limit).ToList() };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult TabledataSecher(int limit, int page)
        {
            string tid = Request.QueryString["tid"];
            string startime = Request.QueryString["olddate"];
            string endtime = Request.QueryString["newdate"];
            string ClassName = Request.QueryString["ClassName"];

            
            StringBuilder sb = new StringBuilder("select * from TeacherAddorBeonDutyView ");
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            if (Tb_Entity.Isjiaowu(UserName.EmpNumber) == 3)
            {
                sb.Append(" and Tearcher_Id='" + UserName.EmpNumber + "'");
            }

            if (!string.IsNullOrEmpty(tid) && tid != "0")
            {
                sb.Append(" and Tearcher_Id='" + tid + "'");
            }
            
            if (!string.IsNullOrEmpty(startime))
            {
                sb.Append(" and  Anpaidate>='" + startime + "'");
            }

            if (!string.IsNullOrEmpty(endtime))
            {
                sb.Append(" and  Anpaidate<='" + endtime + "'");
            }

            if (!string.IsNullOrEmpty(ClassName))
            {
                sb.Append(" and  ClassNumber='" + ClassName + "'");
            }



            List<TeacherAddorBeonDutyView> list = Tb_Entity.AttendSqlGetData(sb.ToString()).OrderBy(l => l.Anpaidate).ToList();
            var jsondata = new { code = 0, Msg = "", count = list.Count, data = list.Skip((page - 1) * limit).Take(limit).ToList() };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 属于教务的模糊查询
        /// </summary>
        /// <returns></returns>
        public ActionResult J_serchData(int limit, int page)
        {
            int dept = Convert.ToInt32(Request.QueryString["ser_jiaowu_depet"]);

            string emp = Request.QueryString["ser_jiaowu_teacher"];

            string starTime = Request.QueryString["starTime"];

            string endTime = Request.QueryString["endTime"];

            StringBuilder sb = new StringBuilder("select * from TeacherAddorBeonDutyView where 1=1");
            if (!string.IsNullOrEmpty(starTime))
            {
                sb.Append(" and Anpaidate>='" + starTime + "'");
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                sb.Append(" and Anpaidate<='" + endTime + "'");
            }

            List<TeacherAddorBeonDutyView> list = Tb_Entity.GetListBySql<TeacherAddorBeonDutyView>(sb.ToString());

            if (emp != "0")
            {
                //获取属于这个老师的值班数据
                list = list.Where(l => l.Tearcher_Id == emp).ToList();
                var ee = list.OrderByDescending(l => l.EmpName).Skip((page - 1) * limit).Take(limit).ToList();

                var data = new { code = 0, msg = "", count = list.Count, data = ee };

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (dept != 0)
            {
                //获取属于部门的值班数据
                List<EmployeesInfo> listemp = Tb_Entity.EmployeesInfo_Entity.GetEmpsByDeptid(dept);
                List<TeacherAddorBeonDutyView> list2 = new List<TeacherAddorBeonDutyView>();
                foreach (EmployeesInfo item in listemp)
                {
                    list2.AddRange(list.Where(l => l.Tearcher_Id == item.EmployeeId).ToList());
                }
                var data = list2.OrderBy(l => l.EmpName).Skip((page - 1) * limit).Take(limit).ToList();
                var datajson = new { code = 0, msg = "", count = list2.Count, data = data };
                return Json(datajson, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = list.OrderBy(l => l.EmpName).Skip((page - 1) * limit).Take(limit).ToList();
                var datajson = new { code = 0, msg = "", count = list.Count, data = data };
                return Json(datajson, JsonRequestBehavior.AllowGet);
            }


        }
        /// <summary>
        /// 手动安排教员值班
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult HandAnpaiFunction()
        {
            AjaxResult a = new AjaxResult();
            string[] evnings = Request.Form["evnings"].Split(',');
            DateTime time = Convert.ToDateTime(Request.Form["mytime"]);
            List<TeacherAddorBeonDuty> list = new List<TeacherAddorBeonDuty>();
            foreach (string item in evnings)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id = Convert.ToInt32(item);
                    TeacherAddorBeonDuty new_t = new TeacherAddorBeonDuty()
                    {
                        Tearcher_Id = Request.Form["Teacherid"],
                        BeOnDuty_Id = Convert.ToInt32(Request.Form["type"]),
                        OnByReak = Request.Form["ramke"],
                        AttendDate = DateTime.Now,
                        evning_Id = id,
                        IsDels = false
                    };

                    bool hava = Tb_Entity.Exits(new_t.evning_Id, time, new_t.Tearcher_Id);
                    if (!hava)
                    {
                        list.Add(new_t);
                    }
                }
            }

            if (list.Count > 0)
            {
                a = Tb_Entity.Add_data(list);
            }
            else
            {
                a.Success = false;
                a.Msg = "数据已重复，请删除数据在进行添加！";
            }

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteFunction()
        {
            string[] ids = Request.Form["ids"].Split(',');
            List<TeacherAddorBeonDuty> list = new List<TeacherAddorBeonDuty>();
            foreach (string item in ids)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id = Convert.ToInt32(item);
                    TeacherAddorBeonDuty find = Tb_Entity.Findid(id);
                    if (find != null && find.IsDels == false)
                    {
                        list.Add(find);
                    }
                }
            }

            AjaxResult a = Tb_Entity.Del_data(list);

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        #region 编辑页面

        /// <summary>
        /// 编辑页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult EditFunction(int id)
        //{
        //    TeacherAddorBeonDutyView find= Tb_Entity.FindViewid(id);

        //    return View(find);
        //}
        #endregion

        /// <summary>
        /// 教学老师模糊选择视图
        /// </summary>
        /// <returns></returns>
        public ActionResult TeacherEACHView()
        {
            return View();
        }

        /// <summary>
        /// 教务审核数据（数据审核后就不能删除了）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Shenhe(int id)
        {
            TeacherAddorBeonDuty find = Tb_Entity.Findid(id);

            find.IsDels = true;

            AjaxResult a = Tb_Entity.Upd_data(find);

            return Json(a, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 将值班表生成Excel文件
        /// </summary>
        /// <returns></returns>
        public ActionResult OutData()
        {
            string[] date = Request.Form["date_Anpaidate"].Split('-');

            int dep = Convert.ToInt32(Request.Form["dep"]);


            //获取属于这个部门的员工

            List<EmployeesInfo> emplist = Tb_Entity.EmployeesInfo_Entity.GetEmpsByDeptid(dep);

            List<TeacherAddorBeonDutyView> beonduty_list = new List<TeacherAddorBeonDutyView>();

            foreach (EmployeesInfo item in emplist)
            {
                var find = Tb_Entity.AttendSqlGetData(" select * from TeacherAddorBeonDutyView where YEAR(Anpaidate)='" + date[0] + "' and MONTH(Anpaidate)='" + date[1] + "' and  Tearcher_Id='" + item.EmployeeId + "'  and IsDels=1");

                beonduty_list.AddRange(find);
            }
            var a = beonduty_list.GroupBy(b => b.Anpaidate);
            List<Onbety_Print> data = new List<Onbety_Print>();
            List<string> list = new List<string>();
            a.ForEach(b =>
            {
                var y = b.Key;
                list.Add(y.ToString());
            });

            foreach (string item in list)
            {
                Onbety_Print one = new Onbety_Print();
                one.time = Convert.ToDateTime(item);
                List<TeacherAddorBeonDutyView> find = beonduty_list.Where(b => b.Anpaidate.ToString() == item).ToList();
                StringBuilder sb = new StringBuilder();
                int i = 0;
                foreach (TeacherAddorBeonDutyView orher in find)
                {
                    i++;
                    sb.Append($"{orher.EmpName}({orher.ClassNumber} / {orher.curd_name})");
                    if (i != find.Count)
                    {
                        sb.Append(",");
                    }
                }

                one.Content = sb.ToString();

                data.Add(one);
            }
            #region
            //a.ForEach(b=>
            //{
            //    try
            //    {
            //        b.ForEach(d =>
            //        {
            //            string dd = d.Anpaidate.ToString();
            //            var tempob = new Onbety_Print()
            //            {
            //                Content = $"{d.EmpName}({d.ClassNumber}/{d.curd_name})"
            //            };

            //            list.Add(dd, tempob);
            //        });
            //    }
            //    catch (Exception ex)
            //    {

            //        string mm = ex.Message;
            //    }

            //});

            #endregion
            string title = date[0] + "年" + date[1] + "月值班表";

            var jsondata = new { titile = title, data = data.OrderBy(t => t.time).ToList() };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取属于这个部门的老师
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetDetEmp(int id)
        {
            List<SelectListItem> listemp = Tb_Entity.EmployeesInfo_Entity.GetEmpsByDeptid(id).Select(l => new SelectListItem() { Text = l.EmpName, Value = l.EmployeeId }).ToList();

            return Json(listemp, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 教务批量数据审核
        /// </summary>
        /// <returns></returns>
        public ActionResult ShenheData()
        {
            string[] strs = Request.Form["str"].Split(',');
            List<TeacherAddorBeonDuty> list = new List<TeacherAddorBeonDuty>();
            foreach (string item in strs)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int id = Convert.ToInt32(item);
                    TeacherAddorBeonDuty find = Tb_Entity.Findid(id);

                    if (find != null)
                    {
                        find.IsDels = true;

                        list.Add(find);
                    }

                }
            }

            AjaxResult a = Tb_Entity.Upd_data(list);

            return Json(a, JsonRequestBehavior.AllowGet);

        }

        #region 导入考勤数据表

        public ActionResult TableInsert()
        {
            return View();
        }

        public ActionResult TableFuntion()
        {
            AjaxResult a = new AjaxResult();
            HttpPostedFileBase files = Request.Files["file"];
            try
            {

                string fileName = "kaoqin.xlsx";

                //在上传文件之前，将之间的文件全部删除
                string path = Server.MapPath("~/uploadXLSXfile/ConsultUploadfile/Kaoqin");

                FileInfo[] list = FileHelper.GetFiles(path);


                if (list.Count() > 0)
                {
                    foreach (FileInfo item in list)
                    {
                        FileHelper.Delete(item.FullName);
                    }
                }

                files.SaveAs(Server.MapPath("~/uploadXLSXfile/ConsultUploadfile/Kaoqin/" + fileName));

                a.Success = true;
                a.Msg = "上传成功！！";
            }
            catch (Exception ex)
            {
                a.Msg = "月份格式不对！";
                a.Success = false;
                return Json(a, JsonRequestBehavior.AllowGet);
            }

            return Json(a, JsonRequestBehavior.AllowGet);
        }


        #endregion

        /// <summary>
        /// 系统审核值班数据
        /// </summary>
        /// <returns></returns>
        public ActionResult SysSheHe()
        {
            AjaxResult a = new AjaxResult();
            try
            {
                //读取值班打卡规则
                string xmlpath = Server.MapPath("~/Xmlconfigure/Punch_the_clock.xml");

                XElement element = XElement.Load(xmlpath);
                var result = (from e in element.Elements("endtime")
                              where e.Attribute("name").Value == "教员"
                              from e2 in e.Elements("time")
                              select new SelectListItem() { Value = e2.Value, Text = e2.Attribute("name").Value }).ToList();




                DateTime mon = Convert.ToDateTime(Request.QueryString["Month"]);//获取月份

                Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
                EmployeesInfo employees = Tb_Entity.EmployeesInfo_Entity.FindEmpData(UserName.EmpNumber, true);
                List<TeacherAddorBeonDutyView> teachers = new List<TeacherAddorBeonDutyView>();
                if (employees.PositionId == 4028)
                {
                    StringBuilder sb = new StringBuilder(@"select tea.Id,tea.OnByReak,tea.BeOnDuty_Id,tea.Anpaidate,tea.Tearcher_Id,tea.AttendDate,tea.evning_Id,tea.ClassroomName,tea.ClassNumber,tea.curd_name,tea.TypeName,tea.EmpName,tea.IsDels from TeacherAddorBeonDutyView as tea left join EmployeesInfo as emp on emp.EmployeeId=tea.Tearcher_Id where YEAR(tea.Anpaidate)='" + mon.Year + "' and MONTH(tea.Anpaidate)='" + mon.Month + "' and (emp.PositionId=3 or emp.PositionId=4 or emp.PositionId=2011  or emp.PositionId=2012 or emp.PositionId=4025 or emp.PositionId=4026 or emp.PositionId=4027 or emp.PositionId=4028 or emp.PositionId=5038 or emp.PositionId=5039 or emp.PositionId=5044)");

                    //s1/s2教务
                    //获取S1/S2教质部门的老师值班数据
                    teachers = Tb_Entity.GetListBySql<TeacherAddorBeonDutyView>(sb.ToString()).OrderBy(te => te.EmpName).ToList();
                }
                else
                {
                    //S3教务
                    StringBuilder sb = new StringBuilder(@"select tea.Id,tea.OnByReak,tea.BeOnDuty_Id,tea.Anpaidate,tea.Tearcher_Id,tea.AttendDate,tea.evning_Id,tea.ClassroomName,tea.ClassNumber,tea.curd_name,tea.TypeName,tea.EmpName,tea.IsDels from TeacherAddorBeonDutyView as tea left join EmployeesInfo as emp on emp.EmployeeId=tea.Tearcher_Id where YEAR(tea.Anpaidate)='" + mon.Year + "' and MONTH(tea.Anpaidate)='" + mon.Month + "' and (emp.PositionId=2013 or emp.PositionId=2014 or emp.PositionId=2015  or emp.PositionId=2016 or emp.PositionId=4040 or emp.PositionId=4041 or emp.PositionId=4042 or emp.PositionId=5040 or emp.PositionId=5041 or emp.PositionId=5042 or emp.PositionId=5043)");

                    teachers = Tb_Entity.GetListBySql<TeacherAddorBeonDutyView>(sb.ToString()).OrderBy(te => te.EmpName).ToList();
                }
                //读取考勤记录表

                System.Data.DataTable t = AsposeOfficeHelper.ReadExcel(Server.MapPath("~/uploadXLSXfile/ConsultUploadfile/Kaoqin/kaoqin.xlsx"), false);//从Excel文件拿值
                List<TeacherAddorBeonDuty> shenList = new List<TeacherAddorBeonDuty>();//存放已经审核的数据
                List<TeacherAddorBeonDutyView> NoShenList = new List<TeacherAddorBeonDutyView>();//存放未审核的数据


                foreach (TeacherAddorBeonDutyView item in teachers)
                {
                    int day = item.Anpaidate.Day;
                    for (int i = 3; i < (t.Rows.Count); i++)
                    {

                        if (t.Rows[i][0].ToString() == item.Tearcher_Id)
                        {
                            int num = 0;
                            for (int j = 4; j < t.Columns.Count; j++)
                            {
                                Regex re = new Regex(@"^\d");
                                var mm = t.Rows[2][j];
                                if (!re.Match(t.Rows[2][j].ToString()).Success)
                                {
                                    if (t.Rows[2][j].ToString() == "六")
                                    {
                                        num = Convert.ToInt32(t.Rows[2][j - 1]) + 1;
                                    }
                                    else if (t.Rows[2][j].ToString() == "日")
                                    {
                                        num = Convert.ToInt32(t.Rows[2][j - 2]) + 2;
                                    }
                                }
                                else
                                {
                                    num = Convert.ToInt32(t.Rows[2][j]);
                                }

                                if (day - num == 0)
                                {
                                    num = j;

                                    string datestr = t.Rows[i][num].ToString();
                                    string[] vae = datestr.Split('\n');
                                    if (vae.Length >= 2)
                                    {
                                        SelectListItem select = null;
                                        //判断类型
                                        if (item.curd_name == "晚一")
                                        {
                                            select = result.Where(r => r.Text.Contains("晚一")).FirstOrDefault();
                                        }
                                        else if (item.curd_name == "晚二")
                                        {
                                            select = result.Where(r => r.Text.Contains("晚二")).FirstOrDefault();
                                        }
                                        DateTime tt1 = Convert.ToDateTime(vae[vae.Length - 1]);
                                        DateTime tt2 = Convert.ToDateTime(select.Value);

                                        if (tt1 >= tt2)
                                        {
                                            //算值班
                                            TeacherAddorBeonDuty finda = Tb_Entity.GetEntity(item.Id);
                                            finda.IsDels = true;
                                            shenList.Add(finda);
                                        }
                                        else
                                        {

                                            if (datestr.IsNullOrEmpty())
                                            {
                                                item.OnByReak = "没有读取到打卡时间!";
                                            }
                                            else
                                            {
                                                var str = datestr.Replace("\n", "-");
                                                item.OnByReak = "考勤表的下班打卡时间为:" + str + ";与正常下班打卡时间不匹配!";
                                            }
                                            NoShenList.Add(item);
                                        }
                                        break;
                                    }
                                    else
                                    {
                                        var str = datestr.Replace("\n", "-");
                                        item.OnByReak = "考勤表的打卡时间为:" + str + ";系统无法识别该打卡时间是上班时间还是下班时间!";
                                        NoShenList.Add(item);
                                        break;
                                    }
                                }
                                if (j == t.Columns.Count - 1)
                                {
                                    item.OnByReak = "系统没有找到" + item.Tearcher_Id + "的" + day + "号考勤情况!";
                                    NoShenList.Add(item);
                                }
                            }
                        }

                    }
                }

                Tb_Entity.Update(shenList);

                if (NoShenList.Count > 0)
                {
                    a.Data = NoShenList;
                    a.Msg = "数据已审核完毕,但是有系统无法识别的数据!";
                    a.ErrorCode = 505;
                }
                else
                {
                    a.Msg = "数据已审核完毕,没有异常!";
                    a.ErrorCode = 200;
                }

            }
            catch (Exception)
            {
                a.Success = false;
                a.ErrorCode = 405;
                a.Msg = "操作异常!";
            }
            return Json(a, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddDataView()
        {
            List<SelectListItem> sle_grand = Reconcile_Com.GetGrand_Id().Select(cl => new SelectListItem() { Text = cl.GrandName, Value = cl.Id.ToString(), Selected = false }).ToList();  //获取阶段
            sle_grand.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.myclass = sle_grand;

            return View();
        }

        /// <summary>
        /// 获取教员
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetTeacher()
        {
            int classid = Convert.ToInt32(Request.Form["classid"]);

            DateTime date = Convert.ToDateTime(Request.Form["date"]);

            //获取XX日期上XX班的老师

            string sqlstr = " select * from ReconcileView where AnPaiDate='" + date + "' and ClassSchedule_Id= " + classid;

            List<ReconcileView> rlist = Tb_Entity.GetListBySql<ReconcileView>(sqlstr);
            List<SelectListItem> select = new List<SelectListItem>();

            List<SelectListItem> select2 = new List<SelectListItem>();
            //判断如果时Y1就将英语，数学，语文老师显示
            List<ClassSchedule> clist = Tb_Entity.GetListBySql<ClassSchedule>("select * from ClassSchedule where id=" + classid).ToList();

            if (rlist.Count > 0)
            {
                select = rlist.Select(r => new SelectListItem() { Text = r.EmpName, Value = r.EmployeesInfo_Id }).ToList();
            }

            if (clist.Count > 0)
            {
                ClassSchedule findataclass = clist[0];

                if (findataclass.grade_Id == 1002)
                {
                    //是y1的班级
                    List<SelectListItem> emp1 = Tb_Entity.EmployeesInfo_Entity.GetEmpByPid(5044).Select(e=>new SelectListItem() { Text=e.EmpName,Value=e.EmployeeId}).ToList();//英语老师
                    List<SelectListItem> emp2 = Tb_Entity.EmployeesInfo_Entity.GetEmpByPid(4027).Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();//语文老师
                    List<SelectListItem> emp3 = Tb_Entity.EmployeesInfo_Entity.GetEmpByPid(4026).Select(e => new SelectListItem() { Text = e.EmpName, Value = e.EmployeeId }).ToList();//数学老师

                    foreach (SelectListItem item in emp1)
                    {
                       int count= select.Where(s => s.Text == item.Text).Count();
                        if (count<=0)
                        {
                            select.Add(item);
                        }
                    }
                    foreach (SelectListItem item in emp2)
                    {
                        int count = select.Where(s => s.Text == item.Text).Count();
                        if (count <= 0)
                        {
                            select.Add(item);
                        }
                    }
                    foreach (SelectListItem item in emp3)
                    {
                        int count = select.Where(s => s.Text == item.Text).Count();
                        if (count <= 0)
                        {
                            select.Add(item);
                        }
                    }                     

                }

                
            }

            //筛选教学部的人员

            select.ForEach(s=> {

                EmployeesInfo pid = Tb_Entity.EmployeesInfo_Entity.GetEntity(s.Value);

                if (pid!=null)
                {
                    int did = Tb_Entity.EmployeesInfo_Entity.GetDeptByPid(pid.PositionId).DeptId;
                    if (did == 5 || did == 1009 || did == 1010)
                    {
                        select2.Add(s);
                    }
                }

                 
            });

            return Json(select2,JsonRequestBehavior.AllowGet);
        }

    }
}