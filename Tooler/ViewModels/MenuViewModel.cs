using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Tooler.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tooler.ViewModels
{
    public class MenuViewModel : ObservableObject, IRecipient<string>
    {
        public static MenuViewModel Current
        {
            get;
            set;
        }

        public MenuViewModel()
        {
            SelectPageCommand = new RelayCommand<object>(selectPageCommand);
            WeakReferenceMessenger.Default.Register(this,"Page");
        }

        public ICommand SelectPageCommand { get; set; }


        public void selectPageCommand(object parameter)
        {
            WeakReferenceMessenger.Default.Send(parameter.ToString(), "Page");
            Enum.TryParse(parameter.ToString(), out PageType pageType);
            SelectedPage = pageType;
        }


        public void Receive(string message)
        {
            Current = this;
        }

        private PageType _selectedPage;

        public PageType SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                if (_selectedPage != value)
                {
                    _selectedPage = value;
                    OnPropertyChanged(nameof(SelectedPage));
                }
            }
        }



        // 其他属性和方法...

    }
}
