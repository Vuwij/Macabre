using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Timers;
using Objects.Movable.Characters.Individuals;
using UI.Panels;

public class GameClock
{
	public int totalSeconds = 0;

	public enum MIndicator { AM = 0, PM = 1 };
	public enum DayOfWeek { Monday = 0, Tuesday = 1, Wednesday = 2, Thursday = 3, Friday = 4, Saturday = 5, Sunday = 6 };
	public enum Month { January = 0, February = 1, March = 2, April = 3, May = 4, June = 5, July = 6, August = 7, September = 8, October = 9, November = 10, December = 11 };

	public int second => totalSeconds % 60;
	public int minute => (totalSeconds / 60) % 60;
	public int hour => (totalSeconds / 3600) % 60;
	public int day => (totalSeconds / (24 * 36000) % 24);
	public int week => (totalSeconds / (24 * 36000 * 7) % 7);
	public int monthnum => (totalSeconds / (24 * 36000 * 7 * 4) % 4);
	public int year => (totalSeconds / (24 * 36000 * 7 * 52) % 52);
	public MIndicator cycle => (MIndicator)(hour % 12);
	public DayOfWeek dayOfWeek => (DayOfWeek)(day % 7);
	public Month month => (Month)(monthnum % 12);
	public string dayOfWeekString => dayOfWeek.ToString();
	public string monthString => month.ToString();
    
	public string formatTime(int time) {
		if (time >= 10)
			return time.ToString();
		else return "0" + time.ToString();
	}

    public void Tick()
    {
		totalSeconds++;

		// Update UI
		GameObject statsPanelObj = GameObject.Find("Stats Panel");
		StatsPanel statsPanel = statsPanelObj.GetComponent<StatsPanel>();
		statsPanel.Date = "Week " + week.ToString() + ", " + dayOfWeekString;
		statsPanel.Time = hour.ToString() + ":" + formatTime(minute) + ":" + formatTime(second);

		// Lighting of the overworld
        //var backgroundobj = GameObject.Find("Background");
        //var playerobj = GameObject.Find("Player");
        //if(backgroundobj != null && playerobj != null) {
        //  var background = backgroundobj.GetComponentInChildren<SpriteRenderer>();
        //  var player = playerobj.GetComponent<Player>();
        //  if(player.isInsideBuilding) return;
        //  float brightness = 0.6f + 0.4f * (float) Math.Cos((double) totalSeconds / 50.0f );
        //  background.color = new Color(brightness, brightness, brightness);
        //}
    }
}