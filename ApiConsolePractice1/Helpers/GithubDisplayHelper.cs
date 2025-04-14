using ApiConsolePractice1.Models;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Helpers
{
    internal class GithubDisplayHelper
    {
        public static void DisplaySingleRepositoryDetails(GithubRepository repo)
        {
            PrintRepositoriesTable(new List<GithubRepository> { repo }, extended: true);
        }

        public static void DisplayRepositoryList(List<GithubRepository> repos)
        {
            PrintRepositoriesTable(repos);
        }

        public static void DisplayCommitsList(List<CommitInfo> commits)
        {
            PrintCommitsTable(commits);
        }

        public static void PrintRepositoriesTable(List<GithubRepository> repos, bool extended = false)
        {
            var table = new Table();
            table.AddColumn("[green]Name[/]");
            table.AddColumn("[blue]Stars[/]");
            table.AddColumn("[cyan]Visibility[/]");
            table.AddColumn("[grey]Description[/]");

            if (extended)
            {
                table.AddColumn("[magenta]Language[/]");
                table.AddColumn("[yellow]Forks[/]");
                table.AddColumn("[purple]Has Wiki[/]");
                table.AddColumn("[white]Last Updated[/]");
            }

            foreach (var repo in repos)
            {
                var baseRow = new List<string>
                {
                    repo.Name,
                    repo.Stars.ToString(),
                    repo.Visibility,
                    repo.Description,
                };

                if (extended)
                {
                    baseRow.Add(repo.Language ?? "N/A");
                    baseRow.Add(repo.Forks.ToString());
                    baseRow.Add(repo.HasWiki.ToString());
                    baseRow.Add(repo.LastUpdate.ToString());
                }
                table.AddRow(baseRow.Select(cell => cell ?? "N/A").ToArray());
            }

            AnsiConsole.Write(table);
        }

        private static void PrintCommitsTable(List<CommitInfo>? commits)
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[green]Commit Message[/]");
            table.AddColumn("[blue]Author[/]");
            table.AddColumn("[grey]Date[/]");

            foreach (var commit in commits)
            {
                var msg = commit.Commit.Message;
                var author = commit.Commit.Author?.Name ?? "Unknown";
                var date = commit.Commit.Author?.Date.ToString("g") ?? "Unkown";
                table.AddRow(msg, author, date);
            }
            AnsiConsole.Write(table);
        }
    }
}
