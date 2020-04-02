//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SiliconValley.InformationSystem.Entity.MyEntity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 学生备案数据表
    /// </summary>
    [Table(name: "StudentPutOnRecord")]
    public partial class StudentPutOnRecord: IEqualityComparer<StudentPutOnRecord>
    {
      
        /// <summary>
        /// 学生备案数据编号
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StuName { get; set; }
        /// <summary>
        /// 学生性别
        /// </summary>
        public Nullable<bool> StuSex { get; set; }
        /// <summary>
        /// 学生出生年月日
        /// </summary>
        public Nullable<System.DateTime> StuBirthy { get; set; }
        /// <summary>
        /// 学生电话
        /// </summary>
        public string StuPhone { get; set; }
        /// <summary>
        /// 学生学校名称
        /// </summary>
        public string StuSchoolName { get; set; }
        /// <summary>
        /// 学生学历
        /// </summary>
        public string StuEducational { get; set; }
        /// <summary>
        /// 学生家庭住址
        /// </summary>
        public string StuAddress { get; set; }
        /// <summary>
        /// 学生微信
        /// </summary>
        public string StuWeiXin { get; set; }
        /// <summary>
        /// 学生QQ
        /// </summary>
        public string StuQQ { get; set; }
        /// <summary>
        /// 信息来源类型
        /// </summary>
        public Nullable<int> StuInfomationType_Id { get; set; }
        /// <summary>
        /// 学生状态
        /// </summary>
        public Nullable<int> StuStatus_Id { get; set; }
        /// <summary>
        /// 学生是否上门
        /// </summary>
        public Nullable<bool> StuIsGoto { get; set; }
        /// <summary>
        /// 学生上门日期
        /// </summary>
        public Nullable<System.DateTime> StuVisit { get; set; }
        /// <summary>
        /// 备案人员
        /// </summary>
        public string EmployeesInfo_Id { get; set; }
        /// <summary>
        /// 备案日期
        /// </summary>
        public System.DateTime StuDateTime { get; set; }
        /// <summary>
        /// 录入人员
        /// </summary>
        public string StuEntering { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Reak { get; set; }
        /// <summary>
        ///  该信息是否删除
        /// </summary>
        public Nullable<bool> IsDelete { get; set; }
        /// <summary>
        /// 学生所在区域
        /// </summary>
        public Nullable<int> Region_id { get; set; }
        /// <summary>
        /// 报名时间
        /// </summary>
        public Nullable<System.DateTime> StatusTime { get; set; }

        public bool Equals(StudentPutOnRecord x, StudentPutOnRecord y)
        {
            if (x.StuName == y.StuName && x.StuPhone == y.StuPhone)
                return true;

            return false;
        }

        public int GetHashCode(StudentPutOnRecord obj)
        {
            return 0;
        }
    }
}
