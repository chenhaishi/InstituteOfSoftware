using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.FinanceBusiness
{
    //财务业务层
 public   class FinanceModelBusiness:BaseBusiness<FinanceModel>
    {
        /// <summary>
        /// 添加财务人员，返回true则为成功
        /// </summary>
        /// <param name="Empid">员工编号</param>
        /// <returns></returns>
        public bool AddFinancialstaff(string Empid)
        {
            FinanceModel financeModel = new FinanceModel();
            financeModel.AddDate = DateTime.Now;
            financeModel.Financialstaff = Empid;
            financeModel.IsDelete = false;
            bool str = false;
            try
            {  this.Insert(financeModel);
                BusHelper.WriteSysLog("添加数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                str = true;
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);

            }
            return str;
        }
        /// <summary>
        /// 员工离职，撤销财务人员,返回true则为成功
        /// </summary>
        /// <param name="Empid">员工编号</param>
        /// <returns></returns>
        public bool UpdateFinancialstaff(string Empid)
        {
          var x=  this.GetList().Where(a => a.IsDelete == false && a.Financialstaff == Empid).FirstOrDefault();
            bool str = false;
            try
            {
                x.IsDelete = true;
                this.Update(x);
                BusHelper.WriteSysLog("添加财务数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                str = true;
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
                throw;
            }
            return str;
        }
    }
}
