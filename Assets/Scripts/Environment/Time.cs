using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Timers;

namespace Environment
{
    /// <summary>
    /// The MacabreDate Time contains methods that is similar the the UNIX clock
    /// </summary>
    [DataContract]
    public class Time
    {
        // The Meridian Indicator
        public enum MIndicator
        {
            AM = 0,
            PM = 1
        }

        public enum DayOfWeek {
            Monday = 0,
            Tuesday = 1,
            Wednesday = 2,
            Thrusday = 3,
            Friday = 4,
            Saturday = 5,
            Sunday = 6
        };

        [DataMember(IsRequired = true, Order = 0)]
        public int totalSeconds = 0;

        // The current gametime in 24 hour time
        public int second
        {
            get { return totalSeconds % 60; }
        }
        public int minute
        {
            get { return (totalSeconds / 60) % 60; }
        }
        public int hour
        {
            get { return (totalSeconds / 3600) % 60; }
        }
        public int day
        {
            get { return (totalSeconds / (24 * 36000) % 24); }
        }
        public int week
        {
            get { return (totalSeconds / (24 * 36000 * 7) % 7); }
        }
        public int month
        {
            get { return (totalSeconds / (24 * 36000 * 7 * 4) % 4); }
        }
        public int year
        {
            get { return (totalSeconds / (24 * 36000 * 7 * 52) % 52); }
        }

        public MIndicator cycle
        {
            get { return (MIndicator)(hour % 12); }
        }
        public DayOfWeek dayOfWeek
        {
            get { return (DayOfWeek)(day % 7); }
        }
        
        public void Tick()
        {
            totalSeconds += 1;
        }

        public override string ToString()
        {
            return "Time: Week " + week + ", " + day + ", Hour " + hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2") + " " + cycle;
        }
    }
}