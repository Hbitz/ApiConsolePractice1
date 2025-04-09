using System.ComponentModel.Design;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiConsolePractice1.Models;
using ApiConsolePractice1.Services;
using ApiConsolePractice1.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiConsolePractice1
{
    internal class Program
    {
        // Personal Todo:
        // Consider creating a Result<T> style wrapper class for better handling errors, bad results and overall logic from api respones
        static async Task Main(string[] args)
        {
            // Dependency injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory()) // Set base path for config
                    .AddJsonFile("appSettings.json", optional: false)
                    .Build())
                .AddSingleton<GithubApiService>() // Register service 
                .AddSingleton<Menu>() // Register the menu
                .BuildServiceProvider(); // Build the service provider

            // Resolve the menu service, which will automatically get all requires API services injected and use the service
            var menu = serviceProvider.GetRequiredService<Menu>();



            await menu.ShowMenu();
        }
    }
}
