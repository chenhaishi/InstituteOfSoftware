using System;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity
{
    /// <summary>
    /// 用于查看学生宿舍数据
    /// </summary>
    public class AccdationinformationView
    {
        public int ID { get; set; }
        /// <summary>
        /// 入住日期
        /// </summary>
        public DateTime StayDate { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 学生编号
        /// </summary>
        public string Studentnumber { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StuName { get; set; }
        /// <summary>
        /// 床位号
        /// </summary>
        public int BedId { get; set; }
        /// <summary>
        /// 其他说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }
        /// <summary>
        /// 寝室编号
        /// </summary>
        public int DormId { get; set; }
        /// <summary>
        /// 寝室名称
        /// </summary>
        public string DormInfoName { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
