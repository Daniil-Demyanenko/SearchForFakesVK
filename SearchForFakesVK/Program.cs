using System;
using System.IO;
using Colors;
using SFFVK_lib;

namespace SearchForFakesVK
{
    class Program
    {
        static void Main(string[] args)
        {
            Color Err = new Color(ConsoleColor.Red);
            Color Dialog = new Color(ConsoleColor.Cyan);
            Color Log = new Color(ConsoleColor.Blue);

            FakeSearcher Searcher;
            string Token = GetVKToken();
            if (Token != String.Empty)
            {
                Searcher = new FakeSearcher(Token);
                Log.PrintLine("Токен успешно найден!");
            }
            else
            {
                Err.PrintLine("Создайте документ \"VK_TOKEN.txt\" рядом с исполняемым файлом и скопируйте в него свой " +
                  "токен ВКонтакте. \nСохраните этот файл в формате UTF-8.");
                return;
            }

            Console.ReadKey();
        }

        static string GetVKToken()
        {
            if (File.Exists("VK_TOKEN.txt"))
                return File.ReadAllText("VK_TOKEN.txt").Trim();
            else return "";
        }
    }
}

