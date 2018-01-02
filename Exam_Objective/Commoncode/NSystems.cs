using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NSystems
{
    namespace Collections
    {
        public static class Lists
        {
            public static void Shuff<T>(this IList<T> list)
            {
                Random rnd = new Random();
                for (var i = 0; i < list.Count; i++)
                    list.Swap(i, rnd.Next(i, list.Count));
            }
            private static void Swap<T>(this IList<T> list, int i, int j)
            {
                var temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }
}
