using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cipher.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Tooler.Common;

namespace Cipher.ViewModels;
public class AsymmetricAlgorithmViewModel : ObservableObject
{

    private string _input;
    public string Input
    {
        get => _input;
        set => SetProperty(ref _input, value);
    }

    private string _signtext;
    public string SignText
    {
        get => _signtext;
        set => SetProperty(ref _signtext, value);
    }

    private string _errorText;
    public string ErrorText
    {
        get => _errorText;
        set => SetProperty(ref _errorText, value);
    }

    private string _strPri;
    public string PrivateKey
    {
        get => _strPri;
        set => SetProperty(ref _strPri, value);
    }

    private string _strPub;
    public string PublicKey
    {
        get => _strPub;
        set => SetProperty(ref _strPub, value);
    }

    private string _decrypData;
    public string DecrypData
    {
        get => _decrypData;
        set => SetProperty(ref _decrypData, value);
    }

    private string _encrytData;
    public string EncrytData
    {
        get => _encrytData;
        set => SetProperty(ref _encrytData, value);
    }


    public ICommand CreateKey => new RelayCommand(() =>
    {
        RSAHelper.CreateKey(out string pubKey, out string priKey);
        PublicKey = pubKey;
        PrivateKey = priKey;
    });

    public ICommand EncrytByPublicKey => new RelayCommand(() =>
    {
        CommonFunc.HandleException(() => EncrytData = RSAHelper.EncrytPub(PublicKey, DecrypData), "公钥加密");
    });



    public ICommand EncrytByPrivateKey => new RelayCommand(() =>
    {
        CommonFunc.HandleException(() => EncrytData = RSAHelper.EncrytPriBatch(PrivateKey, DecrypData), "私钥加密");
    });



    public ICommand DecrypByPublicKey => new RelayCommand(() =>
    {
        CommonFunc.HandleException(() => DecrypData = RSAHelper.DecrypPubBatch(PublicKey, EncrytData), "公钥解密");
    });


    public ICommand DecrypByPrivateKey => new RelayCommand(() =>
    {
        CommonFunc.HandleException(() => DecrypData = RSAHelper.DecrypPri(PrivateKey, EncrytData), "私钥解密");
    });

    public ICommand CreateKeyByPriKey => new RelayCommand(() =>
    {
        CommonFunc.HandleException(() => PublicKey = RSAHelper.CreatePubKeyByPriKey(PrivateKey), "生成公钥");
    });

    public ICommand Java2Net => new RelayCommand(() =>
    {
        CommonFunc.HandleException(() => PrivateKey = RSAHelper.RSAPrivateKeyJava2DotNet(PrivateKey), "Java私钥转Net");
    });

    public ICommand Net2Java => new RelayCommand(() =>
    {
        CommonFunc.HandleException(() => PrivateKey = RSAHelper.RSAPrivateKeyDotNet2Java(PrivateKey), "Net私钥转Java");
    });

    public ICommand SignData => new RelayCommand(() =>
    {
        CommonFunc.HandleException(() => SignText = RSAHelper.SignData(Input, PrivateKey), "私钥签名");
    });

    public ICommand VerifyData => new RelayCommand(() =>
    {
        try
        {
            CommonFunc.Log(RSAHelper.VerifyData(Input, PublicKey, SignText) ? "公钥验签成功" : "公钥验签失败");
        }
        catch (Exception ex)
        {
            CommonFunc.Log($"Error！{ex.Message}");
        }

    });
}
