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
    internal class GithubRepoMenu
    {
        private readonly GithubApiService _githubApiService;

        public GithubRepoMenu(GithubApiService githubApiService)
        {
            _githubApiService = githubApiService;
        }

        public async Task ShowMenu(string username, List<GithubRepository> repos)
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Repo Actions").Color(Color.Yellow));
                _githubApiService.DisplayRepositories(repos);

                // Let user make new choice on what they want to do
                var actionChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What would you like to do next?")
                        .PageSize(5)
                        .AddChoices(new[] { "View Repository Details", "View Commits", "Star a Repository","Back to Menu" })
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

                    case "Star a Repository":
                        var starRepoName = PromptUser("Enter a repository to star");
                        var result = await _githubApiService.StarRepositoryAsync(username, starRepoName);
                        if (result.IsSuccess)
                        {
                            AnsiConsole.MarkupLine("[green]Repository starred successfully![/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine($"[red]Error: {result.ErrorMessage}[/]");
                        }
                        break;

                    case "Back to Menu":
                        return;
                }
                // After the action, wait for the user to press a key to continue
                AnsiConsole.Markup("\n[cyan]Press any key to go back...[/]");
                Console.ReadKey();
            }
        }
        public string PromptUser(string promptMessage)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<string>(promptMessage).PromptStyle(Color.Cyan1)
            );
        }
    }
}
