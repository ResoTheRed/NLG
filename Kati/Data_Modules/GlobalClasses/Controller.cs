﻿using Kati.Module_Hub;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kati.Data_Modules.GlobalClasses {
    /// <summary>
    /// Generic parent of all Module Controllers
    /// </summary>
    public class Controller {

        private ModuleLib lib;
        private Parser parser;
        private GameData game;
        private CharacterData npc;


        public Controller(string path) {
            Lib = new ModuleLib(path);
            Game = new GameData();
            Npc = new CharacterData();
        }

        public ModuleLib Lib { get => lib; set => lib = value; }
        public GameData Game { get => game; set => game = value; }
        public CharacterData Npc { get => npc; set => npc = value; }
        internal Parser Parser { get => parser; set => parser = value; }
        //need to update game data
        //need to update character data
        //need to decide which topic
        //need to decide which type of topic 

        //a list of dialogue topics can be found in Lib.Keys
        
        //overridable method that decides which topic to talk about

    }
}