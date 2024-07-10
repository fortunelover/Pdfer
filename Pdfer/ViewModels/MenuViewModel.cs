using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pdfer.ViewModels
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


        public void selectPageCommand(object obj)
        {
            WeakReferenceMessenger.Default.Send(Convert.ToString(obj),"Page");
            SelectedPage = (PageType)Convert.ToInt32(obj)-1;
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

        public enum PageType
        {
            Page1,
            Page2,
            Page3
        }

        // 其他属性和方法...

    }
}
