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
            int User_id;

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

            Dialog.Print("Введите id пользователя ВКонтакте, которого нужно исследовать на фейковые страницы.\n" +
                "(только цифровой id)\n>>");

            try
            {
                User_id = int.Parse(Console.ReadLine());
            }
            catch {
                Err.PrintLine("Введён неверный id");
                return;
            }

            Dialog.PrintLine("Страницы могут быть не найдены, если настройки приватности ииследуемой страницы запрещают просмотр групп.");
            Log.PrintLine("Начато сканирование. Это может занять несколько минут.");

            var users = Searcher.InformativePredict(User_id);

            Dialog.PrintLine($"Всего у пользователя {Searcher.UserGroupCount} групп.");
            Dialog.PrintLine("Кол-во совпавшах групп и id страниц (топ-20):");

            for (int i = 0; i < 20; i++)
            {
                Dialog.PrintLine($"{users[i].count}\t{users[i].user_id}");
            }
            Log.PrintLine("\nНажмите любую клавишу для завершения...");
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

