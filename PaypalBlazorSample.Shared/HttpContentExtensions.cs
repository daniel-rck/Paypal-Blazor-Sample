using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PaypalBlazorSample.Shared
{
    public static class HttpContentExtensions
    {
        public static HttpContent ToHttpContent<T>(this T item)
        {
            string json = JsonConvert.SerializeObject(item, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            });
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static async Task<T> ReadAsync<T>(this HttpContent content) where T : class
        {
            string json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
