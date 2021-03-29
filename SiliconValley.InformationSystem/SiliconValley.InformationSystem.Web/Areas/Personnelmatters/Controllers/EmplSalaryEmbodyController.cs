using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using System.IO;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    public class EmplSalaryEmbodyController : Controller//员工工资体系
    {
        RedisCache rc=new RedisCache();
        
        // GET: Personnelmatters/EmplSalaryEmbody
        public ActionResult EmpSalaryEmbodyIndex()
        {
            return View();
        }

        //获取员工工资体系表所有数据
        public ActionResult GetSalarySystemData(int page, int limit,string AppCondition)
        {
            EmplSalaryEmbodyManage empsemanage = new EmplSalaryEmbodyManage();//员工工资体系表
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var eselist = empsemanage.GetEmpESEData().Where(s => s.IsDel == false).ToList() ;
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string deptname = str[1];
                string pname = str[2];
                string ContributionBase = str[3];
                string PersonalSocialSecurity = str[4];
                eselist = eselist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(deptname))
                {
                    eselist = eselist.Where(e => empmanage.GetDeptByEmpid(e.EmployeeId).DeptId == int.Parse(deptname)).ToList();
                }
                if (!string.IsNullOrEmpty(pname))
                {
                    eselist = eselist.Where(e => empmanage.GetInfoByEmpID(e.EmployeeId).PositionId == int.Parse(pname)).ToList();
                }
                if (!string.IsNullOrEmpty(ContributionBase))
                {
                    eselist = eselist.Where(e => e.ContributionBase ==int.Parse(ContributionBase)).ToList();
                }
                if (!string.IsNullOrEmpty(PersonalSocialSecurity))
                {
                    eselist = eselist.Where(e => e.PersonalSocialSecurity ==decimal.Parse(PersonalSocialSecurity)).ToList();
                }
 
            }
            var newlist = eselist.OrderBy(e => e.Id).Skip((page - 1) * limit).Take(limit);
            var mylist = from e in newlist
                         select new
                         {
                             #region 获取值
                             e.Id,
                             e.EmployeeId,
                             empName = empmanage.GetEntity(e.EmployeeId).EmpName,//姓名
                             Depart = empmanage.GetDeptByEmpid(e.EmployeeId).DeptName,//部门
                             Position = empmanage.GetPositionByEmpid(e.EmployeeId).PositionName,//岗位
                             e.BaseSalary,
                             e.PositionSalary,
                             e.PerformancePay,
                             e.PersonalSocialSecurity,
                             e.SocialSecuritySubsidy,
                             e.NetbookSubsidy,
                             e.ContributionBase,
                             e.PersonalIncomeTax,
                             e.Remark
                             #endregion


                         };
            var newobj = new
            {
                code = 0,
                msg = "",
                count = eselist.Count(),
                data = mylist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑员工工资体系表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditESE(int id) {
            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            var ese = esemanage.GetEntity(id);
            ViewBag.eseid = id;
            return View(ese);
        }
        [HttpPost]
        public ActionResult EditESE(EmplSalaryEmbody ese) {
            var AjaxResultxx = new AjaxResult();
            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            try
            {
                var myese = esemanage.GetEntity(ese.Id);
                var emp = emanage.GetInfoByEmpID(ese.EmployeeId);
                //if (string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                //{
                //    ese.PositionSalary = emp.ProbationSalary - ese.BaseSalary ;
                //}
                //else {
                //    ese.PositionSalary = emp.Salary - ese.BaseSalary ;
                //}
                //if (!string.IsNullOrEmpty(ese.PerformancePay.ToString())) {
                //    ese.PositionSalary =emp.Salary- ese.BaseSalary - ese.PerformancePay;
                //}
                
                ese.IsDel = myese.IsDel;
                esemanage.Update(ese);
                rc.RemoveCache("InRedisESEData");
                AjaxResultxx= esemanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = esemanage.Error(ex.Message);
            }
            return Json(AjaxResultxx,JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetESEById(int id)
        {
            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var ese = esemanage.GetEntity(id);
            var newobj = new {
                ese.Id,
                ese.EmployeeId,
                empName=empmanage.GetEntity(ese.EmployeeId).EmpName,
                deptName=empmanage.GetDeptByEmpid(ese.EmployeeId).DeptName,
                pName=empmanage.GetPositionByEmpid(ese.EmployeeId).PositionName,
                ese.BaseSalary,
                ese.PositionSalary,
                ese.PerformancePay,
                ese.NetbookSubsidy,
                ese.PersonalSocialSecurity,
                ese.SocialSecuritySubsidy,
                ese.ContributionBase,
                ese.PersonalIncomeTax,
                ese.Remark,
                ese.IsDel
            };
            return Json(newobj,JsonRequestBehavior.AllowGet);
       }

        /// <summary>
        /// 批量修改员工社保缴费基数和个人社保
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ActionResult UpdateEmpSSinfo(string list) {
            ViewBag.idlist = list;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateEmpSSinfo(string idlist, string ContributionBase, string PersonalSocialSecurity) {
            var AjaxResultxx = new AjaxResult();
            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            try
            {
                string[] ids = idlist.Split(',');
                for (int i = 0; i < ids.Length - 1; i++)
                {
                    string id = ids[i];
                    var ad = esemanage.GetEntity(int.Parse(id));
                    ad.ContributionBase =int.Parse(ContributionBase);
                    ad.PersonalSocialSecurity =decimal.Parse(PersonalSocialSecurity);
                    esemanage.Update(ad);
                    rc.RemoveCache("InRedisESEData"); 
                    AjaxResultxx = esemanage.Success();
                }
            }
            catch (Exception ex)
            {
                AjaxResultxx = esemanage.Error(ex.Message);
            }
            return Json(AjaxResultxx,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Import()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Import(HttpPostedFileBase excelfile)
        {
            Stream filestream = excelfile.InputStream;
            EmplSalaryEmbodyManage emplSalary = new EmplSalaryEmbodyManage();
            var result = emplSalary.ImportDataFormExcel(filestream, excelfile.ContentType);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}