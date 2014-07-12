using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;
using Hardcodet.Wpf.TaskbarNotification;

namespace Camera
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskbarIcon tb;
        public MainWindow()
        {
            InitializeComponent();

            //initialize the screen
            tb = (TaskbarIcon)FindResource("NotifyIcon");

        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                    
                    break;
                case WindowState.Minimized:
                    ShowInTaskbar = false;
                    tb.Visibility = Visibility.Visible;

                    break;
                case WindowState.Normal:
                    ShowInTaskbar = true;
                    break;
            }
        }
        
    }
}
