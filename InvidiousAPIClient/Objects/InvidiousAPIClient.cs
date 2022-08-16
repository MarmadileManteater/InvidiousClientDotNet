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
        protected Task LogOptionalSync(string message, LogLevel level, Exception? exception, bool sync = false)
        {
            if (!sync)
            {
                return _logger.Log(message, level, exception);
            }
            else
            {
                _logger.LogSync(message, level, exception);
                return Task.CompletedTask;
            }
        }
        /// <summary>
        /// This fetches a HTTP response message for the given URL. It caches when CacheEnabled is true.
        /// </summary>
        /// <param name="url">the url to the content</param>
        /// <param name="client">an optional override for the HTTP client used</param>
        /// <returns>the HttpResponseMessage returned from the URL</returns>
        private async Task<HttpResponseMessage> FetchOptionalSync(string url, HttpClient? client = null, bool sync = false)
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
                    await LogOptionalSync("There was some issue with setting the base address on the given HttpClient.", LogLevel.Error, exception, sync);
                }
            }

            await LogOptionalSync($"Requesting: {url}{requestUrl}", LogLevel.Trace, null, sync);
            client.Timeout = TimeSpan.FromMinutes(30);
            HttpResponseMessage message;
            try
            {
                if (!sync)
                {
                    message = await client.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead);
                }
                else
                {
                    message = client.Send(new HttpRequestMessage(HttpMethod.Get, requestUrl), HttpCompletionOption.ResponseHeadersRead);
                }
            }
            catch (Exception error)
            {
                message = new HttpResponseMessage
                {
                    StatusCode = (System.Net.HttpStatusCode)500,
                    Content = new StringContent(error.Message)
                };
            }

            await LogOptionalSync($"Received response from: {url}{requestUrl}", LogLevel.Trace, null, sync);
            // If the cache is enabled and message is good
            // we don't want to cache errors because those might change from instance to instance
            if (CacheEnabled && message.IsSuccessStatusCode)
            {
                string? contentType = message.Content.Headers.ContentType?.ToString();
                if (contentType != null && !contentType.Contains("video") && !contentType.Contains("audio"))
                {
                    // Add the entry to the cache
                    // This will never add duplicate entries. If the
                    // key already exists, this method would have
                    // returned in the first conditional.
                    _httpResponseCache[absolutePath] = new KeyValuePair<DateTime, HttpResponseMessage>(DateTime.Now, message);
                    await LogOptionalSync($"Adding to the http response cache: {url}", LogLevel.Trace, null, sync);
                }
            }

            return message;
        }
        protected async Task<HttpResponseMessage> Fetch(string url, HttpClient? client = null)
        {
            return await FetchOptionalSync(url, client, false);
        }
        protected HttpResponseMessage FetchSync(string url, HttpClient? client = null)
        {
            // passing true makes the functionality actually synchronous, so we can just get result right here
            return FetchOptionalSync(url, client, true).GetAwaiter().GetResult();
        }
        private async Task DownloadAllMatchingVideoFormatsOptionalSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null, bool sync = false)
        {
            InvidiousVideo video = await FetchVideoByIdOptionalSync(videoId, new string[] { "formatStreams", "adaptiveFormats" }, sync);
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
                        HttpResponseMessage response = await FetchOptionalSync(url, null, sync);
                        // Save the byte array to the save directory
                        try
                        {
                            Directory.CreateDirectory(Path.Join(saveDirectory, videoId));
                        }
                        catch
                        {

                        }
                        string fileName = Path.Join(saveDirectory, videoId, $"{videoId}_{stream.Itag}.{fileType}");

                        // this section is adapted from code from this github post:
                        // https://github.com/dotnet/runtime/issues/16681#issuecomment-195980023
                        using Stream contentStream = sync? response.Content.ReadAsStreamAsync().GetAwaiter().GetResult():await response.Content.ReadAsStreamAsync(),
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
                        await LogOptionalSync($"Downloading stream with itag of \"{stream.Itag}\" to the file:\r\n{fileName}", LogLevel.Information, null, sync);
                        IProgress<double> progress = _logger.GetProgressInterface();
                        try
                        {
                            do
                            {
                                int read;
                                if (!sync)
                                {
                                    read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                                }
                                else
                                {
                                    read = contentStream.Read(buffer, 0, buffer.Length);
                                }
                                if (read == 0)
                                {
                                    isMoreToRead = false;
                                }
                                else
                                {
                                    if (!sync)
                                    {
                                        await fileStream.WriteAsync(buffer, 0, read);
                                    }
                                    else
                                    {
                                        fileStream.Write(buffer, 0, read);
                                    }

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
                            await LogOptionalSync("Finished downloading!", LogLevel.Trace, null, sync);
                            failedAttempts = 0;
                            connectionFailed = false;
                        }
                        catch (Exception exception)
                        {
                            await LogOptionalSync("An error occured while downloading.", LogLevel.Error, exception, sync);
                            failedAttempts++;
                            connectionFailed = true;
                            Thread.Sleep(1);
                            await LogOptionalSync($"Refetching a new stream url for video id: {videoId}", LogLevel.Trace, null, sync);
                            video = await FetchVideoByIdOptionalSync(videoId, new string[] { "formatStreams", "adaptiveFormats" }, sync);
                        }
                        finally
                        {
                            try
                            {
                                // Dispose the progress if we can
                                ((IDisposable)progress).Dispose();
                            }
                            catch
                            {
                                // this could throw even when everything works
                            }
                        }
                    } while (connectionFailed && failedAttempts < _failureTolerance);
                    if (failedAttempts > _failureTolerance)
                    {
                        throw new Exception($"Failed to download {_failureTolerance} consecutive times!");
                    }
                }
            }
        }
        /// <inheritdoc/>
        /// <exception cref="Exception">Throws if the amount of consecutive failures in a request is greater than the failure tolerance of the client</exception>
        public async Task DownloadAllMatchingVideoFormats(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            await DownloadAllMatchingVideoFormatsOptionalSync(videoId, saveDirectory, condition, false);
        }

        /// <inheritdoc />
        public void DownloadAllMatchingVideoFormatsSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            DownloadAllMatchingVideoFormatsOptionalSync(videoId, saveDirectory, condition, true).GetAwaiter().GetResult();
        }

        private async Task DownloadFirstMatchingVideoFormatOptionalSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null, bool sync = false)
        {
            bool hasMatched = false;
            await DownloadAllMatchingVideoFormatsOptionalSync(videoId, saveDirectory, (jObject) =>
            {
                if (!hasMatched && condition != null && condition(jObject))
                {
                    hasMatched = true;
                    return true;
                }
                return false;
            }, sync);
        }

        /// <inheritdoc />
        public async Task DownloadFirstMatchingVideoFormat(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            await DownloadFirstMatchingVideoFormatOptionalSync(videoId, saveDirectory, condition, false);
        }

        /// <inheritdoc />
        public void DownloadFirstMatchingVideoFormatSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            DownloadFirstMatchingVideoFormatOptionalSync(videoId, saveDirectory, condition, true).GetAwaiter().GetResult();
        }

        private async Task DownloadVideoByFormatTagOptionalSync(string videoId, string saveDirectory, string formatTag, bool sync = false)
        {
            await DownloadFirstMatchingVideoFormatOptionalSync(videoId, saveDirectory, (FormatStream stream) =>
            {
                return stream.Itag == formatTag;
            }, sync);
        }

        /// <inheritdoc />
        public async Task DownloadVideoByFormatTag(string videoId, string saveDirectory, string formatTag)
        {
            await DownloadVideoByFormatTagOptionalSync(videoId, saveDirectory, formatTag);
        }

        /// <inheritdoc />
        public void DownloadVideoByFormatTagSync(string videoId, string saveDirectory, string formatTag)
        {
            DownloadVideoByFormatTagOptionalSync(videoId, saveDirectory, formatTag, true).GetAwaiter().GetResult();
        }
        private async Task<JToken> FetchJSONOptionalSync(string urlPath, string type = "videos", string[]? fields = null, string? server = null, bool sync = false)
        {
            if (server == null)
            {
                server = _defaultServer;
            }
            // if default server is null
            if (server == null)
            {
                IList<string> apis = await GetInvidiousAPIsOptionalSync(null, sync);
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
                server = $"https://{server}";
            }

            string path = urlPath;
            if (type != null)
            {
                path = $"{type}/{urlPath}";
            }

            char startCharacter = '?';
            if (path.Contains('?'))
            {
                startCharacter = '&';
            }
            if (fields != null)
            {
                string fieldString = string.Join(',', fields);
                path += $"{startCharacter}fields={fieldString}&pretty=1";
            }
            else
            {
                path += $"{startCharacter}pretty=1";
            }

            HttpResponseMessage response = await FetchOptionalSync($"{server}/api/v1/{path}", null, sync);
            while (!response.IsSuccessStatusCode && failedAttempts < _failureTolerance)
            {
                try
                {
                    response = await FetchOptionalSync($"{server}/api/v1/{path}", null, sync);
                    response.EnsureSuccessStatusCode();
                    failedAttempts = 0;
                }
                catch (Exception exception)
                {
                    await LogOptionalSync("Failed to fetch from the invidious API", LogLevel.Error, exception, sync);

                    failedAttempts++;
                    if (failedAttempts < _failureTolerance)
                    {// if failure attempts within error tolerance
                        IList<string> apis = await GetInvidiousAPIsOptionalSync(null, sync);// get a new server
                        server = apis[new Random().Next(apis.Count())];
                    }
                }
            }
            // if is is still not a success code we have failed a certain number of consecutive times.
            response.EnsureSuccessStatusCode();

            string content;
            if (!sync)
            {
                content = await response.Content.ReadAsStringAsync();
            }
            else
            {
                content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            JToken? result = JsonConvert.DeserializeObject<JToken>(content);
            if (result == null)
            {
                throw new Exception("Response was null");
            }
            try
            {
                JObject? resultAsJObject = result.Value<JObject>();
                if (resultAsJObject != null)
                {
                    resultAsJObject.Add("_server", server);
                }
                JToken? newResult = resultAsJObject?.Value<JToken>();
                if (newResult != null)
                {
                    return newResult;
                }
                else
                {
                    return result;
                }
            }
            catch
            {
                // quietly catch this because it isn't a real error. This just adds a _server field to objects retrieved from the API. This doesn't work for JArrays obviously.
            }
            return result;
        }
        /// <inheritdoc />
        public async Task<JToken> FetchJSON(string urlPath, string type = "videos", string[]? fields = null, string? server = null)
        {
            return await FetchJSONOptionalSync(urlPath, type, fields, server, false);
        }
        /// <inheritdoc />
        public JToken FetchJSONSync(string urlPath, string type = "videos", string[]? fields = null, string? server = null)
        {
            return FetchJSONOptionalSync(urlPath, type, fields, server, true).GetAwaiter().GetResult();
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
        private async Task<IList<string>> GetInvidiousAPIsOptionalSync(Func<InvidiousInstance, bool>? condition = null, bool sync = false)
        {
            List<string> response = new();
            HttpResponseMessage instancesResponse = await FetchOptionalSync("https://api.invidious.io/instances.json?pretty=1&sort_by=type,users", null, sync);
            // If we get an error response, something is probably wrong.
            instancesResponse.EnsureSuccessStatusCode();
            if (instancesResponse.Content != null)
            {
                string content;
                if (!sync)
                {
                    content = await instancesResponse.Content.ReadAsStringAsync();
                }
                else
                {
                    content = instancesResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
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
                    await LogOptionalSync("Json response from server was null.", LogLevel.Error, new Exception(content));
                }
            }
            return response;
        }

        public async Task<IList<string>> GetInvidiousAPIs(Func<InvidiousInstance, bool>? condition = null)
        {
            return await GetInvidiousAPIsOptionalSync(condition, false);
        }
        /// <inheritdoc />
        public IList<string> GetInvidiousAPIsSync(Func<InvidiousInstance, bool>? condition = null)
        {
            return GetInvidiousAPIsOptionalSync(condition, true).GetAwaiter().GetResult();
        }
        private async Task<InvidiousVideo> FetchVideoByIdOptionalSync(string id, string[]? fields = null, bool sync = false)
        {
            JObject? videoObject = (await FetchJSONOptionalSync(id, "videos", fields, null, sync))?.Value<JObject>();
            return new InvidiousVideo(videoObject);
        }
        /// <inheritdoc />
        public async Task<InvidiousVideo> FetchVideoById(string id, string[]? fields = null)
        {
            return await FetchVideoByIdOptionalSync(id, fields, false);
        }
        /// <inheritdoc />
        public InvidiousVideo FetchVideoByIdSync(string id, string[]? fields = null)
        {
            return FetchVideoByIdOptionalSync(id, fields, true).GetAwaiter().GetResult();
        }
        /// <inheritdoc />
        public async Task<InvidiousChannel> FetchChannelById(string id, string[]? fields = null)
        {
            JObject? channelObject = (await FetchJSONOptionalSync(id, "channels", fields, null, false))?.Value<JObject>();
            return new InvidiousChannel(channelObject);
        }
        /// <inheritdoc />
        public InvidiousChannel FetchChannelByIdSync(string id, string[]? fields = null)
        {
            JObject? channelObject = FetchJSONOptionalSync(id, "channels", fields, null, true).GetAwaiter().GetResult()?.Value<JObject>();
            return new InvidiousChannel(channelObject);
        }
        private async Task<IList<InvidiousChannelVideo>> FetchVideosByChannelIdOptionalSync(string channelId, bool sync = false)
        {
            List<InvidiousChannelVideo> result = new();
            JArray? channelList = (await FetchJSONOptionalSync($"{channelId}/videos", "channels", null, null, sync)).Value<JArray>();
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
        public async Task<IList<InvidiousChannelVideo>> FetchVideosByChannelId(string channelId)
        {
            return await FetchVideosByChannelIdOptionalSync(channelId, false);
        }
        /// <inheritdoc />
        public IList<InvidiousChannelVideo> FetchVideosByChannelIdSync(string channelId)
        {
            return FetchVideosByChannelIdOptionalSync(channelId, true).GetAwaiter().GetResult();
        }
        private async Task<InvidiousPlaylist> FetchPlaylistByIdOptionalSync(string id, string[]? fields = null, bool sync = false)
        {
            JObject? playlistObject = (await FetchJSONOptionalSync(id, "playlists", fields, null, sync))?.Value<JObject>();
            InvidiousPlaylist playlist = new(playlistObject);
            var i = 2;
            bool keepGoing = playlist.VideoCount > playlist.Videos.Count;
            while (keepGoing)
            {
                // Not all videos fetched yet
                JObject? innerPlaylistObject = (await FetchJSONOptionalSync($"{id}?page={i}", "playlists", fields, null, sync))?.Value<JObject>();
                InvidiousPlaylist innerPlaylist = new(innerPlaylistObject);
                IEnumerable<InvidiousPlaylistVideo> results = innerPlaylist.Videos.Where(video => !playlist.Videos.Where(playlistVideo => playlistVideo.VideoId == video.VideoId).Any());
                bool anyAdded = results.Any();
                JArray? videos = playlistObject?["videos"]?.Value<JArray>();
                if (videos == null)
                {
                    videos = new JArray();
                }
                foreach (InvidiousPlaylistVideo video in results)
                {
                    videos.Add(video.GetData());
                }
                if (playlistObject != null)
                {
                    playlistObject["videos"] = videos;
                }
                if (!anyAdded)
                {
                    keepGoing = false;
                }
                if (playlist.VideoCount <= playlist.Videos.Count)
                {
                    keepGoing = false;
                }
                i++;
            }
            return new(playlistObject);
        }
        /// <inheritdoc />
        public async Task<InvidiousPlaylist> FetchPlaylistById(string id, string[]? fields = null)
        {
            return await FetchPlaylistByIdOptionalSync(id, fields, false);
        }
        /// <inheritdoc />
        public InvidiousPlaylist FetchPlaylistByIdSync(string id, string[]? fields = null)
        {
            return FetchPlaylistByIdOptionalSync(id, fields, true).GetAwaiter().GetResult();
        }
        private async Task<IList<JObject>> SearchOptionalSync(string query, int page = 0, SortBy? sortBy = null, DateRange? date = null, Duration? duration = null, SearchType? searchType = null, Feature[]? features = null, string? region = null, bool sync = false)
        {
            List<JObject> result = new();
            string queryInterjection = "";
            if (sortBy != null)
            {
                queryInterjection += "&sort_by=";
                switch (sortBy)
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
                queryInterjection += $"&region={region}";
            }
            JToken response = await FetchJSONOptionalSync($"?q={Uri.EscapeDataString(query)}&page={page}{queryInterjection}", "search", null, null, sync);
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
        public async Task<IList<JObject>> Search(string query, int page = 0, SortBy? sortBy = null, DateRange? date = null, Duration? duration = null, SearchType? searchType = null, Feature[]? features = null, string? region = null)
        {
            return await SearchOptionalSync(query, page, sortBy, date, duration, searchType, features, region, false);
        }
        /// <inheritdoc />
        public IList<JObject> SearchSync(string query, int page = 0, SortBy? sortBy = null, DateRange? date = null, Duration? duration = null, SearchType? searchType = null, Feature[]? features = null, string? region = null)
        {
            return SearchOptionalSync(query, page, sortBy, date, duration, searchType, features, region, true).GetAwaiter().GetResult();
        }
        private async Task<IList<string>> SearchSuggestionsOptionalSync(string partialQuery, bool sync = false)
        {
            IList<string> result = new List<string>();
            JToken response = await FetchJSONOptionalSync("suggestions?q=" + partialQuery, "search", null, null, sync);
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
        public async Task<IList<string>> SearchSuggestions(string partialQuery)
        {
            return await SearchSuggestionsOptionalSync(partialQuery, false);
        }
        /// <inheritdoc/>
        public IList<string> SearchSuggestionsSync(string partialQuery)
        {
            return SearchSuggestionsOptionalSync(partialQuery, false).GetAwaiter().GetResult();
        }
        private async Task<InvidiousComments> FetchCommentsByVideoIdOptionalSync(string videoId, bool sync)
        {
            InvidiousComments result = new(null, this);
            JToken response;
            response = await FetchJSONOptionalSync(videoId, "comments", null, null, sync);
            JObject? commentsJObject = response.Value<JObject>();
            if (commentsJObject != null)
            {
                result = new(commentsJObject, this);
            }
            return result;
        }
        /// <inheritdoc/>
        public async Task<InvidiousComments> FetchCommentsByVideoId(string videoId)
        {
            return await FetchCommentsByVideoIdOptionalSync(videoId, false);
        }
        /// <inheritdoc/>
        public InvidiousComments FetchCommentsByVideoIdSync(string videoId)
        {
            return FetchCommentsByVideoIdOptionalSync(videoId, true).GetAwaiter().GetResult();
        }
    }
}
