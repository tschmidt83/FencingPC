﻿using System;
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
        private Dictionary<int, List<BattleInfo>> BattleCollection = new Dictionary<int, List<BattleInfo>>();
        private List<Fencer> FencersInTournament = new List<Fencer>();

        private Dictionary<int, ResultInfo> FencerResults = new Dictionary<int, ResultInfo>();

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
            if (FencersInTournament.Contains(f1) && FencersInTournament.Contains(f2))
                return;

            if (!FencersInTournament.Contains(f1))
            {
                f1.TournamentID = FencersInTournament.Count;
                FencersInTournament.Add(f1);                
            }
            if (!FencersInTournament.Contains(f2))
            {
                f2.TournamentID = FencersInTournament.Count;
                FencersInTournament.Add(f2);
            }

            if (!BattleCollection.ContainsKey(f1.TournamentID))
                BattleCollection.Add(f1.TournamentID, new List<BattleInfo>());
            if(!BattleCollection.ContainsKey(f2.TournamentID))
                BattleCollection.Add(f2.TournamentID, new List<BattleInfo>());

            BattleCollection[f1.TournamentID].Add(new BattleInfo(f1, score1, f2, score2));
            BattleCollection[f2.TournamentID].Add(new BattleInfo(f2, score2, f1, score1));

            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            // Clear old table
            grdTableaux.Children.Clear();

            // Header row
            grdTableaux.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Create first column and top row: names
            grdTableaux.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            for(int i = 0; i < FencersInTournament.Count; i++)
            {
                // Column 0
                grdTableaux.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                TextBlock tb_left = new TextBlock() { Text = FencersInTournament[i].DisplayName, Style = Application.Current.Resources["TableauxHeaderStyle"] as Style };
                Grid.SetRow(tb_left, i + 1);
                Grid.SetColumn(tb_left, 0);
                grdTableaux.Children.Add(tb_left);

                // Row 0
                grdTableaux.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                TextBlock tb_top = new TextBlock() { Text = FencersInTournament[i].DisplayName, Style = Application.Current.Resources["TableauxTopHeaderStyle"] as Style };
                Grid.SetRow(tb_top, 0);
                Grid.SetColumn(tb_top, i + 1);
                grdTableaux.Children.Add(tb_top);
            }

            // Process battles
            for (int i = 0; i < FencersInTournament.Count; i++)
            {
                int current_ID = FencersInTournament[i].TournamentID;

                if (BattleCollection.ContainsKey(current_ID))
                {
                    List<BattleInfo> b = BattleCollection[current_ID];

                    for (int m = 0; m < b.Count; m++)
                    {
                        TextBlock res1 = new TextBlock() { Text = b[m].Score1.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style };
                        Grid.SetRow(res1, b[m].Fencer1.TournamentID + 1);
                        Grid.SetColumn(res1, b[m].Fencer2.TournamentID + 1);
                        grdTableaux.Children.Add(res1);

                        // Process hits/wins/losses
                        if (!FencerResults.ContainsKey(current_ID))
                            FencerResults.Add(current_ID, new ResultInfo());

                        FencerResults[current_ID].Refresh(b[m]);
                    }
                }
                else
                {
                    throw new Exception("Error: no battles with ID " + FencersInTournament[i].TournamentID.ToString());
                }
            }
        }
    }
}
