using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class PlusMinusControl : UserControl, INotifyPropertyChanged
    {
        /*** ScoreValue ***/
        private int m_ScoreValue = 0;

        public int ScoreValue
        {
            get { return m_ScoreValue; }
            set { m_ScoreValue = value; NotifyPropertyChanged("ScoreValue"); }
        }

        public PlusMinusControl()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            ScoreValue--;
        }

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            ScoreValue++;
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

    }
}
