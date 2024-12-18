using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tooler.Models
{
    public class DbExcelMapModel : ObservableObject
    {
        private string _configName;
        public string ConfigName
        {
            get => _configName;
            set => SetProperty(ref _configName, value);
        }

        private string _table;
        public string Table
        {
            get => _table;
            set => SetProperty(ref _table, value);
        }

        /// <summary>
        /// 所属数据库
        /// </summary>
        public string? DependencyDbConfigID { get; set; }

        private ObservableCollection<ColumnMapModel> _columnMapCollection = new ObservableCollection<ColumnMapModel>();
        public ObservableCollection<ColumnMapModel> ColumnMapCollection
        {

            get => _columnMapCollection;
            set => SetProperty(ref _columnMapCollection, value);
        }
    }
}
