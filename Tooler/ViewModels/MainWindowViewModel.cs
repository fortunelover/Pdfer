using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Tooler.Common.Enum;
using Pdfer.Views;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using NetTaste;
using Org.BouncyCastle.Cms;
namespace Tooler.ViewModels
{

    public class MainWindowViewModel : ObservableObject, IRecipient<string>
    {
        public Page _frameSource;
        public Page FrameSource
        {
            get => _frameSource;
            set => SetProperty(ref _frameSource, value);
        }

        public Page _menuFrameSource;
        public Page MenuFrameSource
        {
            get => _menuFrameSource;
            set => SetProperty(ref _menuFrameSource, value);
        }

        public void InitPage(PageType pageType)
        {
            switch (pageType)
            {
                case PageType.PDFDivisionPage: FrameSource = new Pdfer.Views.DivisionPage(); break;
                case PageType.PDFCombinePage: FrameSource = new Pdfer.Views.CombinePage(); break;
                case PageType.PDFSplicingPage: FrameSource = new Pdfer.Views.SplicingPage(); break;

                case PageType.CipherAESPage: FrameSource = new Cipher.Views.SymmetricAlgorithmPage(); break;
                case PageType.CipherRSAPage: FrameSource = new Cipher.Views.AsymmetricAlgorithmPage(); break;
                case PageType.CipherHashPage: FrameSource = new Cipher.Views.HashPage(); break;

                case PageType.DbExcelImportPage: FrameSource = new DataBase.Views.ExcelImportPage(); break;
                case PageType.DbExcelImportPageTemp: FrameSource = new DataBase.Views.ExcelImportPageTemp(); break;

                default: FrameSource = new Tooler.Views.BlankPage(); break;
            }
        }

        public void InitMenuPage(MenuPageType menuPageType)
        {
            switch (menuPageType)
            {
                case MenuPageType.PDF: MenuFrameSource = new Pdfer.Views.MenuPage(); break;
                case MenuPageType.Cipher: MenuFrameSource = new Cipher.Views.MenuPageCipher(); break;
                case MenuPageType.DataBase: MenuFrameSource = new Cipher.Views.MenuPageDb(); break;
                default: MenuFrameSource = new Tooler.Views.BlankPage(); break;
            }
        }

        public void Receive(string msg)
        {
            if (msg.Equals("Visiable")|| msg.Equals("UnVisiable"))
            {
                IsProgressRingVisibale = msg.Equals("Visiable");
            }
            else
            {
                Enum.TryParse(msg, out PageType pageType);
                InitPage(pageType);
            }
        }

        public MainWindowViewModel()
        {
            WeakReferenceMessenger.Default.Register(this, "Page");
            WeakReferenceMessenger.Default.Register(this, "ProgressRingVisiblie");
            InitPage(PageType.BlankPage);
            SelectedMenuPageType = MenuPageType.DataBase;
            InitMenuPage(SelectedMenuPageType);
            SelectMenuPageCommand = new RelayCommand<object>(selectMenuPageCommand);
        }

        public ICommand SelectMenuPageCommand { get; set; }

        public void selectMenuPageCommand(object parameter)
        {
            Enum.TryParse(parameter.ToString(), out MenuPageType menuPageType); 
            InitMenuPage(menuPageType);
            InitPage(PageType.BlankPage);

            SelectedMenuPageType = menuPageType;
        }


        private MenuPageType _selectedMenuPageType;
        public MenuPageType SelectedMenuPageType
        {
            get => _selectedMenuPageType;
            set => SetProperty(ref _selectedMenuPageType, value);
        }

        private bool _isProgressRingVisibale;
        public bool IsProgressRingVisibale
        {
            get => _isProgressRingVisibale;
            set => SetProperty(ref _isProgressRingVisibale, value);
        }


    }
}

