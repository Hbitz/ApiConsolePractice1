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
                        "Update User Profile Bio",
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
                    case "Update User Profile Bio":
                        await UpdateBioAsync();
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
            
            // Show result, whether it's success or failure/error
            ResultDisplayHelper.DisplayResult(result, GithubDisplayHelper.DisplayRepositoryList);
            // If error, let user go back when he's ready.
            if (!result.IsSuccess)
            {
                AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
                Console.ReadKey();
                return;
            }

            // If successful, let user make a choice if they want to go back to menu or un-star a repository
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("What would you like to go?")
                .AddChoices("Unstar a repository", "Return to Menu"));

            var starredRepos = result.Data;

            switch (choice)
            {
                case "Unstar a repository":
                    var selectedRepo = AnsiConsole.Prompt(
                        new SelectionPrompt<GithubRepository>()
                        .Title("Select a repository to unstar:")
                        .UseConverter(repo => repo.Name)
                        .AddChoices(starredRepos));
                    await UnstarRepositoryAsync(selectedRepo);
                    break;
                case "Return to Menu":
                    break;
            }
        }

        private async Task UnstarRepositoryAsync(GithubRepository repo)
        {
            var result = await _githubApiService.UnstarRepositoryAsync(repo.Owner.Login, repo.Name);
            ResultDisplayHelper.DisplayResult(result);
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

        private async Task UpdateBioAsync()
        {
            var newBio = PromptUser("Enter your new bio:");

            var result = await _githubApiService.UpdateUserBio(newBio);
            ResultDisplayHelper.DisplayResult(result);

            AnsiConsole.MarkupLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

    }
}
