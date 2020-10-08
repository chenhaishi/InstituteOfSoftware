using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("AttendanceAnormalyDetails")]//考勤异常详情表
    public partial class AttendanceAnormalyDetails
    {
        public int Id { get; set; }
        public Nullable<int> AnormalyTypeId { get; set; }//考勤异常类型
      //  public string TimeRange { get; set; }//时间范围
      //  public Nullable<decimal> DebitMoney { get; set; }//扣费金额
       // public int EmpLevel { get; set; }//员工级别（1：普通员工,2：主任及以上）

        public string EmployeeId { get; set; }//员工编号
        public Nullable<System.DateTime> YearAndMonth { get; set; }//年月份
        public string AnormalyDay { get; set; }//具体异常时间
        public Nullable<decimal> AnormalyDuration { get; set; }//异常时长
        public string Remark { get; set; }//备注
        public Nullable<bool> IsDel { get; set; }//是否属实（false代表有效考勤异常；true代表无效考勤异常（与实际不符则该异常改为无效））

    }
}
