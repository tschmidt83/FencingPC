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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

using AForge.Video;
using AForge.Video.DirectShow;

namespace FencingPC
{
    /// <summary>
    /// Interaktionslogik für GetWebcamImageWindow.xaml
    /// </summary>
    public partial class GetWebcamImageWindow : Window
    {
        private DispatcherTimer CameraTimer = new DispatcherTimer();
        private FilterInfoCollection MyVideoDevices;
        private VideoCaptureDevice MyVideoSource;

        private int CameraCountdown = 0;
        private bool StartCountdown = false;

        public BitmapSource LastImage = null;

        public GetWebcamImageWindow()
        {
            InitializeComponent();

            CameraTimer.Interval = TimeSpan.FromSeconds(1);
            CameraTimer.Tick += CameraTimer_Tick;
        }

        private void CameraTimer_Tick(object sender, EventArgs e)
        {
            if (CameraCountdown > 1)
            {
                CameraCountdown--;
                tbCountdown.Text = CameraCountdown.ToString();
            }
            else
            {
                tbCountdown.Text = "---";
                CameraTimer.Stop();
                if (MyVideoSource != null)
                {
                    MyVideoSource.Stop();
                    btnOK.IsEnabled = true;
                }
                btnFoto.IsEnabled = true;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void MyVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            System.Drawing.Bitmap img = (System.Drawing.Bitmap)eventArgs.Frame.Clone();

            System.IO.MemoryStream memory = new System.IO.MemoryStream();
            img.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
            memory.Seek(0, System.IO.SeekOrigin.Begin);
            BitmapImage bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.EndInit();
            bitmapimage.Freeze();

            Dispatcher.BeginInvoke(new System.Threading.ThreadStart(delegate
            {
                if (StartCountdown)
                {
                    StartCountdown = false;
                    CameraTimer.Start();
                }
                LastImage = bitmapimage; imgWebcamImage.Source = bitmapimage;
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            btnOK.IsEnabled = false;

            // Check if requested camera exists
            MyVideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (int i = 0; i < MyVideoDevices.Count; i++)
            {
                if (MyVideoDevices[i].Name == Properties.Settings.Default.SelectedWebCam)
                {
                    MyVideoSource = new VideoCaptureDevice(MyVideoDevices[i].MonikerString);
                }
            }

            if (MyVideoSource != null)
            {
                MyVideoSource.NewFrame += MyVideoSource_NewFrame;
                MyVideoSource.Start();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MyVideoSource != null)
            {
                if (MyVideoSource.IsRunning)
                    MyVideoSource.Stop();
            }
        }

        private void btnFoto_Click(object sender, RoutedEventArgs e)
        {
            if (MyVideoSource != null)
            {
                if (!MyVideoSource.IsRunning)
                    MyVideoSource.Start();
                StartCountdown = true;
            }

            CameraCountdown = 3;
            btnFoto.IsEnabled = false;
            btnOK.IsEnabled = false;
            tbCountdown.Text = CameraCountdown.ToString();
            //CameraTimer.Start();
        }
    }
}
