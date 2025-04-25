using ApiConsolePractice1.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Helpers
{
    internal class ResultDisplayHelper
    {
        // This helper class has methods to DisplayResults on both the Result<T> and non-generic Result class.
        
        // For non-generic Result which don't contain any data we need to process.
        public static void DisplayResult(Result result, bool pauseOnError = true)
        {
            if (result.IsSuccess)
            {
                AnsiConsole.MarkupLine("[green]Operation completed successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
                if (pauseOnError)
                {
                    AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
                    Console.ReadKey();
                }
            }
        }

        // If we have data and want to take action with it if successfull, we use this.
        public static void DisplayResult<T>(Result<T> result, Action<T> onSuccess, bool onPauseError = true)
        {
            if (result.IsSuccess)
            {
                onSuccess(result.Data);
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
                if (onPauseError)
                {
                    AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
                    Console.ReadKey();
                }
            }
        }

        // When we are certain we have error, we use this to easily display and easily adjust our flow(pauseOnError) if needed.
        public static void DisplayErrorOnly<T>(Result<T> result, bool pauseOnError = true)
        {
            if (!result.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
                if (pauseOnError)
                {
                    AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
                    Console.ReadKey();
                }
            }
        }
        
        // Same as above, but for the non-generic Result's.
        public static void DisplayErrorOnly(Result result, bool pauseOnError = true)
        {
            if (!result.IsSuccess)
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
                if (pauseOnError)
                {
                    AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
                    Console.ReadKey();
                }
            }
        }
    }
}
