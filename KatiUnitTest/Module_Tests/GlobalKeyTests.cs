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
        public void TestPronounDataKeywordPron1() {
            
            Assert.IsTrue(global.pronoun.ContainsKey("pron1"));
            Assert.IsTrue(global.pronoun["pron1"].ContainsKey("he"));
            Assert.AreEqual("male",global.pronoun["pron1"]["he"][0]);
            Assert.AreEqual("female",global.pronoun["pron1"]["she"][0]);
        }

        [TestMethod]
        public void TestPronounDataKeywordPron2() {
            Assert.IsTrue(global.pronoun.ContainsKey("pron2"));
            Assert.AreEqual("male",global.pronoun["pron2"]["him"][0]);
            Assert.AreEqual("female",global.pronoun["pron2"]["her"][0]);
            Assert.AreEqual("male",global.pronoun["pron2"]["his"][0]);
            Assert.AreEqual("female",global.pronoun["pron2"]["hers"][0]);
            Assert.AreEqual("possessive",global.pronoun["pron2"]["his"][1]);
            Assert.AreEqual("possessive", global.pronoun["pron2"]["hers"][1]);
        }

        [TestMethod]
        public void TestPronounDataKeywordPron3() {
            Assert.IsTrue(global.pronoun.ContainsKey("pron3"));
            Assert.AreEqual("male", global.pronoun["pron3"]["himself"][0]);
            Assert.AreEqual("female", global.pronoun["pron3"]["herself"][0]);
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
