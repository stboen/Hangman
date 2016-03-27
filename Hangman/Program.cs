using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangman
{
    class Program
    {

        static void OnRepeatedGuess(object sender, char letter)
        {
            Console.WriteLine("You have guessed {0} before!", letter);
        }

        static void OnWrongGuess(object sender, char letter)
        {
            Hangman hangman = sender as Hangman;
            if (hangman != null)
            {
                Console.WriteLine("The word does not contain {0}.", letter);
                Console.WriteLine("You have {0} mistakes remaining.", hangman.GuessesRemaining);
            }
        }


        static void OnCorrectGuess(object sender,char letter)
        {
            Hangman hangman = sender as Hangman;
            if(hangman!=null)
            {
                Console.WriteLine("Correct  ");
 
            }
        }


        static void OnGameLoss(object sender, char letter)
        {
            Hangman hangman = sender as Hangman;
            if (hangman != null)
            {
                Console.WriteLine("The word does not contain {0}.", letter);
                Console.WriteLine("You Lost! The word was {0}.", hangman.Word);
            }
        }

        static void OnGameWin(object sender, char letter)
        {
            Hangman hangman = sender as Hangman;
            if (hangman != null)
            {
                
                Console.WriteLine("Congratulations, you win! The word was {0}.",hangman.Word);
            }
        }


        static void Main(string[] args)
        {

            Console.WriteLine("Welcome to my Hangman game, who are you?");
            string name="";
            while(name=="" || name.Contains(Environment.NewLine))
            {
                name = Console.ReadLine();
            }

            Player player = new Player(name);

            char difficulty = 'e', playAgain = 'y';

            do
            {
                Console.Clear();
                Console.WriteLine("Select the diificulty:\nPress E for easy\nPress M for medium\nPress H for hard");
                difficulty = Char.ToLower(Console.ReadKey().KeyChar);
                GameDifficulty selectedDifficluty = difficulty == 'e' ? GameDifficulty.Easy :
                                                    difficulty == 'm' ? GameDifficulty.Medium :
                                                    difficulty == 'h' ? GameDifficulty.Hard : GameDifficulty.Easy;

                Console.WriteLine("Good luck, {0}, this is the word:\n\n", player.Name);
                Hangman game = new Hangman(player, selectedDifficluty);
                game.RepeatedGuess += OnRepeatedGuess;
                game.WrongGuess += OnWrongGuess;
                game.CorrectGuess += OnCorrectGuess;
                game.GameLoss += OnGameLoss;
                game.GameWin += OnGameWin;


                while (game.Status == GameStatus.Running)
                {
                    Console.WriteLine(string.Join(" ", game.DisplayedWord));
                    Console.WriteLine();
                    Console.WriteLine("Make a Guess: ");
                    var input = Console.ReadKey(true);
                    game.Guess(char.ToLower(input.KeyChar));

                }
                Console.WriteLine("Do you want to play again?\n Y/N for yes/no");
                playAgain = Char.ToLower(Console.ReadKey().KeyChar);
                
            }
            while ((difficulty == 'e' || difficulty == 'm' || difficulty == 'h') && playAgain == 'y');
            Console.ReadKey();
        }

    }
}
