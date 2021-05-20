using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.TeachingDepBusiness
{
   public class ClassTeacherBusiness: BaseBusiness<ClassTeacher>
    {
       
        public List<ClassTeacher> CandidateInfoList()
        {
            return this.GetList();
        }
        /// <summary>
        /// 教员页面带班方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ClassName"></param>
        /// <returns></returns>
        public bool HeadClassEntis(string id, string ClassName)
        {
            
            BaseBusiness<EmployeesInfo> info = new BaseBusiness<EmployeesInfo>();
            BaseBusiness<Teacher> teach = new BaseBusiness<Teacher>();
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            bool str = true;
            //List<ClassTeacher> list = new List<ClassTeacher>();
            var teachid = teach.GetList().Where(k => k.EmployeeId == id).FirstOrDefault().TeacherID;
            var mysex = this.GetList().Where(d => d.IsDel == false && d.TeacherID == teachid && d.EndDate == null ).ToList();
            if (!string.IsNullOrEmpty(ClassName))
            {
                ClassName = ClassName.Substring(0, ClassName.Length - 1);
                string[] ClassNames = ClassName.Split(',');
                foreach (var item in ClassNames)
                {
                    mysex.Remove(mysex.Where(a => a.ClassNumber == int.Parse(item)).FirstOrDefault());
                    var hoad = this.GetList().Where(c => c.IsDel == false && c.TeacherID == teachid && c.EndDate == null && c.ClassNumber.ToString() == item).FirstOrDefault();
                    
                    if (hoad == null)
                    {
                        ClassTeacher tc = new ClassTeacher();
                        tc.TeacherID = teachid;
                        tc.Skill = null;
                        tc.ClassNumber = classScheduleBusiness.FintClassSchedule(int.Parse(item)).id;
                        tc.IsDel = false;
                        tc.BeginDate = DateTime.Now;
                        tc.EndDate = null;
                        this.Insert(tc);
                    }
                }
            }
            try
            {
                List<ClassTeacher> Teacherlist = new List<ClassTeacher>();
                foreach (var item in mysex)
                {
                    item.EndDate = DateTime.Now;
                    Teacherlist.Add(item);
                }
                this.Update(Teacherlist);
                BusHelper.WriteSysLog("修改教员带班数据", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                if (Teacherlist.Count > 0)
                {
                    this.Insert(Teacherlist);
                    BusHelper.WriteSysLog("添加教员带班数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
            }
            catch (Exception ex)
            {
                str = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.系统异常);
            }
            return str;
        }
    }
}
