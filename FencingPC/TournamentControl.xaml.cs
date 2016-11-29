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

using System.Xml.Serialization;

namespace FencingPC
{
    /// <summary>
    /// Interaction logic for TournamentControl.xaml
    /// </summary>
    public partial class TournamentControl : UserControl
    {
        private List<Fencer> FencersInTournament = new List<Fencer>();

        private Dictionary<int, ResultInfo> FencerResults = new Dictionary<int, ResultInfo>();

        public event EventHandler EnterBattleEvent;

        private List<BattleInfo> BattleList = new List<BattleInfo>();

        public TournamentControl()
        {
            InitializeComponent();
        }

        public void ResetTournament()
        {
            // Clear variables
            FencersInTournament.Clear();
            FencerResults = new Dictionary<int, ResultInfo>();
            BattleList.Clear();

            // Remove backup file
            if(System.IO.File.Exists(Properties.Settings.Default.DocumentDir + @"backup.xml"))
            {
                System.IO.File.Delete(Properties.Settings.Default.DocumentDir + @"backup.xml");
            }

            grdTableaux.Children.Clear();
            grdTableaux.RowDefinitions.Clear();
            grdTableaux.ColumnDefinitions.Clear();

            grdResults.Children.Clear();
            grdResults.RowDefinitions.Clear();
        }

        private int FindBattle(Fencer f1, Fencer f2)
        {
            int battleIndex = -1;

            for(int i = 0; i < BattleList.Count; i++)
            {
                BattleInfo bi = BattleList[i];
                if ((bi.Fencer1 == f1 && bi.Fencer2 == f2) || (bi.Fencer1 == f2 && bi.Fencer2 == f1))
                    battleIndex = i;
            }

            return battleIndex;
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

        public void EditBattle(Fencer f1, int score1, Fencer f2, int score2, bool modifyBackupFile)
        {
            int idx = FindBattle(f1, f2);

            if (idx == -1)
                return;

            BattleInfo bi = BattleList[idx];            

            // Check if battle should be removed
            if (score1 == 0 && score2 == 0)
            {
                // Remove battle completely
                BattleList.Remove(bi);
            }
            else
            {
                // Change battle result
                BattleInfo bi_new = new BattleInfo(f1, score1, f2, score2);
                BattleList.Remove(bi);
                BattleList.Add(bi_new);
            }

            if (modifyBackupFile)
            {
                // Write new battle list to backup file
                XmlSerializer serializer = new XmlSerializer(typeof(List<BattleInfo>));
                using (System.IO.FileStream fs = new System.IO.FileStream(Properties.Settings.Default.DocumentDir + @"backup.xml", System.IO.FileMode.Create))
                {
                    serializer.Serialize(fs, BattleList);
                }
            }

            // Recalculate all results
            for (int i = 0; i < FencersInTournament.Count; i++)
            {
                FencerResults[FencersInTournament[i].TournamentID].ClearResult();
            }

            for(int i = 0; i < BattleList.Count; i++)
            {
                BattleInfo b_res = BattleList[i];
                int idx1 = b_res.Fencer1.TournamentID;
                int idx2 = b_res.Fencer2.TournamentID;

                FencerResults[idx1].Refresh(b_res.Score1, b_res.Score2);
                FencerResults[idx2].Refresh(b_res.Score2, b_res.Score1);
            }

            // Refresh Display
            RefreshDisplay();
        }

        public void AddBattle(Fencer f1, int score1, Fencer f2, int score2, bool modifyBackupFile)
        {
            // Check if battle already exists
            if (FindBattle(f1, f2) >= 0)
                return;

            BattleList.Add(new BattleInfo(f1, score1, f2, score2));
            
            if(modifyBackupFile)
            {
                // Write new battle list to backup file
                XmlSerializer serializer = new XmlSerializer(typeof(List<BattleInfo>));
                using (System.IO.FileStream fs = new System.IO.FileStream(Properties.Settings.Default.DocumentDir + @"backup.xml", System.IO.FileMode.Create))
                {
                    serializer.Serialize(fs, BattleList);
                }
            }

            // Check if fencers are already in tournament list
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

            // Ranking
            #region Ranking
            // Process hits/wins/losses
            if (!FencerResults.ContainsKey(f1.TournamentID))
                FencerResults.Add(f1.TournamentID, new ResultInfo(f1.TournamentID));
            if (!FencerResults.ContainsKey(f2.TournamentID))
                FencerResults.Add(f2.TournamentID, new ResultInfo(f2.TournamentID));

            FencerResults[f1.TournamentID].Refresh(score1, score2);
            FencerResults[f2.TournamentID].Refresh(score2, score1);

            // First criterion: sort by wins (descending), index 1 (descending), index 2 (descending)
            IComparer<KeyValuePair<int, ResultInfo>> comparer = new RankingOrderClass();
            List<KeyValuePair<int, ResultInfo>> ranking = FencerResults.ToList();
            ranking.Sort(comparer);

            for (int i = 0; i < ranking.Count; i++)
            {
                FencerResults[ranking[i].Key].Rank = i + 1;
            }
            #endregion

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
            #region Display battles
            for(int i = 0; i < BattleList.Count; i++)
            {
                BattleInfo bi = BattleList[i];

                // Display result for fencer 1                
                bool victory = bi.Score1 > bi.Score2 ? true : false;
                PoolEntryDisplay entry = new PoolEntryDisplay(bi.Score1, victory, bi.Fencer1.TournamentID, bi.Fencer2.TournamentID);
                entry.CallForEdit += PoolEntryDisplay_CallForEdit;
                Grid.SetRow(entry, bi.Fencer1.TournamentID);
                Grid.SetColumn(entry, bi.Fencer2.TournamentID);
                grdTableaux.Children.Add(entry);
                entry.Refresh();

                // Display result for fencer 1                
                PoolEntryDisplay entry2 = new PoolEntryDisplay(bi.Score2, !victory, bi.Fencer2.TournamentID, bi.Fencer1.TournamentID);
                entry2.CallForEdit += PoolEntryDisplay_CallForEdit;
                Grid.SetRow(entry2, bi.Fencer2.TournamentID);
                Grid.SetColumn(entry2, bi.Fencer1.TournamentID);
                grdTableaux.Children.Add(entry2);
                entry2.Refresh();
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

                if (scoreRow > FencersInTournament.Count || scoreCol > FencersInTournament.Count)
                    return;

                Fencer f1 = FencersInTournament[scoreRow - 1];
                Fencer f2 = FencersInTournament[scoreCol - 1];

                // Edit battle
                int idx = FindBattle(f1, f2);
                if (idx >= 0)
                {
                    BattleInfo bi = BattleList[idx];

                    if (EnterBattleEvent != null)
                    {
                        EnterBattleEvent(this, new BattleEventArgs(bi.Fencer1.RosterID, bi.Fencer2.RosterID, bi.Score1, bi.Score2));
                    }
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

        private void btnExport_Battles_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "CSV-File (*.csv)|*.csv";
            dlg.Title = GetResourceString("str_Export_Battles");
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(dlg.FileName, false))
                {
                    foreach (BattleInfo bi in BattleList)
                    {
                        writer.WriteLine(String.Format("{0};{1};{2};{3};", bi.Fencer1.DisplayName, bi.Fencer2.DisplayName, bi.Score1, bi.Score2));
                    }

                    writer.Close();
                }

                AskForTournamentReset();
            }
        }

        private void btnExport_Tableaux_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "CSV-File (*.csv)|*.csv";
            dlg.Title = GetResourceString("str_Export_Table");
            bool? result = dlg.ShowDialog();

            if (result == true)
            {

                AskForTournamentReset();
            }
        }

        private void btnExport_Results_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.Filter = "CSV-File (*.csv)|*.csv";
            dlg.Title = GetResourceString("str_Export_Results");
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(dlg.FileName, false))
                {
                    writer.Write(GetResourceString("str_Name") + ";");
                    writer.Write(GetResourceString("str_Victories") + ";");
                    writer.Write(GetResourceString("str_Index_1") + ";");
                    writer.Write(GetResourceString("str_HitsGiven") + ";");
                    writer.Write(GetResourceString("str_HitsTaken") + ";");
                    writer.Write(GetResourceString("str_Index_2") + ";");
                    writer.WriteLine(GetResourceString("str_Rank") + ";");

                    for (int i = 0; i < FencersInTournament.Count; i++)
                    {
                        int current_ID = FencersInTournament[i].TournamentID;

                        ResultInfo r = null;
                        try
                        {
                            r = FencerResults[current_ID];
                        }
                        catch
                        {
                            r = null;
                        }

                        if (r != null)
                        {
                            writer.WriteLine("{0};{1};{2:0.0};{3};{4};{5};{6}", FencersInTournament[i].DisplayName, r.Wins, r.WinRatio, r.HitsGiven, r.HitsTaken, r.HitIndex, r.Rank);
                        }
                    }

                    writer.Close();
                }

                AskForTournamentReset();
            }
        }

        private void AskForTournamentReset()
        {
            MessageBoxResult r = MessageBox.Show(GetResourceString("str_ResetTournamentQuestion"), GetResourceString("str_ResetTournament"), MessageBoxButton.YesNo);
            if (r == MessageBoxResult.Yes)
            {
                ResetTournament();
            }
        }

        private string GetResourceString(string identifier)
        {
            string result = string.Empty;

            if (Application.Current.Resources[identifier] != null)
                result = Application.Current.Resources[identifier].ToString();

            return result;
        }
    }

    public class RankingOrderClass : IComparer<KeyValuePair<int, ResultInfo>>
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
