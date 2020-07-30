using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using SiliconValley.InformationSystem.Entity.Entity;
using Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.IO;
using SiliconValley.InformationSystem.Entity.ViewEntity;
using SiliconValley.InformationSystem.Entity.ViewEntity.TM_Data.Print;

namespace SiliconValley.InformationSystem.Business.NewExcel
{
   public class ExcelHelper
    {
        private PropertyInfo[] GetPropertyInfoArray()
        {
            PropertyInfo[] props = null;
            try
            {
                Type type = typeof(MyExcelClass);
                object obj = Activator.CreateInstance(type);
                props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            }
            catch (Exception)
            {
                
            }
            return props;
        }
        /// <summary>
        /// 将疑似数据写入Excel中
        /// </summary>
        /// <param name="list">要写入Excel的数据</param>
        /// <param name="mypath">保存的路径</param>
        /// <returns></returns>
        public bool DaoruExcel(List<MyExcelClass> list ,string mypath,List<string> Head)
        {
            bool IsCuss = false;
            object misValue = System.Reflection.Missing.Value;
            Application xlApp = new Application();
            Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);
            Worksheet xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            //PropertyInfo[] props = GetPropertyInfoArray();
            for (int i = 0; i < Head.Count; i++)
            {
                xlWorkSheet.Cells[1, i + 1] = Head[i]; //write the column name
            }
            for (int i = 0; i < list.Count; i++)
            {
                xlWorkSheet.Cells[i + 2, 1] = list[i].StuName;
                xlWorkSheet.Cells[i + 2, 2] = list[i].StuSex;
                xlWorkSheet.Cells[i + 2, 3] = list[i].StuPhone;
                xlWorkSheet.Cells[i + 2, 4] = list[i].StuEducational;
                xlWorkSheet.Cells[i + 2, 5] = list[i].StuAddress;
                xlWorkSheet.Cells[i + 2, 6] = list[i].StuSchoolName;
                xlWorkSheet.Cells[i + 2, 7] = list[i].Region_id;
                xlWorkSheet.Cells[i + 2, 8] = list[i].StuInfomationType_Id;
                xlWorkSheet.Cells[i + 2, 9] = list[i].EmployeesInfo_Id;
                xlWorkSheet.Cells[i + 2, 10] = list[i].Reak;
            }
            try
            {            
                xlWorkBook.SaveAs(mypath, XlFileFormat.xlExcel8, null, null, false, false, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                IsCuss = true;
            }
            catch (Exception)
            {
                return IsCuss;
            }
            return IsCuss;
        }


        public bool DaoruExport(List<ExportStudentBeanData> data,string mypath,List<string> Head)
        {
            bool IsCuss = false;
            object misValue = System.Reflection.Missing.Value;
            Application xlApp = new Application();
            Workbook xlWorkBook = xlApp.Workbooks.Add(misValue);
            Worksheet xlWorkSheet = (Worksheet)xlWorkBook.Worksheets.get_Item(1);

            for (int i = 0; i < Head.Count; i++)
            {
                xlWorkSheet.Cells[1, i + 1] = Head[i]; //write the column name
            }
            for (int i = 0; i < data.Count; i++)
            {
                xlWorkSheet.Cells[i + 2, 1] = data[i].StuName;
                xlWorkSheet.Cells[i + 2, 2] = data[i].StuSex; 
                xlWorkSheet.Cells[i + 2, 3] = data[i].StuBirthy;
                xlWorkSheet.Cells[i + 2, 4] = data[i].Stuphone;
                xlWorkSheet.Cells[i + 2, 5] = data[i].StuSchoolName;
                xlWorkSheet.Cells[i + 2, 6] = data[i].StuEducational;
                xlWorkSheet.Cells[i + 2, 7] = data[i].StuAddress;
                xlWorkSheet.Cells[i + 2, 8] = data[i].StuWeiXin;
                xlWorkSheet.Cells[i + 2, 9] = data[i].StuQQ;
                xlWorkSheet.Cells[i + 2, 10] = data[i].stuinfomation;
                xlWorkSheet.Cells[i + 2, 11] = data[i].StatusName;
                xlWorkSheet.Cells[i + 2, 12] = data[i].StuisGoto == false ? "否" : "是";
                xlWorkSheet.Cells[i + 2, 13] = data[i].StuVisit;
                xlWorkSheet.Cells[i + 2, 14] = data[i].empName;
                xlWorkSheet.Cells[i + 2, 15] = data[i].Party;
                xlWorkSheet.Cells[i + 2, 16] = data[i].BeanDate;
                xlWorkSheet.Cells[i + 2, 17] = data[i].StuEntering;
                xlWorkSheet.Cells[i + 2, 18] = data[i].StatusTime;               
                xlWorkSheet.Cells[i + 2, 19] = data[i].RegionName; 
                xlWorkSheet.Cells[i + 2, 20] = data[i].ConsultTeacher;
                xlWorkSheet.Cells[i + 2, 21] = data[i].Reak;
            }

            try
            {
                xlWorkBook.SaveAs(mypath, XlFileFormat.xlExcel8, null, null, false, false, XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                xlApp.Quit();
                IsCuss = true;
            }
            catch (Exception ex)
            {
                return IsCuss;
            }
            return IsCuss;
        }        
    }
}
