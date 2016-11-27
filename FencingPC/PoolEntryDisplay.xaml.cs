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
    /// Interaktionslogik für PoolEntryDisplay.xaml
    /// </summary>
    public partial class PoolEntryDisplay : UserControl
    {
        public event EventHandler CallForEdit;

        private int m_Score;

        public int Score
        {
            get { return m_Score; }
            set { m_Score = value; }
        }

        private bool m_Victory;

        public bool Victory
        {
            get { return m_Victory; }
            set { m_Victory = value; }
        }

        private int m_ScoreRow;

        public int ScoreRow
        {
            get { return m_ScoreRow; }
        }

        private int m_ScoreCol;

        public int ScoreCol
        {
            get { return m_ScoreCol; }
        }

        public PoolEntryDisplay()
        {
            InitializeComponent();
        }

        public PoolEntryDisplay(int score, bool victory, int row, int col)
        {
            InitializeComponent();
            m_Score = score;
            m_Victory = victory;

            m_ScoreRow = row;
            m_ScoreCol = col;
        }

        public void Refresh()
        {
            string display = (m_Victory ? "V" : "D") + m_Score.ToString();
            tbDisplay.Text = display;
            Color background = (m_Victory ? Colors.Lime : Colors.Red);
            bdrDisplay.Background = new RadialGradientBrush(Colors.White, background);
        }

        private void PoolEntryDisplay_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (CallForEdit != null)
            {
                CallForEdit(this, null);
            }
        }

        private void menuEditBattle_Click(object sender, RoutedEventArgs e)
        {
            if(CallForEdit != null)
            {
                CallForEdit(this, null);
            }
        }
    }
}
