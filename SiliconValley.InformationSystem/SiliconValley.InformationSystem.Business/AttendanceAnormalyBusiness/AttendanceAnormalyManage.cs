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
    {//考勤异常类型 业务类 

        /// <summary>
        /// 通过异常名称获取其编号
        /// </summary>
        /// <param name="aaname">异常类型名称</param>
        /// <returns></returns>
        public int GetaaidByaaname(string aaname) {
            var aaobj = this.GetList().Where(s => s.AnormalyTypeName == aaname).FirstOrDefault();
            return aaobj.Id;
        }
        /// <summary>
        ///  通过异常编号获取其异常名称
        /// </summary>
        /// <param name="aaid"></param>
        /// <returns></returns>
        public string GetaanameByaaid(int aaid) {
            var aaobj = this.GetEntity(aaid);
            return aaobj.AnormalyTypeName;
        }
    }
}
