using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using System.Security.Policy;
using System.Net.Http.Headers;
using System.IO;

namespace MyProjcetEVE
{

    public class listing
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class video
    {
        public string lenghtText { get; set; }
        public string publishedTimeText { get; set; }
        public List<listing> thumbnails { get; set; }
        public string title { get; set; }
        public string videoId { get; set; }
        public string viewCountText { get; set; }

        public video()
        {
            thumbnails = new List<listing>();
        }
    }
    public class content
    {
        public video video { get; set; }
    }

    public class ListChannel
    {
        public List<Channel> channels { get; set; }
        public ListChannel()
        {
            channels = new List<Channel>();
        }
        public void add(Channel channel)
        {
            channels.Add(channel);
        }
        public void FindPopularVideo()
        {
            channels.Sort((x, y) => x.videosCountText.CompareTo(y.videosCountText));
            List<video> Contents = new List<video>();
            for (int i = 0; i != channels.Count; i++)
            {
                for (int j = 0; j != channels[i].contents.Count; j++)
                {
                    channels[i].contents[j].video.viewCountText = new string(channels[i].contents[j].video.viewCountText.Where(t => char.IsDigit(t)).ToArray());
                    Contents.Add(channels[i].contents[j].video);

                }
            }
            Contents.Sort((x, y) => int.Parse(x.viewCountText).CompareTo(int.Parse(y.viewCountText)));
            Contents.Reverse();
            for (int j = 0; j != 3; j++)
            {
                Console.WriteLine(Contents[j].title);
                Console.WriteLine(Contents[j].viewCountText);
            }

        }
    }
    public class Channel
    {
        public avatar avatar { get; set; }
        public List<content> contents { get; set; }
        public string description { get; set; }
        public string links { get; set; }
        public string next { get; set; }
        public string subscriberCountText { get; set; }
        public string title { get; set; }
        public string vanityChannelUrl { get; set; }
        public bool verified { get; set; }
        public string videosCountText { get; set; }
        public Channel()
        {
            contents = new List<content>();
        }
    }


    public class avatar
    {
        public List<listing> thumbnails { get; set; }
        public avatar()
        {
            thumbnails = new List<listing>();
        }
    }
    public class Youtube
    {
        static async Task<string> GetDataFromAPI(string text)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://youtube-search-and-download.p.rapidapi.com/channel?id=UCAfduZumwpRlryPtqkkRgXw&next=" + text),
                Headers =
    {
        { "x-rapidapi-key", "c46037bca6mshce9c62cd8f0b753p187b4djsn41ac4b93b462" },
        { "x-rapidapi-host", "youtube-search-and-download.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }

        static async Task<string> GetDataFromAPI()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://youtube-search-and-download.p.rapidapi.com/channel?id=UCAfduZumwpRlryPtqkkRgXw&sort=n"),
                Headers =
    {
        { "x-rapidapi-key", "c46037bca6mshce9c62cd8f0b753p187b4djsn41ac4b93b462" },
        { "x-rapidapi-host", "youtube-search-and-download.p.rapidapi.com" },
    },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }
        static async Task Main()
        {

            string result = await GetDataFromAPI();
            /*StreamReader stream = new StreamReader(@"C:\Users\Студент\Desktop\Новый текстовый документ.json");
            string result = stream.ReadToEnd();*/
            Channel My = JsonSerializer.Deserialize<Channel>(result);
            int value = int.Parse(My.videosCountText) / 30;
            ListChannel channel = new ListChannel();
            string Next = My.next;
            channel.add(My);
            for (int i = 0; i != value; i++)
            {
                result = await GetDataFromAPI(Next);
                My = JsonSerializer.Deserialize<Channel>(result);
                channel.add(My);
                Next = My.next;
            }
            channel.FindPopularVideo();

        }
    }
}
