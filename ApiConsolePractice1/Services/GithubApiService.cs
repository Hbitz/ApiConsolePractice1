using ApiConsolePractice1.Models;
using Microsoft.Extensions.Configuration;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ApiConsolePractice1.Helpers;

namespace ApiConsolePractice1.Services
{
    internal class GithubApiService : BaseApiService
    {
        public GithubApiService(IConfiguration config) : base(config)
        {

        }

        // Get public repos of searched user
        public async Task GetUserRepositories(string username)
        {
            string apiUrl = GithubUrlBuilder.GetPublicUserRepositories(username);
            var response = await GetApiResponse(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var repos = JsonSerializer.Deserialize<List<GithubRepository>>(json);
                PrintRepositoriesTable(repos);
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error: {response.StatusCode} - {response.ReasonPhrase}[/]");
            }
        }


        // Get a specific repository's details by owner and repository name
        public async Task GetGithubRepoInfo(string owner, string repo)
        {
            string apiUrl = GithubUrlBuilder.GetRepoDetails(owner, repo);
            var response = await GetApiResponse(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var repoInfo = JsonSerializer.Deserialize<GithubRepository>(json);
                PrintRepositoriesTable(new List<GithubRepository> { repoInfo });
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error: {response.StatusCode} - {response.ReasonPhrase}[/]");
            }
        }

        // Get commits of a repository
        public async Task GetRecentCommits(string owner, string repo)
        {
            string apiUrl = GithubUrlBuilder.GetRepoCommits(owner, repo);
            var response = await GetApiResponse(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<List<CommitInfo>>(json);
                PrintCommitsTable(commits);
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Error: {response.StatusCode} - {response.ReasonPhrase}[/]");
            }
        }

        // Display repositories in a table format
        private void PrintRepositoriesTable(List<GithubRepository> repos)
        {
            var table = new Table();
            table.AddColumn("[green]Name[/]");
            table.AddColumn("[blue]Stars[/]");
            table.AddColumn("[cyan]Visibility[/]");
            table.AddColumn("[grey]Description[/]");

            foreach (var repo in repos)
            {
                table.AddRow(repo.Name, repo.Description ?? "No description");
            }

            AnsiConsole.Render(table);
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
