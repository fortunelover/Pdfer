using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Pdfer.ViewModels
{

        public class MainWindowViewModel : ObservableObject, IRecipient<string>
    {
            public Page _FrameSource;
            public Page FrameSource
            {
                get => _FrameSource;
                set => SetProperty(ref _FrameSource, value);
            }



            public void Init(int pageId)
            {
                switch (pageId)
                {
                    case 1: FrameSource = new Pdfer.Views.DivisionPage(); break;
                    case 2: FrameSource = new Pdfer.Views.CombinePage(); break;
                    case 3: FrameSource = new Pdfer.Views.SplicingPage(); break;
                    default: FrameSource = new Pdfer.Views.DivisionPage(); break;

                }
            }



            public void Receive(string pID)
            {
                Init(Convert.ToInt32(pID));
            }

            public MainWindowViewModel()
            {
                WeakReferenceMessenger.Default.Register(this, "Page");
                Init(1);
            }
        }
    }

