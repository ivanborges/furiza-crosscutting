using Furiza.Base.Core.Identity.Abstractions;
using Furiza.Networking.Abstractions;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Furiza.Networking
{
    internal class HttpClientBasedOnUserPrincipalFactory : IHttpClientFactory
    {
        private readonly IUserPrincipalBuilder userPrincipalBuilder;

        public HttpClientBasedOnUserPrincipalFactory(IUserPrincipalBuilder userPrincipalBuilder)
        {
            this.userPrincipalBuilder = userPrincipalBuilder ?? throw new ArgumentNullException(nameof(userPrincipalBuilder));
        }

        public HttpClient Create() => Create(string.Empty);

        public HttpClient Create(string baseAddress)
        {
            var httpClient = new HttpClient();

            if (!string.IsNullOrWhiteSpace(baseAddress))
                httpClient.BaseAddress = new Uri(baseAddress);

            var accessToken = userPrincipalBuilder.GetAccessToken();
            if (!string.IsNullOrWhiteSpace(accessToken))
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            return httpClient;
        }
    }
}