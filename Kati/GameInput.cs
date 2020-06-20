using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kati
{
	/// <summary>
	/// Test Class that creates pseudo data --> to be removed
	/// </summary>

	class GameObject {

		public readonly string weather;
		public readonly string sector;
		public readonly bool isOutside;
		public readonly List<string> buildings;
		public readonly string timeOfDay;
		public readonly int dayOfMonth;
		public readonly int week;
		public readonly string season;
		public readonly string publicEvent;

		public GameObject(string w, string s, bool o, List<string> b, string t, int d, int week, string season, string p) {
			this.weather = w; this.sector = s; this.isOutside = o;
			this.buildings = b; this.timeOfDay = t; this.dayOfMonth = d;
			this.week = week; this.season = season; this.publicEvent = p;
		}

		override
		public string ToString() {
			string str = "Weather: "+weather+"\nSecotr: "+sector+"\nOutside?: "+isOutside;
			str += "\nBuildings: ";
			foreach (string buliding in buildings) {
				str += buildings + " ";
			}
			str += "\nTime of day: " + timeOfDay + "\nDay of Month: "+ dayOfMonth+ "\nWeek of Month: " + week + "\nSeason: " + season;
			str += "\nCurrent Event: " + publicEvent+"\n";
			return str;
		}
	}

    /// <summary>
    /// Test class Game input holds generated values that the game 
    /// would feed dialogue system.  This class is for testing 
    /// </summary>
    class GameInput{
		/*
         1. Weather Keywords (weather.<keyword>): hot, cold, humid, nice_day, rain, windy
	
		//elements at each location (descriptions and land marks)
		2. Location Keywords (location.outside.sector1):
			outside: sector_1 to sector12 and building_patios, building roof_tops
				nps (location.outside.sector_1.christina): list any npcs in the location
			inside: building_name.room (the_blonde_mouse.public_area)
				nps (location.inside.the_blonde_mouse.public_area.christina): list any npcs in the location

		3. Time (time.week.first)
			hour: 12:00 AM to 12:00pm --> time.hour.3:00PM
			day: 
				(time.day.morning): morning, evening, afternoon, sunrise, sunset, noon, midnight
				(time.day.1): 1 to 28 (numbered day of month/season)
			week (time.week.2): 1 to 4 (day number / 7 == week)
			season (time.season.spring): winter, spring, summer, fall
				event (time.event) <event_name>.day.hour (time.event.halloween.28.8:00PM):
					#current_event#: next event scheduled
						near: event is 4 to 0 days away (boolean) 
					spring (time.season.spring.event.art_fest): art_fest, blueberry_fest
					summer: writers_block, carnival
					fall: music_fest, halloween
					winter: bazaar_market, christmas
						day (starting day): time.season.spring.event.art_fest.day.<1 to 28>
						hour (starting): time.season.spring.event.art_fest.day.12.hour.10:00AM

		4. character data: (npc/player)
			name:
			stat: (love, friend, professional, respect, admire, annoy, hate, rival) (0-1000)
			personal: (greedy, kind, boastful)
				has.#variable_personal_attribute_between_pound_signs#
				has.direct_personal_attribute --> npc.has.argumentitive

             */
		private List<string> weather = new List<string>();
		private bool isOutside;
		private List<string> sector = new List<string>();
		private Dictionary<string, List<string>> sectorBuildings = new Dictionary<string, List<string>>();
		private string timeOfDay;
		private int dayOfMonth;
		private int week;
		private string season;
		private string publicEvent;
		private int weatherIndex;
		private string sectorIndex;

		public GameInput() {
			weatherIndex = 0;
			isOutside = true;
			sectorIndex = "sector1";
			setupGameData();
		}

		public GameObject CreateGameObject() {
			GameObject obj = new GameObject(weather[weatherIndex], sectorIndex, isOutside, sectorBuildings[sectorIndex], timeOfDay,
											dayOfMonth, week, season, publicEvent);
			return obj;
		}

		public void setWeather(int index) {
			this.weatherIndex = index;
		}

		public void setSector(string sector) {
			sectorIndex = sector;
		}

		public void setIsOutside(bool outs) {
			isOutside = outs;
		}

		public void setupGameData() {
			setupWeather();
			setupLocation(true);
			setupTime();
			setPublicEvent();
		}

		private void setupWeather() {
			weather.Add("nice_day");
			weather.Add("hot");
			weather.Add("cold");
			weather.Add("humid");
			weather.Add("windy");
			weather.Add("rain");
		}

		private void setupLocation(bool outside) {
			isOutside = outside;
			for (int i = 1; i <= 9; i++) {
				sector.Add("sector" + i);
				sectorBuildings["sector" + i] = new List<string>();
			}
			sectorBuildings["sector1"].Add("old_woods_shack");
			sectorBuildings["sector2"].Add("jims_house");
			sectorBuildings["sector2"].Add("marys_house");
			sectorBuildings["sector3"].Add("grocery_store");
			sectorBuildings["sector3"].Add("coffee_shop");
			sectorBuildings["sector4"].Add("amys_house");
			sectorBuildings["sector5"].Add("park");
			sectorBuildings["sector6"].Add("dans_pub");
			sectorBuildings["sector7"].Add("riverside_resturant");
			sectorBuildings["sector8"].Add("glideroys_mansion");
			sectorBuildings["sector9"].Add("fishing_docks");

		}

		private void setupTime() {
			timeOfDay = "morning";
			dayOfMonth = 1;
			week = 1;
			season = "spring";
		}

		private void incrementTime() {
			switch (timeOfDay) {
				case "morning": timeOfDay = "afternoon"; break;
				case "afternoon": timeOfDay = "evening"; break;
				case "evening": {
						timeOfDay = "morning";
						dayOfMonth++;
						setPublicEvent();
						if (dayOfMonth > 28) {
							dayOfMonth = 1;
							week++;
							if (week > 4) {
								week = 1;
								switch (season) {
									case "spring": season = "summer"; break;
									case "summer": season = "fall"; break;
									case "fall": season = "winter"; break;
									case "winter": season = "spring"; break;
								}
							}
						}
					}
					break;
			}
		}

		private void setPublicEvent() {
			if (dayOfMonth > 4 && dayOfMonth < 12) {
				if (season.Equals("spring"))
					publicEvent = "art_fest";
				if (season.Equals("summer"))
					publicEvent = "writers_block";
				if (season.Equals("fall"))
					publicEvent = "music_fest";
				if (season.Equals("winter"))
					publicEvent = "bizaar_market";
			} else if (dayOfMonth > 18 && dayOfMonth < 26) {
				if (season.Equals("spring"))
					publicEvent = "bluberry_fest";
				if (season.Equals("summer"))
					publicEvent = "carnival";
				if (season.Equals("fall"))
					publicEvent = "halloween";
				if (season.Equals("winter"))
					publicEvent = "yuletide";
			} else {
				publicEvent = "none";
			}

		}

	}
}
