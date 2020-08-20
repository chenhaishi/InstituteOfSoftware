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
    [Table("AttendanceAnormaly")]//考勤异常表
    public partial class AttendanceAnormaly
    {
        [Key]
        public int Id { get; set; }
        public string EmployeeId { get; set; }//员工编号
        public Nullable<int> AnormalyTypeId { get; set; }//考勤异常类型
        public Nullable<System.DateTime> YearAndMonth { get; set; }//年月份

     
    }
}
