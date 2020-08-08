using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{
    /// <summary>
    /// 用于老师晚自习值班成功安排后修改学生晚自习值班数据，学生晚自习值班数据老师改变时，用于改变老师晚自习值班数据
    /// </summary>
   public static class TeacherNightandEvningStudet
    {
          static  TeacherNightManeger teacherNight_Entity = new TeacherNightManeger();
          static EvningSelfStudyManeger evningSelfStudy_Entity = new EvningSelfStudyManeger();

        /// <summary>
        /// 当学生晚自习老师属性修改时，修改老师值班数据的方法
        /// </summary>
        /// <param name="date"></param>
        /// <param name="teacherid"></param>
        /// <param name="classid"></param>
        /// <returns></returns>
        //public static AjaxResult SetTeacherNightData(DateTime date,string teacherid,int classid,int classroomid,DateTime newdate,string timename,int newclassid)
        //{
        //    AjaxResult a = new AjaxResult();
        //    TeacherNight findata= teacherNight_Entity.GetAllTeacherNight().Where(t => t.OrwatchDate == date && t.ClassSchedule_Id==classid).FirstOrDefault();
        //    if (findata!=null)
        //    {
        //        if (teacherid!=null)
        //        {
        //            findata.Tearcher_Id = teacherid;
        //            findata.ClassRoom_id = classroomid;
        //            findata.timename = timename;
        //            findata.OrwatchDate = newdate;
        //            findata.ClassSchedule_Id = newclassid;
        //            try
        //            {
        //                teacherNight_Entity.Update(findata);
        //                teacherNight_Entity.DeleteRedis();
        //                a.Success = true;
        //                a.Msg = "成功修改数据！！";
        //            }
        //            catch (Exception)
        //            {
        //                a.Success = false;
        //                a.Msg = "修改异常，请刷新重试！！";
        //            }
        //        }
        //        else
        //        {
        //            //当teacherid为null时，说明该老师这个日期已不值班，就要删除老师的值班数据
        //            try
        //            {
        //                teacherNight_Entity.Delete(findata);
        //                teacherNight_Entity.DeleteRedis();
        //                a.Success = true;
        //                a.Msg = "成功修改数据！！";
        //            }
        //            catch (Exception)
        //            {
        //                a.Success = false;
        //                a.Msg = "删除异常，请刷新重试！！";
        //            }
                     
        //        }                                 
        //    }else if (teacherid!=null && findata==null)
        //    {
        //        //添加数据
        //        TeacherNight new_data = new TeacherNight();
        //        new_data.AttendDate = DateTime.Now;
        //        BeOnDutyManeger bentity = new BeOnDutyManeger();

        //        new_data.BeOnDuty_Id = bentity.GetSingleBeOnButy("教员晚自习", false).Id;
        //        new_data.ClassRoom_id = classroomid;
        //        new_data.ClassSchedule_Id = newclassid;
        //        new_data.OrwatchDate = newdate;
        //        new_data.timename = timename;
        //        new_data.Tearcher_Id = teacherid;
        //        new_data.IsDelete = false;
        //        new_data.AttendDate = DateTime.Now;

        //        try
        //        {
        //            teacherNight_Entity.Insert(new_data);
        //        }
        //        catch (Exception)
        //        {

        //            a.Success = false;
        //            a.Msg = "操作失败，请刷新页面重试！！";
        //        }
        //    }
        //    else
        //    {
        //        a.Msg = "未找到有效数据";
        //        a.Success = false;
        //    }
        //    return a;
        //}

        /// <summary>
        /// 当值班数据发生修改时需要修改学生晚自习数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="classid"></param>
        /// <param name="timename"></param>
        /// <param name="teacherid"></param>
        /// <returns></returns>
        public static AjaxResult SetEvningStudentData(DateTime date,int classid,string teacherid)
        {
            AjaxResult a = new AjaxResult();
 
            EvningSelfStudy findata = evningSelfStudy_Entity.GetNving(date, classid);
            if (findata!=null)
            {
                if (teacherid==null)
                {
                    //findata.emp_id = null;
                }
                else
                {
                   // findata.emp_id = teacherid;
                }
                try
                {
                    evningSelfStudy_Entity.Update(findata);
                    //evningSelfStudy_Entity.DeleteRedis();
                    a.Success = true;
                    a.Msg = "编辑成功";
                }
                catch (Exception ex)
                {
                    a.Success = false;
                    a.Msg = "编辑异常，请刷新重试！！";
                }
            }
            else
            {
                a.Msg = "未找到有效数据，请刷新重试！！";
                a.Success = false;
            }
            return a;
        }


        /// <summary>
        /// 判断这个时间是否有班级安排晚自习数据，true--可以修改老师值班数据，false--不能修改老师值班数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="classid"></param>
        /// <returns></returns>
        public static bool IsUpdateTeacherNightData(DateTime date,int classid)
        {
           EvningSelfStudyView findata= evningSelfStudy_Entity.GetAllView().Where(e => e.Anpaidate == date && e.ClassSchedule_id == classid).FirstOrDefault();
            if (findata!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// 获取这个日期中某个班级的晚自习安排数据
        /// </summary>
        /// <param name="date"></param>
        /// <param name="classid"></param>
        /// <returns></returns>
        public static List<EvningSelfStudy> GetEvningData(DateTime date,int classid)
        {
            string sql = "select * from EvningSelfStudy where AnpaiDate='"+ date + "' and ClassSchedule_id="+classid+"";
            List<EvningSelfStudy> list = evningSelfStudy_Entity.GetListBySql<EvningSelfStudy>(sql);
            // return  evningSelfStudy_Entity.GetAllView().Where(e => e.Anpaidate == date && e.ClassSchedule_id == classid).FirstOrDefault();

            return list;
        }

        /// <summary>
        /// 添加老师值班数据
        /// </summary>
        /// <param name="newdata"></param>
        /// <returns></returns>
        public static AjaxResult AddTeacherNighData(TeacherNight newdata)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                teacherNight_Entity.Insert(newdata);
                teacherNight_Entity.DeleteRedis();
                a.Msg = "添加成功！！";
                a.Success = true;
            }
            catch (Exception)
            {

                a.Success = false;
                a.Msg = "添加异常，请刷新页面重试！！！";
            }
            return a;
        }
    }
}
