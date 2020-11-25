using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.SocialSecurity_Business
{
   public class SocialSecurityDetailManage: BaseBusiness<SocialSecurityDetail>
    {
        RedisCache rc;
        public List<SocialSecurityDetail> GetSocData()
        {
            rc = new RedisCache();
            rc.RemoveCache("InRedisSocialSecurityData");
            List<SocialSecurityDetail>  soclist = new List<SocialSecurityDetail>();
            if (soclist == null || soclist.Count() == 0)
            {
                soclist = this.GetIQueryable().ToList();
                rc.SetCache("InRedisSocialSecurityData", soclist);
            }
            soclist = rc.GetCache<List<SocialSecurityDetail>>("InRedisSocialSecurityData");
            return soclist;
        }
        public bool CreateSalTab(string time)
        {
            bool result = false;
            try
            {
                var msrlist = this.GetSocData().Where(s => s.IsDel == false).ToList();
                EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
                SocialSecProportionManage social = new SocialSecProportionManage();
                var emplist = esemanage.GetEmpESEData().Where(s => s.IsDel == false&&!string.IsNullOrEmpty(s.ContributionBase.ToString())).ToList();
                //    var emplist = empmanage.GetEmpInfoData();
                var nowtime = DateTime.Parse(time);

                //匹配是否有该月（选择的年月即传过来的参数）的社保明细数据
                var matchlist = msrlist.Where(m => DateTime.Parse(m.CurrentYearAndMonth.ToString()).Year == nowtime.Year && DateTime.Parse(m.CurrentYearAndMonth.ToString()).Month == nowtime.Month).ToList();

                if (matchlist.Count() <= 0)//表示社保明细表中无该月的数据
                {
                  
                    foreach (var item in emplist)
                    {//再将未禁用的员工添加到社保明细表中
                        SocialSecurityDetail soc = new SocialSecurityDetail();
                        var unit = social.GetbyType("单位部分");
                        var personal = social.GetbyType("个人部分");
                        decimal[] unitindex = {(decimal)unit.EndowmentInsurance,(decimal)unit.MaternityInsurance,(decimal)unit.MedicalInsurance,(decimal)unit.UnemploymentInsurance,(decimal)unit.WorkInjuryInsurance };
                        decimal[] personalindex = {(decimal)personal.UnemploymentInsurance,(decimal)personal.EndowmentInsurance,(decimal)personal.MedicalInsurance };
                        soc.EmployeeId = item.EmployeeId;
                        soc.PaymentBase = item.ContributionBase;
                        soc.UnitTotal = GetTotal((int)item.ContributionBase, unitindex);
                        soc.PersonalTotal = GetTotal((int)item.ContributionBase, personalindex);
                        soc.CurrentYearAndMonth = Convert.ToDateTime(time);
                        soc.IsDel = false;
                        this.Insert(soc);

                      var emp=  esemanage.GetEseByEmpid(item.EmployeeId);
                        emp.PersonalSocialSecurity= GetTotal((int)item.ContributionBase, personalindex);
                        esemanage.Update(emp);
                        rc.RemoveCache("InRedisSocialSecurityData");
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;

            }
            return result;
        }
        /// <summary>
        /// 计算个人小计合计或单位小计
        /// </summary>
        /// <param name="PaymentBase">缴费基数</param>
        /// <param name="index"></param>
        /// <returns></returns>
        public decimal GetTotal(int PaymentBase,decimal [] index)
        {
            decimal num = 0;
            for (int i = 0; i < index.Length; i++)
            {
                num += PaymentBase * index[i]*(decimal)0.01;
            }
            return num;
        }
        /// <summary>
        /// 计算社保合计
        /// </summary>
        /// <param name="time"></param>
        /// <param name="empid"></param>
        /// <returns></returns>
        public decimal total(DateTime time,string empid)
        {
            decimal? num = 0;
            var list = GetSocData().Where(i => Convert.ToDateTime(i.CurrentYearAndMonth).Year == time.Year && Convert.ToDateTime(i.CurrentYearAndMonth).Month == time.Month&&i.EmployeeId==empid);
           

            var total = list.FirstOrDefault();

                    if (string.IsNullOrEmpty(total.SeriousIllnessInsurance.ToString()))
                    {
                        total.SeriousIllnessInsurance = 0;
                    }
                    num += total.PersonalTotal + total.UnitTotal;

            return (decimal)num;

        }
        /// <summary>
        /// 获取补缴社保详情
        /// </summary>
        /// <param name="time"></param>
        /// <param name="empid"></param>
        /// <returns></returns>
        public SocialSecurityDetail SupplementaryPayment(DateTime time, string empid)
        {
            var list = GetSocData().Where(i => Convert.ToDateTime(i.CurrentYearAndMonth).Year == time.Year && Convert.ToDateTime(i.CurrentYearAndMonth).Month == time.Month && i.EmployeeId == empid&& !string.IsNullOrEmpty(i.OverPayMonthNum.ToString())).FirstOrDefault();

            return list;
        }
        public List<SocialSecurityDetail > GetsocialsList(int id)
        {
            var soc = GetList().Where(i=>i.Id==id).FirstOrDefault();
            var list = GetList().Where(i=>i.EmployeeId==soc.EmployeeId&&!string.IsNullOrEmpty(i.OverPayMonthNum.ToString())&&i.CurrentYearAndMonth==soc.CurrentYearAndMonth).ToList();

            return list;
        }
        public SocialSecurityDetail GetSocial(DateTime time, string empid)
        {
            var list = GetSocData().Where(i => Convert.ToDateTime(i.CurrentYearAndMonth).Year == time.Year && Convert.ToDateTime(i.CurrentYearAndMonth).Month == time.Month && i.EmployeeId == empid && string.IsNullOrEmpty(i.OverPayMonthNum.ToString())).FirstOrDefault();
            return list;
        }
        /// <summary>
        /// 补缴条数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="empid"></param>
        /// <returns></returns>
       public int Count(DateTime time,string empid)
        {
            var count = GetSocData().Where(i => Convert.ToDateTime(i.CurrentYearAndMonth).Year == time.Year && Convert.ToDateTime(i.CurrentYearAndMonth).Month == time.Month && i.EmployeeId == empid && !string.IsNullOrEmpty(i.OverPayMonthNum.ToString())).Count();

            return count;
        }
    }
}
