using System;
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
                    string begintime = getrow.GetCell(2).ToString();
                    //结束时间[3]
                    string endtime = getrow.GetCell(3).ToString();
                    //时长（h）[4]
                    string duration = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(4))) ? null : getrow.GetCell(4).ToString();
                    //加班原因[5]
                    string overtimereason = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(4))) ? null : getrow.GetCell(4).ToString();
                    //是否调休[6]
                    string Isdayoff = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(4))) ? null : getrow.GetCell(4).ToString();
                    //加班类型[7]
                    string overtimetype = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(4))) ? null : getrow.GetCell(4).ToString();
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
                                otr.YearAndMonth = Convert.ToDateTime(year_month);
                                otr.StartTime = Convert.ToDateTime(begintime);
                                otr.EndTime = Convert.ToDateTime(endtime);
                                otr.Duration = Convert.ToDecimal(duration);
                                otr.OvertimeReason = overtimereason;
                                otr.IsNoDaysOff =Convert.ToBoolean(Isdayoff);
                                otr.OvertimeTypeId =Convert.ToInt32(overtimetype);
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
    }
}
