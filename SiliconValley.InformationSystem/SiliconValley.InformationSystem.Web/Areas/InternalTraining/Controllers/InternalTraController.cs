using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.InternalTraining.Controllers
{
    using SiliconValley.InformationSystem.Business.EducationalBusiness;
    using SiliconValley.InformationSystem.Business.ExaminationSystemBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Entity.Entity;
    using SiliconValley.InformationSystem.Business.DepartmentBusiness;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Util;
    using SiliconValley.InformationSystem.Entity.ViewEntity.zhongyike;
    [CheckLogin]
    public class InternalTraController : Controller
    {
        private readonly InternalTrainingCostBusiness db_internal;
        private readonly QuestionLevelBusiness db_questionLevel;

        public InternalTraController()
        {
            db_internal = new InternalTrainingCostBusiness();
            db_questionLevel = new QuestionLevelBusiness();
           
        }
        // GET: InternalTraining/InternalTra
        /// <summary>
        /// 内训主页
        /// </summary>
        /// <returns></returns>
        public ActionResult InternalTraIndex()
        {
            return View();
        }
        /// <summary>
        /// 内训添加页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddInternalTraIndex()
        {
            //提供难度级别数据
            GrandBusiness grandBusiness = new GrandBusiness();
            DepartmentManage dep = new DepartmentManage();
            //提供阶段数据
            ViewBag.Grand = grandBusiness.AllGrand();
            //提供部门数据
            ViewBag.bumeng = dep.GetList().Where(d => d.DeptId == 5 || d.DeptId == 6 || d.DeptId == 1008 || d.DeptId == 1009).ToList();
            return View();
        }
        /// <summary>
        /// 获取培训人
        /// </summary>
        /// <param name="grandid"></param>
        /// <returns></returns>
        public ActionResult huoqupxunren(int bumengid)
        {
           EmployeesInfoManage yuangong = new EmployeesInfoManage();
            var list = yuangong.GetEmpsByDeptid(bumengid).ToList();
            

            return Json(list, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 内训修改页面
        /// </summary>
        /// <returns></returns>
        public ActionResult XiuGaiInternalTraIndex()
        {
            //提供难度级别数据
            GrandBusiness grandBusiness = new GrandBusiness();
            DepartmentManage dep = new DepartmentManage();
            //提供阶段数据
            ViewBag.Grand = grandBusiness.AllGrand();
            //提供部门数据
            ViewBag.bumeng = dep.GetList().Where(d => d.DeptId == 5 || d.DeptId == 6 || d.DeptId == 1008 || d.DeptId == 1009).ToList();
            return View();
        }
        /// <summary>
        /// 展示数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowShuJu(int page, int limit)
        {
            string sql = "select * from InternalTrainingCost";

            var zhi = db_internal.GetListBySql<InternalTrainingCost>(sql).ToList();

            var list = zhi.Skip((page - 1) * limit).Take(limit).ToList();

            List<InternalTraView> Tra = new List<InternalTraView>();
            //阶段
            GrandBusiness grandBusiness = new GrandBusiness();
            //部门
            DepartmentManage dep = new DepartmentManage();
            //培训人
            EmployeesInfoManage emp = new EmployeesInfoManage();
            foreach (var item in list)
            {
                InternalTraView inter = new InternalTraView();
                inter.grandId = grandBusiness.GetList().Where(d=>d.Id == item.grandId).FirstOrDefault().GrandName;
                inter.Title = item.Title;
                inter.Contents = item.Contents;
                inter.Time = item.Time;
                inter.Department = dep.GetList().Where(d=>d.DeptId == item.Department).FirstOrDefault().DeptName;
                inter.Trainer = emp.GetList().Where(d=>d.EmployeeId == item.Trainer).FirstOrDefault().EmpName;
                inter.ClassHours = item.ClassHours;
                inter.Curriculum = item.Curriculum;

                Tra.Add(inter);
            }


            var obj = new
            {

                code = 0,
                msg = "",
                count = zhi.Count,
                data = Tra
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <returns></returns>
        public ActionResult AddShuJu(string Title,int Grand,string Trainer,int Department,DateTime Time,string Curriculum,int ClassHours,string Contents)
        {
            AjaxResult result = new AjaxResult();
            try
            {
                InternalTrainingCost cost = new InternalTrainingCost();
                cost.ID = Guid.NewGuid().ToString();
                cost.Title = Title;
                cost.grandId = Grand;
                cost.Trainer = Trainer;
                cost.Department = Department;
                cost.Time = Time;
                cost.Curriculum = Curriculum;
                cost.ClassHours = ClassHours;
                cost.Contents = Contents;
                cost.IsUsing = false;
                cost.State = 0;
                db_internal.Insert(cost);

                result.ErrorCode = 200;
                result.Msg = "成功";
                result.Data = null;
            }
            catch (Exception ex)
            {
                result.ErrorCode = 500;
                result.Msg = "失败";
                result.Data = null;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public ActionResult XiuGaiShuJu()
        {
            return null;
        }
    }
}