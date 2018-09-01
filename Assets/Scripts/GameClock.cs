using System;
using UnityEngine;
using System.Runtime.Serialization;
using System.Timers;
using Objects.Movable.Characters.Individuals;
using UI.Panels;

public class GameClock
{
	public int totalSeconds = 0;
	public const int timeSpeed = 600;

	public enum MIndicator { AM = 0, PM = 1 };
	public enum DayOfWeek { Monday = 0, Tuesday = 1, Wednesday = 2, Thursday = 3, Friday = 4, Saturday = 5, Sunday = 6 };
	public enum Month { January = 0, February = 1, March = 2, April = 3, May = 4, June = 5, July = 6, August = 7, September = 8, October = 9, November = 10, December = 11 };

	public int second => totalSeconds % 60;
	public int minute => (totalSeconds / 60) % 60;
	public int hour => (totalSeconds / 60 / 60) % 24;
	public int day => (totalSeconds / 60 / 60 / 24);
	public int week => day / 7;
	public int monthnum => day / 30;
	public int year => day / 365;
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

	Light environmentLight;
	Light characterLight;

	float maxEnvironmentLight = 1.0f;
	float maxCharacterLight = 25.0f;

    public void Tick()
    {
		totalSeconds = totalSeconds + timeSpeed;

		// Update UI
		GameObject statsPanelObj = GameObject.Find("Stats Panel");
		StatsPanel statsPanel = statsPanelObj.GetComponent<StatsPanel>();
		statsPanel.Date = "Week " + week.ToString() + ", " + dayOfWeekString;
		statsPanel.Time = hour.ToString() + ":" + formatTime(minute) + ":" + formatTime(second);

        // Update lighting
		if (environmentLight == null) {
			GameObject cameraObj = GameObject.Find("Main Camera");
			environmentLight = cameraObj.GetComponent<Light>();
			Debug.Assert(environmentLight != null);
		}

		if (characterLight == null) {
            GameObject characterObj = GameObject.Find("Player");
			characterLight = characterObj.GetComponentInChildren<Light>();
			Debug.Assert(characterLight != null);
        }

		float lightPhase = (float)hour / 24 * 2 * Mathf.PI;
		float worldBrightness = Mathf.Cos(lightPhase + Mathf.PI) * maxEnvironmentLight / 2 + maxEnvironmentLight / 2;
		float lampBrightness = Mathf.Cos(lightPhase + 2 * Mathf.PI) * maxCharacterLight / 2 + maxCharacterLight / 2;

		environmentLight.intensity = worldBrightness;
		characterLight.intensity = lampBrightness;

    }
}