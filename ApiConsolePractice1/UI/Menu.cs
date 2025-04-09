using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using ApiConsolePractice1.Services;

namespace ApiConsolePractice1.UI
{
    internal class Menu
    {
        private readonly GithubApiService _githubApiService;

        public Menu(GithubApiService githubApiService)
        {
            _githubApiService = githubApiService;
        }
        public async Task ShowMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("API Caller").Color(Color.Blue)); // Fancy ASCII banner

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an API to call:")
                        .PageSize(5)
                        .AddChoices(new[] { "GitHub Repository Info", "Get GitHub User Repos", "Exit" })
                );

                switch (choice)
                {
                    case "GitHub Repository Info":
                        // Use _githubApiService to fetch repository info
                        await _githubApiService.GetGithubRepoInfo("Hbitz", "ApiConsolePractice1"); // Example repo, hard coded values
                        break;

                    case "Get GitHub User Repos":
                        await HandleGitHubUserRepos();
                        break;

                    case "Exit":
                        AnsiConsole.MarkupLine("[green]Exiting program. Goodbye![/]");
                        return;
                }

                AnsiConsole.Markup("\n[cyan]Press any key to return to menu...[/]");
                Console.ReadKey();
            }
        }

        // Helper method to prompt user. 
        public string PromptUser(string promptMessage)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(promptMessage).PromptStyle(Color.Cyan1)
                );
        }

        // 
        private async Task HandleGitHubUserRepos()
        {
            var username = PromptUser("Enter GitHub username:");
            // old
            //await _githubApiService.GetUserRepositories(username);
            var result = await _githubApiService.GetUserRepositories(username);

            if (!result.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
                return;
            }

            // Let user make new choice on what they want to do
            var actionChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do next?")
                    .PageSize(5)
                    .AddChoices(new[] { "View Repository Details", "View Commits", "Back to Menu" })
            );

            switch (actionChoice)
            {
                // Currently does not expand 
                case "View Repository Details":
                    var repoName = PromptUser("Enter repository name:");
                    await _githubApiService.GetGithubRepoInfo(username, repoName);
                    break;

                case "View Commits":
                    var commitRepoName = PromptUser("Enter repository name:");
                    await _githubApiService.GetRecentCommits(username, commitRepoName);
                    break;

                case "Back to Menu":
                    return;
            }
        }

        // Old code
        //public static async Task ShowMenu()
        //{
        //    while (true)
        //    {
        //        AnsiConsole.Clear();
        //        AnsiConsole.Write(new FigletText("API Caller").Color(Color.Blue)); // Fancy ASCII banner

        //        var choice = AnsiConsole.Prompt(
        //            new SelectionPrompt<string>()
        //                .Title("Choose an API to call:")
        //                .PageSize(5)
        //                .AddChoices(new[] { "GitHub Repository Info", "Get GitHub User Repos", "JSONPlaceholder Post", "Exit" })
        //        );

        //        switch (choice)
        //        {
        //            case "GitHub Repository Info":
        //                await ApiService.GetGithubRepoInfo();
        //                break;
        //            case "Get GitHub User Repos":
        //                await ApiService.GetUserRepositories();
        //                break;
        //            case "JSONPlaceholder Post":
        //                await ApiService.GetJsonPlaceholderPost();
        //                break;
        //            case "Exit":
        //                AnsiConsole.MarkupLine("[green]Exiting program. Goodbye![/]");
        //                return;
        //        }

        //        AnsiConsole.Markup("\n[cyan]Press any key to return to menu...[/]");
        //        Console.ReadKey();
        //    }
        //}
    }
}
