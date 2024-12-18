using Microsoft.Data.SqlClient;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tooler.Helper;
using Tooler.Models;

namespace Tooler.Services
{
    public partial class ExcelImportService
    {
        /// <summary>
        /// 数据库类型转C#类型
        /// </summary>
        /// <param name="sqlDataType"></param>
        /// <returns></returns>
        private static Type GetCSharpDataTypeBySqlType(string sqlDataType)
        {
            switch (sqlDataType.ToLower())
            {
                case "int":
                    return typeof(int);
                case "bigint":
                    return typeof(long);
                case "smallint":
                    return typeof(short);
                case "tinyint":
                    return typeof(byte);
                case "float":
                    return typeof(float);
                case "real":
                    return typeof(double);
                case "decimal":
                case "numeric":
                    return typeof(decimal);
                case "money":
                case "smallmoney":
                    return typeof(decimal);
                case "datetime":
                case "smalldatetime":
                    return typeof(DateTime);
                case "date":
                    return typeof(DateTime);
                case "time":
                    return typeof(TimeSpan);
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                    return typeof(string);
                case "bit":
                    return typeof(bool);
                case "binary":
                case "varbinary":
                case "image":
                    return typeof(byte[]);
                default:
                    return typeof(object);
            }
        }

        private static string GetCSharpDataTypeNullable(string sqlDataType, bool isNullable)
        {
            switch (sqlDataType.ToLower())
            {
                case "int":
                    return isNullable ? "int?" : "int";
                case "bigint":
                    return isNullable ? "long?" : "long";
                case "smallint":
                    return isNullable ? "short?" : "short";
                case "tinyint":
                    return isNullable ? "byte?" : "byte";
                case "float":
                    return isNullable ? "float?" : "float";
                case "real":
                    return isNullable ? "double?" : "double";
                case "decimal":
                case "numeric":
                    return isNullable ? "decimal?" : "decimal";
                case "money":
                case "smallmoney":
                    return isNullable ? "decimal?" : "decimal";
                case "datetime":
                case "smalldatetime":
                    return isNullable ? "DateTime?" : "DateTime";
                case "date":
                    return isNullable ? "DateTime?" : "DateTime";
                case "time":
                    return isNullable ? "TimeSpan?" : "TimeSpan";
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                case "ntext":
                    return isNullable ? "string?" : "string";
                case "bit":
                    return isNullable ? "bool?" : "bool";
                case "binary":
                case "varbinary":
                case "image":
                    return "byte[]";
                default:
                    return "object";
            }
        }

        private static string GetSqlServerDataType(string sqlDataType)
        {
            switch (sqlDataType.ToLower())
            {
                case "int":
                    return "int";
                case "long":
                    return "bigint";
                case "short":
                    return "smallint";
                case "byte":
                    return "tinyint";
                case "float":
                    return "float";
                case "double":
                    return "real";
                case "decimal":
                    return "Decimal";
                case "dateTime":
                    return "Datetime";
                case "date":
                    return "DateTime";
                case "TimeSpan":
                    return "time";
                case "string":
                    return "nvarchar(MAX)";
                case "bool":
                    return "bit";
                default:
                    return "Nvarchar";
            }
        }

        private static void AddMissingColumnsToDataTable(DataTable dt, IEnumerable<ColumnMapModel> models, ref string msg)
        {
            StringBuilder sb = new StringBuilder();


            foreach (var model in models)
            {
                if (!dt.Columns.Contains(model.DatabaseColumnName))
                {
                    var newColumn = new DataColumn();
                    newColumn.DataType = model.DataType;
                    newColumn.DefaultValue = model.DefaultValue;
                    newColumn.ColumnName = model.DatabaseColumnName;
                    dt.Columns.Add(newColumn);
                    sb.AppendLine($@"Excel缺少列[{model.DatabaseColumnName}],已设置默认值{model.DefaultValue}");
                }
            }
            msg += sb.ToString();
        }


        /// <summary>
        /// 删除Excel导出的DataTable中不在字典中的列
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="columns"></param>
        private static void RemoveRedundantColumn(ref DataTable dt, IEnumerable<string> columns)
        {
            var dirValueRemoveList = new List<string>();
            var dirValueDtColList = new List<string>();
            //删除Excel导出的DataTable中不在字典中的列
            foreach (var item in dt.Columns)
            {
                if (!columns.Contains(item.ToString()))
                {
                    dirValueRemoveList.Add(item.ToString());
                }
                else
                {
                    dirValueDtColList.Add(item.ToString());
                }
            }
            foreach (var item in dirValueRemoveList)
            {
                dt.Columns.Remove(item.ToString());
            }
        }

        private static DataTable UpdateDataTable(DataTable argDataTable, IEnumerable<ColumnMapModel> mapModel)
        {
            DataTable dtResult = new DataTable();
            //克隆表结构
            dtResult = argDataTable.Clone();
            foreach (DataColumn item in dtResult.Columns)
            {
                var dataType = mapModel.FirstOrDefault(a => a.DatabaseColumnName == item.ColumnName)?.DataType;
                if (dataType != null)
                    item.DataType = dataType;
            }
            var currentRow = 2;
            var errMsg = "";
            foreach (DataRow row in argDataTable.Rows)
            {
                DataRow rowNew = dtResult.NewRow();
                foreach (DataColumn item in dtResult.Columns)
                {
                    try
                    {
                        rowNew[item.ColumnName] = row[item.ColumnName];
                    }
                    catch (Exception ex)
                    {
                        errMsg += $"行【{currentRow}】/列【{item.ColumnName}】导入失败：{row[item.ColumnName]}--{ex.Message}";
                    }

                }
                currentRow++;
                dtResult.Rows.Add(rowNew);
            }
            if (errMsg.Length > 0)
            {
                throw new Exception(errMsg);
            }
            return dtResult;
        }

        public static void InsertDataFromDataTable(DataTable dt, string tableName)
        {
            using (SqlConnection sqlcon = new SqlConnection(db.CurrentConnectionConfig.ConnectionString))
            {
                sqlcon.Open();
                using (SqlBulkCopy bulkcopy = new SqlBulkCopy(sqlcon))
                {
                    // 设置目标表名
                    bulkcopy.DestinationTableName = tableName;

                    // 将DataTable中的列映射到目标表的列
                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkcopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    // 批量插入数据
                    bulkcopy.WriteToServer(dt);
                }
            }
        }
        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="dt">更新的内容</param>
        /// <param name="tableName">目标表</param>
        /// <returns></returns>
        public static Boolean ImportTempTable(DataTable dt, string tempTableName)
        {
            using (SqlConnection connection = new SqlConnection(db.CurrentConnectionConfig.ConnectionString))
            {
                List<string> fieldList = new List<string>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var name = dt.Columns[i].ColumnName;
                    var dataType = GetSqlServerDataType(dt.Columns[i].DataType.Name);
                    fieldList.Add(name + " " + dataType);
                }
                var fieldStr = string.Join(",", fieldList);
                connection.Open();
                SqlCommand createTempTableCommand = new SqlCommand($"CREATE TABLE {tempTableName} ({fieldStr})", connection);
                createTempTableCommand.ExecuteNonQuery();
                // 使用SqlBulkCopy批量插入数据
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = $"{tempTableName}";
                    bulkCopy.WriteToServer(dt);
                }
                return true;
            }
        }

        public static async Task<bool> ImportTempTableAsync(DataTable dt, string tempTableName, CancellationToken cancellationToken)
        {
            using (SqlConnection connection = new SqlConnection(db.CurrentConnectionConfig.ConnectionString))
            {
                List<string> fieldList = new List<string>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    var name = dt.Columns[i].ColumnName;
                    var dataType = GetSqlServerDataType(dt.Columns[i].DataType.Name);
                    fieldList.Add(name + " " + dataType);
                }
                var fieldStr = string.Join(",", fieldList);

                await connection.OpenAsync(cancellationToken);

                using (SqlCommand createTempTableCommand = new SqlCommand($"CREATE TABLE {tempTableName} ({fieldStr})", connection))
                {
                    await createTempTableCommand.ExecuteNonQueryAsync(cancellationToken);
                }

                // 使用SqlBulkCopy批量插入数据
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = tempTableName;
                    await bulkCopy.WriteToServerAsync(dt, cancellationToken);
                }

                return true;
            }
        }



        public static string GetTableColumnInfoSql(string tableName)
        {
            var sql = $@"
SELECT c.name as COLUMN_NAME,
	   --c.max_length AS CHARACTER_MAXIMUM_LENGTH,
       iso.DATA_TYPE,
       iso.IS_NULLABLE,
       --c.is_nullable AS IS_NULLABLE,
       iso.CHARACTER_MAXIMUM_LENGTH,
	   --dc.definition AS COLUMN_DEFAULT,
	   ep.value AS COLUMN_DESCRIPTION
FROM   sys.columns c
       INNER JOIN sys.tables t ON c.object_id = t.object_id
       INNER JOIN INFORMATION_SCHEMA.COLUMNS iso ON iso.COLUMN_NAME = c.name AND iso.TABLE_NAME = '{tableName}'
       LEFT JOIN sys.extended_properties ep ON c.object_id = ep.major_id AND c.column_id = ep.minor_id AND ep.name = 'MS_Description'
       --LEFT JOIN sys.default_constraints dc ON c.object_id = dc.parent_object_id AND c.column_id = dc.parent_column_id
WHERE  t.name = '{tableName}'
ORDER  BY column_id 
";
            return sql;
        }
    }
}
