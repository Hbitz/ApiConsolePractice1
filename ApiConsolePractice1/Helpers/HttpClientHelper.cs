using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiConsolePractice1.Models;

namespace ApiConsolePractice1.Helpers
{
    // TODO
    // * Static class makes it difficult to extend if you need to add more behavior(retry logic, timeouts etc)
    //   Consider refactoring to using IHttpClientFactory?
    // 
    internal class HttpClientHelper
    {
        public static Result<HttpClient> GetConfiguredHttpClient(IConfiguration config)
        {
            try
            {
                var httpClient = new HttpClient();
                var token = config["GithubToken"];

                if (!string.IsNullOrEmpty(token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("c# console app api practice");

                return Result<HttpClient>.Success(httpClient);
            }
            catch (Exception ex)
            {
                return Result<HttpClient>.Failure($"Error configuring HttpClient: {ex.Message}");
            }
        }

        public static async Task<Result<HttpResponseMessage>> SendRequestAsync(HttpClient client, string apiUrl)
        {
            try
            {
                var response = await client.GetAsync(apiUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return Result<HttpResponseMessage>.Failure($"{(int)response.StatusCode} - {response.ReasonPhrase}");
                }
                return Result<HttpResponseMessage>.Success(response);
            }
            catch (HttpRequestException ex)
            {
                return Result<HttpResponseMessage>.Failure($"[red]HttpRequestException failed: {ex.Message}[/]");
                throw;
            }
            catch (Exception ex)
            {
                return Result<HttpResponseMessage>.Failure($"[red]Exception: {ex.Message}[/]");
                throw;
            }
        }

        public static async Task<Result<HttpResponseMessage>> SendPostRequestAsync(HttpClient client, string apiUrl)
        {
            try
            {
                var content = new StringContent(""); // Github requires empty body for starring
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var response = await client.PutAsync(apiUrl, content); // PUT to star repo
                if (!response.IsSuccessStatusCode)
                {
                    return Result<HttpResponseMessage>.Failure($"{(int)response.StatusCode} - {response.ReasonPhrase}");
                }
                return Result<HttpResponseMessage>.Success(response);
            }
            catch (HttpRequestException ex)
            {
                return Result<HttpResponseMessage>.Failure($"[red]HttpRequestException: {ex.Message}[/]");
                throw;
            }
            catch (Exception ex)
            {
                return Result<HttpResponseMessage>.Failure($"[red]Exception: {ex.Message}[/]");
                throw;
            }
        }

        public static async Task<Result<HttpResponseMessage>> SendDeleteRequestAsync(HttpClient client, string apiUrl)
        {
            try
            {
                // There is a built-in DeleteAsync which could work, since the Github API technically doesn't require a body for DELETE.
                // But by using SendAsync we get full control of our request, and in this case we set the method to delete.
                // But if we ever need to add a body, DeleteAsync won't let us while this solution works.
                // In short, it gives us more flexibility in case we need it later
                var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Result<HttpResponseMessage>.Failure($"{(int)response.StatusCode} - {response.ReasonPhrase}");
                }
                return Result<HttpResponseMessage>.Success(response);
            }
            catch (HttpRequestException ex)
            {
                return Result<HttpResponseMessage>.Failure($"[red]HTTP Request failed: {ex.Message}[/]");
                throw;
            }
            catch (Exception ex)
            {
                return Result<HttpResponseMessage>.Failure($"[red]Exception: {ex.Message}[/]");
                throw;
            }
        }
        
        public static async Task<Result<HttpResponseMessage>> SendPatchRequestAsync(HttpClient client, string apiUrl, Dictionary<string, string> payload)
        {
            try
            {
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Patch, apiUrl)
                {
                    Content = content
                };

                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return Result<HttpResponseMessage>.Failure($"{(int)response.StatusCode} - {response.ReasonPhrase}");
                }

                return Result<HttpResponseMessage>.Success(response);
            }
            catch (HttpRequestException ex)
            {
                return Result<HttpResponseMessage>.Failure($"[red]HTTP Request failed: {ex.Message}[/]");
                throw;
            }
            catch (Exception ex)
            {
                return Result<HttpResponseMessage>.Failure($"[red]Exception: {ex.Message}[/]");
                throw;
            }
        }
    }
}
