using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
    /// <summary>
    /// 学生备案数据导出实体
    /// </summary>
   public class ExportStudentBeanData
    {
        /// <summary> 
        /// 备案编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StuName { get; set; }

        /// <summary>
        /// 学生性别
        /// </summary>
        public string StuSex { get; set; }

        /// <summary>
        /// 学生出生年月日
        /// </summary>
        public DateTime? StuBirthy { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string Stuphone { get; set; }

        /// <summary>
        /// 毕业学校
        /// </summary>
        public string StuSchoolName { get; set; }

        /// <summary>
        /// 学历
        /// </summary>
        public string StuEducational { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string StuAddress { get; set; }

        /// <summary>
        /// 微信号
        /// </summary>
        public string StuWeiXin { get; set; }

        /// <summary>
        /// QQ号
        /// </summary>
        public string StuQQ { get; set; }

        /// <summary>
        /// 信息来源
        /// </summary>
        public string stuinfomation { get; set; }

        /// <summary>
        /// 备案状态
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// 是否来校访问
        /// </summary>
        public bool? StuisGoto { get; set; }

        /// <summary>
        /// 来校访问的日期
        /// </summary>
        public DateTime? StuVisit { get; set; }

        /// <summary>
        /// 备案人
        /// </summary>
        public string empName { get; set; }

        /// <summary>
        /// 备案时间
        /// </summary>
        public DateTime BeanDate { get; set; }

        /// <summary>
        /// 数据录入人员
        /// </summary>
        public string StuEntering { get; set; }

        /// <summary>
        /// 报名时间
        /// </summary>
        public DateTime? StatusTime { get; set; }


        /// <summary>
        /// 所在区域
        /// </summary>
        public string RegionName { get; set; }

        /// <summary>
        /// 其他说明
        /// </summary>
        public string Reak { get; set; }

        /// <summary>
        /// 咨询师
        /// </summary>
        public string ConsultTeacher { get; set; }

        /// <summary>
        /// 关系人
        /// </summary>
        public string Party { get; set; }

        /// <summary>
        /// 市场类型
        /// </summary>
        public string MarketType { get; set; }

        /// <summary>
        /// 详细备案时间
        /// </summary>
        public DateTime StuDateTime { get; set; }

        /// <summary>
        /// 数据状态（0--正常，1--作废，2--冲突数据）
        /// </summary>
        public int IsDelete { get; set; }
    }
}
