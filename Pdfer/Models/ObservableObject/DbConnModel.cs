using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tooler.Models
{
    public class DbConnModel : ObservableObject
    {
        public string ID;

        private string _configName;
        public string ConfigName
        {
            get => _configName;
            set 
            {
                SetProperty(ref _configName, value); 
            }
        }

        public string DbConnStr;

        private string _dataSource = "";
        public string DataSource
        {
            get => _dataSource;
            set
            {
                SetProperty(ref _dataSource, value);
                DbConnStr = string.Format($@"User ID={UserID};Data Source={DataSource};Password={Password};Initial Catalog={InitialCatalog};TrustServerCertificate=true");
            }
        }



        private string _initialCatalog = "";
        public string InitialCatalog
        {
            get => _initialCatalog;
            set
            {
                SetProperty(ref _initialCatalog, value);
                DbConnStr = string.Format($@"User ID={UserID};Data Source={DataSource};Password={Password};Initial Catalog={InitialCatalog};TrustServerCertificate=true");
            }
        }

        private string _userID = "";
        public string UserID
        {
            get => _userID;
            set
            {
                SetProperty(ref _userID, value);
                DbConnStr = string.Format($@"User ID={UserID};Data Source={DataSource};Password={Password};Initial Catalog={InitialCatalog};TrustServerCertificate=true");
            }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                DbConnStr = string.Format($@"User ID={UserID};Data Source={DataSource};Password={Password};Initial Catalog={InitialCatalog};TrustServerCertificate=true");
            }
        }
    }
}
