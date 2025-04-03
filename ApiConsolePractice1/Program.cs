using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiConsolePractice1.Models;

namespace ApiConsolePractice1
{
    internal class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            await GetGithubRepoInfo();
            await GetJsonPlaceholderPost();

        }

        private static async Task GetGithubRepoInfo()
        {
            string apiUrl = "https://api.github.com/repos/dotnet/runtime"; // example api
            client.DefaultRequestHeaders.UserAgent.ParseAdd("C# console app"); //required by github api

            try
            {
                string r = await client.GetStringAsync(apiUrl);
                var jsonDoc = JsonDocument.Parse(r);
                var repo = JsonSerializer.Deserialize<GithubRepository>(r, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // .Serialize<T> Maps the JSNO fields to a c# class(post)

                if (repo != null)
                {
                    Console.WriteLine("\n==== GitHub Repository Info ====");
                    Console.WriteLine($"Name       : {repo.Name}");
                    Console.WriteLine($"Stars      : {repo.Stars}");
                    Console.WriteLine($"Forks      : {repo.Forks}");
                    Console.WriteLine($"Language   : {repo.Language ?? "N/A"}");
                    Console.WriteLine($"Owner      : {repo.Owner.Login}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static async Task GetJsonPlaceholderPost()
        {
            string JsonPlaceholderApiUrl = "https://jsonplaceholder.typicode.com/posts/1";

            try
            {
                string r = await client.GetStringAsync(JsonPlaceholderApiUrl);
                var post = JsonSerializer.Deserialize<Post>(r);

                Console.WriteLine("\n==== JSONPlaceholder Post ====");
                Console.WriteLine($"User ID  : {post.UserId}");
                Console.WriteLine($"Post ID  : {post.Id}");
                Console.WriteLine($"Title    : {post.Title}");
                Console.WriteLine($"Body     : {post.Body}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
