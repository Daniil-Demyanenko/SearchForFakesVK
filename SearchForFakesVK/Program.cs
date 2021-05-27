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
                Searcher = new FakeSearcher(Token); //Создаём объект поисковика
                Log.PrintLine("Токен успешно найден!");
            }
            else
            {
                Err.PrintLine("Создайте документ \"VK_TOKEN.txt\" рядом с исполняемым файлом и скопируйте в него свой " +
                  "токен ВКонтакте. \nСохраните этот файл в формате UTF-8.");
                return;
            }

            Dialog.Print("Введите id пользователя ВКонтакте, которого нужно исследовать на фейковые страницы.\n" +
                "(только цифровой id)\n>> ");
            User_id = int.Parse(Console.ReadLine());

            Dialog.Print("Использовать \"Быстрый поиск\" (игнорирует группы, в которых больше SearchLimit участников)?\n" +
            "y/n >> ");
            Searcher.FastSearch = Console.ReadLine().ToLower() == "n" ? false : true;

            if(Searcher.FastSearch)
            {
                Dialog.Print("Введите максимальное количество участников группы \n(будут игнорироваться группы с кол-вом участников выше этого числа).\n"+
                $"По умолчанию SearchLimit = {Searcher.SearchLimit} \n>> ");
                string input = Console.ReadLine().Trim();
                if(input.Length > 0)
                Searcher.SearchLimit = int.Parse(input);
            }


            Searcher.NeedLog = true;                //включаем лог
            Searcher.Log = (a) => Log.PrintLine(a); //выводить будем в консоль

            Dialog.PrintLine("Страницы могут быть не найдены, если настройки приватности ииследуемой страницы запрещают просмотр групп.");
            Log.PrintLine("Начато сканирование. Это может занять несколько минут.");

            var users = Searcher.InformativePredict(User_id);

            Dialog.PrintLine($"Всего у пользователя {Searcher.UserGroupCount} групп.\n"+
                $"Из них проанализированно {Searcher.UserGroupAnalyzed}.");
            Dialog.PrintLine($"Кол-во совпавшах групп и id страниц: {users.Count}");

            for (int i = 0; i < users.Count; i+= 10)
            {
                for (int j = i; j < i + 10 && j < users.Count; j++)
                    Dialog.PrintLine($"{users[j].count}\tid{users[j].user_id}");
                Log.PrintLine($"Нажмите Enter, чтоб вывести ещё 10 страниц из {users.Count - 10*i}...");
                Console.ReadLine();
            }

            Log.PrintLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }

        static string GetVKToken()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (File.Exists(path+"/VK_TOKEN.txt"))
                return File.ReadAllText(path+"/VK_TOKEN.txt").Trim();
            else return "";
        }
    }
}

