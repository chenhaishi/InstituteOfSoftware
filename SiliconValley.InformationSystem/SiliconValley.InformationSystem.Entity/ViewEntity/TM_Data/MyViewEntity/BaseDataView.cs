using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity
{
    [Table("BaseDataView")]
   public class BaseDataView
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 班级编号
        /// </summary>
        public int ClassSchedule_Id { get; set; }
        /// <summary>
        /// 巡班时间段
        /// </summary>
        public string BaseDataTime { get; set; }
        /// <summary>
        /// 添加日期
        /// </summary>
        public Nullable<System.DateTime> RecodeDate { get; set; }
        /// <summary>
        /// 备注说明
        /// </summary>
        public string Rmark { get; set; }

        public bool IsDelete { get; set; }

        /// <summary>
        /// 违纪类型编号
        /// </summary>
        public int Violationofdiscipline_Id { get; set; }

        /// <summary>
        /// 登记人编号
        /// </summary>
        public string Emp_Id { get; set; }

        /// <summary>
        /// 违纪人数
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 登记人姓名
        /// </summary>
        public string EmpName { get; set; }

        /// <summary>
        /// 违纪类型
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassNumber { get; set; }

        public string empstring { get; set; }
    }
     
}
