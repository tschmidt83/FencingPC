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

        public event EventHandler EnterBattleEvent;

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

        public void EditBattle(Fencer f1, int score1, Fencer f2, int score2, bool modifyLockFile)
        {
            if (!FencersInTournament.Contains(f1))
                return;
            if (!FencersInTournament.Contains(f2))
                return;
            if (!BattleCollection.ContainsKey(f1.TournamentID))
                return;
            if (!BattleCollection.ContainsKey(f2.TournamentID))
                return;

            if (!FencerResults.ContainsKey(f1.TournamentID))
                return;
            if (!FencerResults.ContainsKey(f2.TournamentID))
                return;

            // Edit battle
            for (int i = 0; i < BattleCollection[f1.TournamentID].Count; i++)
            {
                BattleInfo bi = BattleCollection[f1.TournamentID][i];
                if (bi.Fencer1.RosterID == f1.RosterID && bi.Fencer2.RosterID == f2.RosterID)
                {
                    // Check if battle should be removed
                    if (score1 == 0 && score2 == 0)
                    {
                        BattleCollection[f1.TournamentID].RemoveAt(i);
                        break;
                    }
                    else
                    {
                        bi.Score1 = score1;
                        bi.Score2 = score2;
                        break;
                    }
                }
                else if (bi.Fencer1.RosterID == f2.RosterID && bi.Fencer2.RosterID == f1.RosterID)
                {
                    // Check if battle should be removed
                    if (score1 == 0 && score2 == 0)
                    {
                        BattleCollection[f1.TournamentID].RemoveAt(i);
                        break;
                    }
                    else
                    {
                        bi.Score1 = score2;
                        bi.Score2 = score1;
                        break;
                    }
                }
            }

            for (int i = 0; i < BattleCollection[f2.TournamentID].Count; i++)
            {
                BattleInfo bi = BattleCollection[f2.TournamentID][i];
                if (bi.Fencer1.RosterID == f1.RosterID && bi.Fencer2.RosterID == f2.RosterID)
                {
                    // Check if battle should be removed
                    if (score1 == 0 && score2 == 0)
                    {
                        BattleCollection[f2.TournamentID].RemoveAt(i);
                        break;
                    }
                    else
                    {
                        bi.Score1 = score1;
                        bi.Score2 = score2;
                        break;
                    }
                }
                else if (bi.Fencer1.RosterID == f2.RosterID && bi.Fencer2.RosterID == f1.RosterID)
                {
                    // Check if battle should be removed
                    if (score1 == 0 && score2 == 0)
                    {
                        BattleCollection[f2.TournamentID].RemoveAt(i);
                        break;
                    }
                    else
                    {
                        bi.Score1 = score2;
                        bi.Score2 = score1;
                        break;
                    }
                }
            }

            FencerResults[f1.TournamentID].ClearResult();
            FencerResults[f2.TournamentID].ClearResult();

            // Recalculate results
            for (int i = 0; i < BattleCollection[f1.TournamentID].Count; i++)
            {
                BattleInfo bi = BattleCollection[f1.TournamentID][i];
                FencerResults[f1.TournamentID].Refresh(bi);
            }

            for (int i = 0; i < BattleCollection[f2.TournamentID].Count; i++)
            {
                BattleInfo bi = BattleCollection[f2.TournamentID][i];
                FencerResults[f2.TournamentID].Refresh(bi);
            }

            // Refresh Display
            RefreshDisplay();
        }

        public void AddBattle(Fencer f1, int score1, Fencer f2, int score2, bool modifyLockFile)
        {
            bool battleExists = false;

            if (!FencersInTournament.Contains(f1))
            {
                f1.TournamentID = FencersInTournament.Count + 1;
                FencersInTournament.Add(f1);
            }
            if (!FencersInTournament.Contains(f2))
            {
                f2.TournamentID = FencersInTournament.Count + 1;
                FencersInTournament.Add(f2);
            }

            if (!BattleCollection.ContainsKey(f1.TournamentID))
                BattleCollection.Add(f1.TournamentID, new List<BattleInfo>());
            if (!BattleCollection.ContainsKey(f2.TournamentID))
                BattleCollection.Add(f2.TournamentID, new List<BattleInfo>());

            // Check if battle has already been fought           
            for (int i = 0; i < BattleCollection[f1.TournamentID].Count; i++)
            {
                BattleInfo bi = BattleCollection[f1.TournamentID][i];
                if (bi.Fencer1.RosterID == f1.RosterID && bi.Fencer2.RosterID == f2.RosterID)
                    battleExists = true;
                if (bi.Fencer1.RosterID == f2.RosterID && bi.Fencer2.RosterID == f1.RosterID)
                    battleExists = true;
            }

            if (!battleExists)
            {
                BattleInfo b1 = new BattleInfo(f1, score1, f2, score2);
                BattleInfo b2 = new BattleInfo(f2, score2, f1, score1);

                BattleCollection[f1.TournamentID].Add(b1);
                BattleCollection[f2.TournamentID].Add(b2);

                // Ranking
                #region Ranking
                // Process hits/wins/losses
                if (!FencerResults.ContainsKey(f1.TournamentID))
                    FencerResults.Add(f1.TournamentID, new ResultInfo(f1.TournamentID));
                if (!FencerResults.ContainsKey(f2.TournamentID))
                    FencerResults.Add(f2.TournamentID, new ResultInfo(f2.TournamentID));

                FencerResults[f1.TournamentID].Refresh(b1);
                FencerResults[f2.TournamentID].Refresh(b2);

                // First criterion: sort by wins (descending), index 1 (descending), index 2 (descending)
                IComparer<KeyValuePair<int, ResultInfo>> comparer = new RangingOrderClass();
                List<KeyValuePair<int, ResultInfo>> ranking = FencerResults.ToList();
                ranking.Sort(comparer);

                for (int i = 0; i < ranking.Count; i++)
                {
                    FencerResults[ranking[i].Key].Rank = i + 1;
                }
                #endregion
            }
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            // Clear old table
            grdTableaux.Children.Clear();
            grdTableaux.ColumnDefinitions.Clear();
            grdTableaux.RowDefinitions.Clear();

            // Header row
            grdTableaux.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });

            // Create first column and top row: names
            grdTableaux.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            for (int i = 0; i < FencersInTournament.Count; i++)
            {
                // Column 0
                grdTableaux.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                TextBlock tb_left = new TextBlock() { Text = FencersInTournament[i].TournamentID.ToString() + ": " + FencersInTournament[i].DisplayName, Style = Application.Current.Resources["TableauxHeaderStyle"] as Style };
                Grid.SetRow(tb_left, i + 1);
                Grid.SetColumn(tb_left, 0);
                grdTableaux.Children.Add(tb_left);

                // Row 0
                grdTableaux.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                TextBlock tb_top = new TextBlock() { Text = FencersInTournament[i].TournamentID.ToString(), Style = Application.Current.Resources["TableauxTopHeaderStyle"] as Style };
                Grid.SetRow(tb_top, 0);
                Grid.SetColumn(tb_top, i + 1);
                grdTableaux.Children.Add(tb_top);
            }

            // Display battles
            #region Process battles
            for (int i = 0; i < FencersInTournament.Count; i++)
            {
                int current_ID = FencersInTournament[i].TournamentID;

                if (BattleCollection.ContainsKey(current_ID))
                {
                    List<BattleInfo> b = BattleCollection[current_ID];

                    for (int m = 0; m < b.Count; m++)
                    {
                        bool victory = b[m].Score1 > b[m].Score2 ? true : false;
                        PoolEntryDisplay entry = new PoolEntryDisplay(b[m].Score1, victory, b[m].Fencer1.TournamentID - 1, b[m].Fencer2.TournamentID - 1);
                        entry.CallForEdit += PoolEntryDisplay_CallForEdit;
                        Grid.SetRow(entry, b[m].Fencer1.TournamentID);
                        Grid.SetColumn(entry, b[m].Fencer2.TournamentID);
                        grdTableaux.Children.Add(entry);
                        entry.Refresh();

                        /*
                        TextBlock res1 = new TextBlock() { Text = b[m].Score1.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style };
                        Grid.SetRow(res1, b[m].Fencer1.TournamentID);
                        Grid.SetColumn(res1, b[m].Fencer2.TournamentID);
                        grdTableaux.Children.Add(res1);
                        */
                    }
                }
                else
                {
                    throw new Exception("Error: no battles with ID " + current_ID.ToString());
                }
            }
            #endregion

            // Display results
            #region Display results            
            grdResults.Children.Clear();
            grdResults.RowDefinitions.Clear();

            grdResults.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100, GridUnitType.Pixel) });
            AddToResultGrid(new TextBlock() { Text = Application.Current.Resources["str_Victories"].ToString(), Style = Application.Current.Resources["ResultsTopHeaderStyle"] as Style }, 0, 0);
            AddToResultGrid(new TextBlock() { Text = Application.Current.Resources["str_Index_1"].ToString(), Style = Application.Current.Resources["ResultsTopHeaderStyle"] as Style }, 0, 1);
            AddToResultGrid(new TextBlock() { Text = Application.Current.Resources["str_HitsGiven"].ToString(), Style = Application.Current.Resources["ResultsTopHeaderStyle"] as Style }, 0, 2);
            AddToResultGrid(new TextBlock() { Text = Application.Current.Resources["str_HitsTaken"].ToString(), Style = Application.Current.Resources["ResultsTopHeaderStyle"] as Style }, 0, 3);
            AddToResultGrid(new TextBlock() { Text = Application.Current.Resources["str_Index_2"].ToString(), Style = Application.Current.Resources["ResultsTopHeaderStyle"] as Style }, 0, 4);
            AddToResultGrid(new TextBlock() { Text = Application.Current.Resources["str_Rank"].ToString(), Style = Application.Current.Resources["ResultsTopHeaderStyle"] as Style }, 0, 5);

            for (int i = 0; i < FencersInTournament.Count; i++)
            {
                int current_ID = FencersInTournament[i].TournamentID;

                try
                {
                    ResultInfo r = FencerResults[current_ID];
                    grdResults.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    AddToResultGrid(new TextBlock() { Text = r.Wins.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style }, current_ID, 0);
                    AddToResultGrid(new TextBlock() { Text = r.WinRatio.ToString("0.0"), Style = Application.Current.Resources["TableauxResultStyle"] as Style }, current_ID, 1);
                    AddToResultGrid(new TextBlock() { Text = r.HitsGiven.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style }, current_ID, 2);
                    AddToResultGrid(new TextBlock() { Text = r.HitsTaken.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style }, current_ID, 3);
                    AddToResultGrid(new TextBlock() { Text = r.HitIndex.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style }, current_ID, 4);
                    AddToResultGrid(new TextBlock() { Text = r.Rank.ToString(), Style = Application.Current.Resources["TableauxResultStyle"] as Style }, current_ID, 5);
                }
                catch
                {
                    throw new Exception("Error: no results with ID " + current_ID.ToString());
                }
            }
            #endregion
        }

        private void PoolEntryDisplay_CallForEdit(object sender, EventArgs e)
        {
            PoolEntryDisplay pe = sender as PoolEntryDisplay;
            if (pe != null)
            {
                int scoreRow = pe.ScoreRow;
                int scoreCol = pe.ScoreCol;

                if (scoreRow >= FencersInTournament.Count || scoreCol >= FencersInTournament.Count)
                    return;

                Fencer f1 = FencersInTournament[scoreRow];
                Fencer f2 = FencersInTournament[scoreCol];

                int bscore1 = 0;
                int bscore2 = 0;

                // Edit battle
                for (int i = 0; i < BattleCollection[f1.TournamentID].Count; i++)
                {
                    BattleInfo bi = BattleCollection[f1.TournamentID][i];
                    if (bi.Fencer1.RosterID == f1.RosterID && bi.Fencer2.RosterID == f2.RosterID)
                    {
                        bscore1 = bi.Score1;
                        bscore2 = bi.Score2;
                    }
                    else if (bi.Fencer1.RosterID == f2.RosterID && bi.Fencer2.RosterID == f1.RosterID)
                    {
                        bscore1 = bi.Score2;
                        bscore2 = bi.Score1;
                    }
                }

                if (EnterBattleEvent != null)
                {
                    EnterBattleEvent(this, new BattleEventArgs(f1.RosterID, f2.RosterID, bscore1, bscore2));
                }
            }
        }

        private void AddToResultGrid(UIElement child, int row, int col)
        {
            Grid.SetRow(child, row);
            Grid.SetColumn(child, col);
            grdResults.Children.Add(child);
        }

        private void btnEnterBattle_Click(object sender, RoutedEventArgs e)
        {
            if (EnterBattleEvent != null)
            {
                EnterBattleEvent(this, null);
            }
        }
    }

    public class RangingOrderClass : IComparer<KeyValuePair<int, ResultInfo>>
    {
        public int Compare(KeyValuePair<int, ResultInfo> x, KeyValuePair<int, ResultInfo> y)
        {
            // Compare by wins, descending
            int compareResult = y.Value.Wins.CompareTo(x.Value.Wins);
            if (compareResult == 0)
            {
                // Compare by index 1, descending
                compareResult = y.Value.WinRatio.CompareTo(x.Value.WinRatio);
                if (compareResult == 0)
                {
                    // Compare by index 2, descending
                    compareResult = y.Value.HitIndex.CompareTo(x.Value.HitIndex);
                }
            }
            return compareResult;
        }
    }
}
