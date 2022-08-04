using System.Net.Http.Headers;

namespace ComplianceFileDownloader
{
    public class HttpRequestBuilder
    {
        private HttpMethod? method = null;
        private string requestUri = "";
        private HttpContent? content = null;
        private string bearerToken = "";
        private string acceptHeader = "application/json";
        private TimeSpan timeout = new TimeSpan(0, 0, 30);

        public HttpRequestBuilder()
        {
        }

        public HttpRequestBuilder AddMethod(HttpMethod method)
        {
            this.method = method;
            return this;
        }

        public HttpRequestBuilder AddRequestUri(string requestUri)
        {
            this.requestUri = requestUri;
            return this;
        }

        public HttpRequestBuilder AddContent(HttpContent content)
        {
            this.content = content;
            return this;
        }

        public HttpRequestBuilder AddBearerToken(string bearerToken)
        {
            this.bearerToken = bearerToken;
            return this;
        }

        public HttpRequestBuilder AddAcceptHeader(string acceptHeader)
        {
            this.acceptHeader = acceptHeader;
            return this;
        }

        public HttpRequestBuilder AddTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        public async Task<HttpResponseMessage> SendAsync(TimeSpan cancelationTime)
        {
            this.timeout = cancelationTime;
            return await SendAsync();
        }

        public async Task<HttpResponseMessage> SendAsync()
        {
            // Check required arguments
            //EnsureArguments();

            var httpUri =
                (!string.IsNullOrEmpty(this.requestUri) && this.requestUri.StartsWith("file://"))
                ? this.requestUri.Replace("file://", "https://") : this.requestUri;


            // Setup request
            var request = new HttpRequestMessage
            {
                Method = this.method,
                RequestUri = new Uri(httpUri)
            };

            if (this.content != null)
                request.Content = this.content;

            if (!string.IsNullOrEmpty(this.bearerToken))
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", this.bearerToken);

            request.Headers.Accept.Clear();
            if (!string.IsNullOrEmpty(this.acceptHeader))
                request.Headers.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(this.acceptHeader));

            // Setup client
            var client = new System.Net.Http.HttpClient();
            client.Timeout = this.timeout;

            return await client.SendAsync(request);
        }

        public static HttpRequestBuilder Get(string url)
        {
            return CreateInternal(HttpMethod.Get, url);
        }

        public static HttpRequestBuilder Post(string url)
        {
            return CreateInternal(HttpMethod.Post, url);
        }

        private static HttpRequestBuilder CreateInternal(HttpMethod method, string url)
        {
            return new HttpRequestBuilder()
                .AddMethod(method)
                .AddRequestUri(url);
        }
    }
}
