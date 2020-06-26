using Kati.Module_Hub;
using System;
using System.Collections.Generic;

namespace Kati
{
    class Program{

		
        static void Main(string[] args){
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


    }
}
