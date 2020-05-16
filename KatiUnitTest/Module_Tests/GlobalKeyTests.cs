using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kati.Data_Modules;
using System;

namespace KatiUnitTest.Module_Tests{
    
    [TestClass]
    public class GlobalKeyTests{

        private GlobalKeys global;

        [TestInitialize]
        public void Start() {
            try {
                global = new GlobalKeys();
            } catch (Exception) {}
        }

        [TestMethod]
        public void TestGlobalConstructor() {
            Assert.IsNotNull(global);
        }

        [TestMethod]
        public void TestPathsPronoun() {
            Assert.IsNotNull(global.paths);
            Assert.IsTrue(global.paths.ContainsKey("pronoun"));
            if(global.paths.ContainsKey("pronoun"))
                Assert.AreEqual(global.paths["pronoun"],Kati.SourceFiles.Constants.pronoun);
        }

    }

}
