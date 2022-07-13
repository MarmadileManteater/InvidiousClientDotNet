using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace InvidiousAPIClient.Extensions
{
    public static class JArrayExtensions
    {
        public static List<Dictionary<string, T>> ToListOfDictionaryWithClass<T>(this JArray jArray) where T : class
        {
            List<Dictionary<string, T>> resultDict = new List<Dictionary<string, T>>();
            foreach (var entry in jArray)
            {
                JObject? jObject = entry?.Value<JObject>();
                Dictionary<string, T>? dictionary = jObject?.ToDictionaryWithClass<T>();
                if (dictionary != null)
                {
                    resultDict.Add(dictionary);
                }
            }
            return resultDict;
        }
        public static List<Dictionary<string, T>> ToListOfDictionaryWithStruct<T>(this JArray jArray) where T : struct
        {
            List<Dictionary<string, T>> resultDict = new List<Dictionary<string, T>>();
            foreach (var entry in jArray)
            {
                JObject? jObject = entry?.Value<JObject>();
                Dictionary<string, T>? dictionary = jObject?.ToDictionaryWithStruct<T>();
                if (dictionary != null)
                {
                    resultDict.Add(dictionary);
                }
            }
            return resultDict;
        }
        public static JArray ArrayJoin(this JArray arrayA, JArray arrayB)
        {
            foreach (JToken token in arrayB)
            {
                arrayA.Add(token);
            }
            return arrayA;
        }
    }
}
