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
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
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

        //public List<MeritsCheckView> ExcelView(ISheet sheet)
        //{
        //    List<MeritsCheckView> meritsCheck = new List<MeritsCheckView>();
        //    EmployeesInfoManage emanage = new EmployeesInfoManage();
        //    int num = 2;
        //    AjaxResult ajaxResult = new AjaxResult();
        //    try
        //    {
        //        while (true)
        //        {
        //            MeritsCheckView merits = new MeritsCheckView();
        //            num++;
        //            var getrow = sheet.GetRow(num);

        //            if (string.IsNullOrEmpty(Convert.ToString(getrow)))
        //            {
        //                break;
        //            }
        //            if (!string.IsNullOrEmpty(getrow.GetCell(1).ToString())|| !string.IsNullOrEmpty(getrow.GetCell(0).ToString())|| !string.IsNullOrEmpty(getrow.GetCell(2).ToString()))
        //            {
        //            string Year = string.IsNullOrEmpty(Convert.ToString(sheet.GetRow(1).GetCell(0))) ? null : sheet.GetRow(1).GetCell(0).ToString();
        //            string name = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(0))) ? null : getrow.GetCell(0).ToString();
        //            string ddid = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(1))) ? null : getrow.GetCell(1).ToString();
        //            string finalgrade = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(2))) ? null : getrow.GetCell(2).ToString();

        //            var empid=  emanage.GetList().Where(i=>i.DDAppId==int.Parse(ddid)).FirstOrDefault().EmployeeId;
        //            merits.EmployeeId = empid;
                    
        //            if (!string.IsNullOrEmpty(Year))
        //            {
        //                string year = Year.Substring(0, 4);
        //                string month = Year.Substring(5, 2);
        //                string yearandmonth = year + "-" + month + "-" + 01;
        //                merits.YearAndMonth = Convert.ToDateTime(yearandmonth);
        //            }
        //            if (!string.IsNullOrEmpty(finalgrade))
        //            {
        //                merits.FinalGrade = decimal.Parse(finalgrade);
        //            }
                    
        //                 meritsCheck.Add(merits);                 
        //            }

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        var s = e.Message;
        //        return null;
        //    }
        //    return meritsCheck;
        //}

        public AjaxResult ExcelDeposit(ISheet sheet)
        {
            var result = new AjaxResult();
            List<MeritsCheckErrorDataView> error = new List<MeritsCheckErrorDataView>();
            try
            {
                EmployeesInfoManage emanage = new EmployeesInfoManage();
                int num = 2;
                AjaxResult ajaxResult = new AjaxResult();
                //string sql = "insert into MeritsCheck values";
                while (true)
                {
                    //MeritsCheckView merits = new MeritsCheckView();
                    num++;
                    var getrow = sheet.GetRow(num);
                    if (string.IsNullOrEmpty(Convert.ToString(getrow)))
                    {
                        break;
                    }
                    if (!string.IsNullOrEmpty(getrow.GetCell(1).ToString()) || !string.IsNullOrEmpty(getrow.GetCell(0).ToString()) || !string.IsNullOrEmpty(getrow.GetCell(2).ToString()))
                    {
                        string Year = string.IsNullOrEmpty(Convert.ToString(sheet.GetRow(1).GetCell(0))) ? null : sheet.GetRow(1).GetCell(0).ToString();
                        string name = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(0))) ? null : getrow.GetCell(0).ToString();
                        string ddid = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(1))) ? null : getrow.GetCell(1).ToString();
                        string finalgrade = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(2))) ? null : getrow.GetCell(2).ToString();
                        
                       


                        MeritsCheckErrorDataView errorDataView = new MeritsCheckErrorDataView();
                        MeritsCheck merits1 = new MeritsCheck();
                        if (string.IsNullOrEmpty(Year))
                        {
                            MeritsCheckErrorDataView err = new MeritsCheckErrorDataView();
                            err.excelId = name;
                            err.errorExplain = "原因是第二行时间未填";
                            error.Add(err);
                        }
                        else
                        {
                            string year = Year.Substring(0, 4);
                            string month = Year.Substring(5, 2);
                            Year = year + "-" + month + "-" + 01;

                            if (string.IsNullOrEmpty(ddid))
                        {
                            errorDataView.excelId = name;
                            errorDataView.errorExplain = "原因是该员工工号为空";
                            error.Add(errorDataView);
                        }
                            else
                        {
                                if (!emanage.DDidIsExist(int.Parse(ddid)))
                        {
                            errorDataView.excelId = name;
                            errorDataView.errorExplain = "原因是该员工钉钉号不存在！";
                            error.Add(errorDataView);
                        }
                                else
                        {
                                var empid = emanage.GetList().Where(i => i.DDAppId == int.Parse(ddid)).FirstOrDefault().EmployeeId;
                                //merits.EmployeeId = empid;
                                if (string.IsNullOrEmpty(finalgrade))
                            {
                                errorDataView.excelId = name;
                                errorDataView.errorExplain = "原因是该员工绩效分为空！";
                                error.Add(errorDataView);
                            }
                                    else
                            {
                                //merits1.EmployeeId = empid;
                                //merits1.FinalGrade = int.Parse(finalgrade);
                                //merits1.YearAndMonth = Convert.ToDateTime(Year);
                                //merits1.IsDel = false;
                                //merits1.IsApproval = false;
                                //this.Insert(merits1);
                                ExecuteSql("insert into MeritsCheck (EmployeeId,YearAndMonth,FinalGrade,IsDel,IsApproval)values(" + empid + ",'" + Year + "'," + finalgrade + ",0,0)");
                                BusHelper.WriteSysLog("Excel文件导入成功", Entity.Base_SysManage.EnumType.LogType.Excle文件导入);
                                rc.RemoveCache("InRedisMCData");
                            }
                        }
                        }
                        }


                    }
                }
                //sql = sql.Substring(0, sql.Length - 1);
                //ExecuteSql(sql);


                int sum = num - 3;
                if (sum - error.Count() == sum)
                {//说明没有出错数据，导入的数据全部添加成功
                    result.Success = true;
                    result.ErrorCode = 100;
                    result.Msg = sum.ToString();
                    result.Data = error;
                }
                else
                {//说明有出错数据，导入的数据条数就是导入的数据总数-错误数据总数
                    result.Success = true;
                    result.ErrorCode = 200;
                    result.Msg = (sum - error.Count()).ToString();
                    result.Data = error;
                }

            }
            catch (Exception e)
            {
                result.Success = false;
                result.ErrorCode = 500;
                result.Msg = e.Message;
                result.Data = 0;
                BusHelper.WriteSysLog(e.Message, Entity.Base_SysManage.EnumType.LogType.Excle文件导入);
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
