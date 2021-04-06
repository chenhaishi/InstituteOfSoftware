using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Entity;
using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;//获取树实体
using SiliconValley.InformationSystem.Business.Common;//获取日志实体
using SiliconValley.InformationSystem.Business.StuSatae_Maneger;//获取学生状态实体
using SiliconValley.InformationSystem.Business.StuInfomationType_Maneger;//获取学生信息来源实体
using SiliconValley.InformationSystem.Business.DepartmentBusiness; //获取岗位信息实体 
using SiliconValley.InformationSystem.Business.PositionBusiness;//获取岗位实体
using SiliconValley.InformationSystem.Entity.ViewEntity;//获取员工岗位部门实体
using SiliconValley.InformationSystem.Entity.Entity;
using System.Text;
using System.IO;
using System.Data;
using SiliconValley.InformationSystem.Entity.Base_SysManage;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using Microsoft.Office.Interop;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using SiliconValley.InformationSystem.Business.NewExcel;
using DataTable = System.Data.DataTable;
using SiliconValley.InformationSystem.Depository.CellPhoneSMS;
using SiliconValley.InformationSystem.Business.RegionManage;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using System.Threading;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using System.Text.RegularExpressions;
using BaiduBce;
using BaiduBce.Auth;
using BaiduBce.Services.Bos;
using System.Configuration;
using SiliconValley.InformationSystem.Business.BaiduAPI_Business;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;

namespace SiliconValley.InformationSystem.Web.Areas.Market.Controllers
{
    
    [CheckLogin]
    public class StudentDataKeepController : BaseMvcController
    {
        // GET: /Market/StudentDataKeep/Conflict

        #region 创建实体

        //创建一个用于操作数据的备案实体
        private StudentDataKeepAndRecordBusiness s_Entity = new StudentDataKeepAndRecordBusiness();

        ExcelHelper Excel_Entity;
       
        #endregion


        #region 数据操作

         public List<SelectListItem> Marketgrand()
        {
            List<SelectListItem> typelist = new List<SelectListItem>() {
                new SelectListItem() { Text = "--无--", Value = "0" },
                new SelectListItem() { Text="A类",Value="A"},
                new SelectListItem() { Text = "B类", Value = "B" },
                new SelectListItem() { Text = "C类", Value = "C" } ,
                new SelectListItem() { Text = "D类", Value = "D" }
            };

            return typelist;
        }

        //这是一个数据备案的主页面
        public ActionResult StudentDataKeepIndex()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            //获取信息来源的所有数据
            List<SelectListItem> se = s_Entity.StuInfomationType_Entity.GetList().Select(s => new SelectListItem { Text = s.Name, Value = s.Name }).ToList();
            se.Add(new SelectListItem() { Text = "请选择", Selected = true, Value = "0" });
            ViewBag.infomation = se;
            //获取区域所有信息
            SelectListItem newselectitem = new SelectListItem() { Text = "请选择", Value = "0", Selected = true };
            var r_list = s_Entity.GetEffectiveRegionAll(true).Select(r => new SelectListItem { Text = r.RegionName, Value = r.RegionName }).ToList();
            r_list.Add(newselectitem);
            ViewBag.are = r_list;
            //获取咨询师的所有数据
            List<SelectListItem> list_cteacher = new List<SelectListItem>();
            List<SelectListItem> list_one = new List<SelectListItem>();
            list_cteacher.Add(new SelectListItem() { Text = "请选择", Value = "0", Selected = true });
            list_one.Add(new SelectListItem() { Text = "请选择", Value = "0", Selected = true });
            list_cteacher.AddRange(EmployandCounTeacherCoom.getallCountTeacher(true).Select(c => new SelectListItem() { Text = c.empname, Value = c.empname }).ToList());
            list_one.AddRange(EmployandCounTeacherCoom.GetTeacher().Select(c=>new SelectListItem() { Text=c.Employees_Id,Value=c.Id.ToString()}).ToList());
            ViewBag.teacherlist = list_cteacher;
            ViewBag.Teacher = list_one;
            //获取学生状态所有数据
            List<SelectListItem> ss = new List<SelectListItem>();
            ss.Add(new SelectListItem() { Value = "0", Text = "请选择", Selected = true });
            ss.AddRange(s_Entity.Stustate_Entity.GetList().Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusName }).ToList());

            ViewBag.slist = ss;

            ViewBag.Pers = s_Entity.GetPostion(UserName.EmpNumber);

            //获取市场类型
            ViewBag.type = Marketgrand();
            return View();
        }
       
        /// <summary>
        /// 第一次加载数据的
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult TableData(int limit, int page)
        {
            //82000
            List<ExportStudentBeanData> list = s_Entity.GetAllTopNumber(82000).OrderByDescending(s => s.StuDateTime).ToList();

            var data = list.Skip((page - 1) * limit).Take(limit).ToList();

            var josndata = new { code = 0, count = list.Count, data = data };

            return Json(josndata, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 模糊查询方法
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetTableData(int limit, int page)
        {
            List<ExportStudentBeanData> list = new List<ExportStudentBeanData>();
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            sb1.Append("select * from StudentBeanView where IsDelete=0");
            sb2.Append("select * from Sch_MarketView where IsDel=0");
            string findName = Request.QueryString["findName"];
            string findPhone = Request.QueryString["findPhone"];

            string Isgo = Request.QueryString["Isgo"];//是否上门

            if (!string.IsNullOrEmpty(findName) || !string.IsNullOrEmpty(findPhone))
            {
                if (findName.Length > 0)
                {
                    sb1.Append("and  StuName like  '%" + findName + "%'");
                    sb2.Append(" and StudentName like  '%" + findName + "%'");
                }

                if (findPhone.Length > 0)
                {
                    sb1.Append(" and Stuphone = '" + findPhone + "'");
                    sb2.Append(" and Phone = '" + findPhone + "'");
                }


            }
            else
            {
                #region 模糊查询
                string findNamevalue = Request.QueryString["findNamevalue"].Trim();//姓名
                string findPhonevalue = Request.QueryString["findPhonevalue"].Trim();//电话
                string findInformationvalue = Request.QueryString["findInformationvalue"];//信息来源
                string findStartvalue = Request.QueryString["findStartvalue"];//录入开始时间
                string findEndvalue = Request.QueryString["findEndvalue"];//录入结束时间
                string findBeanManvalue = Request.QueryString["findBeanManvalue"].Trim();//备案人
                string findAreavalue = Request.QueryString["findAreavalue"];//区域
                string findTeacher = Request.QueryString["S_consultTeacher"];//咨询师
                string findStatus = Request.QueryString["S_status"];//备案状态
                string findPary = Request.QueryString["S_party"].Trim();//关系人
                string findCreateMan = Request.QueryString["S_intosysMan"];//录入人
                string markety = Request.QueryString["marketype"];//市场类型

                string qq = Request.QueryString["S_QQ"].Trim();//QQ
                string edution = Request.QueryString["eduttion"];//学历
                string reack = Request.QueryString["S_Reack"];//其他说明
                string S_School = Request.QueryString["S_School"];//学校

                
                if (Isgo!="0")
                {
                    sb1.Append(" and StuisGoto = '" + Isgo + "'");
                }                 
                if (!string.IsNullOrEmpty(findNamevalue))
                {
                    sb1.Append("and  StuName like  '%" + findNamevalue + "%'");
                    sb2.Append(" and StudentName like  '%" + findNamevalue + "%'");
                }
                if (!string.IsNullOrEmpty(findPhonevalue))
                {
                    sb1.Append(" and Stuphone like '%" + findPhonevalue + "%'");
                    sb2.Append(" and Phone like'%" + findPhonevalue + "%'");
                }
                if (findInformationvalue != "0" && !string.IsNullOrEmpty(findInformationvalue))
                {
                    sb1.Append(" and stuinfomation = '" + findInformationvalue + "'");
                    sb2.Append(" and source = '" + findInformationvalue + "'");
                }
                if (!string.IsNullOrEmpty(findBeanManvalue))
                {
                    sb1.Append(" and empName = '" + findBeanManvalue + "'");
                    sb2.Append(" and SalePerson = '" + findBeanManvalue + "'");
                }
                if (findAreavalue != "0" && !string.IsNullOrEmpty(findAreavalue))
                {
                    sb1.Append(" and RegionName = '" + findAreavalue + "'");
                    sb2.Append(" and Area = '" + findAreavalue + "'");
                }
                if (findStatus != "0" && !string.IsNullOrEmpty(findStatus))
                {
                    sb1.Append(" and StatusName = '" + findStatus + "'");
                    sb2.Append(" and  MarketState like '已报名%'");
                }
                if (!string.IsNullOrEmpty(findPary))
                {
                    sb1.Append(" and Party = '" + findPary + "'");
                    sb2.Append(" and RelatedPerson = '" + findPary + "'");
                }
                if (!string.IsNullOrEmpty(findCreateMan))
                {
                    sb1.Append(" and StuEntering = '" + findCreateMan + "'");
                    sb2.Append(" and CreateUserName = '" + findCreateMan + "'");
                }
                //当查询的上门数据时
                if (Isgo!="0")
                {
                    //匹配上门日期
                    if (!string.IsNullOrEmpty(findStartvalue))
                    {
                        sb1.Append(" and StuVisit >= '" + findStartvalue + "'");
                    }

                    if (!string.IsNullOrEmpty(findEndvalue))
                    {
                        sb1.Append(" and StuVisit <= '" + findEndvalue + "'");
                    }
                    
                }
                else
                {
                    //匹配备案日期

                    if (!string.IsNullOrEmpty(findStartvalue))
                    {
                        sb1.Append(" and BeanDate >= '" + findStartvalue + "'");
                        sb2.Append(" and CreateDate >= '" + findStartvalue + "'");
                    }

                    if (!string.IsNullOrEmpty(findEndvalue))
                    {
                        sb1.Append(" and BeanDate <= '" + findEndvalue + "'");
                        sb2.Append(" and CreateDate <= '" + findEndvalue + "'");
                    }
                }

                 

                if (!string.IsNullOrEmpty(findTeacher) && findTeacher != "0")
                {
                    sb1.Append(" and ConsultTeacher = '" + findTeacher + "'");
                    sb2.Append(" and Inquiry = '" + findTeacher + "'");
                }

                

                if (markety != "0" && !string.IsNullOrEmpty(markety))
                {
                    sb1.Append(" and MarketType = '" + markety + "'");
                    sb2.Append(" and MarketState like '" + markety + "%'");
                }

                if (!string.IsNullOrEmpty(qq))
                {
                    sb1.Append(" and StuQQ = '" + qq + "'");
                    sb2.Append(" and QQ = '" + qq + "'");
                }

                if (!string.IsNullOrEmpty(edution) && edution!="0")
                {
                    sb1.Append(" and StuEducational = '" + edution + "'");
                    sb2.Append(" and Education = '" + edution + "'");
                }

                if (!string.IsNullOrEmpty(reack))
                {
                    sb1.Append(" and Reak like '" + reack + "%'");
                    sb2.Append(" and Remark like '" + reack + "%'");
                }

                if (!string.IsNullOrEmpty(S_School))
                {
                    sb1.Append(" and StuSchoolName like '%" + S_School + "%'");
                    sb2.Append(" and School like '%" + S_School + "%'");
                }

                
                #endregion               

            }
            if (Isgo != "0")
            {
                list = s_Entity.Serch(sb1.ToString(), sb2.ToString(),false).OrderByDescending(s => s.StuDateTime).ToList();
            }
            else
            {
                list = s_Entity.Serch(sb1.ToString(), sb2.ToString(),true).OrderByDescending(s => s.StuDateTime).ToList();
            }
            

            var data = list.Skip((page - 1) * limit).Take(limit).ToList();

            var josndata = new { code = 0, count = list.Count, data = data };

            return Json(josndata, JsonRequestBehavior.AllowGet);
        }

        //这是一个添加数据的页面
        public ActionResult AddorEdit(string id)
        {
           
            //判断登录人员是否是网络部
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            int getid= s_Entity.GetPostion(UserName.EmpNumber);
            if (getid ==3 || getid==2)
            {
                ViewBag.infomation = s_Entity.StuInfomationType_Entity.GetList().Where(s => s.IsDelete == false).Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString(),Selected=s.Name.Contains("网络")?true:false }).ToList();
            }
            else
            {
                ViewBag.infomation = s_Entity.StuInfomationType_Entity.GetList().Where(s => s.IsDelete == false).Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();
            }
           
            
            //获取所有区域
            SelectListItem s2 = new SelectListItem() { Text = "区域外", Value = "区域外" ,Selected=true};
            var r_list = s_Entity.GetEffectiveRegionAll(true).Select(r => new SelectListItem { Text = r.RegionName, Value = r.ID.ToString() }).ToList();
            r_list.Add(s2);
            ViewBag.area = r_list;
            List<SelectListItem> infoTeacher = new List<SelectListItem>();
            infoTeacher.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            //获取咨询师              
            infoTeacher.AddRange(EmployandCounTeacherCoom.getallCountTeacher(false).Select(d => new SelectListItem() { Text = d.empname, Value = d.empname }).ToList());
            ViewBag.ConsultTeacher = infoTeacher;
            return View();
        }

        //添加备案数据
        public ActionResult StudentDataKeepAdd(StudentPutOnRecord news)
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
       
            AjaxResult a;
            try
            { 
                //判断是否是网咨，如果是网咨则不需要发短信
                StuInfomationType find_type = s_Entity.StuInfomationType_Entity.SerchSingleData("网络", false);
                news.StuDateTime = DateTime.Now;
                news.BeanDate = DateTime.Now;
                news.IsDelete = 0;
                news.StuEntering = s_Entity.Enplo_Entity.GetEntity(UserName.EmpNumber).EmpName;
                news.StuStatus_Id = 1013;
                news.StuSex = news.StuSex == "0" ? null : news.StuSex;
                news.StuEducational = news.StuEducational == "0" ? null : news.StuEducational;
                if (news.ConsultTeacher == "0")
                {
                    news.ConsultTeacher = null;
                }
                //判断是否有姓名相同的备案数据                
                if (s_Entity.StudentOrrideData(news.StuName, news.StuPhone,news.StuWeiXin,news.StuQQ) == null)
                {
                     
                    a = s_Entity.Add_data(news);
                    if (a.Success == true)
                    {
                        StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now, userId = UserName.EmpNumber, operationType = Entity.Base_SysManage.EnumType.LogType.添加数据 + ":" + news.StuName + "备案成功！" };
                        s_Entity.log_s.Add_data(log);

                       
                        if (news.StuInfomationType_Id != find_type.Id)
                        {
                            //通知备案人备案成功
                            if (news.EmployeesInfo_Id!=null)
                            {
                                string phone = s_Entity.Enplo_Entity.GetEntity(news.EmployeesInfo_Id).Phone;
                          
                                string smsText = "硅谷信息平台学生备案提示:" + news.StuName + "学生在" + DateTime.Now + "已备案成功,录入人:" + news.StuEntering;
                                string t = PhoneMsgHelper.SendMsg(phone, smsText);
                            }
                             
                        }
                        else
                        {
                            //如果是网咨，则添加到王咨回访表中
                            StudentPutOnRecord find_stu = s_Entity.StudentOrreideData_OnRecord(news.StuName, news.StuPhone,news.StuDateTime,0);
                            bool sm = s_Entity.NetClient_Entity.AddNCRData(find_stu.Id);

                            string phoen = Request.Form["ShorPhone"];
                            string reak = Request.Form["ShorReacke"];
                            if (!string.IsNullOrEmpty(phoen) && !string.IsNullOrEmpty(reak) && a.Success == true)
                            {
                                string t = PhoneMsgHelper.SendMsg(phoen, reak);
                            }
                                                             
                        }

                        //判断是否指派了咨询师  

                        if (news.ConsultTeacher != null)
                        {
                            ExportStudentBeanData find = s_Entity.StudentOrrideData(news.StuName, news.StuPhone,null,null);
                            Consult new_c = new Consult();
                            new_c.TeacherName = EmployandCounTeacherCoom.getallCountTeacher(false).Where(s => s.empname == news.ConsultTeacher).FirstOrDefault().consultercherid; 
                            new_c.StuName = Convert.ToInt32(find.Id);
                            new_c.IsDelete = false;
                            new_c.ComDate = DateTime.Now;
                            a.Success = EmployandCounTeacherCoom.AddConsult(new_c);
                        }
                        
                    }
                }
                else
                {
                    a = new AjaxResult();
                    news.IsDelete = 2;
                    var list = s_Entity.GetListBySql<ExportStudentBeanData>("select * from  StudentBeanView where StuName='" + news.StuName + "' and Stuphone='" + news.StuPhone + "' and IsDelete=2");
                    if (list.Count<=0)
                    {
                        a = s_Entity.Add_data(news);
                         
                    }
                    a.Msg = "该学生在此之前已有备案信息,需要将该数据成为正常备案数据，请联系杨校或阳亮！";             
                    a.Success = false;
                    
                    if (news.StuInfomationType_Id == find_type.Id)
                    {
                        //如果是网咨，则添加到王咨回访表中
                        StudentPutOnRecord find_stu = s_Entity.StudentOrreideData_OnRecord(news.StuName, news.StuPhone, news.StuDateTime,2);
                        bool sm = s_Entity.NetClient_Entity.AddNCRData(find_stu.Id);
                    }
                     
                    StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now , userId = UserName.EmpNumber , operationType = Entity.Base_SysManage.EnumType.LogType.添加数据 + ":"+ news.StuName+ "备案数据冲突！" };
                    s_Entity.log_s.Add_data(log);
                }
                return Json(a);
            }
            catch (Exception ex)
            {
                //将错误填写到日志中     
                //BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
                StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now , userId = UserName.EmpNumber , operationType = Entity.Base_SysManage.EnumType.LogType.添加数据error + ex.Message };
                s_Entity.log_s.Add_data(log);
                return Json(Error("数据添加有误"), JsonRequestBehavior.AllowGet);
            }
        }

        //将所有员工显示给用户选择
        public ActionResult ShowEmployeInfomation()
        {
  
            List<EmployeesInfo> list_Enploy = s_Entity.GetEffectiveEmpAll(true);//获取所有在职员工
            List<TreeClass> list_Tree = s_Entity.Department_Entity.GetList().Select(d => new TreeClass() { id = d.DeptId.ToString(), title = d.DeptName, children = new List<TreeClass>(), disable = false, @checked = false, spread = false }).ToList();
            List<Position> list_Position = s_Entity.Position_Entity.GetList().Where(s => s.IsDel == false).ToList();//获取所有岗位有用的数据
            foreach (TreeClass item1 in list_Tree)
            {
                List<TreeClass> bigTree = new List<TreeClass>();
                foreach (Position item2 in list_Position)
                {
                    if (item1.id == item2.DeptId.ToString())
                    {
                        foreach (EmployeesInfo item3 in list_Enploy)
                        {
                            if (item3.PositionId == item2.Pid)
                            {
                                TreeClass tcc2 = new TreeClass();
                                tcc2.id = item3.EmployeeId;
                                tcc2.title = item3.EmpName;
                                bigTree.Add(tcc2);
                            }
                        }
                        item1.children = bigTree;
                    }
                }
            }
            return Json(list_Tree, JsonRequestBehavior.AllowGet);
        }

        //树形图
        public ActionResult ShowTree()
        {
            return View();
        }

        //查看是否是选中员工
        public ActionResult FindEmply(string id)
        {
            EmployeesInfo finde = s_Entity.GetEffectiveEmpAll(true).Where(s => s.EmployeeId == id).FirstOrDefault();
            if (finde != null)
            {
                return Json(finde, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("no", JsonRequestBehavior.AllowGet);
            }

        }
       

        //查看是否有重复的学员信息名称
        public ActionResult FindStudent(string id)
        {
            AjaxResult a = new AjaxResult();
            if (!string.IsNullOrEmpty(id))
            {
                List<ExportStudentBeanData> fins = s_Entity.StudentOrride(id);
                if (fins.Count > 0)
                {
                    a.Data = fins;
                    a.Success = false;
                }
                else
                {
                    a.Success = true;
                }
            }
            else
            {
                a.Success = false;
            }
            
            return Json(a, JsonRequestBehavior.AllowGet);
        }

        //创建一个编辑页面
        public ActionResult EditView(string id)
        {
            //判断当前登陆人是否是网络部人员，如果是那就不要显示市场类型
            ViewBag.UserId = 22;
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            int IdCorad = s_Entity.GetPostion(UserName.EmpNumber);
            if (IdCorad==3 || IdCorad==2 || IdCorad==-1)
            {
                ViewBag.UserId = 33;
            }
 
 
            ViewBag.id = id;
            //获取信息来源的所有数据
            ViewBag.infomation =s_Entity.StuInfomationType_Entity.GetList().Where(s => s.IsDelete == false).Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();

            //获取所有区域
            SelectListItem s2 = new SelectListItem() { Text = "区域外", Value = "区域外" };
            var r_list = s_Entity.GetEffectiveRegionAll(true).Select(r => new SelectListItem { Text = r.RegionName, Value = r.ID.ToString() }).ToList();
            r_list.Add(s2);
            ViewBag.area = r_list;

            //获取所有市场类型
             ExportStudentBeanData find= s_Entity.findId(id);
            ViewBag.typemarket = Marketgrand().Select(c => new SelectListItem() { Text = c.Text, Value = c.Value, Selected = c.Value == find.MarketType ? true : false }).ToList();
            return View();
        }

        //创建一个用于编辑的处理方法
        public ActionResult EditFunction(StudentPutOnRecord olds)
        {         
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息  //需要判断是咨询部人员修改还是网络部人员修改   
            int IdCorad = s_Entity.GetPostion(UserName.EmpNumber);
            AjaxResult a = new AjaxResult();
            StuInfomationType fins= s_Entity.StuInfomationType_Entity.GetEntity(olds.StuInfomationType_Id);
            if(!fins.Name.Contains("网络"))
            {
                if (IdCorad == 3 || IdCorad == 2)
                {
                    a.Success = false;
                    a.Msg = "抱歉，这条备案数据你没有权限修改！！";
                    return Json(a, JsonRequestBehavior.AllowGet);
                }
                 
            }
            else if (IdCorad == 3 && IdCorad == 2)
            {
                a.Success = false;
                a.Msg = "抱歉，这条备案数据你没有权限修改！！";
                return Json(a, JsonRequestBehavior.AllowGet);
            }

            //判断是否将其他来源改为网络，如果是将该信息添加到网咨跟踪表中
            StudentPutOnRecord find= s_Entity.whereStudentId(olds.Id);
            //StuInfomationType fins2 = s_Entity.StuInfomationType_Entity.GetEntity(find.StuInfomationType_Id);
            //if (!fins2.Name.Contains("网络"))
            //{
            //    if (fins.Name.Contains("网络"))
            //    {

            //            StudentPutOnRecord find_stu = s_Entity.StudentOrreideData_OnRecord(find.StuName, find.StuPhone, find.StuDateTime);
            //            bool s=  s_Entity.NetClient_Entity.IsExsitSprStu(find_stu.Id);
            //            if (!s)
            //            {
            //                bool sm = s_Entity.NetClient_Entity.AddNCRData(find_stu.Id);
            //            }                                                                                 
            //    }
            //}

            //if (find.StuPhone==null)
            //{
            //    if (s_Entity.StudentOrrideData(olds.StuName, olds.StuPhone, olds.StuWeiXin, olds.StuQQ) == null)
            //    {
            //        
            //    }
            //    else
            //    {
            //        a.Success = false;
            //        //a.Msg = "备案中已有重复的数据！";
            //        //StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now, userId = UserName.EmpNumber, operationType = Entity.Base_SysManage.EnumType.LogType.编辑数据 + ":备案编号为" + olds.Id + "," + olds.StuName + "备案数据编辑时，已有重复的电话，或微信，或QQ！" };
            //        //s_Entity.log_s.Add_data(log);
            //    }

            //}
            a = s_Entity.Update_data(olds);
            if (a.Success==true)
            {
                StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now, userId = UserName.EmpNumber, operationType = Entity.Base_SysManage.EnumType.LogType.编辑数据 + ":备案编号为"+olds.Id+"," + olds.StuName + "备案数据编辑成功！" };
                s_Entity.log_s.Add_data(log);
            }
            else
            {
                StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now, userId = UserName.EmpNumber, operationType = Entity.Base_SysManage.EnumType.LogType.编辑数据error + ":备案编号为" + olds.Id + "," + olds.StuName + "备案数据编辑失败！" };
                s_Entity.log_s.Add_data(log);
            }
            string marketvalue= Request.Form["market"];

            string phoen = Request.Form["ShorPhone"];

            string reak = Request.Form["ShorReacke"];

            if (!string.IsNullOrEmpty(phoen) && !string.IsNullOrEmpty(reak) && a.Success==true)
            {
                string msg=   PhoneMsgHelper.SendMsg(phoen, reak);
            }

            string t = PhoneMsgHelper.SendMsg(phoen, reak);

            if (!string.IsNullOrEmpty(marketvalue) && marketvalue!="0")
            {
                //判断是否有分量
               Consult consult=  EmployandCounTeacherCoom.consult.AccordingStuIdGetConsultData(olds.Id);
                if (consult!=null)
                {
                   if(consult.MarketType!= marketvalue)
                    {
                        consult.MarketType = marketvalue;
                        a= EmployandCounTeacherCoom.consult.MyUpdate(consult);
                    }
                }
                else
                {
                    a.Success = false;

                    a.Msg = "没有指定咨询师，无法修改市场类型！！！";
                }
            }

            return Json(a, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateGotoSchool()
        {
            List<StudentPutOnRecord> list = new List<StudentPutOnRecord>();
            Request.Form["ids"].Split(',').ForEach(item =>
            {
                if (!string.IsNullOrEmpty(item) && item != "0")
                {
                    int id = Convert.ToInt32(item);
                    list.Add(s_Entity.GetEntity(id));
                }
            });
            DateTime date = Convert.ToDateTime(Request.Form["date"]);
            AjaxResult a = s_Entity.UpdateGotoShcool(list, date);
            return Json(a, JsonRequestBehavior.AllowGet);
        }
      
        //根据ID找到学生信息并赋值
        public ActionResult FindStudentInfomation(string id)
        {
            if (!string.IsNullOrEmpty(id) && id != "undifind")
            {
                ExportStudentBeanData finds = s_Entity.findId(id);
                int intId = Convert.ToInt32(id);
 
                    StudentPutOnRecord finds2 = s_Entity.GetEntity(intId);
                    var newdata = new
                    {
                        EmployeesInfo_Id = finds2.EmployeesInfo_Id,
                        Id = finds.Id,
                        Reak = finds.Reak,
                        StuAddress = finds.StuAddress,
                        StuBirthy = finds.StuBirthy,
                        StuDateTime = s_Entity.GetEntity(finds.Id).StuDateTime,
                        StuEducational = finds.StuEducational,
                        StuEntering = finds.StuEntering,
                        StuInfomationType_Id = s_Entity.StuInfomationType_Entity.SerchSingleData(finds.stuinfomation, false)?.Id ?? null,
                        Region_id = s_Entity.region_Entity.SerchRegionName(finds.RegionName, false)?.ID ?? null,
                        StuIsGoto = finds.StuisGoto == null ? false : finds.StuisGoto,
                        StuName = finds.StuName,
                        StuPhone = finds.Stuphone,
                        StuQQ = finds.StuQQ,
                        StuSchoolName = finds.StuSchoolName,
                        StuSex = finds.StuSex,
                        StuVisit = finds.StuVisit,
                        StuWeiXin = finds.StuWeiXin,
                        e_Name = finds.empName,
                        StuEntering_1 = finds.StuEntering,
                        InfomationTypeName = finds.stuinfomation,
                        StatusName = finds.StatusName,
                        Region_Name = finds.RegionName,
                        Party = finds.Party,
                        reamke = finds.Reak
                    };
                    return Json(newdata, JsonRequestBehavior.AllowGet);               
            }
            else
            {
                return Json("学生ID未拿到", JsonRequestBehavior.AllowGet);
            }

        }
        //数据详情查看页面 2020年之后的数据详情页面
        public ActionResult LookDetailsView(string id)
        {
 
            ExportStudentBeanData find = s_Entity.findId(id);
            //region_Entity = new RegionManeges();
            ViewBag.id = id;
            //获取信息来源的所有数据
            ViewBag.infomation = s_Entity.StuInfomationType_Entity.GetList().Where(s => s.IsDelete == false).Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }).ToList();

            //获取学生状态来源的所有数据
            ViewBag.state = s_Entity.Stustate_Entity.GetList().Where(s => s.IsDelete == false).Select(s => new SelectListItem { Text = s.StatusName, Value = s.Id.ToString() }).ToList();

            //获取区域
            ViewBag.regin = find.RegionName == null ? "区域外" :  find.RegionName;
            return View();
        }

        //显示有疑似的数据
        public ActionResult StudentSomeData(string id)
        {
 
 
 
            List<ExportStudentBeanData> list = s_Entity.StudentOrride(id);
            var data = list.Select(s => new SiliconValley.InformationSystem.Entity.ViewEntity.StudentData
            {
                stuSex = s.StuSex,
                StuName = s.StuName,
                StuPhone = s.Stuphone,
                StuSchoolName = s.StuSchoolName,
                StuAddress = s.StuAddress,
                StuInfomationType_Id = s.stuinfomation,
                StuStatus_Id = s.StatusName,
                StuIsGoto = s.StuisGoto == false ? "否" : "是",
                StuVisit = s.StuVisit,
                EmployeesInfo_Id = s.empName,
                StuDateTime = s.BeanDate,
                StuEntering = s.StuEntering,
                AreName = s.RegionName
            }).ToList();
            ViewBag.Student = data;
            return View(data);
        }


        #endregion

        
        #region Excle文件导入
        //文件上传页面
        public ActionResult ExcleIntoView()
        {
            return View();
        }

        //处理文件上传的方法
        public ActionResult IntoFunction()
        {
            StringBuilder ProName = new StringBuilder();
            try
            {
                HttpPostedFileBase file = Request.Files["file"];
                string fname = Request.Files["file"].FileName; //获取上传文件名称（包含扩展名）
                string f = Path.GetFileNameWithoutExtension(fname);//获取文件名称
                string name = Path.GetExtension(fname);//获取扩展名
                string pfilename = AppDomain.CurrentDomain.BaseDirectory + "uploadXLSXfile/ConsultUploadfile/";//获取当前程序集下面的uploads文件夹中的excel文件夹目录
                //获取当前上传的操作人
                string UserName = Base_UserBusiness.GetCurrentUser().UserName;
                string completefilePath = f + DateTime.Now.ToString("yyyyMMddhhmmss") + UserName + name;//将上传的文件名称转变为当前项目名称
                ProName.Append(Path.Combine(pfilename, completefilePath));//合并成一个完整的路径;
                file.SaveAs(ProName.ToString());//上传文件   
                SessionHelper.Session["filename"] = ProName.ToString();
                bool studentlist = GetExcel();
                if (studentlist)//说明文件格式是可以读取的
                {
                    var jsondata = new
                    {
                        code = "",
                        msg = "ok",
                        data = SessionHelper.Session["ExcelData"] as List<MyExcelClass>,
                    };
                    return Json(jsondata, JsonRequestBehavior.AllowGet);
                }
                else //该文件格式不正确
                {
                    var jsondata = new
                    {
                        code = "",
                        msg = "文件格式错误",
                        data = "",
                    };
                    DeleteFile();//如果格式不符合规范则删除上传的文件
                    return Json(jsondata, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ee)
            {
                BusHelper.WriteSysLog(ee.Message, EnumType.LogType.上传文件);
                return Json("no", JsonRequestBehavior.AllowGet);
            }

        }

        //获取已备案中的相似数据
        public List<MyExcelClass> SercherStudent(List<MyExcelClass> ex)
        {
 
            List<MyExcelClass> list = new List<MyExcelClass>();
            foreach (MyExcelClass item1 in ex)
            {
                ExportStudentBeanData find = s_Entity.StudentOrrideData(item1.StuName, item1.StuPhone,null,null);
                if (find != null)
                {
                    MyExcelClass m = new MyExcelClass();
                    m.StuSex = find.StuSex;
                    m.StuName = find.StuName;
                    m.StuPhone = find.Stuphone;
                    m.StuSchoolName = find.StuSchoolName;
                    m.StuInfomationType_Id = find.stuinfomation;
                    m.Region_id = find.RegionName;
                    m.StuEducational = find.StuEducational;
                    m.StuAddress = find.StuAddress;
                    m.Reak = find.Reak;
                    m.EmployeesInfo_Id = find.empName;
                    m.QQ = find.StuQQ;
                    m.ConsultTeacher = find.ConsultTeacher;
                    if (list.Where(s => s.StuName == m.StuName && s.StuPhone == m.StuPhone).FirstOrDefault() == null)
                    {
                        list.Add(m);
                    }
                }
               
            }

            return list;
        }
        ///一个删除文件的方法
        public void DeleteFile()
        {
            var namef = SessionHelper.Session["filename"];
            var namef2 = SessionHelper.Session["filename2"];
            if (namef != null)
            {
                FileInfo fi = new FileInfo(namef.ToString());
                bool ishave = fi.Exists;
                if (ishave)
                {
                    fi.Delete();
                }
            }

        }

        //获取Excle表要导入的数据(目的:有些是区域外的，会被处理)
        public bool GetExcel()
        {
            string namef = SessionHelper.Session["filename"].ToString();//获取要读取的Excel文件名称
            System.Data.DataTable t = AsposeOfficeHelper.ReadExcel(namef, false);//从Excel文件拿值
            List<MyExcelClass> new_listStudent = new List<MyExcelClass>();
            if (t.Rows[0][0].ToString() == "姓名" && t.Rows[0][1].ToString() == "性别" && t.Rows[0][2].ToString() == "电话" && t.Rows[0][3].ToString() == "学校" && t.Rows[0][4].ToString() == "家庭住址" && t.Rows[0][5].ToString() == "区域" && t.Rows[0][6].ToString() == "信息来源" && t.Rows[0][7].ToString() == "学历" && t.Rows[0][8].ToString() == "咨询师" && t.Rows[0][9].ToString() == "备案人" && t.Rows[0][10].ToString() == "关联人" && t.Rows[0][11].ToString() == "QQ" && t.Rows[0][12].ToString() == "备注")
            {
                //直接设为Excel实体
                for (int i = 1; i < (t.Rows.Count); i++)
                {
                    MyExcelClass create_s = new MyExcelClass();
                    create_s.StuName = t.Rows[i][0].ToString();
                    create_s.StuSex = t.Rows[i][1].ToString();
                    create_s.StuPhone = t.Rows[i][2].ToString();
                    create_s.StuSchoolName = t.Rows[i][3].ToString();//学校
                    create_s.StuAddress = t.Rows[i][4].ToString();//家庭住址
                    create_s.Region_id = t.Rows[i][5].ToString();//区域
                    create_s.StuInfomationType_Id = t.Rows[i][6].ToString();//信息来源
                    create_s.StuEducational = t.Rows[i][7].ToString();
                    create_s.ConsultTeacher = t.Rows[i][8].ToString();//咨询师
                    create_s.EmployeesInfo_Id = t.Rows[i][9].ToString();//备案人
                    create_s.Party = t.Rows[i][10].ToString();
                    create_s.QQ = t.Rows[i][11].ToString();
                    create_s.Reak = t.Rows[i][12].ToString();//备注                    
                    new_listStudent.Add(create_s);
                }
                DeleteFile();
                SessionHelper.Session["ExcelData"] = new_listStudent;
                return true;
            }
            else
            {
                DeleteFile();
                return false;
            }
        }

        //判断是否有重复的值true--获取相同的数据false--获取没有冲突的数据
        public List<MyExcelClass> Repeatedly(bool IsRepea)
        {
            List<MyExcelClass> ExcelList = SessionHelper.Session["ExcelData"] as List<MyExcelClass>;//获取Excle文件数据     
            
            List<MyExcelClass> result = new List<MyExcelClass>();

            foreach (MyExcelClass a2 in ExcelList)
            {
                if (s_Entity.StudentOrrideData(a2.StuName, a2.StuPhone,null,null) != null)
                {
                    result.Add(a2);
                }
            }
            if (IsRepea)
            {
                //获取冲突的数据                
                return result;
            }
            else
            {
                //获取没有冲突的数据                
                for (int i = 0; i < result.Count; i++)
                {
                     
                    ExcelList.RemoveAll(e => e.StuName == result[i].StuName);
                }
                return ExcelList;
            }

        }

        //将Excel中的数据导入到数据库中
        public bool AddExcelToServer(List<MyExcelClass> list)
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
 
            AjaxResult add_result = new AjaxResult();
            try
            {
                List<StudentPutOnRecord> listnew = new List<StudentPutOnRecord>();                
                foreach (MyExcelClass item1 in list)
                {
                    if (!string.IsNullOrEmpty(item1.StuName))
                    {
                        StudentPutOnRecord s = new StudentPutOnRecord();
                        s.StuName = item1.StuName;
                        s.StuSex = item1.StuSex;
                        s.EmployeesInfo_Id = s_Entity.Enplo_Entity.FindEmpData(item1.EmployeesInfo_Id, false) == null ? null : s_Entity.Enplo_Entity.FindEmpData(item1.EmployeesInfo_Id, false).EmployeeId;
                        s.IsDelete = 0;
                        s.Reak = item1.Reak;
                        Region find_region = s_Entity.region_Entity.SerchRegionName(item1.Region_id, false);
                        if (find_region != null) { s.Region_id = find_region.ID; } else { s.Reak = s.Reak + ",所在区域:" + item1.Region_id; }
                        s.StatusTime = null;
                        s.StuAddress = item1.StuAddress;
                        s.StuBirthy = null;
                        s.StuDateTime = DateTime.Now;
                        s.BeanDate = DateTime.Now;
                        s.StuEducational = item1.StuEducational;
                        s.StuEntering = s_Entity.Enplo_Entity.GetEntity(UserName.EmpNumber).EmpName;
                        StuInfomationType find_sinfomation = s_Entity.StuInfomationType_Entity.SerchSingleData(item1.StuInfomationType_Id, false);
                        if (find_sinfomation != null) { s.StuInfomationType_Id = find_sinfomation.Id; } else { s.StuInfomationType_Id = 1; };
                        s.StuIsGoto = false;
                        s.StuPhone = item1.StuPhone;
                        s.StuSchoolName = item1.StuSchoolName;
                        s.StuStatus_Id = 1013;
                        s.StuQQ = item1.QQ;
                        s.Party = item1.Party;
                        if (item1.ConsultTeacher==null || !string.IsNullOrEmpty(item1.ConsultTeacher))
                        {
                            s.ConsultTeacher = item1.ConsultTeacher;
                        }
                         
                        listnew.Add(s);
                    }
                    
                }
                add_result = s_Entity.Add_data(listnew);

                ///向分量表添加数据
                if (add_result.Success)
                {
                    List<StudentPutOnRecord> one = listnew.Where(l => l.ConsultTeacher != null).ToList();
                    List<Consult> c_list = new List<Consult>(); 
                    foreach (StudentPutOnRecord item in one)
                    {
                        ConsultTeacher find_t = EmployandCounTeacherCoom.Consult_entity.FindOne(item.ConsultTeacher);
                        if (find_t!= null)
                        {
                            Consult c = new Consult();
                            c.TeacherName = find_t.Id;
                            c.StuName = s_Entity.StudentOrreideData_OnRecord(item.StuName, item.StuPhone, item.BeanDate,0).Id;
                            c.IsDelete = false;
                            c.ComDate = DateTime.Now;
                            c_list.Add(c);
                        }
                         
                    }


                    //添加分量
                    EmployandCounTeacherCoom.consult.Add_Data(c_list);

                }

                if (add_result.Success==true)
                {
                    List<StudentPutOnRecord> list_W = new List<StudentPutOnRecord>();
                    //查询备案中是网络备案
                    StuInfomationType find= s_Entity.StuInfomationType_Entity.SerchSingleData("网络", false);
                    listnew= listnew.Where(l => l.StuInfomationType_Id == find.Id).ToList();
                    foreach (StudentPutOnRecord item in listnew)
                    {
                        StudentPutOnRecord m= s_Entity.StudentOrreideData_OnRecord(item.StuName, item.StuPhone,item.BeanDate,0);
                        if (m!=null)
                        {
                            list_W.Add(m);
                        }
                    }

                    //向网络回访添加数据
                    bool s= s_Entity.NetClient_Entity.AddNCRData(list_W);
                }
            }

            catch (Exception)
            {
                //BusHelper.WriteSysLog(UserName + "Excel大批量导入数据时出现:" + ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }


            return add_result.Success;
        }
        //将文件中的内容写入到数据库中
        public ActionResult IntoServer()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息

            string result = "no";
            string msg = "系统错误,请重试！！！";

            Excel_Entity = new ExcelHelper();

            try
            {
                List<MyExcelClass> equally_list = Repeatedly(true); //两个数据集合之间比较取交集(挑出相同的)
                List<MyExcelClass> equally_list2 = Repeatedly(false);//将未重复的数据添加到数据库中
                if (equally_list.Count > 0)
                {                    
                    
                    if (equally_list2.Count>0)
                    {
                        bool mis = AddExcelToServer(equally_list2);  //添加数据

                        if (!mis)
                        {
                            var json = new { resut = result, msg = msg };
                            return Json(json, JsonRequestBehavior.AllowGet);
                        }

                        //通知备案人备案成功
                        string number = s_Entity.Enplo_Entity.FindEmpData(equally_list2[0].EmployeesInfo_Id, false).Phone;
                        string smsText = "备案提示:Excel文件备案成功,但是有重复数据，请去系统查看！！！";
                        string t = PhoneMsgHelper.SendMsg(number, smsText);

                        StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now, userId = UserName.EmpNumber, operationType = Entity.Base_SysManage.EnumType.LogType.添加数据 + ":Excel备案成功,但是有重复数据！" };
                        s_Entity.log_s.Add_data(log);
                    }

                    //有重复的值                                
                    //获取当前年月日
                    //string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + equally_list[0].EmployeesInfo_Id + ".xls";
                    //string path = "~/uploadXLSXfile/ConsultUploadfile/ConflictExcel/" + filename;
                    //SessionHelper.Session["filename2"] = path;
                    ////获取表头数据
                    //string jsonfile = Server.MapPath("/Config/MyExcelClass.json");
                    //System.IO.StreamReader file = System.IO.File.OpenText(jsonfile);
                    //JsonTextReader reader = new JsonTextReader(file);
                    ////转化为JObject
                    //JObject ojb = (JObject)JToken.ReadFrom(reader);

                    //var jj = ojb["MyExcelClass"].ToString();

                    //JObject jo = (JObject)JsonConvert.DeserializeObject(jj);
                    //DataTable user = equally_list.ToDataTable<MyExcelClass>();
                    ////生成字段名称 
                    //List<string> Head = new List<string>();
                    //foreach (DataColumn col in user.Columns)
                    //{
                    //    Head.Add(jo[col.ColumnName].ToString());
                    //}
                    //bool s = Excel_Entity.DaoruExcel(equally_list, Server.MapPath(path), Head);//将有重复的数据写入Excel表格中
                    //if (s)
                    //{
                        //获取已备案的数据集合
                        var data = new
                        {
                            result = "okk",
                            msg = "其他数据已导入,以下是有冲突的数据",
                            old = SercherStudent(equally_list),
                            news = equally_list
                        };
                         
                        return Json(data, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    //写入出错
                    //    DeleteFile();
                    //}
                }
                else if(equally_list2.Count>0)
                {
                    //没有重复的值
                    List<MyExcelClass> nochongfu_list = SessionHelper.Session["ExcelData"] as List<MyExcelClass>;
                    if (AddExcelToServer(nochongfu_list))
                    {
                        result = "ok";
                        //通知备案人备案成功
                        string number = s_Entity.Enplo_Entity.FindEmpData(nochongfu_list[0].EmployeesInfo_Id,false).Phone;
                        string smsText = "备案提示:Excel文件备案成功，无重复数据";
                        string t = PhoneMsgHelper.SendMsg(number, smsText);

                        StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now, userId = UserName.EmpNumber, operationType = Entity.Base_SysManage.EnumType.LogType.添加数据 + ":Excel备案成功,没有重复数据！" };
                        s_Entity.log_s.Add_data(log);
                    }
                }

            }
            catch (Exception ex)
            {

                StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now, userId = UserName.EmpNumber, operationType = Entity.Base_SysManage.EnumType.LogType.添加数据error +ex.Message };
                s_Entity.log_s.Add_data(log);
                result = "no";
                msg = "系统异常，请刷新重试！！！";
            }

            var datajson = new{resut = result,msg = msg};

            return Json(datajson, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 模板下载 
        /// </summary>
        /// <returns></returns>        
        public FileStreamResult DownFile()
        {
            //const string accessKeyId = "a43996ac0c6d40c69d3ebb47127909e9"; // 用户的Access Key ID
            //const string secretAccessKey = "2cfdf8b1f0e548f28cafcfd1aafc9226"; // 用户的Secret Access Key
            //const string endpoint = "http://bj.bcebos.com";
            //// 初始化一个BosClient
            //BceClientConfiguration config = new BceClientConfiguration();
            // config.Credentials = new DefaultBceCredentials(accessKeyId, secretAccessKey);
            //config.Endpoint = endpoint;
            //BosClient client = new BosClient(config);


            //var filedata = client.GetObject("xinxihua", "/TangminFiles/Template/Excle模板.xls");

            //Stream  stream=  s_Entity.MyFiles.DownloadFile("xinxihua", "/TangminFiles/Template/", "Excle模板.xls");


            string rr = Server.MapPath("/uploadXLSXfile/Template/Excle模板.xls");  //获取下载文件的路径         
            FileStream stream = new FileStream(rr, FileMode.Open);
            return File(stream, "application/octet-stream", Server.UrlEncode("ExcleTemplate.xls"));
        }

        #endregion


        #region 疑似数据业务
        public ActionResult SuspectedView()
        {
            return View();
        }

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <returns></returns>
        public void LoadFile()
        {
            string id = Request.QueryString["id"];
            if (!string.IsNullOrEmpty(id) && id != "undefined")
            {
                string path = Server.MapPath("~/uploadXLSXfile/ConsultUploadfile/ConflictExcel/" + id);

                FileStream fs = new FileStream(path, FileMode.Open);
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                //二进制流数据（如常见的文件下载）
                Response.ContentType = "application/octet-stream";
                //通知浏览器下载文件而不是打开 
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(id, System.Text.Encoding.UTF8));
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
        }

        public ActionResult PageData()
        {
            int page = Convert.ToInt32(Request.Form["page"]);
            int limit = Convert.ToInt32(Request.Form["limit"]);
            string filename = Request.Form["filename"];
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(Server.MapPath("~/uploadXLSXfile/ConsultUploadfile/ConflictExcel"));
            FileInfo[] files = di.GetFiles();
            List<SelectListItem> FilesName = new List<SelectListItem>();
            foreach (FileInfo f in files)
            {
                SelectListItem s = new SelectListItem();
                int lenth = f.Name.Length;
                s.Text = f.Name.Substring(0, 4) + "年" + f.Name.Substring(4, 2) + "月" + f.Name.Substring(6, 2) + "日" + f.Name.Substring(14, lenth - 14);
                s.Value = f.Name;
                FilesName.Add(s);
            }

            if (!string.IsNullOrEmpty(filename))
            {
                FilesName = FilesName.Where(f => f.Value.Contains(filename)).ToList();
            }
            int count = Convert.ToInt32(Math.Ceiling(FilesName.Count / 12.0));
            List<SelectListItem> selectdata = FilesName.Skip(((Convert.ToInt32(page) - 1) * Convert.ToInt32(limit))).Take(Convert.ToInt32(limit)).ToList();

            var data = new { count = count, data = selectdata, page = page };

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult FielsPath()
        {
            string name = Request.Form["id"];
            List<string> list = new List<string>();
            string[] names = name.Split(',');
            foreach (string id in names)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    list.Add(Server.MapPath("~/uploadXLSXfile/ConsultUploadfile/ConflictExcel/" + id));
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Excel数据导出

        /// <summary>
        /// 数据筛选导出页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportView()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            ViewBag.Pers = s_Entity.GetPostion(UserName.EmpNumber);

            List<SelectListItem> ss = new List<SelectListItem>();
            ss.Add(new SelectListItem() { Value = "0", Text = "全部", Selected = true });
            ss.AddRange(s_Entity.Stustate_Entity.GetList().Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusName }).ToList());

            ViewBag.slist = ss;

            return View();
        }

        /// <summary>
        /// 生成Excel文件
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExportFunction()
        {
            AjaxResult a = new AjaxResult();
            string zhuangtai = Request.Form["S_status"];//学生状态
            string oneTime = Request.Form["oneTime"];//开始日期
            string twoTime = Request.Form["twoTime"];//结束日期
            int page =Convert.ToInt16(Request.Form["page"]);

            string sql = "select * from StudentBeanView where statusName='"+zhuangtai+"' and BeanDate >= '" + oneTime + "' and BeanDate <='" + twoTime + "' and IsDelete=0";
            if (zhuangtai =="0") {
                sql = "select * from StudentBeanView where BeanDate >= '" + oneTime + "' and BeanDate <='" + twoTime + "' and IsDelete=0";
            }
            List<ExportStudentBeanData> Exportlist = s_Entity.GetListBySql<ExportStudentBeanData>(sql);
  
            var list = Exportlist.Select(ac => new{
                StuName = ac.StuName ?? string.Empty,
                StuSex = ac.StuSex ?? string.Empty,
                StuBirthy = ac.StuBirthy?.ToString() ?? string.Empty,
                Stuphone = ac.Stuphone?? string.Empty,
                StuSchoolName = ac.StuSchoolName?? string.Empty,
                StuEducational = ac.StuEducational ?? string.Empty,
                StuAddress = ac.StuAddress?? string.Empty,
                StuWeiXin = ac.StuWeiXin ?? string.Empty,
                StuQQ = ac.StuQQ ?? string.Empty,
                stuinfomation = ac.stuinfomation?? string.Empty,
                StatusName = ac.StatusName ?? string.Empty,
                StuVisit = ac.StuVisit?.ToString()?? string.Empty,
                empName = ac.empName?? string.Empty,
                BeanDate = ac.BeanDate.ToString()?? string.Empty,
                StatusTime = ac.StatusTime?.ToString()?? string.Empty,
                RegionName = ac.RegionName ?? string.Empty,
                ConsultTeacher = ac.ConsultTeacher ?? string.Empty,
                Party = ac.Party?? string.Empty
            }).ToList();

            var jsondata = new { title = "", Data = list, Success = true,count=0 };
            if (list.Count > 7000)
            {
                jsondata = new { title = "备案数据Excel", Data = list.Skip((page-1)*7000).Take(7000).ToList(), Success = true, count = (list.Count / 7000)+1 };
            }
            else
            {
                jsondata = new { title = "备案数据Excel", Data = list, Success = true, count = 0 };
            }
            return Json(jsondata,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取Excel备案所有数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetExcelport()
        {
            string truePath = SessionHelper.Session["truePath"].ToString();
            FileStream stream = new FileStream(truePath, FileMode.Open);
            return File(stream, "application/octet-stream", Server.UrlEncode("Excel备案数据.xls"));
        }       
        #endregion


        #region 远程数据导入       
        public ActionResult InlocalServer()
        {
            s_Entity.InServer();
            //s_Entity.FF();
            return null;
        }
        #endregion


        #region 学员注册
        //班主任注册页面
        public ActionResult HandMasterRegisterds()
        {
            return View();
        }
        //注册页面
        public ActionResult Registeredstudentnumber(string id)
        {
            List<ZhuceShowData> list = new List<ZhuceShowData>();
            string[] ids = id.Split(',');
            foreach (string item in ids)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    ZhuceShowData data = new ZhuceShowData();
                    int Myid = Convert.ToInt32(item);
                    if (Myid<= 54117)
                    {
                       Sch_Market findda= s_Entity.whereMarketId(Myid);
                        data.stuName = findda.StudentName;
                        data.stuSex = findda.Sex;
                        data.stuPhone = findda.Phone;
                        data.IdCare = s_Entity.GetylfCord(Myid) == "" ? null : s_Entity.GetylfCord(Myid);
                        data.Id = item;
                    }
                    else
                    {
                       StudentPutOnRecord finda= s_Entity.whereStudentId(Myid);
                        data.stuName = finda.StuName;
                        data.stuSex = finda.StuSex;
                        data.stuPhone = finda.StuPhone;
                        data.IdCare= s_Entity.GetylfCord(Myid) == "" ? null : s_Entity.GetylfCord(Myid);
                        data.Id = item;
                    }
                    list.Add(data);
                }
            }

            ViewBag.list = list;
            return View();
        }
        //注册方法
        [HttpPost]
        public ActionResult RegistersFunction(ZhuceShowData data)
        {
            StudentInformationBusiness informationBusiness = new StudentInformationBusiness();
            StudentInformation student = new StudentInformation();
            student.Name = data.stuName;
            student.Sex = data.stuSex=="男"?true:false;
            AjaxResult a = new AjaxResult();
            bool m = true;
            if (data.YesHei)
            {
                bool sm= Regex.IsMatch(data.IdCare, "^\\d{8}$");
                if(!sm)
                {
                    a.Success = false;
                    a.Msg = "出生年月日不正确！！";

                    return Json(a, JsonRequestBehavior.AllowGet);
                }
                AjaxResult sust = s_Entity.GetIdCard(data.IdCare);
                if (sust.Success==false)
                {
                    return Json(sust, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    student.identitydocument = sust.Data as string;
                }

                
                //添加到黑户表
                HeiHu hu = new HeiHu();
                hu.IdCard = student.identitydocument;
                bool s= s_Entity.heiHu.Add_SingData(hu);
            }
            else
            {
                //判断身份证
                m = s_Entity.TrueCrad(data.IdCare);
                student.identitydocument = data.IdCare;
            }
             
            student.Telephone = data.stuPhone;          
            if (m)
            {               
                //判断学号是否被注册
                int count = informationBusiness.GetList().Where(g => g.StudentPutOnRecord_Id == Convert.ToInt32(data.Id)).Count();
                if (count > 0)
                {
                    a.Success = false;
                    a.Msg = "该学生已注册过学号！";
                    return Json(a, JsonRequestBehavior.AllowGet);
                }

                a = informationBusiness.StudfentEnti(student, Convert.ToInt32(data.Class_ID), Convert.ToInt32(data.Id), data.gradeName);
                
            }
            else
            {
                a.Success = false;
                a.Msg = "身份证格式错误！";
            }
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 图片上传页面
        /// </summary>
        /// <returns></returns>
        public ActionResult IdCordLoad()
        {
            return View();
        }

        /// <summary>
        /// 获取身份证
        /// </summary>
        /// <returns></returns>
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
                result.Data = data.CardNumber;
            }
            catch (Exception ex)
            {

                result.ErrorCode = 500;
                result.Msg = "异常";
                result.Data = null;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult UpDataStudentName(int id)
        {
           ExportStudentBeanData data= s_Entity.findId(id.ToString());
            return View(data);
        }

        /// <summary>
        /// 修改学生姓名跟性别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpDataStudentFunction(ExportStudentBeanData news)
        {
            AjaxResult a = new AjaxResult();
            if (news.Id>=54118)
            {
                StudentPutOnRecord findata= s_Entity.whereStudentId(news.Id);
                findata.StuName = news.StuName;
                findata.StuSex = news.StuSex;
                a.Success= s_Entity.My_update(findata);
                a.Msg = "操作成功！！！";
            }
            else
            {
               Sch_Market findata= s_Entity.whereMarketId(news.Id);
                findata.StudentName = news.StuName;
                findata.Sex = news.StuSex;
                a= s_Entity.s_entity.MyUpdate(findata);

            }

            return Json(a,JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 一键转咨询师
        public ActionResult ChangTeacher()
        {
            string[] id = Request.Form["id"].Split(',') ;
            int teacherid =Convert.ToInt32(Request.Form["teacherid"]);
            List<int> list = new List<int>();
            List<StudentPutOnRecord> s_list = new List<StudentPutOnRecord>();
            List<Sch_Market> s_list2 = new List<Sch_Market>();
            foreach (string item in id)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    int number = Convert.ToInt32(item);
                    list.Add(number);

                    if (number>=54118)
                    {
                        StudentPutOnRecord studentPut= s_Entity.whereStudentId(number);
                        studentPut.ConsultTeacher = s_Entity.Enplo_Entity.GetEntity(EmployandCounTeacherCoom.Consult_entity.GetEntity(teacherid).Employees_Id).EmpName;
                        s_list.Add(studentPut);
                    }
                    else
                    {
                        Sch_Market market = s_Entity.whereMarketId(number);
                        market.Inquiry= s_Entity.Enplo_Entity.GetEntity(EmployandCounTeacherCoom.Consult_entity.GetEntity(teacherid).Employees_Id).EmpName;
                        s_list2.Add(market);
                    }

                }
            }

            AjaxResult a= EmployandCounTeacherCoom.consult.ChangTeacher(list, teacherid);
            if (s_list.Count>0)
            {
               a= s_Entity.Update_data(s_list);
            }

            if (s_list2.Count>0)
            {
                a = s_Entity.s_entity.MyUpdate(s_list2);
            }
            return Json(a, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 获取缴费情况
        public ActionResult GetStudentNumber(string id)
        {
            BaseBusiness<StudentInformation> su_entity = new BaseBusiness<StudentInformation>();
            List<StudentInformation> list= su_entity.GetListBySql<StudentInformation>("select * from StudentInformation where StudentPutOnRecord_Id=" + id);
            //判断当前登陆人(网络部只能看网络的数据)
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息

            Department department= s_Entity.Enplo_Entity.GetDeptByEmpid(UserName.EmpNumber);

            AjaxResult a = new AjaxResult();

            ExportStudentBeanData findata= s_Entity.findId(id);

            StudentFeeStandardBusinsess dbtext = new StudentFeeStandardBusinsess();

            if (department.DeptName.Contains("网络部"))
            {
                if (findata.stuinfomation.Contains("网络"))
                {
                    if (list.Count > 0)
                    {                        

                        List<vierprice> list_cout= dbtext.FienPrice(list[0].StudentNumber);

                        if (list_cout.Count>0)
                        {
                            a.Success = true;
                            a.Data = list[0];
                        }
                        else
                        {
                            a.Success = false;
                            a.Msg = "目前没有检测到缴费信息，请联系财务进行查询！";
                        }
                         
                    }
                    else
                    {
                        a.Success = false;
                        a.Msg = "该学生没有学号，无法查询缴费！";
                    }

                }
                else
                {
                    a.Success = false;
                    a.Msg = "你没有权限查看该学生的缴费详情！！！";
                }
            }
            else
            {
                if (list.Count > 0)
                {
                    List<vierprice> list_cout = dbtext.FienPrice(list[0].StudentNumber);

                    if (list_cout.Count > 0)
                    {
                        a.Success = true;
                        a.Data = list[0];
                    }
                    else
                    {
                        a.Success = false;
                        a.Msg = "目前没有检测到缴费信息，请联系财务进行查询！";
                    }

                }
                else
                {
                    a.Success = false;
                    a.Msg = "该学生没有学号，无法查询缴费！";
                }
            }

            return Json(a,JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 缴费详情
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPayView(string id)
        {
            StudentFeeStandardBusinsess dbtext = new StudentFeeStandardBusinsess();
            string student = id;
            ViewBag.vier = dbtext.FienPrice(student);//查询缴费记录
            ViewBag.Tuitionrefund = dbtext.FienTuitionrefund(dbtext.FienPrice(student));
            ViewBag.StudentPrentryfeeDate = dbtext.StudentPrentryfeeDate(student);
            return View();
        }
        #endregion


        #region 获取在校学生信息
        /// <summary>
        /// 判断某个学生是否有在校信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Ture(int id)
        {
            AjaxResult a = new AjaxResult();
            a.Success = true;
            PutStudentDataView find = s_Entity.FindStudentData(id);
            if (find==null)
            {
                a.Success = false;
            }

            return Json(a,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudent(int id)
        {
             
            PutStudentDataView find= s_Entity.FindStudentData(id);
            ViewBag.Teacher = "";
            if (find!=null)
            {
                ViewBag.Teacher = s_Entity.GetTeacher(find.ClassID).EmpName;
            }
       
            //根据备案Id获取数据
            return View(find);
        }
        #endregion


        #region 短信发送
        public ActionResult ShortInfoMationView(int id)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text="--请选择--",Value="0" ,Selected=true});
              // 获取属于这个区域的市场老师             
              list.AddRange( s_Entity.Channerl_Entity.GetAreaEmplist(id).Select(l=>new SelectListItem() { Text=l.EmpName,Value=l.Phone,Selected=false}).ToList());
              ViewBag.list = list;
            return View();
        }
       
        //public ActionResult ShortInfomationFuntion()
        //{
        //    string phone= Request.Form["phone"];
        //    string rand = Request.Form["rank"];

        //    string t = PhoneMsgHelper.SendMsg(phone, rand);

        //    return Json(t, JsonRequestBehavior.AllowGet);
        //}
        #endregion        


        #region 报名、预录
        public ActionResult Sign_up()
        {
            //List<SelectListItem> list  = s_Entity.StuInfomationType_Entity.GetList().Where(s => s.IsDelete == false).Select(s=>new SelectListItem() { Text=s.Name,Value=s.Id.ToString()}).ToList();

            //ViewBag.list = list;

            //获取阶段
            ViewBag.glist= s_Entity.grand.GetList().Where(g => g.IsDelete == false).Select(g => new SelectListItem() { Text=g.GrandName,Value=g.Id.ToString()});
            return View();
        }
        
        [HttpPost]
        public ActionResult Signupfunction()
        {
            DateTime date =Convert.ToDateTime(Request.Form["date"]);//日期
            int type =Convert.ToInt32(Request.Form["type"]);//类型
            //int infomation =Convert.ToInt32( Request.Form["infomation"]);//信息来源
            int grand = Convert.ToInt32(Request.Form["grand"]);

            //if (infomation!=0)
            //{

            //}
            AjaxResult data= s_Entity.Costitems.TypeSelect(date, type,grand);


            return Json(data,JsonRequestBehavior.AllowGet);
        }

        #endregion        


        #region 获取跟踪详情
        public ActionResult FllowView(int id)
        {
            //获取该备案数据的所有跟踪结果
            Consult consult= EmployandCounTeacherCoom.consult.AccordingStuIdGetConsultData(id);
            List<FollwingInfo> list= EmployandCounTeacherCoom.consult.Fi_Entity.GetFoll_ConsltId(consult.Id).OrderBy(c=>c.FollwingDate).ToList();
            ViewBag.list = list;
            return View();
        }
        #endregion


        #region 给市场显示的数据
          public ActionResult MarkeView()
          {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            //获取信息来源的所有数据
            List<SelectListItem> se = s_Entity.StuInfomationType_Entity.GetList().Select(s => new SelectListItem { Text = s.Name, Value = s.Name }).ToList();
            se.Add(new SelectListItem() { Text = "请选择", Selected = true, Value = "0" });
            ViewBag.infomation = se;
            //获取区域所有信息
            SelectListItem newselectitem = new SelectListItem() { Text = "请选择", Value = "0", Selected = true };
            var r_list = s_Entity.GetEffectiveRegionAll(true).Select(r => new SelectListItem { Text = r.RegionName, Value = r.RegionName }).ToList();
            r_list.Add(newselectitem);
            ViewBag.are = r_list;
            //获取咨询师的所有数据
            List<SelectListItem> list_cteacher = new List<SelectListItem>();
            List<SelectListItem> list_one = new List<SelectListItem>();
            list_cteacher.Add(new SelectListItem() { Text = "请选择", Value = "0", Selected = true });
            list_one.Add(new SelectListItem() { Text = "请选择", Value = "0", Selected = true });
            list_cteacher.AddRange(EmployandCounTeacherCoom.getallCountTeacher(true).Select(c => new SelectListItem() { Text = c.empname, Value = c.empname }).ToList());
            list_one.AddRange(EmployandCounTeacherCoom.GetTeacher().Select(c => new SelectListItem() { Text = c.Employees_Id, Value = c.Id.ToString() }).ToList());
            ViewBag.teacherlist = list_cteacher;
            ViewBag.Teacher = list_one;
            //获取学生状态所有数据
            List<SelectListItem> ss = new List<SelectListItem>();
            ss.Add(new SelectListItem() { Value = "0", Text = "请选择", Selected = true });
            ss.AddRange(s_Entity.Stustate_Entity.GetList().Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusName }).ToList());

            ViewBag.slist = ss;

            ViewBag.Pers = s_Entity.GetPostion(UserName.EmpNumber);

            //获取市场类型
            ViewBag.type = Marketgrand();
            return View();
          }
        /// <summary>
        /// 模糊查询的
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetTableData_Mark(int limit, int page)
        {
            List<ExportStudentBeanData> list = new List<ExportStudentBeanData>();
            //获取当前登录人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            EmployeesInfo indata= s_Entity.Enplo_Entity.FindEmpData(UserName.EmpNumber, true) ;
            if (indata!=null)
            {
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                sb1.Append("select * from StudentBeanView  where 1=1 and  empName='"+ indata.EmpName + "'");
                sb2.Append("select * from Sch_MarketView where 1=1 and SalePerson='"+ indata.EmpName+ "' or RelatedPerson='"+ indata.EmpName + "'");                
                 
                    #region 模糊查询
                    string findNamevalue = Request.QueryString["findNamevalue"];//姓名
                    string findPhonevalue = Request.QueryString["findPhonevalue"];//电话
                    string findInformationvalue = Request.QueryString["findInformationvalue"];//信息来源
                    string findStartvalue = Request.QueryString["findStartvalue"];//录入开始时间
                    string findEndvalue = Request.QueryString["findEndvalue"];//录入结束时间
                    string findBeanManvalue = Request.QueryString["findBeanManvalue"];//备案人
                    string findAreavalue = Request.QueryString["findAreavalue"];//区域
                    string findTeacher = Request.QueryString["S_consultTeacher"];//咨询师
                    string findStatus = Request.QueryString["S_status"];//备案状态
                    string findPary = Request.QueryString["S_party"];//关系人
                    string findCreateMan = Request.QueryString["S_intosysMan"];//录入人
                    string markety = Request.QueryString["marketype"];//市场类型

                    string qq = Request.QueryString["S_QQ"];//QQ
                    string edution = Request.QueryString["eduttion"];//学历
                    string reack = Request.QueryString["S_Reack"];//其他说明
                    if (!string.IsNullOrEmpty(findNamevalue))
                    {
                        sb1.Append("and  StuName like  '" + findNamevalue + "%'");
                        sb2.Append(" and StudentName like  '" + findNamevalue + "%'");
                    }
                    if (!string.IsNullOrEmpty(findPhonevalue))
                    {
                        sb1.Append(" and Stuphone = '" + findPhonevalue + "'");
                        sb2.Append(" and Phone = '" + findPhonevalue + "'");
                    }
                    if (findInformationvalue != "0" && !string.IsNullOrEmpty(findInformationvalue))
                    {
                        sb1.Append(" and stuinfomation = '" + findInformationvalue + "'");
                        sb2.Append(" and source = '" + findInformationvalue + "'");
                    }
                    if (!string.IsNullOrEmpty(findBeanManvalue))
                    {
                        sb1.Append(" and empName = '" + findBeanManvalue + "'");
                        sb2.Append(" and SalePerson = '" + findBeanManvalue + "'");
                    }
                    if (findAreavalue != "0" && !string.IsNullOrEmpty(findAreavalue))
                    {
                        sb1.Append(" and RegionName = '" + findAreavalue + "'");
                        sb2.Append(" and Area = '" + findAreavalue + "'");
                    }
                    if (findStatus != "0" && !string.IsNullOrEmpty(findStatus))
                    {
                        sb1.Append(" and StatusName = '" + findStatus + "'");
                        sb2.Append(" and  MarketState like '已报名%'");
                    }
                    if (!string.IsNullOrEmpty(findPary))
                    {
                        sb1.Append(" and Party = '" + findPary + "'");
                        sb2.Append(" and RelatedPerson = '" + findPary + "'");
                    }
                    if (!string.IsNullOrEmpty(findCreateMan))
                    {
                        sb1.Append(" and StuEntering = '" + findCreateMan + "'");
                        sb2.Append(" and CreateUserName = '" + findCreateMan + "'");
                    }

                    if (!string.IsNullOrEmpty(findStartvalue))
                    {
                        sb1.Append(" and BeanDate >= '" + findStartvalue + "'");
                        sb2.Append(" and CreateDate >= '" + findStartvalue + "'");
                    }

                    if (!string.IsNullOrEmpty(findTeacher) && findTeacher != "0")
                    {
                        sb1.Append(" and ConsultTeacher = '" + findTeacher + "'");
                        sb2.Append(" and Inquiry = '" + findTeacher + "'");
                    }

                    if (!string.IsNullOrEmpty(findEndvalue))
                    {
                        sb1.Append(" and BeanDate <= '" + findEndvalue + "'");
                        sb2.Append(" and CreateDate <= '" + findEndvalue + "'");
                    }

                    if (markety != "0" && !string.IsNullOrEmpty(markety))
                    {
                        sb1.Append(" and MarketType = '" + markety + "'");
                        sb2.Append(" and MarketState like '" + markety + "%'");
                    }

                    if (!string.IsNullOrEmpty(qq))
                    {
                        sb1.Append(" and StuQQ = '" + qq + "'");
                        sb2.Append(" and QQ = '" + qq + "'");
                    }

                    if (!string.IsNullOrEmpty(edution) && edution != "0")
                    {
                        sb1.Append(" and StuEducational = '" + edution + "'");
                        sb2.Append(" and Education = '" + edution + "'");
                    }

                    if (!string.IsNullOrEmpty(reack))
                    {
                        sb1.Append(" and Reak like '" + reack + "%'");
                        sb2.Append(" and Remark like '" + reack + "%'");
                    }
                    #endregion
                
                list = s_Entity.Serch(sb1.ToString(), sb2.ToString(),true).OrderByDescending(s => s.StuDateTime).ToList();

                var data = list.Skip((page - 1) * limit).Take(limit).ToList();

                var josndata = new { code = 0, count = list.Count, data = data };

                return Json(josndata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult OnewData(int limit, int page)
        {
            List<ExportStudentBeanData> list = new List<ExportStudentBeanData>();
            //获取当前登录人
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            EmployeesInfo indata = s_Entity.Enplo_Entity.FindEmpData(UserName.EmpNumber, true);
            if (indata != null)
            {
                StringBuilder sb1 = new StringBuilder();
                StringBuilder sb2 = new StringBuilder();
                sb1.Append("select * from StudentBeanView  where 1=1 and  empName='" + indata.EmpName + "'");
                sb2.Append("select * from Sch_MarketView where 1=1 and SalePerson='" + indata.EmpName + "' or RelatedPerson='" + indata.EmpName + "'");
                
                list = s_Entity.Serch(sb1.ToString(), sb2.ToString(),true).OrderByDescending(s => s.StuDateTime).ToList();

                var data = list.Skip((page - 1) * limit).Take(limit).ToList();

                var josndata = new { code = 0, count = list.Count, data = data };

                return Json(josndata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

        }
        #endregion


        #region  修改账号密码
        public ActionResult updatePassword()
        {
            //获取咨询部的所有员工
             //.Select(s=>new SelectListItem() { Text=s.EmpName,Value=s_Entity.B_USER.GetUserByEmpid(s.EmployeeId).Id}).ToList();
            List<SelectListItem> empslect = new List<SelectListItem>();
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            if (s_Entity.GetPostion(UserName.EmpNumber)==0)//加载咨询部
            {
                List<EmployeesInfo> emp_list = s_Entity.Enplo_Entity.GetEmpsByDeptid(1);
                foreach (EmployeesInfo emp in emp_list)
                {
                    Base_User find = s_Entity.B_USER.GetUserByEmpid(emp.EmployeeId);
                    if (find != null)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Value = find.UserId;
                        s.Text = emp.EmpName;
                        empslect.Add(s);
                    }
                }
            }
            else 
            {
                //加载网络部
                List<EmployeesInfo> emp_list = s_Entity.Enplo_Entity.GetEmpsByDeptid(2);
                foreach (EmployeesInfo emp in emp_list)
                {
                    Base_User find = s_Entity.B_USER.GetUserByEmpid(emp.EmployeeId);
                    if (find != null)
                    {
                        SelectListItem s = new SelectListItem();
                        s.Value = find.UserId;
                        s.Text = emp.EmpName;
                        empslect.Add(s);
                    }
                }
            }
             
            ViewBag.emp_list = empslect;

            return View();
        }
       
        public ActionResult updatepasswordFunction(string userid,string passwd)
        {
            Base_UserBusiness db_user = new Base_UserBusiness();
            AjaxResult result = new AjaxResult();
            try
            {
                db_user.UpdatePassword(userid, passwd);
                result.ErrorCode = 200;
                result.Msg = "系统错误，请重试！！";
            }
            catch (Exception )
            {

                result.ErrorCode = 500;
                result.Msg = "系统错误，请重试！！";
            }

            return Json(result,JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region 数据作废页面
        public ActionResult Delete()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            //获取信息来源的所有数据
            List<SelectListItem> se = s_Entity.StuInfomationType_Entity.GetList().Select(s => new SelectListItem { Text = s.Name, Value = s.Name }).ToList();
            se.Add(new SelectListItem() { Text = "请选择", Selected = true, Value = "0" });
            ViewBag.infomation = se;
            //获取区域所有信息
            SelectListItem newselectitem = new SelectListItem() { Text = "请选择", Value = "0", Selected = true };
            var r_list = s_Entity.GetEffectiveRegionAll(true).Select(r => new SelectListItem { Text = r.RegionName, Value = r.RegionName }).ToList();
            r_list.Add(newselectitem);
            ViewBag.are = r_list;
            //获取咨询师的所有数据
            List<SelectListItem> list_cteacher = new List<SelectListItem>();
            List<SelectListItem> list_one = new List<SelectListItem>();
            list_cteacher.Add(new SelectListItem() { Text = "请选择", Value = "0", Selected = true });
            list_one.Add(new SelectListItem() { Text = "请选择", Value = "0", Selected = true });
            list_cteacher.AddRange(EmployandCounTeacherCoom.getallCountTeacher(true).Select(c => new SelectListItem() { Text = c.empname, Value = c.empname }).ToList());
            list_one.AddRange(EmployandCounTeacherCoom.GetTeacher().Select(c => new SelectListItem() { Text = c.Employees_Id, Value = c.Id.ToString() }).ToList());
            ViewBag.teacherlist = list_cteacher;
            ViewBag.Teacher = list_one;
            //获取学生状态所有数据
            List<SelectListItem> ss = new List<SelectListItem>();
            ss.Add(new SelectListItem() { Value = "0", Text = "请选择", Selected = true });
            ss.AddRange(s_Entity.Stustate_Entity.GetList().Select(s => new SelectListItem { Text = s.StatusName, Value = s.StatusName }).ToList());

            ViewBag.slist = ss;

            ViewBag.Pers = s_Entity.GetPostion(UserName.EmpNumber);

            //获取市场类型
            ViewBag.type = Marketgrand();
            return View();
        }

        public ActionResult DeleteFunction(int id)
        {
            AjaxResult a = new AjaxResult();
            a.Success= s_Entity.Shenhe_changeDele(id);

            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息


            if (a.Success)
            {
                a.Msg = "操作成功！";

                string reamk = Request.QueryString["reavke"];

                Delte_StudentPutinfo putinfo = new Delte_StudentPutinfo();
                putinfo.datetims = DateTime.Now;
                putinfo.Reamk = reamk;
                putinfo.StudentID = id;
                putinfo.UserName =s_Entity.Enplo_Entity.FindEmpData(UserName.EmpNumber,true).EmpName;

               a.Success= s_Entity.deletdata.Add_data(putinfo);
            }
            else
            {
                a.Msg = "数据异常！请刷新重试！";
            }
            return Json(a,JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// 冲突数据处理页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Conflict()
        {
            ViewBag.Ishava = 1;
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            if (UserName.EmpNumber== "202005300074" || UserName.EmpNumber== "202005300003")
            {
                ViewBag.Ishava = 2;
            }
             
            return View();
        }


        public ActionResult ConfilictData(int limit,int page)
        {
            List<ExportStudentBeanData> list= s_Entity.GetListBySql<ExportStudentBeanData>(" select * from  StudentBeanView where IsDelete=2");
            var data = list.OrderByDescending(a => a.BeanDate).Skip((page - 1) * limit).Take(limit);

            var jsondata = new {data=data,code=0,count=list.Count,msg="" };

            return Json(jsondata,JsonRequestBehavior.AllowGet); ; 
        }
       
        /// <summary>
        /// 将冲突数据改为正常数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ChuliConfilict(int id)
        {
            AjaxResult result = new AjaxResult() { Success=false,Msg="操作有误！请重试"};

            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            try
            {
                StudentPutOnRecord finda = s_Entity.GetEntity(id);
                string str = "select * from  StudentBeanView where StuName='" + finda.StuName + "' and Stuphone='" + finda.StuPhone + "' and IsDelete=0";
                var list= s_Entity.GetListBySql<ExportStudentBeanData>(str);

                var list2= s_Entity.s_entity.GetListBySql<Sch_MarketView>("select * from Sch_MarketView where StudentName='"+ finda.StuName + "' and Phone ='"+ finda.StuPhone + "' and IsDel=0");

                if ((list.Count+list2.Count)<=0)
                {
                    finda.IsDelete = 0;
                    s_Entity.Update(finda);
                    result.Msg = "操作成功！";
                    result.Success = true;
                    StudentbeanLog log = new StudentbeanLog() { insertDate = DateTime.Now, userId = UserName.EmpNumber, operationType = Entity.Base_SysManage.EnumType.LogType.编辑数据 + ":"  + "将一条编号为："+finda.Id+"，学生姓名为:"+finda.StuName+ "的冲突数据成功修改为正常数据！" };
                    s_Entity.log_s.Add_data(log);
                }
                else
                {
                    result.Msg = "有相同的数据，不能修改！";
                    result.Success = false;
                }                 
            }
            catch (Exception)
            {

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}