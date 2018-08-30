using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Timers;
using Objects.Movable.Characters.Individuals;
using UI.Panels;

public class GameClock
{
	public int totalSeconds = 0;

	public enum MIndicator { AM, PM };
	public enum DayOfWeek { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
    public enum Month { January, February, March, April, May, June, July, August, September, October, November, December };

	public int second => totalSeconds % 60;
	public int minute => (totalSeconds / 60) % 60;
	public int hour => (totalSeconds / 3600) % 60;
	public int day => (totalSeconds / (24 * 36000) % 24);
	public int week => (totalSeconds / (24 * 36000 * 7) % 7);
	public int month => (totalSeconds / (24 * 36000 * 7 * 4) % 4);
	public int year => (totalSeconds / (24 * 36000 * 7 * 52) % 52);
	public MIndicator cycle => (MIndicator)(hour % 12);
	public DayOfWeek dayOfWeek => (DayOfWeek)(day % 7);
    
    public void Tick()
    {
		totalSeconds++;

		// Update UI
		GameObject statsPanelObj = GameObject.Find("Stats Panel");
		StatsPanel statsPanel = statsPanelObj.GetComponent<StatsPanel>();
		statsPanel.Date = dayOfWeek.ToString() + " , month";

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