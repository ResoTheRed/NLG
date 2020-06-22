﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Kati.Module_Hub;
using Newtonsoft.Json;

namespace Kati.Data_Models{

    /*
       Conversation tones based on stats: both speaker and responder
       Type          |   required high       |      optional/mid      |      required lower
       ------------------------------------------------------------------------------------------
       -strangers    |    -none              |       -none            |       -all
       -neutral      |    -*                 |       -*               |       -*
       -friends      |    -friendship        |       -prof/respect    |       -all others
       -Lovers       |    -Romance           |       -all positive    |       -all negative       
       -Professional |    -prof/respect      |       -friends/rivalry |       -disgust/hatred
       -crush        |    -admiration        |       -all pos         |       -all neg
       -annoying     |    -disgust           |       -all neg         |       -Rom,Admiration,Friend
       -enemy        |    -hatred            |       -all neg         |       -all pos
       -rival        |    -rivalry           |       -all             |       -none
       --------------------------------------------------------------------------------------------
       -Neutral happens when none of the others are true.
       -other tones may use the neutral category
       --------------------------------------------------------------------------------------------
    */
    class SmallTalk_Module
    {
        //can talk to itself
        //topics questions or statements (invoke a response or not)
        /**question, statement, response
         * Weather: (relies on game input (what is the weather?))
         * Current Events: (relies on game input (what is currently going on?))
         * Social Events (relies on game input (where is the character?))
         * Observation about person: (relies on game input (who are they talking to and what do they have with them))
         * greetings and How are you questions 
         * **/

        /** SmallTalk Requirement Keys 
         *  weather.[nice_day, rain, hot, windy, cold, humid]
         *  
         * **/

        /** SmallTalk Leads To Keys format
         *  Module_name.section_in_module --> [small_talk.weather_response]
         *  
         * **/
    
        //Tone Type: conversation phrase : [tags about responses or leads to] 
        public Dictionary<string, Dictionary<string, List<string>>> weatherQuestion;
        public Dictionary<string, Dictionary<string, List<string>>> weatherStatement;
        public Dictionary<string, Dictionary<string, List<string>>> weatherResponse;
        public Dictionary<string, Dictionary<string, List<string>>> eventQuestion;
        public Dictionary<string, Dictionary<string, List<string>>> eventStatement;
        public Dictionary<string, Dictionary<string, List<string>>> eventResponse;
        public Dictionary<string, Dictionary<string, List<string>>> greetingStatement;
        public Dictionary<string, Dictionary<string, List<string>>> greetingQuestion;
        public Dictionary<string, Dictionary<string, List<string>>> greetingResponse;

        private string pathToJson;

        public SmallTalk_Module(string path) {
            pathToJson = path;
            SmallTalk_Loader.LoadFromFile(this);
        }

        public string PathToJson { get => pathToJson; set => pathToJson = value; }
    }

    /// <summary>
    /// Controls who is talking and decides what is said and 
    /// when the smallTalk chain is over.
    /// </summary>

    class SmallTalk_Controller {

        // I think I will need to keep track of the entire dialogue sequence here
        // It will be reset after the dialogue event has ended
        // character will repeat the last line of dialogue over and over again after
        // hub will take care of remembering this.

        /* possible conversation chains:
         *      *greeting
         *      greeting && [event || weather]
         *      greeting && [event && weather] || [weather && event]
         *      *[event || weather]
         *      [event && weather] || [weather && event]
         */

        /* Order of operations
         * Initiator: SelectStartingTopic() --> statement or question from possible categories
         * Responder: respond to question or skip turn
         * Initiator: SelectNextTopic() or end smalltalk chain
         * Responder: respond to question or skip turn
         */

        //need to establish turns

        // set up the initiator default to NPC
        // the roles will be reversed.

        //constants
        private const string INITIATOR = "initiator";
        private const string RESPONDER = "reponder";
        private const string EVENT = "event";
        private const string WEATHER = "weather";
        private const string GREETING = "greeting";
        private const string STATEMENT = "statement";
        private const string QUESTION = "question";
        private const string RESPONSE = "response";
        private const string SKIPPED = "skipped";


        //variables reference the current line of dialogue
        private string speaking = INITIATOR;
        private string dialogueState = STATEMENT;
        private string topic = GREETING;
        private string leadsTo;

        //contains the dialogue after parsing
        private string dialogue;
        //contains the full conversation between initiator and responder
        //format {"initiator": ["pc_name","dialogue1",...], "reponder":["pc_name","dialogue 1",...] }
        private Dictionary<string, List<string>> conversation;
        private Dictionary<string, bool> conversationTopicsDiscussed;
        private SmallTalk_Module module;
        private SmallTalk_Parser parser;
        //how is character Data stored?  reference to GameCharacter
        private GameData gameData;
        private CharacterData characterData;
        //need to withdraw allSmallTalk related attributes for each player
        private Random dice = new Random();


        public SmallTalk_Controller(SmallTalk_Module module) {
            this.module = module;
            this.parser = new SmallTalk_Parser(module);
            conversation[INITIATOR] = new List<string>();
            conversation[RESPONDER] = new List<string>();
            ConversationTopicsDiscussed[EVENT] = false;
            ConversationTopicsDiscussed[WEATHER] = false;
            ConversationTopicsDiscussed[GREETING] = false;
            leadsTo = "";
        }

        //contains data from the game for requirements
        public GameData _GameData { get => gameData; set => gameData = value; }
        public CharacterData _CharacterData { get => characterData; set => characterData = value; }
        public string Dialogue { get => dialogue; set => dialogue = value; }
        public Dictionary<string, List<string>> Conversation { get => conversation; }
        public Dictionary<string, bool> ConversationTopicsDiscussed { get => conversationTopicsDiscussed; set => conversationTopicsDiscussed = value; }

        public void SetupConversation(GameData gameData, CharacterData characterData) {
            _GameData = gameData;
            _CharacterData = characterData;
            conversation[INITIATOR].Add(_CharacterData.InitiatorsName);//NPC
            conversation[RESPONDER].Add(_CharacterData.RespondersName);//Player
        }

        public void Converse() {
            SelectStartingTopic();
        }

        //which module is being targeted? Random vs. intentional
        //this method is utilized by the initiator
        public void SelectStartingTopic() {
            int option = (int) dice.NextDouble()*10+1;
            //50% Greeting
            //20% Weather
            //30% Event
            if (option <= 5 ) { //maybe add or if stranger to
                PullGreeting();
            } else if (option <= 8) {
                PullEvent();
            } else {
                PullWeather();
            }
        }

        //build off of the previous dialogue
        public void SelectNextTopic() { 
            //cannot contain greeting, Greetings are reserved for starting topics
            //cannot be the same topic as one already talked about
            //may not always have a second smallTalk topic
            //strangers are less likely to have multiple topics
            // generally smalltalk will not have a next topic 
        }

        //which dialogue string will be used
        //compare requirements with qualifications of character
        //return reference from dictionary in SmallTalk_Module

        //parse through greetings
        private void PullGreeting() {
            ConversationTopicsDiscussed[GREETING] = true;
            topic = GREETING;
            CheckSpeaker();
        }

        private void PullEvent() {
            ConversationTopicsDiscussed[EVENT] = true;
            topic = EVENT;
            CheckSpeaker();
        }

        private void PullWeather() {
            ConversationTopicsDiscussed[WEATHER] = true;
            topic = WEATHER;
            CheckSpeaker();
        }

        private void CheckSpeaker() {
            if (speaking.Equals(INITIATOR)) {
                RunInitiatorsTurn();
            } else {
                RunRespondersTurn();
            }
        }

        //must be able to work for all topics
        private void RunInitiatorsTurn() {
            //the floor is opened to the npc to choose what they want to do
            string stateType = STATEMENT;
            if (!dialogueState.Equals(QUESTION)) {
                int prob = (int)dice.NextDouble() * 10 + 1;
                if (prob <= 7) {
                    dialogueState = STATEMENT;
                } else {
                    dialogueState = stateType = QUESTION;
                }
            } else {
                dialogueState = stateType = RESPONSE;
            }
            string dialogue = CallMethod(topic, stateType);
            SetSpeakersTurn(dialogue, INITIATOR, stateType);
        }

        //must be able to run all topics
        private void RunRespondersTurn() {
            if (dialogueState.Equals(QUESTION)) {
                //answer question here
                dialogueState = RESPONSE;
                string dialogue = CallMethod(topic, RESPONSE);
                SetSpeakersTurn(dialogue, RESPONDER, RESPONSE);
            } else {
                dialogueState = SKIPPED;
            }
        }

        /* Call after the initiator or the responder speaks, record
         * the dialogue and switch to the next speaker or end chain
         */
        private void SetSpeakersTurn(string dialogue, string speaker, string topic) {
            if (speaker.Equals(INITIATOR)) {
                Conversation[topic].Add(dialogue);
                this.dialogue = dialogue;
                this.speaking = RESPONDER;
            } else {
                Conversation[topic].Add(dialogue);
                this.dialogue = dialogue;
                this.speaking = INITIATOR;
            }

        }

        private string CallMethod(string topic, string stateType) {
            string dialogue = "";
            if (stateType.Equals(STATEMENT)) {
                dialogue = PullStatement(topic);
            } else if (stateType.Equals(QUESTION)) {
                dialogue = PullQuestion(topic);
            } else {
                dialogue = PullResponse(topic);
            }
            return dialogue;
        }

        private string PullStatement(string type) {
            parser.SetStage(speaking,dialogueState,type);
            return parser.GetDialogue();
        }
        private string PullQuestion(string type) {
            parser.SetStage(speaking, dialogueState, type);
            return parser.GetDialogue(); ;
        }
        private string PullResponse(string type) {
            parser.SetStage(speaking, dialogueState, type);
            return parser.GetDialogue(); ;
        }


    }

    /// <summary>
    /// Class functions to parse rules and find which dialogue topics are
    /// possible or relevant
    /// </summary>
    class SmallTalk_Parser {

        private SmallTalk_Module module;
        private string speaker;//initiator or responder
        private string stateType;//statement, question, or response
        private string topic; //weather, event, or greeting

        public SmallTalk_Parser(SmallTalk_Module module) {
            this.module = module;
        }

        public void SetStage(string speaking, string stateType, string topic) { 
            
        }

        public string GetDialogue() {
            return "";
        }

    }

    /// <summary>
    /// Class functions to pull raw data from the json file and load it into 
    /// SmallTalk_Module dictionaries
    /// </summary>

    class SmallTalk_Loader {

        /*Starts the loading sequence*/
        public static SmallTalk_Loader LoadFromFile(SmallTalk_Module module) {
            SmallTalk_Loader loader = new SmallTalk_Loader(module);
            loader.SetupDataQueries();
            return loader;
        }

        private SmallTalk_Module module;

        public SmallTalk_Loader(SmallTalk_Module module) {
            this.module = module;
        }

        /// <summary>
        /// converts a file into a string.  Requires path.
        /// </summary>
        /// <param name="filePaht">File to be read</param>
        /// <returns>files content as String</returns>
        private string ReadFile(string filePath) {
            using StreamReader reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        private void SetupDataQueries() {
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp;
            try {
                temp = ConvertJsonToDictionary();
            } catch (Exception e) {
                Console.WriteLine(e);
                return;
            }
            SetupDictionaries(temp);
        }

        private Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> ConvertJsonToDictionary() {
            string json = ReadFile(module.PathToJson);
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> data =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>(json);
            return data;
        }

        private void SetupDictionaries(Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp) {
            SetupWeather(temp);
            SetupCurrentEvent(temp);
            SetupGreetings(temp);

        }

        private void SetupGreetings(Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp) {
            try {
                module.greetingStatement = temp["Greeting_statement"];
                module.greetingQuestion = temp["Greeting_question"];
                module.greetingResponse = temp["Greeting_response"];
            } catch (Exception e) {
                Console.WriteLine("Faild to uplaod Greeting Conversations\n" + e);
            }
        }

        private void SetupCurrentEvent(Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp) {
            try {
                module.eventQuestion = temp["current_event_question"];
                module.eventResponse = temp["current_event_response"];
                module.eventStatement = temp["current_event_statement"];
            } catch (Exception e) {
                Console.WriteLine("Failed to upload Event Conversations\n" + e);
            }
        }

        private void SetupWeather(Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp) {
            try {
                module.weatherQuestion = temp["weather_question"];
                module.weatherResponse = temp["weather_response"];
                module.weatherStatement = temp["weather_statement"];
            } catch (Exception e) {
                Console.WriteLine("Failed to upload Weather Conversations\n" + e);
            }
        }

    }

}
