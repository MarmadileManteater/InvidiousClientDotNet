using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace InvidiousAPIClient.Objects.Data
{
    public class InvidiousInstance
    {
        private readonly JObject _data;
        public InvidiousInstance(JObject? instanceObject)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
        }
        public virtual string Flag
        {
            get
            {
                string? result = _data["flag"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public virtual string Region
        {
            get
            {
                string? result = _data["region"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }

        public virtual bool IsCorsEnabled
        {
            get
            {
                try
                {
                    return _data["cors"]?.Value<bool>() == true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public virtual bool IsApiEnabled
        {
            get
            {
                try
                {
                    return _data["api"]?.Value<bool>() == true;
                } 
                catch
                {
                    return false;
                }
            }
        }

        public virtual string Type
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

        public virtual string Uri
        {
            get
            {
                string? result = _data["uri"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }

        public virtual InvidiousInstanceStats Stats
        {
            get
            {
                JObject? result = _data["stats"]?.Value<JObject>();
                return new InvidiousInstanceStats(result);
            }
        }
        public virtual InvidiousInstanceMonitor Monitor
        {
            get
            {
                JObject? result = _data["monitor"]?.Value<JObject>();
                return new InvidiousInstanceMonitor(result);
            }
        }
    }
}
