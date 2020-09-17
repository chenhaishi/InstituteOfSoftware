using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Util;
namespace SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness
{
   

  public  class EmplSalaryEmbodyManage:BaseBusiness<EmplSalaryEmbody>
    {
        RedisCache rc= new RedisCache();
        /// <summary>
        /// 将员工工资体系表的数据存储于redis服务器
        /// </summary>
        /// <returns></returns>
        public List<EmplSalaryEmbody> GetEmpESEData() {
            rc.RemoveCache("InRedisESEData");
            List<EmplSalaryEmbody> eselist = new List<EmplSalaryEmbody>();
            if (eselist==null || eselist.Count==0) {
                eselist = this.GetList();
                rc.SetCache("InRedisESEData",eselist);
            }
            eselist = rc.GetCache<List<EmplSalaryEmbody>>("InRedisESEData");
            return eselist;
         
        }

        /// <summary>
        /// 往员工工资体系表加入员工编号
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool AddEmpToEmpSalary(string empid) {
            bool result = false;
            try
            {
                EmplSalaryEmbody ese = new EmplSalaryEmbody();               
                ese.EmployeeId = empid;

                EmployeesInfoManage empmanage = new EmployeesInfoManage();
                var emp=empmanage.GetEntity(empid);
                if (!string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                {
                    if (emp.Salary < 2000)
                    {
                        ese.BaseSalary = emp.Salary;
                        ese.PositionSalary = 0;
                    }
                    else {
                        ese.BaseSalary = 2000;

                        var positionname = empmanage.GetPositionByEmpid(empid).PositionName;//获取该员工岗位
                        if (positionname.Contains("主任") && !positionname.Contains("班主任"))
                        {
                            ese.PerformancePay = 1000;
                        }
                        else if (empmanage.GetDeptByEmpid(empid).DeptName == "校办")
                        {
                            ese.PerformancePay = 3000;
                        }
                        else
                        {
                            ese.PerformancePay = 500;
                        }
                        ese.PositionSalary = emp.Salary - ese.BaseSalary - ese.PerformancePay;
                    }
                  
                }
                else {
                    if ( emp.ProbationSalary<2000) {
                        ese.BaseSalary = emp.ProbationSalary;
                        ese.PositionSalary = 0;
                    }
                    else
                    {
                        ese.BaseSalary = 2000;
                        ese.PositionSalary = emp.ProbationSalary - ese.BaseSalary;
                    }
                }
             
                ese.IsDel = false;
                this.Insert(ese);
                rc.RemoveCache("InRedisESEData");
                result = true;
                BusHelper.WriteSysLog("工资体系表添加员工成功", Entity.Base_SysManage.EnumType.LogType.添加数据);
                
            }
            catch (Exception ex)
            {
                result = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return result;          
        }


        /// <summary>
        /// 禁用某员工
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool EditEmpSalaryState(string empid) {
            var ese = this.GetEmpESEData().Where(e => e.EmployeeId == empid ).FirstOrDefault() ;
            bool result = false;
            try
            {
                ese.IsDel = true;
                this.Update(ese);
                rc.RemoveCache("InRedisESEData");
                result = true;
                BusHelper.WriteSysLog("工资体系表禁用该员工成功！", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {
                result = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return result;
        }


        /// <summary>
        /// 根据员工编号获取该员工工资体系对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public EmplSalaryEmbody GetEseByEmpid(string empid) {
           var ese= this.GetEmpESEData().Where(s => s.EmployeeId == empid).FirstOrDefault();
            return ese;
        }


        public AjaxResult EditEseByEmp(EmployeesInfo emp) {
            var ajaxresult = new AjaxResult();

            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            try
            {
                var ese = this.GetEseByEmpid(emp.EmployeeId);
                var positionname = empmanage.GetPositionByEmpid(emp.EmployeeId).PositionName;
                if (!string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                {
                    if (emp.Salary < 2000)
                    {
                        ese.BaseSalary = emp.Salary;
                        ese.PositionSalary = 0;
                    }
                    else
                    {
                        ese.BaseSalary = 2000;

                        if (positionname.Contains("主任") && !positionname.Contains("班主任"))
                        {
                            ese.PerformancePay = 1000;
                        }
                        else if (empmanage.GetDeptByEmpid(emp.EmployeeId).DeptName == "校办")
                        {
                            ese.PerformancePay = 3000;
                        }
                        else
                        {
                            ese.PerformancePay = 500;
                        }
                        ese.PositionSalary = emp.Salary - ese.BaseSalary - ese.PerformancePay;
                    }

                }
                else
                {
                    if (emp.ProbationSalary < 2000)
                    {
                        ese.BaseSalary = emp.ProbationSalary;
                        ese.PositionSalary = 0;
                    }
                    else
                    {
                        ese.BaseSalary = 2000;
                        ese.PositionSalary = emp.ProbationSalary - ese.BaseSalary;
                    }
                }
                this.Update(ese);
                rc.RemoveCache("InRedisESEData");
                ajaxresult = this.Success();
            }
            catch (Exception ex)
            {
                ajaxresult = this.Error(ex.Message);
            }
            return ajaxresult;
        }
    }
}
