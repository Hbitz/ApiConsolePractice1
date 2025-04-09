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
        // Instead of simply returning a Task, we return a task of <Result> to improve error handling

        public async Task<Result<string>> GetAuthenticatedUsername()
        {
            string apiUrl = GithubUrlBuilder.GetAuthenticatedUserInfo();
            var response = await GetApiResponse(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<GithubUserInfo>(json);
                return Result<string>.Success(userInfo.Login);
            }
            return Result<string>.Failure("Failed to retrieve authenticated user information.");
        }

        // Get public repos of searched user
        public async Task<Result<List<GithubRepository>>> GetUserRepositories(string username)
        {
            string apiUrl = GithubUrlBuilder.GetPublicUserRepositories(username);
            var response = await GetApiResponse(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var repos = JsonSerializer.Deserialize<List<GithubRepository>>(json);
                if (repos == null || !repos.Any())
                {
                    AnsiConsole.MarkupLine($"[yellow]No repositories found for user '{username}'.[/]");
                    return Result<List<GithubRepository>>.Failure("No repositories found or failed to parse response");
                }
                PrintRepositoriesTable(repos);
                return Result<List<GithubRepository>>.Success(repos);
            }
            else
            {
                return Result<List<GithubRepository>>.Failure($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task<Result<List<GithubRepository>>> GetAuthenticatedUserRepositories()
        {
            string apiUrl = GithubUrlBuilder.GetAuthenticatedUserRepositories();
            var response = await GetApiResponse(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var repos = JsonSerializer.Deserialize<List<GithubRepository>>(json);
                if (repos == null || !repos.Any())
                {
                    return Result<List<GithubRepository>>.Failure("No repositories found.");
                }
                PrintRepositoriesTable(repos);
                return Result<List<GithubRepository>>.Success(repos);
            }
            return Result<List<GithubRepository>>.Failure($"Error: {response.StatusCode} - {response.ReasonPhrase}");
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
                PrintRepositoriesTable(new List<GithubRepository> { repoInfo }, true); // True to print extended/detailed info
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
        // In some cases, print extensive info about repository.
        private void PrintRepositoriesTable(List<GithubRepository> repos, bool extended = false)
        {
            var table = new Table();
            table.AddColumn("[green]Name[/]");
            table.AddColumn("[blue]Stars[/]");
            table.AddColumn("[cyan]Visibility[/]");
            table.AddColumn("[grey]Description[/]");

            if (extended)
            {
                table.AddColumn("[magenta]Language[/]");
                table.AddColumn("[yellow]Forks[/]");
                table.AddColumn("[purple]Has Wiki[/]");
                table.AddColumn("[white]Last Updated[/]");
            }

            foreach (var repo in repos)
            {
                var baseRow = new List<string>
                {
                    repo.Name,
                    repo.Stars.ToString(),
                    repo.Visibility,
                    repo.Description,
                };

                if (extended)
                {
                    baseRow.Add(repo.Language ?? "N/A");
                    baseRow.Add(repo.Forks.ToString());
                    baseRow.Add(repo.HasWiki.ToString());
                    baseRow.Add(repo.LastUpdate.ToString());
                }
                table.AddRow(baseRow.Select(cell => cell ?? "N/A").ToArray());
            }

            AnsiConsole.Write(table);
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
