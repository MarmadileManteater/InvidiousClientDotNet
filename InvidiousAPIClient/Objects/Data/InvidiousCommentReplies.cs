using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Interfaces;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects.Data
{
    public class InvidiousCommentReplies
    {
        private readonly JObject _data;
        private readonly IInvidiousAPIClient _client;
        private readonly string _videoId;
        internal InvidiousCommentReplies(JObject? instanceObject, IInvidiousAPIClient client, string videoId)
        {
            if (instanceObject == null)
            {
                instanceObject = new JObject();
            }
            _data = instanceObject;
            _client = client;
            _videoId = videoId;
        }
        public int ReplyCount
        {
            get
            {
                int? result = _data["replyCount"]?.Value<int>();
                if (result != null)
                {
                    return result.Value;
                }
                return 0;
            }
        }
        public string Continuation
        {
            get
            {
                string? result = _data["continuation"]?.Value<string>();
                if (result != null)
                {
                    return result;
                }
                return "";
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
                JToken? response = await _client.FetchJSON(_videoId + "?continuation=" + Continuation, "comments");
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
