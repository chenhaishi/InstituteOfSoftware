﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Business.StuSatae_Maneger;

namespace SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness
{
   public class StudentDataKeepAndRecordBusiness: BaseBusiness<StudentPutOnRecord>
    {
        StuStateManeger Statu_Entity = new StuStateManeger();

        /// <summary>
        /// 这是一个获取报名学生的方法
        /// </summary>
        /// <param name="EmpyId">员工编号</param>
        /// <returns></returns>
        public List<StudentPutOnRecord> GetrReport(string EmpyId)
        {
            //根据员工获取报名的数据
          return  this.GetList().Where(s => s.StuStatus_Id == 2 && s.EmployeesInfo_Id == EmpyId).ToList();
        }
        /// <summary>
        /// 获取某一年的每个月的上门量
        /// </summary>
        /// <param name="YeanName"></param>
        /// <returns></returns>
        public int[] GetYearGotoCount(DateTime YeanName)
        {
           List<StudentPutOnRecord> student_data= this.GetList().Where(s => s.StuIsGoto == true && s.StuVisit <= YeanName).ToList();//获取匹配的数据
           //拿到一月到12的人数


            return new int[12];
        }
        /// <summary>
        /// 这个方法是在学生报名之后就修改学生状态的方法
        /// </summary>
        /// <param name="id">备案id</param>
        /// <returns></returns>
        public bool ChangeStudentState(int id)
        {
            StudentPutOnRecord find_s= this.GetEntity(id);
            if (find_s!=null)
            {
               StuStatus find_statu= Statu_Entity.GetStu("已报名");
                if (find_statu!=null)
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
                return false;
            }
        }

        public int GetMonthCount(List<StudentPutOnRecord> student_list,int monthName)
        {
            //var count = student_list.Where(s=>s)
            return 0;
        }
    }
}
