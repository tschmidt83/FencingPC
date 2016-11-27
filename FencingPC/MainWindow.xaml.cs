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

        // Edit mode
        private EditModeType EditMode = EditModeType.None;

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

            if (string.IsNullOrEmpty(Properties.Settings.Default.DocumentDir))
            {
                Properties.Settings.Default.DocumentDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.None);
                Properties.Settings.Default.DocumentDir += @"\ts.software\FencingPC\";
                Properties.Settings.Default.Save();
            }

            // If a roster file exists, load it
            if (System.IO.File.Exists(Properties.Settings.Default.DocumentDir + "roster.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Fencer>));
                using (System.IO.FileStream fs = new System.IO.FileStream(Properties.Settings.Default.DocumentDir + "roster.xml", System.IO.FileMode.Open))
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
            if(EditMode == EditModeType.None)
            {
                // Enable edit mide
                EditMode = EditModeType.Edit;
                grdFencerData.IsEnabled = true;
                pnlRoster.IsEnabled = false;

                // Select fencer
                if(lbRoster.SelectedItem != null)
                {
                    SelectedFencer = lbRoster.SelectedItem as Fencer;
                    SetEditFencerData();
                }
            }
        }

        private void btnNewFencer_Click(object sender, RoutedEventArgs e)
        {
            if (EditMode == EditModeType.None)
            {
                int rosterID = 1;
                // Determine next free roster ID;
                if (Roster.Count > 0)
                {
                    Fencer last = Roster.Last();
                    rosterID = last.RosterID + 1;
                }

                // Enable edit mode
                EditMode = EditModeType.Create;
                grdFencerData.IsEnabled = true;
                pnlRoster.IsEnabled = false;

                // Create new (blank) fencer
                Fencer f = new Fencer();
                f.FirstName = GetResourceString("str_FirstName");
                f.LastName = GetResourceString("str_LastName");
                f.Gender = GenderType.Male;
                f.BirthDate = DateTime.Now;
                f.Membership = MembershipType.RegularMember;
                f.RosterID = rosterID;

                // Add to list
                Roster.Add(f);

                // Select current fencer
                SelectedFencer = f;
                SetEditFencerData();
            }
        }

        private void SetEditFencerData()
        {
            if (SelectedFencer != null)
            {
                tbFencerFirstName.Text = SelectedFencer.FirstName;
                tbFencerLastName.Text = SelectedFencer.LastName;
                tbFencerBirthDay.Text = SelectedFencer.BirthDate.Day.ToString();
                tbFencerBirthMonth.Text = SelectedFencer.BirthDate.Month.ToString();
                tbFencerBirthYear.Text = SelectedFencer.BirthDate.Year.ToString();
                cbFencerGender.SelectedIndex = (int)SelectedFencer.Gender;
                cbFencerMembership.SelectedIndex = (int)SelectedFencer.Membership;
                tbFencerID.Text = SelectedFencer.RosterID.ToString();
                SetFencerImage(SelectedFencer.ImageName);
            }
        }

        private void SetFencerImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                imgFencerImage.Source = null;
            }
            else
            {
                if (System.IO.File.Exists(Properties.Settings.Default.DocumentDir + @"images\" + path))
                {
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(Properties.Settings.Default.DocumentDir + @"images\" + path, UriKind.Absolute);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    imgFencerImage.Source = img;
                }
                else
                {
                    imgFencerImage.Source = null;
                }
            }
        }

        private void ClearTextBoxes()
        {
            tbFencerFirstName.Text = "";
            tbFencerLastName.Text = "";
            tbFencerBirthDay.Text = "";
            tbFencerBirthMonth.Text = "";
            tbFencerBirthYear.Text = "";
            tbFencerID.Text = "";
        }

        private void SaveRoster()
        {
            if (!System.IO.Directory.Exists(Properties.Settings.Default.DocumentDir))
                System.IO.Directory.CreateDirectory(Properties.Settings.Default.DocumentDir);

            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Fencer>));
            using (System.IO.FileStream fs = new System.IO.FileStream(Properties.Settings.Default.DocumentDir + @"roster.xml", System.IO.FileMode.Create))
            {
                serializer.Serialize(fs, Roster);
            }
        }

        private void btnDeleteFencer_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFencer != null)
            {
                Roster.Remove(SelectedFencer);

                ClearTextBoxes();
                EditMode = EditModeType.None;
                grdFencerData.IsEnabled = false;
                pnlRoster.IsEnabled = true;
                imgFencerImage.Source = null;
                SelectedFencer = null;
                SaveRoster();
            }
        }

        private void btnFencerImage_Webcam_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFencer != null)
            {
                GetWebcamImageWindow w = new GetWebcamImageWindow();

                bool? result = w.ShowDialog();
                if (result == true)
                {
                    // Determine file name
                    string FileName = String.Format("{0:yyyy-MM-dd_HH-mm-ss}.png", DateTime.Now);
                    if (!System.IO.Directory.Exists(Properties.Settings.Default.DocumentDir + @"images"))
                    {
                        System.IO.Directory.CreateDirectory(Properties.Settings.Default.DocumentDir + @"images");
                    }

                    BitmapSource bmp = w.LastImage;

                    string imgPath = Properties.Settings.Default.DocumentDir + @"images\" + FileName;

                    using (System.IO.FileStream fs = new System.IO.FileStream(imgPath, System.IO.FileMode.Create))
                    {
                        PngBitmapEncoder enc = new PngBitmapEncoder();
                        enc.Frames.Add(BitmapFrame.Create(bmp));
                        enc.Save(fs);
                    }

                    SelectedFencer.ImageName = FileName;
                    SetFencerImage(FileName);
                }
            }
        }

        private void btnFencerImage_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFencer != null)
            {
                if (!string.IsNullOrEmpty(SelectedFencer.ImageName))
                {
                    if (System.IO.File.Exists(Properties.Settings.Default.DocumentDir + @"images\" + SelectedFencer.ImageName))
                    {
                        try
                        {
                            imgFencerImage.Source = null;
                            System.IO.File.Delete(Properties.Settings.Default.DocumentDir + @"images\" + SelectedFencer.ImageName);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void btnFencerOK_Click(object sender, RoutedEventArgs e)
        {
            bool success = true;

            string firstName = tbFencerFirstName.Text;
            if (string.IsNullOrEmpty(firstName))
                success = false;

            string lastName = tbFencerLastName.Text;
            if (string.IsNullOrEmpty(lastName))
                success = false;

            GenderType gender = GenderType.Male;
            MembershipType membership = MembershipType.RegularMember;


            int birthDay = 0;
            int birthMonth = 0;
            int birthYear = 0;

            int rosterID = 0;

            try
            {
                birthDay = int.Parse(tbFencerBirthDay.Text);
                birthMonth = int.Parse(tbFencerBirthMonth.Text);
                birthYear = int.Parse(tbFencerBirthYear.Text);

                gender = (GenderType)cbFencerGender.SelectedIndex;
                membership = (MembershipType)cbFencerMembership.SelectedIndex;

                rosterID = int.Parse(tbFencerID.Text);

                firstName = SetNameCase(firstName);
                lastName = SetNameCase(lastName);
            }
            catch
            {
                success = false;
            }

            if (success)
            {
                if (EditMode == EditModeType.Create)
                {
                    SelectedFencer.FirstName = firstName;
                    SelectedFencer.LastName = lastName;
                    SelectedFencer.Gender = gender;
                    SelectedFencer.BirthDate = new DateTime(birthYear, birthMonth, birthDay);
                    SelectedFencer.Membership = membership;
                    SaveRoster();
                    lbRoster.ItemsSource = null;
                    lbRoster.ItemsSource = Roster;
                }
                else if (EditMode == EditModeType.Edit)
                {
                    SelectedFencer.FirstName = firstName;
                    SelectedFencer.LastName = lastName;
                    SelectedFencer.Gender = gender;
                    SelectedFencer.BirthDate = new DateTime(birthYear, birthMonth, birthDay);
                    SelectedFencer.Membership = membership;
                    SaveRoster();
                    lbRoster.ItemsSource = null;
                    lbRoster.ItemsSource = Roster;
                }
                ClearTextBoxes();
                EditMode = EditModeType.None;
                grdFencerData.IsEnabled = false;
                pnlRoster.IsEnabled = true;
                imgFencerImage.Source = null;
                SelectedFencer = null;
            }
        }

        private string SetNameCase(string name)
        {
            string temp = name.Trim(new char[] { ' ', '-' });
            string result = char.ToUpper(temp[0]).ToString();
            
            // Goal: make every character that follows ' ' or '-' uppercase
            for (int i = 0; i < temp.Length - 1; i++)
            {
                if (temp[i] == ' ' || temp[i] == '-')
                {
                    result += char.ToUpper(temp[i + 1]).ToString();
                }
                else
                {
                    result += temp[i + 1];
                }
            }            

            return result;
        }

        private void btnFencerCancel_Click(object sender, RoutedEventArgs e)
        {
            if(EditMode == EditModeType.Create)
                Roster.Remove(SelectedFencer);

            ClearTextBoxes();
            EditMode = EditModeType.None;
            grdFencerData.IsEnabled = false;
            pnlRoster.IsEnabled = true;
            imgFencerImage.Source = null;
            SelectedFencer = null;
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

            // Register Events
            CtrTournament.EnterBattleEvent += CtrTournament_EnterBattleEvent;

            // Start Tournament
            StartTournament();
        }

        private void CtrTournament_EnterBattleEvent(object sender, EventArgs e)
        {
            SingleBattleWindow w = new SingleBattleWindow();
            w.InitializeRoster(Roster);
            if (e != null)
            {
                // Edit Battle
                BattleEventArgs be = e as BattleEventArgs;
                if (be != null)
                {
                    w.SetBattle(be.ID1, be.ID2, be.Score1, be.Score2);
                }
            }
            bool? result = w.ShowDialog();
            if (result == true)
            {
                if (w.EditMode)
                {
                    CtrTournament.EditBattle(w.Battle_Fencer1, w.Battle_Score1, w.Battle_Fencer2, w.Battle_Score2, true);
                }
                else
                {
                    CtrTournament.AddBattle(w.Battle_Fencer1, w.Battle_Score1, w.Battle_Fencer2, w.Battle_Score2, true);
                }
            }
        }

        private Fencer GetFencerByID(int rosterID)
        {
            Fencer f = null;

            for(int i = 0; i < Roster.Count; i++)
            {
                if(Roster[i].RosterID == rosterID)
                {
                    f = Roster[i];
                    break;
                }
            }

            return f;
        }

        private void StartTournament()
        {
            // Check for backup file
            if (System.IO.File.Exists(Properties.Settings.Default.DocumentDir + @"backup.xml"))
            {
                List<BattleInfo> backup_battles = null;

                // Get battles from backup file
                XmlSerializer serializer = new XmlSerializer(typeof(List<BattleInfo>));
                using (System.IO.FileStream fs = new System.IO.FileStream(Properties.Settings.Default.DocumentDir + @"backup.xml", System.IO.FileMode.Open))
                {
                    backup_battles = (List<BattleInfo>)serializer.Deserialize(fs);
                }

                if (backup_battles != null)
                {
                    for(int i = 0; i < backup_battles.Count; i++)
                    {
                        BattleInfo bi = backup_battles[i];
                        Fencer f;
                        CtrTournament.AddBattle(GetFencerByID(bi.Fencer1.RosterID), bi.Score1, GetFencerByID(bi.Fencer2.RosterID), bi.Score2, false);
                    }
                }
            }
            else
            {

#if DEBUG
                if (Roster.Count > 1)
                {
                    Random r = new Random();
                    for (int i = 0; i < 10; i++)
                    {
                        int f1 = 0;
                        int f2 = 0;
                        do
                        {
                            f1 = r.Next(Roster.Count);
                            f2 = r.Next(Roster.Count);
                        }
                        while (f1 == f2);

                        int s1 = 0;
                        int s2 = 0;
                        do
                        {
                            s1 = r.Next(6);
                            s2 = r.Next(6);
                        }
                        while (s1 == s2);

                        if (f1 != f2 && s1 != s2)
                        {
                            CtrTournament.AddBattle(Roster[f1], s1, Roster[f2], s2, true);
                        }
                    }
                }
#endif
            }
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
            CtrTournament.ResetTournament();
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
