using Newtonsoft.Json;
using NPOI.POIFS.FileSystem;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tooler.Models;

namespace Tooler.Services
{
    public static class ConfigService
    {
        public static ObservableCollection<DbConnModel>? _dbConnModelList = new ObservableCollection<DbConnModel>();
        public static ObservableCollection<DbExcelMapModel>? _columnMapModelList = new ObservableCollection<DbExcelMapModel>();

        private static readonly string _pathDbConn = @"Config/DbConn.json";
        private static readonly string _pathDbExcelMap = @"Config/DbExcelMap.json";

        public static void LoadConfig()
        {
            LoadConfigDbConn();
            LoadConfigColumnMap();
        }


        public static void LoadConfigDbConn()
        {
            if (System.IO.File.Exists(_pathDbConn))
            {
                string json = System.IO.File.ReadAllText(_pathDbConn);
                _dbConnModelList = JsonConvert.DeserializeObject<ObservableCollection<DbConnModel>>(json);
            }
        }
        public static void LoadConfigColumnMap()
        {
            if (System.IO.File.Exists(_pathDbExcelMap))
            {
                string json = System.IO.File.ReadAllText(_pathDbExcelMap);
                _columnMapModelList = JsonConvert.DeserializeObject<ObservableCollection<DbExcelMapModel>>(json);
            }
        }


        public static void SaveConfigDbConn()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            var json = JsonConvert.SerializeObject(_dbConnModelList, settings);
            System.IO.File.WriteAllText(_pathDbConn, json);
        }
        public static void SaveConfigColumnMap()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings { Formatting = Formatting.Indented };
            var json = JsonConvert.SerializeObject(_columnMapModelList, settings);
            System.IO.File.WriteAllText(_pathDbExcelMap, json);
        }

        internal static readonly List<string> SqliteConnectionStrings = new();
        public static bool AddConnectionString(string str, string configId, bool needCodeFirst)
        {
            SqliteConnectionStrings.Add(str);
            return AddDb(str, configId, needCodeFirst);
        }
        public static bool AddDbFile(string filePath, string configId, bool needCodeFirst)
        {
            SqliteConnectionStrings.Add(@"DataSource=" + filePath);
            return AddDb(@"DataSource=" + filePath, configId, needCodeFirst);
        }


        /// <summary>
        /// 按数据库连接字符串创建多仓库数据库
        /// </summary>
        /// <param name="connet"></param>
        /// <param name="configId"></param>
        internal static bool AddDb(string connet, string configId, bool needCodeFirst)
        {
            if (!Dbs.ContainsKey(configId))
            {
                Dbs.Add(configId, new SqlSugarScope(new ConnectionConfig()
                {
                    ConnectionString = connet,
                    DbType = DbType.Sqlite,
                    IsAutoCloseConnection = true,
                    ConfigId = configId,
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
                }
                ));
                if (CodeFirst(configId))
                {
                    return true;
                }
                else
                {
                    Dbs.Remove(configId);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 管理所有仓库数据库连接
        /// </summary>
        internal static readonly Dictionary<string, SqlSugarScope> Dbs = new();
        /// <summary>
        /// 是否有数据库连接
        /// </summary>
        internal static bool IsEmpty
        {
            get => Dbs.Count == 0;
        }
        /// <summary>
        /// 获取仓库数据库Scope实例
        /// 不要自己到Dbs拿，统一使用此方法，避免以后改回ORM的多租户模式
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        internal static SqlSugarScope GetConnection(string dbId)
        {
            if (!string.IsNullOrEmpty(dbId) && Dbs.TryGetValue(dbId, out SqlSugarScope scope))
            {
                return scope;
            }
            else
            {
                throw new Exception("Database does not exist");
            }
        }
        /// <summary>
        /// 仓库数据库自动建表建库
        /// </summary>
        /// <param name="dbId"></param>
        /// <returns></returns>
        private static bool CodeFirst(string dbId)
        {
            if (GetConnection(dbId).DbMaintenance.CreateDatabase())
            {
                List<Type> types = new List<Type>();
                types.Add(typeof(ColumnMapModelDb));
                GetConnection(dbId).CodeFirst.InitTables(types.ToArray());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}