using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cipher.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Tooler.Common;

namespace Cipher.ViewModels
{
    public class SymmetricAlgorithmViewModel : ObservableObject
    {
        public SymmetricAlgorithmViewModel()
        {
            CipherModeStr = "ECB";
            PaddingModeStr = "PKCS7";
            EncordingModeStr = "Base64";
        }

        private string _ciphertext;
        public string Ciphertext
        {
            get => _ciphertext;
            set => SetProperty(ref _ciphertext, value);
        }

        private string _plaintext;
        public string Plaintext
        {
            get => _plaintext;
            set => SetProperty(ref _plaintext, value);
        }

        private string _key;
        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }

        private string _iv;
        public string IV
        {
            get => _iv;
            set => SetProperty(ref _iv, value);
        }


        private CipherMode GetEnumeratorCipherMode(string cipherModeStr)
        {
            switch (cipherModeStr)
            {
                case "CBC":
                    return CipherMode.CBC;
                case "OFB":
                    return CipherMode.OFB;
                case "CFB":
                    return CipherMode.CFB;
                case "CTS":
                    return CipherMode.CTS;
                case "ECB":
                    return CipherMode.ECB;
                default:
                    return CipherMode.ECB;
            }
        }
        private PaddingMode GetEnumeratorPaddingMode(string paddingModeStr)
        {
            switch (paddingModeStr)
            {
                case "None":
                    return PaddingMode.None;
                case "Zeros":
                    return PaddingMode.Zeros;
                case "ISO10126":
                    return PaddingMode.ISO10126;
                case "ANSIX923":
                    return PaddingMode.ANSIX923;
                case "PKCS7":
                    return PaddingMode.PKCS7;
                default:
                    return PaddingMode.PKCS7;
            }
        }
        public ICommand Encryt => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                //AESHelper.EncryptStringToBytes_Aes(Plaintext, "0123456789abcdef");
                CipherMode cipherMode = GetEnumeratorCipherMode(CipherModeStr);
                PaddingMode paddingMode = GetEnumeratorPaddingMode(PaddingModeStr);
                Ciphertext = string.Empty;
                Ciphertext = AESHelper.AesEncrypt(Plaintext, Key, cipherMode, paddingMode, EncordingModeStr, IV);
            }, "加密");
        });
        public ICommand Decryp => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                CipherMode cipherMode = GetEnumeratorCipherMode(CipherModeStr);
                PaddingMode paddingMode = GetEnumeratorPaddingMode(PaddingModeStr);
                Plaintext = string.Empty;
                Plaintext = AESHelper.AesDecrypt(Ciphertext, Key, cipherMode, paddingMode, EncordingModeStr, IV);
            }, "解密");
        });

        public ICommand CreateKey => new RelayCommand(() =>
        {
            CommonFunc.HandleException(() =>
            {
                AESHelper.AesCreateKey(out string key, out string iv, EncordingModeStr);
                Key = key;
                IV = iv;
            },"生成秘钥");
        });

        public ICommand Introduce => new RelayCommand(() =>
        {
            CommonFunc.ShowtextWindowDialog("AES Introduce", @"
CBC模式（Cipher Block Chaining）：
优点：  
        ①明文的规律不会反映到密文中
        ②解密过程可以并发处理
        ③可以解密任意密文块
缺点：  
        ①有位单位错误的密文块解密后，对应的明文块也将出错
        ②加密过程不可以并发处理

ECB模式（Electronic Codebook）：
优点：  
        ①简单
        ②高速
        ③加密与解密过程均可以并发处理。
缺点：  
        ①明文中的规律直观反映到密文中
        ②可以通过对密文块的删除和交换顺序对明文进行一定功能意义上的操作和改变
        ③有位单位错误的密文块解密后，对应的明文块也将出错
        ④再传送攻击可操作

OFB模式（Output Feedback）：dotNet内置不支持
优点：  
        ①不需要填充(padding)
        ②加密与解密可以事前预先准备
        ③加密解密过程构造相同
        ④有位单位错误的密文块解密后，对应的明文块不会出错，仅影响对应位数据
缺点：  
        ①不可以并发处理
        ②如果密文某位数据反转，则明文数据也将反转

CTR模式（CounTeR）（计数器模式）：dotNet内置不支持
优点：  
        ①不需要填充(padding)
        ②解密过程可以并发处理
        ③可以解密任意密文块
缺点：  
        ①加密过程不可以并发处理
        ②有位单位错误的密文块解密后，对应的明文块也将出错
        ③再传送攻击可操作

CFB模式（Cipher-FeedBack）：
优点：  
        ①不需要填充(padding)
        ②加密与解密可以事前预先准备
        ③加密解密过程构造相同
        ④有位单位错误的密文块解密后，对应的明文块不会出错，仅影响对应位数据
        ⑤可以并发处理
缺点：  
        ①如果密文某位数据反转，则明文数据也将反转

CTS模式（加密块链分割模式）：dotNet内置不支持
优点：  
        ①不需要填充(padding)
        ②对于最后一个数据块的处理能够提高安全性，避免了填充可能带来的一些安全隐患，如填充相关攻击
缺点：  
        ①需要额外的计算来对最后一个数据块进行处理，加密和解密过程都会引入一些额外的开销。这可能会导致性能略微降低
");
        });

        private string _cipherMode;
        public string CipherModeStr
        {
            get => _cipherMode;
            set => SetProperty(ref _cipherMode, value);
        }

        private string _paddingMode;
        public string PaddingModeStr
        {
            get => _paddingMode;
            set => SetProperty(ref _paddingMode, value);
        }

        private string _encordingModeStr;
        public string EncordingModeStr
        {
            get => _encordingModeStr;
            set => SetProperty(ref _encordingModeStr, value);
        }
    }
}
