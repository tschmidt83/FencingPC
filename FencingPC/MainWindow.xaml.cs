using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Xml.Serialization;

using AForge.Video;
using AForge.Video.DirectShow;
namespace FencingPC
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Roster
        private ObservableCollection<Fencer> Roster = new ObservableCollection<Fencer>();

        // Directory of the executable
        private string ProjectDir = string.Empty;

        private bool m_EditModeEnabled = false;

        /// <summary>
        /// Indicates whether the edit mode (for a fencer's data in the roster) is enabled
        /// </summary>
        public bool EditModeEnabled
        {
            get { return m_EditModeEnabled; }
            set { m_EditModeEnabled = value; NotifyPropertyChanged("EditModeEnabled"); }
        }

        // Currently selected fencer
        private Fencer SelectedFencer = null;

        // Array of connected screens
        private System.Windows.Forms.Screen[] ConnectedScreens;

        public MainWindow()
        {
            InitializeComponent();

            // Populate list boxes with localized strings
            cbFencerGender.Items.Add(GetResourceString("str_Gender_Male"));
            cbFencerGender.Items.Add(GetResourceString("str_Gender_Female"));
            cbFencerGender.SelectedIndex = 0;

            cbFencerMembership.Items.Add(GetResourceString("str_Member_Regular"));
            cbFencerMembership.Items.Add(GetResourceString("str_Member_Student"));
            cbFencerMembership.Items.Add(GetResourceString("str_Member_Guest"));
            cbFencerMembership.SelectedIndex = 0;

            // Determine directory of the executable
            ProjectDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // If a roster file exists, load it
            if (System.IO.File.Exists(ProjectDir + @"\roster.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Fencer>));
                using (System.IO.FileStream fs = new System.IO.FileStream(ProjectDir + @"\roster.xml", System.IO.FileMode.Open))
                {
                    Roster = (ObservableCollection<Fencer>)serializer.Deserialize(fs);
                }
            }

            // Set data context
            grdFencerData.DataContext = this;
            lbRoster.ItemsSource = Roster;
            lbRoster.DisplayMemberPath = "DisplayName";

            // Enable touchscreen support
            InkInputHelper.DisableWPFTabletSupport();
        }

        private string GetResourceString(string identifier)
        {
            string result = string.Empty;

            if (Application.Current.Resources[identifier] != null)
                result = Application.Current.Resources[identifier].ToString();

            return result;
        }

        private void btnEditFencer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNewFencer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDeleteFencer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFencerImage_Webcam_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFencerImage_Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFencerOK_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFencerCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /* --- TO DO
            if (myResultWindow != null)
                myResultWindow.Close();
            */
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Enable touchscreen
            InputPanelConfiguration cp = new InputPanelConfiguration();
            IInputPanelConfiguration icp = cp as IInputPanelConfiguration;
            if (icp != null)
                icp.EnableFocusTracking();

            // Detect webcams
            FilterInfoCollection cams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            cbSettings_WebcamSelect.Items.Clear();
            foreach (FilterInfo fi in cams)
            {
                cbSettings_WebcamSelect.Items.Add(fi.Name);
                if (fi.Name == Properties.Settings.Default.SelectedWebCam)
                    cbSettings_WebcamSelect.SelectedItem = fi.Name;
            }

            // Detect screens
            ConnectedScreens = System.Windows.Forms.Screen.AllScreens;
            int screenIndex = 0;
            foreach (System.Windows.Forms.Screen s in ConnectedScreens)
            {
                cbSettings_MainScreen.Items.Add(screenIndex.ToString() + ": " + s.DeviceName);
                cbSettings_ResultScreen.Items.Add(screenIndex.ToString() + ": " + s.DeviceName);
                screenIndex++;
            }

            // Set screen for window
            if (Properties.Settings.Default.MainScreen < ConnectedScreens.Length)
            {
                this.Top = ConnectedScreens[Properties.Settings.Default.MainScreen].WorkingArea.Top;
                this.Left = ConnectedScreens[Properties.Settings.Default.MainScreen].WorkingArea.Left;
                this.Height = ConnectedScreens[Properties.Settings.Default.MainScreen].WorkingArea.Height;
                this.Width = ConnectedScreens[Properties.Settings.Default.MainScreen].WorkingArea.Width;

                cbSettings_MainScreen.SelectedIndex = Properties.Settings.Default.MainScreen;
            }
            else
            {
                this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top;
                this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Left;
                this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
                this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            }

            this.WindowState = WindowState.Maximized;

            if (Properties.Settings.Default.ResultScreen < ConnectedScreens.Length)
            {
                cbSettings_ResultScreen.SelectedIndex = Properties.Settings.Default.ResultScreen;
            }

            // Set screen for results window --- TO DO
            /*
            if (Properties.Settings.Default.UseResultScreen == true)
            {
                myResultWindow = new OnlineResultsWindow();
                myResultWindow.Show();

                if (Properties.Settings.Default.ResultScreen < ConnectedScreens.Length)
                {
                    myResultWindow.Top = ConnectedScreens[Properties.Settings.Default.ResultScreen].WorkingArea.Top;
                    myResultWindow.Left = ConnectedScreens[Properties.Settings.Default.ResultScreen].WorkingArea.Left;
                    myResultWindow.Height = ConnectedScreens[Properties.Settings.Default.ResultScreen].WorkingArea.Height;
                    myResultWindow.Width = ConnectedScreens[Properties.Settings.Default.ResultScreen].WorkingArea.Width;

                    cbSettings_ResultScreen.SelectedIndex = Properties.Settings.Default.ResultScreen;
                }
                else
                {
                    myResultWindow.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top;
                    myResultWindow.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Left;
                    myResultWindow.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
                    myResultWindow.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
                }

                myResultWindow.WindowState = WindowState.Maximized;
            }
            */

            StartTournament();
        }

        private void StartTournament()
        {
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void btnRestartTournament_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSettings_Apply_Click(object sender, RoutedEventArgs e)
        {
            if (cbSettings_MainScreen.SelectedIndex >= 0)
                Properties.Settings.Default.MainScreen = cbSettings_MainScreen.SelectedIndex;
            if (cbSettings_ResultScreen.SelectedIndex >= 0)
                Properties.Settings.Default.ResultScreen = cbSettings_ResultScreen.SelectedIndex;

            if (cbSettings_WebcamSelect.SelectedIndex >= 0)
            {
                Properties.Settings.Default.SelectedWebCam = cbSettings_WebcamSelect.SelectedItem.ToString();
            }
            else
            {
                Properties.Settings.Default.SelectedWebCam = string.Empty;
            }

            Properties.Settings.Default.UseResultScreen = (cbEnableResultScreen.IsChecked == true ? true : false);
            Properties.Settings.Default.Save();

            // Set standard screen
            this.WindowState = WindowState.Normal;

            if (Properties.Settings.Default.MainScreen < ConnectedScreens.Length)
            {
                this.Top = ConnectedScreens[Properties.Settings.Default.MainScreen].WorkingArea.Top;
                this.Left = ConnectedScreens[Properties.Settings.Default.MainScreen].WorkingArea.Left;
                this.Height = ConnectedScreens[Properties.Settings.Default.MainScreen].WorkingArea.Height;
                this.Width = ConnectedScreens[Properties.Settings.Default.MainScreen].WorkingArea.Width;
            }
            else
            {
                this.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top;
                this.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Left;
                this.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
                this.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
            }

            this.WindowState = WindowState.Maximized;

            // Set result screen --- TO DO
            /*
            if (Properties.Settings.Default.UseResultScreen == true)
            {
                if (myResultWindow == null)
                    myResultWindow = new OnlineResultsWindow();
                myResultWindow.Show();

                myResultWindow.WindowState = WindowState.Normal;

                if (Properties.Settings.Default.ResultScreen < ConnectedScreens.Length)
                {
                    myResultWindow.Top = ConnectedScreens[Properties.Settings.Default.ResultScreen].WorkingArea.Top;
                    myResultWindow.Left = ConnectedScreens[Properties.Settings.Default.ResultScreen].WorkingArea.Left;
                    myResultWindow.Height = ConnectedScreens[Properties.Settings.Default.ResultScreen].WorkingArea.Height;
                    myResultWindow.Width = ConnectedScreens[Properties.Settings.Default.ResultScreen].WorkingArea.Width;
                }
                else
                {
                    myResultWindow.Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Top;
                    myResultWindow.Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Left;
                    myResultWindow.Height = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
                    myResultWindow.Width = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
                }

                myResultWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                if (myResultWindow != null)
                    myResultWindow.Close();
                myResultWindow = null;
            }
            */
        }

        private void btnSettings_Refresh_Click(object sender, RoutedEventArgs e)
        {
            ConnectedScreens = System.Windows.Forms.Screen.AllScreens;

            cbSettings_MainScreen.Items.Clear();
            cbSettings_ResultScreen.Items.Clear();
            int screenIndex = 0;

            foreach (System.Windows.Forms.Screen s in ConnectedScreens)
            {
                cbSettings_MainScreen.Items.Add(screenIndex.ToString() + ": " + s.DeviceName);
                cbSettings_ResultScreen.Items.Add(screenIndex.ToString() + ": " + s.DeviceName);
                screenIndex++;
            }

            if (Properties.Settings.Default.MainScreen < ConnectedScreens.Length)
            {
                cbSettings_MainScreen.SelectedIndex = Properties.Settings.Default.MainScreen;
            }
            else
            {
                cbSettings_MainScreen.SelectedIndex = -1;
            }

            if (Properties.Settings.Default.ResultScreen < ConnectedScreens.Length)
            {
                cbSettings_ResultScreen.SelectedIndex = Properties.Settings.Default.ResultScreen;
            }
            else
            {
                cbSettings_ResultScreen.SelectedIndex = -1;
            }

            // Detect webcams
            FilterInfoCollection cams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            cbSettings_WebcamSelect.Items.Clear();
            foreach (FilterInfo fi in cams)
            {
                cbSettings_WebcamSelect.Items.Add(fi.Name);
            }
        }
    }
}
