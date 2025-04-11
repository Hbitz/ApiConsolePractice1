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

        // Fetch repos based on url(for authenticated or normal user)
        private async Task<Result<List<GithubRepository>>> FetchAllRepositories(string apiUrl, int perPage = 100)
        {
            var url = $"{apiUrl}?per_page={perPage}";
            var response = await GetApiResponse(url);

            if (!response.IsSuccessStatusCode)
            {
                return Result<List<GithubRepository>>.Failure($"Error: {response.StatusCode} - {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var repos = JsonSerializer.Deserialize<List<GithubRepository>>(json);

            if (repos == null || !repos.Any())
            {
                return Result<List<GithubRepository>>.Failure("No repositories found or failed to parse response.");
            }

            return Result<List<GithubRepository>>.Success(repos);
        }

        // Fetch repositories based on username or authenticated user
        public async Task<Result<List<GithubRepository>>> GetUserRepositories(string username= "", bool isAuthenticatedUser = false)
        {
            string apiUrl = isAuthenticatedUser
                ? GithubUrlBuilder.GetAuthenticatedUserRepositories()
                : GithubUrlBuilder.GetPublicUserRepositories(username);

            // Get all repos
            var result = await FetchAllRepositories(apiUrl);

            if (!result.IsSuccess)
            {
                if (!isAuthenticatedUser)
                {
                    AnsiConsole.MarkupLine($"[yellow]No repositories found for user \"{username}\".[/]");
                }
                return result;
            }

            PrintRepositoriesTable(result.Data);
            return result;
        }

        public async Task<Result<List<GithubRepository>>> GetAuthenticatedUserRepositories()
        {
            return await GetUserRepositories(isAuthenticatedUser: true);
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

        // Public method 
        public void DisplayRepositories(List<GithubRepository> repositories)
        {   
            PrintRepositoriesTable(repositories);
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
