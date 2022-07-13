using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvidiousAPIClient.Extensions;
using InvidiousAPIClient.Interfaces;
using InvidiousAPIClient.Objects.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InvidiousAPIClient.Objects
{
    public class APIClient : IInvidiousAPIClient
    {
        private readonly Dictionary<string, HttpResponseMessage> _httpResponseCache;
        private readonly int _chunkSize;
        private readonly ILogger _logger;
        private int failedAttempts = 0;
        public readonly bool CacheEnabled;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEnabled">whether or not to cache responses from the invidious API will hit up the same url over an over again</param>
        /// <param name="logger"></param>
        public APIClient(bool cacheEnabled, ILogger logger)
        {
            _httpResponseCache = new Dictionary<string, HttpResponseMessage>();
            CacheEnabled = cacheEnabled;
            _logger = logger;
            _chunkSize = 8192;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheEnabled">whether or not to cache responses from the invidious API will hit up the same url over an over again</param>
        public APIClient(bool cacheEnabled = true)
        {
            _httpResponseCache = new Dictionary<string, HttpResponseMessage>();
            CacheEnabled = cacheEnabled;
            _logger = new ConsoleLogger();
            _chunkSize = 8192;
        }

        protected async Task<HttpResponseMessage> Fetch(string url, HttpClient? client = null)
        {
            string requestUrl = "";
            string absolutePath = new Uri(url).AbsolutePath;
            // If there is an entry in the cache for this url,
            // we don't need to check if cache is enabled because
            // the cache will always be empty if it is disabled.
            if (_httpResponseCache.ContainsKey(absolutePath))
            {
                return _httpResponseCache[absolutePath];
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
                message = new HttpResponseMessage();
                message.StatusCode = (System.Net.HttpStatusCode) 500;
                message.Content = new StringContent(error.Message);
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
                    _httpResponseCache.Add(absolutePath, message);
                    await _logger.Trace("Adding to the http response cache: " + url);
                }
            }

            return message;
        }

        public async Task DownloadAllMatchingVideoFormats(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            JObject videoObject = await FetchJSON(videoId, "videos", new string[] { "formatStreams", "adaptiveFormats" });
            JArray? formatStreams = videoObject["formatStreams"]?.Value<JArray>();
            JArray? adaptiveFormats = videoObject["adaptiveFormats"]?.Value<JArray>();
            if (formatStreams != null && adaptiveFormats != null)
            {
                // add the apdative formats to the format streams
                formatStreams = formatStreams.ArrayJoin(adaptiveFormats);

                foreach (JObject jObject in formatStreams)
                {
                    FormatStream stream = new FormatStream(jObject);
                    if (condition == null || condition(stream))
                    {
                        string url = stream.Url;
                        string mimeType = stream.Type;

                        string fileType = mimeType.Split("/")[1].Split(";")[0];

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
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(), 
                                        fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None, _chunkSize, true))
                        {
                            long? contentLength = response.Content?.Headers?.ContentLength.Value;
                            var totalRead = 0L;
                            var totalReads = 0L;
                            var buffer = new byte[_chunkSize];
                            var isMoreToRead = true;
                            await _logger.LogInfo("Downloading stream with itag of \"" + stream.Itag + "\" to the file:\r\n" + fileName);
                            IProgress<double> progress = _logger.GetProgressInterface();
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
                                        double? progressReport = ((double) totalRead) / contentLength;
                                        if (progressReport != null)
                                        {
                                            progress.Report(progressReport.Value);
                                        }
                                    }
                                }

                            } while (isMoreToRead);
                            await _logger.Trace("Finished downloading!");
                        }
                    }
                }
            }
        }

        public void DownloadAllMatchingVideoFormatsSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            Task task = DownloadAllMatchingVideoFormats(videoId, saveDirectory, condition);
            task.Wait();
            return;
        }

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

        public void DownloadFirstMatchingVideoFormatSync(string videoId, string saveDirectory, Func<FormatStream, bool>? condition = null)
        {
            Task task = DownloadFirstMatchingVideoFormat(videoId, saveDirectory, condition);
            task.Wait();
            return;
        }

        public async Task DownloadVideoByFormatTag(string videoId, string saveDirectory, string formatTag)
        {
            await DownloadFirstMatchingVideoFormat(videoId, saveDirectory, (FormatStream stream) =>
            {
                return stream.Itag == formatTag;
            });
        }

        public void DownloadVideoByFormatTagSync(string videoId, string saveDirectory, string formatTag)
        {
            Task task = DownloadVideoByFormatTag(videoId, saveDirectory, formatTag);
            task.Wait();
            return;
        }

        public async Task<JObject> FetchJSON(string urlPath, string type = "videos", string[]? fields = null, string? server = null)
        {
            IList<string> apis = await GetInvidiousAPIs();
            if (server == null)
            {
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

            if (fields != null)
            {
                path += "?fields=" + string.Join(',', fields) + "&pretty=1";
            } else
            {
                path += "?pretty=1";
            
            }

            HttpResponseMessage response = await Fetch(server + "/api/v1/" + path);
            while (!response.IsSuccessStatusCode && failedAttempts < 5)
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
                    server = apis[new Random().Next(apis.Count())];
                    failedAttempts++;
                }
            }
            // if is is still not a success code we have failed 5 consecutive times.
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();
            JObject? result = JsonConvert.DeserializeObject<JObject>(content);
            if (result == null)
            {
                throw new Exception("Response was null");
            }
            return result;
        }

        public JObject FetchJSONSync(string urlPath, string type = "videos", string[]? fields = null, string? server = null)
        {
            Task<JObject> task = FetchJSON(urlPath, type, fields, server);
            task.Wait();
            return task.Result;
        }

        public async Task<List<string>> FetchVideoFormatTags(string videoId)
        {
            List<string> itags = new();
            JObject videoObject = await FetchJSON(videoId, "videos", new string[] { "formatStreams", "adaptiveFormats" });
            JArray? formatStreams = videoObject["formatStreams"]?.Value<JArray>();
            JArray? adaptiveFormats = videoObject["adaptiveFormats"]?.Value<JArray>();
            if (formatStreams != null && adaptiveFormats != null)
            {
                // add the apdative formats to the format streams
                formatStreams = formatStreams.ArrayJoin(adaptiveFormats);
                foreach (JObject streamObject in formatStreams)
                {
                    string? itag = streamObject["itag"]?.Value<string>();
                    if (itag != null)
                    {
                        itags.Add(itag);
                    }
                }
            }
            return itags;
        }

        public List<string> FetchVideoFormatTagsSync(string videoId)
        {
            Task<List<string>> task = FetchVideoFormatTags(videoId);
            task.Wait();
            return task.Result;
        }

        public async Task<List<string>> GetInvidiousAPIs(Func<InvidiousInstance, bool>? condition = null)
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

        public List<string> GetInvidiousAPIsSync(Func<InvidiousInstance, bool>? condition = null)
        {
            Task<List<string>> task = GetInvidiousAPIs(condition);
            task.Wait();
            return task.Result;
        }
    }
}
