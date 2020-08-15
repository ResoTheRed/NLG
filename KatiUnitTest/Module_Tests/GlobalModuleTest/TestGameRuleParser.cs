using Kati.Data_Modules.GlobalClasses;
using Kati.Module_Hub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

using System.Text;

namespace KatiUnitTest.Module_Tests.GlobalModuleTest {

    [TestClass]
    public class TestGameRuleParser {

        Controller ctrl;
        GameRules game;
        Dictionary<string, Dictionary<string, List<string>>> data;

        [TestInitialize]
        public void Start() {
            ctrl = new Controller("C:/Users/User/Documents/NLG/KatiUnitTest/Module_Tests/GlobalModuleTest/RuleTester.json");
            SetGameData(ctrl.Game);
            data = ctrl.Lib.Data["sample1_statement"]["neutral"];
            game = ctrl.Parser.Game;
        }

        public void SetGameData(GameData g) {
            g.Weather = "nice_day";
            g.Sector = "5";
            g.TimeOfDay = "morning";
            g.Season = "spring";
            g.DayOfWeek = 1;
            g.DayOfMonth = 8;
        }

        [TestMethod]
        public void TestSetup() {
            Assert.IsNotNull(ctrl);
            Assert.IsNotNull(ctrl.Game);
            Assert.IsNotNull(ctrl.Parser);
            Assert.IsNotNull(game.Ctrl.Game);
        }

        [TestMethod]
        public void TestRuleDirector1() {
            SetGameData(ctrl.Game);
            string[] arr = { GameRules.WEATHER, "nice_day" };
            bool remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.WEATHER, "hot" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.WEATHER, "albatruas" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.WEATHER, "" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.WEATHER, null };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

            arr = new string[]{ GameRules.SECTOR, "5" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.SECTOR, "9" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.SECTOR, "Hunk of Meat" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.SECTOR, "" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.SECTOR, null };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);


        }

        [TestMethod]
        public void TestRuleDirectory2() {
            string[] arr = new string[] { GameRules.SEASON, "spring" };
            bool remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.SEASON, "fall" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.SEASON, "" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.SEASON, null };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

            arr = new string[] { GameRules.DAY_OF_WEEK, "mon" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.DAY_OF_WEEK, "fri" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.DAY_OF_WEEK, "" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.DAY_OF_WEEK, null };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
        }

        [TestMethod]
        public void TestRuleDirectory3() {
            string[] arr = new string[] { GameRules.PUBLIC_EVENT, "next" };
            bool remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.PUBLIC_EVENT, "art_fest" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.PUBLIC_EVENT, "Alberto Brenchki" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.PUBLIC_EVENT, "" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.PUBLIC_EVENT, null };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

            arr = new string[] { GameRules.TIME_OF_DAY, "morning" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.TIME_OF_DAY, "evening" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.TIME_OF_DAY, "" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
            arr = new string[] { GameRules.TIME_OF_DAY, null };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);
        }

        [TestMethod]
        public void TestRuleDirectory4() { 
            
        }

        [TestMethod]
        public void TestRemoveElement1() {
            string s =  "game.weather.nice_day";
            bool removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = "game.sector.5";
            removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = "game.season.spring";
            removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = "game.day.mon";
            removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = "game.time.morning";
            removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = "game.publicEvent.next";
            removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = "game.publicEvent.art_fest";
            removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
        }

        [TestMethod]
        public void TestRemoveElement2() {
            string s = "game.weather.hot";
            bool removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.sector.7";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.season.june";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.day.hairy";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.day.fri";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.time.blandy";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.publicEvent.near";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.publicEvent.blueberry_fest";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
        }

        [TestMethod]
        public void TestRemoveElement3() {
            string s = "ham.weather.nice_day";
            bool removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = "game";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.season";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "crumb.game.day.mon";
            removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = "";
            removed = game.RemoveElement(s);
            Assert.IsFalse(removed);
            s = null;
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
            s = "game.l.l.l.p.o.p.t.r.j.k.k.l.o";
            removed = game.RemoveElement(s);
            Assert.IsTrue(removed);
        }

        [TestMethod]
        public void TestParseGameRequirements() {
            var data = game.ParseGameRequirments(ctrl.Lib.Data["sample1_statement"]["neutral"]);
            Assert.IsTrue(data.Count >= 8);
            Assert.IsTrue(data.ContainsKey(" neutral test 0"));
            Assert.IsTrue(data.ContainsKey(" neutral test 1"));
            Assert.IsTrue(data.ContainsKey(" neutral test 4"));
            Assert.IsTrue(data.ContainsKey(" neutral test 5"));
            Assert.IsTrue(data.ContainsKey(" neutral test 7"));
            Assert.IsTrue(data.ContainsKey(" neutral test 8"));
            Assert.IsTrue(data.ContainsKey(" neutral test 9"));
            Assert.IsTrue(data.ContainsKey(" neutral test 10"));

            Assert.IsFalse(data.ContainsKey(" neutral test 2"));
            Assert.IsFalse(data.ContainsKey(" neutral test 3"));
            Assert.IsFalse(data.ContainsKey(" neutral test 6"));
            Assert.IsFalse(data.ContainsKey(" neutral test 11"));
            Assert.IsFalse(data.ContainsKey(" neutral test 12"));
        }
    }
}
