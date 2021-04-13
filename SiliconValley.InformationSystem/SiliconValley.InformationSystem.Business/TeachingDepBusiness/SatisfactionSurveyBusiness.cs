using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.TeachingDepBusiness
{
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Entity.Base_SysManage;
    using SiliconValley.InformationSystem.Entity.Entity;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity;
    using System.Xml;



    /// <summary>
    /// 满意度调查业务类
    /// </summary>
    public class SatisfactionSurveyBusiness : BaseBusiness<SatisficingItem>
    {
        CourseBusiness db_course = new CourseBusiness();

        EmployeesInfoManage db_emp = new EmployeesInfoManage();

        BaseBusiness<StudentInformation> db_student = new BaseBusiness<StudentInformation>();

        BaseBusiness<ClassSchedule> db_class = new BaseBusiness<ClassSchedule>();

        BaseBusiness<SatisficingResult> db_satisresult = new BaseBusiness<SatisficingResult>();

        BaseBusiness<SatisficingConfig> db_satisconfig = new BaseBusiness<SatisficingConfig>();

        BaseBusiness<SatisficingResultDetail> db_satisresultdetail = new BaseBusiness<SatisficingResultDetail>();

        private readonly BaseBusiness<SatisfactionSurveyObject> db_satisfactionObject;


        /// <summary>
        /// 满意度调查对象实例
        /// </summary>
        private readonly BaseBusiness<SatisficingType> db_saitemtype;
        public SatisfactionSurveyBusiness()
        {

            db_saitemtype = new BaseBusiness<SatisficingType>();
            db_satisfactionObject = new BaseBusiness<SatisfactionSurveyObject>();
        }

        public List<SatisficingItem> GetAllSatisfactionItems()
        {

            return this.GetList().Where(d => d.IsDel == false).ToList();

        }

        public List<SatisficingResult> AllsatisficingResults()
        {
            return db_satisresult.GetIQueryable().ToList();
        }


        /// <summary>
        /// 获取所有满意度调查对象实例
        /// </summary>
        /// <returns></returns>
        public List<SatisfactionSurveyObject> AllSatisfactionSurveyObject()
        {
            return db_satisfactionObject.GetList().ToList();
        }

        public void insertSatisfactionResult(SatisficingResult satisficingResult)
        {
            db_satisresult.Insert(satisficingResult);
        }

        /// <summary>
        /// 对调查具体项进行筛选
        /// </summary>
        /// <param name="DepID">调查对象Id</param>
        /// <param name="satisfactionTypeID">调查类型ID 比如:学术能力、教学态度、教学能力</param>
        /// <returns></returns>
        public List<SatisficingItem> Screen(int DepID, int satisfactionTypeID)
        {

            List<SatisficingItem> resultlist = new List<SatisficingItem>();


            if (DepID == 0 && satisfactionTypeID == 0)
            {
                resultlist = this.GetAllSatisfactionItems();
            }
            else if (satisfactionTypeID == 0)
            {
                var list = this.GetAllSatisfactionItems();

                foreach (var item in list)
                {
                    //获取项的类型
                    var objtype = db_saitemtype.GetList().Where(d => d.ID == item.ItemType).FirstOrDefault();

                    if (objtype.DepartmentID == DepID)
                    {
                        resultlist.Add(item);
                    }
                }
            }
            else
            {
                var list = this.GetAllSatisfactionItems();

                foreach (var item in list)
                {
                    //获取项的类型
                    var objtype = db_saitemtype.GetList().Where(d => d.ID == satisfactionTypeID && d.DepartmentID == DepID).FirstOrDefault();

                    if (item.ItemType == objtype.ID)
                    {

                        resultlist.Add(item);

                    }

                }

            }

            return resultlist;


        }


        /// <summary>
        /// 转为视图模型
        /// </summary>
        /// <param name="satisficingItem">数据实体</param>
        /// <returns></returns>
        public SatisfactionSurveyView ConvertModelView(SatisficingItem satisficingItem)
        {
            SatisfactionSurveyView surveyView = new SatisfactionSurveyView();

            surveyView.IsDel = satisficingItem.IsDel;
            surveyView.ItemContent = satisficingItem.ItemContent;
            surveyView.ItemID = satisficingItem.ItemID;

            var type = db_saitemtype.GetList().Where(d => d.ID == satisficingItem.ItemType).FirstOrDefault();

            if (type != null)
            {
                surveyView.ItemType = type;
            }
            else
            {
                surveyView.ItemType = null;
            }

            surveyView.Remark = satisficingItem.Remark;

            return surveyView;
        }

        /// <summary>
        /// 获取所有调查类型
        /// </summary>
        /// <returns></returns>
        public List<SatisficingType> GetSatisficingTypes()
        {
            return db_saitemtype.GetList();
        }

        /// <summary>
        /// 筛选调查项类型
        /// </summary>
        /// <param name="typename">类型名称</param>
        /// <returns></returns>
        public List<SatisficingType> Screen(string typename, int depid)
        {

            if (string.IsNullOrEmpty(typename))

            {
                return this.GetSatisficingTypes().Where(d => depid == d.DepartmentID).ToList();

            }
            else
            {
                return this.GetSatisficingTypes().Where(d => d.TypeName.Trim() == typename.Trim() && depid == d.DepartmentID).ToList();
            }

        }

        /// <summary>
        /// 添加调查项类型
        /// </summary>
        /// <param name="typename">类型名称</param>
        public void AddSurveyItemType(string typename, int depid)
        {
            SatisficingType satisficingType = new SatisficingType();

            satisficingType.DepartmentID = depid;
            satisficingType.TypeName = typename.Trim();

            db_saitemtype.Insert(satisficingType);

        }


        /// <summary>
        /// 删除调查项类型
        /// </summary>
        /// <param name="typeid">类型ID</param>
        public void RemoveItemType(int typeid)
        {

            //首先删除这个类型下面的具体项

            var list = this.GetAllSatisfactionItems().Where(d => d.ItemType == typeid).ToList();


            this.Delete(list);

            var obj = db_saitemtype.GetList().Where(d => d.ID == typeid).FirstOrDefault();

            db_saitemtype.Delete(obj);

        }

        public SatisficingConfigDataView ConvertToSatisficingConfigDataView(SatisficingConfig satisficingConfig)
        {

            SatisficingConfigDataView view = new SatisficingConfigDataView();
            view.Curriculum = db_course.GetCurriculas().Where(d => d.CurriculumID == satisficingConfig.CurriculumID).FirstOrDefault();
            view.Emp = db_emp.GetList().Where(d => d.EmployeeId == satisficingConfig.EmployeeId && d.IsDel == false).FirstOrDefault();
            view.investigationClass = db_class.GetList().Where(d => d.id == satisficingConfig.ClassNumber).FirstOrDefault(); 
            view.investigationDate = (DateTime)satisficingConfig.CreateTime;
            view.SatisficingConfigId = satisficingConfig.ID;
            //计算总分
            var staresultlist = this.AllsatisficingResults().Where(d => d.SatisficingConfig == satisficingConfig.ID).ToList();

            var total = 0;

            staresultlist.ForEach(d=>
            {
                 var templist = db_satisresultdetail.GetList().Where(b => b.SatisficingBill == d.ID).ToList();

                templist.ForEach(x=>
                {
                    total += (int)x.Scores;
                });
            });

            view.TotalScore = total;
            if (staresultlist.Count == 0)
            {
                view.Average = total / 1;
            }
            else {
                view.Average = total / staresultlist.Count;
            }
            return view;



        } 
        public SatisfactionSurveyDetailView ConvertToViewModel(SatisficingResult satisficingResult)
        {
            SatisfactionSurveyDetailView detailView = new SatisfactionSurveyDetailView();

            var satisficingConfig = db_satisconfig.GetList().Where(d => d.IsDel == false && d.ID == satisficingResult.SatisficingConfig).FirstOrDefault();

            detailView.SatisficingConfigId = satisficingConfig.ID;
            detailView.Curriculum = db_course.GetCurriculas().Where(d => d.CurriculumID == satisficingConfig.CurriculumID).FirstOrDefault();

            detailView.Emp = db_emp.GetList().Where(d => d.EmployeeId == satisficingConfig.EmployeeId && d.IsDel == false).FirstOrDefault();

            detailView.investigationClass = db_class.GetList().Where(d => d.id == satisficingConfig.ClassNumber).FirstOrDefault();

            detailView.FillInPerson = db_student.GetList().Where(d => d.StudentNumber == satisficingResult.Answerer).FirstOrDefault();

            detailView.investigationDate = (DateTime)satisficingConfig.CreateTime;

            detailView.Proposal = satisficingResult.Suggest;

            detailView.SurveyResultID = satisficingResult.ID;

            var templist = db_satisresultdetail.GetList().Where(d => d.SatisficingBill == satisficingResult.ID).ToList();

            foreach (var item in templist)
            {

                detailView.TotalScore += (int)item.Scores;

            }

            List<SatisficingResultDetailView> templist1 = new List<SatisficingResultDetailView>();

            var ss = db_satisresultdetail.GetList().Where(d => d.SatisficingBill == satisficingResult.ID).ToList();

            foreach (var item in ss)
            {

                var obj1 = this.ConvertToSatisfactionSurveyDetailView(item);

                templist1.Add(obj1);

            }

            detailView.detailitem = templist1;

            return detailView;

        }


        public SatisficingResultDetailView ConvertToSatisfactionSurveyDetailView(SatisficingResultDetail detail)
        {



            SatisficingResultDetailView detailview = new SatisficingResultDetailView();

            detailview.ID = detail.ID;
            detailview.SatisficingBill = detail.SatisficingBill;
            detailview.SatisficingItem = this.GetList().Where(d => d.ItemID == detail.SatisficingItem).FirstOrDefault();
            detailview.Scores = detail.Scores;

            return detailview;



        }
        /// <summary>
        /// 获取满意度调查详细数据
        /// </summary>
        /// <param name="empid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<SatisfactionSurveyDetailView> SurveyHistoryData(string empid, string date, int? Curriculum, string classnumber)
        {


            List<SatisfactionSurveyDetailView> resultlist = new List<SatisfactionSurveyDetailView>();

            //判断部门

            var emp = db_emp.GetList().Where(d => d.EmployeeId == empid).FirstOrDefault();

            EmployeesInfoManage empmanage = new EmployeesInfoManage();

            var dep = empmanage.GetDept(emp.PositionId);

            if (dep.DeptName.Contains("教质部"))
            {
                //教质部
                resultlist = SurveyHistoryData(empid, classnumber, date);
            }

            if (dep.DeptName.Contains("教学部"))
            {
                //教学部
                resultlist = SurveyHistoryData(empid, (int)Curriculum, classnumber);

            }
            return resultlist;

        }
        /// <summary>
        /// 获取满意度调查详细数据 --教员
        /// </summary>
        /// <returns></returns>
        public List<SatisfactionSurveyDetailView> SurveyHistoryData(string empid, int Curriculum, string classnumber)
        {
            List<SatisfactionSurveyDetailView> resultlist = new List<SatisfactionSurveyDetailView>();
            List<SatisficingConfig> templist = new List<SatisficingConfig>();

            if (string.IsNullOrEmpty(classnumber) || Curriculum == 0)
            {
                templist = db_satisconfig.GetList().Where(d => d.EmployeeId == empid).ToList();
            }
            else
            {
                templist = db_satisconfig.GetList().Where(d => d.EmployeeId == empid && d.CurriculumID == Curriculum && d.ClassNumber == int.Parse(classnumber)).ToList();
            }

            foreach (var item in templist)
            {

                var temp1list = db_satisresult.GetList().Where(d => d.SatisficingConfig == item.ID).ToList();

                foreach (var item1 in temp1list)
                {

                    var obj = this.ConvertToViewModel(item1);

                    if (obj != null)
                    {

                        resultlist.Add(obj);
                    }

                }

            }

            return resultlist;

        }

        public List<SatisfactionSurveyDetailView> SurveyHistoryData(string empid, string classnumber, string date)
        {
            List<SatisfactionSurveyDetailView> resultlist = new List<SatisfactionSurveyDetailView>();
            List<SatisficingConfig> templist = new List<SatisficingConfig>();

            if (classnumber == null)
            {
                templist = db_satisconfig.GetList().Where(d => d.EmployeeId == empid && d.CreateTime >= DateTime.Parse(date) && d.CreateTime < DateTime.Parse(date).AddMonths(1)).ToList();
            }
            else
            {
                templist = db_satisconfig.GetList().Where(d => d.EmployeeId == empid && d.CreateTime >= DateTime.Parse(date) && d.CreateTime < DateTime.Parse(date).AddMonths(1) && d.ClassNumber == int.Parse(classnumber)).ToList();
            }

            foreach (var item in templist)
            {

                var temp1list = db_satisresult.GetList().Where(d => d.SatisficingConfig == item.ID).ToList();

                foreach (var item1 in temp1list)
                {

                    var obj = this.ConvertToViewModel(item1);

                    if (obj != null)
                    {

                        resultlist.Add(obj);
                    }

                }

            }

            return resultlist;
        }


        /// <summary>
        /// 返回满意度调查结果
        /// </summary>
        /// <param name="id">iD</param>
        /// <returns></returns>
        public SatisficingResult GetSatisficingResultByID(int id)
        {
            return db_satisresult.GetList().Where(d => d.IsDel == false && d.ID == id).FirstOrDefault();

        }

        public List<SatisficingResult> SatisficingResults()
        {

            return db_satisresult.GetList();
        }

        public SatisfactionSurveyDetailView GetSatisficingBy(SatisficingResult satisficingResult)
        {

            return this.ConvertToViewModel(satisficingResult);

        }

        public List<SatisficingConfig> satisficingConfigs()
        {
            return db_satisconfig.GetList().Where(d => d.IsDel == false).ToList();
        }

        /// <summary>
        /// 添加 满意度调查总单
        /// </summary>
        /// <returns></returns>
        public bool AddSatisficingConfig(SatisficingConfig satisficingConfig)
        {

            bool result = true;

            try
            {
                db_satisconfig.Insert(satisficingConfig);
            }
            catch (Exception)
            {

                result = false;
            }

            return result;



        }

        /// <summary>
        /// 判断本月班主任满意度调查单是否已经生成
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="classnumber"></param>
        /// <param name="empid"></param>
        /// <returns></returns>

        public bool IsHaveHeadMasterSurveyConfig(string Date, string classnumber, string empid)
        {

            DateTime da = DateTime.Parse(Date);

            var list = this.satisficingConfigs().Where(d => d.IsDel == false && d.ClassNumber == int.Parse(classnumber) && d.EmployeeId == empid && DateTime.Parse(d.CreateTime.ToString()).Year == da.Year && DateTime.Parse(d.CreateTime.ToString()).Month == da.Month).ToList();

            return list != null;

        }

        /// <summary>
        /// 获取还未过期的满意度调查
        /// </summary>
        /// <returns></returns>
        public List<SatisficingConfig> GetSatisficingConfigNoCutOffdate()
        {
            return db_satisconfig.GetIQueryable().ToList().Where(d => d.CutoffDate >= DateTime.Now).ToList();
        }


        /// <summary>
        /// 获取学员在某个满意度调查单中的结果
        /// </summary>
        /// <param name="student"></param>
        /// <param name="SatisficingConfigId"></param>
        /// <returns></returns>
        public SatisficingResult GetSatisficingResult(string student, int SatisficingConfigId)
        {
            var result = db_satisresult.GetIQueryable().Where(d => d.Answerer == student && d.SatisficingConfig == SatisficingConfigId).FirstOrDefault();

            return result;
        }

        /// <summary>
        /// 获取学员可以填写的满意度
        /// </summary>
        /// <param name="student"></param>
        ///   /// <param name="type">教员(teacher), 教职(jiaozhi)</param>
        /// <returns></returns>
        public List<SatisficingConfig> GetSatisficingConfigsByStudent(string student, string type)
        {
            List<SatisficingConfig> result = new List<SatisficingConfig>();

            //条件 未到满意度填写截止日期 为当前学员的班级 未填写过
            //获取学员当前班级
            TeacherClassBusiness db_teaclsss = new TeacherClassBusiness();

            var stuClass = db_teaclsss.GetScheduleByStudent(student);

            //var 

            List<SatisficingConfig> list = new List<SatisficingConfig>();
            if (type == "teacher")
            {
                list = GetSatisficingConfigNoCutOffdate().Where(d => d.ClassNumber == stuClass.id && d.CurriculumID != null).ToList();
            }
            if (type == "jiaozhi")
            {
                list = GetSatisficingConfigNoCutOffdate().Where(d => d.ClassNumber == stuClass.id && d.CurriculumID == null).ToList();
            }
            if (type == "shitang")
            {
                list = GetSatisficingConfigNoCutOffdate().Where(d => d.ClassNumber == stuClass.id && d.CurriculumID == null && d.Isitacanteen == true).ToList();
            }
            if (type == "yuwen")
            {
                list = GetSatisficingConfigNoCutOffdate().Where(d => d.ClassNumber == stuClass.id && d.CurriculumID != null && d.Isitayuwen == true).ToList();
            }
            if (type == "shuxue")
            {
                list = GetSatisficingConfigNoCutOffdate().Where(d => d.ClassNumber == stuClass.id && d.CurriculumID != null && d.isitashuxue == true).ToList();
            }
            if (type == "yingyu")
            {
                list = GetSatisficingConfigNoCutOffdate().Where(d => d.ClassNumber == stuClass.id && d.CurriculumID != null && d.isitayingyu == true).ToList();
            }
            if (list.Count == 0)
            {
                return result;
            }

            foreach (var item in list)
            {
                //证明未填写过
                var tempobj = GetSatisficingResult(student, item.ID);

                if (tempobj == null)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public SatisficingConfigView ConvertToview(SatisficingConfig satisficingConfig)
        {
            SatisficingConfigView view = new SatisficingConfigView();

            TeacherClassBusiness db_teachclss = new TeacherClassBusiness();

            view.ClassNumber = db_teachclss.GetClassByClassNumber(satisficingConfig.ClassNumber.ToString());
            view.CreateTime = satisficingConfig.CreateTime;
            view.CurriculumID = db_course.GetCurriculas().Where(d => d.CurriculumID == satisficingConfig.CurriculumID).FirstOrDefault();
            view.CutoffDate = satisficingConfig.CutoffDate;
            view.EmployeeId = db_emp.GetInfoByEmpID(satisficingConfig.EmployeeId);
            view.ID = satisficingConfig.ID;
            view.IsDel = satisficingConfig.IsDel;
            view.IsPastDue = satisficingConfig.IsPastDue;
            view.Remark = satisficingConfig.Remark;

            return view;
        }


        public bool IsContains(List<EmployeesInfo> sources, EmployeesInfo employeesInfo)
        {
            foreach (var item in sources)
            {
                if (item.EmployeeId == employeesInfo.EmployeeId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsContains(List<Department> sources, Department department)
        {
            foreach (var item in sources)
            {
                if (item.DeptId == department.DeptId)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// 根据角色获取满意度历史记录
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetMyDepEmp(Base_UserModel user)
        {

            //获取账号所有的角色

            var userRoles = user.RoleIdList;

            //当前登录人的部门下的人  (人员可能重复)
            List<EmployeesInfo> emplist = new List<EmployeesInfo>();

            //循环获取每个角色的权限

            foreach (var role in userRoles)
            {
                // 权限id 权限名称 ,可查看的部门 


                //var permissions = PermissionManage.GetRolePermissionModules(role);  //获取角色所拥有的的权限
                BaseBusiness<OtherRoleMapPermissionValue> db_permissrole = new BaseBusiness<OtherRoleMapPermissionValue>();

                var permissions = db_permissrole.GetIQueryable().Where(d => d.RoleId == role).ToList();

                foreach (var permission in permissions)
                {
                    //根据权限到 配置文件中去匹配
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(System.Web.HttpContext.Current.Server.MapPath("/Areas/Teaching/config/empmanageConfig.xml"));

                    var xmlRoot = xmlDocument.DocumentElement;

                    var permissionConfig = (XmlElement)xmlRoot.GetElementsByTagName("permissions")[0];

                    //获取配置文件中的权限
                    XmlNodeList permissNmaes = permissionConfig.ChildNodes;

                    foreach (XmlElement item in permissNmaes)
                    {
                        if (item.Attributes["permissionid"].Value == permission.PermissionValue)
                        {
                            //获取部门
                            var depStr = item.Attributes["depIds"].Value.Split(',');
                            List<string> deplist = depStr.ToList();
                            if (depStr[depStr.Length - 1] == "")
                            {
                                deplist.RemoveAt(depStr.Length - 1);
                            }
                            //获取部门人员

                            foreach (var depItem in deplist)
                            {
                                emplist.AddRange(db_emp.GetEmpsByDeptid(int.Parse(depItem)));
                            }
                        }
                    }

                }


            }

            List<EmployeesInfo> resultEmplist = new List<EmployeesInfo>();

            foreach (var item in emplist)
            {
                if (!IsContains(resultEmplist, item))
                {
                    resultEmplist.Add(item);
                }
            }

            return resultEmplist;
        }

        public List<SatisficingConfig> SurveyData_filter(string empnumber, string date)
        {
            DateTime surveyDate = DateTime.Parse(date);

            if (empnumber == null || empnumber == "")
            {

                return this.satisficingConfigs().Where(d => ((DateTime)d.CreateTime).Year == surveyDate.Year && ((DateTime)d.CreateTime).Month == surveyDate.Month).ToList();
            }
            else
            {
                return this.satisficingConfigs().Where(d => d.EmployeeId == empnumber && ((DateTime)d.CreateTime).Year == surveyDate.Year && ((DateTime)d.CreateTime).Month == surveyDate.Month).ToList();
            }

        }


        /// <summary>
        /// 获取参加满意度调查的学员
        /// </summary>
        /// <returns></returns>
        public List<StudentInformation> JoinSurveyStudents(int SurveyConfigId)
        {
            List<StudentInformation> studentlist = new List<StudentInformation>();

            var list = this.AllsatisficingResults().Where(d => d.SatisficingConfig == SurveyConfigId).ToList();

            foreach (var item in list)
            {
                var tempobj = db_student.GetIQueryable().Where(d => d.StudentNumber == item.Answerer).FirstOrDefault();

                if (tempobj != null)
                    studentlist.Add(tempobj);
            }

            return studentlist;

        }

        /// <summary>
        /// 获取员工满意度统计
        /// </summary>
        /// <param name="empnumber"></param>
        /// <param name="date"></param>
        public List<SatisfactionSurveyDetailView> SurveyResult_Cost(string empnumber, DateTime date)
        {
            List<SatisfactionSurveyDetailView> result = new List<SatisfactionSurveyDetailView>();

            List<SatisficingConfig> configlist = SurveyData_filter(empnumber, date.ToString());


            foreach (var item in configlist)
            {
                var confgresult = AllsatisficingResults().Where(d => d.SatisficingConfig == item.ID).ToList();

                foreach (var item1 in confgresult)
                {
                    var detailview = ConvertToViewModel(item1);

                    if (detailview != null)
                    {
                        result.Add(detailview);
                    }
                    
                }
            }

            return result;


        }



    }
}
