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

        /// <summary>
        /// Displays the result of operation
        /// If successful, 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <param name="onSuccess"></param>
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
