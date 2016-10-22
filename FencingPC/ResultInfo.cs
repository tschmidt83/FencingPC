using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FencingPC
{
    public class ResultInfo : INotifyPropertyChanged
    {
        /*** FencerID ***/
        private int m_FencerID;

        public int FencerID
        {
            get { return m_FencerID; }
            set { m_FencerID = value; NotifyPropertyChanged("FencerID"); }
        }        

        /*** HitsGiven ***/
        private int m_HitsGiven = 0;

        public int HitsGiven
        {
            get { return m_HitsGiven; }
            set { m_HitsGiven = value; NotifyPropertyChanged("HitsGiven"); }
        }

        /*** HitsTaken ***/
        private int m_HitsTaken = 0;

        public int HitsTaken
        {
            get { return m_HitsTaken; }
            set { m_HitsTaken = value; NotifyPropertyChanged("HitsTaken"); }
        }

        /*** NumberOfBattles ***/
        private int m_NumberOfBattles = 0;

        public int NumberOfBattles
        {
            get { return m_NumberOfBattles; }
            set { m_NumberOfBattles = value; NotifyPropertyChanged("NumberOfBattles"); }
        }

        /*** Wins ***/
        private int m_Wins = 0;

        public int Wins
        {
            get { return m_Wins; }
            set { m_Wins = value; NotifyPropertyChanged("Wins"); }
        }

        /*** WinRatio ***/
        private double m_WinRatio = 0;

        public double WinRatio
        {
            get { return m_WinRatio; }
            set { m_WinRatio = value; NotifyPropertyChanged("WinRatio"); }
        }

        /*** HitIndex ***/
        private int m_HitIndex = 0;

        public int HitIndex
        {
            get { return m_HitIndex; }
            set { m_HitIndex = value; NotifyPropertyChanged("HitIndex"); }
        }

        /*** Rank ***/
        private int m_Rank;

        public int Rank
        {
            get { return m_Rank; }
            set { m_Rank = value; NotifyPropertyChanged("Rank"); }
        }

        public ResultInfo()
        {            
        }

        public ResultInfo(int id)
        {
            m_FencerID = id;
        }

        /// <summary>
        /// Recalculate all values based on new battle info
        /// </summary>
        /// <param name="b">Result of recent battle</param>
        public void Refresh(BattleInfo b)
        {
            HitsGiven += b.Score1;
            HitsTaken += b.Score2;

            NumberOfBattles = NumberOfBattles + 1;

            if (b.Score1 > b.Score2)
                Wins = Wins + 1;

            WinRatio = (double)Wins / NumberOfBattles;

            HitIndex = HitsGiven - HitsTaken;
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
