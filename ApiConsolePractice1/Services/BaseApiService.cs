using ApiConsolePractice1.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Services
{
    internal class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected BaseApiService(IConfiguration config)
        {
            _httpClient = HttpClientHelper.GetConfiguredHttpClient(config);
        }

        protected async Task<HttpResponseMessage> GetApiResponse(string apiUrl)
        {
            return await HttpClientHelper.SendRequestAsync(_httpClient, apiUrl);
        }

        protected async Task<HttpResponseMessage> SendPutRequest(string apiUrl)
        {
            return await HttpClientHelper.SendPostRequestAsync(_httpClient, apiUrl);
        }

        protected async Task<HttpResponseMessage> SendDeleteRequest(string apiUrl)
        {
            return await HttpClientHelper.SendDeleteRequestAsync(_httpClient, apiUrl);
        }

        protected async Task<HttpResponseMessage> SendPatchRequest(string apiUrl, object payload)
        {
            return await HttpClientHelper.SendPatchRequestAsync(_httpClient, apiUrl, payload);
        }
    }
}
