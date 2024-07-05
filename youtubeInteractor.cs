using System;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Api;
using Google.Rpc;
using Newtonsoft.Json.Linq;
using System.Speech.Recognition;

namespace YoutubeFeatures
{
    class Youtube
    {
        //youtube data api key: AIzaSyA0Byd24NHZ4rO4yuGL-1QveVo-7Iw6E2g
        private static readonly string apiKey = "AIzaSyA0Byd24NHZ4rO4yuGL-1QveVo-7Iw6E2g";
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<string> searchOnYoutube(string query)
        {
            string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={Uri.EscapeDataString(query)}&key={apiKey}";
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(jsonResponse);


                var firstResult = json["items"]?[0]?["id"]?["videoId"]?.ToString();
                if(!string.IsNullOrEmpty(firstResult))
                {
                    return $"https://www.youtube.com/watch?v={firstResult}";
                }
            }
            return null;

        }
    }
}












