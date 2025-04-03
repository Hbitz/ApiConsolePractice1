using ApiConsolePractice1.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Services
{
    internal static class ApiService
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task GetGithubRepoInfo()
        {
            string apiUrl = "https://api.github.com/repos/dotnet/runtime"; // example api
            client.DefaultRequestHeaders.UserAgent.ParseAdd("C# console app"); //required by github api

            try
            {
                string r = await client.GetStringAsync(apiUrl);
                var jsonDoc = JsonDocument.Parse(r);
                var repo = JsonSerializer.Deserialize<GithubRepository>(r, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // .Serialize<T> Maps the JSNO fields to a c# class(post)

                var table = new Table()
                    .AddColumn("Property")
                    .AddColumn("Value")
                    .AddRow("Name", repo.Name)
                    .AddRow("Stars", repo.Stars.ToString())
                    .AddRow("Forks", repo.Forks.ToString())
                    .AddRow("Language", repo.Language ?? "N/A")
                    .AddRow("Owner", repo.Owner.Login);

                AnsiConsole.Write(table);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine(ex.Message);
            }
        }

        public static async Task GetJsonPlaceholderPost()
        {
            string JsonPlaceholderApiUrl = "https://jsonplaceholder.typicode.com/posts/1";

            try
            {
                string r = await client.GetStringAsync(JsonPlaceholderApiUrl);
                var post = JsonSerializer.Deserialize<Post>(r);


                var panel = new Panel($"[bold]{post.Title}[/]\n\n{post.Body}")
                    .Header($"Post ID: {post.Id} | User ID: {post.UserId}")
                    .Border(BoxBorder.Rounded)
                    .Padding(2, 1);

                AnsiConsole.Write(panel);
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteLine(ex.Message);
            }
        }
    }
}
