using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Employment;

namespace SiliconValley.InformationSystem.Business.EducationalBusiness
{
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;

    public class TeacherNightManeger : BaseBusiness<TeacherNight>
    {
        RedisCache Redis = new RedisCache();
        BeOnDutyManeger BeOnDuty_Entity;
        public EvningSelfStudyManeger EvningSelfStudent_Entity = new EvningSelfStudyManeger();
        public EmployeesInfoManage Employee = new EmployeesInfoManage();
        ReconcileManeger reconcile_Entity = new ReconcileManeger();


        /// <summary>
        /// 根据Id获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TeacherNight FindId(int id)
        {
           List<TeacherNight> list= this.GetListBySql<TeacherNight>("select * from TeacherNight where Id=" + id);
            return list.Count > 0 ? list[0] : null;
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public List<TeacherNight> GetAllTeacherNight()
        {
            Redis = new RedisCache();
            Redis.RemoveCache("TeacherNight");
            List<TeacherNight> teacherNights = new List<TeacherNight>();
            teacherNights = Redis.GetCache<List<TeacherNight>>("TeacherNight");

            if (teacherNights == null || teacherNights.Count <= 0)
            {
                teacherNights = this.GetIQueryable().ToList();
                Redis.SetCache("TeacherNight", teacherNights);
            }
            return teacherNights;
        }
  

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="new_t"></param>
        /// <returns></returns>
        public AjaxResult Add_data(TeacherNight new_t)
        {

            AjaxResult a = new AjaxResult();
            try
            {
                this.Insert(new_t);
                a.Success = true;
                a.Msg = "操作成功！";
                Redis.RemoveCache("TeacherNight");
            }
            catch (Exception ex)
            {
                a.Msg = "操作失败！";
                a.Success = false;
            }

            return a;
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void DeleteRedis()
        {
            Redis.RemoveCache("TeacherNight");
        }

        public AjaxResult Add_data(List<TeacherNight> new_t)
        {
            int data_override = 0;

            AjaxResult a = new AjaxResult();
            try
            {
                foreach (TeacherNight new_data in new_t)
                {
                    //判断是否有重复数据
                    int cout = this.GetList().Where(all => all.Tearcher_Id == new_data.Tearcher_Id&& all.OrwatchDate == new_data.OrwatchDate).ToList().Count;
                    if (cout > 0)
                    {
                        data_override++;
                    }
                    else
                    {
                        this.Insert(new_data);
                        
                    }

                }
                a.Success = true;
                if (data_override != 0)
                {
                    a.Msg = "有重复数据" + data_override + "条，系统自动处理完毕！";
                }
                else
                {
                    a.Msg = "无重复数据，成功安排！";
                }
                Redis.RemoveCache("TeacherNight");
            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;
            }

            return a;
        }

        /// <summary>
        /// 判断XX日期XX时间XX班级XX老师是否值班
        /// </summary>
        /// <returns></returns>
        public AjaxResult Exits(DateTime date,string curname,int class_id)
        {
           int count= this.GetListBySql<TeacherNight>("select * from TeacherNight where ClassSchedule_Id=" + class_id + " and OrwatchDate='" + date + "' and timename='" + curname + "'").Count;
            AjaxResult a = new AjaxResult() { Success=false};
            if (count>0)
            {
                a.Success = true;
            }
            return a;
        }

        /// <summary>
        /// 删除值班数据
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public AjaxResult My_Delete(int Id)
        {
            //找到数据
            TeacherNight find_data_del = this.GetAllTeacherNight().Where(al => al.Id == Id).FirstOrDefault();
            //删除
            AjaxResult a = new AjaxResult();
            try
            {
                this.Delete(find_data_del);
                a.Success = true;
                a.Msg = "删除成功！";
                Redis.RemoveCache("TeacherNight");
            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;

            }

            return a;
        }

        public AjaxResult My_Delete(List<TeacherNight> list)
        {
            //删除
            AjaxResult a = new AjaxResult();
            try
            {
                this.Delete(list);
                a.Success = true;
                a.Msg = "删除成功！";
                Redis.RemoveCache("TeacherNight");
            }
            catch (Exception ex)
            {
                a.Msg = ex.Message;
                a.Success = false;

            }

            return a;
        }
        ///// <summary>
        ///// 获取空教室
        ///// </summary>
        ///// <param name="timename">上课时间</param>
        ///// <param name="time">安排日期</param>
        ///// <param name="schooladdress_id">所属校区</param>
        ///// <returns></returns>
        //public List<Classroom> GetEmptyClassroom(string timename, DateTime time, int schooladdress_id)
        //{
        //    ClassroomManeger classroom_Entity = new ClassroomManeger();
        //    List<Classroom> roomlist = classroom_Entity.GetAddreeClassRoom(schooladdress_id);//获取某个校区的所有有效教室

        //    List<EvningSelfStudy> e_list = EvningSelfStudent_Entity.EvningSelfStudyGetAll().Where(e => e.Anpaidate == time).ToList(); //获取符合日期的晚自习安排

        //    foreach (EvningSelfStudy item in e_list)
        //    {
        //        Classroom find = roomlist.Where(c => c.Id == item.Classroom_id).FirstOrDefault();//获取没有安排的空教室
        //        if (find != null)
        //        {
        //            roomlist.Remove(find);
        //        }
        //    }

        //    return roomlist;
        //}

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="new_t"></param>
        /// <returns></returns>
        public AjaxResult Edit_Data(List<TeacherNight> new_t)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Update(new_t);
                a.Success = true;
                a.Msg = "编辑成功！！";
                Redis.RemoveCache("TeacherNight");
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = "数据编辑有误，请刷新重试！！";
            }
            return a;
        }

        public AjaxResult Edit_Data(TeacherNight new_t)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                this.Update(new_t);
                a.Success = true;
                a.Msg = "编辑成功！！";
                Redis.RemoveCache("TeacherNight");
            }
            catch (Exception)
            {
                a.Success = false;
                a.Msg = "数据编辑有误，请刷新重试！！";
            }
            return a;
        }
        /// <summary>
        /// 更改日期
        /// </summary>
        /// <returns></returns>
        public AjaxResult Update_Date(bool ischangdate, List<TeacherNight> updata_data, int count, DateTime newdate)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                if (ischangdate)//调课
                {
                    foreach (TeacherNight t in updata_data)
                    {
                        t.OrwatchDate = t.OrwatchDate.AddDays(count);
                        this.Update(t);
                    }
                }
                else //日期调换
                {
                    foreach (TeacherNight t in updata_data)
                    {
                        t.OrwatchDate = newdate;
                        this.Update(t);
                    }
                }
                a.Success = true;
                a.Msg = "操作成功！！！";
                Redis.RemoveCache("TeacherNight");
            }
            catch (Exception ex)
            {
                a.Success = false;
                a.Msg = "数据错误，请重试！！！";
                throw;
            }

            return a;
        }

        public AjaxResult Add_masterdata(List<TeacherNight> list)
        {
            AjaxResult a = new AjaxResult();
            try
            {
                foreach (TeacherNight i in list)
                {
                    int overridecount = this.GetIQueryable().Where(t => t.Tearcher_Id == i.Tearcher_Id && t.OrwatchDate == i.OrwatchDate).ToList().Count;
                    if (overridecount <= 0)
                    {
                        this.Insert(i);
                    }
                }
                a.Success = true;
                Redis.RemoveCache("TeacherNight");
                a.Msg = "安排成功！！！";
            }
            catch (Exception ex)
            {
                a.Msg = "数据异常！！,请刷新重试";
                a.Success = false;
            }

            return a;
        }

        /// <summary>
        /// 获取就业部跟班主任老师
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GEThEADmASTER()
        {
            HeadmasterBusiness headmaster = new HeadmasterBusiness();


            EmploymentStaffBusiness employmentStaff = new EmploymentStaffBusiness();

            List<EmploymentStaff> list2 = employmentStaff.GetIQueryable().ToList();

            EmployeesInfoManage emp = new EmployeesInfoManage();

            List<EmployeesInfo> result = new List<EmployeesInfo>();

            List<Headmaster> list = headmaster.GetIQueryable().ToList();//获取所有班主任
            list.ForEach(h =>
            {
                EmployeesInfo e = emp.GetEntity(h.informatiees_Id);
                if (e != null)
                {
                    result.Add(e);
                }
            }
            );

            list2.ForEach(f => {
                EmployeesInfo e = emp.GetEntity(f.EmployeesInfo_Id);
                if (e != null)
                {
                    result.Add(e);
                }
            });

            return result;
        }

        /// <summary>
        /// 是否显示所有数据0--数据全部显示，1--教学主任或副主任,2--教质主任或副主任,3--就业主任或副主任,4--就业班主任/班主任/XX老师
        /// </summary>
        /// <returns></returns>
        public int IsShowData(string emp)
        {
            int i = 0;
            EmployeesInfoManage emp_Entity = new EmployeesInfoManage();
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            Position position = emp_Entity.GetPositionByEmpid(UserName.EmpNumber);
            if (position.PositionName == "教学副主任" || position.PositionName == "教学主任")
            {
                //说明
                i = 1;
            } else if (position.PositionName == "教质主任" || position.PositionName == "教质副主任")
            {
                i = 2;
            } else if (position.PositionName == "就业主任" || position.PositionName == "就业副主任")
            {
                i = 3;
            }
            else if (position.PositionName == "就业班主任" || position.PositionName == "班主任" || position.PositionName.Contains("老师"))
            {
                i = 4;
            }

            return i;
        }


        /// <summary>
        /// 获取属于登录人的数据
        /// </summary>
        /// <param name="s">true--匹配与登录人值班的数据，false--获取所有数据</param>
        /// <param name="emp"></param>
        /// <param name="typeid">值班类型</param>
        /// <returns></returns>
        public List<TeacherNightView> AccordingtoEmpGetData(bool s, string emp, int typeid)
        {
            StringBuilder sb = new StringBuilder("select * from TeacherNightView where 1=1");
            if (!s)
            {
                sb.Append(" and BeOnDuty_Id=" + typeid);
            }
            else
            {
                sb.Append(" and BeOnDuty_Id=" + typeid + " and Tearcher_Id='" + emp + "'");
            }
            return this.GetListBySql<TeacherNightView>(sb.ToString());
        }

        /// <summary>
        /// 获取属于班主任值班的数据
        /// </summary>
        /// <param name="emp"></param>
        /// <param name="typeid1"></param>
        /// <param name="typeid2"></param>
        /// <returns></returns>
        public List<TeacherNightView> AccordingtoEmpGetData(string emp, int typeid1,int typeid2)
        {
            StringBuilder sb = new StringBuilder("select * from TeacherNightView where Tearcher_Id='"+ emp + "' and (BeOnDuty_Id="+ typeid1 + " or BeOnDuty_Id="+ typeid2 + ")");

            return this.GetListBySql<TeacherNightView>(sb.ToString());
        }

        /// <summary>
        /// 获取这个部门的所有值班数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<TeacherNightView> AccordingtoDepartMentData(List<EmployeesInfo> list,int type) {
            List<TeacherNightView> all = this.GetListBySql<TeacherNightView>("select * from TeacherNightView where BeOnDuty_Id="+type);

            List<TeacherNightView> list_v = new List<TeacherNightView>();
            foreach (EmployeesInfo item in list)
            {
                list_v.AddRange( all.Where(a => a.Tearcher_Id == item.EmployeeId).ToList());
            }

            return list_v;
        }

        public List<TeacherNightView> AccordingtoDepartMentData(List<EmployeesInfo> list, int type1,int type2)
        {
            List<TeacherNightView> all = this.GetListBySql<TeacherNightView>("select * from TeacherNightView where BeOnDuty_Id=" + type1+ " or BeOnDuty_Id="+type2);

            List<TeacherNightView> list_v = new List<TeacherNightView>();
            foreach (EmployeesInfo item in list)
            {
                list_v.AddRange(all.Where(a => a.Tearcher_Id == item.EmployeeId).ToList());
            }

            return list_v;
        }
        /// <summary>
        /// 获取与登录人同部门的员工
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public List<EmployeesInfo> AccordingtoEmplyess(string emp)
        {
            EmployeesInfoManage emp_Entity = new EmployeesInfoManage();
            EmployeesInfo employees= emp_Entity.FindEmpData(emp,true);
            Position position = emp_Entity.GetPobjById(employees.PositionId);
            return emp_Entity.GetEmpsByDeptid(position.DeptId);
        }

        /// <summary>
        /// 获取所有班主任的值班数据
        /// </summary>
        /// <param name="typeid1"></param>
        /// <param name="typeid2"></param>
        /// <returns></returns>
        public List<TeacherNightView> GetHeadMasterAll(int typeid1, int typeid2)
        {
            StringBuilder sb = new StringBuilder("select * from TeacherNightView where BeOnDuty_Id=" + typeid1 + " or BeOnDuty_Id=" + typeid2 + "");
            return this.GetListBySql<TeacherNightView>(sb.ToString());
        }
         
        /// <summary>
        /// 判断是否是教务（false--否，true--是）
        /// </summary>
        /// <param name="empnumber"></param>
        /// <returns></returns>
        public bool IsJiaowu(string empnumber)
        {
          Position findp=  Reconcile_Com.Employees_Entity.GetPositionByEmpid(empnumber);
            bool s = false;
            if (findp.PositionName.Contains("教务"))
            {
                s = true;
            }

            return s;
        }
    }
}
