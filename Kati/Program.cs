using System;

namespace Kati
{
    class Program{

		
        static void Main(string[] args){
            GameInput gameInput = new GameInput();
            GameObject data = gameInput.CreateGameObject();
			Character temp = Character.GenerateCharacter();
			Console.WriteLine(temp.ToString());
            Console.WriteLine(data.ToString());
        }


    }
}
