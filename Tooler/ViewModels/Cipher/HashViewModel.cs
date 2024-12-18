using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Security.Cryptography;
using Cipher.Helper;

namespace Cipher.ViewModels
{
    public class HashViewModel : ObservableObject
    {
        private string _cipherModeStr;

        public string CipherModeStr
        {
            get => _cipherModeStr;
            set => SetProperty(ref _cipherModeStr, value);
        }

        private string _source;

        public string Source
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }


        private string _destination;

        public string Destination
        {
            get => _destination;
            set => SetProperty(ref _destination, value);
        }

        Func<string, string> func = null;

        public ICommand exe => new RelayCommand(() =>
        {
            try
            {
                GetHash getHash = null;
                switch (CipherModeStr)
                {
                    case "MD5":
                        getHash = HashHelper.GetMD5;
                        break;
                    case "SHA1":
                        getHash = HashHelper.GetSHA1;
                        break;
                    case "SM3":
                        getHash = HashHelper.GetSM3;
                        break;
                    case "SHA256":
                        getHash = HashHelper.GetSHA256;
                        break;
                    default:
                        getHash = null;
                        break;
                }
                if (getHash != null)
                {
                    Destination = getHash.Invoke(Source);
                }

                //switch (CipherModeStr)
                //{
                //    case "MD5":
                //        func = new Func<string, string>(HashHelper.GetMD5);
                //        break;
                //    case "SHA1":
                //        func = new Func<string, string>(HashHelper.GetSHA1);
                //        break;
                //    case "SM3":
                //        func = new Func<string, string>(HashHelper.GetSM3);
                //        break;
                //    case "SHA256":
                //        func = new Func<string, string>(HashHelper.GetSHA256);
                //        break;
                //    default:
                //        func = null;
                //        break;
                //}
                //if (func != null)
                //{
                //    Destination = func(Source);
                //}
            }
            catch (Exception e)
            {
                Destination = "Error!\n" + e.Message;
            }
        });


    }

    public delegate string GetHash(string source);
}
