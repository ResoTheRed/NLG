﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Module_Hub {
    /// <summary>
    /// Stat that the current module is at after each dialogue retrieval
    /// RETURN: must return to the active module
    /// CONTINUE: can return or exit active module
    /// EXIT: must leave current Module
    /// </summary>
    public enum ModuleStatus{ 
        RETURN,
        CONTINUE,
        EXIT
    }
    /// <summary>
    /// Class provides a way to package data from each module to the hub and untimately to the game
    /// </summary>
    public class ModuleDialoguePackage {

        private Dictionary<string, List<string>> dialogueAndEffects;
        private string moduleName;
        private ModuleStatus status;

        public ModuleDialoguePackage(Dictionary<string, List<string>> data, string name) {
            DialogueAndEffects = data;
            ModuleName = name;
        }
        public ModuleDialoguePackage(Dictionary<string, List<string>> data, string name, ModuleStatus state) {
            DialogueAndEffects = data;
            ModuleName = name;
            status = state;
        }

        public Dictionary<string, List<string>> DialogueAndEffects { get => dialogueAndEffects; set => dialogueAndEffects = value; }
        public string ModuleName { get => moduleName; set => moduleName = value; }
        public ModuleStatus Status { get => status; set => status = value; }
    }
}
