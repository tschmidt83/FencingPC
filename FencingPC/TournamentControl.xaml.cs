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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FencingPC
{
    /// <summary>
    /// Interaction logic for TournamentControl.xaml
    /// </summary>
    public partial class TournamentControl : UserControl
    {
        private List<Fencer> m_FencersInTournament = new List<Fencer>();

        private List<BattleInfo> Battles = new List<BattleInfo>();

        public TournamentControl()
        {
            InitializeComponent();
        }

        private void scrTableaux_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if(sender == scrTableaux)
            {
                scrResults.ScrollToVerticalOffset(scrTableaux.VerticalOffset);
            }
            else
            {
                scrTableaux.ScrollToVerticalOffset(scrResults.VerticalOffset);
            }
        }

        public void AddBattle(Fencer f1, int score1, Fencer f2, int score2)
        {
            // Check if battle has already been fought
            if (m_FencersInTournament.Contains(f1) && m_FencersInTournament.Contains(f2))
                return;

            if (!m_FencersInTournament.Contains(f1))
            {
                f1.TournamentID = m_FencersInTournament.Count;
                m_FencersInTournament.Add(f1);                
            }
            if (!m_FencersInTournament.Contains(f2))
            {
                f2.TournamentID = m_FencersInTournament.Count;
                m_FencersInTournament.Add(f2);
            }

            Battles.Add(new BattleInfo(f1, score1, f2, score2));
            Battles.Add(new BattleInfo(f2, score2, f1, score1));

            Refresh();
        }

        private void Refresh()
        {
            // Clear old table
            grdTableaux.Children.Clear();

            // Header row
            grdTableaux.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Create first column: names
            grdTableaux.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            for(int i = 0; i < m_FencersInTournament.Count; i++)
            {
                grdTableaux.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                TextBlock tb_left = new TextBlock() { Text = (m_FencersInTournament[i].TournamentID + 1).ToString() + ": " + m_FencersInTournament[i].DisplayName, Style = Application.Current.Resources["TableauxHeaderStyle"] as Style };
                Grid.SetRow(tb_left, i + 1);
                Grid.SetColumn(tb_left, 0);
                grdTableaux.Children.Add(tb_left);

                grdTableaux.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                TextBlock tb_top = new TextBlock() { Text = (m_FencersInTournament[i].TournamentID + 1).ToString(), Style = Application.Current.Resources["TableauxTopHeaderStyle"] as Style };
                Grid.SetRow(tb_top, 0);
                Grid.SetColumn(tb_top, i + 1);
                grdTableaux.Children.Add(tb_top);
            }

            // Process battles
            for (int i = 0; i < Battles.Count; i++)
            {
                TextBlock res1 = new TextBlock() { Text = Battles[i].Score1.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style };
                Grid.SetRow(res1, Battles[i].Fencer1.TournamentID + 1);
                Grid.SetColumn(res1, Battles[i].Fencer2.TournamentID + 1);
                grdTableaux.Children.Add(res1);

                //TextBlock res2 = new TextBlock() { Text = Battles[i].Score2.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style };
                //Grid.SetRow(res2, Battles[i].Fencer2.TournamentID + 1);
                //Grid.SetColumn(res2, Battles[i].Fencer1.TournamentID + 1);
                //grdTableaux.Children.Add(res2);
            }
        }
    }
}
