using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public  class OvertimeRecordView
    {
        public int Id { get; set; }
        public string  EmployeeId { get; set; }
        public Nullable<int> DDAppId { get; set; }
        public string  empName { get; set; }
        public string  empDept { get; set; }
        public string empPosition { get; set; }
        public Nullable<bool> IsApproval { get; set; }
        public Nullable<DateTime> YearAndMonth { get; set; }
        public Nullable<DateTime> StartTime { get; set; }
        public Nullable<DateTime> EndTime { get; set; }
        public Nullable<decimal> Duration { get; set; }
        public string  Remark { get; set; }
        public string  OvertimeReason { get; set; }
        public Nullable<bool> IsNoDaysOff { get; set; }
        public int OvertimeTypeId { get; set; }
        public Nullable<bool> IsPass { get; set; }
    }
}
