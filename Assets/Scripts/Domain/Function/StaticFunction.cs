using System;
using System.Collections.Generic;

namespace Game.Domain
{
    public static class StaticFunction
    {
        public static void Shuffle<T>(List<T> list, Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);  // 0 ~ i
                (list[i], list[j]) = (list[j], list[i]);  // ½»»»
            }
        }
    }
}