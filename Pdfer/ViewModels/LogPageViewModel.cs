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

    public class LogPageViewModel : ObservableObject, IRecipient<string>
    {
        public LogPageViewModel()
        {
            WeakReferenceMessenger.Default.Register(this,"Log");
        }

        private string outResult;
        public string OutResult
        {
            get { return outResult; }
            set => SetProperty(ref outResult, value);
        }

        public void Receive(string msg)
        {
            OutResult += $"--{DateTime.Now} {msg}\n";
        }
    }
}

