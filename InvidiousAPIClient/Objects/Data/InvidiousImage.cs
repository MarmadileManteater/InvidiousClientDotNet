using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousImage
    {
        protected readonly JObject _data;
        internal InvidiousImage(JObject? instanceObject)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
        }
        public string Url
        {
            get
            {
                string? result = _data["url"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public int Width
        {
            get
            {
                int? result = _data["width"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public int Height
        {
            get
            {
                int? result = _data["height"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
    }
}
