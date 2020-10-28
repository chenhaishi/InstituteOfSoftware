﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;

namespace SiliconValley.InformationSystem.Business.SchoolAttendanceManagementBusiness
{
   public class OvertimeRecordManage:BaseBusiness<OvertimeRecord>
    {
        /// <summary>
        /// 拿到考勤excel表中的第一个单元
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
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
        /// 将excel数据类的数据存入到数据库的考勤表中
        /// </summary>
        /// <returns></returns>
        public AjaxResult ExcelImportAtdSql(ISheet sheet)
        {
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var ajaxresult = new AjaxResult();
            int num = 2;
            List<OvertimeRecordErrorDataView> otratalist = new List<OvertimeRecordErrorDataView>();
            try
            {
                //获取第二行数据（年月份）
                string time1 = sheet.GetRow(1).Cells[0].StringCellValue;
                string[] str = time1.Split('-');
                var time = str[1];

                while (true)
                {
                    num++;
                    var getrow = sheet.GetRow(num);
                    if (getrow == null)
                    {
                        break;
                    }
                    #region 循环拿值
                    //工号(钉钉号)[0]
                    string ddid = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(0))) ? null : getrow.GetCell(0).ToString();
                    //加班人[1]
                    string name = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(1))) ? null : getrow.GetCell(1).ToString();
                    //开始时间[2]
                    string begintime = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(2))) ? null: getrow.GetCell(2).ToString();
                    //结束时间[3]
                    string endtime = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(3))) ? null : getrow.GetCell(3).ToString();
                    //时长（h）[4]
                    string duration = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(4))) ? null : getrow.GetCell(4).ToString();
                    //加班原因[5]
                    string overtimereason = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(5))) ? null : getrow.GetCell(5).ToString();
                    //是否调休[6]
                    string Isdayoff = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(6))) ? "是" : getrow.GetCell(6).ToString();
                    //加班类型[7]
                    string overtimetype = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(7))) ? "1" : getrow.GetCell(7).ToString();
                    #endregion

                    string year_month = time;

                    OvertimeRecord otr = new OvertimeRecord();
                    OvertimeRecordErrorDataView otrview = new OvertimeRecordErrorDataView();
                    if (string.IsNullOrEmpty(year_month))
                    {
                        //otrview.empname = name;
                        otrview.errorExplain = "第二行的时间为空！";
                        otratalist.Add(otrview);
                    }
                    else
                    {
                        if (!empmanage.DDidIsExist(Convert.ToInt32(ddid)))
                        {//判断员工钉钉号是否为空
                            otrview.empname = name;
                            otrview.errorExplain = "工号为空！";
                            otratalist.Add(otrview);
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(duration))
                            {
                                otrview.empname = name;
                                otrview.errorExplain = "加班时长为空！";
                                otratalist.Add(otrview);
                            }
                            else
                            {

                                var emp = empmanage.GetEmpByDDid(Convert.ToInt32(ddid));

                                otr.EmployeeId = emp.EmployeeId;
                                if (!string.IsNullOrEmpty(year_month))
                                {
                                    otr.YearAndMonth = Convert.ToDateTime(year_month);
                                }
                                if(!string.IsNullOrEmpty(begintime)){
                                    otr.StartTime= Convert.ToDateTime(begintime);
                                }
                                if (!string.IsNullOrEmpty(endtime))
                                {
                                    otr.EndTime = Convert.ToDateTime(endtime);
                                }
                                otr.Duration = Convert.ToDecimal(duration);
                                otr.OvertimeReason = overtimereason;
                                if (Isdayoff == "是")
                                {
                                    otr.IsNoDaysOff = true;
                                }
                                else
                                {
                                    otr.IsNoDaysOff = false;
                                }
                                if (overtimetype!="1" && overtimetype!= "2" && overtimetype != "3" && overtimetype != "4")
                                {
                                    otr.OvertimeTypeId = 1;
                                }
                                else {
                                    otr.OvertimeTypeId = Convert.ToInt32(overtimetype);
                                }
                                otr.IsPass = false;
                                this.Insert(otr);

                            }
                        }

                    }
                }
                int exceldatasum = num - 3;
                if (exceldatasum - otratalist.Count() == exceldatasum)
                {//说明没有出错数据，导入的数据全部添加成功
                    ajaxresult.Success = true;
                    ajaxresult.ErrorCode = 100;
                    ajaxresult.Msg = exceldatasum.ToString();
                    ajaxresult.Data = otratalist;
                }
                else
                {//说明有出错数据，导入的数据条数就是导入的数据总数-错误数据总数
                    ajaxresult.Success = true;
                    ajaxresult.ErrorCode = 200;
                    ajaxresult.Msg = (exceldatasum - otratalist.Count()).ToString();
                    ajaxresult.Data = otratalist;
                }
            }
            catch (Exception ex)
            {
                ajaxresult.Success = false;
                ajaxresult.ErrorCode = 500;
                ajaxresult.Msg = ex.Message;
                ajaxresult.Data = "0";
            }
            
            return ajaxresult;
        }

        /// <summary>
        /// 加班费用计算规则
        /// </summary>
        /// <param name="type"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public decimal OvertimeWithhold(int type,decimal myduration) {
           var result = 0;
            var duration = Convert.ToDouble(myduration);
            if (type == 1)
            {//晚上加班
                if (duration >= 1.5 && duration <= 30)
                {
                    result = 30;
                }
                else if (duration > 30)
                {
                    result = 50;
                }
            }
            else if (type == 2)
            {//周末加班
                if (duration == 3.5)
                {//半天
                    result = 50;
                }
                else if (duration == 7)
                {//一天
                    result = 100;
                }
            }
            else if (type == 3)
            {
                if (duration == 3.5)//节假日半天
                {
                    result = 100;
                }
                else if (duration == 7)//节假日一天
                {
                    result = 200;
                }
            }
            else if(type==4){
                result = 100;
            }
            return result;
        }


        public List<OvertimeRecord> GetOTRData(string empid,DateTime year_month) {
            var year = year_month.Year;
            var month = year_month.Month;
            var otrsqllist = this.GetListBySql<OvertimeRecord>("select * from OvertimeRecord where EmployeeId="+empid+" and YEAR(YearAndMonth)="+year+ " and MONTH(YearAndMonth)="+month);
            return otrsqllist;
        }

        public List<OvertimeRecord> GetOTRDataByAtdid(int id) {
            AttendanceInfoManage atdmanage = new AttendanceInfoManage();
            var atd = atdmanage.GetEntity(id);
            string empid = atd.EmployeeId;
            var time = atd.YearAndMonth;
            var otdlist = this.GetOTRData(empid,(DateTime)time);
            return otdlist;
        }
    }
}
