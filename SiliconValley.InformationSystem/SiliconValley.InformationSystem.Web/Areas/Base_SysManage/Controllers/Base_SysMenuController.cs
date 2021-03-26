using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.DormitoryMantainBusiness;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Base_SysManage.Controllers
{
    public class Base_SysMenuController : BaseMvcController
    {

        //员工业务类
        public EmployeesInfoManage EmployeesInfoManage_Entity = new EmployeesInfoManage();
        PricedormitoryarticlesManeger PriceManger = new PricedormitoryarticlesManeger();

        // GET: Base_SysManage/Base_SysMenu
        public ActionResult Index()
        {

            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();
            Department department = EmployeesInfoManage_Entity.GetDeptByEmpid(UserName.EmpNumber);
            ViewBag.Dept = department.DeptId;

            return View();
        }

        public ActionResult LoadMenus()
        {

            //加入登陆的用户是Admin


            //Base_User user = userdb.GetList().Where(u => u.UserId == "Admin").FirstOrDefault();

            //SessionHelper._Session session = new SessionHelper._Session();
            //session["UserId"] = user.UserId;

            //1.0：学生登陆的时候，
            //提前询问的是自主就业的学生，事先要排除掉，就不加载这个就业意向这个菜单
            List<Menu> menus = SystemMenuManage.GetOperatorMenu();

            return Json(menus, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取登陆人信息
        /// </summary>
        /// <returns></returns>
        public ActionResult UserClass()
        {

            //session["UserId"] = user.UserId;
            return Json(SystemMenuManage.UserClass(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查看宿舍物品价格表中的保险柜每月扣费是否正常
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public bool CheckDormState()
        {
            string sql = "select * from Pricedormitoryarticles where Nameofarticle='保险柜每月扣费'";
            Pricedormitoryarticles price = PriceManger.GetListBySql<Pricedormitoryarticles>(sql).FirstOrDefault();
            return price.Dateofregistration;
        }
        
    }
}