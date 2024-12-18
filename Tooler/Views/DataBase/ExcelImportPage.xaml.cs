﻿using DataBase.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataBase.Views
{
    /// <summary>
    /// Excel2DbPage.xaml 的交互逻辑
    /// </summary>
    public partial class ExcelImportPage : Page
    {
        public ExcelImportViewModel VM = new ExcelImportViewModel();
        public ExcelImportPage()
        {
            this.DataContext = VM;
            InitializeComponent();
        }


    }
}