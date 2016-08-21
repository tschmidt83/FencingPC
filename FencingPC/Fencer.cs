using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Serialization;

namespace FencingPC
{
    [Serializable]
    public class Fencer : INotifyPropertyChanged
    {
        /*** Gender ***/
        private GenderType m_Gender;

        /// <summary>
        /// Fencer's gender (defined in Enums.cs)
        /// </summary>
        public GenderType Gender
        {
            get { return m_Gender; }
            set { m_Gender = value; NotifyPropertyChanged("Gender"); }
        }

        /*** Membership ***/
        private MembershipType m_Membership;

        /// <summary>
        /// Fencer's type of membership (defined in Enums.cs)
        /// </summary>
        public MembershipType Membership
        {
            get { return m_Membership; }
            set { m_Membership = value; NotifyPropertyChanged("Membership"); }
        }

        /*** FirstName ***/
        private string m_FirstName;

        /// <summary>
        ///  Fencer's first name(s)
        /// </summary>
        public string FirstName
        {
            get { return m_FirstName; }
            set { m_FirstName = value; NotifyPropertyChanged("FirstName"); NotifyPropertyChanged("DisplayName"); }
        }

        /*** LastName ***/
        private string m_LastName;

        /// <summary>
        /// Fencer's last name(s)
        /// </summary>
        public string LastName
        {
            get { return m_LastName; }
            set { m_LastName = value; NotifyPropertyChanged("LastName"); NotifyPropertyChanged("DisplayName"); }
        }

        /*** BirthDate ***/
        private DateTime m_BirthDate;

        /// <summary>
        /// Fencer's date of birth
        /// </summary>
        public DateTime BirthDate
        {
            get { return m_BirthDate; }
            set { m_BirthDate = value; NotifyPropertyChanged("BirthDate"); }
        }

        /*** RosterID ***/
        private int m_RosterID;

        /// <summary>
        /// Unique roster ID
        /// </summary>
        public int RosterID
        {
            get { return m_RosterID; }
            set { m_RosterID = value; NotifyPropertyChanged("RosterID"); }
        }

        /*** TournamentID ***/
        private int m_TournamentID;

        /// <summary>
        /// Fencer's ID number in a tournament (will not be serialized)
        /// </summary>
        [XmlIgnore]
        public int TournamentID
        {
            get { return m_TournamentID; }
            set { m_TournamentID = value; NotifyPropertyChanged("TournamentID"); }
        }

        /*** ImageName ***/
        private string m_ImageName = string.Empty;

        /// <summary>
        /// Path to image file for fencer (if any)
        /// </summary>
        public string ImageName
        {
            get { return m_ImageName; }
            set { m_ImageName = value; NotifyPropertyChanged("ImageName"); }
        }

        /// <summary>
        /// Returns the display name of the fencer in the form [FirstName], [LastName]
        /// </summary>
        [XmlIgnore]
        public string DisplayName
        {
            get { return m_LastName + ", " + m_FirstName; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Fencer()
        {
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
