using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class SmallTalk_Module
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
        //"neutral" : {"phrase" : {"req": [], "leads to": [] } }
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> weatherQuestion;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> weatherStatement;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> weatherResponse;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> eventQuestion;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> eventStatement;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> eventResponse;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> greetingStatement;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> greetingQuestion;
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> greetingResponse;

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

    public class SmallTalk_Controller {

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
        public const string INITIATOR = "initiator";
        public const string RESPONDER = "responder";
        public const string EVENT = "event";
        public const string WEATHER = "weather";
        public const string GREETING = "greeting";
        public const string STATEMENT = "statement";
        public const string QUESTION = "question";
        public const string RESPONSE = "response";
        public const string SKIPPED = "skipped";

        //variables reference the current line of dialogue
        private string speaking = INITIATOR;
        private string dialogueState = STATEMENT;
        private string topic = GREETING;
        private string leadsTo;
        private bool endConversation = false;

        //variables reference selecting next topic
        private int improveProbabilityOfMoreDialogue = 3;

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
            InstatiateStructures();
            leadsTo = "";
            parser.Ctrl = this;
        }

        private void InstatiateStructures() {
            conversation = new Dictionary<string, List<string>>();
            conversation[INITIATOR] = new List<string>();
            conversation[RESPONDER] = new List<string>();
            conversationTopicsDiscussed = new Dictionary<string, bool>();
            ConversationTopicsDiscussed[EVENT] = false;
            ConversationTopicsDiscussed[WEATHER] = false;
            ConversationTopicsDiscussed[GREETING] = false;
        }

        //contains data from the game for requirements
        public GameData _GameData { get => gameData; set => gameData = value; }
        public CharacterData _CharacterData { get => characterData; set => characterData = value; }
        public string Dialogue { get => dialogue; set => dialogue = value; }
        public Dictionary<string, List<string>> Conversation { get => conversation; }
        public Dictionary<string, bool> ConversationTopicsDiscussed { get => conversationTopicsDiscussed; set => conversationTopicsDiscussed = value; }
        public string Speaking { get => speaking; set => speaking = value; }
        public string DialogueState { get => dialogueState; set => dialogueState = value; }
        public string Topic { get => topic; set => topic = value; }
        public bool EndConversation { get => endConversation; set => endConversation = value; }
        public Random Dice { get => dice; set => dice = value; }

        public void SetupConversation(GameData gameData, CharacterData characterData) {
            _GameData = gameData;
            _CharacterData = characterData;
            conversation[INITIATOR].Add(_CharacterData.InitiatorsName);//NPC
            conversation[RESPONDER].Add(_CharacterData.RespondersName);//Player
        }

        public void Converse() {
            RunFirstRound();
            while (!EndConversation) {
                RunNextRound();
            }
        }

        public void RunFirstRound() {
            EndConversation = false;
            SelectStartingTopic();
            RunRespondersTurn();
        }

        public void RunNextRound() {
            SelectNextTopic();
            RunRespondersTurn();
        }

        //which module is being targeted? Random vs. intentional
        //this method is utilized by the initiator
        public int SelectStartingTopic() {
            int option = (int) (Dice.NextDouble()*10+1);
            if (option <= 5 ) {
                PullGreeting();
            } else if (option <= 8) {
                PullEvent();
            } else {
                PullWeather();
            }
            return option;
        }

        //build off of the previous dialogue
        public int SelectNextTopic() {
            int probability = 0;
            //cannot contain greeting, Greetings are reserved for starting topics
            //cannot be the same topic as one already talked about
            //may not always have a second smallTalk topic
            //strangers are less likely to have multiple topics
            // generally smalltalk will not have a next topic 
            if (!conversationTopicsDiscussed[EVENT] || !conversationTopicsDiscussed[WEATHER]) {
                probability = (int)(Dice.NextDouble() * 10 + 1);
                probability += NextTopicGreetingRule();
                if (probability > 8) {//70%chance there will not be another topic unless greetin was other topic
                    if (conversationTopicsDiscussed[EVENT] && !conversationTopicsDiscussed[WEATHER]) {
                        PullWeather();
                    } else if (!conversationTopicsDiscussed[EVENT] && conversationTopicsDiscussed[WEATHER]) {
                        PullEvent();
                    }
                } else {
                    EndConversation = true;
                }
            }else{
                EndConversation = true;
            }
            return probability;
        }

        /* checks to see if the last topic talked about was Greeting
         * if so return an integer value that boosts the chances of a
         * next topic. Greetings can only be said on the first round*/
        public int NextTopicGreetingRule() {
            bool lastTopicWasGreeting = 
                conversationTopicsDiscussed[GREETING] &&
                !(conversationTopicsDiscussed[WEATHER] ||
                conversationTopicsDiscussed[EVENT]);
            return lastTopicWasGreeting ? improveProbabilityOfMoreDialogue : 0;
        }

        public bool NextTopicEventRule() {
            bool runEvent = NextTopicGreetingRule() > 0;//only greeting was run
            if (parser.EventAvaliable()) {
                if (!runEvent) {
                    if (conversationTopicsDiscussed[EVENT]) {
                        runEvent = false;
                    } else if (conversationTopicsDiscussed[WEATHER]) {
                        //further discourage Event if Weather has taken player
                        runEvent = ((int)(Dice.NextDouble() * 10 + 1)) > 5 ? true : false;
                    }
                }
            }
            return runEvent;
        }

        //which dialogue string will be used
        //compare requirements with qualifications of character
        //return reference from dictionary in SmallTalk_Module

        //parse through greetings
        private void PullGreeting() {
            ConversationTopicsDiscussed[GREETING] = true;
            Topic = GREETING;
            CheckWhosSpeakingAndRunThereTurn();
        }

        private void PullEvent() {
            ConversationTopicsDiscussed[EVENT] = true;
            Topic = EVENT;
            CheckWhosSpeakingAndRunThereTurn();
        }

        private void PullWeather() {
            ConversationTopicsDiscussed[WEATHER] = true;
            Topic = WEATHER;
            CheckWhosSpeakingAndRunThereTurn();
        }

        //run the 
        private string CheckWhosSpeakingAndRunThereTurn() {
            if (Speaking.Equals(INITIATOR)) {
                RunInitiatorsTurn();
                return INITIATOR;
            } else {
                RunRespondersTurn();
                return RESPONDER;
            }
        }

        //must be able to work for all topics
        private void RunInitiatorsTurn() {
            //the floor is opened to the npc to choose what they want to do
            string stateType = STATEMENT;
            if (!DialogueState.Equals(QUESTION)) {
                int prob = (int)(Dice.NextDouble() * 10 + 1);
                if (prob <= 7) {
                    DialogueState = STATEMENT;
                } else {
                    DialogueState = stateType = QUESTION;
                }
            } else {
                DialogueState = stateType = RESPONSE;
            }
            string dialogue = CallMethod(Topic, stateType);
            SetSpeakersTurn(dialogue, INITIATOR, stateType);
        }

        //must be able to run all topics
        private void RunRespondersTurn() {
            string dialogue = "";
            if (DialogueState.Equals(QUESTION)) {
                //answer question here
                DialogueState = RESPONSE;
                dialogue = CallMethod(Topic, RESPONSE);
                SetSpeakersTurn(dialogue, RESPONDER, RESPONSE);
            } else {
                DialogueState = SKIPPED;
                SetSpeakersTurn(dialogue, RESPONDER, SKIPPED);
            }
        }

        /* Call after the initiator or the responder speaks, record
         * the dialogue and switch to the next speaker or end chain
         */
        private void SetSpeakersTurn(string dialogue, string speaker, string topic) {
            if (speaker.Equals(INITIATOR)) {
                conversation[INITIATOR].Add(topic+": "+dialogue);
                this.dialogue = dialogue;
                this.Speaking = RESPONDER;
            } else {
                conversation[RESPONDER].Add(topic+": "+dialogue);
                this.dialogue = dialogue;
                this.Speaking = INITIATOR;
            }

        }

        public string CallMethod(string topic, string stateType) {
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
            parser.SetStage(Speaking,DialogueState,type);
            return parser.GetDialogue();
        }
        private string PullQuestion(string type) {
            parser.SetStage(Speaking, DialogueState, type);
            return parser.GetDialogue(); ;
        }
        private string PullResponse(string type) {
            parser.SetStage(Speaking, DialogueState, type);
            return parser.GetDialogue(); ;
        }


    }

    /// <summary>
    /// Class functions to parse rules and find which dialogue topics are
    /// possible or relevant
    /// </summary>
    public class SmallTalk_Parser {

        public const double low_threshold = 0.25;
        public const double mid_threshold = 0.5;
        public const double high_threshold = 0.75;

        private SmallTalk_Module module;
        private SmallTalk_Controller ctrl;
        //set to the target list to parse
        private Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> currentList;
        private string speaker;//initiator or responder
        private string stateType;//statement, question, or response
        private string topic; //weather, event, or greeting

        private Dictionary<string, List<string>> weatherKeys;
        private Dictionary<string, List<string>> eventKeys;
        private Dictionary<string, List<string>> greetingKeys;



        public string Speaker { get => speaker; set => speaker = value; }
        public string StateType { get => stateType; set => stateType = value; }
        public string Topic { get => topic; set => topic = value; }
        public SmallTalk_Controller Ctrl { get => ctrl; set => ctrl = value; }
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> CurrentList { get => currentList; set => currentList = value; }
        public Dictionary<string, List<string>> WeatherKeys { get => weatherKeys; set => weatherKeys = value; }
        public Dictionary<string, List<string>> EventKeys { get => eventKeys; set => eventKeys = value; }
        public Dictionary<string, List<string>> GreetingKeys { get => greetingKeys; set => greetingKeys = value; }

        public SmallTalk_Parser(SmallTalk_Module module) {
            this.module = module;
            SetupConversationKeys();
        }

        public void SetStage(string speaking, string stateType, string topic) {
            Speaker = speaking;
            this.StateType = stateType;
            this.Topic = topic;
        }

        //all methods that check requirements lead to this method
        //requires SetStage
        public string GetDialogue() {
            SetCurrentListTopic();
            string attributeBranch = SetRelationshipDialogueTone();
            string dialogue = GenerateDialogue(attributeBranch);
            dialogue = ReplaceRawDialogueWords(dialogue);
            return dialogue;
        }

        public string ReplaceRawDialogueWords(string dialogue) {
            string[] words = dialogue.Split(" ");
            for (int i = 0; i < words.Length; i++) {
                if (words[i].Contains("#current_event#")) {
                    words[i] = CaseCurrent_event(words[i]);
                } else if (words[i].Contains("#player.name#")) {
                    words[i] = CasePlayer_Name(words[i]);
                }   
            }
            return StringArrayToSentence(words);
        }

        public string CasePlayer_Name(string word) {
            string name = (speaker.Equals(SmallTalk_Controller.INITIATOR)) ?
                Ctrl._CharacterData.RespondersName : Ctrl._CharacterData.InitiatorsName;
            if (word[word.Length-1].Equals("."))
                name += ".";
            return name;
        }

        public string CaseCurrent_event(string word) {
            string[] _event = Ctrl._GameData.EventIsNear().Split("_");
            string newWord = UppercaseFirst(_event[0]);
            newWord += " "+ UppercaseFirst(_event[1]);
            if (word.Contains(".")) {
                newWord += ".";
            }
            return newWord;
        }

        public string StringArrayToSentence(string[] arr) {
            string sentence = "";
            for (int i = 0; i < arr.Length; i++) {
                if (i != arr.Length - 1)
                    sentence += arr[i] + " ";
                else
                    sentence += arr[i];
            }
            return sentence;
        }

        private string UppercaseFirst(string s) {
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        private string GenerateDialogue(string attributeBranch) {
            var dialogueList = TrimDialogueOptions(attributeBranch);
            string dialogue;
            if (topic.Equals(SmallTalk_Controller.WEATHER)) {
                dialogue = CycleWeatherKeyReqVersusDialogueReq(dialogueList);
            } else if (topic.Equals(SmallTalk_Controller.EVENT)) {
                dialogue = CycleEventKeysReqVersusDialogueReq(dialogueList);
            } else  {
                dialogue = CycleGreetingKeysReqVersusDialogueReq(dialogueList);
            }
            return dialogue;
        }


        //looks if an event is available
        public bool EventAvaliable() {
            //ctrl._GameData.EventIsNear();
            return true;
        }

        public void SetCurrentListTopic() {
            if (topic.Equals(SmallTalk_Controller.WEATHER)) {
                SetCurrentListToWeather();
            } else if (topic.Equals(SmallTalk_Controller.EVENT)) {
                SetCurrentListToEvent();
            } else if (topic.Equals(SmallTalk_Controller.GREETING)) {
                SetCurrentListToGreeting();
            }
        }

        private void SetCurrentListToGreeting() {
            if (stateType.Equals(SmallTalk_Controller.STATEMENT))
                CurrentList = module.greetingStatement;
            else if (stateType.Equals(SmallTalk_Controller.QUESTION))
                CurrentList = module.greetingQuestion;
            else
                CurrentList = module.greetingResponse;
        }
        private void SetCurrentListToEvent() {
            if (stateType.Equals(SmallTalk_Controller.STATEMENT))
                CurrentList = module.eventStatement;
            else if (stateType.Equals(SmallTalk_Controller.QUESTION))
                CurrentList = module.eventQuestion;
            else
                CurrentList = module.eventResponse;
        }
        private void SetCurrentListToWeather() {
            if (stateType.Equals(SmallTalk_Controller.STATEMENT))
                CurrentList = module.weatherStatement;
            else if (stateType.Equals(SmallTalk_Controller.QUESTION))
                CurrentList = module.weatherQuestion;
            else
                CurrentList = module.weatherResponse;
        }

        public void SetupConversationKeys() {
            SetupWeatherKeys();
            SetupEventKeys();
            SetupGreetingKeys();
        }

        private void SetupWeatherKeys() {
            WeatherKeys = new Dictionary<string, List<string>>();
            WeatherKeys["req"] = new List<string>() { "weather.nice_day", "weather.rain",
                "weather.hot", "weather.windy", "weather.humid"};
            WeatherKeys["leads to"] = new List<string>(){"small_talk.weather_response",
                "trigger.romantic_advance", "trigger.cynical","trigger.hateful","trigger.rude",
                "trigger.next_topic","trigger.end_conversation"};
        }

        private void SetupEventKeys() {
            EventKeys = new Dictionary<string, List<string>>();
            EventKeys["req"] = new List<string>() { "time.#current_event#", "npc#loves_current_event#",
                "npc.#hates_current_event#", "time.event.art_fest", "npc.loves_art_fest","time.event.blueberry_fest",
                "npc.loves_blueberry_fest","npc.hates_blueberry_fest","time.event.writers_block","npc.loves_writers_block",
                "npc.hates_writers_block", "time.event.music_fest", "npc.loves_music_fest","npc.hates_music_fest",
                "time.event.halloween","npc.loves_halloween","npc.hates_halloween","time.event.bazaar","npc.loves_bazaar",
                "npc.hates_bazaar","time.event.yuletide","npc.loves_yuletide", "npc.hates_yuletide"};
            EventKeys["leads to"] = new List<string>() {"PlayerOption.likeCurrentEvent",
                "PlayerOption.goingToCurrentEvent","#current_event#.like++","#current_event#.like+",
                "#current_event#.hates+","#current_event#.hates++"};
        }

        private void SetupGreetingKeys() {
            GreetingKeys = new Dictionary<string, List<string>>();
            GreetingKeys["req"] = new List<string>() { "time.day.morning", "time.day.afternoon",
                "time.day.evening", "time.day.morning", "player.female", "player.male"};
            GreetingKeys["leads to"] = new List<string>(){"greeting_question", "greeting_response",
                "positive++","positive+","negative+","negative++" };
        }
        //get the winning attribute branch from character ex Romance;
        public string SetRelationshipDialogueTone() {
            //Set the current dialogue topic to the approriate dialogue list
            SetCurrentListTopic();
            //Mellow Character attributes if other attributes are present romace vs disgust
            var tone = CancelAttributeTones();
            //remove character attributes that aren't avaiable in the dialogue listing
            var matchedTone = MatchCharacterAttributesToAvailableDialogueTones(tone);
            //categorize the attributes from high, mid, low
            var cat = CategorizeTone(tone);
            //pulls the top ranking attribute
            string winningAttribute = PickMostInfluentialAttribute(cat);
            return winningAttribute;
        }

        public Dictionary<string, double> MatchCharacterAttributesToAvailableDialogueTones
                                                            (Dictionary<string, double> tone) {
            Dictionary<string, double> updatedTone = new Dictionary<string, double>();
            foreach
                (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> item in currentList) {
                if (tone.ContainsKey(item.Key)) {
                    updatedTone[item.Key] = tone[item.Key];
                } else {
                    updatedTone[item.Key] = 0.0;
                }
            }
            return updatedTone;
        }

      
        //get relationship "neutral","romance","friend","hate","disgust"
        //get the major relationship tone attributes
        public Dictionary<string, Dictionary<string, double>> CategorizeTone
                                                (Dictionary<string, double> tone) {
            Dictionary<string, Dictionary<string, double>> categorized =
                new Dictionary<string, Dictionary<string, double>>();
            categorized["high"] = new Dictionary<string, double>();
            categorized["mid"] = new Dictionary<string, double>();
            categorized["low"] = new Dictionary<string, double>();
            categorized["none"] = new Dictionary<string, double>();
            
            foreach (KeyValuePair<string, double> obj in tone) {
                if (obj.Value >= high_threshold)
                    categorized["high"][obj.Key] = obj.Value;
                else if (obj.Value >= mid_threshold)
                    categorized["mid"][obj.Key] = obj.Value;
                else if (obj.Value >= low_threshold)
                    categorized["low"][obj.Key] = obj.Value;
                else
                    categorized["none"][obj.Key] = obj.Value;
            }
            return categorized;
        }

        //cancel out attributes that are in opposition 
        public Dictionary<string, double> CancelAttributeTones() {
            var tone = (speaker.Equals(SmallTalk_Controller.INITIATOR)) ?
                ctrl._CharacterData.InitiatorsTone : ctrl._CharacterData.RespondersTone;
            Dictionary<string, double> _tone = new Dictionary<string, double>();
            _tone["romance"] = (tone["romance"] - tone["disgust"] >= 0) ?
                tone["romance"] - tone["disgust"] : 0;
            _tone["disgust"] = (tone["disgust"] - tone["romance"] >= 0) ?
                tone["disgust"] - tone["romance"] : 0;
            _tone["friend"] = (tone["friend"] - tone["hate"] >= 0) ?
                tone["friend"] - tone["hate"] : 0;
            _tone["hate"] = (tone["hate"] - tone["friend"] >= 0) ?
                tone["hate"] - tone["friend"] : 0;
            _tone["professional"] = (tone["professional"] - tone["rivalry"] >= 0) ?
                tone["professional"] - tone["rivalry"] : 0;
            _tone["rivalry"] = (tone["rivalry"] - tone["professional"] >= 0) ?
                tone["rivalry"] - tone["professional"] : 0;
            return _tone;
        }

        //infuence order: romance, hate, disgust, friend, rivalry, professional
        public string PickMostInfluentialAttribute
            (Dictionary<string, Dictionary<string, double>> fullList) {
            string winningAttribute = "neutral";
            if (fullList.ContainsKey("high") && fullList["high"].Count > 0)
                winningAttribute = PullHighAttribute(fullList["high"]);
            else if (fullList.ContainsKey("mid") && fullList["mid"].Count > 0)
                winningAttribute = PullMiddleAttribute(fullList["mid"]);
            else if (fullList.ContainsKey("low") && fullList["low"].Count > 0)
                winningAttribute = PullLowAttribute(fullList["low"]);
            else
                winningAttribute = PullFromNothing(fullList["none"]);
            return winningAttribute;
        }

        public string PullHighAttribute(Dictionary<string, double> high) {
            string attribute = "neutral";
            int percent = (int)(Ctrl.Dice.NextDouble() * 10 + 1);
            if (high.Count == 0 || percent == 10) {
                return attribute;
            }
            return PullAttribute(high, attribute);
        }

        public string PullMiddleAttribute(Dictionary<string, double> mid) {
            string attribute = "neutral";
            int percent = (int)(Ctrl.Dice.NextDouble() * 10 + 1);
            if (mid.Count == 0 || percent >= 7) {//70% chance for neutral 
                return attribute;
            }
            return PullAttribute(mid,attribute);
        }

        public string PullLowAttribute(Dictionary<string, double> low) {
            string attribute = "neutral";
            int percent = (int)(Ctrl.Dice.NextDouble() * 10 + 1);
            if (low.Count == 0 || percent >= 4) {
                return attribute;
            }
            return PullAttribute(low,attribute);
        }

        public string PullAttribute(Dictionary<string, double> level, string attribute) {
            if (level.ContainsKey("romance"))
                attribute = "romance";
            else if (level.ContainsKey("hate"))
                attribute = "hate";
            else if (level.ContainsKey("disgust"))
                attribute = "disgust";
            else if (level.ContainsKey("friend"))
                attribute = "friend";
            else if (level.ContainsKey("rivalry"))
                attribute = "rivalry";
            else if (level.ContainsKey("professional"))
                attribute = "professional";
            return attribute;
        }

        //pull non attribute value that is contained in dialogue ex--> neutral, stranger
        public string PullFromNothing(Dictionary<string, double> none) {
            string finalAttribute = "neutral";
            Dictionary<string, string> attribute = (speaker.Equals(SmallTalk_Controller.INITIATOR)) ?
                Ctrl._CharacterData.InitiatorAttributeList : Ctrl._CharacterData.ResponderAttributeList;
            if (attribute.ContainsKey("stranger to"))
                finalAttribute = "stranger";
            return finalAttribute;
        }

        //check requirements
        public Dictionary<string, Dictionary<string, List<string>>> TrimDialogueOptions(string attributeBranch) {
            Dictionary<string, Dictionary<string, List<string>>> dialogueList = null;
            if(currentList.ContainsKey(attributeBranch))
                dialogueList = currentList[attributeBranch];
            return dialogueList;
        }

        public string CycleWeatherKeyReqVersusDialogueReq
            (Dictionary<string, Dictionary<string, List<string>>> dialogueList) {
            List<string> list = null;
            string pick = "";
            if (dialogueList != null) {
                var dialogue = WeatherKeyReqVersusDialogueReq(dialogueList);
                list = new List<string>();
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> item in dialogue) {
                    list.Add(item.Key);
                }
                pick = list[(int)(Ctrl.Dice.NextDouble() * list.Count)];
            } else {
                dialogueList = TrimDialogueOptions("neutral");
                pick = CycleWeatherKeyReqVersusDialogueReq(dialogueList);
            }
            return pick;
        }

        //method hub that ultimately will return the dialogue string
        public Dictionary<string, Dictionary<string, List<string>>> WeatherKeyReqVersusDialogueReq
            (Dictionary<string, Dictionary<string, List<string>>> dialogueList) {
            string weatherType = "weather."+Ctrl._GameData.Weather;
            Dictionary<string, Dictionary<string, List<string>>> dialogue = new Dictionary<string, Dictionary<string, List<string>>>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> item in dialogueList) {
                if (dialogueList[item.Key]["req"].Count == 0 || dialogueList[item.Key]["req"][0].Equals(weatherType)) {
                    dialogue[item.Key] = dialogueList[item.Key];  
                }
            }
            //default to neutral if no types are avaialble
            if (dialogue.Count == 0) {
                dialogueList = TrimDialogueOptions("neutral");
                dialogue = WeatherKeyReqVersusDialogueReq(dialogueList);
            }
            return dialogue;
        }

        //method hub that ultimately will return the dialogue string
        public string CycleEventKeysReqVersusDialogueReq
            (Dictionary<string, Dictionary<string, List<string>>> dialogueList) {
            string dialogueChoice;
            if (dialogueList != null) {
                var dialogue = DropLeadsToFromList(dialogueList);
                dialogue = DropDialogueMissingCharacterRequirements(dialogue);
                dialogue = DropDialogueMissingGameRequirements(dialogue);
                List<string> topPicks = PullBestEventStrings(dialogue);
                dialogueChoice = topPicks[(int)(Ctrl.Dice.NextDouble() * topPicks.Count)];
            } else {//default to "neutral"
                dialogueList = TrimDialogueOptions("neutral");
                dialogueChoice = CycleEventKeysReqVersusDialogueReq(dialogueList);
            }
            return dialogueChoice;
        }

        public Dictionary<string, List<string>> DropDialogueMissingGameRequirements
            (Dictionary<string, List<string>> dialogue) {
            foreach (KeyValuePair<string, List<string>> item in dialogue) {
                if (item.Value.Count > 0) {
                    bool hasReq = (item.Value.Count == 1) ? CompareSingleCaseGameReq(item.Value) :
                        CompareMultipleCaseGameReq(item.Value);
                    if (!hasReq)
                        dialogue.Remove(item.Key);
                }
            }
            return dialogue;
        }

        public bool CompareMultipleCaseGameReq(List<string> value) {
            List<string> words1 = new List<string>() { value[0] };
            List<string> words2 = new List<string>() { value[1] };
            return (CompareSingleCaseGameReq(words1) ||
                CompareSingleCaseGameReq(words2));
        }

        public bool CompareSingleCaseGameReq(List<string> value) {
            string[] words = value[0].Split(".");
            bool isGood = false; ;
            if (words[0].Equals("time"))
                isGood = CompareDialogueReqAndGameReq(words);
            return isGood;
        }

        public bool CompareDialogueReqAndGameReq(string[] words) {
            string nextEvent = GetCurrentEvent("");
            bool keepDialogue = false;
            if (words.Length == 2) {
                keepDialogue = !nextEvent.Equals("None") && !nextEvent.Equals("none");
            } else if (words.Length == 3) {
                keepDialogue = words[2].Equals(nextEvent);
            }
            return keepDialogue;
        }

        //remove all dialogue chioce that the character is miss requirements for
        public Dictionary<string, List<string>> DropDialogueMissingCharacterRequirements
            (Dictionary<string, List<string>> dialogue) {
            foreach (KeyValuePair<string, List<string>> item in dialogue) {
                if (item.Value.Count > 0) {
                    bool hasReq = (item.Value.Count == 1) ? CompareSingleCasesCharacterReq(item.Value) :
                        CompareMultipleCasesCharacterReq(item.Value);
                    if (!hasReq)
                        dialogue.Remove(item.Key);
                }
            }
            return dialogue;
        }
        private bool CompareSingleCasesCharacterReq(List<string> reqs) {
            string[] words = reqs[0].Split(".");
            bool isGood = false; ;
            if (words[0].Equals("time"))
                isGood = true;
            else if (words[0].Equals("npc"))
                isGood = CompareDialogueReqAndCharacterReq(words);
            return isGood;
        }
        private bool CompareMultipleCasesCharacterReq(List<string> req) {
            string[] words1 = req[0].Split(".");
            string[] words2 = req[1].Split(".");
            return CompareDialogueReqAndCharacterReq(words1) ||
                CompareDialogueReqAndCharacterReq(words2);
        }
        //returns true if character has reqired dialogue req
        public bool CompareDialogueReqAndCharacterReq(string[] reqs) {
            bool has = false;
            if (reqs.Length>0 && reqs[0].Equals("npc")) {
                var attribute = GetSpeakersAttributes();
                if (reqs[1].Equals("#hates_current_event#"))
                    reqs[1] = GetCurrentEvent("hates_");
                else if(reqs[1].Equals("#loves_current_event#"))
                    reqs[1] = GetCurrentEvent("loves_");
                if (attribute.ContainsKey(reqs[1]))
                    has = true;
            }
            return has;
        }
        //param: love_; return love_art_fest
        public string GetCurrentEvent(string prefix) {
            string nextEvent = Ctrl._GameData.EventIsNear();
            if (!nextEvent.Equals("none")&& !nextEvent.Equals("None")) {
                nextEvent = prefix + nextEvent;
            }
            return nextEvent;
        }

        public Dictionary<string, string> GetSpeakersAttributes() {
            Dictionary<string, string> attribute = (speaker.Equals(SmallTalk_Controller.INITIATOR)) ?
                Ctrl._CharacterData.InitiatorAttributeList : Ctrl._CharacterData.ResponderAttributeList;
            return attribute;
        }

        public Dictionary<string, List<string>> DropLeadsToFromList
            (Dictionary<string, Dictionary<string, List<string>>> dialogueList) {
            Dictionary<string, List<string>> dialogue = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> item in dialogueList) {
                dialogue[item.Key] = dialogueList[item.Key]["req"];
            }
            return dialogue;
        }

        
        
        //pull best string: use as key to find lead to
        public List<string> PullBestEventStrings(Dictionary<string, List<string>> dialogue) {
            List<string> topPicks = new List<string>();
            List<string> defaultPicks = new List<string>();
            //int endPoint = (int)ctrl.Dice.NextDouble() * dialogue.Count;
            foreach (KeyValuePair<string, List<string>> item in dialogue) {
                foreach (string req in item.Value) {
                    string[] words = req.Split(".");
                    if (words.Length > 1) {
                        words = words[1].Split("_");
                        if (words.Contains<string>("#loves") ||
                            words.Contains<string>("loves")  ||
                            words.Contains<string>("#hates") ||
                            words.Contains<string>("hates")) {
                            topPicks.Add(item.Key);
                        }
                    }
                }
                defaultPicks.Add(item.Key);
            }
            if (topPicks.Count > 0)
                return topPicks;
            else
                return defaultPicks;
        }
        /*
          GreetingKeys["req"] = new List<string>() { "time.day.morning", "time.day.afternoon",
                "time.day.evening", "player.female", "player.male"};
             */
        public string CycleGreetingKeysReqVersusDialogueReq
            (Dictionary<string, Dictionary<string, List<string>>> dialogueList) {
            string pick;
            if (dialogueList != null) {
                var dialogue = DropLeadsToFromList(dialogueList);
                dialogue = RemoveGeetingsMissingTimeReq(dialogue);
                dialogue = RemoveGeetingMissingCharacterReq(dialogue);
                List<string> list = GatherGreetingPicks(dialogue);
                if (list == null || list.Count == 0) {
                    dialogueList = TrimDialogueOptions("neutral");
                    pick = CycleGreetingKeysReqVersusDialogueReq(dialogueList);
                } else 
                    pick = list[(int)(ctrl.Dice.NextDouble()*list.Count)];
            } else {
                dialogueList = TrimDialogueOptions("neutral");
                 pick = CycleGreetingKeysReqVersusDialogueReq(dialogueList);
            }
            return pick;
        }

        public List<string> GatherGreetingPicks(Dictionary<string, List<string>> dialogue) {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, List<string>> item in dialogue) {
                list.Add(item.Key);
            }
            return list;
        }

        public Dictionary<string, List<string>> RemoveGeetingMissingCharacterReq
            (Dictionary<string, List<string>> dialogue) {
            foreach (KeyValuePair<string, List<string>> item in dialogue) {
                if (item.Value.Count > 0) {
                    if (!CorrectGender(item.Value)) {
                        dialogue.Remove(item.Key);
                    }
                }
            }
            return dialogue;
        }

        public bool CorrectGender(List<string> value) {
            bool keep = true;
            foreach (string item in value) {
                string[] words = item.Split(".");
                if (words.Length > 0 && words[0].Equals("player")) {
                    string gender = (speaker.Equals(SmallTalk_Controller.INITIATOR)) ?
                        ctrl._CharacterData.RespondersGender : ctrl._CharacterData.InitialorsGender;
                    if (!words[1].Equals(gender)) {
                        keep = false;
                        break;
                    }
                }
            }
            return keep;
        }

        public Dictionary<string, List<string>> RemoveGeetingsMissingTimeReq
            (Dictionary<string, List<string>> dialogue) {
            foreach (KeyValuePair<string, List<string>> item in dialogue) {
                if (item.Value.Count > 0) {
                    if (!CorrectTimeOfDay(item.Value)) {
                        dialogue.Remove(item.Key);
                    }
                }
            }

            return dialogue;
        }
        //
        public bool CorrectTimeOfDay(List<string> list) {
            bool hasReq = true;
            if (list.Count == 1) {
                string[] words = list[0].Split(".");
                if (words.Length == 3&&words[0].Equals("time")) {
                    if (!words[2].Equals(Ctrl._GameData.TimeOfDay)) {
                        hasReq = false;
                    }
                }
            }
            if (list.Count == 2) {
                string[] words1 = list[0].Split(".");
                string[] words2 = list[1].Split(".");
                hasReq = (words1.Length == 3) ? words1[2].Equals(Ctrl._GameData.TimeOfDay) :
                    (words2.Length==3)?words2[2].Equals(Ctrl._GameData.TimeOfDay):true;
            }
            return hasReq;
        }
    }

    /// <summary>
    /// Class functions to pull raw data from the json file and load it into 
    /// SmallTalk_Module dictionaries
    /// </summary>

    public class SmallTalk_Loader {

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
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> temp;
            try {
                temp = ConvertJsonToDictionary();
            } catch (Exception e) {
                Console.WriteLine(e);
                return;
            }
            SetupDictionaries(temp);
        }

        private Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> ConvertJsonToDictionary() {
            string json = ReadFile(module.PathToJson);
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> data =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>>(json);
            return data;
        }

        private void SetupDictionaries(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> temp) {
            SetupWeather(temp);
            SetupCurrentEvent(temp);
            SetupGreetings(temp);

        }

        private void SetupGreetings(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> temp) {
            try {
                module.greetingStatement = temp["Greeting_statement"];
                module.greetingQuestion = temp["Greeting_question"];
                module.greetingResponse = temp["Greeting_response"];
            } catch (Exception e) {
                Console.WriteLine("Faild to uplaod Greeting Conversations\n" + e);
            }
        }

        private void SetupCurrentEvent(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> temp) {
            try {
                module.eventQuestion = temp["current_event_question"];
                module.eventResponse = temp["current_event_response"];
                module.eventStatement = temp["current_event_statement"];
            } catch (Exception e) {
                Console.WriteLine("Failed to upload Event Conversations\n" + e);
            }
        }

        private void SetupWeather(Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> temp) {
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
