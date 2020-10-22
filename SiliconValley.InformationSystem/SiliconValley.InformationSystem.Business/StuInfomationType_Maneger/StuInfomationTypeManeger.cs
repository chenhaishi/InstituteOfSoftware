using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.StuInfomationType_Maneger
{
   public class StuInfomationTypeManeger:BaseBusiness<StuInfomationType>
    {
        public StuInfomationType SerchSingleData(string id,bool IsKey)
        {
            StuInfomationType s = new StuInfomationType();
            if (IsKey)
            {
                //是主键
                int Id = Convert.ToInt32(id);
                s=this.GetEntity(Id);
            }
            else
            {
                //不是主键
                s= this.GetList().Where(ss => ss.Name == id).FirstOrDefault();
            }           
            return s;
        }

        //这个方法是用于通过名字来查询信息来源Id的
        public StuInfomationType GetNameSearchId(string name)
        {
           return this.GetList().Where(i => i.Name == name).FirstOrDefault();
            
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="new_s"></param>
        /// <returns></returns>
        public AjaxResult Add_Data(StuInfomationType new_s)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                StuInfomationType find= this.GetNameSearchId(new_s.Name);
                if (find!=null)
                {
                    a.Success = false;
                    a.Msg = "该信息来源已存在！！";
                }
                else
                {
                    a.Success = true;
                    a.Msg = "添加成功! !";
                    this.Insert(new_s);
                }
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = "数据错误，请刷新重试！！";
            }
            return a;
        }

        /// <summary>
        /// 查询班级
        /// </summary>
        /// <param name="studentID">备案id</param>
        /// <returns></returns>
        public AjaxResult select_class(int keepID)
        {
            AjaxResult list = new AjaxResult();
            BaseBusiness<ScheduleForTrainees> select_cl = new BaseBusiness<ScheduleForTrainees>();
            BaseBusiness<StudentInformation> su = new BaseBusiness<StudentInformation>();
            List<StudentInformation> student = su.GetListBySql<StudentInformation>("select * from StudentInformation where StudentPutOnRecord_Id="+keepID);
            try
            {
                if (student.Count > 0)
                {
                    //list.Success = true;
                    //list.Data = student[0];
                    List<ScheduleForTrainees> clID = select_cl.GetListBySql<ScheduleForTrainees>("select * from ScheduleForTrainees where StudentID=" + student[0].StudentNumber);
                    if (clID.Count > 0)
                    {
                        list.Success = true;
                        list.Data = clID[0].ClassID;
                    }
                }
                else
                {
                    list.Success = false;
                    list.Msg = "没有该信息";
                }
            }
            catch (Exception ex)
            {
                list.Success = false;
                list.Msg = "数据错误，请刷新重试！！";
            }
            return list;
        }
        /// <summary>
        /// 修改错误班级
        /// </summary> 
        /// <param name="keepID">备案id</param>
        /// <param name="classID">班级名</param>
        /// <param name="ClassName">班级id</param>
        /// <returns></returns>
        public AjaxResult Update_Class(int keepID, string classID, int ClassName)
        {
            AjaxResult list = new AjaxResult();
            ScheduleForTraineesBusiness update = new ScheduleForTraineesBusiness();
            BaseBusiness<StudentInformation> su = new BaseBusiness<StudentInformation>();
            BaseBusiness<ScheduleForTrainees> cl = new BaseBusiness<ScheduleForTrainees>();
            List<StudentInformation> student = su.GetListBySql<StudentInformation>("select * from StudentInformation where StudentPutOnRecord_Id=" + keepID);
            try
            {
                if (student.Count > 0)
                {
                    List<ScheduleForTrainees> clID = cl.GetListBySql<ScheduleForTrainees>("select * from ScheduleForTrainees where StudentID=" + student[0].StudentNumber);

                    if (clID.Count > 0)
                    {
                        ScheduleForTrainees old = clID[0];
                        old.ClassID = classID;
                        old.ID_ClassName = ClassName;
                        update.Update(old);
                        list.Success = true;
                    }

                }
                else
                {
                    list.Success = false;
                    list.Msg = "数据为空";
                }
            }
            catch (Exception ex)
            {
                list.Success = false;
                list.Msg = "数据错误，请刷新重试！！";
            }
            return list;

        }

    }
}
