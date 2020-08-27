using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiliconValley.InformationSystem.Business.ExaminationSystemBusiness
{
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using SiliconValley.InformationSystem.Business.CourseSyllabusBusiness;
    using SiliconValley.InformationSystem.Entity.MyEntity;
    using SiliconValley.InformationSystem.Entity.ViewEntity.ExaminationSystemView;
    using SiliconValley.InformationSystem.Business.TeachingDepBusiness;
    using SiliconValley.InformationSystem.Util;
    using System.IO;
    using NPOI.XSSF.UserModel;
    using NPOI.SS.Util;


    /// <summary>
    /// 选择题题库业务类
    /// </summary>
    public class ChoiceQuestionBusiness:BaseBusiness<MultipleChoiceQuestion>
    {

        private readonly QuestionLevelBusiness db_questionLevel; 

        private readonly TeacherBusiness db_teacher;

        private readonly BaseBusiness<EmployeesInfo> db_emp;


        /// <summary>
        /// 试卷业务类实例
        /// </summary>
        private readonly ExaminationPaperBusiness db_ExamPaper;

        public ChoiceQuestionBusiness()
        {
            db_questionLevel = new QuestionLevelBusiness();
            db_teacher = new TeacherBusiness();
            db_emp = new BaseBusiness<EmployeesInfo>();
            db_ExamPaper = new ExaminationPaperBusiness();
        }

        /// <summary>
        /// 获取所有选择题题库
        /// </summary>
        /// <returns>选择题集合</returns>
        public List<MultipleChoiceQuestion> AllChoiceQuestionData()
        {

            return this.GetList().ToList();


        }

        /// <summary>
        /// 选择题数据表格实体
        /// </summary>
        /// <param name="multipleChoiceQuestion"></param>
        /// <param name="IsNeedProposition">是否需要阅卷人</param>
        /// <returns></returns>
        public ChoiceQuestionTableView ConvertToChoiceQuestionTableView(MultipleChoiceQuestion multipleChoiceQuestion,bool IsNeedProposition = true)
        {

            CourseBusiness courseBusiness = new CourseBusiness();

            ChoiceQuestionTableView choiceQuestionTableView = new ChoiceQuestionTableView();

            choiceQuestionTableView.Answer = multipleChoiceQuestion.Answer;

            choiceQuestionTableView.Grand = multipleChoiceQuestion.Grand;

            //获取题目所属课程
            if (multipleChoiceQuestion.Course == 0)
            {
                choiceQuestionTableView.Course = null;
            }
            else
            {
                choiceQuestionTableView.Course = courseBusiness.GetCurriculas().Where(d => d.CurriculumID == multipleChoiceQuestion.Course).FirstOrDefault();
            }
            choiceQuestionTableView.CreateTime = multipleChoiceQuestion.CreateTime;
            choiceQuestionTableView.Id = multipleChoiceQuestion.Id;
            choiceQuestionTableView.IsRadio = multipleChoiceQuestion.IsRadio;
            choiceQuestionTableView.IsUsing = multipleChoiceQuestion.IsUsing;
            //获取难度级别
            choiceQuestionTableView.Level = db_questionLevel.AllQuestionLevel().Where(d => d.LevelID == multipleChoiceQuestion.Level).FirstOrDefault();
            choiceQuestionTableView.OptionA = multipleChoiceQuestion.OptionA;
            choiceQuestionTableView.OptionB = multipleChoiceQuestion.OptionB;
            choiceQuestionTableView.OptionC = multipleChoiceQuestion.OptionC;
            choiceQuestionTableView.OptionD = multipleChoiceQuestion.OptionD;
            if (IsNeedProposition)
            {
                //获取命题人

                var empid =  db_teacher.GetEntity(multipleChoiceQuestion.Proposition).EmployeeId;
                choiceQuestionTableView.Proposition = db_emp.GetList().Where(d=>d.EmployeeId == empid).FirstOrDefault();
            }
            else
            {
                choiceQuestionTableView.Proposition = null;
            }
            

            choiceQuestionTableView.Remark = multipleChoiceQuestion.Remark;
            choiceQuestionTableView.Title = multipleChoiceQuestion.Title;


            return choiceQuestionTableView;
        }


        /// <summary>
        /// 禁用或启用题目
        /// </summary>
        /// <param name="questionIds"></param>
        public void DisableOrEnable(string [] questionIds)
        {

            foreach (var item in questionIds)
            {

                var obj = this.AllChoiceQuestionData().Where(d=>d.Id==int.Parse(item)).FirstOrDefault();

                bool isUsing = obj.IsUsing == true ? false : true;

                obj.IsUsing = isUsing;

                this.Update(obj);

            }



        }
        /// <summary>
        /// 批量更改选择题难度
        /// </summary>
        /// <param name="questionIds"></param>
        /// <param name="courseS"></param>
        public void PiLiangXiuGaiND(List<string> questionIds, int Level)
        {

            foreach (var item in questionIds)
            {

                var obj = this.AllChoiceQuestionData().Where(d => d.Id == int.Parse(item)).FirstOrDefault();

                int level = Level;
                obj.Level = level;
                this.Update(obj);

            }



        }
        /// <summary>
        /// 批量修改选择题课程
        /// </summary>
        /// <param name="questionIds"></param>
        public void PiLiangXiuGai(List<string> questionIds, int courseS)
        {

            foreach (var item in questionIds)
            {

                var obj = this.AllChoiceQuestionData().Where(d => d.Id == int.Parse(item)).FirstOrDefault();

                int course = courseS;
                obj.Course = course;
                this.Update(obj);

            }



        }


        /// <summary>
        /// 判断选择题是否被使用过
        /// </summary>
        /// <param name="id">选择题ID</param>
        /// <returns></returns>
        public bool IsItUsed(int id)
        {
            var compaerObj = db_ExamPaper.AllChoiceQuestionPaper().Where(d => d.ChooseQuestion == id).FirstOrDefault();

            return compaerObj != null;
        }

        /// <summary>
        /// 删除选择题目
        /// </summary>
        /// <param name="id">选择题ID</param>

        public AjaxResult RemoveChoiceQuestion(int id)
        {

            AjaxResult result = new AjaxResult();

            //被使用的选择题不能被删除

            if (!this.IsItUsed(id))
            {
               var obj = this.AllChoiceQuestionData().Where(d => d.Id == id).FirstOrDefault();

                //可以删除

                try
                {
                    this.Delete(obj);

                    result.ErrorCode = 200;
                    result.Msg = "成功";
                    result.Data = null;
                }
                catch (Exception ex)
                {

                    result.ErrorCode = 500;
                    result.Msg = "失败";
                    result.Data = null;
                }
                


            }
            else
            {
                result.ErrorCode = 400;
                result.Msg = "失败";
                result.Data = null;
            }

            return result;

        }

        public List<MultipleChoiceQuestion> ReadQuestionForExcel(Stream stream, string contentType)
        {

            List<MultipleChoiceQuestion> result = new List<MultipleChoiceQuestion>();

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

        //一个题目占19行
        const int danwei = 19;

        /// <summary>
        /// 获取题目
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public MultipleChoiceQuestion getQuestion(int Number, ISheet sheet)
        {
            MultipleChoiceQuestion result = new MultipleChoiceQuestion();

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
                
                //获取 是否单选
                string IsRedio = sheet.GetRow(beginRowindex + 5).Cells[1].StringCellValue.Trim();
                result.IsRadio = IsRedio == "是" ? true : false;

                //获取 难度级别
                string Level = sheet.GetRow(beginRowindex + 5).Cells[3].StringCellValue.Trim();

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
                string answer = sheet.GetRow(beginRowindex + 5).Cells[5].StringCellValue.Trim();
                result.Answer = answer;

                // 获取 A 选项
                var optionACell = sheet.GetRow(beginRowindex + 7).Cells[1];
                string optionA = optionACell.CellType.ToString() == "Numeric" ? optionACell.NumericCellValue.ToString().Trim() : optionACell.StringCellValue.Trim();
                result.OptionA = optionA;


                // 获取 B 选项
                var  optionBCell = sheet.GetRow(beginRowindex + 10).Cells[1];

                string optionB = optionBCell.CellType.ToString() == "Numeric" ? optionBCell.NumericCellValue.ToString().Trim() : optionBCell.StringCellValue.Trim();
                result.OptionB = optionB;

                // 获取 C 选项
                var optionCCell = sheet.GetRow(beginRowindex + 13).Cells[1];
                string optionC = optionCCell.CellType.ToString() == "Numeric" ? optionCCell.NumericCellValue.ToString().Trim() : optionCCell.StringCellValue.Trim();
                result.OptionC = optionC;

                // 获取 D 选项
                var optionDCell = sheet.GetRow(beginRowindex + 16).Cells[1];
                string optionD = optionDCell.CellType.ToString() == "Numeric"? optionDCell.NumericCellValue.ToString().Trim() : optionDCell.StringCellValue.Trim();
                result.OptionD = optionD;

            }
            catch (Exception ex)
            {

                result = null;
            }

            return result;
 
        }

        /// <summary>
        /// 初始化Excel
        /// </summary>
        /// <param name="initCount"></param>
        /// <returns></returns>
        public bool InitQuestionTemplate(int QuestionNumber, string path)
        {  
            try
            {
                FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

                HSSFWorkbook workbook = new HSSFWorkbook();

                //创建工作簿
                ISheet sheet = (ISheet)workbook.CreateSheet();

                for (int i = 0; i < QuestionNumber; i++)
                {
                    //1 0,   2 ,20  19*i+1

                    int beginIndex = danwei * i + 1;

                    Console.WriteLine(beginIndex.ToString());

                    //创建行
                    HSSFRow row_title = (HSSFRow)sheet.CreateRow(beginIndex);

                    HSSFCellStyle style = (HSSFCellStyle)workbook.CreateCellStyle();

                    HSSFFont font = (HSSFFont)workbook.CreateFont();

                    font.IsBold = true;

                    style.Alignment = HorizontalAlignment.Center;

                    style.SetFont(font);

                    HSSFCell cell_title = (HSSFCell)row_title.CreateCell(0);

                    cell_title.CellStyle = style;

                    cell_title.SetCellValue("题目");

                    CellRangeAddress title_range = new CellRangeAddress(beginIndex, beginIndex, 0, 9);

                    sheet.AddMergedRegion(title_range);


                    //创建题目内容行
                    HSSFRow row_titleContent = (HSSFRow)sheet.CreateRow(beginIndex + 1);

                    CellRangeAddress titleContent_range = new CellRangeAddress(beginIndex + 1, beginIndex + 3, 0, 9);

                    sheet.AddMergedRegion(titleContent_range);

                    //创建 单选、难度、答案
                    HSSFRow row_isRadio_Level_Answer = (HSSFRow)sheet.CreateRow(beginIndex + 5);

                    HSSFCell cell_isRadio = (HSSFCell)row_isRadio_Level_Answer.CreateCell(1);

                    cell_isRadio.SetCellValue("单选");

                    cell_isRadio.CellStyle = style;

                    HSSFCell cell_isRadio_defaultValue = (HSSFCell)row_isRadio_Level_Answer.CreateCell(2);

                    cell_isRadio_defaultValue.SetCellValue("是");

                    HSSFCell cell_level = (HSSFCell)row_isRadio_Level_Answer.CreateCell(4);

                    cell_level.SetCellValue("难度");

                    cell_level.CellStyle = style;

                    HSSFCell cell_levelContent = (HSSFCell)row_isRadio_Level_Answer.CreateCell(5);

                    cell_levelContent.SetCellValue("普通");

                    HSSFCell cell_answer = (HSSFCell)row_isRadio_Level_Answer.CreateCell(7);

                    cell_answer.SetCellValue("答案");

                    cell_answer.CellStyle = style;

                    //创建 选项区域

                    //选项A
                    HSSFRow row_optionA = (HSSFRow)sheet.CreateRow(beginIndex + 7);

                    HSSFCell cell_optionA = (HSSFCell)row_optionA.CreateCell(0);

                    cell_optionA.SetCellValue("A");

                    cell_optionA.CellStyle = style;

                    CellRangeAddress optionARange = new CellRangeAddress(beginIndex + 7, beginIndex + 8, 0, 0);

                    sheet.AddMergedRegion(optionARange);

                    CellRangeAddress optionA_Content_Range = new CellRangeAddress(beginIndex + 7, beginIndex + 8, 1, 9);

                    sheet.AddMergedRegion(optionA_Content_Range);

                    //选项B
                    HSSFRow row_optionB = (HSSFRow)sheet.CreateRow(beginIndex + 10);

                    HSSFCell cell_optionB = (HSSFCell)row_optionB.CreateCell(0);

                    cell_optionB.SetCellValue("B");

                    cell_optionB.CellStyle = style;

                    CellRangeAddress optionBRange = new CellRangeAddress(beginIndex + 10, beginIndex + 11, 0, 0);

                    sheet.AddMergedRegion(optionBRange);

                    CellRangeAddress optionB_Content_Range = new CellRangeAddress(beginIndex + 10, beginIndex + 11, 1, 9);

                    sheet.AddMergedRegion(optionB_Content_Range);

                    //选项C
                    HSSFRow row_optionC = (HSSFRow)sheet.CreateRow(beginIndex + 13);

                    HSSFCell cell_optionC = (HSSFCell)row_optionC.CreateCell(0);

                    cell_optionC.SetCellValue("C");

                    cell_optionC.CellStyle = style;

                    CellRangeAddress optionCRange = new CellRangeAddress(beginIndex + 13, beginIndex + 14, 0, 0);

                    sheet.AddMergedRegion(optionCRange);

                    CellRangeAddress optionC_Content_Range = new CellRangeAddress(beginIndex + 13, beginIndex + 14, 1, 9);

                    sheet.AddMergedRegion(optionC_Content_Range);

                    //选项D
                    HSSFRow row_optionD = (HSSFRow)sheet.CreateRow(beginIndex + 16);

                    HSSFCell cell_optionD = (HSSFCell)row_optionD.CreateCell(0);

                    cell_optionD.SetCellValue("D");

                    cell_optionD.CellStyle = style;

                    CellRangeAddress optionDRange = new CellRangeAddress(beginIndex + 16, beginIndex + 17, 0, 0);

                    sheet.AddMergedRegion(optionDRange);

                    CellRangeAddress optionD_Content_Range = new CellRangeAddress(beginIndex + 16, beginIndex + 17, 1, 9);

                    sheet.AddMergedRegion(optionD_Content_Range);
                }

                workbook.Write(filestream);

                workbook.Close();

                filestream.Close();

                filestream.Dispose();

                return true;

            }
            catch (Exception)
            {

                return false;
            }
        }

    }
}
