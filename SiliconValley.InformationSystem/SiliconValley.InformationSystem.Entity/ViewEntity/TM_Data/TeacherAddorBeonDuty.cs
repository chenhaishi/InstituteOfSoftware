using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data
{
    /// <summary>
    /// 教员值班数据表
    /// </summary>
   [Table("TeacherAddorBeonDuty")]
   public class TeacherAddorBeonDuty
    {
        [Key]
       public int Id { get; set; }
        /// <summary>
        /// --值班情况
        /// </summary>
        public string OnByReak { get; set; }
        /// <summary>
        /// --值班类型
        /// </summary>
        public int BeOnDuty_Id { get; set; }
        /// <summary>
        /// --员工编号
        /// </summary>
        public string Tearcher_Id { get; set; }
        /// <summary>
        /// --数据创建日期
        /// </summary>
        public DateTime AttendDate { get; set; }
        /// <summary>
        ///  --晚自习编号
        /// </summary>
        public int evning_Id { get; set; } 
        /// <summary>
        /// 是否已审核
        /// </summary>
        public bool IsDels { get; set; }
    }

    /// <summary>
    /// 教员值班，晚自习加班数据视图表
    /// </summary>
    [Table("TeacherAddorBeonDutyView")]
    public class TeacherAddorBeonDutyView {
        [Key]
       public int Id { get; set; }
       public string OnByReak { get; set; }
       public int BeOnDuty_Id { get; set; }
       public DateTime Anpaidate { get; set; }
       public string Tearcher_Id { get; set; }
       public DateTime AttendDate { get; set; }
       public int evning_Id { get; set; }
       public string ClassroomName { get; set; }
       public  string ClassNumber { get; set; }
       /// <summary>
       /// 值班时间段（晚一或晚二）
       /// </summary>
       public  string curd_name { get; set; }
        /// <summary>
        /// 值班类型
        /// </summary>
       public string TypeName { get; set; }
        /// <summary>
        /// 教员名称
        /// </summary>
        public string EmpName { get; set; }

        public bool IsDels { get; set; }

       public static TeacherAddorBeonDuty ToModel(TeacherAddorBeonDutyView data)
        {
            TeacherAddorBeonDuty model = new TeacherAddorBeonDuty();
            model.AttendDate = data.AttendDate;
            model.BeOnDuty_Id = data.BeOnDuty_Id;
            model.evning_Id = data.evning_Id;
            model.Id = data.Id;
            model.OnByReak = data.OnByReak;
            model.Tearcher_Id = data.Tearcher_Id;
            model.IsDels = data.IsDels;
            return model;
        }

    }

}
