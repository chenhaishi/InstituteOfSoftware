using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness
{
    public class MeritsCheckManage : BaseBusiness<MeritsCheck>
    {
        RedisCache rc=new RedisCache();
        /// <summary>
        /// 将员工绩效考核表数据存储到redis服务器中
        /// </summary>
        /// <returns></returns>
        public List<MeritsCheck> GetEmpMCData() {
            rc.RemoveCache("InRedisMCData");
            List<MeritsCheck> mclist = new List<MeritsCheck>();
            if (mclist==null || mclist.Count()==0) {
                mclist = this.GetList();
                rc.SetCache("InRedisMCData",mclist);
            }
            mclist= rc.GetCache<List<MeritsCheck>>("InRedisMCData");
            return mclist;
        }
        /// <summary>
        /// 往员工绩效考核表加入员工编号
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        //public bool AddEmpToMeritsCheck(string empid)
        //{
        //    bool result = false;
        //    try
        //    {
        //        MeritsCheck ese = new MeritsCheck();
        //        ese.EmployeeId = empid;
        //        ese.IsDel = false;
        //        if (this.GetEmpMCData().Count() == 0)
        //        {
        //            ese.YearAndMonth = DateTime.Now;
        //        }
        //        else
        //        {
        //            ese.YearAndMonth = this.GetEmpMCData().LastOrDefault().YearAndMonth;
        //        }

        //        this.Insert(ese);
        //        rc.RemoveCache("InRedisMCData");
        //        result = true;
        //        BusHelper.WriteSysLog("绩效考核表添加员工成功", Entity.Base_SysManage.EnumType.LogType.添加数据);

        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
        //    }
        //    return result;

        //}

        /// <summary>
        /// 绩效考核表禁用员工方法
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool EditEmpStateToMC(string empid,string time)
        {
            bool result = false;
            try
            {
                var ymtime = DateTime.Parse(time);
                var mc = this.GetEmpMCData().Where(e => e.EmployeeId == empid&&DateTime.Parse(e.YearAndMonth.ToString()).Year==ymtime.Year&&DateTime.Parse(e.YearAndMonth.ToString()).Month==ymtime.Month).FirstOrDefault();
                mc.IsDel = true;
                this.Update(mc);
                rc.RemoveCache("InRedisMCData");
                result = true;
                BusHelper.WriteSysLog("绩效考核表禁用员工成功", Entity.Base_SysManage.EnumType.LogType.编辑数据);

            }
            catch (Exception ex)
            {
                result = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;

        }
        /// <summary>
        /// 将某员工的绩效分默认改为100
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool GetmcempByEmpid(string empid)
        {
            bool result = true;
            try
            {
                var mcemp = this.GetEmpMCData().Where(s => s.EmployeeId == empid).FirstOrDefault();
                if (mcemp!=null) {
                    mcemp.FinalGrade = 100;
                    this.Update(mcemp);
                    rc.RemoveCache("InRedisMCData");
                    result = true;
                    BusHelper.WriteSysLog("将该员工绩效分默认修改为100成功！", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                }

            }
            catch (Exception ex)
            {
                result = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;
        }

        //public MeritsCheck GetBaseEmp(string empid) {
        //    MeritsCheck mc = new MeritsCheck();
        //    return mc;
        //}

        public List<MeritsCheckView> ExcelView(ISheet sheet)
        {
            List<MeritsCheckView> meritsCheck = new List<MeritsCheckView>();
            int num = 2;
            AjaxResult ajaxResult = new AjaxResult();
            try
            {
                while (true)
                {
                    MeritsCheckView merits = new MeritsCheckView();
                    num++;
                    var getrow = sheet.GetRow(num);

                    if (string.IsNullOrEmpty(Convert.ToString(getrow)))
                    {
                        break;
                    }
                    string Year = string.IsNullOrEmpty(Convert.ToString(sheet.GetRow(1).GetCell(0))) ? null : sheet.GetRow(1).GetCell(0).ToString();
                    string name = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(0))) ? null : getrow.GetCell(0).ToString();
                    string employeeId = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(1))) ? null : getrow.GetCell(1).ToString();
                    string finalgrade = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(2))) ? null : getrow.GetCell(2).ToString();


                    merits.EmployeeId = employeeId;
                    if (!string.IsNullOrEmpty(Year))
                    {
                        string year = Year.Substring(0, 4);
                        string month = Year.Substring(5, 2);
                        string yearandmonth = year + "-" + month + "-" + 01;
                        merits.YearAndMonth = Convert.ToDateTime(yearandmonth);
                    }
                    if (!string.IsNullOrEmpty(finalgrade))
                    {
                        merits.FinalGrade = decimal.Parse(finalgrade);
                    }
                    meritsCheck.Add(merits);
                }
            }
            catch (Exception e)
            {
                return null;
            }
            return meritsCheck;
        }

        public AjaxResult ExcelDeposit(ISheet sheet)
        {
            var result = new AjaxResult();
            List<MeritsCheckErrorDataView> error = new List<MeritsCheckErrorDataView>();
            try
            {
                var merits = ExcelView(sheet);
                foreach (var item in merits)
                {
                    MeritsCheckErrorDataView errorDataView = new MeritsCheckErrorDataView();
                    MeritsCheck merits1 = new MeritsCheck();
                    if (string.IsNullOrEmpty(item.EmployeeId))
                    {
                        errorDataView.excelId = item.EmployeeId;
                        errorDataView.errorExplain = "原因是该员工工号为空！";
                        error.Add(errorDataView);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.FinalGrade.ToString()))
                        {
                            errorDataView.excelId = item.EmployeeId;
                            errorDataView.errorExplain = "原因是该员工绩效分为空！";
                            error.Add(errorDataView);
                        }
                        else
                        {
                            merits1.EmployeeId = item.EmployeeId;
                            merits1.FinalGrade = item.FinalGrade;
                            merits1.YearAndMonth = item.YearAndMonth;
                            merits1.IsDel = false;
                            this.Insert(merits1);
                            BusHelper.WriteSysLog("Excel文件导入成功", Entity.Base_SysManage.EnumType.LogType.Excle文件导入);
                            rc.RemoveCache("InRedisEmpInfoData");
                        }
                    }
                }

                if (merits.Count() - error.Count() == merits.Count())
                {//说明没有出错数据，导入的数据全部添加成功
                    result.Success = true;
                    result.ErrorCode = 100;
                    result.Msg = merits.Count().ToString();
                    result.Data = error;
                }
                else
                {//说明有出错数据，导入的数据条数就是导入的数据总数-错误数据总数
                    result.Success = true;
                    result.ErrorCode = 200;
                    result.Msg = (merits.Count() - error.Count()).ToString();
                    result.Data = error;
                }
            }
            catch (Exception e)
            {
                result.Success = false;
                result.ErrorCode = 500;
                result.Msg = e.Message;
                result.Data = 0;
            }
            return result;
        }
        public AjaxResult ImportDataFormExcel(Stream stream, string contentType)
        {
            var ajaxresult = new AjaxResult();
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
            var result = ExcelDeposit(sheet);
            stream.Close();
            stream.Dispose();
            workbook.Close();

            return result;
        }
    }
}
