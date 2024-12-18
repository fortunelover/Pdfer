using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using Tooler.ViewModels;

namespace Tooler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.MainWindowViewModel();
            RectMenuBotton.Content = "<";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation widthAnimation = new DoubleAnimation();
            widthAnimation.Duration = TimeSpan.FromSeconds(0.3); // 定义动画持续时间
            if (RectMenuBotton.Content == ">")
            {
                RectMenuBotton.Content = "<";
                this.GridMenuBottonColumn.Width = 200;
                this.GridMenuBotton.Visibility = System.Windows.Visibility.Visible;
                widthAnimation.From = 61;
                widthAnimation.To = 201;
            }
            else
            {
                RectMenuBotton.Content = ">";
                this.GridMenuBottonColumn.Width = 60;
                this.GridMenuBotton.Visibility = System.Windows.Visibility.Collapsed;
                widthAnimation.From = 201;
                widthAnimation.To = 61;
            }
            // 创建Storyboard并将动画加入
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(widthAnimation);

            // 将动画应用到窗口宽度属性
            Storyboard.SetTarget(widthAnimation, GridMenuBottonColumn);
            Storyboard.SetTargetProperty(widthAnimation, new PropertyPath(Window.WidthProperty));

            // 开始动画
            storyboard.Begin(this);           
        }
    }
}
