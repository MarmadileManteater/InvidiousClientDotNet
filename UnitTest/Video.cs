
namespace UnitTest
{
    public class Video
    {
        [Fact]
        public void FetchSync()
        {
            IInvidiousAPIClient client = new InvidiousAPIClient();
            string videoId = "jNQXAC9IVRw";
            string title = "Me at the zoo";
            string authorId = "UC4QobU6STFB0P71PMvOGN5A";
            string[] keywords = new string[] { "me at the zoo", "jawed karim", "first youtube video" };
            InvidiousVideo video = client.FetchVideoByIdSync(videoId);
            Assert.NotNull(video);
            Assert.Equal(video.Title, title);
            Assert.Equal(video.VideoId, videoId);
            Assert.Equal(video.AuthorId, authorId);
            var contains = true;
            foreach (string keyword in keywords)
            {
                Assert.Equal(video.Keywords.Contains(keyword), contains);
            }
        }
    }
}