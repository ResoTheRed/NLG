using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kati.Data_Models;
using System;
using System.Collections.Generic;
using System.Text;
using Kati.Module_Hub;

namespace KatiUnitTest.Module_Tests
{
    [TestClass()]
    public class SmallTalkControllerTester{

        private SmallTalk_Module module;
        private SmallTalk_Controller controller;
        private SmallTalk_Loader loader;

        [TestInitialize]
        public void Start() {
            module = new SmallTalk_Module(Kati.SourceFiles.Constants.smallTalk);
            loader = new SmallTalk_Loader(module);
            controller = new SmallTalk_Controller(module);
            controller._GameData = SetupGameData();
            controller._CharacterData = SetupCharacterData();
            controller.SetupConversation(controller._GameData,controller._CharacterData);
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
            data.InitiatorAttributeList["loves_Art_Fest"] = "characterTrait";
            data.ResponderAttributeList["loves_Art_Fest"] = "characterTrait";
            data.InitiatorScalarList["charming"] = 200;
            data.ResponderScalarList["charming"] = 200;
            return data;
        }

        public Dictionary<string, double> SetTone() {
            Random dice = new Random();
            string[] str = { "romance", "friends","professional","respect",
                             "Admiration","disgust","hate","rivalry"};
            Dictionary<string, double> temp = new Dictionary<string, double>();
            for (int i = 0; i < str.Length; i++)
                temp[str[i]] = dice.NextDouble();
            return temp;
        }

        [TestMethod]
        public void TestConstructor() {
            controller = new SmallTalk_Controller(module);
        }

        [TestMethod]
        public void TestGameData() {
            Assert.IsNotNull(controller._GameData);
        }

        [TestMethod]
        public void TestCharacterData() {
            Assert.IsNotNull(controller._CharacterData);
        }

        [TestMethod]
        public void TestSetupConversation() {
            controller.SetupConversation(controller._GameData,
                controller._CharacterData);
            Assert.IsTrue(controller.Conversation["initiator"][0].Equals("Kati"));
            Assert.IsTrue(controller.Conversation["responder"][0].Equals("Stephen"));
        }

        [TestMethod]
        public void TestSelectStartingTopic() {
            int option = controller.SelectStartingTopic();
            if (option <= 5) {
                Assert.IsTrue(controller.Topic.Equals("greeting"));
            } else if (option <= 8) {
                Assert.IsTrue(controller.Topic.Equals("event"));
            } else {
                Assert.IsTrue(controller.Topic.Equals("weather"));
            }
            Assert.IsNotNull(controller.DialogueState);
            Assert.IsTrue(controller.Speaking.Equals("responder"));
            Assert.IsTrue(controller.Dialogue.Length > 2);
            Assert.IsTrue(controller.ConversationTopicsDiscussed["event"] ||
                            controller.ConversationTopicsDiscussed["weather"] ||
                            controller.ConversationTopicsDiscussed["greeting"]);
            Assert.IsTrue(controller.Conversation["initiator"][0].Equals("Kati"));
            Assert.IsTrue(controller.Conversation["initiator"].Count==2);
        }

        [TestMethod]
        public void TestRunFirstRound() {
            controller.RunFirstRound();
            Assert.IsNotNull(controller.Dialogue);
            Assert.IsTrue(controller.Conversation["initiator"].Count == 2);
            Assert.IsTrue(controller.Conversation["responder"].Count == 2);
            Assert.IsTrue(controller.Speaking.Equals("initiator"));
        }

        [TestMethod]
        public void TestRunNextRound() {
            controller.RunFirstRound();
            int prob = controller.SelectNextTopic();
            Assert.IsTrue((prob > 8) ? !controller.EndConversation : controller.EndConversation);
            Assert.IsTrue((prob > 8) ? !controller.Speaking.Equals("responder") :
                controller.Speaking.Equals("initiator"));
            Assert.IsTrue((prob > 8) ? controller.Conversation["initiator"].Count==3:
                controller.Conversation["initiator"].Count==2);
        }

        [TestMethod]
        public void TestConverse() {
            Assert.IsFalse(controller.EndConversation);
            controller.Converse();
            Assert.IsTrue(controller.EndConversation);
        }

    }
}
