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
                        "Exit"
                        }));

                switch (choice)
                {
                    case "Get Github User Repos":
                        await HandleGitHubUserRepos();
                        //await GithubRepoMenu.ShowMenu(isAuthenticated: true);
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


        private async Task HandleGitHubUserRepos()
        {
            // Get username to search for
            var username = PromptUser("Enter GitHub username:");

            // Gets username of via bearer token
            var authUserResult = await _githubApiService.GetAuthenticatedUsername();
            if (!authUserResult.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]{authUserResult.ErrorMessage}[/]");
                return;
            }
            // Compare usernames to determine endpoint
            var isSelf = string.Equals(username, authUserResult.Data, StringComparison.OrdinalIgnoreCase);
            var result = await _githubApiService.GetUserRepositories(username, isSelf);

            if (!result.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
                return;
            }



            // Display the next options
            await _githubRepoMenu.ShowMenu(username, result.Data);

            //// Let user make new choice on what they want to do
            //var actionChoice = AnsiConsole.Prompt(
            //    new SelectionPrompt<string>()
            //        .Title("What would you like to do next?")
            //        .PageSize(5)
            //        .AddChoices(new[] { "View Repository Details", "View Commits", "Back to Menu" })
            //);

            //switch (actionChoice)
            //{
            //    // Currently does not expand 
            //    case "View Repository Details":
            //        var repoName = PromptUser("Enter repository name:");
            //        await _githubApiService.GetGithubRepoInfo(username, repoName);
            //        break;

            //    case "View Commits":
            //        var commitRepoName = PromptUser("Enter repository name:");
            //        await _githubApiService.GetRecentCommits(username, commitRepoName);
            //        break;

            //    case "Back to Menu":
            //        return;
            //}
        }
    }
}
