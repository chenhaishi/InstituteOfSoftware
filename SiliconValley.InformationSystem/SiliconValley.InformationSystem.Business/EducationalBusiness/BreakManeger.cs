﻿using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{
   public class BreakManeger:BaseBusiness<Break>
    {         
        public static readonly ClassroomManeger Classroom_Entity = new ClassroomManeger();
        public static readonly EmployeesInfoManage Employee_Entity = new EmployeesInfoManage();
        public static readonly GrandBusiness Grand_Entity = new GrandBusiness();
        public static readonly BaseDataEnumManeger BaseDataEnum_Entity = new BaseDataEnumManeger();
        public static readonly ClassScheduleBusiness classSchedule_Entity = new ClassScheduleBusiness();

         /// <summary>
         /// 根据名称或主键查询单条员工数据
         /// </summary>
         /// <param name="name">名称或Id</param>
         /// <param name="key">true--按主键查询,false-- 按名称查询</param>
         /// <returns></returns>
        public EmployeesInfo GetEmploySingData(string name,bool key)
        {
            EmployeesInfo new_e = new EmployeesInfo();
            if (key)
            {                
                new_e= Employee_Entity.GetEntity(name);
            }
            else
            {
                new_e = Employee_Entity.GetList().Where(e => e.EmpName == name).FirstOrDefault();
            }

            return new_e;
        }
         /// <summary>
         /// 获取所有有效的班级数据
         /// </summary>
         /// <returns></returns>
        public List<ClassSchedule> GetClassSchedules()
        {
            return classSchedule_Entity.GetList().Where(c => c.IsDelete == false && c.ClassStatus == false).ToList();
        }
        /// <summary>
        /// 获取有效的教室数据
        /// </summary>
        /// <returns></returns>
        public List<Classroom> GetClassRoomData()
        {
            return Classroom_Entity.GetList().Where(c => c.IsDelete == false).ToList();
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

        public AjaxResult Addlist(List<Break> list)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Insert(list);
                a.Success = true;
                a.Msg = "操作成功！";
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = "系统异常，请刷新重试！";
            }

            return a;
        }
    }
}
