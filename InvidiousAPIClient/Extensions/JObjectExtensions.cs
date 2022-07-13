using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace InvidiousAPIClient.Extensions
{
    public static class JObjectExtensions
    {
        public static Dictionary<string, T> ToDictionaryWithStruct<T>(this JObject jObject) where T : struct
        {
            Dictionary<string, T> resultDict = new Dictionary<string, T>();
            foreach (var entry in jObject)
            {
                T? value = entry.Value?.Value<T>();
                if (value != null)
                {
                    resultDict.Add(entry.Key, value.Value);
                }
            }
            return resultDict;

        }
        public static Dictionary<string, T> ToDictionaryWithClass<T>(this JObject jObject) where T : class
        {
            Dictionary<string, T> resultDict = new Dictionary<string, T>();
            foreach (var entry in jObject)
            {
                T? value = entry.Value?.Value<T>();
                if (value != null)
                {
                    resultDict.Add(entry.Key, value);
                }
            }
            return resultDict;

        }
    }
}
