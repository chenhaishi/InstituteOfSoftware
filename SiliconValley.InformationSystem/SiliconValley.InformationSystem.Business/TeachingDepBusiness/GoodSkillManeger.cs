using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.TeachingDepBusiness
{
    //老师可以上的课程
   public class GoodSkillManeger:BaseBusiness<GoodSkill>
    {
        /// <summary>
        /// 查看这个集合是否有等于name的值
        /// </summary>
        /// <param name="t"></param>
        /// <param name="name"></param>
        /// <returns></returns>
       public bool IsOrride(List<Teacher> t,int name)
        {
            bool s = false;
            foreach (Teacher item in t)
            {
                if (item.TeacherID==name)
                {
                    s = true;
                }
            }
            return s;
        }
         /// <summary>
         /// 通过课程获取老师
         /// </summary>
         /// <param name="curr">课程Id</param>
         /// <returns></returns>
        public List<Teacher> GetTeachers(int curr)
        {
            List<GoodSkill> sk_list= this.GetList().Where(t => t.Curriculum == curr).ToList();
            List<Teacher> ts = new List<Teacher>();
            List<Teacher> get_t = new BaseBusiness<Teacher>().GetIQueryable().ToList();
            foreach (GoodSkill item1 in sk_list)
            {
               Teacher t= get_t.Where(g => g.TeacherID == item1.TearchID && g.IsDel == false).FirstOrDefault();
                if (t!=null)
                {
                    bool s = IsOrride(ts, t.TeacherID);
                    if (s == false)
                    {
                        ts.Add(t);
                    }
                }
                            
            }
            return ts;
        }
    }
}
