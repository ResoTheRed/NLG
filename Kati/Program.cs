using Kati.Data_Models;
using Kati.Module_Hub;
using System;
using System.Collections.Generic;
using Kati.Data_Modules.GlobalClasses;
using Kati.SourceFiles;

namespace Kati{

    class Program{

        


        static void Main(string[] args){
            //TestSmallTalkText(5);
            //TestSmallTalkEventResponse(20000);
            //TestEventResponse2();
            //TestAB();
            //TestController(50);
            //TestA(5);
            TestGameRule();
        }

        /*-----Parse Game Data-------*/
        static Dictionary<string, Dictionary<string, List<string>>> data;
        static GameRules game;

        public static void _Start() {
            ctrl = new Controller("C:/Users/User/Documents/NLG/KatiUnitTest/Module_Tests/GlobalModuleTest/RuleTester.json");
            _SetGameData(ctrl.Game);
            data = ctrl.Lib.Data["sample1_statement"]["neutral"];
            game = ctrl.Parser.Game;
        }

        public static  void _SetGameData(GameData g) {
            g.Weather = "nice_day";
            g.Sector = "5";
            g.TimeOfDay = "morning";
            g.Season = "spring";
            g.DayOfWeek = 1;
            g.DayOfMonth = 8;
        }

        public static void TestGameRule() {
            _Start();
            Console.WriteLine("\nWhat is contained in the dataset before\n");
            foreach (string key in ctrl.Lib.Data["sample1_statement"]["neutral"].Keys) {
                Console.WriteLine(key);
            }

            var data = game.ParseGameRequirments(ctrl.Lib.Data["sample1_statement"]["neutral"]);

            Console.WriteLine("\nWhat is contained in the dataset after\n");            
            foreach (string key in data.Keys) {
                Console.WriteLine(key);
            }
        }



        static Controller ctrl;
        static BranchDecision bd;

        public static void Start() {
            ctrl = new Controller(Constants.DayDreamWonder);
            bd = ctrl.Parser.Branch;
        }

        public static void SetNpcTone(double[] val) {
            ctrl.Npc.InitiatorsTone["romance"] = val[0];
            ctrl.Npc.InitiatorsTone["friend"] = val[1];
            ctrl.Npc.InitiatorsTone["professional"] = val[2];
            ctrl.Npc.InitiatorsTone["respect"] = val[3];
            ctrl.Npc.InitiatorsTone["affinity"] = val[4];
            ctrl.Npc.InitiatorsTone["disgust"] = val[5];
            ctrl.Npc.InitiatorsTone["hate"] = val[6];
            ctrl.Npc.InitiatorsTone["rivalry"] = val[7];
        }

        public static void TestA(int n) {
            
            for (int i = 0; i < n; i++) {
                Start();
                double[] arr = { 700, 600, 500, 400, 300, 200, 100, 0 };
                SetNpcTone(arr);
                Dictionary<string, double> d = ctrl.Npc.InitiatorsTone;
                d = bd.CancelAttributeTones();
                Console.WriteLine("d[romance] == 600: "+ d["romance"] );
                Console.WriteLine("d[friend] == 750.0" + d["friend"] );
                Console.WriteLine("d[professional] == 600.0: "+(d["professional"]));
                Console.WriteLine("d[respect] == 400.0: "+(d["respect"]));
                Console.WriteLine("d[affinity] == 400.0: " + d["affinity"]);
                Console.WriteLine("d[disgust] < 0: " + d["disgust"]);
                Console.WriteLine("d[hate] < 0" + d["hate"]);
                Console.WriteLine("d[rivalry] < 0" + d["rivalry"]);

                //high
                Console.WriteLine("\n--------Running High Stats-------------------------");
                var copy = bd.FindMostInfluentialAttribute(d);
                Console.WriteLine("copy.ContainsKey(friend) " + copy.ContainsKey("friend"));
                Console.WriteLine("copy.Count == 1: " + (copy.Count == 1));
                foreach (KeyValuePair<string, double> item in copy) {
                    Console.WriteLine("copy["+item.Key+"] : "+item.Value);
                }

                double total = bd.ProbabilityOffset(copy);
                Console.WriteLine("total 120: "+total+"\n Should only contain friend");//was 870 but reduced by min with is 750 and also max
                foreach (KeyValuePair<string, double> item in copy) {
                    Console.WriteLine("copy[" + item.Key + "] : " + item.Value + " == total: " + total);
                }
                List<string> list = bd.PickAtttibutes(copy, total);
                Console.WriteLine("List should have one element friend");
                int counter = 0;
                foreach (string s in list) {
                    Console.WriteLine("list["+counter+"] : "+s);
                    counter++;
                }
                //mid
                Console.WriteLine("\n--------Running Mid Stats--------------------------");
                copy = bd.FindMostInfluentialAttribute(d);
                Console.WriteLine("Should contain only romance and professional");
                foreach (KeyValuePair<string, double> item in copy) {
                    Console.WriteLine("copy[" + item.Key + "] : " + item.Value);
                }
                total = bd.ProbabilityOffset(copy);
                Console.WriteLine("total 280: " + total + "\n Should only contain romance and professional");//was 870 but reduced by min with is 750 and also max
                foreach (KeyValuePair<string, double> item in copy) {
                    Console.WriteLine("copy[" + item.Key + "] : " + item.Value+" == total: "+ total);
                }
                list = bd.PickAtttibutes(copy, total);
                Console.WriteLine("List should have romance and professional");
                counter = 0;
                foreach (string s in list) {
                    Console.WriteLine("list[" + counter + "] : " + s);
                    counter++;
                }
                //low
                Console.WriteLine("\n--------Running Low Stats--------------------------");
                copy = bd.FindMostInfluentialAttribute(d);
                Console.WriteLine("Should contain only affinity and respect");
                foreach (KeyValuePair<string, double> item in copy) {
                    Console.WriteLine("copy[" + item.Key + "] : " + item.Value);
                }
                total = bd.ProbabilityOffset(copy);
                Console.WriteLine("total 240: " + total + "\n Should only contain affinity and respect");//was 870 but reduced by min with is 750 and also max
                foreach (KeyValuePair<string, double> item in copy) {
                    Console.WriteLine("copy[" + item.Key + "] : " + item.Value + " == total: " + total);
                }
                list = bd.PickAtttibutes(copy, total);
                Console.WriteLine("List should have afinity and respect");
                counter = 0;
                foreach (string s in list) {
                    Console.WriteLine("list[" + counter + "] : " + s);
                    counter++;
                }
                //none-->neutral
                Console.WriteLine("\n--------Running Neutral----------------------------");
                copy = bd.FindMostInfluentialAttribute(d);
                total = bd.ProbabilityOffset(copy);
                Console.WriteLine("Total: "+total+" IsNeutral: "+bd.IsNeutral);
                list = bd.PickAtttibutes(copy, total);
                Console.WriteLine("List should have neutral");
                counter = 0;
                foreach (string s in list) {
                    Console.WriteLine("list[" + counter + "] : " + s);
                    counter++;
                }
            }
        }





        /*  Depreciated elements SmallTalk Module

        /// <summary>
        /// SmallTalk Stuff
        /// </summary>
        /// <param name="iterations"></param>

        private static SmallTalk_Module module;
        private static SmallTalk_Parser parser;
        private static SmallTalk_Controller c;

        public static void Setup() {
            module = new SmallTalk_Module(Kati.SourceFiles.Constants.smallTalk);
            parser = new SmallTalk_Parser(module);
            c = new SmallTalk_Controller(module);
            c._GameData = SetupGameData();
            c._CharacterData = SetupCharacterData();
            c.SetupConversation(c._GameData, c._CharacterData);
            parser.Ctrl = c;
        }
        public static GameData SetupGameData() {
            GameData data = new GameData();
            data.DayOfMonth = 6;
            data.Sector = "Sector 3";
            data.Season = "Spring";
            data.TimeOfDay = "evening";
            data.Weather = "nice_day";
            data.SetDayOfWeek();
            data.SetWeek();
            data.EventCalendar["Spring"]["Art_Fest"] = 12;
            data.EventCalendar["Spring"]["Blueberry_Fest"] = 21;
            data.SetPublicEvent();
            return data;
        }
        public static CharacterData SetupCharacterData() {
            CharacterData data = new CharacterData();
            data.InitiatorsName = "Kati"; data.InitialorsGender = "Female";
            data.RespondersName = "Stephen"; data.InitialorsGender = "male";
            data.InteractionTone = SetTone();
            data.InitiatorsTone = SetTone();
            data.RespondersTone = SetTone();
            data.InitiatorPersonalList["loves_art_fest"] = "characterTrait";
            data.ResponderAttributeList["loves_art_fest"] = "characterTrait";
            data.InitiatorScalarList["charming"] = 200;
            data.ResponderScalarList["charming"] = 200;
            return data;
        }
        public static Dictionary<string, double> SetTone(int att) {
            string[] str = { "romance", "friend","professional","respect",
                             "Admiration","disgust","hate","rivalry"};
            Dictionary<string, double> temp = new Dictionary<string, double>();
            for (int i = 0; i < str.Length; i++) {
                if (i != att)
                    temp[str[i]] = 0;
                else
                    temp[str[i]] = 1.0;
            }
            return temp;
        }


        public static void TestController(int iterations) {
            Setup();
            ModuleDialoguePackage mod;
            for (int i = 0; i < iterations; i++) {
                Console.WriteLine("\n----------------------------Conversation " + (i + 1) + "---------------------------");
                mod = c.RunFirstRound();
                do {
                    PrintPackage(mod);
                    mod = c.RunNextRound();
                } while (!c.EndConversation);
                c.ResetSmallTalk();
                Console.WriteLine("---------------------------------------------------------------------\n");
            }
        }

        private static void PrintPackage(ModuleDialoguePackage mod) {
            Console.WriteLine("######################### Mod Data ##############################");
            Console.WriteLine(mod.ToString());
            Console.WriteLine("######################### CTRL Data #############################");
            string output;
            output = "EndConversation:     " + c.EndConversation + "\n\n";
            output += "Greeting discussed: " + c.ConversationTopicsDiscussed["greeting"] + "\n";
            output += "Weather discussed:  " + c.ConversationTopicsDiscussed["weather"] + "\n";
            output += "Event discussed:    " + c.ConversationTopicsDiscussed["event"] + "\n\n";
            output += "Conversation Topic: " + c.Topic + "\n";
            output += "Dialogue type:      " + c.DialogueState + "\n";
            output += "Speaker:            " + c.Speaking + "\n";
            Console.WriteLine(output);
        }

        public static void TestAB() {
            Setup();
            parser.SetStage("initiator", "question", "event");
            string question = parser.GetDialogue();
            parser.SetStage("responder", "response", "event");
            //parser.ResponseType = "goingToCurrentEvent";
            var response = parser.ParseResponse();
            Console.WriteLine(question);
            Console.WriteLine(parser.ResponseType);
            foreach (string key in response.Keys) {
                Console.WriteLine(key);
            }
        }


        public static void TestEventResponse2() {
            Setup();
            var verbose = parser.ModuleDictDeepCopy(module.eventResponse);
            parser.ResponseType = "goingToCurrentEvent";
            verbose = parser.NarrowEventResponses(verbose);
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> item in verbose) {
                foreach (KeyValuePair<string, Dictionary<string, List<string>>> inner in verbose[item.Key]) {
                    string words = verbose[item.Key][inner.Key]["req"][0];
                    if (!words.Equals(parser.ResponseType)) {
                        verbose[item.Key].Remove(inner.Key);
                        Console.WriteLine("Removed: "+item.Key + " \"req\" " + words + ":\n\t" + inner.Key);
                    } else {
                        Console.WriteLine("Keeping:"+item.Key + " \"req\" " + words + ":\n\t" + inner.Key);
                    }
                }
            }

        }

        public static void TestSmallTalkEventResponse(int iterations) {
            Setup();
            int counter = 0;
            for (int i = 0; i < iterations; ++i) {
                parser.ResponseType = "likeCurrentEvent";
                var response = parser.ParseEventResponse();
                if (response.Count != 4) {
                    Console.WriteLine("likeCurrentEvent");
                    foreach (KeyValuePair<string, List<string>> r in response) {
                        string req = "";
                        if (r.Value.Count > 0)
                            req = r.Value[0];
                        Console.WriteLine(r.Key + " " + req);
                    }
                    Console.WriteLine();
                }
                parser.ResponseType = "goingToCurrentEvent";
                response = parser.ParseEventResponse();
                if (response.Count != 4) {
                    counter++;
                    Console.WriteLine("goingToCurrentEvent");
                    foreach (KeyValuePair<string, List<string>> r in response) {
                        string req = "";
                        if (r.Value.Count > 0)
                            req = r.Value[0];
                        Console.WriteLine(r.Key + " " + req);
                    }
                    Console.WriteLine();
                }
            }
            Console.WriteLine(counter+" out of "+iterations);
        }

        public static void TestSmallTalkResponse(int iterations) {
            Setup();
            for (int i = 0; i < iterations; ++i) {
                parser.ResponseType= "likeCurrentEvent";
                //var response = parser.ParseWeatherResponse();
                //foreach (KeyValuePair<string, List<string>> r in response) {
                //    string req = "";
                //    if (r.Value.Count > 0)
                //        req = r.Value[0];
                //    Console.WriteLine(r.Key+" "+req);
                //}
                var response = parser.ParseEventResponse();
                foreach (KeyValuePair<string, List<string>> r in response) {
                    string req = "";
                    if (r.Value.Count > 0)
                        req = r.Value[0];
                    Console.WriteLine(r.Key + " " + req);
                }
                //response = parser.ParseGreetingResponse();
                //foreach (KeyValuePair<string, List<string>> r in response) {
                //    string req = "";
                //    if (r.Value.Count > 0)
                //        req = r.Value[0];
                //    Console.WriteLine(r.Key + " " + req);
                //}
                Console.WriteLine("\n");
            }
            
        }

        public static void TestSmallTalkText(int length) {
            for (int i = 0; i < length; i++) {
                Setup();
                Console.WriteLine("\n Round " + (i + 1) + "\n");
                for (int j = 0; j < 8; j++) {
                    parser.Ctrl._CharacterData.InitiatorsTone = SetTone(j);
                    PrintAttributeBranch(j);
                    //PrintCharacter(parser.Ctrl._CharacterData);
                    Console.WriteLine("Weather");
                    parser.SetStage("initiator", "statement", "weather");
                    Console.WriteLine("Statement: " + parser.GetDialogue());
                    parser.SetStage("initiator", "question", "weather");
                    Console.WriteLine("Question: " + parser.GetDialogue());
                    Console.WriteLine("Event");
                    parser.SetStage("initiator", "statement", "event");
                    Console.WriteLine("Statement: " + parser.GetDialogue());
                    parser.SetStage("initiator", "question", "event");
                    Console.WriteLine("Question: " + parser.GetDialogue());
                    Console.WriteLine("Greeting");
                    parser.SetStage("initiator", "statement", "greeting");
                    Console.WriteLine("Statement: " + parser.GetDialogue());
                    parser.SetStage("initiator", "question", "greeting");
                    Console.WriteLine("Question: " + parser.GetDialogue());
                    Console.WriteLine("\n");
                }
            }
            Console.WriteLine("ran "+(length)+" times.");
        }

        public static void PrintCharacter(CharacterData  npc) {
            Console.WriteLine("Initiator: " + npc.InitiatorsName);
            foreach (KeyValuePair<string, double> item in npc.InitiatorsTone) {
                int value = (int)(item.Value * 100);
                Console.WriteLine(item.Key + ": " + value);
            }
            //Console.WriteLine("Responder: "+npc.RespondersName);
            //foreach (KeyValuePair<string, double> item in npc.RespondersTone) {
            //    int value = (int)(item.Value * 100);
            //    Console.WriteLine(item.Key+": "+value);    
            //}
        }

        

        public static void PrintAttributeBranch(int att) {
            if (att == 0)
                Console.WriteLine("romance");
            else if (att == 1)
                Console.WriteLine("friend");
            else if (att == 2)
                Console.WriteLine("professional");
            else if (att == 3)
                Console.WriteLine("respect");
            else if (att == 4)
                Console.WriteLine("Admiration");
            else if (att == 5)
                Console.WriteLine("disgust");
            else if (att == 6)
                Console.WriteLine("hate");
            else if (att == 7)
                Console.WriteLine("rivalry");
        }

        public static Dictionary<string, double> SetTone() {
            Random dice = new Random();
            string[] str = { "romance", "friend","professional","respect",
                             "Admiration","disgust","hate","rivalry"};
            Dictionary<string, double> temp = new Dictionary<string, double>();
            for (int i = 0; i < str.Length; i++)
                temp[str[i]] = dice.NextDouble();
            return temp;
        }

        public static void TestGameAndCharacterData() {
            GameInput gameInput = new GameInput();
            GameObject data = gameInput.CreateGameObject();
            Character temp = Character.GenerateCharacter();
            //Console.WriteLine(temp.ToString());
            //Console.WriteLine(data.ToString());
            GameData gameData = new GameData();
            gameData.Season = "Spring";
            gameData.EventCalendar["Spring"] = new Dictionary<string, int>();
            gameData.EventCalendar["Spring"]["Art_Fest"] = 12;
            gameData.EventCalendar["Spring"]["Blueberry_Fest"] = 21;
            gameData.EventCalendar["Fall"] = new Dictionary<string, int>();
            gameData.EventCalendar["Fall"]["Holloween"] = 28;
            gameData.DayOfMonth = 3;
            gameData.SetPublicEvent();
            Console.WriteLine(gameData.PublicEvent);
        }
        */
    }
}
