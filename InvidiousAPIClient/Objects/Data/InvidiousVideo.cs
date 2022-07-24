using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousVideo
    {
        private readonly JObject _data;
        internal InvidiousVideo(JObject? instanceObject)
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

        public List<InvidiousVideoStoryboard> Storyboards
        {
            get
            {
                List<InvidiousVideoStoryboard> result = new();
                JArray? storyboards = _data["storyboards"]?.Value<JArray>();
                if (storyboards != null)
                {
                    foreach (JObject storyboard in storyboards)
                    {
                        result.Add(new InvidiousVideoStoryboard(storyboard));
                    }
                }

                return result;
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
        public List<string> Keywords
        {
            get
            {
                List<string> result = new List<string>();
                JArray? keywords = _data["keywords"]?.Value<JArray>();
                if (keywords != null)
                {
                    foreach (JToken keyword in keywords)
                    {
                        string? stringValue = keyword?.Value<string>();
                        if (stringValue != null)
                        {
                            result.Add(stringValue);
                        }
                    }
                }

                return result;
            }
        }
        public long ViewCount
        {
            get
            {
                long? result = _data["viewCount"]?.Value<long>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public long LikeCount
        {
            get
            {
                long? result = _data["likeCount"]?.Value<long>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public int DislikeCount
        {
            get
            {
                int? result = _data["dislikeCount"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public bool Paid
        {
            get
            {
                return _data["paid"]?.Value<bool>() == true;
            }
        }
        public bool Premium
        {
            get
            {
                return _data["premium"]?.Value<bool>() == true;
            }
        }
        public bool IsFamilyFriendly
        {
            get
            {
                return _data["isFamilyFriendly"]?.Value<bool>() == true;
            }
        }
        public List<string> AllowedRegions
        {
            get
            {
                List<string> result = new List<string>();
                JArray? keywords = _data["allowedRegions"]?.Value<JArray>();
                if (keywords != null)
                {
                    foreach (JToken keyword in keywords)
                    {
                        string? stringValue = keyword?.Value<string>();
                        if (stringValue != null)
                        {
                            result.Add(stringValue);
                        }
                    }
                }

                return result;
            }
        }
        public string Genre
        {
            get
            {
                string? result = _data["genre"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string GenreUrl
        {
            get
            {
                string? result = _data["genreUrl"]?.Value<string>();
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
        public string SubCountText
        {
            get
            {
                string? result = _data["subCountText"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public long LengthSeconds
        {
            get
            {
                long? result = _data["lengthSeconds"]?.Value<long>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public bool AllowRatings
        {
            get
            {
                return _data["allowRatings"]?.Value<bool>() == true;
            }
        }
        public int Rating
        {
            get
            {
                int? result = _data["rating"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public bool IsListed
        {
            get
            {
                return _data["isListed"]?.Value<bool>() == true;
            }
        }
        public bool IsLiveNow
        {
            get
            {
                return _data["liveNow"]?.Value<bool>() == true;
            }
        }
        public bool IsUpcoming
        {
            get
            {
                return _data["isUpcoming"]?.Value<bool>() == true;
            }
        }
        public string DashUrl
        {
            get
            {
                string? result = _data["dashUrl"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public List<FormatStream> FormatStreams
        {
            get
            {
                List<FormatStream> result = new();
                JArray? formatStreams = _data["formatStreams"]?.Value<JArray>();
                JArray? adapativeFormats = _data["adaptiveFormats"]?.Value<JArray>();

                if (formatStreams != null)
                {
                    if (adapativeFormats != null)
                    {
                        formatStreams = formatStreams.ArrayJoin(adapativeFormats);
                    }
                    foreach (JObject stream in formatStreams)
                    {
                        result.Add(new FormatStream(stream));
                    }
                }

                return result;
            }
        }
        public List<InvidiousCaption> Captions
        {
            get
            {
                List<InvidiousCaption> result = new();
                JArray? captions = _data["captions"]?.Value<JArray>();

                if (captions != null)
                {
                    foreach (JObject caption in captions)
                    {
                        result.Add(new InvidiousCaption(caption));
                    }
                }

                return result;

            }
        }
        public List<InvidiousRecommendedVideo> RecommendedVideos
        {
            get
            {
                List<InvidiousRecommendedVideo> result = new();
                JArray? recommendedVideos = _data["recommendedVideos"]?.Value<JArray>();

                if (recommendedVideos != null)
                {
                    foreach (JObject video in recommendedVideos)
                    {
                        result.Add(new InvidiousRecommendedVideo(video));
                    }
                }

                return result;

            }
        }
    }
}
