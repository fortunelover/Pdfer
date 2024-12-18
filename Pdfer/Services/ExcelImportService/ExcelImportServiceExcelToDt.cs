using Microsoft.Data.SqlClient;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tooler.Helper;
using Tooler.Models;

namespace Tooler.Services
{
    public partial class ExcelImportService
    {
        /// <summary>
        /// 读取Excel文件特定名字sheet的内容到DataTable
        /// </summary>
        /// <param name="strFileName">excel文件路径</param>
        /// <param name="sheet">需要导出的sheet</param>
        /// <param name="HeaderRowIndex">列头所在行号，-1表示没有列头</param>
        /// <param name="dir">excel列名和DataTable列名的对应字典</param>
        /// <returns></returns>
        public static DataTable ImportExceltoDt(string strFileName, List<ColumnMapModel> model, int SheetIndex = 0, int HeaderRowIndex = 1)
        {
            DataTable table = new DataTable();
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                if (file.Length > 0)
                {
                    IWorkbook wb = WorkbookFactory.Create(file);
                    ISheet isheet = wb.GetSheetAt(SheetIndex);
                    table = ImportDt(isheet, model, HeaderRowIndex);
                }
            }
            return table;
        }

        static DataTable ImportDt(ISheet sheet, List<ColumnMapModel> model, int HeaderRowIndex)
        {
            DataTable table = new DataTable();
            IRow headerRow;
            int cellCount;
            try
            {
                //没有标头或者不需要表头用excel列的序号（1,2,3..）作为DataTable的列名
                if (HeaderRowIndex < 0)
                {
                    headerRow = sheet.GetRow(0);
                    cellCount = headerRow.LastCellNum;

                    for (int i = headerRow.FirstCellNum; i <= cellCount; i++)
                    {
                        DataColumn column = new DataColumn(Convert.ToString(i));
                        table.Columns.Add(column);
                    }
                }
                //有表头，使用表头做为DataTable的列名
                else
                {
                    headerRow = sheet.GetRow(HeaderRowIndex);
                    cellCount = headerRow.LastCellNum;
                    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    {
                        //如果excel某一列列名不存在：以该列的序号作为Datatable的列名，如果DataTable中包含了这个序列为名的列，那么列名为重复列名+序号
                        if (headerRow.GetCell(i) == null)
                        {
                            if (table.Columns.IndexOf(Convert.ToString(i)) > 0)
                            {
                                DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(Convert.ToString(i));
                                table.Columns.Add(column);
                            }

                        }
                        //excel中的某一列列名不为空，但是重复了：对应的Datatable列名为“重复列名+序号”
                        else if (table.Columns.IndexOf(headerRow.GetCell(i).ToString()) > 0)
                        {
                            DataColumn column = new DataColumn(Convert.ToString("重复列名" + i));
                            table.Columns.Add(column);
                        }
                        else
                        //正常情况，列名存在且不重复：用excel中的列名作为datatable中对应的列名
                        {
                            /*string colName = dir.Where(s => s.Value == headerRow.GetCell(i).ToString()).First().Key;*/
                            string colNameValue = headerRow.GetCell(i).ToString();
                            string colNameKey = string.Empty;
                            var keys = model.Where(a => a.ExcelColumnName.Equals(colNameValue) && (!string.IsNullOrWhiteSpace(a.ExcelColumnName)));

                            if (keys.Any())
                            {
                                colNameKey = keys.FirstOrDefault().DatabaseColumnName.ToString();
                                DataColumn column = new DataColumn(colNameKey);
                                table.Columns.Add(column);
                            }
                            else
                            {
                                DataColumn column = new DataColumn(colNameValue);
                                table.Columns.Add(column);
                            }
                        }
                    }
                }
                int rowCount = sheet.LastRowNum;
                for (int i = (HeaderRowIndex + 1); i <= sheet.LastRowNum; i++)//excel行遍历
                {
                    try
                    {
                        IRow row;
                        if (sheet.GetRow(i) == null)//如果excel有空行，则添加缺失的行
                        {
                            row = sheet.CreateRow(i);
                        }
                        else
                        {
                            row = sheet.GetRow(i);
                        }

                        DataRow dataRow = table.NewRow();

                        for (int j = row.FirstCellNum; j <= cellCount; j++)//excel列遍历
                        {
                            try
                            {
                                if (row.GetCell(j) != null)
                                {
                                    switch (row.GetCell(j).CellType)
                                    {
                                        case CellType.String://字符串
                                            string str = row.GetCell(j).StringCellValue;
                                            if (str != null && str.Length > 0)
                                            {
                                                dataRow[j] = str.ToString();
                                            }
                                            else
                                            {
                                                dataRow[j] = string.Empty;
                                            }
                                            break;
                                        case CellType.Numeric://数字
                                            if (DateUtil.IsCellDateFormatted(row.GetCell(j)))//时间戳数字
                                            {
                                                dataRow[j] = DateTime.FromOADate(row.GetCell(j).NumericCellValue);
                                            }
                                            else
                                            {
                                                //var scientificNotationValue = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                //var parsedValue = ConvertToNormalDouble(scientificNotationValue);
                                                //dataRow[j] = parsedValue;
                                                //string decimalNotationValue = parsedValue.ToString(); // 转换为普通小数表示形式
                                                dataRow[j] = Convert.ToDecimal(row.GetCell(j).NumericCellValue);
                                            }
                                            break;
                                        case CellType.Boolean:
                                            dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                            break;
                                        case CellType.Error:
                                            dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                            break;
                                        case CellType.Formula://公式
                                            switch (row.GetCell(j).CachedFormulaResultType)
                                            {
                                                case CellType.String:
                                                    string strFORMULA = row.GetCell(j).StringCellValue;
                                                    if (strFORMULA != null && strFORMULA.Length > 0)
                                                    {
                                                        dataRow[j] = strFORMULA.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[j] = null;
                                                    }
                                                    break;
                                                case CellType.Numeric:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).NumericCellValue);
                                                    break;
                                                case CellType.Boolean:
                                                    dataRow[j] = Convert.ToString(row.GetCell(j).BooleanCellValue);
                                                    break;
                                                case CellType.Error:
                                                    dataRow[j] = ErrorEval.GetText(row.GetCell(j).ErrorCellValue);
                                                    break;
                                                default:
                                                    dataRow[j] = "";
                                                    break;
                                            }
                                            break;
                                        default:
                                            dataRow[j] = "";
                                            break;
                                    }
                                }
                                else
                                {
                                    if (dataRow.ItemArray.Count() > j)
                                        dataRow[j] = "";
                                }
                            }
                            catch (Exception exception)
                            {
                                //loger.Error(exception.ToString());
                            }
                        }
                        table.Rows.Add(dataRow);
                    }
                    catch (Exception exception)
                    {
                        //loger.Error(exception.ToString());
                    }
                }
            }
            catch (Exception exception)
            {
                //loger.Error(exception.ToString());
            }
            return table;
        }

    }
}
