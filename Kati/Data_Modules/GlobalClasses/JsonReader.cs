using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kati.Data_Modules.GlobalClasses {
    public class JsonReader {

        private static JsonReader reader;

        private static void InstantiateJsonReader() {
            if (reader == null) {
                reader = new JsonReader();
            }
        }

        /*Starts the loading sequence*/
        public static void LoadFromFile(I_Module module) {
            InstantiateJsonReader();
            reader.ConvertJSONtoQuery(module);
            ((ModuleLib)module).SetLibrary(reader.Data);
            var keys = reader.SetAndReturnKeys();
            ((ModuleLib)module).SetConversationTypeKeys(keys);
        }

        public static void ShallowCopyDictionariesByType
            (Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> dict, string key) {
            dict = reader.data[key];
        }

        public static void DeepCopyDictionariesByType
            (Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> dict, string key) {
            dict = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> s1 in reader.data[key]) {
                dict[s1.Key] = new Dictionary<string, Dictionary<string, List<string>>>();
                foreach (KeyValuePair<string,Dictionary<string, List<string>>>  s2 in reader.data[key][s1.Key]) {
                    dict[s1.Key][s2.Key] = new Dictionary<string, List<string>>();
                    foreach (KeyValuePair<string, List<string>> s3 in reader.data[key][s1.Key][s2.Key]) {
                        dict[s1.Key][s2.Key][s3.Key] = new List<string>();
                        foreach (string element in reader.data[key][s1.Key][s2.Key][s3.Key]) {
                            dict[s1.Key][s2.Key][s3.Key].Add(element);
                        }
                    }
                }
            }
        }
        
        public static void ShallowCopyCompleteDictionary
            (Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> dict) {
            dict = reader.data;
        }

        public static void DeepCopyCompleteDictionary
            (Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> dict) {
            dict = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>();
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> s0 in reader.data) { 
                dict[s0.Key] = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
                foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, List<string>>>> s1 in reader.data[s0.Key]) {
                    dict[s0.Key][s1.Key] = new Dictionary<string, Dictionary<string, List<string>>>();
                    foreach (KeyValuePair<string, Dictionary<string, List<string>>> s2 in reader.data[s0.Key][s1.Key]) {
                        dict[s0.Key][s1.Key][s2.Key] = new Dictionary<string, List<string>>();
                        foreach (KeyValuePair<string, List<string>> s3 in reader.data[s0.Key][s1.Key][s2.Key]) {
                            dict[s0.Key][s1.Key][s2.Key][s3.Key] = new List<string>();
                            foreach (string element in reader.data[s0.Key][s1.Key][s2.Key][s3.Key]) {
                                dict[s0.Key][s1.Key][s2.Key][s3.Key].Add(element);
                            }
                        }
                    }
                }
            }
        }


        private Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> data;
        
        public Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> Data { get => data; }

        private JsonReader() { }

        
        /// <summary>
        /// converts a file into a string.  Requires path.
        /// </summary>
        /// <param name="filePaht">File to be read</param>
        /// <returns>files content as String</returns>
        private string ReadFile(string filePath) {
            using StreamReader reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        private void ConvertJSONtoQuery(I_Module module) {
            try {
                ConvertJsonToDictionary(module);
            } catch (Exception e) {
                Console.WriteLine(e);
                return;
            }
        }

        //returns a list of topic keys to the data 
        //Format: { "dream" : ["dream_statement", "dream_question", "dream_response"]
        public Dictionary<string, List<string>> SetAndReturnKeys() {
            Dictionary<string, List<string>> keys = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> item in data) {
                string temp = item.Key.Split("_")[0];
                if (keys.ContainsKey(temp)) {
                    keys[temp].Add(item.Key);
                } else {
                    keys[temp] = new List<string>();
                    keys[temp].Add(item.Key);
                }
            }
            return keys;
        }

        private void ConvertJsonToDictionary(I_Module module) {
            string json = ReadFile(module.getPathToJson());
            Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>> data =
                JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>>>(json);
            this.data = data;
        }
        
    }
}
