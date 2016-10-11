using Microsoft.Office.Interop.Excel;
using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;

namespace JW18001
{
    internal class ExcelHelper
    {
        public static System.Data.DataTable ExcelToDataTable(string fileName)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("TestSerialNumber", typeof(string));
            dt.Columns.Add("TestChannelNumber", typeof(int));
            dt.Columns.Add("TestIl1", typeof(string));
            dt.Columns.Add("TestPdl1", typeof(string));
            dt.Columns.Add("TestRl1", typeof(string));
            dt.Columns.Add("TestTime1", typeof(string));
            dt.Columns.Add("TestIl2", typeof(string));
            dt.Columns.Add("TestPdl2", typeof(string));
            dt.Columns.Add("TestRl2", typeof(string));
            dt.Columns.Add("TestTime2", typeof(string));
            dt.Columns.Add("TestIl3", typeof(string));
            dt.Columns.Add("TestPdl3", typeof(string));
            dt.Columns.Add("TestRl3", typeof(string));
            dt.Columns.Add("TestTime3", typeof(string));
            dt.Columns.Add("TestIl4", typeof(string));
            dt.Columns.Add("TestPdl4", typeof(string));
            dt.Columns.Add("TestRl4", typeof(string));
            dt.Columns.Add("TestTime4", typeof(string));
            string conStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";
            OleDbConnection myConn = new OleDbConnection(conStr);
            string strCom = " SELECT * FROM [Sheet1$]";
            myConn.Open();
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strCom, myConn);
            // dt = new System.Data.DataTable();
            myCommand.Fill(dt);
            myConn.Close();
            return dt;
        }

        //public static void DataTabletoExcel(System.Data.DataTable tmpDataTable, string strFileName)
        //{
        //    if (tmpDataTable == null)

        //        return;
        //    int rowNum = tmpDataTable.Rows.Count;
        //    int columnNum = tmpDataTable.Columns.Count;
        //    int rowIndex = 1;
        //    int columnIndex = 0;
        //    Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
        //    xlApp.DefaultFilePath = "";
        //    xlApp.DisplayAlerts = true;
        //    xlApp.SheetsInNewWorkbook = 1;
        //    Workbook xlBook = xlApp.Workbooks.Add(true);
        //    //将DataTable的列名导入Excel表第一行
        //    foreach (DataColumn dc in tmpDataTable.Columns)
        //    {
        //        columnIndex++;
        //        xlApp.Cells.NumberFormat = "@"; //  如果数据中存在数字类型 可以让它变文本格式显示
        //        xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
        //    }
        //    //将DataTable中的数据导入Excel中
        //    for (int i = 0; i < rowNum; i++)
        //    {
        //        rowIndex++;
        //        columnIndex = 0;
        //        for (int j = 0; j < columnNum; j++)
        //        {
        //            columnIndex++;

        //            xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();
        //        }
        //    }
        //    xlBook.SaveCopyAs(strFileName);
        //    xlBook.Close(false);
        //}

        public static void CreateExcelFile(string fileName)
        {
            //create
            object nothing = System.Reflection.Missing.Value;
            var app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = false;
            Workbook workBook = app.Workbooks.Add(nothing);
            Worksheet worksheet = (Worksheet)workBook.Sheets[1];
            worksheet.Name = "Sheet1";

            worksheet.SaveAs(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing);
            workBook.Close(false, Type.Missing, Type.Missing);
            app.Quit();
        }

        //public static void DataTabletoExcel(System.Data.DataTable tmpDataTable, string strFileName)
        //{
        //    if (tmpDataTable == null)

        //        return;
        //    int rowNum = tmpDataTable.Rows.Count;
        //    int columnNum = tmpDataTable.Columns.Count;
        //    int rowIndex = ExcelToDataTable(strFileName).Rows.Count + 1;
        //    int columnIndex = 0;
        //    Application xlApp = new Application
        //    {
        //        DefaultFilePath = "",
        //        DisplayAlerts = true,
        //        SheetsInNewWorkbook = 1
        //    };
        //    Workbook xlBook = xlApp.Workbooks.Add(true);
        //    //将DataTable的列名导入Excel表第一行
        //    foreach (DataColumn dc in tmpDataTable.Columns)
        //    {
        //        columnIndex++;
        //        xlApp.Cells.NumberFormat = "@"; //  如果数据中存在数字类型 可以让它变文本格式显示
        //        xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
        //    }
        //    //将DataTable中的数据导入Excel中
        //    for (int i = 0; i < rowNum; i++)
        //    {
        //        rowIndex++;
        //        columnIndex = 0;
        //        for (int j = 0; j < columnNum; j++)
        //        {
        //            columnIndex++;

        //            xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();
        //        }
        //    }

        //    xlBook.SaveCopyAs(strFileName);
        //    xlBook.Close(false);
        //}
        private void InitDataTable()
        {
        }

        public static void DataTabletoExcel(System.Data.DataTable tmpDataTable, string strFileName)
        {
            if (tmpDataTable == null)

                return;

            System.Data.DataTable sourceDataTable = ExcelToDataTable(strFileName);

            if (sourceDataTable.Rows.Count > 0)
            {
                sourceDataTable.Merge(tmpDataTable);
                tmpDataTable = sourceDataTable;
            }

            int rowNum = tmpDataTable.Rows.Count;
            int columnNum = tmpDataTable.Columns.Count;
            int rowIndex = 1;
            int columnIndex = 0;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            xlApp.DefaultFilePath = "";
            xlApp.DisplayAlerts = true;
            xlApp.SheetsInNewWorkbook = 1;
            Workbook xlBook = xlApp.Workbooks.Add(true);
            //将DataTable的列名导入Excel表第一行
            foreach (DataColumn dc in tmpDataTable.Columns)
            {
                columnIndex++;
                xlApp.Cells.NumberFormat = "@"; //  如果数据中存在数字类型 可以让它变文本格式显示
                xlApp.Cells[rowIndex, columnIndex] = dc.ColumnName;
            }
            //将DataTable中的数据导入Excel中
            for (int i = 0; i < rowNum; i++)
            {
                rowIndex++;
                columnIndex = 0;
                for (int j = 0; j < columnNum; j++)
                {
                    columnIndex++;

                    xlApp.Cells[rowIndex, columnIndex] = tmpDataTable.Rows[i][j].ToString();
                }
            }
            try
            {
                xlBook.SaveCopyAs(strFileName);
                xlBook.Close(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}