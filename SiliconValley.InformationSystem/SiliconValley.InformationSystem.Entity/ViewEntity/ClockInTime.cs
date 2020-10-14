using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
   public class ClockInTime//打卡时间视图类
    {
        public Nullable<int> DDAppId { get; set; }//工号
        public Nullable<bool> IsHavingNoontime { get; set; }//是否有午休打卡时间
    }
}
