using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Data_Modules.GlobalClasses {
    /// <summary>
    /// Default class to decide which dialogue branch shuld be chosen based on 
    /// character data and inner ruling.  It is only for Statements and questions
    /// </summary>
    class BranchDecision {

        private Controller ctrl;
        //attribute Threshold
        private int high;
        private int mid;
        private int low;
        public Controller Ctrl { get => ctrl; set => ctrl = value; }
        public int High { get => high; set => high = value; }
        public int Mid { get => mid; set => mid = value; }
        public int Low { get => low; set => low = value; }

        public BranchDecision(Controller ctrl) {
            Ctrl = ctrl;
        }

        //holds all method calls for ease of use
        public Dictionary<string, Dictionary<string, List<string>>> RunDecision
            (Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> data) {
            SetThresholds();
            var tone = CancelAttributeTones();
            tone = FindMostInfluentialAttribute(tone);
            return null;
        }

        //set threshold levels
        public void SetThresholds() {
            High = 700;
            Mid = 450;
            low = 200;
        }

        public Dictionary<string, double> CancelAttributeTones() {
            var tone = Ctrl.Npc.InitiatorsTone;
            Dictionary<string, double> new_tone = new Dictionary<string, double>();
            new_tone["romance"] = RomanceRule(tone);
            new_tone["disgust"] = DisgustRule(tone);
            new_tone["friend"] = FriendRule(tone);
            new_tone["hate"] = HateRule(tone);
            new_tone["professional"] = ProfessionalRule(tone);
            new_tone["rivalry"] = RivalryRule(tone);
            new_tone["admiration"] = AdmirationRule(tone);
            new_tone["respect"] = RespectRule(tone);
            return new_tone;
        }

        //romance += admiration - (disgust+(hate/2))
        public double RomanceRule(Dictionary<string, double> tone) {
            double romance = tone["romance"] + tone["admiration"] / 2;
            romance -= (tone["disgust"] + tone["hate"] / 2);
            return romance;
        }

        public double DisgustRule(Dictionary<string, double> tone) {
            double disgust = tone["disgust"] + tone["hate"] / 2;
            disgust -= (tone["romance"] + tone["admiration"] / 2);
            return disgust;
        }

        public double FriendRule(Dictionary<string, double> tone) {
            double friend = tone["friend"] + (tone["respect"] / 2) + (tone["admiration"]/2);
            friend -= (tone["hate"] + tone["disgust"] / 2);
            return friend;
        }
        
        public double HateRule(Dictionary<string, double> tone) {
            double hate = tone["hate"] + (tone["disgust"] / 2);
            hate -= (tone["friend"] + (tone["admiration"] / 2));
            return hate;
        }
        
        public double ProfessionalRule(Dictionary<string, double> tone) {
            double prof = tone["professional"] + (tone["respect"] / 2);
            prof -= (tone["rivalry"] + (tone["disgust"] / 2));
            return prof;
        }
        
        public double RivalryRule(Dictionary<string, double> tone) {
            double prof = tone["rivalry"] + (tone["respect"] / 2);
            prof -= (tone["professional"] + (tone["admiration"] / 2));
            return prof;
        }
        
        public double AdmirationRule(Dictionary<string, double> tone) {
            double admire = tone["admiration"] + (tone["romance"] / 2);
            admire -= (tone["disgust"] + tone["hate"] / 2);
            return admire;
        }
        
        public double RespectRule(Dictionary<string, double> tone) {
            double respect = tone["respect"] + (tone["professional"] / 2);
            respect -= (tone["disgust"] + (tone["hate"] / 2));
            return respect;
        }

        
        public Dictionary<string, double> ExtractQualifyingAttributes
            (Dictionary<string, double> tone, int threshold) {
            Dictionary<string, double> qualifying = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> item in tone) {
                if (item.Value >= threshold)
                    qualifying[item.Key] = item.Value;
            }
            return tone;
        }

        //find if any attributes meet the threshold starting from high to none
        public Dictionary<string, double> FindMostInfluentialAttribute
                                        (Dictionary<string, double> tone) {
            Dictionary<string, double> options;
            options = ExtractQualifyingAttributes(tone, high);
            if (options.Count == 0) {
                options = ExtractQualifyingAttributes(tone, mid);
            } else if (options.Count == 0) {
                options = ExtractQualifyingAttributes(tone, low);
            } else if (options.Count == 0) {
                options = tone;
            } 
            return options;
        }

        public Dictionary<string, Dictionary<string, List<string>>> GetAttributeBranch
                                                        (Dictionary<string, double> tone) {
            
            return null;
        }


        public bool RomanceProbability() {

            return true;
        }
        //has between 2 and 8 entries



    }
}
