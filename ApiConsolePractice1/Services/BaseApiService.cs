using ApiConsolePractice1.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiConsolePractice1.Models;

namespace ApiConsolePractice1.Services
{
    internal class BaseApiService
    {
        protected readonly HttpClient _httpClient;
        protected BaseApiService(IConfiguration config)
        {
            var result = HttpClientHelper.GetConfiguredHttpClient(config);
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException($"Failed to configure HttpClient: {result.ErrorMessage}");
            }
            else
            {
                _httpClient = result.Data;
            }
        }

        protected async Task<Result<HttpResponseMessage>> GetApiResponse(string apiUrl)
        {
            return await HttpClientHelper.SendRequestAsync(_httpClient, apiUrl);
        }

        protected async Task<Result<HttpResponseMessage>> SendPutRequest(string apiUrl)
        {
            return await HttpClientHelper.SendPostRequestAsync(_httpClient, apiUrl);
        }

        protected async Task<Result<HttpResponseMessage>> SendDeleteRequest(string apiUrl)
        {
            return await HttpClientHelper.SendDeleteRequestAsync(_httpClient, apiUrl);
        }

        protected async Task<Result<HttpResponseMessage>> SendPatchRequest(string apiUrl, Dictionary<string, string> payload)
        {
            return await HttpClientHelper.SendPatchRequestAsync(_httpClient, apiUrl, payload);
        }
    }
}
