using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Data_Modules.GlobalClasses {
    public class Parser {

        private Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> data;
        private string topic;//main topic "dreams","weather"...
        private string type;//Statement or Question
        private Controller ctrl;
        private BranchDecision branchDecision;

        public string Topic { get => topic; set => topic = value; }
        public string Type { get => type; set => type = value; }
        public Controller Ctrl { get => ctrl; set => ctrl = value; }
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> Data 
            { get => data; set => data = value; }
        public BranchDecision BranchDecision { get => branchDecision; set => branchDecision = value; }

        public Parser(Controller ctrl) {
            Ctrl = ctrl;
            BranchDecision = new BranchDecision(Ctrl);
        }

        public void Setup(string topic, string type,
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> data) {
            Topic = topic;
            Type = type;
            Data = data;
        }

        public void Parse() { 
            //var = BranchDecision.runDecision()
        }

    }
}
