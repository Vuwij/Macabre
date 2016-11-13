using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

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
        public static int ClockRate
        {
            get { return GameSettings.clockRate; }
        }

        public static Timer timer = new Timer(1000);

        public static Time time
        {
            get {
                if (MacabreWorld.current == null) return null;
                return MacabreWorld.current.gameTime;
            }
        }
        
        public static void Setup()
        {
            timer.Elapsed += TickTimeIfTimeExists;
            timer.Interval = 1000;
            Start();
        }

        public static void Start()
        {
            timer.Start();
        }

        public static void Stop()
        {
            if (timer != null) timer.Stop();
        }

        public static void TickTimeIfTimeExists(object sender, ElapsedEventArgs e)
        {
            if (time != null) time.Tick();
        }

        public static void ResetTime()
        {
            time.totalSeconds = 0;
        }
    }
}