using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.Windows.Interop;
using System.Windows;
using Tooler.Models;
using Tooler.Services;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Tooler.Common.Utils.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using Tooler.ViewModels;

namespace DataBase.ViewModels
{
    public partial class ExcelImportViewModel : ExcelDbViewModel, IRecipient<string>
    {
        public ExcelImportViewModel()
        {
            InitConfig();
            WeakReferenceMessenger.Default.Register(this, "ButtonEnable");
        }

        public void Receive(string message) 
        {
            this.BtnEnabled = message.Equals("Enable");
        }

        private void InitConfig()
        {
            ConfigService.LoadConfigDbConn();
            ConfigService.LoadConfigColumnMap();
            this.DbConnCollection = ConfigService._dbConnModelList;
            this.DbConn = DbConnCollection.FirstOrDefault();
            this.DbExcelMapCollection = ConfigService._columnMapModelList?.Where(a=>a.DependencyDbConfigID==DbConn.ID)?.ToObservableCollection();
            this.DbExcelMap = DbExcelMapCollection.FirstOrDefault();
            ExcelImportService.SetDb(DbConn?.DbConnStr);
            this.BtnEnabled = true;
        }
    }
}
