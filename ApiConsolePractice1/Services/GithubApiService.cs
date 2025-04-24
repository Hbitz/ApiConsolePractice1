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
using System.Linq.Expressions;

namespace ApiConsolePractice1.Services
{
    internal class GithubApiService : BaseApiService
    {
        /* Methods are listed by type of request:
         * GET
         * DELETE
         * POST
         * PATCH 
         * Display/public/helper methods
         */

        // TODO
        // * Review if this is handling too many GithubRelated task, not just the GithubApi.
        //   Possibly make services like GithubRepositoryService for all interactions regarding repository?
        // * Currently using Result<T> for success/failure states.
        //   Maybe create exception class for more contextual information, e.g. error due to authentication failure, network issue or specific API response error.
        //   Custom exceptions provides a more cosistent and readable error flow.
        public GithubApiService(IConfiguration config) : base(config)
        {

        }
        // Instead of simply returning a Task, we return a task of <Result> to improve error handling

        // *** GET-requests ***

        public async Task<Result<string>> GetAuthenticatedUsername()
        {
            string apiUrl = GithubUrlBuilder.GetAuthenticatedUserInfo();
            var responseResult = await GetApiResponse(apiUrl);

            if (responseResult.IsSuccess)
            {
                var response = responseResult.Data;
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
            var responseResult = await GetApiResponse(url);


            if (!responseResult.IsSuccess)
            {
                var errorMessage = responseResult.ErrorMessage;
                return Result<List<GithubRepository>>.Failure($"Error: {errorMessage}");
            }

            var response = responseResult.Data;

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
        public async Task<Result<GithubRepository>> GetGithubRepoInfo(string owner, string repo)
        {
            try
            {
                string apiUrl = GithubUrlBuilder.GetRepoDetails(owner, repo);
                var responseResult = await GetApiResponse(apiUrl);

                if (!responseResult.IsSuccess)
                {
                    return Result<GithubRepository>.Failure($"[red]Error: {responseResult.ErrorMessage}[/]");
                }

                var response = responseResult.Data;

                var json = await response.Content.ReadAsStringAsync();
                var repoInfo = JsonSerializer.Deserialize<GithubRepository>(json);
                
                if (repoInfo == null)
                {
                    return Result<GithubRepository>.Failure("Failed to deserialize repository details");
                }

                
                //PrintRepositoriesTable(new List<GithubRepository> { repoInfo }, true); // True to print extended/detailed info
                return Result<GithubRepository>.Success(repoInfo);
                }
            catch (Exception ex)
            {
                return Result<GithubRepository>.Failure($"Exception: {ex.Message}");
            }
        }

        // Get commits of a repository
        public async Task<Result<List<CommitInfo>>> GetRecentCommits(string owner, string repo)
        {
            try
            {
                string apiUrl = GithubUrlBuilder.GetRepoCommits(owner, repo);
                var responseResult = await GetApiResponse(apiUrl);


                if (!responseResult.IsSuccess)
                {
                    return Result<List<CommitInfo>>.Failure($"[red]Error: {responseResult.ErrorMessage}[/]");
                }
                var response = responseResult.Data;
                var json = await response.Content.ReadAsStringAsync();
                var commits = JsonSerializer.Deserialize<List<CommitInfo>>(json);
                //PrintCommitsTable(commits);

                if (commits == null || commits.Count == 0)
                {
                    return Result<List<CommitInfo>>.Failure("[yellow]No commits found.[/]");
                }

                return Result<List<CommitInfo>>.Success(commits);
            }
            catch (Exception ex)
            {
                return Result<List<CommitInfo>>.Failure($"Exception: {ex.Message}");
            }
        }

        public async Task<Result<List<GithubRepository>>> GetStarredRepositoriesAsync()
        {
            try
            {
                string apiUrl = GithubUrlBuilder.GetStarredRepositories();
                var responseResult = await GetApiResponse(apiUrl);


                if (!responseResult.IsSuccess)
                {
                    return Result<List<GithubRepository>>.Failure($"Error: {responseResult.ErrorMessage}");
                }
                var response = responseResult.Data;

                var json = await response.Content.ReadAsStringAsync();
                var starredRepos = JsonSerializer.Deserialize<List<GithubRepository>>(json);

                if (starredRepos == null || starredRepos.Count == 0) // Todo "Any vs Count?"
                {
                    return Result<List<GithubRepository>>.Failure("No starred repositories.");
                }

                return Result<List<GithubRepository>>.Success(starredRepos);
            }
            catch (Exception ex)
            {
                return Result<List<GithubRepository>>.Failure($"Exception: {ex.Message}");
            }
        }
        
        // This is using the same endpoint as when we want to authenticate user. Is this a problem?
        public async Task<Result<GithubUserProfile>> GetAuthenticatedUserProfileAsync()
        {
            try
            {
                string apiUrl = GithubUrlBuilder.GetAuthenticatedUserInfo();
                var responseResult = await GetApiResponse(apiUrl);


                if (!responseResult.IsSuccess)
                {
                    return Result<GithubUserProfile>.Failure($"[red]Error: {responseResult.ErrorMessage}[/]");
                }
                var response = responseResult.Data;

                var json = await response.Content.ReadAsStringAsync();
                var profile = JsonSerializer.Deserialize<GithubUserProfile>(json);

                return Result<GithubUserProfile>.Success(profile);
            }
            catch (Exception ex)
            {
                return Result<GithubUserProfile>.Failure($"Excetion: {ex.Message}");
            }
        }

        
        // *** DELETE-requetss ***
        public async Task<Result> UnstarRepositoryAsync(string owner, string repo)
        {
            try
            {
                string url = GithubUrlBuilder.UnstarRepository(owner, repo);
                var responseResult = await SendDeleteRequest(url);


                if (!responseResult.IsSuccess)
                {
                    return Result.Failure($"Failed to unstar repository: {responseResult.ErrorMessage}");
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Exception: {ex.Message}");
            }
        }



        // *** POST-requests ***
        public async Task<Result> StarRepositoryAsync(string owner, string repo)
        {
            string url = GithubUrlBuilder.StarRepository(owner, repo);
            var responseResult = await SendPutRequest(url);

            if (!responseResult.IsSuccess)
            {
                return Result.Failure($"[red]Error: {responseResult.ErrorMessage}[/]");
            }

            return Result.Success();
        }


        // *** PATCH-requests ***

        public async Task<Result> UpdateUserProfile(Dictionary<string, string> updateRequest)
        {
            try
            {
                string url = GithubUrlBuilder.UpdateUserBio();
                var responseResult = await SendPatchRequest(url, updateRequest);

                if (!responseResult.IsSuccess)
                {
                    return Result.Failure($"Failed to update bio: {responseResult.ErrorMessage}");
                }
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Exception: {ex.Message}");
            }
        }



        // *** Display and helper methods ***

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
