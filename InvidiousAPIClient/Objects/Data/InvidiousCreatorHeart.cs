using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousCreatorHeart
    {
        private readonly JObject _data;
        internal InvidiousCreatorHeart(JObject? instanceObject)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
        }
        public string CreatorThumbnail
        {
            get
            {
                string? result = _data["creatorThumbnail"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string CreatorName
        {
            get
            {
                string? result = _data["creatorName"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
    }
}
