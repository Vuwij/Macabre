using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Environment.Time
{

    /// <summary>
    /// The GameClock deals with the time of day of the clock, it does one duck every single second, which then updates the game.
    /// </summary>
    public class GameClock
    {
        public static GameClock main;

        /// <summary>
        /// The clock rate
        /// At 1 : The clock rate is the same as real world
        /// At 60: It is 60 times faster thant the real world
        /// </summary>
        public const int ClockRate = 60;

        public static MacabreDateTime time;

        // We need a list of events that the gameclock invokes
        public delegate void UpdateTime();
        public delegate void UpdateSecond();
        public delegate void UpdateMinute();
        public delegate void UpdateHour();
        public delegate void UpdateDay();
        public delegate void UpdateWeek();

        public event UpdateTime UpdateTimeEvent;
        public event UpdateSecond UpdateSecondEvent;
        public event UpdateMinute UpdateMinuteEvent;
        public event UpdateHour UpdateHourEvent;
        public event UpdateDay UpdateDayEvent;
        public event UpdateWeek UpdateWeekEvent;

        public GameClock()
        {
            main = this;
            time = new MacabreDateTime(true);
        }

        public GameClock(MacabreDateTime time_)
        {
            main = this;
            time = time_;
        }

        void IncrementTime()
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

        void ResetGameClock()
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