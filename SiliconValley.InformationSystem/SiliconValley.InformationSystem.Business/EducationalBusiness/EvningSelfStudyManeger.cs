using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{
    public class EvningSelfStudyManeger : BaseBusiness<EvningSelfStudy>
    {
        //static readonly RedisCache redisCache = new RedisCache();
        public BaseDataEnumManeger BaseDataEnum_Entity =new BaseDataEnumManeger();
        
        #region 查询数据
        /// <summary>
        /// 获取所有晚自习视图数据
        /// </summary>
        /// <returns></returns>
        public List<EvningSelfStudyView> GetAllView()
        {
            return this.GetListBySql<EvningSelfStudyView>("select * from EvningSelfStudyView");
        }

        /// <summary>
        /// 获取指定日期区间的晚自习数据
        /// </summary>
        /// <returns></returns>
        public List<EvningSelfStudy> Getdaterange(DateTime startdate, DateTime enddate)
        {
            return this.GetListBySql<EvningSelfStudy>("select * from EvningSelfStudy where Anpaidate>='" + startdate + "' and Anpaidate<='" + enddate + "'");
        }

        /// <summary>
        /// 根据Id获取晚自习数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EvningSelfStudy FindId(int id)
        {
            List<EvningSelfStudy> list = this.GetListBySql<EvningSelfStudy>("select * from EvningSelfStudy where  id=" + id);

            return list.Count > 0 ? list[0] : null;
        }

        public EvningSelfStudyView FindIdView(int id)
        {
            List<EvningSelfStudyView> list = this.GetListBySql<EvningSelfStudyView>("select * from EvningSelfStudyView where  id=" + id);

            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// 获取XX班级在XX日期段的晚自习安排
        /// </summary>
        /// <param name="starTime"></param>
        /// <param name="endTime"></param>
        /// <param name="class_id"></param>
        /// <returns></returns>
        public List<EvningSelfStudy> GetConditionEvningData(DateTime starTime, DateTime endTime, int class_id)
        {
            string sql = "select * from EvningSelfStudy where Anpaidate>='" + starTime + "' and Anpaidate<='" + endTime + "' and ClassSchedule_id=" + class_id + "";
            return this.GetListBySql<EvningSelfStudy>(sql);
        }

        /// <summary>
        /// 根据日期获取晚自习数据(true等于输入时间,false大于等于输入时间)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public List<EvningSelfStudy> GetEmpClass(DateTime date, bool s)
        {
            StringBuilder sb = new StringBuilder("select * from EvningSelfStudy where AnpaiDate='" + date + "'");

            if (s == false)
            {
                sb.Replace("select * from EvningSelfStudy where AnpaiDate='" + date + "'", "select * from EvningSelfStudy where AnpaiDate>='" + date + "'");
            }
            return this.GetListBySql<EvningSelfStudy>(sb.ToString());
        }
       
        /// <summary>
        /// 获取等于或大于这个日期的XX班级的晚自习数据
        /// </summary>
        /// <param name="time"></param>
        /// <param name="classid"></param>
        /// <returns></returns>
        public List<EvningSelfStudy> AcctoingDate(DateTime time, int classid)
        {
            string sql = "select * from EvningSelfStudy where AnpaiDate>='" + time + "' and ClassSchedule_id= " + classid + "";

            return this.GetListBySql<EvningSelfStudy>(sql);
        }
       
        /// <summary>
        /// 根据sql获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<EvningSelfStudy> GetSQLDat(string sql)
        {
           return this.GetListBySql<EvningSelfStudy>(sql);
        }

        /// <summary>
        /// 获取这个时间的晚自习安排视图数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<EvningSelfStudyView> GetTimeData(DateTime time)
        {
            return this.GetListBySql<EvningSelfStudyView>("select * from EvningSelfStudyView where AnpaiDate='" + time + "'");
        }

        /// <summary>
        /// 获取晚自习在XX教室上课的班级
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="classoom_id"></param>
        /// <returns></returns>
        public List<EvningSelfStudy> GetOnCurrClass(DateTime dateTime, int classoom_id)
        {
            string sql = "select * from EvningSelfStudy where AnpaiDate='" + dateTime + "' and Classroom_id=" + classoom_id + "";
            return this.GetListBySql<EvningSelfStudy>(sql);

        }

        /// <summary>
        /// 获取XX日期XX教室晚自习安排
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="classroom_id">教室</param>
        /// <returns></returns>
        public EvningSelfStudy GetNving(DateTime time, int class_id)
        {
            string sql = "select * from EvningSelfStudy where AnpaiDate='" + time + "' and ClassSchedule_id=" + class_id + "";
            List<EvningSelfStudy> list = this.GetListBySql<EvningSelfStudy>(sql);
            return list.Count > 0 ? list[0] : null;
        }
        
        #endregion


        #region 添加、编辑、修改数据
        /// <summary>
        /// 添加单个数据
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public AjaxResult Add_Data(EvningSelfStudy e)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                int count = GetAllView().Where(e1 => e1.Classroom_id == e.Classroom_id && e1.ClassSchedule_id == e.ClassSchedule_id && e1.curd_name == e.curd_name && e1.Anpaidate == e.Anpaidate).ToList().Count;
                if (count <= 0)
                {
                    this.Insert(e);
                    //EvningSelfStudyManeger.redisCache.RemoveCache("EvningSelfStudyList");
                    a.Success = true;
                    //if (e.emp_id != null)//如果安排的老师上课，则要去值班表添加值班信息
                    //{
                    //    TeacherNight newteachernightdata = new TeacherNight();
                    //    newteachernightdata.OrwatchDate = e.Anpaidate;
                    //    newteachernightdata.IsDelete = false;
                    //    newteachernightdata.AttendDate = DateTime.Now;
                    //    BeOnDutyManeger bb = new BeOnDutyManeger();
                    //    newteachernightdata.BeOnDuty_Id = bb.GetSingleBeOnButy("教员晚自习", false).Id;
                       
                    //    newteachernightdata.timename = e.curd_name;
                    //    newteachernightdata.ClassSchedule_Id = e.ClassSchedule_id;
                    //    newteachernightdata.ClassRoom_id = e.Classroom_id;
                    //    a = TeacherNightandEvningStudet.AddTeacherNighData(newteachernightdata);
                    //}

                }
            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;
            }
            return a;
        }

        /// <summary>
        /// 添加多个数据
        /// </summary>
        /// <param name="e_list"></param>
        ///  <param name="isjudge">true---要判断数据是否有重复，false--不需要判断数据</param>
        /// <returns></returns>
        public AjaxResult Add_Data(List<EvningSelfStudy> e_list, bool isjudge)
        {
            AjaxResult a = new AjaxResult();
            List<EvningSelfStudy> ore = new List<EvningSelfStudy>();//获取没有重复数据的集合
            try
            {
                if (isjudge == false)//不用判断数据
                {
                    this.Insert(e_list);
                    //EvningSelfStudyManeger.redisCache.RemoveCache("EvningSelfStudyList");
                    a.Success = true;
                    a.Msg = "晚自习安排成功！！！";
                }
                else
                {
                    //判断数据是否有重复值
                    int index = 0;
                    foreach (EvningSelfStudy new_e in e_list)
                    {
                        int count = GetAllView().Where(e1 => e1.Classroom_id == new_e.Classroom_id && e1.ClassSchedule_id == new_e.ClassSchedule_id && e1.curd_name == new_e.curd_name && e1.Anpaidate == new_e.Anpaidate).ToList().Count;
                        if (count <= 0)
                        {
                            ore.Add(new_e);

                        }
                        else
                        {
                            index++;
                        }
                    }
                    this.Insert(ore);
                    //EvningSelfStudyManeger.redisCache.RemoveCache("EvningSelfStudyList");
                    a.Success = true;
                    if (index == 0)
                    {
                        a.Msg = "晚自习安排成功！！！，没有发现重复数据";
                    }
                    else
                    {
                        a.Msg = "晚自习安排成功！！！，重复数据" + index + "条，已排除";
                    }
                }


            }
            catch (Exception ex)
            {
                a.Success = true;
                a.Msg = "添加数据有误，请重试！！";
            }
            return a;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="eid"></param>
        /// <returns></returns>
        public AjaxResult Delete_Data(int eid)
        {
            AjaxResult a = new AjaxResult();
            EvningSelfStudy find_e = this.GetEntity(eid);
            try
            {
                if (find_e != null)
                {
                    this.Delete(find_e);                    
                    a.Success = true;
                }
            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;
            }
            return a;
        }

        public AjaxResult Delete_Data(List<EvningSelfStudy> ids)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Delete(ids);
                a.Success = true;
                a.Msg = "操作成功！！";
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = "系统错误，请刷新重试!!!";
            }
            return a;
        }

        /// <summary>
        /// 修改 
        /// </summary>
        /// <param name="new_e"></param>
        /// <returns></returns>
        public AjaxResult Update_Data(EvningSelfStudy new_e)
        {
            AjaxResult a = new AjaxResult();
            
            try
            {
                
                this.Update(new_e);

                a.Success = true;                
            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;
            }
            return a;
        }
      
        public AjaxResult Update_Data(List<EvningSelfStudy> new_e)
        {
            AjaxResult a = new AjaxResult();
            a.Success = true;
            a.Msg = "操作成功！";
            try
            {
                this.Update(new_e);
            }
            catch (Exception)
            {
                a.Msg = "操作失败！";
                a.Success = false;
            }

            return a;
        }

        /// <summary>
        /// 晚自习安排
        /// </summary>
        /// <param name="startime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public AjaxResult GetNewEvningData(DateTime startime, DateTime endtime, bool Isemptyclass, bool Doublerest)
        {
            AjaxResult a = new AjaxResult();
            List<EmptyClass> emotylist = new List<EmptyClass>();
            ReconcileManeger Reconcile_Entity = new ReconcileManeger();
            StringBuilder msg = new StringBuilder();
            List<EvningSelfStudy> ev_list = new List<EvningSelfStudy>();
            int curtypeid = Reconcile_Com.CourseType_Entity.FindSingeData("专业课", false).Id;//获取课程类型id
            int curtypeid2 = Reconcile_Com.CourseType_Entity.FindSingeData("语文课", false).Id;
            int curtypeid3 = Reconcile_Com.CourseType_Entity.FindSingeData("数学课", false).Id;
            int curtypeid4 = Reconcile_Com.CourseType_Entity.FindSingeData("英语课", false).Id;
            List<ReconcileView> Recon_all = Reconcile_Entity.SQLGetReconcileDate().Where(r => r.AnPaiDate >= startime && r.AnPaiDate <= endtime && Reconcile_Com.GetNameGetCur(r.Curriculum_Id) != null).ToList();
            Recon_all = Recon_all.Where(r => Reconcile_Com.GetNameGetCur(r.Curriculum_Id).CourseType_Id == curtypeid || Reconcile_Com.GetNameGetCur(r.Curriculum_Id).CourseType_Id == curtypeid2 || Reconcile_Com.GetNameGetCur(r.Curriculum_Id).CourseType_Id == curtypeid3 || Reconcile_Com.GetNameGetCur(r.Curriculum_Id).CourseType_Id == curtypeid4).OrderBy(r => r.AnPaiDate).ToList();//获取这个时间段上专业课的排课数据

            Recon_all.AddRange(Reconcile_Entity.SQLGetReconcileDate().Where(r => r.AnPaiDate >= startime && r.AnPaiDate <= endtime && Reconcile_Com.GetNameGetCur(r.Curriculum_Id) == null).ToList());

            int timenameindex = 0;
            string[] timename = new string[] { "晚一", "晚二" };

            TimeSpan t = endtime - startime;
            int days = t.Days;
            for (int n = days; n >= 0; n--)
            {
                List<ClassSchedule> classSchedule_all = Reconcile_Com.GetClass();//获取所有有效的班级
                bool isok = true;
                if (Doublerest)//双休
                {
                    int studyindex = IsSaturday(startime);
                    if (studyindex == 1 || studyindex == 2)
                    {
                        isok = false;//该日期不能安排晚自习
                    }
                }
                else //单休
                {
                    int studyindex = IsSaturday(startime);
                    if (studyindex == 2)
                    {
                        isok = false;
                    }
                }

                if (isok) //这个日期合理可以安排
                {
                    EmptyClass new_e = new EmptyClass();
                    new_e.date = startime;
                    List<string> classname = new List<string>();
                    for (int i = 0; i < classSchedule_all.Count; i++)//判断这个日期这个班级是否安排了专业课
                    {
                        ReconcileView find_r = Recon_all.Where(r => r.ClassSchedule_Id == classSchedule_all[i].id && r.AnPaiDate == startime).FirstOrDefault();

                        //返回没有排课的班级
                        if (find_r == null)
                        {
                            classname.Add(classSchedule_all[i].ClassNumber);
                        }
                        else
                        {
                            if (find_r != null)//安排晚自习
                            {
                                ReconcileView find_r2 = Recon_all.Where(r => r.ClassRoom_Id == find_r.ClassRoom_Id && r.ClassSchedule_Id != find_r.ClassSchedule_Id).FirstOrDefault();
                                EvningSelfStudy new_ev = new EvningSelfStudy();
                                new_ev.Anpaidate = startime;
                                new_ev.Classroom_id = Convert.ToInt32(find_r.ClassRoom_Id);
                                new_ev.ClassSchedule_id = classSchedule_all[i].id;
                                new_ev.curd_name = timename[timenameindex];
                               
                                new_ev.IsDelete = false;
                                new_ev.Newdate = DateTime.Now;
                                ev_list.Add(new_ev);
                                if (find_r2 != null)
                                {
                                    ClassSchedule find_class = classSchedule_all.Where(c => c.id == find_r2.ClassSchedule_Id).FirstOrDefault();
                                    if (find_class != null)
                                    {
                                        classSchedule_all.Remove(find_class);
                                        EvningSelfStudy new_ev2 = new EvningSelfStudy();
                                        new_ev2.Anpaidate = startime;
                                        new_ev2.Classroom_id = Convert.ToInt32(find_r2.ClassRoom_Id);
                                        new_ev2.ClassSchedule_id = find_r2.ClassSchedule_Id;
                                        new_ev2.curd_name = new_ev.curd_name == "晚一" ? "晚二" : "晚一";
                                     
                                        new_ev2.IsDelete = false;
                                        new_ev2.Newdate = DateTime.Now;
                                        ev_list.Add(new_ev2);
                                    }

                                }
                            }
                        }
                    }
                    startime = startime.AddDays(1);
                    new_e.ClassName = classname;
                    if (classname.Count > 0)
                    {
                        emotylist.Add(new_e);
                    }
                }
                else
                {
                    startime = startime.AddDays(1);
                }
                timenameindex = timenameindex == 0 ? 1 : 0;
            }

            if (emotylist.Count > 0)
            {
                a.Data = emotylist;
                a.Success = false;//有没安排专业课的班级
            }
            else
            {
                a.Success = true;//没有
                a.Data = ev_list;
            }
            return a;
        }

        /// <summary>
        /// 阶段安排一周晚自习
        /// </summary>
        /// <param name="class_ids">班级id</param>
        /// <param name="s_time">开始日期</param>
        /// <param name="e_time">结束日期</param>
        /// <returns></returns>
        public GrandClassAnpaiEvningSelf GoodsEvningSelfStudyFunction(List<ClassSchedule> class_ids, DateTime s_time, DateTime e_time)
        {
            GrandClassAnpaiEvningSelf ResultData = new GrandClassAnpaiEvningSelf();
            List<EvningSelfStudy> evlist = new List<EvningSelfStudy>();
            ReconcileManeger reconcile = new ReconcileManeger();
            List<EmptyClass> emotylist = new List<EmptyClass>();


            List<EvningSelfStudyView> All = GetAllView();

            string time = "晚一";
            int count = (e_time - s_time).Days;
            for (int i = 0; i <= count; i++)
            {
                EmptyClass empty = new EmptyClass();
                empty.date = s_time;
                List<string> classnames = new List<string>();
                for (int j = 0; j < class_ids.Count; j++)
                {

                    List<Reconcile> list = reconcile.GetSingData(s_time, class_ids[j].id); //查看这个日期是否安排了专业课
                    string time2 = null;
                    if (list.Count > 0)
                    {
                        int classroomid = Convert.ToInt32(list[0].ClassRoom_Id);
                        List<EvningSelfStudy> find_e = GetOnCurrClass(s_time, classroomid);
                        if (find_e.Count <= 0)
                        {
                            if (evlist.Count > 0)
                            {
                                EvningSelfStudy findata = evlist.Where(e => e.Classroom_id == list[0].ClassRoom_Id && e.Anpaidate == s_time && e.ClassSchedule_id != class_ids[j].id).FirstOrDefault();
                                if (findata != null)
                                {
                                    time2 = findata.curd_name == "晚一" ? "晚二" : "晚一";
                                }
                            }

                            EvningSelfStudy new_data1 = new EvningSelfStudy();
                            new_data1.Anpaidate = s_time;
                            new_data1.Classroom_id = Convert.ToInt32(list[0].ClassRoom_Id);
                            new_data1.ClassSchedule_id = class_ids[j].id;
                            new_data1.curd_name = time2 == null ? time : time2;
                            new_data1.IsDelete = false;
                            new_data1.Newdate = DateTime.Now;

                            evlist.Add(new_data1);
                        }
                        else if (find_e.Count == 1)
                        {
                            if (find_e[0].ClassSchedule_id != class_ids[j].id)
                            {
                                EvningSelfStudy new_data1 = new EvningSelfStudy();
                                new_data1.Anpaidate = s_time;
                                new_data1.Classroom_id = Convert.ToInt32(list[0].ClassRoom_Id);
                                new_data1.ClassSchedule_id = class_ids[j].id;
                                new_data1.curd_name = find_e[0].curd_name == "晚二" ? "晚一" : "晚二";
                                new_data1.IsDelete = false;
                                new_data1.Newdate = DateTime.Now;

                                evlist.Add(new_data1);
                            }
                        }


                    }
                    else
                    {
                        string empclassname = class_ids[j].ClassNumber + ",";
                        classnames.Add(empclassname);
                    }


                }
                empty.ClassName = classnames;
                if (empty.ClassName.Count > 0)
                {
                    emotylist.Add(empty);
                }
                time = time == "晚一" ? "晚二" : "晚一";
                s_time = s_time.AddDays(1);
            }
            ResultData.emplist = emotylist;
            ResultData.evnlist = evlist;
            return ResultData;
        }

        public GrandClassAnpaiEvningSelf GoodsEvningSelfStudyFunction(List<ClassSchedule> class_ids, DateTime s_time)
        {
            ReconcileManeger Reconcile_Entity = new ReconcileManeger();
            ReconcileManeger reconcile = new ReconcileManeger();
            GrandClassAnpaiEvningSelf ResultData = new GrandClassAnpaiEvningSelf();
            List<EmptyClass> emp = new List<EmptyClass>();//存放没有安排专业课的班级
            List<EvningSelfStudy> evlist = new List<EvningSelfStudy>();

            List<EmptyClass> emotylist = new List<EmptyClass>();

            int curtypeid = Reconcile_Com.CourseType_Entity.FindSingeData("专业课", false).Id;//获取课程类型id
            int curtypeid2 = Reconcile_Com.CourseType_Entity.FindSingeData("语文课", false).Id;
            int curtypeid3 = Reconcile_Com.CourseType_Entity.FindSingeData("数学课", false).Id;
            int curtypeid4 = Reconcile_Com.CourseType_Entity.FindSingeData("英语课", false).Id;
            List<ReconcileView> Recon_all = Reconcile_Entity.SQLGetReconcileDate().Where(r => r.AnPaiDate == s_time && Reconcile_Com.GetNameGetCur(r.Curriculum_Id) != null).ToList();
            Recon_all = Recon_all.Where(r => Reconcile_Com.GetNameGetCur(r.Curriculum_Id).CourseType_Id == curtypeid || Reconcile_Com.GetNameGetCur(r.Curriculum_Id).CourseType_Id == curtypeid2 || Reconcile_Com.GetNameGetCur(r.Curriculum_Id).CourseType_Id == curtypeid3 || Reconcile_Com.GetNameGetCur(r.Curriculum_Id).CourseType_Id == curtypeid4).OrderBy(r => r.AnPaiDate).ToList();//获取这个时间段上专业课的排课数据

            Recon_all.AddRange(Reconcile_Entity.SQLGetReconcileDate().Where(r => r.AnPaiDate == s_time && Reconcile_Com.GetNameGetCur(r.Curriculum_Id) == null).ToList());

            DateTime time = s_time.AddDays(-1);
            List<EvningSelfStudy> All = GetEmpClass(time, true);
            StringBuilder sb = new StringBuilder();
            EmptyClass empty = new EmptyClass();
            empty.date = s_time;
            List<string> st = new List<string>();

            for (int i = 0; i < class_ids.Count; i++)
            {
                EvningSelfStudy find_e = All.Where(a => a.ClassSchedule_id == class_ids[i].id).FirstOrDefault();//获取前天的晚自习安排数据
                if (find_e == null)//说明昨天没有安排数据
                {
                    ReconcileView find_r = Recon_all.Where(r => r.ClassSchedule_Id == class_ids[i].id && r.AnPaiDate == s_time).FirstOrDefault();//是否安排了专业课
                    if (find_r == null)
                    {
                        sb.Append(class_ids[i].ClassNumber + ",");
                    }
                    else
                    {
                        EvningSelfStudy evning = new EvningSelfStudy(class_ids[i].id, find_r.ClassRoom_Id, "晚一", s_time, DateTime.Now, null, false);
                        evlist.Add(evning);

                        ReconcileView find_r2 = Recon_all.Where(r => r.ClassSchedule_Id != class_ids[i].id && r.AnPaiDate == s_time && r.ClassRoom_Id == find_r.ClassRoom_Id).FirstOrDefault();//同教室的班级
                        if (find_r2 != null)
                        {
                            string cu = evning.curd_name == "晚一" ? "晚二" : "晚一";
                            EvningSelfStudy evning2 = new EvningSelfStudy(find_r2.ClassSchedule_Id, find_r2.ClassRoom_Id, cu, s_time, DateTime.Now, null, false);
                            evlist.Add(evning2);

                            ClassSchedule schedule = class_ids.Where(c => c.id == find_r2.ClassSchedule_Id).FirstOrDefault();

                            if (schedule != null)
                            {
                                class_ids.Remove(schedule);
                            }
                        }

                    }
                }
                else
                {
                    //说明安排的晚自习，那么昨天是晚一，今天就算晚二
                    ReconcileView find_r = Recon_all.Where(r => r.ClassSchedule_Id == class_ids[i].id && r.AnPaiDate == s_time).FirstOrDefault();//是否安排了专业课
                    if (find_r == null)
                    {
                        sb.Append(class_ids[i].ClassNumber + ",");
                    }
                    else
                    {

                        string cur = find_e.curd_name == "晚一" ? "晚二" : "晚一";

                        EvningSelfStudy evning = new EvningSelfStudy(class_ids[i].id, find_r.ClassRoom_Id, cur, s_time, DateTime.Now, null, false);
                        evlist.Add(evning);
                        ReconcileView find_r2 = Recon_all.Where(r => r.ClassSchedule_Id != class_ids[i].id && r.AnPaiDate == s_time && r.ClassRoom_Id == find_r.ClassRoom_Id).FirstOrDefault();//同教室的班级
                        if (find_r2 != null)
                        {
                            string cu = evning.curd_name == "晚一" ? "晚二" : "晚一";
                            EvningSelfStudy evning2 = new EvningSelfStudy(find_r2.ClassSchedule_Id, find_r2.ClassRoom_Id, cu, s_time, DateTime.Now, null, false);
                            evlist.Add(evning2);

                            ClassSchedule schedule = class_ids.Where(c => c.id == find_r2.ClassSchedule_Id).FirstOrDefault();

                            if (schedule != null)
                            {
                                class_ids.Remove(schedule);
                            }
                        }
                    }
                }

            }
            st.Add(sb.ToString());
            empty.ClassName = st;
            emotylist.Add(empty);

            ResultData.evnlist = evlist;

            ResultData.emplist = emotylist;

            return ResultData;
        }

        /// <summary>
        /// 日期推迟
        /// </summary>
        /// <param name="s1ors3"></param>
        /// <param name="count"></param>
        /// <param name="starTime"></param>
        /// <returns></returns>
        public AjaxResult ALLDataADI(int count, List<EvningSelfStudy> e_list,GetYear year)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (EvningSelfStudy item in e_list)
                    {
                        DayOfWeek week = item.Anpaidate.DayOfWeek;
                        if (item.Anpaidate.Month>=year.StartmonthName && item.Anpaidate.Month<=year.EndmonthName)
                        {
                            //单休
                            if (week==DayOfWeek.Saturday)
                            {
                                item.Anpaidate = item.Anpaidate.AddDays(2);
                            }
                            else
                            {
                                item.Anpaidate = item.Anpaidate.AddDays(1);
                            }
                        }
                        else
                        {
                            //双休
                            if (week == DayOfWeek.Friday)
                            {
                                item.Anpaidate = item.Anpaidate.AddDays(3);
                            }
                            else
                            {
                                item.Anpaidate = item.Anpaidate.AddDays(1);
                            }

                        }
                         
                        this.Update(item);
                    }
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
        /// 日期提前
        /// </summary>
        /// <param name="count"></param>
        /// <param name="e_list"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public AjaxResult AccoreDataADI(int count, List<EvningSelfStudy> e_list, GetYear year)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (EvningSelfStudy item in e_list)
                    {
                        DayOfWeek week = item.Anpaidate.DayOfWeek;
                        if (item.Anpaidate.Month >= year.StartmonthName && item.Anpaidate.Month <= year.EndmonthName)
                        {
                            //单休
                            if (week == DayOfWeek.Saturday)
                            {
                                item.Anpaidate = item.Anpaidate.AddDays(-2);
                            }
                            else
                            {
                                item.Anpaidate = item.Anpaidate.AddDays(-1);
                            }
                        }
                        else
                        {
                            //双休
                            if (week == DayOfWeek.Friday)
                            {
                                item.Anpaidate = item.Anpaidate.AddDays(-3);
                            }
                            else
                            {
                                item.Anpaidate = item.Anpaidate.AddDays(-1);
                            }

                        }

                        this.Update(item);
                    }
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
        /// 上课日期调换
        /// </summary>
        /// <param name="e"></param>
        /// <param name="d1"></param>
        /// <returns></returns>
        public AjaxResult ChangDate(List<EvningSelfStudy> e, DateTime d1)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                foreach (EvningSelfStudy it in e)
                {
                    it.Anpaidate = d1;
                    this.Update(it);
                }
               // EvningSelfStudyManeger.redisCache.RemoveCache("EvningSelfStudyList");
                a.Success = true;
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = ex.Message;
            }

            return a;
        }


        #endregion
        
        
        #region 其他
        

        /// <summary>
        /// 判断XX班级是否安排了晚自习
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="class_id">班级编号</param>
        /// <returns></returns>
        public bool IsAlreadAnpai(DateTime time, int class_id)
        {
            int count = GetAllView().Where(e => e.Anpaidate == time && e.ClassSchedule_id == class_id).Count();
            return count > 0 ? true : false;
        }
                
        /// <summary>
        /// 获取空教室
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="old">false-达康维嘉校区，true--继善高科校区</param>
        /// <returns></returns>
        public ClassRoom_AddCourse GetEmptyClassroom(DateTime dateTime, bool old)
        {

            ClassRoom_AddCourse result = new ClassRoom_AddCourse();
      
            List<EvningSelfStudy> getlist = GetEmpClass(dateTime,true);
            int base_id = 0;
            if (old)
            {
                //获取继善高科校区教室
                base_id = BaseDataEnum_Entity.GetSingData("继善高科校区", false).Id;
            }
            else
            {
                base_id = BaseDataEnum_Entity.GetSingData("达嘉维康校区", false).Id;
            }
            if (base_id != 0)
            {
                List<Classroom> list_classroom1 = Reconcile_Com.Classroom_Entity.GetAddreeClassRoom(base_id);
                foreach (var item in list_classroom1)
                {
                    //ClassRoom_AddCourse
                    List<EvningSelfStudy> list_ev = getlist.Where(e => e.Classroom_id == item.Id && e.Anpaidate == dateTime).ToList();
                    int count = list_ev.Count;
                    if (count == 0)
                    {
                        result.ClassRoomId = item.Id;
                        result.TimeName = "晚一";
                        break;
                    }
                    else if (count == 1)
                    {
                        result.ClassRoomId = item.Id;
                        result.TimeName = list_ev[0].curd_name == "晚一" ? "晚二" : "晚一";
                        break;
                    }
                }
            }
            return result;
        }
        


        /// <summary>
        /// 获取单休月份数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="XmlFile_url"></param>
        /// <returns></returns>
        public GetYear MyGetYear(string year, string XmlFile_url)
        {
            ReconcileManeger Reconcile_Entity = new ReconcileManeger();
            return Reconcile_Entity.MyGetYear(year, XmlFile_url);
        }

        /// <summary>
        /// 判断该日期是否是周六或周末（1--周六，2--周日）
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int IsSaturday(DateTime date)
        {
            ReconcileManeger Reconcile_Entity = new ReconcileManeger();
            return Reconcile_Entity.IsSaturday(date);
        }

        /// <summary>
        /// 获取空教室
        /// </summary>
        /// <param name="timename">时间</param>
        /// <param name="time">日期</param>
        /// <param name="shcooladrees_id">校区ID</param>
        /// <returns></returns>
        public List<Classroom> GetEmptyClassrooms(string timename, DateTime time, int shcooladrees_id)
        {
            var classroomid = this.GetAllView().Where(e => e.Anpaidate == time && e.curd_name == timename).ToList().Select(e => e.Classroom_id).ToList();//获取被安排的教室id

            List<Classroom> list = Reconcile_Com.Classroom_Entity.GetAddreeClassRoom(shcooladrees_id);//获取校区的有效教室

            foreach (var id in classroomid)
            {
                int cid = Convert.ToInt32(id);
                Classroom find = list.Where(c => c.Id == cid).FirstOrDefault();
                if (find != null)
                {
                    list.Remove(find);
                }
            }

            return list;
        }
    

        /// <summary>
        /// 获取提示用户的班级
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<AnPaiData> GetClassname(string[] ids)
        {
            List<AnPaiData> a_list = new List<AnPaiData>();
            foreach (string id in ids)
            {
                int myid = Convert.ToInt32(id);
                EvningSelfStudy find= this.GetEntity(myid);
                AnPaiData a = new AnPaiData();
                a.R_Id = find.id.ToString();
                a.ClassName = Reconcile_Com.ClassSchedule_Entity.GetEntity(find.ClassSchedule_id).ClassNumber;
                a_list.Add(a);
            }

            return a_list;
        }


        #endregion

    }
}
