using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tooler.Common;
using Tooler.Common.Utils.Extensions;

namespace Tooler.Helper
{
    public static partial class ExcelHelper
    {
        /// <summary>
        /// Excel导入到DataTable
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="SheetIndex"></param>
        /// <param name="HeaderRowIndex"></param>
        /// <returns></returns>
        public static DataTable ExcelImportDataTable(string strFileName, int SheetIndex, CancellationToken cancellationToken, int HeaderRowIndex = 0)
        {
            DataTable table = new DataTable();
            using (FileStream file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                IWorkbook wb = WorkbookFactory.Create(file);
                ISheet isheet = wb.GetSheetAt(SheetIndex);
                table = SheetImportDataTable(isheet, cancellationToken, HeaderRowIndex);
            }
            return table;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="headerRowIndex"></param>
        /// <returns></returns>
        public static DataTable SheetImportDataTable(ISheet sheet, CancellationToken cancellationToken, int headerRowIndex)
        {
            DataTable table = new DataTable(); // 创建一个新的数据表

            // 创建数据表的列，依据headerRowIndex来判断是使用索引还是表头行
            if (headerRowIndex < 0)
            {
                // 使用默认索引创建列
                AddColumnsFromIndexes(table, sheet.GetRow(0));
            }
            else
            {
                // 使用表头行的数据创建列
                AddColumnsFromHeaderRow(table, sheet.GetRow(headerRowIndex));
            }

            // 遍历数据行，从headerRowIndex的下一行开始
            for (int i = headerRowIndex + 1; i <= sheet.LastRowNum; i++)
            {
                cancellationToken.ThrowIfCancellationRequested(); // 检查是否被取消



                // 获取当前行，如果当前行为空则创建一行
                IRow row = sheet.GetRow(i) ?? sheet.CreateRow(i);

                if (IsRowEmpty(row))
                {
                    // 处理空行的逻辑
                    // 例如，可以选择跳过该行或记录空行
                    CommonFunc.Log($"{i}行为空行，停止导入到DataTable");
                    break; // 跳过空行
                }


                DataRow dataRow = table.NewRow(); // 创建一个新的数据行

                // 遍历当前行的每个单元格
                for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                {
                    cancellationToken.ThrowIfCancellationRequested(); // 检查是否被取消

                    // 获取单元格的值并进行处理
                    object cellValue = GetCellValue(row.GetCell(j));
                    dataRow[j] = cellValue ?? string.Empty; // 如果值为null则设置为空字符串
                }

                // 将构建好的数据行添加到数据表中
                table.Rows.Add(dataRow);
            }

            return table; // 返回填充好的数据表
        }


        // 从行索引创建数据表的列
        private static void AddColumnsFromIndexes(DataTable table, IRow headerRow)
        {
            int cellCount = headerRow.LastCellNum; // 获取单元格总数
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                // 使用索引作为列名
                string columnName = Convert.ToString(i);
                table.Columns.Add(columnName); // 添加列
            }
        }


        // 从表头行创建数据表的列
        private static void AddColumnsFromHeaderRow(DataTable table, IRow headerRow)
        {
            int cellCount = headerRow.LastCellNum; // 获取单元格总数
            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                // 获取单元格内容作为列名，并确保其唯一性
                string columnName = headerRow.GetCell(i)?.ToString() ?? Convert.ToString(i);
                columnName = columnName.RemoveString("(", ")", "-","（","）","，");
                columnName = GetUniqueColumnName(table, columnName, i);
                table.Columns.Add(columnName); // 添加列
            }
        }

        // 确保列名的唯一性
        private static string GetUniqueColumnName(DataTable table, string columnName, int index)
        {
            // 如果列名为空，则使用索引
            if (string.IsNullOrEmpty(columnName))
            {
                columnName = Convert.ToString(index);
            }

            // 如果数据表中已经存在该列名，则添加后缀以保持唯一性
            if (table.Columns.Contains(columnName))
            {
                int suffix = 1;
                while (table.Columns.Contains(columnName + "重复列名" + suffix))
                {
                    suffix++; // 增加后缀
                }
                columnName += "重复列名" + suffix; // 更新列名
            }

            return columnName; // 返回唯一的列名
        }


        // 获取单元格的值，并处理不同类型
        private static object GetCellValue(ICell cell)
        {
            if (cell != null) // 如果单元格不为空
            {
                switch (cell.CellType) // 根据单元格类型处理值
                {
                    case CellType.String:
                        return cell.StringCellValue; // 字符串类型
                    case CellType.Numeric:
                        // 检查单元格是否为日期格式，进行相应处理
                        return DateUtil.IsCellDateFormatted(cell) ? (object)DateTime.FromOADate(cell.NumericCellValue) : cell.NumericCellValue;
                    case CellType.Boolean:
                        return cell.BooleanCellValue.ToString(); // 布尔值
                    case CellType.Error:
                        return ErrorEval.GetText(cell.ErrorCellValue); // 错误值
                    case CellType.Formula:
                        return GetCellValue(cell); // 递归处理公式
                    default:
                        return string.Empty; // 其他类型返回空字符串
                }
            }
            return null; // 如果单元格为空，返回null
        }

        // 检查行是否为空
        private static bool IsRowEmpty(IRow row)
        {
            if (row == null) return true; // 如果行为空，返回true

            for (int i = row.FirstCellNum; i < row.LastCellNum; i++)
            {
                ICell cell = row.GetCell(i);
                if (cell != null && cell.CellType != CellType.Blank) // 如果有非空单元格，返回false
                {
                    return false;
                }
            }

            return true; // 所有单元格均为空，返回true
        }

    }
}
