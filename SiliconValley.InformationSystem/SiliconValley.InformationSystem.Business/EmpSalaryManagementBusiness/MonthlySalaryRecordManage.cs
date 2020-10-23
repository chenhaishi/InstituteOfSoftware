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
                var emplist = esemanage.GetEmpESEData().Where(s => s.IsDel == false).ToList();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public AjaxResult ExcelToImg(MonthlySalaryRecord m)
        {
            AjaxResult result = new AjaxResult();
            Workbook workbook = new Workbook();
            EmployeesInfoManage manage = new EmployeesInfoManage();

            string name = manage.GetEntity(m.EmployeeId).EmpName + m.EmployeeId;
            //workbook.LoadFromFile("C:/XinxihuaData/PaySlipExcel2020-10/" +name+ ".xls");
            Worksheet sheet = workbook.Worksheets[0];

            CreateHeader();

            CreateCell(2, 1, manage.GetEntity(m.EmployeeId).EmpName);
            CreateCell(2, 2, manage.GetPositionByEmpid(m.EmployeeId).PositionName);
            CreateCell(2, 3, m.PerformancePay.ToString());
            CreateCell(2, 4, m.PositionSalary.ToString());
            CreateCell(2, 5, m.BaseSalary.ToString());


            //获得项目的基目录
            string path = System.AppDomain.CurrentDomain.BaseDirectory.Split('\\')[0];

            var Path = System.IO.Path.Combine(path, @"\XinxihuaData\PaySlipImage" + DateTime.Now.ToString("yyyy-MM")); //进到基目录录找“Uploadss->PaySlipImage”文件夹

            if (!System.IO.Directory.Exists(Path))     //判断是否有该文件夹
                System.IO.Directory.CreateDirectory(Path); //如果没有在Uploads文件夹下创建文件夹PaySlipImage
            string saveFileName = Path + "\\" + name+"." + ImageFormat.Jpeg;
            try
            {
                sheet.SaveToImage(saveFileName);

                result.Success = true;
                result.ErrorCode = 200;
                result.Msg = "导入成功！文件位置" + saveFileName;
            }
            catch (Exception e)
            {
                result.Success = true;
                result.ErrorCode = 200;
                result.Msg = "导入失败";
            }

            return result;

            void CreateHeader()
            {
                CreateCell(1, 1, "姓名");
                CreateCell(1, 2, "岗位");
                CreateCell(1, 3, "到勤天数");
                CreateCell(1, 4, "基本工资");
                CreateCell(1, 5, "岗位工资");
                CreateCell(1, 6, "绩效分");
                CreateCell(1, 7, "绩效工资");
                CreateCell(1, 8, "笔记本补助");
                CreateCell(1, 9, "社保补贴");
                CreateCell(1, 10, "应发工资");

            }
            void CreateCell(int row, int index, string value)
            {

                sheet.Range[row, index].Value = value;
                sheet.AllocatedRange.AutoFitRows();
                sheet.AllocatedRange.AutoFitColumns();
                sheet.Range[1, index].Style.Font.IsBold = true;
                var style = sheet.Range[row, index].Style;
                style.HorizontalAlignment = HorizontalAlignType.Center;
                style.VerticalAlignment = VerticalAlignType.Center;
                
                style.Font.IsBold = false;
                style.Font.Size = 12;
                style.Font.FontName = "宋体";
                style.Borders[BordersLineType.EdgeLeft].LineStyle = LineStyleType.Thin;
                style.Borders[BordersLineType.EdgeRight].LineStyle = LineStyleType.Thin;
                style.Borders[BordersLineType.EdgeTop].LineStyle = LineStyleType.Thin;
                style.Borders[BordersLineType.EdgeBottom].LineStyle = LineStyleType.Thin;
            }

        }

    

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FromMail">发件人地址</param>
        /// <param name="ToMail">收件人地址</param>
        /// <param name="AuthorizationCode">发件人授权码</param>
        /// <returns></returns>
        public AjaxResult WagesDataToEmail(string FromMail, string ToMail, string name,string EmployeeId, string AuthorizationCode)
        {

            string File_Path = "C:/XinxihuaData/PaySlipImage2020-10/" + name + EmployeeId + ".Jpeg";
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
                mail.Subject = "guigu";
                //邮件内容。
                mail.Body = name+"，您好，这是您"+DateTime.Now.ToString("yyyy年MM月")+"的工资<br/>";

                //设置邮件的附件，将在客户端选择的附件先上传到服务器保存一个，然后加入到mail中  
                if (File_Path != "" && File_Path != null)
                {

                    //Attachment attachment = new System.Net.Mail.Attachment(File_Path);
                    //将附件添加到邮件
                    Attachment attachment = new System.Net.Mail.Attachment(File_Path);
                    mail.Attachments.Add(attachment);
                    mail.Body += "<img src=\"cid:" + attachment.ContentId + "\"/>";

                    //获取或设置此电子邮件的发送通知。
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                }

                //实例化一个SmtpClient类。
                SmtpClient client = new SmtpClient();

                #region 设置邮件服务器地址

                //在这里我使用的是163邮箱，所以是smtp.163.com，如果你使用的是qq邮箱，那么就是smtp.qq.com。
                // client.Host = "smtp.163.com";

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
                            //client.Port = 587;
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
                result.ErrorCode = 200;
                result.Msg = "邮件发送成功";
            }
            catch (Exception)
            {
                result.Success = false;
                result.ErrorCode = 100;
                result.Msg = "邮件发送失败";

            }
            return result;
        }

    } 
    }

