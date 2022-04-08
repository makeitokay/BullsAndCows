using System;

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

            do
            {
                Console.WriteLine("Прежде чем начать, скажи, число из скольки цифр мне загадать (от 1 до 10)?");
                int numberLength;
                while (!(int.TryParse(Console.ReadLine(), out numberLength) && numberLength is >= 1 and <= 10))
                    Console.WriteLine("Вводи, пожалуйста, корректное значение: число от 1 до 10.");
                var game = new Game(numberLength);
                game.Run();
                Console.WriteLine("Хочешь сыграть еще? Нажми Enter для продолжения и напиши `exit`, чтобы выйти.");
            } while (Console.ReadLine() != "exit");
        }
    }
}