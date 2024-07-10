using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pdfer.Models
{
    public class PathModel: ObservableObject
    {

        private string seq;
        public string Seq
        {
            get { return seq; }
            set => SetProperty(ref seq, value);
        }

        private string srcPath ;
        public string SrcPath
        {
            get { return srcPath; }
            set => SetProperty(ref srcPath, value);
        }

        private string dstPath ;
        public string DstPath
        {
            get { return dstPath; }
            set => SetProperty(ref dstPath, value);
        }

        private string pageHeight;
        public string PageHeight
        {
            get { return pageHeight; }
            set => SetProperty(ref pageHeight, value);
        }

        private string pageWidth;
        public string PageWidth
        {
            get { return pageWidth; }
            set => SetProperty(ref pageWidth, value);
        }

        private string pageCount;
        public string PageCount
        {
            get { return pageCount; }
            set => SetProperty(ref pageCount, value);
        }
    }
}
