using System.ComponentModel.Design;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiConsolePractice1.Menus;
using ApiConsolePractice1.Models;
using ApiConsolePractice1.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiConsolePractice1
{
    internal class Program
    {
        // Major concerns:
        // New Exception class, or extend Result<T> to handle error codes for more specification regarding error handling? 
        
        // TODO
        // Possible extension of above: Add status codes of failures to Result?
        // Clear up duplicated error messages and creat consistent UI behavior after displaying messages

        static async Task Main(string[] args)
        {
            // Dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // Set base path for config
                    .AddJsonFile("appSettings.json", optional: false)
                    .Build())
                .AddSingleton<GithubApiService>() // Register service 
                .AddSingleton<GithubApiService>()  // Register your services
                .AddSingleton<GithubMenu>()       // Register the menus
                .AddSingleton<GithubRepoMenu>()   // Register the repository menu
                .AddSingleton<MainMenu>()         // Register the main menu
                .BuildServiceProvider(); // Build the service provider

            // Resolve the menu service, which will automatically get all requires API services injected and use the service
            var mainMenu = serviceProvider.GetRequiredService<MainMenu>();



            await mainMenu.ShowMenu();
        }
    }
}
