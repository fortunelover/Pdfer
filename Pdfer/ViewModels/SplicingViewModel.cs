using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pdfer.Common;
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
    public class SplicingViewModel : ObservableObject
    {
        public SplicingViewModel()
        {

        }

        private ObservableCollection<PathModel> gridModelList = new ObservableCollection<PathModel>();

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
                SavePath = dialog.FileName + @".pdf";
            }
        });

        public ICommand GetInfo => new RelayCommand<string>((id) =>
        {
            OutResult = "获取pdf信息结果：\n";
            try
            {
                PdfService.PdfGetInfo(GridModelList.Where(a => a.Seq == id).FirstOrDefault().SrcPath, out List<Tuple<int, double, double>> pageInfoCollection);
                OutResult += string.Format("获取成功！\n");
                OutResult += string.Format($"此pdf共有{pageInfoCollection.Count}页\n");
                foreach (var pageInfo in pageInfoCollection)
                {
                    OutResult += string.Format($"第{pageInfo.Item1}页 宽度{pageInfo.Item2} 高度{pageInfo.Item3}\n");
                }
                GridModelList.Where(a => a.Seq == id).FirstOrDefault().PageWidth = Convert.ToString(pageInfoCollection.FirstOrDefault().Item2);
                GridModelList.Where(a => a.Seq == id).FirstOrDefault().PageHeight = Convert.ToString(pageInfoCollection.FirstOrDefault().Item3);
                GridModelList.Where(a => a.Seq == id).FirstOrDefault().PageCount = Convert.ToString(pageInfoCollection.Count);

            }
            catch (Exception ex)
            {
                OutResult += string.Format("获取失败！{0}\n", ex.Message);
            }
        });


        private string savePath;

        public string SavePath
        {
            get { return savePath; }
            set => SetProperty(ref savePath, value);
        }


        private int Index = 0;



        //删除行
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


        //执行
        public ICommand Execute => new RelayCommand(() =>
        {
            outResult = "执行结果：\n";
            var param = new SpliclingParam();
            param.MaxRow = PageRow;
            param.Left = Left;
            param.Right = Right;
            param.Top = Top;
            param.Bottom = Bottom;

            try
            {
                PdfService.PdfSplicling(GridModelList.Select(a => a.SrcPath).ToArray(), SavePath, param);
                OutResult += string.Format("拼接成功！");
            }
            catch (Exception ex)
            {
                OutResult += string.Format("执行失败！{0}\n", ex.Message);
            }
        });

        private double top;

        public double Top
        {
            get { return top; }
            set => SetProperty(ref top, value);
        }

        private double bottom;

        public double Bottom
        {
            get { return bottom; }
            set => SetProperty(ref bottom, value);
        }

        private double left;

        public double Left
        {
            get { return left; }
            set => SetProperty(ref left, value);
        }


        private double right;

        public double Right
        {
            get { return right; }
            set => SetProperty(ref right, value);
        }

        private int pageRow;

        public int PageRow
        {
            get { return pageRow; }
            set => SetProperty(ref pageRow, value);
        }


        private string outResult;

        public string OutResult
        {
            get { return outResult; }
            set => SetProperty(ref outResult, value);
        }


    }
}
