namespace AutoMovie
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtension
    {
        /// <summary>
        /// 枚举器中是否存在null条目
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="enumerable">元素枚举</param>
        /// <returns>存在null条目返回true,否则返回false</returns>
        public static bool HaveNullItem<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Any(item => item == null);
        }

        /// <summary>
        /// 枚举器中是否全为指定类型的实例
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="enumerable">元素枚举</param>
        /// <param name="type">指定类型信息</param>
        /// <returns>全为指定类型的实例返回true,否则返回false</returns>
        public static bool IsAllInstanceOfType<T>(this IEnumerable<T> enumerable, Type type)
        {
            return enumerable.All(item => type.IsInstanceOfType(item));
        }
    }
}