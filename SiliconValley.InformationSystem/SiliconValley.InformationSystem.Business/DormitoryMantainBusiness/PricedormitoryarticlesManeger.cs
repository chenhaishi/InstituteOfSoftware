using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.DormitoryMantainBusiness
{
    /// <summary>
    /// 宿舍物品维修价格表业务类
    /// </summary>
    public class PricedormitoryarticlesManeger:BaseBusiness<Pricedormitoryarticles>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="all">是否获取所有的数据，true--是，false--只获取有效的数据</param>
        /// <returns></returns>
        public List<Pricedormitoryarticles> GetList(bool all)
        {
            List<Pricedormitoryarticles> data = new List<Pricedormitoryarticles>();
            if (all)
            {
                data = this.GetList();
            }
            else
            {
                data = this.GetList().Where(d => d.Dateofregistration == true).ToList();
            }

            return data;
        }


        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <param name="key">true--根据主键查询，false--根据物品名称查询</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Pricedormitoryarticles Find(bool key,string value)
        {
            Pricedormitoryarticles data = new Pricedormitoryarticles();
            if (key)
            {
                int id = Convert.ToInt32(value);
                data= this.GetEntity(id);
            }
            else
            {
                data = this.GetList().Where(d => d.Nameofarticle == value).FirstOrDefault();
            }
            return data;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddData(Pricedormitoryarticles data)
        {
            bool result = true;
            try
            {
                this.Insert(data);
            }
            catch (Exception)
            {
                result = false;                
            }

            return result;
        }

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateData(Pricedormitoryarticles data)
        {
            bool result = true;
            try
            {
                this.Update(data);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }


    }
}
