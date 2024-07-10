using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pdfer.Common
{
    public static class CommonFunc
    {
        public static void Log(string msg)
        {
            WeakReferenceMessenger.Default.Send(msg, "Log");
        }
    }
}
