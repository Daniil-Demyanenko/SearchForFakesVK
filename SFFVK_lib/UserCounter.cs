using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFFVK_lib
{
    public class UserCounter
    {
        /// <summary>
        /// id пользователя ВКонтакте
        /// </summary>
        public int user_id;
        /// <summary>
        /// Кол-во совпавших групп с исследуемой страницей
        /// </summary>
        public int count;

        public UserCounter(int id)
        {
            user_id = id;
            count = 1;
        }
    }
}
