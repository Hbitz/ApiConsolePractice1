using ApiConsolePractice1.Helpers;
using ApiConsolePractice1.Models;
using ApiConsolePractice1.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Menus
{
    internal class GithubMenu
    {
        private readonly GithubRepoMenu _githubRepoMenu;
        private readonly GithubApiService _githubApiService;

        public GithubMenu(GithubApiService githubApiService, GithubRepoMenu githubRepoMenu)
        {
            _githubApiService = githubApiService;
            _githubRepoMenu = githubRepoMenu;
        }

        public async Task ShowMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("GitHub API").Color(Color.Green));

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an option")
                        .AddChoices(new[]
                        {
                        "Get Github User Repos",
                        "View My Starred Repositories",
                        "Exit"
                        }));

                switch (choice)
                {
                    case "Get Github User Repos":
                        await HandleGitHubUserRepos();
                        break;
                    case "View My Starred Repositories":
                        await ViewStarredRepositoriesAsync();
                        break;
                    case "Exit":
                        return;
                }
            }
        }

        public string PromptUser(string promptMessage)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(promptMessage).PromptStyle(Color.Cyan1)
                );
        }

        private async Task ViewStarredRepositoriesAsync()
        {
            var result = await _githubApiService.GetStarredRepositoriesAsync();
            ResultDisplayHelper.DisplayResult(result, GithubDisplayHelper.DisplayRepositoryList);
            AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
            Console.ReadKey();
        }
        private async Task HandleGitHubUserRepos()
        {
            // Get username to search for
            var username = PromptUser("Enter GitHub username:");

            // Gets username of via bearer token
            var authUserResult = await GetAuthenticatedUsername();
            if (!authUserResult.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]{authUserResult.ErrorMessage}[/]");
                return;
            }

            // Compare usernames to determine endpoint
            var isSelf = string.Equals(username, authUserResult.Data, StringComparison.OrdinalIgnoreCase);
            var result = await GetUserRepositories(username, isSelf);

            if (!result.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
                return;
            }

            // Display the next options
            await _githubRepoMenu.ShowMenu(username, result.Data);
        }

        private async Task<Result<List<GithubRepository>>> GetUserRepositories(string username, bool isSelf)
        {
            return await _githubApiService.GetUserRepositories(username, isSelf);
        }

        private async Task<Result<string>> GetAuthenticatedUsername()
        {
            return await _githubApiService.GetAuthenticatedUsername();
        }

    }
}
