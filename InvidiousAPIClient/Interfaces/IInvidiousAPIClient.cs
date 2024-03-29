﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Enums;
using MarmadileManteater.InvidiousClient.Objects.Data;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Interfaces
{
    /// <summary>
    /// An interface for the abstracting calls to the invidious API
    /// </summary>
    public interface IInvidiousAPIClient
    {
        /// <summary>
        /// Gets a list of invidious instances which support API functionality
        /// </summary>
        /// <param name="condition">(optional) if given will check each instance returned from the official list and add instances to the response based on the response of the condition</param>
        /// <returns>a list of instances which support API and the given condition</returns>
        Task<IList<string>> GetInvidiousAPIs(Func<InvidiousInstance, bool>? condition = null);
        /// <summary>
        /// Gets a list of invidious instances which support API functionality
        /// </summary>
        /// <param name="condition">(optional) if given will check each instance returned from the official list and add instances to the response based on the response of the condition</param>
        /// <returns>a list of instances which support API and the given condition</returns>
        IList<string> GetInvidiousAPIsSync(Func<InvidiousInstance, bool>? condition = null);
        /// <summary>
        /// Fetches a JSON response from the Invidious API
        /// </summary>
        /// <param name="urlPath">the path to the content on the invidious API</param>
        /// <param name="type">the type of data being requested (EX: 'videos', 'channels', 'comments')</param>
        /// <param name="fields">a list of fields which will be retrieved from the API (returns all fields if null is given)</param>
        /// <param name="server">a server to specifically make the API call to, if not given, will pick one randomly from getInvidiousAPIs</param>
        /// <returns>the JSON response from the invidious API</returns>
        Task<JToken> FetchJSON(string urlPath, string type = "videos", string[]? fields = null, string? server = null);
        /// <summary>
        /// Fetches a JSON response from the Invidious API
        /// </summary>
        /// <param name="urlPath">the path to the content on the invidious API</param>
        /// <param name="type">the type of data being requested (EX: 'videos', 'channels', 'comments')</param>
        /// <param name="fields">a list of fields which will be retrieved from the API (returns all fields if null is given)</param>
        /// <param name="server">a server to specifically make the API call to, if not given, will pick one randomly from getInvidiousAPIs</param>
        /// <returns>the JSON response from the invidious API</returns>
        JToken FetchJSONSync(string urlPath, string type = "videos", string[]? fields = null, string? server = null);
        /// <summary>
        /// Fetches a video object from the invidious API
        /// </summary>
        /// <param name="id">the id of the youtube video</param>
        /// <param name="fields">the fields to pull back</param>
        /// <returns>the video object</returns>
        Task<InvidiousVideo> FetchVideoById(string id, string[]? fields = null);
        /// <summary>
        /// Fetches a video object from the invidious API
        /// </summary>
        /// <param name="id">the id of the youtube video</param>
        /// <param name="fields">the fields to pull back</param>
        /// <returns>the video object</returns>
        InvidiousVideo FetchVideoByIdSync(string id, string[]? fields = null);
        /// <summary>
        /// Fetches all video objects authored by the channel with the id given from the invidious API
        /// </summary>
        /// <param name="channelId">the id of the youtube channel</param>
        /// <returns>a list of videos authored by the channel</returns>
        Task<IList<InvidiousChannelVideo>> FetchVideosByChannelId(string channelId);
        /// <summary>
        /// Fetches all video objects authored by the channel with the id given from the invidious API
        /// </summary>
        /// <param name="channelId">the id of the youtube channel</param>
        /// <returns>a list of videos authored by the channel</returns>
        IList<InvidiousChannelVideo> FetchVideosByChannelIdSync(string channelId);
        /// <summary>
        /// Fetches a channel object from the invidious API
        /// </summary>
        /// <param name="id">the id of the youtube channel</param>
        /// <param name="fields">the fields to pull back</param>
        /// <returns></returns>
        Task<InvidiousChannel> FetchChannelById(string id, string[]? fields = null);
        /// <summary>
        /// Fetches a channel object from the invidious API
        /// </summary>
        /// <param name="id">the id of the youtube channel</param>
        /// <param name="fields">the fields to pull back</param>
        /// <returns></returns>
        InvidiousChannel FetchChannelByIdSync(string id, string[]? fields = null);
        /// <summary>
        /// Fetches a playlist object from the invidious API
        /// </summary>
        /// <param name="id">the id of the youtube playlist</param>
        /// <param name="fields">the fields to pull back</param>
        /// <returns>the playlist object associated with the given id</returns>
        Task<InvidiousPlaylist> FetchPlaylistById(string id, string[]? fields = null);
        /// <summary>
        /// Fetches a playlist object from the invidious API
        /// </summary>
        /// <param name="id">the id of the youtube playlist</param>
        /// <param name="fields">the fields to pull back</param>
        /// <returns>the playlist object associated with the given id</returns>
        InvidiousPlaylist FetchPlaylistByIdSync(string id, string[]? fields = null);
        /// <summary>
        /// Fetches the video formats available for the given videoId by their itag
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <returns>a list of itags for the formats available for this video</returns>
        Task<IList<string>> FetchVideoFormatTags(string videoId);
        /// <summary>
        /// Fetches the video formats available for the given videoId by their itag
        /// </summary>
        /// <param name="videoId">the id of the youtube video</param>
        /// <returns>a list of itags for the formats available for this video</returns>
        IList<string> FetchVideoFormatTagsSync(string videoId);
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
        /// <summary>
        /// Searches using the invidious API
        /// </summary>
        /// <param name="query">a search query</param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <param name="searchType"></param>
        /// <param name="features"></param>
        /// <param name="region"></param>
        /// <returns>search results</returns>
        Task<IList<JObject>> Search(string query, int page = 0, SortBy? sortBy = null, DateRange? date = null, Duration? duration = null, SearchType? searchType = null, Feature[]? features = null, string? region = null);
        /// <summary>
        /// Searches using the invidious API
        /// </summary>
        /// <param name="query">a search query</param>
        /// <param name="page"></param>
        /// <param name="sortBy"></param>
        /// <param name="date"></param>
        /// <param name="duration"></param>
        /// <param name="searchType"></param>
        /// <param name="features"></param>
        /// <param name="region"></param>
        /// <returns>search results</returns>
        IList<JObject> SearchSync(string query, int page = 0, SortBy? sortBy = null, DateRange? date = null, Duration? duration = null, SearchType? searchType = null, Feature[]? features = null, string? region = null);
        /// <summary>
        /// Retrieves a list of suggested search results
        /// </summary>
        /// <param name="partialQuery">the partially typed query</param>
        /// <returns></returns>
        Task<IList<string>> SearchSuggestions(string partialQuery);
        /// <summary>
        /// Retrieves a list of suggested search results
        /// </summary>
        /// <param name="partialQuery">the partially typed query</param>
        /// <returns></returns>
        IList<string> SearchSuggestionsSync(string partialQuery);
        /// <summary>
        /// Returns the associated comments object for the given video id
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        Task<InvidiousComments> FetchCommentsByVideoId(string videoId);
        /// <summary>
        /// Returns the associated comments object for the given video id
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        InvidiousComments FetchCommentsByVideoIdSync(string videoId);
    }
}
