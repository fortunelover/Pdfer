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
    public partial class ExcelImportTempViewModel : ExcelDbViewModel, IRecipient<string>
    {
        private string _table;

        public string Table
        {
            get => _table;
            set => SetProperty(ref _table, value);
        }
    }
}
