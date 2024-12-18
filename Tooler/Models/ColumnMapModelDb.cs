using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tooler.Models
{
    public class ColumnMapModelDb
    {
        /// <summary>
        /// 本地唯一Id
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DatabaseColumnName { get; set; }
        public string DatabaseColumnDesc { get; set; }
        public string ExcelColumnName { get; set; }

        public Type DataType { get; set; }

        public object DefaultValue { get; set; }

        public bool IsNullable { get; set; }

        public bool IsMap { get; set; }

    }
}
