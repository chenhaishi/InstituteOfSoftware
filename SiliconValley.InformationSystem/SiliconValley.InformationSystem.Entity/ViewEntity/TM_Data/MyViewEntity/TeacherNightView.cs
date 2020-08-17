using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity
{
    /// <summary>
    /// 班主任、教员值班视图
    /// </summary>
    [Table("TeacherNightView")]
    public class TeacherNightView
    {
        [Key]
        public int Id { get; set; }
        public string OnByReak { get; set; }
        public int BeOnDuty_Id { get; set; }
        public DateTime OrwatchDate { get; set; }
        public string Rmark { get; set; }
        public bool IsDelete { get; set; }
       // public string timename { get; set; }已删除
       // public int? ClassRoom_id { get; set; }已删除
        public string Tearcher_Id { get; set; }
       // public int? ClassSchedule_Id { get; set; } 已删除
        public DateTime AttendDate { get; set; }
        public string TypeName { get; set; }
        public string EmpName { get; set; }
        //public string ClassNumber { get; set; }已删除
        //public string ClassroomName { get; set; }已删除


        public TeacherNight ToModel(TeacherNightView t)
        {
            TeacherNight night = new TeacherNight();
            night.Id = t.Id;
            night.IsDelete = t.IsDelete;
            night.OnByReak = t.OnByReak;
            night.OrwatchDate = t.OrwatchDate;
            night.Rmark = t.Rmark;
            night.Tearcher_Id = t.Tearcher_Id;
            //night.timename = t.timename;
            night.BeOnDuty_Id = t.BeOnDuty_Id;
           // night.ClassRoom_id = t.ClassRoom_id;
            //night.ClassSchedule_Id = t.ClassSchedule_Id;
            night.AttendDate = t.AttendDate;
            return night;
        }
    }
}
