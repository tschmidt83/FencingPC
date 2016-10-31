using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace FencingPC
{
    [Serializable]
    public class BattleInfo
    {
        private Fencer m_Fencer1;

        public Fencer Fencer1
        {
            get { return m_Fencer1; }
            set { m_Fencer1 = value; }
        }

        private int m_Score1;

        public int Score1
        {
            get { return m_Score1; }
            set { m_Score1 = value; }
        }

        private Fencer m_Fencer2;

        public Fencer Fencer2
        {
            get { return m_Fencer2; }
            set { m_Fencer2 = value; }
        }

        private int m_Score2;

        public int Score2
        {
            get { return m_Score2; }
            set { m_Score2 = value; }
        }

        public BattleInfo()
        {
        }

        public BattleInfo(Fencer f1, int s1, Fencer f2, int s2)
        {
            m_Fencer1 = f1;
            m_Score1 = s1;
            m_Fencer2 = f2;
            m_Score2 = s2;
        }
    }
}
