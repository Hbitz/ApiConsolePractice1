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
    // This class is now obsolete/used.
    // Was used before big refactor.
    // Soon to be removed entirely.
    internal static class ApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly IConfiguration _config;

        static ApiService()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

            // Some endpoints requires authorization via bearer token, some doesn't.
            // But for consistency and for api call rate limit, we set our httpClient to always include our bearer.
            var token = _config["GithubToken"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("c# console practice app");
        }

        // Get info about a specific repo
        public static async Task GetGithubRepoInfo()
        {
            try
            {
                string apiUrl = "https://api.github.com/repos/Hbitz/ApiConsolePractice1"; // Private repo 
                var r = await _httpClient.GetAsync(apiUrl);

                if (!r.IsSuccessStatusCode)
                {
                    AnsiConsole.MarkupLine($"[red]Error: {r.StatusCode} - {r.ReasonPhrase}[/]");
                    return;
                }

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
        public static async Task GetUserRepositories()
        {
            // Get the username to search for
            string enteredUsername = PromptForUsername();

            // Gets username of our current bearer token
            string myUsername = await GetAuthenticatedUsername();

            // Decide which endpoint depending on result.
            // If both match it means we have authorization, so we get all the repositories.
            // Else we just get the public ones
            string apiUrl = enteredUsername == myUsername
                ? "https://api.github.com/user/repos"
                : $"https://api.github.com/users/{enteredUsername}/repos";

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

            PrintRepositoriesTable(repos);

            // Allow user to choose if they want more info about a certain repo, or go back to menu
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Would you like to see commits of a certain repository, or go back to menu?")
                    .PageSize(5)
                    .AddChoices(new[] { "See commits", "Go back" })
            );

            switch (choice)
            {
                case "See commits":
                    await ApiService.GetRecentCommits(myUsername);
                    break;
                case "Go back":
                    break;
            }
        }

        private static void PrintRepositoriesTable(List<GithubRepository>? repos)
        {
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
        }

        private static async Task<string> GetAuthenticatedUsername()
        {
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
            return myUsername;
        }

        private static string PromptForUsername()
        {
            while (true)
            {
                AnsiConsole.Markup("[yellow]Enter GitHub username: [/] ");
                string enteredUsername = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(enteredUsername))
                {
                    return enteredUsername;
                }
                AnsiConsole.MarkupLine("[red]Invalid username.[/]");
            }
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

        public static async Task GetRecentCommits(string owner, int count = 5)
        {
            AnsiConsole.Markup("[yellow]Enter repo name you want more info on: [/]");
            string repoName = Console.ReadLine();
            string apiUrl = $"https://api.github.com/repos/{owner}/{repoName}/commits";

            var response = await _httpClient.GetAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                AnsiConsole.MarkupLine($"[red]Error fetching commits: {response.StatusCode} - {response.ReasonPhrase}[/]");
            }

            var json = await response.Content.ReadAsStringAsync();
            var commits = JsonSerializer.Deserialize<List<CommitInfo>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (commits == null || commits.Count == 0)
            {
                AnsiConsole.Markup("[yellow]No commits found.[/]");
            }

            PrintCommitsTable(commits);
        }

        private static void PrintCommitsTable(List<CommitInfo>? commits)
        {
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
