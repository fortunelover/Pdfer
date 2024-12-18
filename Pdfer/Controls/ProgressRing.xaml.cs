using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Tooler.Controls
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressRing : UserControl
    {
        Storyboard trans;
        public ProgressRing()
        {
            InitializeComponent();
            trans = Resources["Trans"] as Storyboard;
            this.Loaded += ((sender, e) =>
            {
                Active();
            });
        }

        public async void Active()
        {
            el.BeginStoryboard(trans);
            await Task.Delay(170);
            el2.BeginStoryboard(trans);
            await Task.Delay(170);
            el3.BeginStoryboard(trans);
            await Task.Delay(170);
            el4.BeginStoryboard(trans);
            await Task.Delay(170);
            el5.BeginStoryboard(trans);
            await Task.Delay(170);
            el6.BeginStoryboard(trans);
        }

        public void Stop()
        {
            trans.Stop(el);
            trans.Stop(el2);
            trans.Stop(el3);
            trans.Stop(el4);
            trans.Stop(el5);
            trans.Stop(el6);
        }
    }
}
