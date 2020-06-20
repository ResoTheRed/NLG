using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Module_Hub{

    /// <summary>
    /// Contains all of the game meta data needed for dialogue
    /// </summary>
    public class GameData{
        //whats the weather like outside (nice_day)
        private string weather;
        //what sector of the game is the player in? It should be a number if they are outside
        // or a location name if inside. ("sector 1" outside, "The Blonde Mouse Coffee House" inside)
        // these need to be predifined
        private string sector; 
        // time of day: morining, afternoon, evening, night
        private string timeOfDay;
        // 1-28 days in month --> all months have 28 days
        private int dayOfMonth;
        // 1-4 may not keep this, can be found using arithmetic
        private int week;
        // spring, summer, fall, winter
        private string season;
        //public event near.  event dates are set in stone and can be found
        //using the dayOfMonth and event calendar
        private string publicEvent;
        //format: season : { event : day }
        private Dictionary<string, Dictionary<string, int>> eventCalendar;

        public string Weather { get => weather; set => weather = value; }
        public string Sector { get => sector; set => sector = value; }
        public string TimeOfDay { get => timeOfDay; set => timeOfDay = value; }
        public int DayOfMonth { get => dayOfMonth; set => dayOfMonth = value; }
        public int Week { get => week; set => week = value; }
        public string Season { get => season; set => season = value; }
        public string PublicEvent { get => publicEvent; set => publicEvent = value; }
        public Dictionary<string, Dictionary<string, int>> EventCalendar { get => eventCalendar; set => eventCalendar = value; }
    }

    /// <summary>
    /// Contains all of the character data from the game
    /// </summary>
    public class CharacterData {
        private string initiatorsName;
        private string respondersName;
        private string initialorsGender;
        private string respondersGender;
        /*combined thone/feeling/emotion of the conversation as a percent*/
        private Dictionary<string, double> interactionTone;
        /*init/respond feelings toward or outlook on th econversation 8 attributes with 8 numbers 0-1000*/
        private Dictionary<string, double> initiatorsTone;
        private Dictionary<string, double> respondersTone;
        /*Collections of character's boolean attributes-> format: "lucky" : "characterTrait" */
        private Dictionary<string, string> initiatorAttributeList;
        private Dictionary<string, string> responderAttributeList;
        /*collection of every social and personal scalar trait*/
        private Dictionary<string, int> initiatorScalarList;
        private Dictionary<string, int> responderScalarList;

        public string InitiatorsName { get => initiatorsName; set => initiatorsName = value; }
        public string RespondersName { get => respondersName; set => respondersName = value; }
        public string InitialorsGender { get => initialorsGender; set => initialorsGender = value; }
        public string RespondersGender { get => respondersGender; set => respondersGender = value; }
        public Dictionary<string, double> InteractionTone { get => interactionTone; set => interactionTone = value; }
        public Dictionary<string, double> InitiatorsTone { get => initiatorsTone; set => initiatorsTone = value; }
        public Dictionary<string, double> RespondersTone { get => respondersTone; set => respondersTone = value; }
        public Dictionary<string, string> InitiatorAttributeList { get => initiatorAttributeList; set => initiatorAttributeList = value; }
        public Dictionary<string, string> ResponderAttributeList { get => responderAttributeList; set => responderAttributeList = value; }
        public Dictionary<string, int> InitiatorScalarList { get => initiatorScalarList; set => initiatorScalarList = value; }
        public Dictionary<string, int> ResponderScalarList { get => responderScalarList; set => responderScalarList = value; }
    }


}
