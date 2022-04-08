using System;
using System.Linq;

namespace BullsAndCows
{
    /// <summary>
    /// Основной рабочий класс программы; точка входа в программу - класс `Main`.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Точка входа в программу; метод спрашивает у пользователя длину загадываемого числа
        /// и запускаем игру до тех пор, пока пользователь не выйдет командой `exit`.
        /// </summary>
        /// <param name="args">---</param>
        static void Main(string[] args)
        {
            Game.PrintIntro();
            
            // Спрашиваем пользователя о кол-ве цифр в числе, обрабатываем некорректный ввод.
            int numberLength;
            Console.WriteLine("Прежде чем начать, скажи, число из скольки цифр мне загадать (от 1 до 10)?");
            while (!(int.TryParse(Console.ReadLine(), out numberLength) && numberLength is >= 1 and <= 10))
                Console.WriteLine("Вводи, пожалуйста, корректное значение: число от 1 до 10.");
            
            // Для доработки: перед началом новой игры предлагать пользователю поменять количество цифр в числе.
            Game game = new Game(numberLength);
            do
            {
                game.Run();
                Console.WriteLine("Хочешь сыграть еще? Нажми Enter для продолжения и напиши `exit`, чтобы выйти.");
            } while (Console.ReadLine() != "exit");
        }

    }

    /// <summary>
    /// Класс, содержащий всю логику игры и вспомогательные методы.
    /// </summary>
    class Game
    {
        // Разделитель текста: длина и символ разделителя, используется для печати в методе `PrintSeparator()`.
        private const int TextSeparatorLength = 50;
        private const string SeparatorSymbol = "=";
        
        // Количество разрядов в загаданном числе
        private int _numberLength;

        // Хранить само число будем в виде массива символов - так его будет удобнее сравнивать.
        private char[] _numberArray;

        // Лучший результат пользователя - минимальное время в секундах, за которое число было угадано.
        private long _bestSecondsResult = 0;

        /// <summary>
        /// Конструктор класса; необходим для создания массива из количества элементов, которое указал пользователь.
        /// </summary>
        /// <param name="numberLength">Количество цифр в загадываемом числе.</param>
        public Game(int numberLength)
        {
            _numberLength = numberLength;
            _numberArray = new char[_numberLength];
        }

        /// <summary>
        /// Реализация всей основной логики: генерация числа, пользовательский ввод.
        /// </summary>
        public void Run()
        {
            PrintSeparator();

            GenerateRandomNumber();
            Console.WriteLine("Начинаем игру! Я загадал число. Твои варианты?");
            
            // Сохраняем текущее время для того, чтобы потом вывести пользователю длительность игры.
            long startUnixTime = DateTimeOffset.Now.ToUnixTimeSeconds();

            char[] userGuessArray = new char[_numberLength];
            // Главный цикл игры: продолжается до тех пор, пока массив загаданного числа не равен массиву пользователя.
            while (!userGuessArray.SequenceEqual(_numberArray))
            {
                userGuessArray = GetUserGuess();
                int bulls = 0;
                int cows = 0;
                for (var i = 0; i < _numberLength; i++)
                {
                    // Логика сравнения проста: если текущий элемент равен элементу с таким же индексом, это бык.
                    if (userGuessArray[i] == _numberArray[i])
                        bulls += 1;
                    // Если же этот элемент просто содержится в загаданном числе, то это корова.
                    else if (_numberArray.Contains(userGuessArray[i]))
                        cows += 1;
                }
                Console.WriteLine($"Коров: {cows}, быков: {bulls}!");
            }
            
            PrintSeparator();

            // Вычитаем из текущего времени стартовое, чтобы посчитать, сколько секунд пользователь угадывал число.
            long guessDuration = DateTimeOffset.Now.ToUnixTimeSeconds() - startUnixTime;
            Console.WriteLine($"Супер! Ты отгадал число за {guessDuration} секунд!");
            // Присвоение нового рекорда в двух случаях: если новый лучше предыдущего или если рекорда ещё нет.
            if (guessDuration < _bestSecondsResult || _bestSecondsResult == 0)
            {
                Console.Write("Это, кстати, твой новый рекорд.");
                Console.WriteLine(_bestSecondsResult != 0 ? 
                    $" Твой предыдущий рекорд - {_bestSecondsResult} секунд." : "");
                _bestSecondsResult = guessDuration;
            }

            PrintSeparator();
        }

        /// <summary>
        /// Выводит обучение игре
        /// </summary>
        public static void PrintIntro()
        {
            PrintSeparator();
            Console.WriteLine("Привет! Это игра \"Быки и коровы\".");
            Console.WriteLine("В чём суть? Я загадаю четырехзначное число, " +
                              "состоящее из разных цифр; твоя задача - угадать его.");
            Console.WriteLine("Предлагай свои варианты, а я подскажу, сколько цифр угадано, " +
                              "но находится не на своих местах (\"коровы\"), и сколько угадано цифр, находящихся на " +
                              "своих местах (\"быки\").");
            PrintSeparator();
        }

        /// <summary>
        /// Печатает разделитель текста для удобства чтения.
        /// </summary>
        private static void PrintSeparator()
        {
            for (var i = 0; i < TextSeparatorLength; i++)
                Console.Write(SeparatorSymbol);
            Console.Write("\n");
        }

        /// <summary>
        /// Генерирует число для игры, записывается в массив `_numberArray`.
        /// </summary>
        private void GenerateRandomNumber()
        {
            var randomInstance = new Random();
            char[] digitsArray = {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
            // Рандомизируем массив цифр и берем из него первые `_numberLength` элементов - это загаданное число.
            do
                digitsArray = digitsArray.OrderBy(_ => randomInstance.Next()).ToArray(); 
            while (digitsArray[0] == '0');

            for (var i = 0; i < _numberLength; i++)
                _numberArray[i] = digitsArray[i];
        }

        /// <summary>
        /// Спрашивает у пользователя число и проверяет его корректность; возвращает массив символов.
        /// </summary>
        /// <returns>Массив символов из числа, которое ввел пользователь</returns>
        private char[] GetUserGuess()
        {
            long userInput;
            // Сразу проверяем, что введеное число корректно и состоит из нужного количества разных цифр.
            while (!long.TryParse(Console.ReadLine(), out userInput) 
                   || userInput.ToString().Length != _numberLength
                   || !userInput.ToString().Distinct().SequenceEqual(userInput.ToString()))
                Console.WriteLine("Ты ввёл некорректное значение: " +
                                  $"оно обязательно должно состоять из {_numberLength} разных цифр(ы).");
            return userInput.ToString().ToCharArray();
        }
    }
}