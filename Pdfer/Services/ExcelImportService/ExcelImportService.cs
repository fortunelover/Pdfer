using Microsoft.Data.SqlClient;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tooler.Common.Utils.Extensions;
using Tooler.Helper;
using Tooler.Models;

namespace Tooler.Services
{
    public partial class ExcelImportService
    {
        public static SqlSugarScope db;
        public static void SetDb(string conn)
        {
            db = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = conn,
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true,
                ConfigId = Guid.NewGuid(),
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        if (c.PropertyType.IsGenericType && c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            p.IsNullable = true;
                        }
                    }
                },
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoRemoveDataCache = true
                }
            });
        }

        /// <summary>
        /// 获取数据库表结构
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static async Task<List<ColumnMapModel>> GetDbTableColumnMapModelAsync(string tableName)
        {
            var models = new List<ColumnMapModel>();
            var sql = GetTableColumnInfoSql(tableName);
            var result = await db.Ado.SqlQueryAsync<dynamic>(sql);
            foreach (var item in result)
            {
                string strIsNullable = Convert.ToString(item.IS_NULLABLE);
                string strDataType = Convert.ToString(item.DATA_TYPE);
                ColumnMapModel model = new ColumnMapModel()
                {
                    DatabaseColumnName = item.COLUMN_NAME,
                    DatabaseColumnDesc = item.COLUMN_DESCRIPTION,
                    DataType = GetCSharpDataTypeBySqlType(strDataType),
                    DatabaseColumnMaxLength = item.CHARACTER_MAXIMUM_LENGTH,
                    IsNullable = strIsNullable.Equals("YES"),
                    IsMap = true,
                };
                models.Add(model);
            }
            return models;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapModel"></param>
        /// <param name="filePath"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="msg"></param>
        /// <exception cref="Exception"></exception>
        public static string ExcelImportDb(IEnumerable<ColumnMapModel> mapModel, string tableName, string filePath, int sheetIndex)
        {
            var msg = string.Empty;
            if (!System.IO.File.Exists(filePath))
                throw new Exception("Excel路径不正确");
            //用于支持gb2312    
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            //DataTable的列名和excel的列名对应字典

            //读取数据到DataTable，参数依此是（excel文件路径，列名所在行，sheet索引）
            var columnMap = mapModel.Where(a => a.IsMap == true);
            var columnMapExcel = mapModel.Where(a => !a.ExcelColumnName.IsNullOrEmptyOrWhiteSpazeOrCountZero());
            DataTable dt = ImportExceltoDt(filePath, columnMapExcel.ToList(), sheetIndex, 0);

            var dbColumns = columnMap.Select(x => x.DatabaseColumnName.ToString());
            RemoveRedundantColumn(ref dt, dbColumns);

            AddMissingColumnsToDataTable(dt, columnMap.ToList(), ref msg);//添加Excel中缺少的列
            var resultDt = UpdateDataTable(dt, mapModel);//根据数据库表刷新格式

            InsertDataFromDataTable(resultDt, tableName);

            msg += "导入成功";
            return msg;
        }

        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public async static Task<string> GetEntityCodeStrAsync(string tableName)
        {
            var sb = new StringBuilder();
            var sql = GetTableColumnInfoSql(tableName);
            var result = await db.Ado.SqlQueryAsync<dynamic>(sql);

            foreach (var item in result)
            {
                string strIsNullable = Convert.ToString(item.IS_NULLABLE);
                string sqlDataType = Convert.ToString(item.DATA_TYPE);
                bool isNullable = strIsNullable.Equals("YES");
                var propertyType = GetCSharpDataTypeNullable(sqlDataType, isNullable);
                var codeLine = $@"
/// <summary>
/// {item.COLUMN_DESCRIPTION}
/// </summary>
[DisplayName(""{item.COLUMN_DESCRIPTION}"")]
public {propertyType} {item.COLUMN_NAME} {{ get; set; }}
";
                sb.Append(codeLine);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="tableName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<string> DataTableImportDbTempTableAsync(DataTable dt, string tableName, CancellationToken cancellationToken)
        {
            var result = await ImportTempTableAsync(dt, tableName, cancellationToken);
            var msg = $"DataTable导入数据库临时表[{tableName}]成功";
            return msg;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filePath"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataTable ExcelImportDataTable(string tableName, string filePath, int sheetIndex, CancellationToken cancellationToken)
        {
            if (!System.IO.File.Exists(filePath))
                throw new Exception("Excel路径不正确");
            //用于支持gb2312    
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //读取数据到DataTable，参数依此是（excel文件路径，列名所在行，sheet索引）
            DataTable dt = ExcelHelper.ExcelImportDataTable(filePath, sheetIndex, cancellationToken);
            return dt;
        }
    }
}
