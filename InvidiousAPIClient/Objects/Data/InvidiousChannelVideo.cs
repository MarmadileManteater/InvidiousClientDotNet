using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousChannelVideo
    {
        private readonly JObject _data;
        internal InvidiousChannelVideo(JObject? instanceObject)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
        }
        /// <summary>
        /// Gets the inner JObject data from the video
        /// </summary>
        /// <returns></returns>
        public JObject GetData()
        {
            JObject? data = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(_data));
            if (data != null)
            {
                return data;
            }
            return new JObject();
        }
        public string Title
        {
            get
            {
                string? result = _data["title"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string VideoId
        {
            get
            {
                string? result = _data["videoId"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string Author
        {
            get
            {
                string? result = _data["author"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string AuthorId
        {
            get
            {
                string? result = _data["authorId"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string AuthorUrl
        {
            get
            {
                string? result = _data["authorUrl"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public List<InvidiousThumbnail> VideoThumbnails
        {
            get
            {
                List<InvidiousThumbnail> result = new();
                JArray? thumbnails = _data["videoThumbnails"]?.Value<JArray>();
                if (thumbnails != null)
                {
                    foreach (JObject thumbnail in thumbnails)
                    {
                        result.Add(new InvidiousThumbnail(thumbnail));
                    }
                }

                return result;
            }
        }
        public int ViewCount
        {
            get
            {
                int? result = _data["viewCount"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public long Published
        {
            get
            {
                long? result = _data["published"]?.Value<long>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public string PublishedText
        {
            get
            {
                string? result = _data["publishedText"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public int LengthSeconds
        {
            get
            {
                int? result = _data["lengthSeconds"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public bool IsLiveNow
        {
            get
            {
                return _data["liveNow"]?.Value<bool>() == true;
            }
        }
        public bool Premium
        {
            get
            {
                return _data["premium"]?.Value<bool>() == true;
            }
        }
        public bool IsUpcoming
        {
            get
            {
                return _data["isUpcoming"]?.Value<bool>() == true;
            }
        }
    }
}
