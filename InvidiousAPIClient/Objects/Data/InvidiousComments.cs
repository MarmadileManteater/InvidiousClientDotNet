using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Interfaces;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousComments
    {
        private readonly JObject _data;
        private readonly IInvidiousAPIClient _client;
        internal InvidiousComments(JObject? instanceObject, IInvidiousAPIClient client)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
            _client = client;
        }
        public int? CommentCount
        {
            get
            {
                return _data["commentCount"]?.Value<int?>();
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
        public IList<InvidiousCommentObject> Comments
        {
            get
            {
                IList<InvidiousCommentObject> result = new List<InvidiousCommentObject>();
                JArray? comments = _data["comments"]?.Value<JArray>();
                if (comments != null)
                {
                    foreach (JToken comment in comments)
                    {
                        JObject? commentJObject = comment.Value<JObject>();
                        result.Add(new InvidiousCommentObject(commentJObject, _client, VideoId));
                    }
                }
                return result;
            }
        }
        public string? Continuation
        {
            get
            {
                return _data["continuation"]?.Value<string?>();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">If Continuation is ""</exception>
        public async Task<InvidiousComments> FetchContinuation()
        {
            JObject? jObject;
            if (Continuation != "")
            {
                JToken? response = await _client.FetchJSON(VideoId + "?continuation=" + Continuation, "comments");
                jObject = response?.Value<JObject>();
                if (jObject == null)
                {
                    jObject = new JObject();
                }
            }
            else
            {
                throw new Exception("No continuation to fetch.");
            }
            return new InvidiousComments(jObject, _client);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">If Continuation is ""</exception>
        public InvidiousComments FetchContinuationSync()
        {
            Task<InvidiousComments> task = FetchContinuation();
            task.Wait();
            return task.Result;
        }
    }
}
