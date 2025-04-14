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
        public static void DisplayResult(Result result)
        {
            if (result.IsSuccess)
            {
                AnsiConsole.MarkupLine("[green]Operation completed successfully![/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
            }
        }

        public static void DisplayResult<T>(Result<T> result, Action<T> onSuccess)
        {
            if (result.IsSuccess)
            {
                onSuccess(result.Data);
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{result.ErrorMessage}[/]");
            }
        }
    }
}
