using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

//https://vkhost.github.io/

namespace SFFVK_lib
{
    public class FakeSearcher
    {
        static readonly HttpClient client = new HttpClient();
        private string TOKEN;
        private const string CLIENT_ID = "6121396";

        public FakeSearcher(string VK_Token)
        {
            TOKEN = VK_Token;
        }

        private async Task<GroupResponse> GetUserGroups(string User_id)
        {
            string Json = await client.GetStringAsync($"https://api.vk.com/method/groups.get?user_id={User_id}&v=5.131&access_token={TOKEN}");
            return JsonSerializer.Deserialize<GroupResponse>(Json);
        }

        private async Task<List<int>> GetGroupMembers(string Group_id)
        {
            //Узнаём кол-во участников в группе
            string GetById = await client.GetStringAsync($"https://api.vk.com/method/groups.getById?group_id={Group_id}&fields=members_count&v=5.131&access_token={TOKEN}");
            int count = (int)JsonSerializer.Deserialize<GroupsGetByIdResponse>(GetById).response[0].members_count;
            List<int> Result = new List<int>();

            //Составляем список участников группы
            for (int i = 0; i < count; i += 1000)
            {
                string Json = await client.GetStringAsync($"https://api.vk.com/method/groups.getMembers?group_id={Group_id}&offset={i}&v=5.131&access_token={TOKEN}");
                var response = JsonSerializer.Deserialize<GroupResponse>(Json).response;
                Result.AddRange(response.items);
            }

            return Result;
        }

    }
}
