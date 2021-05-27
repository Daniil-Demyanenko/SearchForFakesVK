using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFFVK_lib
{
    public class UserCounter : IComparable
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
        public UserCounter(int id, int count)
        {
            this.user_id = id;
            this.count = count;
        }

        public int CompareTo(object o)
        {
            UserCounter a = o as UserCounter;
            if (a != null)
                return this.user_id.CompareTo(a.user_id);
            else
                throw new Exception("Невозможно сравнить два объекта");
        }
    }
}
