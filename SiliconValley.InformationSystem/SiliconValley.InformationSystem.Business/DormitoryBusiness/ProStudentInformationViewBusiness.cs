using SiliconValley.InformationSystem.Business.EmployeesBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.DormitoryBusiness
{
    /// <summary>
    /// 用于晚归登记显示学生信息 业务类
    /// </summary>
    public class ProStudentInformationViewBusiness
    {
        private dbprosutdent_dbproheadmaster dbprosutdent_dbproheadmaster;
        private  ProStudentInformationBusiness dbproStudentInformationBusiness;
        private NotreturningLateBusiness dbnotreturningLateBusiness;
        private AccdationinformationBusiness dbacc;
        private DormInformationBusiness dbdorm;
        private EmployeesInfoManage dbemployeesInfoManage;
        private ProClassSchedule dbproclassSchedule;
        private ProScheduleForTrainees dbproScheduleForTrainees;
        private ProHeadClass dbproHeadClass;
        private ProHeadmaster dbproHeadmaster;
        /// <summary>
        /// 根据姓名返回学生涉及信息(晚归)
        /// </summary>
        /// <param name="name0"></param>
        /// <returns></returns>
        public List<ProStudentInformationView> GetProStudentInformationViews(string name1) {
            dbproStudentInformationBusiness = new ProStudentInformationBusiness();
            dbprosutdent_dbproheadmaster = new dbprosutdent_dbproheadmaster();
            dbacc = new AccdationinformationBusiness();
            dbnotreturningLateBusiness = new NotreturningLateBusiness();
            dbdorm = new DormInformationBusiness();
            dbemployeesInfoManage = new EmployeesInfoManage();
            var list0= dbproStudentInformationBusiness.GetStudentInSchoolData().Where(a => a.Name == name1).ToList();
            var list1 = new List<ProStudentInformationView>();
            foreach (var item in list0)
            {
                ProStudentInformationView proStudentInformationView = new ProStudentInformationView();
                proStudentInformationView.ClassNO = dbprosutdent_dbproheadmaster.GetClassScheduleByStudentNumber(item.StudentNumber).ClassNumber;
                proStudentInformationView.Count = dbnotreturningLateBusiness.GeNotreturningLatesByStudentNumber(item.StudentNumber).Count;
                var obj0 = dbacc.GetAccdationByStudentNumber(item.StudentNumber);

                if (obj0==null)
                {
                    proStudentInformationView.DormNO = string.Empty;
                }
                else
                {
                    proStudentInformationView.DormNO = dbdorm.GetEntity(obj0.DormId).DormInfoName;
                }
                
                var obj1 = dbprosutdent_dbproheadmaster.GetHeadmasterByStudentNumber(item.StudentNumber);
                var obj2= dbemployeesInfoManage.GetEntity(obj1.informatiees_Id);
                proStudentInformationView.MasterName = obj2.EmpName;
                proStudentInformationView.MasterPhone = obj2.Phone;
                proStudentInformationView.StudentNmber = item.StudentNumber;
                proStudentInformationView.StuPhone = item.Telephone;
                proStudentInformationView.SutdentName = item.Name;
                list1.Add(proStudentInformationView);
            }
            return list1;
        }

        /// <summary>
        /// 学生基础信息涉及到（调寝）
        /// </summary>
        /// <param name="param1">楼层的房间</param>
        /// <param name="name1"></param>
        /// <returns></returns>
        public List<ProBedtimeStudentsView> GetProBedtimeStudentsViews(List<DormInformation> param1, string name1) {

            dbproStudentInformationBusiness = new ProStudentInformationBusiness();
            dbacc = new AccdationinformationBusiness();
            dbdorm = new DormInformationBusiness();
            dbemployeesInfoManage = new EmployeesInfoManage();
            dbproclassSchedule = new ProClassSchedule();
            dbproScheduleForTrainees = new ProScheduleForTrainees();
            dbproHeadClass = new ProHeadClass();
            dbproHeadmaster=new ProHeadmaster();
            var list0 = new List<Accdationinformation>();
            foreach (var item in param1)
            {
                var list1= dbacc.GetAccdationinformationByDormId(item.ID);
                list0.AddRange(list1);
            }
            List<ProBedtimeStudentsView> result0 = new List<ProBedtimeStudentsView>();

            foreach (var item in list0)
            {
                ProBedtimeStudentsView proBedtimeStudentsView = new ProBedtimeStudentsView();
                var obj = dbproStudentInformationBusiness.GetEntity(item.Studentnumber);
                if (!string.IsNullOrEmpty(name1))
                {
                    if (obj.Name!=name1)
                    {
                        continue;
                    }
                }
                proBedtimeStudentsView.StudentNmber = obj.StudentNumber;
                proBedtimeStudentsView.SutdentName = obj.Name;
                proBedtimeStudentsView.Sex = (bool)obj.Sex;

                var obj0 = dbproScheduleForTrainees.GetTraineesByStudentNumber(obj.StudentNumber);
                if (obj0 != null)
                {

                    var obj1 = dbproclassSchedule.GetEntity(obj0.ID_ClassName);
                    proBedtimeStudentsView.ClassNO = obj1.ClassNumber;
                    var obj12 = dbproHeadClass.GetClassByClassid(obj1.id);
                    if (obj12 != null)
                    {
                        var obj13 = dbproHeadmaster.GetEmployeesInfoByHeadID(obj12.LeaderID);
                        if (obj13!=null)
                        {
                            proBedtimeStudentsView.EmpName = obj13.EmpName;
                        }
                        else
                        {
                            proBedtimeStudentsView.EmpName = "无";
                        }
                         
                    }
                    else
                        proBedtimeStudentsView.EmpName = string.Empty;
                }
                else
                {
                    proBedtimeStudentsView.ClassNO = string.Empty;
                    proBedtimeStudentsView.EmpName = string.Empty;
                }
                var obj2 = dbacc.GetAccdationByStudentNumber(obj.StudentNumber);
                var obj3 = dbdorm.GetEntity(obj2.DormId);
                proBedtimeStudentsView.DromName = obj3.DormInfoName;
                proBedtimeStudentsView.DromID = obj3.ID;
                result0.Add(proBedtimeStudentsView);
          
            }
            return result0;
        }
    }
}
