using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.TeachingDepBusiness
{
    using SiliconValley.InformationSystem.Entity.MyEntity;
    /// <summary>
    /// 阶段 业务类
    /// </summary>
   public class GrandBusiness:BaseBusiness<Grand>
    {
        /// <summary>
        /// 根据阶段编号获取阶段对象
        /// </summary>
        /// <param name="GrandID">阶段编号</param>
        /// <returns></returns>
        public Grand GetGrandByID(int GrandID)
        {
           return this.GetList().Where(d=>d.IsDelete==false && d.Id==GrandID).FirstOrDefault();
        }

        /// <summary>
        /// 根据阶段名称获取阶段数据
        /// </summary>
        /// <param name="Name">阶段名称</param>
        /// <returns></returns>
        public Grand FindNameGetData(string Name)
        {
            return this.GetList().Where(g => g.GrandName == Name).FirstOrDefault();
        }

        public List<Grand> AllGrand()
        {
            return this.GetList();
        }

        public bool IsContains(List<Grand> souces, Grand grand)
        {
            foreach (var item in souces)
            {
                if (item.Id == grand.Id)
                {
                    return true;
                }
            }

            return false;
        }


    }
}
