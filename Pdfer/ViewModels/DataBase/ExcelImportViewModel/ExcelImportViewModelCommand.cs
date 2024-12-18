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
using Tooler.Common;
using Tooler.Helper;
using Tooler.Common.Utils.Extensions;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Tooler.Models;
using Tooler.Services;
using CommunityToolkit.Mvvm.Messaging;
using Tooler.ViewModels;

namespace DataBase.ViewModels
{
    public partial class ExcelImportViewModel : ExcelDbViewModel, IRecipient<string>
    {
        public ICommand OpenPopupCommand => new RelayCommand(async () =>
        {
            IsPopupOpen = !IsPopupOpen;
        });
        public ICommand OpenExcelPopupCommand => new RelayCommand(async () =>
        {
            IsExcelPopupOpen = !IsExcelPopupOpen;
        });

        public ICommand ComboBoxSelectionChangedCommand => new RelayCommand<object>(async (parameter) =>
        {
            await CommonFunc.HandleExceptionAsync(async () =>
            {
                ExcelImportService.SetDb(DbConn?.DbConnStr);
                CommonFunc.Log($"数据库连接{(ExcelImportService.db.CurrentConnectionConfig == null ? "失败" : "成功")}");
                this.DbExcelMapCollection = ConfigService._columnMapModelList?.Where(a => a.DependencyDbConfigID == DbConn.ID)?.ToObservableCollection();
                this.DbExcelMap = DbExcelMapCollection.FirstOrDefault();
            }, "切换数据库配置");
        });

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


        public ICommand ImportExcelCommand => new RelayCommand(async () =>
        {
            await CommonFunc.HandleExceptionAsync(async () =>
            {
                var sheetIndex = Sheets.IndexOf(CurrentSheet);
                string msg = await Task.Run<string>(() => ExcelImportService.ExcelImportDb(DbExcelMap.ColumnMapCollection, DbExcelMap.Table, ExcelPath, sheetIndex));
            });
        });

        public ICommand EntityGeneratorCommand => new RelayCommand(async () =>
        {
            await CommonFunc.HandleExceptionAsync(async () =>
            {
                var result = await ExcelImportService.GetEntityCodeStrAsync(DbExcelMap.Table);
                CommonFunc.ShowtextWindowDialog("实体类代码", result);
            });
        });

        public ICommand NewMapConfigCommand => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                DbExcelMapModel model = new DbExcelMapModel();
                if (!DbConn.IsNullOrEmptyOrWhiteSpazeOrCountZero())
                {
                    model.ConfigName = $"配置-{DateTime.Now}";
                    model.DependencyDbConfigID = this.DbConn.ID;
                }
                DbExcelMapCollection.Add(model);
                DbExcelMap = model;
                ConfigService._columnMapModelList.Add(model);
            }, "新增Excel映射配置");
        });

        public ICommand DeleteMapConfigCommand => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                DbExcelMapCollection.Remove(DbExcelMap);
                ConfigService._columnMapModelList.Remove(DbExcelMap);
                DbExcelMap = DbExcelMapCollection.FirstOrDefault();
            }, "删除Excel映射配置");
        });
        public ICommand SaveMapConfigCommand => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                ConfigService.SaveConfigColumnMap();
            }, "保存Excel映射配置");
        });



        public ICommand OpenExcelCommand => new RelayCommand(() =>
        {

        });

        public ICommand GetTableStructureCommand => new RelayCommand(async () =>
        {
            await CommonFunc.HandleExceptionAsync(async () =>
            {
                var columns = await ExcelImportService.GetDbTableColumnMapModelAsync(DbExcelMap.Table);
                DbExcelMap.ColumnMapCollection = columns.ToObservableCollection();
            });
        });

        public ICommand SelectExcelCommand => new RelayCommand(async () =>
        {
            IsExcelPopupOpen = !IsExcelPopupOpen;
            await CommonFunc.HandleExceptionAsync(async () =>
            {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
                dialog.Filter = @"xls文件|*.xls;*.xlsx";
                Nullable<bool> result = dialog.ShowDialog();
                if (result == true)
                {
                    ExcelPath = dialog.FileName;
                    CommonFunc.Log("获取工作表中");
                    var sheets = await Task.Run(() => ExcelHelper.GetSheetName(ExcelPath));
                    Sheets = new ObservableCollection<string>(sheets);
                    CurrentSheet = Sheets.Count > 0 ? Sheets[0] : null; // 设置当前工作表 防止索引超出范围
                }
            }, "选择Excel文件");
            IsExcelPopupOpen = !IsExcelPopupOpen;
        });



    }
}
