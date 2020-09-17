using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.DormitoryBusiness;
using SiliconValley.InformationSystem.Entity.Entity;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiliconValley.InformationSystem.Web.Areas.Dormitory.Controllers
{
    [CheckLogin]
    /// <summary>
    /// 房间基础信息
    /// </summary>
    public class RoomBasisController : Controller
    {
        private RoomStayTypeBusiness dbroomtype;
        private RoomStayNumberBusiness dbroomnumber;
        private DormInformationBusiness dbdorm;
        private RoomdeWithPageXmlHelp dbroomxml;
        private TungBusiness dbtung;
        private AccdationinformationBusiness dbacc;
        private TungFloorBusiness dbtungfloor;
        private StaffAccdationBusiness dbstaffacc;
        // GET: Dormitory/RoomBasis
        public ActionResult RoomBasisIndex()
        {
            return View();
        }
        /// <summary>
        /// 添加房间
        /// </summary>
        /// <param name="Direction"></param>
        /// <param name="Floorid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddRoomPage()
        {
            dbroomtype = new RoomStayTypeBusiness();
            dbroomnumber = new RoomStayNumberBusiness();
            var data = dbroomtype.GetRoomStayTypes();
            var SelectListItemlist = data.Select(a => new SelectListItem()
            {
                Text = a.RoomStayTypeName,
                Value = a.Id.ToString()
            });
            ViewBag.SelectListItemlist = SelectListItemlist;

            var RoomNumber = dbroomnumber.GetRoomStayNumbers().Select(a => new SelectListItem()
            {
                Text = a.StayNumber + "人制",
                Value = a.Id.ToString()
            });
            ViewBag.RoomNumber = RoomNumber;
            return View();
        }
        [HttpPost]
        /// <summary>
        /// post 添加
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public ActionResult AddRoomPage(DormInformation jsonStr, int FloorId, int TungId)
        {
            AjaxResult result = new AjaxResult();
            dbdorm = new DormInformationBusiness();
            dbroomxml = new RoomdeWithPageXmlHelp();
            dbtung = new TungBusiness();
            dbtungfloor = new TungFloorBusiness();
            try
            {
                TungFloor querytungfloor = dbtungfloor.GetTungFloorByTungIDAndFloorID(TungId, FloorId);

                //根据这个楼层来说房间号是不能重复的。
                List<DormInformation> querydormlist = dbdorm.GetDormsByTungFloorID(querytungfloor.Id).ToList();
                DormInformation querydorm = querydormlist.Where(a => a.DormInfoName == jsonStr.DormInfoName).FirstOrDefault();
                if (querydormlist.Count != 0 && querydorm != null)
                {
                    result.Msg = "不可添加重复房间号";
                    result.Success = false;
                }
                else
                {
                    jsonStr.TungFloorId = querytungfloor.Id;
                    //仓库
                    if (jsonStr.RoomStayTypeId == dbroomxml.GetRoomType(RoomTypeEnum.RoomType.Warehouse))
                    {
                        jsonStr.RoomStayNumberId = 0;
                    }
                    if (jsonStr.RoomStayTypeId == dbroomxml.GetRoomType(RoomTypeEnum.RoomType.StudentRoom) || jsonStr.RoomStayTypeId == dbroomxml.GetRoomType(RoomTypeEnum.RoomType.StaffRoom))
                    {

                    }
                    else
                    {
                        jsonStr.SexType = 0;
                    }
                    jsonStr.CreationTime = DateTime.Now;

                    dbdorm.Insert(jsonStr);
                    BusHelper.WriteSysLog("添加数据位置于Dormitory/DormitoryInfo/AddTungPage", Entity.Base_SysManage.EnumType.LogType.添加数据);
                    result.Msg = "添加成功";
                    result.Success = true;


                }

            }
            catch (Exception ex)
            {

                result.Msg = "请及时联系信息部，为你即时解决问题。";
                result.Success = false;
            }




            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 禁用该房间
        /// </summary>
        /// <param name="param0"></param>
        /// <returns></returns>
        public ActionResult DelRoom(int param0, string param1)
        {
            AjaxResult ajaxResult = new AjaxResult();
            dbacc = new AccdationinformationBusiness();
            dbdorm = new DormInformationBusiness();
            var entity0 = dbdorm.GetEntity(param0);
            bool Candelete = dbacc.HasSomeone(param0);
            if (!Candelete)
            {
                ajaxResult.Success = false;
                ajaxResult.Msg = "该房间还有人员居住，</br>请将居住人员撤离再执行！";
            }
            else
            {
                try
                {

                    entity0.IsDelete = true;
                    entity0.ProhibitRemark = param1;
                    dbdorm.Update(entity0);
                    ajaxResult.Success = true;
                    ajaxResult.Msg = "操作成功！";
                }
                catch (Exception)
                {

                    ajaxResult.Success = false;
                    ajaxResult.Msg = "请及时联系信息部成员！";
                }
            }
            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 是否允许修改
        /// </summary>
        /// <param name="param0"></param>
        /// <returns></returns>
        public ActionResult AllowEditors(int param0)
        {
            AjaxResult ajaxResult = new AjaxResult();
            dbacc = new AccdationinformationBusiness();
            bool fpxnb = dbacc.HasSomeone(param0);
            if (!fpxnb)
            {
                ajaxResult.Success = false;
                ajaxResult.Msg = "房间有人居住。</br>请将人员撤离再进行修改。";
            }
            else
            {
                ajaxResult.Success = true;
            }
            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 验证名字
        /// </summary>
        /// <param name="param0">tungid</param>
        /// <param name="param1">floorid</param>
        /// <param name="param2">dromid</param>
        /// <param name="param3">name</param>
        /// <returns></returns>
        public ActionResult VerifyName(int param0, int param1, int param2, string param3)
        {
            dbdorm = new DormInformationBusiness();
            dbtungfloor = new TungFloorBusiness();
            AjaxResult ajaxResult = new AjaxResult();
            try
            {
                var fpx = dbtungfloor.GetTungFloorByTungIDAndFloorID(param0, param1);

                var fpxnb = dbdorm.GetEntity(param2);
                if (fpxnb.DormInfoName != param3)
                {
                    if (dbdorm.DuplicateName(fpx.Id, param3))
                    {
                        ajaxResult.Success = true;
                    }
                }
                else
                {
                    ajaxResult.Success = false;
                }
            }
            catch (Exception)
            {

                ajaxResult.Success = false;
                ajaxResult.Msg = "false";
            }
            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="param0"></param>
        /// <returns></returns>
        public ActionResult HttpGetUpdRoom(int param0)
        { 
            dbdorm = new DormInformationBusiness();
            dbroomtype = new RoomStayTypeBusiness();
            dbroomnumber = new RoomStayNumberBusiness();
            var fpx = dbdorm.GetEntity(param0);

            var data = dbroomtype.GetRoomStayTypes();

            ViewBag.obj = Newtonsoft.Json.JsonConvert.SerializeObject(fpx);
            List<SelectListItem> Champion = new List<SelectListItem>();
            foreach (var item in data)
            {
                SelectListItem selectListItem = new SelectListItem();
                if (fpx.RoomStayTypeId == item.Id)
                {
                    selectListItem.Selected = true;
                }
                selectListItem.Text = item.RoomStayTypeName;
                selectListItem.Value = item.Id.ToString();
                Champion.Add(selectListItem);
            }
            ViewBag.SelectListItemlist = Champion;

            List<SelectListItem> Champion1 = new List<SelectListItem>();
            var data1 = dbroomnumber.GetRoomStayNumbers();
            foreach (var item in data1)
            {
                SelectListItem selectListItem = new SelectListItem();
                if (fpx.RoomStayNumberId == item.Id)
                {
                    selectListItem.Selected = true;
                }
                selectListItem.Text = item.StayNumber + "人制";
                selectListItem.Value = item.Id.ToString();
                Champion1.Add(selectListItem);
            }

            ViewBag.RoomNumber = Champion1;
            return View();
        }

        /// <summary>
        /// post 请求
        /// </summary>
        /// <param name="param0">对象</param>
        /// <param name="param1">tungid</param>
        /// <param name="param2">floorid</param>
        /// <returns></returns>
        public ActionResult HttpPostUpdRoom(DormInformation param0, int param1, int param2)
        {
            AjaxResult ajaxResult = new AjaxResult();
            try
            {
                dbdorm = new DormInformationBusiness();
                dbacc = new AccdationinformationBusiness();
                dbtungfloor = new TungFloorBusiness();
                var obj = dbdorm.GetEntity(param0.ID);
                if (!dbacc.HasSomeone(param0.ID))
                {
                    ajaxResult.Success = false;
                    ajaxResult.Msg = "存在人员不能修改。";

                    /*记录下来是哪个用户登陆进行的一个使用js操作恶意进行的请求*/
                }
                else
                {

                    ///如果修改的是自己
                    if (obj.DormInfoName == param0.DormInfoName)
                    {
                        Update(param0, param1, param2, obj);
                        ajaxResult.Success = true;
                        ajaxResult.Msg = "操作成功";
                    }
                    ///如果不是自己
                    else
                    {
                        var fpx = dbtungfloor.GetTungFloorByTungIDAndFloorID(param1, param2);
                        bool iscun = dbdorm.DuplicateName(fpx.Id, param0.DormInfoName);
                        if (iscun)
                        {
                            ajaxResult.Success = true;
                            ajaxResult.Msg = "该楼层已经存在该房间号";
                        }
                        else
                        {
                            Update(param0, param1, param2, obj);
                            ajaxResult.Success = true;
                            ajaxResult.Msg = "操作成功";
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                ajaxResult.Success = false;
                ajaxResult.Msg = "联系管理员";
            }
            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="param0"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public void Update(DormInformation param0, int param1, int param2,DormInformation param3) {
            dbtungfloor = new TungFloorBusiness();
            dbroomxml = new RoomdeWithPageXmlHelp();
            dbdorm = new DormInformationBusiness();
            var fpx = dbtungfloor.GetTungFloorByTungIDAndFloorID(param1, param2);
            var fpxnb = dbroomxml.GetRoomType(RoomTypeEnum.RoomType.Warehouse);
            var fpxnb1 = dbroomxml.GetRoomType(RoomTypeEnum.RoomType.VisitRoom);
            
            if (param0.RoomStayTypeId == fpxnb)
            {
                param3.RoomStayNumberId = 0;
                param3.SexType = 0;
            }
            if (param0.RoomStayTypeId == fpxnb1)
            {
                param3.SexType = 0;
            }
            param3.DormInfoName = param0.DormInfoName;
            param3.Remark = param0.Remark;
            param3.RoomStayNumberId = param0.RoomStayNumberId;
            param3.RoomStayTypeId = param0.RoomStayTypeId;
            param3.SexType = param0.SexType;
            param3.Direction = param0.Direction;
            dbdorm.Update(param3);
        }
    }
}