using SiliconValley.InformationSystem.Entity.Entity;
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
    /// 晚自习数据视图
    /// </summary>
    [Table("EvningSelfStudyView")]
   public class EvningSelfStudyView
    {
        [Key]
       public int id { get; set; }
       public int ClassSchedule_id { get; set; }
       public int Classroom_id { get; set; }
       public string curd_name { get; set; }
       public DateTime Anpaidate { get; set; }
       public DateTime Newdate { get; set; }
       public string Rmark { get; set; }
       public bool IsDelete { get; set; }
       public string ClassNumber { get; set; }
       public string ClassroomName { get; set; }
       public string EmpName { get; set; }
        /// <summary>
        /// 班级所在阶段
        /// </summary>
       public int grandid { get; set; }

        /// <summary>
        /// 转为实体模型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public EvningSelfStudy ToModel(EvningSelfStudyView t)
        {
            EvningSelfStudy e = new EvningSelfStudy();
            e.id = e.id;
            e.ClassSchedule_id = t.ClassSchedule_id;
            e.Classroom_id = t.Classroom_id;
            e.curd_name = t.curd_name;
            e.Anpaidate = t.Anpaidate;
            e.Newdate = t.Newdate;
            e.Rmark = t.Rmark;
            e.IsDelete = t.IsDelete;
            return e;
        }
    }
}
