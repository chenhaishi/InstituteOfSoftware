using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Consult_Business;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.StuSatae_Maneger;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Business.StuInfomationType_Maneger;

namespace SiliconValley.InformationSystem.Web.Areas.Market.Controllers
{
    [CheckLogin]
    public class FollwingInfoController : BaseMvcController
    {
        ConsultManeger CM_Entity = new ConsultManeger();
        StuStateManeger ST_Entity = new StuStateManeger();
        EmployeesInfoManage Enplo_Entity;
        ConsultTeacherManeger ConsultTeacher;
        StuInfomationTypeManeger StuInfomationType = new StuInfomationTypeManeger();
        // GET: /Market/FollwingInfo/UpdateChange


        public List<SelectListItem> GetMarketGrand()
        {
            List<SelectListItem> marketGrand = new List<SelectListItem>();
            marketGrand.Add(new SelectListItem() { Text = "--无--", Value = "0", Selected = false });
            marketGrand.Add(new SelectListItem() { Text = "A类", Value = "A", Selected = false });
            marketGrand.Add(new SelectListItem() { Text = "B类", Value = "B", Selected = false });
            marketGrand.Add(new SelectListItem() { Text = "C类", Value = "C", Selected = false });
            marketGrand.Add(new SelectListItem() { Text = "D类", Value = "D", Selected = false });
            return marketGrand;
        }

        public ActionResult FollwingInfoIndex()
        {
            ConsultTeacher = new ConsultTeacherManeger();
            //获取当前上传的操作人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            int f_id = ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault() == null ? 0 : ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault().Id;
            return View();
        }
        /// <summary>
        /// 初始化柱状图
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImageData()
        {
            ConsultTeacher = new ConsultTeacherManeger();
            //获取当前上传的操作人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            int f_id = ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault() == null ? 0 : ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault().Id;
            //判断是哪个咨询师
            if (f_id != 0)
            {
                List<ConsultZhuzImageData> ConsultZhuzImageData_data = CM_Entity.GetImageData(f_id.ToString());
                return Json(ConsultZhuzImageData_data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }

        public ActionResult GetMonData(int MonthName, string Status)
        {
            //获取当前上传的操作人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            int f_id = ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault() == null ? 0 : ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault().Id;
            List<StudentPutOnRecord> result = new List<StudentPutOnRecord>();
            if (f_id != 0)
            {
                //判断是哪个咨询师         
                if (Status == "完成量")
                {
                    result = CM_Entity.GetStudentData(MonthName, "1", f_id);
                }
                else if (Status == "未完成量")
                {
                    result = CM_Entity.GetStudentData(MonthName, "2", f_id);
                }
                else if (Status == "分量数")
                {
                    result = CM_Entity.GetStudentData(MonthName, "3", f_id);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }
        /// <summary>
        /// 查询是否有该学生
        /// </summary>
        /// <param name="id">学生名称</param>
        /// <returns></returns>
        public ActionResult GetMonthStudent(string id)
        {
            List<ExportStudentBeanData> stu_list = CM_Entity.GetStudentPutRecored(id, true);
            List<ExportStudentBeanData> find_list = new List<ExportStudentBeanData>();
            List<Consult> find_consult = new List<Consult>();
            //获取当前上传的操作人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            int f_id = ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault() == null ? 0 : ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault().Id;
            if (f_id != 0)
            {
                foreach (ExportStudentBeanData item2 in stu_list)
                {
                    find_consult = CM_Entity.GetIQueryable().Where(c => c.TeacherName == f_id && c.StuName == item2.Id).ToList();//获取XX咨询师的分量情况
                    if (find_consult.Count > 0)
                    {
                        find_list.Add(item2);
                    }
                }
                //获取这个学生的跟踪信息次数
                int j = 0;
                List<FollwingInfo> find = CM_Entity.Fi_Entity.GetIQueryable().ToList();
                if (find_consult.Count == 1)
                {
                    int i = 0;
                    foreach (FollwingInfo item in find)
                    {
                        foreach (Consult c in find_consult)
                        {
                            if (item.Consult_Id == c.Id)
                            {
                                i++;
                            }
                        }
                    }
                    j = i;
                    SessionHelper.Session["Number"] = j;
                }
                var jsondata = new
                {
                    data = find_list,
                    Number = j
                };
                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }

        }

        //这是一个添加页面
        public ActionResult AddFollwingInfo(int id)
        {
            StudentPutOnRecord find_Stu = CM_Entity.GetSingleStudent(id);//获取备案信息
            Consult consult = CM_Entity.GetIQueryable().Where(c => c.StuName == id).FirstOrDefault();
            SessionHelper.Session["consult_id"] = consult.Id;

            ViewBag.Name = find_Stu.StuName;
            ViewBag.Sex = find_Stu.StuSex;

            //获取学生跟踪等级
            List<SelectListItem> marketGrand = GetMarketGrand().Select(c => new SelectListItem() { Text = c.Text, Value = c.Value, Selected = c.Value == consult.MarketType ? true : false }).ToList();
            ViewBag.marketlist = marketGrand;
            return View();
        }
        //这是一个添加方法
        public ActionResult AddFunction()
        {
            //获取当前上传的操作人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            Enplo_Entity = new EmployeesInfoManage();
            AjaxResult a = new AjaxResult();
            try
            {
                int count_id = Convert.ToInt32(SessionHelper.Session["consult_id"]);//获取分量Id

                string Rank = Request.Form["Marktype"];
                string TailAfterSituation = Request.Form["TailAfterSituation"];
                Consult consult = CM_Entity.GetEntity(count_id);
                FollwingInfo new_f = new FollwingInfo();
                new_f.Consult_Id = Convert.ToInt32(SessionHelper.Session["consult_id"]);
                new_f.FollwingDate = DateTime.Now;
                new_f.IsDelete = false;
                new_f.TailAfterSituation = TailAfterSituation;

                a = CM_Entity.Fi_Entity.Addsingdate(new_f);

                if (consult.MarketType == null)
                {
                    consult.MarketType = Rank;
                }
                else if (consult.MarketType != null || consult.MarketType != Rank)
                {
                    consult.MarketType = Rank;
                }

                a = CM_Entity.MyUpdate(consult);

                BusHelper.WriteSysLog(Enplo_Entity.GetEntity(UserName.EmpNumber).EmpName + "添加了一条跟踪学生信息", Entity.Base_SysManage.EnumType.LogType.添加数据);
                return Json(a, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                a.Success = false;
                //将错误填写到日志中     
                BusHelper.WriteSysLog(Enplo_Entity.GetEntity(UserName.EmpNumber).EmpName + "添加数据时出现:" + ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
                return Json(a, JsonRequestBehavior.AllowGet);
            }

        }

        //这是一个编辑页面
        public ActionResult EditView(int id)
        {
            StudentPutOnRecord find_stu = CM_Entity.GetSingleStudent(id);
            //获取该学生的Id
            ViewBag.Name = find_stu.StuName;
            ViewBag.Sex = find_stu.StuSex;
            //获取这个学生的所有咨询信息
            Consult find_c = CM_Entity.GetIQueryable().Where(c => c.StuName == id).FirstOrDefault();
            List<FollwingInfo> flist = CM_Entity.Fi_Entity.GetIQueryable().Where(f => f.Consult_Id == find_c.Id).ToList();
            ViewBag.flist = flist;
            ViewBag.stuId = id;
            //获取学生跟踪等级
            List<SelectListItem> marketGrand = GetMarketGrand().Select(c => new SelectListItem() { Text = c.Text, Value = c.Value, Selected = c.Value == find_c.MarketType ? true : false }).ToList();


            ViewBag.marketlist = marketGrand;
            return View();
        }
        //获取某个跟踪信息
        public ActionResult GetSingFollwingData(string Id)
        {
            if (!string.IsNullOrEmpty(Id))
            {
                int fid = Convert.ToInt32(Id);
                FollwingInfo find = CM_Entity.Fi_Entity.GetEntity(fid);
                return Json(find, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("no", JsonRequestBehavior.AllowGet);
            }
        }
        //这是一个编辑方法
        public ActionResult EditFunction()
        {
            AjaxResult a = new AjaxResult();
            ConsultTeacher = new ConsultTeacherManeger();
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            var f_ids = ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault() == null ? 0 : ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault().Id;
            try
            {
                string MyRank = Request.Form["Marktype"];
                string My_TailAfterSituation = Request.Form["My_TailAfterSituation"];
                int F_Id = Convert.ToInt32(Request.Form["F_Id"]);
                FollwingInfo find_f = CM_Entity.Fi_Entity.GetEntity(F_Id);

                find_f.TailAfterSituation = My_TailAfterSituation;
                a = CM_Entity.Fi_Entity.UpdatesingDate(find_f);

                Consult find = CM_Entity.GetEntity(find_f.Consult_Id);

                if (find.MarketType != MyRank && MyRank != "0")
                {
                    find.MarketType = MyRank;
                    a = CM_Entity.MyUpdate(find);
                }

                //BusHelper.WriteSysLog(Enplo_Entity.GetEntity(UserName.EmpNumber).EmpName + "成功编辑了一条跟踪信息数据", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                return Json(a, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                a.Success = false;
                //BusHelper.WriteSysLog(Enplo_Entity.GetEntity(UserName.EmpNumber).EmpName + "编辑跟踪信息时出现:"+ex, Entity.Base_SysManage.EnumType.LogType.编辑数据);
                return Json(a, JsonRequestBehavior.AllowGet);
            }

        }
        //这是找到多个学生显示的页面
        public ActionResult ListStudentView(string id)
        {
            string[] stulist = id.Split(',');
            List<ExportStudentBeanData> student = new List<ExportStudentBeanData>();

            foreach (string item1 in stulist)
            {
                if (!string.IsNullOrEmpty(item1))
                {

                    student.AddRange(CM_Entity.GetStudentPutRecored(item1, false));

                }
            }
            List<StudentData> data = student.Select(s => new StudentData()
            {
                Id = Convert.ToInt32(s.Id),
                stuSex = s.StuSex,
                StuName = s.StuName,
                StuPhone = s.Stuphone,
                StuSchoolName = s.StuSchoolName,
                StuAddress = s.StuAddress,
                StuInfomationType_Id = s.stuinfomation,//CM_Entity.getTypeName(s.StuInfomationType_Id.ToString(), true).Name,
                StuStatus_Id = s.StatusName,//ST_Entity.GetIdGiveName(s.StuStatus_Id.ToString(),true).Success==true? (ST_Entity.GetIdGiveName(s.StuStatus_Id.ToString(), true).Data as StuStatus).StatusName:null,
                StuIsGoto = s.StuisGoto == false ? "否" : "是",
                StuVisit = s.StuVisit,
                EmployeesInfo_Id = s.empName,//CM_Entity.GetEmplyeesInfo(s.EmployeesInfo_Id).EmpName,
                StuDateTime = s.BeanDate,
                StuEntering = s.StuEntering,
                AreName = s.RegionName,//CM_Entity.GetRegionName(s.Region_id).RegionName,
                Party = s.Party
            }).ToList();
            ViewBag.Student = data;
            return View();
        }

        /// <summary>
        /// 获取分量学生数据
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetTableData(int limit, int page)
        {
            //获取当前上传的操作人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            //ConsultTeacher = new ConsultTeacherManeger();
            Enplo_Entity = new EmployeesInfoManage();
            //int f_id = ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault() == null ? 0 : ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault().Id;
            //List<Consult> find_consult= CM_Entity.GetList().Where(c => c.TeacherName == f_id).ToList();
            string teacher = Enplo_Entity.FindEmpData(UserName.EmpNumber, true).EmpName;


            string sql1 = @"select * from StudentBeanView where ConsultTeacher= '" + teacher + "'";
            string sql2 = @"select * from Sch_MarketView  where Inquiry = '" + teacher + "'";
            List<ExportStudentBeanData> list = EmployandCounTeacherCoom.Studentrecond.Serch(sql1, sql2);//装载属于该咨询师的学生备案数据

            string Name = Request.QueryString["findNamevalue"].Trim();
            string Phone = Request.QueryString["findPhonevalue"].Trim();
            string StarDate = Request.QueryString["findStartvalue"];
            string EndDate = Request.QueryString["findEndvalue"];
            string findBeanManvalue = Request.QueryString["findBeanManvalue"].Trim();//备案人
            string findInformationvalue = Request.QueryString["findInformationvalue"];//信息来源
            string findAreavalue = Request.QueryString["findAreavalue"];//区域
            string S_party = Request.QueryString["S_party"].Trim();//关系人
            string Marktype = Request.QueryString["Marktype"];//市场类型
            string statis = Request.QueryString["statis"];//学生状态
            if (Name.Length != 0)
            {
                list = list.Where(l => l.StuName.Contains(Name)).ToList();
            }

            if (Phone.Length != 0)
            {
                list = list.Where(l => l.Stuphone == Phone).ToList();
            }

            if (StarDate.Length != 0)
            {
                DateTime d1 = Convert.ToDateTime(StarDate);
                list = list.Where(l => l.BeanDate >= d1).ToList();
            }

            if (EndDate.Length != 0)
            {
                DateTime d2 = Convert.ToDateTime(EndDate);
                list = list.Where(l => l.BeanDate <= d2).ToList();
            }

            if (findBeanManvalue.Length != 0)
            {
                list = list.Where(l => l.empName != null).ToList();
                list = list.Where(l => l.empName.Contains(findBeanManvalue)).ToList();
            }

            if (findInformationvalue.Length != 0 && findInformationvalue != "0")
            {
                list = list.Where(l => l.stuinfomation == findInformationvalue).ToList();
            }

            if (findAreavalue.Length != 0 && findAreavalue != "0")
            {
                list = list.Where(l => l.RegionName == findAreavalue).ToList();
            }

            if (S_party.Length != 0)
            {
                list = list.Where(l => l.Party.Contains(S_party)).ToList();
            }

            if (Marktype.Length != 0 && Marktype != "0")
            {
                list = list.Where(l => l.MarketType == Marktype).ToList();
            }

            if (statis.Length != 0 && statis != "0")
            {
                list = list.Where(l => l.StatusName != null).ToList();
                list = list.Where(l => l.StatusName.Contains(statis)).ToList();
            }

            StudentInformationBusiness stu = new StudentInformationBusiness();
            var mydata = list.OrderByDescending(l => l.Id).Skip((page - 1) * limit).Take(limit).Select(l => new
            {
                Id = l.Id,
                StuName = l.StuName,
                StuSex = l.StuSex,
                Stuphone = l.Stuphone,
                StuSchoolName = l.StuSchoolName,
                StuEducational = l.StuEducational,
                StuAddress = l.StuAddress,
                stuinfomation = l.stuinfomation,
                StatusName = l.StatusName,
                StuisGoto = l.StuisGoto,
                StuVisit = l.StuVisit,
                empName = l.empName,
                BeanDate = l.BeanDate,
                StuEntering = l.StuEntering,
                StatusTime = l.StatusTime,
                RegionName = l.RegionName,
                Reak = l.Reak,
                Party = l.Party,
                MarketType = l.MarketType,
                StuQQ = l.StuQQ,
                ConsultTeacher = l.ConsultTeacher,
                CountBeanDate = CM_Entity.AccordingStuIdGetConsultData(Convert.ToInt32(l.Id)).ComDate,
                Iszhuce = stu.IsRegister(l.Id)
            }).ToList();
            var data = new { data = mydata, count = list.Count, code = 0, msg = "" };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyFollwingInfoIndex()
        {
            ConsultTeacher = new ConsultTeacherManeger();

            //获取当前上传的操作人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();

            int f_id = ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault() == null ? 0 : ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault().Id;

            //获取市场类型
            ViewBag.marketlist = GetMarketGrand();

            //获取区域
            SelectListItem newselectitem = new SelectListItem() { Text = "请选择", Value = "0", Selected = true };
            var r_list = EmployandCounTeacherCoom.Studentrecond.GetEffectiveRegionAll(true).Select(r => new SelectListItem { Text = r.RegionName, Value = r.RegionName }).ToList();
            r_list.Add(newselectitem);
            ViewBag.are = r_list;


            //获取信息来源的所有数据
            List<SelectListItem> se = EmployandCounTeacherCoom.Studentrecond.StuInfomationType_Entity.GetList().Select(s => new SelectListItem { Text = s.Name, Value = s.Name }).ToList();
            se.Add(new SelectListItem() { Text = "请选择", Selected = true, Value = "0" });
            ViewBag.infomation = se;

            //获取学生状态
            List<SelectListItem> ss = new List<SelectListItem>();
            ss.Add(new SelectListItem() { Value = "0", Text = "请选择", Selected = true });
            ss.AddRange(EmployandCounTeacherCoom.Studentrecond.Stustate_Entity.GetList().Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusName }).ToList());

            ViewBag.slist = ss;
            return View();
        }

        //第一次获取数据
        public ActionResult OneTableData(int limit, int page)
        {
            StudentInformationBusiness stu = new StudentInformationBusiness();
            ConsultTeacher = new ConsultTeacherManeger();

            //获取当前上传的操作人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            //int f_id = ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault() == null ? 0 : ConsultTeacher.GetIQueryable().Where(cc => cc.Employees_Id == UserName.EmpNumber).FirstOrDefault().Id;
            Enplo_Entity = new EmployeesInfoManage();
            // List<Consult> find_consult = CM_Entity.GetList().Where(c => c.TeacherName == f_id).ToList();//获取属于该咨询师分的量
            //string sql1 =@"select s.Id,s.StuName,s.StuSex,s.StuBirthy,s.Stuphone,s.StuSchoolName,s.StuEducational,s.StuAddress,s.StuWeiXin, s.StuQQ,stusttype.Name as 'stuinfomation',stas.StatusName,s.StuisGoto,s.StuVisit,e.empName,s.Party,s.BeanDate,s.StuEntering ,s.StatusTime,reg.RegionName,s.ConsultTeacher,s.Reak,con.MarketType from studentPutOnRecord as s left join EmployeesInfo as e on s.EmployeesInfo_Id = e.EmployeeId left join StuInfomationType as stusttype on stusttype.Id = s.StuInfomationType_Id
            // left join StuStatus as stas on stas.Id = s.StuStatus_Id left join Region as reg on reg.ID = s.Region_id left join Consult as con on con.StuName = s.Id where con.TeacherName = "+f_id+"";
            //           string sql2 = @" select M.Id,M.StudentName,m.Sex,m.Phone,m.Education,m.CreateUserName,m.CreateDate,m.QQ,m.School,m.Inquiry,
            //m.source,m.Area,m.SalePerson,m.RelatedPerson,m.MarketState,M.sex as 'Info',M.sex as 'Remark'
            //from Consult as C left join  Sch_MarketView as M on M.Id = C.StuName where c.TeacherName = " + f_id+ " and m.Id is not null";
            string teacher = Enplo_Entity.FindEmpData(UserName.EmpNumber, true).EmpName;


            string sql1 = @"select * from StudentBeanView where ConsultTeacher= '" + teacher + "'";
            List<ExportStudentBeanData> list = EmployandCounTeacherCoom.Studentrecond.Serch(sql1, true);//装载属于该咨询师的学生备案数据

            var mydata = list.OrderByDescending(l => l.Id).Skip((page - 1) * limit).Take(limit).Select(l => new
            {
                Id = l.Id,
                StuName = l.StuName,
                StuSex = l.StuSex,
                Stuphone = l.Stuphone,
                StuSchoolName = l.StuSchoolName,
                StuEducational = l.StuEducational,
                StuAddress = l.StuAddress,
                stuinfomation = l.stuinfomation,
                StatusName = l.StatusName,
                StuisGoto = l.StuisGoto,
                StuVisit = l.StuVisit,
                empName = l.empName,
                BeanDate = l.BeanDate,
                StuEntering = l.StuEntering,
                StatusTime = l.StatusTime,
                RegionName = l.RegionName,
                Reak = l.Reak,
                Party = l.Party,
                MarketType = l.MarketType,
                StuQQ = l.StuQQ,
                ConsultTeacher = l.ConsultTeacher,
                CountBeanDate = CM_Entity.AccordingStuIdGetConsultData(Convert.ToInt32(l.Id)).ComDate,
                Iszhuce = stu.IsRegister(l.Id)
            }).ToList();
            var data = new { data = mydata, count = list.Count, code = 0, msg = "" };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //修改班级注册错误页面
        public ActionResult UpdateChange(int id)
        {
            ExportStudentBeanData data = CM_Entity.Stu_Entity.findId(id.ToString());
            //通过备案id获取这个学生注册的班级
            BaseBusiness<StudentInformation> su = new BaseBusiness<StudentInformation>();
            
            ViewBag.classold = StuInfomationType.select_class(id).Data;
            return View(data);
        }

        [HttpPost]
        public ActionResult UpdateChangeFunction()
        {
            int id = Convert.ToInt32(Request.Form["Id"]);

            int classid = Convert.ToInt32(Request.Form["Class_ID"]);

            string classname = Request.Form["Class_Name"];

           AjaxResult a= StuInfomationType.Update_Class(id, classname, classid);
            return Json(a,JsonRequestBehavior.AllowGet);
        }

        
    }
}