using System;
using UnityEngine;

namespace Environment.Time
{

    public enum MIndicator { AM, PM };
    public enum DayOfWeek { Monday, Tuesday, Wednesday, Thrusday, Friday, Saturday, Sunday };

    /// <summary>
    /// The MacabreDate Time contains methods that is similar the the UNIX clock
    /// </summary>
    public struct MacabreDateTime
    {
        // The current gametime
        public int second;
        public int minute;
        public int hour;

        public int day;
        public MIndicator cycle;

        public int week;
        public DayOfWeek dayOfWeek;

        public void AddSecond(int second_)
        {
            second += second_;
            if (second >= 60)
            {
                second -= 60;
                AddMinute(1);
            }
        }

        public void AddMinute(int minute_)
        {
            minute += minute_;
            if (minute >= 60)
            {
                minute -= 60;
                AddHour(1);
            }
        }

        public void AddHour(int Ahour)
        {
            hour += Ahour;
            if (hour >= 12)
            {
                hour -= 12;
                if (cycle == MIndicator.PM)
                {
                    cycle = MIndicator.AM;
                    AddDay(1);

                }
                if (cycle == MIndicator.AM)
                {
                    cycle = MIndicator.PM;
                }
            }
            if (hour >= 12)
            {
                hour -= 12;
                if (cycle == MIndicator.PM)
                {
                    cycle = MIndicator.AM;
                    AddDay(1);
                }
                if (cycle == MIndicator.AM)
                {
                    cycle = MIndicator.PM;
                }
            }

        }

        public void AddDaySingle()
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday: dayOfWeek = DayOfWeek.Tuesday; break;
                case DayOfWeek.Tuesday: dayOfWeek = DayOfWeek.Wednesday; break;
                case DayOfWeek.Wednesday: dayOfWeek = DayOfWeek.Thrusday; break;
                case DayOfWeek.Thrusday: dayOfWeek = DayOfWeek.Friday; break;
                case DayOfWeek.Friday: dayOfWeek = DayOfWeek.Saturday; break;
                case DayOfWeek.Saturday: dayOfWeek = DayOfWeek.Sunday; break;
                case DayOfWeek.Sunday:
                    dayOfWeek = DayOfWeek.Monday;
                    week += 1;
                    break;
            }
        }
        public void AddDay(int day)
        {
            for (int i = 0; i < day; i++) AddDaySingle();
        }

        public void AddWeek(int week_)
        {
            week += week_;
        }

        public MacabreDateTime AddTime(ref MacabreDateTime t)
        {
            AddSecond(t.second);
            AddMinute(t.minute);
            AddHour(t.hour);
            AddDay(t.day);
            AddWeek(t.week);

            return this;
        }

        public string GetTimeString()
        {
            return "Current Time Week " + week + ", " + dayOfWeek + " " + day + ", Hour " + hour + ":" + minute + ":" + second + " " + cycle;
        }

        public void PrintTime()
        {
            Debug.Log(GetTimeString());
        }

        public MacabreDateTime(bool newDateTime)
        {
            second = 0;
            minute = 0;
            hour = 0;
            day = 0;
            week = 0;
            cycle = MIndicator.AM;
            dayOfWeek = DayOfWeek.Monday;
        }

        public MacabreDateTime(ref MacabreDateTime m)
        {
            second = m.second;
            minute = m.minute;
            hour = m.hour;
            day = m.day;
            week = m.week;
            cycle = m.cycle;
            dayOfWeek = m.dayOfWeek;
        }
    }
}