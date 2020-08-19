using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kati.Module_Hub;
using Kati.SourceFiles;
using System;
using System.Collections.Generic;
using Kati.Data_Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Threading;

namespace KatiUnitTest.Module_Tests{
    
    [TestClass()]
    public class SmallTalkParserTester{

        private SmallTalk_Module module;
        private SmallTalk_Parser parser;

        [TestInitialize]
        public void Start() {
            //A lot of test rely on a certain count of elements based on a criteria
            //Using a static version of hte json data to not break tests.
            module = new SmallTalk_Module("C:/Users/User/Documents/NLG/KatiUnitTest/Module_Tests/SmallTalk/smallTalk.json");
            parser = new SmallTalk_Parser(module);
            SmallTalk_Controller c = new SmallTalk_Controller(module);
            c._GameData = SetupGameData();
            c._CharacterData = SetupCharacterData();
            c.SetupConversation(c._GameData, c._CharacterData);
            parser.Ctrl = c;
        }

        public GameData SetupGameData() {
            GameData data = new GameData();
            data.DayOfMonth = 6;
            data.Sector = "Sector 3";
            data.Season = "Spring";
            data.TimeOfDay = "evening";
            data.Weather = "nice_day";
            data.SetDayOfWeek();
            data.SetWeek();
            data.EventCalendar["Spring"]["Art_Fest"] = 12;
            data.EventCalendar["Spring"]["Blueberry_Fest"] = 21;
            data.SetPublicEvent();
            return data;
        }

        public CharacterData SetupCharacterData() {
            CharacterData data = new CharacterData();
            data.InitiatorsName = "Kati"; data.InitialorsGender = "Female";
            data.RespondersName = "Stephen"; data.InitialorsGender = "male";
            data.InteractionTone = SetTone();
            data.InitiatorsTone = SetTone();
            data.RespondersTone = SetTone();
            data.InitiatorPersonalList["loves_art_fest"] = "characterTrait";
            data.ResponderAttributeList["loves_art_fest"] = "characterTrait";
            data.InitiatorScalarList["charming"] = 200;
            data.ResponderScalarList["charming"] = 200;
            return data;
        }

        public Dictionary<string, double> SetTone() {
            Random dice = new Random();
            string[] str = { "romance", "friend","professional","respect",
                             "Admiration","disgust","hate","rivalry"};
            Dictionary<string, double> temp = new Dictionary<string, double>();
            for (int i = 0; i < str.Length; i++)
                temp[str[i]] = dice.NextDouble();
            return temp;
        }

        [TestMethod]
        public void TestConstructor() {
            parser = new SmallTalk_Parser(module);
        }

        [TestMethod]
        public void TestSetStage() {
            parser.SetStage("initiator","statement","greeting");
            Assert.IsTrue(parser.Speaker.Equals("initiator"));
            Assert.IsTrue(parser.StateType.Equals("statement"));
            Assert.IsTrue(parser.Topic.Equals("greeting"));
        }

        [TestMethod]
        public void TestCurrentListWeatherStatement() {
            parser.SetStage("initiator", "statement", "weather");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["neutral"]
                ["It sure is a nice day."]["req"][0].Equals("weather.nice_day"));
        }

        [TestMethod]
        public void TestCurrentListWeatherQuestion() {
            parser.SetStage("initiator", "question", "weather");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["neutral"]
                ["Nice day, don't you think?"]["req"][0].Equals("weather.nice_day"));
        }

        [TestMethod]
        public void TestCurrentListWeatherResponse() {
            parser.SetStage("initiator", "response", "weather");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["neutral"]
                ["I know, the weather is great."]["req"][0].Equals("weather.nice_day"));
        }

        [TestMethod]
        public void TestCurrentListEventStatement() {
            parser.SetStage("initiator", "statement", "event");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["neutral"]
                ["It's almost time for #current_event#."]["req"][0]
                .Equals("time.#current_event#"));
        }

        [TestMethod]
        public void TestCurrentListEventQuestion() {
            parser.SetStage("initiator", "question", "event");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["stranger"]
                ["Do you like #current_event#?"]["req"][0]
                .Equals("time.#current_event#"));
        }

        [TestMethod]
        public void TestCurrentListEventResponse() {
            parser.SetStage("initiator", "response", "event");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["neutral"]
                ["The #current_event# is okay. I enjoy it well enough."]["req"][0]
                .Equals("likeCurrentEvent"));
        }

        [TestMethod]
        public void TestCurrentListGreetingStatement() {
            parser.SetStage("initiator", "statement", "greeting");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["neutral"]
                ["Hi."]["leads to"][0]
                .Equals("greeting_question"));
        }

        [TestMethod]
        public void TestCurrentListGreetingQuestion() {
            parser.SetStage("initiator", "question", "greeting");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["neutral"]
                ["How are you today?"]["leads to"][0]
                .Equals("greeting_response"));
        }

        [TestMethod]
        public void TestCurrentListGreetingResponse() {
            parser.SetStage("initiator", "response", "greeting");
            parser.SetCurrentListTopic();
            Assert.IsNotNull(parser.CurrentList);
            Assert.IsTrue(parser.CurrentList["positive"]
                ["I'm doing great."]["leads to"][0]
                .Equals("friend.+"));
        }

        [TestMethod]
        public void TestCancelAttributeTone() {
            Dictionary<string, double> temp = new Dictionary<string, double>();
            temp["romance"] = 0.51; temp["friend"] = 0.8;
            temp["professional"] = 0.01; temp["respect"] = 0.6;
            temp["admiration"] = 0.01; temp["disgust"] = 0.2;
            temp["hate"] = 0.01; temp["rivalry"] = 0.75;
            parser.SetStage("initiator", "statement", "greeting");
            parser.Ctrl._CharacterData.InitiatorsTone = temp;
            temp = parser.CancelAttributeTones();
            Assert.IsNotNull(temp);
            Assert.IsTrue(temp["romance"] == 0.31);
            Assert.IsTrue(temp["disgust"] == 0.0);
            Assert.IsTrue(temp["friend"] == 0.79);
            Assert.IsTrue(temp["hate"] == 0.0);
            Assert.IsTrue(temp["professional"] == 0.0);
            Assert.IsTrue(temp["rivalry"] == 0.74);
        }

        private Dictionary<string, double> SetUpToneValues
            (double rom, double fri, double pro, double res, double adm, double dis, double hate, double riv) {
            Dictionary<string, double> temp = new Dictionary<string, double>();
            temp["romance"] = rom; temp["friend"] = fri;
            temp["professional"] = pro; temp["respect"] = res;
            temp["admiration"] = adm; temp["disgust"] = dis;
            temp["hate"] = hate; temp["rivalry"] = riv;
            parser.SetStage("initiator", "statement", "greeting");
            parser.Ctrl._CharacterData.InitiatorsTone = temp;
            return temp;
        }

        [TestMethod]
        public void TestMatchCharacterAttributesToAvailableDialogueTones() {
            Dictionary<string, double> temp = new Dictionary<string, double>();
            temp["romance"] = 0.51; temp["friend"] = 0.8;
            temp["professional"] = 0.01; temp["respect"] = 0.6;
            temp["admiration"] = 0.01; temp["disgust"] = 0.2;
            temp["hate"] = 0.01; temp["rivalry"] = 0.75;
            parser.SetStage("initiator", "statement", "greeting");
            parser.Ctrl._CharacterData.InitiatorsTone = temp;
            parser.SetCurrentListTopic();
            temp = parser.MatchCharacterAttributesToAvailableDialogueTones(temp);
            Assert.IsFalse(temp.ContainsKey("respect"));
            Assert.IsFalse(temp.ContainsKey("admiration"));
            Assert.IsFalse(temp.ContainsKey("disgust"));
            Assert.IsFalse(temp.ContainsKey("rivalry"));
            Assert.IsTrue(temp.ContainsKey("friend"));
            Assert.IsTrue(temp.ContainsKey("romance"));
            Assert.IsTrue(temp.ContainsKey("professional"));
            Assert.IsTrue(temp.ContainsKey("neutral"));
            Assert.IsTrue(temp.ContainsKey("stranger"));
            Assert.IsTrue(temp["romance"] == 0.51);
            Assert.IsTrue(temp["professional"] == 0.01);
            Assert.IsTrue(temp["friend"] == 0.8);
            Assert.IsTrue(temp["neutral"] == 0.0);
            Assert.IsTrue(temp["stranger"] == 0.0);
        }

        [TestMethod]
        public void TestCategorizedTone() {
            Dictionary<string, double> temp = new Dictionary<string, double>();
            temp["romance"] = 0.51; temp["friend"] = 0.8;
            temp["professional"] = 0.01; temp["respect"] = 0.6;
            temp["admiration"] = 0.01; temp["disgust"] = 0.2;
            temp["hate"] = 0.01; temp["rivalry"] = 0.75;
            
            Dictionary<string, Dictionary<string, double>> sorted = 
                new Dictionary<string, Dictionary<string, double>>();
            sorted = parser.CategorizeTone(temp);
            Assert.IsNotNull(sorted);
            Assert.IsTrue(sorted["high"].ContainsKey("rivalry"));
            Assert.IsTrue(sorted["high"].ContainsKey("friend"));
            Assert.IsTrue(sorted["mid"].ContainsKey("romance"));
            Assert.IsTrue(sorted["mid"].ContainsKey("respect"));
            //Assert.IsTrue(sorted[""].Count==0);
        }

        [TestMethod]
        public void TestPullHighAttribute() {
            //romance, friend, professional, respect, admiration, disgust, hate, rivalry 
            Dictionary<string, double> tone = SetUpToneValues(0.9, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2);
            Dictionary<string, Dictionary<string, double>> cat = parser.CategorizeTone(tone);
            string attribute = parser.PullHighAttribute(cat["high"]);
            Assert.IsTrue(attribute.Equals("romance")|| attribute.Equals("neutral"));
            //friend
            tone = SetUpToneValues(0.2, 0.9, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullHighAttribute(cat["high"]);
            Assert.IsTrue(attribute.Equals("friend") || attribute.Equals("neutral"));
            //professional
            tone = SetUpToneValues(0.2, 0.2, 0.9, 0.2, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullHighAttribute(cat["high"]);
            Assert.IsTrue(attribute.Equals("professional") || attribute.Equals("neutral"));
            //respect
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.9, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullHighAttribute(cat["high"]);
            Assert.IsTrue(attribute.Equals("respect") || attribute.Equals("neutral"));
            //admiration
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.9, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullHighAttribute(cat["high"]);
            Assert.IsTrue(attribute.Equals("admiration") || attribute.Equals("neutral"));
            //disgust
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.9, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullHighAttribute(cat["high"]);
            Assert.IsTrue(attribute.Equals("disgust") || attribute.Equals("neutral"));
            //hate
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.9, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullHighAttribute(cat["high"]);
            Assert.IsTrue(attribute.Equals("hate") || attribute.Equals("neutral"));
            //rivalry
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.9);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullHighAttribute(cat["high"]);
            Assert.IsTrue(attribute.Equals("rivalry") || attribute.Equals("neutral"));
        }

        [TestMethod]
        public void TestPullMidAttribute() {
            //romance, friend, professional, respect, admiration, disgust, hate, rivalry 
            Dictionary<string, double> tone = SetUpToneValues(0.9, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2);
            Dictionary<string, Dictionary<string, double>> cat = parser.CategorizeTone(tone);
            string attribute = parser.PullMiddleAttribute(cat["mid"]);
            Assert.IsTrue(attribute.Equals("romance") || attribute.Equals("neutral"));
            //friend
            tone = SetUpToneValues(0.2, 0.9, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullMiddleAttribute(cat["mid"]);
            Assert.IsTrue(attribute.Equals("friend") || attribute.Equals("neutral"));
            //professional
            tone = SetUpToneValues(0.2, 0.2, 0.9, 0.2, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullMiddleAttribute(cat["mid"]);
            Assert.IsTrue(attribute.Equals("professional") || attribute.Equals("neutral"));
            //respect
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.9, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullMiddleAttribute(cat["mid"]);
            Assert.IsTrue(attribute.Equals("respect") || attribute.Equals("neutral"));
            //admiration
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.9, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullMiddleAttribute(cat["mid"]);
            Assert.IsTrue(attribute.Equals("admiration") || attribute.Equals("neutral"));
            //disgust
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.9, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullMiddleAttribute(cat["mid"]);
            Assert.IsTrue(attribute.Equals("disgust") || attribute.Equals("neutral"));
            //hate
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.9, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullMiddleAttribute(cat["mid"]);
            Assert.IsTrue(attribute.Equals("hate") || attribute.Equals("neutral"));
            //rivalry
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.9);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullMiddleAttribute(cat["mid"]);
            Assert.IsTrue(attribute.Equals("rivalry") || attribute.Equals("neutral"));
        }

        [TestMethod]
        public void TestPullLowAttribute() {
            //romance, friend, professional, respect, admiration, disgust, hate, rivalry 
            Dictionary<string, double> tone = SetUpToneValues(0.3, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2);
            Dictionary<string, Dictionary<string, double>> cat = parser.CategorizeTone(tone);
            string attribute = parser.PullLowAttribute(cat["low"]);
            Assert.IsTrue(attribute.Equals("romance") || attribute.Equals("neutral"));
            //friend
            tone = SetUpToneValues(0.2, 0.3, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullLowAttribute(cat["low"]);
            Assert.IsTrue(attribute.Equals("friend") || attribute.Equals("neutral"));
            //professional
            tone = SetUpToneValues(0.2, 0.2, 0.3, 0.2, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullLowAttribute(cat["low"]);
            Assert.IsTrue(attribute.Equals("professional") || attribute.Equals("neutral"));
            //respect
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.3, 0.2, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullLowAttribute(cat["low"]);
            Assert.IsTrue(attribute.Equals("respect") || attribute.Equals("neutral"));
            //admiration
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.3, 0.2, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullLowAttribute(cat["low"]);
            Assert.IsTrue(attribute.Equals("admiration") || attribute.Equals("neutral"));
            //disgust
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.3, 0.2, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullLowAttribute(cat["low"]);
            Assert.IsTrue(attribute.Equals("disgust") || attribute.Equals("neutral"));
            //hate
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.3, 0.2);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullLowAttribute(cat["low"]);
            Assert.IsTrue(attribute.Equals("hate") || attribute.Equals("neutral"));
            //rivalry
            tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.3);
            cat = parser.CategorizeTone(tone);
            attribute = parser.PullLowAttribute(cat["low"]);
            Assert.IsTrue(attribute.Equals("rivalry") || attribute.Equals("neutral"));
        }

        [TestMethod]
        public void TestPullFromNothing() {
            Dictionary<string, double> tone = SetUpToneValues(0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2, 0.2);
            Dictionary<string, Dictionary<string, double>> cat = parser.CategorizeTone(tone);
            string attribute = parser.PullFromNothing(cat["none"]);
            Assert.IsTrue(attribute.Equals("neutral"));

            parser.Ctrl._CharacterData.InitiatorPersonalList["stranger to"] = "characterTrait";
            parser.Ctrl._CharacterData.ResponderAttributeList["stranger to"] = "characterTrait";

            attribute = parser.PullFromNothing(cat["none"]);
            Assert.IsTrue(attribute.Equals("stranger"));
        }

        [TestMethod]
        public void TestSetupKeys() {
            parser.SetupConversationKeys();
            Assert.IsTrue(parser.WeatherKeys["req"][0].Equals("weather.nice_day"));
            Assert.IsTrue(parser.WeatherKeys["leads to"][0].Equals("small_talk.weather_response"));
            Assert.IsTrue(parser.EventKeys["req"].Contains("npc.hates_halloween"));
            Assert.IsTrue(parser.EventKeys["leads to"].Contains("#current_event#.like+"));
            Assert.IsTrue(parser.GreetingKeys["req"].Contains("player.female"));
            Assert.IsTrue(parser.GreetingKeys["leads to"].Contains("greeting_response"));
        }

        [TestMethod]
        public void TestTrimDialogue() {
            parser.SetStage("initiator", "statement", "greeting");
            parser.SetCurrentListTopic();
            var dialogue = parser.TrimDialogueOptions("neutral");
            Assert.IsTrue(dialogue.Count==3);
            Assert.IsTrue(dialogue.ContainsKey("Hi."));
            Assert.IsTrue(dialogue.ContainsKey("Hey."));
            Assert.IsTrue(dialogue.ContainsKey("Hello #player.name#."));
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> item in dialogue) {
                Assert.IsTrue(item.Value["req"].Count == 0);
                Assert.IsTrue(item.Value["leads to"][0].Equals("greeting_question"));
            }
            dialogue = parser.TrimDialogueOptions("lover");
            Assert.IsNull(dialogue);
        }

        [TestMethod]
        public void TestCycleWeatherKeyReqVersusDialogueReq() {
            parser.SetStage("initiator", "statement", "weather");
            parser.SetCurrentListTopic();
            var dialogue = parser.TrimDialogueOptions("neutral");
            string choice;
            for (int i = 0; i < 10; i++) {
                choice = parser.CycleWeatherKeyReqVersusDialogueReq(dialogue);
                Assert.IsTrue(choice.Equals("It is a wonderful day. The sky is so clear.") ||
                    choice.Equals("It sure is a nice day.") || 
                    choice.Equals("Its a nice day to go outside.")||
                    choice.Equals("The weather has been favorable lately.")||
                    choice.Equals("It's such a lovely day today."));
            }
        }

        [TestMethod]
        public void TestWeatherKeyReqVersusDialogueReq() {
            parser.SetStage("initiator", "statement", "weather");
            parser.SetCurrentListTopic();
            var dialogue = parser.TrimDialogueOptions("neutral");
            dialogue = parser.WeatherKeyReqVersusDialogueReq(dialogue);
            Assert.IsTrue(dialogue.Count==5);
            Assert.IsTrue(dialogue.ContainsKey("It is a wonderful day. The sky is so clear."));
            Assert.IsTrue(dialogue.ContainsKey("It sure is a nice day."));
            Assert.IsTrue(dialogue.ContainsKey("Its a nice day to go outside."));
            Assert.IsTrue(dialogue.ContainsKey("The weather has been favorable lately."));
            Assert.IsTrue(dialogue.ContainsKey("It's such a lovely day today."));
            dialogue = parser.TrimDialogueOptions("romance");
            dialogue = parser.WeatherKeyReqVersusDialogueReq(dialogue);
            Assert.IsTrue(dialogue.Count == 5);
            Assert.IsTrue(dialogue.ContainsKey("It is a wonderful day. The sky is so clear."));
            Assert.IsTrue(dialogue.ContainsKey("It sure is a nice day."));
            Assert.IsTrue(dialogue.ContainsKey("Its a nice day to go outside."));
            Assert.IsTrue(dialogue.ContainsKey("The weather has been favorable lately."));
            Assert.IsTrue(dialogue.ContainsKey("It's such a lovely day today."));
            parser.Ctrl._GameData.Weather = "hot";
            dialogue = parser.TrimDialogueOptions("romance");
            dialogue = parser.WeatherKeyReqVersusDialogueReq(dialogue);
            Assert.IsTrue(dialogue.Count == 1);
            Assert.IsTrue(dialogue.ContainsKey("The weather outside is almost as hot as you."));
            dialogue = parser.TrimDialogueOptions("disgust");
            dialogue = parser.WeatherKeyReqVersusDialogueReq(dialogue);
            Assert.IsTrue(dialogue.Count == 1);
            Assert.IsTrue(dialogue.ContainsKey("It's hot."));
        }

        [TestMethod]
        public void TestDropLeadsFromList() {
            parser.SetStage("initiator", "statement", "event");
            parser.Ctrl._GameData.DayOfMonth = 10;
            parser.Ctrl._CharacterData.InitiatorPersonalList["loves_art_fest"] = "character_trait";
            parser.SetCurrentListTopic();
            var dialogueList = parser.TrimDialogueOptions("neutral");
            var dialogue = parser.DropLeadsToFromList(dialogueList);
            Assert.IsTrue(dialogue.Count>0);
            Assert.IsTrue(!dialogue.ContainsKey("It's nearly time for #current_event#."));
            Assert.IsTrue(dialogue["Happy Halloween."][1].Equals("npc.loves_halloween"));
            Assert.IsTrue(dialogue["It's almost time for #current_event#."][0]
                .Equals("time.#current_event#"));
            Assert.AreEqual(38, dialogue.Count);
        }

        [TestMethod]
        public void TestGetSpeakersAttributes() {
            parser.Ctrl._CharacterData.InitiatorPersonalList["farty"] = "characterTrait";
            parser.SetStage("initiator", "statement", "event");
            var attributes = parser.GetSpeakersAttributes();
            Assert.IsTrue(attributes.Count==2);
            Assert.IsTrue(attributes.ContainsKey("farty"));
            Assert.IsTrue(attributes.ContainsKey("loves_art_fest"));
            parser.SetStage("responder", "statement", "event");
            attributes = parser.GetSpeakersAttributes();
            Assert.IsTrue(attributes.Count == 1);
            Assert.IsTrue(!attributes.ContainsKey("farty"));
            Assert.IsTrue(attributes.ContainsKey("loves_art_fest"));
        }

        [TestMethod]
        public void TestGetCurrentEvent() {
            string attribute = parser.GetCurrentEvent("loves_");
            Assert.AreEqual("loves_art_fest", attribute);
            attribute = parser.GetCurrentEvent("hates_");
            Assert.AreEqual("hates_art_fest", attribute);
            parser.Ctrl._GameData.DayOfMonth=2;
            parser.Ctrl._GameData.SetPublicEvent();
            attribute = parser.GetCurrentEvent("loves_");
            Assert.AreEqual("none", attribute);
            attribute = parser.GetCurrentEvent("hates_");
            Assert.AreEqual("none", attribute);
        }

        [TestMethod]
        public void TestCompareDialogueReqAndCharacterReq() {
            parser.SetStage("initiator", "statement", "event");
            string[] words = { "npc", "#hates_current_event#" };
            Assert.IsTrue(!parser.CompareDialogueReqAndCharacterReq(words));
            words[1] =  "#loves_current_event#";
            Assert.IsTrue(parser.CompareDialogueReqAndCharacterReq(words));
            words[1] = "loves_art_fest";
            Assert.IsTrue(parser.CompareDialogueReqAndCharacterReq(words));
            words[1] = "hates_art_fest";
            Assert.IsTrue(!parser.CompareDialogueReqAndCharacterReq(words));
            parser.SetStage("responder", "statement", "event");
            words[1] = "#hates_current_event#";
            Assert.IsTrue(!parser.CompareDialogueReqAndCharacterReq(words));
            words[1] = "#loves_current_event#";
            Assert.IsTrue(parser.CompareDialogueReqAndCharacterReq(words));
            words[1] = "loves_art_fest";
            Assert.IsTrue(parser.CompareDialogueReqAndCharacterReq(words));
            words[1] = "hates_art_fest";
            Assert.IsTrue(!parser.CompareDialogueReqAndCharacterReq(words));
        }

        [TestMethod]
        public void TestDropDialogueMissingCharacterRequirements() {
            parser.SetStage("initiator", "statement", "event");
            parser.SetCurrentListTopic();
            var d = parser.TrimDialogueOptions("neutral");
            var dialogue = parser.DropLeadsToFromList(d);
            dialogue = parser.DropDialogueMissingCharacterRequirements(dialogue);
            Assert.IsTrue(dialogue.Count==19);
            Assert.IsTrue(dialogue.ContainsKey(
                "You know, the art festival is happening soon.  I love seeing all of the art on display."));
            Assert.IsTrue(dialogue.ContainsKey("It's almost time for #current_event#."));
        }

        [TestMethod]
        public void TestCompareDialogueReqAndGameReq() {
            string[] two = { "time", "#current_event#" };
            string[] three = { "time","event","art_fest" };
            bool isTrue = parser.CompareDialogueReqAndGameReq(two);
            Assert.IsTrue(isTrue);
            isTrue = parser.CompareDialogueReqAndGameReq(three);
            Assert.IsTrue(isTrue);
            three[2] = "halloween";
            isTrue = parser.CompareDialogueReqAndGameReq(three);
            Assert.IsTrue(!isTrue);
            parser.Ctrl._GameData.DayOfMonth = 1;
            parser.Ctrl._GameData.SetPublicEvent();
            isTrue = parser.CompareDialogueReqAndGameReq(two);
            Assert.IsTrue(!isTrue);
            three[2] = "art_fest";
            isTrue = parser.CompareDialogueReqAndGameReq(three);
            Assert.IsTrue(!isTrue);
            parser.Ctrl._GameData.DayOfMonth = 11;
            parser.Ctrl._GameData.SetPublicEvent();
            isTrue = parser.CompareDialogueReqAndGameReq(three);
            Assert.IsTrue(isTrue);
        }

        [TestMethod]
        public void TestCompareSingleCaseGameReq() {
            List<string> two = new List<string>(){ "time.#current_event#" };
            bool isTrue = parser.CompareSingleCaseGameReq(two);
            Assert.IsTrue(isTrue);
            parser.Ctrl._GameData.DayOfMonth = 1;
            parser.Ctrl._GameData.SetPublicEvent();
            isTrue = parser.CompareSingleCaseGameReq(two);
            Assert.IsTrue(!isTrue);
            two = new List<string>() {"time.event.art_fest"};
            isTrue = parser.CompareSingleCaseGameReq(two);
            Assert.IsTrue(!isTrue);
            parser.Ctrl._GameData.DayOfMonth = 11;
            parser.Ctrl._GameData.SetPublicEvent();
            isTrue = parser.CompareSingleCaseGameReq(two);
            Assert.IsTrue(isTrue);
            two = new List<string>() { "npc.loves_art_fest" };
            isTrue = parser.CompareSingleCaseGameReq(two);
            Assert.IsTrue(!isTrue);
            two = new List<string>() { "time.event.yuletide" };
            isTrue = parser.CompareSingleCaseGameReq(two);
            Assert.IsTrue(!isTrue);
        }

        [TestMethod]
        public void TestSetResponseType() {
            parser.SetStage("initiator", "question", "event");
            string str = "I was thinking about going to #current_event#, are you going?";
            parser.SetResponseType(str);
            Assert.IsTrue(parser.ResponseType.Equals("goingToCurrentEvent"));
            str = "The #current_event# will be happening soon.  A lot of people venture of to see the festivities. How about you? Will you be going?";
            parser.SetResponseType(str);
            Assert.IsTrue(parser.ResponseType.Equals("goingToCurrentEvent"));
            for (int i = 0; i < 50; i++) {
                str = parser.SetRelationshipDialogueTone();
                parser.SetResponseType(str);
                Assert.IsTrue(parser.ResponseType.Equals("goingToCurrentEvent")||
                    parser.ResponseType.Equals("likeCurrentEvent"));
            }
        }

        [TestMethod]
        public void TestCompareMultipleCaseGameReq() {
            parser.Ctrl._GameData.DayOfMonth = 10;
            parser.Ctrl._GameData.SetPublicEvent();
            List<string> list = new List<string>() { "time.#current_event#", "npc.loves_art_fest" };
            bool isTrue = parser.CompareMultipleCaseGameReq(list);
            Assert.IsTrue(isTrue);
            list = new List<string>() { "npc.loves_art_fest", "time.#current_event#" };
            isTrue = parser.CompareMultipleCaseGameReq(list);
            Assert.IsTrue(isTrue);
            list = new List<string>() { "npc.loves_art_fest", "time.event.art_fest" };
            isTrue = parser.CompareMultipleCaseGameReq(list);
            Assert.IsTrue(isTrue);
            list[1] = "npc.loves_art_fest";
            list[0] = "time.event.halloween";
            isTrue = parser.CompareMultipleCaseGameReq(list);
            Assert.IsTrue(!isTrue);
            parser.Ctrl._GameData.Season = "Fall";
            parser.Ctrl._GameData.DayOfMonth = 11;
            parser.Ctrl._GameData.SetPublicEvent();
            list = new List<string>() { "npc.loves_art_fest", "time.#current_event#" };
            isTrue = parser.CompareMultipleCaseGameReq(list);
            Assert.IsTrue(isTrue);
            list = new List<string>() { "npc.loves_art_fest", "time.event.art_fest" };
            isTrue = parser.CompareMultipleCaseGameReq(list);
            Assert.IsTrue(!isTrue);
            list[1] = "time.event.music_fest";
            isTrue = parser.CompareMultipleCaseGameReq(list);
            Assert.IsTrue(isTrue);
        }

        [TestMethod]
        public void TestDropDialogueMissingGameRequirements() {
            parser.SetStage("initiator", "statement", "event");
            parser.SetCurrentListTopic();
            var dialogueList = parser.TrimDialogueOptions("neutral");
            var dialogue = parser.DropLeadsToFromList(dialogueList);
            dialogue = parser.DropDialogueMissingGameRequirements(dialogue);
            Assert.IsTrue(dialogue.Count == 19);
            Assert.IsTrue(dialogue.ContainsKey("I've been so busy lately. The time seems to just fly by."));
            Assert.IsTrue(dialogue.ContainsKey
                ("I'm so excited about #current_event#. There is so much to do and see."));
            Assert.IsTrue(dialogue.ContainsKey
                ("It's almost time for #current_event#."));
            Assert.IsTrue(dialogue.ContainsKey
                ("You know, the art festival is happening soon.  I love seeing all of the art on display."));
            Assert.IsTrue(!dialogue.ContainsKey
                ("The Music Festival is almost here.  It is the one time of the year that music can be heard on the streets.  It's kind of a large party."));
        }

        [TestMethod]
        public void TestDropDialogueCombo() {
            parser.SetStage("initiator", "statement", "event");
            parser.SetCurrentListTopic();
            var dialogueList = parser.TrimDialogueOptions("neutral");
            var dialogue = parser.DropLeadsToFromList(dialogueList);
            dialogue = parser.DropDialogueMissingCharacterRequirements(dialogue);
            dialogue = parser.DropDialogueMissingGameRequirements(dialogue);
            Assert.IsTrue(dialogue.Count == 13);
        }

        [TestMethod]
        public void TestPullBestEventStrings() {
            parser.SetStage("initiator", "statement", "event");
            parser.SetCurrentListTopic();
            var dialogueList = parser.TrimDialogueOptions("neutral");
            var dialogue = parser.DropLeadsToFromList(dialogueList);
            dialogue = parser.DropDialogueMissingCharacterRequirements(dialogue);
            dialogue = parser.DropDialogueMissingGameRequirements(dialogue);
            var list = parser.PullBestEventStrings(dialogue);
            Assert.IsTrue(list.Count == 6);
            Assert.IsTrue(list.Contains
                ("You know, the art festival is happening soon.  I love seeing all of the art on display."));
        }

        [TestMethod]
        public void TestCycleEventKeysReqVersusDialogueReq1() {
            parser.SetStage("initiator", "statement", "event");
            parser.SetCurrentListTopic();
            for (int i = 0; i < 10; i++) {
                var dialogueList = parser.TrimDialogueOptions("neutral");
                string choice = parser.CycleEventKeysReqVersusDialogueReq(dialogueList);
                Assert.IsTrue(choice.Equals("The #current_event# is around the corner. I just get so excited about during this time of the year.")
                    || choice.Equals("I just love #current_event#. It is always so exciting.")
                    || choice.Equals("It's almost #current_event#. It's my favorite festival of the year.  It always fills me with inspiration.")
                    || choice.Equals("I'm so excited about #current_event#. There is so much to do and see.")
                    || choice.Equals("The art festival is near. I just love going out and looking at all of those master pieces.  Every year I Say that I won't buy anything, but I always do.")
                    || choice.Equals("You know, the art festival is happening soon.  I love seeing all of the art on display."));
            }
            parser.SetStage("initiator", "statement", "event");
            parser.SetCurrentListTopic();
            var dialogue = parser.TrimDialogueOptions("stranger");
            string choice2 = parser.CycleEventKeysReqVersusDialogueReq(dialogue);
            Assert.IsTrue(choice2.Equals("It's nearly time for the #current_event#."));
            dialogue = parser.TrimDialogueOptions("burrow owl");
            choice2 = parser.CycleEventKeysReqVersusDialogueReq(dialogue);
            Assert.IsTrue(choice2.Equals("The #current_event# is around the corner. I just get so excited about during this time of the year.")
                    || choice2.Equals("I just love #current_event#. It is always so exciting.")
                    || choice2.Equals("It's almost #current_event#. It's my favorite festival of the year.  It always fills me with inspiration.")
                    || choice2.Equals("I'm so excited about #current_event#. There is so much to do and see.")
                    || choice2.Equals("The art festival is near. I just love going out and looking at all of those master pieces.  Every year I Say that I won't buy anything, but I always do.")
                    || choice2.Equals("You know, the art festival is happening soon.  I love seeing all of the art on display."));

        }

        [TestMethod]
        public void TestCorrectTimeOfDay() {
            List<string> req = new List<string>() { "not.inTheGame", "time.day.morning" };
            parser.Ctrl._GameData.TimeOfDay = "morning";
            bool isTrue = parser.CorrectTimeOfDay(req);
            Assert.IsTrue(isTrue);
            parser.Ctrl._GameData.TimeOfDay = "evening";
            isTrue = parser.CorrectTimeOfDay(req);
            Assert.IsTrue(!isTrue);
            req[1] = "Turd.explosion.chemical";
            isTrue = parser.CorrectTimeOfDay(req);
            Assert.IsTrue(!isTrue);
            req[1] = "Time.day.evening";
            isTrue = parser.CorrectTimeOfDay(req);
            Assert.IsTrue(isTrue);
            req = new List<string>();
            isTrue = parser.CorrectTimeOfDay(req);
            Assert.IsTrue(isTrue);
        }

        [TestMethod]
        public void TestRemoveGeetingsMissingTimeReq() {
            parser.SetStage("initiator", "statement", "greeting");
            parser.SetCurrentListTopic();
            parser.Ctrl._GameData.TimeOfDay = "morning";
            var dialogueList = parser.TrimDialogueOptions("stranger");
            var dialogue = parser.DropLeadsToFromList(dialogueList);
            dialogue = parser.RemoveGeetingsMissingTimeReq(dialogue);
            Assert.IsTrue(dialogue.Count == 2);
            Assert.IsTrue(dialogue.ContainsKey("Good morning"));
            Assert.IsTrue(dialogue.ContainsKey("Hello"));
            dialogueList = parser.TrimDialogueOptions("professional");
            dialogue = parser.DropLeadsToFromList(dialogueList);
            dialogue = parser.RemoveGeetingsMissingTimeReq(dialogue);
            Assert.IsTrue(dialogue.Count ==5 );
        }

        [TestMethod]
        public void TestCorrectGender() {
            parser.Ctrl._CharacterData.RespondersGender = "male";
            parser.SetStage("initiator", "statement", "greeting");
            List<string> req = new List<string>();
            bool isTrue = parser.CorrectGender(req);
            Assert.IsTrue(isTrue);
            req.Add("time.day.afternoon");
            isTrue = parser.CorrectGender(req);
            Assert.IsTrue(isTrue);
            req.Add("player.male");
            isTrue = parser.CorrectGender(req);
            Assert.IsTrue(isTrue);
            req[1] = "player.female";
            isTrue = parser.CorrectGender(req);
            Assert.IsFalse(isTrue);
            req = new List<string>() { "time.day.morning", "player.female" };
            isTrue = parser.CorrectGender(req);
            Assert.IsTrue(!isTrue);
            req = new List<string>() { "time.day.morning", "player.male" };
            isTrue = parser.CorrectGender(req);
            Assert.IsTrue(isTrue);
        }

        [TestMethod]
        public void TestRemoveGeetingMissingCharacterReq() {
            parser.Ctrl._CharacterData.RespondersGender = "female";
            parser.SetStage("initiator", "statement", "greeting");
            parser.SetCurrentListTopic();
            parser.Ctrl._GameData.TimeOfDay = "morning";
            var dialogueList = parser.TrimDialogueOptions("romance");
            var dialogue = parser.DropLeadsToFromList(dialogueList);
            var list = parser.RemoveGeetingMissingCharacterReq(dialogue);
            Assert.IsTrue(list.Count == 1);
            Assert.IsTrue(list.ContainsKey("Hey beutiful."));
            Assert.IsFalse(list.ContainsKey("Hey handsome."));
        }

        [TestMethod]
        public void TestGatherGreetingPicks() {
            parser.Ctrl._CharacterData.RespondersGender = "female";
            parser.SetStage("initiator", "statement", "greeting");
            parser.SetCurrentListTopic();
            parser.Ctrl._GameData.TimeOfDay = "morning";
            var dialogueList = parser.TrimDialogueOptions("professional");
            Assert.IsTrue(dialogueList.Count == 9);
            var dialogue = parser.DropLeadsToFromList(dialogueList);
            Assert.IsTrue(dialogueList.Count == 9);
            dialogue = parser.RemoveGeetingsMissingTimeReq(dialogue);
            Assert.IsTrue(dialogue.Count == 5);
            dialogue = parser.RemoveGeetingMissingCharacterReq(dialogue);
            Assert.IsTrue(dialogue.Count==3);
            List<string> list = parser.GatherGreetingPicks(dialogue);
            Assert.IsNotNull(list);
            Assert.IsTrue(list.Count == 3);
            Assert.IsTrue(list.Contains("Hello  ma'am."));
            Assert.IsTrue(list.Contains("Good morning ma'am."));
            Assert.IsTrue(list.Contains("Greeting #player.name#."));
        }

        [TestMethod]
        public void TestCycleGreetingKeysReqVersusDialogueReq() {
            parser.Ctrl._CharacterData.RespondersGender = "female";
            parser.SetStage("initiator", "statement", "greeting");
            parser.SetCurrentListTopic();
            parser.Ctrl._GameData.TimeOfDay = "morning";
            var dialogueList = parser.TrimDialogueOptions("professional");
            string choice = parser.CycleGreetingKeysReqVersusDialogueReq(dialogueList);
            for (int i = 0; i < 10; i++) {
                Assert.IsTrue(choice.Equals("Hello  ma'am.") ||
                    choice.Equals("Good morning ma'am.") ||
                    choice.Equals("Greeting #player.name#."));
            }
            dialogueList = parser.TrimDialogueOptions("romance");
            choice = parser.CycleGreetingKeysReqVersusDialogueReq(dialogueList);
            Assert.IsTrue(choice.Equals("Hey beutiful."));
        }

        [TestMethod]
        public void TestGenerateDialogue() {
            parser.Ctrl._CharacterData.RespondersGender = "female";
            parser.SetStage("initiator","statement","weather");
            string choice = parser.GetDialogue();
            Assert.IsTrue(choice.Length > 0);

            parser.SetStage("initiator", "question", "weather");
            choice = parser.GetDialogue();
            Assert.IsTrue(choice.Length > 0);
            
            parser.SetStage("initiator", "statement", "event");
            choice = parser.GetDialogue();
            Assert.IsTrue(choice.Length > 0);
            
            parser.SetStage("initiator", "question", "event");
            choice = parser.GetDialogue();
            Assert.IsTrue(choice.Length > 0);
            
            
            parser.SetStage("initiator", "statement", "greeting");
            choice = parser.GetDialogue();
            Assert.IsTrue(choice.Length > 0);
            
            parser.SetStage("initiator", "question", "greeting");
            choice = parser.GetDialogue();
            Assert.IsTrue(choice.Length > 0);
        }

        [TestMethod]
        public void TestNarrowWeatherResponse() {
            parser.SetStage("initiator", "question", "weather");
            parser.GetDialogue();
            //default game data: evening, nice day
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> test =
                parser.NarrowWeatherResponses();
            Assert.IsTrue(test.Count == 4);
            Assert.IsTrue(test["positive"].ContainsKey("It's always a nice_day when I'm in good company."));
            Assert.IsTrue(test["positive"].Count==1);
            Assert.IsTrue(test["neutral"].Count == 14);
            Assert.IsFalse(test["neutral"].ContainsKey("I know, it's so hot out."));
            Assert.IsTrue(test["neutral"].ContainsKey("Yea, it is."));
            Assert.IsTrue(test["negative"].Count == 5);
            Assert.IsFalse(test["negative"].ContainsKey("Uhuh, hot."));
            Assert.IsTrue(test["negative"].ContainsKey("Thanks for the weather report."));
            Assert.IsTrue(test["negative+"].Count == 11);
            Assert.IsFalse(test["negative+"].ContainsKey("... hot, whatever."));
            Assert.IsTrue(test["negative+"].ContainsKey("Piss Off."));
        }

        [TestMethod]
        public void TestResponseKeyRules1() {
            for (int i = 0; i < 100; i++) {
                List<string> list = new List<string>() { "positive", "neutral", "negative", "negative+" };
                var keys = parser.ResponseKeyRules1(list);
                Assert.IsTrue(keys.Count == 4);
                Assert.IsTrue(keys[0].Equals("positive"));
                Assert.IsTrue(keys[1].Equals("neutral"));
                Assert.IsTrue(keys[2].Equals("negative") || keys[2].Equals("negative+"));
            }
            for (int i = 0; i < 100; i++) {
                List<string> list = new List<string>() { "positive", "positive+", "neutral", "negative", "negative+" };
                var keys = parser.ResponseKeyRules1(list);
                Assert.IsTrue(keys.Count == 4);
                Assert.IsTrue(keys[0].Equals("positive")|| keys[0].Equals("positive+"));
                Assert.IsTrue(keys[1].Equals("neutral"));
                Assert.IsTrue(keys[2].Equals("negative") || keys[2].Equals("negative+"));
            }
            for (int i = 0; i < 100; i++) {
                List<string> list = new List<string>() { "positive", "positive+", "neutral" };
                var keys = parser.ResponseKeyRules1(list);
                Assert.IsTrue(keys.Count == 4);
                Assert.IsTrue(keys[0].Equals("positive") || keys[0].Equals("positive+"));
                Assert.IsTrue(keys[1].Equals("neutral"));
                Assert.IsTrue(keys[2].Equals("neutral"));
            }
        }

        [TestMethod]
        public void TestReturnFourDialogueBranches() {
            for (int i = 0; i < 25; i++) {
                parser.SetStage("initiator", "question", "weather");
                parser.GetDialogue();
                //default game data: evening, nice day
                Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp =
                    parser.NarrowWeatherResponses();
                var test = parser.ReturnFourChoiceBranches(temp);
                Assert.IsTrue(test.Count == 4);
                Assert.IsTrue(test["positive"].ContainsKey("It's always a nice_day when I'm in good company."));
                Assert.IsTrue(test["positive"].Count == 1);
                Assert.IsTrue(test["neutral"].Count == 14);
                Assert.IsFalse(test["neutral"].ContainsKey("I know, it's so hot out."));
                Assert.IsTrue(test["neutral"].ContainsKey("Yea, it is."));
                Assert.IsTrue(test.ContainsKey("negative") || test.ContainsKey("negative+"));
                if (test["negative"].Count == 5) {
                    Assert.IsTrue(test["negative"].Count == 5);
                    Assert.IsFalse(test["negative"].ContainsKey("Uhuh, hot."));
                    Assert.IsTrue(test["negative"].ContainsKey("Thanks for the weather report."));
                } else {
                    Assert.IsTrue(test["negative"].Count == 11);
                    Assert.IsFalse(test["negative"].ContainsKey("... hot, whatever."));
                    Assert.IsTrue(test["negative"].ContainsKey("Piss Off."));
                }
                Assert.IsTrue(test["random"].Count > 0);
            }
        }

        [TestMethod]
        public void TestGetResponse() {
            for (int j = 0; j < 20; j++) {
                parser.SetStage("initiator", "question", "weather");
                parser.GetDialogue();
                //default game data: evening, nice day
                Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp =
                    parser.NarrowWeatherResponses();
                var test = parser.ReturnFourChoiceBranches(temp);
                List<List<string>> responses = parser.GetResponse(test);
                Assert.IsTrue(responses.Count == 4);
                for (int i = 0; i < responses[0].Count; i++) {
                    Assert.IsTrue(test["positive"].ContainsKey(responses[0][i]));
                }
                for (int i = 0; i < responses[1].Count; i++) {
                    Assert.IsTrue(test["neutral"].ContainsKey(responses[1][i]));
                }
                for (int i = 0; i < responses[2].Count; i++) {
                    Assert.IsTrue(test["negative"].ContainsKey(responses[2][i]));
                }
                for (int i = 0; i < responses[3].Count; i++) {
                    Assert.IsTrue(test["random"].ContainsKey(responses[3][i]));
                }
                Assert.IsTrue(responses[0].Count > 0);
                Assert.IsTrue(responses[1].Count > 0);
                Assert.IsTrue(responses[2].Count > 0);
                Assert.IsTrue(responses[3].Count > 0);
            }
        }

        [TestMethod]
        public void TestResponseQualityControl() {
            for (int i = 0; i < 100; i++) {
                parser.SetStage("initiator", "question", "weather");
                parser.GetDialogue();
                //default game data: evening, nice day
                Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp =
                    parser.NarrowWeatherResponses();
                var testB = parser.ReturnFourChoiceBranches(temp);
                List<List<string>> responses = parser.GetResponse(testB);
                List<string> test = parser.ResponseQualityControl(responses);
                Assert.IsTrue(test.Count == 4);
                Assert.IsTrue(responses[0].Contains(test[0]));
                Assert.IsTrue(responses[1].Contains(test[1]));
                Assert.IsTrue(responses[2].Contains(test[2]));
                Assert.IsTrue(test[3].Length > 0);
            }
        }

        [TestMethod]
        public void TestBuildResponseStructure() {
            for (int i = 0; i < 50; i++) {
                parser.SetStage("initiator", "question", "weather");
                parser.GetDialogue();
                //default game data: evening, nice day
                Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp =
                    parser.NarrowWeatherResponses();
                var testB = parser.ReturnFourChoiceBranches(temp);
                List<List<string>> responses = parser.GetResponse(testB);
                List<string> testC = parser.ResponseQualityControl(responses);
                Dictionary<string, List<string>> test = parser.BuildResponseStructure(testB, testC);
                Assert.IsTrue(test.Count >= 2);
                Assert.IsTrue(test["It's always a nice_day when I'm in good company."][0].Equals("friend.+"));
            }
        }

        [TestMethod]
        public void TestNarrowEventResponses() {
            parser.SetStage("initiator", "question", "event");
            parser.ResponseType = "likeCurrentEvent";
            var copy = parser.ModuleDictDeepCopy(module.eventResponse);
            var responses = parser.NarrowEventResponses(copy);
            Assert.IsTrue(responses.Count == 5);
            Assert.IsTrue(responses["positive+"].Count == 4);
            Assert.IsTrue(responses["positive"].Count == 4);
            Assert.IsTrue(responses["neutral"].Count == 4);
            Assert.IsTrue(responses["negative"].Count == 4);
            Assert.IsTrue(responses["negative+"].Count == 4);
            Assert.IsTrue(responses["positive+"].ContainsKey("I love the #current_event#."));
            Assert.IsFalse(responses["positive+"].ContainsKey("You better believe I'm going to the #current_event#."));
            Assert.IsTrue(responses["positive"].ContainsKey("I like the #current_event#. It's fun."));
            Assert.IsFalse(responses["positive"].ContainsKey("Oh yeah I'm going.  I go every year."));
            Assert.IsTrue(responses["neutral"].ContainsKey("I don't know, the #current_event# is okay I guess."));
            Assert.IsFalse(responses["neutral"].ContainsKey("I might stop by."));
            Assert.IsTrue(responses["negative"].ContainsKey("The #current_event# is kind of boring."));
            Assert.IsFalse(responses["negative"].ContainsKey("Don't count on it."));
            Assert.IsTrue(responses["negative+"].ContainsKey("I hate the #current_event#."));

            parser.ResponseType = "goingToCurrentEvent";
            copy = parser.ModuleDictDeepCopy(module.eventResponse);
            responses = parser.NarrowEventResponses(copy);
            Assert.IsTrue(responses.Count == 5);
            Assert.IsTrue(responses["positive+"].Count == 3);
            Assert.IsTrue(responses["positive"].Count == 3);
            Assert.IsTrue(responses["neutral"].Count == 2);
            Assert.IsTrue(responses["negative"].Count == 6);
            Assert.IsTrue(responses["negative+"].Count == 4);
        }

        [TestMethod]
        public void TestReturnFourChoiceBranchEvent() {
            parser.SetStage("initiator", "question", "event");
            parser.GetDialogue();
            parser.ResponseType = "likeCurrentEvent";
            //default game data: evening, nice day
            var copy = parser.ModuleDictDeepCopy(module.eventResponse);
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp =
                parser.NarrowEventResponses(copy);
            var test = parser.ReturnFourChoiceBranches(temp);
            Assert.IsTrue(test.Count == 4);
            Assert.IsTrue(test.ContainsKey("positive"));
            Assert.IsTrue(test["positive"].Count==4);
            Assert.IsTrue(test.ContainsKey("neutral"));
            Assert.IsTrue(test["neutral"].Count==4);
            Assert.IsTrue(test.ContainsKey("negative"));
            Assert.IsTrue(test["negative"].Count==4);
            Assert.IsTrue(test.ContainsKey("random"));
            Assert.IsTrue(test["random"].Count==4);

            parser.ResponseType = "goingToCurrentEvent";
            //default game data: evening, nice day
            copy = parser.ModuleDictDeepCopy(module.eventResponse);
            temp = parser.NarrowEventResponses(copy);
            test = parser.ReturnFourChoiceBranches(temp);
            Assert.IsTrue(test.Count == 4);
            Assert.IsTrue(test.ContainsKey("positive"));
            Assert.IsTrue(test["positive"].Count == 3);
            Assert.IsTrue(test.ContainsKey("neutral"));
            Assert.IsTrue(test["neutral"].Count == 2);
            Assert.IsTrue(test.ContainsKey("negative"));
            Assert.IsTrue(test["negative"].Count == 6 || test["negative"].Count == 4);
            Assert.IsTrue(test.ContainsKey("random"));
            Assert.IsTrue(test["random"].Count >0);
        }

        [TestMethod]
        public void TestGetResponseEvent() {
            parser.SetStage("initiator", "question", "event");
            parser.GetDialogue();
            parser.ResponseType = "likeCurrentEvent";
            //default game data: evening, nice day
            var copy = parser.ModuleDictDeepCopy(module.eventResponse);
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> temp =
                parser.NarrowEventResponses(copy);
            temp = parser.ReturnFourChoiceBranches(temp);
            var test = parser.GetResponse(temp);
            Assert.IsTrue(test.Count == 4);
            Assert.IsTrue(test[0].Count == 4);
            Assert.IsTrue(test[1].Count == 4);
            Assert.IsTrue(test[2].Count == 4);
            Assert.IsTrue(test[3].Count == 4);

            parser.ResponseType = "goingToCurrentEvent";
            //default game data: evening, nice day
            copy = parser.ModuleDictDeepCopy(module.eventResponse);
            temp = parser.NarrowEventResponses(copy);
            temp = parser.ReturnFourChoiceBranches(temp);
            test = parser.GetResponse(temp);
            Assert.IsTrue(test.Count == 4);
            Assert.IsTrue(test[0].Count == 3);
            Assert.IsTrue(test[1].Count == 2);
            Assert.IsTrue(test[2].Count == 6|| test[2].Count == 4);
            Assert.IsTrue(test[3].Count> 0);
        }

        [TestMethod]
        public void TestParseEventResponse() {
            parser.ResponseType = "goingToCurrentEvent";// "likeCurrentEvent";
            var test = parser.ParseEventResponse();
            Assert.IsTrue(test.Count==4);
        }

        [TestMethod]
        public void TestParseGreetingResponse() {
            parser.SetStage("initiator", "question", "greeting");
            parser.GetDialogue();
            var test = parser.ParseGreetingResponse();
            Assert.IsTrue(test.Count==4);
        }

        [TestMethod]
        public void TestParseResponse() {
            for (int i = 0; i < 10000; i++) {
                parser.SetStage("initiator", "question", "event");
                parser.ResponseType = "goingToCurrentEvent";
                var dialogue = parser.ParseResponse();
                Assert.IsTrue(dialogue.Count >= 3);
                parser.SetStage("initiator", "question", "greeting");
                dialogue = parser.ParseResponse();
                Assert.IsTrue(dialogue.Count >= 3);
                parser.SetStage("initiator", "question", "weather");
                dialogue = parser.ParseResponse();
                Assert.IsTrue(dialogue.Count == 4);
            }
        }

        [TestMethod]
        public void TestParserDeepCopy() {
            var copy = parser.ModuleDictDeepCopy(module.eventResponse);
            var verbose = module.eventResponse;
            Assert.IsTrue(copy.Count==verbose.Count);
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> item in verbose) {
                Assert.IsTrue(copy.ContainsKey(item.Key));
                Assert.IsTrue(copy[item.Key].Count == verbose[item.Key].Count);
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> inner in verbose[item.Key]) {
                    Assert.IsTrue(copy[item.Key].ContainsKey(inner.Key));
                    Assert.IsTrue(copy[item.Key][inner.Key].Count == verbose[item.Key][inner.Key].Count);
                    if (verbose[item.Key][inner.Key]["req"].Count > 0)
                        Assert.IsTrue(copy[item.Key][inner.Key]["req"][0].Equals(verbose[item.Key][inner.Key]["req"][0]));
                }
            }
        }

        [TestMethod]
        public void TestQuestionToResponseTransition() {
            parser.SetStage("initiator", "question", "event");
            string question = parser.GetDialogue();
            parser.SetStage("responder", "response", "event");
            var response = parser.ParseResponse();
            Assert.IsTrue(response.Count >= 3);


        }
    }
}
