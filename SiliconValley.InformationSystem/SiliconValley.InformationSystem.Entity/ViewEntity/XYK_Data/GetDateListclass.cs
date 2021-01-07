using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Entity.ViewEntity.XYK_Data
{
     public class GetDateListclass
    {
        //学号
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
        /// 家庭住址
        /// </summary>
        public string Familyaddress { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public Nullable<System.DateTime> BirthDate { get; set; }
        //宿舍号
        public string dormitory { get; set; }
        //访谈记录
        public string interview { get; set; }
    }
}
