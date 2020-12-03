using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Personnelmatters.Controllers
{
    using SiliconValley.InformationSystem.Business.RecruitingDataSummaryBusiness;
    using SiliconValley.InformationSystem.Business.TalentDemandPlanBusiness;
    using SiliconValley.InformationSystem.Business.PositionBusiness;
    using SiliconValley.InformationSystem.Business.DepartmentBusiness;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Business.RecruitPhoneTraceBusiness;

    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Util;
    using System.Text;
    using System.IO;
    using SiliconValley.InformationSystem.Business.Common;
    using SiliconValley.InformationSystem.Entity.Base_SysManage;
    using System.Data;
    using SiliconValley.InformationSystem.Entity.ViewEntity;

    public class RecruitingDataSummaryController : Controller
    {
        // GET: Personnelmatters/RecruitingDataSummary
        public ActionResult RecruitIndex()
        {
            return View();
        }

        //获取招聘电话追踪数据
        public ActionResult GetTraceData(int page, int limit,string AppCondition)
        {
            RecruitPhoneTraceManage rptmanage = new RecruitPhoneTraceManage();
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var rdslist = rptmanage.GetRptFromSql()/*.Where(s => s.IsDel == false).ToList()*/;
            List<RecruitPhoneTraceView> rptviewlist = new List<RecruitPhoneTraceView>();
            foreach (var item in rdslist)
            {
                RecruitPhoneTraceView rptview = new RecruitPhoneTraceView();
                var dept = empmanage.GetDeptByPid((int)item.Pid);
                #region 获取值
                rptview.Id = item.Id;
                rptview.Pid = item.Pid;
                rptview.Dname = dept.DeptName;
                rptview.Deptid = dept.DeptId;
                rptview.Pname = empmanage.GetPobjById((int)item.Pid).PositionName;
                rptview.Name = item.Name;
                rptview.PhoneNumber = item.PhoneNumber;
                rptview.TraceTime = item.TraceTime;
                rptview.Channel = item.Channel;
                rptview.ResumeType = item.ResumeType;
                rptview.PhoneCommunicateResult = item.PhoneCommunicateResult;
                rptview.IsEntry = rptmanage.GetNewest((int)item.SonId).IsEntry;
                rptview.Remark = item.Remark;
                rptview.IsDel = item.IsDel;
                rptview.SonId = item.SonId;
                rptview.forwardDate = rptmanage.GetNewestForwardDate((int)item.SonId);
                rptview.result = rptmanage.GetPhoneCommunicateResult((int)item.SonId);
                rptview.NewesRemark = rptmanage.GetNewest((int)item.SonId).Remark;
                rptview.NewesIsEntry = rptmanage.GetNewest((int)item.SonId).IsEntry;
                #endregion
                rptviewlist.Add(rptview);
            }

            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string name = str[0];
                string deptid = str[1];
                string pid = str[2];
                string PhoneNumber = str[3];
                string Channel = str[4];
                string ResumeType = str[5];
                string result = str[6];
                string IsEntry = str[7];
                string start_TraceTime = str[8];
                string end_TraceTime = str[9];
                string start_ForwardDate = str[10];
                string end_ForwardDate = str[11];
                string remarks = str[12];
                rptviewlist = rptviewlist.Where(e => e.Name.Contains(name)).ToList();
              
                if (!string.IsNullOrEmpty(deptid))
                {
                    rptviewlist = rptviewlist.Where(e => empmanage.GetDept((int)e.Pid).DeptId== int.Parse(deptid)).ToList();
                }
                if (!string.IsNullOrEmpty(pid))
                {
                    rptviewlist = rptviewlist.Where(e => e.Pid == int.Parse(pid)).ToList();
                }
                if (!string.IsNullOrEmpty(PhoneNumber))
                {
                    rptviewlist = rptviewlist.Where(e => e.PhoneNumber.Contains(PhoneNumber)).ToList();
                }
                
                if (!string.IsNullOrEmpty(Channel))
                {
                    rptviewlist = rptviewlist.Where(e =>e.Channel.Contains(Channel)).ToList();
                }
                if (!string.IsNullOrEmpty(ResumeType))
                {
                    rptviewlist = rptviewlist.Where(e =>e.ResumeType.Contains(e.ResumeType)).ToList();
                }
                if (!string.IsNullOrEmpty(result))
                {
                    rptviewlist = rptviewlist.Where(e => e.result == Convert.ToBoolean(result)).ToList();
                }
                if (!string.IsNullOrEmpty(IsEntry))
                {
                    rptviewlist = rptviewlist.Where(e => e.IsEntry==Convert.ToBoolean(IsEntry)).ToList();
                }
                if (!string.IsNullOrEmpty(start_TraceTime))
                {
                    DateTime stime = Convert.ToDateTime(start_TraceTime.Substring(0,11));
                    rptviewlist = rptviewlist.Where(a => a.TraceTime >= stime).ToList();
                }
                if (!string.IsNullOrEmpty(end_TraceTime))
                {
                    DateTime stime = Convert.ToDateTime(end_TraceTime);
                    rptviewlist = rptviewlist.Where(a => a.TraceTime <= stime).ToList();
                }
                if (!string.IsNullOrEmpty(start_ForwardDate))
                {
                    DateTime stime = Convert.ToDateTime(start_ForwardDate );
                    rptviewlist = rptviewlist.Where(a => a.forwardDate >= stime).ToList();
                }
                if (!string.IsNullOrEmpty(end_ForwardDate))
                {
                    DateTime stime = Convert.ToDateTime(end_ForwardDate);
                    rptviewlist = rptviewlist.Where(a => a.forwardDate <= stime).ToList();
                }
                if (!string.IsNullOrEmpty(remarks))
                {
                    rptviewlist = rptviewlist.Where(e => e.NewesRemark != null&& e.NewesRemark.Contains(remarks)).ToList();
                }

            }
            var myrdslist = rptviewlist.OrderByDescending(r => r.Id).Skip((page - 1) * limit).Take(limit).ToList();
         
            var newobj = new
            {
                code = 0,
                msg = "",
                count = rptviewlist.Count(),
                data = myrdslist
            }; 
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        #region 获取某个部门或岗位

        /// <summary>
        /// 获取部门对象
        /// </summary>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public Department GetDept(int deptid)
        {
            DepartmentManage dmanage = new DepartmentManage();
            var deptobj = dmanage.GetEntity(deptid);
            return deptobj;
        }

        /// <summary>
        ///获取所属岗位对象
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Position GetPosition(int pid)
        {
            PositionManage pmanage = new PositionManage();
            var str = pmanage.GetEntity(pid);
            return str;
        }

        /// <summary>
        /// 根据部门名称获取部门编号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetDeptidByName(string name)
        {
            DepartmentManage dmanage = new DepartmentManage();
            var dept = dmanage.GetList().Where(s => s.DeptName == name).FirstOrDefault();
            return dept.DeptId;
        }
        /// <summary>
        /// 根据岗位名称获取岗位编号
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetPidByName(string name)
        {
            PositionManage pmanage = new PositionManage();
            var pid = pmanage.GetList().Where(p => p.PositionName == name).FirstOrDefault().Pid;
            return pid;
        }
        #endregion
        /// <summary>
        /// 通过编号获取某条招聘记录数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetRPTById(int id)
        {
            RecruitPhoneTraceManage rptmanage = new RecruitPhoneTraceManage();
            var rptview = rptmanage.GetRptView(id);
            return Json(rptview, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加招聘电话追踪基本信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Addrpt()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Addrpt(RecruitPhoneTrace rpt)
        {
            RecruitPhoneTraceManage rmanage = new RecruitPhoneTraceManage();
            var count = 0;
            var AjaxResultxx = new AjaxResult();
            try
            {
                rpt.IsEntry = false;
                rpt.IsDel = false;
                rpt.PhoneCommunicateResult = false;

               count = rmanage.GetList().Where(i=>i.Name==rpt.Name&&i.PhoneNumber==rpt.PhoneNumber).Count();
               
                    rmanage.Insert(rpt);
                    AjaxResultxx = rmanage.Success();
               

            }
            catch (Exception ex)

            {
                AjaxResultxx = rmanage.Error(ex.Message);
            }
            if (AjaxResultxx.Success)
            {
                    rpt.SonId = rpt.Id;
                    rmanage.Update(rpt);
                    AjaxResultxx = rmanage.Success();

                AjaxResultxx = rmanage.Success();
            }
            if (AjaxResultxx.Success)
            {
                AddRecruitData();
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加招聘的面试记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddTrack(int id)
        {
            RecruitPhoneTraceManage rmanage = new RecruitPhoneTraceManage();
            ViewBag.Id = id;
            var rds = rmanage.GetEntity(id);
            var rdslist = rmanage.GetList().Where(r => r.SonId == rds.SonId).ToList();
            ViewBag.Number = rdslist.Count() - 1;
            ViewBag.date =rmanage.GetNewestForwardDate(id);
            // ViewBag.rdslist = rdslist;
            return View();
        }
        [HttpPost]
        public ActionResult AddTrack(RecruitPhoneTrace rpt)
        {
            RecruitPhoneTraceManage rptmanage = new RecruitPhoneTraceManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                var beforerpt = rptmanage.GetList().Where(r => r.Id == rpt.Id && r.IsDel == false).FirstOrDefault();
                RecruitPhoneTrace rptnew = new RecruitPhoneTrace();
                rptnew.SonId = beforerpt.SonId;
                rptnew.Name = beforerpt.Name;
                rptnew.PhoneNumber = beforerpt.PhoneNumber;
                rptnew.TraceTime = rpt.TraceTime;
                rptnew.ForwardDate = rpt.ForwardDate;
                rptnew.PhoneCommunicateResult = rpt.PhoneCommunicateResult;
                rptnew.Channel = beforerpt.Channel;
                rptnew.ResumeType = beforerpt.ResumeType;
                rptnew.IsEntry = beforerpt.IsEntry;
                rptnew.Remark = rpt.Remark;
                rptnew.IsDel = true;
                rptnew.Pid = rpt.Pid;

                rptmanage.Insert(rptnew);
                AjaxResultxx = rptmanage.Success();
                if (AjaxResultxx.Success)
                {
                    beforerpt.PhoneCommunicateResult = rptnew.PhoneCommunicateResult;
                    rptmanage.Update(beforerpt);
                    AjaxResultxx = rptmanage.Success();
                   
                }

            } 
            catch (Exception ex)
            {
                AjaxResultxx = rptmanage.Error(ex.Message);
            }

            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑某条招聘追踪数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditTrack(int id)
        {
            RecruitPhoneTraceManage rmanage = new RecruitPhoneTraceManage();
            List<RecruitPhoneTraceView> rptviewlist = new List<RecruitPhoneTraceView>();

            ViewBag.Id = id;
            rptviewlist = rmanage.GetRptViewList(id);//获取回访记录集合
            ViewBag.rptviewlist = rptviewlist;
            ViewBag.Number = rptviewlist.Count();
            var rpt = rmanage.GetRptView(id);
            ViewBag.forwarddate = rmanage.GetNewestForwardDate(id);
            ViewBag.pid = rpt.Pid;
            ViewBag.pname = rpt.Pname;
            return View(rpt);
        }
        /// <summary>
        /// 没有面试记录的追踪编辑提交
        /// </summary>
        /// <param name="Tracklist"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditTracks(string Tracklist)
        {
            var AjaxResultxx = new AjaxResult();
            RecruitPhoneTraceManage rptmanage = new RecruitPhoneTraceManage();
            try
            {
                string[] str = Tracklist.Split(',');
                string id = str[0];
                string pid = str[1];
                string time = str[2];
                string PhoneNumber = str[3];
                string Channel = str[4];
                string ResumeType = str[5];
                string remark = str[6];
                string ForwardDate = str[7];
                var rpt = rptmanage.GetEntity(int.Parse(id));
                rpt.Pid = int.Parse(pid);
                rpt.TraceTime = Convert.ToDateTime(time);
                rpt.PhoneNumber = PhoneNumber;
                rpt.Channel = Channel;
                rpt.ResumeType = ResumeType;
                rpt.Remark = remark;
                rpt.ForwardDate = Convert.ToDateTime(ForwardDate);
                rptmanage.Update(rpt);

                AjaxResultxx = rptmanage.Success();

            }
            catch (Exception ex)
            {
                AjaxResultxx = rptmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 有面试记录的追踪编辑提交
        /// </summary>
        /// <param name="mydata"></param>
        /// <param name="Tracklist"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditTrack(RecruitPhoneTrace mydata, string Tracklist)
        {
            var AjaxResultxx = new AjaxResult();
            RecruitPhoneTraceManage rptmanage = new RecruitPhoneTraceManage();
            try
            {
                var rpt1 = rptmanage.GetEntity(mydata.Id);
                rpt1.TraceTime = mydata.TraceTime;
                rpt1.PhoneCommunicateResult = mydata.PhoneCommunicateResult;
                rpt1.Remark = mydata.Remark;
                rptmanage.Update(rpt1);
                AjaxResultxx = rptmanage.Success();
                if (AjaxResultxx.Success)
                {
                    string[] str = Tracklist.Split(',');
                    string id = str[0];
                    string pid = str[1];
                    string time = str[2];
                    string PhoneNumber = str[3];
                    string Channel = str[4];
                    string ResumeType = str[5];
                    string ForwardDate = str[7];
                    // string result = str[6];
                    string remark = str[6];
                    rptmanage.UpdNewestForwardDate(int.Parse(id), ForwardDate);
                    var rpt2 = rptmanage.GetEntity(int.Parse(id));
                    rpt2.Pid = int.Parse(pid);
                    rpt2.TraceTime = Convert.ToDateTime(time);
                    rpt2.PhoneNumber = PhoneNumber;
                    rpt2.Channel = Channel;
                    rpt2.ResumeType = ResumeType;
                    //  rpt2.PhoneCommunicateResult =Convert.ToBoolean(result);
                    rpt2.Remark = remark;
                    rpt2.PhoneCommunicateResult = rpt1.PhoneCommunicateResult;
                    rptmanage.Update(rpt2);

                    AjaxResultxx = rptmanage.Success();

                }
            }
            catch (Exception ex)
            {
                AjaxResultxx = rptmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 追踪记录详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult TrackDetailInfo(int id)
        {
            RecruitPhoneTraceManage rmanage = new RecruitPhoneTraceManage();
            List<RecruitPhoneTraceView> rptviewlist = new List<RecruitPhoneTraceView>();
            ViewBag.Id = id;
            rptviewlist = rmanage.GetRptViewList(id);//获取回访记录集合
            ViewBag.rptviewlist = rptviewlist;
            ViewBag.Number = rptviewlist.Count();
            return View();
        }

        public ActionResult IsEntry(int id)
        {
            RecruitPhoneTraceManage rmanage = new RecruitPhoneTraceManage();
            var rpt = rmanage.GetRptView(id);
            ViewBag.Id = id;
            ViewBag.pid = rpt.Pid;
            ViewBag.pname = rpt.Pname;
            return View();
        }
        [HttpPost]
        public ActionResult EdiIsEntrytTrack(int id)
        {
            RecruitPhoneTraceManage rmanage = new RecruitPhoneTraceManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                var rpt = rmanage.GetEntity(id);
                var rptlist = rmanage.GetList().Where(s => s.SonId == rpt.SonId).ToList();
                foreach (var item in rptlist)
                {
                    item.IsEntry = true;
                    rmanage.Update(item);
                }
                AjaxResultxx = rmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = rmanage.Error(ex.Message);
            }

            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }

        //获取月度招聘数据汇总
        public ActionResult GetRecruitData(int page, int limit, string AppCondition)
        {
            AddRecruitData();
            RecruitingDataSummaryManage rdsmanage = new RecruitingDataSummaryManage();
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var rdslist = rdsmanage.GetList();
            var myrdslist = rdslist.OrderByDescending(r => r.Id).Skip((page - 1) * limit).Take(limit).ToList();
            if (!string.IsNullOrEmpty(AppCondition))
            {
                string[] str = AppCondition.Split(',');
                string deptid = str[0];
                string pname = str[1];
                string start_time = str[2];
                string end_time = str[3];
                if (!string.IsNullOrEmpty(deptid))
                {
                    myrdslist = myrdslist.Where(e => empmanage.GetDeptByPid((int)e.Pid).DeptId == int.Parse(deptid)).ToList();
                }
                if (!string.IsNullOrEmpty(pname))
                {
                    myrdslist = myrdslist.Where(e => e.Pid == int.Parse(pname)).ToList();
                }
                if (!string.IsNullOrEmpty(start_time))
                {
                    DateTime stime = Convert.ToDateTime(start_time);
                    myrdslist = myrdslist.Where(a => a.YearAndMonth >= stime).ToList();
                }
                if (!string.IsNullOrEmpty(end_time))
                {
                    DateTime etime = Convert.ToDateTime(end_time);
                    myrdslist = myrdslist.Where(a => a.YearAndMonth <= etime).ToList();
                }
            }
            var newlist = from rds in myrdslist
                          select new
                          {
                              #region 赋值
                              rds.Id,
                              rds.YearAndMonth,
                              pname = GetPosition((int)rds.Pid).PositionName,
                              rds.PlanRecruitNum,
                              rds.ResumeSum,
                              rds.OutboundCallSum,
                              rds.InstantInviteSum,
                              rds.InstantToFacesSum,
                              rds.InstantRetestSum,
                              rds.InstantRetestPassSum,
                              rds.InstantEntryNum,
                              rds.InstantToFacesRate,
                              rds.InstantInviteRate,
                              rds.InstantRetestPassrate,
                              rds.EntryRate,
                              rds.RecruitPercentage,
                              rds.Remark
                              #endregion
                          };
            var newobj = new
            {
                code = 0,
                msg = "",
                count = rdslist.Count(),
                data = newlist
            };
            return Json(newobj, JsonRequestBehavior.AllowGet);
        }
        public string Condition(DateTime date, string type)
        {
            if (type == "day")
            {
                return date.ToString("yyyy-M-d");
            }
            else if (type == "month")
            {
                return date.ToString("yyyy-M");
            }
            return date.Year.ToString();
        }

        /// <summary>
        ///为计算某月某岗位的复试总数
        /// </summary>
        /// <param name="rptlist"></param>
        /// <returns></returns>
        public int GetRefacednum(List<RecruitPhoneTrace> rptlist)
        {
            int result = 0;
            var conditionrpt = rptlist.GroupBy(s => s.SonId);

            conditionrpt.ForEach(d =>
            {
                if (d.Count() > 1)
                {
                    result += d.Count() - 1;
                }

            });

            // result = conditionrpt.Count();
            return result;
        }
        //月度招聘数据汇总添加
        public AjaxResult AddRecruitData()
        {
            var AjaxResultxx = new AjaxResult();
            RecruitPhoneTraceManage rpt = new RecruitPhoneTraceManage();
            RecruitingDataSummaryManage rdsmanage = new RecruitingDataSummaryManage();
            List<RecruitingDataSummary> rlist = new List<RecruitingDataSummary>();

            var rptlist = rpt.GetList();
            try
            {
                var list = from r in rptlist
                           where r.TraceTime != null
                           group r by new
                           { r.Pid, month = Condition((DateTime)r.TraceTime, "month") }
                           into g
                           select g;
                foreach (var item in list)
                {
                    var month = item.Key.month;//月份
                    var position = item.Key.Pid;//岗位
                    var resumenum = item.Count(s => s.IsDel == false);//简历总数(排除这些应聘者的面试记录)
                    var PhoneCommunicatenum = item.Count(s => s.IsDel == false && !string.IsNullOrEmpty(Convert.ToString(s.TraceTime)));//联系的总数(有联系时间的才算联系过)
                    var invitednum = item.Count(t => t.IsDel == false && t.ForwardDate != null);//当月邀约总数
                    var Facednum = item.Count(t => t.IsDel == true);//当月到面总数
                    var Refacednum = GetRefacednum(item.Where(s => s.IsDel == true).ToList());//当月复试总数
                    var Refacepassednum = GetRefacednum(item.Where(s => s.IsDel == true && s.PhoneCommunicateResult == true).ToList());//当月复试通过总数
                    var Entrynum = item.Count(t => t.IsDel == false && t.IsEntry == true);//当月入职人数
                    #region 给对象赋值
                    RecruitingDataSummary rdsdate = new RecruitingDataSummary();
                    rdsdate.YearAndMonth = DateTime.Parse(month);
                    rdsdate.Pid = position;
                    rdsdate.ResumeSum = resumenum;
                    rdsdate.OutboundCallSum = PhoneCommunicatenum;
                    rdsdate.InstantInviteSum = invitednum;
                    rdsdate.InstantToFacesSum = Facednum;
                    rdsdate.InstantRetestSum = Refacednum;
                    rdsdate.InstantRetestPassSum = Refacepassednum;
                    rdsdate.InstantEntryNum = Entrynum;
                    if (invitednum != 0)
                    {
                        rdsdate.InstantToFacesRate = Convert.ToDecimal(Facednum) / Convert.ToDecimal(invitednum);
                    }
                    if (PhoneCommunicatenum != 0)
                    {
                        rdsdate.InstantInviteRate = Convert.ToDecimal(invitednum) / Convert.ToDecimal(PhoneCommunicatenum);
                    }
                    if (Refacednum != 0)
                    {
                        rdsdate.InstantRetestPassrate = Convert.ToDecimal(Refacepassednum) / Convert.ToDecimal(Refacednum);
                    }
                    if (Refacepassednum != 0)
                    {
                        rdsdate.EntryRate = Convert.ToDecimal(Entrynum) / Convert.ToDecimal(Refacepassednum);
                    }
                    var rds = rdsmanage.GetList().Where(a => a.Pid == rdsdate.Pid && Condition((DateTime)a.YearAndMonth, "month") == Condition((DateTime)rdsdate.YearAndMonth, "month")).FirstOrDefault();
                    if (rds != null)
                    {
                        rds.YearAndMonth = rdsdate.YearAndMonth;
                        rds.Pid = rdsdate.Pid;
                        rds.ResumeSum = rdsdate.ResumeSum;
                        rds.OutboundCallSum = rdsdate.OutboundCallSum;
                        rds.InstantInviteSum = rdsdate.InstantInviteSum;
                        rds.InstantToFacesSum = rdsdate.InstantToFacesSum;
                        rds.InstantRetestSum = rdsdate.InstantRetestSum;
                        rds.InstantRetestPassSum = rdsdate.InstantRetestPassSum;
                        rds.InstantEntryNum = rdsdate.InstantEntryNum;
                        rds.InstantToFacesRate = rdsdate.InstantToFacesRate;
                        rds.InstantInviteRate = rdsdate.InstantInviteRate;
                        rds.InstantRetestPassrate = rdsdate.InstantRetestPassrate;
                        rds.EntryRate = rdsdate.EntryRate;
                        if (!string.IsNullOrEmpty(rds.PlanRecruitNum.ToString()))
                        {
                            rds.RecruitPercentage = Convert.ToDecimal(rds.InstantEntryNum) / Convert.ToDecimal(rds.PlanRecruitNum);
                        }
                        else
                        {
                            rds.RecruitPercentage = 0;
                        }
                        rdsmanage.Update(rds);
                    }
                    else if (rdsmanage.GetList().Count() == 0)
                    {
                        rdsmanage.Insert(rdsdate);
                    }
                    else
                    {
                        rdsmanage.Insert(rdsdate);

                    }
                    #endregion
                }


                AjaxResultxx = rdsmanage.Success();

            }
            catch (Exception ex)
            {
                AjaxResultxx = rdsmanage.Error(ex.Message);
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.查询数据error);
            }
            return AjaxResultxx;
        }
        /// <summary>
        /// 招聘数据汇总[计划招聘人数]字段的单元格编辑方法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="endvalue"></param>
        /// <returns></returns>
        public ActionResult EditTableCell(int id, string attribute, string endvalue)
        {
            RecruitingDataSummaryManage rdsmanage = new RecruitingDataSummaryManage();
            var AjaxResultxx = new AjaxResult();
            try
            {
                var rds = rdsmanage.GetEntity(id);
                switch (attribute)
                {
                    case "PlanRecruitNum":
                        rds.PlanRecruitNum = int.Parse(endvalue);
                        if (!string.IsNullOrEmpty(rds.PlanRecruitNum.ToString()))
                        {
                            rds.RecruitPercentage = (decimal)rds.InstantEntryNum / (decimal)rds.PlanRecruitNum;
                        }
                        else { rds.RecruitPercentage = 0; }
                        rdsmanage.Update(rds);
                        break;
                    case "Remark":
                        rds.Remark = endvalue;
                        rdsmanage.Update(rds);
                        break;
                }
                AjaxResultxx = rdsmanage.Success();
            }
            catch (Exception ex)
            {
                AjaxResultxx = rdsmanage.Error(ex.Message);
            }
            return Json(AjaxResultxx, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DuplicateData(string name,string phone)
        {
            RecruitPhoneTraceManage recruit = new RecruitPhoneTraceManage();
            AjaxResult result = new AjaxResult();
            int count=0;
            try
            {
                if (!string.IsNullOrEmpty(phone))
                {
                     count= recruit.GetList().Where(i => i.Name == name && i.PhoneNumber == phone&&i.IsDel==false).Count();
                }
                if (count>0)
                {
                    result.Success = true;
                    result.ErrorCode = count;
                    result.Msg= "已有此人，请确认姓名或手机号正确";
                }
                else
                {
                    result = recruit.Success();
                }
               
            }
            catch (Exception e)
            {
                result = recruit.Error(e.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}