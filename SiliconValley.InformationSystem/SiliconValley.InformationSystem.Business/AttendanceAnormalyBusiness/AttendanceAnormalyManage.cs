using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Entity.Entity;

namespace SiliconValley.InformationSystem.Business.AttendanceAnormalyBusiness
{
    using System.IO;
    using NPOI.SS.UserModel;
    using NPOI.HSSF.UserModel;
    using NPOI.XSSF.UserModel;
    using SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;

    public class AttendanceAnormalyManage : BaseBusiness<AttendanceAnormaly>
    {
        /// <summary>
        /// 添加考勤异常表数据
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="yearandmonth"></param>
        /// <returns></returns>
        public bool InsertAtdanor(string empid, DateTime yearandmonth)
        {
            AttendanceAnormaly atdanor = new AttendanceAnormaly();
            var result = true;
            try
            {
                atdanor.EmployeeId = empid;
                atdanor.YearAndMonth = yearandmonth;
                this.Insert(atdanor);
                   
               //考勤异常详细表编号
                result = true;
                
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
