using SiliconValley.InformationSystem.Business.Channel;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.Consult_Business;
using SiliconValley.InformationSystem.Business.DormitoryBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Employment;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
using SiliconValley.InformationSystem.Business.EmpTransactionBusiness;
using SiliconValley.InformationSystem.Business.FinanceBusiness;
using SiliconValley.InformationSystem.Business.SchoolAttendanceManagementBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    public class EmpTransactionRecordController : Controller
    {
        RedisCache rc = new RedisCache();

        // GET: Personnelmatters/EmpTransactionRecord
        public ActionResult EmpTransactionRecordIndex()
        {
            return View();
        }

        //获取员工异动表数据
        public ActionResult GetEtrData(int page, int limit, string AppCondition)
        {
            EmpTransactionManage etmanage = new EmpTransactionManage();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var list = etmanage.GetList().Where(s=>s.IsDel==false).ToList();
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string deptname = str[1];
                string pname = str[2];
                string Type = str[3];
                string start_time = str[4];
                string end_time = str[5];
                list = list.Where(e => emanage.GetInfoByEmpID(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(deptname))
                {
                    list = list.Where(e => emanage.GetDeptByEmpid(e.EmployeeId).DeptId == int.Parse(deptname)).ToList();
                }
                if (!string.IsNullOrEmpty(pname))
                {
                    list = list.Where(e => emanage.GetPositionByEmpid(e.EmployeeId).Pid == int.Parse(pname)).ToList();
                }
                if (!string.IsNullOrEmpty(Type))
                {
                    list = list.Where(e => e.TransactionType==int.Parse(Type)).ToList();
                }
                if (!string.IsNullOrEmpty(start_time))
                {
                    DateTime stime = Convert.ToDateTime(start_time + " 00:00:00.000");
                    list = list.Where(a => a.TransactionTime >= stime).ToList();
                }
                if (!string.IsNullOrEmpty(end_time))
                {
                    DateTime etime = Convert.ToDateTime(end_time + " 23:59:59.999");
                    list = list.Where(a => a.TransactionTime <= etime).ToList();
                }
            }
            var mylist = list.OrderByDescending(e => e.TransactionId).Skip((page - 1) * limit).Take(limit).ToList();
            var etlist = from e in mylist
                         select new
                         {
                             e.TransactionId,
                             empName = emanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                             type = emanage.GetETById(e.TransactionType).MoveTypeName,
                             e.TransactionTime,
                             predname = e.PreviousDept == null ? null : emanage.GetDeptById((int)e.PreviousDept).DeptName,
                             prepname = e.PreviousPosition == null ? null : emanage.GetPobjById((int)e.PreviousPosition).PositionName,
                             nowdname = e.PresentDept == null ? null : emanage.GetDeptById((int)e.PresentDept).DeptName,
                             nowpname = e.PresentPosition == null ? null : emanage.GetPobjById((int)e.PresentPosition).PositionName,
                             e.Remark,
                             e.PreviousSalary,
                             e.PresentSalary,
                             e.Reason,
                             e.BeforeContractStartTime,
                             e.BeforeContractEndTime,
                             e.AfterContractStartTime,
                             e.AfterContractEndTime,
                             e.IsDel
                         };
            var newobj = new
            {
                code = 0,
                msg = "",
                count = list.Count(),
                data = etlist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);


        }
        //编辑员工异动信息      
        public ActionResult EditETR(int id)
        {
            EmpTransactionManage etmanage = new EmpTransactionManage();
            var et = etmanage.GetEntity(id);
            ViewBag.id = id;
            return View(et);
        }
        //根据编号获取异动对象信息
        public ActionResult GetertById(int id)
        {
            EmpTransactionManage etmanage = new EmpTransactionManage();
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            var e = etmanage.GetEntity(id);
            var empobj = new
            {
                #region 获取属性值 
                e.TransactionId,
                empName = emanage.GetInfoByEmpID(e.EmployeeId).EmpName,
                esex = emanage.GetInfoByEmpID(e.EmployeeId).Sex,
                dname = emanage.GetDept(emanage.GetInfoByEmpID(e.EmployeeId).PositionId).DeptName,
                pname = emanage.GetPosition(emanage.GetInfoByEmpID(e.EmployeeId).PositionId).PositionName,
                EntryTime = emanage.GetInfoByEmpID(e.EmployeeId).EntryTime,
                education = emanage.GetInfoByEmpID(e.EmployeeId).Education,
                positiveDate = emanage.GetInfoByEmpID(e.EmployeeId).PositiveDate,
                type = emanage.GetETById(e.TransactionType).MoveTypeName,
                e.TransactionTime,
                predname = e.PreviousDept == null ? null : emanage.GetDeptById((int)e.PreviousDept).DeptName,
                prepname = e.PreviousPosition == null ? null : emanage.GetPobjById((int)e.PreviousPosition).PositionName,
                nowdname = e.PresentDept == null ? null : emanage.GetDeptById((int)e.PresentDept).DeptName,
                nowpname = e.PresentPosition == null ? null : emanage.GetPobjById((int)e.PresentPosition).PositionName,
                e.Remark,
                e.PreviousSalary,
                e.PresentSalary,
                e.Reason,
                e.BeforeContractStartTime,
                e.BeforeContractEndTime,
                e.AfterContractStartTime,
                e.AfterContractEndTime,
                e.IsDel,
                e.BeforePositiveDate,
                e.AfterPositiveDate,
                e.PreviousInternshipSalary,
                e.PresentInternshipSalary
                #endregion
            };

            return Json(empobj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult EditETR(EmpTransaction et)
        {
            EmpTransactionManage etmanage = new EmpTransactionManage();
            var ajaxresult = new AjaxResult();
            var e = etmanage.GetEntity(et.TransactionId);
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            MoveTypeManage mt = new MoveTypeManage();

            try
            {
                e.TransactionTime = et.TransactionTime;
                e.Remark = et.Remark;
                etmanage.Update(e);
                ajaxresult = etmanage.Success();
                try
                {
                    var mtype1 = mt.GetList().Where(s => s.MoveTypeName == "转正").FirstOrDefault().ID;
                    // var mtype2 = mt.GetList().Where(s => s.MoveTypeName == "离职").FirstOrDefault().ID;
                    var mtype3 = mt.GetList().Where(s => s.MoveTypeName == "调岗").FirstOrDefault().ID;
                    var myype4 = mt.GetList().Where(s => s.MoveTypeName == "加薪").FirstOrDefault().ID;
                    var emp = empmanage.GetEntity(e.EmployeeId);
                    //当异动时间修改好之后且是转正异动的情况下将该员工的转正日期修改
                    if (ajaxresult.Success && e.TransactionType == mtype1)
                    {
                        emp.PositiveDate = e.TransactionTime;
                        empmanage.Update(emp);
                        rc.RemoveCache("InRedisEmpInfoData");
                        ajaxresult = empmanage.Success();
                        if (ajaxresult.Success) {
                            //员工转正时间修改好之后将该员工的绩效工资及岗位工资修改一下
                                EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                               
                            ajaxresult = esemanage.EditEseByEmp(emp);
                                //并将该员工绩效分默认改为100
                                if (ajaxresult.Success)
                                {
                                    MeritsCheckManage mcmanage = new MeritsCheckManage();
                                    var mcemp = mcmanage.GetmcempByEmpid(emp.EmployeeId);
                                    ajaxresult.Success = mcemp;
                                }
                           
                        }
                    }
                    #region 注释              
                    //else if (ajaxresult.Success && e.TransactionType == mtype2)//当异动时间修改好之后且是离职异动的情况下将该员工的在职状态改为离职状态
                    //{
                    //    emp.IsDel = true;
                    //    empmanage.Update(emp);
                    //    ajaxresult = empmanage.Success();

                    //}
                    //当异动时间修改好之后且是调岗异动的情况下将该员工的工资及岗位进行修改
                    //else if (ajaxresult.Success && e.TransactionType == mtype3)
                    //{
                    //    emp.PositionId = (int)e.PresentPosition;
                    //    if (string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                    //    {
                    //        emp.ProbationSalary = e.PresentSalary;
                    //    }
                    //    else
                    //    {
                    //        emp.Salary = e.PresentSalary;
                    //    }
                    //    empmanage.Update(emp);
                    //    ajaxresult = empmanage.Success();
                    //}
                    //else if (ajaxresult.Success && e.TransactionType == myype4)
                    //{
                    //    if (ajaxresult.Success)
                    //    {//异动修改成功后且是加薪异动的情况下将员工表中的员工工资也改变    
                    //        if (string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                    //        {
                    //            emp.ProbationSalary = et.PresentSalary;
                    //        }
                    //        else
                    //        {
                    //            emp.Salary = et.PresentSalary;
                    //        }
                    //        empmanage.Update(emp);
                    //        ajaxresult = empmanage.Success();
                    //    }
                    //}
                    #endregion
                }
                catch (Exception ex)
                {
                    ajaxresult = empmanage.Error(ex.Message);
                }

            }
            catch (Exception ex)
            {
                ajaxresult = etmanage.Error(ex.Message);
            }

            return Json(ajaxresult, JsonRequestBehavior.AllowGet);
        }
        //员工异动详情信息
        public ActionResult EmpETRDetail(int id)
        {
            EmpTransactionManage etmanage = new EmpTransactionManage();
            var et = etmanage.GetEntity(id);
            ViewBag.id = id;
            return View(et);
        }

        //异动信息添加
        public ActionResult AddTransactionInfo() {
            MoveTypeManage mt = new MoveTypeManage();
            var mtlist = mt.GetList().Where(s=>s.IsDel==false).ToList();
            ViewBag.etrType = mtlist;
            return View();
        }
       
        /// <summary>
        ///  异动信息添加时选择员工
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectEmp(string type) {
            ViewBag.type = type;
            return View();
        }
        //获取员工
        public ActionResult GetEmpData(int page, int limit,string type, string ename)
        {
            EmployeesInfoManage empinfo = new EmployeesInfoManage();
            MoveTypeManage mtmanage = new MoveTypeManage();
            var typename = mtmanage.GetList().Where(s=>s.ID==int.Parse(type)).FirstOrDefault();
            //当选择的异动类型是离职或调岗或加薪时，只加载在职员工
            var list = empinfo.GetList().Where(e => e.IsDel == false).ToList();
           
            //当选择的异动类型是转正时，就只加载未转正的在职员工
            if (typename.MoveTypeName == "转正")
            {
                list = list.Where(s =>string.IsNullOrEmpty(s.PositiveDate.ToString())).ToList();
            }
            if (!string.IsNullOrEmpty(ename))
            {
                list = list.Where(e => e.EmpName.Contains(ename)).ToList();
            }
            var mylist = list.OrderBy(e => e.EmployeeId).Skip((page - 1) * limit).Take(limit).ToList();
            var newlist = from e in mylist
                          select new
                          {
                              #region 获取属性值 
                              e.EmployeeId,
                              e.DDAppId,
                              e.EmpName,
                              Position = empinfo.GetPosition((int)e.PositionId).PositionName,
                              Depart = empinfo.GetDept((int)e.PositionId).DeptName,
                              e.PositionId,
                              empinfo.GetDept((int)e.PositionId).DeptId,
                              e.Sex,
                              e.Age,
                              e.Nation,
                              e.Phone,
                              e.IdCardNum,
                              e.ContractStartTime,
                              e.ContractEndTime,
                              e.EntryTime,
                              e.Birthdate,
                              e.Birthday,
                              e.PositiveDate,
                              e.UrgentPhone,
                              e.DomicileAddress,
                              e.Address,
                              e.Education,
                              e.MaritalStatus,
                              e.IdCardIndate,
                              e.PoliticsStatus,
                              e.InvitedSource,
                              e.ProbationSalary,
                              e.Salary,
                              e.SSStartMonth,
                              e.BCNum,
                              e.Material,
                              e.Remark,
                              e.IsDel
                              #endregion

                          };
            var newobj = new
            {
                code = 0,
                msg = "",
                count = list.Count(),
                data = newlist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 提交异动信息的添加
        /// </summary>
        /// <param name="etr"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddEtrInfo(EmpTransaction etr)
        {
            var ajaxresult = new AjaxResult();
            EmpTransactionManage etrmanage = new EmpTransactionManage();
            MoveTypeManage mtmanage = new MoveTypeManage();
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            try
            {
                etr.IsDel = false;
                etrmanage.Insert(etr);
                ajaxresult = etrmanage.Success();
            }
            catch (Exception ex)
            {
                ajaxresult = etrmanage.Error(ex.Message);
            }

            if (ajaxresult.Success) {
                var mtname = mtmanage.GetList().Where(s => s.IsDel == false && s.ID == etr.TransactionType).FirstOrDefault().MoveTypeName;
                var emp = empmanage.GetEntity(etr.EmployeeId);//这是未改变部门岗位之前的员工对象
                if (mtname.Equals("离职")) {
                    #region 员工表（及相关子表）修改（离职）
                    emp.IsDel = true;
                    empmanage.Update(emp);
                    rc.RemoveCache("InRedisEmpInfoData");
                    ajaxresult = empmanage.Success();

                    if (ajaxresult.Success)
                    {
                        bool result = empmanage.DelEmpToCorrespondingDept(emp);//将对应的部门员工表状态也改变                                                                                         
                        ajaxresult.Success = result;
                    }

                }
                #endregion
                else if (mtname.Equals("转正"))
                {
                    if (ajaxresult.Success)
                    {
                        emp.PositiveDate = etr.TransactionTime;
                        empmanage.Update(emp);
                        rc.RemoveCache("InRedisEmpInfoData");
                        ajaxresult = empmanage.Success();

                        //员工转正时间修改好之后将该员工的绩效工资及岗位工资修改一下
                        if (ajaxresult.Success)
                        {
                            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                            ajaxresult = esemanage.EditEseByEmp(emp);
                            //并将该员工绩效分默认改为100
                            //if (ajaxresult.Success)
                            //{
                            //    MeritsCheckManage mcmanage = new MeritsCheckManage();
                            //    var mcemp = mcmanage.GetmcempByEmpid(emp.EmployeeId);
                            //    ajaxresult.Success = mcemp;
                            //}
                        }
                    }
                }
                else if (mtname.Equals("调岗"))
                {
                    if (ajaxresult.Success)
                    {
                        emp.PositionId = (int)etr.PresentPosition;
                        emp.PositiveDate = etr.AfterPositiveDate;
                        if (string.IsNullOrEmpty(etr.AfterPositiveDate.ToString())) {
                            emp.ProbationSalary = etr.PresentInternshipSalary;
                            emp.Salary = etr.PresentSalary;
                        }
                        empmanage.Update(emp);
                        rc.RemoveCache("InRedisEmpInfoData");
                        ajaxresult = empmanage.Success();
                        if (ajaxresult.Success)
                        {
                            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                          
                            ajaxresult = esemanage.EditEseByEmp(emp);
                            if (ajaxresult.Success) {
                                var emp2 = empmanage.GetInfoByEmpID(etr.EmployeeId);//这是部门岗位改变之后的员工对象
                                if (etr.PreviousDept!=etr.PresentDept) {
                                    ajaxresult.Success = empmanage.DelEmpToCorrespondingDept(emp);
                                    if (ajaxresult.Success) {
                                        ajaxresult.Success = empmanage.AddEmpToCorrespondingDept(emp2);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (mtname.Equals("加薪")) {
                    if (ajaxresult.Success) {
                        //异动添加成功后将员工表中的员工工资也改变
                        if (string.IsNullOrEmpty(emp.PositiveDate.ToString()))
                        {
                            emp.ProbationSalary = etr.PresentInternshipSalary;
                        }
                        else
                        {
                            emp.Salary = etr.PresentSalary;
                        }
                        empmanage.Update(emp);
                        rc.RemoveCache("InRedisEmpInfoData");
                        ajaxresult = empmanage.Success();
                        //员工工资改变之后，将员工岗位工资也改一下
                        if (ajaxresult.Success)
                        {
                            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                            ajaxresult = esemanage.EditEseByEmp(emp);
                           
                        }
                    }
                }
                else if (mtname.Equals("续签")) {
                    
                    emp.ContractStartTime = etr.AfterContractStartTime;
                    emp.ContractEndTime = etr.AfterContractEndTime;
                    empmanage.Update(emp);
                    rc.RemoveCache("InRedisEmpInfoData");
                    ajaxresult = empmanage.Success();
                }
            }

            return Json(ajaxresult,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除异动信息，即修改异动信息的状态
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteTransactionInfo(string list) {
            EmpTransactionManage brmanage = new EmpTransactionManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                string[] arr = list.Split(',');

                for (int i = 0; i < arr.Length - 1; i++)
                {
                    string id = arr[i];
                    var br = brmanage.GetEntity(int.Parse(id));
                    br.IsDel = true;
                    brmanage.Update(br);
                    AjaxResultxx = brmanage.Success();
                }
            }
            catch (Exception ex)
            {
                AjaxResultxx = brmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 异动类型下拉框绑定
        /// </summary>
        /// <returns></returns>
        public ActionResult BindTypeSelect() {
            MoveTypeManage mtmanage = new MoveTypeManage();
            var mtlist= mtmanage.GetList();
            var newstr = new
            {
                code = 0,
                msg = "",
                count = mtlist.Count(),
                data = mtlist
            };
            return Json(newstr,JsonRequestBehavior.AllowGet);
        }
    }
}