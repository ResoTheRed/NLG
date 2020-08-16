using Kati.Data_Modules.GlobalClasses;
using Kati.SourceFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace KatiUnitTest.Module_Tests.GlobalModuleTest {
    [TestClass]
    public class TestPersonalCharacterRules {
        Controller ctrl;
        PersonalCharacterRules p;

        [TestInitialize]
        public void Start() {
            ctrl = new Controller(Constants.TestJson);
            p = ctrl.Parser.Personal;
            SetCharacterData(ctrl.Npc);
        }

        public void SetCharacterData(Kati.Module_Hub.CharacterData d) {
            Dictionary<string, string> con = new Dictionary<string, string>();
            Dictionary<string, int> scal = new Dictionary<string, int>();
            string[] arr = 
                {  "active","adventurous","agreeable","articulate","clever","calm","cheerful","Greedy",
                "courteous","dramatic","forgiving","generous","honest","humble","humorous","logical"};
            foreach (string s in arr) {
                con[s] = PersonalCharacterRules.TRAIT;
            }
            arr = new string[] { "lonely","drunk","aroused"};
            foreach (string s in arr) {
                con[s] = PersonalCharacterRules.STATUS;
            }
            arr = new string[] 
            {"likes movies","likes sports","likes music","likes making music","likes art","likes making art",
             "likes dancing","likes coffee","likes socializing","likes working","likes excersie","likes gaming",
             "likes being alone","likes being quiet","likes talking","likes cooking","likes shopping" };
            foreach (string s in arr) {
                con[s] = PersonalCharacterRules.INTEREST;
            }
            arr = new string[] 
            {"has a great smile","has a creepy smile","has nice eyes","has a kind face","has a stern face",
                "has great hair","has nappy hair","has nice curves","has great skin","has bad skin","is balding"};
            foreach (string s in arr) {
                con[s] = PersonalCharacterRules.PHYSICAL_FEATURES;
            }
            arr = new string[]
                { "charm","confident","leader","popular","attractive","persuasive","creative",
                "style","athletics","charisma","luck" };
            foreach (string s in arr) {
                scal[s] = 500;
            }
            d.InitiatorAttributeList = con;
            d.InitiatorScalarList = scal;
        }

        [TestMethod]
        public void TestPopQueue() {
            string[] arr = { "trait", "not", "humble","extra" };
            string key = "";
            (key, arr) = p.PopQueue(arr); 
            Assert.IsTrue(key.Equals("trait"));
            Assert.IsTrue(arr.Length == 3);
            (key, arr)= p.PopQueue(arr);
            Assert.IsTrue(key.Equals("not"));
            Assert.IsTrue(arr.Length == 2);
            (key, arr) = p.PopQueue(arr);
            Assert.IsTrue(key.Equals("humble"));
            Assert.IsTrue(arr.Length == 1);
        }

    }
}
