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
using System.Windows;
using System.Windows.Input;

namespace Pdfer.ViewModels
{
    public class DivisionViewModel : ObservableObject
    {
        public DivisionViewModel()
        {
        }

        public void Del(string id)
        {
            var gm = GridModelList.Where(a => a.Seq == id).FirstOrDefault();
            var model = GridModelList.Remove(gm);
            if (gm != null) Index--;
        }

        private ObservableCollection<PathModel> gridModelList = new ObservableCollection<PathModel>();

        public ObservableCollection<PathModel> GridModelList
        {
            get { return gridModelList; }
            set => SetProperty(ref gridModelList, value);
        }


        private int Index = 0;

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




        public ICommand Edit => new RelayCommand(() =>
        {
        });


        public DragEventHandler dragEventHandler;
        public ICommand Remove => new RelayCommand<string>((id) =>
        {
            Del(Convert.ToString(id));
        });

        public ICommand Execute => new RelayCommand(() =>
        {
            outResult = "执行结果：\n";
            foreach (var item in GridModelList)
            {
                try
                {
                    PdfService.PdfDivision(item.SrcPath, item.DstPath);
                    OutResult += string.Format("序号{0}：{1}\n", item.Seq, "拆分成功！");
                }
                catch (Exception ex)
                {
                    OutResult += string.Format("序号{0}：执行失败！{1}\n", item.Seq, ex.Message);
                }
            }
        });

        private string outResult;

        public string OutResult
        {
            get { return outResult; }
            set => SetProperty(ref outResult, value);
        }

    }
}
