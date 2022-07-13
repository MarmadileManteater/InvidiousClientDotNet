using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Extensions;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousInstanceStats
    {
        private readonly JObject _data;
        internal InvidiousInstanceStats(JObject? instanceObject)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
        }
        public virtual string Version
        {
            get
            {
                string? result = _data["version"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual Dictionary<string, string> Software
        {
            get
            {
                JObject? result = _data["software"]?.Value<JObject>();
                if (result != null)
                {
                    return result.ToDictionaryWithClass<string>();
                }
                return new Dictionary<string, string>();
            }
        }
        public virtual bool OpenRegistrations
        {
            get
            {
                return _data["openRegistrations"]?.Value<bool>() == true;
            }
        }
        public virtual Dictionary<string, string> Usage
        {
            get
            {
                JObject? result = _data["usage"]?["users"]?.Value<JObject>();
                if (result != null)
                {
                    return result.ToDictionaryWithClass<string>();
                }
                return new Dictionary<string, string>();
            }
        }
        public virtual Dictionary<string, int> Metadata
        {
            get
            {
                JObject? result = _data["metadata"]?.Value<JObject>();
                if (result != null)
                {
                    return result.ToDictionaryWithStruct<int>();
                }
                return new Dictionary<string, int>();
            }
        }
    }
}
