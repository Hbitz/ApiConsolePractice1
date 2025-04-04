using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using ApiConsolePractice1.Services;

namespace ApiConsolePractice1.UI
{
    internal static class Menu
    {

        public static async Task ShowMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("API Caller").Color(Color.Blue)); // Fancy ASCII banner

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose an API to call:")
                        .PageSize(5)
                        .AddChoices(new[] { "GitHub Repository Info", "Get GitHub User Repos", "JSONPlaceholder Post", "Exit" })
                );

                switch (choice)
                {
                    case "GitHub Repository Info":
                        await ApiService.GetGithubRepoInfo();
                        break;
                    case "Get GitHub User Repos":
                        await ApiService.GetUserRepositories();
                        break;
                    case "JSONPlaceholder Post":
                        await ApiService.GetJsonPlaceholderPost();
                        break;
                    case "Exit":
                        AnsiConsole.MarkupLine("[green]Exiting program. Goodbye![/]");
                        return;
                }

                AnsiConsole.Markup("\n[cyan]Press any key to return to menu...[/]");
                Console.ReadKey();
            }
        }
    }
}
