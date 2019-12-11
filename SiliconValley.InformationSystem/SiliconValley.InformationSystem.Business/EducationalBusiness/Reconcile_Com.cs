﻿using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Employment;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{
   public class Reconcile_Com
    {
        //教室业务类
        public static readonly ClassroomManeger Classroom_Entity = new ClassroomManeger();
        //课程业务类
        public static readonly CourseBusiness Curriculum_Entity = new CourseBusiness();
        //课程类型业务类
        public static readonly CourseTypeBusiness CourseType_Entity = new CourseTypeBusiness();
        //班级业务类
        public static readonly ClassScheduleBusiness ClassSchedule_Entity = new ClassScheduleBusiness();
        //阶段业务类
        public static readonly GrandBusiness Grand_Entity = new GrandBusiness();
        //教学老师业务类
        public static readonly TeacherBusiness Teacher_Entity = new TeacherBusiness();
        //专业业务类
        public static readonly SpecialtyBusiness Specialty_Entity = new SpecialtyBusiness();
        //老师擅长课程业务类
        public static readonly GoodSkillManeger GoodSkill_Entity = new GoodSkillManeger();

        public static readonly HeadmasterBusiness Headmaster_Etity = new HeadmasterBusiness();

        public static readonly TeacherClassBusiness TeacherClass_Entity = new TeacherClassBusiness();

        public static readonly BaseBusiness<HeadClass> Hoadclass_Entity = new BaseBusiness<HeadClass>();

        public static readonly  RedisCache redisCache = new RedisCache();

        public static readonly EmployeesInfoManage Employees_Entity = new EmployeesInfoManage();

        //就业部老师
        public static readonly EmploymentStaffBusiness EmploymentStaff_Entity = new EmploymentStaffBusiness();

        /// <summary>
        /// 根据阶段名称获取阶段Id
        /// </summary>
        /// <param name="s1ors3">ture--获取S1,s2，Y1阶段,false--获取S3，s4阶段</param>
        /// <returns></returns>
        public static List<Grand> GetGrand_Id(bool s1ors3)
        {
            if (s1ors3)
            {
                //获取S1，S2,Y1
                return Grand_Entity.GetList().Where(g => g.GrandName.Equals("S1", StringComparison.CurrentCultureIgnoreCase) || g.GrandName.Equals("S2", StringComparison.CurrentCultureIgnoreCase) || g.GrandName.Equals("Y1", StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            else
            {
                //获取S3，S4
                return Grand_Entity.GetList().Where(g => g.GrandName.Equals("S3", StringComparison.CurrentCultureIgnoreCase) || g.GrandName.Equals("S4", StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
           
        }
        /// <summary>
        /// 获取XX父级下的XX子集
        /// </summary>
        /// <param name="father">父级名称</param>
        /// <param name="name">子集名称</param>
        /// <returns></returns>
        public static int GetBase_Id(string father,string name)
        {
            List<BaseDataEnum> b_list= ClassSchedule_Entity.BaseDataEnum_Entity.GetsameFartherData(father);
            return b_list.Where(b=>b.Name==name).FirstOrDefault().Id;
        }
        /// <summary>
        /// 获取带班的班主任
        /// </summary>
        /// <param name="class_id"></param>
        /// <returns></returns>
        public static AjaxResult GetZhisuTeacher(int class_id)
        {
            AjaxResult result = new AjaxResult();
            result.Success = false;            
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            HeadClass mysex = Hoadclass_Entity.GetList().Where(a => a.ClassID ==class_id && a.EndingTime==null).FirstOrDefault();
            if (mysex!=null)
            {
                Headmaster heasmaster=  Headmaster_Etity.GetEntity(mysex.LeaderID);
                if (heasmaster!=null)
                {
                    EmployeesInfo find_e= Employees_Entity.GetEntity(heasmaster.informatiees_Id);
                    if (find_e!=null)
                    {
                        result.Data = find_e.EmployeeId;
                        result.Success = true;
                    }
                }
                else
                {
                    result.Msg = "没有找到数据";
                }
            }
            else
            {
                result.Msg = "没有找到数据";
            }
            return result;
        }
        /// <summary>
        /// 获取班主任带领的班级
        /// </summary>
        /// <param name="emp_id"></param>
        /// <returns></returns>
        public static AjaxResult GetHadMasterClass(string emp_id)
        {
            AjaxResult result = new AjaxResult();
            result.Success = false;
            //根据员工Id找班主任编号
            Headmaster find_h= Headmaster_Etity.GetList().Where(h => h.informatiees_Id == emp_id).FirstOrDefault();
            //根据编号获取班主任带的班级
            List<HeadClass> hc_list= Hoadclass_Entity.GetList().Where(h => h.EndingTime == null && h.LeaderID == find_h.ID).ToList();
            if (hc_list.Count>0)
            {
                result.Success = true;
                result.Data = hc_list;
            }

            return result;
                 
        }
        /// <summary>
        /// 获取xx阶段有效的班级集合
        /// </summary>
        /// <param name="s1ors3">ture-获取S1，s2，Y1班级,false-获取S3,s4班级集合</param>
        /// <param name="grand_list">阶段集合</param>
        /// <param name="timename">上课时间段</param>
        /// <returns></returns>
        public static List<ClassSchedule> GetAppointClass(bool s1ors3,List<Grand> grand_list,int timename)
        {
            List<ClassSchedule> all = ClassSchedule_Entity.GetIQueryable().Where(c => c.IsDelete == false && c.ClassStatus == false && c.ClassstatusID == null && c.BaseDataEnum_Id==timename).ToList();
            List<ClassSchedule> new_class = new List<ClassSchedule>();
            if (s1ors3) 
            {
                //获取Y1,S1,S2班级
                foreach (Grand g_id in grand_list)
                {
                    foreach (ClassSchedule c_id in all)
                    {
                        if (c_id.grade_Id==g_id.Id)
                        {
                            new_class.Add(c_id);
                        }
                    }
                }
            }
            else
            {
                //获取S3，S4班级
                foreach (Grand g_id in grand_list)
                {
                    foreach (ClassSchedule c_id in all)
                    {
                        if (c_id.grade_Id == g_id.Id)
                        {
                            new_class.Add(c_id);
                        }
                    }
                }
            }
            return new_class;
        }
        /// <summary>
        /// 判断该班级是否是Y1班级(true--是，false-否)
        /// </summary>
        /// <param name="grand_id">阶段Id</param>
        /// <returns></returns>
        public static bool GetBrand(int grand_id)
        {
            bool s = false;
            Grand find_g= Grand_Entity.GetEntity(grand_id);
            if (find_g!=null)
            {
                if (find_g.GrandName.Equals("Y1",StringComparison.CurrentCultureIgnoreCase))
                {
                    s = true;
                }
            }
            return s;
        }
        /// <summary>
        /// 根据名称查找部门
        /// </summary>
        /// <param name="name">部门名称</param>
        /// <returns></returns>
        public static Department GetDempt(string name)
        {
            BaseBusiness<Department> baseBusiness = new BaseBusiness<Department>();
            return  baseBusiness.GetList().Where(d=>d.DeptName.Equals(name,StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }
        /// <summary>
        /// 根据部门获取某个岗位
        /// </summary>
        /// <param name="d_id"></param>
        /// <returns></returns>
        public static Position GetPsit(Department d_id,string name)
        {
            BaseBusiness<Position> baseBusiness = new BaseBusiness<Position>();
            return baseBusiness.GetList().Where(p => p.DeptId == d_id.DeptId && p.PositionName.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }
    }
}