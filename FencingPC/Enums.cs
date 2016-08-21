using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FencingPC
{
    /// <summary>
    ///  Enumeration of possible weapon types
    /// </summary>
    public enum WeaponType
    {
        Epee = 0,
        Foil,
        Sabre
    }

    /// <summary>
    /// Enumeration of possible genders
    /// </summary>
    public enum GenderType
    {
        Male = 0,
        Female
    }

    /// <summary>
    /// Enumeration for possible types of club membership.
    /// Useful for generating statistics.
    /// </summary>
    public enum MembershipType
    {
        RegularMember = 0,      // Regular fencing club member, useful for club rankings
        Student,                // Student menber, typically for a short period of time, e.g. from a university course
        Guest                   // Guest (no member at all), could be excluded from ranking
    }
}
