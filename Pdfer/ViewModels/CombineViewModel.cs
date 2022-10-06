using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pdfer.Models;
using Pdfer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pdfer.ViewModels
{
    public class CombineViewModel: ObservableObject
    {
        public CombineViewModel()
        {
            gridModelList = new ObservableCollection<PathModel>();
        }

        private ObservableCollection<PathModel> gridModelList ;

        public ObservableCollection<PathModel> GridModelList
        {
            get { return gridModelList; }
            set => SetProperty(ref gridModelList, value);
        }
        public ICommand SelectSrcFile => new RelayCommand(() =>
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = @"pdf文件|*.pdf";
            Nullable<bool> result = dialog.ShowDialog();
            if (dialog.FileName.Length > 4)
            {
                var pm = new PathModel();
                pm.SrcPath = dialog.FileName;
                pm.DstPath = dialog.FileName.Substring(0, dialog.FileName.Length - 4);
                Index++;
                pm.Seq = Convert.ToString(Index);
                GridModelList.Add(pm);
            }
        });

        public ICommand SelectOutputPath => new RelayCommand(() =>
        {

            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            Nullable<bool> result = dialog.ShowDialog();
            if (dialog.FileName.Length > 0)
            {
                SavePath=dialog.FileName+@".pdf";
            }
        });

        private string savePath;

        public string SavePath
        {
            get { return savePath; }
            set => SetProperty(ref savePath, value);
        }


        private int Index = 0;

        private string outResult;

        public string OutResult
        {
            get { return outResult; }
            set => SetProperty(ref outResult, value);
        }

        public ICommand Remove => new RelayCommand<string>((id) =>
        {
            Del(Convert.ToString(id));
        });
        public void Del(string id)
        {
            var gm = GridModelList.Where(a => a.Seq == id).FirstOrDefault();
            var model = GridModelList.Remove(gm);
            if (gm != null) Index--;
        }

        public ICommand Execute => new RelayCommand(() =>
        {
            outResult = "执行结果：\n";
            try
            {
                PdfService.PdfCombine(GridModelList.Select(a=>a.SrcPath).ToArray(), SavePath);
                OutResult += string.Format("合并成功！");
            }
            catch (Exception ex)
            {
                OutResult += string.Format("执行失败！{0}\n", ex.Message);
            }
        });

    }
}
