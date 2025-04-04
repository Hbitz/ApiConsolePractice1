using ApiConsolePractice1.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ApiConsolePractice1.Services
{
    internal static class ApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task GetGithubRepoInfo()
        {
            try
            {
                var config = new ConfigurationBuilder()
                    //.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appSettings.json")
                    .Build();

                var token = config["GithubToken"];
                if (string.IsNullOrEmpty(token))
                {
                    AnsiConsole.MarkupLine("[red]GitHub token is missing! Set the GITHUB_TOKEN environment variable.[/]");
                    return;
                }

                // New request headers for authentication
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CSharpConsoleApp");


                // API request
                string apiUrl = "https://api.github.com/repos/Hbitz/ApiConsolePractice1"; // Private repo 
                var r = await _httpClient.GetAsync(apiUrl);

                if (!r.IsSuccessStatusCode)
                {
                    AnsiConsole.MarkupLine($"[red]Error: {r.StatusCode} - {r.ReasonPhrase}[/]");
                    return;
                }

                // Read and deseralize response
                var jsonR = await r.Content.ReadAsStringAsync();
                var repo = JsonSerializer.Deserialize<GithubRepository>(jsonR, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // .Serialize<T> Maps the JSNO fields to a c# class(post)

                // Display Data
                AnsiConsole.MarkupLine($"[green]Name:[/] {repo.Name}");
                AnsiConsole.MarkupLine($"[green]Creator/Owner:[/] {repo.Owner.Login}");
                AnsiConsole.MarkupLine($"[green]Visibility:[/] {repo.Visibility}");
                AnsiConsole.MarkupLine($"[green]Stars:[/] {repo.Stars}");
                AnsiConsole.MarkupLine($"[green]Description:[/] {repo.Description}");
            }
            catch (Exception ex)
            { 
                AnsiConsole.MarkupLine($"[red]Exception: {ex.Message}[/]");

            }
        }

        public static async Task GetUserRepositories()
        {
            AnsiConsole.Markup("[yellow]Enter GitHub username:[/] ");
            string username = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(username))
            {
                AnsiConsole.MarkupLine("[red]Invalid username.[/]");
                return;
            }

            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

            var token = config["GithubToken"];
            // Not needed for endpoint, but we include it anyway. Why?
            // Rate limit of unauthenticated users are 60 request/hour. Authentication with token gets 5000/hour.
            // Consistency and forward compatibility.
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("C# practice console app");

            string apiUrl = $"https://api.github.com/users/{username}/repos";

            var r = await _httpClient.GetAsync(apiUrl);

            if (!r.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[red]Error: {r.StatusCode} - {r.ReasonPhrase}[/]");
                return;
            }

            var json = await r.Content.ReadAsStringAsync();
            var repos = JsonSerializer.Deserialize<List<GithubRepository>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            if (repos == null || repos.Count == 0)
            {
                AnsiConsole.Markup("[yellow]No repositories found.[/]");
                return;
            }

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]Name[/]");
            table.AddColumn("[blue]Stars[/]");
            table.AddColumn("[cyan]Visibility[/]"); // Current endpoints only gets public, so this is unused.
            table.AddColumn("[grey]Description[/]");

            foreach (var repo in repos)
            {
                table.AddRow(repo.Name, repo.Stars.ToString(), repo.Visibility, repo.Description ?? "[italic]No description[/]");
            }

            AnsiConsole.Write(table);


        }

        public static async Task GetJsonPlaceholderPost()
        {
            string JsonPlaceholderApiUrl = "https://jsonplaceholder.typicode.com/posts/1";

            try
            {
                string r = await _httpClient.GetStringAsync(JsonPlaceholderApiUrl);
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
