using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
using SiliconValley.InformationSystem.Business.DepartmentBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SiliconValley.InformationSystem.Business.DormitoryBusiness;//教官业务类
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{
    public class ReconcileManeger : BaseBusiness<Reconcile>
    {
        public static readonly EvningSelfStudyManeger EvningSelfStudy_Entity = new EvningSelfStudyManeger();

        public EmployeesInfoManage dbemployeesInfo = new EmployeesInfoManage();
        public TeacherClassBusiness TeacherClass_Entity = new TeacherClassBusiness();
        public ClassScheduleBusiness ClassSchedule_Entity = new ClassScheduleBusiness();

        public Reconcile GetReconcile_classschedule(int year, int month, string EmpID, string CourseName)
        {
            string sql = $"select * from Reconcile where YEAR(AnPaiDate)='{year}' and MONTH(AnPaiDate)='{month}' and EmployeesInfo_Id='{EmpID}' and Curriculum_Id = '{CourseName}'";
            return GetListBySql<Reconcile>(sql).FirstOrDefault();
        }

        /// <summary>
        /// 查询改员工在本月的教课数量
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="Emp_ID"></param>
        /// <returns></returns>
        public int GetClassCount(int year, int month, string Emp_ID)
        {
            string sql = $"select * from Reconcile where YEAR(AnPaiDate)='{year}' and MONTH(AnPaiDate)= '{month}'and EmployeesInfo_Id = '{Emp_ID}'";
            List<Reconcile> myrecon = GetListBySql<Reconcile>(sql);
            var concile_List = (from m in myrecon
                                group m by m.Curriculum_Id
                           into list
                                select list).ToList();
            return concile_List.Count;
        }

        /// <summary>
        /// 根据年 月 员工ID 返回集合
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="Emp_ID"></param>
        /// <returns></returns>
        public List<Reconcile> GetReoncileByDate(int year, int month, string Emp_ID)
        {
            string sql = $"select * from Reconcile where YEAR(AnPaiDate)='" + year + "' and MONTH(AnPaiDate)= '" + month + "' and EmployeesInfo_Id = '" + Emp_ID + "'";
            return GetListBySql<Reconcile>(sql);
        }

        /// <summary>
        /// 通过sql语句获取所有数据
        /// </summary>
        /// <returns></returns>
        public List<ReconcileView> SQLGetReconcileDate()
        {
            List<ReconcileView> list = this.GetListBySql<ReconcileView>("select * from ReconcileView");
            return list;
        }

        /// <summary>
        /// 根据班级获取某个时间之后的排课数据
        /// </summary>
        /// <param name="class_id"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<Reconcile> Class_Date(int class_id, DateTime date)
        {
            string sql = "select * from Reconcile where ClassSchedule_Id=" + class_id + " and AnPaiDate>='" + date + "'";
            return this.GetListBySql<Reconcile>(sql);
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="date"></param>
        /// <param name="class_id"></param>
        /// <returns></returns>
        public List<Reconcile> GetSingData(DateTime date, int class_id)
        {
            Grand find_g = Reconcile_Com.GetGrand_Id().Where(g => g.GrandName == "Y1" || g.GrandName == "y1").FirstOrDefault();
            List<Reconcile> list = this.GetListBySql<Reconcile>("select * from Reconcile where anpaidate='" + date + "' and ClassSchedule_Id=" + class_id + "");
            List<Reconcile> koshi = list.Where(l => l.Curriculum_Id.Contains("考试")).ToList();
            ClassSchedule find_c = Reconcile_Com.ClassSchedule_Entity.GetEntity(class_id);
            List<CourseType> type = Reconcile_Com.CourseType_Entity.GetList();
            CourseType find_t1 = type.Where(t => t.TypeName.Equals("专业课")).FirstOrDefault();
            if (find_c.grade_Id == find_g.Id)
            {
                //如果是Y1,只需要专业，英语、数学、语文

                CourseType find_t2 = type.Where(t => t.TypeName.Equals("语文课")).FirstOrDefault();
                CourseType find_t3 = type.Where(t => t.TypeName.Equals("数学课")).FirstOrDefault();
                CourseType find_t4 = type.Where(t => t.TypeName.Equals("英语课")).FirstOrDefault();

                list = list.Where(l => Reconcile_Com.findname(l.Curriculum_Id) != null).ToList();//排除考试及其他活动的安排数据

                list = list.Where(l => Reconcile_Com.findname(l.Curriculum_Id).CourseType_Id == find_t1.Id || Reconcile_Com.findname(l.Curriculum_Id).CourseType_Id == find_t2.Id || Reconcile_Com.findname(l.Curriculum_Id).CourseType_Id == find_t3.Id || Reconcile_Com.findname(l.Curriculum_Id).CourseType_Id == find_t4.Id).ToList();

                list.AddRange(koshi);

            }
            else
            {
                //只需要专业、课程考试、升学考试

                list = list.Where(l => Reconcile_Com.findname(l.Curriculum_Id) != null).ToList();//排除考试及其他活动的安排数据
                list = list.Where(l => Reconcile_Com.findname(l.Curriculum_Id).CourseType_Id == find_t1.Id).ToList();//排除考试及其他活动的安排数据

                list.AddRange(koshi);
            }


            return list;
        }

        /// <summary>
        /// 获取排课所有数据
        /// </summary>
        /// <returns></returns>
        public List<Reconcile> GetAll()
        {
            return this.GetListBySql<Reconcile>("select * from  Reconcile");
        }
        /// <summary>
        /// 根据iD获取排课数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Reconcile FindId(int id)
        {
            List<Reconcile> list = this.GetListBySql<Reconcile>("select * from  Reconcile where Id=" + id);
            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReconcileView GetSingleData(int id)
        {
            return SQLGetReconcileDate().Where(r => r.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 单条数据添加（false--失败，true--成功）
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public bool AddData(Reconcile r)
        {
            bool s = false;
            try
            {
                List<ReconcileView> find_list = SQLGetReconcileDate().Where(rs => rs.AnPaiDate == r.AnPaiDate && rs.ClassSchedule_Id == r.ClassSchedule_Id && rs.Curriculum_Id == r.Curriculum_Id && rs.Curse_Id == r.Curse_Id).ToList();
                int count = find_list.Count;
                if (count <= 0)
                {
                    this.Insert(r);
                    s = true;

                }
                else
                {
                    if (r.Curriculum_Id.Contains("项目答辩"))
                    {
                        this.Insert(r);
                    }

                    s = true;

                }
            }
            catch (Exception ex)
            {
                string e = ex.Message;
                s = false;
            }
            return s;
        }

        public AjaxResult AddData(List<Reconcile> r)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Insert(r);
                a.Success = true;
                a.Msg = "操作成功！！";
            }
            catch (Exception ex)
            {
                a.Msg = "系统错误，请刷新重试！！";
                a.Success = false;
            }

            return a;
        }

        /// <summary>
        /// 修改数据 （false--失败,true--成功）
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public AjaxResult UpdateReconcile(Reconcile r)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                ReconcileView find_r = SQLGetReconcileDate().Where(f => f.Id == r.Id).FirstOrDefault();
                find_r.AnPaiDate = r.AnPaiDate;//日期                
                a.Success = true;
                Reconcile_Com.redisCache.RemoveCache("ReconcileList");
            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;
            }

            return a;
        }
        /// <summary>
        /// 修改数据（true--成功，false--失败）
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public AjaxResult Updata_data(ReconView r)
        {
            AjaxResult a = new AjaxResult();
            Reconcile find_r = this.GetEntity(r.id);
            try
            {
                //除了专业课之外的改为自习课
                if (find_r.Curse_Id.Contains("12") || find_r.Curse_Id.Contains("34"))
                {
                    find_r.Curriculum_Id = "自习";
                    find_r.EmployeesInfo_Id = null;
                    find_r.Rmark = r.ramak;
                    this.Update(find_r);
                    //清空缓存
                    Reconcile_Com.redisCache.RemoveCache("ReconcileList");
                    a.Success = true;
                    a.Msg = "编辑成功";
                }
                else
                {
                    if (r.curdname == "上午四节" || r.curdname == "下午四节")
                    {
                        //直接改为自习
                        find_r.Curriculum_Id = "自习";
                        find_r.EmployeesInfo_Id = null;
                        find_r.Rmark = r.ramak;
                        this.Update(find_r);
                        //清空缓存
                        Reconcile_Com.redisCache.RemoveCache("ReconcileList");
                        a.Success = true;
                        a.Msg = "编辑成功";
                    }
                    else
                    {
                        //添加数据
                        Reconcile add = new Reconcile();
                        add.AnPaiDate = find_r.AnPaiDate;
                        add.ClassRoom_Id = find_r.ClassRoom_Id;
                        add.ClassSchedule_Id = find_r.ClassSchedule_Id;

                        if (r.curdname == "上午12节")
                        {
                            find_r.Curse_Id = "上午34节";
                        }
                        else if (r.curdname == "下午12节")
                        {
                            find_r.Curse_Id = "下午34节";
                        }
                        else if (r.curdname == "上午34节")
                        {
                            find_r.Curse_Id = "上午12节";
                        }
                        else if (r.curdname == "下午34节")
                        {
                            find_r.Curse_Id = "下午12节";
                        }
                        add.EmployeesInfo_Id = null;
                        add.IsDelete = false;
                        add.NewDate = DateTime.Now;
                        add.Curse_Id = r.curdname;
                        add.Curriculum_Id = "自习";
                        bool s = this.AddData(add);
                        if (s)
                        {
                            this.Update(find_r);

                            //清空缓存
                            Reconcile_Com.redisCache.RemoveCache("ReconcileList");
                            a.Success = true;
                            a.Msg = "编辑成功";
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;
            }
            return a;
        }

        public AjaxResult Update_data2(List<ReconcileView> r, string timename, int room)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                foreach (ReconcileView item in r)
                {
                    if (item.Curse_Id == "下午" || item.Curse_Id == "上午")
                    {
                        item.Curse_Id = timename;
                    }
                    else
                    {
                        string tr = item.Curse_Id.Substring(2, 2);
                        item.Curse_Id = timename == "上午" ? "下午" + tr + "节" : "上午" + tr + "节";
                    }

                    if (item.ClassRoom_Id != room)
                    {
                        item.ClassRoom_Id = room;
                    }

                    this.Update(ReconcileView.ToModel(item));
                }
                Reconcile_Com.redisCache.RemoveCache("ReconcileList");
                a.Success = true;
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = ex.Message;

            }

            return a;
        }

        public AjaxResult Update_date(List<Reconcile> r)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Update(r);
                a.Success = true;
                //Reconcile_Com.redisCache.RemoveCache("ReconcileList");
            }
            catch (Exception)
            {
                a.Success = false;
                a.Msg = "系统异常！！！";
            }
            return a;
        }

        public AjaxResult update_date4(List<ReconcileView> r, DateTime time)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                foreach (ReconcileView em in r)
                {
                    em.AnPaiDate = time;
                    this.Update(ReconcileView.ToModel(em));
                }
                a.Success = true;
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = ex.Message;
            }
            return a;

        }
        /// <summary>
        /// 大批量删除
        /// </summary>
        /// <returns></returns>
        public AjaxResult Delete_More(List<Reconcile> list)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Delete(list);
                a.Success = true;
                a.Msg = "操作成功！！！";
            }
            catch (Exception)
            {
                a.Success = false;
                a.Msg = "系统异常，请重试！！！";
            }

            return a;
        }
        /// <summary>
        /// 判断日期是否可以排课（true--可以排课，false--不可以排课）
        /// </summary>
        /// <param name="xmlfile">xml路径</param>
        /// <param name="time">日期</param>
        /// <returns></returns>
        public bool IsShangKe(string xmlfile, DateTime time)
        {
            bool s = true;
            GetYear yy = MyGetYear(time.Year.ToString(), xmlfile);
            if (time.Month >= yy.StartmonthName && time.Month <= yy.EndmonthName)
            {
                //单休
                //判断time是否是星期天
                int tt = IsSaturday(time);
                if (tt == 2)
                {
                    s = false;
                }
            }
            else
            {
                //双休
                int tt = IsSaturday(time);
                if (tt == 2 || tt == 1)
                {
                    s = false;
                }
            }
            return s;
        }
        /// <summary>
        /// 获取可以上职素的班主任集合
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timename"></param>
        /// <returns></returns>
        public List<EmployeesInfo> GetMasTeacher(DateTime time, string timename)
        {
            List<EmployeesInfo> list = new List<EmployeesInfo>();

            //s1，s2,s3的班主任
            BaseBusiness<Headmaster> basemaster = new BaseBusiness<Headmaster>();
            //List<EmployeesInfo> elist = Reconcile_Com.GetAllNoGoingEMP();
            List<Headmaster> find_m = basemaster.GetList().Where(m => m.IsDelete == false && m.IsAttend == true).ToList();//获取没有辞职的可以上职素课的班主任
            foreach (Headmaster e1 in find_m)
            {
                EmployeesInfo findata = Reconcile_Com.Employees_Entity.GetEntity(e1.informatiees_Id);
                if (findata != null)
                {
                    //判断这个班主任是否有课
                    bool s = IsHaveClass(findata.EmployeeId, timename, time);
                    if (s == false)
                    {
                        list.Add(findata);
                    }
                }
            }
            List<EmployeesInfo> list_saff = Reconcile_Com.GetObtainTeacher();//获取未辞职的就业部老师

            foreach (EmployeesInfo item in list_saff)
            {
                bool s = IsHaveClass(item.EmployeeId, timename, time);
                if (s == false)
                {
                    list.Add(item);//获取就业部空闲的老师
                }
            }



            return list;
        }

        /// <summary>
        /// 获取所有教官
        /// </summary>
        /// <param name="s">true--获取所有教官，false--获取未离职的教官</param>
        /// <returns></returns>
        public List<EmployeesInfo> GetAlljiaoguan(bool s)
        {
            InstructorListBusiness instructor = new InstructorListBusiness();
            List<InstructorList> list = instructor.GetList();
            List<EmployeesInfo> emolist = new List<EmployeesInfo>();
            foreach (InstructorList item in list)
            {
                if (s)
                {
                    //获取所有教官
                    emolist.Add(dbemployeesInfo.FindEmpData(item.EmployeeNumber, true));
                }
                else
                {
                    //获取未离职教官
                    if (item.IsDel == false)
                    {
                        emolist.Add(dbemployeesInfo.GetEntity(item.EmployeeNumber));
                    }
                }

            }

            return emolist;
        }

        /// <summary>
        /// 获取教官
        /// </summary>
        /// <param name="time"></param>
        /// <param name="timename"></param>
        /// <returns></returns>
        public List<EmployeesInfo> GetSir(DateTime time, string timename)
        {

            List<EmployeesInfo> employees = GetAlljiaoguan(false);
            List<EmployeesInfo> em2 = new List<EmployeesInfo>();//获取空闲的教官

            //判断教官是否在这个时间段有课
            foreach (EmployeesInfo ee in employees)
            {
                bool s = IsHaveClass(ee.EmployeeId, timename, time);
                if (s == false)
                {
                    em2.Add(ee);
                }
            }
            return em2;
        }
        /// <summary>
        /// 判断这个老师在这个日期中的这个时间段是否有课(false--没有，true--有)
        /// </summary>
        /// <param name="Teacher_Id">老师编号</param>
        /// <param name="timename">时间段</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public bool IsHaveClass(string Teacher_Id, string timename, DateTime date)
        {
            bool s = false;
            List<ReconcileView> find_r = SQLGetReconcileDate().Where(r => r.EmployeesInfo_Id == Teacher_Id && r.AnPaiDate == date && r.Curse_Id == timename).ToList();
            if (find_r.Count > 0)
            {
                s = true;
            }
            return s;
        }
        /// <summary>
        /// 获取可以上哪个阶段课程的老师
        /// </summary>
        /// <param name="grand_id"></param>
        /// <returns></returns>
        public List<Teacher> GetListTeacher(int grand_id, string curr_name)
        {
            List<Teacher> list_Teacher = new List<Teacher>();
            Curriculum find_c = Reconcile_Com.Curriculum_Entity.GetList().Where(c => c.IsDelete == false && c.CourseName.Equals(curr_name, StringComparison.CurrentCultureIgnoreCase) && c.Grand_Id == grand_id).FirstOrDefault();//获取课程
            if (find_c != null)
            {
                List<GoodSkill> all_goodskill = Reconcile_Com.GoodSkill_Entity.GetList().Where(g => g.Curriculum == find_c.CurriculumID).ToList();//获取可以上这个课程的老师
                foreach (GoodSkill goods in all_goodskill)
                {
                    if (list_Teacher.Count > 0)
                    {
                        for (int i = 0; i < list_Teacher.Count; i++)
                        {
                            if (list_Teacher[i].TeacherID == goods.TearchID)
                            {
                                list_Teacher.Remove(list_Teacher[i]);
                                list_Teacher.Add(list_Teacher[i]);
                            }
                            else
                            {
                                list_Teacher.Add(list_Teacher[i]);
                            }
                        }
                    }
                    else
                    {
                        list_Teacher.Add(Reconcile_Com.Teacher_Entity.GetEntity(goods.TearchID));
                    }
                }
            }

            return list_Teacher.Where(c => c.IsDel == false).ToList();
        }
        /// <summary>
        /// 安排英语老师上课
        /// </summary>
        /// <param name="grand_id">阶段Id</param>
        /// <param name="curr_name">课程名称</param>
        /// <param name="timename">时间段</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public string AnpaiTeacher(int grand_id, string curr_name, string timename, DateTime date)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            List<Teacher> gettea = GetListTeacher(grand_id, curr_name);//获取英语老师
            foreach (Teacher tt in gettea)
            {
                bool s = IsHaveClass(tt.EmployeeId, timename, date);
                if (s == false)
                {
                    sb.Append(tt.EmployeeId);
                    break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 向Xml文件读取配置
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="XmlFile_url">xml的url</param>
        /// <returns></returns>
        public GetYear MyGetYear(string year, string XmlFile_url)
        {
            GetYear g = new GetYear();
            XElement xx = XElement.Load(XmlFile_url);
            XElement s = xx.Elements("Year").Where(e => e.Attribute("name").Value == year.ToString()).First();//筛选
            g.YearName = year;
            g.StartmonthName = Convert.ToInt32(s.Element("startmonth").Value);
            g.EndmonthName = Convert.ToInt32(s.Element("endmonth").Value);
            return g;

        }
        /// <summary>
        /// 判断该日期是否是周六或周末（1--周六，2--周日）
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int IsSaturday(DateTime date)
        {
            var day = date.DayOfWeek;
            //判断是否为周末

            if (day == DayOfWeek.Sunday)
            {
                return 2;//周日
            }
            else if (day == DayOfWeek.Saturday)
            {
                return 1;//周六
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取所有阶段集合
        /// </summary>
        /// <returns></returns>
        public List<Grand> GetEffectiveData()
        {
            List<Grand> grands = Reconcile_Com.Grand_Entity.GetList().Where(g => g.IsDelete == false).ToList();
            return grands;
        }
        /// <summary>
        /// 获取属于某个阶段的有效班级集合
        /// </summary>
        /// <param name="grand_id"></param>
        /// <returns></returns>
        public List<ClassSchedule> GetGrandClass(int grand_id)
        {
            //获取有效的班级数据//获取属于某个阶段的班级
            List<ClassSchedule> c_list = Reconcile_Com.GetClass().Where(c => c.grade_Id == grand_id).ToList();
            return c_list;
        }
        /// <summary>
        /// 获取班级上课时间
        /// </summary>
        /// <param name="FartherName">父级名称</param>
        /// <returns></returns>
        public List<BaseDataEnum> GetTimeCheckBox(string FartherName)
        {
            return Reconcile_Com.ClassSchedule_Entity.BaseDataEnum_Entity.GetsameFartherData(FartherName);
        }
        /// <summary>
        /// 获取某个校区的有效教室
        /// </summary>
        /// <param name="basedae_id">校区编号</param>
        /// <returns></returns>
        public List<Classroom> GetEffectioveClassRoom(int basedae_id)
        {
            return Reconcile_Com.Classroom_Entity.GetList().Where(c => c.IsDelete == false && c.BaseData_Id == basedae_id).ToList();
        }
        /// <summary>
        /// 获取班主任员工编号（获取可以上班会课的老师编号）
        /// </summary>
        /// <param name="class_id"></param>
        /// <returns></returns>
        public string GetMasterTeacher(int class_id, DateTime time, string timename)
        {
            //获取该班级的班主任
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            AjaxResult find_a = Reconcile_Com.GetZhisuTeacher(class_id);
            int count = 0;
            if (find_a.Success == true)
            {
                EmployeesInfo find_ff = find_a.Data as EmployeesInfo;
                AjaxResult find_class = Reconcile_Com.GetHadMasterClass(find_ff.EmployeeId);
                if (find_class.Success == true)
                {
                    List<HeadClass> headClasses = find_class.Data as List<HeadClass>;
                    foreach (HeadClass c in headClasses)
                    {
                        //判断是否有带的班级已在这时间段安排了班会课
                        count = count + SQLGetReconcileDate().Where(r => r.AnPaiDate == time && r.Curse_Id == timename && r.EmployeesInfo_Id == find_a.Data.ToString() && r.ClassSchedule_Id == c.ClassID).ToList().Count;
                    }
                }
                if (count <= 0)
                {
                    sb.Append(find_ff.EmployeeId);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 根据名称获取上课时间段
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BaseDataEnum GetClassTime(string name)
        {
            List<BaseDataEnum> find_list_base = GetTimeCheckBox("上课时间类型");
            return find_list_base.Where(b => b.Name == name).FirstOrDefault();
        }
        /// <summary>
        /// 是否是最后一门课程
        /// </summary>
        /// <param name="name">课程名称</param>
        /// <returns></returns>
        public bool IsEndCurr(string name)
        {
            bool s = false;
            Curriculum fin_c = Reconcile_Com.Curriculum_Entity.GetList().Where(c => c.CourseName == name).FirstOrDefault();
            if (fin_c != null)
            {
                s = Convert.ToBoolean(fin_c.IsEndCurr);
            }
            return s;
        }
        /// <summary>
        /// 大批量添加数据
        /// </summary>
        /// <param name="new_list">排课数据集合</param>
        /// <returns></returns>
        public AjaxResult Inser_list(List<Reconcile> new_list)
        {
            AjaxResult s = new AjaxResult();
            List<Reconcile> list = new List<Reconcile>();
            int orrindex = 0;
            try
            {
                foreach (Reconcile item in new_list)
                {
                    //判断是否有重复的值
                    bool ss = IsExcit(item, false);
                    if (ss)
                    {
                        orrindex++;
                    }
                    else
                    {
                        list.Add(item);

                    }

                }
                if (orrindex != 0)
                {
                    s.Msg = "已有重复数据" + orrindex + "条";
                }
                else
                {
                    s.Msg = "无重复数据，已安排成功";
                }
                this.Insert(list);
                s.Success = true;
                //Reconcile_Com.redisCache.RemoveCache("ReconcileList");
            }
            catch (Exception ex)
            {
                s.Success = false;
                s.Msg = ex.Message;
            }
            return s;
        }
        /// <summary>
        /// 是否有重复的数据--用于大批量添加数据 (false--没有重复，true--重复)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="singledata">true--单条数据判断,false--大批量数据判断</param>
        /// <returns></returns>
        public bool IsExcit(Reconcile r, bool singledata)
        {
            bool s = false;
            if (singledata)
            {
                //单条数据判断
                ReconcileView find_r = SQLGetReconcileDate().Where(rs => rs.ClassSchedule_Id == r.ClassSchedule_Id && rs.Curriculum_Id == r.Curriculum_Id && rs.AnPaiDate == r.AnPaiDate && rs.Curse_Id == r.Curse_Id).FirstOrDefault();
                if (find_r != null)
                {
                    if (!find_r.Curriculum_Id.Contains("项目答辩"))
                    {
                        s = true;
                    }

                }
            }
            else
            {
                //集合数据判断
                ReconcileView find_r = SQLGetReconcileDate().Where(rs => rs.Curriculum_Id == r.Curriculum_Id && rs.AnPaiDate == r.AnPaiDate && rs.ClassSchedule_Id == r.ClassSchedule_Id && r.Curriculum_Id !="软件工厂" && r.Curriculum_Id !="工厂项目").FirstOrDefault();
                if (find_r != null)
                {
                    s = true;
                }
            }

            return s;
        }
        /// <summary>
        /// 判断某班级某天安排了某个课程(false--没有安排，true--已安排)
        /// </summary>
        /// <param name="class_id"></param>
        /// <param name="time"></param>
        /// <param name="currname"></param>
        /// <returns></returns>
        public bool ToHavaKe(int class_id, DateTime time, string currname)
        {
            bool s = false;
            int count = SQLGetReconcileDate().Where(r => r.ClassSchedule_Id == class_id && r.AnPaiDate == time && r.Curriculum_Id == currname).ToList().Count;
            if (count > 0)
            {
                s = true;
            }
            return s;
        }
        /// <summary>
        /// 获取一周或两周的日期
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public Mydate GetMydate(DateTime d, int type)
        {
            Mydate mydate = new Mydate();
            DayOfWeek week = d.DayOfWeek;
            switch (week)
            {
                case DayOfWeek.Monday://星期一
                    mydate.StarTime = d;
                    mydate.EndTime = d.AddDays(5);
                    break;
                case DayOfWeek.Tuesday://星期二
                    mydate.StarTime = d.AddDays(-1);
                    mydate.EndTime = d.AddDays(4);
                    break;
                case DayOfWeek.Wednesday://星期三
                    mydate.StarTime = d.AddDays(-2);
                    mydate.EndTime = d.AddDays(3);
                    break;
                case DayOfWeek.Thursday://星期四
                    mydate.StarTime = d.AddDays(-3);
                    mydate.EndTime = d.AddDays(2);
                    break;
                case DayOfWeek.Friday://星期五
                    mydate.StarTime = d.AddDays(-4);
                    mydate.EndTime = d.AddDays(2);
                    break;
                case DayOfWeek.Saturday://星期六
                    mydate.StarTime = d.AddDays(-5);
                    mydate.EndTime = d;
                    break;
            }
            if (type == 2)
            {
                //输出两周的日期
                mydate.StarTime = mydate.StarTime.AddDays(-7);
            }
            return mydate;
        }
        /// <summary>
        /// 通过课程名称获取课程数据
        /// </summary>
        /// <param name="Name">课程名称</param>
        /// <returns></returns>
        public Curriculum GetCurriculum(string Name)
        {
            return Reconcile_Com.Curriculum_Entity.GetList().Where(w => w.CourseName == Name).FirstOrDefault();
        }
        /// <summary>
        /// 查看这个班级在这周有没有安排XXX课（true-已安排,false-未安排）
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="ClassName">班级</param>
        /// <param name="CurrName">英语课或班会课</param>
        /// <returns></returns>
        public bool Existence(Mydate time, int ClassName, string CurrName)
        {
            bool s = false;
            int count = SQLGetReconcileDate().Where(r => r.AnPaiDate >= time.StarTime && r.AnPaiDate <= time.EndTime && r.ClassSchedule_Id == ClassName && r.Curriculum_Id == CurrName).ToList().Count;
            if (count > 0)
            {
                s = true;
            }
            return s;
        }
        /// <summary>
        /// 查看xx班级是否安排了XX课程(false=没有安排，true=已安排)
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="class_id">班级</param>
        /// <param name="currname">课程名称</param>
        /// <returns></returns>
        public bool ExistenceToday(DateTime? time, int class_id, string currname)
        {
            bool s = false;
            int count = SQLGetReconcileDate().Where(r => r.ClassSchedule_Id == class_id && r.AnPaiDate == time && r.Curriculum_Id == currname).ToList().Count;
            if (count > 0)
            {
                s = true;
            }
            return s;
        }
        /// <summary>
        /// 查看这个教室是否已满
        /// </summary>
        /// <param name="c_id">教室编号</param>
        /// <param name="d">周期</param>
        /// <param name="time">上午1234，下午1234</param>
        /// <returns></returns>
        public string RoomTime(int c_id, DateTime d, int? base_id)
        {
            BaseDataEnum find_b = Reconcile_Com.ClassSchedule_Entity.BaseDataEnum_Entity.GetEntity(base_id);
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            List<string> str = new List<string>();
            if (find_b.Name == "上午")
            {
                str.Add("下午12节");
                str.Add("下午34节");
            }
            else
            {
                str.Add("上午12节");
                str.Add("上午34节");
            }
            List<ReconcileView> reconciles = SQLGetReconcileDate().Where(r => r.AnPaiDate == d && r.ClassRoom_Id == c_id).ToList();
            for (int i = 0; i < reconciles.Count; i++)
            {
                for (int j = 0; j < str.Count; j++)
                {
                    if (reconciles[i].Curse_Id == "上午")
                    {
                        str.Remove("上午12节");
                        str.Remove("上午34节");
                    }
                    else if (reconciles[i].Curse_Id == "下午")
                    {
                        str.Remove("下午12节");
                        str.Remove("下午34节");
                    }
                    else
                    {
                        if (reconciles[i].Curse_Id == str[j])
                        {
                            str.Remove(str[j]);
                        }
                    }

                }
            }
            if (str.Count > 0)
            {
                Random rs = new Random();
                int num = rs.Next(0, str.Count);
                sb.Append(str[num]);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 获取空教室
        /// </summary>
        /// <param name="TimeName">'上午'--获取下午的空教室'下午'--获取上午的空教室</param>
        /// <param name="time">获取哪个日期里的空教室</param>
        /// <returns></returns>
        public List<Classroom> GetClassrooms(string TimeName, DateTime time)
        {
            List<Classroom> c_list = Reconcile_Com.Classroom_Entity.GetEffectiveClass();
            if (TimeName == "晚自习" || TimeName == "晚一" || TimeName == "晚二")
            {
                List<ReconcileView> reconciles = SQLGetReconcileDate().Where(r => (r.Curse_Id == "晚自习" || r.Curse_Id == "晚一" || r.Curse_Id == "晚二") && r.AnPaiDate == time).ToList();
                for (int i = 0; i < reconciles.Count; i++)
                {
                    for (int j = 0; j < c_list.Count; j++)
                    {
                        if (reconciles[i].ClassRoom_Id == c_list[j].Id)
                        {
                            c_list.Remove(c_list[j]);
                        }
                    }
                }
                return c_list;
            }
            else
            {
                List<ReconcileView> reconciles = new List<ReconcileView>();
                if (TimeName.Contains("上午"))
                {
                    reconciles = SQLGetReconcileDate().Where(r => (r.Curse_Id == TimeName || r.Curse_Id == "上午") && r.AnPaiDate == time).ToList();
                }
                else if (TimeName.Contains("下午"))
                {
                    reconciles = SQLGetReconcileDate().Where(r => (r.Curse_Id == TimeName || r.Curse_Id == "下午") && r.AnPaiDate == time).ToList();
                }
                for (int i = 0; i < reconciles.Count; i++)
                {
                    for (int j = 0; j < c_list.Count; j++)
                    {
                        if (reconciles[i].ClassRoom_Id == c_list[j].Id)
                        {
                            c_list.Remove(c_list[j]);
                        }
                    }
                }
                return c_list;
            }

        }

        /// <summary>
        /// 判断这个班级在这个日期中的时间段是否有课(false--没有，true--有)
        /// </summary>
        /// <param name="class_id">班级编号</param>
        /// <param name="timename">时间段</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public bool IshavaOrther(int class_id, string timename, DateTime time)
        {
            bool s = false;
            int count = SQLGetReconcileDate().Where(r => r.ClassSchedule_Id == class_id && r.AnPaiDate == time && r.Curse_Id == timename).ToList().Count;
            if (count > 0)
            {
                s = true;
            }
            return s;
        }

        /// <summary>
        ///  判断XX班级在这期间是否上过XX课程(false--没有，ture--有)
        /// </summary>
        /// <param name="startime">开始日期</param>
        /// <param name="endtime">结束日期</param>
        /// <param name="currName">课程名称</param>
        /// <param name="class_id">班级名称</param>
        /// <returns></returns>
        public bool IsHaveCurr(DateTime startime, DateTime endtime, string currName, int class_id)
        {
            bool s = false;
            int count = SQLGetReconcileDate().Where(r => r.ClassSchedule_Id == class_id && r.Curriculum_Id == currName && r.AnPaiDate >= startime && r.AnPaiDate <= endtime).ToList().Count();
            if (count > 0)
            {
                s = true;
            }
            return s;
        }

        /// <summary>
        /// 获取非专业老师
        /// </summary>
        /// <param name="PosionName">岗位名称</param>
        /// <returns></returns>
        public List<EmployeesInfo> GetEmployees(string PosionName)
        {
            BaseBusiness<EmployeesInfo> baseEmplo = new BaseBusiness<EmployeesInfo>();
            BaseBusiness<Position> basePosition = new BaseBusiness<Position>();
            Position p = basePosition.GetIQueryable().Where(b => b.IsDel == false && b.PositionName == PosionName).FirstOrDefault();
            if (p != null)
            {
                return baseEmplo.GetIQueryable().Where(b => b.IsDel == false && b.PositionId == p.Pid).ToList();//根据岗位匹配未辞职的老师
            }
            else
            {
                return new List<EmployeesInfo>();
            }
        }
        /// <summary>
        /// 删除数据(true--成功 ，true--失败)
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public AjaxResult DeleteReconcile(int r_id)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                ReconcileView find_r = SQLGetReconcileDate().Where(r => r.Id == r_id).FirstOrDefault();
                this.Delete(ReconcileView.ToModel(find_r));
                a.Success = true;
                a.Msg = "操作成功";

            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = ex.Message;
            }
            return a;
        }

        /// <summary>
        /// 获取单个语数英老师
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="currname">课程</param>
        /// <param name="timename">时间段</param>
        /// <returns></returns>
        public string GetNomaginTeacher(DateTime time, string currname, string timename, int grand_id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            switch (currname)
            {
                case "语文":
                    List<EmployeesInfo> list_e = GetEmployees("语文老师");
                    if (list_e.Count > 0)
                    {
                        sb.Append(list_e[0].EmployeeId);
                    }
                    break;
                case "数学":
                    List<EmployeesInfo> list_e1 = GetEmployees("数学老师");
                    if (list_e1.Count > 0)
                    {
                        sb.Append(list_e1[0].EmployeeId);
                    }
                    break;
                case "英语":
                    sb.Append(AnpaiTeacher(grand_id, currname, timename, time));
                    break;
            }
            //查看这个老师这一天是否有课
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                int count = SQLGetReconcileDate().Where(r => r.EmployeesInfo_Id == sb.ToString() && r.AnPaiDate == time && r.Curse_Id == timename).ToList().Count;
                if (count > 0)
                {
                    sb.Append("");
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取任课老师集合
        /// </summary>
        /// <param name="cur_id">课程Id</param>
        /// <param name="timename">时间段</param>
        /// <param name="time">日期</param>
        /// <returns></returns>
        public List<EmployeesInfo> GetMarjorTeacher(int cur_id, string timename, DateTime time)
        {
            //获取可以上这个课程的老师
            List<Teacher> g_list = Reconcile_Com.GoodSkill_Entity.GetTeachers(cur_id);
            //判断这个老师在XX日期XX时间段是否有课
            List<EmployeesInfo> e_list = Reconcile_Com.Employees_Entity.GetList();
            List<EmployeesInfo> emp = new List<EmployeesInfo>();
            foreach (EmployeesInfo item1 in e_list)
            {
                foreach (Teacher item2 in g_list)
                {
                    if (item1.EmployeeId == item2.EmployeeId)
                    {
                        bool s = IsHaveClass(item2.EmployeeId, timename, time);//判断老师是否有课
                        if (s == false)
                        {
                            emp.Add(item1);
                        }
                    }
                }
            }
            return emp;
        }
        /// <summary>
        /// 推后课程
        /// </summary>
        /// <param name="date">开始日期</param>
        /// <param name="days">天数</param>
        /// <returns></returns>
        public bool AidAllData(int days, GetYear year, List<Reconcile> reconciles)
        {
            bool s = false;
            //if (days>1)
            //{
            //    days = days - 1;
            //}
            try
            {
                List<Reconcile> Recon = new List<Reconcile>();
                for (int i = 0; i < days; i++)
                {
                    foreach (Reconcile re in reconciles)
                    {
                        if (re.AnPaiDate.Month >= year.StartmonthName && re.AnPaiDate.Month <= year.EndmonthName)
                        {
                            //单休
                            if (this.IsSaturday(re.AnPaiDate) == 1)
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays((2));
                            }
                            else
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(1);
                            }
                        }
                        else
                        {
                            //双休
                            DayOfWeek week = re.AnPaiDate.DayOfWeek;
                            if (week == DayOfWeek.Friday)
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays((3));
                            }
                            else
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(1);
                            }
                        }
                        Recon.Add(re);
                    }
                    this.Update(Recon);
                }



                s = true;
            }
            catch (Exception)
            {

                s = false;
            }

            return s;

        }
        /// <summary>
        /// 提前课程
        /// </summary>
        /// <param name="days"></param>
        /// <param name="year"></param>
        /// <param name="reconciles"></param>
        /// <returns></returns>
        public bool DescData(int days, GetYear year, List<Reconcile> reconciles)
        {
            bool s = false;
            try
            {
                List<Reconcile> Recon = new List<Reconcile>();
                for (int i = days; i < 0; i++)
                {
                    foreach (Reconcile re in reconciles)
                    {
                        DayOfWeek week = re.AnPaiDate.DayOfWeek;
                        if (re.AnPaiDate.Month >= year.StartmonthName && re.AnPaiDate.Month <= year.EndmonthName)
                        {

                            //单休
                            if (week == DayOfWeek.Monday)
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays((-2));
                            }
                            else
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(-1);
                            }
                        }
                        else
                        {
                            //双休                        
                            if (week == DayOfWeek.Monday)
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays((-3));
                            }
                            else
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(-1);
                            }
                        }
                        Recon.Add(re);
                    }
                    Recon = Recon.OrderBy(r => r.AnPaiDate).ToList();
                    this.Update(Recon);
                }



                s = true;
            }
            catch (Exception)
            {

                s = false;
            }

            return s;
        }

        /// <summary>
        /// 班级大批量的调课
        /// </summary>
        /// <param name="date">开始</param>
        /// <param name="days"></param>
        /// <param name="class_id"></param>
        /// <returns></returns>
        public bool AidClassData(DateTime date, int days, int class_id, GetYear YearMon, int curse)
        {
            bool s = false;
            //days = days - 1;
            try
            {


                List<Reconcile> recon = new List<Reconcile>();
                List<Reconcile> reconciles = Time_intervalDate(date, curse).Where(r => r.ClassSchedule_Id == class_id).ToList();
                for (int i = 0; i < days; i++)
                {
                    foreach (Reconcile re in reconciles)
                    {
                        if (re.AnPaiDate.Month >= YearMon.StartmonthName && re.AnPaiDate.Month <= YearMon.EndmonthName)
                        {
                            //单休
                            if (this.IsSaturday(re.AnPaiDate) == 1)
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(2);
                            }
                            else
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(1);
                            }
                        }
                        else
                        {
                            //双休
                            DayOfWeek week = re.AnPaiDate.DayOfWeek;
                            if (week == DayOfWeek.Friday)
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays((3));
                            }
                            else
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(1);
                            }
                        }

                        recon.Add(re);

                    }
                    this.Update(recon);
                }

                s = true;
            }
            catch (Exception)
            {
                s = false;
            }

            return s;
        }

        public bool DescClassData(DateTime date, int days, int class_id, GetYear YearMon, int curse)
        {
            bool s = false;
            //days = days - 1;
            try
            {
                List<Reconcile> recon = new List<Reconcile>();
                List<Reconcile> reconciles = Time_intervalDate(date, curse).Where(r => r.ClassSchedule_Id == class_id).OrderBy(r => r.AnPaiDate).ToList();
                for (int i = days; i < 0; i++)
                {
                    foreach (Reconcile re in reconciles)
                    {
                        DayOfWeek week = re.AnPaiDate.DayOfWeek;
                        if (re.AnPaiDate.Month >= YearMon.StartmonthName && re.AnPaiDate.Month <= YearMon.EndmonthName)
                        {
                            //单休
                            if (week == DayOfWeek.Monday)
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(-2);
                            }
                            else
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(-1);
                            }
                        }
                        else
                        {
                            //双休

                            if (week == DayOfWeek.Monday)
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays((-3));
                            }
                            else
                            {
                                re.AnPaiDate = re.AnPaiDate.AddDays(-1);
                            }
                        }

                        recon.Add(re);

                    }
                    this.Update(recon);
                }

                s = true;
            }
            catch (Exception)
            {
                s = false;
            }

            return s;
        }

        #region 提供修改排课数据的方法
        /// <summary>
        /// 获取XX班级在这XX天上XX课程的排课情况
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="class_id">班级编号</param>
        /// <param name="Time">上午或下午</param>
        /// <returns></returns>
        public List<ReconcileView> GetReconcile(DateTime time, int class_id, string currName)
        {
            return SQLGetReconcileDate().Where(r => r.AnPaiDate == time && r.ClassSchedule_Id == class_id && r.Curse_Id == currName).ToList();
        }

        /// <summary>
        /// 转视图模型
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public ResconcileView ConvertToView(ReconcileView r)
        {
            ResconcileView view = new ResconcileView();
            view.AnPaiDate = r.AnPaiDate;
            view.ClassRoom_Id = new Classroom() { Id = r.ClassRoom_Id, ClassroomName = r.ClassroomName };
            view.ClassSchedule_Id = new ClassSchedule() { id = r.ClassSchedule_Id, ClassNumber = r.ClassNumber };
            view.Curriculum_Id = r.Curriculum_Id;
            view.Curse_Id = r.Curse_Id;
            view.EmployeesInfo_Id = new EmployeesInfo() { EmployeeId = r.EmployeesInfo_Id, EmpName = r.EmpName };
            view.Id = r.Id;
            view.IsDelete = r.Isdelete;
            view.NewDate = r.NewDate;
            view.Rmark = r.Rmark;
            return view;
        }
        /// <summary>
        /// 代课业务(fasle--操作不成功，ture--操作成功)
        /// </summary>
        /// <param name="dateTime">代课日期</param>
        /// <param name="emp_id">代课老师</param>
        /// <param name="timename">代课时间段（上午，下午）</param>
        /// <param name="class_id">代课班级Id</param>
        /// <returns></returns>
        public AjaxResult Daike(DateTime dateTime, string emp_id, int class_id)
        {
            AjaxResult s = new AjaxResult();
            try
            {
                ReconcileView find_r = SQLGetReconcileDate().Where(r => r.AnPaiDate == dateTime && r.ClassSchedule_Id == class_id).FirstOrDefault();
                find_r.EmployeesInfo_Id = emp_id;
                this.Update(ReconcileView.ToModel(find_r));
                s.Msg = "代课成功";
                s.Success = true;
            }
            catch (Exception ex)
            {
                s.Msg = ex.Message;
                s.Success = false;
            }
            return s;
        }
        /// <summary>
        /// 加课
        /// </summary>
        /// <param name="dateTime">开始加课日期</param>
        /// <param name="emp_id">加课老师</param>
        /// <param name="timename">时间段(上午，下午，晚自习)</param>
        /// <param name="class_id">班级Id</param>
        /// <param name="curr_name">课程名称</param>
        /// <param name="addcount">加课次数</param>
        /// <param name="XmlFile">Server.MapPath("~/Xmlconfigure/Reconcile_XML.xml")</param>
        /// <returns></returns>
        public AjaxResult Addke(DateTime dateTime, string emp_id, string timename, int class_id, string curr_name, int addcount, string XmlFile)
        {
            AjaxResult data = new AjaxResult(); data.Success = false;
            int COU = 0;
            bool Schooladdress = Reconcile_Com.judgeClass(class_id);
            //判断是否是晚自习
            if (timename.Contains("晚自习"))
            {
                List<EvningSelfStudy> e_list = new List<EvningSelfStudy>();
                for (int i = 0; i < addcount; i++)
                {
                    if (i == 0)
                    {
                        dateTime = dateTime.AddDays(0);
                    }
                    else
                    {
                        dateTime = dateTime.AddDays(1);
                    }

                    //判断这个时间是否可以加课
                    GetYear getYear = MyGetYear(dateTime.Year.ToString(), XmlFile);
                    if (dateTime.Month >= getYear.StartmonthName && dateTime.Month <= getYear.EndmonthName)
                    {
                        //单休
                        if (IsSaturday(dateTime) == 2)
                        {
                            dateTime = dateTime.AddDays(1);
                        }
                    }
                    else
                    {
                        //双休
                        if (IsSaturday(dateTime) == 1)
                        {
                            dateTime = dateTime.AddDays(2);
                        }
                    }
                    //获取这个班级在哪里上晚自习
                    EvningSelfStudy find_eving = EvningSelfStudy_Entity.GetNving(dateTime, class_id);
                    if (find_eving == null)//如果还没有排晚自习
                    {
                        //获取空教室
                        ClassRoom_AddCourse find_emptyone = EvningSelfStudy_Entity.GetEmptyClassroom(dateTime, Schooladdress);
                        EvningSelfStudy new_eone = new EvningSelfStudy();
                        new_eone.Anpaidate = dateTime;
                        new_eone.Classroom_id = find_emptyone.ClassRoomId;
                        new_eone.ClassSchedule_id = class_id;
                        new_eone.curd_name = "晚一";
                        //new_eone.emp_id = emp_id;
                        new_eone.IsDelete = false;
                        new_eone.Newdate = DateTime.Now;
                        bool e1 = EvningSelfStudy_Entity.Add_Data(new_eone).Success;
                        EvningSelfStudy new_eTwo = new EvningSelfStudy();
                        new_eTwo.Anpaidate = dateTime;
                        new_eTwo.Classroom_id = find_emptyone.ClassRoomId;
                        new_eTwo.ClassSchedule_id = class_id;
                        new_eTwo.curd_name = "晚二";
                        //new_eTwo.emp_id = emp_id;
                        new_eTwo.IsDelete = false;
                        new_eTwo.Newdate = DateTime.Now;
                        bool e2 = EvningSelfStudy_Entity.Add_Data(new_eTwo).Success;
                        if (e1 == true && e2 == true)
                        {
                            COU++;
                            data.Success = true;
                            data.Msg = "加课成功";
                        }
                        else
                        {
                            data.Success = false;
                            data.Msg = "添加失败";
                        }
                    }
                    else
                    {
                        //修改晚自习排课信息
                        //find_eving.emp_id = emp_id;
                        //bool s1 = EvningSelfStudy_Entity.Update_DataTwo(find_eving).Success;
                        //获取这个教室的另外一个班级
                        EvningSelfStudy find_eving2 = EvningSelfStudy_Entity.GetOnCurrClass(dateTime, find_eving.Classroom_id).Where(e => e.ClassSchedule_id != class_id).FirstOrDefault();

                        //获取这一天晚自习的空教室
                        ClassRoom_AddCourse find_empty = EvningSelfStudy_Entity.GetEmptyClassroom(dateTime, Schooladdress);
                        find_eving2.Classroom_id = find_empty.ClassRoomId;
                        find_eving2.curd_name = find_empty.TimeName;
                        //修改另外一个表
                        //bool s2 = EvningSelfStudy_Entity.Update_DataTwo(find_eving2).Success;
                        //添加一条数据
                        EvningSelfStudy new_e = new EvningSelfStudy();
                        new_e.Anpaidate = dateTime;
                        new_e.Classroom_id = find_eving.Classroom_id;
                        new_e.ClassSchedule_id = find_eving.ClassSchedule_id;
                        new_e.curd_name = find_eving.curd_name == "晚一" ? "晚二" : "晚一";
                        //new_e.emp_id = emp_id;
                        new_e.IsDelete = false;
                        new_e.Newdate = DateTime.Now;
                        bool s3 = EvningSelfStudy_Entity.Add_Data(new_e).Success;

                        //if (s1 == true && s2 == true && s3 == true)
                        //{
                        //    COU++;
                        //}

                    }


                }
                if (COU == addcount)
                {
                    data.Success = true;
                }
            }
            else
            {
                int yes = 0;
                try
                {
                    List<ReconcileView> find_list = SQLGetReconcileDate().Where(r => r.ClassSchedule_Id == class_id && r.AnPaiDate > dateTime).ToList();
                    foreach (ReconcileView item in find_list)
                    {
                        item.AnPaiDate = item.AnPaiDate.AddDays(addcount);
                        this.Update(ReconcileView.ToModel(item));
                    }
                    //清空缓存
                    //Reconcile_Com.redisCache.RemoveCache("ReconcileList");
                    for (int i = 1; i <= addcount; i++)
                    {
                        Reconcile r = new Reconcile();
                        r.AnPaiDate = dateTime.AddDays(i);
                        r.ClassRoom_Id = find_list[0].ClassRoom_Id;
                        r.ClassSchedule_Id = class_id;
                        r.Curriculum_Id = curr_name;
                        r.Curse_Id = timename;
                        r.EmployeesInfo_Id = emp_id;
                        r.IsDelete = false;
                        r.NewDate = DateTime.Now;
                        bool s = this.AddData(r);
                        if (s == false)
                        {
                            break;
                        }
                        yes++;
                    }

                    if (yes == addcount)
                    {
                        data.Success = true;
                    }
                    else
                    {
                        data.Success = false;
                    }
                }
                catch (Exception ex)
                {

                    data.Success = false;
                    data.Msg = ex.Message;
                }


            }
            return data;
        }
        #endregion

        #region
        /// <summary>
        /// 获取这个日期的课表
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<ReconcileView> GetReconciles(DateTime time)
        {
            return this.GetListBySql<ReconcileView>("select * from ReconcileView where AnPaiDate='" + time + "'");
        }

        /// <summary>
        /// 高中生排课
        /// </summary>
        /// <param name="doublecease">true--双休，false--单休</param>
        /// <param name="curNo">课程编号</param>
        /// <param name="time">安排日期</param>
        /// <param name="timename">上课时间</param>
        /// <param name="classroomid">教室编号</param>
        /// <param name="empNo">老师员工编号</param>
        /// <param name="classNo">班级编号</param>
        /// <returns></returns>
        public List<Reconcile> HeghtStudentReconcileFunction(GetYear getYear, int curNo, DateTime time, string timename, int classroomid, string empNo, int classNo, int grand, int marjion_id)
        {
            Curriculum find_curdata = Reconcile_Com.Curriculum_Entity.GetEntity(curNo);   //获取课程

            List<Curriculum> find_curr_list = this.GetCurr(grand, true, marjion_id).Where(c => c.Sort >= find_curdata.Sort).OrderBy(c => c.Sort).ToList();

            List<Reconcile> r_list = new List<Reconcile>();

            foreach (Curriculum cur in find_curr_list)
            {
                int Sumcout = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(cur.CourseCount / 4.0))); //获取总节数,不算考试在内
                if (!cur.CourseName.Contains("STB") && cur.Grand_Id != 3 && cur.Grand_Id != 4 && cur.Grand_Id != 1002)//StB不需要考试，Y1，S3,S4不需要课程考试，需要项目答辩
                {
                    Sumcout = Sumcout + 1;
                }
                else if (cur.Grand_Id == 3 || cur.Grand_Id == 4 || cur.Grand_Id == 1002)
                {
                    if (this.IsEndCurr(cur.CourseName))
                    {
                        Sumcout = Sumcout + 1;
                    }
                }

                int index = 0;
                for (int i = 0; i < Sumcout; i++)
                {
                    index++;

                    if (time.Month <= getYear.EndmonthName && time.Month >= getYear.StartmonthName) //单休
                    {
                        if (this.IsSaturday(time) == 2)
                        {
                            time = time.AddDays(1);
                            Sumcout++;
                            index++;
                            i++;
                        }
                    }
                    else //双休
                    {

                        if (this.IsSaturday(time) == 1)
                        {
                            Sumcout = Sumcout + 2;
                            index = index + 2;
                            i = i + 2;
                            time = time.AddDays(2);
                        }

                    }
                    Reconcile new_R = new Reconcile();
                    new_R.AnPaiDate = time;
                    new_R.Curse_Id = timename;
                    new_R.ClassRoom_Id = classroomid;
                    new_R.ClassSchedule_Id = classNo;
                    new_R.EmployeesInfo_Id = empNo;
                    new_R.NewDate = DateTime.Now;
                    new_R.IsDelete = false;
                    new_R.Curriculum_Id = cur.CourseName;
                    if (index == Sumcout)
                    {
                        if (this.IsEndCurr(cur.CourseName))
                        {
                            new_R.Curriculum_Id = "升学考试";
                            new_R.EmployeesInfo_Id = null;
                        }
                        else
                        {
                            //如果是STB则不需要考试，如果是S3、S4、Y1的课程，不需要处理
                            if (!cur.CourseName.Contains("STB") && cur.Grand_Id != 3 && cur.Grand_Id != 4 && cur.Grand_Id != 1002)
                            {
                                new_R.Curriculum_Id = cur.CourseName + "考试";
                                new_R.EmployeesInfo_Id = null;
                            }

                        }

                    }

                    r_list.Add(new_R);
                    time = time.AddDays(1);

                    if (cur.IsEndCurr == true && index == Sumcout)
                    {
                        break;
                    }
                }
            }

            return r_list;
        }

        #region 初中生阶段排课
        /// <summary>
        /// 初中生排课
        /// </summary>
        /// <param name="doublecease"></param>
        /// <param name="curNo"></param>
        /// <param name="time"></param>
        /// <param name="timename"></param>
        /// <param name="classroomid"></param>
        /// <param name="empNo"></param>
        /// <param name="classNo"></param>
        /// <param name="grandid"></param>
        /// <returns></returns>
        //public List<Reconcile> MiddleStudentReconcileFunction(GetYear doublecease, int curNo, DateTime time, string timename, int classroomid, string empNo, int classNo, int grandid,string yuwen,string shuxue,string yingyu)
        //{
        //    Curriculum find_curdata = Reconcile_Com.Curriculum_Entity.GetEntity(curNo);   //获取课程

        //    List<Curriculum> find_curr_list = Reconcile_Com.GetbehindCurri(find_curdata).OrderBy(c=>c.Sort).ToList();

        //    List<Reconcile> r_list = new List<Reconcile>();

        //    bool IsMargin = true;//true--安排专业课，false--安排语数英

        //    string[] strcurr = new string[] { "语文,数学", "数学,英语", "英语,语文" };

        //    string[] strtimename = new string[] { "上午12节,上午34节", "下午12节,下午34节" };

        //    foreach (Curriculum cur in find_curr_list)
        //    {
        //        int Sumcout = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(cur.CourseCount / 4.0))); //获取总节数
        //        int nomargin = 0;
        //        for (int i = 0; i < Sumcout; i++)
        //        {

        //            Reconcile r = new Reconcile();
        //            //判断是否是单休

        //            if (time.Month>=doublecease.StartmonthName && time.Month<=doublecease.EndmonthName)
        //            {
        //                //单休
        //                if (this.IsSaturday(time) == 2)
        //                {
        //                    //如果是周日

        //                    time = time.AddDays(1);
        //                    i++;
        //                    Sumcout++;
        //                }

        //            }
        //            else
        //            {                        
        //                //双休
        //                if (this.IsSaturday(time) == 1)
        //                {
        //                    //如果是周六
        //                    // r.AnPaiDate = time.AddDays(i + 2);
        //                    time = time.AddDays(2);
        //                    i = i + 2;
        //                    Sumcout = Sumcout + 2;
        //                }
        //            }
        //            r.AnPaiDate = time;
        //            r.ClassRoom_Id = classroomid;
        //            r.ClassSchedule_Id = classNo;

        //            if (IsMargin == true)
        //            {
        //                r.EmployeesInfo_Id = empNo;
        //                r.Curse_Id = timename;
        //                r.Curriculum_Id = cur.CourseName;
        //                r.Curse_Id = timename;
        //                r.NewDate = DateTime.Now;
        //                r.IsDelete = false;
        //                r_list.Add(r);
        //            }
        //            else
        //            {
        //                Sumcout++;
        //                string[] currname1 = strcurr[nomargin].Split(',');

        //                //安排语数英
        //                if (timename == "上午")
        //                {
        //                    string[] timename1 = strtimename[0].Split(',');

        //                    for (int j = 0; j < timename1.Length; j++)
        //                    {
        //                        Reconcile cc1 = new Reconcile();
        //                        cc1.AnPaiDate = r.AnPaiDate;
        //                        cc1.ClassRoom_Id = classroomid;
        //                        cc1.ClassSchedule_Id = classNo;
        //                        cc1.Curriculum_Id = currname1[j];
        //                        cc1.Curse_Id = timename1[j];
        //                        cc1.NewDate = DateTime.Now;
        //                        cc1.IsDelete = false;
        //                        if (currname1[j] == "语文")
        //                        {
        //                            cc1.EmployeesInfo_Id = yuwen == null ? null : yuwen;
        //                        }else if (currname1[j] == "数学")
        //                        {
        //                            cc1.EmployeesInfo_Id = shuxue == null ? null : shuxue;
        //                        }
        //                        else if (currname1[j] == "英语")
        //                        {
        //                            cc1.EmployeesInfo_Id = yingyu == null ? null : yingyu;
        //                        }                                                                 
        //                       r_list.Add(cc1);
        //                    }

        //                }
        //                else if (timename == "下午")
        //                {
        //                    string[] timename1 = strtimename[1].Split(',');

        //                    for (int j = 0; j < timename1.Length; j++)
        //                    {
        //                        Reconcile cc1 = new Reconcile();
        //                        cc1.AnPaiDate = r.AnPaiDate;
        //                        cc1.ClassRoom_Id = classroomid;
        //                        cc1.ClassSchedule_Id = classNo;
        //                        cc1.Curriculum_Id = currname1[j];
        //                        cc1.Curse_Id = timename1[j];
        //                        cc1.NewDate = DateTime.Now;
        //                        cc1.IsDelete = false;
        //                        cc1.EmployeesInfo_Id = null;
        //                        if (currname1[j] == "语文")
        //                        {
        //                            cc1.EmployeesInfo_Id = yuwen == null ? null : yuwen;
        //                        }
        //                        else if (currname1[j] == "数学")
        //                        {
        //                            cc1.EmployeesInfo_Id = shuxue == null ? null : shuxue;
        //                        }
        //                        else if (currname1[j] == "英语")
        //                        {
        //                            cc1.EmployeesInfo_Id = yingyu == null ? null : yingyu;
        //                        }
        //                        r_list.Add(cc1);
        //                    }
        //                }

        //                if (nomargin < 2)
        //                {
        //                    nomargin++;//用于判断语数英
        //                }
        //                else
        //                {
        //                    nomargin = 0;
        //                }
        //            }
        //            IsMargin = IsMargin ? false : true;
        //            time= time.AddDays(1);
        //        }

        //        if (cur.IsEndCurr == true)
        //        {
        //            Reconcile mycc = new Reconcile();
        //            mycc.AnPaiDate = time.AddDays(1);
        //            mycc.ClassRoom_Id = classroomid;
        //            mycc.ClassSchedule_Id = classNo;
        //            mycc.Curriculum_Id = "升学考试";
        //            mycc.Curse_Id = timename;
        //            mycc.NewDate = DateTime.Now;
        //            mycc.IsDelete = false;
        //            r_list.Add(mycc);
        //        }
        //    }

        //    return r_list;
        //}
        #endregion

        /// <summary>
        /// 高中生单个课程数据安排
        /// </summary>
        /// <param name="getYear"></param>
        /// <param name="curNo"></param>
        /// <param name="time"></param>
        /// <param name="timename"></param>
        /// <param name="classroomid"></param>
        /// <param name="empNo"></param>
        /// <param name="classNo"></param>
        /// <param name="grand"></param>
        /// <param name="marjion_id"></param>
        /// <returns></returns>
        public List<Reconcile> HeghtStudent_SingCurs(GetYear getYear, int curNo, DateTime time, string timename, int classroomid, string empNo, int classNo, int grand, int marjion_id)
        {
            Curriculum find_curdata = Reconcile_Com.Curriculum_Entity.GetEntity(curNo);   //获取课程


            List<Reconcile> r_list = new List<Reconcile>();


            int Sumcout = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(find_curdata.CourseCount / 4.0))); //获取总节数,不算考试在内

            for (int i = 0; i < Sumcout; i++)
            {
                if (time.Month <= getYear.EndmonthName && time.Month >= getYear.StartmonthName) //单休
                {
                    if (this.IsSaturday(time) == 2)
                    {
                        time = time.AddDays(1);
                        Sumcout++;

                        i++;
                    }
                }
                else //双休
                {

                    if (this.IsSaturday(time) == 1)
                    {
                        Sumcout = Sumcout + 2;

                        i = i + 2;
                        time = time.AddDays(2);
                    }

                }
                Reconcile new_R = new Reconcile();
                new_R.AnPaiDate = time;
                new_R.Curse_Id = timename;
                new_R.ClassRoom_Id = classroomid;
                new_R.ClassSchedule_Id = classNo;
                new_R.EmployeesInfo_Id = empNo;
                new_R.NewDate = DateTime.Now;
                new_R.IsDelete = false;
                new_R.Curriculum_Id = find_curdata.CourseName;


                r_list.Add(new_R);
                time = time.AddDays(1);


            }


            if (this.IsEndCurr(find_curdata.CourseName))
            {
                Reconcile new_R = new Reconcile();
                new_R.AnPaiDate = time;
                new_R.Curse_Id = timename;
                new_R.ClassRoom_Id = classroomid;
                new_R.ClassSchedule_Id = classNo;
                new_R.EmployeesInfo_Id = empNo;
                new_R.NewDate = DateTime.Now;
                new_R.IsDelete = false;
                new_R.Curriculum_Id = find_curdata.CourseName;
                new_R.Curriculum_Id = "升学考试";
                new_R.EmployeesInfo_Id = null;
                r_list.Add(new_R);
            }
            else
            {
                //如果是STB则不需要考试，如果是Y1、S3、S4的课程，不需要处理
                if (!find_curdata.CourseName.Contains("STB") && find_curdata.Grand_Id != 3 && find_curdata.Grand_Id != 4 && find_curdata.Grand_Id != 1002)
                {
                    Reconcile new_R = new Reconcile();
                    new_R.AnPaiDate = time;
                    new_R.Curse_Id = timename;
                    new_R.ClassRoom_Id = classroomid;
                    new_R.ClassSchedule_Id = classNo;
                    new_R.EmployeesInfo_Id = empNo;
                    new_R.NewDate = DateTime.Now;
                    new_R.IsDelete = false;
                    new_R.Curriculum_Id = find_curdata.CourseName;
                    new_R.Curriculum_Id = find_curdata.CourseName + "考试";
                    new_R.EmployeesInfo_Id = null;
                    r_list.Add(new_R);
                }

            }


            return r_list;
        }

        #region 初中生单个课程排课
        /// <summary>
        /// 初中生单个课程数据安排
        /// </summary>
        /// <param name="doublecease"></param>
        /// <param name="curNo"></param>
        /// <param name="time"></param>
        /// <param name="timename"></param>
        /// <param name="classroomid"></param>
        /// <param name="empNo"></param>
        /// <param name="classNo"></param>
        /// <param name="grandid"></param>
        /// <param name="yuwen"></param>
        /// <param name="shuxue"></param>
        /// <param name="yingyu"></param>
        /// <returns></returns>
        //public List<Reconcile> MiddleStudent_SingCurs(GetYear doublecease, int curNo, DateTime time, string timename, int classroomid, string empNo, int classNo, int grandid, string yuwen, string shuxue, string yingyu)
        //{
        //    Curriculum find_curdata = Reconcile_Com.Curriculum_Entity.GetEntity(curNo);   //获取课程



        //    List<Reconcile> r_list = new List<Reconcile>();

        //    bool IsMargin = true;//true--安排专业课，false--安排语数英

        //    string[] strcurr = new string[] { "语文,数学", "数学,英语", "英语,语文" };

        //    string[] strtimename = new string[] { "上午12节,上午34节", "下午12节,下午34节" };


        //        int Sumcout = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(find_curdata.CourseCount / 4.0))); //获取总节数
        //        int nomargin = 0;
        //        for (int i = 0; i < Sumcout; i++)
        //        {

        //            Reconcile r = new Reconcile();
        //            //判断是否是单休

        //            if (time.Month >= doublecease.StartmonthName && time.Month <= doublecease.EndmonthName)
        //            {
        //                //单休
        //                if (this.IsSaturday(time) == 2)
        //                {
        //                    //如果是周日

        //                    time = time.AddDays(1);
        //                    i++;
        //                    Sumcout++;
        //                }

        //            }
        //            else
        //            {
        //                //双休
        //                if (this.IsSaturday(time) == 1)
        //                {
        //                    //如果是周六
        //                    // r.AnPaiDate = time.AddDays(i + 2);
        //                    time = time.AddDays(2);
        //                    i = i + 2;
        //                    Sumcout = Sumcout + 2;
        //                }
        //            }
        //            r.AnPaiDate = time;
        //            r.ClassRoom_Id = classroomid;
        //            r.ClassSchedule_Id = classNo;

        //            if (IsMargin == true)
        //            {
        //                r.EmployeesInfo_Id = empNo;
        //                r.Curse_Id = timename;
        //                r.Curriculum_Id = find_curdata.CourseName;
        //                r.Curse_Id = timename;
        //                r.NewDate = DateTime.Now;
        //                r.IsDelete = false;
        //                r_list.Add(r);
        //            }
        //            else
        //            {
        //                Sumcout++;
        //                string[] currname1 = strcurr[nomargin].Split(',');

        //                //安排语数英
        //                if (timename == "上午")
        //                {
        //                    string[] timename1 = strtimename[0].Split(',');

        //                    for (int j = 0; j < timename1.Length; j++)
        //                    {
        //                        Reconcile cc1 = new Reconcile();
        //                        cc1.AnPaiDate = r.AnPaiDate;
        //                        cc1.ClassRoom_Id = classroomid;
        //                        cc1.ClassSchedule_Id = classNo;
        //                        cc1.Curriculum_Id = currname1[j];
        //                        cc1.Curse_Id = timename1[j];
        //                        cc1.NewDate = DateTime.Now;
        //                        cc1.IsDelete = false;
        //                        if (currname1[j] == "语文")
        //                        {
        //                            cc1.EmployeesInfo_Id = yuwen == null ? null : yuwen;
        //                        }
        //                        else if (currname1[j] == "数学")
        //                        {
        //                            cc1.EmployeesInfo_Id = shuxue == null ? null : shuxue;
        //                        }
        //                        else if (currname1[j] == "英语")
        //                        {
        //                            cc1.EmployeesInfo_Id = yingyu == null ? null : yingyu;
        //                        }
        //                        r_list.Add(cc1);
        //                    }

        //                }
        //                else if (timename == "下午")
        //                {
        //                    string[] timename1 = strtimename[1].Split(',');

        //                    for (int j = 0; j < timename1.Length; j++)
        //                    {
        //                        Reconcile cc1 = new Reconcile();
        //                        cc1.AnPaiDate = r.AnPaiDate;
        //                        cc1.ClassRoom_Id = classroomid;
        //                        cc1.ClassSchedule_Id = classNo;
        //                        cc1.Curriculum_Id = currname1[j];
        //                        cc1.Curse_Id = timename1[j];
        //                        cc1.NewDate = DateTime.Now;
        //                        cc1.IsDelete = false;
        //                        cc1.EmployeesInfo_Id = null;
        //                        if (currname1[j] == "语文")
        //                        {
        //                            cc1.EmployeesInfo_Id = yuwen == null ? null : yuwen;
        //                        }
        //                        else if (currname1[j] == "数学")
        //                        {
        //                            cc1.EmployeesInfo_Id = shuxue == null ? null : shuxue;
        //                        }
        //                        else if (currname1[j] == "英语")
        //                        {
        //                            cc1.EmployeesInfo_Id = yingyu == null ? null : yingyu;
        //                        }
        //                        r_list.Add(cc1);
        //                    }
        //                }

        //                if (nomargin < 2)
        //                {
        //                    nomargin++;//用于判断语数英
        //                }
        //                else
        //                {
        //                    nomargin = 0;
        //                }
        //            }
        //            IsMargin = IsMargin ? false : true;
        //            time = time.AddDays(1);
        //        }

        //        if (find_curdata.IsEndCurr == true)
        //        {
        //            Reconcile mycc = new Reconcile();
        //            mycc.AnPaiDate = time.AddDays(1);
        //            mycc.ClassRoom_Id = classroomid;
        //            mycc.ClassSchedule_Id = classNo;
        //            mycc.Curriculum_Id = "升学考试";
        //            mycc.Curse_Id = timename;
        //            mycc.NewDate = DateTime.Now;
        //            mycc.IsDelete = false;
        //            r_list.Add(mycc);
        //        }


        //    return r_list;
        //}
        #endregion
        /// <summary>
        /// 获取阶段课程
        /// </summary>
        /// <param name="grand_id"></param>
        /// <param name="Marji_id"></param>
        /// <param name="whyMarjio">true---获取专业课程，false--获取非专业课程</param>
        /// <returns></returns>
        public List<Curriculum> GetCurr(int grand_id, bool whyMarjio, int marjoi_id)
        {
            int typeid = Reconcile_Com.CourseType_Entity.FindSingeData("专业课", false).Id;
            List<Curriculum> list = Reconcile_Com.Curriculum_Entity.GetIQueryable().Where(c => c.IsDelete == false).ToList(); //获取所有有效的课程           
            List<Curriculum> find_list = new List<Curriculum>();
            if (whyMarjio)//获取专业课程
            {
                list = list.Where(c => c.CourseType_Id == typeid && c.Grand_Id == grand_id).ToList();
                //判断是否是Y1/S1
                GrandBusiness grand_entity = new GrandBusiness();
                int g_id1 = grand_entity.GetList().Where(g => g.GrandName.Equals("Y1", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Id;
                int g_id2 = grand_entity.GetList().Where(g => g.GrandName.Equals("S1", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Id;
                if (grand_id == g_id1 || grand_id == g_id2)
                {
                    find_list.AddRange(list);
                }
                else
                {
                    if (marjoi_id != 0)
                    {
                        find_list.AddRange(list.Where(c => c.MajorID == marjoi_id).ToList());//获取相关专业的课程
                    }

                    find_list.AddRange(list.Where(c => c.MajorID == null).ToList());//获取公共课程
                }

            }
            else //获取非专业课程
            {
                list = list.Where(c => c.Grand_Id == grand_id || c.Grand_Id == null).ToList();
                List<Curriculum> find = list.Where(c => c.CourseType_Id != typeid && c.MajorID == null).OrderBy(c => c.CurriculumID).ToList();
                find_list.AddRange(find);

            }
            find_list = find_list.OrderBy(c => c.Sort).ToList();
            return find_list;
        }

        /// <summary>
        /// 获取可以上职素课的所有老师(包括就业部老师)
        /// </summary>
        /// <param name="MyIsAttend">true--获取可以上班会的老师，false--获取可以上职素课的老师</param>
        /// <returns></returns>
        public List<EmployeesInfo> GetMaster_All(bool MyIsAttend)
        {
            List<EmployeesInfo> list = new List<EmployeesInfo>();
            BaseBusiness<Headmaster> headmarster_Entity = new BaseBusiness<Headmaster>();
            List<EmployeesInfo> emplist = Reconcile_Com.Employees_Entity.GetIQueryable().ToList();//获取所有员工
            List<Headmaster> headmasterlist = new List<Headmaster>();
            if (MyIsAttend)//获取可以上班会课的老师
            {
                headmasterlist = headmarster_Entity.GetIQueryable().Where(h => h.IsDelete == false).ToList(); //获取所有在职的可以上职素课的班主任
            }
            else
            {
                headmasterlist = headmarster_Entity.GetIQueryable().Where(h => h.IsDelete == false && h.IsAttend == true).ToList(); //获取所有在职的可以上职素课的班主任
            }


            foreach (Headmaster h in headmasterlist)
            {
                list.Add(emplist.Where(e => e.EmployeeId == h.informatiees_Id).FirstOrDefault());
            }
            list.AddRange(Reconcile_Com.GetObtainTeacher());//获取就业部人员
            return list;
        }
        /// <summary>
        /// 获取所有的教官
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetInstructorAll()
        {
            List<Position> plist = Reconcile_Com.PositionBusiness.GetIQueryable().Where(p => p.PositionName.Contains("教官")).ToList();//获取教官岗位Id
            List<EmployeesInfo> emplist = Reconcile_Com.Employees_Entity.GetIQueryable().ToList();
            List<EmployeesInfo> list = new List<EmployeesInfo>();
            foreach (Position item in plist)
            {
                list.AddRange(emplist.Where(e => e.PositionId == item.Pid).ToList());
            }

            return list;
        }
        /// <summary>
        /// 获取所有的英语老师
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetEnglishTeacherAll()
        {
            int pid = Reconcile_Com.PositionBusiness.GetIQueryable().Where(p => p.PositionName.Contains("英语老师")).FirstOrDefault().Pid;
            return Reconcile_Com.Employees_Entity.GetEmpByPid(pid).Where(e => e.IsDel == false).ToList();
        }

        public int Days(DateTime time)
        {
            var day = time.DayOfWeek;
            switch (day)
            {
                case DayOfWeek.Friday://星期五
                    return 5;
                case DayOfWeek.Monday://星期一
                    return 1;
                case DayOfWeek.Thursday://星期四
                    return 4;
                case DayOfWeek.Tuesday:
                    return 2;//星期二
                case DayOfWeek.Wednesday:
                    return 3;//星期三
            }
            return 0;
        }

        public List<Reconcile> AnPaiNoMarginFuntion(DateTime time, int class_id, int classroom_id, string emp_id, string timename, int curtype_id, int grand_id, int days)
        {
            List<Reconcile> list = new List<Reconcile>();
            Curriculum find_cur = Reconcile_Com.Curriculum_Entity.GetIQueryable().Where(c => c.CourseType_Id == curtype_id && c.Grand_Id == grand_id).FirstOrDefault(); //根据课程类型/阶段获取课程编号
            if (find_cur != null)
            {
                for (int i = 0; i < find_cur.CourseCount; i++)
                {
                    Reconcile new_r = new Reconcile();
                    new_r.ClassRoom_Id = classroom_id;
                    new_r.ClassSchedule_Id = class_id;
                    new_r.Curriculum_Id = find_cur.CourseName;
                    new_r.Curse_Id = timename;
                    new_r.EmployeesInfo_Id = emp_id;
                    new_r.IsDelete = false;
                    new_r.NewDate = DateTime.Now;
                    if (find_cur.CourseName.Contains("军事")) //两周一次
                    {
                        if (i == 0)
                        {
                            while (Days(time) != days)
                            {
                                time = time.AddDays(1);
                            }
                        }
                        else
                        {
                            time = time.AddDays(21);
                        }
                        new_r.AnPaiDate = time;
                    }
                    else if (find_cur.CourseName.Contains("班会") || find_cur.CourseName.Contains("英语"))//一周一次 
                    {
                        if (i == 0)
                        {
                            while (Days(time) != days)
                            {
                                time = time.AddDays(1);
                            }
                        }
                        else
                        {
                            time = time.AddDays(14);
                        }
                        new_r.AnPaiDate = time;
                    }
                    else if (find_cur.CourseName.Contains("职素"))//高中--两周一次，初中一周一次
                    {
                        Grand find_g = Reconcile_Com.Grand_Entity.GetEntity(grand_id);
                        if (find_g.GrandName.Equals("Y1", StringComparison.CurrentCultureIgnoreCase))
                        {
                            //初中
                            if (i == 0)
                            {
                                while (Days(time) != days)
                                {
                                    time = time.AddDays(1);
                                }
                            }
                            else
                            {
                                time = time.AddDays(14);
                            }
                            new_r.AnPaiDate = time;
                        }
                        else
                        {
                            //高中
                            if (i == 0)
                            {
                                while (Days(time) != days)
                                {
                                    time = time.AddDays(1);
                                }
                            }
                            else
                            {
                                time = time.AddDays(21);
                            }
                            new_r.AnPaiDate = time;
                        }
                    }
                    list.Add(new_r);
                }
            }

            return list;
        }
        /// <summary>
        /// 获取所有在职的专业课老师
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetTeacherAll()
        {
            TeacherBusiness teacher_Entity = new TeacherBusiness();
            List<EmployeesInfo> list = new List<EmployeesInfo>();
            List<Teacher> tt_list = Reconcile_Com.Teacher_Entity.GetTeachers();
            List<EmployeesInfo> ee_list = Reconcile_Com.Employees_Entity.GetIQueryable().ToList();
            foreach (Teacher t in tt_list)
            {
                list.Add(ee_list.Where(e => e.EmployeeId == t.EmployeeId).FirstOrDefault());
            }

            return list;
        }


        public AjaxResult UpdateSingleData(Reconcile reconcile)
        {
            StringBuilder ab = new StringBuilder();

            AjaxResult a = new AjaxResult();
            try
            {
                List<ReconcileView> findlist = this.SQLGetReconcileDate().Where(r => r.ClassRoom_Id == reconcile.ClassRoom_Id && r.Curse_Id == reconcile.Curse_Id && r.ClassSchedule_Id != reconcile.ClassSchedule_Id).ToList();
                if (findlist.Count > 0)
                {
                    for (int i = 0; i < findlist.Count; i++)
                    {
                        ClassSchedule findclass = Reconcile_Com.ClassSchedule_Entity.GetEntity(findlist[i].ClassSchedule_Id);
                        if (i == (findlist.Count - 1))
                        {
                            ab.Append(findclass.ClassNumber);
                        }
                        else
                        {
                            ab.Append(findclass.ClassNumber + "、");
                        }

                    }
                    ab.Append("班级已在这个教室安排了课程,请注意查看!!!");
                    a.Msg = ab.ToString();
                }
                else
                {
                    a.Msg = "没有冲突数据,修改成功!!";
                }
                a.Success = true;
                this.Update(reconcile);
                //清空缓存
                //Reconcile_Com.redisCache.RemoveCache("ReconcileList");
            }
            catch (Exception)
            {

                a.Success = false;
                a.Msg = "修改数据有误！，请刷新重试！！";
            }

            return a;
        }

        /// <summary>
        /// 获取某个日期的排课数据
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="equsls">true--获取大于并等于该日期的数据，false--获取等于该日期的数据</param>
        /// <returns></returns>
        public List<Reconcile> GetReconcileDate(DateTime date, bool equsls)
        {
            string dd = date.Year + "-" + date.Month + "-" + date.Day;
            if (equsls)
            {
                //大于等于该日期的数据
                List<Reconcile> list = this.GetListBySql<Reconcile>("select * from Reconcile where AnPaiDate>='" + dd + "'");
                return list;
            }
            else
            {
                //等于该日期的数据
                List<Reconcile> list = this.GetListBySql<Reconcile>("select * from Reconcile where AnPaiDate='" + dd + "'");
                return list;
            }

        }

        /// <summary>
        /// 获取时间段内上午所有的课，或者下午所有的课
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="curse">时段：上午、下午  上午0，下午1，上午下午2</param>
        /// <returns></returns>
        public List<Reconcile> Time_intervalDate(DateTime date, int curse)
        {
            string dd = date.Year + "-" + date.Month + "-" + date.Day;
            if (curse == 0)
            {
                List<Reconcile> list = this.GetListBySql<Reconcile>("select * from Reconcile where AnPaiDate>='" + dd + "' and curse_id like '上午%'");
                return list;
            }
            else if (curse == 1)
            {
                List<Reconcile> list = this.GetListBySql<Reconcile>("select * from Reconcile where AnPaiDate>='" + dd + "' and curse_id like '下午%'");
                return list;
            }
            else
            {
                List<Reconcile> list = this.GetListBySql<Reconcile>("select * from Reconcile where AnPaiDate>='" + dd + "'");
                return list;
            }
        }

        #endregion

        /// <summary>
        /// 获取空教室
        /// </summary>
        /// <param name="addressid">校区</param>
        /// <returns></returns>
        public List<EmtyClassroom> GetEmtyClassroom(int addressid, DateTime date, List<Classroom> classrooms)
        {
            List<EmtyClassroom> ddlit = new List<EmtyClassroom>();
            try
            {
                string mm = "select * from ReconcileView where AnPaiDate='" + date + "' and BaseData_Id=" + addressid + "";

                List<ReconcileView> reconciles = this.GetListBySql<ReconcileView>(mm);//获取某个校区已排好的数据

                foreach (Classroom it in classrooms)
                {

                    List<string> str = new List<string>() { "上午12节", "上午34节", "下午12节", "下午34节" };
                    List<ReconcileView> list = reconciles.Where(r => r.ClassRoom_Id == it.Id).ToList();
                    int count1 = list.Where(l => l.Curse_Id == "上午12节").Count();
                    if (count1 > 0)
                    {
                        str.Remove("上午12节");
                    }
                    int count2 = list.Where(l => l.Curse_Id == "下午12节").Count();
                    if (count2 > 0)
                    {
                        str.Remove("下午12节");
                    }
                    int count5 = list.Where(l => l.Curse_Id == "上午34节").Count();
                    if (count5 > 0)
                    {
                        str.Remove("上午34节");
                    }
                    int count6 = list.Where(l => l.Curse_Id == "下午34节").Count();
                    if (count6 > 0)
                    {
                        str.Remove("下午34节");
                    }
                    int count4 = list.Where(l => l.Curse_Id == "上午").Count();
                    if (count4 > 0)
                    {
                        str.Remove("上午12节");
                        str.Remove("上午34节");
                    }
                    int count3 = list.Where(l => l.Curse_Id == "下午").Count();
                    if (count3 > 0)
                    {
                        str.Remove("下午12节");
                        str.Remove("下午34节");
                    }

                    if (str.Count > 0)
                    {
                        foreach (string item in str)
                        {
                            EmtyClassroom emty = new EmtyClassroom();
                            emty.ClassroomId = it.Id;
                            emty.ClassroomName = it.ClassroomName;
                            emty.Time = item;

                            ddlit.Add(emty);
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                string m = ex.Message;
            }


            return ddlit;
        }

        #region 系统安排班级上机
        /// <summary>
        /// 自动安排自习课
        /// </summary>
        /// <param name="myclass"></param>
        /// <param name="classrooms"></param>
        /// <returns></returns>
        public List<Reconcile> SysclassStuty(List<ClassSchedule> myclass, List<EmtyClassroom> classrooms, DateTime date)
        {
            List<Reconcile> list = new List<Reconcile>();

            string str = @"select * from ReconcileView where Curriculum_Id like '语文' or Curriculum_Id like '数学' or Curriculum_Id like '英语' or
Curriculum_Id like '职素' or Curriculum_Id like '班会' or Curriculum_Id like '军事'";
            List<ReconcileView> all = this.GetListBySql<ReconcileView>(str);
            all = all.Where(a => a.AnPaiDate == date).ToList();
            List<ClassSchedule> myclass2 = new List<ClassSchedule>();
            for (int i = 0; i < myclass.Count; i++)
            {
                //判断班级是否安排了英语，数学，语文，班会，职素，军事,如果安排了，就不需要自习了       
                List<ReconcileView> finddata = all.Where(a => a.ClassSchedule_Id == myclass[i].id).ToList();
                if (finddata.Count <= 0)
                {
                    myclass2.Add(myclass[i]);
                }
            }
            int count = myclass2.Count;
            for (int i = 0; i < classrooms.Count; i++)
            {
                int j = i;
                if (j < count)
                {
                    Reconcile r = new Reconcile();
                    r.AnPaiDate = date;
                    r.ClassRoom_Id = classrooms[i].ClassroomId;
                    r.ClassSchedule_Id = myclass2[j].id;
                    r.Curriculum_Id = "自习";
                    r.Curse_Id = classrooms[i].Time;
                    r.EmployeesInfo_Id = null;
                    r.NewDate = DateTime.Now;
                    r.Rmark = null;
                    r.IsDelete = false;

                    list.Add(r);
                }
            }

            return list;
        }

        /// <summary>
        /// 去掉重复的数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<Reconcile> DeleteOrrdieData(List<Reconcile> list)
        {
            List<ReconcileView> Rlist = this.SQLGetReconcileDate();
            List<Reconcile> newdata = new List<Reconcile>();
            foreach (Reconcile item in list)
            {
                int count = Rlist.Where(r => r.AnPaiDate == item.AnPaiDate && r.ClassSchedule_Id == item.ClassSchedule_Id && r.Curriculum_Id == item.Curriculum_Id).Count();

                if (count <= 0)
                {
                    newdata.Add(item);
                }
            }

            return newdata;
        }
        #endregion

        #region 给教员值班提供的数据查询
        /// <summary>
        /// 获取这天上专业课的数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="clasid"></param>
        /// <returns></returns>
        public Reconcile Teacher_Reconfile(DateTime date, int clasid)
        {
            List<Reconcile> list = this.GetListBySql<Reconcile>("select * from Reconcile where ClassSchedule_Id=" + clasid + " and AnPaiDate='" + date + "'");
            Reconcile reconcile = null;
            foreach (Reconcile item in list)
            {
                Curriculum findata = Reconcile_Com.GetNameGetCur(item.Curriculum_Id);

                if (findata != null && findata.CourseType_Id == 1)
                {


                    return item;
                }
            }

            return reconcile;
        }

        /// <summary>
        /// 如果有考试就返回true
        /// </summary>
        /// <param name="date"></param>
        /// <param name="clasid"></param>
        /// <returns></returns>
        public bool FindCouse(DateTime date, int clasid)
        {
            List<Reconcile> list = this.GetListBySql<Reconcile>("select * from Reconcile where ClassSchedule_Id=" + clasid + " and AnPaiDate='" + date + "' and Curriculum_Id is not null");
            int cout = list.Where(l => l.Curriculum_Id.Contains("考试") || l.Curriculum_Id.Contains("语文") || l.Curriculum_Id.Contains("数学")).Count();

            return cout > 0 ? true : false;
        }


        /// <summary>
        /// S3S4阶段班级的任课老师的课时计算
        /// </summary>
        /// <param name="empid">员工id</param>
        /// <param name="currName">课程名称</param>
        /// <param name="list">排课数据集合</param>
        /// <returns></returns>
        public int S3S4_jiecount(List<Reconcile> list)
        {
            int number = 0;
            foreach (Reconcile item in list)
            {
                if (item.Curse_Id.Contains("12") || item.Curse_Id.Contains("34"))
                {
                    number += 2;
                }
                else
                {
                    number += 4;
                }
            }
            return number;
        }

        /// <summary>
        /// 获取XX教员XX年XX月XX课程的节数
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="empname">员工编号</param>
        /// <param name="currName">课程名称</param>
        /// <param name="IsCount">人数是否>10人   false 小于  true  大于</param>
        /// <returns></returns>
        public int GetTeacherClassCount(int year, int month, string empname, string currName, bool IsCount)
        {
            string sqlstr = @"select * from Reconcile where YEAR(AnPaiDate)='" + year + "' and MONTH(AnPaiDate)='" + month + "' and EmployeesInfo_Id='" + empname + "' and Curriculum_Id='" + currName + "' and IsDelete = 0";

            List<Reconcile> list = this.GetListBySql<Reconcile>(sqlstr);

            int number = 0;
            if (!IsCount)
            {
                foreach (Reconcile item in list)
                {
                    if (item.Curse_Id.Contains("12") || item.Curse_Id.Contains("34"))
                    {
                        number += 1;
                    }
                    else
                    {
                        number += 2;
                    }
                }
            }
            else
            {
                foreach (Reconcile item in list)
                {
                    if (item.Curse_Id.Contains("12") || item.Curse_Id.Contains("34"))
                    {
                        if (item.Curriculum_Id.Contains("班会"))
                        {
                            number += 1;
                        }
                        else
                        {
                            number += 2;
                        }

                    }
                    else if (item.Curse_Id == "上午" || item.Curse_Id == "下午")
                    {
                        number += 4;
                    }
                    else if (item.Curse_Id == "上午3节" || item.Curse_Id == "下午3节")
                    {
                        number += 3;
                    } else if (item.Curse_Id == "上午1节" || item.Curse_Id == "下午1节") {
                        number += 1;
                    }
                }
            }

            return number;
        }

        public int GetTeacherJieshu(int year, int month, string empname)
        {
            string sqlstr = @"select * from Reconcile where YEAR(AnPaiDate)='" + year + "' and MONTH(AnPaiDate)='" + month + "' and EmployeesInfo_Id='" + empname + "' and IsDelete = 0";

            List<Reconcile> list = this.GetListBySql<Reconcile>(sqlstr);

            int number = 0;

            foreach (Reconcile item in list)
            {
                if (item.Curse_Id.Contains("12") || item.Curse_Id.Contains("34"))
                {
                    number += 2;
                }
                else
                {
                    number += 4;
                }
            }
            return number;
        }
        #endregion

        #region 给教质提供的数据查询

        /// <summary>
        /// 用于查询XX班级是否已有XX课程
        /// </summary>
        /// <param name="class_id">班级编号</param>
        /// <param name="currname">课程名称</param>
        /// <returns></returns>
        public bool Further_education(int class_id, string currname)
        {
            string sql = "select * from  ReconcileView where ClassSchedule_Id=" + class_id + " and Curriculum_Id='" + currname + "' and AnPaiDate<='" + DateTime.Now + "'";

            int count = this.GetListBySql<ReconcileView>(sql).Count;

            return count > 0 ? true : false;
        }

        #endregion


        #region 加课

        /// <summary>
        /// 添加课程
        /// </summary>
        /// <param name="count">天数</param>
        /// <param name="date">加课日期</param>
        /// <param name="class_id">班级编号</param>
        /// <param name="year">单双休规则</param>
        /// <param name="currname">课程名称</param>
        /// <param name="classroomid">教室编号</param>
        /// <param name="curse">上课时间段</param>
        /// <param name="emp">上课老师</param>
        /// <returns></returns>
        public List<Reconcile> AddCurr(int count, DateTime date, int class_id, GetYear year, string currname, int classroomid, string curse, string emp)
        {
            List<Reconcile> datalist = new List<Reconcile>();
            if (curse.Contains("晚上"))
            {
                for (int i = 0; i < count; i++)
                {
                    Reconcile r = new Reconcile();
                    r.AnPaiDate = date;
                    if (date.Month >= year.StartmonthName && date.Month <= year.EndmonthName)
                    {
                        //单休
                        if (date.DayOfWeek == DayOfWeek.Friday)
                        {
                            //星期五
                            date = date.AddDays(3);
                        }
                        else
                        {
                            date = date.AddDays(1);
                        }
                    }
                    else
                    {
                        //双休
                        if (date.DayOfWeek == DayOfWeek.Thursday)
                        {
                            //星期四
                            date = date.AddDays(4);
                        }
                        else
                        {
                            date = date.AddDays(1);
                        }
                    }

                    r.ClassRoom_Id = classroomid;
                    r.ClassSchedule_Id = class_id;
                    r.Curse_Id = curse;
                    r.Curriculum_Id = currname;
                    r.EmployeesInfo_Id = emp;
                    r.IsDelete = false;
                    r.NewDate = DateTime.Now;

                    datalist.Add(r);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    Reconcile r = new Reconcile();
                    r.AnPaiDate = date;
                    if (date.Month >= year.StartmonthName && date.Month <= year.EndmonthName)
                    {
                        //单休
                        if (date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            //星期六
                            date = date.AddDays(2);
                        }
                        else
                        {
                            date = date.AddDays(1);
                        }
                    }
                    else
                    {
                        //双休
                        if (date.DayOfWeek == DayOfWeek.Friday)
                        {
                            //星期五
                            date = date.AddDays(3);
                        }
                        else
                        {
                            date = date.AddDays(1);
                        }
                    }

                    r.ClassRoom_Id = classroomid;
                    r.ClassSchedule_Id = class_id;
                    r.Curse_Id = curse;
                    r.Curriculum_Id = currname;
                    r.EmployeesInfo_Id = emp;
                    r.IsDelete = false;
                    r.NewDate = DateTime.Now;

                    datalist.Add(r);
                }
            }
            return datalist;
        }

        #endregion
    }
}

