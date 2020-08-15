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
        public void TestRuleDirector() {
            SetGameData(ctrl.Game);
            string[] arr = { GameRules.WEATHER, "nice_day" };
            bool remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.WEATHER, "hot" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

            arr = new string[]{ GameRules.SECTOR, "5" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.SECTOR, "9" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

            arr = new string[] { GameRules.SEASON, "spring" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.SEASON, "fall" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

            arr = new string[] { GameRules.DAY_OF_WEEK, "mon" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.DAY_OF_WEEK, "fri" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

            arr = new string[] { GameRules.TIME_OF_DAY, "morning" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.TIME_OF_DAY, "evening" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

            arr = new string[] { GameRules.PUBLIC_EVENT, "near" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.PUBLIC_EVENT, "art_fest" };
            remove = game.RuleDirectory(arr);
            Assert.IsFalse(remove);
            arr = new string[] { GameRules.PUBLIC_EVENT, "gurg" };
            remove = game.RuleDirectory(arr);
            Assert.IsTrue(remove);

        }

    }
}
