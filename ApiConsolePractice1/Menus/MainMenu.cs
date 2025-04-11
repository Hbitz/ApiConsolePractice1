using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Menus
{
    internal class MainMenu
    {
        private readonly GithubMenu _githubMenu;

        public MainMenu(GithubMenu githubMenu)
        {
            _githubMenu = githubMenu;
        }
        public async Task ShowMenu()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("API Caller").Color(Color.Blue)); // Fancy ASCII banner

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold green]Main Menu[/]")
                        .AddChoices(new[]
                        {
                            "GitHub API",
                            "Exit"
                        })
                );

                switch (choice)
                {
                    case "GitHub API":
                        await _githubMenu.ShowMenu();
                        break;
                    case "Exit":
                        AnsiConsole.MarkupLine("[green]Exiting program. Goodbye![/]");
                        return;
                }
                AnsiConsole.Markup("\n[cyan]Press any key to return to the main menu...[/]");
                Console.ReadKey();
            }
        }
    }
}
