

# Invidious API Client for .NET 6.0

An library which allows you to easily interface with the [Invidious](https://github.com/iv-org/invidious) API within a .NET 6.0 codebase.

## Downloading a Video
### C#
```c#
    string videoId = "jNQXAC9IVRw";
    string outputDirectory = "C:/temp";

    IInvidiousAPIClient client = new InvidiousAPIClient();
    // Fetch what format tags are available for this video
    IList<string> formatTags = client.FetchVideoFormatTagsSync(videoId);
    // Download the first available format for this video
    client.DownloadVideoByFormatTagSync(videoId, outputDirectory, formatTags.First());
```
### F#
```f#
    let videoId = "jNQXAC9IVRw"
    let outputDirectory = "C:/temp"
    
    let client = new InvidiousAPIClient()
    // Fetch what format tags are available for this video
    let formatTags = client.FetchVideoFormatTagsSync(videoId)
    // Download the first available format for this video
    client.DownloadVideoByFormatTagSync(videoId, outputDirectory, formatTags.First())
```

## Output

![Screenshot 1](https://raw.githubusercontent.com/MarmadileManteater/InvidiousClientDotNet/development/screenshots/screenshot1.gif)
![Screenshot 4](https://raw.githubusercontent.com/MarmadileManteater/InvidiousClientDotNet/development/screenshots/screenshot4.png)

## Executing a Search
### C#
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
### F#
```f#
    let client = new InvidiousAPIClient()
    let searchResults = client.SearchSync("vaporwave mixes", 0, new Nullable<SortBy>(), new Nullable<DateRange>(), new Nullable<Duration>(), SearchType.All)
    for searchResult in searchResults do
        // You can sort between the different types of results with
        // these extension methods
        if searchResult.IsVideo() then
            let video = searchResult.ToVideo()
            Console.WriteLine("VIDEO")
            Console.WriteLine("\t\t" + video.VideoId)
            Console.WriteLine("\t\t" + video.Title)
            Console.WriteLine("\t\t" + video.Author)
        if searchResult.IsChannel() then
            let channel = searchResult.ToChannel()
            Console.WriteLine("CHANNEL")
            Console.WriteLine("\t\t" + channel.AuthorId)
            Console.WriteLine("\t\t" + channel.Author)
        if searchResult.IsPlaylist() then
            let playlist = searchResult.ToPlaylist()
            Console.WriteLine("PLAYLIST")
            Console.WriteLine("\t\t" + playlist.PlaylistId)
            Console.WriteLine("\t\t" + playlist.Title)
            Console.WriteLine("\t\t" + playlist.Author)
    done
```

## Output

![Screenshot 2](https://raw.githubusercontent.com/MarmadileManteater/InvidiousClientDotNet/development/screenshots/screenshot2.gif)

## Fetching search suggestions
### C#
```c#
    IInvidiousAPIClient client = new InvidiousAPIClient();
    IList<string> suggestions = client.SearchSuggestionsSync("vaporwav");
    foreach (string suggestion in suggestions) {
        Console.WriteLine(suggestion);
    }
```
### F#
```f#
    let client = new InvidiousAPIClient()
    let suggestions = client.SearchSuggestionsSync("vaporwav")
    for suggestion in suggestions do
        Console.WriteLine(suggestion)
```

## Output

![Screenshot 5](https://raw.githubusercontent.com/MarmadileManteater/InvidiousClientDotNet/development/screenshots/screenshot5.gif)


## When reaching out to an instance fails

It sends a request to another instance as long as the amount of failures is within the failure tolerance (default 5).

![Screenshot 3](https://raw.githubusercontent.com/MarmadileManteater/InvidiousClientDotNet/development/screenshots/screenshot3.gif)


