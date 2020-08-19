using Kati.Module_Hub;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Kati.Data_Modules.GlobalClasses {
    public class PersonalCharacterRules {

        public const string PERSONAL = "personal";
        public const string TRAIT = "trait";
        public const string STATUS = "status";
        public const string INTEREST = "interest";
        public const string PHYSICAL_FEATURES = "physicalFeature";
        public const string SCALAR_TRAIT = "scalarTrait";
        private Controller ctrl;
        private CharacterData  npc;

        public PersonalCharacterRules(Controller ctrl) {
            this.Ctrl = ctrl;
            npc = Ctrl.Npc;
        }

        public Controller Ctrl { get => ctrl; set => ctrl = value; }
        public CharacterData Npc { get => npc; set => npc = value; }

        public Dictionary<string, Dictionary<string, List<string>>> ParsePersonalRequirments
                                (Dictionary<string, Dictionary<string, List<string>>> data) {
            List<string> keysToDelete = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, List<string>>> item in data) {
                foreach (string req in data[item.Key]["req"]) {
                    if (RemoveElement(req)) {
                        keysToDelete.Add(item.Key);
                    }
                }
            }
            foreach (string key in keysToDelete) {
                if (data.ContainsKey(key))
                    data.Remove(key);
            }
            return data;
        }

        public bool RemoveElement(string req) {
            if (req == null)
                return true;
            string[] arr = req.Split(".");
            if (arr.Length < 3 && arr[0].Equals(PERSONAL)) {
                return true;
            } else if (arr.Length == 0 || !arr[0].Equals(PERSONAL)) {
                return false;
            } else {
                string[] temp = new string[arr.Length - 1];
                for (int i = 1; i <= arr.Length - 1; i++) {//remove game Keyword
                    temp[i - 1] = arr[i];
                }
                return RuleDirectory(temp);
            }
        }

        public bool RuleDirectory(string[] arr) {
            bool remove;
            (string key, string[] temp) = PopQueue(arr);
            if (temp.Length < 1)
                return true;
            bool inverse = temp[0].Equals("not");
            if (inverse) {
                var t = PopQueue(temp);
                temp = t.Item2;
            }
            remove = RulesDirectory(key, temp);
            if (inverse)
                return !remove;
            return remove;
        }

        private bool RulesDirectory(string key, string[] temp) {
            bool remove;
            switch (key) {
                case TRAIT: { remove = CheckTrait(temp); } break;
                case STATUS: { remove = CheckStatus(temp); } break;
                case INTEREST: { remove = CheckInterest(temp); } break;
                case PHYSICAL_FEATURES: { remove = CheckPhysicalFeatures(temp); } break;
                case SCALAR_TRAIT: { remove = CheckScalarTrait(temp); } break;
                default: { return true; }
            }
            return remove;
        }
        //pop first element and return the element and the shortend array
        public (string, string[]) PopQueue(string[] arr) {
            string key = "";
            string[] temp = { };
            if (arr.Length > 0) {
                key = arr[0];
                temp = new string[arr.Length - 1];
                for (int i = 1; i <= arr.Length - 1; i++) {
                    temp[i - 1] = arr[i];
                }
            }
            return (key, temp);
        }

        protected bool CheckScalarTrait(string[] temp) {
            int value = 0;
            if (temp.Length < 2 || !Ctrl.Npc.InitiatorScalarList.ContainsKey(temp[0]))
                return true;
            try {
                value = Int32.Parse(temp[1]);
            } catch (FormatException) {
                return true;
            }
            return Ctrl.Npc.InitiatorScalarList[temp[0]]<value;
        }

        protected bool CheckPhysicalFeatures(string[] temp) {
            if (temp.Length < 1)
                return true;
            if (Ctrl.Npc.InitiatorPersonalList.ContainsKey(temp[0])) {
                if (Ctrl.Npc.InitiatorPersonalList[temp[0]].Equals(PHYSICAL_FEATURES)) {
                    return false;
                }
            }
            return true;
        }

        protected bool CheckInterest(string[] temp) {
            if (temp.Length < 1)
                return true;
            if (Ctrl.Npc.InitiatorPersonalList.ContainsKey(temp[0])) {
                if (Ctrl.Npc.InitiatorPersonalList[temp[0]].Equals(INTEREST)) {
                    return false;
                }
            }
            return true;
        }

        protected bool CheckStatus(string[] temp) {
            if (temp.Length < 1)
                return true;
            if (Ctrl.Npc.InitiatorPersonalList.ContainsKey(temp[0])) {
                if (Ctrl.Npc.InitiatorPersonalList[temp[0]].Equals(STATUS)) {
                    return false;
                }
            }
            return true;
        }

        protected bool CheckTrait(string[] temp) {
            if (temp.Length<1)
                return true;
            if (Ctrl.Npc.InitiatorPersonalList.ContainsKey(temp[0])) {
                if (Ctrl.Npc.InitiatorPersonalList[temp[0]].Equals(TRAIT)) {
                    return false;
                }
            }
            return true;
        }
    }
}
