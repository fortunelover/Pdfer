using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using Tooler.Common.Utils.Extensions;
using System.Windows.Media.Media3D;
using Tooler.ViewModels;
using System.Windows.Interop;

namespace Tooler.Common
{
    public static class CommonFunc
    {
        public static void Log(string msg)
        {
            WeakReferenceMessenger.Default.Send(msg, "Log");
        }


        public static async Task HandleExceptionAsync(Func<Task> func, string operateName = "")
        {
            operateName = operateName.IsNullOrEmptyOrWhiteSpazeOrCountZero() ? "操作" : operateName;
            try
            {
                WeakReferenceMessenger.Default.Send("Visiable", "ProgressRingVisiblie");
                WeakReferenceMessenger.Default.Send("UnEnable", "ButtonEnable");
                await func();
                Log($"{operateName}成功！");
            }
            catch (Exception ex)
            {
                Log($"{operateName}失败！{ex.Message}");
            }
            finally
            {
                WeakReferenceMessenger.Default.Send("UnVisiable", "ProgressRingVisiblie");
                WeakReferenceMessenger.Default.Send("Enable", "ButtonEnable");
            }
        }

        public static void HandleException(Action action, string operateName = "")
        {
            operateName = operateName.IsNullOrEmptyOrWhiteSpazeOrCountZero() ? "操作" : operateName;
            try
            {
                action();
                Log($"{operateName}成功！");
            }
            catch (Exception ex)
            {
                Log($"{operateName}失败！{ex.Message}");
            }
        }

        public static void ShowtextWindowDialog(string title, string text, bool isFixedHeightAndWidth = true)
        {
            Window textWindow = new Window
            {
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Owner = Application.Current.MainWindow, // Set the Owner property to the main window
                ShowInTaskbar = false, // Optional: Hide the window from the taskbar
                Width = 800,
                Height = 600,
            };

            TextBox textBox = new TextBox
            {
                Text = text,
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,

                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            /*Button copyButton = new Button
            {
                Content = "复制脚本",
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            copyButton.Click += (copySender, copyEvent) =>
            {
                Clipboard.SetText(text);
                MessageBox.Show("Text copied to clipboard");
            };*/

            // Calculate the width and height based on the text content

            FormattedText formattedText = new FormattedText(
                textBox.Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
                textBox.FontSize,
                Brushes.Black
            );
            if (isFixedHeightAndWidth)
            {
                textWindow.MinWidth = 800; // Set a minimum width
                textWindow.MinHeight = 600; // Set a minimum height
                textBox.Width = 800;
                textBox.Height = 500;
            }

            else
            {
                textWindow.Width = formattedText.Width + 80; // Adding some margin for better display
                textWindow.Height = formattedText.Height + 100; // Adding some margin for better display
                textBox.Width = formattedText.Width;
                textBox.Height = formattedText.Height;
            }


            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(textBox);
            //stackPanel.Children.Add(copyButton);

            textWindow.Content = stackPanel;
            //textWindow.ShowDialog();
            textWindow.Show();
        }

    }
}
