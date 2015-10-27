using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Service.ServiceTests
{
    internal static class RestRequestUtils
    {
        public const string BaseUrl = "http://localhost:9999/";
        
        public static async Task<HttpResponseMessage> PostAsync(string url, object data)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                return await client.PostAsync(url, content);
            }
        }

        public static async Task<HttpResponseMessage> GetAsync(string url)
        {
            using (var client = new HttpClient { BaseAddress = new Uri(BaseUrl) })
            {
                return await client.GetAsync(url);
            }
        }
    }
}