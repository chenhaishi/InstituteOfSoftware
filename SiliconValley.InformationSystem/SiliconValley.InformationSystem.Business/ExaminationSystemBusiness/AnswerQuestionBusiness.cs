using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.ExaminationSystemBusiness
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using NPOI.XSSF.UserModel;
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Business.EmployeesBusiness;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
    using SiliconValley.InformationSystem.Util;
    using System.IO;

    /// <summary>
    /// 解答题库业务类
    /// </summary>
    public class AnswerQuestionBusiness:BaseBusiness<AnswerQuestionBank>
    {

        private readonly QuestionLevelBusiness db_questionLevel;

        private readonly TeacherBusiness db_teacher;

        private readonly EmployeesInfoManage db_emp;

        /// <summary>
        /// 试卷业务类实例
        /// </summary>
        private readonly ExaminationPaperBusiness db_ExamPaper;

        public AnswerQuestionBusiness()
        {
            db_questionLevel = new QuestionLevelBusiness();
            db_emp = new EmployeesInfoManage();
            db_teacher = new TeacherBusiness();
            db_ExamPaper = new ExaminationPaperBusiness();
        }

        /// <summary>
        /// 获取所有解答题木
        /// </summary>
        /// <returns></returns>
        public List<AnswerQuestionBank> AllAnswerQuestion()
        {
            return this.GetList();
        }


        /// <summary>
        /// 转换视图模型
        /// </summary>
        /// <param name="">AnswerQuestionBank类</param>
        /// <returns></returns>
        public AnswerQuestionView ConvertToAnswerQuestionView(AnswerQuestionBank answerQuestionBank, bool IsNeedProposition = true)
        {
            AnswerQuestionView answerQuestionView = new AnswerQuestionView();

            CourseBusiness courseBusiness = new CourseBusiness();
            answerQuestionView.Grand = answerQuestionBank.Grand;

            if (answerQuestionBank.Course == 0)
            {
                answerQuestionView.Course = null;
            }
            else
            {
                answerQuestionView.Course = courseBusiness.GetCurriculas().Where(d => d.CurriculumID == answerQuestionBank.Course).FirstOrDefault();
            }

            

            answerQuestionView.ID = answerQuestionBank.ID;
            answerQuestionView.IsUsing = answerQuestionBank.IsUsing;
            answerQuestionView.Level = db_questionLevel.AllQuestionLevel().Where(d => d.LevelID == answerQuestionBank.Level).FirstOrDefault();

            if (IsNeedProposition)
            {
                var empid = db_teacher.GetEntity(answerQuestionBank.Proposition).EmployeeId;
                answerQuestionView.Proposition = db_emp.GetAll().Where(c => c.EmployeeId == empid).FirstOrDefault();

            }
            else
            {
                answerQuestionView.Proposition = null;
            }

            answerQuestionView.ReferenceAnswer = answerQuestionBank.ReferenceAnswer;
            answerQuestionView.Remark = answerQuestionBank.Remark;
            answerQuestionView.Title = answerQuestionBank.Title;
            answerQuestionView.PropositionDate = answerQuestionBank.PropositionDate;

            return answerQuestionView;
        }



        /// <summary>
        /// 启用或者禁用解答题题目
        /// </summary>
        /// <param name="answerquestionIds"></param>

        public void DisableOrEnable(List<string> answerquestionIds)
        {

            if (answerquestionIds.Count > 0)
            {
                foreach (var item in answerquestionIds)
                {

                    var obj = this.AllAnswerQuestion().Where(d => d.ID == int.Parse(item)).FirstOrDefault() ;

                    bool IsUsing = obj.IsUsing == true ? false : true;

                    obj.IsUsing = IsUsing;

                    this.Update(obj);



                }
            }


        }

        /// <summary>
        /// 判断解答题是否已经被使用
        /// </summary>
        /// <returns></returns>
        public bool IsItUsed(int answerQuestionId)
        {

            var obj = db_ExamPaper.AllAnswerQuestionPaper().Where(d => d.AnswerQuestionID == answerQuestionId).FirstOrDefault();

            return obj != null;
        }

        /// <summary>
        /// 删除解答题题目
        /// </summary>
        /// <returns></returns>
        public AjaxResult Remove(int answerQuestionId)
        {

            AjaxResult result = new AjaxResult();

            //如果已经试卷中被使用则不能删除
            if (!IsItUsed(answerQuestionId))
            {
                var obj = this.AllAnswerQuestion().Where(d => d.ID == answerQuestionId).FirstOrDefault();

                try
                {
                    this.Delete(obj);

                    result.ErrorCode = 200;

                }
                catch (Exception ex)
                {

                    result.ErrorCode = 500;
                }
            }
            else
            {
                result.ErrorCode = 400;
            }

            return result;

        }



        /// <summary>
        /// 筛选解答题题目
        /// </summary>
        /// <returns></returns>
        public List<AnswerQuestionBank> Search(string Title, int course)
        {

            List<AnswerQuestionBank> resultlist = new List<AnswerQuestionBank>();

            if (course == 0 && Title == "")
            {
                resultlist = this.AllAnswerQuestion().ToList();
            }

            if (course != 0 && Title == "")
            {
                resultlist = this.AllAnswerQuestion().Where(d => d.Course == course).ToList();
            }

            if (Title != "" && course != 0)
            {
                resultlist = this.AllAnswerQuestion().Where(d => d.Course == course && d.Title.Contains(Title)).ToList();
            }
            if (Title != "" && course == 0)
            {
                resultlist = this.AllAnswerQuestion().Where(d => d.Title.Contains(Title)).ToList();
            }

            return resultlist;
        }

        /// <summary>
        /// 读取Excel数据
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public List<AnswerQuestionBank> ReadQuestionForExcel(Stream stream, string contentType)
        {
            List<AnswerQuestionBank> result = new List<AnswerQuestionBank>();

            #region 创建excel实例
            IWorkbook workbook = null;

            if (contentType == "application/vnd.ms-excel")
            {
                // 2003版本
                workbook = new HSSFWorkbook(stream);
            }

            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                // 2007版本
                workbook = new XSSFWorkbook(stream);
            }

            ISheet sheet = (ISheet)workbook.GetSheetAt(0);

            #endregion

            var num = 0;

            // 循环获取题目
            while (true)
            {
                num++;
                // 获取题目
                var obj = getQuestion(num, sheet);
                if (obj == null)
                {
                    break;
                }

                result.Add(obj);

            }

            stream.Close();
            stream.Dispose();
            workbook.Close();

            return result;
        }

        const int danwei = 20;

        public AnswerQuestionBank getQuestion(int Number, ISheet sheet)
        {
            AnswerQuestionBank result = new AnswerQuestionBank();

            //1 0; 2 18 +1; 3 18*2 +1

            int beginRowindex = (danwei * (Number - 1)) + 1;

            //获取题目

            try
            {
                string title = sheet.GetRow(beginRowindex + 1).Cells[0].StringCellValue;
                if (string.IsNullOrEmpty(title))
                {
                    return null;
                }
                result.Title = title;

                //获取 难度级别
                string Level = sheet.GetRow(beginRowindex + 5).Cells[1].StringCellValue;
                
                switch (Level)
                {
                    case "简单":
                        result.Level = 1;
                        break;
                    case "普通":
                        result.Level = 2;
                        break;
                    case "困难":
                        result.Level = 3;
                        break;
                }

                // 获取 参考答案

                var answerCell = sheet.GetRow(beginRowindex + 8).Cells[0];
                string answer = answerCell == null ? "":answerCell.ToString();
                result.ReferenceAnswer = answer;

                var remarkCell = sheet.GetRow(beginRowindex + 15).Cells[0];

                string remark = remarkCell==null? "": remarkCell.StringCellValue;
                result.Remark = remark;

            }
            catch (Exception ex)
            {

                result = null;
            }

            return result;

        }

        public bool InitQuestionTemplate(int QuestionNumber, string path)
        {
            int danwei = 20;

            try
            {
                FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

                HSSFWorkbook workbook = new HSSFWorkbook();

                HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();

                for (int i = 0; i < QuestionNumber; i++)
                {
                    int beginIndex = danwei * i + 1;

                    HSSFCellStyle style = (HSSFCellStyle)workbook.CreateCellStyle();

                    HSSFFont font = (HSSFFont)workbook.CreateFont();

                    font.IsBold = true;

                    style.Alignment = HorizontalAlignment.Center;

                    style.SetFont(font);

                    HSSFRow row_title = (HSSFRow)sheet.CreateRow(beginIndex);
                    HSSFCell cell_title = (HSSFCell)row_title.CreateCell(0);

                    cell_title.SetCellValue("题目");
                    cell_title.CellStyle = style;

                    CellRangeAddress title_range = new CellRangeAddress(beginIndex, beginIndex, 0, 10);
                    sheet.AddMergedRegion(title_range);

                    CellRangeAddress title_row_title_content_range = new CellRangeAddress(beginIndex + 1, beginIndex + 4, 0, 10);
                    sheet.AddMergedRegion(title_row_title_content_range);

                    HSSFRow row_level = (HSSFRow)sheet.CreateRow(beginIndex + 5);
                    HSSFCell cell_level = (HSSFCell)row_level.CreateCell(2);
                    cell_level.SetCellValue("难度");
                    cell_level.CellStyle = style;


                    HSSFCell cell_level_defaultValue = (HSSFCell)row_level.CreateCell(3);
                    cell_level_defaultValue.SetCellValue("普通");

                    HSSFRow row_answer = (HSSFRow)sheet.CreateRow(beginIndex + 7);

                    HSSFCell cell_answer = (HSSFCell)row_answer.CreateCell(0);
                    cell_answer.SetCellValue("参考答案");
                    cell_answer.CellStyle = style;

                    CellRangeAddress answer_range = new CellRangeAddress(beginIndex + 7, beginIndex + 7, 0, 10);
                    sheet.AddMergedRegion(answer_range);

                    CellRangeAddress answer_content_range = new CellRangeAddress(beginIndex + 8, beginIndex + 12, 0, 10);
                    sheet.AddMergedRegion(answer_content_range);

                    HSSFRow row_remark = (HSSFRow)sheet.CreateRow(beginIndex + 14);
                    HSSFCell cell_remark = (HSSFCell)row_remark.CreateCell(0);
                    cell_remark.SetCellValue("备注");
                    cell_remark.CellStyle = style;

                    CellRangeAddress remark_range = new CellRangeAddress(beginIndex + 14, beginIndex + 14, 0, 10);
                    sheet.AddMergedRegion(remark_range);

                    CellRangeAddress remark_content_range = new CellRangeAddress(beginIndex + 15, beginIndex + 18, 0, 10);
                    sheet.AddMergedRegion(remark_content_range);
                }

                workbook.Write(fileStream);
                workbook.Close();

                fileStream.Close();
                fileStream.Dispose();

                return true;
            }
            catch (Exception)
            {

                return false;
            }

            
            
        }

    }
}
