using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Extensions;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousRecommendedVideo
    {
        private readonly JObject _data;
        internal InvidiousRecommendedVideo(JObject? instanceObject)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
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
    }
}
