using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Kati.Data_Modules.GlobalClasses;
using Kati.SourceFiles;

namespace KatiUnitTest.Module_Tests.GlobalModuleTest {
    
    [TestClass]
    public class TestBranchDecision {
        Controller ctrl;
        BranchDecision bd;

        [TestInitialize]
        public void Start() {
            ctrl = new Controller(Constants.DayDreamWonder);
            bd = ctrl.Parser.Branch;
        }

        public void SetNpcTone(double[] val) {
            ctrl.Npc.InitiatorsTone["romance"] = val[0];
            ctrl.Npc.InitiatorsTone["friend"] = val[1];
            ctrl.Npc.InitiatorsTone["professional"] = val[2];
            ctrl.Npc.InitiatorsTone["respect"] = val[3];
            ctrl.Npc.InitiatorsTone["affinity"] = val[4];
            ctrl.Npc.InitiatorsTone["disgust"] = val[5];
            ctrl.Npc.InitiatorsTone["hate"] = val[6];
            ctrl.Npc.InitiatorsTone["rivalry"] = val[7];
        }

        [TestMethod]
        public void TestCancelAttributeTones() {
            double[] arr = {700,600,500,400,300,200,100,0 };
            SetNpcTone(arr);
            Dictionary<string, double> temp = bd.CancelAttributeTones();
            Assert.IsTrue(ctrl.Npc.InitiatorsTone.ContainsKey("romance"));
            Assert.IsTrue(temp["romance"]==600.0);
            Assert.IsTrue(temp["friend"]==750.0);
            Assert.IsTrue(temp["professional"]==600.0);
            Assert.IsTrue(temp["respect"]==400.0);
            Assert.IsTrue(temp["affinity"]==400.0);
            Assert.IsTrue(temp["disgust"]<0);
            Assert.IsTrue(temp["hate"]<0);
            Assert.IsTrue(temp["rivalry"]<0);
        }

        [TestMethod]
        public void TestExtractQualifyingAttributes() {
            double[] arr = { 700, 600, 500, 400, 300, 200, 100, 0 };
            SetNpcTone(arr);
            Dictionary<string, double> d = ctrl.Npc.InitiatorsTone;
            Dictionary<string,double> temp = bd.ExtractQualifyingAttributes(d, bd.High);
            Assert.IsTrue(temp.ContainsKey("romance"));
            Assert.IsTrue(temp.Count==1);
            Assert.IsTrue(!d.ContainsKey("romance"));
            Assert.IsTrue(d.Count == 7);
            temp = bd.ExtractQualifyingAttributes(d,bd.Mid);
            Assert.IsTrue(temp.ContainsKey("friend"));
            Assert.IsTrue(temp.ContainsKey("professional"));
            Assert.IsTrue(temp.Count == 2);
            Assert.IsTrue(!d.ContainsKey("friend"));
            Assert.IsTrue(!d.ContainsKey("professional"));
            Assert.IsTrue(d.Count == 5);
            temp = bd.ExtractQualifyingAttributes(d, bd.Low);
            Assert.IsTrue(temp.ContainsKey("respect"));
            Assert.IsTrue(temp.ContainsKey("affinity"));
            Assert.IsTrue(temp.ContainsKey("disgust"));
            Assert.IsTrue(temp.Count == 3);
            Assert.IsTrue(!d.ContainsKey("respect"));
            Assert.IsTrue(!d.ContainsKey("affinity"));
            Assert.IsTrue(!d.ContainsKey("disgust"));
            Assert.IsTrue(d.Count == 2);

        }
        
        [TestMethod]
        public void TestFindMostInfluential() {
            double[] arr = { 700, 600, 500, 400, 300, 200, 100, 0 };
            SetNpcTone(arr);
            Dictionary<string, double> d = ctrl.Npc.InitiatorsTone;
            Dictionary<string, double> temp = bd.FindMostInfluentialAttribute(d);
            Assert.IsTrue(temp.ContainsKey("romance"));
            Assert.IsTrue(temp.Count==1);
            Assert.IsTrue(!d.ContainsKey("romance"));
            Assert.IsTrue(d.Count == 7);
            temp = bd.FindMostInfluentialAttribute(d);
            Assert.IsTrue(temp.ContainsKey("friend"));
            Assert.IsTrue(temp.ContainsKey("professional"));
            Assert.IsTrue(temp.Count == 2);
            Assert.IsTrue(!d.ContainsKey("friend"));
            Assert.IsTrue(!d.ContainsKey("professional"));
            Assert.IsTrue(d.Count == 5);
            temp = bd.FindMostInfluentialAttribute(d);
            Assert.IsTrue(temp.ContainsKey("respect"));
            Assert.IsTrue(temp.ContainsKey("affinity"));
            Assert.IsTrue(temp.ContainsKey("disgust"));
            Assert.IsTrue(temp.Count == 3);
            Assert.IsTrue(!d.ContainsKey("respect"));
            Assert.IsTrue(!d.ContainsKey("affinity"));
            Assert.IsTrue(!d.ContainsKey("disgust"));
            Assert.IsTrue(d.Count == 2);
            temp = bd.FindMostInfluentialAttribute(d);
            Assert.IsTrue(temp == d);
            Assert.IsTrue(bd.IsNeutral);
        }

        [TestMethod]
        public void TestProbabilityOffset() {
            double[] arr = { 0, 0, 0, 0, 0, 0, 0, 0 };
            SetNpcTone(arr);
            Dictionary<string, double> d = ctrl.Npc.InitiatorsTone;
            double total = bd.ProbabilityOffset(d);
            Assert.IsTrue(total == 1120);
            Assert.IsTrue(d["romance"] == 280);
            Assert.IsTrue(d["hate"] == 520);
            Assert.IsTrue(d["disgust"] == 720);
            Assert.IsTrue(d["affinity"] == 880);
            Assert.IsTrue(d["friend"] == 1000);
            Assert.IsTrue(d["respect"] == 1080);
            Assert.IsTrue(d["rivalry"] == 1120);
            Assert.IsTrue(d["professional"] == 1120);
            arr = new double[]{ 110, 110, 10, 110, 110, 110, 110, 110 };
            SetNpcTone(arr);
            d = ctrl.Npc.InitiatorsTone;
            total = bd.ProbabilityOffset(d);
            Assert.IsTrue(total == 1820);
            Assert.IsTrue(d["romance"] == 380);
            Assert.IsTrue(d["hate"] == 720);
            Assert.IsTrue(d["disgust"] == 1020);
            Assert.IsTrue(d["affinity"] == 1280);
            Assert.IsTrue(d["friend"] == 1500);
            Assert.IsTrue(d["respect"] == 1680);
            Assert.IsTrue(d["rivalry"] == 1820);
            Assert.IsTrue(d["professional"] == 1820);
        }

        [TestMethod]
        public void TestPickAttributes() {
            for (int i = 0; i < 100; i++) {
                double[] arr = { 700, 600, 500, 400, 300, 200, 100, 0 };
                SetNpcTone(arr);
                Dictionary<string, double> d = ctrl.Npc.InitiatorsTone;
                d = bd.CancelAttributeTones();
                Assert.IsTrue(d["romance"] == 600);
                Assert.IsTrue(d["friend"] == 750.0);
                Assert.IsTrue(d["professional"] == 600.0);
                Assert.IsTrue(d["respect"] == 400.0);
                Assert.IsTrue(d["affinity"] == 400.0);
                Assert.IsTrue(d["disgust"] < 0);
                Assert.IsTrue(d["hate"] < 0);
                Assert.IsTrue(d["rivalry"] < 0);

                //high
                var copy = bd.FindMostInfluentialAttribute(d);
                Assert.IsTrue(copy.ContainsKey("friend"));
                Assert.IsTrue(!d.ContainsKey("friend"));
                Assert.IsTrue(copy.Count == 1);

                double total = bd.ProbabilityOffset(copy);
                Assert.IsTrue(total <= 120);//was 870 but reduced by min with is 750 and also max
                Assert.IsTrue(copy.ContainsKey("friend"));
                Assert.IsTrue(copy.Count == 1);
                Assert.IsTrue(copy["friend"] >= total);
                
                List<string> list = bd.PickAtttibutes(copy, total);
                Assert.IsTrue(list.Count == 1);
                Assert.IsTrue(list[0].Equals("friend")|| list[0].Equals("neutral"));
                
                //mid
                copy = bd.FindMostInfluentialAttribute(d);
                Assert.IsTrue(copy.ContainsKey("romance"));
                Assert.IsTrue(!d.ContainsKey("romance"));
                Assert.IsTrue(copy.ContainsKey("professional"));
                Assert.IsTrue(!d.ContainsKey("professional"));
                Assert.IsTrue(copy.Count == 2);
                
                total = bd.ProbabilityOffset(copy);
                Assert.IsTrue(total == 280);//was 870 but reduced by min with is 750 and also max
                Assert.IsTrue(copy.ContainsKey("romance"));
                Assert.IsTrue(copy.ContainsKey("professional"));
                Assert.IsTrue(copy.Count == 2);
                
                list = bd.PickAtttibutes(copy, total);
                Assert.IsTrue(list.Count == 2);
                Assert.IsTrue(list[0].Equals("romance"));
                Assert.IsTrue(list[1].Equals("professional"));
                //low
                copy = bd.FindMostInfluentialAttribute(d);
                Assert.IsTrue(copy.ContainsKey("affinity"));
                Assert.IsTrue(!d.ContainsKey("affinity"));
                Assert.IsTrue(copy.ContainsKey("respect"));
                Assert.IsTrue(!d.ContainsKey("respect"));
                Assert.IsTrue(copy.Count == 2);
                total = bd.ProbabilityOffset(copy);
                Assert.IsTrue(total == 240);
                Assert.IsTrue(copy.ContainsKey("affinity"));
                Assert.IsTrue(copy.ContainsKey("respect"));
                Assert.IsTrue(copy.Count == 2);
                list = bd.PickAtttibutes(copy, total);
                Assert.IsTrue(list.Count == 2);
                Assert.IsTrue(list[0].Equals("affinity") || list[0].Equals("respect"));
                Assert.IsTrue(list[1].Equals("affinity") || list[1].Equals("respect"));
                //none-->neutral
                copy = bd.FindMostInfluentialAttribute(d);
                Assert.IsTrue(copy.Count == 3);
                Assert.IsTrue(d.Count == 3);
                total = bd.ProbabilityOffset(copy);
                Assert.IsTrue(total == 0);
                Assert.IsTrue(bd.IsNeutral);
                list = bd.PickAtttibutes(copy, total);
                Assert.IsTrue(list.Count == 1);
                Assert.IsTrue(list[0].Equals("neutral"));
                bd.IsNeutral = false;
            }
        }//end test method

        [TestMethod]
        public void TestOrderConversationBranches() {
            for (int i = 0; i < 1000; i++) {
                double[] arr = { 700, 600, 500, 400, 300, 200, 100, 0 };
                SetNpcTone(arr);
                Dictionary<string, double> d = ctrl.Npc.InitiatorsTone;
                d = bd.CancelAttributeTones();
                List<string> list = bd.OrderConversationBranches(d);
                //Assert.IsTrue(list.Count == 6);
                Assert.IsTrue(list[0].Equals("friend"));
                Assert.IsTrue(list[1].Equals("romance"));
                Assert.IsTrue(list[2].Equals("professional"));
                Assert.IsTrue(list[3].Equals("affinity") || list[3].Equals("respect"));
                Assert.IsTrue(list[4].Equals("affinity") || list[4].Equals("respect"));
                Assert.IsTrue(list[5].Equals("neutral"));
                Assert.IsFalse(bd.IsNeutral);
            }
        }

        [TestMethod]
        public void TestRunDecision() {
            double[] arr = { 700, 600, 500, 400, 300, 200, 100, 0 };
            SetNpcTone(arr);
            var decision = bd.RunDecision(ctrl.Lib.Data[ctrl.Lib.Keys["dream"][ctrl.Lib.STATEMENT]]);
            Assert.IsTrue(decision.Count == 14);
            Assert.IsTrue(decision.ContainsKey("I've been day dreaming a lot lately.  I wonder why?"));

        }


    }
}
