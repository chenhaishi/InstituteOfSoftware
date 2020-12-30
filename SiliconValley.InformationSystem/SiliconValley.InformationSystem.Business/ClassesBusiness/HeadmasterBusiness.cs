using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.Cloudstorage_Business;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Business.Employment;
using SiliconValley.InformationSystem.Entity.Base_SysManage;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.ClassesBusiness
{
  public  class HeadmasterBusiness:BaseBusiness<Headmaster>
    {

        //班主任带班
        BaseBusiness<HeadClass> Hoadclass = new BaseBusiness<HeadClass>();
        //添加班主任
        public bool AddHeadmaster(string informatiees_Id)
        {
            bool str = true;
          
            try
            {
                Headmaster informatiees = new Headmaster();
            informatiees.IsDelete = false;
            informatiees.AddTime = DateTime.Now;
            informatiees.informatiees_Id = informatiees_Id;
                informatiees.IsAttend = true;
                
                this.Insert(informatiees);
                BusHelper.WriteSysLog("添加班主任成功", Entity.Base_SysManage.EnumType.LogType.添加数据);

            }
            catch (Exception ex)
            {
                
                str = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            return str;

        }
          /// <summary>
          /// 班主任离职
          /// </summary>
          /// <param name="informatiees_Id">员工编号</param>
          /// <returns></returns>
        public bool removeHeadmaster(string informatiees_Id)
        {
            bool str = true;

            try
            {
             var x=   this.GetList().Where(a => a.informatiees_Id == informatiees_Id).FirstOrDefault();
                x.IsDelete = true;
                this.Update(x);
                BusHelper.WriteSysLog("修改班主任状态", Entity.Base_SysManage.EnumType.LogType.编辑数据);

            }
            catch (Exception ex)
            {

                str = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return str;

        }
        //员工表
        EmployeesInfoManage employeesInfoManage = new EmployeesInfoManage();
        //学员班级
        ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
        //班主任职业素养培训
        BaseBusiness<Professionala> ProfessionalaBusiness = new BaseBusiness<Professionala>();
      
        //班主任离职时间
        public bool QuitEntity(string informatiees_Id)
        {
            bool str = true;
            try
            {
                var x = this.GetEntity(informatiees_Id);
                x.IsDelete = true;
                this.Update(x);
            }
            catch (Exception ex)
            {

                str = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            
            }
            return str;
        }
        /// <summary>
        /// 带班业务左右回调操作
        /// </summary>
        /// <param name="id">带班人id</param>
        /// <param name="ClassName">班级名称</param>
        /// <param name="Index">添加或者删除(1则删除)</param>
        /// <returns></returns>
        public bool HeadClassEnti(string id,string ClassName,int Index)
        {
            bool str = true;
            List<HeadClass> list = new List<HeadClass>();
            ClassName = ClassName.Substring(0, ClassName.Length - 1);
            string[] ClassNames = ClassName.Split(',');
            foreach (var item in ClassNames)
            {
                //学员班级
                ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
                var class_id = classScheduleBusiness.FintClassSchedule(int.Parse(item)).id;
                var mysex = Hoadclass.GetList().Where(a => a.IsDelete == false && a.LeaderID == int.Parse(id) && a.ClassID == class_id).FirstOrDefault();

                if (Index==0)
                {
                    HeadClass headClass = new HeadClass();
                    headClass.AddTime = DateTime.Now;
                    headClass.LeadTime = DateTime.Now;
                    headClass.ClassID = class_id;
                    headClass.IsDelete = false;
                    headClass.LeaderID = int.Parse(id);
                    list.Add(headClass);
                }
                else
                {
                    list.Add(mysex);
                }
                    
               
            }
            try
            {
                if (Index == 1)
                {
                   
                    Hoadclass.Delete(list);
                    BusHelper.WriteSysLog("删除数据", Entity.Base_SysManage.EnumType.LogType.删除数据);
                }
                else
                {
                    Hoadclass.Insert(list);
                    BusHelper.WriteSysLog("添加数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                  
                }
            }
            catch (Exception ex)
            {
                str = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.系统异常);
                
            }
            return str;
        
        }
        /// <summary>
        /// 通过班级号获取班主任
        /// </summary>
        /// <param name="ClassName">班级名称</param>
        /// <returns></returns>
        public EmployeesInfo ClassHeadmaster(int ClassName)
        {
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            var mysex = Hoadclass.GetList().Where(a =>  a.ClassID ==ClassName && a.EndingTime == null).ToList();
            HeadClass head = new HeadClass();
            if (mysex.Count>1)
            {
                head = mysex.Where(a => a.EndingTime == null).FirstOrDefault();
            }
            else
            {
                head = mysex.FirstOrDefault();
            }
            var leid = head == null?new Headmaster(): this.GetEntity(head.LeaderID);
            return leid == null ? new EmployeesInfo() : employeesInfoManage.GetEntity(leid.informatiees_Id);
        }
        /// <summary>
        /// 通过班级号获取最后一位班主任
        /// </summary>
        /// <param name="ClassName"></param>
        /// <returns></returns>
        public EmployeesInfo ClassHeadmasterPro(int ClassName)
        {
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            var mysex = Hoadclass.GetList().Where(a => a.ClassID == ClassName).ToList().OrderByDescending(a=>a.ID).FirstOrDefault();
           
          
            var leid = mysex == null ? new Headmaster() : this.GetEntity(mysex.LeaderID);
            return leid == null ? new EmployeesInfo() : employeesInfoManage.GetEntity(leid.informatiees_Id);
        }
        public EmployeesInfo HeadmastaerClassFine(int ClassID)
        {
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            var mysex = Hoadclass.GetList().Where(a => a.ClassID == ClassID&&a.EndingTime==null).ToList();
            HeadClass head = new HeadClass();
           
                head = mysex.OrderByDescending(a=>a.ID).FirstOrDefault();
          
            var leid = head == null ? new Headmaster() : this.GetEntity(head.LeaderID);
            return leid == null ? new EmployeesInfo() : employeesInfoManage.GetEntity(leid.informatiees_Id);
        }
        /// <summary>
        /// 带班业务按钮操作
        /// </summary>
        /// <param name="id">带班人id</param>
        /// <param name="ClassName">班级名称</param>
        /// <returns></returns>
        public bool HeadClassEntis(string id, string ClassName)
        {  //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            bool str = true;
            List<HeadClass> list = new List<HeadClass>();
            var mysex = Hoadclass.GetList().Where(a => a.IsDelete == false && a.LeaderID == int.Parse(id)&& a.EndingTime == null).ToList();
            if (!string.IsNullOrEmpty( ClassName))
            {
                ClassName = ClassName.Substring(0, ClassName.Length - 1);
                string[] ClassNames = ClassName.Split(',');
                foreach (var item in ClassNames)
                {
                    mysex.Remove(mysex.Where(a => a.ClassID == int.Parse(item)).FirstOrDefault());
                  var hoad=  Hoadclass.GetList().Where(a => a.EndingTime == null && a.ClassID == int.Parse(item) && a.LeaderID == int.Parse(id)).FirstOrDefault();
                    if (hoad==null)
                    {
                        HeadClass headClass = new HeadClass();
                        headClass.AddTime = DateTime.Now;
                        headClass.LeadTime = DateTime.Now;
                        headClass.ClassID = classScheduleBusiness.FintClassSchedule(int.Parse(item)).id;
                        headClass.IsDelete = false;
                        headClass.LeaderID = int.Parse(id);
                        list.Add(headClass);
                    }
                  
                }
            }
         
            try
            {
              
                    List<HeadClass> HeadList = new List<HeadClass>();
                    foreach (var item in mysex)
                    {
                     
                        item.EndingTime = DateTime.Now;
                        HeadList.Add(item);
                    }
                    Hoadclass.Update(HeadList);
                    BusHelper.WriteSysLog("修改班主任带班数据", Entity.Base_SysManage.EnumType.LogType.编辑数据);
                if (list.Count>0)
                {
                    Hoadclass.Insert(list);
                    BusHelper.WriteSysLog("添加班主任带班数据", Entity.Base_SysManage.EnumType.LogType.添加数据);
                }
                  
            }
            catch (Exception ex)
            {
                str = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.系统异常);

            }
            return str;

        }
        /// <summary>
        /// 根据学员学号获取当前班主任
        /// </summary>
        /// <param name="StudentID">学员id</param>
        /// <returns></returns>
        public EmployeesInfo Listheadmasters(string StudentID)
        {

            var ClassID = scheduleForTraineesBusiness.SutdentCLassName(StudentID);//获取班级
            var leid =ClassID==null?new HeadClass(): Hoadclass.GetList().Where
                (c => c.IsDelete == false  && c.ClassID == ClassID.ID_ClassName).FirstOrDefault();//查询带班班主任id

            var Empid =leid==null?new Headmaster(): this.GetEntity(leid.LeaderID);//员工编号
            return Empid==null?new EmployeesInfo(): employeesInfoManage.GetEntity(Empid.informatiees_Id);
        }

        /// <summary>
        /// 根据学员学号获取当前班主任
        /// </summary>
        /// <param name="StudentID">学员id</param>
        /// <returns>员工实体</returns>
        public EmployeesInfo GetEmployessByStuid(string StudentID)
        {
            var ClassID = scheduleForTraineesBusiness.SutdentCLassName(StudentID);//获取班级

            var leid =  Hoadclass.GetList().Where
                (c => c.IsDelete == false && c.ClassID == ClassID.ID_ClassName).OrderBy(c=>c.EndingTime).FirstOrDefault();//查询带班班主任id

            var Empid = leid == null ? new Headmaster() : this.GetEntity(leid.LeaderID);//员工编号
            return Empid == null ? new EmployeesInfo() : employeesInfoManage.GetEntity(Empid.informatiees_Id);
        }


        /// <summary>
        /// 班主任职业素养培训数据
        /// </summary>
        /// <param name="page">第几页</param>
        /// <param name="limit">多少条数据</param>
        /// <returns></returns>
        public List<Professionala> GetDateProfessionala(int page, int limit)
        {
            SessionHelper.Session["Professionalapage"] = page;
            SessionHelper.Session["Professionalalimit"] = limit;
           var list= ProfessionalaBusiness.GetList().Where(a => a.Dateofregistration == false).ToList();
           return list.OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList();
        }
        /// <summary>
        /// 记录班主任职业素养培训课件
        /// </summary>
        /// <param name="professionala">数据对象</param>
        /// <returns></returns>
        public object AddProfessionala(Professionala professionala)
        {
            var result=new object();

            try
            {
                professionala.Dateofregistration = false;
                professionala.AddTime = DateTime.Now;
                ProfessionalaBusiness.Insert(professionala);
                result = new {
                    Success = true,
                    Msg = "录入成功",
                    page = SessionHelper.Session["Professionalapage"],
                    limit = SessionHelper.Session["Professionalalimit"]
                };

                BusHelper.WriteSysLog("班主任职业素养课件添加", Entity.Base_SysManage.EnumType.LogType.添加数据);
            }
            catch (Exception ex)
            {
                result = new
                {
                    Success = false,
                    Msg = "服务器错误！",
                    page = SessionHelper.Session["Professionalapage"],
                    limit = SessionHelper.Session["Professionalalimit"]
                };

                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);
               
            }
            return result;
        }
        /// <summary>
        /// 查询单条班主任职业素养课件培训
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public ProfessionalaView FineProfessionala(int id)
        {
            var x = ProfessionalaBusiness.GetEntity(id);
            ProfessionalaView professionala = new ProfessionalaView();
            professionala.ID = x.ID;
            professionala.Remarks = x.Remarks;
            professionala.Trainee = x.Trainee;
            professionala.Trainingcontent = x.Trainingcontent;
            professionala.TrainingDate = x.TrainingDate;
            professionala.TrainingTitle = x.TrainingTitle;
            professionala.EmpNameTraine = employeesInfoManage.GetEntity(this.GetEntity(x.Trainee).informatiees_Id).EmpName;
            return professionala;
        }
        /// <summary>
        /// 班主任带班数据
        /// </summary>
        /// <returns></returns>
        public List<TeamleaderdistributionView> ListTeamleaderdistributionView()
        {
            //班级业务
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();

            CloudstorageBusiness db_Bos = new CloudstorageBusiness();
            var ClassZ=  classScheduleBusiness.ClassList();
           
            return ClassZ.Select(a => new TeamleaderdistributionView
            {
                HeadmasterName = classScheduleBusiness.HeadSraffFine(a.id).EmployeeId==null?"无带班老师": classScheduleBusiness.HeadSraffFine(a.id).EmpName,
              
                ClassName = classScheduleBusiness.GetEntity(a.id).ClassNumber,
                ClassID=(int)a.id,
                 Stage= classScheduleBusiness.GetClassGrand((int)a.id, 222),
                 Major= classScheduleBusiness.GetClassGrand((int)a.id, 1),
                 HeadmasterImages= classScheduleBusiness.HeadSraffFine(a.id).EmployeeId == null?"": db_Bos.ImagesFine("xinxihua", "EmpImage", classScheduleBusiness.HeadSraffFine(a.id).Image, 10) 

            }).ToList();
        }
        /// <summary>
        ///通过班主任获取带班记录
        /// </summary>
        /// <param name="informatiees_Id">班主任id</param>
        /// <returns></returns>
        public object SuccessionrecordDate(int page, int limit,int informatiees_Id)
        {
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();

            var LIST = Hoadclass.GetList().Where(a => a.LeaderID == informatiees_Id).ToList();
               var Mylist=LIST.Select(a=>new {
              ClassName= classScheduleBusiness.GetEntity(a.ClassID).ClassNumber,//班级名称
              ClassID= classScheduleBusiness.GetEntity(a.ClassID).id,//班级编号
         
              a.LeadTime,//开始时间
              a.EndingTime,//结束时间
              a.Remarks, //备注
             a.ID
          }).OrderBy(a => a.ID).Skip((page - 1) * limit).Take(limit).ToList(); ;
            var data = new
            {
                code = "",
                msg = "",
                count = LIST.Count,
                data = Mylist
            };
            return data;

        }
        /// <summary>
        /// 通过班主任id获取带班班级
        /// </summary>
        /// <param name="informatiees_Id">班主任id</param>
        /// <param name="Endtime">true为正在带班数据，false为带班所有数据</param>
        /// <returns></returns>
        public List<ClassSchedule> EmpClass(int informatiees_Id,bool Endtime)
        {
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            //班主任带班数据
            var list=  Hoadclass.GetList().Where(a => a.LeaderID == informatiees_Id).ToList();
            if (Endtime==true)
            {
                list = list.Where(a => a.EndingTime == null).ToList();
            }
            //班级数据
            List<ClassSchedule> classlist = new List<ClassSchedule>();
            foreach (var item in list)
            {
                classlist.Add(classScheduleBusiness.GetEntity(item.ClassID));
            }
            return classlist;
        }
        /// <summary>
        /// 根据班级编号结束班主任带班
        /// </summary>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public bool EndDai(int ClassID)
        {
            bool bol= false;
         var x= Hoadclass.GetList().Where(a => a.ClassID == ClassID && a.EndingTime == null).FirstOrDefault();
            if (x==null)
            {
                return false;
            }
            x.EndingTime = DateTime.Now;
            try
            {
                Hoadclass.Update(x);
                bol = true;
                BusHelper.WriteSysLog("修改班主任带班数据", EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, EnumType.LogType.编辑数据);
            }
            return bol;
            
        }
        /// <summary>
        /// 是否允许或者禁止班主任上质素课
        /// </summary>
        /// <param name="id">班主任表主键id</param>
        /// <param name="Isdele">允许或者拒绝</param>
        /// <returns></returns>
        public bool IsAttend(int id,bool Isdele)
        {
            bool str=true;
            try
            {
                var headmaster = this.GetEntity(id);
                headmaster.IsAttend = Isdele;
                this.Update(headmaster);
               
                BusHelper.WriteSysLog("班主任表修改质素课属性数据", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {
                str = false;
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            return str;

        }
        /// <summary>
        /// 班主任培训人数据
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="EmployeeId"员工编号></param>
        /// <param name="EmpName">员工姓名</param>
        /// <param name="department">部门</param>
        /// <returns></returns>
        public object TraineeDate(int page, int limit, string EmployeeId, string EmpName, string department)
        {
            List<TraineeDateVIew> listteainees = new List<TraineeDateVIew>();
            //班主任表
           var x= this.GetList().Where(a=>a.IsDelete==false).ToList();
            foreach (var item in x)
            {
                TraineeDateVIew traineeDateVIew = new TraineeDateVIew();
                traineeDateVIew.id = item.ID;
                traineeDateVIew.EmployeeId = item.informatiees_Id;
                //拿到员工对象
                var employee=  employeesInfoManage.GetList().Where(a => a.EmployeeId == item.informatiees_Id).FirstOrDefault();
                traineeDateVIew.EmpName = employee.EmpName;
                traineeDateVIew.sex = employee.Sex;
                traineeDateVIew.department = employeesInfoManage.GetDeptByEmpid(employee.EmployeeId).DeptName;
                traineeDateVIew.departmentID = employeesInfoManage.GetDeptByEmpid(employee.EmployeeId).DeptId;
                listteainees.Add(traineeDateVIew);
            }
            if (!string.IsNullOrEmpty(EmployeeId))
            {
                listteainees = listteainees.Where(a => a.EmployeeId.Contains(EmployeeId)).ToList();
            }
            if (!string.IsNullOrEmpty(EmpName))
            {
                listteainees = listteainees.Where(a => a.EmpName.Contains(EmpName)).ToList();
            }
            if (!string.IsNullOrEmpty(department))
            {
                int departmentID = int.Parse(department);
                listteainees = listteainees.Where(a => a.departmentID==departmentID).ToList();
            }
          var   dataList= listteainees.OrderBy(a => a.id).Skip((page - 1) * limit).Take(limit).ToList();
            var data = new
            {
                code = "",
                msg = "",
                count = listteainees.Count,
                data = dataList
            };
            return data;
        }

      
    }
}
