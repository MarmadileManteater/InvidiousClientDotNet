using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousCaption
    {
        private readonly JObject _data;
        internal InvidiousCaption(JObject? instanceObject)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
        }
        public string Label
        {
            get
            {
                string? result = _data["label"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string LanguageCode
        {
            get
            {
                string? result = _data["language_code"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string Url
        {
            get
            {
                string? server = _data["_server"]?.Value<string>();
                string? result = _data["url"]?.Value<string>();
                if (server != null && result != null)
                {
                    if (!server.StartsWith("https://") && !server.StartsWith("http://"))
                    {
                        return $"{server}{result}";
                    }
                    else
                    {
                        return result;
                    }
                }
                return "";
            }
        }
    }
}
