using System;

namespace Colors
{
    public class Color
    {
        /// <summary>
        /// Фон для TextOut
        /// </summary>
        public ConsoleColor BackText;
        /// <summary>
        /// Цвет текста для TextOut
        /// </summary>
        public ConsoleColor ColorText;

        public Color() 
        {
            ColorText = ConsoleColor.Yellow;
            BackText = ConsoleColor.Black;
        }
        /// <summary>
        /// Цвет вывода
        /// </summary>
        /// <param name="Text">цвет текста</param>
        public Color(ConsoleColor Text)
        {
            ColorText = Text;
            BackText = ConsoleColor.Black;
        }
        /// <summary>
        /// Цвет вывода
        /// </summary>
        /// <param name="Text">цвет текста</param>
        /// <param name="Back">цвет фона</param>
        public Color(ConsoleColor Text, ConsoleColor Back) 
        {
            ColorText = Text;
            BackText = Back;
        }

        /// <summary>
        /// Цветной вывод.
        /// </summary>
        /// <param name="text">Текст.</param>
        public void Print(string text)
        {
            var Back = Console.BackgroundColor;
            var Color = Console.ForegroundColor;
            Console.BackgroundColor = BackText;
            Console.ForegroundColor = ColorText;
            Console.Write(text);
            Console.BackgroundColor = Back;
            Console.ForegroundColor = Color;
        }
        /// <summary>
        /// Цветной вывод.
        /// </summary>
        /// <param name="text">Текст.</param>
        public void PrintLine(string text)
        {
            Print(text + "\n");
        }
    }
}
