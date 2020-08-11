using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kati.Data_Modules.GlobalClasses;
using Kati.SourceFiles;
using System.Collections.Generic;

namespace KatiUnitTest.Module_Tests.GlobalModuleTest {
    
    [TestClass]
    public class TestModuleLib {

        private ModuleLib lib;

        [TestInitialize]
        public void Start() {
            lib = new ModuleLib(Constants.DayDreamWonder);
        }

        [TestMethod]

        public void TestLibKeys() {
            Assert.IsTrue(lib.Keys["dream"][lib.STATEMENT].Equals("dream_statement"));
            Assert.IsTrue(lib.Keys["dream"][lib.QUESTION].Equals("dream_question"));
            Assert.IsTrue(lib.Keys["dream"][lib.RESPONSE].Equals("dream_response"));
            Assert.IsTrue(lib.Keys["trivia"][lib.STATEMENT].Equals("trivia_statement"));
            Assert.IsTrue(lib.Keys["trivia"][lib.QUESTION].Equals("trivia_question"));
            Assert.IsTrue(lib.Keys["trivia"][lib.RESPONSE].Equals("trivia_response"));
            Assert.IsTrue(lib.Keys["wonder"][lib.STATEMENT].Equals("wonder_statement"));
            Assert.IsTrue(lib.Keys["wonder"][lib.QUESTION].Equals("wonder_question"));
            Assert.IsTrue(lib.Keys["wonder"][lib.RESPONSE].Equals("wonder_response"));
        }

        [TestMethod]
        public void TestLibData() {
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]].Count == 9);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["neutral"].Count == 6);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["romance"].Count == 8);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["friend"].Count == 14);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["respect"].Count == 1);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["admiration"].Count == 2);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["rivalry"].Count == 5);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["disgust"].Count == 1);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["disgust"].ContainsKey
                ("#art_mean# I had a dream about you last night.  I don't really want to talk about it."));
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["hate"].Count == 2);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.STATEMENT]]["dreamDesc"].Count == 15);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.QUESTION]]["neutral"].Count == 6);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.QUESTION]]["friend"].Count == 6);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.QUESTION]].Count == 2);
            Assert.IsTrue(lib.Data[lib.Keys["dream"][lib.RESPONSE]].Count == 5);
            Assert.IsTrue(lib.Data[lib.Keys["trivia"][lib.STATEMENT]].Count == 8);
            Assert.IsTrue(lib.Data[lib.Keys["trivia"][lib.QUESTION]].Count == 8);
            Assert.IsTrue(lib.Data[lib.Keys["trivia"][lib.RESPONSE]].Count == 5);
            Assert.IsTrue(lib.Data[lib.Keys["wonder"][lib.STATEMENT]].Count == 3);
            Assert.IsTrue(lib.Data[lib.Keys["wonder"][lib.QUESTION]].Count == 1);
            Assert.IsTrue(lib.Data[lib.Keys["wonder"][lib.RESPONSE]].Count == 5);
        }

        [TestMethod]
        public void TestShallowCopy() {
            var d1 = lib.ShallowCopyDictionaryByTopic("trivia",lib.STATEMENT);
            var d2 = lib.ShallowCopyDictionaryByTopic("trivia",lib.STATEMENT);
            Assert.IsTrue(System.Object.ReferenceEquals(d1, d2));
            Assert.IsTrue(System.Object.ReferenceEquals(d2, lib.Data[lib.Keys["trivia"][lib.STATEMENT]]));
        }

        [TestMethod]
        public void TestDeepCopy() {
            var d1 = lib.DeepCopyDictionaryByTopic("dream",lib.STATEMENT);
            var d2 = lib.DeepCopyDictionaryByTopic("dream",lib.STATEMENT);
            var d3 = d1;
            Assert.IsNotNull(d1);
            Assert.IsNotNull(d2);
            Assert.IsNotNull(d3);
            Assert.IsFalse(d1 == d2);
            Assert.IsTrue(d1 == d3);
            Assert.IsFalse(System.Object.ReferenceEquals(d1, d2));
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> item in lib.Data[lib.Keys["dream"][lib.STATEMENT]]) {
                Assert.IsTrue(d1.ContainsKey(item.Key));
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> item2 in lib.Data[lib.Keys["dream"][lib.STATEMENT]][item.Key]) {
                    Assert.IsTrue(d1[item.Key].ContainsKey(item2.Key));
                }
            }
            Assert.IsTrue(d1.Count == lib.Data[lib.Keys["dream"][lib.STATEMENT]].Count);
        }

    }
}
