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
        /// <summary>
        /// Unicode representation of the country flag
        /// </summary>
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
        /// <summary>
        /// Does this instance support Cross Origin Resource Sharing (CORS)?
        /// </summary>
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

        /// <summary>
        /// Does this instance support API requests?
        /// </summary>
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
        /// <summary>
        /// Mimetype and encoding
        /// </summary>
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
        /// <summary>
        /// Uri for the instance
        /// </summary>
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
