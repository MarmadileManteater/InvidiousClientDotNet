using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousPlaylist
    {
        private readonly JObject _data;
        internal InvidiousPlaylist(JObject? instanceObject)
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
        public string PlaylistId
        {
            get
            {
                string? result = _data["playlistId"]?.Value<string>();
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
        public List<InvidiousImage> AuthorThumbnails
        {
            get
            {
                List<InvidiousImage> result = new();
                JArray? thumbnails = _data["authorThumbnails"]?.Value<JArray>();
                if (thumbnails != null)
                {
                    foreach (JObject thumbnail in thumbnails)
                    {
                        result.Add(new InvidiousImage(thumbnail));
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
        public string Description
        {
            get
            {
                string? result = _data["description"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string DescriptionHtml
        {
            get
            {
                string? result = _data["descriptionHtml"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public List<InvidiousPlaylistVideo> Videos
        {
            get
            {
                List<InvidiousPlaylistVideo> result = new List<InvidiousPlaylistVideo>();
                JArray? videos = _data["videos"]?.Value<JArray>();
                if (videos != null)
                {
                    foreach (JObject video in videos)
                    {
                        result.Add(new InvidiousPlaylistVideo(video));
                    }
                }

                return result;
            }
        }
    }
}
