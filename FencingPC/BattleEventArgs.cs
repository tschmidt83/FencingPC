using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FencingPC
{
    public class BattleEventArgs : EventArgs
    {
        private int m_ID1;

        public int ID1
        {
            get { return m_ID1; }
        }

        private int m_ID2;

        public int ID2
        {
            get { return m_ID2; }
        }

        private int m_Score1;

        public int Score1
        {
            get { return m_Score1; }
        }

        private int m_Score2;

        public int Score2
        {
            get { return m_Score2; }
        }

        public BattleEventArgs(int id_1, int id_2, int score_1, int score_2) : base()
        {
            m_ID1 = id_1;
            m_ID2 = id_2;
            m_Score1 = score_1;
            m_Score2 = score_2;
        }
    }
}
