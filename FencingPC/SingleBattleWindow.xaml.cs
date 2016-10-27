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

namespace FencingPC
{
    /// <summary>
    /// Interaktionslogik für SingleBattleWindow.xaml
    /// </summary>
    public partial class SingleBattleWindow : Window
    {
        private IEnumerable<Fencer> Roster = null;
        //public BattleInfo ResultingBattle = null;
        private string ProgramPath;

        private Fencer m_Battle_Fencer1;

        public Fencer Battle_Fencer1
        {
            get { return m_Battle_Fencer1; }
        }

        private int m_Battle_Score1;

        public int Battle_Score1
        {
            get { return m_Battle_Score1; }
        }

        private Fencer m_Battle_Fencer2;

        public Fencer Battle_Fencer2
        {
            get { return m_Battle_Fencer2; }
        }

        private int m_Battle_Score2;

        public int Battle_Score2
        {
            get { return m_Battle_Score2; }
        }

        private bool m_EditMode = false;

        public bool EditMode
        {
            get { return m_EditMode; }
        }

        public SingleBattleWindow()
        {
            ProgramPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            InitializeComponent();
        }

        public void InitializeRoster(IEnumerable<Fencer> roster)
        {
            Roster = roster;
            cbFencer1.ItemsSource = roster;
            cbFencer1.DisplayMemberPath = "DisplayName";
            cbFencer2.ItemsSource = roster;
            cbFencer2.DisplayMemberPath = "DisplayName";
        }

        public void SetBattle(int id1, int id2, int score1, int score2)
        {
            try
            {
                Fencer f = Roster.First(o => o.RosterID == id1);
                cbFencer1.SelectedItem = f;
                cbFencer1.IsEnabled = false;
            }
            catch
            {
                cbFencer1.SelectedItem = null;
            }

            try
            {
                Fencer f = Roster.First(o => o.RosterID == id2);
                cbFencer2.SelectedItem = f;
                cbFencer2.IsEnabled = false;
            }
            catch
            {
                cbFencer2.SelectedItem = null;
            }

            pmScore1.ScoreValue = score1;
            pmScore2.ScoreValue = score2;

            m_EditMode = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (cbFencer1.SelectedIndex >= 0 && cbFencer2.SelectedIndex >= 0 && cbFencer1.SelectedIndex != cbFencer2.SelectedIndex)
            {
                m_Battle_Fencer1 = (Fencer)cbFencer1.SelectedValue;
                m_Battle_Score1 = pmScore1.ScoreValue;
                m_Battle_Fencer2 = (Fencer)cbFencer2.SelectedValue;
                m_Battle_Score2 = pmScore2.ScoreValue;

                this.DialogResult = true;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(this, "Gefecht löschen?", "Gefecht löschen", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                m_Battle_Fencer1 = (Fencer)cbFencer1.SelectedValue;
                m_Battle_Score1 = 0;
                m_Battle_Fencer2 = (Fencer)cbFencer2.SelectedValue;
                m_Battle_Score2 = 0;

                this.DialogResult = true;
            }
        }

        private void cbFencer1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fencer f = (Fencer)cbFencer1.SelectedValue;
            string imageID = f.ImageName;

            // Clear exisiting image
            imgFencer1.Source = null;

            if (!string.IsNullOrEmpty(imageID))
            {
                // Check for image
                if (System.IO.File.Exists(ProgramPath + @"\images\" + imageID))
                {
                    // Load image
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(ProgramPath + @"\images\" + imageID, UriKind.Absolute);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();

                    imgFencer1.Source = img;
                }
            }
        }

        private void cbFencer2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fencer f = (Fencer)cbFencer2.SelectedValue;
            string imageID = f.ImageName;

            // Clear exisiting image
            imgFencer2.Source = null;

            if (!string.IsNullOrEmpty(imageID))
            {
                // Check for image
                if (System.IO.File.Exists(ProgramPath + @"\images\" + imageID))
                {
                    // Load image
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(ProgramPath + @"\images\" + imageID, UriKind.Absolute);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();

                    imgFencer2.Source = img;
                }
            }
        }
    }
}
