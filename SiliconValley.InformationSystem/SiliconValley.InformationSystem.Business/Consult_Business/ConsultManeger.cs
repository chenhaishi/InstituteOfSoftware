using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness;
using SiliconValley.InformationSystem.Business.RegionManage;
using SiliconValley.InformationSystem.Business.StuInfomationType_Maneger;
using SiliconValley.InformationSystem.Business.StuSatae_Maneger;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.Consult_Business
{
    public class ConsultManeger : BaseBusiness<Consult>
    {
        EmployeesInfoManage emp_Entity = new EmployeesInfoManage();
        ConsultTeacherManeger Ct_Entity = new ConsultTeacherManeger();
       public FollwingInfoManeger Fi_Entity = new FollwingInfoManeger();
       public  StudentDataKeepAndRecordBusiness Stu_Entity = new StudentDataKeepAndRecordBusiness();
        RegionManeges RM_Entity = new RegionManeges();
        StuInfomationTypeManeger ST_Entity = new StuInfomationTypeManeger();
        StuStateManeger SSta_Entity = new StuStateManeger();
        /// <summary>
        /// 根据主键或名称查询类型
        /// </summary>
        /// <param name="idorname">Id或名字</param>
        /// <param name="IsKey">是否为主键</param>
        /// <returns></returns>
        public StuInfomationType getTypeName(string idorname, bool IsKey)
        {
            if (IsKey)
            {
                int id = Convert.ToInt32(idorname);
                StuInfomationType type = ST_Entity.GetEntity(id);
                if (type != null)
                {
                    return type;
                }
                else
                {
                    StuInfomationType t = new StuInfomationType();
                    t.Name = "区域外";
                    t.Id = -1;
                    return t;
                }
            }
            else
            {
                StuInfomationType type = ST_Entity.GetList().Where(t => t.Name == idorname).FirstOrDefault();
                if (type != null)
                {
                    return type;
                }
                else
                {
                    StuInfomationType t = new StuInfomationType();
                    t.Name = "区域外";
                    t.Id = -1;
                    return t;
                }
            }
             
        }
        //获取某个月份所有备案数据
        public List<ExportStudentBeanData> GetMonStudent(int monName)
        {
            List<ExportStudentBeanData> All_stu= Stu_Entity.WhereDateYear((DateTime.Now).Year).Where(s => Convert.ToDateTime(s.BeanDate).Month == monName).ToList();//获取某个月份备案的所有数据
            List<Consult> All_con = this.GetList();//获取所有分量数据
            List<ExportStudentBeanData> result = new List<ExportStudentBeanData>();

            if (All_stu.Count > 0)
            {
                //去分量的地方筛选没有被分量的学生
                for (int i = 0; i < All_stu.Count; i++)
                {
                    int count = All_con.Where(c => c.StuName == All_stu[i].Id).ToList().Count;
                    if (count<=0)
                    {
                        result.Add(All_stu[i]);
                    }
                }
            }
            return All_stu;
        }
        /// <summary>
        /// 获取区域名称
        /// </summary>
        /// <param name="regionId">区域Id</param>
        /// <returns></returns>
        public Region GetRegionName(int? regionId)
        {
            Region r = RM_Entity.GetEntity(regionId);
            if (r==null)
            {
                Region r1 = new Region();
                r1.RegionName = "区域外";
                r1.ID = -1;
                return r1;
            }
             
           return r ;
        }
        /// <summary>
        /// 获取咨询次数
        /// </summary>
        /// <param name="consult_id"></param>
        /// <returns></returns>
        public int GetFollwingCount(int consult_id)
        {
           return  Fi_Entity.GetList().Where(f => f.Consult_Id == consult_id).ToList().Count;
        }
        /// <summary>
        /// 根据咨询师Id找咨询师
        /// </summary>
        /// <param name="id">咨询师</param>
        /// <returns></returns>
        public ConsultTeacher GetSingleFollwingData(int? id)
        {
           return Ct_Entity.GetEntity(id);
        }
        /// <summary>
        /// 根据学生Id找分量
        /// </summary>
        /// <param name="id">学生Id</param>
        /// <returns></returns>
        public Consult FindStudentIdGetConultdata(int id)
        {
           return this.GetList().Where(c => c.StuName == id).FirstOrDefault();
        }
        /// <summary>
        /// 根据分量Id或g根据咨询Id查询分量数据
        /// </summary>
        /// <param name="key">属性</param>
        /// <param name="value">值</param>
        /// <returns></returns>
         public List<Consult> GetConsultSingle(string key,int value)
        {
            switch (key)
            {
                case "Id":
                   return this.GetList().Where(c=>c.Id==value).ToList();
                case "TeacherName":
                  return  this.GetList().Where(c => c.TeacherName == value).ToList();

            }
            return new List<Consult>();
        }
        /// <summary>
        /// 获取所有咨询师的数据
        /// </summary>
        /// <returns></returns>
        public List<ConsultTeacher> GetConsultTeacher()
        {
            return Ct_Entity.GetIQueryable().ToList();
        }
        /// <summary>
        /// 获取单个咨询师的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmployeesInfo GetEmplyeesInfo(string id)
        {
            return emp_Entity.GetEntity(id);
        }
        /// <summary>
        /// 获取备案学生数据
        /// </summary>
        /// <returns></returns>
        public List<ExportStudentBeanData> GetStudentPutRecored(string name,bool IsName)
        {
            if (IsName)
            {
                //根据Name查询
                return Stu_Entity.StudentOrride(name);
            }
            else
            {
                //跟据Id查询
                List<ExportStudentBeanData> list = new List<ExportStudentBeanData>();
                list.Add(Stu_Entity.whereStudentId(name));
                return list ;
            }
           
        }
        /// <summary>
        /// 获取当个学生备案数据
        /// </summary>
        /// <param name="id">学生备案Id</param>
        /// <returns></returns>
        public StudentPutOnRecord GetSingleStudent(int? id)
        {
            return Stu_Entity.GetEntity(id);
        }       
        /// <summary>
        /// 获取单个咨询师指定的分量数据
        /// </summary>
        /// <param name="empid">员工id</param>
        /// <returns></returns>
        public ConsultAndStedent GetSingleData(string empid)
        {
            ConsultAndStedent cc1 = new ConsultAndStedent();
            cc1.TeacherId = empid;
            cc1.TeacherName = GetEmplyeesInfo(empid).EmpName;
            //根据员工id找到咨询师Id
            ConsultTeacher find_ct=  Ct_Entity.GetList().Where(c => c.Employees_Id == empid).FirstOrDefault();
            //根据咨询Id找到分量Id
            List<Consult> fin_c=  GetConsultSingle("TeacherName", find_ct.Id);
            //定义一个存储学生的集合
            List<StudentPutOnRecord> div = new List<StudentPutOnRecord>();
            //根据分量Id去找学生
            foreach (Consult item1 in fin_c)
            {
               div.Add(GetSingleStudent(item1.StuName));
            }
            cc1.ListStudent = div;
            return cc1;
        }
        /// <summary>
        /// 获取分量的学生
        /// </summary>
        /// <returns></returns>
        public List<ConsultAndStedent> GetEverythingData()
        {
            List<ConsultAndStedent> con_list = new List<ConsultAndStedent>();
            List<ConsultTeacher> ct_list = Ct_Entity.GetList();
            foreach (ConsultTeacher item in ct_list)
            {
                con_list.Add(GetSingleData(item.Employees_Id));
            }
            return con_list;
        }
        /// <summary>
        /// 获取今年的某个月份的分量次数
        /// </summary>
        /// <param name="monthName">月份名称</param>
        /// <param name="IsTure">未完成或者已完成</param>
        /// <returns></returns>
        public int GetMonthData(int monthName,string IsTure,string TeacherId)
        {
            //未完成的条件是:状态不能是已完成，如果状态已完成还要看报名时间是不是当前年份，月份是不是当前月份，如果不是就是---未完成
            //获取已报名的Id
            int satateid = SSta_Entity.GetStu("已报名").Success==true? (SSta_Entity.GetStu("已报名").Data as StuStatus).Id:0;
            //获取当前的年份
            int year = DateTime.Now.Year;
            int list_count = 0;
            List<Consult> find_consult = this.GetIQueryable().ToList();
            if (!string.IsNullOrEmpty(TeacherId))
            {
                int teId = Convert.ToInt32(TeacherId);
                find_consult = this.GetIQueryable().Where(g => g.TeacherName == teId).ToList();
            }            
            if (IsTure== "未完成")
            {
                var Student_Id_list = find_consult.Where(g => Convert.ToDateTime(g.ComDate).Month == monthName && Convert.ToDateTime(g.ComDate).Year==year).Select(tt=>tt.StuName).ToList();        
                List<StudentPutOnRecord> student_list = Stu_Entity.GetList();
                List<StudentPutOnRecord> ss = new List<StudentPutOnRecord>();
                for (int j = 0; j < student_list.Count; j++)
                {
                    for (int i = 0; i < Student_Id_list.Count; i++)
                    {
                        if (student_list[j].Id == Student_Id_list[i])
                        {
                            if (student_list[j].StuStatus_Id != satateid)
                            {
                                ss.Add(student_list[j]);
                            }
                            else if (Convert.ToDateTime(student_list[j].StatusTime).Year != year)
                            {
                                ss.Remove(student_list[j]);
                                ss.Add(student_list[j]);
                            }
                            else if (Convert.ToDateTime(student_list[j].StatusTime).Month != monthName)
                            {
                                ss.Remove(student_list[j]);
                                ss.Add(student_list[j]);
                            }
                        }
                    }
                }
                list_count = ss.ToList().Count;
            }
            else if (IsTure == "已完成")
            {
                //完成数 
                if (string.IsNullOrEmpty(TeacherId))
                {
                    //(这是整体的完成量)
                    list_count = Stu_Entity.GetList().Where(s => s.StuStatus_Id == satateid && Convert.ToDateTime(s.StatusTime).Month == monthName && Convert.ToDateTime(s.StatusTime).Year == year).ToList().Count;
                }
                else
                {
                    List<StudentPutOnRecord> find_s = new List<StudentPutOnRecord>();
                    foreach (Consult item in find_consult)
                    {
                        find_s.Add(GetSingleStudent(item.StuName));
                    }
                    list_count= find_s.Where(f => f.StuStatus_Id == satateid && Convert.ToDateTime(f.StatusTime).Year == year && Convert.ToDateTime(f.StatusTime).Month == monthName).ToList().Count;
                }
                
            }
            else if (IsTure=="分量数量")
            {
                //分量数
                list_count = find_consult.Where(g => Convert.ToDateTime(g.ComDate).Month == monthName && Convert.ToDateTime(g.ComDate).Year == year).ToList().Count;
            }
            return list_count;
        }
        /// <summary>
        /// 获取今年所有月份咨询师完成量与未完成量
        /// </summary>
        /// <param name="Id">咨询师Id</param>
        /// <returns></returns>
        public List<ConsultZhuzImageData> GetImageData(string Id)
        {
            List<ConsultZhuzImageData> list_CZD = new List<ConsultZhuzImageData>();
            //如果没有值那么就是整体
            if (string.IsNullOrEmpty(Id))
            {               
                for (int i = 1; i <= 12; i++)
                {
                    ConsultZhuzImageData New_cc = new ConsultZhuzImageData();
                    New_cc.MonthName = i;
                    New_cc.wanchengcount = GetMonthData(i, "已完成",null);
                    New_cc.nowanchengcount = GetMonthData(i,"未完成",null);
                    New_cc.fengliangarry = GetMonthData(i,"分量数量",null);
                    list_CZD.Add(New_cc);
                }
                return list_CZD;
            }
            else
            //某个咨询师的情况
            {
                int t_Id = Convert.ToInt32(Id);
                for (int i = 1; i <= 12; i++)
                {
                    ConsultZhuzImageData New_cc = new ConsultZhuzImageData();
                    New_cc.MonthName = i;
                    New_cc.wanchengcount = GetMonthData(i,"已完成",t_Id.ToString());
                    New_cc.nowanchengcount = GetMonthData(i, "未完成", t_Id.ToString());
                    New_cc.fengliangarry = GetMonthData(i,"分量数量", t_Id.ToString());
                    list_CZD.Add(New_cc);
                }
                return list_CZD;
            }
             
 
        }
        /// <summary>
        /// 获取某个咨询师某个月的未完成跟完成数据
        /// </summary>
        /// <param name="monthName"></param>
        /// <param name="ConsultTeacherid"></param>
        /// <returns></returns>
        public List<ALLDATA> GetTeacherMonthCount(int monthName,int teacherid,int Number)
        {
            //获取当前年份
            int year = DateTime.Now.Year;
            List<ALLDATA> aLLDATAs = new List<ALLDATA>();
            //获取当前年份的要查询的月份的分量数据
          
            if (Number==0)
            {
                List<Consult> Getlist_all = this.GetIQueryable().ToList().Where(c =>c.TeacherName==teacherid && GetSingleStudent(c.StuName).StatusTime!=null && Convert.ToDateTime(GetSingleStudent(c.StuName).StatusTime).Month==monthName).ToList();
                //完成的量
                List<Consult> ok_all = Getlist_all.Where(c => Convert.ToDateTime(GetSingleStudent(c.StuName).StatusTime).Month == monthName).ToList();
                ALLDATA a_list = new ALLDATA();//完成的
                List<DivTwoData> towdata = new List<DivTwoData>();
                foreach (Consult item1 in ok_all)
                {
                    a_list.Name = 1;
                    DivTwoData d = new DivTwoData();
                    d.ConsultId = item1.Id;
                    d.StudentId = item1.StuName;
                    d.StudentName = GetSingleStudent(item1.StuName).StuName;
                    d.ConsultTeacherId = item1.TeacherName;
                    d.ConsultTeacherName = GetEmplyeesInfo(Ct_Entity.GetEntity(item1.TeacherName).Employees_Id).EmpName;
                    towdata.Add(d);
                }
                a_list.list = towdata;

                aLLDATAs.Add(a_list);

                return aLLDATAs;
            }
            else if(Number==1)
            {

                List<Consult> Getlist_all = this.GetIQueryable().ToList().Where(c => Convert.ToDateTime(c.ComDate).Year == year && Convert.ToDateTime(c.ComDate).Month == monthName).ToList();
                //未完成的量
                List<Consult> no_all = Getlist_all.Where(c => c.TeacherName == teacherid && (GetSingleStudent(c.StuName).StatusTime == null || Convert.ToDateTime(GetSingleStudent(c.StuName).StatusTime).Month != monthName)).ToList();
                ALLDATA aa_list = new ALLDATA();//未完成的
                List<DivTwoData> towdata2 = new List<DivTwoData>();
                foreach (Consult item1 in no_all)
                {
                    aa_list.Name = 0;
                    DivTwoData d = new DivTwoData();
                    d.ConsultId = item1.Id;
                    d.StudentId = item1.StuName;
                    d.StudentName = GetSingleStudent(item1.StuName).StuName;
                    d.ConsultTeacherId = item1.TeacherName;
                    d.ConsultTeacherName = GetEmplyeesInfo(Ct_Entity.GetEntity(item1.TeacherName).Employees_Id).EmpName;
                    towdata2.Add(d);
                }
                aa_list.list = towdata2;
                aLLDATAs.Add(aa_list);

                return aLLDATAs;
            } else 
            {
                //分量数
                List<Consult> Getlist_all = this.GetIQueryable().ToList().Where(c => Convert.ToDateTime(c.ComDate).Year == year && Convert.ToDateTime(c.ComDate).Month == monthName && c.TeacherName == teacherid).ToList();
                ALLDATA aa_list = new ALLDATA();
                List<DivTwoData> towdata2 = new List<DivTwoData>();
                foreach (Consult item1 in Getlist_all)
                {
                    aa_list.Name = 0;
                    DivTwoData d = new DivTwoData();
                    d.ConsultId = item1.Id;
                    d.StudentId = item1.StuName;
                    d.StudentName = GetSingleStudent(item1.StuName).StuName;
                    d.ConsultTeacherId = item1.TeacherName;
                    d.ConsultTeacherName = GetEmplyeesInfo(Ct_Entity.GetEntity(item1.TeacherName).Employees_Id).EmpName;
                    towdata2.Add(d);
                }
                aa_list.list = towdata2;
                aLLDATAs.Add(aa_list);

                return aLLDATAs;
            }        
        }
        /// <summary>
        /// 通过备案学生找分量信息
        /// </summary>
        /// <param name="id">备案学生Id</param>
        /// <returns></returns>
        public Consult TongStudentIdFindConsult(int id)
        {
            return this.GetIQueryable().Where(c => c.StuName == id).FirstOrDefault();
         
        }
        /// <summary>
        /// 根据分量查询备案学生跟踪信息
        /// </summary>
        /// <param name="ConsultId">分量Id</param>
        /// <returns></returns>
        public List<FollwingInfo> GetFllowInfoData(int? ConsultId)
        {
          return  Fi_Entity.GetList().Where(f => f.Consult_Id == ConsultId).ToList();
        }
        //获取某个咨询师某个月分量的个数
        public int GetCount(int id,int monthName)
        {
            int year = DateTime.Now.Year;
            return this.GetIQueryable().ToList().Where(c => c.TeacherName == id && Convert.ToDateTime(c.ComDate).Month == monthName && Convert.ToDateTime(c.ComDate).Year == year).Count();
        }
        //获取某个咨询师某个月份完成的个数
        public int GetWangcenCount(int id,int monthName)
        {
            int year = DateTime.Now.Year;
            return this.GetList().Where(c => c.TeacherName == id && GetSingleStudent(c.StuName).StatusTime != null && Convert.ToDateTime(GetSingleStudent(c.StuName).StatusTime).Month == monthName && Convert.ToDateTime(GetSingleStudent(c.StuName).StatusTime).Year==year).Count();
        }
        
        /// <summary>
        /// 添加单条分量数据
        /// </summary>
        /// <param name="new_c"></param>
        /// <returns></returns>
        public bool AddSing(Consult new_c)
        {
            bool s = false;
            try
            {
                this.Insert(new_c);
                s = true;
            }
            catch (Exception)
            {
                s = false;
            }

            return s;
        }

        /// <summary>
        /// 获取备案学生数据
        /// </summary>
        /// <param name="Stu_id">学生备案Id</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public ExportStudentBeanData GetStudentData(int Stu_id)
        {
            List<ExportStudentBeanData> data = new List<ExportStudentBeanData>();
            string str = "select * from  StudentBeanView where Id=" + Stu_id;
            if (Stu_id < 54117)
            {
                data = Stu_Entity.GetListBySql<Sch_MarketView>("select * from Sch_MarketView where Id=" + Stu_id).Select(d => new ExportStudentBeanData()
                {
                    StuName = d.StudentName,
                    StuSex = d.Sex,
                    StuBirthy = null,
                    Stuphone = d.Phone,
                    StuSchoolName = d.School,
                    StuEducational = d.Education,
                    StuAddress = null,
                    StuWeiXin = null,
                    StuQQ = d.QQ,
                    stuinfomation = d.source,
                    StatusName = d.MarketState,
                    StuisGoto = false,
                    StuVisit = null,
                    empName = d.SalePerson,
                    Party = d.RelatedPerson,
                    BeanDate = d.CreateDate,
                    StuEntering = d.CreateUserName,
                    StatusTime = null,
                    RegionName = d.Area,
                    Reak = null,
                    ConsultTeacher = d.Inquiry
                }).ToList();
            }
            else
            {
                data = Stu_Entity.GetListBySql<ExportStudentBeanData>("select * from  StudentBeanView where Id=" + Stu_id);
            }
            return data[0];
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <returns></returns>
        public AjaxResult Add_Data(List<Consult> list)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Insert(list);
                a.Success = true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                a.Success = false;
            }

            return a;
        }

        /// <summary>
        /// 转换咨询师
        /// </summary>
        /// <param name="list">备案Id</param>
        /// <param name="TeacherId">咨询师Id</param>
        /// <returns></returns>
        public AjaxResult ChangTeacher(List<int> list,int TeacherId)
        {
            List<Consult> countdata = new List<Consult>();
            foreach (int id in list)
            {
               Consult find= this.GetList().Where(t => t.StuName == id).FirstOrDefault();

                if (find!=null)
                {
                    find.TeacherName = TeacherId;
                    countdata.Add(find);
                }
                else
                {
                    //添加数据
                    Consult c = new Consult();
                    c.ComDate = DateTime.Now;
                    c.IsDelete = false;
                    c.MarketType = null;
                    c.StuName = id;
                    c.TeacherName = TeacherId;

                    this.Insert(c);
                }
            }
            AjaxResult a = new AjaxResult();
            try
            {
                this.Update(countdata);
                a.Success = true;
                a.Msg = "操作成功";
            }
            catch (Exception)
            {
                a.Success = false;
                a.Msg = "系统异常！，请刷新重试！";
            }

            return a;
        }

        /// <summary>
        /// 根据备案编号获取分量数据
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        public Consult AccordingStuIdGetConsultData(int studentID)
        {
           List<Consult> find= this.GetListBySql<Consult>("select * from Consult where StuName=" + studentID);
            if (find.Count>0)
            {
                return find[0];
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        public AjaxResult MyUpdate(Consult news)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Update(news);
                a.Success = true;
            }
            catch (Exception)
            {
                a.Success = false;               
            }

            return a;
        }

        #region  给跟踪业务使用的方法
        //给咨询分量的数据
        public List<StudentPutOnRecord> GetStudentData(int monName,string Status,int? consultTeacherId)
        {
            List<StudentPutOnRecord> finds_list = new List<StudentPutOnRecord>();
            List<StudentPutOnRecord> student_list = Stu_Entity.GetList();
            switch (Status)
            {
                //完成量
                case "1":
                    List<Consult> c_list = this.GetList().Where(c => GetSingleStudent(c.StuName).StatusTime != null).ToList();
                    List<Consult> find_c = c_list.Where(c => c.TeacherName == consultTeacherId && Convert.ToDateTime(GetSingleStudent(c.StuName).StatusTime).Month == monName).ToList();                    
                    foreach (Consult con in find_c)
                    {
                        foreach (StudentPutOnRecord stu in student_list)
                        {
                            if (con.StuName==stu.Id)
                            {
                                finds_list.Add(stu);
                            }
                        }
                    }
                    break;
                //未完成量
                case "2":
                    List<Consult> c_list2 = this.GetList().Where(c => (GetSingleStudent(c.StuName).StatusTime== null || Convert.ToDateTime(GetSingleStudent(c.StuName).StatusTime).Month!= monName) && c.TeacherName == consultTeacherId &&Convert.ToDateTime( c.ComDate).Month== monName).ToList();
                    foreach (Consult con in c_list2)
                    {
                        foreach (StudentPutOnRecord stu in student_list)
                        {
                            if (con.StuName == stu.Id)
                            {
                                finds_list.Add(stu);
                            }
                        }
                    }
                    break;
                //分量数
                case "3":
                    List<Consult> c_list3 = this.GetList().Where(c =>Convert.ToDateTime( c.ComDate).Month== monName && c.TeacherName==consultTeacherId).ToList();
                    foreach (Consult con in c_list3)
                    {
                        foreach (StudentPutOnRecord stu in student_list)
                        {
                            if (con.StuName == stu.Id)
                            {
                                finds_list.Add(stu);
                            }
                        }
                    }
                    break;
            }
            return finds_list;
        }

        #endregion

        public void OneUpdate()
        {
            List<FollwingInfo> list = Fi_Entity.GetList();
            List<Consult> c = new List<Consult>();
            foreach (FollwingInfo item in list)
            {
                Consult finda= this.GetEntity(item.Consult_Id);
               // finda.MarketType = item.Rank;
                c.Add(finda);
            }

            try
            {
                this.Update(c);
            }
            catch (Exception ex)
            {
                string w = ex.Message;
                 
            }

            
        }


        public int Count(int TEAID)
        {
            List<Consult> find = this.GetListBySql<Consult>("select * from Consult where TeacherName=" + TEAID);

            return find.Count;
        }
    }
}
