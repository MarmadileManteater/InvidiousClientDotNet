using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousVideoStoryboard
    {
        private readonly JObject _data;
        internal InvidiousVideoStoryboard(JObject? instanceObject)
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
        public string TemplateUrl
        {
            get
            {
                string? result = _data["templateUrl"]?.Value<string>();
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
        public int Count
        {
            get
            {
                int? result = _data["count"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public int Interval
        {
            get
            {
                int? result = _data["interval"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public int StoryboardWidth
        {
            get
            {
                int? result = _data["storyboardWidth"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public int StoryboardHeight
        {
            get
            {
                int? result = _data["storyboardHeight"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public int StoryboardCount
        {
            get
            {
                int? result = _data["storyboardCount"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        
    }
}
