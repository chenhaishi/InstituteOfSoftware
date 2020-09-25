using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
namespace SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness
{
    public class MonthlySalaryRecordManage : BaseBusiness<MonthlySalaryRecord>
    {
        RedisCache rc = new RedisCache();
        /// <summary>
        /// 将员工月度工资表数据存储到redis服务器中
        /// </summary>
        /// <returns></returns>
        public List<MonthlySalaryRecord> GetEmpMsrData()
        {
            rc.RemoveCache("InRedisMSRData");
            List<MonthlySalaryRecord> msrlist = new List<MonthlySalaryRecord>();

            if (msrlist == null || msrlist.Count == 0)
            {
                msrlist = this.GetList();
                rc.SetCache("InRedisMSRData", msrlist);
            }
            msrlist = rc.GetCache<List<MonthlySalaryRecord>>("InRedisMSRData");
            return msrlist;

        }

        /// <summary>
        /// 往员工月度工资表加入员工编号及年月份属性
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        //public bool AddEmpToEmpMonthSalary(string empid)
        //{ 
        //    bool result = false;
        //    try
        //    {
        //        MonthlySalaryRecord ese = new MonthlySalaryRecord();
        //        ese.EmployeeId = empid;
        //        ese.IsDel = false;
        //        ese.YearAndMonth = DateTime.Now;
        //        if (CreateSalTab(ese.YearAndMonth.ToString())) {
        //            this.Insert(ese); 
        //            rc.RemoveCache("InRedisMSRData");
        //            result = true;
        //            BusHelper.WriteSysLog("月度工资表添加员工成功", Entity.Base_SysManage.EnumType.LogType.添加数据);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
        //    }
        //    return result;

        //}

        /// <summary>
        /// 去除员工月度工资表中的该员工
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool EditEmpMS(int id)
        {
            var ems = this.GetEntity(id);
            bool result = false;
            try
            {
                ems.IsDel = true;
                this.Update(ems);
                rc.RemoveCache("InRedisMSRData");
                result = true;
                BusHelper.WriteSysLog("月度工资表禁用该员工", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {
                result = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;
        }

        /// <summary>
        /// 根据员工编号获取员工体系表中该员工对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public EmplSalaryEmbody GetEmpsalaryByEmpid(string empid)
        {
            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();

            var ese = esemanage.GetEmpESEData().Where(s => s.EmployeeId == empid).FirstOrDefault();
            return ese;
        }

        /// <summary>
        /// 根据员工编号获取考勤统计表中对应月份的该员工对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public AttendanceInfo GetAttendanceInfoByEmpid(string empid, DateTime time)
        {
            AttendanceInfoManage attmanage = new AttendanceInfoManage();
            var att = attmanage.GetADInfoData().Where(s => s.EmployeeId == empid && DateTime.Parse(s.YearAndMonth.ToString()).Year == time.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == time.Month).FirstOrDefault();
            return att;
        }


        /// <summary>
        /// 根据员工编号获取绩效考核表中的该员工对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public MeritsCheck GetMCByEmpid(string empid, DateTime time)
        {
            MeritsCheckManage mcmanage = new MeritsCheckManage();
            var mcobj = mcmanage.GetEmpMCData().Where(s => s.EmployeeId == empid && DateTime.Parse(s.YearAndMonth.ToString()).Year == time.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == time.Month).FirstOrDefault();
            return mcobj;
        }

        //工资表生成的方法
        public bool CreateSalTab(string time)
        {
            bool result = false;
            try
            {
                var msrlist = this.GetEmpMsrData().Where(s => s.IsDel == false).ToList();
               // EmployeesInfoManage empmanage = new EmployeesInfoManage();
                EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                var emplist = esemanage.GetEmpESEData().Where(s=>s.IsDel==false).ToList();
            //    var emplist = empmanage.GetEmpInfoData();
                var nowtime = DateTime.Parse(time);

                //匹配是否有该月（选择的年月即传过来的参数）的月度工资数据
                var matchlist = msrlist.Where(m => DateTime.Parse(m.YearAndMonth.ToString()).Year == nowtime.Year && DateTime.Parse(m.YearAndMonth.ToString()).Month == nowtime.Month).ToList();

                if (matchlist.Count() <= 0)//表示月度工资表中无该月的数据
                {
                    //找到已禁用的或者该月份的员工集合
                    var forbiddenlist = this.GetEmpMsrData().Where(s => s.IsDel == true || (DateTime.Parse(s.YearAndMonth.ToString()).Year == nowtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == nowtime.Month)).ToList();

                    for (int i = 0; i < forbiddenlist.Count(); i++)
                    {//将月度工资表中已禁用的员工去员工表中去除
                        emplist.Remove(emplist.Where(e => e.EmployeeId == forbiddenlist[i].EmployeeId).FirstOrDefault());
                    }
                    foreach (var item in emplist)
                    {//再将未禁用的员工添加到月度工资表中
                        MonthlySalaryRecord msr = new MonthlySalaryRecord();
                        msr.EmployeeId = item.EmployeeId;
                        msr.YearAndMonth = Convert.ToDateTime(time);
                        msr.IsDel = false;
                        msr.IsApproval = false;
                        this.Insert(msr);
                        rc.RemoveCache("InRedisMSRData");
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;

            }
            return result;
        }

        /// <summary>
        /// 计算绩效工资
        /// </summary>
        /// <param name="finalGrade">绩效分</param>
        /// <param name="performancelimit">绩效额度</param>
        /// <returns></returns>
        public decimal? GetempPerformanceSalary(decimal? finalGrade, decimal? performancelimit)
        {
            decimal? result;
            if (finalGrade == 100)
            {
                result = performancelimit;
            }
            else if (finalGrade < 100)
            {
                result = performancelimit - performancelimit * (1 - finalGrade * (decimal)0.01);
            }
            else
            {
                result = performancelimit * (finalGrade * (decimal)0.01);
            }
            return result;
        }
        /// <summary>
        /// 计算应发工资1
        /// </summary>
        /// <param name="one">基本工资+岗位工资</param>
        /// <param name="PerformanceSalary">绩效工资</param>
        /// <param name="netbookSubsidy">笔记本补助</param>
        /// <param name="socialSecuritySubsidy">社保补贴</param>
        /// <returns></returns>
        public decimal? GetSalaryone(decimal? one, decimal? PerformanceSalary, decimal? netbookSubsidy, decimal? socialSecuritySubsidy)
        {
            decimal? SalaryOne = one;
            if (!string.IsNullOrEmpty(PerformanceSalary.ToString()))
            {
                SalaryOne = SalaryOne + PerformanceSalary;
            }
            if (!string.IsNullOrEmpty(netbookSubsidy.ToString()))
            {
                SalaryOne = SalaryOne + netbookSubsidy;
            }
            if (!string.IsNullOrEmpty(socialSecuritySubsidy.ToString()))
            {
                SalaryOne = SalaryOne + socialSecuritySubsidy;
            }

            return SalaryOne;
        }


        /// <summary>
        /// 计算请假扣款
        /// </summary>
        /// <param name="one">基本工资+岗位工资</param>
        /// <param name="persalary">绩效工资</param>
        /// <param name="shouldday">应出勤天数</param>
        /// <param name="leaveday">请假天数</param>
        /// <returns></returns>
        public decimal? GetLeaveDeductions(int id, decimal? one, decimal? persalary, decimal? shouldday, decimal? leaveday)
        {
            AjaxResult result = new AjaxResult();
            decimal? countsalary = one;
            var msr = this.GetEntity(id);
            try
            {
                if (!string.IsNullOrEmpty(shouldday.ToString()))
                {
                    if (!string.IsNullOrEmpty(persalary.ToString()))
                    {
                        countsalary = countsalary + persalary;
                    }

                    if (!string.IsNullOrEmpty(leaveday.ToString()))
                    {
                        countsalary = countsalary / shouldday * leaveday;

                        msr.LeaveDeductions = countsalary;
                        this.Update(msr);
                        rc.RemoveCache("InRedisMSRData");
                        result = this.Success();
                        countsalary = (decimal)Math.Round(Convert.ToDouble(countsalary), 2);
                    }
                    else
                    {
                        countsalary = null;
                    }
                }
                else
                {
                    countsalary = null;
                }


            }
            catch (Exception ex)
            {
                result = this.Error(ex.Message);
                countsalary = null;
            }

            return countsalary;
        }

        /// <summary>
        /// 计算缺卡扣款
        /// </summary>
        /// <param name="one">基本工资+岗位工资</param>
        /// <param name="persalary">绩效工资</param>
        /// <param name="shouldday">应出勤天数</param>
        /// <param name="leaveday">请假天数</param>
        /// <returns></returns>
        public decimal? GetNoClockWithhold(int id, decimal? one, decimal? persalary, decimal? shouldday)
        {
            AjaxResult result = new AjaxResult();
            decimal? countsalary = one;
            var msr = this.GetEntity(id);
            try
            {
                if (!string.IsNullOrEmpty(shouldday.ToString()))
                {
                    if (!string.IsNullOrEmpty(persalary.ToString()))
                    {
                        countsalary = countsalary + persalary;

                    }
                    countsalary = countsalary / shouldday / 2;
                    msr.NoClockWithhold = countsalary;
                    this.Update(msr);
                    rc.RemoveCache("InRedisMSRData");
                    result = this.Success();
                    countsalary = (decimal)Math.Round(Convert.ToDouble(countsalary), 2);
                }
                else
                {
                    countsalary = null;
                }


            }
            catch (Exception ex)
            {
                result = this.Error(ex.Message);
                countsalary = null;
            }

            return countsalary;
        }


        /// <summary>
        /// 计算应发工资2
        /// </summary>
        /// <param name="salaryone">应发工资1</param>
        /// <param name="OvertimeCharges">加班费用</param>
        /// <param name="Bonus">奖金/元</param>
        /// <param name="LeaveDeductions">请假扣款</param>
        /// <param name="TardyAndLeaveWithhold">迟到扣款</param>
        ///// <param name="LeaveWithhold">早退扣款</param>
        ///  /// <param name="NoClockWithhold">缺卡扣款</param>
        /// <param name="OtherDeductions">其他扣款</param>
        /// <returns></returns>
        public decimal? GetSalarytwo(decimal? salaryone, decimal? OvertimeCharges, decimal? Bonus, decimal? LeaveDeductions, decimal? TardyAndLeaveWithhold/*, decimal? LeaveWithhold*/, decimal? NoClockWithhold, decimal? OtherDeductions)
        {
            decimal? SalaryTwo = salaryone;
            if (!string.IsNullOrEmpty(OvertimeCharges.ToString()))
            {
                SalaryTwo = SalaryTwo + OvertimeCharges;
            }
            if (!string.IsNullOrEmpty(Bonus.ToString()))
            {
                SalaryTwo = SalaryTwo + Bonus;
            }
            if (!string.IsNullOrEmpty(LeaveDeductions.ToString()))
            {
                SalaryTwo = SalaryTwo - LeaveDeductions;
            }
            if (!string.IsNullOrEmpty(TardyAndLeaveWithhold.ToString()))
            {
                SalaryTwo = SalaryTwo - TardyAndLeaveWithhold;
            }
            //if (!string.IsNullOrEmpty(LeaveWithhold.ToString()))
            //{
            //    SalaryTwo = SalaryTwo - LeaveWithhold;
            //}
            if (!string.IsNullOrEmpty(NoClockWithhold.ToString()))
            {
                SalaryTwo = SalaryTwo - NoClockWithhold;
            }
            if (!string.IsNullOrEmpty(OtherDeductions.ToString()))
            {
                SalaryTwo = SalaryTwo - OtherDeductions;
            }

            return SalaryTwo;
        }

        /// <summary>
        /// 计算工资合计
        /// </summary>
        /// <param name="id">月度工资编号</param>
        /// <param name="salarytwo">应发工资2</param>
        /// <param name="PersonalSocialSecurity">个人社保</param>
        /// <param name="PersonalIncomeTax">个税</param>
        /// <returns></returns>
        public decimal? GetTotal(int id, decimal? salarytwo, decimal? PersonalSocialSecurity, decimal? PersonalIncomeTax)
        {
            AjaxResult result = new AjaxResult();
            decimal? Total = salarytwo;
            if (!string.IsNullOrEmpty(PersonalSocialSecurity.ToString()))
            {
                Total = Total - PersonalSocialSecurity;
            }
            if (!string.IsNullOrEmpty(PersonalIncomeTax.ToString()))
            {
                Total = Total - PersonalIncomeTax;
            }
            try
            {
                var msr = this.GetEntity(id);
                msr.Total = Total;
                this.Update(msr);
                rc.RemoveCache("InRedisMSRData");
                result = this.Success();
            }
            catch (Exception ex)
            {
                result = this.Error(ex.Message);
            }
            return Total;
        }

        /// <summary>
        /// 计算工资卡实发工资
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total">合计</param>
        /// <param name="PersonalSocialSecurity">个人社保</param>
        /// /// <param name="PersonalSocialSecurity">社保缴费基数</param>
        /// <returns></returns>
        public decimal? GetPaycardSalary(int id, decimal? total, decimal? PersonalSocialSecurity, decimal? ContributionBase)
        {
            decimal? paycardsalary = null;
            if (!string.IsNullOrEmpty(PersonalSocialSecurity.ToString()))
            {

                var msr = this.GetEntity(id);
                if (total <= ContributionBase)
                {
                    paycardsalary = total;
                }
                else
                {
                    paycardsalary = ContributionBase - PersonalSocialSecurity;
                }
                msr.PayCardSalary = paycardsalary;
                this.Update(msr);
                rc.RemoveCache("InRedisMSRData");
            }
            else
            {
                paycardsalary = null;
            }
            return paycardsalary;
        }

        /// <summary>
        /// 计算现金实发工资
        /// </summary>
        /// <param name="id"></param>
        /// <param name="total">合计</param>
        /// <param name="PaycardSalary">工资卡工资</param>
        /// <returns></returns>
        public decimal? GetCashSalary(int id, decimal? total, decimal? PaycardSalary)
        {

            decimal? cash = 0;
            try
            {
                if (!string.IsNullOrEmpty(PaycardSalary.ToString()))
                {
                    var msr = this.GetEntity(id);
                    msr.CashSalary = total - PaycardSalary;
                    this.Update(msr);
                    rc.RemoveCache("InRedisMSRData");
                    cash = msr.CashSalary;
                }
                else
                {
                    cash = total;
                }
            }
            catch (Exception)
            {

                cash = null;
            }

            return cash;
        }

    }
}
