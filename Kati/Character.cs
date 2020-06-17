using System;
using System.Collections.Generic;
using System.Text;

namespace Kati
{
    /// <summary>
    /// Temp class used for test as a pseudo game
    /// creates a character with stats towards the player 
    /// and stats towards two fake characters Lena and Geoffrey
    /// </summary>
    public class Character
    {
        public static Character GenerateCharacter() {
            Random dice = new Random();
            string[] names = { "Bienvenu", "Blackstone","Boeckman","Champagne","Cole","Coleman","Davis","Evans","Gupta","Jimenez",
                                "Kumar","Marino","Mejia","Mendoza","Muhammad","Nguyen","Nolan","Robertson","Scott","Theriot","Tregre",
                                "Yu","Ty","Hakeem","Christopher","Asia","John","Lane","Aidan","Tyrone","Vishal","Ricardo","Deepanshu",
                                "Amanda","Eric","Carlos","Nooman","Khoa","Katelyn","Jordan","Jared","Austin","Gianna","Andrea","Eric",
                                "Dustine","Mike","Tibby","Lauren","Sam","James","Riley","Carl","Smith","Jonas","Vergios","Miley","Darrel" };
            int index = dice.Next(0,names.Length);
            return GenerateCharacter(names[index]);
        }

        public static Character GenerateCharacter(string name) {
            Random dice = new Random();
            return GenerateCharacter(name, dice.Next(0, 8), dice.Next(0, 8), dice.Next(0, 8));
        }

        public static Character GenerateCharacter(String name, int leanaType, int geofType, int playerType) {
            Character character = new Character(name);
            character.setStats("Leana",(character.dice.Next(1,6)%2==1),leanaType);
            character.setStats("Geoffrey",(character.dice.Next(1,6)%2==1),geofType);
            character.setStats("Player",(character.dice.Next(1,6)%2==1),playerType);
            return character;
        }

        public const string POSITIVE = "positive";
        public const string NEGATIVE = "negative";
        public const string NEUTRAL = "neutral";
        public const string LOVE = "love";


        public const int ROMANCE = 0;
        public const int FRIENDSHIP = 1;
        public const int PROFESSIONAL = 2;
        public const int RESPECT = 3;
        public const int AFFINITY = 4;
        public const int HATRED = 5;
        public const int DISGUST = 6;
        public const int RIVALRY = 7;

        private Dictionary<int, string[]> attributes;
        private int[] leana = new int[8];
        private int[] geoffrey = new int[8];
        private int[] stats = new int[8];
        private string name;
        private Random dice;
        private string x, y;
    

        public Character(string name) {
            dice = new Random();
            this.name = name;
        }
    
        public void setStats(string character, bool isPositive, int type) {
            int[] arr;
            if (character.Equals("Leana")) {
                arr = leana;
            } else if (character.Equals("Geoffrey")) {
                arr = geoffrey;
            } else {
                arr = stats;
            }
            if (isPositive) {
                generatePositive(arr, type);
            } else {
                generateRandom(arr);
            }

        }

        public int[] getLeana() { return leana; }
        public int[] getGeoff() { return geoffrey; }
        public string getName() { return name; }
        public void setStatusPositive(string pc) {
            if (pc.Equals("x")) { x = POSITIVE; } else { y = POSITIVE; }
        }
        public void setStatusNegative(String pc) {
            if (pc.Equals("x")) { x = NEGATIVE; } else { y = NEGATIVE; }
        }
        public void setStatusNeutral(String pc) {
            if (pc.Equals("x")) { x = NEUTRAL; } else { y = NEUTRAL; }
        }
        public void setStatusLove(String pc) {
            if (pc.Equals("x")) { x = LOVE; } else { y = LOVE; }
        }

        private void generatePositive(int[] arr, int type) {
            arr[ROMANCE] = type == ROMANCE ? high() : low();
            arr[FRIENDSHIP] = type == FRIENDSHIP ? high() : low();
            arr[PROFESSIONAL] = type == PROFESSIONAL ? high() : low();
            arr[RESPECT] = type == RESPECT ? high() : low();
            arr[AFFINITY] = type == AFFINITY ? high() : low();
            arr[HATRED] = type == HATRED ? high() : low();
            arr[RIVALRY] = type == RIVALRY ? high() : low(); low();
            arr[DISGUST] = type == DISGUST ? high() : low();
        }

        private void generateRandom(int[] arr) {
            for (int i = 0; i < arr.Length; i++) {
                int num = (int)(dice.Next(0,3));
                if (num == 0)
                    arr[i] = gaus();
                else
                    arr[i] = low();
            }
        }

        public void custom(int[] stats, String character) {
            if (character.Equals("Leana")) {
                leana = stats;
            } else {
                geoffrey = stats;
            }
        }

        private int gaus() { return dice.Next(1, 6) + dice.Next(1,6); }
        private int low() { return dice.Next(1, 5); }
        private int high() { return dice.Next(5, 11); }

        override
        public string ToString() {
            string str = name + "\n";
            str += StatsString("Leana");
            str += StatsString("Geoff");
            str += StatsString("Player");
            return str;
        }

        private string StatsString(string name) {
            int[] arr;
            string str = "";
            if (name.Equals("Leana")) {
                arr = leana;
            } else if (name.Equals("Geoff")) {
                arr = geoffrey;
            } else {
                arr = stats;
            }
            str += name + "\tRom: " + arr[ROMANCE] + " FRD: " + arr[FRIENDSHIP] + " Pro: " + arr[PROFESSIONAL] + " Res: " + arr[RESPECT];
            str += " Adm: " + arr[AFFINITY] + " Hat: " + arr[HATRED] + " Riv: " + arr[RIVALRY] + " dis: " + arr[DISGUST]+"\n";
            return str;
        }

        private void setupAttributeLists() {
            attributes = new Dictionary<int, string[]>();
            attributes[1] = new string[] { "loves_art_fest", "hates_art_fest", "" };
            attributes[2] = new string[] { "loves_blueberry_fest", "hates_blueberry_fest", "" };
            attributes[3] = new string[] { "loves_writers_block", "hates_writers_block", "" };
            attributes[4] = new string[] { "loves_music_fest", "hates_music_fest", "" };
            attributes[5] = new string[] { "loves_halloween", "hates_halloween", "" };
            attributes[6] = new string[] { "loves_halloween", "hates_halloween", "" };
            attributes[7] = new string[] { "loves_bazaar", "hates_bazaar", "" };
            attributes[8] = new string[] { "loves_yuletide", "hates_yuletide", "" };


        }

    }
}

