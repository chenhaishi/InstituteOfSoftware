using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
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
        /// 员工信息业务类
        /// </summary>
        public BaseBusiness<EmployeesInfo> EmployeesInfo_Entity = new BaseBusiness<EmployeesInfo>();

        /// <summary>
        /// 班级业务类
        /// </summary>
        public ClassScheduleBusiness ClassSchedule_Entity = new ClassScheduleBusiness();

        /// <summary>
        /// 班主任带班业务类
        /// </summary>
        public HeadmasterBusiness Headmaster_Entity = new HeadmasterBusiness();

        /// <summary>
        /// 阶段业务类
        /// </summary>
        public BaseBusiness<Grand> Grand_Entity = new BaseBusiness<Grand>();


        /// <summary>
        ///  获取XX期间XX寝室的所有学生
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="Number">宿舍编号</param>
        /// <returns></returns>
        public List<Accdationinformation> GetStudentSushe(DateTime date, int Number)
        {
            string sqlstr = "select * from Accdationinformation where StayDate>='" + date + "' and DormId =" + Number + " and (EndDate is null or EndDate<='" + date + "') ";

            List<Accdationinformation> list = this.Accdationinformation_Entity.GetListBySql<Accdationinformation>(sqlstr);//获取属于这个寝室的所有学生


            return list;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool AddData(List<DormitoryDeposit> data)
        {
            bool result = true;
            try
            {
                this.Insert(data);
            }
            catch (Exception ex)
            {

                result = false;
            }

            return result;

        }


        public bool AddData(DormitoryDeposit data)
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
        public bool UpdateData(DormitoryDeposit data)
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


        public bool UpdateData(List<DormitoryDeposit> data)
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

        /// <summary>
        /// 获取目前带班班主任
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        public string ClassTeacherName(int classid)
        {
            string sqlstr = @"select emp.EmpName as 'Result' from HeadClass as hc 
							  inner join Headmaster as hd on hc.LeaderID =hd.ID
							  inner join ClassSchedule as cs on hc.ClassID=cs.id
							  inner join EmployeesInfo as emp on emp.EmployeeId=hd.informatiees_Id
							  where hc.EndingTime is null  and cs.id=" + classid;

            List<SqlData> list = this.GetListBySql<SqlData>(sqlstr);

            if (list.Count > 0)
            {
                return list[0].Result.ToString();
            }
            else
            {
                return "无";
            }
        }

        /// <summary>
        /// 判断的登录岗位 1--Admin,2--教质，3---财务
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public int Number(string empid)
        {
            int result = 1;
            string sqlstr = @"select dp.DeptName as 'Result' from EmployeesInfo as emp 
							  inner join Position as pt on pt.Pid=emp.PositionId
							  inner join Department as dp on dp.DeptId=pt.DeptId where emp.EmployeeId='" + empid + "'";
            List<SqlData> list = this.GetListBySql<SqlData>(sqlstr);

            if (list.Count > 0)
            {
                if (list[0].Result.ToString().Contains("教质"))
                {
                    result = 2;

                }
                else if (list[0].Result.ToString().Contains("财务"))
                {
                    result = 3;
                }
            }

            return result;
        }


        /// <summary>
        /// 获取XX学生缴的押金
        /// </summary>
        /// <param name="stunumber"></param>
        /// <returns></returns>
        public decimal GetStudentMoney(string stunumber)
        {
            decimal money = 0;
            string sql = @"select sr.Amountofmoney as 'Data' from StudentFeeRecord as sr inner join Costitems as cm on sr.Costitemsid=cm.id where cm.Name like '%宿舍押金%'  and StudenID='" + stunumber + "'";

            List<SqlData> list = this.GetListBySql<SqlData>(sql);

            if (list.Count > 0)
            {
                if (list[0].Data != null)
                {
                    money = (decimal)list[0].Data;
                }
            }

            return money;
        }

        /// <summary>
        /// 获取学生维修未结算的总费用
        /// </summary>
        /// <param name="stunumber"></param>
        /// <returns></returns>
        public decimal GetMantainMoney(string stunumber)
        {
            decimal money = 0;
            string sqlstr = @" select SUM(GoodPrice) as 'Data' from DormitoryDeposit where StuNumber='" + stunumber + "' and MaintainState='1'";
            List<SqlData> list = this.GetListBySql<SqlData>(sqlstr);

            if (list.Count > 0)
            {
                if (list[0].Data != null)
                {
                    money = (decimal)list[0].Data;
                }

            }

            return money;
        }

        /// <summary>
        /// 获取XX学生的维修数据
        /// </summary>
        /// <param name="Isall">true--显示全部数据，false--显示未支付的数据</param>
        /// <returns></returns>
        public List<DormitoryDeposit> StudentDormitoryDepsitData(string stuNumber,bool Isall)
        {
            if (Isall)
            {
                string sqlstr = "select * from DormitoryDeposit where  StuNumber='" + stuNumber + "'";

                return this.GetListBySql<DormitoryDeposit>(sqlstr);
            }
            else
            {
                string sqlstr = "select * from DormitoryDeposit where MaintainState=1 and StuNumber='" + stuNumber + "'";

                return this.GetListBySql<DormitoryDeposit>(sqlstr);
            }
             
        }



        /// <summary>
        /// 获取某个校区的某个月份的总维修费用
        /// </summary>
        /// <param name="XiaoquAddress">校区编号</param>
        /// <param name="Year">年份</param>
        /// <param name="Month">月份</param>
        /// <returns></returns>
        public decimal MonMantainMoney(int XiaoquAddress, int Year, int Month)
        {
            decimal SumMoney = 0;
            //获取属于这个校区的所有宿舍
            string sqlstr = @"select SUM(GoodPrice) as 'Data' from DormitoryDeposit where DormId in(
                              select Id from  DormInformation where TungFloorId in( select Id from TungFloor where TungId in (select id from Tung where Id=" + XiaoquAddress + " )))  and YEAR(Maintain)='" + Year + "' and  MONTH(Maintain)='" + Month + "'  and MaintainState='1'";
            List<SqlData> list = this.GetListBySql<SqlData>(sqlstr);

            if (list.Count > 0)
            {
                if (list[0].Data != null)
                {
                    SumMoney = (decimal)list[0].Data;
                }

            }
            return SumMoney;
        }

        /// <summary>
        /// 获取这个学生目前所在班级
        /// </summary>
        /// <param name="stuNumber"></param>
        /// <returns></returns>
        public string GetClass(string stuNumber)
        {
            string sqlstr = "select * from ScheduleForTrainees where  CurrentClass=1 and StudentID='" + stuNumber + "'";

            List<ScheduleForTrainees> list = this.GetListBySql<ScheduleForTrainees>(sqlstr);

            if (list.Count > 0)
            {
                return list[0].ClassID;
            }
            else
            {
                return "无数据";
            }
        }

        /// <summary>
        /// 获取某个学生的宿舍押金(如果这个入学时间有问题，请找信息部谢彦奎，谢谢！)
        /// </summary>
        /// <param name="stuNumber"></param>
        /// <returns></returns>
        public decimal BaoxianguiStu(string stuNumber)
        {
            decimal Money = 0;
            //看学生是否住了宿舍
            string sqlstr = "select * from Accdationinformation where Studentnumber='" + stuNumber + "'";
            List<Accdationinformation> list = this.GetListBySql<Accdationinformation>(sqlstr);
            if (list.Count > 0)
            {
                //获取学生的入住时间
                string sqlstr1 = "select * from Accdationinformation where Studentnumber='" + stuNumber + "' order by CreationTime";
                List<Accdationinformation> list2 = this.GetListBySql<Accdationinformation>(sqlstr1);

                if (list.Count > 0)
                {
                    DateTime date = Convert.ToDateTime(list2[0].StayDate);

                    //判断这个学生是否退学、休学、开除
                    string strsql2 = "select * from  ApplicationDropout where Studentnumber='" + stuNumber + "'";//退学

                    List<ApplicationDropout> list3 = this.GetListBySql<ApplicationDropout>(strsql2);
                    if (list3.Count > 0)
                    {
                        DateTime dateTime = Convert.ToDateTime(list3[0].Addtime);

                        int SumMonth = (dateTime.Year - date.Year) * 12 + (dateTime.Month - date.Month);//入学日期-退学日期

                        if (SumMonth > -1)
                        {
                            SumMonth = SumMonth + 1;
                        }
                        Money = SumMonth * 10;

                        return Money;

                    }

                    string strsql3 = "select * from Suspensionofschool where Studentnumber='" + stuNumber + "'";//休学
                    List<Suspensionofschool> list4 = this.GetListBySql<Suspensionofschool>(strsql3);
                    if (list4.Count > 0)
                    {
                        DateTime date1 = list4[0].Startingperiod;//开始休学日期
                        DateTime date2 = list4[0].Deadline;//结束日期

                        int count = (date2 - date1).Days;//获取休学天数


                        int SumMonth = (date1.Year - date.Year) * 12 + (date1.Month - date.Month);//入学日期-开始休学日期

                        if (SumMonth > -1)
                        {
                            SumMonth = SumMonth + 1;
                        }

                        if (count < 15)
                        {
                            //这个月算宿舍押金
                            SumMonth = SumMonth - 1;
                        }

                        Money = SumMonth * 10;

                        return Money;
                    }

                    string strsql4 = "select * from Expels where Studentnumber='" + stuNumber + "'";//开除

                    List<Expels> list5 = this.GetListBySql<Expels>(strsql4);

                    if (list5.Count > 0)
                    {
                        DateTime dd = list5[0].Applicationtime;//开除日期

                        int SumMonth = (dd.Year - date.Year) * 12 + (dd.Month - date.Month);//入学日期-开除日期

                        Money = SumMonth * 10;

                        return Money;
                    }

                    //如果没有异常
                    string strsql6 = @" select * from Accdationinformation where DormId in(
                                        select ID from DormInformation where TungFloorId in(
                                        select tf.Id from TungFloor as tf inner join Tung as t on tf.TungId=t.Id where t.Id=27) ) and Studentnumber= '" + stuNumber + "'";

                    List<Accdationinformation> list6 = this.GetListBySql<Accdationinformation>(strsql6);

                    if (list6.Count > 0)
                    {
                        DateTime dd = list6[0].CreationTime;

                        int SumMonth = (dd.Year - date.Year) * 12 + (dd.Month - date.Month);//退住日期-入住日期

                        if (SumMonth > -1)
                        {
                            SumMonth = SumMonth + 1;
                        }
                        Money = SumMonth * 10;

                        return Money;
                    }

                }

            }

            return Money;
        }

        /// <summary>
        /// 获取某个学生的住宿信息
        /// </summary>
        /// <param name="stuNumber"></param>
        /// <returns></returns>
        public List<Accdationinformation> Stulist(string stuNumber){
            string sqlstr = "select * from Accdationinformation where Studentnumber='" + stuNumber + "' order by CreationTime";
            List<Accdationinformation> list = this.GetListBySql<Accdationinformation>(sqlstr);

            return list;
        }

    }
}
