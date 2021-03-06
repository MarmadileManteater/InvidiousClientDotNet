using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Enums;
using MarmadileManteater.InvidiousClient.Extensions;
using MarmadileManteater.InvidiousClient.Interfaces;
using MarmadileManteater.InvidiousClient.Objects.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarmadileManteater.InvidiousClient.Objects
{
    /// <summary>
    /// A client which can automatically find an available Invidious instance and use that instance to retrieve data stored from the Invidious API.
    /// Optionally, you can give it a default server to connect to.
    /// </summary>
    public class InvidiousAPIClient : IInvidiousAPIClient
    {
        private readonly Dictionary<string, KeyValuePair<DateTime, HttpResponseMessage>> _httpResponseCache;
        private readonly int _expireMinutes = 60;
        private readonly int _chunkSize;
        private readonly int _failureTolerance = 5;
        private readonly ILogger _logger;
        private int failedAttempts = 0;
        private readonly string? _defaultServer;
        /// <summary>
        /// Is the cache enabled for this client?
        /// </summary>
        public readonly bool CacheEnabled;

        /// <summary>
        /// This client will start with the default server if given, but otherwise, and during failures within the fail tolerance, it will use a random server.
        /// It can also have the output of the client redirected to a different type of ILogger.
        /// </summary>
        /// <param name="cacheEnabled">whether or not to cache responses from the invidious API will hit up the same url over an over again</param>
        /// <param name="logger"></param>
        /// <param name="failureTolerance"></param>
        /// <param name="defaultServer">if given will make requests out to this server first, and it will still fallback to a random instance if this instance fails</param>
        public InvidiousAPIClient(bool cacheEnabled, ILogger logger, int failureTolerance = 5, string? defaultServer = null)
        {
            _httpResponseCache = new Dictionary<string, KeyValuePair<DateTime, HttpResponseMessage>>();
            CacheEnabled = cacheEnabled;
            _logger = logger;
            _chunkSize = 8192;
            _failureTolerance = failureTolerance;
            _defaultServer = defaultServer;
        }

        /// <summary>
        /// This client will only try to reach out to the given server and throw immediately when that server fails instead of retrying another server.
        /// It also has an overridable ILogger for redirecting logs to something or somewhere.
        /// </summary>
        /// <param name="cacheEnabled">whether or not to cache responses from the invidious API will hit up the same url over an over again</param>
        /// /// <param name="logger"></param>
        /// <param name="defaultServer"></param>
        public InvidiousAPIClient(bool cacheEnabled, ILogger logger, string defaultServer)
        {
            _httpResponseCache = new Dictionary<string, KeyValuePair<DateTime, HttpResponseMessage>>();
            CacheEnabled = cacheEnabled;
            _logger = logger;
            _chunkSize = 8192;
            _failureTolerance = 0;
            _defaultServer = defaultServer;
        }

        /// <summary>
        /// This client will only try to reach out to the given server and throw immediately when that server fails instead of retrying another server
        /// </summary>
        /// <param name="cacheEnabled">whether or not to cache responses from the invidious API will hit up the same url over an over again</param>
        /// <param name="defaultServer"></param>
        public InvidiousAPIClient(bool cacheEnabled, string defaultServer)
        {
            _httpResponseCache = new Dictionary<string, KeyValuePair<DateTime, HttpResponseMessage>>();
            CacheEnabled = cacheEnabled;
            _logger = new ConsoleLogger();
            _chunkSize = 8192;
            _failureTolerance = 0;
            _defaultServer = defaultServer;
        }

        /// <summary>
        /// This client will start with the default server if given, but otherwise, and during failures within the fail tolerance, it will use a random server
        /// </summary>
        /// <param name="cacheEnabled">whether or not to cache responses from the invidious API will hit up the same url over an over again</param>
        /// <param name="failureTolerance"></param>
        /// <param name="defaultServer"></param>
        public InvidiousAPIClient(bool cacheEnabled = true, int failureTolerance = 5, string? defaultServer = null)
        {
            _httpResponseCache = new Dictionary<string, KeyValuePair<DateTime, HttpResponseMessage>>();
            CacheEnabled = cacheEnabled;
            _logger = new ConsoleLogger();
            _chunkSize = 8192;
            _failureTolerance = failureTolerance;
            _defaultServer = defaultServer;
        }

        /// <summary>
        /// This fetches a HTTP response message for the given URL. It caches when CacheEnabled is true.
        /// </summary>
        /// <param name="url">the url to the content</param>
        /// <param name="client">an optional override for the HTTP client used</param>
        /// <returns>the HttpResponseMessage returned from the URL</returns>
        protected async Task<HttpResponseMessage> Fetch(string url, HttpClient? client = null)
        {
            string requestUrl = "";
            string absolutePath = new Uri(url).PathAndQuery;
            // If there is an entry in the cache for this url,
            // we don't need to check if cache is enabled because
            // the cache will always be empty if it is disabled.
            if (_httpResponseCache.ContainsKey(absolutePath) && ((DateTime.Now - _httpResponseCache[absolutePath].Key).TotalMinutes <= _expireMinutes))
            {
                return _httpResponseCache[absolutePath].Value;
            }

            // If the client parameter is null,
            if (client == null)
            {
                // Make a new one using the url as a base URL
                client = new()
                {
                    BaseAddress = new Uri(url)
                };
            }
            else
            {
                // if it isn't,
                // set the base address
                try
                {
                    client.BaseAddress = new Uri(url);
                }
                catch (Exception exception)
                {
                    // There was some issue with setting this value.
                    // This value is read only, so this is unsurprising
                    // just set the request url and act like this was
                    // what we wanted
                    requestUrl = url;
                    await _logger.LogError("There was some issue with setting the base address on the given HttpClient.", exception);
                }
            }

            await _logger.Trace("Requesting: " + url + requestUrl);
            client.Timeout = TimeSpan.FromMinutes(30);
            HttpResponseMessage message;
            try
            {
                message = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
            } 
            catch (Exception error)
            {
                message = new HttpResponseMessage
                {
                    StatusCode = (System.Net.HttpStatusCode)500,
                    Content = new StringContent(error.Message)
                };
            }

            await _logger.Trace("Received response from: " + url + requestUrl);
            // If the cache is enabled and message is good
            // we don't want to cache errors because those might change from instance to instance
            if (CacheEnabled && message.IsSuccessStatusCode)
            {
                string? contentType = message.Content.Headers.ContentType?.ToString();
                if (contentType != null && !contentType.Contains("video") && !contentType.Contains("audio")) {
                    // Add the entry to the cache
                    // This will never add duplicate entries. If the
                    // key already exists, this method would have
                    // returned in the first conditional.
                    _httpResponseCache[absolutePath] = new KeyValuePair<DateTime, HttpResponseMessage>(DateTime.Now, message);
                    await _logger.Trace("Adding to the http response cache: " + url);
                }
            }

            return message;
        }
        /// <inheritdoc/>
        /// <exception cref="Exception">Throws if the amount of consecutive failures in a request is greater than the failure tolerance of the client</exception>
        public async Task DownloadAllMatchingVideoFormats(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            InvidiousVideo video = await FetchVideoById(videoId, new string[] { "formatStreams", "adaptiveFormats" });
            foreach (FormatStream stream in video.FormatStreams)
            {
                if (condition == null || condition(stream))
                {
                    string url = stream.Url;
                    string mimeType = stream.Type;

                    string fileType = mimeType.Split("/")[1].Split(";")[0];

                    bool connectionFailed = false;

                    do
                    {
                        HttpResponseMessage response = await Fetch(url);
                        // Save the byte array to the save directory
                        try
                        {
                            Directory.CreateDirectory(Path.Join(saveDirectory, videoId));
                        }
                        catch
                        {

                        }
                        string fileName = Path.Join(saveDirectory, videoId, videoId + "_" + stream.Itag + "." + fileType);

                        // this section is adapted from code from this github post:
                        // https://github.com/dotnet/runtime/issues/16681#issuecomment-195980023
                        using Stream contentStream = await response.Content.ReadAsStreamAsync(),
                                        fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, _chunkSize, true);
                        long? contentLength = 0;
                        if (response.Content?.Headers?.ContentLength.HasValue == true)
                        {
                            contentLength = response.Content.Headers?.ContentLength.Value;
                        }
                        var totalRead = 0L;
                        var totalReads = 0L;
                        var buffer = new byte[_chunkSize];
                        var isMoreToRead = true;
                        await _logger.LogInfo("Downloading stream with itag of \"" + stream.Itag + "\" to the file:\r\n" + fileName);
                        IProgress<double> progress = _logger.GetProgressInterface();
                        try
                        {
                            do
                            {
                                var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                if (read == 0)
                                {
                                    isMoreToRead = false;
                                }
                                else
                                {
                                    await fileStream.WriteAsync(buffer, 0, read);

                                    totalRead += read;
                                    totalReads += 1;
                                    if (contentLength != null)
                                    {
                                        double? progressReport = ((double)totalRead) / contentLength;
                                        if (progressReport != null)
                                        {
                                            progress.Report(progressReport.Value);
                                        }
                                    }
                                }

                            } while (isMoreToRead);
                            await _logger.Trace("Finished downloading!");
                            failedAttempts = 0;
                            connectionFailed = false;
                        }
                        catch (Exception exception)
                        {
                            await _logger.LogError("An error occured while downloading.", exception);
                            failedAttempts++;
                            connectionFailed = true;
                            Thread.Sleep(1);
                            await _logger.Trace("Refetching a new stream url for video id: " + videoId);
                            video = await FetchVideoById(videoId, new string[] { "formatStreams", "adaptiveFormats" });
                        }
                    } while (connectionFailed && failedAttempts < _failureTolerance);
                    if (failedAttempts > _failureTolerance)
                    {
                        throw new Exception("Failed to download " + _failureTolerance + " consecutive times!");
                    }
                }
            }
        }

        /// <inheritdoc />
        public void DownloadAllMatchingVideoFormatsSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            Task task = DownloadAllMatchingVideoFormats(videoId, saveDirectory, condition);
            task.Wait();
            return;
        }

        /// <inheritdoc />
        public async Task DownloadFirstMatchingVideoFormat(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            bool hasMatched = false;
            await DownloadAllMatchingVideoFormats(videoId, saveDirectory, (jObject) =>
            {
                if (!hasMatched && condition != null && condition(jObject))
                {
                    hasMatched = true;
                    return true;
                }
                return false;
            });
        }

        /// <inheritdoc />
        public void DownloadFirstMatchingVideoFormatSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            Task task = DownloadFirstMatchingVideoFormat(videoId, saveDirectory, condition);
            task.Wait();
            return;
        }

        /// <inheritdoc />
        public async Task DownloadVideoByFormatTag(string videoId, string saveDirectory, string formatTag)
        {
            await DownloadFirstMatchingVideoFormat(videoId, saveDirectory, (FormatStream stream) =>
            {
                return stream.Itag == formatTag;
            });
        }

        /// <inheritdoc />
        public void DownloadVideoByFormatTagSync(string videoId, string saveDirectory, string formatTag)
        {
            Task task = DownloadVideoByFormatTag(videoId, saveDirectory, formatTag);
            task.Wait();
            return;
        }

        /// <inheritdoc />
        public async Task<JToken> FetchJSON(string urlPath, string type = "videos", string[]? fields = null, string? server = null)
        {
            if (server == null)
            {
                server = _defaultServer;
            }
            // if default server is null
            if (server == null) {
                IList<string> apis = await GetInvidiousAPIs();
                Random random = new();
                int randomIndex = random.Next(apis.Count);
                server = apis[randomIndex];
            }

            if (server.StartsWith("http://"))
            {
                server = server.Replace("http://", "https://");
            }

            if (!server.StartsWith("https://"))
            {
                server = "https://" + server;
            }

            string path = urlPath;
            if (type != null)
            {
                path = type + "/" + urlPath;
            }

            char startCharacter = '?';
            if (path.Contains('?'))
            {
                startCharacter = '&';
            }
            if (fields != null)
            {

                path += startCharacter + "fields=" + string.Join(',', fields) + "&pretty=1";
            } else
            {
                path += startCharacter + "pretty=1";
            
            }

            HttpResponseMessage response = await Fetch(server + "/api/v1/" + path);
            while (!response.IsSuccessStatusCode && failedAttempts < _failureTolerance)
            {
                try
                {
                    response = await Fetch(server + "/api/v1/" + path);
                    response.EnsureSuccessStatusCode();
                    failedAttempts = 0;
                }
                catch (Exception exception)
                {
                    await _logger.LogError("Failed to fetch from the invidious API", exception);
                    
                    failedAttempts++;
                    if (failedAttempts < _failureTolerance)
                    {// if failure attempts within error tolerance
                        IList<string> apis = await GetInvidiousAPIs();// get a new server
                        server = apis[new Random().Next(apis.Count())];
                    }
                }
            }
            // if is is still not a success code we have failed a certain number of consecutive times.
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            JToken? result = JsonConvert.DeserializeObject<JToken>(content);
            if (result == null)
            {
                throw new Exception("Response was null");
            }
            return result;
        }
        /// <inheritdoc />
        public JToken FetchJSONSync(string urlPath, string type = "videos", string[]? fields = null, string? server = null)
        {
            Task<JToken> task = FetchJSON(urlPath, type, fields, server);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc />
        public async Task<IList<string>> FetchVideoFormatTags(string videoId)
        {
            List<string> itags = new();
            InvidiousVideo videoObject = await FetchVideoById(videoId, new string[] { "formatStreams", "adaptiveFormats" });
            foreach (FormatStream streamObject in videoObject.FormatStreams)
            {
                string itag = streamObject.Itag;
                if (itag != "")
                {
                    itags.Add(itag);
                }
            }
            return itags;
        }
        /// <inheritdoc />
        public IList<string> FetchVideoFormatTagsSync(string videoId)
        {
            Task<IList<string>> task = FetchVideoFormatTags(videoId);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc />
        public async Task<IList<string>> GetInvidiousAPIs(Func<InvidiousInstance, bool>? condition = null)
        {
            List<string> response = new();
            HttpResponseMessage instancesResponse = await Fetch("https://api.invidious.io/instances.json?pretty=1&sort_by=type,users");
            // If we get an error response, something is probably wrong.
            instancesResponse.EnsureSuccessStatusCode();
            if (instancesResponse.Content != null)
            {
                string content = await instancesResponse.Content.ReadAsStringAsync();
                JArray? jsonResponse = JsonConvert.DeserializeObject<JArray>(content);
                if (jsonResponse != null)
                {
                    foreach (JArray entry in jsonResponse)
                    {
                        InvidiousInstance? instanceObject = new(entry[1].Value<JObject>());
                        string? uri = instanceObject?.Uri;
                        bool? apiEnabled = instanceObject?.IsApiEnabled;
                        if (apiEnabled == null)
                        {
                            apiEnabled = false;
                        }

                        bool callbackApproved = true;
                        if (condition != null && instanceObject != null)
                        {
                            callbackApproved = condition(instanceObject);
                        }

                        if (callbackApproved == true && apiEnabled == true && uri != null)
                        {
                            response.Add(uri);
                        }
                    }
                }
                else
                {
                    await _logger.LogError("Json response from server was null.", new Exception(content));
                }
            }
            return response;
        }
        /// <inheritdoc />
        public IList<string> GetInvidiousAPIsSync(Func<InvidiousInstance, bool>? condition = null)
        {
            Task<IList<string>> task = GetInvidiousAPIs(condition);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc />
        public async Task<InvidiousVideo> FetchVideoById(string id, string[]? fields = null)
        {
            JObject? videoObject = (await FetchJSON(id, "videos", fields))?.Value<JObject>();
            return new InvidiousVideo(videoObject);
        }
        /// <inheritdoc />
        public InvidiousVideo FetchVideoByIdSync(string id, string[]? fields = null)
        {
            Task<InvidiousVideo> task = FetchVideoById(id, fields);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc />
        public async Task<InvidiousChannel> FetchChannelById(string id, string[]? fields = null)
        {
            JObject? channelObject = (await FetchJSON(id, "channels", fields))?.Value<JObject>();
            return new InvidiousChannel(channelObject);
        }
        /// <inheritdoc />
        public InvidiousChannel FetchChannelByIdSync(string id, string[]? fields = null)
        {
            Task<InvidiousChannel> task = FetchChannelById(id, fields);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc />
        public async Task<IList<InvidiousChannelVideo>> FetchVideosByChannelId(string channelId)
        {
            List<InvidiousChannelVideo> result = new();
            JArray? channelList = (await FetchJSON(channelId + "/videos", "channels")).Value<JArray>();
            if (channelList != null)
            {
                foreach (JObject video in channelList)
                {
                    result.Add(new InvidiousChannelVideo(video));
                }
            }
            return result;
        }
        /// <inheritdoc />
        public IList<InvidiousChannelVideo> FetchVideosByChannelIdSync(string channelId)
        {
            Task<IList<InvidiousChannelVideo>> task = FetchVideosByChannelId(channelId);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc />
        public async Task<InvidiousPlaylist> FetchPlaylistById(string id, string[]? fields = null)
        {
            JObject? playlistObject = (await FetchJSON(id, "playlists", fields))?.Value<JObject>();
            return new InvidiousPlaylist(playlistObject);
        }
        /// <inheritdoc />
        public InvidiousPlaylist FetchPlaylistByIdSync(string id, string[]? fields = null)
        {
            Task<InvidiousPlaylist> task = FetchPlaylistById(id, fields);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc />
        public async Task<IList<JObject>> Search(string query, int page = 0, SortBy? sortBy = null, DateRange? date = null, Duration? duration = null, SearchType? searchType = null, Feature[]? features = null, string? region = null)
        {
            List<JObject> result = new();
            string queryInterjection = "";
            if (sortBy != null)
            {
                queryInterjection += "&sort_by=";
                switch(sortBy)
                {
                    case SortBy.Relevance:
                        queryInterjection += "relevance";
                        break;
                    case SortBy.Rating:
                        queryInterjection += "rating";
                        break;
                    case SortBy.UploadDate:
                        queryInterjection += "upload_date";
                        break;
                    case SortBy.ViewCount:
                        queryInterjection += "view_count";
                        break;
                }
            }
            if (date != null)
            {
                queryInterjection += "&date=";
                switch (date)
                {
                    case DateRange.LastHour:
                        queryInterjection += "hour";
                        break;
                    case DateRange.LastDay:
                        queryInterjection += "day";
                        break;
                    case DateRange.LastWeek:
                        queryInterjection += "week";
                        break;
                    case DateRange.LastMonth:
                        queryInterjection += "month";
                        break;
                    case DateRange.LastYear:
                        queryInterjection += "year";
                        break;
                }
            }
            if (duration != null)
            {
                queryInterjection += "&duration=";
                switch (duration)
                {
                    case Duration.Short:
                        queryInterjection += "short";
                        break;
                    case Duration.Long:
                        queryInterjection += "long";
                        break;
                }
            }
            if (searchType != null)
            {
                queryInterjection += "&type=";
                switch (searchType)
                {
                    case SearchType.Video:
                        queryInterjection += "video";
                        break;
                    case SearchType.Playlist:
                        queryInterjection += "playlist";
                        break;
                    case SearchType.Channel:
                        queryInterjection += "channel";
                        break;
                    case SearchType.All:
                        queryInterjection += "all";
                        break;
                }
            }
            if (features != null)
            {
                queryInterjection += "&features=";
                foreach (Feature feature in features)
                {
                    switch (feature)
                    {
                        case Feature.HD:
                            queryInterjection += "hd";
                            break;
                        case Feature.Subtitles:
                            queryInterjection += "subtitles";
                            break;
                        case Feature.CreativeCommons:
                            queryInterjection += "creative_commons";
                            break;
                        case Feature._3d:
                            queryInterjection += "3D";
                            break;
                        case Feature.Live:
                            queryInterjection += "live";
                            break;
                        case Feature.Purchased:
                            queryInterjection += "purchased";
                            break;
                        case Feature._4k:
                            queryInterjection += "4k";
                            break;
                        case Feature._360:
                            queryInterjection += "360";
                            break;
                        case Feature.Location:
                            queryInterjection += "location";
                            break;
                        case Feature.HDR:
                            queryInterjection += "hdr";
                            break;
                    }
                    queryInterjection += ",";
                }
                queryInterjection = queryInterjection.Substring(0, queryInterjection.Length - 1);// trim the last comma
            }
            if (region != null)
            {
                queryInterjection += "&region=" + region;
            }
            JToken response = await FetchJSON("?q=" + Uri.EscapeDataString(query) + "&page=" + page.ToString() + queryInterjection, "search");
            JArray? searchList = response.Value<JArray>();
            if (searchList != null)
            {
                foreach (JObject searchItem in searchList)
                {
                    result.Add(searchItem);
                }
            }
            return result;
        }
        /// <inheritdoc />
        public IList<JObject> SearchSync(string query, int page = 0, SortBy? sortBy = null, DateRange? date = null, Duration? duration = null, SearchType? searchType = null, Feature[]? features = null, string? region = null)
        {
            Task<IList<JObject>> task = Search(query, page, sortBy, date, duration, searchType, features);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc/>
        public async Task<IList<string>> SearchSuggestions(string partialQuery)
        {
            IList<string> result = new List<string>();
            JToken response = await FetchJSON("suggestions?q=" + partialQuery, "search");
            JObject? searchSuggestionsObject = response.Value<JObject>();
            if (searchSuggestionsObject != null)
            {
                JArray? suggestions = searchSuggestionsObject["suggestions"]?.Value<JArray>();
                if (suggestions != null)
                {
                    foreach (JToken suggestion in suggestions)
                    {
                        string? deserializedSuggestion = suggestion?.Value<string>();
                        if (deserializedSuggestion != null)
                        {
                            result.Add(deserializedSuggestion);
                        }
                    }
                }
            }
            return result;
        }
        /// <inheritdoc/>
        public IList<string> SearchSuggestionsSync(string partialQuery)
        {
            Task<IList<string>> task = SearchSuggestions(partialQuery);
            task.Wait();
            return task.Result;
        }
        /// <inheritdoc/>
        public async Task<InvidiousComments> FetchCommentsByVideoId(string videoId)
        {
            InvidiousComments result = new(null, this);
            JToken response = await FetchJSON(videoId, "comments");
            JObject? commentsJObject = response.Value<JObject>();
            if (commentsJObject != null)
            {
                result = new(commentsJObject, this);
            }
            return result;
        }
        /// <inheritdoc/>
        public InvidiousComments FetchCommentsByVideoIdSync(string videoId)
        {
            Task<InvidiousComments> task = FetchCommentsByVideoId(videoId);
            task.Wait();
            return task.Result;
        }
    }
}
