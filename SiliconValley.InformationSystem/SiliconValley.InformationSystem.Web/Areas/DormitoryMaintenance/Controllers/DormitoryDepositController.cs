using SiliconValley.InformationSystem.Business.DormitoryMantainBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.DormitoryMaintenance.Controllers
{
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Entity.Entity;
    using SiliconValley.InformationSystem.Entity.ViewEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
    using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.MyViewEntity;
    using SiliconValley.InformationSystem.Util;
    using System.Text;
    [CheckLogin]
    public class DormitoryDepositController : Controller
    {
        DormitoryDepositManeger Dormitory_Entity = new DormitoryDepositManeger();
        // GET: /DormitoryMaintenance/DormitoryDeposit/StuNameDefileFuntion

        #region 登记人操作

        public ActionResult DormitoryDepositIndex()
        {
            //Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            //int number = Dormitory_Entity.Number(UserName.EmpNumber);
            //ViewBag.number = number; 
            return View();
        }


        /// <summary>
        /// 第一次数据加载
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetData(int limit, int page)
        {
            string sql = "select * from DormitoryDeposit";

            List<DormitoryDeposit> listall = Dormitory_Entity.GetListBySql<DormitoryDeposit>(sql);
            var data = listall.OrderByDescending(l => l.CreaDate).Skip((page - 1) * limit).Take(limit).Select(l => new
            {
                ID = l.ID,
                Maintain = l.Maintain,//维修日期
                DorName = Dormitory_Entity.DormInformation_Entity.GetEntity(l.DormId).DormInfoName,//房间编号
                EmpName = Dormitory_Entity.EmployeesInfo_Entity.GetEntity(l.EntryPersonnel).EmpName,//录入人员
                GoodPrice = l.GoodPrice,
                Nameofarticle = Dormitory_Entity.DormitoryMaintenance_Entity.GetEntity(l.MaintainGood).Nameofarticle,//物品名称
                MaintainState = l.MaintainState,
                stuName = Dormitory_Entity.StudentInformation_Entity.GetEntity(l.StuNumber).Name,
                ChuangNumber = l.ChuangNumber,//床位号
                ClassName = Dormitory_Entity.GetClass(l.StuNumber)
            }).ToList();

            var jsondata = new { count = listall.Count, code = 0, data = data };

            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DoubleGetData(int limit, int page)
        {
            string stuname = Request.QueryString["stuName"];
            string startime = Request.QueryString["starTime"];
            string endtime = Request.QueryString["endTime"];

            StringBuilder sql = new StringBuilder("select * from DormitoryDeposit where 1=1");

            if (!string.IsNullOrEmpty(startime))
            {
                sql.Append(" and Maintain>= '" + startime + "'");
            }


            if (!string.IsNullOrEmpty(endtime))
            {
                sql.Append(" and Maintain<= '" + endtime + "'");
            }

            List<DormitoryDeposit> listall = Dormitory_Entity.GetListBySql<DormitoryDeposit>(sql.ToString());
            var data = listall.OrderByDescending(l => l.CreaDate).Skip((page - 1) * limit).Take(limit).Select(l => new
            {
                ID = l.ID,
                Maintain = l.Maintain,//维修日期
                DorName = Dormitory_Entity.DormInformation_Entity.GetEntity(l.DormId).DormInfoName,//房间编号
                EmpName = Dormitory_Entity.EmployeesInfo_Entity.GetEntity(l.EntryPersonnel).EmpName,//录入人员
                GoodPrice = l.GoodPrice,
                Nameofarticle = Dormitory_Entity.DormitoryMaintenance_Entity.GetEntity(l.MaintainGood).Nameofarticle,//物品名称
                MaintainState = l.MaintainState,
                stuName = Dormitory_Entity.StudentInformation_Entity.GetEntity(l.StuNumber).Name,
                ChuangNumber = l.ChuangNumber,
                ClassName = Dormitory_Entity.GetClass(l.StuNumber)
            }).ToList();


            if (!string.IsNullOrEmpty(stuname))
            {
                data = data.Where(d => d.stuName.Contains(stuname)).ToList();
            }

            var jsondata = new { count = listall.Count, code = 0, data = data };

            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AddView()
        {
            //获取宿舍楼地址
            ViewBag.tung = Dormitory_Entity.Tung_Entity.GetList().Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.TungName }).ToList();
            //获取维修物品 
            ViewBag.goodname = Dormitory_Entity.DormitoryMaintenance_Entity.GetList().Where(s => s.Dateofregistration == true).Select(s => new SelectListItem() { Value = s.ID.ToString(), Text = s.Nameofarticle }).ToList();
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
            List<SelectListItem> list = Dormitory_Entity.GetListBySql<TungFloor>(sqlstr).Select(t => new SelectListItem() { Text = t.Remark, Value = t.Id.ToString() }).ToList();

            //tlist.ForEach(t=> {
            //     Dormitory_Entity.Dormitoryfloor_Entity.GetList().Where(d => d.ID == t.FloorId).ToList();
            //});

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取宿舍
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDormitory(int id)
        {
            List<SelectListItem> list = Dormitory_Entity.DormInformation_Entity.GetList().Where(s => s.TungFloorId == id).ToList().Select(s => new SelectListItem() { Text = s.DormInfoName, Value = s.ID.ToString() }).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取某个日期中属于XX宿舍的所有学生
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetSusheStudent()
        {
            DateTime date = Convert.ToDateTime(Request.Form["date"]);//日期
            int susheNumber = Convert.ToInt32(Request.Form["sushenumber"]);//宿舍编号


            List<Accdationinformation> list = Dormitory_Entity.GetStudentSushe(date, susheNumber);//获取属于这个寝室的所有学生

            List<SelectListItem> studentlist = new List<SelectListItem>();

            list.ForEach(l =>
            {
                StudentInformation student = Dormitory_Entity.StudentInformation_Entity.GetEntity(l.Studentnumber);
                if (student != null)
                {
                    SelectListItem item = new SelectListItem() { Text = student.Name, Value = student.StudentNumber };

                    studentlist.Add(item);
                }
            });

            AjaxResult result = new AjaxResult() { Data = studentlist, Success = true };
            if (list.Count < 0)
            {
                result.Success = false;
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddFunction()
        {

            AjaxResult result = new AjaxResult() { Success = true, Msg = "登记成功！" };


            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息

            DateTime mantinDate = Convert.ToDateTime(Request.Form["mantinDate"]);//维修的日期

            int weixiugood = Convert.ToInt32(Request.Form["weixiugood"]);//维修物品

            int dorname = Convert.ToInt32(Request.Form["dorname"]);//宿舍编号

            int JieType = Convert.ToInt32(Request.Form["JieType"]);//类型

            Pricedormitoryarticles goods = Dormitory_Entity.Pricedormitoryarticles_Entity.GetEntity(weixiugood);//查询维修物品的信息

            List<DormitoryDeposit> Dormlist = new List<DormitoryDeposit>();//存放学生维修费用

            string sturadi = Request.Form["sturadi"];//学生编号

            if (JieType == 1)
            {

                //寝室平摊
                List<Accdationinformation> list = Dormitory_Entity.GetStudentSushe(mantinDate, dorname); //获取属于这个寝室的学生

                List<StudentInformation> studentlist = new List<StudentInformation>();


                list.ForEach(l =>
                {
                    StudentInformation student = Dormitory_Entity.StudentInformation_Entity.GetEntity(l.Studentnumber);
                    if (student != null)
                    {
                        studentlist.Add(student);

                    }
                });

                if (studentlist.Count > 0)
                {
                    foreach (var stu in studentlist)
                    {
                        DormitoryDeposit dormitory = new DormitoryDeposit();
                        dormitory.ID = Guid.NewGuid().ToSequentialGuid();
                        dormitory.CreaDate = DateTime.Now;
                        dormitory.DormId = dorname;
                        dormitory.EntryPersonnel = UserName.EmpNumber;
                        dormitory.GoodPrice = goods.Reentry / studentlist.Count;
                        dormitory.Maintain = mantinDate;
                        dormitory.MaintainGood = weixiugood;
                        dormitory.MaintainState = 1;
                        dormitory.StuNumber = stu.StudentNumber;
                        dormitory.ChuangNumber = list.Where(l => l.Studentnumber == stu.StudentNumber).FirstOrDefault().BedId;
                        Dormlist.Add(dormitory);
                    }

                    bool s = Dormitory_Entity.AddData(Dormlist);

                    if (s == false)
                    {
                        result.Msg = "网络异常，请刷新重试！";
                        result.Success = false;
                    }
                }
                else
                {
                    result.Msg = "该寝室没有学生入住！";
                    result.Success = false;
                }






            }
            else if (JieType == 2)
            {
                //个人承担
                DormitoryDeposit dormitory = new DormitoryDeposit();
                dormitory.ID = Guid.NewGuid().ToSequentialGuid();
                dormitory.CreaDate = DateTime.Now;
                dormitory.DormId = dorname;
                dormitory.EntryPersonnel = UserName.EmpNumber;
                dormitory.GoodPrice = goods.Reentry;
                dormitory.Maintain = mantinDate;
                dormitory.MaintainGood = weixiugood;
                dormitory.MaintainState = 1;
                dormitory.StuNumber = sturadi;

                bool s = Dormitory_Entity.AddData(dormitory);

                if (s == false)
                {
                    result.Msg = "网络异常，请刷新重试！";
                    result.Success = false;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 班主任操作
        /// <summary>
        /// 班级费用查看页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassStudentMoneyView()
        {

            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            int number = Dormitory_Entity.Number(UserName.EmpNumber);
            ViewBag.number = number;
            if (number == 2)
            {
                //获取登录人信息
                string empname = Dormitory_Entity.EmployeesInfo_Entity.GetEntity(UserName.EmpNumber).EmpName;
                //获取班主任所带的班级
                List<SelectListItem> classlist = Dormitory_Entity.Headmaster_Entity.ListTeamleaderdistributionView().Where(h => h.HeadmasterName == empname).Select(h => new SelectListItem() { Text = h.ClassName, Value = h.ClassID.ToString() }).ToList();

                classlist.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
                ViewBag.classlist = classlist;
            }
            else if (number == 3 || number == 1)
            {
                //获取所有阶段
                List<SelectListItem> grandlist = Dormitory_Entity.Grand_Entity.GetList().Where(g => g.IsDelete == false).Select(g => new SelectListItem() { Text = g.GrandName, Value = g.Id.ToString() }).ToList();


                ViewBag.grandlist = grandlist;
            }

            return View();
        }

        /// <summary>
        /// 数据第一次加载
        /// </summary>
        /// <returns></returns>
        public ActionResult OneData()
        {
            List<StudentDorMoney> list = new List<StudentDorMoney>() { new StudentDorMoney() { StuName = "请查询", PayMoney = 0, MantainMoney = 0, SumMoney = 0, BaoxianguiMoney = 0 } };
            var jsondata = new { code = 0, count = 0, data = list };
            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取某个班级的所有学生维修费用(包括保险柜押金扣除的费用)
        /// </summary>
        /// <returns></returns>
        public ActionResult ClassMoneyFuntion()
        {
            int classid = Convert.ToInt32(Request.QueryString["classid"]);

            string sqlstr = "select * from ScheduleForTrainees where ID_ClassName ='" + classid + "' and CurrentClass =1 ";

            List<ScheduleForTrainees> stulist = Dormitory_Entity.GetListBySql<ScheduleForTrainees>(sqlstr);//获取这个班级的所有学生

            List<StudentDorMoney> stumoneylist = new List<StudentDorMoney>();

            stulist.ForEach(s =>
            {
                StudentDorMoney data = new StudentDorMoney()
                {
                    StuName = Dormitory_Entity.StudentInformation_Entity.GetEntity(s.StudentID).Name,
                    StuNumber = s.StudentID,
                    PayMoney = Dormitory_Entity.GetStudentMoney(s.StudentID),
                    MantainMoney = Dormitory_Entity.GetMantainMoney(s.StudentID),
                    BaoxianguiMoney = Dormitory_Entity.BaoxianguiStu(s.StudentID),
                    SumMoney = Dormitory_Entity.GetStudentMoney(s.StudentID) - Dormitory_Entity.GetMantainMoney(s.StudentID) - Dormitory_Entity.BaoxianguiStu(s.StudentID)
                };
                stumoneylist.Add(data);
            });

            var jsondata = new { code = 0, data = stumoneylist, count = stumoneylist.Count };

            return Json(jsondata, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 维修数据详情视图
        /// </summary>
        /// <returns></returns>
        public ActionResult MantainDefileView(string id)
        {
            //去查询这个学生的没有支付的维修费用
            List<DormitoryDeposit> dormitoryDeposits = Dormitory_Entity.StudentDormitoryDepsitData(id, false);

                var liststu = dormitoryDeposits.Select(s => new StuSusheData()
                {
                    DeaID = s.ID,
                    DeaMaintain = s.Maintain,//维修日期
                    DeaDorName = Dormitory_Entity.DormInformation_Entity.GetEntity(s.DormId).DormInfoName,//房间编号
                    DeaGoodPrice = s.GoodPrice,//维修金额
                    DeaNameofarticle = Dormitory_Entity.DormitoryMaintenance_Entity.GetEntity(s.MaintainGood).Nameofarticle,//物品名称
                    DeastuName = Dormitory_Entity.StudentInformation_Entity.GetEntity(s.StuNumber).Name
                }).ToList();

                decimal SumMantanMoney = 0;//维修总金额

                liststu.ForEach(s =>
                {
                    SumMantanMoney += s.DeaGoodPrice;
                });

                //获取这个学生所缴宿舍押金
                decimal GetStuMoney = Dormitory_Entity.GetStudentMoney(id);
                //宿舍保险费费用
                decimal baoxiangMoenty = Dormitory_Entity.BaoxianguiStu(id);
                //获取应退费用
                decimal GetTuiMoney = GetStuMoney - SumMantanMoney - baoxiangMoenty;

                ListStusheData datas = new ListStusheData() { listdata = liststu, SumMantanMoney = SumMantanMoney, GetTuiMoney = GetTuiMoney };
                ViewBag.listdata = datas;
                return View();
           
        }
        /// <summary>
        /// 保险柜费用数据添加方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddSafeFuntion()
        {

            int classid = Convert.ToInt32(Request.Form["classid"]);//班级
            int goodsid = Convert.ToInt32(Request.Form["weixiugood"]);//物品编号
            DateTime dateTime = Convert.ToDateTime(Request.Form["mantinDate"]);//获取日期

            AjaxResult result = new AjaxResult();
            //获取这个班的所有学生
            string sqlstr = "select * from ScheduleForTrainees where  CurrentClass=1 and ID_ClassName='" + classid + "'";
            List<ScheduleForTrainees> stulist = Dormitory_Entity.GetListBySql<ScheduleForTrainees>(sqlstr);

            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息

            List<DormitoryDeposit> Dorlist = new List<DormitoryDeposit>();
            StringBuilder sb = new StringBuilder();
            foreach (ScheduleForTrainees s in stulist)
            {
                //判断这个学生在哪个宿舍
                string sqlstr2 = @"select * from DormInformation where Id= (select DormId from Accdationinformation where Studentnumber='" + s.StudentID + "' and StayDate >= '" + dateTime + "' and (EndDate is null or EndDate<='" + dateTime + "'))";
                List<DormInformation> list2 = Dormitory_Entity.GetListBySql<DormInformation>(sqlstr2);
                if (list2.Count > 0)
                {
                    DormitoryDeposit dormitory = new DormitoryDeposit()
                    {
                        ID = Guid.NewGuid().ToSequentialGuid(),
                        Maintain = dateTime,
                        DormId = list2[0].ID,
                        StuNumber = s.StudentID,
                        MaintainGood = goodsid,
                        GoodPrice = Dormitory_Entity.Pricedormitoryarticles_Entity.GetEntity(goodsid).Reentry,
                        MaintainState = 1,
                        CreaDate = DateTime.Now,
                        EntryPersonnel = UserName.EmpNumber
                    };

                    //去数据库查看是否这个月的宿舍保险柜数据已经录入成功了
                    string sqlstr3 = @"select * from DormitoryDeposit where StuNumber='" + s.StudentID + "' and YEAR(Maintain)='" + dateTime.Year + "' and MONTH(Maintain)='" + dateTime.Month + "' and MaintainGood='" + goodsid + "'";
                    int count = Dormitory_Entity.GetListBySql<DormitoryDeposit>(sqlstr3).Count;
                    if (count <= 0)
                    {
                        Dorlist.Add(dormitory);
                    }

                }
                else
                {
                    string stuname = Dormitory_Entity.StudentInformation_Entity.GetEntity(s.StudentID).Name;
                    sb.Append(stuname + "、");
                }
            }


            if (Dorlist.Count == stulist.Count)
            {
                result.Success = Dormitory_Entity.AddData(Dorlist);
                result.Msg = result.Success == false ? "操作失败" : "操作成功";
            }
            else if (sb.Length > 0)
            {
                result.Success = false;
                result.Msg = sb.ToString() + "没有宿舍信息！,请核对学生数据在进行数据录入！";
            }
            else if (Dorlist.Count == 0)
            {
                result.Success = false;
                result.Msg = "这个班的这个月的保险费已录入了，请操作其他班级的！";
            }



            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddSafeView()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息

            string empname = Dormitory_Entity.EmployeesInfo_Entity.GetEntity(UserName.EmpNumber).EmpName;

            List<SelectListItem> classlist = Dormitory_Entity.Headmaster_Entity.ListTeamleaderdistributionView().Where(h => h.HeadmasterName == empname).Select(h => new SelectListItem() { Text = h.ClassName, Value = h.ClassID.ToString() }).ToList();  //获取班主任所带的班级

            //获取所带班级
            ViewBag.tung = classlist;
            //获取维修物品 
            ViewBag.goodname = Dormitory_Entity.DormitoryMaintenance_Entity.GetList().Where(s => s.Dateofregistration == true).Select(s => new SelectListItem() { Value = s.ID.ToString(), Text = s.Nameofarticle }).ToList();
            return View();
        }

        /// <summary>
        /// 学生住宿详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult StuNameDefileView(string id)
        {
            ViewBag.StuNumber = id;
            return View();
        }

        public ActionResult StuNameDefileFuntion(string id)
        {
            var data = Dormitory_Entity.Stulist(id).Select(s => new
            {
                stuName = Dormitory_Entity.StudentInformation_Entity.GetEntity(s.Studentnumber).Name,
                Studentnumber = s.Studentnumber,
                BedId = s.BedId,
                DorName = Dormitory_Entity.DormInformation_Entity.GetEntity(s.DormId).DormInfoName,//房间编号            
                StarTime = s.StayDate,
                EndTime = s.EndDate
            }).ToList();

            var jsondata = new { count = data.Count, data = data, code = 0 };

            return Json(jsondata, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region 财务操作
        public ActionResult FinanceIndex()
        {
            //获取两边校区的地址
            ViewBag.Addree = Dormitory_Entity.Tung_Entity.GetList().Where(s => s.IsDel == false).Select(s => new SelectListItem() { Text = s.TungName, Value = s.Id.ToString() }).ToList();
            return View();
        }

        public ActionResult StudentinfomatuonView()
        {

            return View();
        }



        [HttpGet]
        public ActionResult GetStudentData()
        {
            string stuname = Request.QueryString["stuname"];
            if (!string.IsNullOrEmpty(stuname))
            {
                string sqlstr = @"select si.Name as 'Stuname',si.StudentNumber,st.ClassID,g.GrandName,ID_ClassName from ScheduleForTrainees as st 
                              inner join StudentInformation as si on st.StudentID = si.StudentNumber
                              inner join ClassSchedule as cs on st.ID_ClassName = cs.id
                              inner join Grand as g on cs.grade_Id = g.Id                             
                              where st.CurrentClass = 1 and si.Name like '%" + stuname + "%' ";

                List<StudentInfoData> stulist = Dormitory_Entity.GetListBySql<StudentInfoData>(sqlstr);

                var data = stulist.Select(s => new
                {
                    GrandName = s.GrandName,
                    Stuname = s.Stuname,
                    StudentNumber = s.StudentNumber,
                    ClassID = s.ClassID,
                    EmpName = Dormitory_Entity.ClassTeacherName(s.ID_ClassName)
                });

                var jsondata = new { code = 0, count = stulist.Count, data = data };

                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<object> list = new List<object>();
                var data = new { GrandName = "请查询", Stuname = "请查询", StudentNumber = "请查询", ClassID = "请查询", EmpName = "请查询" };
                list.Add(data);
                var jsondata = new { code = 0, count = list.Count, data = list };

                return Json(jsondata, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 学生寝室维修数据
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentDorManinData()
        {
            string id = Request.Form["stunumber"];
            string startime = Request.Form["star"];
            string endtime = Request.Form["end"];


            //去查询这个学生的没有支付的维修费用
            var liststu = Dormitory_Entity.StudentDormitoryDepsitData(id, true).Select(s => new StuSusheData()
            {
                DeaID = s.ID,
                DeaMaintain = s.Maintain,//维修日期
                DeaDorName = Dormitory_Entity.DormInformation_Entity.GetEntity(s.DormId).DormInfoName,//房间编号
                DeaGoodPrice = s.GoodPrice,//维修金额
                DeaNameofarticle = Dormitory_Entity.DormitoryMaintenance_Entity.GetEntity(s.MaintainGood).Nameofarticle,//物品名称
                DeastuName = Dormitory_Entity.StudentInformation_Entity.GetEntity(s.StuNumber).Name,
                Isdelete = s.MaintainState == 1 ? "未支付" : "已支付",
            }).ToList();

            if (!string.IsNullOrEmpty(startime))
            {
                DateTime d1 = Convert.ToDateTime(startime);
                liststu = liststu.Where(l => l.DeaMaintain >= d1).ToList();
            }

            if (!string.IsNullOrEmpty(endtime))
            {
                DateTime d2 = Convert.ToDateTime(endtime);
                liststu = liststu.Where(l => l.DeaMaintain <= d2).ToList();
            }
            decimal SumMantanMoney = 0;//维修总金额



            liststu.ForEach(s =>
            {
                if (s.Isdelete == "未支付")
                {
                    SumMantanMoney += s.DeaGoodPrice;
                }

            });

            //获取这个学生所缴宿舍押金
            decimal GetStuMoney = Dormitory_Entity.GetStudentMoney(id);

            //获取应退费用
            decimal GetTuiMoney = GetStuMoney - SumMantanMoney;

            ListStusheData datas = new ListStusheData() { listdata = liststu, SumMantanMoney = SumMantanMoney, GetTuiMoney = GetTuiMoney };

            return Json(datas, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult addressMoney()
        {
            int addressid = Convert.ToInt32(Request.Form["address"]);//宿舍地址编号

            DateTime date = Convert.ToDateTime(Request.Form["date"]);//月份

            decimal Money = Dormitory_Entity.MonMantainMoney(addressid, date.Year, date.Month);

            return Json(Money, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 班级结算
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ClassJiesuan()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息

            AjaxResult result = new AjaxResult();

            int classid = Convert.ToInt32(Request.Form["classid"]);

            string sqlstr = @"select * from DormitoryDeposit where StuNumber in(
                              select StudentID from ScheduleForTrainees where ID_ClassName=" + classid + " and CurrentClass =1) and MaintainState=1";

            //获取属于这个班的所有学生的维修数据
            List<DormitoryDeposit> list = Dormitory_Entity.GetListBySql<DormitoryDeposit>(sqlstr);

            List<DormitoryDeposit> updatelist = new List<DormitoryDeposit>();

            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].MaintainState = 2;
                    list[i].SettlementStaff = UserName.EmpNumber;

                    updatelist.Add(list[i]);
                }

                result.Success = Dormitory_Entity.UpdateData(updatelist);
                result.Msg = result.Success == false ? "操作失误！" : "操作成功！";
            }
            else
            {
                result.Success = false;
                result.Msg = "这个班级没有维修数据!";
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}