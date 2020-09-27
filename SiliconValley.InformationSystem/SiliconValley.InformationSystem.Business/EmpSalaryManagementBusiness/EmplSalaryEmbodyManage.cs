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
               
                var deptname = empmanage.GetDeptByEmpid(empid).DeptName;
                var positionname = empmanage.GetPositionByEmpid(emp.EmployeeId).PositionName;
                //员工转正或无试用期的员工，工资计算按正式工资来算
                if (!string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                {
                    ese.PerformancePay = GetPerformancePay(deptname, positionname);
                    var surplusalary = emp.Salary - ese.PerformancePay;
                    if (surplusalary <= 2000)
                    {
                        ese.BaseSalary = surplusalary;
                    }
                    else
                    {
                        ese.BaseSalary = 2000;
                        ese.PositionSalary = emp.Salary - ese.BaseSalary - ese.PerformancePay;
                    }
                }
                //有试用期的员工，工资计算按试用期工资来算
                else
                {
                    if (emp.ProbationSalary <= 2000)
                    {
                        ese.BaseSalary = emp.ProbationSalary;
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

        /// <summary>
        /// 修改某员工的工资体系
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public AjaxResult EditEseByEmp(EmployeesInfo emp) {
            var ajaxresult = new AjaxResult();

            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            try
            {
                var ese = this.GetEseByEmpid(emp.EmployeeId);
                var deptname = empmanage.GetDeptByEmpid(emp.EmployeeId).DeptName;
                var positionname = empmanage.GetPositionByEmpid(emp.EmployeeId).PositionName;
                //员工转正或无试用期的员工，工资计算按正式工资来算
                if (!string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                {
                    ese.PerformancePay = GetPerformancePay(deptname, positionname);
                    var surplusalary = emp.Salary - ese.PerformancePay;
                    if (surplusalary <= 2000)
                    {
                        ese.BaseSalary = surplusalary;
                    }
                    else {
                        ese.BaseSalary = 2000;
                        ese.PositionSalary = emp.Salary - ese.BaseSalary - ese.PerformancePay;
                    }               
                }
                //有试用期的员工，工资计算按试用期工资来算
                else
                {
                    if (emp.ProbationSalary <= 2000)
                    {
                        ese.BaseSalary = emp.ProbationSalary;
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

        /// <summary>
        /// 通过部门名称和岗位名称获取相应条件的绩效额度
        /// </summary>
        /// <param name="dname"></param>
        /// <param name="pname"></param>
        /// <returns></returns>
        public decimal GetPerformancePay(string dname,string pname) {
            var resultsalary = 0;
            if (dname == "校办")
            {
                if (pname == "常务副校长" || pname == "工会主席兼办公室主任")
                {//指杨校和黄主任
                    resultsalary = 3000;//他俩的月度绩效额度为3000
                }
                else
                {
                    resultsalary = 0;
                }
            }
            else
            {
                if (pname == "市场主任" || pname == "咨询主任")
                {
                    resultsalary = 2000;
                }
                else if ((pname.Contains("主任") && !pname.Contains("班主任")) || pname == "人事总监")
                {//主任及副主任和人事总监的月绩效额度是1000
                    resultsalary = 1000;
                }
                else if (pname == "市场专员" || (dname == "后勤部" && (pname != "后勤主任" || pname != "后勤专员" || pname != "水电工")))
                {
                    resultsalary = 0;
                }
                else {
                    resultsalary = 500;
                }
            }
            return resultsalary;
        }
    }
}
