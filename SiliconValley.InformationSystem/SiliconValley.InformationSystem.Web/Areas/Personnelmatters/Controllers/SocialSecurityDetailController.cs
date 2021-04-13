using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiliconValley.InformationSystem.Business.SocialSecurity_Business;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Util;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    public class SocialSecurityDetailController : Controller
    {
        // GET: Personnelmatters/SocialSecurityDetail
        RedisCache rc=new RedisCache();
        static string GetFirstTime()
        {
            SocialSecurityDetailManage  social = new SocialSecurityDetailManage();//员工月度社保详情
            string mytime = "";
            if (social.GetSocData().Where(s => s.IsDel == false).Count() > 0)
            {
                var time = social.GetSocData().Where(s => s.IsDel == false).LastOrDefault().CurrentYearAndMonth;
                mytime = DateTime.Parse(time.ToString()).Year + "-" + DateTime.Parse(time.ToString()).Month;
            }
            else
            {
                mytime = "";
            }
            return mytime;
        }
        static string FirstTime = GetFirstTime();
        public ActionResult Index()
        {
            @ViewBag.yearandmonth = FirstTime;
            var year = 0;
            if (!string.IsNullOrEmpty(FirstTime))
            {
                 year = Convert.ToDateTime(FirstTime).Month -1;
            if (year==0)
            {
                year = 12;
            }
            }
            ViewBag.year = year;
            SocialSecProportionManage socialSec = new SocialSecProportionManage();
            var comsoc = socialSec.GetbyType("单位部分");
            var persoc = socialSec.GetbyType("个人部分");
            if (persoc!=null)
            {

            ViewBag.PersonalEndowmentInsurance = persoc.EndowmentInsurance ;
            ViewBag.PersonalMedicalInsurance = persoc.MedicalInsurance ;
            ViewBag.PersonalUnemploymentInsurance = persoc.UnemploymentInsurance ;
            }
            //if (comsoc!=null)
            //{

            //}
           

            //SocialSecurityDetailManage social = new SocialSecurityDetailManage();

            return View(comsoc);
        }
        public ActionResult GetSocialSecurityList(int page,int limit, string AppCondition, string ymtime)
        {
            ymtime = FirstTime;
            SocialSecProportionManage socialSec = new SocialSecProportionManage();
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
            List<SocialSecurityView> securityViews = new List<SocialSecurityView>();
            EmployeesInfoManage manage = new EmployeesInfoManage();

            var list = social.GetSocData().Where(s => s.IsDel == false).ToList();
            if (!string.IsNullOrEmpty(ymtime))
            {
                var time = DateTime.Parse(ymtime);
                list = list.Where(s => DateTime.Parse(s.CurrentYearAndMonth.ToString()).Year == time.Year && DateTime.Parse(s.CurrentYearAndMonth.ToString()).Month == time.Month&&string.IsNullOrEmpty(s.OverPayMonthNum.ToString())).ToList();
            }
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string ename = str[0];
                string deptname = str[1];
                string pname = str[2];
                string Empstate = str[3];
                list = list.Where(e => manage.GetInfoByEmpID(e.EmployeeId).EmpName.Contains(ename)).ToList();
                if (!string.IsNullOrEmpty(deptname))
                {
                    list = list.Where(e => manage.GetDeptByEmpid(e.EmployeeId).DeptId == int.Parse(deptname)).ToList();
                }
                if (!string.IsNullOrEmpty(pname))
                {
                    list = list.Where(e => manage.GetPositionByEmpid(e.EmployeeId).Pid == int.Parse(pname)).ToList();
                }
                if (!string.IsNullOrEmpty(Empstate))
                {
                    list = list.Where(e => manage.GetInfoByEmpID(e.EmployeeId).IsDel == bool.Parse(Empstate)).ToList();
                }

            }
            var soclist = list.Skip((page - 1) * limit).Take(limit).ToList();
            foreach (var item in soclist)
            {
                SocialSecurityView soc = new SocialSecurityView();
                var comsoc = socialSec.GetbyType("单位部分");
                var persoc = socialSec.GetbyType("个人部分");
                soc.Id = item.Id;
                soc.EmployeeId = item.EmployeeId;
                soc.empName = manage.GetEntity(item.EmployeeId).EmpName;
                soc.Depart = manage.GetDeptByEmpid(item.EmployeeId).DeptName;
                soc.Position = manage.GetPositionByEmpid(item.EmployeeId).PositionName;
                soc.Type = manage.GetEntity(item.EmployeeId).IsDel==false?"在职":"离职";
                soc.CurrentYearAndMonth = item.CurrentYearAndMonth;
                soc.YearAndMonth =Convert.ToDateTime(item.CurrentYearAndMonth).ToString("yyyy年MM月");
                soc.PaymentBase = item.PaymentBase;
                soc.SeriousIllnessInsurance = item.SeriousIllnessInsurance;
                soc.OverPayMonthNum = item.OverPayMonthNum;
                soc.UnitTotal = item.UnitTotal;
                soc.PersonalTotal = item.PersonalTotal;
                soc.EndowmentInsurance = item.PaymentBase * comsoc.EndowmentInsurance* (decimal)0.01;
                soc.MedicalInsurance = item.PaymentBase * comsoc.MedicalInsurance * (decimal)0.01;
                soc.WorkInjuryInsurance = item.PaymentBase * comsoc.WorkInjuryInsurance * (decimal)0.01;
                soc.MaternityInsurance = item.PaymentBase * comsoc.MaternityInsurance * (decimal)0.01;
                soc.UnemploymentInsurance = item.PaymentBase * comsoc.UnemploymentInsurance * (decimal)0.01;
                soc.PersonalEndowmentInsurance = item.PaymentBase * persoc.EndowmentInsurance * (decimal)0.01;
                soc.PersonalMedicalInsurance = item.PaymentBase * persoc.MedicalInsurance * (decimal)0.01;
                soc.PersonalUnemploymentInsurance = item.PaymentBase * persoc.UnemploymentInsurance * (decimal)0.01;
                soc.Total = social.total((DateTime)item.CurrentYearAndMonth,item.EmployeeId);
                soc.Count = social.Count((DateTime)item.CurrentYearAndMonth,item.EmployeeId);
                if (social.SupplementaryPayment((DateTime)item.CurrentYearAndMonth, item.EmployeeId)!=null)
                {
                soc.UnitSupplementaryPayment = social.SupplementaryPayment((DateTime)item.CurrentYearAndMonth, item.EmployeeId).UnitTotal;
                soc.PersonalSupplementaryPayment = social.SupplementaryPayment((DateTime)item.CurrentYearAndMonth, item.EmployeeId).PersonalTotal;
                }
               
                securityViews.Add(soc);
                
            }
            var newobj = new
            {
                code = 0,
                msg = "",
                count = list.Count(),
                data = securityViews
            };

            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SocialSecProportionEdit()
        {
            SocialSecProportionManage social = new SocialSecProportionManage();
            var s = social.GetbyType("单位部分");
            var soc = social.GetbyType("个人部分");
            if (soc != null)
            {
            ViewBag.PersonalEndowmentInsurance= social.GetbyType("个人部分").EndowmentInsurance;
            ViewBag.PersonalMedicalInsurance = social.GetbyType("个人部分").MedicalInsurance;
            ViewBag.PersonalUnemploymentInsurance = social.GetbyType("个人部分").UnemploymentInsurance;
            }

            return View(s);
        }
        [HttpPost]
        public ActionResult SocialSecProportionEdit(SocialSecProportionView soc)
        {
            SocialSecProportionManage social = new SocialSecProportionManage();

            AjaxResult result = new AjaxResult();
            try
            {
                var unit = social.GetbyType("单位部分");
                unit.UnemploymentInsurance = soc.UnemploymentInsurance;
                unit.WorkInjuryInsurance = soc.WorkInjuryInsurance;
                unit.EndowmentInsurance = soc.EndowmentInsurance;
                unit.MedicalInsurance = soc.MedicalInsurance;
                unit.MaternityInsurance = soc.MaternityInsurance;
                social.Update(unit);

                var personal= social.GetbyType("个人部分");
                personal.EndowmentInsurance = soc.PersonalEndowmentInsurance;
                personal.MedicalInsurance = soc.PersonalMedicalInsurance;
                personal.UnemploymentInsurance = soc.PersonalUnemploymentInsurance;
                social.Update(personal);

                rc.RemoveCache("InRedisSocialSecurityData");
                result.Success = true;
                result.Msg = "社保比例修改成功";
            }
            catch (Exception ex)
            {
                result = social.Error(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult UpdateTime()
        {
            ViewBag.time = FirstTime;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateTime(string CurrentTime)
        {
            var AjaxResultxx = new AjaxResult();
            var newobj = new object();
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
            var soclist = social.GetSocData().Where(s => s.IsDel == false).ToList();
            var nowtime = DateTime.Parse(CurrentTime);
            var matchlist = soclist.Where(m => DateTime.Parse(m.CurrentYearAndMonth.ToString()).Year == nowtime.Year && DateTime.Parse(m.CurrentYearAndMonth.ToString()).Month == nowtime.Month).ToList();
            AjaxResultxx.Data = matchlist.Count();
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SocialSecurityRefresh(string time)
        {
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
            bool result = true;
            if (social.CreateSalTab(time))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            FirstTime = time;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SupplementaryPayment(int id)
        {
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
           ViewBag.empid = social.GetEntity(id).EmployeeId;
            return View();
        }
        [HttpPost]
        public ActionResult SupplementaryPayment(SocialSecurityDetail soc)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                SocialSecurityDetailManage socialmanage = new SocialSecurityDetailManage();
                soc.CurrentYearAndMonth =Convert.ToDateTime(FirstTime);
                soc.IsDel = false;
                socialmanage.Insert(soc);
                

                var s = socialmanage.GetList().Where(i=>i.EmployeeId==soc.EmployeeId&&string.IsNullOrEmpty(i.OverPayMonthNum.ToString())&&i.CurrentYearAndMonth==soc.CurrentYearAndMonth).FirstOrDefault();
                s.UnitTotal += soc.UnitTotal;
                s.PersonalTotal += soc.PersonalTotal;

                socialmanage.Update(s);
                rc.RemoveCache("InRedisSocialSecurityData");

                result = socialmanage.Success();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PaySicknessInsurance(int id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult PaySicknessInsurance(SocialSecurityDetail soc)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                SocialSecurityDetailManage socialmanage = new SocialSecurityDetailManage();
                var s = socialmanage.GetEntity(soc.Id);
                s.SeriousIllnessInsurance = soc.SeriousIllnessInsurance;
                s.PersonalTotal += soc.SeriousIllnessInsurance;

                socialmanage.Update(s);
                var emp = esemanage.GetEseByEmpid(soc.EmployeeId);
                emp.PersonalSocialSecurity = s.PersonalTotal;
                esemanage.Update(emp);
                rc.RemoveCache("InRedisSocialSecurityData");

                result = socialmanage.Success();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditSupplementaryPayment(int id)
        {
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
            var list = social.GetsocialsList(id);
            ViewBag.list=list;
            return View();
        }
        [HttpPost]
        public ActionResult EditSupplementaryPayment(SocialSecurityDetail soc)
        {
            AjaxResult result = new AjaxResult();
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();

            try
            {
                var so = social.GetEntity(soc.Id);
                var s = social.GetSocial((DateTime)so.CurrentYearAndMonth, so.EmployeeId);
                if ((bool)soc.IsDel)
                {
                    s.UnitTotal -= so.UnitTotal;
                    s.PersonalTotal -= so.PersonalTotal;
                }
                else
                {
                    s.UnitTotal = (s.UnitTotal - so.UnitTotal) + soc.UnitTotal;
                    s.PersonalTotal = (s.PersonalTotal - so.PersonalTotal) + soc.PersonalTotal;
                }
                if (!(bool)soc.IsDel && (bool)so.IsDel)
                {
                    s.UnitTotal += so.UnitTotal;
                    s.PersonalTotal += so.PersonalTotal;
                }


                social.Update(s);
                so.UnitTotal = soc.UnitTotal;
                so.PersonalTotal = soc.PersonalTotal;
                so.PaymentBase = soc.PaymentBase;
                so.IsDel = soc.IsDel;
                social.Update(so);
                rc.RemoveCache("InRedisSocialSecurityData");

                EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                var emplSalary=  esemanage.GetEseByEmpid(so.EmployeeId);
                emplSalary.PersonalSocialSecurity = soc.PersonalTotal;
                esemanage.Update(emplSalary);

                result = social.Success();
            }
            catch (Exception e)
            {
                result = social.Error(e.TargetSite+e.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public ActionResult EditPaySicknessInsurance(int id)
        {
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
            var list = social.GetEntity(id);
            return View(list);
        }
        [HttpPost]
        public ActionResult EditPaySicknessInsurance(SocialSecurityDetail soc)
        {
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
            AjaxResult result = new AjaxResult();
            try
            {
                var so = social.GetEntity(soc.Id);
                so.PersonalTotal = (so.PersonalTotal - so.SeriousIllnessInsurance) + soc.SeriousIllnessInsurance;
                so.SeriousIllnessInsurance =soc.SeriousIllnessInsurance ;

                social.Update(so);
                rc.RemoveCache("InRedisSocialSecurityData");
                result = social.Success();
            }
            catch (Exception e)
            {
                result = social.Error(e.TargetSite + e.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SicknessInsurance(string id,int SeriousIllnessInsurance)
        {
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
            EmplSalaryEmbodyManage emplSalary = new EmplSalaryEmbodyManage();
            AjaxResult result = new AjaxResult();
            try
            {
                var soclies = social.GetListBySql<SocialSecurityDetail>("select *from SocialSecurityDetail  where Id in(" + id + ")");
                foreach (var item in soclies)
                {
                    if (string.IsNullOrEmpty(item.SeriousIllnessInsurance.ToString())||item.SeriousIllnessInsurance==0)
                    {
                        item.PersonalTotal = item.PersonalTotal + SeriousIllnessInsurance;
                    }
                    else
                    {
                        item.PersonalTotal = (item.PersonalTotal - item.SeriousIllnessInsurance) + SeriousIllnessInsurance;

                    }
                          item.SeriousIllnessInsurance = SeriousIllnessInsurance;              
                    social.Update(item);
                   var salary= emplSalary.GetEseByEmpid(item.EmployeeId);
                    salary.PersonalSocialSecurity = item.PersonalTotal;
                    rc.RemoveCache("InRedisSocialSecurityData");
                    
                }

                result = social.Success();
                result.ErrorCode = 200;

            }
            catch (Exception ex)
            {
                result = social.Error(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MultipleSocialSecurity(string id)
        {
            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult MultipleSocialSecurity(string id,SocialSecurityDetail soc)
        {
            SocialSecurityDetailManage social = new SocialSecurityDetailManage();
            AjaxResult result = new AjaxResult();
            try
            {
                var soclies = social.GetListBySql<SocialSecurityDetail>("select *from SocialSecurityDetail  where Id in(" + id + ")");
                foreach (var item in soclies)
                {
                    soc.EmployeeId = item.EmployeeId;
                    soc.CurrentYearAndMonth = Convert.ToDateTime(FirstTime);
                    soc.IsDel = false;
                    social.Insert(soc);


                    var s = social.GetSocial(Convert.ToDateTime(FirstTime), item.EmployeeId);
                    s.UnitTotal += soc.UnitTotal;
                    s.PersonalTotal += soc.PersonalTotal;

                    social.Update(s);
                    rc.RemoveCache("InRedisSocialSecurityData");
                }
                result = social.Success();
                result.ErrorCode = 200;

            }
            catch (Exception ex)
            {
                result = social.Error(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
    }
} 