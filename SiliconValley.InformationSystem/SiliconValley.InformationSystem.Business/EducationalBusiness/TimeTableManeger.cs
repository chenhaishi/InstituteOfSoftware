using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{
    /// <summary>
    /// 显示课表数据的业务类
    /// </summary>
   public class TimeTableManeger 
    {
        public ReconcileManeger Reconcile_Entity;
        public EvningSelfStudyManeger EvningSelfStudy_Entity;
        public TeacherNightManeger Teacher_Entity = new TeacherNightManeger();
        
        /// <summary>
        /// 获取排课数据(显示课表形式)
        /// </summary>
        /// <param name="time">日期</param>
        /// <param name="timename">上课时间段</param>
        /// <param name="classrooms">教室集合</param>
        /// <returns></returns>
        public List<AnPaiData> GetPaiDatas(DateTime time, string timename, List<Classroom> classrooms)
        {
            List<AnPaiData> a_list = new List<AnPaiData>();
            Reconcile_Entity = new ReconcileManeger();
            List<ReconcileView> r_list = Reconcile_Entity.GetReconciles(time);//获取这个日期的排课数据
            EvningSelfStudy_Entity = new EvningSelfStudyManeger();
            List<EvningSelfStudyView> e_list = EvningSelfStudy_Entity.GetTimeData(time);//获取这个日期的晚自习安排数据
            string timenames = timename.Substring(0, 2);
            if (timename == "上午12节" || timename == "上午34节" || timename == "下午12节" || timename == "下午34节")
            {
                foreach (Classroom c in classrooms)
                {
                     List<ReconcileView> finddata=  r_list.Where(r => (r.Curse_Id == timename || r.Curse_Id == timenames) && r.ClassRoom_Id == c.Id).ToList();
                     AnPaiData new_a = new AnPaiData();
                    if (finddata.Count==1)
                    {
                      
                        new_a.R_Id = finddata[0].Id.ToString();
                        new_a.ClassName = Reconcile_Com.ClassSchedule_Entity.GetEntity(finddata[0].ClassSchedule_Id).ClassNumber;
                        new_a.Teacher = finddata[0].EmployeesInfo_Id == null ? "无" : Reconcile_Com.Employees_Entity.GetEntity(finddata[0].EmployeesInfo_Id).EmpName;
                        new_a.NeiRong = finddata[0].Curriculum_Id;
                        a_list.Add(new_a);
                    }else if (finddata.Count>1)
                    {
                        List<string> sbRid = new List<string>();
                        List<string> sbclassname = new List<string>();
                        List<string> sbTeacher = new List<string>();
                        List<string> sbNeiRong = new List<string>();
                        StringBuilder idsb = new StringBuilder();
                        StringBuilder classsb = new StringBuilder();
                        StringBuilder teachersb = new StringBuilder();
                        StringBuilder neirongsb = new StringBuilder();
                        //int index = 0;
                        foreach (ReconcileView rr in finddata)
                        {
                            int Idcount= sbRid.Where(r => r == rr.Id.ToString()).Count();
                            if (Idcount<=0)
                            {
                                sbRid.Add(rr.Id.ToString());
                            }

                            int classcount = sbclassname.Where(cl => cl == rr.ClassNumber).Count();
                            if (classcount<=0)
                            {
                                sbclassname.Add(rr.ClassNumber);
                            }
                            string emp = rr.EmpName;
                            int teachercount= sbTeacher.Where(t => t ==emp).Count();
                            if (teachercount <= 0)
                            {                               
                                sbTeacher.Add(emp);
                            }

                            int nrcount= sbNeiRong.Where(n => n == rr.Curriculum_Id).Count();

                            if (nrcount <= 0)
                            {
                                sbNeiRong.Add(rr.Curriculum_Id);
                            }
                             
                        }
                         
                        for (int i = sbRid.Count-1; i>=0; i--)
                        {
                            if (i==0)
                            {
                                idsb.Append(sbRid[i]);
                            }
                            else
                            {
                                idsb.Append(sbRid[i]+",");
                            }
                        }
                        
                        for (int i = sbclassname.Count-1; i >=0; i--)
                        {
                            if (i == 0)
                            {
                                classsb.Append(sbclassname[i]);
                            }
                            else
                            {
                                classsb.Append(sbclassname[i] + ",");
                            }
                        }
                      
                        for (int i = sbTeacher.Count-1; i >= 0; i--)
                        {
                            if (i == 0)
                            {
                                teachersb.Append(sbTeacher[i]);
                            }
                            else
                            {
                                teachersb.Append(sbTeacher[i] + ",");
                            }
                        }
                        
                         for (int i = sbNeiRong.Count-1; i >= 0; i--)
                        {
                            if (i == 0)
                            {
                                neirongsb.Append(sbNeiRong[i]);
                            }
                            else
                            {
                                neirongsb.Append(sbNeiRong[i] + ",");
                            }
                        }
                        new_a.R_Id = idsb.ToString();
                        new_a.ClassName = classsb.ToString();
                        new_a.Teacher = teachersb.ToString();
                        new_a.NeiRong = neirongsb.ToString();
                        a_list.Add(new_a);
                    }
                    else
                    {
                        a_list.Add(new_a);
                    }    
                }        
            }            
             
            else if (timename == "晚一" || timename == "晚二")
            {
                foreach (Classroom c in classrooms)
                {
                    List<EvningSelfStudyView> finddata= e_list.Where(e => e.curd_name == timename && e.Classroom_id==c.Id).ToList();
                    AnPaiData a = new AnPaiData();
                    if (finddata.Count==1)
                    {                        
                        a.R_Id = finddata[0].id.ToString();
                        a.ClassName =Reconcile_Com.ClassSchedule_Entity.GetEntity( finddata[0].ClassSchedule_id).ClassNumber;
                        a.Teacher = finddata[0].EmpName;
                        a_list.Add(a);
                    }
                    else if(finddata.Count >1)
                    {
                        StringBuilder sbrid = new StringBuilder();
                        StringBuilder sbclassname = new StringBuilder();
                        StringBuilder sbteacher = new StringBuilder();
                        List<string> rid = new List<string>();
                        List<string> classname = new List<string>();
                        List<string> teacher = new List<string>();
                        //int index = 0;
                        foreach (EvningSelfStudyView item in finddata)
                        {
                            int ridcount = rid.Where(r => r == item.id.ToString()).Count();
                            if (ridcount<=0)
                            {
                                rid.Add(item.id.ToString());
                            }

                            int classnamecount = classname.Where(cl => cl == item.ClassNumber).Count();

                            if (classnamecount<=0)
                            {
                                classname.Add(item.ClassNumber);
                            }
                            string emp = item.EmpName;
                            int teachercount = classname.Where(t => t == emp).Count();
                            if (teachercount<=0)
                            {
                                teacher.Add(emp);
                            }                             
                        }

                        for (int i = (rid.Count-1); i >=0 ; i--)
                        {
                            if (i==0)
                            {
                                sbrid.Append(rid[i].ToString());
                            }
                            else
                            {
                                sbrid.Append(rid[i].ToString()+",");
                            }
                        }

                        for (int i = (classname.Count-1); i >=0; i--)
                        {
                            if (i==0)
                            {
                                sbclassname.Append(classname[i]);
                            }
                            else
                            {
                                sbclassname.Append(classname[i]+",");
                            }
                        }

                        for (int i = (teacher.Count-1); i >=0; i--)
                        {
                            if (i==0)
                            {
                                sbteacher.Append(teacher[i]);
                            }
                            else
                            {
                                sbteacher.Append(teacher[i]+",");
                            }
                        }
                        a.R_Id = sbrid.ToString();
                        a.ClassName = sbclassname.ToString();
                        a.Teacher = sbteacher.ToString();
                        a_list.Add(a);
                    }
                    else
                    {
                        a_list.Add(a);
                    }
                }
            }
            return a_list;
        }

        /// <summary>
        /// 根据排课id获取班级排课信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<AnPaiData> GetClassName(string[] id)
        {
            List<AnPaiData> list = new List<AnPaiData>();
            Reconcile_Entity = new ReconcileManeger();
            foreach (string item in id)
            {
                AnPaiData a = new AnPaiData();
                int number_id=  Convert.ToInt32(item);
               Reconcile finddata= Reconcile_Entity.GetEntity(number_id);
                a.R_Id = item;
                a.ClassName = Reconcile_Com.ClassSchedule_Entity.GetEntity(finddata.ClassSchedule_Id).ClassNumber;
                list.Add(a);
            }

            return list;
        }

        /// <summary>
        /// 根据晚自习id获取班级数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<AnPaiData> GetEvningClassName(string[] id)
        {
            EvningSelfStudy_Entity = new EvningSelfStudyManeger();
            return   EvningSelfStudy_Entity.GetClassname(id);
        }
    }
}
