using Kati.Module_Hub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KatiUnitTest.Module_Tests{

    /// <summary>
    /// Test GameData.cs in Module_Hub directory
    /// </summary>
    [TestClass()]
    public class GameDataTester{

        private GameData gameData;

        [TestInitialize]
        public void Start() {
            try {
                gameData = new GameData();
            } catch (Exception) { }
        }

        [TestMethod]
        public void TestConstructor() {
            Assert.IsNotNull(gameData);
            Assert.IsNull(gameData.Season);
            gameData = new GameData("nice_day", "Sector 3", "afternoon", 3, "Fall");
            Assert.IsTrue(gameData.Season.Equals("Fall"));
            Assert.IsTrue(gameData.Weather.Equals("nice_day"));
            Assert.IsTrue(gameData.Sector.Equals("Sector 3"));
            Assert.IsTrue(gameData.TimeOfDay.Equals("afternoon"));
            Assert.IsTrue(gameData.DayOfMonth.Equals(3));
        }

        [TestMethod]
        public void TestWeatherString() {
            gameData.Weather = "nice_day";
            Assert.IsTrue(gameData.Weather.Equals("nice_day"));
        }

        [TestMethod]
        public void TestSectorString() {
            gameData.Sector = "Sector 1";
            Assert.IsTrue(gameData.Sector.Equals("Sector 1"));
        }

        [TestMethod]
        public void TestTimeOfDayString() {
            gameData.TimeOfDay = "afternoon";
            Assert.IsTrue(gameData.TimeOfDay.Equals("afternoon"));
        }

        [TestMethod]
        public void TestDayOfMonthInt() {
            gameData.DayOfMonth = 20;
            Assert.IsTrue(gameData.DayOfMonth == 20);
        }

        [TestMethod]
        public void TestWeekInt() {
            gameData.DayOfMonth = 20;
            gameData.SetWeek();
            Assert.IsTrue(gameData.Week == 3);
            gameData.DayOfMonth = 2;
            gameData.SetWeek();
            Assert.IsTrue(gameData.Week == 1);
        }

        [TestMethod]
        public void TestDayOfWeekInt() {
            gameData.DayOfMonth = 20;
            gameData.SetDayOfWeek();
            Assert.IsTrue(gameData.DayOfWeek == 6);
        }

        [TestMethod]
        public void TestSeasonString() {
            gameData.Season = "Fall";
            Assert.IsTrue(gameData.Season.Equals("Fall"));
        }

        [TestMethod]
        public void TestPublicEventString() {
            gameData.Season = "Spring";
            gameData.EventCalendar["Spring"] = new Dictionary<string, int>();
            gameData.EventCalendar["Spring"]["Art_Fest"] = 12;
            gameData.EventCalendar["Spring"]["Blueberry_Fest"] = 21;
            gameData.EventCalendar["Fall"] = new Dictionary<string, int>();
            gameData.EventCalendar["Fall"]["Holloween"] = 28;
            gameData.DayOfMonth = 7;
            gameData.SetPublicEvent();
            Assert.IsTrue(gameData.PublicEvent.Equals("Art_Fest"));
            gameData.DayOfMonth = 12;
            gameData.SetPublicEvent();
            Assert.IsTrue(gameData.PublicEvent.Equals("Art_Fest"));
            gameData.DayOfMonth = 18;
            gameData.SetPublicEvent();
            Assert.IsTrue(gameData.PublicEvent.Equals("Blueberry_Fest"));
            gameData.DayOfMonth = 26;
            gameData.SetPublicEvent();
            Assert.IsTrue(gameData.PublicEvent.Equals("None"));
            gameData.Season = "Fall";
            gameData.SetPublicEvent();
            Assert.IsTrue(gameData.PublicEvent.Equals("Holloween"));
            gameData.Season = "Spring";
            gameData.DayOfMonth = 1;
            gameData.SetPublicEvent();
            Assert.IsTrue(gameData.PublicEvent.Equals("None"));
        }

        [TestMethod]
        public void TestEventCalendar() {
            gameData.EventCalendar["Spring"] = new Dictionary<string, int>();
            gameData.EventCalendar["Spring"]["Art_Fest"] = 12;
            Assert.IsNotNull(gameData.EventCalendar);
            Assert.IsTrue(gameData.EventCalendar["Spring"]["Art_Fest"] == 12);
        }

        [TestMethod]
        public void TestEventIsNearNoParams() {
            GameData gameData = new GameData("nice_day", "Sector 3", "afternoon", 3, "Spring");
            Assert.IsTrue(gameData.EventIsNear().Equals("none"));
            gameData = new GameData("nice_day", "Sector 3", "afternoon", 10, "Spring");
            Assert.IsTrue(gameData.EventIsNear().Equals("art_fest"));
            gameData = new GameData("nice_day", "Sector 3", "afternoon", 13, "Spring");
            Assert.IsTrue(gameData.EventIsNear().Equals("none"));
            gameData = new GameData("nice_day", "Sector 3", "afternoon", 17, "Spring");
            Assert.IsTrue(gameData.EventIsNear().Equals("blueberry_fest"));
            gameData = new GameData("nice_day", "Sector 3", "afternoon", 22, "Spring");
            Assert.IsTrue(gameData.EventIsNear().Equals("none"));
            gameData = new GameData("nice_day", "Sector 3", "afternoon", 17, "Fall");
            Assert.IsTrue(gameData.EventIsNear().Equals("none"));
            gameData = new GameData("nice_day", "Sector 3", "afternoon", 22, "Fall");
            Assert.IsTrue(gameData.EventIsNear().Equals("halloween"));
        }

    }

    [TestClass()]
    public class CharacterDataTester {

        private CharacterData data;
        private readonly string[] stats = { "romance","friends","professional","respect",
            "admiration","disgust","hate","rivalry"};
        private Random dice = new Random();

        [TestInitialize]
        public void Start() {
            data = new CharacterData();
        }
        /*
        [TestMethod]
        public void TestInitiatorNameString() {
            data.InitiatorsName = "Stephen";
            Assert.IsTrue(data.InitiatorsName.Equals("Stephen"));
        }

        [TestMethod]
        public void TestRespondersNameString() {
            data.RespondersName = "Kati";
            Assert.IsTrue(data.RespondersName.Equals("Kati"));
        }

        [TestMethod]
        public void TestInitiatorsGender() {
            data.InitialorsGender = "Male";
            Assert.IsTrue(data.InitialorsGender.Equals("Male"));
        }

        [TestMethod]
        public void TestRespondersGender() {
            data.RespondersGender = "Female";
            Assert.IsTrue(data.RespondersGender.Equals("Female"));
        }

        [TestMethod]
        public void TestInteracitonTone() {
            Dictionary<string, double> tones = new Dictionary<string, double>();
            for (int i = 0; i < stats.Length; i++) {
                tones[stats[i]] = dice.NextDouble();
            }
            data.InteractionTone = tones;
            for (int i = 0; i < stats.Length; i++) {
                Assert.IsNotNull(data.InteractionTone[stats[i]]);
            }
        }

        [TestMethod]
        public void TestInitiatorsTone() {
            Dictionary<string, double> tones = new Dictionary<string, double>();
            for (int i = 0; i < stats.Length; i++) {
                tones[stats[i]] = dice.NextDouble();
            }
            data.InitiatorsTone = tones;
            for (int i = 0; i < stats.Length; i++) {
                Assert.IsNotNull(data.InitiatorsTone[stats[i]]);
            }
        }

        [TestMethod]
        public void TestRespondersTone() {
            Dictionary<string, double> tones = new Dictionary<string, double>();
            for (int i = 0; i < stats.Length; i++) {
                tones[stats[i]] = dice.NextDouble();
            }
            data.RespondersTone = tones;
            for (int i = 0; i < stats.Length; i++) {
                Assert.IsNotNull(data.RespondersTone[stats[i]]);
            }
        }

        [TestMethod]
        public void TestIniatorsAttributeList() {
            Dictionary<string, string> list = new Dictionary<string, string>();
            list["lucky"] = "characterTrait";
            data.InitiatorPersonalList = list;
            Assert.AreEqual(data.InitiatorPersonalList["lucky"], "characterTrait");
        }

        [TestMethod]
        public void TestRespondersAttributeList() {
            Dictionary<string, string> list = new Dictionary<string, string>();
            list["lucky"] = "characterTrait";
            data.ResponderAttributeList = list;
            Assert.AreEqual(data.ResponderAttributeList["lucky"], "characterTrait");
        }

        [TestMethod]
        public void TestInitiatorsScalarList() {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list["charm"] = 200;
            data.InitiatorScalarList = list;
            Assert.AreEqual(data.InitiatorScalarList["charm"],200);
        }

        [TestMethod]
        public void TestRespondersScalarList() {
            Dictionary<string, int> list = new Dictionary<string, int>();
            list["charm"] = 200;
            data.ResponderScalarList = list;
            Assert.AreEqual(data.ResponderScalarList["charm"], 200);
        }
        */
    }

}
