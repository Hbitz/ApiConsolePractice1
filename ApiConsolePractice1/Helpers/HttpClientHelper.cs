using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Helpers
{
    internal class HttpClientHelper
    {
        public static HttpClient GetConfiguredHttpClient(IConfiguration config)
        {
            var httpClient = new HttpClient();
            var token = config["GithubToken"];

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("c# console app api practice");

            return httpClient;
        }

        public static async Task<HttpResponseMessage> SendRequestAsync(HttpClient client, string apiUrl)
        {
            try
            {
                var response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    AnsiConsole.MarkupLine($"[red]Error: {response.StatusCode} - {response.ReasonPhrase}[/]");
                }
                return response;
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Exception: {ex.Message}[/]");
                throw ex;
            }
        }
    }
}
