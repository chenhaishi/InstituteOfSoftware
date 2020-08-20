using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.XYK_Data
{
    /// <summary>
    /// 学生缴费
    /// </summary>
  public   class Student
    {
        //学生名字
        public string StuName { get; set; }
        //学生备案id
        public int Keepid { get; set; }
        //费用
        public int num { get; set; }
    }
}
