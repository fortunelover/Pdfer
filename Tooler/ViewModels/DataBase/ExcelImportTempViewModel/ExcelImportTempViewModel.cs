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
using Tooler.Common;
using Tooler.Helper;
using Tooler.ViewModels;
using System.Threading;

namespace DataBase.ViewModels
{
    public partial class ExcelImportTempViewModel : ExcelDbViewModel, IRecipient<string>
    {
        public ExcelImportTempViewModel()
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
            this.DbConnCollection = ConfigService._dbConnModelList;
            this.DbConn = DbConnCollection.FirstOrDefault();
            ExcelImportService.SetDb(DbConn?.DbConnStr);
            this.BtnEnabled = true;
        }

        public ICommand ImportExcelTempCommand => new RelayCommand(async () =>
        {
            // 创建新的 CancellationTokenSource
            _cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = _cancellationTokenSource.Token;

            await CommonFunc.HandleExceptionAsync(async () =>
            {
                var sheetIndex = Sheets.IndexOf(CurrentSheet);
                await Task.Run(async() =>
                {
                    // 在这里检查取消请求
                    cancellationToken.ThrowIfCancellationRequested();
                    //CommonFunc.Log($"Excel文件[{ExcelPath}]页[{CurrentSheet}]数据插入到DataTable中...");
                    CommonFunc.Log($"Excel文件数据插入到DataTable中...");
                    var dt = ExcelImportService.ExcelImportDataTable(this.Table, this.ExcelPath, sheetIndex, cancellationToken);
                    CommonFunc.Log($"DataTable数据插入到数据库临时表[{this.Table}]中...");
                    await ExcelImportService.DataTableImportDbTempTableAsync(dt, this.Table, cancellationToken);
                }, cancellationToken);
            }, "Excel导入到临时表");
        });

        public ICommand SelectExcelCommand => new RelayCommand(async () =>
        {
            await CommonFunc.HandleExceptionAsync(async() =>
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
            },"选择Excel文件");
        });

        public ICommand ComboBoxSelectionChangedCommand => new RelayCommand<object>(async (parameter) =>
        {
            await CommonFunc.HandleExceptionAsync(async () =>
            {
                ExcelImportService.SetDb(DbConn?.DbConnStr);
                CommonFunc.Log($"数据库连接{(ExcelImportService.db.CurrentConnectionConfig == null ? "失败" : "成功")}");
            }, "切换数据库配置");
        });

        public ICommand CancelCommand => new RelayCommand(() =>
        {
            // 取消操作
            _cancellationTokenSource?.Cancel();
            CommonFunc.Log("操作已取消");
        });

        private CancellationTokenSource _cancellationTokenSource;

        private BackgroundWorker _worker;


    }
}
