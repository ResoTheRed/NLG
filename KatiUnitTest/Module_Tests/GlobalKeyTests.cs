using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kati.Data_Modules;
using System;
using System.Collections.Generic;

namespace KatiUnitTest.Module_Tests{
    
    [TestClass]
    public class GlobalKeyTests{

        private GlobalKeys global;

        [TestInitialize]
        public void Start() {
            try {
                this.global = new GlobalKeys();
            } catch (Exception) {}
        }

        [TestMethod]
        public void TestGlobalConstructor() {
            Assert.IsNotNull(this.global);
            Assert.IsInstanceOfType(global,typeof(GlobalKeys));
        }

        [TestMethod]
        public void TestPathsPronoun() {
            Assert.IsNotNull(this.global.paths);
            Assert.IsTrue(this.global.paths.ContainsKey("pronoun"));
            Assert.AreEqual(global.paths["pronoun"],Kati.SourceFiles.Constants.pronoun);
        }

        [TestMethod]
        public void TestForGlobalKeysJsonFile() {
            string json = global.ReadFile(Kati.SourceFiles.Constants.pronoun);
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Length>1);
        }

        [TestMethod]
        public void TestPronounDataStructure() {
            Assert.IsNotNull(global.pronoun);
        }

        [TestMethod]
        public void TestPronounDataKeywordPronoun() {
            Assert.IsTrue(global.pronoun.ContainsKey("pronoun"));
            Assert.IsTrue(global.pronoun["pronoun"].ContainsKey("male"));
            Assert.IsTrue(global.pronoun["pronoun"].ContainsKey("female"));
        }

        [TestMethod]
        public void TestPronounDataKeywordPron2() {
            Assert.AreEqual("he", global.pronoun["pronoun"]["male"][0]);
            Assert.AreEqual("she", global.pronoun["pronoun"]["female"][0]);
            Assert.AreEqual("him", global.pronoun["pronoun"]["male"][1]);
            Assert.AreEqual("her", global.pronoun["pronoun"]["female"][1]);
            Assert.AreEqual("his", global.pronoun["pronoun"]["male"][2]);
            Assert.AreEqual("hers", global.pronoun["pronoun"]["female"][2]);

        }

        [TestMethod]
        public void TestPronounDataKeywordPron3() {
            Assert.AreEqual("himself", global.pronoun["pronoun"]["male"][3]);
            Assert.AreEqual("herself", global.pronoun["pronoun"]["female"][3]);
            Assert.AreEqual("mr.", global.pronoun["pronoun"]["male"][4]);
            Assert.AreEqual("ms.", global.pronoun["pronoun"]["female"][4]);
            Assert.AreEqual("mr.", global.pronoun["pronoun"]["male"][5]);
            Assert.AreEqual("mrs.", global.pronoun["pronoun"]["female"][5]);
            Assert.AreEqual("sir", global.pronoun["pronoun"]["male"][6]);
            Assert.AreEqual("ma'am", global.pronoun["pronoun"]["female"][6]);
        }

        [TestMethod]
        public void TestPronounMethod() {
            Dictionary<string, Dictionary<string, List<string>>> pron = global.Pronoun();
            Assert.IsNotNull(pron);
        }

        [TestMethod]
        public void TestLoadPronoun() {
            Dictionary<string, Dictionary<string, List<string>>> pron = 
                global.LoadPronouns(Kati.SourceFiles.Constants.pronoun);
            Assert.IsNotNull(pron);
        }

    }

}
