using SiliconValley.InformationSystem.Business.ClassesBusiness;
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

        public static readonly BaseBusiness<Position> PositionBusiness = new BaseBusiness<Position>();

        public static readonly BaseBusiness<Department> DeparmentBusiness = new BaseBusiness<Department>();

        /// <summary>
        /// 获取所有有效的阶段
        /// </summary>    
        /// <returns></returns>
        public static List<Grand> GetGrand_Id()
        {

            return Grand_Entity.GetList().Where(g => g.IsDelete == false).ToList();
           
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
                        result.Data = find_e;
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
            return GetAll_Department().Where(d=>d.DeptName.Equals(name,StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }
        /// <summary>
        /// 根据部门获取某个岗位
        /// </summary>
        /// <param name="d_id"></param>
        /// <returns></returns>
        public static Position GetPsit(Department d_id,string name)
        {            
            return GetAll_Postion().Where(p => p.DeptId == d_id.DeptId && p.PositionName.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
        }        
        /// <summary>
        /// 获取未辞职的所有员工
        /// </summary>
        /// <returns></returns>
        public static List<EmployeesInfo> GetAllNoGoingEMP()
        {
           return Employees_Entity.GetList().Where(e => e.IsDel == false).ToList();
        }
        /// <summary>
        /// 获取就业部未辞职的老师
        /// </summary>
        /// <returns></returns>
        public static List<EmployeesInfo> GetObtainTeacher()
        {
            List<EmployeesInfo> e_list = GetAllNoGoingEMP();
            List<EmploymentStaff> f_list = EmploymentStaff_Entity.GetALl();//获取就业部的所有员工
            List<EmployeesInfo> list = new List<EmployeesInfo>();
            foreach (EmploymentStaff item1 in f_list)
            {
                EmployeesInfo find= e_list.Where(e => e.EmployeeId == item1.EmployeesInfo_Id).FirstOrDefault();
                if (find!=null)
                {
                    list.Add(find);
                }
                 
            }
            return list;
        }
        /// <summary>
        /// 获取所有有效的部门
        /// </summary>
        /// <returns></returns>
        public static List<Department> GetAll_Department()
        {
           return DeparmentBusiness.GetList().Where(d => d.IsDel == false).ToList();
        }
        /// <summary>
        /// 获取所有有效的岗位
        /// </summary>
        /// <returns></returns>
        public static List<Position> GetAll_Postion()
        {
           return  PositionBusiness.GetList().Where(p => p.IsDel == false).ToList();
        }
         
        /// <summary>
        /// 获取员工姓名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetEmpName(string id)
        {
           return Employees_Entity.GetEntity(id) == null ? "" : Employees_Entity.GetEntity(id).EmpName;
        }
        /// <summary>
        /// 获取班级名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetClassName(int id)
        {
           return ClassSchedule_Entity.GetEntity(id) == null ? "" : ClassSchedule_Entity.GetEntity(id).ClassNumber;
        }
        /// <summary>
        /// 获取所有有效班级
        /// </summary>
        /// <returns></returns>
        public static List<ClassSchedule> GetClass()
        {
            List<ClassSchedule> list = new List<ClassSchedule>();
            list= Reconcile_Com.ClassSchedule_Entity.ClassList();
            return list;
        }
        /// <summary>
        /// 获取这个阶段的下面所有课程
        /// </summary>
        /// <param name="find_c"></param>
        /// <returns></returns>
        public static List<Curriculum> GetbehindCurri(Curriculum find_c)
        {
            CourseType courseType= CourseType_Entity.GetList().Where(ct => ct.TypeName.Contains("专业")).FirstOrDefault();
           return Curriculum_Entity.GetList().Where(c =>c.IsDelete==false && c.Grand_Id == find_c.Grand_Id && c.Sort >= find_c.Sort &&c.CourseType_Id== courseType.Id).OrderBy(c=>c.CurriculumID).ToList();
        }
        /// <summary>
        /// 根据课程名称获取课程数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Curriculum GetNameGetCur(string name)
        {
           return Curriculum_Entity.GetList().Where(c => c.CourseName == name).FirstOrDefault();
        }

        /// <summary>
        /// 判断这个班级应该去达康维嘉校区上晚自习还是继善高科校区上晚自习 (true--继善高科校区,fale--达康维嘉校区)
        /// </summary>
        /// <param name="class_id"></param>
        /// <returns></returns>
        public static bool judgeClass(int class_id)
        {
            ClassSchedule find = ClassSchedule_Entity.GetEntity(class_id);
            List<Grand> grand1 = GetGrand_Id().Where(g => g.Id == find.grade_Id).ToList();
            if (grand1.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 通过课程名称获取课程数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Curriculum findname(string name)
        {
           return Curriculum_Entity.GetList().Where(c => c.CourseName == name).FirstOrDefault();
        }
 
        /// <summary>
        /// 修改授课班级数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="teacherid"></param>
        /// <returns></returns>
       
        
        #region
        //public static bool UpdateTeacherClassData(DateTime date,int teacherid,int class_id,int )
        //{
        //    ClassTeacher classTeacher=  TeacherClass_Entity.GetList().Where(t => t.ClassNumber == class_id && t.IsDel == false).FirstOrDefault();//获取授课数据

        //    classTeacher.EndDate = date;
        //    classTeacher.IsDel = true;
        //    try
        //    {
        //        TeacherClass_Entity.Update(classTeacher);

        //        ClassTeacher newdata = new ClassTeacher();
        //        newdata.BeginDate = date;
        //        newdata.IsDel = false;
        //        newdata.ClassNumber = class_id;
        //        newdata.Skill = 1;


        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }



        //}

        #endregion
    }
}
