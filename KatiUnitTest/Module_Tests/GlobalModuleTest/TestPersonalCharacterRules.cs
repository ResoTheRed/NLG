using Kati.Data_Modules.GlobalClasses;
using Kati.SourceFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            d.InitiatorPersonalList = con;
            d.InitiatorScalarList = scal;
        }

        [TestMethod]
        public void TestPopQueue() {
            string[] arr = { "trait", "not", "humble","extra" };
            string key = "";
            (key, arr) = p.PopQueue(arr); 
            Assert.IsTrue(key.Equals("trait"));
            Assert.IsTrue(arr[0].Equals("not"));
            Assert.IsTrue(arr.Length == 3);
            (key, arr)= p.PopQueue(arr);
            Assert.IsTrue(key.Equals("not"));
            Assert.IsTrue(arr.Length == 2);
            (key, arr) = p.PopQueue(arr);
            Assert.IsTrue(key.Equals("humble"));
            Assert.IsTrue(arr.Length == 1);
        }

        [TestMethod]
        public void TestRuleDirectoryTrait() {
            string[] arr = { "trait", "not", "active" };
            bool removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "trait","active" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[]{ "trait", "not", "Winnie the Pooh" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[]{ "trait", "not", "sloppy" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[]{ "trait","active" ,"Bolder Dash"};
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[]{ "trait" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "not" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "l","r","e","3" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
        }

        [TestMethod]
        public void TestRuleDirectoryStatus() {
            string[] arr = { "status", "not", "drunk" };
            bool removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "status", "drunk" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[]{ "status", "not", "happy" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[]{ "status", "happy" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "status", "drunk","billy-bob" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[]{ "status"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "not"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "not","status"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "a","b","c","d","e","f" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
        }

        [TestMethod]
        public void TestRuleDirectoryInterest() {
            string[] arr = { "interest", "not", "likes music" };
            bool removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "interest", "likes music" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[]{ "interest", "likes the opera" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[]{ "interest", "not", "likes the opera" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            //not negates anything after it so not something that doesn't exist is true;
            arr = new string[]{ "TURKEY", "not", "likes the opera" }; 
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[] { "not", "interest" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "not"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] {"interest" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "2","4","f","r","elle"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
        }

        [TestMethod]
        public void TestRuleDirectoryPhysicalFeature() {
            string[] arr = { "physicalFeature", "not", "has nice curves" };
            bool removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "physicalFeature", "has nice curves" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[] { "physicalFeature", "not", "long jacket" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[] { "physicalFeature", "not", "" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[] { "physicalFeature", "belly button lent" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "physical","Feature", "belly ","button ","lent" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "physicalFeature"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "not"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
        }

        [TestMethod]
        public void TestRuleDirectoryScalarTrait() {
            string[] arr = { "scalarTrait", "not", "charm", "500" };
            bool removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "scalarTrait", "charm","500" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[] { "scalarTrait", "not", "charm","100" };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "scalarTrait", "charm","100" };
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[] { "scalarTrait", "charm", };
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "scalarTrait", "megaman", "250"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "scalarTrait", "not", "megaman", "250"};
            removed = p.RuleDirectory(arr);
            Assert.IsFalse(removed);
            arr = new string[] { "not","scalarTrait"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "scalarTrait"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);
            arr = new string[] { "not"};
            removed = p.RuleDirectory(arr);
            Assert.IsTrue(removed);

        }

        [TestMethod]
        public void TestRemoveElement1() {
            string req = "personal.trait.active";
            bool removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = "personal.trait.not.active";
            removed = p.RemoveElement(req);
            Assert.IsTrue(removed);
            req = "personal.status.aroused";
            removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = "personal.status.not.aroused";
            removed = p.RemoveElement(req);
            Assert.IsTrue(removed);
            req = "personal.interest.likes music";
            removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = "personal.interest.not.likes music";
            removed = p.RemoveElement(req);
            Assert.IsTrue(removed);
            req = "personal.physicalFeature.has nice curves";
            removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = "personal.physicalFeature.not.has nice curves";
            removed = p.RemoveElement(req);
            Assert.IsTrue(removed);
            req = "personal.scalarTrait.charm.150";
            removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = "personal.scalarTrait.charm.not.150";
            removed = p.RemoveElement(req);
            Assert.IsTrue(removed);

        }

        [TestMethod]
         public void TestRemoveElement2() {
            string req = "person.trait.active";
            bool removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = "personal.trait.not.activation";
            removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = "personal.activation.ball.lightning";
            removed = p.RemoveElement(req);
            Assert.IsTrue(removed);
            req = "personal.status";
            removed = p.RemoveElement(req);
            Assert.IsTrue(removed);
            req = "p.status.not.aroused";
            removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = "";
            removed = p.RemoveElement(req);
            Assert.IsFalse(removed);
            req = null;
            removed = p.RemoveElement(req);
            Assert.IsTrue(removed);
            req = "west.jefferson.parish";
            removed = p.RemoveElement(req);
            Assert.IsFalse(removed);

        }

        [TestMethod]
        public void TestParsePersonalRequirements() {
            var data = p.ParsePersonalRequirments(ctrl.Lib.Data["sample1_statement"]["romance"]);
            Assert.IsTrue(data.Count<=15);
            Assert.IsTrue(data.ContainsKey("romance test 0"));
            Assert.IsTrue(data.ContainsKey("romance test 1"));
            Assert.IsTrue(data.ContainsKey("romance test 2-not"));
            Assert.IsTrue(data.ContainsKey("romance test 5"));
            Assert.IsTrue(data.ContainsKey("romance test 6"));
            Assert.IsTrue(data.ContainsKey("romance test 7-not"));
            Assert.IsTrue(data.ContainsKey("romance test 10"));
            Assert.IsTrue(data.ContainsKey("romance test 11-not"));
            Assert.IsTrue(data.ContainsKey("romance test 12"));
            Assert.IsTrue(data.ContainsKey("romance test 14"));
            Assert.IsTrue(data.ContainsKey("romance test 15-not"));
            Assert.IsTrue(data.ContainsKey("romance test 17"));
            Assert.IsTrue(data.ContainsKey("romance test 18"));
            Assert.IsTrue(data.ContainsKey("romance test 19"));
            Assert.IsTrue(data.ContainsKey("romance test 21"));

            Assert.IsFalse(data.ContainsKey("romance test 14-not"));
            Assert.IsFalse(data.ContainsKey("romance test 15"));
            Assert.IsFalse(data.ContainsKey("romance test 16"));
        }

    }
}
