using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity
{
  public  class JuniorCollegeView
    {

        public int ID { get; set; }
        /// <summary>
        /// 学号
        /// </summary>
        public string StudentNumber { get; set; }
        /// <summary>
        /// 班级名称
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 班主任
        /// </summary>
        public string Headmasters { get; set; }
        /// <summary>
        /// 成考学号
        /// </summary>
        public string ChengkaoStudentNumber { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string identitydocument { get; set; }
        /// <summary>
        /// 学生本人电话
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// 学生家长电话
        /// </summary>
        public string Familyphone { get; set; }
        /// <summary>
        /// 学校id
        /// </summary>
        public string School { get; set; }
        /// <summary>
        /// 专业id
        /// </summary>
        public string Major { get; set; }
        /// <summary>
        /// 学历层次
        /// </summary>
        public string EducationLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 是否毕业
        /// </summary>
        public bool IsGraduation { get; set; }
    }
}
