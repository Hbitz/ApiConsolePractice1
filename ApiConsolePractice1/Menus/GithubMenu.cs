using ApiConsolePractice1.Helpers;
using ApiConsolePractice1.Models;
using ApiConsolePractice1.Services;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Menus
{
    internal class GithubMenu
    {
        private readonly GithubRepoMenu _githubRepoMenu;
        private readonly GithubApiService _githubApiService;

        // TODO - Decouple menus from each other?
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
                        await UpdateUserProfileAsync();
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
            
            // If IsSuccess is true, DisplayRepositoryList, else displays error information.
            ResultDisplayHelper.DisplayResult(result, GithubDisplayHelper.DisplayRepositoryList);
            if (!result.IsSuccess)
            {
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
        }

        private async Task HandleGitHubUserRepos()
        {
            // Get username to search for
            var username = PromptUser("Enter GitHub username:");

            // Gets username of user via bearer token
            var authUserResult = await GetAuthenticatedUsername();

            if (!authUserResult.IsSuccess)
            {
                ResultDisplayHelper.DisplayErrorOnly(authUserResult);
                return;
            }

            // Compare usernames to determine endpoint
            var isSelf = string.Equals(username, authUserResult.Data, StringComparison.OrdinalIgnoreCase);
            var result = await GetUserRepositories(username, isSelf);

            // If we have an error we only need to display the error here.
            // If success we are passing the repo to GithubRepoMenu for further actions.
            if (!result.IsSuccess)
            {
                ResultDisplayHelper.DisplayErrorOnly(result);
                return;
            }

            // Let GithubRepoMenu handle further actions with repository
            await _githubRepoMenu.ShowMenu(username, result.Data);
        }

        // Todo - Worth extracing a method just for the api call?
        private async Task<Result<List<GithubRepository>>> GetUserRepositories(string username, bool isSelf)
        {
            return await _githubApiService.GetUserRepositories(username, isSelf);
        }

        private async Task<Result<string>> GetAuthenticatedUsername()
        {
            return await _githubApiService.GetAuthenticatedUsername();
        }

        private async Task UpdateUserProfileAsync()
        {
            // Get profile information of user
            var profileResult = await _githubApiService.GetAuthenticatedUserProfileAsync();

            // Display the current information
            DisplayGithubUserProfileInformation(profileResult);

            // Let use select which information they want to update
            var updateRequest = PromptForUserProfileUpdate();

            // Ensure we are updating at least one value
            if (!updateRequest.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No fields selected or no values provided. Nothing to update.[/]");
                AnsiConsole.MarkupLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            var result = await _githubApiService.UpdateUserProfile(updateRequest);
            ResultDisplayHelper.DisplayResult(result, false); // add "false" to not pause on error and duplicate "press any key to return to menu" behavior.

            AnsiConsole.MarkupLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        private void DisplayGithubUserProfileInformation(Result<GithubUserProfile> profileResult)
        {
            if (!profileResult.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]{profileResult.ErrorMessage}[/]");
                return;
            }

            var profile = profileResult.Data;

            AnsiConsole.MarkupLine("[blue]Current Profile Info:[/]");
            AnsiConsole.MarkupLine($"[green]Name:[/] {profile.Name ?? "N/A"}");
            AnsiConsole.MarkupLine($"[green]Email:[/] {profile.Email ?? "N/A"}");
            AnsiConsole.MarkupLine($"[green]Blog:[/] {profile.Blog ?? "N/A"}");
            AnsiConsole.MarkupLine($"[green]Bio:[/] {profile.Bio ?? "N/A"}");
            AnsiConsole.MarkupLine("");
        }

        // Validation and specified requirements are handled in Helpers/GithubUserProfileValidator.cs
        private Dictionary<string, string> PromptForUserProfileUpdate()
        {
            var updates = new Dictionary<string, string>();

            // Multi-select
            var fieldsToUpdate = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                .Title("[green]Which fields would you like to update?[/]")
                .NotRequired()
                .InstructionsText("[grey]Use [blue]<space>[/] to seelect, [green]<enter>[/] to accept[/]")
                .AddChoices("name", "email", "blog", "bio"));

            if (!fieldsToUpdate.Any())
            {
                AnsiConsole.MarkupLine("[yellow]No fields selected. Nothing to update.[/]");
                return updates; 
            }

            foreach (var field in fieldsToUpdate)
            {
                while (true)
                {
                    var input = PromptUser($"Enter new value for {field}");
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        break;
                    }
                    
                    // We use the non-generic Result class that we use for all our api calls.
                    // We could create a ValidationResult class, but that would be redundant.
                    var result = GithubUserProfileValidator.Validate(field, input);
                    
                    // The bool is called "IsSuccess" but in this case, we can simply pretend it's called "IsValidated"
                    if (result.IsSuccess)
                    {
                        updates[field] = input;
                        break;
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]{result.ErrorMessage} Please try again.[/]");
                    }
                }
            }
            return updates;
        }
    }
}
