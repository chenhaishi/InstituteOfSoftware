using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Net.Mail;
using System.Net;
using System.IO;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using System.Drawing.Imaging;
using Spire.Xls;
using NPOI.SS.Util;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using System.Diagnostics;

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
        public AjaxResult CreateSalTab(string time)
        {
            AjaxResult result = new AjaxResult();
            int id =0;
            try
            {
                var msrlist = this.GetEmpMsrData().Where(s => s.IsDel == false).ToList();
                EmployeesInfoManage empmanage = new EmployeesInfoManage();
                EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                var emplist = esemanage.GetEmpESEData().Where(s => s.IsDel == false).OrderBy(i=>i.Id).ToList();
                //    var emplist = empmanage.GetEmpInfoData();
                var nowtime = DateTime.Parse(time);

                //匹配是否有该月（选择的年月即传过来的参数）的月度工资数据
                var matchlist = msrlist.Where(m => DateTime.Parse(m.YearAndMonth.ToString()).Year == nowtime.Year && DateTime.Parse(m.YearAndMonth.ToString()).Month == nowtime.Month).ToList();

                if (matchlist.Count() <= 0)//表示月度工资表中无该月的数据
                {
                    //找到已禁用的或者该月份的员工集合 
                    var forbiddenlist = this.GetEmpMsrData().Where(s => s.IsDel == true || (DateTime.Parse(s.YearAndMonth.ToString()).Year == nowtime.Year && DateTime.Parse(s.YearAndMonth.ToString()).Month == nowtime.Month)).ToList();
                        for (int i = 0; i < forbiddenlist.Count(); i++)
                        {//将月度工资表中已禁用的员工去员工工资体系表中去除
                            emplist.Remove(emplist.Where(e => e.EmployeeId == forbiddenlist[i].EmployeeId).FirstOrDefault());
                        }
                   
                    foreach (var item in emplist)
                    {//再将未禁用的员工添加到月度工资表中
                        
                        AttendanceInfo attendance = GetAttendanceInfoByEmpid(item.EmployeeId,Convert.ToDateTime(time));
                        MeritsCheck merits = GetMCByEmpid(item.EmployeeId,Convert.ToDateTime(time));
                        MonthlySalaryRecord msr = new MonthlySalaryRecord();
                        id = (int)empmanage.GetEntity(item.EmployeeId).DDAppId;
                        msr.EmployeeId = item.EmployeeId;
                            msr.YearAndMonth = Convert.ToDateTime(time);
                        msr.FinalGrade = string.IsNullOrEmpty(merits.FinalGrade.ToString()) == true ? 0 : merits.FinalGrade;
                            msr.BaseSalary = item.BaseSalary;
                            msr.PositionSalary = item.PositionSalary;
                            msr.PerformancePay = item.PerformancePay;
                            msr.PersonalSocialSecurity = item.PersonalSocialSecurity;
                            msr.SocialSecuritySubsidy = item.SocialSecuritySubsidy;
                            msr.NetbookSubsidy = item.NetbookSubsidy;
                            msr.ContributionBase = item.ContributionBase;
                            msr.PersonalIncomeTax = item.PersonalIncomeTax;
                            msr.OvertimeCharges = attendance.OvertimeCharges;
                            msr.TardyAndLeaveWithhold = attendance.TardyAndLeaveWithhold;
                            msr.AbsenteeismWithhold = attendance.AbsenteeismWithhold;
                            msr.AbsentNumWithhold = attendance.AbsentNumWithhold;
                            msr.MonthPerformancePay = GetempPerformanceSalary(merits.FinalGrade, item.PerformancePay);
                            msr.IsDel = false;
                            msr.IsApproval = false;
                            msr.IsFinancialAudit = 0;
                            this.Insert(msr);
                            rc.RemoveCache("InRedisMSRData");
                        }
                           
                    }
                
               
                result.Success = true;
                result.Msg = "生成成功！" ;

            }

            catch (Exception ex)
            {
                    result = Error("生成失败！自工号为"+id+"的员工起截至，请查看绩效考核，考勤是否有数据。");
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

            rc.RemoveCache("InRedisMSRData");
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
        //public decimal? GetNoClockWithhold(int id, decimal? one, decimal? persalary, decimal? shouldday)
        //{
        //    AjaxResult result = new AjaxResult();
        //    decimal? countsalary = one;
        //    var msr = this.GetEntity(id);
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(shouldday.ToString()))
        //        {
        //            if (!string.IsNullOrEmpty(persalary.ToString()))
        //            {
        //                countsalary = countsalary + persalary;

        //            }
        //            countsalary = countsalary / shouldday / 2;
        //            msr.NoClockWithhold = countsalary;
        //            this.Update(msr);
        //            rc.RemoveCache("InRedisMSRData");
        //            result = this.Success();
        //            countsalary = (decimal)Math.Round(Convert.ToDouble(countsalary), 2);
        //        }
        //        else
        //        {
        //            countsalary = null;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        result = this.Error(ex.Message);
        //        countsalary = null;
        //    }

        //    return countsalary;
        //}

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FromMail">发件人地址</param>
        /// <param name="ToMail">收件人地址</param>
        /// <param name="AuthorizationCode">发件人授权码</param>
        /// <returns></returns>
        public AjaxResult WagesDataToEmail(string FromMail, string ToMail,string AuthorizationCode,MonthlySalaryRecord m)
        {

            EmployeesInfoManage manage = new EmployeesInfoManage();
            MeritsCheckManage merits = new MeritsCheckManage();
            AttendanceInfoManage attendance = new AttendanceInfoManage();
            SchoolAttendanceManagementBusiness.OvertimeRecordManage overtime = new SchoolAttendanceManagementBusiness.OvertimeRecordManage();
            var att = GetAttendanceInfoByEmpid(m.EmployeeId,Convert.ToDateTime(m.YearAndMonth));

            AjaxResult result = new AjaxResult();
            try
            {
                //实例化一个发送邮件类
                MailMessage mail = new MailMessage();
                //电子邮件的优先级
                mail.Priority = MailPriority.Normal;
                //发件人地址
                mail.From = new MailAddress(FromMail);
                //收件人地址
                mail.To.Add(new MailAddress(ToMail));
                //标题中文
                mail.SubjectEncoding = Encoding.GetEncoding(936);

                //邮件正文是否是HTML格式
                mail.IsBodyHtml = true;
                
                //邮件标题。
                mail.Subject = "您好！这是您"+Convert.ToDateTime(m.YearAndMonth).ToString("yyyy年MM月") + "的工资详情";
                //邮件内容。

                #region 邮件正文

                mail.Body = "<div style='margin:5px;'><div style='width:100%' ><h3>工资详情</h3><br/>";
                mail.Body += "<table border='1' cellspacing='0' style='text-align:center;white-space: nowrap;'>";

                mail.Body += @"<tr><th rowspan='2'>姓名</th>
                <th rowspan='2'>所属部门</th>
                <th rowspan='2'>所属岗位</th>
                <th rowspan='2'>出勤天数</th>
                <th rowspan='2'>基本工资</th>
                <th rowspan='2'>岗位工资</th>
                <th rowspan='2'>绩效分</th>
                <th rowspan='2'>绩效工资</th>
                <th rowspan='2'>笔记本补助</th>
                <th rowspan='2'>社保补贴</th>
                <th rowspan='2'>应发工资</th>
                <th rowspan='2'>加班费用</th>
                <th rowspan='2'>奖金(元)</th>
                <th rowspan='2'>请假天数</th>
                <th rowspan='2'>请假扣款(元)</th>
                <th colspan='3'>考勤扣款</th>
                <th rowspan='2'>其他扣款</th>
                <th rowspan='2'>应发工资2</th>
                <th rowspan='2'>个人社保</th>
                <th rowspan='2'>个税</th>
                <th rowspan='2'>实发工资(工资卡)</th>
                <th rowspan='2'>实发工资(现金)</th>
            </tr>
            <tr>
                <th>迟到/早退扣款</th>
                <th>缺卡扣款(元)</th>
                <th>旷工扣款(元)</th>
            </tr>";
                var SalaryOne = GetSalaryone(m.BaseSalary + m.PositionSalary, m.MonthPerformancePay, m.NetbookSubsidy, m.SocialSecuritySubsidy);
                var SalaryTwo = GetSalarytwo(SalaryOne, m.OvertimeCharges, m.Bonus, m.LeaveDeductions, m.TardyAndLeaveWithhold, m.AbsentNumWithhold, m.OtherDeductions);

                mail.Body += "<tr><td>" + manage.GetEntity(m.EmployeeId).EmpName + "</td>" +
                        "<td>" + manage.GetDeptByEmpid(m.EmployeeId).DeptName + "</td>" +
                        "<td>" + manage.GetPositionByEmpid(m.EmployeeId).PositionName + "</td>" +
                        "<td>" + att.ToRegularDays.ToString() + "</td>" +
                        "<td>" + m.BaseSalary + "</td>" +
                        "<td>" + m.PositionSalary + "</td>" +
                        "<td>" + m.FinalGrade + "</td>" +
                        "<td>" + m.MonthPerformancePay + "</td>" +
                        "<td>" + m.NetbookSubsidy + "</td>" +
                        "<td>" + m.SocialSecuritySubsidy + "</td>" +
                        "<td>" + SalaryOne + "</td>" +
                        "<td>" + m.OvertimeCharges + "</td>" +
                        "<td>" + m.Bonus + "</td>" +
                        "<td>" + att.LeaveDays + "</td>" +
                        "<td>" + m.LeaveDeductions + "</td>" +
                        "<td>" + m.TardyAndLeaveWithhold + "</td>" +
                        "<td>" + m.AbsentNumWithhold + "</td>" +
                        "<td>" + m.AbsenteeismWithhold + "</td>" +
                        "<td>" + m.OtherDeductions + "</td>" +
                        "<td>" + SalaryTwo + "</td>" +
                        "<td>" + m.PersonalSocialSecurity + "</td>" +
                        "<td>" + m.PersonalIncomeTax + "</td>" +
                        "<td>" + m.PayCardSalary + "</td>" +
                        "<td>" + m.CashSalary + "</td>" +

                   "</tr></table></div>";
                #region
                //mail.Body += "<div>绩效详情";

                //mail.Body += "<table border='1' cellspacing='0' style='text-align:center'>";

                //mail.Body += @"<tr><th>日常工作内容</th>
                //    <th>日常工作权重占比</th>
                //    <th>日常工作完成率</th>           
                //    <th>其他或领导临时指派任务</th>
                //    <th>其他工作权重占比</th>
                //    <th>其他工作完成率</th>
                //    <th>上级评分</th>
                //    <th>绩效分</th>
                //    <th>绩效工资</th>
                //</tr>";

                //var mer = merits.GetEmpMCData().Where(i => i.EmployeeId == m.EmployeeId).FirstOrDefault();
                ////绩效工资
                //mail.Body += "<tr><td>" + mer.RoutineWork + "</td>" +
                //            "<td>" + mer.RoutineWorkPropotion + "%</td>" +
                //            "<td>" + mer.RoutineWorkFillRate + "%</td>" +
                //            "<td>" + mer.OtherWork + "</td>" +
                //            "<td>" + mer.OtherWorkPropotion + "%</td>" +
                //            "<td>" + mer.OtherWorkFillRate + "%</td>" +
                //            "<td>" + mer.SuperiorGrade + "</td>" +
                //            "<td>" + m.FinalGrade + "</td>" +
                //            "<td>" + m.MonthPerformancePay + "</td>" +
                //        "</tr></table></div>";
                #endregion



                mail.Body += "<div><h3>考勤详情</h3><br/><h4>迟到早退</h4><table border='1' cellspacing='0' style='text-align:center'>";

                mail.Body += @"<tr><th>详情</th>
                         <th>内容</th>
                         <th>金额</th>
                     </tr>";

                
                ////迟到早退
                if (!att.TardyAndLeaveWithhold.IsNullOrEmpty())
                {
                    mail.Body += "<tr><td>迟到/早退扣款</td>" +
                        "<td>迟到记录:" + att.TardyRecord + "<br/>" +
                        "迟到次数:" + att.TardyNum + "<br/>" +
                        "早退记录:" + att.LeaveEarlyRecord + "<br/>" +
                        "早退次数:" + att.LeaveEarlyNum +
                        "</td>" +
                        "<td>" + att.TardyAndLeaveWithhold + "</td>" +
                        "</tr>";
                }
                //旷工
                if (!att.AbsenteeismWithhold.IsNullOrEmpty())
                {
                    mail.Body += "<tr><td>旷工扣款</td>" +
                       "<td>旷工记录:" + att.AbsenteeismRecord + "<br/>" +
                       "旷工天数:" + att.AbsenteeismDays +
                       "</td>" +
                       "<td>" + att.AbsenteeismWithhold + "</td>" +
                       "</tr>";
                }
                //缺卡
                if (!m.AbsentNumWithhold.IsNullOrEmpty())
                {
                    mail.Body += "<tr><td>缺卡扣款</td>" +
                      "<td>上班缺卡记录:" + att.WorkAbsentRecord + "<br/>" +
                      "上班缺卡次数:" + att.WorkAbsentNum + "<br/>" +
                      "中午缺卡记录:" + att.NoonAbsentRecord + "<br/>" +
                      "中午缺卡次数:" + att.NoonAbsentNum + "<br/>" +
                      "下班缺卡记录:" + att.OffDutyAbsentRecord + "<br/>" +
                      "下班缺卡次数:" + att.OffDutyAbsentNum +
                      "</td>" +
                      "<td>" + m.AbsentNumWithhold + "</td>" +
                      "</tr>";
                }
                //请假
                if (!m.LeaveDeductions.IsNullOrEmpty())
                {
                    mail.Body += "<tr><td>请假扣款</td>" +
                     "<td>请假记录:" + att.LeaveRecord + "<br/>" +
                     "请假天数:" + att.LeaveDays +
                     "</td>" +
                     "<td>" + m.LeaveDeductions + "</td>" +
                     "</tr>";
                }
                //加班
                var over = overtime.GetList().Where(i=>i.EmployeeId==m.EmployeeId&&Convert.ToDateTime(i.YearAndMonth).Year== Convert.ToDateTime(m.YearAndMonth).Year&& Convert.ToDateTime(i.YearAndMonth).Month == Convert.ToDateTime(m.YearAndMonth).Month);
                
                if (!att.OvertimeCharges.IsNullOrEmpty())
                {
                    mail.Body += "<tr><td>加班费用</td><td>";
                    foreach (var i in over)
                    {
                        var type = "";
                        switch (i.OvertimeTypeId)
                        {
                            case 1:
                                type = "晚上加班";
                                break;
                            case 2:
                                type = "周末加班";
                                break;
                            case 3:
                                type = "法定节假日加班";
                                break;
                            case 4:
                                type = "行政值班";
                                break;
                            default:
                                break;
                        }
                        mail.Body +="加班记录:" + i.StartTime+"至"+i.EndTime+"<br/>";
                        mail.Body += "加班类型:"+type+"<br/>";
                        mail.Body += "加班时长:" + i.Duration + "<br/>";
                    }
                    mail.Body += "</td><td>" + att.OvertimeCharges + "</td>" +
                      "</tr>";
                }
                //调休
                if (!att.DaysoffRecord.IsNullOrEmpty() && !att.DaysoffDuration.IsNullOrEmpty())
                {
                    mail.Body += "<tr><td>调休</td>" +
                     "<td>调休记录:" + att.DaysoffRecord + "<br/>" +
                     "调休时长:" + att.DaysoffDuration +
                     "</td><td></td></tr>";
                }
               //外出
                if (!att.GoOutRecord.IsNullOrEmpty() && !att.GoOutNum.IsNullOrEmpty())
                {
                    mail.Body += "<tr><td>外出</td>" +
                     "<td>外出记录:" + att.GoOutRecord + "<br/>" +
                     "外出次数:" + att.GoOutNum +
                     "</td><td></td></tr>";
                }
                //出差
                if (!att.EvectionRecord.IsNullOrEmpty() && !att.EvectionNum.IsNullOrEmpty())
                {
                    mail.Body += "<tr><td>出差</td>" +
                     "<td>出差记录:" + att.EvectionRecord + "<br/>" +
                     "出差次数:" + att.EvectionNum +
                     "</td><td></td></tr>";
                }

                mail.Body += "</table></div></div>";

                #endregion
                //实例化一个SmtpClient类。
                SmtpClient client = new SmtpClient();

                #region 设置邮件服务器地址

                if (FromMail.Length != 0)
                {
                    //根据发件人的邮件地址判断发件服务器地址   默认端口一般是25
                    string[] addressor = FromMail.Trim().Split(new Char[] { '@', '.' });
                    switch (addressor[1])
                    {
                        case "163":
                            client.Host = "smtp.163.com";
                            break;
                        case "126":
                            client.Host = "smtp.126.com";
                            break;
                        case "qq":
                            client.Host = "smtp.qq.com";
                            break;
                        case "gmail":
                            client.Host = "smtp.gmail.com";
                            break;
                        case "hotmail":
                            client.Host = "smtp.live.com";//outlook邮箱
                            break;
                        case "foxmail":
                            client.Host = "smtp.foxmail.com";
                            break;
                        case "sina":
                            client.Host = "smtp.sina.com.cn";
                            break;
                        default:
                            client.Host = "smtp.exmail.qq.com";//qq企业邮箱
                            break;
                    }
                }
                #endregion

                //使用安全加密连接。
                client.EnableSsl = true;
                //不和请求一块发送。
                client.UseDefaultCredentials = false;

                //验证发件人身份(发件人的邮箱，邮箱里的生成授权码);
                client.Credentials = new NetworkCredential(FromMail, AuthorizationCode);

                //如果发送失败，SMTP 服务器将发送 失败邮件告诉我  
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                //发送
                client.Send(mail);

                result.Success = true;
                //result.ErrorCode = 200;
                result.Msg = "邮件发送成功";
            }
            catch (Exception e)
            {
                result.Success = false;
                //result.ErrorCode = 100;
                result.Msg =e.Message;

            }
            return result;
        }

        public AjaxResult Month(List<MonthlySalaryRecord> data)
        {

            var ajaxresult = new AjaxResult();

            var workbook = new HSSFWorkbook();
            EmployeesInfoManage manage = new EmployeesInfoManage();
            AttendanceInfoManage attendance = new AttendanceInfoManage();

            //创建工作区
            var sheet = workbook.CreateSheet();

            #region 表头样式

            HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

            HeadercellStyle.Alignment = HorizontalAlignment.Center;
            HeadercellStyle.VerticalAlignment = VerticalAlignment.Center;
            HeadercellFont.IsBold = true;

            HeadercellStyle.SetFont(HeadercellFont);

            #endregion


            HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

            ContentcellStyle.Alignment = HorizontalAlignment.Center;

            CreateHeader();

            int num = 2;
            var YearAndMonth = "";
            data.ForEach(d =>
            {
                var row = (HSSFRow)sheet.CreateRow(num);
                YearAndMonth = d.YearAndMonth.ToString();

                var Salaryone = GetSalaryone(d.BaseSalary + d.PositionSalary, d.MonthPerformancePay, d.NetbookSubsidy, d.SocialSecuritySubsidy);
                var Salarytwo = GetSalarytwo(Salaryone, d.OvertimeCharges, d.Bonus, d.LeaveDeductions, d.TardyAndLeaveWithhold, d.AbsentNumWithhold, d.OtherDeductions);
                var PaycardSalary = GetPaycardSalary(d.Id, d.Total, d.PersonalSocialSecurity, d.ContributionBase);

                CreateCell(row, ContentcellStyle, 0, d.EmployeeId);//员工编号
                CreateCell(row, ContentcellStyle, 1, manage.GetEntity(d.EmployeeId).EmpName);//员工姓名
                CreateCell(row, ContentcellStyle, 2, manage.GetDeptByEmpid(d.EmployeeId).DeptName);//所属部门
                CreateCell(row, ContentcellStyle, 3, manage.GetPositionByEmpid(d.EmployeeId).PositionName);//所属岗位
                CreateCell(row, ContentcellStyle, 4, d.BaseSalary.ToString());//基本工资
                CreateCell(row, ContentcellStyle, 5, d.PositionSalary.ToString());//岗位工资
                CreateCell(row, ContentcellStyle, 6, d.FinalGrade.ToString());//绩效分
                CreateCell(row, ContentcellStyle, 7, d.MonthPerformancePay.ToString());//绩效工资
                CreateCell(row, ContentcellStyle, 8, d.NetbookSubsidy.ToString());//笔记本补助
                CreateCell(row, ContentcellStyle, 9, d.SocialSecuritySubsidy.ToString());//社保补贴
                CreateCell(row, ContentcellStyle, 10, Salaryone.ToString());//应发工资1
                CreateCell(row, ContentcellStyle, 11, d.OvertimeCharges.ToString());//加班费用
                CreateCell(row, ContentcellStyle, 12, d.Bonus.ToString());//奖金(元)
                CreateCell(row, ContentcellStyle, 13, GetAttendanceInfoByEmpid(d.EmployeeId,(DateTime)d.YearAndMonth).LeaveDays.ToString());//请假天数
                CreateCell(row, ContentcellStyle, 14, d.LeaveDeductions.ToString());//请假扣款(元)
                CreateCell(row, ContentcellStyle, 15, d.TardyAndLeaveWithhold.ToString());//迟到/早退扣款(元)
                CreateCell(row, ContentcellStyle, 16, d.AbsentNumWithhold.ToString());//缺卡扣款(元)
                CreateCell(row, ContentcellStyle, 17, d.AbsenteeismWithhold.ToString());//旷工扣款(元)
                CreateCell(row, ContentcellStyle, 18, d.OtherDeductions.ToString());//其他扣款(元)
                CreateCell(row, ContentcellStyle, 19, Salarytwo.ToString());//应发工资2
                CreateCell(row, ContentcellStyle, 20, d.PersonalSocialSecurity.ToString());//个人社保
                CreateCell(row, ContentcellStyle, 21, d.PersonalIncomeTax.ToString());//个税
                CreateCell(row, ContentcellStyle, 22, PaycardSalary.ToString());//实发工资(工资卡)
                CreateCell(row, ContentcellStyle, 23, GetCashSalary(d.Id,d.Total, PaycardSalary).ToString());//实发工资(现金)
                num++;

            });

            string path = System.AppDomain.CurrentDomain.BaseDirectory.Split('\\')[0];    //获得项目的基目录
            //var s = path.Split('\\'); 
            //var mypath = s[0];
            var Path = System.IO.Path.Combine(path, @"\XinxihuaData\Excel"); //进到基目录录找“Uploadss->Excel”文件夹

            if (!System.IO.Directory.Exists(Path))     //判断是否有该文件夹
                System.IO.Directory.CreateDirectory(Path); //如果没有在Uploads文件夹下创建文件夹Excel
            string saveFileName = Path + "\\" + Convert.ToDateTime(YearAndMonth).ToString("yyyy年MM月") + "员工工资" + ".xls"; //路径+表名+文件类型
            //}
            try
            {
                FileStream fs = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);  //写入文件
                workbook.Close();  //关闭
                ajaxresult.ErrorCode = 200;
                ajaxresult.Msg = "导入成功！文件地址：" + saveFileName;
                //Process.Start(saveFileName);
                // ajaxresult.Data = list;

            }
            catch (Exception ex)
            {
                ajaxresult.ErrorCode = 100;
                ajaxresult.Msg = "导入失败，" + ex.Message;

            }
            return ajaxresult;
            void CreateHeader()
            {
                HSSFRow Header = (HSSFRow)sheet.CreateRow(0);

                for (int i = 0; i < 24; i++)
                {
                    if (i<15||i>17)
                    {
                        sheet.AddMergedRegion(new CellRangeAddress(0, 1, i, i));
                    }
                }
                sheet.AddMergedRegion(new CellRangeAddress(0, 0, 15, 17));

                CreateCell(Header, HeadercellStyle, 0, "员工编号");
                CreateCell(Header, HeadercellStyle, 1, "员工姓名");
                CreateCell(Header, HeadercellStyle, 2, "所属部门");
                CreateCell(Header, HeadercellStyle, 3, "所属岗位");
                CreateCell(Header, HeadercellStyle, 8, "出勤天数");
                CreateCell(Header, HeadercellStyle, 4, "基本工资");
                CreateCell(Header, HeadercellStyle, 5, "岗位工资");
                CreateCell(Header, HeadercellStyle, 6, "绩效分");
                CreateCell(Header, HeadercellStyle, 7, "绩效工资");
                CreateCell(Header, HeadercellStyle, 8, "笔记本补助");
                CreateCell(Header, HeadercellStyle, 9, "社保补贴");
                CreateCell(Header, HeadercellStyle, 10, "应发工资1");
                CreateCell(Header, HeadercellStyle, 11, "加班费用");
                CreateCell(Header, HeadercellStyle, 12, "奖金(元)");
                CreateCell(Header, HeadercellStyle, 13, "请假天数");
                CreateCell(Header, HeadercellStyle, 14, "请假扣款(元)");
                CreateCell(Header, HeadercellStyle, 15, "考勤扣款");
                CreateCell(Header, HeadercellStyle, 18, "其他扣款(元)");
                CreateCell(Header, HeadercellStyle, 19, "应发工资2");
                CreateCell(Header, HeadercellStyle, 20, "个人社保");
                CreateCell(Header, HeadercellStyle, 21, "个税");
                CreateCell(Header, HeadercellStyle, 22, "实发工资(工资卡)");
                CreateCell(Header, HeadercellStyle, 23, "实发工资(现金)");
                HSSFRow Header2 = (HSSFRow)sheet.CreateRow(1);
                CreateCell(Header2, HeadercellStyle, 15, "迟到/早退扣款(元)");
                CreateCell(Header2, HeadercellStyle, 16, "缺卡扣款(元)");
                CreateCell(Header2, HeadercellStyle, 17, "旷工扣款(元)");

            }

            void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
            {
                HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                Header_Name.SetCellValue(value);

                Header_Name.CellStyle = TcellStyle;
            }
        }

        
    } 
    }

