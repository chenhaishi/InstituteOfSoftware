using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Business.StuSatae_Maneger;
using SiliconValley.InformationSystem.Business.Psychro;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.RegionManage;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using System.Data.SqlClient;
using System.Data;
using SiliconValley.InformationSystem.Business.StuInfomationType_Maneger;
using SiliconValley.InformationSystem.Business.DepartmentBusiness;
using SiliconValley.InformationSystem.Business.PositionBusiness;
using System.Text.RegularExpressions;
using SiliconValley.InformationSystem.Business.NetClientRecordBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
using SiliconValley.InformationSystem.Business.StudentmanagementBusinsess;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.FinaceBusines;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;

namespace SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness
{
    public class StudentDataKeepAndRecordBusiness : BaseBusiness<StudentPutOnRecord>
    {
        #region 创建业务类

        SchoolYearPlanBusiness Syb_Entity = new SchoolYearPlanBusiness();
        /// <summary>
        /// 创建一个用于查询数据的员工信息实体
        /// </summary>
        public EmployeesInfoManage Enplo_Entity = new EmployeesInfoManage();

        /// <summary>
        /// 创建一个用于查询区域的实体
        /// </summary>
        public RegionManeges region_Entity = new RegionManeges();

        /// <summary>
        /// 备案状态实体
        /// </summary>
        public StuStateManeger Stustate_Entity = new StuStateManeger();

        /// <summary>
        /// 创建一个用于查询数据的上门学生信息来源实体 
        /// </summary>
        public StuInfomationTypeManeger StuInfomationType_Entity = new StuInfomationTypeManeger();

        /// <summary>
        /// 创建一个用于查询数据的部门信息实体
        /// </summary>
        public DepartmentManage Department_Entity = new DepartmentManage();

        /// <summary>
        /// 创建一个用于查询岗位信息实体
        /// </summary>
        public PositionManage Position_Entity = new PositionManage();

        public Sch_MarketManeger s_entity = new Sch_MarketManeger();

        /// <summary>
        /// 用于查询黑户数据
        /// </summary>
        public HeiHuManeger heiHu = new HeiHuManeger();

        /// <summary>
        /// 用于添加网咨回访数据
        /// </summary>
        public NetClientRecordManage NetClient_Entity = new NetClientRecordManage();

        /// <summary>
        /// 用户获取区域市场负责人
        /// </summary>
        public ChannelAreaBusiness Channerl_Entity = new ChannelAreaBusiness();

        /// <summary>
        /// 文件操作业务类
        /// </summary>
        public CloudstorageBusiness MyFiles = new CloudstorageBusiness();

        /// <summary>
        /// 获取账号业务
        /// </summary>
        public Base_UserBusiness B_USER = new Base_UserBusiness();

        /// <summary>
        /// 获取备案日志业务类
        /// </summary>
        public StudentbeanLogManeger log_s = new StudentbeanLogManeger();

        /// <summary>
        /// 数据作废记录业务类
        /// </summary>
        public Delte_StudentPutinfoManeger deletdata = new Delte_StudentPutinfoManeger();

        /// <summary>
        /// 获取指点日期的报名、预录费用
        /// </summary>
        public CostitemsBusiness Costitems = new CostitemsBusiness();

        /// <summary>
        /// 阶段业务类
        /// </summary>
        public GrandBusiness grand = new GrandBusiness();
        #endregion


        /// <summary>
        /// 获取登录的岗位3--网络咨询师，4--咨询助理，0--咨询主任，2--网络主任0--校办--(-1其他)
        /// </summary>
        /// <param name="emp"> </param>
        /// <returns></returns>
        public int GetPostion(string emp)
        {
            int Key = -1;
            //根据Emp获取岗位
            int id = Enplo_Entity.GetEntity(emp).PositionId;
            Position find_p = Position_Entity.GetEntity(id);
            if (find_p.PositionName == "网络咨询师")
            {
                Key = 3;
            }
            else if (find_p.PositionName == "咨询助理")
            {
                Key = 4;
            }
            else if (find_p.PositionName == "咨询主任")
            {
                Key = 0;
            }
            else if (find_p.PositionName == "网络主任")
            {
                Key = 2;
            }
            else
            {
                Department find_b = Department_Entity.GetEntity(find_p.DeptId);
                if (find_b.DeptName == "校办")
                {
                    Key = 0;
                }
            }
            return Key;
        }

        /// <summary>
        /// 获取员工 
        /// </summary>
        /// <param name="IsIncumbency">(true--获取在职员工,false--获取所有员工)</param>
        /// <returns></returns>
        public List<EmployeesInfo> GetEffectiveEmpAll(bool IsIncumbency)
        {
            if (IsIncumbency)
            {
                return Enplo_Entity.GetListBySql<EmployeesInfo>(" select * from EmpView  where IsDel=0");
            }
            else
            {
                //获取所有员工
                return Enplo_Entity.GetListBySql<EmployeesInfo>("select * from EmpView"); ;
            }
        }

        /// <summary>
        /// 根据部门获取部门下所有的员工
        /// </summary>
        /// <param name="did"></param>
        /// <returns></returns>
        public List<EmployeesInfo> GetDepartmentPeople(int did)
        {
            return Enplo_Entity.GetEmpsByDeptid(did);
        }

        /// <summary>
        /// 获取所有区域 
        /// </summary>
        /// <param name="IsIncumbercy">（true--获取有效区域,false--获取所有区域）</param>
        /// <returns></returns>
        public List<Region> GetEffectiveRegionAll(bool IsIncumbercy)
        {
            region_Entity = new RegionManeges();
            if (IsIncumbercy)
            {
                //获取有效的区域
                return region_Entity.GetList().Where(r => r.IsDel == false).ToList();
            }
            else
            {
                //获取所有区域
                return region_Entity.GetList();
            }
        }

        #region 对数据的操作

        #region  查询数据

        /// <summary>
        /// 获取备案的所有数据
        /// </summary>
        /// <returns></returns>
        public List<ExportStudentBeanData> GetAll()
        {
            List<ExportStudentBeanData> listall = this.GetListBySql<ExportStudentBeanData>("select * from StudentBeanView where IsDelete=0");

            List<Sch_MarketView> list2 = this.GetListBySql<Sch_MarketView>("select * from Sch_MarketView where IsDel=0");

            listall.AddRange(LongrageDataToViewmodel(list2));

            return listall;
        }

        /// <summary>
        /// 获取XX条后面的数据
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public List<ExportStudentBeanData> GetAllTopNumber(int number)
        {
            List<ExportStudentBeanData> listall = this.GetListBySql<ExportStudentBeanData>("select * from StudentBeanView where Id>="+number+ " and IsDelete=0 order by BeanDate desc ");
            return listall;
        }

        /// <summary>
        /// 获取某一年的已报名备案数据
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns></returns>
        public List<ExportStudentBeanData> GetEffectiveData(int year)
        {
            List<ExportStudentBeanData> list = new List<ExportStudentBeanData>();
            List<ExportStudentBeanData> All = WhereDateYear(year);
            foreach (ExportStudentBeanData item in All)
            {
                if (item.StatusTime != null)
                {
                    if (Convert.ToDateTime(item.StatusTime).Year == year)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取StudentPutOnRecord表的所有数据
        /// </summary>
        /// <returns></returns>
        public List<ExportStudentBeanData> GetSudentDataAll()
        {
            List<ExportStudentBeanData> listall = this.GetListBySql<ExportStudentBeanData>("select * from StudentBeanView where IsDelete=0");

            return listall;
        }

        /// <summary>
        /// 获取页面总条数
        /// </summary>
        /// <returns></returns>
        public int SumCount()
        {

            SqlConnection con = new SqlConnection("server=106.13.104.179;database=Coldairarrow.Fx.Net.Easyui.GitHub;uid=sa;pwd=tangdan2020@;Max Pool Size = 512;");
            con.Open();
            SqlCommand cmd = new SqlCommand("select count(*) as count from StudentBeanView where IsDelete=0", con);
            SqlCommand cmd2 = new SqlCommand("select count(*) as count from Sch_MarketView where IsDel=0", con);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            count = count + Convert.ToInt32(cmd2.ExecuteScalar());
            con.Close();
            return count;
        }

        /// <summary>
        /// 获取有相同学生姓名备案数据
        /// </summary>
        /// <param name="stuName"></param>
        /// <returns></returns>
        public List<ExportStudentBeanData> StudentOrride(string stuName)
        {
            if (!string.IsNullOrEmpty(stuName))
            {
                stuName = stuName.Trim();
                List<ExportStudentBeanData> listall = this.GetListBySql<ExportStudentBeanData>("select * from StudentBeanView where StuName='" + stuName + "' and IsDelete=0");

                List<Sch_MarketView> listal2 = this.GetListBySql<Sch_MarketView>("select * from Sch_MarketView where StudentName='" + stuName + "' and IsDel=0");


                listall.AddRange(this.LongrageDataToViewmodel(listal2));

                return listall;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 获取已备案的疑似数据
        /// </summary>
        /// <param name="stuName"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public ExportStudentBeanData StudentOrrideData(string stuName, string phone,string Weixi,string qq)
        {
            stuName = stuName.Trim();
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            sb1.Append("select * from StudentBeanView where StuName='" + stuName + "' and IsDelete=0");
            sb2.Append("select * from Sch_MarketView where StudentName='" + stuName + "' and IsDel=0");
            if (phone != null)
            {
                sb1.Append(" and Stuphone='" + phone + "'");
                sb2.Append(" and Phone='" + phone + "'");
            }
            else if(Weixi != null && qq == null)
            {
                sb1.Append(" and StuWeiXin='" + Weixi + "'");
                sb2.Append(" and QQ='" + qq + "'");
            }else if (Weixi == null && qq != null)
            {
                sb1.Append(" and StuQQ='" + qq + "'");
                sb2.Append(" and QQ='" + qq + "'");
            }

             
            List<ExportStudentBeanData> listall = this.GetListBySql<ExportStudentBeanData>(sb1.ToString());
            List<Sch_MarketView> listal2 = this.GetListBySql<Sch_MarketView>(sb2.ToString());

            listall.AddRange(this.LongrageDataToViewmodel(listal2));

            return listall.Count > 0 ? listall[0] : null;
        }
        /// <summary>
        /// 获取已备案的数据
        /// </summary>
        /// <param name="stuName"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public StudentPutOnRecord StudentOrreideData_OnRecord(string stuName, string phone, DateTime date)
        {
            StringBuilder sb = new StringBuilder();
            string yy = date.Year + "-" + date.Month + "-" + date.Day;
            sb.Append("select top 1 * from studentPutOnRecord where StuName='" + stuName + "' and BeanDate='" + yy + "'  and IsDelete=0");


            if (phone != null)
            {
                sb.Append(" and StuPhone='" + phone + "'");
            }
            sb.Append(" order by  Id desc");

            string bb = sb.ToString();
            List<StudentPutOnRecord> listall = this.GetListBySql<StudentPutOnRecord>(sb.ToString());

            return listall.Count > 0 ? listall[0] : null;
        }
        /// <summary>
        /// 模糊查询
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public List<ExportStudentBeanData> Serch(string str1, string str2)
        {
            List<ExportStudentBeanData> listall = this.GetListBySql<ExportStudentBeanData>(str1);
            List<Sch_MarketView> old = this.GetListBySql<Sch_MarketView>(str2);
            listall.AddRange(this.LongrageDataToViewmodel(old));
            return listall;
        }

        /// <summary>
        /// 通过sql语句返回对应的视图数据（true--返回2020年之后的，false--返回2017~2019的备案视图集合）
        /// </summary>
        /// <param name="str1"></param>
        /// <param name="isStudent"></param>
        /// <returns></returns>
        public List<ExportStudentBeanData> Serch(string str1, bool isStudent)
        {
            if (isStudent)
            {
                return this.GetListBySql<ExportStudentBeanData>(str1);
            }
            else
            {
                List<Sch_MarketView> old = this.GetListBySql<Sch_MarketView>(str1);
                return this.LongrageDataToViewmodel(old);
            }

        }
        /// <summary>
        /// 根据Id获取备案视图数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExportStudentBeanData findId(string id)
        {
            List<ExportStudentBeanData> listall = this.GetListBySql<ExportStudentBeanData>("select * from StudentBeanView where Id='" + id + "'");

            if (listall.Count == 0)
            {
                List<Sch_MarketView> old = this.GetListBySql<Sch_MarketView>("select * from Sch_MarketView where Id='" + id + "'");
                listall = LongrageDataToViewmodel(old);
                return listall.Count <= 0 ? null : listall[0];
            }
            else
            {
                return listall[0];
            }
        }

        /// <summary>
        /// 根据Id获取StudentPutOnRecord表的备案数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public StudentPutOnRecord whereStudentId(int id)
        {
            return this.GetEntity(id);
        }

        public ExportStudentBeanData whereStudentId(string id)
        {
            int ids = Convert.ToInt32(id);

            List<ExportStudentBeanData> data = this.GetListBySql<ExportStudentBeanData>("select * from StudentBeanView where Id='" + id + "'");
            return data[0];
        }
        /// <summary>
        /// 根据Id获取Sch_Market表中的备案数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Sch_Market whereMarketId(int id)
        {
            return s_entity.GetEntity(id);
        }

        /// <summary>
        /// 获去某个年份的数据
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public List<ExportStudentBeanData> WhereDateYear(int year)
        {
            List<ExportStudentBeanData> data = new List<ExportStudentBeanData>();
            if (year == 2017 || year == 2018 || year == 2019)
            {
                switch (year)
                {
                    case 2017:
                        string str = "select *from Sch_MarketView where CreateDate>'2016-12-31' and CreateDate<'2018-01-01'";
                        List<Sch_MarketView> list = s_entity.GetListBySql<Sch_MarketView>(str);
                        data = LongrageDataToViewmodel(list);
                        break;
                    case 2018:
                        string str2 = "select *from Sch_MarketView where CreateDate>'2017-12-31' and CreateDate<'2019-01-01'";
                        List<Sch_MarketView> list2 = s_entity.GetListBySql<Sch_MarketView>(str2);
                        data = LongrageDataToViewmodel(list2);
                        break;
                    case 2019:
                        string str3 = "select *from Sch_MarketView where CreateDate>'2019-12-31' and CreateDate<'2020-01-01'";
                        List<Sch_MarketView> list3 = s_entity.GetListBySql<Sch_MarketView>(str3);
                        data = LongrageDataToViewmodel(list3);
                        break;
                }
            }
            else
            {
                string starYear = year + "-01-01";
                string endYear = year + "-12-31";

                string str = "select * from StudentBeanView where BeanDate>='" + starYear + "' and BeanDate<='" + endYear + "'";

                data = this.GetListBySql<ExportStudentBeanData>(str);
            }

            return data;
        }

        #endregion

        #region 增、改数据

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="new_s"></param>
        /// <returns></returns>
        public AjaxResult Add_data(StudentPutOnRecord new_s)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Insert(new_s);
                //redisCache.RemoveCache("StudentKeepList");
                a.Success = true;
                a.Msg = "备案成功";
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = ex.Message;
            }

            return a;
        }

        public AjaxResult Add_data(List<StudentPutOnRecord> new_s)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Insert(new_s);
                //redisCache.RemoveCache("StudentKeepList");
                a.Success = true;
                a.Msg = "备案成功";
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = ex.Message;
            }

            return a;
        }

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="olds"></param>
        /// <returns></returns>
        public AjaxResult Update_data(StudentPutOnRecord olds)
        {
            StuStateManeger Stustate_Entity = new StuStateManeger();
            AjaxResult a = new AjaxResult();
            try
            {
                if (olds.Id >= 54118)
                {
                    StudentPutOnRecord fins = this.whereStudentId(olds.Id);
                    //找到要修改的实体
                    //if (fins.StuStatus_Id!= 1012)
                    //{
                    //    fins.StuName = olds.StuName;
                    //} 

                    //fins.EmployeesInfo_Id = olds.EmployeesInfo_Id;
                    if (fins.StuPhone == null )
                    {
                        //判断是否有相同的电话号码
                        if (olds.StuPhone!=null)
                        {
                            ExportStudentBeanData findata = StudentOrrideData(olds.StuName, olds.StuPhone, null, null);
                            if (findata != null)
                            {
                                a.Success = false;
                                a.Msg = "已有相同学生，相同电话号码的数据，所以不能修改！！";

                                return a;
                            }
                        }
                         
                        fins.StuPhone = olds.StuPhone;
                    }

                    if (fins.StuSex == "0" || fins.StuSex ==null)
                    {
                        if (olds.StuSex=="0")
                        {
                            fins.StuSex = null;
                        }
                        else
                        {
                            fins.StuSex = olds.StuSex;
                        }
                         
                    }

                    if (fins.StuBirthy == null)
                    {
                        fins.StuBirthy = olds.StuBirthy;
                    }

                    if (fins.StuSchoolName == null)
                    {
                        fins.StuSchoolName = olds.StuSchoolName;
                    }

                    if (fins.StuEducational == null || fins.StuEducational == "0" || fins.StuEducational == "其他")
                    {
                        fins.StuEducational = olds.StuEducational=="0"?null: olds.StuEducational;
                    }

                    if (fins.StuAddress == null)
                    {
                        fins.StuAddress = olds.StuAddress;
                    }

                    if (fins.StuWeiXin == null)
                    {
                        if (olds.StuWeiXin!=null)
                        {
                            ExportStudentBeanData findata = StudentOrrideData(olds.StuName, null, olds.StuWeiXin, null);
                            if (findata != null)
                            {
                                a.Success = false;
                                a.Msg = "已有相同学生，相同微信的数据，所以不能修改！！";

                                return a;
                            }
                        }
                         
                        fins.StuWeiXin = olds.StuWeiXin;
                    }

                    if (fins.StuQQ == null)
                    {
                        if (olds.StuQQ!=null)
                        {
                            ExportStudentBeanData findata = StudentOrrideData(olds.StuName, null, null, olds.StuQQ);
                            if (findata != null)
                            {
                                a.Success = false;
                                a.Msg = "已有相同学生，相同QQ的数据，所以不能修改！！";

                                return a;
                            }
                        }
                         
                        fins.StuQQ = olds.StuQQ;
                    }

                    fins.StuIsGoto = olds.StuIsGoto;
                    fins.StuVisit = olds.StuVisit;

                    if (fins.StuInfomationType_Id == null)
                    {
                        fins.StuInfomationType_Id = olds.StuInfomationType_Id;
                    }

                    if (fins.Party == null ||string.IsNullOrEmpty( fins.Party))
                    {
                        fins.Party = olds.Party;
                    }

                    if (fins.Region_id == null)
                    {
                        fins.Region_id = olds.Region_id;
                    }

                   
                    fins.Reak = olds.Reak;
                    this.Update(fins);

                    //redisCache.RemoveCache("StudentKeepList");
                    a.Success = true;
                    a.Msg = "修改成功！";
                }
                 

            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;
            }
            return a;
        }

        public AjaxResult Update_data(List<StudentPutOnRecord> olds)
        {
            AjaxResult a = new AjaxResult();

            try
            {
                this.Update(olds);
                a.Success = true;
            }
            catch (Exception)
            {
                a.Success = false;
            }

            return a;
        }

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="olds"></param>
        /// <returns></returns>
        public bool My_update(StudentPutOnRecord olds)
        {
            bool s = true;
            try
            {
                this.Update(olds);
            }
            catch (Exception ex)
            {
                s = false;

            }

            return s;
        }

        /// <summary>
        /// 将未上门学生改为已上门
        /// </summary>
        /// <param name="data"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public AjaxResult UpdateGotoShcool(List<StudentPutOnRecord> data, DateTime date)
        {
            AjaxResult a = new AjaxResult();
            List<StudentPutOnRecord> newdata = new List<StudentPutOnRecord>();
            foreach (StudentPutOnRecord item in data)
            {
                item.StuIsGoto = true;
                item.StuVisit = date;
                newdata.Add(item);
            }

            try
            {
                this.Update(newdata);
                a.Success = true;
            }
            catch (Exception)
            {
                a.Success = false;
            }

            return a;
        }

        /// <summary>
        /// 指派咨询师
        /// </summary>
        /// <param name="olds"></param>
        /// <returns></returns>
        public bool ZhipaiConsultTeacher(StudentPutOnRecord olds)
        {

            bool s = true;
            try
            {
                this.Update(olds);
            }
            catch (Exception)
            {
                s = false;
            }

            return s;
        }

        #endregion
        #endregion

        #region 给市场用的方法
        /// <summary>
        /// 这是一个获取报名学生的方法
        /// </summary>
        /// <param name="EmpyId">员工编号</param>
        /// <returns></returns>
        public List<StudentPutOnRecord> GetrReport(string EmpyId)
        {    //获取报名id

            int id = Stustate_Entity.GetList().Where(s => s.StatusName == "已报名").FirstOrDefault().Id;
            //根据员工获取报名的数据
            return this.GetList().Where(s => s.StuStatus_Id == id && s.EmployeesInfo_Id == EmpyId).ToList();
        }
        /// <summary>
        /// 获取某一年的每个月的上门量
        /// </summary>
        /// <param name="YeanName"></param>
        /// <returns></returns>
        public int[] GetYearGotoCount(DateTime YeanName)
        {
            List<StudentPutOnRecord> student_data = this.GetList().Where(s => s.StuIsGoto == true && s.StuVisit <= YeanName).ToList();//获取匹配的数据
            int j = 1;                                                                                                               //拿到一月到12的人数            
            int[] ary = new int[12];
            for (int i = 0; i < 12; i++)
            {
                ary[i] = GetMonthCount(student_data, j);
                j++;
            }
            return ary;
        }

        public int GetMonthCount(List<StudentPutOnRecord> student_list, int monthName)
        {
            var count = student_list.Select(s => Convert.ToDateTime(s.StuVisit).Month).Where(s => s == monthName).ToList().Count;
            return count;
        }
        /// <summary>
        /// 获取上门量
        /// </summary>
        /// <param name="PlanId"></param>
        /// <param name="EmpId"></param>
        /// <returns></returns>
        public List<StudentPutOnRecord> GetGoSchoolByPlan(int? PlanId, string EmpId)
        {
            //当前查询的计划
            var nowplan = Syb_Entity.GetPlanByID(PlanId);
            //拿到下一个计划
            var nextplan = Syb_Entity.GetNextPlan(nowplan);

            List<StudentPutOnRecord> list_s = this.GetList().Where(s => s.EmployeesInfo_Id == EmpId && s.StuIsGoto == true).ToList();
            List<StudentPutOnRecord> resultlist = new List<StudentPutOnRecord>();
            foreach (var item in list_s)
            {
                if (item.StuVisit >= nowplan.PlanDate)
                {
                    if (nextplan.ID != 0)
                    {
                        if (item.StuVisit <= nextplan.PlanDate)
                        {
                            resultlist.Add(item);
                        }
                    }
                    else
                    {
                        resultlist.Add(item);
                    }
                }
            }
            return resultlist;
        }
        /// <summary>
        /// 获取备案量
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="PlanId"></param>
        /// <returns></returns>
        public List<StudentPutOnRecord> GetBeanCount(string EmpId, int? PlanId)
        {
            //当前查询的计划
            var nowplan = Syb_Entity.GetPlanByID(PlanId);
            //拿到下一个计划
            var nextplan = Syb_Entity.GetNextPlan(nowplan);

            List<StudentPutOnRecord> list_s = this.GetList().Where(s => s.EmployeesInfo_Id == EmpId).ToList();
            List<StudentPutOnRecord> resultlist = new List<StudentPutOnRecord>();
            foreach (var item in list_s)
            {
                if (item.StuDateTime >= nowplan.PlanDate)
                {
                    if (nextplan.ID != 0)
                    {
                        if (item.StuDateTime <= nextplan.PlanDate)
                        {
                            resultlist.Add(item);
                        }
                    }
                    else
                    {
                        resultlist.Add(item);
                    }
                }
            }
            return resultlist;
        }
        /// <summary>
        /// 获取报名量
        /// </summary>
        /// <param name="EmpId"></param>
        /// <param name="PlanId"></param>
        /// <returns></returns>
        public List<StudentPutOnRecord> GetBaoMingCount(string EmpId, int? PlanId)
        {
            //当前查询的计划
            var nowplan = Syb_Entity.GetPlanByID(PlanId);
            //拿到下一个计划
            var nextplan = Syb_Entity.GetNextPlan(nowplan);
            //获取报名id

            int id = Stustate_Entity.GetList().Where(s => s.StatusName == "已报名").FirstOrDefault().Id;
            List<StudentPutOnRecord> list_s = this.GetList().Where(s => s.EmployeesInfo_Id == EmpId && s.StuStatus_Id == id).ToList();
            List<StudentPutOnRecord> resultlist = new List<StudentPutOnRecord>();
            foreach (var item in list_s)
            {
                if (item.StatusTime >= nowplan.PlanDate)
                {
                    if (nextplan.ID != 0)
                    {
                        if (item.StatusTime <= nextplan.PlanDate)
                        {
                            resultlist.Add(item);
                        }
                    }
                    else
                    {
                        resultlist.Add(item);
                    }
                }
            }
            return resultlist;
        }
        #endregion

        #region 给财务的方法

        /// <summary>
        /// 这个方法是就修改学生状态的方法
        /// </summary>
        /// <param name="id">备案id</param>
        /// <returns></returns>
        public bool ChangeStudentState(int id)
        {
            if (id >= 54118)
            {
                StudentPutOnRecord find_s = this.GetEntity(id);
                if (find_s != null)
                {
                    AjaxResult a = Stustate_Entity.GetStu("已报名");
                    StuStatus find_statu = a.Data as StuStatus;

                    if (find_s.StuStatus_Id != find_statu.Id)
                    {
                        if (a.Success == true)
                        {

                            find_s.StuStatus_Id = find_statu.Id;
                            find_s.StatusTime = DateTime.Now;
                            this.Update(find_s);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }

                }
                else
                {
                    return false;
                }
            }
            else
            {
                Sch_Market find_s = s_entity.GetEntity(id);
                if (find_s != null)
                {
                    find_s.MarketState = "已报名";

                    return s_entity.MyUpdate(find_s).Success;
                }
                else
                {
                    return false;
                }
            }

        }
        #endregion

        #region 用于将远程数据库中的备案数据导入当前数据库中

        /// <summary>
        /// 获取远程备案数据
        /// </summary>
        /// <returns></returns>
        public List<ADDdataview> GetLongrageData(string str)
        {
            //ADDdataview/TM_ConsultView/TM_FlowwView
            List<ADDdataview> list = new List<ADDdataview>();
            string sql = "server=121.43.166.117;database=GUIGU;uid=sa;pwd=Guigu20202020;Max Pool Size = 512";
            SqlConnection con = new SqlConnection(sql);
            con.Open();
            SqlCommand comm = new SqlCommand(str, con);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(comm);
            da.Fill(ds);
            DataTable mm = ds.Tables[0];
            list = mm.ToList<ADDdataview>();
            con.Close();
            return list;
        }

        public void InServer()
        {
            #region
            Sch_MarketManeger marketEntity = new Sch_MarketManeger();
            string str = "select StudentName,Sex,CreateUserName,CreateDate,Phone,QQ,School,Education,Inquiry,Source,Area,SalePerson,RelatedPerson,Remark,MarketState,MarketType,Info from Sch_Market  where  CreateDate>='2019-01-01' and CreateDate<='2019-12-30' and Source='网络'";
            List<ADDdataview> all = GetLongrageData(str);
            List<StudentPutOnRecord> studentlist = new List<StudentPutOnRecord>();

            foreach (ADDdataview item in all)
            {
                StudentPutOnRecord one = new StudentPutOnRecord();
                one.Reak = item.Remark;
                one.StuName = item.StudentName;
                one.StuSex = string.IsNullOrEmpty(item.Sex) ? "男" : item.Sex;
                one.EmployeesInfo_Id = Enplo_Entity.FindEmpData(item.SalePerson, false)?.EmployeeId ?? null;
                var mm = Enplo_Entity.FindEmpData(item.SalePerson, false);
                if (string.IsNullOrEmpty(item.SalePerson) || item.SalePerson == "其他")
                {

                }
                else
                {
                    if (Enplo_Entity.FindEmpData(item.SalePerson, false).EmpName == null)
                    {
                        one.Reak = one.Reak + ".由于该备案人已离职,员工数据丢失,所以在此标注真实备案人为:" + item.SalePerson + ",";
                    }
                }

                one.StuDateTime = item.CreateDate;
                string date = item.CreateDate.Year + "-" + item.CreateDate.Month + "-" + item.CreateDate.Day;
                one.BeanDate = Convert.ToDateTime(date);

                one.StuPhone = item.Phone;
                one.IsDelete = false;
                one.StuQQ = item.QQ;
                one.StuSchoolName = item.School;
                one.StuEducational = item.Education;
                one.StuStatus_Id = 1013;//默认未报名
                one.StuIsGoto = item.MarketState == "已上门" ? true : false;
                if (item.MarketState == "已上门")
                {
                    one.StuVisit = Convert.ToDateTime(date);
                }
                else if (item.MarketState == "已报名交清" || item.MarketState == "已报名未交清")
                {
                    one.StuStatus_Id = 1012;
                }
                one.StuInfomationType_Id = StuInfomationType_Entity.SerchSingleData(item.Source, false) == null ? StuInfomationType_Entity.SerchSingleData("渠道", false).Id : StuInfomationType_Entity.SerchSingleData(item.Source, false).Id;
                one.Region_id = region_Entity.SerchRegionName(item.Area, false)?.ID ?? null;
                one.Party = item.RelatedPerson;
                one.StuEntering = Enplo_Entity.FindEmpData(item.CreateUserName, false)?.EmpName ?? null;
                if (item.Inquiry != null)
                {
                    one.ConsultTeacher = item.Inquiry;
                    if (Enplo_Entity.FindEmpData(item.Inquiry, false) == null)
                    {
                        one.ConsultTeacher = null;
                        one.Reak = one.Reak + "之前咨询师是:" + item.Inquiry + "但是离职了。";
                    }

                }

                try
                {
                    this.Insert(one);
                }
                catch (Exception ex)
                {

                    string str1 = ex.Message;
                }
            }


            #endregion
        }

        /// <summary>
        /// 将远程数据转成本地视图数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<ExportStudentBeanData> LongrageDataToViewmodel(List<Sch_MarketView> list)
        {
            List<ExportStudentBeanData> newlist = list.Select(s2 => new ExportStudentBeanData
            {
                Id = s2.Id,
                StuName = s2.StudentName,
                StuSex = s2.Sex,
                StuBirthy = null,
                Stuphone = s2.Phone,
                StuSchoolName = s2.School,
                StuEducational = s2.Education,
                StuAddress = null,
                StuWeiXin = null,
                StuQQ = s2.QQ,
                stuinfomation = s2.source,
                StatusName = s2.MarketState,
                StuisGoto = false,
                StuVisit = null,
                empName = s2.SalePerson,
                Party = s2.RelatedPerson,
                BeanDate = s2.CreateDate,
                StuDateTime = s2.CreateDate,
                StuEntering = s2.CreateUserName,
                StatusTime = null,
                RegionName = s2.Area,
                Reak = s2.Info + "," + s2.Remark,
                ConsultTeacher = s2.Inquiry
            }).ToList();

            return newlist;
        }

        #endregion


        /// <summary>
        ///  生成黑户身份证
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public AjaxResult GetIdCard(string date)
        {
            AjaxResult a = new AjaxResult();
            string one = "404";

            string two = "000";

            string three = "0001";

            int count = heiHu.Count();
            if (count == 0)
            {
                a.Success = true;
                a.Data = one + two + date + three;
                return a;
            }
            else
            {
                HeiHu find = heiHu.LastData();
                int number1 = Convert.ToInt32(find.IdCard.Substring(14, 4));
                int number2 = Convert.ToInt32(find.IdCard.Substring(3, 3));
                if (number1 < 9999)
                {
                    number1++;
                    if (number1.ToString().Length == 1)
                    {

                        three = "000" + number1;
                    }
                    else if (number1.ToString().Length == 2)
                    {
                        three = "00" + number1;
                    }
                    else if (number1.ToString().Length == 3)
                    {
                        three = "0" + number1;
                    }
                    else if (number1.ToString().Length == 4)
                    {
                        three = number1.ToString();
                    }
                }
                else if (number1 == 9999)
                {
                    if (number2 == 999)
                    {
                        a.Success = false;
                        a.Msg = "黑户账号已满！请联系管理员重新设计黑户规则";

                        return a;
                    }
                    else
                    {
                        number2++;
                        if (number2.ToString().Length == 1)
                        {
                            two = "00" + number2;
                        }
                        else if (number2.ToString().Length == 2)
                        {
                            two = "0" + number2;
                        }
                        else if (number2.ToString().Length == 3 && number2 < 999)
                        {
                            two = number2.ToString();
                        }
                    }

                }

                a.Success = true;
                a.Data = one + two + date + three;
            }

            return a;
        }

        /// <summary>
        /// 根据备案查询学生班级数据
        /// </summary>
        /// <param name="Sid"></param>
        /// <returns></returns>
        public PutStudentDataView FindStudentData(int Sid)
        {
            List<PutStudentDataView> list = this.GetListBySql<PutStudentDataView>(" select * from StudentInfoView where id=" + Sid);
            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据班级获取任课老师
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        public EmployeesInfo GetTeacher(int classid)
        {
            EmployeesInfo info = new EmployeesInfo();
            List<ClassTeacher> find = this.GetListBySql<ClassTeacher>("select * from ClassTeacher where EndDate is null and ClassNumber=" + classid);
            if (find.Count > 0)
            {
                List<EmployeesInfo> list = this.GetListBySql<EmployeesInfo>(" select * from EmployeesInfo as e left join  Teacher as t on t.EmployeeId = e.EmployeeId where t.TeacherId =" + find[0].TeacherID);

                if (list.Count > 0)
                {
                    info = list[0];
                }
            }

            return info;
        }
        /// <summary>
        /// 判断身份证
        /// </summary>
        /// <param name="crad"></param>
        /// <returns></returns>
        public bool TrueCrad(string crad)
        {
            string reg1 = "^[1-9]\\d{5}(18|19|([23]\\d))\\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\\d{3}[0-9Xx]$";//十八位
            string reg2 = "^[1-9]\\d{5}\\d{2}((0[1-9])|(10|11|12))(([0-2][1-9])|10|20|30|31)\\d{2}[0-9Xx]$";//十五位

            bool s1 = Regex.IsMatch(crad, reg1);
            bool s2 = Regex.IsMatch(crad, reg2);

            if (!s1 && !s2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取已缴预录费的身份证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetylfCord(int id)
        {
            StudentFeeStandardBusinsess student = new StudentFeeStandardBusinsess();
            return student.Identityid(id);

        }

        /// <summary>
        /// 数据作废
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Shenhe_changeDele(int id)
        {
            bool s = true;
            if (id>=54118)
            {
                StudentPutOnRecord entity= this.GetEntity(id);
                entity.IsDelete = true;
                try
                {
                    this.Update(entity);
                }
                catch (Exception)
                {

                    s = false;
                }
            }
            else
            {
               Sch_Market sch= s_entity.GetEntity(id);
                sch.IsDel = true;
                try
                {
                    s_entity.Update(sch);
                }
                catch (Exception)
                {

                    s = false;
                }
                
            }

            return s;
        }

        
    }
}
