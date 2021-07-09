using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SiliconValley.InformationSystem.Business.ClassesBusiness;
using SiliconValley.InformationSystem.Business.ClassSchedule_Business;
using SiliconValley.InformationSystem.Business.Common;
using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
using SiliconValley.InformationSystem.Entity.MyEntity;
using SiliconValley.InformationSystem.Util;

namespace SiliconValley.InformationSystem.Business.StudentBusiness
{
    public class StudentInformationBusiness : BaseBusiness<StudentInformation>
    {
        //阶段专业表
        BaseBusiness<StageGrade> tagegrade = new BaseBusiness<StageGrade>();
        //专业
        BaseBusiness<Specialty> specia = new BaseBusiness<Specialty>();
        //学员学费标准
        BaseBusiness<StudentFeeStandard> StudentFee = new BaseBusiness<StudentFeeStandard>();
        //迟交配置
        BaseBusiness<Tuitionallocation> tuitionalloca = new BaseBusiness<Tuitionallocation>();
        //学员缴费
        BaseBusiness<StudentFeeRecord> StudeRecord = new BaseBusiness<StudentFeeRecord>();
        //学费明细
        BaseBusiness<Studenttuitionfeestandard> feestandard = new BaseBusiness<Studenttuitionfeestandard>();
        //学员班级表
        ScheduleForTraineesBusiness scheduleForTraineesBusiness = new ScheduleForTraineesBusiness();
        /// <summary>
        /// 根据选择的阶段弹出专业选项
        /// </summary>
        /// <param name="id">阶段id</param>
        /// <returns></returns>
        public object Stage(int id)
        {
            var x = tagegrade.GetList().Where(a => a.IsDelete == false && a.Grand_Id == id).Select(a => new
            {
                code = specia.GetEntity(a.Major_Id).Id,
                name = specia.GetEntity(a.Major_Id).SpecialtyName
            }).ToList();
            return x;
        }

        /// <summary>
        /// 获取当前学费
        /// </summary>
        /// <param name="Stage">阶段id</param>
        /// <param name="Major_Id">专业id</param>
        /// <returns></returns>
        public StudentFeeStandard GetFeestandard(int Stage, int Major_Id)
        {
            var x = tagegrade.GetList().Where(a => a.IsDelete == false && a.Grand_Id == Stage && a.Major_Id == Major_Id).FirstOrDefault();
            return StudentFee.GetList().Where(a => a.IsDelete == false && a.Stage == x.Id).FirstOrDefault();
        }
        /// <summary>
        /// 迟交数据显示
        /// </summary>
        /// <param name="Stage">阶段id</param>
        /// <returns></returns>
        public Tuitionallocation Latetuitionfee(int Stage)
        {
            return tuitionalloca.GetList().Where(a => a.IsDelete == false && a.Stage == Stage).FirstOrDefault();
        }



        public object Studenttuitionfeestandard(int id)
        {


            var x = feestandard.GetList().Where(a => a.IsDelete == false && a.Stage == id).Select(a => new
            {
                code = a.id,
                name = a.Unitpricename
            }).ToList();
            return x;
        }
        /// <summary>
        /// 学员照片添加，返回true为成功
        /// </summary>
        /// <param name="studenid">学号</param>
        /// <param name="imgurl">照片名称</param>
        /// <returns></returns>
        public bool StudentAddImg(string studenid, string imgurl)
        {
            bool bo = true;
            try
            {
                var x = this.GetEntity(studenid);
                x.Picture = imgurl;
                this.Update(x);
                BusHelper.WriteSysLog("修改学员信息图片", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {
                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
                bo = false;

            }
            return bo;
        }
        /// <summary>
        /// 学员身份证正反面上传
        /// </summary>
        /// <param name="studenid">学员学号</param>
        /// <param name="types">类型，1则为正面，否则为反面</param>
        /// <param name="imgname"></param>
        /// <returns></returns>
        public bool StudentIdentityImg(string studenid, int types, string imgname)
        {
            bool bo = true;
            try
            {
                var x = this.GetEntity(studenid);
                if (types == 1)
                {
                    x.Identityjustimg = imgname;
                }
                else
                {
                    x.Identitybackimg = imgname;
                }
                this.Update(x);
                BusHelper.WriteSysLog("修改学员身份证信息", Entity.Base_SysManage.EnumType.LogType.编辑数据);
            }
            catch (Exception ex)
            {

                BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.编辑数据);
                bo = false;
            }
            return bo;
        }
        /// <summary>
        /// 根据学号获取学员姓名，班级
        /// </summary>
        /// <param name="studentid">学号</param>
        /// <returns></returns>
        public object StuClass(string studentid)
        {
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            var x = this.GetEntity(studentid);
            var Enti = new
            {
                x.Name,
                x.StudentNumber,
                ClassName = scheduleForTraineesBusiness.SutdentCLassName(studentid).ClassID
            };
            return Enti;
        }
        /// <summary>
        /// 验证是否有照片了
        /// </summary>
        /// <param name="studentid">学号</param>
        /// <returns></returns>
        public int boolImg(string studentid)
        {
            if (this.GetEntity(studentid).Picture != null)
            {
                return 2;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 获取学在校学员
        /// </summary>
        /// <returns></returns>
        public List<StudentInformation> StudentList()
        {
            return this.GetList().Where(a => a.IsDelete != true && a.State == null).ToList();
        }

        //获取网络时间
        public string Date()
        {

            WebRequest request = null;
            WebResponse response = null;
            WebHeaderCollection headerCollection = null;

            string datetime = string.Empty;
            try
            {
                request = WebRequest.Create("https://www.baidu.com");
                request.Timeout = 3000;
                request.Credentials = CredentialCache.DefaultCredentials;
                response = (WebResponse)request.GetResponse();
                headerCollection = response.Headers;
                foreach (var h in headerCollection.AllKeys)
                { if (h == "Date") { datetime = headerCollection[h]; } }
                return datetime;
            }

            catch (Exception) { return datetime; }
            finally
            {
                if (request != null)
                { request.Abort(); }
                if (response != null)
                { response.Close(); }
                if (headerCollection != null)
                { headerCollection.Clear(); }
            }
        }
        //月份前面加个零
        public string Month(int a)
        {
            if (a < 10)
            {
                return "0" + a;
            }
            string c = a.ToString();
            return c;
        }
        //生成学号
        public string StudentID(string IDnumber,string gradeName)
        {
            if (gradeName=="S1")
            {
                gradeName = "2";
            }
            else if (gradeName=="S2")
            {
                gradeName="3";
            }
            else if (gradeName=="S3")
            {
                gradeName = "4";
            }
            else if (gradeName=="Y1")
            {
                gradeName = "1";
            }
            string mingci = string.Empty;
            DateTime date = Convert.ToDateTime(DateTime.Now);//Date()
            //当前年份
            string n = date.Year.ToString().Substring(2);//获取年份
            //学员总数Mylist("StudentInformation")
            var laststr = this.GetList().Where(a => Convert.ToDateTime(a.InsitDate).Year.ToString("yyyy-MM-dd").Substring(2).ToString() == n).Count() + 1;
            string sfz = IDnumber.Substring(12,6);
            string y = Month(Convert.ToInt32(date.Month)).ToString();
            // string count = Count().ToString();
            string count = laststr.ToString();
            if (count.Length < 2)
                mingci = gradeName+"000" + count;
            else if (count.Length < 3)
                mingci = gradeName+"00" + count;
            else if (count.Length < 4)
                mingci = gradeName+"0" + count;
            else if (count.Length < 5)
                mingci = gradeName + count;
            else mingci = count;
            string xuehao = n + y + sfz + mingci;
            return xuehao;
        }


        /// <summary>
        /// 以身份证查询是否有重复学员
        /// </summary>
        /// <param name="identitydocument">身份证号</param>
        /// <returns></returns>
        public bool Isidentitydocument(string identitydocument)
        {
            var x = this.GetList().Where(a => a.identitydocument == identitydocument && a.IsDelete != true).ToList();
            if (x.Count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        /// <summary>
        /// 注册学员
        /// </summary>
        /// <param name="studentInformation">学员数据对象</param>
        /// <param name="List">班级id</param>
        /// <param name="NameKeysid">备案id</param>
        /// <param name="grade_Id">阶段id</param>
        /// <returns></returns>
        public AjaxResult StudfentEnti(StudentInformation studentInformation, int List, int NameKeysid,string gradeName)
        {
            //阶段
            GrandBusiness Grandcontext = new GrandBusiness();
            //学员班级
            ClassScheduleBusiness classScheduleBusiness = new ClassScheduleBusiness();
            AjaxResult result = null;
            if (Isidentitydocument(studentInformation.identitydocument))
            {
                try
                {
                    
                    
                    studentInformation.StudentNumber = StudentID(studentInformation.identitydocument, gradeName);
                    studentInformation.InsitDate = DateTime.Now;
                    studentInformation.Password = "000000";
                    studentInformation.StudentPutOnRecord_Id = NameKeysid;
                    studentInformation.IsDelete = false;
                    //dataKeepAndRecordBusiness.ChangeStudentState(NameKeysid);
                    this.Insert(studentInformation);
                    ScheduleForTrainees scheduleForTrainees = new ScheduleForTrainees();
                    scheduleForTrainees.ClassID = classScheduleBusiness.GetEntity(List).ClassNumber;//班级名称
                    scheduleForTrainees.ID_ClassName = List;//班级编号
                    scheduleForTrainees.CurrentClass = true;
                    scheduleForTrainees.StudentID = studentInformation.StudentNumber;
                    scheduleForTrainees.AddDate = DateTime.Now;
                    scheduleForTrainees.IsGraduating = false;
                    scheduleForTraineesBusiness.Insert(scheduleForTrainees);
                    // Stuclass.Remove("ScheduleForTrainees");
                    result = new SuccessResult();
                    result.Msg = "注册成功";
                    result.Success = true;
                    result.Data = studentInformation.StudentNumber;
                    //dbtext.Remove("StudentInformation");
                    BusHelper.WriteSysLog("注册学员成功", Entity.Base_SysManage.EnumType.LogType.添加数据);

                }
                catch (Exception ex)
                {
                    result = new ErrorResult();
                    result.ErrorCode = 500;
                    result.Msg = "服务器错误1";

                    BusHelper.WriteSysLog(ex.Message, Entity.Base_SysManage.EnumType.LogType.添加数据);

                }
            }
            else
            {
                result = new SuccessResult();
                result.Success = false;
                result.Msg = "身份证重复";
            }
            return result;
        }
        /// <summary>
        /// 验证是否注册
        /// </summary>
        /// <param name="id">备案id</param>
        /// <returns></returns>
        public bool IsRegister(int id)
        {
           int count = this.GetListBySql<StudentInformation>("select * from StudentInformation where StudentPutOnRecord_Id="+id).Count;
            if (count<=0)
            {
                return false;
            }
            return true;
        }
    }
}
