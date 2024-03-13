using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Movies.Client.HttpHandlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ClientCredentialsTokenRequest _tokenRequest;

        public AuthenticationDelegatingHandler(IHttpClientFactory httpClientFactory, ClientCredentialsTokenRequest tokenRequest)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _tokenRequest = tokenRequest ?? throw new ArgumentNullException(nameof(tokenRequest));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpclient = _httpClientFactory.CreateClient("idpclient");

            var tokenresponse = await httpclient.RequestClientCredentialsTokenAsync(_tokenRequest);
            if (tokenresponse.IsError)
            {
                throw new HttpRequestException("something went wrong while requesting the access token");
            }
            request.SetBearerToken(tokenresponse.AccessToken);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
