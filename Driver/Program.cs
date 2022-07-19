using System;
using MarmadileManteater.InvidiousClient.Extensions;
using MarmadileManteater.InvidiousClient.Interfaces;
using MarmadileManteater.InvidiousClient.Objects;
using MarmadileManteater.InvidiousClient.Objects.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MarmadileManteater.InvidiousClient.Enums;
internal class Program
{
    static void Main(string[] args)
    {
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

    }
}