using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hangman
{
    class Hangman
    {
        public event EventHandler<char> RepeatedGuess;
        public event EventHandler<char> WrongGuess;
        public event EventHandler<char> CorrectGuess;
        public event EventHandler<char> GameWin;
        public event EventHandler<char> GameLoss;


        public Hangman(Player player, GameDifficulty difficulty=GameDifficulty.Easy)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));
            
            Status = GameStatus.Running;
            Player = player;
            Difficulty = difficulty;
            if (Difficulty == GameDifficulty.Easy) maxWrongGuesses = 10;
            else if (Difficulty == GameDifficulty.Medium) maxWrongGuesses = 7;
            else if (Difficulty == GameDifficulty.Hard) maxWrongGuesses = 5;
            AllWords = new List<string>(GetAllWords());
            Word = GetGameWord();

            DisplayedWord = new char[Word.Length];
            for (int i = 0; i < DisplayedWord.Length; i++)
                DisplayedWord[i] = '_';
        }

        public Hangman(): this(new Player(),GameDifficulty.Easy)
        {

        }

        public Player Player { get; }

        public GameDifficulty Difficulty { get; private set; }
       
        public string Word { get; }

        public char[] DisplayedWord { get; private set; }

        public GameStatus Status { get; private set; }
        
        public int GuessesRemaining
        {
            get
            {
                return maxWrongGuesses - wrongGuesses;
            }
        }
        public List<string> AllWords { get; private set; }

        private int maxWrongGuesses;
        private HashSet<char> guessedLetters = new HashSet<char>();
        private int wrongGuesses = 0;

        private static Random rand = new Random();
        private string GetGameWord()
        {
            return AllWords[rand.Next(AllWords.Count())];
        }

        private List<string> GetAllWords()
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            var names = currentAssembly.GetManifestResourceNames();
            using (var stream = currentAssembly.GetManifestResourceStream("Hangman.wordsEn.txt"))
            using (var reader = new StreamReader(stream))
            {
                List<string> finalList = new List<string>();
                string[] words =
                     reader
                    .ReadToEnd()
                    .Split(
                        new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                int minlength=0, maxlength=0;
                switch (Difficulty)
                {
                    case GameDifficulty.Easy:
                         minlength = 3;maxlength = 6; break;
                    case GameDifficulty.Medium:
                        minlength = 7;maxlength = 10; break;
                    case GameDifficulty.Hard:
                        minlength = 10;maxlength = 20; break;

                }
                foreach (string word in words)
                {
                    if (word.Length >= minlength && word.Length<=maxlength) finalList.Add(word);
                }
                return finalList;

            }

        }


        public void Guess(char letter)
        {
            if (Status != GameStatus.Running)
                throw new InvalidOperationException("Game isn't running");
            if (guessedLetters.Contains(letter))
            {
                if (RepeatedGuess != null)
                    RepeatedGuess(this, letter);
                return;
            }
            guessedLetters.Add(letter);
            
            if(!Word.Contains(letter))
            {
                wrongGuesses++;
                if (wrongGuesses > maxWrongGuesses)
                {
                    Status = GameStatus.Lost;
                    if (GameLoss != null)
                        GameLoss(this, letter);
                    return;                 
                }
                else
                {
                    if (WrongGuess != null)
                        WrongGuess(this, letter);
                }
            }
            else
            {
                RecordGuess(letter);
                if (!DisplayedWord.Contains('_'))
                {
                    Status = GameStatus.Won;
                    if (GameWin != null)
                        GameWin(this, letter);
                    return;
                }
                if (CorrectGuess != null)
                    CorrectGuess(this, letter);
                
            } 
        }

        private void RecordGuess(char letter)
        {
            for(int i = 0;i<Word.Length;i++)
            {
                if (Word[i] == letter)
                    DisplayedWord[i] = letter;
            }
        }
    }
}
