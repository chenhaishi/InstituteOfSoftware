using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SiliconValley.InformationSystem.Business.DormitoryBusiness
{
    using SiliconValley.InformationSystem.Business.ClassesBusiness;
    using SiliconValley.InformationSystem.Entity.Entity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;

    /// <summary>
    /// 学生调寝业务类
    /// </summary>
    public class ChangeDorStudent:BaseBusiness<Accdationinformation>
    {

       public DormInformationBusiness DormInfo_Entity = new DormInformationBusiness();

        /// <summary>
        /// 班主任业务类
        /// </summary>
        public HeadmasterBusiness Headmaster_Entity = new HeadmasterBusiness();

        public BaseBusiness<Tung> Tung_Entity = new BaseBusiness<Tung>();

        public BaseBusiness<Accdationinformation> Accdationinformation_Entity = new BaseBusiness<Accdationinformation>();

        /// <summary>
        /// 通过学号获取所在班级
        /// </summary>
        /// <param name="stuName"></param>
        /// <returns></returns>
        public ScheduleForTrainees GetClassName(string stuName)
        {
            ScheduleForTrainees classentity =new ScheduleForTrainees();
            string sqlstr = @"select * from ScheduleForTrainees where  CurrentClass=1 and StudentID='"+stuName+"'";

            List<ScheduleForTrainees> list= this.GetListBySql<ScheduleForTrainees>(sqlstr);

            if (list.Count>0)
            {
                classentity = list[0];
            }
            return classentity;
        }

        /// <summary>
        /// 获取学生当前所在的寝室
        /// </summary>
        /// <param name="stuName"></param>
        /// <returns></returns>
        public DorChuang GetDorName(string stuName)
        {
            DorChuang chuang = new DorChuang();
            string sqlstr = "select * from Accdationinformation where Studentnumber='"+stuName+ "' and EndDate is null and IsDel=0";

            List<Accdationinformation> acclist= this.GetListBySql<Accdationinformation>(sqlstr);

            if (acclist.Count>0)
            {
                chuang.ChuangNumber = acclist[0].BedId;
                chuang.DorNumber= DormInfo_Entity.GetEntity(acclist[0].DormId).DormInfoName;
            }

            return chuang;
        }

        /// <summary>
        /// 添加学员住宿数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddData(Accdationinformation data)
        {
            bool retult = true;
            try
            {
                Accdationinformation_Entity.Insert(data);
            }
            catch (Exception)
            {

                retult = false;
            }

            return retult;
        }


        public bool AddData(List<Accdationinformation> data)
        {
            bool retult = true;
            try
            {
                Accdationinformation_Entity.Insert(data);
            }
            catch (Exception)
            {

                retult = false;
            }

            return retult;
        }

        /// <summary>
        /// 编辑学员寝室数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool UpdateData(Accdationinformation data)
        {
            bool retult = true;
            try
            {
                Accdationinformation_Entity.Update(data);
            }
            catch (Exception)
            {

                retult = false;
            }

            return retult;
        }

        public bool UpdateData(List<Accdationinformation> data)
        {
            bool retult = true;
            try
            {
                Accdationinformation_Entity.Update(data);
            }
            catch (Exception)
            {

                retult = false;
            }

            return retult;
        }
    }
}
