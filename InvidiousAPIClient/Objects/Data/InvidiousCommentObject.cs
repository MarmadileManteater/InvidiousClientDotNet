using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Interfaces;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousCommentObject
    {
        private readonly JObject _data;
        private readonly IInvidiousAPIClient _client;
        private readonly string _videoId;
        internal InvidiousCommentObject(JObject? instanceObject, IInvidiousAPIClient client, string videoId)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
            _client = client;
            _videoId = videoId;
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
        public bool IsEdited
        {
            get
            {
                return _data["isEdited"]?.Value<bool>() == true;
            }
        }
        public string Content
        {
            get
            {
                string? result = _data["content"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public string ContentHtml
        {
            get
            {
                string? result = _data["contentHtml"]?.Value<string>();
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
        public string CommentId
        {
            get
            {
                string? result = _data["commentId"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
            }
        }
        public bool IsAuthorChannelOwner
        {
            get
            {
                return _data["authorIsChannelOwner"]?.Value<bool>() == true;
            }
        }
        public InvidiousCreatorHeart? CreatorHeart
        {
            get
            {
                JObject? result = _data["creatorHeart"]?.Value<JObject>();
                if (result == null)
                {
                    return null;
                }
                return new InvidiousCreatorHeart(result);
            }
        }
        public InvidiousCommentReplies? Replies
        {
            get
            {
                JObject? result = _data["replies"]?.Value<JObject>();
                if (result == null)
                {
                    return null;
                }
                return new InvidiousCommentReplies(result, _client, _videoId);
            }
        }
    }
}
