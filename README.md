

# Invidious API Client for .NET 6.0

An library which allows you to easily interface with the [Invidious](https://github.com/iv-org/invidious) API within a .NET 6.0 codebase.

## Downloading a Video

```c#
    string videoId = "jNQXAC9IVRw";
    string outputDirectory = "C:/temp";

    IInvidiousAPIClient client = new InvidiousAPIClient();
    // Fetch what format tags are available for this video
    IList<string> formatTags = client.FetchVideoFormatTagsSync(videoId)
    // Download the first available format for this video
    client.DownloadVideoByFormatTagSync(videoId, outputDirectory, formatTags.First());
```

## Output

<img src="screenshots/screenshot1.gif"/>
<img src="screenshots/screenshot4.gif" />

## Executing a Search
```c#
    IInvidiousAPIClient client = new InvidiousAPIClient();
    IList<JObject> searchResults = client.SearchSync("vaporwave mixes", 0, null, null, null, SearchType.All);
    foreach (JObject searchResult in searchResults)
    {
        // You can sort between the different types of results with
        // these extension methods
        if (searchResult.IsVideo())
        {
            var video = searchResult.ToVideo();
            Console.WriteLine("VIDEO");
            Console.WriteLine("\t\t" + video.VideoId);
            Console.WriteLine("\t\t" + video.Title);
            Console.WriteLine("\t\t" + video.Author);
        }
        if (searchResult.IsChannel())
        {
            var channel = searchResult.ToChannel();
            Console.WriteLine("CHANNEL");
            Console.WriteLine("\t\t" + channel.AuthorId);
            Console.WriteLine("\t\t" + channel.Author);
        }
        if (searchResult.IsPlaylist())
        {
            var playlist = searchResult.ToPlaylist();
            Console.WriteLine("PLAYLIST");
            Console.WriteLine("\t\t" + playlist.PlaylistId);
            Console.WriteLine("\t\t" + playlist.Title);
            Console.WriteLine("\t\t" + playlist.Author);
        }
    }
```

## Output

<img src="screenshots/screenshot2.gif" />

## When reaching out to an instance fails

It sends a request to another instance as long as the amount of failures is within the failure tolerance (default 5).

<img src="screenshots/screenshot3.gif" />