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
            module = new SmallTalk_Module("C:/Users/User/Documents/NLG/KatiUnitTest/Module_Tests/SmallTalk/smallTalk.json");
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
            data.InitiatorPersonalList["loves_Art_Fest"] = "characterTrait";
            data.ResponderAttributeList["loves_Art_Fest"] = "characterTrait";
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
        public void TestRunNextRound() {
            for (int i = 0; i < 1000; i++) {
                controller.EndConversation = false;
                int option = (int)(controller.Dice.NextDouble()*3+1);
                controller.DialogueState = (option == 1) ? SmallTalk_Controller.STATEMENT :
                    (option == 2) ? SmallTalk_Controller.QUESTION : SmallTalk_Controller.RESPONSE;
                controller.ConversationTopicsDiscussed["event"] = false;
                controller.ConversationTopicsDiscussed["weather"] = false;
                controller.ConversationTopicsDiscussed["greeting"] = false;
                ModuleDialoguePackage mod = controller.RunNextRound();
                if (controller.DialogueState == SmallTalk_Controller.RESPONSE) {
                    Assert.IsNotNull(mod.DialogueAndEffects);
                    Assert.IsTrue(mod.Status == ModuleStatus.CONTINUE);
                }
                if (controller.DialogueState == SmallTalk_Controller.QUESTION) {
                    Assert.IsNotNull(mod.DialogueAndEffects);
                    Assert.IsTrue(mod.Status == ModuleStatus.RETURN);
                }
                    

            }
        }

        [TestMethod]
        public void TestRunFirstRound() {
            controller.ConversationTopicsDiscussed["weather"] = false;
            controller.ConversationTopicsDiscussed["event"] = false;
            controller.ConversationTopicsDiscussed["greeting"] = false;
            for (int i = 0; i < 1000; i++) {
                ModuleDialoguePackage data = controller.RunFirstRound();
                Assert.IsTrue(data.DialogueAndEffects.Count == 2);
                if (i < 3)
                    Assert.IsFalse(controller.EndConversation);
                else {//can only run a max of three times 
                    Assert.IsTrue(controller.EndConversation);
                    Assert.IsTrue(data.Status == ModuleStatus.EXIT);
                }
                if (data.Status == ModuleStatus.CONTINUE) {
                    Assert.IsTrue(data.DialogueAndEffects["setStage"][1].Equals(SmallTalk_Controller.STATEMENT));
                } else if (data.Status == ModuleStatus.RETURN) {
                    Assert.IsTrue(data.DialogueAndEffects["setStage"][1].Equals(SmallTalk_Controller.QUESTION));
                }
            }

        }

        [TestMethod]
        public void TestGetReturnStatus() {
            controller.EndConversation = true;
            ModuleStatus s = controller.GetReturnStatus();
            Assert.IsTrue(s == ModuleStatus.EXIT);
            controller.EndConversation = false;
            controller.DialogueState = SmallTalk_Controller.QUESTION;
            s = controller.GetReturnStatus();
            Assert.IsTrue(s == ModuleStatus.RETURN);
            controller.DialogueState = SmallTalk_Controller.STATEMENT;
            s = controller.GetReturnStatus();
            Assert.IsTrue(s == ModuleStatus.CONTINUE);
        }

        [TestMethod]
        public void TestSelectTopic() {
            controller.ConversationTopicsDiscussed["event"] = true;
            controller.ConversationTopicsDiscussed["greeting"] = true;
            controller.ConversationTopicsDiscussed["weather"] = false;
            bool value = controller.SelectTopic(1);
            Assert.IsFalse(value);
            Assert.IsTrue(controller.Topic.Equals("weather"));
            Assert.IsTrue(controller.ConversationTopicsDiscussed["weather"]);
            controller.ConversationTopicsDiscussed["event"] = true;
            controller.ConversationTopicsDiscussed["greeting"] = false;
            controller.ConversationTopicsDiscussed["weather"] = true;
            value = controller.SelectTopic(1);
            Assert.IsFalse(value);
            Assert.IsTrue(controller.Topic.Equals("greeting"));
            Assert.IsTrue(controller.ConversationTopicsDiscussed["greeting"]);
            controller.ConversationTopicsDiscussed["event"] = false;
            controller.ConversationTopicsDiscussed["greeting"] = true;
            controller.ConversationTopicsDiscussed["weather"] = true;
            value = controller.SelectTopic(1);
            Assert.IsFalse(value);
            Assert.IsTrue(controller.Topic.Equals("event"));
            Assert.IsTrue(controller.ConversationTopicsDiscussed["event"]);
            controller.ConversationTopicsDiscussed["event"] = false;
            controller.ConversationTopicsDiscussed["greeting"] = true;
            controller.ConversationTopicsDiscussed["weather"] = true;
            value = controller.SelectTopic(9);
            Assert.IsTrue(value);
            controller.ConversationTopicsDiscussed["event"] = true;
            controller.ConversationTopicsDiscussed["greeting"] = true;
            controller.ConversationTopicsDiscussed["weather"] = true;
            value = controller.SelectTopic(1);
            Assert.IsFalse(value);
            Assert.IsTrue(controller.EndConversation);
        }

        
        [TestMethod]
        public void TestCallPullMethodWeather() {
            controller.Speaking = SmallTalk_Controller.INITIATOR;
            controller.Topic = SmallTalk_Controller.WEATHER;
            controller.DialogueState = SmallTalk_Controller.STATEMENT;
            var dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count == 2);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.STATEMENT));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.WEATHER));

            controller.DialogueState = SmallTalk_Controller.QUESTION;
            dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count == 2);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.QUESTION));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.WEATHER));
            
            controller.Speaking = SmallTalk_Controller.RESPONDER;
            controller.DialogueState = SmallTalk_Controller.RESPONSE;
            dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count >= 4);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.RESPONSE));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.WEATHER));

        }

        [TestMethod]
        public void TestCallPullMethodEvent() {
            controller.Speaking = SmallTalk_Controller.INITIATOR;
            controller.Topic = SmallTalk_Controller.EVENT;
            controller.DialogueState = SmallTalk_Controller.STATEMENT;
            var dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count == 2);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.STATEMENT));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.EVENT));

            controller.DialogueState = SmallTalk_Controller.QUESTION;
            dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count == 2);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.QUESTION));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.EVENT));

            controller.Speaking = SmallTalk_Controller.RESPONDER;
            controller.DialogueState = SmallTalk_Controller.RESPONSE;
            dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count >= 4);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.RESPONSE));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.EVENT));
        }

        [TestMethod]
        public void TestCallPullMethodGreeting() {
            controller.Speaking = SmallTalk_Controller.INITIATOR;
            controller.Topic = SmallTalk_Controller.GREETING;
            controller.DialogueState = SmallTalk_Controller.STATEMENT;
            var dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count == 2);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.STATEMENT));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.GREETING));

            controller.DialogueState = SmallTalk_Controller.QUESTION;
            dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count == 2);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.QUESTION));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.GREETING));

            controller.Speaking = SmallTalk_Controller.RESPONDER;
            controller.DialogueState = SmallTalk_Controller.RESPONSE;
            dialogue = controller.CallPullMethod();
            Assert.IsTrue(dialogue.Count >= 4);
            Assert.IsTrue(dialogue["setStage"].Count == 3);
            Assert.IsTrue(dialogue["setStage"][1].Equals(SmallTalk_Controller.RESPONSE));
            Assert.IsTrue(dialogue["setStage"][2].Equals(SmallTalk_Controller.GREETING));
        }

    }
}
