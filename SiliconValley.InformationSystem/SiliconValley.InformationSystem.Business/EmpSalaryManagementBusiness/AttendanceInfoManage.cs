using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness
{
    using System.IO;
    using NPOI.SS.UserModel;
    using NPOI.HSSF.UserModel;
    using NPOI.XSSF.UserModel;
    using SiliconValley.InformationSystem.Entity.ViewEntity.SalaryView;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Entity.ViewEntity;

    public class AttendanceInfoManage : BaseBusiness<AttendanceInfo>
    {
        RedisCache rc = new RedisCache();
        /// <summary>
        /// 将员工考勤表数据存储到redis服务器中
        /// </summary>
        /// <returns></returns>
        public List<AttendanceInfo> GetADInfoData()
        {
            rc.RemoveCache("InRedisATDData");
            List<AttendanceInfo> atdinfolist = new List<AttendanceInfo>();
            if (atdinfolist == null || atdinfolist.Count == 0)
            {
                atdinfolist = this.GetList();
                rc.SetCache("InRedisATDData", atdinfolist);
            }
            atdinfolist = rc.GetCache<List<AttendanceInfo>>("InRedisATDData");
            return atdinfolist;
        }
        /// <summary>
        /// 员工入职时往员工考勤表加入该员工
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        //public bool AddEmpToAttendanceInfo(string empid)
        //{
        //    bool result = false;
        //    try
        //    {
        //        AttendanceInfo ese = new AttendanceInfo();
        //        ese.EmployeeId = empid;
        //        ese.IsDel = false;
        //        ese.YearAndMonth = DateTime.Now;
        //        this.Insert(ese);
        //        rc.RemoveCache("InRedisATDData");
        //        result = true;
        //        BusHelper.WriteSysLog("考勤表添加员工成功", Entity.Base_SysManage.EnumType.LogType.添加数据);

        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
        //    }
        //    return result;

        //}

        /// <summary>
        /// 编辑考勤表禁用员工
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool EditEmpStateToAds(string empid,string time)
        {
            bool result = false;
            try
            {
                var ymtime = DateTime.Parse(time);
                var ads = this.GetADInfoData().Where(e => e.EmployeeId == empid &&DateTime.Parse(e.YearAndMonth.ToString()).Year==ymtime.Year&& DateTime.Parse(e.YearAndMonth.ToString()).Month==ymtime.Month).FirstOrDefault();
                ads.IsDel = true;
                this.Update(ads);
                rc.RemoveCache("InRedisATDData");
                result = true;
                BusHelper.WriteSysLog("考勤表禁用员工成功", Entity.Base_SysManage.EnumType.LogType.编辑数据);

            }
            catch (Exception ex)
            {
                result = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;

        }

      
        public AjaxResult ImportDataFormExcel(Stream stream, string contentType)
        {

            IWorkbook workbook = null;

            if (contentType == "application/vnd.ms-excel")
            {
                workbook = new HSSFWorkbook(stream);
            }

            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(0);
            var result = ExcelImportAtdSql(sheet);
            stream.Close();
            stream.Dispose();
            workbook.Close();

            return result;
        }

        /// <summary>
        /// 将导过来的excel数据存入excel视图类
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public List<MyAtdDataFromExcelView> CreateExcelData(ISheet sheet)
        {
            List<MyAtdDataFromExcelView> result = new List<MyAtdDataFromExcelView>();
            int num = 3;
            AjaxResult ajaxresult = new AjaxResult();
            try
            {
                //获取第一行数据（年月份）
                //  string time = sheet.GetRow(1).Cells[1].StringCellValue;
                string time1 = sheet.GetRow(0).Cells[0].StringCellValue;
                string[] str = time1.Split('至');
               // string[] strtime = str[1];
                var time = str[1];
                //获取第二行数据（应到勤天数）
                //   double DeserveToRegularDays = sheet.GetRow(2).Cells[1].NumericCellValue;
                while (true)
                {
                    MyAtdDataFromExcelView matd = new MyAtdDataFromExcelView();
                    num++;
                    //获取第三行"姓名"列的数据
                    var getrow = sheet.GetRow(num);
                    if (getrow == null)
                    {
                        break;
                    }
                    //姓名[0]
                    string name = getrow.GetCell(0).StringCellValue;
                    //工号(钉钉号)[1]
                    string ddid = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(1))) ? null : getrow.GetCell(1).ToString();
                    //到勤天数[2]
                    string workeddays = getrow.GetCell(2).NumericCellValue.ToString();

                    //迟到次数[5]
                    string tardyNum = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(5))) ? null : getrow.GetCell(5).ToString();
                    //早退次数[10]
                    string leaveEarlyNum = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(10))) ? null : getrow.GetCell(10).ToString();
                    //上班缺卡次数[12]
                    string workAbsentNum = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(12))) ? null : getrow.GetCell(12).ToString();
                    //下班缺卡次数[13]
                    string offDutyAbsentNum = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(13))) ? null : getrow.GetCell(13).ToString();

                    //请假天数
                    string leaveddays = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(18))) ? null : getrow.GetCell(18).ToString();
                    //请假记录
                    string leaveRecord = "";

                    //迟到记录[5]
                    string tardyRecord = "" /*= getrow.GetCell(9) == null ? null : getrow.GetCell(9).StringCellValue*/;
                    //早退记录
                    string leaveEarlyRecord = "";
                    //上班缺卡记录
                    string workAbsentRecord = "";
                    //下班缺卡记录
                    string OffDutyAbsentRecord = "";
                    //加班记录
                    string OvertTimeRecord = "";
                    //调休记录
                    string DaysoffRecord = "";
                    //旷工记录
                    string AbsenteeismRecord = "";

                    //这些付款都是在员工工资表
                    MonthlySalaryRecordManage msrmanage = new MonthlySalaryRecordManage();
            
                    int cells = 24;
                    while (true)
                    {//(循环拿到日期列数据)
                        var getcell = getrow.GetCell(cells);
                        if (getcell == null)
                        {                            
                            break;
                        }
                        var titlerow= sheet.GetRow(2);//表头行（日期）
                        var title = cells-23;
                      
                        if (getcell.StringCellValue.Contains("迟到"))
                        {
                            tardyRecord += title + "号," + getcell.StringCellValue + ";";
                        }
                        else if (getcell.StringCellValue.Contains("早退")) {
                            leaveEarlyRecord += title + "号," + getcell.StringCellValue + ";";
                        }
                        else if (getcell.StringCellValue.Contains("上班缺卡"))
                        {
                            workAbsentRecord += title + "号," + getcell.StringCellValue + ";";
                        }
                        else if (getcell.StringCellValue.Contains("下班缺卡"))
                        {
                            OffDutyAbsentRecord += title + "号," + getcell.StringCellValue + ";";
                        }
                        else if (getcell.StringCellValue.Contains("事假")) {
                            leaveRecord += getcell.StringCellValue + ";";
                        } else if (getcell.StringCellValue.Contains("加班")) {
                            OvertTimeRecord += getcell.StringCellValue + ";";
                        } else if (getcell.StringCellValue.Contains("调休")) {
                            DaysoffRecord += getcell.StringCellValue + ";";
                        } else if (getcell.StringCellValue.Contains("旷工")) {
                            AbsenteeismRecord += title + "号,"+ getcell.StringCellValue +";";
                        }       
                        
                        //迟到扣款
                        // string tardyWithhold = "";
                        //早退扣款
                        //
                        cells++;

                    } 
                    // string leaveddays = getrow.GetCell(3) == null ? null : getrow.GetCell(3).NumericCellValue.ToString();
                    // string tardyWithhold = getrow.GetCell(10) == null ? null : getrow.GetCell(10).NumericCellValue.ToString();
                    // string leaveWithhold = getrow.GetCell(13) == null ? null : getrow.GetCell(13).NumericCellValue.ToString();
                    // string remark = getrow.GetCell(14) == null ? null : getrow.GetCell(14).StringCellValue;
       
                    matd.YearAndMonth = Convert.ToDateTime(time);
                 //  matd.DeserveToRegularDays =Convert.ToDecimal(DeserveToRegularDays);

                    matd.EmpName = name;
                    matd.EmpDDid = Convert.ToInt32(ddid);
                    matd.ToRegularDays = Convert.ToInt32(workeddays);
                    matd.LeaveDays = Convert.ToDecimal(leaveddays);
                    matd.LeaveRecord = leaveRecord; 
                    matd.WorkAbsentNum =Convert.ToInt32(workAbsentNum);
                    matd.WorkAbsentRecord = workAbsentRecord;
                    matd.OffDutyAbsentNum = Convert.ToInt32(offDutyAbsentNum);
                    matd.OffDutyAbsentRecord = OffDutyAbsentRecord;
                    matd.TardyNum = Convert.ToInt32(tardyNum);
                    matd.TardyRecord = tardyRecord;
                    matd.LeaveEarlyNum =Convert.ToInt32(leaveEarlyNum);
                    matd.LeaveEarlyRecord = leaveEarlyRecord;
                    matd.OvertTimeRecord = OvertTimeRecord;
                    matd.DaysoffRecord = DaysoffRecord;
                    matd.AbsenteeismRecord = AbsenteeismRecord;

                    //matd.TardyWithhold =tardyWithhold==null?matd.TardyWithhold=null: Convert.ToInt32(tardyWithhold);

                    //matd.LeaveWithhold =leaveWithhold==null?matd.LeaveWithhold=null: Convert.ToInt32(leaveWithhold);
                    //matd.Remark = remark;

                    result.Add(matd);
                }

            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;

        }
           

        /// <summary>
        /// 将excel数据类的数据存入到数据库的考勤表中
        /// </summary>
        /// <returns></returns>
        public AjaxResult ExcelImportAtdSql(ISheet sheet)
        {
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var ajaxresult = new AjaxResult();
            List<AttendanceInfoErrorDataView> attdatalist = new List<AttendanceInfoErrorDataView>();
            try
            {
                var mateviewlist = CreateExcelData(sheet);

                //获取第一个人的出勤天数，将它设为默认的应出勤天数
                var days = mateviewlist.FirstOrDefault().ToRegularDays;
                foreach (var item in mateviewlist)
                { 
                    AttendanceInfo atd = new AttendanceInfo();
                    AttendanceInfoErrorDataView attview = new AttendanceInfoErrorDataView();
                    if (!empmanage.DDidIsExist(item.EmpDDid))
                    {//判断员工钉钉号是否为空
                        attview.empname = item.EmpName;
                        attview.errorExplain = "姓名是" + attview.empname + "的员工工号为空！";
                        attdatalist.Add(attview);
                    }
                    else {

                    var emp = empmanage.GetEmpByDDid(item.EmpDDid);
                  
                    atd.EmployeeId = emp.EmployeeId;
                    atd.YearAndMonth = item.YearAndMonth;
                    atd.DeserveToRegularDays = days;
                    atd.ToRegularDays = item.ToRegularDays;

                    atd.LeaveRecord = item.LeaveRecord;
                    atd.LeaveDays = item.LeaveDays;
                    atd.WorkAbsentNum = item.WorkAbsentNum;
                    atd.WorkAbsentRecord = item.WorkAbsentRecord;
                    atd.OffDutyAbsentNum = item.OffDutyAbsentNum;
                    atd.OffDutyAbsentRecord = item.OffDutyAbsentRecord;
                    atd.TardyNum = item.TardyNum;
                    atd.TardyRecord = item.TardyRecord;
                    atd.LeaveEarlyNum = item.LeaveEarlyNum;
                    atd.LeaveEarlyRecord = item.LeaveEarlyRecord;
                    atd.TardyWithhold = item.TardyWithhold;
                    atd.LeaveWithhold = item.LeaveWithhold;
                    //统计总缺卡次(计算缺卡扣款,>3次则扣半天工资,见月度工资表）
                    var AbsentNum = atd.WorkAbsentNum + atd.OffDutyAbsentNum;
                    atd.Remark = item.Remark;
                    atd.IsDel = false;
                    atd.IsApproval = false;
                    this.Insert(atd);
                    rc.RemoveCache("InRedisATDData");
                   ajaxresult.Success=true;
                   ajaxresult.ErrorCode = 200;
                   ajaxresult.Msg = atd.YearAndMonth.ToString()+","+atd.DeserveToRegularDays;
                   ajaxresult.Data = mateviewlist.Count();

                    }
                }
                if (mateviewlist.Count() - attdatalist.Count() == mateviewlist.Count())
                {//说明没有出错数据，导入的数据全部添加成功
                    ajaxresult.Success = true;
                    ajaxresult.ErrorCode = 100;
                    ajaxresult.Msg = mateviewlist.Count().ToString();
                    ajaxresult.Data = attdatalist;
                }
                else
                {//说明有出错数据，导入的数据条数就是导入的数据总数-错误数据总数
                    ajaxresult.Success = true;
                    ajaxresult.ErrorCode = 200;
                    ajaxresult.Msg = (mateviewlist.Count() - attdatalist.Count()).ToString();
                    ajaxresult.Data = attdatalist;
                }
            }
            catch (Exception ex)
            {
                ajaxresult.Success = false;
                ajaxresult.ErrorCode = 500;
                ajaxresult.Msg = "失败";
                ajaxresult.Data = "0";
            }
            return ajaxresult;   
        }

        //public decimal GetAbsent

        /// <summary>
        /// 计算请假扣款
        /// </summary>
        /// <param name="tardyRecord"></param>
        /// <returns></returns>
        public decimal GetTardyCount(string tardyRecord)
        {
            var str = tardyRecord.Split(';');
            var result = 0;
            int num = 0;
            foreach (var item in str)
            {
                var tardy = item[num];

            }
            return result;
        }
        //迟到扣款
        public decimal TardyWithhold(string tardyRecord)
        {
            var result = 0;
            return result;
        }
    }
}
