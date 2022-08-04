using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace ComplianceFileDownloader
{
    public class HttpRequestFactory
    {
        public static async Task<string> GetApiToken(string userName, string password, string authUri)
        {
            using (var client = new HttpClient())
            {
                var httpUri =
                    (!string.IsNullOrEmpty(authUri) && authUri.StartsWith("file://"))
                    ? authUri.Replace("file://", "https://") : authUri;

                //setup client
                client.BaseAddress = new Uri(httpUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //setup login data
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", userName),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("scope", "openid email phone profile offline_access roles"),
                });

                //send request
                try
                {
                    HttpResponseMessage responseMessage = await client.PostAsync(httpUri, formContent);

                    //get access token from response body
                    var responseJson = await responseMessage.Content.ReadAsStringAsync();
                    var jObject = JObject.Parse(responseJson);
                    return jObject.GetValue("access_token").ToString();
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                }

                return null;
            }
        }

        public static async Task<HttpResponseMessage> PostWithAuthToken(string requestUri, string authToken, object value)
        {
            string strRequest = JsonConvert.SerializeObject(value);
            HttpContent content = new StringContent(strRequest, Encoding.UTF8, "application/json");
            var builder = new HttpRequestBuilder()
                .AddMethod(HttpMethod.Post)
                .AddRequestUri(requestUri)
                .AddBearerToken(authToken)
                .AddContent(content);

            return await builder.SendAsync();
        }
    }
}
