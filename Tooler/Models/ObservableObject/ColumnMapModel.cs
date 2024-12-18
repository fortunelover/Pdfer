using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tooler.Models
{
    public class ColumnMapModel : ObservableObject
    {
        private string _databaseColumnName;
        public string DatabaseColumnName
        {
            get => _databaseColumnName;
            set => SetProperty(ref _databaseColumnName, value);
        }

        private string _databaseColumnDesc;
        public string DatabaseColumnDesc
        {
            get => _databaseColumnDesc;
            set => SetProperty(ref _databaseColumnDesc, value);
        }

        private int? _databaseColumnMaxLength;
        public int? DatabaseColumnMaxLength
        {
            get => _databaseColumnMaxLength;
            set => SetProperty(ref _databaseColumnMaxLength, value);
        }

        private string _excelColumnName;
        public string ExcelColumnName
        {
            get => _excelColumnName;
            set => SetProperty(ref _excelColumnName, value);
        }

        private Type _dataType;
        public Type DataType
        {
            get => _dataType;
            set => SetProperty(ref _dataType, value);
        }

        public object _defaultValue;
        public object DefaultValue
        {
            get => _defaultValue;
            set => SetProperty(ref _defaultValue, value);
        }

        private bool _isNullable;
        public bool IsNullable
        {
            get => _isNullable;
            set => SetProperty(ref _isNullable, value);
        }


        private bool _isMap;
        public bool IsMap
        {
            get => _isMap;
            set => SetProperty(ref _isMap, value);
        }
    }
}
