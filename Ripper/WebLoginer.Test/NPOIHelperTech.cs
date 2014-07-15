using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NPOI;
using NPOI.HPSF;
using NPOI.HSSF;
using NPOI.HSSF.UserModel;
using System.Data;
using NPOI.SS.UserModel;

namespace StarTech.NPOI
{


    public class MyNPOI
    {

        public MyNPOI()
        {
            InitializeWorkbook();
        }

        HSSFWorkbook hssfworkbook;
        void InitializeWorkbook()
        {
            hssfworkbook = new HSSFWorkbook();

            ////create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = "NPOI Team";
            hssfworkbook.DocumentSummaryInformation = dsi;

            ////create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = "NPOI SDK Example";
            hssfworkbook.SummaryInformation = si;
        }

        public void GenerateData()
        {
            ISheet sheet1 = hssfworkbook.CreateSheet("Sheet1");

            sheet1.CreateRow(0).CreateCell(0).SetCellValue("This is a Sample");
            int x = 1;
            for (int i = 1; i <= 15; i++)
            {
                IRow row = sheet1.CreateRow(i);
                for (int j = 0; j < 15; j++)
                {
                    row.CreateCell(j).SetCellValue(x++);
                }
            }
            using (FileStream file = new FileStream("测试.xls", FileMode.Create))
            {
                hssfworkbook.Write(file);
            }


        }
    }
    ///// <summary>
    ///// Excel生成操作类
    ///// </summary>
    //public class NPOIHelper
    //{
    //    /// <summary>
    //    /// 导出列名
    //    /// </summary>
    //    public static System.Collections.SortedList ListColumnsName;
    //    /// <summary>
    //    /// 导出Excel
    //    /// </summary>
    //    /// <param name="dgv"></param>
    //    /// <param name="filePath"></param>
    //    public static void ExportExcel(DataTable dtSource, string filePath)
    //    {
    //        if (ListColumnsName == null || ListColumnsName.Count == 0)
    //            throw (new Exception("请对ListColumnsName设置要导出的列名！"));

    //        HSSFWorkbook excelWorkbook = CreateExcelFile();
    //        InsertRow(dtSource, excelWorkbook);
    //        SaveExcelFile(excelWorkbook, filePath);
    //    }
    //    /// <summary>
    //    /// 导出Excel
    //    /// </summary>
    //    /// <param name="dgv"></param>
    //    /// <param name="filePath"></param>
    //    public static void ExportExcel(DataTable dtSource, Stream excelStream)
    //    {
    //        if (ListColumnsName == null || ListColumnsName.Count == 0)
    //            throw (new Exception("请对ListColumnsName设置要导出的列名！"));

    //        HSSFWorkbook excelWorkbook = CreateExcelFile();
    //        InsertRow(dtSource, excelWorkbook);
    //        SaveExcelFile(excelWorkbook, excelStream);
    //    }
    //    /// <summary>
    //    /// 保存Excel文件
    //    /// </summary>
    //    /// <param name="excelWorkBook"></param>
    //    /// <param name="filePath"></param>
    //    protected static void SaveExcelFile(HSSFWorkbook excelWorkBook, string filePath)
    //    {
    //        FileStream file = null;
    //        try
    //        {
    //            file = new FileStream(filePath, FileMode.Create);
    //            excelWorkBook.Write(file);
    //        }
    //        finally
    //        {
    //            if (file != null)
    //            {
    //                file.Close();
    //            }
    //        }
    //    }
    //    /// <summary>
    //    /// 保存Excel文件
    //    /// </summary>
    //    /// <param name="excelWorkBook"></param>
    //    /// <param name="filePath"></param>
    //    protected static void SaveExcelFile(HSSFWorkbook excelWorkBook, Stream excelStream)
    //    {
    //        try
    //        {
    //            excelWorkBook.Write(excelStream);
    //        }
    //        finally
    //        {

    //        }
    //    }
    //    /// <summary>
    //    /// 创建Excel文件
    //    /// </summary>
    //    /// <param name="filePath"></param>
    //    protected static HSSFWorkbook CreateExcelFile()
    //    {
    //        HSSFWorkbook hssfworkbook = new HSSFWorkbook();
    //        return hssfworkbook;
    //    }
    //    /// <summary>
    //    /// 创建excel表头
    //    /// </summary>
    //    /// <param name="dgv"></param>
    //    /// <param name="excelSheet"></param>
    //    protected static void CreateHeader(HSSFSheet excelSheet)
    //    {
    //        int cellIndex = 0;
    //        //循环导出列
    //        foreach (System.Collections.DictionaryEntry de in ListColumnsName)
    //        {
    //            HSSFRow newRow = excelSheet.CreateRow(0);
    //            HSSFCell newCell = newRow.CreateCell(cellIndex);
    //            newCell.SetCellValue(de.Value.ToString());
    //            cellIndex++;
    //        }
    //    }
    //    /// <summary>
    //    /// 插入数据行
    //    /// </summary>
    //    protected static void InsertRow(DataTable dtSource, HSSFWorkbook excelWorkbook)
    //    {
    //        int rowCount = 0;
    //        int sheetCount = 1;
    //        HSSFSheet newsheet = null;

    //        //循环数据源导出数据集
    //        newsheet = excelWorkbook.CreateSheet("Sheet" + sheetCount);
    //        CreateHeader(newsheet);
    //        foreach (DataRow dr in dtSource.Rows)
    //        {
    //            rowCount++;
    //            //超出10000条数据 创建新的工作簿
    //            if (rowCount == 10000)
    //            {
    //                rowCount = 1;
    //                sheetCount++;
    //                newsheet = excelWorkbook.CreateSheet("Sheet" + sheetCount);
    //                CreateHeader(newsheet);
    //            }

    //            HSSFRow newRow = newsheet.CreateRow(rowCount);
    //            InsertCell(dtSource, dr, newRow, newsheet, excelWorkbook);
    //        }
    //    }
    //    /// <summary>
    //    /// 导出数据行
    //    /// </summary>
    //    /// <param name="dtSource"></param>
    //    /// <param name="drSource"></param>
    //    /// <param name="currentExcelRow"></param>
    //    /// <param name="excelSheet"></param>
    //    /// <param name="excelWorkBook"></param>
    //    protected static void InsertCell(DataTable dtSource, DataRow drSource, HSSFRow currentExcelRow, HSSFSheet excelSheet, HSSFWorkbook excelWorkBook)
    //    {
    //        for (int cellIndex = 0; cellIndex < ListColumnsName.Count; cellIndex++)
    //        {
    //            //列名称
    //            string columnsName = ListColumnsName.GetKey(cellIndex).ToString();
    //            HSSFCell newCell = null;
    //            System.Type rowType = drSource[columnsName].GetType();
    //            string drValue = drSource[columnsName].ToString().Trim();
    //            switch (rowType.ToString())
    //            {
    //                case "System.String"://字符串类型
    //                    drValue = drValue.Replace("&", "&");
    //                    drValue = drValue.Replace(">", ">");
    //                    drValue = drValue.Replace("<", "<");
    //                    newCell = currentExcelRow.CreateCell(cellIndex);
    //                    newCell.SetCellValue(drValue);
    //                    break;
    //                case "System.DateTime"://日期类型
    //                    DateTime dateV;
    //                    DateTime.TryParse(drValue, out dateV);
    //                    newCell = currentExcelRow.CreateCell(cellIndex);
    //                    newCell.SetCellValue(dateV);

    //                    //格式化显示
    //                    HSSFCellStyle cellStyle = excelWorkBook.CreateCellStyle();
    //                    HSSFDataFormat format = excelWorkBook.CreateDataFormat();
    //                    cellStyle.DataFormat = format.GetFormat("yyyy-mm-dd hh:mm:ss");
    //                    newCell.CellStyle = cellStyle;

    //                    break;
    //                case "System.Boolean"://布尔型
    //                    bool boolV = false;
    //                    bool.TryParse(drValue, out boolV);
    //                    newCell = currentExcelRow.CreateCell(cellIndex);
    //                    newCell.SetCellValue(boolV);
    //                    break;
    //                case "System.Int16"://整型
    //                case "System.Int32":
    //                case "System.Int64":
    //                case "System.Byte":
    //                    int intV = 0;
    //                    int.TryParse(drValue, out intV);
    //                    newCell = currentExcelRow.CreateCell(cellIndex);
    //                    newCell.SetCellValue(intV.ToString());
    //                    break;
    //                case "System.Decimal"://浮点型
    //                case "System.Double":
    //                    double doubV = 0;
    //                    double.TryParse(drValue, out doubV);
    //                    newCell = currentExcelRow.CreateCell(cellIndex);
    //                    newCell.SetCellValue(doubV);
    //                    break;
    //                case "System.DBNull"://空值处理
    //                    newCell = currentExcelRow.CreateCell(cellIndex);
    //                    newCell.SetCellValue("");
    //                    break;
    //                default:
    //                    throw (new Exception(rowType.ToString() + "：类型数据无法处理!"));
    //            }
    //        }
    //    }
    //}
    ////排序实现接口 不进行排序 根据添加顺序导出
    //public class NoSort : System.Collections.IComparer
    //{
    //    public int Compare(object x, object y)
    //    {
    //        return -1;
    //    }
    //}
}