using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Kati.Data_Modules{
    /// <summary>
    /// Class contains commomly used words and phrases that are present in all conversations.
    /// contains lists like pronouns
    /// </summary>
    public class GlobalKeys{
        /* Load data through json files */
        /// <summary>
        /// Contains the path+file name to each json file coresponding to each keyword dictionary
        /// </summary>
        public Dictionary<string, string> paths = new Dictionary<string, string>();

        /// <summary>
        /// Contains pronouns listed as: pron1, pron2 ...
        /// </summary>
        public  readonly Dictionary<string, Dictionary<string,List<string>>> pronoun;
        
        
        public GlobalKeys(string path) {
            paths = LoadPaths(path);
            pronoun = Pronoun();
        }

        public GlobalKeys() {
            LoadSinglePath("pronoun",SourceFiles.Constants.pronoun);
            pronoun = Pronoun();
        }
        
        /// <summary>
        /// converts a file into a string.  Requires path.
        /// </summary>
        /// <param name="fileName">File to be read</param>
        /// <returns>files content as String</returns>
        public string ReadFile(string fileName) {
            using StreamReader reader = new StreamReader(fileName);
            return reader.ReadToEnd();
        }

        /*exception handling for pronoun loading*/
        public Dictionary<string, Dictionary<string,List<string>>> Pronoun() {
            if (paths.ContainsKey("pronoun")) {
                try {
                    return LoadPronouns(paths["pronoun"]);
                } catch (Exception e) { Console.WriteLine(e); }
            }
            return null;
        }

        /*Populate pronoun dictionary*/
        public Dictionary<string, Dictionary<string,List<string>>> LoadPronouns(string fileName) {
            string json = ReadFile(fileName);
            Dictionary<string, Dictionary<string, List<string>>> pron = 
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<string>>>>(json); 
            return pron;
        }

        /*populate the paths dictionary*/
        public Dictionary<string, string> LoadPaths(string fileName) {
            string json = ReadFile(fileName);
            Dictionary<string, string> path = 
                JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return path;
        }

        private void LoadSinglePath(string key, string path) {
            paths.Add(key,path);
        }

        
    }
}
