using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Objects.Data;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Extensions
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

        public static bool IsVideo(this JObject jObject)
        {
            return jObject?["type"]?.Value<string>() == "video";
        }
        public static InvidiousChannelVideo ToVideo(this JObject jObject)
        {
            return new InvidiousChannelVideo(jObject);
        }
        public static bool IsPlaylist(this JObject jObject)
        {
            return jObject?["type"]?.Value<string>() == "playlist";
        }
        public static InvidiousPlaylist ToPlaylist(this JObject jObject)
        {
            return new InvidiousPlaylist(jObject);
        }

        public static bool IsChannel(this JObject jObject)
        {
            return jObject?["type"]?.Value<string>() == "channel";
        }
        public static InvidiousChannel ToChannel(this JObject jObject)
        {
            return new InvidiousChannel(jObject);
        }
    }
}
