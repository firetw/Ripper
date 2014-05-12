using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Reflection;

namespace ConsoleApplication1
{
    public class ExcelHelper
    {
        public static void Test(List<List<string>> list)
        {
            string excelFilename = @"E:/excel.xlsx";
            foreach (var item in list)
            {

            }
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.Visible = true;
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = xlApp.Workbooks.Add();

            Console.WriteLine(xlWorkBook.Sheets.Count);
            Microsoft.Office.Interop.Excel.Worksheet preWorkSheet = null;
            for (int i = 1; i <= list.Count; i++)
            {
                List<string> content = list[i - 1];
                Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = null;
                if (i > xlWorkBook.Sheets.Count)
                    xlWorkSheet = xlWorkBook.Sheets.Add(Type.Missing, preWorkSheet);
                else
                    xlWorkSheet = xlWorkBook.Sheets[i];
                preWorkSheet = xlWorkSheet;

                xlWorkSheet.Name = "mysheet" + i;


                Range range = xlWorkSheet.Range[xlWorkSheet.Cells[1, 1], xlWorkSheet.Cells[1, content.Count]];//xlWorkSheet.get_Range("A1", "C1");
                range.Value2 = content.ToArray();
            }
            xlWorkBook.SaveCopyAs(excelFilename);
            if (xlWorkBook != null)
                xlWorkBook.Close(false, Missing.Value, Missing.Value);
            if (xlApp != null)
                xlApp.Quit();
        }
    }
}
