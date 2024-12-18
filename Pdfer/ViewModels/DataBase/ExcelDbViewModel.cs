using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tooler.Common;
using Tooler.Common.Utils.Extensions;
using Tooler.Helper;
using Tooler.Models;
using Tooler.Services;

namespace Tooler.ViewModels
{
    public class ExcelDbViewModel : ObservableObject
    {

        #region Command

       
        public ICommand NewDbConfigCommand => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                DbConnModel model = new DbConnModel();
                if (!DbConn.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                {
                    model.CopyFrom<DbConnModel>(DbConn);
                    model.ConfigName = $"{model.ConfigName} - 副本";
                    model.ID = Guid.NewGuid().ToString();
                }
                DbConnCollection.Add(model);
                DbConn = model;
            }, "新增数据库配置");
        });

        public ICommand DeleteDbConfigCommand => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                DbConnCollection.Remove(DbConn);
                DbConn = DbConnCollection.FirstOrDefault();
                ExcelImportService.SetDb(DbConn?.DbConnStr);
            }, "删除数据库配置");
        });

        public ICommand SaveDbConfigCommand => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                //var x = this.DbConnCollection;
                //var b = this.DbConn;
                ConfigService.SaveConfigDbConn();
                ExcelImportService.SetDb(DbConn?.DbConnStr);
            }, "保存数据库配置");
        });




        #endregion

        #region Field
        private ObservableCollection<DbConnModel> _dbConnCollection;
        public ObservableCollection<DbConnModel> DbConnCollection
        {
            get => _dbConnCollection;
            set => SetProperty(ref _dbConnCollection, value);
        }

        private DbConnModel _dbConn;
        public DbConnModel DbConn
        {
            get => _dbConn;
            set => SetProperty(ref _dbConn, value);
        }
        private Boolean _progressRingVisibale;
        public Boolean ProgressRingVisibale
        {
            get => _progressRingVisibale;
            set => SetProperty(ref _progressRingVisibale, value);
        }


        private bool _btnEnabled;
        public bool BtnEnabled
        {
            get => _btnEnabled;
            set => SetProperty(ref _btnEnabled, value);
        }

        private string _currentSheet;
        public string CurrentSheet
        {
            get => _currentSheet;
            set => SetProperty(ref _currentSheet, value);
        }
        private ObservableCollection<string> _sheets;
        public ObservableCollection<string> Sheets
        {
            get => _sheets;
            set => SetProperty(ref _sheets, value);
        }

        private string _excelPath;
        public string ExcelPath
        {
            get => _excelPath;
            set => SetProperty(ref _excelPath, value);
        }
        #endregion

    }
}
