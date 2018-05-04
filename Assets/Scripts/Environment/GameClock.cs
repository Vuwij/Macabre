using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Timers;
using Objects.Movable.Characters.Individuals;

namespace Environment
{
    public class GameClock
    {
		public int totalSeconds {
			get {
				return (int) Time.time;
			}
		}
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

        public override string ToString()
        {
            return "Time: Week " + week + ", " + day + ", Hour " + hour.ToString("D2") + ":" + minute.ToString("D2") + ":" + second.ToString("D2") + " " + cycle;
        }

		public void PeriodicUpdate() {
			// Lighting of the overworld
			//var backgroundobj = GameObject.Find("Background");
			//var playerobj = GameObject.Find("Player");
			//if(backgroundobj != null && playerobj != null) {
			//	var background = backgroundobj.GetComponentInChildren<SpriteRenderer>();
			//	var player = playerobj.GetComponent<Player>();
			//	if(player.isInsideBuilding) return;
			//	float brightness = 0.6f + 0.4f * (float) Math.Cos((double) totalSeconds / 50.0f );
			//	background.color = new Color(brightness, brightness, brightness);
			//}
		}
    }
}