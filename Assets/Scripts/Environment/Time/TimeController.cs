using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Environment.Time
{

    /// <summary>
    /// The GameClock deals with the time of day of the clock, it does one duck every single second, which then updates the game.
    /// </summary>
    public static class TimeController
    {
        /// <summary>
        /// The clock rate
        /// At 1 : The clock rate is the same as real world
        /// At 60: It is 60 times faster thant the real world
        /// </summary>
        public const int ClockRate = 60;

        public static MacabreDateTime time = new MacabreDateTime(true);

        // We need a list of events that the gameclock invokes
        public delegate void UpdateTime();
        public delegate void UpdateSecond();
        public delegate void UpdateMinute();
        public delegate void UpdateHour();
        public delegate void UpdateDay();
        public delegate void UpdateWeek();

        public static event UpdateTime UpdateTimeEvent;
        public static event UpdateSecond UpdateSecondEvent;
        public static event UpdateMinute UpdateMinuteEvent;
        public static event UpdateHour UpdateHourEvent;
        public static event UpdateDay UpdateDayEvent;
        public static event UpdateWeek UpdateWeekEvent;
        
        static void IncrementTime()
        {
            MacabreDateTime newTime = new MacabreDateTime(ref time);
            time.AddSecond(1);

            if (newTime.second != time.second) UpdateSecondEvent();
            if (newTime.minute != time.minute) UpdateMinuteEvent();
            if (newTime.hour != time.hour) UpdateHourEvent();
            if (newTime.day != time.day) UpdateDayEvent();
            if (newTime.week != time.second) UpdateWeekEvent();

            time.PrintTime();
        }

        static void ResetGameClock()
        {
            IncrementTime();
            UpdateTimeEvent();
        }

        public static MacabreDateTime GetTime()
        {
            return time;
        }
    }
}