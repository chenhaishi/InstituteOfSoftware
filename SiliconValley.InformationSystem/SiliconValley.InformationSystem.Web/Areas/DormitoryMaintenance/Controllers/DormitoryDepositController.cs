using SiliconValley.InformationSystem.Business.DormitoryMantainBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.DormitoryMaintenance.Controllers
{
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Entity.Entity;
    using SiliconValley.InformationSystem.Util;
    public class DormitoryDepositController : Controller
    {
        DormitoryDepositManeger Dormitory_Entity = new DormitoryDepositManeger();
        // GET: /DormitoryMaintenance/DormitoryDeposit/GetDormitory
        public ActionResult DormitoryDepositIndex()
        {
            return View();
        }

        public ActionResult AddView()
        {
            //获取宿舍楼地址
            ViewBag.tung= Dormitory_Entity.Tung_Entity.GetList().Select(s=>new SelectListItem() { Value=s.Id.ToString(),Text=s.TungName}).ToList();
            //获取维修物品 
            ViewBag.goodname= Dormitory_Entity.DormitoryMaintenance_Entity.GetList().Select(s => new SelectListItem() { Value = s.ID.ToString(), Text = s.Nameofarticle }).ToList();
            return View();

        }

        /// <summary>
        /// 根据地址获取楼层
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetAllFoor(int id)
        {
            string sqlstr = @"select t.Id,t.FloorId ,t.CreationTime, d.FloorName as'Remark',t.IsDel,t.TungId from TungFloor as t 
 left join Dormitoryfloor as d on t.FloorId = d.ID
 where t.IsDel = 0 and t.TungId = " + id;
            List<SelectListItem> list =  Dormitory_Entity.GetListBySql<TungFloor>(sqlstr).Select(t=>new SelectListItem() { Text=t.Remark,Value=t.Id.ToString()}).ToList();

            //tlist.ForEach(t=> {
            //     Dormitory_Entity.Dormitoryfloor_Entity.GetList().Where(d => d.ID == t.FloorId).ToList();
            //});
            
            return Json(list,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取宿舍
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDormitory(int id)
        {
           List<SelectListItem> list= Dormitory_Entity.DormInformation_Entity.GetList().Where(s => s.TungFloorId == id).ToList().Select(s => new SelectListItem() { Text = s.DormInfoName, Value = s.ID.ToString() }).ToList();

            return Json(list,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取某个日期中属于XX宿舍的所有学生
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSusheStudent()
        {
            DateTime date=Convert.ToDateTime( Request.Form["date"]);//日期
            int susheNumber = Convert.ToInt32(Request.Form["sushenumber"]);//宿舍编号
           

            List<Accdationinformation> list = Dormitory_Entity.GetStudentSushe(date, susheNumber);//获取属于这个寝室的所有学生

            List<SelectListItem> studentlist = new List<SelectListItem>();

            list.ForEach(l=> {
               StudentInformation student= Dormitory_Entity.StudentInformation_Entity.GetEntity(l.Studentnumber);
                if (student!=null)
                {
                    SelectListItem item = new SelectListItem() {Text=student.Name,Value=student.StudentNumber };

                    studentlist.Add(item);
                }
            });

            AjaxResult result = new AjaxResult() { Data= studentlist, Success=true};
            if (list.Count<0)
            {
                result.Success = false;
            }


            return Json(result,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFunction()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息

            DateTime mantinDate=Convert.ToDateTime(Request.Form["mantinDate"]);//维修的日期

            int weixiugood =Convert.ToInt32(Request.Form["weixiugood"]);//维修物品

            int dorname = Convert.ToInt32(Request.Form["dorname"]);//宿舍编号

            int JieType = Convert.ToInt32(Request.Form["JieType"]);//类型

            Pricedormitoryarticles goods= Dormitory_Entity.Pricedormitoryarticles_Entity.GetEntity(weixiugood);//查询维修物品的信息

            List<DormitoryDeposit> Dormlist = new List<DormitoryDeposit>();

            int sturadi = Convert.ToInt32(Request.Form["sturadi"]);//学生编号

            if (JieType==1)
            {
                
                //寝室平摊
                List<Accdationinformation> list=  Dormitory_Entity.GetStudentSushe(mantinDate, dorname); //获取属于这个寝室的学生

                List<StudentInformation> studentlist = new List<StudentInformation>();


                list.ForEach(l => {
                    StudentInformation student = Dormitory_Entity.StudentInformation_Entity.GetEntity(l.Studentnumber);
                    if (student != null)
                    {                        
                        studentlist.Add(student);
                    }
                });

                DormitoryDeposit dormitory = new DormitoryDeposit();

                dormitory.CreaDate = DateTime.Now;
                dormitory.DormId = dorname;
                dormitory.EntryPersonnel = UserName.EmpNumber;
                dormitory.GoodPrice = (goods.Reentry / studentlist.Count);
                dormitory.Maintain = mantinDate;
                dormitory.MaintainGood = weixiugood;
                //dormitory.MaintainState=
            }
            else if (JieType == 2)
            {
                //个人承担

            }

            return null;
        }
    }
}