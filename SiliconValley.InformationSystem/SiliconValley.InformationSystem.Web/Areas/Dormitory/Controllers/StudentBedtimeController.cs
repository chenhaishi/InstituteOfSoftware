using SiliconValley.InformationSystem.Business;
using SiliconValley.InformationSystem.Business.Base_SysManage;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.DormitoryBusiness;
using SiliconValley.InformationSystem.Business.DormitoryMantainBusiness;
using SiliconValley.InformationSystem.Business.StudentBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Dormitory.Controllers
{
    [CheckLogin]
    public class StudentBedtimeController : Controller
    {
        public HeadmasterBusiness Headmaster_Entity = new HeadmasterBusiness();
        private ProStudentInformationViewBusiness dbproStudentInformationViewBusiness = new ProStudentInformationViewBusiness ();
        private ProClassScheduleViewBusiness dbproClassScheduleViewBusiness = new ProClassScheduleViewBusiness ();
        private DormInformationBusiness dbdorm = new DormInformationBusiness ();
        private TungFloorBusiness dbtungfloor = new TungFloorBusiness ();
        private RoomdeWithPageXmlHelp dbxml = new RoomdeWithPageXmlHelp ();
        private dbacc_dbroomnumber dbaccroomnumber = new dbacc_dbroomnumber ();
        private DormitoryLeaderBusiness dbleader = new DormitoryLeaderBusiness ();
        private ProStudentInformationBusiness dbprostudent = new ProStudentInformationBusiness();
        private dbacc_dbben_dbroomnumber_dbdorm dbacc_Dbben_Dbroomnumber_Dbdorm = new dbacc_dbben_dbroomnumber_dbdorm ();
        private AccdationinformationBusiness dbacc =new AccdationinformationBusiness ();
        private ProDormInfoViewBusiness dbproDormInfoViewBusiness = new ProDormInfoViewBusiness ();
        private dbprosutdent_dbproheadmaster dbprosutdent_Dbproheadmaster = new dbprosutdent_dbproheadmaster ();
        private ChangeDorStudent ChangeDorStudent_Entity = new ChangeDorStudent();
        private StudentInformationBusiness StudentInfo_Entity = new StudentInformationBusiness();
        private PricedormitoryarticlesManeger PriceManger = new PricedormitoryarticlesManeger();
        BaseBusiness<DormitoryDeposit> Dormitory_Entity = new BaseBusiness<DormitoryDeposit>();
        //private DormitoryDepositManeger Dormitory_Entity = new DormitoryDepositManeger();
        public BaseBusiness<EmployeesInfo> EmployeesInfo_Entity = new BaseBusiness<EmployeesInfo>();

        // GET: /Dormitory/StudentBedtime/EndFunction
        public ActionResult StudentBedtimeIndex()
        {
            return View();
        }

        /// <summary>
        /// 加载#table00
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="classno"></param>
        /// <returns></returns>
        public ActionResult table00(int page, int limit, string classno)
        {
            
            dbproClassScheduleViewBusiness = new ProClassScheduleViewBusiness();
            dbprosutdent_Dbproheadmaster = new dbprosutdent_dbproheadmaster();
            dbacc = new AccdationinformationBusiness();
            var list0 = dbacc.GetAccdationinformations();
            var param0 = new List<ClassSchedule>();
            foreach (var item in list0)
            {
                param0.Add(dbprosutdent_Dbproheadmaster.GetClassScheduleByStudentNumber(item.Studentnumber));
            }
            if (!string.IsNullOrEmpty(classno))
            {
                param0 = param0.Where(a => a.ClassNumber == classno).ToList();
            }
            var query0 = dbproClassScheduleViewBusiness.Conversion(param0);
            for (int i = 0; i < query0.Count; i++)  //外循环是循环的次数
            {
                for (int j = query0.Count - 1; j > i; j--)  //内循环是 外循环一次比较的次数
                {

                    if (query0[i].ClassNumber == query0[j].ClassNumber)
                    {
                        query0.RemoveAt(j);
                    }
                }
            }
            
            var data = query0.OrderByDescending(a => a.ClassNumber).Skip((page - 1) * limit).Take(limit).ToList();
            var returnObj = new
            {
                code = 0,
                msg = "",
                count = query0.Count(),
                data = data
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 加载#table01
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="tungid"></param>
        /// <param name="floorid"></param>
        /// <param name="dormname"></param>
        /// <returns></returns>
        public ActionResult table01(int page, int limit, int tungid,int floorid,string dormname) {

            dbdorm = new DormInformationBusiness();
            dbtungfloor = new TungFloorBusiness();
            dbxml = new RoomdeWithPageXmlHelp();
            dbacc = new AccdationinformationBusiness();
            var obj0= dbtungfloor.GetTungFloorByTungIDAndFloorID(tungid, floorid);
            int roomtype = dbxml.GetRoomType(Entity.ViewEntity.RoomTypeEnum.RoomType.StudentRoom);

            var list0= dbdorm.GetDormsByTungFloorIDing(obj0.Id).Where(a=>a.RoomStayTypeId== roomtype).ToList();
            if (!string.IsNullOrEmpty(dormname))
            {
                list0 = list0.Where(a => a.DormInfoName == dormname).ToList();
            }
            for (int i = list0.Count-1; i >=0; i--)
            {
                var querylist0 = dbacc.GetAccdationinformationByDormId(list0[i].ID);
                if (querylist0.Count<1)
                {
                    list0.Remove(list0[i]);
                }
            }
            var resutl0 = list0.Select(a => new
            {
                ID = a.ID,
                SexType = a.SexType,
                DormInfoName=a.DormInfoName
            }).ToList();

            var data = resutl0.OrderByDescending(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();

            var returnObj = new
            {
                code = 0,
                msg = "",
                count = resutl0.Count(),
                data = data
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 加载#table02
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="tungid"></param>
        /// <param name="floorid"></param>
        /// <param name="name1"></param>
        /// <returns></returns>
        public ActionResult table02(int page, int limit, int tungid, int floorid, string name1) {

            dbproStudentInformationViewBusiness = new ProStudentInformationViewBusiness();
            dbdorm = new DormInformationBusiness();
            dbtungfloor = new TungFloorBusiness();
            dbxml = new RoomdeWithPageXmlHelp();
            var obj0 = dbtungfloor.GetTungFloorByTungIDAndFloorID(tungid, floorid);
            int roomtype = dbxml.GetRoomType(Entity.ViewEntity.RoomTypeEnum.RoomType.StudentRoom);
            var list0 = dbdorm.GetDormsByTungFloorIDing(obj0.Id).Where(a => a.RoomStayTypeId == roomtype).ToList();
            var resutl0=dbproStudentInformationViewBusiness.GetProBedtimeStudentsViews(list0,name1);
            var data = resutl0.OrderByDescending(a => a.StudentNmber).Skip((page - 1) * limit).Take(limit).ToList();
            var returnObj = new
            {
                code = 0,
                msg = "",
                count = resutl0.Count(),
                data = data
            };
            return Json(returnObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 楼层全部学生数据
        /// </summary>
        /// <param name="tungid"></param>
        /// <param name="floorid"></param>
        /// <returns></returns>
        public ActionResult loadall(int tungid, int floorid) {
            dbproStudentInformationViewBusiness = new ProStudentInformationViewBusiness();
            dbdorm = new DormInformationBusiness();
            dbtungfloor = new TungFloorBusiness();
            dbxml = new RoomdeWithPageXmlHelp();
            var obj0 = dbtungfloor.GetTungFloorByTungIDAndFloorID(tungid, floorid);
            int roomtype = dbxml.GetRoomType(Entity.ViewEntity.RoomTypeEnum.RoomType.StudentRoom);
            var list0 = dbdorm.GetDormsByTungFloorIDing(obj0.Id).Where(a => a.RoomStayTypeId == roomtype).ToList();
            var resutl0 = dbproStudentInformationViewBusiness.GetProBedtimeStudentsViews(list0, string.Empty);
            return Json(resutl0,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 寝室查询
        /// </summary>
        /// <param name="sex"></param>
        /// <param name="TungID"></param>
        /// <param name="FloorID"></param>
        /// <returns></returns>
        public ActionResult ChoiceInfo(bool sex, int TungID, int FloorID)
        {
            dbdorm = new DormInformationBusiness();
            dbaccroomnumber = new dbacc_dbroomnumber();
            dbxml = new RoomdeWithPageXmlHelp();
            dbtungfloor = new TungFloorBusiness();
            dbproDormInfoViewBusiness = new ProDormInfoViewBusiness();
            AjaxResult ajaxResult = new AjaxResult();

            try
            {
                TungFloor querytungfloor = dbtungfloor.GetTungFloorByTungIDAndFloorID(TungID, FloorID);
              
                var data = dbdorm.GetDormsByTungFloorIDing(querytungfloor.Id);
                //默认男寝
                int maleid = 1;
                //男寝
                if (sex)
                {
                    //男寝数据
                    data = data.Where(a => a.SexType == maleid).ToList();

                }
                else
                {
                    maleid = dbxml.Getmale(RoomTypeEnum.SexType.Female);

                    //女寝寝数据
                    data = data.Where(a => a.SexType == maleid).ToList();
                }


                var xmlroomtype = dbxml.GetRoomType(RoomTypeEnum.RoomType.StudentRoom);
                //学生宿舍
                data = data.Where(a => a.RoomStayTypeId == xmlroomtype).ToList();

                var dormInfoViews = dbproDormInfoViewBusiness.dormInfoViewsByStudent(data);
                ajaxResult.Data = dormInfoViews;
                ajaxResult.Success = true;
                BusHelper.WriteSysLog("查询学生寝室数据Dormitory/DormitoryInfo/ChoiceInfo", Entity.Base_SysManage.EnumType.LogType.查询数据success);
            }
            catch (Exception ex)
            {

                ajaxResult.Msg = "请及时的联系信息部";
                ajaxResult.Success = false;
            }


            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetDormstu()
        {
            int dormid = Convert.ToInt32(Request.Form["dormid"]);
            string sql = "select * from Accdationinformation where DormId = " + dormid + " and EndDate is null";
            List<Accdationinformation> list = dbacc.GetListBySql<Accdationinformation>(sql);
            List<SelectListItem> studentlist = new List<SelectListItem>();

            list.ForEach(l =>
            {
                StudentInformation student = StudentInfo_Entity.GetEntity(l.Studentnumber);
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
        /// 返回房间床位数
        /// </summary>
        /// <param name="DorminfoID"></param>
        /// <returns></returns>
        public ActionResult BedInfo(int DorminfoID, string datatype)
        {
            AjaxResult ajaxResult = new AjaxResult();

            dbacc_Dbben_Dbroomnumber_Dbdorm = new dbacc_dbben_dbroomnumber_dbdorm();
            try
            {
                var querydata = dbacc_Dbben_Dbroomnumber_Dbdorm.GetBensByDorminfoID(DorminfoID);
                BusHelper.WriteSysLog("位于Dormitory/DormitoryInfo/BedInfo", Entity.Base_SysManage.EnumType.LogType.查询数据success);
                ajaxResult.Data = querydata;
                ajaxResult.Success = true;
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message + "位于Dormitory/DormitoryInfo/BedInfo", Entity.Base_SysManage.EnumType.LogType.查询数据error);
                ajaxResult.Data = "";
                ajaxResult.Success = false;
            }

            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加居住信息
        /// </summary>
        /// <param name="BedId"></param>
        /// <param name="DormId"></param>
        /// <param name="Studentnumber"></param>
        /// <returns></returns>
        public ActionResult ArrangeDorm(int BedId, int DormId, string resultdata)
        {
            AjaxResult ajaxResult = new AjaxResult();
            dbacc = new AccdationinformationBusiness();

            //这个房间的床位号是否有人居住，如果有人居住 需要删除这个原来的居住人员，之后才能进行添加
           var obj=  dbacc.GetAccdationinformationByDormId(DormId).Where(a => a.BedId == BedId).FirstOrDefault();
            if (obj!=null)
            {
                obj.IsDel = true;
                obj.EndDate = DateTime.Now;
                dbacc.Update(obj);
                //如果是寝室长。将职位去除掉
                dbleader = new DormitoryLeaderBusiness();
                var obj1 = dbleader.GetLeaderByStudentNumber(obj.Studentnumber);
                if (obj1 != null)
                {
                    dbleader.Cancellation(obj1);
                }
            }
            var obj0 = dbacc.GetAccdationByStudentNumber(resultdata);
            if (obj0 != null)
            {
                dbacc = new AccdationinformationBusiness();
                //首先将改为居住信息改为false
                obj0.IsDel = true;
                obj0.EndDate = DateTime.Now;
                dbacc.Update(obj0);
                dbleader = new DormitoryLeaderBusiness();
                var obj1 = dbleader.GetLeaderByStudentNumber(resultdata);
                if (obj1 != null)
                {
                    dbleader.Cancellation(obj1);
                }
                //如果是寝室长。将职位去除掉
            }
            dbacc = new AccdationinformationBusiness();
            Accdationinformation accdationinformation = new Accdationinformation();
            accdationinformation.CreationTime = DateTime.Now;
            accdationinformation.IsDel = false;
            accdationinformation.Remark = string.Empty;
            accdationinformation.StayDate = DateTime.Now;
            accdationinformation.BedId = BedId;
            accdationinformation.DormId = DormId;
            accdationinformation.Studentnumber = resultdata;
            ajaxResult.Success = dbacc.AddAcc(accdationinformation);


            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 学员信息显示页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentChangDorView()
        {
            Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
            //获取登录人信息
            string empname = EmployeesInfo_Entity.GetEntity(UserName.EmpNumber).EmpName;
            //获取班主任所带的班级
            List<SelectListItem> classlist = Headmaster_Entity.ListTeamleaderdistributionView().Where(h => h.HeadmasterName == empname).Select(h => new SelectListItem() { Text = h.ClassName, Value = h.ClassID.ToString() }).ToList();

            classlist.Add(new SelectListItem() { Text = "--请选择--", Value = "0", Selected = true });
            ViewBag.classlist = classlist;
            return View();
        }

        [HttpGet]
        public ActionResult StudentChangDorData()
        {
            string StuName= Request.QueryString["classNumber"];
            string sqlstr = "select * from StudentInformation where Name like '%"+StuName+"%'";

            List<StudentInformation> stulist= ChangeDorStudent_Entity.GetListBySql<StudentInformation>(sqlstr);

            //学生姓名，所在班级，班主任姓名，所在寝室，所在床位
            List<DorChangeStudentData> dorchanglist = new List<DorChangeStudentData>();

            foreach (StudentInformation item in stulist)
            {
                DorChangeStudentData studentData = new DorChangeStudentData();
                studentData.StuName = item.Name;
                studentData.StuNumber = item.StudentNumber;
                ScheduleForTrainees classname = ChangeDorStudent_Entity.GetClassName(item.StudentNumber);
                if (classname.ClassID!=null)
                {
                    studentData.ClassName = classname.ClassID;
                }
                else
                {
                    studentData.ClassName = "无";
                }
                studentData.ClassName = ChangeDorStudent_Entity.GetClassName(item.StudentNumber).ClassID;
                studentData.TeacherName = ChangeDorStudent_Entity.Headmaster_Entity.GetEmployessByStuid(item.StudentNumber).EmpName;

                DorChuang chuang = ChangeDorStudent_Entity.GetDorName(item.StudentNumber);
                if (chuang.DorNumber!=null)
                {
                    studentData.DorName = chuang.DorNumber;
                    studentData.ChuangNumber = chuang.ChuangNumber;
                }
                else
                {
                    studentData.DorName = "无";
                    studentData.ChuangNumber =-1;
                }

                dorchanglist.Add(studentData);
            }

            var jsondata = new {data= dorchanglist ,code=0,count= dorchanglist.Count };

            return Json(jsondata,JsonRequestBehavior.AllowGet) ;

        }

        /// <summary>
        /// 学员调寝页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangDorView(string id)
        {
            //获取宿舍楼地址
            ViewBag.tung = ChangeDorStudent_Entity.Tung_Entity.GetList().Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.TungName }).ToList();
            ViewBag.StuNumber = id;
            return View();
        }

        /// <summary>
        /// 调寝
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangDorFunction(string [] stuNumber)
        {
            AjaxResult result = new AjaxResult() { Success=false,Msg="操作失败"};

            int DorId = Convert.ToInt32(Request.Form["dorname1"]);//应去宿舍
            int OldDorId = Convert.ToInt32(Request.Form["dorname"]);//原宿舍
            
            DateTime endtime = Convert.ToDateTime(Request.Form["endtime"]);

            for (int i = 0; i < stuNumber.Length; i++)
            {
                //扣除保险柜押金   学号判断当前的入住宿舍，如果在老校区，需要调去新校区扣除押金
                //没调寝前住的宿舍，校区
                Pricedormitoryarticles Price_List = PriceManger.GetList().Where(s => s.Nameofarticle.Contains("保险柜每月扣费")).FirstOrDefault();
                if(Price_List != null) { 
                string X_Dormidsql = "select * from Accdationinformation where studentnumber = " + stuNumber[i] + " and Enddate is null";
                Accdationinformation accinfo = dbacc.GetListBySql<Accdationinformation>(X_Dormidsql).FirstOrDefault();
                string X_TungFidsql = "select * from DormInformation where  Id = " + accinfo.DormId + "";
                DormInformation dorminfo = dbdorm.GetListBySql<DormInformation>(X_TungFidsql).FirstOrDefault();
                string X_Tungidsql = "select * from TungFloor where Id=" + dorminfo.TungFloorId + "";
                TungFloor tunginfo = dbtungfloor.GetListBySql<TungFloor>(X_Tungidsql).FirstOrDefault();

                //需要调去的宿舍，校区
                string X_TungFidsql1 = "select * from DormInformation where  Id = " + DorId + "";
                DormInformation dorminfo1 = dbdorm.GetListBySql<DormInformation>(X_TungFidsql1).FirstOrDefault();
                string X_Tungidsql1 = "select * from TungFloor where Id=" + dorminfo1.TungFloorId + "";
                TungFloor tunginfo1 = dbtungfloor.GetListBySql<TungFloor>(X_Tungidsql1).FirstOrDefault();
                //如果调寝前住的宿舍属于老校区，调寝后住的宿舍属于新校区，扣除宿舍保险柜的押金 
                if (tunginfo.TungId == 27 && tunginfo1.TungId == 34)
            {
                string yuandate = "select top 1 * from Accdationinformation where studentnumber = " + stuNumber[i] + " order by staydate asc";
                Accdationinformation accinfo1 = dbacc.GetListBySql<Accdationinformation>(yuandate).FirstOrDefault();
                var months = ((endtime.Year - accinfo1.StayDate.Year) * 12) + DateTime.Now.Month - accinfo1.StayDate.Month;
                
                Base_UserModel UserName = Base_UserBusiness.GetCurrentUser();//获取登录人信息
                DormitoryDeposit dormitory = new DormitoryDeposit()
                {
                    ID = Guid.NewGuid().ToSequentialGuid(),
                    Maintain = endtime,
                    DormId = DorId,
                    StuNumber = stuNumber[i],
                    MaintainGood = Price_List.ID,
                    GoodPrice = months * 10,
                    MaintainState = 1,
                    CreaDate = DateTime.Now,
                    RepairContent = "每月扣除的保险柜金额",
                    EntryPersonnel = UserName.EmpNumber
                };
                Dormitory_Entity.Insert(dormitory);
            }
                }
                //记录1-8号床位为空并已经安排的人数
                int count = 0;

                for (int c = 1; c <= 8; c++)
                {
                  string searchChuanghao = "select * from Accdationinformation where DormId="+DorId+" and BedId="+c+" and EndDate  is null";
                  List<Accdationinformation> ChuangList = ChangeDorStudent_Entity.GetListBySql<Accdationinformation>(searchChuanghao);
                  if (ChuangList.Count <= 0)
                  {
                      string sqlstr = @"select * from Accdationinformation where Studentnumber='" + stuNumber[i] + "' and EndDate is null";

                      List<Accdationinformation> list = ChangeDorStudent_Entity.GetListBySql<Accdationinformation>(sqlstr);

                      List<Accdationinformation> Update = new List<Accdationinformation>();

                      foreach (Accdationinformation item in list)
                      {
                          item.EndDate = endtime;
                          item.IsDel = true;

                          Update.Add(item);
                      }
                      if (Update.Count > 0)
                        {
                               bool Isretult = ChangeDorStudent_Entity.UpdateData(Update);
                                       
                               Accdationinformation data = new Accdationinformation();
                               data.BedId = c;
                               data.CreationTime = DateTime.Now;
                               data.DormId = DorId;
                               data.IsDel = false;
                               data.StayDate = DateTime.Now;
                               data.Studentnumber = stuNumber[i];
                               data.Remark = string.Empty;
                               result.Success = ChangeDorStudent_Entity.AddData(data);
                               result.Msg = result.Success == false ? "操作失败！" : "调寝成功！";
                            count += 1;
                        }
                        break;
                  }
                }
                if (count<stuNumber.Length) {
                    string checkDorm = "select * from Accdationinformation where studentnumber = " + stuNumber[i] + " and Enddate is null";
                    Accdationinformation checklist = ChangeDorStudent_Entity.GetListBySql<Accdationinformation>(checkDorm).FirstOrDefault();
                    if (checklist == null) {
                         int n = ChangeDorStudent_Entity.GetList().Where(a=>a.DormId==DorId).Max(a=>a.BedId);
                        string sqlstr = @"select * from Accdationinformation where Studentnumber='" + stuNumber[i] + "' and EndDate is null";

                        List<Accdationinformation> list = ChangeDorStudent_Entity.GetListBySql<Accdationinformation>(sqlstr);

                        List<Accdationinformation> Update = new List<Accdationinformation>();

                        foreach (Accdationinformation item in list)
                        {
                            item.EndDate = endtime;
                            item.IsDel = true;

                            Update.Add(item);
                        }
                        if (Update.Count > 0)
                        {
                            bool Isretult = ChangeDorStudent_Entity.UpdateData(Update);

                            Accdationinformation data = new Accdationinformation();
                            data.BedId = n+1;
                            data.CreationTime = DateTime.Now;
                            data.DormId = DorId;
                            data.IsDel = false;
                            data.StayDate = DateTime.Now;
                            data.Studentnumber = stuNumber[i];
                            data.Remark = string.Empty;
                            result.Success = ChangeDorStudent_Entity.AddData(data);
                            result.Msg = result.Success == false ? "操作失败！" : "调寝成功！";
                            
                        }
                    }
                    
                }

            }

            for (int k = 1; k <= 8; k++)
            {
                string Bedisnotsql = "select * from Accdationinformation where DormId = "+OldDorId+" and BedId ="+k+" and Enddate is null";
                List<Accdationinformation> bedlist = ChangeDorStudent_Entity.GetListBySql<Accdationinformation>(Bedisnotsql);
                if (bedlist.Count==0) {
                    int count = 0;
                     string maxlistsql = "select * from Accdationinformation where DormId = "+OldDorId+" and BedId >8";
                     List<Accdationinformation> maxlist = ChangeDorStudent_Entity.GetListBySql<Accdationinformation>(maxlistsql);
                    if (maxlist.Count != 0) {
                            maxlist[count].BedId = k;
                            bool isFlush = ChangeDorStudent_Entity.UpdateData(maxlist[count]);
                            if (isFlush) {
                                count += 1;
                            }
                    }
                    
                }
            }
            return Json(result,JsonRequestBehavior.AllowGet);

            
        }
        

        [HttpPost]
        public ActionResult EndFunction()
        {
            AjaxResult result = new AjaxResult() { Success = false, Msg = "操作失败" };

            string stuNumber = Request.Form["StuNumber"];

            string sqlstr = @"select * from Accdationinformation where Studentnumber='" + stuNumber + "' and EndDate is null";

            List<Accdationinformation> list = ChangeDorStudent_Entity.GetListBySql<Accdationinformation>(sqlstr);

            List<Accdationinformation> Update = new List<Accdationinformation>();
            foreach (Accdationinformation item in list)
            {
                item.EndDate = DateTime.Now;
                item.IsDel = true;

                Update.Add(item);
            }

            bool Isretult = ChangeDorStudent_Entity.UpdateData(Update);

            if (Isretult)
            {
                result.Msg = "操作成功！";
                result.Success = Isretult;
            }

            return Json(result,JsonRequestBehavior.AllowGet);
        }
    }
}