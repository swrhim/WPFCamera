using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using AForge.Video.DirectShow;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification.Interop;
using Color = System.Drawing.Color;
using DragEventArgs = System.Windows.Forms.DragEventArgs;
using Pen = System.Drawing.Pen;
using Point = System.Drawing.Point;
using System;
using DashStyle = System.Drawing.Drawing2D.DashStyle;
using System.IO;

namespace Camera.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        #region global

        AForgeUtil _aForgeUtil = null;
        //private List<string> _cameras = null;
        private Rectangle GiantHeads;
        private double _x0;
        private double _x1;
        private double _y0;
        private double _y1;

        #endregion global

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
            // Code runs in Blend --> create design time data.
            }
            else
            {
            // Code runs "for real"
            }

            //I love threading
            Task.Factory.StartNew(() => Initialize()).ContinueWith(x=>
                _aForgeUtil.StartCamera(0));
            Messenger.Default.Register<NotificationMessage<Bitmap>>(this, (message) => UpdateCameraFrame(message));

            LeftButtonEvent = new RelayCommand<MouseButtonEventArgs>(arg=>
            {
                var p = arg.GetPosition((IInputElement)arg.Source);
                _x0 = p.X;
                _y0 = p.Y;
            });

            DraggingEvent = new RelayCommand<MouseEventArgs>(arg =>
            {
                //hack
                var encoder = new BmpBitmapEncoder();
                MemoryStream ms = new MemoryStream();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)((System.Windows.Controls.Image)arg.Source).Source));
                encoder.Save(ms);
                var img = System.Drawing.Image.FromStream(ms);
                

                var p = arg.GetPosition((IInputElement)arg.Source);
                _x0 = p.X;
                _y0 = p.Y;
                var g = Graphics.FromImage(img);
                
                using (Pen pen = new Pen(Color.Red))
                {
                    pen.DashStyle = DashStyle.Dash;
                    GiantHeads = MakeRectangle((int)_x0, (int)_y0, (int)_x1, (int)_y1);
                    g.DrawRectangle(pen, GiantHeads);

                }
            });

            DropEvent = new RelayCommand<MouseButtonEventArgs>(arg =>
            {
                MessageBox.Show("Dropped event");
            });
        }

        #region properties

        private BitmapSource _cameraFrame;

        public BitmapSource CameraFrame
        {
            get { return _cameraFrame; }
            set
            {
                _cameraFrame = value;
                RaisePropertyChanged("CameraFrame");
                
            }
        }

        public RelayCommand<MouseButtonEventArgs> LeftButtonEvent { get; set; }

        public RelayCommand<MouseEventArgs> DraggingEvent { get; set; }

        public RelayCommand<MouseButtonEventArgs> DropEvent { get; set; }

        #endregion

        #region helper methods

        private void UpdateCameraFrame(NotificationMessage<Bitmap> message)
        {
            if (message.Notification == MessengerMessages.NewFrame.ToString())
                CameraFrame = message.Content.ToWpfSource();
        }

        private void Initialize()
        {
            _aForgeUtil = new AForgeUtil();
        }

        internal void CloseCamera()
        {
            _aForgeUtil.CloseCamera();
        }

        // Return a Rectangle with these points as corners.
        private Rectangle MakeRectangle(int x0, int y0, int x1, int y1)
        {
            return new Rectangle(
                Math.Min(x0, x1),
                Math.Min(y0, y1),
                Math.Abs(x0 - x1),
                Math.Abs(y0 - y1));
        }

        #endregion

        
    }
}