using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Extensions;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousInstanceMonitor
    {
        private readonly JObject _data;
        internal InvidiousInstanceMonitor(JObject? instanceObject)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
        }

        public int MonitorId
        {
            get
            {
                int? result = _data["monitorId"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public int CreatedAt
        {
            get
            {
                int? result = _data["createdAt"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public string StatusClass
        {
            get
            {
                string? result = _data["statusClass"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string Name
        {
            get
            {
                string? result = _data["name"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string? Url
        {
            get
            {
                // this value is supposed to be nullable
                return _data["url"]?.Value<string>();
            }
        }
        public string Type
        {
            get
            {
                string? result = _data["type"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public List<Dictionary<string, string>> DailyRatios
        {
            get
            {
                JArray? result = _data["dailyRatios"]?.Value<JArray>();
                if (result != null)
                {
                    return result.ToListOfDictionaryWithClass<string>();
                }
                return new List<Dictionary<string, string>>();
            }
        }
        public Dictionary<string, string> Ratio90d
        {
            get
            {
                JObject? result = _data["90dRatio"]?.Value<JObject>();
                if (result != null)
                {
                    return result.ToDictionaryWithClass<string>();
                }
                return new Dictionary<string, string>();
            }
        }
        public Dictionary<string, string> Ratio30d
        {
            get
            {
                JObject? result = _data["30dRatio"]?.Value<JObject>();
                if (result != null)
                {
                    return result.ToDictionaryWithClass<string>();
                }
                return new Dictionary<string, string>();
            }
        }
    }
}
