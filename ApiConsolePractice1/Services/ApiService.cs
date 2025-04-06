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
using System.Text.Json.Serialization;

namespace ApiConsolePractice1.Services
{
    internal static class ApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        // Currently a few if/else, depending on authorization.
        // Could extract the if/else authorization and just give the api to this method
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

        // Gets repositores of an user.
        // If searched user is the same as current bearer token, also include private repos.
        public static async Task GetUserRepositories()
        {
            // Get the username to search for
            AnsiConsole.Markup("[yellow]Enter GitHub username:[/] ");
            string enteredUsername = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(enteredUsername))
            {
                AnsiConsole.MarkupLine("[red]Invalid username.[/]");
                return;
            }

            // Get bearer token and setup client.
            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

            var token = config["GithubToken"];
            // Bearer not needed for normal endpoint, but we include it anyway. Why?
            // Rate limit of unauthenticated users are 60 request/hour. Authentication with token gets 5000/hour.
            // Consistency and forward compatibility.
            // Also, it is required if the enteredUser is same as myUsername(of current bearer token)
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("C# practice console app");


            // Get username of current bearer token 
            string authUserUrl = "Https://api.github.com/user";
            var authResponse = await _httpClient.GetAsync(authUserUrl);
            var authJson = await authResponse.Content.ReadAsStringAsync();

            string myUsername = null;
            if (authResponse.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(authJson);
                myUsername = doc.RootElement.GetProperty("login").GetString();
            }
            else
            {
                AnsiConsole.Markup("[red]Could not determine authenticated user.[/]");
            }



            // If entered user has the same username as our bearer token, get all repos of user(even private).
            // Else, get the repositories of an user(only public)
            string apiUrl = enteredUsername == myUsername
                ? "https://api.github.com/user/repos"
                : $"https://api.github.com/users/{enteredUsername}/repos";

            // Make the api call
            var r = await _httpClient.GetAsync(apiUrl);

            if (!r.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[red]Error: {r.StatusCode} - {r.ReasonPhrase}[/]");
                return;
            }

            // If no errors, deserliaze and show all repos in a table.
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
            table.AddColumn("[cyan]Visibility[/]");
            table.AddColumn("[grey]Description[/]");

            foreach (var repo in repos)
            {
                table.AddRow(repo.Name, repo.Stars.ToString(), repo.Visibility, repo.Description ?? "[italic]No description[/]");
            }

            AnsiConsole.Write(table);

            AnsiConsole.Markup("[yellow]Enter repo name you want more info on[/]");
            string repoToGetMoreInfoOn = Console.ReadLine();

            await ApiService.GetRecentCommits(myUsername, repoToGetMoreInfoOn);



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

        public static async Task GetRecentCommits(string owner, string repoName, int count = 5)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

            var token = config["GithubToken"];
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("is this necessary question mark");

            string apiUrl = $"https://api.github.com/repos/{owner}/{repoName}/commits";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[red]Error fetching commits: {response.StatusCode} - {response.ReasonPhrase}[/]");
            }

            var json = await response.Content.ReadAsStringAsync();
            //AnsiConsole.WriteLine(JsonSerializer.Deserialize<JsonElement>(json, new JsonSerializerOptions { WriteIndented = true }).ToString());

            var commits = JsonSerializer.Deserialize<List<CommitInfo>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (commits == null || commits.Count == 0)
            {
                AnsiConsole.Markup("[yellow]No commits found.[/]");
            }

            //var jsonElement = JsonSerializer.Deserialize<JsonElement>(json, new JsonSerializerOptions { WriteIndented = true});
            //AnsiConsole.WriteLine(JsonSerializer.Deserialize<JsonElement>(json, new JsonSerializerOptions { WriteIndented = true}).ToString());

            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]Commit Message[/]");
            table.AddColumn("[blue]Author[/]");
            table.AddColumn("[grey]Date[/]");

            foreach (var commit in commits)
            {
                var msg = commit.Commit.Message;
                var author = commit.Commit.Author?.Name ?? "Unknown";
                var date = commit.Commit.Author?.Date.ToString("g") ?? "Unkown";

                table.AddRow(msg, author, date);
            }

            AnsiConsole.Write(table);

        }
    }
}
