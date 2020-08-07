using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness
{
    public class MeritsCheckManage : BaseBusiness<MeritsCheck>
    {
        RedisCache rc=new RedisCache();
        /// <summary>
        /// 将员工绩效考核表数据存储到redis服务器中
        /// </summary>
        /// <returns></returns>
        public List<MeritsCheck> GetEmpMCData() {
            rc.RemoveCache("InRedisMCData");
            List<MeritsCheck> mclist = new List<MeritsCheck>();
            if (mclist==null || mclist.Count()==0) {
                mclist = this.GetList();
                rc.SetCache("InRedisMCData",mclist);
            }
            mclist= rc.GetCache<List<MeritsCheck>>("InRedisMCData");
            return mclist;
        }
        /// <summary>
        /// 往员工绩效考核表加入员工编号
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        //public bool AddEmpToMeritsCheck(string empid)
        //{
        //    bool result = false;
        //    try
        //    {
        //        MeritsCheck ese = new MeritsCheck();
        //        ese.EmployeeId = empid;
        //        ese.IsDel = false;
        //        if (this.GetEmpMCData().Count() == 0)
        //        {
        //            ese.YearAndMonth = DateTime.Now;
        //        }
        //        else
        //        {
        //            ese.YearAndMonth = this.GetEmpMCData().LastOrDefault().YearAndMonth;
        //        }

        //        this.Insert(ese);
        //        rc.RemoveCache("InRedisMCData");
        //        result = true;
        //        BusHelper.WriteSysLog("绩效考核表添加员工成功", Entity.Base_SysManage.EnumType.LogType.添加数据);

        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
        //    }
        //    return result;

        //}

        /// <summary>
        /// 绩效考核表禁用员工方法
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool EditEmpStateToMC(string empid,string time)
        {
            bool result = false;
            try
            {
                var ymtime = DateTime.Parse(time);
                var mc = this.GetEmpMCData().Where(e => e.EmployeeId == empid&&DateTime.Parse(e.YearAndMonth.ToString()).Year==ymtime.Year&&DateTime.Parse(e.YearAndMonth.ToString()).Month==ymtime.Month).FirstOrDefault();
                mc.IsDel = true;
                this.Update(mc);
                rc.RemoveCache("InRedisMCData");
                result = true;
                BusHelper.WriteSysLog("绩效考核表禁用员工成功", Entity.Base_SysManage.EnumType.LogType.编辑数据);

            }
            catch (Exception ex)
            {
                result = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;

        }
        /// <summary>
        /// 将某员工的绩效分默认改为100
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool GetmcempByEmpid(string empid)
        {
            bool result = true;
            try
            {
                var mcemp = this.GetEmpMCData().Where(s => s.EmployeeId == empid).FirstOrDefault();
                if (mcemp!=null) {
                    mcemp.FinalGrade = 100;
                    this.Update(mcemp);
                    rc.RemoveCache("InRedisMCData");
                    result = true;
                    BusHelper.WriteSysLog("将该员工绩效分默认修改为100成功！", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                }

            }
            catch (Exception ex)
            {
                result = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;
        }

        //public MeritsCheck GetBaseEmp(string empid) {
        //    MeritsCheck mc = new MeritsCheck();
        //    return mc;
        //}
    }
}
