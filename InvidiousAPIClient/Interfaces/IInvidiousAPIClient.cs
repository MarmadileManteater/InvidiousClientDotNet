using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Objects.Data;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Interfaces
{
    public interface IInvidiousAPIClient
    {
        /// <summary>
        /// Gets a list of invidious instances which support API functionality
        /// </summary>
        /// <param name="condition">(optional) if given will check each instance returned from the official list and add instances to the response based on the response of the condition</param>
        /// <returns>a list of instances which support API and the given condition</returns>
        Task<List<string>> GetInvidiousAPIs(Func<InvidiousInstance, bool>? condition = null);
        /// <summary>
        /// Gets a list of invidious instances which support API functionality
        /// </summary>
        /// <param name="condition">(optional) if given will check each instance returned from the official list and add instances to the response based on the response of the condition</param>
        /// <returns>a list of instances which support API and the given condition</returns>
        List<string> GetInvidiousAPIsSync(Func<InvidiousInstance, bool>? condition = null);
        /// <summary>
        /// Fetches a JSON response from the Invidious API
        /// </summary>
        /// <param name="urlPath">the path to the content on the invidious API</param>
        /// <param name="type">the type of data being requested (EX: 'videos', 'channels', 'comments')</param>
        /// <param name="fields">a list of fields which will be retrieved from the API (returns all fields if null is given)</param>
        /// <param name="server">a server to specifically make the API call to, if not given, will pick one randomly from getInvidiousAPIs</param>
        /// <returns>the JSON response from the invidious API</returns>
        Task<JObject> FetchJSON(string urlPath, string type = "videos", string[]? fields = null, string? server = null);
        /// <summary>
        /// Fetches a JSON response from the Invidious API
        /// </summary>
        /// <param name="urlPath">the path to the content on the invidious API</param>
        /// <param name="type">the type of data being requested (EX: 'videos', 'channels', 'comments')</param>
        /// <param name="fields">a list of fields which will be retrieved from the API (returns all fields if null is given)</param>
        /// <param name="server">a server to specifically make the API call to, if not given, will pick one randomly from getInvidiousAPIs</param>
        /// <returns>the JSON response from the invidious API</returns>
        JObject FetchJSONSync(string urlPath, string type = "videos", string[]? fields = null, string? server = null);
        /// <summary>
        /// Fetches the video formats available for the given videoId by their itag
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <returns>a list of itags for the formats available for this video</returns>
        Task<List<string>> FetchVideoFormatTags(string videoId);
        /// <summary>
        /// Fetches the video formats available for the given videoId by their itag
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <returns>a list of itags for the formats available for this video</returns>
        List<string> FetchVideoFormatTagsSync(string videoId);
        /// <summary>
        /// Downloads a video stream by its' videoId and formatTag
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <param name="saveDirectory">the directory to save to</param>
        /// <param name="formatTag">the format tag to download the stream of</param>
        /// <returns></returns>
        Task DownloadVideoByFormatTag(string videoId, string saveDirectory, string formatTag);
        /// <summary>
        /// Downloads a video stream by its' videoId and formatTag
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <param name="saveDirectory">the directory to save to</param>
        /// <param name="formatTag">the format tag to download the stream of</param>
        void DownloadVideoByFormatTagSync(string videoId, string saveDirectory, string formatTag);
        /// <summary>
        /// Downloads all matching video streams by the condition
        /// If no condition is given, all streams are downloaded.
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <param name="saveDirectory">the directory to save to</param>
        /// <param name="condition">the condition by which to select videos to download</param>
        /// <returns></returns>
        Task DownloadAllMatchingVideoFormats(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null);
        /// <summary>
        /// Downloads all matching video streams by the condition
        /// If no condition is given, all streams are downloaded.
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <param name="saveDirectory">the directory to save to</param>
        /// <param name="condition">the condition by which to select videos to download</param>
        void DownloadAllMatchingVideoFormatsSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null);
        /// <summary>
        /// Downloads all matching video streams by the condition
        /// If no condition is given, all streams are downloaded.
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <param name="saveDirectory">the directory to save to</param>
        /// <param name="condition">the condition by which to select the first matching video to download</param>
        /// <returns></returns>
        Task DownloadFirstMatchingVideoFormat(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null);
        /// <summary>
        /// Downloads all matching video streams by the condition
        /// If no condition is given, all streams are downloaded.
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <param name="saveDirectory">the directory to save to</param>
        /// <param name="condition">the condition by which to select the first matching video to download</param>
        void DownloadFirstMatchingVideoFormatSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null);
    }
}
