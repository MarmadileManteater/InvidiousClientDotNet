using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Objects.Data;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousThumbnail : InvidiousImage
    {
        internal InvidiousThumbnail(JObject? instanceObject) : base(instanceObject) {}
        public string Quality
        {
            get
            {
                string? result = _data["quality"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
    }
}
