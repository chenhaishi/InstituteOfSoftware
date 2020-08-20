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
        public string AnormalyTypeName { get; set; }//异常类型名称
        public string TimeRange { get; set; }//时间范围
        public Nullable<decimal> DebitMoney { get; set; }//扣费金额
        public int EmpLevel { get; set; }//员工级别（1：普通员工,2：主任及以上）
    }
}
