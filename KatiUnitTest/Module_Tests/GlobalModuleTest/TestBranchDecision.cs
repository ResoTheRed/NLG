using System;
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
            bd = ctrl.Parser.BranchDecision;
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
            Assert.IsTrue(d["hate"] == 240);
            Assert.IsTrue(d["disgust"] == 200);
            Assert.IsTrue(d["affinity"] == 160);
            Assert.IsTrue(d["friend"] == 120);
            Assert.IsTrue(d["respect"] == 80);
            Assert.IsTrue(d["rivalry"] == 40);
            Assert.IsTrue(d["professional"] == 0);
            arr = new double[]{ 110, 110, 10, 110, 110, 110, 110, 110 };
            SetNpcTone(arr);
            d = ctrl.Npc.InitiatorsTone;
            total = bd.ProbabilityOffset(d);
            Assert.IsTrue(total == 1900);
            Assert.IsTrue(d["romance"] == 380);
            Assert.IsTrue(d["hate"] == 340);
            Assert.IsTrue(d["disgust"] == 300);
            Assert.IsTrue(d["affinity"] == 260);
            Assert.IsTrue(d["friend"] == 220);
            Assert.IsTrue(d["respect"] == 180);
            Assert.IsTrue(d["rivalry"] == 140);
            Assert.IsTrue(d["professional"] == 0);

        }


    }
}
