using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Employment;
using SiliconValley.InformationSystem.Business.Psychro;
using SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness;
using SiliconValley.InformationSystem.Business.StuSatae_Maneger;
using SiliconValley.InformationSystem.Entity.Base_SysManage;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.Channel
{
    /// <summary>
    /// 渠道员工的业务类
    /// </summary>
    public class ChannelStaffBusiness : BaseBusiness<ChannelStaff>
    {
        private EmployeesInfoManage dbempinfo;
        private EmployeesInfoManage dbstaff;
        private MrdEmpTransactionBusiness dbyidong;
        /// <summary>
        /// 学校年度计划的业务类
        /// </summary>
        private SchoolYearPlanBusiness dbschoolpaln;
        private ChannelAreaBusiness dbchannelarea;
        private StuStateManeger Statu_Entity;



        /// <summary>
        /// 得到没有离职的渠道专员
        /// </summary>
        /// <returns>List<ChannelStaff></returns>
        public List<ChannelStaff> GetChannelStaffs()
        {
            return this.GetIQueryable().Where(a => a.IsDel == false).ToList();
        }

        /// <summary>
        /// 获取所有的渠道员工包括离职的
        /// </summary>
        /// <returns></returns>
        public List<ChannelStaff> GetAll()
        {
            return this.GetIQueryable().ToList();
        }
        /// <summary>
        /// 根据这个渠道专员id获取渠道专员对象
        /// </summary>
        /// <param name="ChanneID"></param>
        /// <returns>ChannelStaff</returns>
        public ChannelStaff GetChannelByID(int? ChanneID)
        {
            return this.GetChannelStaffs().Where(a => a.ID == ChanneID).FirstOrDefault();
        }
        /// <summary>
        /// 根据员工id获取渠道专员对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public ChannelStaff GetChannelByEmpID(string empid)
        {
            return this.GetChannelStaffs().Where(a => a.EmployeesInfomation_Id == empid).FirstOrDefault();

        }



        /// <summary>
        /// 删除渠道员工
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool DelChannelStaff(string empid)
        {
            ChannelStaff channelStaff = this.GetChannelByEmpID(empid);
            if (channelStaff==null)
            {
                return true ;
            }
            channelStaff.IsDel = true;
            channelStaff.QuitDate = DateTime.Now;
            bool result = false;
            try
            {
                this.Update(channelStaff);
                result = true;
                BusHelper.WriteSysLog("当渠道员工离职的时候，对渠道员工的isdel进行修改，位于Channel文件夹中ClassesBusiness业务类中DelChannelStaff方法，编辑成功。", EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {

                result = false;
                BusHelper.WriteSysLog("当渠道员工离职的时候，对渠道员工的isdel进行修改，位于Channel文件夹中ClassesBusiness业务类中DelChannelStaff方法，编辑失败。", EnumType.LogType.编辑数据);
            }
            return result;
        }
        /// <summary>
        /// 添加渠道员工
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool AddChannelStaff(string empid)
        {
            ChannelStaff channelStaff = new ChannelStaff();
            channelStaff.IsDel = false;
            channelStaff.EmployeesInfomation_Id = empid;
            channelStaff.ChannelDate = DateTime.Now;
            bool result = false;
            try
            {
                this.Insert(channelStaff);
                result = true;
                BusHelper.WriteSysLog("当添加渠道员工的时候，位于Channel文件夹中ClassesBusiness业务类中AddChannelStaff方法，添加成功。", EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                result = false;
                BusHelper.WriteSysLog("当添加渠道员工的时候，位于Channel文件夹中ClassesBusiness业务类中AddChannelStaff方法，添加失败。", EnumType.LogType.添加数据);
            }
            return result;

        }

        /// <summary>
        /// 根据年度计划的id获取该年度的人员情况
        /// </summary>
        /// <param name="PlanID"></param>
        /// <returns></returns>
        public List<ChannelStaff> GetChannelByYear(SchoolYearPlan nowschoolplan, SchoolYearPlanBusiness dbschoolpaln)
        {
            var nextdata = dbschoolpaln.GetNextPlan(nowschoolplan);
            List<ChannelStaff> resultlist = new List<ChannelStaff>();
            var channelstafflist = this.GetAll();
            for (int i = channelstafflist.Count - 1; i >= 0; i--)
            {
                //现在员工
                if (channelstafflist[i].ChannelDate >= nowschoolplan.PlanDate)
                {
                    if (nextdata.ID != 0)
                    {
                        if (channelstafflist[i].ChannelDate < nextdata.PlanDate)
                        {
                            resultlist.Add(channelstafflist[i]);
                        }
                    }
                    else
                    {
                        resultlist.Add(channelstafflist[i]);
                    }
                }
                //老员工
                else
                {
                    if (channelstafflist[i].IsDel == false)
                    {
                        resultlist.Add(channelstafflist[i]);
                    }
                    else
                    {
                        if (nextdata.ID != 0)
                        {
                            if (channelstafflist[i].QuitDate <= nextdata.PlanDate)
                            {
                                resultlist.Add(channelstafflist[i]);
                            }
                        }
                        else
                        {
                            resultlist.Add(channelstafflist[i]);
                        }
                    }
                }
            }
            return resultlist;
        }
        /// <summary>
        /// 根据年度计划，获取这个年度的主任 如果这个人离职了，但是离职时间是当前这个年度，丢弃不加载。 这个人这个季度是主任，下个季度降职，加载，如果这个人这个季度不是i主任，下个季度升职的不加载。
        /// </summary>
        /// <param name="nowschoolplan"></param>
        /// <param name="dbschoolpaln"></param>
        /// <returns></returns>
        public List<ChannelStaff> GetChannelZhurenByPlan(SchoolYearPlan nowschoolplan, SchoolYearPlanBusiness dbschoolpaln)
        {
            dbyidong = new MrdEmpTransactionBusiness();
            dbstaff = new EmployeesInfoManage();
            var data = this.GetChannelByYear(nowschoolplan, dbschoolpaln);
            List<ChannelStaff> zhurenlist = new List<ChannelStaff>();
            foreach (var item in data)
            {
                var yidong = dbyidong.GetTransactionByPlan_EmpID(item.EmployeesInfomation_Id, nowschoolplan);
                if (yidong != null)
                {
                  var  query=   dbstaff.GetEntity(yidong.EmployeeId);
                    if (dbstaff.IsChannelZhuren(query))
                    {
                        zhurenlist.Add(item);
                    }
                    
                }
                else
                {
                    var empinfo = dbstaff.GetInfoByEmpID(item.EmployeesInfomation_Id);
                    if (dbstaff.IsChannelZhuren(empinfo))
                    {
                        zhurenlist.Add(item);
                    }
                    
                }
            }
            return zhurenlist;
        }

        /// <summary>
        /// 根据主任的渠道id获取底下副主任员工
        /// </summary>
        /// <param name="ZhurenChannelID"></param>
        /// <param name="nowschoolplan"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<EmployeesInfo> GetFUzhurenByPlan(string ZhurenEmpID, SchoolYearPlan nowschoolplan, List<ChannelStaff> data) {
            dbchannelarea = new ChannelAreaBusiness();
            dbempinfo = new EmployeesInfoManage();
            var zhuren= this.GetChannelByEmpID(ZhurenEmpID);
            List<ChannelArea> listarea = new List<ChannelArea>();
            List<EmployeesInfo> result = new List<EmployeesInfo>();

            foreach (var item in data)
            {
                var mydata = dbchannelarea.GetAreaByPaln(item.ID, nowschoolplan);
                listarea.AddRange(mydata);
            }

            //副主任
            foreach (var item in listarea)
            {
                if (item.RegionalDirectorID == zhuren.ID)
                {
                    var dudu= dbempinfo.GetInfoByChannelID(item.ChannelStaffID);
                    result.Add(dudu);
                }
            }
            //去除重复的渠道员工
            for (int i = 0; i < result.Count; i++)
            {
                for (int j = result.Count - 1; j > i; j--)
                {

                    if (result[i].EmployeeId == result[j].EmployeeId)
                    {
                        result.RemoveAt(j);
                    }

                }
            }
            return result;

        }

        /// <summary>
        /// 获取这个员工在该季度根据月份进行的查询备案数据
        /// </summary>
        /// <returns></returns>
        public List<StudentPutOnRecord> GetChannelMonthKeepOnRecord(int month,List<StudentPutOnRecord> data)
        {
            List<StudentPutOnRecord> result = new List<StudentPutOnRecord>();
            foreach (var item in data)
            {
                if (item.StuDateTime!=null)
                {
                    if (item.StuDateTime.Month == month)
                    {
                        result.Add(item);
                    }
                }
                
            }

            return result;
        }

        /// <summary>
        /// 获取这个员工在该季度根据月份进行的查询上门
        /// </summary>
        /// <returns></returns>
        public List<StudentPutOnRecord> GetChannelGoSchoolCount(int month, List<StudentPutOnRecord> data)
        {
            List<StudentPutOnRecord> result = new List<StudentPutOnRecord>();
            foreach (var item in data)
            {
                if (item.StuVisit!=null)
                {
                    if (Convert.ToDateTime(item.StuVisit).Month == month)
                    {
                        result.Add(item);
                    }
                }
                
            }

            return result;
        }
        /// <summary>
        /// 获取这个员工在该季度根据月份进行的查询报名
        /// </summary>
        /// <returns></returns>
        public List<StudentPutOnRecord> GetChannelSignUpCount(int month, List<StudentPutOnRecord> data)
        {
            List<StudentPutOnRecord> result = new List<StudentPutOnRecord>();
            //获取报名id
            Statu_Entity = new StuStateManeger();
            int id = Statu_Entity.GetList().Where(s => s.StatusName == "已报名").FirstOrDefault().Id;
            foreach (var item in data)
            {
                if (item.StuStatus_Id== id)
                {
                    if (Convert.ToDateTime(item.StatusTime).Month == month)
                    {
                        result.Add(item);
                    }
                }
                
            }

            return result;
        }

        /// <summary>
        /// 判断当前这个员工是不是主任
        /// </summary>
        /// <param name="empinfoid"></param>
        /// <returns></returns>
        public bool IsZhuren(string empinfoid, List<ChannelStaff> querylist) {

            bool IsZhuren = false;
            //循环判断这个值是不是主任
            foreach (var item in querylist)
            {
                if (item.EmployeesInfomation_Id == empinfoid)
                {
                    IsZhuren = true;
                }
            }
            return IsZhuren;
        }
    }
}
