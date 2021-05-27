using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;


//https://vkhost.github.io/

namespace SFFVK_lib
{
    public class FakeSearcher
    {
        static readonly HttpClient client = new HttpClient();
        private string TOKEN;

        private float dropout = 0.10f;
        /// <summary>
        /// Исключает пользователей, у которых меньше DROPOUT (процентов) общих групп.
        /// Dropout принадлежит [0,1].
        /// </summary>
        public float Dropout
        {
            get { return Dropout; }
            set
            {
                if (value >= 0 || value <= 1) dropout = value;
                else dropout = value < 0 ? 0 : 0.25f;
            }
        }


        public delegate void Logger(string log);
        public Logger Log;
        public bool NeedLog = false;

        /// <summary>
        /// Ускоряет поиск, игнорируя группы, в которых больше SearchLimit пользователей
        /// </summary>
        public bool FastSearch = true;
        /// <summary>
        /// При включенном FastSearch, устанавливает максимальное число участников группы
        /// </summary>
        public int SearchLimit = 120000;

        /// <summary>
        /// Кол-во групп у исследуемого пользователя
        /// </summary>
        public int UserGroupCount { get; private set; }
        /// <summary>
        /// Кол-во групп пользователя, которые были проанализированны алгоритмом
        /// </summary>
        public int UserGroupAnalyzed { get; private set; } // TODO счётчик проанализированных групп

        public FakeSearcher(string VK_Token)
        {
            TOKEN = VK_Token;
        }

        /// <summary>
        /// Возвращает id групп пользователя
        /// </summary>
        private async Task<GroupResponse> GetUserGroups(int User_id)
        {
            try
            {
                string Json = await client.GetStringAsync($"https://api.vk.com/method/groups.get?user_id={User_id}&v=5.131&access_token={TOKEN}");
                return JsonSerializer.Deserialize<GroupResponse>(Json);
            }
            catch
            {
                if (NeedLog) Log($"Не удалось получить список групп пользователя. Возможно они закрыты настройками приватности");
                throw new Exception("Не удалось получить список групп пользователя. Возможно они защищены настройками приватности");
            }
        }

        /// <summary>
        /// Возвращает id участников группы
        /// </summary>
        private async Task<List<int>> GetGroupMembers(int Group_id)
        {
            //Узнаём кол-во участников в группе
            string GetById = await client.GetStringAsync($"https://api.vk.com/method/groups.getById?group_id={Group_id}&fields=members_count&v=5.131&access_token={TOKEN}");
            int count = (int)JsonSerializer.Deserialize<GroupsGetByIdResponse>(GetById).response[0].members_count;
            List<int> Result = new List<int>();

            if (NeedLog) Log($"Кол-во участников в группе id{Group_id}: {count}.");

            if (FastSearch && count > SearchLimit) //игнорируемые группы при включённом FastSearch
            {
                if (NeedLog) Log($"Пропуск группы. Участников больше SearchLimit.");
                return Result;
            }
            else UserGroupAnalyzed++; // Инкрементируем счётчик проанализированных групп.


            //Составляем список участников группы
            for (int i = 0; i < count; i += 1000)
            {
                try
                {
                    string Json = await client.GetStringAsync($"https://api.vk.com/method/groups.getMembers?group_id={Group_id}&offset={i}&v=5.131&access_token={TOKEN}");
                    var response = JsonSerializer.Deserialize<GroupResponse>(Json).response;
                    if (response.items.Length > 0)
                        Result.AddRange(response.items);

                    if (NeedLog && i % 25000 == 0) Log($"Получено {Result.Count} участников группы из {count}.");
                }
                catch
                {
                    if (NeedLog) Log($"!# Ошибка получения данных пользователей группы!");
                    throw new Exception("Ошибка получения данных пользователей группы");
                }
            }

            if (NeedLog) Log($"Получены все {Result.Count} из {count} участников.");
            return Result;
        }

        /// <summary>
        /// Предсказывает фейковые страницы пользователя (по убыванию вероятности).
        /// </summary>
        /// <param name="User_id">id пользователя</param>
        /// <returns>Объекты c id поользователя и кол-вом совпавших групп</returns>
        public List<UserCounter> InformativePredict(int User_id)
        {
            var UserStatistics = new Dictionary<int, int>();
            GroupResponse UserGroup = GetUserGroups(User_id).Result;
            UserGroupCount = UserGroup.response.count; //Установим кол-во групп пользователя
            UserGroupAnalyzed = 0; // Сбросим счётчик проанализированных групп

            if (NeedLog) Log($"Найдено {UserGroupCount} групп.");

            int c = 1;
            foreach (int i in UserGroup.response.items)
            {
                if (NeedLog) Log($"Анализируется группа № {c}/{UserGroupCount}.");
                c++;

                List<int> id;
                try
                {
                    id = GetGroupMembers(i).Result;
                }
                catch { ShowVKLogErr(); break; }

                if (NeedLog && id.Count > 0) Log($"Анализ подписчиков группы id{i}");
                foreach (var j in id)
                    CountUser(ref UserStatistics, j);

                
            }

            if (NeedLog) Log($"Найдено {UserStatistics.Count} страниц.\n"+
                $"Проанализированно {UserGroupAnalyzed} групп." +
                "\nСортировка страниц по убыванию вероятности и нормализация списка.");

            return ToNormalizeList(UserStatistics);
        }

        /// <summary>
        /// Предсказывает фейковые страницы пользователя (по убыванию вероятности).
        /// </summary>
        /// <param name="User_id">id пользователя</param>
        /// <returns>Список id пользователей</returns>
        public List<int> SimplePredict(int User_id)
        {
            var predict = InformativePredict(User_id);
            List<int> result = new List<int>();

            foreach (var i in predict)
                result.Add(i.user_id);

            return result;
        }

        //Если id есть в списке, то увеличиваем счётчик. Иначе добавляем в список
        private void CountUser(ref Dictionary<int, int> dict, int id)
        {
            if (dict.ContainsKey(id)) dict[id] += 1;
            else dict.Add(id, 1);
        }

        private List<UserCounter> ToNormalizeList(in Dictionary<int, int> dict)
        {
            var list = new List<UserCounter>();
            foreach (var i in dict)
            {
                if (i.Value / (float)UserGroupAnalyzed <= dropout) continue; //Отбрасываем пользователей, которые имеют мало общих групп
                list.Add(new UserCounter(i.Key, i.Value));
            }

            list.Sort((a, b) =>
            {
                if (a.count == b.count) return 0;
                return a.count > b.count ? -1 : 1;
            });

            if (NeedLog) Log($"После нормализации в списке осталось {list.Count} записей");
            return list;
        }

        /// <summary>
        /// Ошибка 29 от ВК. Временный запрет некоторых запросов.
        /// </summary>
        private void ShowVKLogErr() 
        {
            if (NeedLog) Log("\n#############################################\n"+
            "Видимо, превышен лимит запросов к ВК.\n" +
            "Данная ошибка работает как временный бан. Обычно ВК запрещает вызывать определенные методы на время от 4 до 48 часов. " +
            "Ускорить время бана вы не сможете, а вот увеличить - сможете!\n"+
            "Постарайтесь не пользоваться приложением 8-24 часов.");
        }
    }
}
