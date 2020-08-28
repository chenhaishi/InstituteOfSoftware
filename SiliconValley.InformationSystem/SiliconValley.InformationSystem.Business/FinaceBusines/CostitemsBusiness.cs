using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.StudentKeepOnRecordBusiness;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.XYK_Data;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.FinaceBusines
{
    //费用明细
 public   class CostitemsBusiness:BaseBusiness<Costitems>
    {
        //阶段
        GrandBusiness Grandcontext = new GrandBusiness();
        //费用明目类别
        BaseBusiness<CostitemsX> cisitemesx = new BaseBusiness<CostitemsX>();
        /// <summary>
        /// 查询名目是否重复
        /// </summary>
        /// <param name="Stage">阶段id</param>
        /// <param name="Name">名目名称</param>
        /// <returns></returns>
        public int BoolName(int ?Stage,string Name)
        {
            Stage = Stage == 0 ? null : Stage;
           return this.GetList().Where(a => a.IsDelete == false && a.Grand_id == Stage && a.Name == Name).Count();
        }
        /// <summary>
        /// 名目数据添加
        /// </summary>
        /// <param name="costitems">数据对象</param>
        /// <returns></returns>
        public AjaxResult AddCostitems(Costitems costitems)
        {  
            AjaxResult retus = null;
            try
            {
                costitems.IsDelete = false;
               
               
                 this.Insert(costitems);
             
               
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "添加数据";
                BusHelper.WriteSysLog("添加数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }
        /// <summary>
        /// 验证明目表是否有重复的数据
        /// </summary>
        /// <param name="costitems"></param>
        /// <returns></returns>
        public int BoolCostitems(Costitems costitems)
        {
            costitems.Grand_id= costitems.Grand_id > 0 ? costitems.Grand_id : null;
        return this.GetList().Where(a => a.IsDelete==false&&a.Name==costitems.Name&&a.Rategory==costitems.Rategory&&a.Grand_id==costitems.Grand_id).Count();
          
        }
        /// <summary>
        /// 费用名目查询
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public object DateCostitems(int page, int limit,string grade_Id,string Typex)
        {
           
            var list = this.GetList().Where(a=>a.Rategory != cisitemesx.GetList().Where(z => z.Name == "其它" && z.IsDelete == false).FirstOrDefault().id).Select(a=>new {
                a.id,a.Name,a.Amountofmoney,Stage=a.Grand_id>0? Grandcontext.GetEntity(a.Grand_id).GrandName:"暂无",
                Typex= cisitemesx.GetEntity(a.Rategory).Name,
                a.Grand_id,
                a.Rategory,
                a.IsDelete
            }).ToList();
            if (!string.IsNullOrEmpty(grade_Id))
            {
                int gradeid = int.Parse(grade_Id);
                list= list.Where(a => a.Grand_id == gradeid).ToList();
            }
            if (!string.IsNullOrEmpty(Typex))
            {
                int TypexID = int.Parse(Typex);
                list = list.Where(a => a.Rategory == TypexID).ToList();
            }
            var dataList = list.OrderBy(a => a.id).Skip((page - 1) * limit).Take(limit).ToList();
            //  var x = dbtext.GetList();
            var data = new
            {
                code = "",
                msg = "",
                count = list.Count,
                data = dataList
            };
            return data;
        }
        /// <summary>
        /// 查询费用类别是否重复
        /// </summary>
        /// <param name="Name">名称</param>
        /// <returns></returns>
        public int TypeName(string Name)
        {
           return cisitemesx.GetList().Where(a => a.IsDelete == false && a.Name == Name).Count();
        }
        /// <summary>
        /// 添加费用明目类型
        /// </summary>
        /// <param name="costitemsX">数据对象</param>
        /// <returns></returns>
        public AjaxResult AddType(CostitemsX costitemsX)
        {
            AjaxResult retus = null;
            try
            {
                costitemsX.IsDelete = false;
                costitemsX.AddDate = DateTime.Now;
                cisitemesx.Insert(costitemsX);
                retus = new SuccessResult();
                retus.Success = true;
                retus.Msg = "添加数据";
                BusHelper.WriteSysLog("添加数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {

                retus = new ErrorResult();
                retus.Msg = "服务器错误";
                retus.Success = false;
                retus.ErrorCode = 500;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return retus;
        }
        /// <summary>
        /// 查询缴费名单
        /// </summary>
        /// <param name="date">按时间查询</param>
        /// <param name="type">按类型查询1--查询学费，2--查询预录费</param>
        ///  <param name="grand">阶段</param>
        /// <returns></returns>
        public AjaxResult TypeSelect(DateTime date,int type,int grand)
        {
            AjaxResult list = new AjaxResult() { Msg="没有查询到数据",Success=false};

            StudentDataKeepAndRecordBusiness studentdatakeep = new StudentDataKeepAndRecordBusiness();

            List<Student> liststudent = new List<Student>();

            if (type==1)
            {
                StringBuilder sb = new StringBuilder(@"select stu.Name as 'StuName',StudentPutOnRecord_Id as 'Keepid',SUM(Amountofmoney) as 'sumMoney' from StudentFeeRecord as s  inner join StudentInformation as stu  on stu.StudentNumber = s.StudenID where Costitemsid in (select c.id from Costitems as c inner join Grand as g on c.Grand_id = " + grand + "   where g.Id = 1) and MONTH(s.AddDate)= " + date.Month + " and YEAR(s.AddDate)= " + date.Year + " group by s.StudenID,stu.Name,stu.StudentPutOnRecord_Id");

                    liststudent= this.GetListBySql<Student>(sb.ToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder(@"select p.identitydocument as 'StuName',p.keeponrecordid as 'Keepid',Sum(p.Amountofmoney) as 'sumMoney' from Preentryfee as p 
where MONTH(p.AddDate) = "+date.Month+" and YEAR(p.AddDate) = "+date.Year+" group by p.identitydocument, p.keeponrecordid");

                liststudent = this.GetListBySql<Student>(sb.ToString()).Select(l=>new Student() { Keepid=l.Keepid,StuName= studentdatakeep.findId(l.Keepid.ToString())==null?"无":studentdatakeep.findId(l.Keepid.ToString()).StuName, sumMoney =l.sumMoney }).ToList();
                
            }

            if (liststudent.Count>0)
            {
                list.Success = true;
                list.Data = liststudent;
            }

            return list;
        }
        /// <summary>
        /// 获取明目类型的所有数据
        /// </summary>
        /// <returns></returns>
        public List<CostitemsX> TypeDate()
        {
          return  cisitemesx.GetList().Where(a => a.IsDelete == false&&a.Name!="其它").ToList();
        }
        /// <summary>
        /// 获取名目的所有数据
        /// </summary>
        /// <returns></returns>
        public List< Costitems> costitemslist()
        {
            return this.GetList().Where(a => a.IsDelete == false).ToList();
        }

   

       
    }
}
