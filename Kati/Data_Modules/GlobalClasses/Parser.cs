using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Data_Modules.GlobalClasses {
    public class Parser {

        private Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> data;
        private string topic;//main topic "dreams","weather"...
        private string type;//Statement or Question

        private Controller ctrl;
        private BranchDecision branch;
        private GameRules game;
        private PersonalCharacterRules personal;

        public string Topic { get => topic; set => topic = value; }
        public string Type { get => type; set => type = value; }
        public Controller Ctrl { get => ctrl; set => ctrl = value; }
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> Data 
            { get => data; set => data = value; }
        public BranchDecision Branch { get => branch; set => branch = value; }
        public GameRules Game { get => game; set => game = value; }
        public PersonalCharacterRules Personal { get => personal; set => personal = value; }

        public Parser(Controller ctrl) {
            Ctrl = ctrl;
            Branch = new BranchDecision(Ctrl);
            Game = new GameRules(Ctrl);
            Personal = new PersonalCharacterRules(Ctrl);
        }

        public void Setup(string topic, string type,
            Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> data) {
            Topic = topic;
            Type = type;
            Data = data;
        }

        public void Parse() {
            //Data most not point to Lib data but be a copy
            var data = Branch.RunDecision(Data);
            //Remove GameData Selections
            //Remove CharacterData personal attributes
            //Remove CharacterData social attributes
        }

    }
}
