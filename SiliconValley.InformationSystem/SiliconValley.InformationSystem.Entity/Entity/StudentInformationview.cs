using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.Entity
{
    [Table("StudentInformationview")]
   public  class StudentInformationview
    {
        /// <summary>
        /// 学号
        /// </summary>
        [Key]
        public string StudentNumber { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
      
        public string identitydocument { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// QQ号
        /// </summary>
        public string qq { get; set; }
        /// <summary>
        /// 微信号
        /// </summary>
        public string WeChat { get; set; }
        /// <summary>
        /// 学员照片
        /// </summary>
        public string Picture { get; set; }
        /// <summary>
        /// 异动操作，需要在线生则为null
        /// </summary>13
        public Nullable<int> State { get; set; }
        /// <summary>
        /// 爱好
        /// </summary>
        public string Hobby { get; set; }
        /// <summary>
        /// 家庭住址
        /// </summary>
        public string Familyaddress { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public Nullable<System.DateTime> BirthDate { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public string Education { get; set; }
        /// <summary>
        /// 性格
        /// </summary>
        public string Traine { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 备案信息
        /// </summary>
        public Nullable<int> StudentPutOnRecord_Id { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public Nullable<bool> IsDelete { get; set; }
        /// <summary>
        /// 添加时间
        /// </summary>
        public Nullable<System.DateTime> InsitDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Reack { get; set; }
        /// <summary>
        /// 亲属姓名+类型（父亲）
        /// </summary>
        public string Guardian { get; set; }
        /// <summary>
        /// 亲属电话
        /// </summary>
        public string Familyphone { get; set; }

        /// <summary>
        /// 身份证正面
        /// </summary>
        public string Identityjustimg { get; set; }
        /// <summary>
        /// 身份证反面
        /// </summary>
        public string Identitybackimg { get; set; }
    }
}
