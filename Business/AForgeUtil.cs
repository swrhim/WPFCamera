using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using GalaSoft.MvvmLight.Messaging;
using AForge.Vision.Motion;

namespace Camera
{
    public class AForgeUtil
    {
        #region Singleton

        public static readonly AForgeUtil Instance = new AForgeUtil();

        #endregion Singleton

        public VideoCaptureDevice VideoCaptureDevice = null;
        public FilterInfoCollection FilterInfo = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        public IMotionDetector MotionDetector = new TwoFramesDifferenceDetector();
        BlobCountingObjectsProcessing MotionProcessing = new BlobCountingObjectsProcessing();
        MotionDetector Detector = null;
        const int MinObjectsSize = 30;

        public AForgeUtil()
        {
            MotionProcessing.HighlightColor = System.Drawing.Color.Red;
            MotionProcessing.MinObjectsHeight = MinObjectsSize;
            MotionProcessing.MinObjectsWidth = MinObjectsSize;
            Detector = new MotionDetector(MotionDetector, MotionProcessing);
        }

        /// <summary>
        /// Get the right camera
        /// </summary>
        /// <param name="cameraIndex">0 is screen capture. 1 is actual camera</param>
        public void StartCamera(int cameraIndex)
        {
            this.VideoCaptureDevice = new VideoCaptureDevice(FilterInfo[cameraIndex].MonikerString);
            this.VideoCaptureDevice.NewFrame += new AForge.Video.NewFrameEventHandler(videoCaptureDevice_NewFrame);
            this.VideoCaptureDevice.Start();
        }

        /// <summary>
        /// event handler to capture each new frame
        /// </summary>
        internal void videoCaptureDevice_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            MotionDetection((Bitmap) eventArgs.Frame.Clone());
        }

        internal void MotionDetection(Bitmap bitmap)
        {
            if (Detector.ProcessFrame(bitmap) > 0.02)
            {
                if (MotionProcessing.ObjectsCount > 1)
                {
                    Messenger.Default.Send(MessengerMessages.BadPosture.ToString());
                }
            }

            Messenger.Default.Send(new NotificationMessage<Bitmap>(bitmap, MessengerMessages.NewFrame.ToString()));
        }

        internal void CloseCamera()
        {
            if (VideoCaptureDevice != null && VideoCaptureDevice.IsRunning)
            {
                VideoCaptureDevice.SignalToStop();
                VideoCaptureDevice.WaitForStop();
                VideoCaptureDevice = null;
            }
        }
    }
}
