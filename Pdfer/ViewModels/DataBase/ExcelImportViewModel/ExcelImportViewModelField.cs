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
using CommunityToolkit.Mvvm.Messaging;
using Tooler.ViewModels;

namespace DataBase.ViewModels
{
    public partial class ExcelImportViewModel : ExcelDbViewModel, IRecipient<string>
    {
       /* private ObservableCollection<DbConnModel> _dbConnCollection;
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
        }*/


        #region 字段


        /*private string _currentSheet;
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

        private bool _btnEnabled;
        public bool BtnEnabled
        {
            get => _btnEnabled;
            set => SetProperty(ref _btnEnabled, value);
        }*/

        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetProperty(ref _isPopupOpen, value);
        }

        private bool _isExcelPopupOpen;
        public bool IsExcelPopupOpen
        {
            get => _isExcelPopupOpen;
            set => SetProperty(ref _isExcelPopupOpen, value);
        }

        /*private Boolean _progressRingVisibale;
        public Boolean ProgressRingVisibale
        {
            get => _progressRingVisibale;
            set => SetProperty(ref _progressRingVisibale, value);
        }*/

        private ObservableCollection<DbExcelMapModel> _dbExcelMapCollection = new ObservableCollection<DbExcelMapModel>();
        public ObservableCollection<DbExcelMapModel> DbExcelMapCollection
        {
            get => _dbExcelMapCollection;
            set => SetProperty(ref _dbExcelMapCollection, value);
        }
        private DbExcelMapModel _dbExcelMapModel = new DbExcelMapModel();
        public DbExcelMapModel DbExcelMap
        {
            get => _dbExcelMapModel;
            set => SetProperty(ref _dbExcelMapModel, value);
        }


        #endregion

    }
}
