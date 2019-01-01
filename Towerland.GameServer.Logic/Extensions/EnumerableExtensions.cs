using System.Collections.Generic;

namespace Towerland.GameServer.Logic.Extensions
{
    public static class EnumerableExtensions
    {
        public static IList<T> AddValue<T>(this IList<T> list, T val)
        {
            list.Add(val);
            return list;
        } 
    }
}