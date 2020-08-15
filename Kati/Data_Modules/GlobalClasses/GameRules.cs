using Kati.Module_Hub;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Data_Modules.GlobalClasses {

    /// <summary>
    /// parser class that removes all dialogues that req game
    /// rules and are not met.
    /// </summary>
    public class GameRules {

        //Default rule branches from game
        public const string GAME = "game";
        public const string WEATHER = "weather";
        public const string SECTOR = "sector";
        public const string TIME_OF_DAY = "time";
        public const string DAY_OF_WEEK = "day";
        public const string SEASON = "season";
        public const string PUBLIC_EVENT = "publicEvent";
        public const string TRIGGER_EVENT = "trigger";
        private Controller ctrl;
        private Dictionary<string, List<string>> keys;

        public GameRules(Controller ctrl) {
            this.Ctrl = ctrl;
            keys = new Dictionary<string, List<string>>();
        }

        public Controller Ctrl { get => ctrl; set => ctrl = value; }
        public Dictionary<string, List<string>> Keys { get => keys; set => keys = value; }

        public void SetKeys(string key, List<string> value) {
            Keys[key] = value;
        }

        /**********************Remove GameData Non aplicable Content**************************/

        //format:: {"dialogue":{"req" : [],"leads to" : []}}
        public Dictionary<string, Dictionary<string, List<string>>> RemoveGameRequirments
                                (Dictionary<string, Dictionary<string, List<string>>> data) {
            List<string> keysToDelete = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> item in data) {
                foreach (string req in data[item.Key]["req"]) {
                    if (RemoveElement(req)) {
                        keysToDelete.Add(item.Key);
                    }
                }
            }
            return data;
        }

        public bool RemoveElement(string req) {
            string[] arr = req.Split(".");
            if (arr.Length == 0 || !arr[0].Equals(GAME)) {
                return false;
            } else {
                string[] temp = new string[arr.Length-1];
                for (int i = 1; i < arr.Length-1; i++) {//remove game Keyword
                    temp[i - 1] = arr[i];
                }
                return RuleDirectory(temp);   
            }
        }

        public bool RuleDirectory(string[] arr) {
            string key = arr[0];
            string[] temp = new string[arr.Length - 1];
            for (int i = 1; i <= arr.Length - 1; i++) {//remove game Keyword
                temp[i - 1] = arr[i];
            }
            switch (key) {
                case WEATHER: { return CheckWeather(temp); }
                case SECTOR: { return CheckSector(temp); }
                case TIME_OF_DAY: { return CheckTimeOfDay(temp); }
                case DAY_OF_WEEK: { return CheckDayOfWeek(temp); }
                case SEASON: { return CheckSeason(temp); }
                case PUBLIC_EVENT: { return CheckPublicEvent(temp); }
                case TRIGGER_EVENT: { return CheckTriggerEvent(temp); }
                default: { return false; }
            }
        }
        //no rules made yet
        protected bool CheckTriggerEvent(string[] temp) {
            return true;
        }

        protected bool CheckPublicEvent(string[] temp) {
            if (temp[0] == null)
                return true;
            if (temp[0].Equals("next")) {
                bool isNear = false;
                foreach (KeyValuePair<string, int> item in Ctrl.Game.EventCalendar[Ctrl.Game.Season]) {
                    isNear = isNear || Ctrl.Game.EventIsNear(item.Value - 6, Ctrl.Game.Season, item.Key);
                }
                return isNear;
            }
            return Ctrl.Game.EventIsNear
                (Ctrl.Game.EventCalendar[Ctrl.Game.Season][temp[0]] - 6, Ctrl.Game.Season, temp[0]);
        }

        protected bool CheckSeason(string[] temp) {
            if (temp[0] == null)
                return true;
            return !(temp[0].Equals(Ctrl.Game.Season));
        }

        protected bool CheckDayOfWeek(string[] temp) {
            if (temp[0] == null)
                return true;
            int day = 0;
            switch (temp[0]) {
                case "mon": { day = 1; }break;
                case "tues": { day = 2; }break;
                case "weds": { day = 3; }break;
                case "thurs": { day = 4; }break;
                case "fri": { day = 5; }break;
                case "sat": { day = 6; }break;
                case "sun": { day = 7; }break;
            }
            return !(day==(Ctrl.Game.DayOfWeek));
        }

        protected bool CheckTimeOfDay(string[] temp) {
            if (temp[0] == null)
                return true;
            return !(temp[0].Equals(Ctrl.Game.TimeOfDay));
        }

        protected bool CheckSector(string[] temp) {
            if (temp[0] == null)
                return true;
            return !(temp[0].Equals(Ctrl.Game.Sector.ToString()));
        }

        protected bool CheckWeather(string[] temp) {
            //if(should I check to see if it exists in rule set)
            if (temp[0] == null)
                return true;
            return !(temp[0].Equals(Ctrl.Game.Weather));
        }


        /******************Weigh Game Rule for Global dialogue Probability**********************/

        //will most definately be in it's own class
        public Dictionary<string, double> AssignRuleWeight(Dictionary<string, double> data) {
            return data;
        }
    }
}
