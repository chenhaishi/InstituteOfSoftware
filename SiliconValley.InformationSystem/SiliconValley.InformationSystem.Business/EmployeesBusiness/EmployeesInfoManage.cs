using System;
using System.Collections.Generic;
using System.Linq;

namespace SiliconValley.InformationSystem.Business.EmployeesBusiness
{
    using SiliconValley.InformationSystem.Business.Channel;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Business.PositionBusiness;
    using SiliconValley.InformationSystem.Business.DepartmentBusiness;
    using SiliconValley.InformationSystem.Business.SchoolAttendanceManagementBusiness;
    using SiliconValley.InformationSystem.Util;
    using SiliconValley.InformationSystem.Business.Employment;
    using SiliconValley.InformationSystem.Business.ClassesBusiness;
    using SiliconValley.InformationSystem.Business.Consult_Business;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Business.FinanceBusiness;
    using SiliconValley.InformationSystem.Business.EmpSalaryManagementBusiness;
    using SiliconValley.InformationSystem.Business.DormitoryBusiness;
    using NPOI.SS.UserModel;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using NPOI.XSSF.UserModel;
    using SiliconValley.InformationSystem.Entity.ViewEntity;
    using SiliconValley.InformationSystem.Business.EmpTransactionBusiness;
    using SiliconValley.InformationSystem.Business.Base_SysManage;
    using SiliconValley.InformationSystem.Entity;
    /// <summary>
    /// 员工业务类
    /// </summary>
    public class EmployeesInfoManage : BaseBusiness<EmployeesInfo>
    {
        RedisCache rc;
        /// <summary>
        /// 将员工信息表数据存储到redis服务器中 
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetEmpInfoData()
        {
            rc = new RedisCache();
            rc.RemoveCache("InRedisEmpInfoData");
            List<EmployeesInfo> emplist = new List<EmployeesInfo>();
            if (emplist == null || emplist.Count() == 0)
            {
                emplist = this.GetIQueryable().ToList();
                rc.SetCache("InRedisEmpInfoData", emplist);
            }
            emplist = rc.GetCache<List<EmployeesInfo>>("InRedisEmpInfoData");
            return emplist;
        }

        #region 获取部门或岗位的方法

        /// <summary>
        ///  获取所属岗位对象
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
        /// 根据员工编号获取所属岗位对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public Position GetPositionByEmpid(string empid)
        {
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            PositionManage pmanage = new PositionManage();
            var pstr = pmanage.GetEntity(emanage.GetEntity(empid).PositionId);
            return pstr;
        }

        /// <summary>
        /// 根据员工编号获取所属部门对象
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public Department GetDeptByEmpid(string empid)
        {
            EmployeesInfoManage emanage = new EmployeesInfoManage();
            DepartmentManage dmanage = new DepartmentManage();
            var dstr = dmanage.GetEntity(GetPositionByEmpid(empid).DeptId);
            return dstr;
        }

        /// <summary>
        /// 获取所属岗位的所属部门对象
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Department GetDept(int pid)
        {
            DepartmentManage deptmanage = new DepartmentManage();
            var str = deptmanage.GetEntity(GetPosition(pid).DeptId);
            return str;
        }

        /// <summary>
        /// 根据部门编号获取部门对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Department GetDeptById(int id)
        {
            DepartmentManage deptmanage = new DepartmentManage();
            var dept = deptmanage.GetEntity(id);
            return dept;
        }

        /// <summary>
        /// 根据岗位编号获取岗位对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Position GetPobjById(int id)
        {
            PositionManage pmanage = new PositionManage();
            return pmanage.GetEntity(id);
        }

        //根据岗位编号获取该岗位的员工
        public List<EmployeesInfo> GetEmpByPid(int pid)
        {
          
              List<EmployeesInfo> emplist=  this.GetEmpInfoData().Where(s => s.PositionId == pid && s.IsDel == false).ToList();
             
            return emplist;
        }

        /// <summary>
        /// 通过岗位编号获取该岗位所属部门
        /// </summary>
        /// <returns></returns>
        public Department GetDeptByPid(int pid)
        {
            PositionManage pmanage = new PositionManage();
            DepartmentManage dmanage = new DepartmentManage();
            var deptid = pmanage.GetEntity(pid);
            return dmanage.GetEntity(deptid.DeptId);
        }

        /// <summary>
        /// 根据部门的名称获取部门对象
        /// </summary>
        /// <param name="dname"></param>
        /// <returns></returns>
        public Department GetDeptByDname(string dname)
        {
            DepartmentManage dmanage = new DepartmentManage();
            var dept = dmanage.GetDepartmentByName(dname);
            return dept;
        }

        /// <summary>
        /// 获取指定部门某岗位对象
        /// </summary>
        /// <param name="deptid"></param>
        /// <param name="pname"></param>
        /// <returns></returns>
        public Position GetPositionByDeptidPname(int deptid, string pname)
        {
            PositionManage pmanage = new PositionManage();
            var plist = pmanage.GetPositionByDepeID(deptid);
            var position = plist.Where(s => s.PositionName == pname).FirstOrDefault();
            return position;
        }

        #endregion      

        /// <summary>
        /// 根据部门编号获取该部门下的所有员工
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetEmpsByDeptid(int deptid)
        {
            return this.GetAll().Where(a => this.GetDeptByPid(a.PositionId).DeptId == deptid).ToList();
        }


        /// <summary>
        /// 筛选出部门为教质，教学，信息部
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetEmpByDeptName()
        {
            return this.GetAll().Where(s =>
            this.GetDeptByPid(s.PositionId).DeptName.Contains("教学部") ||
            this.GetDeptByPid(s.PositionId).DeptName.Contains("教质部") ||
            this.GetDeptByPid(s.PositionId).DeptName.Contains("信息部")
            ).ToList();
        }

        /// <summary>
        /// 渠道
        /// </summary>
        private ChannelStaffBusiness dbchannel;
        /// <summary>
        /// 获取教学教致及信息部的员工
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> Getguigu()
        {
            return this.GetAll().Where(a => this.GetDeptByPid(a.PositionId).DeptId == 5 || this.GetDeptByPid(a.PositionId).DeptId == 6 || this.GetDeptByPid(a.PositionId).DeptId == 7 || this.GetDeptByPid(a.PositionId).DeptId == 1008 || this.GetDeptByPid(a.PositionId).DeptId == 1009).ToList();
        }
        /// <summary>
        /// 获取所有没有离职的员工
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetAll()
        {
            return this.GetIQueryable().Where(a => a.IsDel == false).ToList();
        }

        /// <summary>
        /// 根据员工编号获取员工对象
        /// </summary>
        /// <param name="empinfoid">员工编号</param>
        /// <returns></returns>
        public EmployeesInfo GetInfoByEmpID(string empinfoid)
        {
            return this.GetEntity(empinfoid);
        }

        /// <summary>
        /// 添加员工借资
        /// </summary>
        /// <param name="debit"></param>
        /// <returns></returns>
        public bool Borrowmoney(Debit debit)
        {
            BaseBusiness<Debit> dbdebit = new BaseBusiness<Debit>();
            bool result = false;
            try
            {
                dbdebit.Insert(debit);
                result = true;
            }
            catch (Exception)
            {

            }
            return result;
        }


        /// <summary>
        /// 查询是市场主任员工集合
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetChannelStaffZhuren()
        {
            return this.GetAll().Where(a => this.GetPositionByEmpid(a.EmployeeId).PositionName == "市场主任").ToList();

        }

        /// <summary>
        /// 获取市场副主任
        /// </summary>
        /// <returns></returns>
        public List<EmployeesInfo> GetChannelStaffFuzhuren()
        {
            return this.GetAll().Where(a => this.GetPositionByEmpid(a.EmployeeId).PositionName == "市场副主任").ToList();
        }

        /// <summary>
        /// 根据渠道员工id获取员工对象
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public EmployeesInfo GetInfoByChannelID(int ChannelID)
        {
            dbchannel = new ChannelStaffBusiness();
            var channel = dbchannel.GetChannelByID(ChannelID);
            return this.GetInfoByEmpID(channel.EmployeesInfomation_Id);
        }
        /// <summary>
        /// 杨校(常务副校长岗位)
        /// </summary>
        /// <returns></returns>
        public EmployeesInfo GetYangxiao()
        {
            return this.GetAll().Where(a => this.GetPositionByEmpid(a.EmployeeId).PositionName == "常务副校长").FirstOrDefault();
        }
        /// <summary>
        /// 判断是否是渠道主任
        /// </summary>
        /// <param name="empinfoid">员工编号</param>
        /// <returns></returns>
        public bool IsChannelZhuren(string empinfoid)
        {
            bool iszhuren = false;
            var data = this.GetChannelStaffZhuren();
            foreach (var item in data)
            {
                if (item.EmployeeId == empinfoid)
                {
                    iszhuren = true;
                }
            }
            return iszhuren;
        }
        /// <summary>
        /// 判断是不是主任
        /// </summary>
        /// <param name="empinfo"></param>
        /// <returns></returns>
        public bool IsChannelZhuren(EmployeesInfo empinfo)
        {
            bool iszhuren = false;
            var query = this.GetChannelStaffZhuren().Where(a => a.EmployeeId == empinfo.EmployeeId).FirstOrDefault();
            if (query != null)
            {
                iszhuren = true;
            }
            return iszhuren;
        }

        /// <summary>
        /// 判断是不是副主任
        /// </summary>
        /// <param name="empinfo"></param>
        /// <returns></returns>
        public bool IsFuzhiren(EmployeesInfo empinfo)
        {
            bool isfuzhuren = false;
            var query = this.GetChannelStaffFuzhuren().Where(a => a.EmployeeId == empinfo.EmployeeId).FirstOrDefault();
            if (query != null)
            {
                isfuzhuren = true;
            }
            return isfuzhuren;
        }

        #region tangmin--Write
        /// <summary>
        /// 根据员工Id或者员工名称查询名称
        /// </summary>
        /// <param name="name">员工编号或员工名称</param>
        /// <param name="key">true---按编号查，false---按名称查</param>
        /// <returns></returns>
        public EmployeesInfo FindEmpData(string name, bool key)
        {
            EmployeesInfo employees = new EmployeesInfo();
            if (key)
            {
                List<EmployeesInfo> list2 = this.GetListBySql<EmployeesInfo>("select * from EmpView where EmployeeId='" + name + "'").ToList();
                employees = list2.Count > 0 ? list2[0] : null;
            }
            else
            {
                List<EmployeesInfo> list = this.GetListBySql<EmployeesInfo>("select * from EmpView where EmpName='" + name + "'").ToList();
                if (list.Count > 0)
                {
                    employees = list[0];
                }
            }
            return employees;
        }
        #endregion

        /// <summary>
        /// 判断某员工是否属于人事部的
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public bool JudgeIsHR(string empid)
        {
            var result = false;
            var emp = this.GetInfoByEmpID(empid);
            if (this.GetDeptByEmpid(empid).DeptName == "人事部")
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// 将员工加入对相应的部门
        /// </summary>
        /// <param name="emp">员工对象</param>
        /// <returns></returns>
        public bool AddEmpToCorrespondingDept(EmployeesInfo emp)
        {
            bool result = true;
            #region 给该员工创建用户账号
            Base_UserBusiness db_user = new Base_UserBusiness();
            EnCh ench = new EnCh();
            var empname = ench.convertCh(emp.EmpName);
            db_user.createAccount(empname, emp.EmployeeId);
            #endregion

            var dname = this.GetDept(emp.PositionId).DeptName;//获取该员工所属部门名称
            var pname = this.GetPosition(emp.PositionId).PositionName;//获取该员工所属岗位名称
            if (dname.Equals("就业部"))
            {
                EmploymentStaffBusiness esmanage = new EmploymentStaffBusiness();
                result = esmanage.AddEmploystaff(emp.EmployeeId);//给就业部员工表添加员工
            }
            if (dname.Equals("市场部"))
            {
                ChannelStaffBusiness csmanage = new ChannelStaffBusiness();
                result = csmanage.AddChannelStaff(emp.EmployeeId);
            }//给市场部员工表添加员工
            if ((dname.Equals("s1、s2教质部") || dname.Equals("s3教质部")) && !pname.Equals("教官"))
            {
                HeadmasterBusiness hm = new HeadmasterBusiness();
                result = hm.AddHeadmaster(emp.EmployeeId);
            }//给两个教质部员工表添加除教官外的员工
            if ((dname.Equals("s1、s2教质部") || dname.Equals("s3教质部")) && pname.Equals("教官") || dname.Equals("教导大队"))
            {
                InstructorListBusiness itmanage = new InstructorListBusiness();
                result = itmanage.AddInstructorList(emp.EmployeeId);
            }//给教官员工表添加教官
            if (pname.Equals("咨询师") || pname.Equals("咨询主任"))
            {
                ConsultTeacherManeger cmanage = new ConsultTeacherManeger();
                result = cmanage.AddConsultTeacherData(emp.EmployeeId);
            }//给咨询部员工表添加除咨询助理外的员工
            if (dname.Equals("s1、s2教学部") || dname.Equals("s3教学部") || dname.Equals("s4教学部"))//给三个教学部员工表添加员工
            {
                TeacherBusiness teamanage = new TeacherBusiness();
                Teacher tea = new Teacher();
                tea.EmployeeId = emp.EmployeeId;
                result = teamanage.AddTeacher(tea);
            }
            if (dname.Equals("财务部"))
            {
                FinanceModelBusiness fmmanage = new FinanceModelBusiness();
                result = fmmanage.AddFinancialstaff(emp.EmployeeId);
            }//给财务部员工表添加员工

            EmplSalaryEmbodyManage esemanage = new EmplSalaryEmbodyManage();
            if (!dname.Equals("教导大队"))
            {
                result = esemanage.AddEmpToEmpSalary(emp.EmployeeId);//往员工工资体系表添加员工
            }


            return result;
        }

        /// <summary>
        /// 员工离职将对应的部门员工状态改变(删除)
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public bool DelEmpToCorrespondingDept(EmployeesInfo emp)
        {
            bool result = true;
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var dname = empmanage.GetDept(emp.PositionId).DeptName;
            var pname = empmanage.GetPosition(emp.PositionId).PositionName;
            if (dname.Equals("就业部"))
            {
                EmploymentStaffBusiness esmanage = new EmploymentStaffBusiness();
                result = esmanage.DelEmploystaff(emp.EmployeeId);
            }
            if (dname.Equals("市场部"))
            {
                ChannelStaffBusiness csmanage = new ChannelStaffBusiness();
                result = csmanage.DelChannelStaff(emp.EmployeeId);
            }
            if ((dname.Equals("s1、s2教质部") || dname.Equals("s3教质部")) && !pname.Equals("教官"))
            {
                HeadmasterBusiness hmmanage = new HeadmasterBusiness();
                result = hmmanage.removeHeadmaster(emp.EmployeeId);
            }
            if ((dname.Equals("s1、s2教质部") || dname.Equals("s3教质部")) && pname.Equals("教官") || dname.Equals("教导大队"))
            {
                InstructorListBusiness itmanage = new InstructorListBusiness();
                result = itmanage.RemoveInstructorList(emp.EmployeeId);
            }
            if (pname.Equals("咨询师") || pname.Equals("咨询主任"))
            {
                ConsultTeacherManeger cmanage = new ConsultTeacherManeger();
                result = cmanage.DeltConsultTeacher(emp.EmployeeId);
            }
            if (dname.Equals("s1、s2教学部") || dname.Equals("s3教学部") || dname.Equals("s4教学部"))
            {
                TeacherBusiness teamanage = new TeacherBusiness();
                result = teamanage.dimission(emp.EmployeeId);
            }
            if (dname.Equals("财务部"))
            {
                FinanceModelBusiness fmmanage = new FinanceModelBusiness();
                result = fmmanage.UpdateFinancialstaff(emp.EmployeeId);
            }
            return result;
        }

        /// <summary>
        /// 员工离职
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        public AjaxResult DelEmp(EmployeesInfo emp)
        {
            var ajaxresult = new AjaxResult();
            try
            {
                emp.IsDel = true;
                this.Update(emp);
                rc.RemoveCache("InRedisEmpInfoData");
                ajaxresult = this.Success();
            }
            catch (Exception ex)
            {
                ajaxresult = this.Error(ex.Message);
            }

            if (ajaxresult.Success)
            {
                ajaxresult.Success = this.DelEmpToCorrespondingDept(emp);//将对应的部门员工表状态也改变
            }
            if (ajaxresult.Success)
            {
                Base_UserBusiness user = new Base_UserBusiness();
                ajaxresult = user.Change(emp.EmployeeId, false);//将该员工的账号禁用且密码改为后台设置的默认密码
            }
            return ajaxresult;
        }

        #region 员工数据批量导入和导出
        /// <summary>
        /// 将导过来的excel数据赋给员工视图类中
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public List<EmployeeInfoView> CreateExcelData(ISheet sheet)
        {
            List<EmployeeInfoView> result = new List<EmployeeInfoView>();
            int num = 0;
            AjaxResult ajaxresult = new AjaxResult();
            try
            {
                while (true)
                {
                    EmployeeInfoView empview = new EmployeeInfoView();
                    num++;
                    //循环获取num行的数据
                    var getrow = sheet.GetRow(num);
                    if (string.IsNullOrEmpty(Convert.ToString(getrow)))
                    {
                        break;
                    }
                    #region 获取excel中的每一列数据
                    //获取第num行"序号"列的数据
                    string excelid = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(0))) ? null : getrow.GetCell(0).ToString();
                    //获取第num行"姓名"列的数据(必填)
                    string name = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(1))) ? null : getrow.GetCell(1).ToString();
                    //获取第num行"部门"列的数据(必填)
                    string dept = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(2))) ? null : getrow.GetCell(2).ToString();
                    //获取第num行"岗位"列的数据(必填)
                    string position = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(3))) ? null : getrow.GetCell(3).ToString();
                    //获取第num行"工号"列的数据(必填)
                    string ddid = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(4))) ? null : getrow.GetCell(4).ToString();
                    //获取第num行"招聘来源"列的数据
                    string original = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(5))) ? null : getrow.GetCell(5).ToString();
                    //获取第num行"身份证号码"列的数据(必填)
                    // var idnum = System.Text.RegularExpressions.Regex.Replace(getrow.GetCell(6).StringCellValue, @"[^0-9]+", "");
                    string idcardnum = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(6))) ? null : getrow.GetCell(6).ToString();
                    //获取第num行"电话号码"列的数据
                    string phonenum = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(7))) ? null : getrow.GetCell(7).ToString();
                    //获取第num行"性别"列的数据
                    string empsex = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(8))) ? "女" : getrow.GetCell(8).ToString();
                    //获取第num行"年龄"列的数据
                    string empage = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(9))) ? null : getrow.GetCell(9).ToString();
                    //获取第num行"民族"列的数据
                    string nation = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(10))) ? "汉" : getrow.GetCell(10).ToString();
                    //获取第num行"入职时间"列的数据(必填)
                    string entertime = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(11))) ? null : getrow.GetCell(11).ToString();
                    //获取第num行"转正时间"列的数据
                    string positivetime = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(12))) ? null : getrow.GetCell(12).ToString();
                    //  string positivetime = getrow.GetCell(12)?.StringCellValue ?? null;
                    //获取第num行"试用期工资"列的数据
                    string probationsalary = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(13))) ? null : getrow.GetCell(13).ToString();
                    //获取第num行"转正后工资"列的数据（必填）
                    string salary = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(14))) ? null : getrow.GetCell(14).ToString();
                    //获取第num行"学历"列的数据
                    string education = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(15))) ? "大专" : getrow.GetCell(15).ToString();
                    //获取第num行"合同起始日期"列的数据
                    string contractStartTime = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(17))) ? null : getrow.GetCell(17).ToString();
                    //获取第num行"合同终止日期"列的数据
                    string contractEndTime = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(18))) ? null : getrow.GetCell(18).ToString();
                    //获取第num行"生日"列的数据
                    string birthday = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(19))) ? null : getrow.GetCell(19).ToString();
                    //获取第num行"紧急联系电话"列的数据
                    string urgentphone = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(20))) ? null : getrow.GetCell(20).ToString();
                    //获取第num行"户籍地址"列的数据
                    string domicileAddress = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(21))) ? null : getrow.GetCell(21).ToString();
                    //获取第num行"现地址"列的数据
                    string address = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(22))) ? null : getrow.GetCell(22).ToString();
                    //获取第num行"婚姻状况"列的数据
                    string maritalStatus = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(23))) ? "未婚" : getrow.GetCell(23).ToString();
                    //获取第num行"身份证有效期"列的数据
                    string idcardIndate = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(24))) ? null : getrow.GetCell(24).ToString();
                    //获取第num行"政治面貌"列的数据
                    string politicsStatus = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(25))) ? "党员" : getrow.GetCell(25).ToString();
                    //获取第num行"社保起始月份"列的数据
                    string SSstartTime = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(26))) ? null : getrow.GetCell(26).ToString();
                    //获取第num行"银行卡号"列的数据
                    string bankCardnum = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(27))) ? null : getrow.GetCell(27).ToString();
                    //获取第num行"纸质材料"列的数据
                    string paperyMaterial = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(28))) ? null : getrow.GetCell(28).ToString();
                    // ICell leaveddays_cell = sheet.GetRow(num).GetCell(3);
                    //获取第num行"备注"列的数据
                    string remark = string.IsNullOrEmpty(Convert.ToString(getrow.GetCell(29))) ? null : getrow.GetCell(29).ToString();
                    #endregion
                    #region 将excel中拿过来的数据赋给员工视图对象
                    empview.excelid = excelid;
                    empview.name = name;
                    empview.dept = dept;
                    empview.position = position;
                    empview.ddid = string.IsNullOrEmpty(ddid) ? null : ddid;
                    empview.original = original;
                    empview.idcardnum = idcardnum;
                    empview.phonenum = phonenum;
                    empview.empsex = empsex;
                    empview.nation = nation;
                    if (!string.IsNullOrEmpty(entertime))
                    {
                        empview.entertime = Convert.ToDateTime(entertime);
                    }
                    if (!string.IsNullOrEmpty(positivetime))
                    {
                        empview.positivetime = Convert.ToDateTime(positivetime);
                    }
                    if (!string.IsNullOrEmpty(probationsalary))
                    {
                        empview.probationsalary = Convert.ToDecimal(probationsalary);
                    }
                    if (!string.IsNullOrEmpty(salary))
                    {
                        empview.salary = Convert.ToDecimal(salary);
                    }
                    empview.education = education;
                    if (!string.IsNullOrEmpty(contractStartTime))
                    {
                        empview.contractStartTime = Convert.ToDateTime(contractStartTime);
                    }
                    if (!string.IsNullOrEmpty(contractEndTime))
                    {
                        empview.contractEndTime = Convert.ToDateTime(contractEndTime);
                    }
                    empview.birthday = birthday;
                    empview.urgentphone = urgentphone;
                    empview.domicileAddress = domicileAddress;
                    empview.address = address;
                    empview.maritalStatus = maritalStatus == "已婚" ? true : false;
                    empview.idcardIndate = Convert.ToDateTime(idcardIndate);
                    empview.politicsStatus = politicsStatus;
                    if (!string.IsNullOrEmpty(SSstartTime) && !SSstartTime.Equals("/"))
                    {
                        // DateTime.ParseExact(str, "yyyyMMdd", null);
                        DateTime stime = DateTime.ParseExact(SSstartTime, "yyyyMM", null);
                        empview.SSstartTime = stime;
                    }
                    empview.bankCardnum = bankCardnum;
                    empview.paperyMaterial = paperyMaterial;
                    empview.Remark = remark;
                    #endregion
                    result.Add(empview);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;

        }

        /// <summary>
        /// 将excel数据类的数据存入到数据库的员工表中
        /// </summary>
        /// <returns></returns>
        public AjaxResult ExcelImportEmpSql(ISheet sheet)
        {
            var ajaxresult = new AjaxResult();
            List<EmpErrorDataView> emperrorlist = new List<EmpErrorDataView>();
            try
            {
                var mateviewlist = CreateExcelData(sheet);
                foreach (var item in mateviewlist)
                {
                    EmployeesInfo emp = new EmployeesInfo();
                    EmpErrorDataView emperror = new EmpErrorDataView();
                    var deptobj = GetDeptByDname(item.dept);
                    if (string.IsNullOrEmpty(item.name))
                    {//判断员工名称是否为空
                        emperror.excelId = item.excelid;
                        emperror.errorExplain = "原因是该员工名称为空！";
                        emperrorlist.Add(emperror);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(item.dept))//判断员工部门是否为空
                        {
                            emperror.excelId = item.excelid;
                            emperror.errorExplain = "原因是该员工部门为空！";
                            emperrorlist.Add(emperror);
                        }
                        else
                        {
                            if (deptobj == null)//判断员工部门在信息系统中是否存在
                            {
                                emperror.excelId = item.excelid;
                                emperror.errorExplain = "原因是不存在" + item.dept + "该部门!";
                                emperrorlist.Add(emperror);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(item.position))//判断员工岗位是否为空
                                {
                                    emperror.excelId = item.excelid;
                                    emperror.errorExplain = "原因是该员工岗位为空!";
                                    emperrorlist.Add(emperror);
                                }
                                else
                                {
                                    //判断员工岗位在信息系统中是否存在
                                    if (deptobj != null && GetPositionByDeptidPname(deptobj.DeptId, item.position) == null)
                                    {
                                        emperror.errorExplain = "原因是" + item.dept + "中不存在" + item.position + "该岗位!";
                                        emperror.excelId = item.excelid;
                                        emperrorlist.Add(emperror);
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(Convert.ToString(item.ddid)))
                                        {//判断员工工号（钉钉号）是否为空
                                            emperror.excelId = item.excelid;
                                            emperror.errorExplain = "原因是该员工工号为空！";
                                            emperrorlist.Add(emperror);
                                        }
                                        else
                                        {
                                            if (DDidIsExist(int.Parse(item.ddid)) == true)//判断员工工号是否已存在（唯一）
                                            {
                                                emperror.excelId = item.excelid;
                                                emperror.errorExplain = "原因是该钉钉号已存在！";
                                                emperrorlist.Add(emperror);
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(item.idcardnum))//判断员工身份证是否为空
                                                {
                                                    emperror.excelId = item.excelid;
                                                    emperror.errorExplain = "原因是该员工身份证为空！";
                                                    emperrorlist.Add(emperror);
                                                }
                                                else
                                                {
                                                    if (string.IsNullOrEmpty(item.entertime.ToString()))//判断员工入职时间是否为空
                                                    {
                                                        emperror.excelId = item.excelid;
                                                        emperror.errorExplain = "原因是该员工入职时间为空！";
                                                        emperrorlist.Add(emperror);
                                                    }
                                                    else
                                                    {
                                                        #region 没有任何错误的数据就加入员工信息表
                                                        emp.EmployeeId = EmpId();
                                                        emp.DDAppId = int.Parse(item.ddid);
                                                        emp.EmpName = item.name;
                                                        emp.PositionId = GetPositionByDeptidPname(deptobj.DeptId, item.position).Pid;
                                                        emp.Sex = item.empsex;
                                                        emp.IdCardIndate = item.idcardIndate;

                                                        emp.IdCardNum = item.idcardnum;
                                                        if (emp.IdCardNum != null)
                                                        {
                                                            emp.Birthdate = DateTime.Parse(GetBirth(emp.IdCardNum));
                                                        }
                                                        if (emp.Birthdate != null)
                                                        {
                                                            emp.Age = Convert.ToInt32(this.GetAge((DateTime)emp.Birthdate, DateTime.Now));
                                                        }
                                                        emp.Nation = item.nation;
                                                        emp.Phone = item.phonenum;
                                                        emp.ContractStartTime = item.contractStartTime;
                                                        emp.ContractEndTime = item.contractEndTime;
                                                        emp.EntryTime = (DateTime)item.entertime;
                                                        emp.Birthday = item.birthday;
                                                        emp.PositiveDate = item.positivetime;
                                                        emp.UrgentPhone = item.urgentphone;
                                                        emp.DomicileAddress = item.domicileAddress;
                                                        emp.Address = item.address;
                                                        emp.Education = item.education;
                                                        emp.MaritalStatus = item.maritalStatus;
                                                        emp.IdCardIndate = item.idcardIndate;
                                                        emp.PoliticsStatus = item.politicsStatus;
                                                        emp.ProbationSalary = item.probationsalary;
                                                        emp.Salary = item.salary;
                                                        emp.SSStartMonth = item.SSstartTime;
                                                        emp.BCNum = item.bankCardnum;
                                                        emp.Material = item.paperyMaterial;
                                                        emp.Remark = item.Remark;
                                                        emp.InvitedSource = item.original;
                                                        emp.IsDel = false;
                                                        emp.Image = "guigu.jpg";
                                                        this.Insert(emp);
                                                        rc.RemoveCache("InRedisEmpInfoData");
                                                        AddEmpToCorrespondingDept(emp);
                                                        #endregion
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
                if (mateviewlist.Count() - emperrorlist.Count() == mateviewlist.Count())
                {//说明没有出错数据，导入的数据全部添加成功
                    ajaxresult.Success = true;
                    ajaxresult.ErrorCode = 100;
                    ajaxresult.Msg = mateviewlist.Count().ToString();
                    ajaxresult.Data = emperrorlist;
                }
                else
                {//说明有出错数据，导入的数据条数就是导入的数据总数-错误数据总数
                    ajaxresult.Success = true;
                    ajaxresult.ErrorCode = 200;
                    ajaxresult.Msg = (mateviewlist.Count() - emperrorlist.Count()).ToString();
                    ajaxresult.Data = emperrorlist;
                }
            }
            catch (Exception ex)
            {
                ajaxresult.Success = false;
                ajaxresult.ErrorCode = 500;
                ajaxresult.Msg = ex.Message;
                ajaxresult.Data = 0;
            }
            return ajaxresult;
        }

        public AjaxResult ImportDataFormExcel(Stream stream, string contentType)
        {
            var ajaxresult = new AjaxResult();
            IWorkbook workbook = null;

            if (contentType == "application/vnd.ms-excel")
            {
                workbook = new HSSFWorkbook(stream);
            }

            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = workbook.GetSheetAt(0);
            var result = ExcelImportEmpSql(sheet);
            stream.Close();
            stream.Dispose();
            workbook.Close();

            return result;
        }

        //保存员工数据
        public AjaxResult EmpDataToExcel(List<EmployeesInfo> data, string filename)
        {//SaveStaff_CostData
            var ajaxresult = new AjaxResult();

            var workbook = new HSSFWorkbook();

            //创建工作区
            var sheet = workbook.CreateSheet();

            #region 表头样式

            HSSFCellStyle HeadercellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont HeadercellFont = (HSSFFont)workbook.CreateFont();

            HeadercellStyle.Alignment = HorizontalAlignment.Center;
            HeadercellFont.IsBold = true;

            HeadercellStyle.SetFont(HeadercellFont);

            #endregion


            HSSFCellStyle ContentcellStyle = (HSSFCellStyle)workbook.CreateCellStyle();
            HSSFFont ContentcellFont = (HSSFFont)workbook.CreateFont();

            ContentcellStyle.Alignment = HorizontalAlignment.Center;

            CreateHeader();

            int num = 1;

            //  CourseBusiness dbcourse = new CourseBusiness();

            GrandBusiness dbgrand = new GrandBusiness();

            data.ForEach(d =>
            {
                var row = (HSSFRow)sheet.CreateRow(num);

                CreateCell(row, ContentcellStyle, 0, d.EmployeeId);//员工编号
                CreateCell(row, ContentcellStyle, 1, d.DDAppId.ToString());//钉钉号

                CreateCell(row, ContentcellStyle, 2, d.EmpName);//员工名称
                CreateCell(row, ContentcellStyle, 3, d.Sex);//性别
                CreateCell(row, ContentcellStyle, 4, this.GetDeptByPid(d.PositionId).DeptName);//部门名称
                CreateCell(row, ContentcellStyle, 5, this.GetPobjById(d.PositionId).PositionName);//岗位名称

                CreateCell(row, ContentcellStyle, 6, d.Age.ToString());//年龄
                CreateCell(row, ContentcellStyle, 7, d.Nation);//民族
                CreateCell(row, ContentcellStyle, 8, d.Phone);//电话号码
                CreateCell(row, ContentcellStyle, 9, d.IdCardNum);//身份证号
                CreateCell(row, ContentcellStyle, 10, d.EntryTime.ToString());//入职时间
                CreateCell(row, ContentcellStyle, 11, d.PositiveDate.ToString());//转正时间

                CreateCell(row, ContentcellStyle, 12, d.ContractStartTime.ToString());//合同起始时间
                CreateCell(row, ContentcellStyle, 13, d.ContractEndTime.ToString());//合同终止时间
                CreateCell(row, ContentcellStyle, 14, d.Birthdate.ToString());//出生日期
                CreateCell(row, ContentcellStyle, 15, d.Birthday);//生日
                CreateCell(row, ContentcellStyle, 16, d.ContractStartTime.ToString());//紧急联系电话
                CreateCell(row, ContentcellStyle, 17, d.DomicileAddress);//户籍地址
                CreateCell(row, ContentcellStyle, 18, d.Address);//现居地址
                CreateCell(row, ContentcellStyle, 19, d.Education);//学历
                CreateCell(row, ContentcellStyle, 20, d.MaritalStatus == true ? "已婚" : "未婚");//婚姻状态
                CreateCell(row, ContentcellStyle, 21, d.IdCardIndate.ToString());//身份证有效期
                CreateCell(row, ContentcellStyle, 22, d.PoliticsStatus);//政治面貌
                CreateCell(row, ContentcellStyle, 23, d.InvitedSource);//招聘来源
                CreateCell(row, ContentcellStyle, 24, d.ProbationSalary.ToString());//试用期工资
                CreateCell(row, ContentcellStyle, 25, d.Salary.ToString());//转正后工资
                CreateCell(row, ContentcellStyle, 26, d.SSStartMonth.ToString());//社保起始月份
                CreateCell(row, ContentcellStyle, 27, d.BCNum);//银行卡号
                CreateCell(row, ContentcellStyle, 28, d.Material);//材料
                CreateCell(row, ContentcellStyle, 29, d.Remark);//备注
                CreateCell(row, ContentcellStyle, 30, d.IsDel == false ? "在职" : "离职");//员工状态

                num++;

            });

            string path = System.AppDomain.CurrentDomain.BaseDirectory.Split('\\')[0];    //获得项目的基目录
            //var s = path.Split('\\'); 
            //var mypath = s[0];
            var Path = System.IO.Path.Combine(path, @"\XinxihuaData\Excel"); //进到基目录录找“Uploadss->Excel”文件夹

            if (!System.IO.Directory.Exists(Path))     //判断是否有该文件夹
                System.IO.Directory.CreateDirectory(Path); //如果没有在Uploads文件夹下创建文件夹Excel
            string saveFileName = Path + "\\" + "员工信息" + ".xlsx"; //路径+表名+文件类型
            //var path = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            //string saveFileName = path + "\\XinxihuaData\\信息表.xlsx";
            //if (!System.IO.Directory.Exists(saveFileName))
            //{
            //    System.IO.Directory.CreateDirectory(saveFileName);
            //}
            try
            {
                FileStream fs = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
                workbook.Write(fs);  //写入文件
                workbook.Close();  //关闭
                ajaxresult.ErrorCode = 200;
                ajaxresult.Msg = "导入成功！文件地址：" + saveFileName;
                // ajaxresult.Data = list;

            }
            catch (Exception ex)
            {
                ajaxresult.ErrorCode = 100;
                ajaxresult.Msg = "导入失败，" + ex.Message;

            }
            return ajaxresult;
            //workbook.Close();


            void CreateHeader()
            {
                HSSFRow Header = (HSSFRow)sheet.CreateRow(0);
                Header.HeightInPoints = 40;

                CreateCell(Header, HeadercellStyle, 0, "员工编号");

                CreateCell(Header, HeadercellStyle, 1, "钉钉号");

                CreateCell(Header, HeadercellStyle, 2, "姓名");

                CreateCell(Header, HeadercellStyle, 3, "性别");

                CreateCell(Header, HeadercellStyle, 4, "所属部门");

                CreateCell(Header, HeadercellStyle, 5, "所属岗位");

                CreateCell(Header, HeadercellStyle, 6, "年龄");

                CreateCell(Header, HeadercellStyle, 7, "民族");

                CreateCell(Header, HeadercellStyle, 8, "电话号码");

                CreateCell(Header, HeadercellStyle, 9, "身份证号码");

                CreateCell(Header, HeadercellStyle, 10, "入职时间");

                CreateCell(Header, HeadercellStyle, 11, "转正时间");

                CreateCell(Header, HeadercellStyle, 12, "合同起始时间");

                CreateCell(Header, HeadercellStyle, 13, "合同终止时间");

                CreateCell(Header, HeadercellStyle, 14, "出生日期");

                CreateCell(Header, HeadercellStyle, 15, "生日");

                CreateCell(Header, HeadercellStyle, 16, "紧急联系电话");

                CreateCell(Header, HeadercellStyle, 17, "户籍地址");

                CreateCell(Header, HeadercellStyle, 18, "现居地址");

                CreateCell(Header, HeadercellStyle, 19, "学历");
                CreateCell(Header, HeadercellStyle, 20, "婚姻状态");
                CreateCell(Header, HeadercellStyle, 21, "身份证有效期");
                CreateCell(Header, HeadercellStyle, 22, "政治面貌");
                CreateCell(Header, HeadercellStyle, 23, "招聘来源");
                CreateCell(Header, HeadercellStyle, 24, "试用期工资");
                CreateCell(Header, HeadercellStyle, 25, "转正后工资");
                CreateCell(Header, HeadercellStyle, 26, "社保起始月份");
                CreateCell(Header, HeadercellStyle, 27, "银行卡号");
                CreateCell(Header, HeadercellStyle, 28, "材料");
                CreateCell(Header, HeadercellStyle, 29, "备注");
                CreateCell(Header, HeadercellStyle, 30, "员工状态");

            }

            void CreateCell(HSSFRow row, HSSFCellStyle TcellStyle, int index, string value)
            {
                HSSFCell Header_Name = (HSSFCell)row.CreateCell(index);

                Header_Name.SetCellValue(value);

                Header_Name.CellStyle = TcellStyle;
            }

        }
        #endregion

        /// <summary>
        ///计算员工年龄
        /// </summary>
        /// <param name="dtBirthday"></param>
        /// <param name="dtNow"></param>
        /// <returns></returns>
        public string GetAge(DateTime dtBirthday, DateTime dtNow)
        {
            string strAge = string.Empty; // 年龄的字符串表示
            int intYear = 0; // 岁
            int intMonth = 0; // 月
            int intDay = 0; // 天

            // 如果没有设定出生日期, 返回空
            if (dtBirthday == null)
            {
                return string.Empty;
            }

            // 计算天数
            intDay = dtNow.Day - dtBirthday.Day;
            if (intDay < 0)
            {
                dtNow = dtNow.AddMonths(-1);
                intDay += DateTime.DaysInMonth(dtNow.Year, dtNow.Month);
            }

            // 计算月数
            intMonth = dtNow.Month - dtBirthday.Month;
            if (intMonth < 0)
            {
                intMonth += 12;
                dtNow = dtNow.AddYears(-1);
            }

            // 计算年数
            intYear = dtNow.Year - dtBirthday.Year;

            // 格式化年龄输出
            if (intYear >= 1) // 年份输出
            {
                strAge = intYear.ToString();
            }

            //if (intMonth > 0 && intYear <= 5) // 五岁以下可以输出月数
            //{
            //    strAge += intMonth.ToString() + "月";
            //}

            //if (intDay >= 0 && intYear < 1) // 一岁以下可以输出天数
            //{
            //    if (strAge.Length == 0 || intDay > 0)
            //    {
            //        strAge += intDay.ToString() + "日";
            //    }
            //}

            return strAge;
        }
        #region 员工编号生成相关
        /// <summary>
        ///生成员工编号
        /// </summary>
        /// <returns></returns>
        public string EmpId()
        {
            string mingci = string.Empty;
            // DateTime date = Convert.ToDateTime(Date());
            DateTime date = DateTime.Now;
            string n = date.Year.ToString();//获取年份
            string y = MonthAndDay(Convert.ToInt32(date.Month)).ToString();//获取月份
            string d = MonthAndDay(Convert.ToInt32(date.Day)).ToString();//获取日期

            EmployeesInfoManage empinfo = new EmployeesInfoManage();
            var lastobj = empinfo.GetEmpInfoData().LastOrDefault();
            if (lastobj == null)
            {
                mingci = "0001";
            }
            else
            {
                string laststr = lastobj.EmployeeId;
                string startfournum = laststr.Substring(0, 4);
                string endfournum = laststr.Substring(laststr.Length - 4, 4);
                if (int.Parse(n) > int.Parse(startfournum))
                {
                    mingci = "0001";
                }
                else
                {
                    string newstr = (int.Parse(endfournum) + 1).ToString();
                    if (int.Parse(newstr) < 10)
                    {
                        mingci = "000" + newstr;
                    }
                    else if (int.Parse(newstr) >= 10 && int.Parse(newstr) < 100)
                    {
                        mingci = "00" + newstr;
                    }
                    else if (int.Parse(newstr) >= 100 && int.Parse(newstr) < 1000)
                    {
                        mingci = "0" + newstr;
                    }
                    else
                    {
                        mingci = newstr;
                    }
                }
            }
            string EmpidResult = n + y + d + mingci;
            return EmpidResult;
        }
        //月份及日期前面加个零
        public string MonthAndDay(int a)
        {
            if (a < 10)
            {
                return "0" + a;
            }
            string c = a.ToString();
            return c;
        }
        /// <summary>
        /// 根据身份证号码获取出生日期
        /// </summary>
        /// <param name="idnum"></param>
        /// <returns></returns>
        public string GetBirth(string idnum)
        {
            string year = idnum.Substring(6, 4);
            string month = idnum.Substring(10, 2);
            string date = idnum.Substring(12, 2);
            string result = year + "-" + month + "-" + date;
            return result;
        }
        #endregion

        /// <summary>
        /// 判断某钉钉号是否已存在
        /// </summary>
        /// <param name="ddid"></param>
        /// <returns></returns>
        public bool DDidIsExist(int ddid)
        {
            bool result = false;

            var emp = this.GetEmpInfoData().Where(s => s.DDAppId == ddid).FirstOrDefault();
            if (emp != null)
            {
                result = true;//表示存在该钉钉号
            }
            else
            {
                result = false;//表示不存在该钉钉号
            }
            return result;

        }

        /// <summary>
        /// 根据类型编号获取员工异动类型对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public MoveType GetETById(int id)
        {
            MoveTypeManage mtmanage = new MoveTypeManage();
            return mtmanage.GetEntity(id);
        }

        /// <summary>
        /// 获取某员工的异动详情
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public List<EmpTransactionView> GetEmpEtrdetails(string empid)
        {
            EmpTransactionManage etrmanage = new EmpTransactionManage();
            var etrlist = etrmanage.GetList().Where(s => s.EmployeeId == empid && s.IsDel == false).ToList();
            MoveTypeManage mtmanage = new MoveTypeManage();
            List<EmpTransactionView> etrviewlist = new List<EmpTransactionView>();
            foreach (var item in etrlist)
            {
                EmpTransactionView etrview = new EmpTransactionView();
                etrview.TransactionId = item.TransactionId;
                etrview.EmployeeId = item.EmployeeId;
                etrview.TransactionTime = item.TransactionTime;
                etrview.Reason = item.Reason;
                etrview.Remark = item.Remark;
                etrview.IsDel = item.IsDel;
                etrview.TransactionType = item.TransactionType;
                etrview.PreviousDept = item.PreviousDept;
                etrview.PreviousPosition = item.PreviousPosition;
                etrview.PreviousSalary = item.PreviousSalary;
                etrview.PresentDept = item.PresentDept;
                etrview.PresentPosition = item.PresentPosition;
                etrview.PresentSalary = item.PresentSalary;
                etrview.BeforeContractStartTime = item.BeforeContractStartTime;
                etrview.BeforeContractEndTime = item.BeforeContractEndTime;
                etrview.AfterContractStartTime = item.AfterContractStartTime;
                etrview.AfterContractEndTime = item.AfterContractEndTime;

                etrview.Transactionname = mtmanage.GetEntity(item.TransactionType).MoveTypeName;
                etrview.beforedname = item.PreviousDept == null ? null : this.GetDeptById((int)item.PreviousDept).DeptName;
                etrview.beforepname = item.PreviousPosition == null ? null : this.GetPobjById((int)item.PreviousPosition).PositionName;
                etrview.afterdname = item.PresentDept == null ? null : this.GetDeptById((int)item.PresentDept).DeptName;
                etrview.afterpname = item.PresentPosition == null ? null : this.GetPobjById((int)item.PresentPosition).PositionName;
                etrview.ename = this.GetInfoByEmpID(item.EmployeeId).EmpName;
                etrview.EntryTime = this.GetInfoByEmpID(item.EmployeeId).EntryTime;
                etrviewlist.Add(etrview);
            }
            return etrviewlist;
        }

        /// <summary>
        /// 根据员工钉钉号返回员工对象
        /// </summary>
        /// <param name="ddid"></param>
        /// <returns></returns>
        public EmployeesInfo GetEmpByDDid(int ddid)
        {
            EmployeesInfoManage empmanage = new EmployeesInfoManage();
            var emp = empmanage.GetList().Where(s => s.DDAppId == ddid).FirstOrDefault();
            return emp;
        }

        /// <summary>
        /// 判断某员工的类型（1代表校长；2代表副校长（含黄主任，不含王院长）；3代表主任；4代表参与考勤的普通员工）
        /// </summary>
        /// <param name="empid"></param>
        /// <returns></returns>
        public int JudgeEmpType(string empid)
        {
            int result = 1;
            var emp = this.GetInfoByEmpID(empid);
            var dname = this.GetDeptByEmpid(empid).DeptName;
            var pname = this.GetPositionByEmpid(empid).PositionName;
            if (dname == "校办" && pname == "校长")
            {
                result = 1;
            }
            else if (dname == "校办" && (pname != "校长" || pname != "荣誉院长"))
            {
                result = 2;
            }
            else if ((pname.Contains("主任") && !pname.Contains("班主任")) || pname == "人事总监")
            {
                result = 3;
            }
            else
            {
                result = 4;
            }
            return result;
        }

    }
}
