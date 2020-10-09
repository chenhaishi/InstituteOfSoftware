using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;

namespace SiliconValley.InformationSystem.Business.DormitoryMantainBusiness
{
    public class DormitoryDepositManeger : BaseBusiness<DormitoryDeposit>
    {
        /// <summary>
        /// 宿舍楼地址业务类
        /// </summary>
        public BaseBusiness<Tung> Tung_Entity = new BaseBusiness<Tung>();

        /// <summary>
        /// 地址楼层业务类
        /// </summary>
        public BaseBusiness<TungFloor> TungFloo_Entity = new BaseBusiness<TungFloor>();

        /// <summary>
        /// 宿舍业务类
        /// </summary>
        public BaseBusiness<DormInformation> DormInformation_Entity = new BaseBusiness<DormInformation>();

        /// <summary>
        /// 物品维修业务类
        /// </summary>
        public BaseBusiness<Pricedormitoryarticles> DormitoryMaintenance_Entity = new BaseBusiness<Pricedormitoryarticles>();

        /// <summary>
        /// 学生住宿安排业务类
        /// </summary>
        public BaseBusiness<Accdationinformation> Accdationinformation_Entity = new BaseBusiness<Accdationinformation>();

        /// <summary>
        /// 学生信息业务类
        /// </summary>
        public BaseBusiness<StudentInformation> StudentInformation_Entity = new BaseBusiness<StudentInformation>();


        /// <summary>
        /// 宿舍楼层
        /// </summary>
        public BaseBusiness<Dormitoryfloor> Dormitoryfloor_Entity = new BaseBusiness<Dormitoryfloor>();

        /// <summary>
        /// 物品维修表业务类
        /// </summary>
        public BaseBusiness<Pricedormitoryarticles> Pricedormitoryarticles_Entity = new BaseBusiness<Pricedormitoryarticles>();

        /// <summary>
        ///  获取XX期间XX寝室的所有学生
        /// </summary>
        /// <param name="date"></param>
        /// <param name="Number"></param>
        /// <returns></returns>
        public List<Accdationinformation> GetStudentSushe(DateTime date,int Number)
        {
            string sqlstr = "select * from Accdationinformation where StayDate>='" + date + "' and EndDate is null and DormId =" + Number;

            List<Accdationinformation> list = this.Accdationinformation_Entity.GetListBySql<Accdationinformation>(sqlstr);//获取属于这个寝室的所有学生


            return list;
        }
    }
}
