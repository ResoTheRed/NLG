using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Data_Modules.GlobalClasses {
    /// <summary>
    /// Default class to decide which dialogue branch shuld be chosen based on 
    /// character data and inner ruling.  It is only for Statements and questions
    /// </summary>
    public class BranchDecision {

        private Controller ctrl;
        //attribute Threshold
        private int high;
        private int mid;
        private int low;
        //flags
        private bool isNeutral;
        public Controller Ctrl { get => ctrl; set => ctrl = value; }
        public int High { get => high; set => high = value; }
        public int Mid { get => mid; set => mid = value; }
        public int Low { get => low; set => low = value; }
        public bool IsNeutral { get => isNeutral; set => isNeutral = value; }
       
        public BranchDecision(Controller ctrl) {
            Ctrl = ctrl;
            IsNeutral = false;
            SetThresholds();
        }

        //holds all method calls for ease of use
        public Dictionary<string, Dictionary<string, List<string>>> RunDecision
            (Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> data) {
            SetThresholds();
            var tone = CancelAttributeTones();
            return GetAttributeBranch(tone, data);
        }

        //set threshold levels
        protected void SetThresholds() {
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
            new_tone["affinity"] = Affinity(tone);
            new_tone["respect"] = RespectRule(tone);
            return new_tone;
        }

        //romance += admiration - (disgust+(hate/2))
        protected double RomanceRule(Dictionary<string, double> tone) {
            double romance = tone["romance"] + tone["affinity"] / 2;
            romance -= (tone["disgust"] + tone["hate"] / 2);
            return romance;
        }

        protected double DisgustRule(Dictionary<string, double> tone) {
            double disgust = tone["disgust"] + tone["hate"] / 2;
            disgust -= (tone["romance"] + tone["affinity"] / 2);
            return disgust;
        }

        protected double FriendRule(Dictionary<string, double> tone) {
            double friend = tone["friend"] + (tone["respect"] / 2) + (tone["affinity"]/2);
            friend -= (tone["hate"] + tone["disgust"] / 2);
            return friend;
        }

        protected double HateRule(Dictionary<string, double> tone) {
            double hate = tone["hate"] + (tone["disgust"] / 2);
            hate -= (tone["friend"] + (tone["affinity"] / 2));
            return hate;
        }

        protected double ProfessionalRule(Dictionary<string, double> tone) {
            double prof = tone["professional"] + (tone["respect"] / 2);
            prof -= (tone["rivalry"] + (tone["disgust"] / 2));
            return prof;
        }

        protected double RivalryRule(Dictionary<string, double> tone) {
            double prof = tone["rivalry"] + (tone["respect"] / 2);
            prof -= (tone["professional"] + (tone["affinity"] / 2));
            return prof;
        }

        protected double Affinity(Dictionary<string, double> tone) {
            double admire = tone["affinity"] + (tone["romance"] / 2);
            admire -= (tone["disgust"] + tone["hate"] / 2);
            return admire;
        }

        protected double RespectRule(Dictionary<string, double> tone) {
            double respect = tone["respect"] + (tone["professional"] / 2);
            respect -= (tone["disgust"] + (tone["hate"] / 2));
            return respect;
        }

        //remove qualifying attributes from original and move them into copy
        public Dictionary<string, double> ExtractQualifyingAttributes
            (Dictionary<string, double> tone, int threshold) {
            Dictionary<string, double> qualifying = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> item in tone) {
                if (item.Value >= threshold) {
                    qualifying[item.Key] = item.Value;
                    tone.Remove(item.Key);
                }
            }
            return qualifying;
        }

        //return first dialogue branch from list of ordered branches
        public Dictionary<string, Dictionary<string, List<string>>> GetAttributeBranch
           (Dictionary<string, double> tone,Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> data) {
            List<string> full = OrderConversationBranches(tone);//orders all branches from most important to least
            for (int i=0;i<full.Count;i++) {
                if (data.ContainsKey(full[i])) {
                    return data[full[i]];
                }
            }
            return null;
        }

        //returns an ordered list of conversation branches
        //the idea is to strat with the first and if the
        //dialogue JSON doesn't have it move to the next one
        public List<string> OrderConversationBranches
                                    (Dictionary<string, double> tone) {
            List<string> full = new List<string>();
            while (!IsNeutral) {
                var copy = FindMostInfluentialAttribute(tone);//toggles on IsNeutral
                double total = ProbabilityOffset(copy);
                List<string> partial = PickAtttibutes(copy,total);//adds neutral to full list
                AddToList(full, partial);
            }
            IsNeutral = false;
            return full;
        }

        /*TODO:
         * find most influential Attributes
         * reduce attributes value
         * offset their probability
         * pick each based on probability
         * add them to the ordered list
         * remove them from the dictionary
         * repeat for high, mid, and low
         * when neutral is true end at neutral
         * if isStranger is true list == [stranger, neutral]
         */

        //find if any attributes meet the threshold starting from high to none
        //@param: tone--> original dict that will have elements removed
        public Dictionary<string, double> FindMostInfluentialAttribute
                                        (Dictionary<string, double> tone) {
            Dictionary<string, double> options;
            options = ExtractQualifyingAttributes(tone, High);
            if (options.Count == 0) {
                options = ExtractQualifyingAttributes(tone, Mid);
            } 
            if (options.Count == 0) {
                options = ExtractQualifyingAttributes(tone, Low);
            }
            if (options.Count == 0) {
                options = tone;
                IsNeutral = true;
            } 
            return options;
        }

        //weight the options to yeild some emotions as stronger than others
        //done by reference
        public double ProbabilityOffset(Dictionary<string, double> copy) {
            if (IsNeutral) return 0;
            double total = 0;
            ReduceAttributeValue(copy);
            total = ProbabilityOffsetSingle(copy,"romance",total,280);
            total = ProbabilityOffsetSingle(copy,"hate",total,240);
            total = ProbabilityOffsetSingle(copy,"disgust",total,200);
            total = ProbabilityOffsetSingle(copy,"affinity",total,160);
            total = ProbabilityOffsetSingle(copy,"friend",total,120);
            total = ProbabilityOffsetSingle(copy,"respect",total,80);
            total = ProbabilityOffsetSingle(copy,"rivalry",total,40);
            total = ProbabilityOffsetSingle(copy,"professional",total,0);
            return total;
        }

        public Dictionary<string, double> DeepCopyBrachDict(Dictionary<string, double> original) {
            Dictionary<string, double> copy = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> item in original) {
                copy[item.Key] = item.Value;
            }
            return copy;
        }

        //devise the probability for a single attribute
        public double ProbabilityOffsetSingle
            (Dictionary<string, double> tone, string att, double total, int offset) {
            if (tone.ContainsKey(att)) {
               //increase each size
                total += tone[att] + offset;
                tone[att] = total;
            }
            return total;
        }

        //order attributes based on
        public List<string> PickAtttibutes(Dictionary<string, double> copy, double max) {
            List<string> sort = new List<string>();
            if (IsNeutral) {
                sort.Add("neutral");
                return sort;
            }
            while (copy.Count > 0) {
                double choice = (Controller.dice.NextDouble() * max);
                if (copy.ContainsKey("romance") && copy["romance"] >= choice) {
                    sort.Add("romance");
                    copy.Remove("romance");
                } else if (copy.ContainsKey("hate") && copy["hate"] >= choice) {
                    sort.Add("hate");
                    copy.Remove("hate");
                } else if (copy.ContainsKey("disgust") && copy["disgust"] >= choice) {
                    sort.Add("disgust");
                    copy.Remove("disgust");
                } else if (copy.ContainsKey("affinity") && copy["affinity"] >= choice) {
                    sort.Add("affinity");
                    copy.Remove("affinity");
                } else if (copy.ContainsKey("friend") && copy["friend"] >= choice) {
                    sort.Add("friend");
                    copy.Remove("friend");
                } else if (copy.ContainsKey("respect") && copy["respect"] >= choice) {
                    sort.Add("respect");
                    copy.Remove("respect");
                } else if (copy.ContainsKey("rivalry") && copy["rivalry"] >= choice) {
                    sort.Add("rivaly");
                    copy.Remove("rivalry");
                } else if (copy.ContainsKey("professional") && copy["professional"] >= choice) {
                    sort.Add("professional");
                    copy.Remove("professional");
                } else {
                    //break;//something went wrong
                }
            }
            return sort;
        }

        //public string 

        //subtract each tone branch value by the value with the 
        //smallest ammount
        public void ReduceAttributeValue(Dictionary<string, double> tone) {
            double min =  GetMinValue(tone);
            Dictionary<string, double> temp = new Dictionary<string, double>();
            foreach (KeyValuePair<string, double> item in tone) {
                temp[item.Key] = (tone[item.Key] - min);
            }
            foreach (KeyValuePair<string, double> item in temp) {
                tone[item.Key] = temp[item.Key];
            }
        }

        protected double GetMinValue(Dictionary<string, double> tone) {
            double min = 10000;
            foreach (KeyValuePair<string, double> item in tone) {
                if (item.Value < min)
                    min = item.Value;
            }
            return min;
        }

        public void AddToList(List<string> full, List<string> partial) {
            foreach (string s in partial) {
                full.Add(s);
            }
        }

        public bool CheckIsStranger() {
            return false;
        }
        


    }
}
