using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AIService.Helper
{
    /// <summary>
    /// IEnumerable&lt;T&gt;的扩展方法 序列化为Json字符串
    /// </summary>
    public static partial class IEnumerableExtensionMethods
    {
        /// <summary>
        /// 序列化为Json字符串
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToJsonString<TSource>(this IEnumerable<TSource> source)
        {
            return JsonConvert.SerializeObject(source);
        }
    }
}
