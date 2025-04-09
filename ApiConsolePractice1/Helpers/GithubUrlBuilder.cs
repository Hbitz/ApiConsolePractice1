using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Helpers
{
    internal static class GithubUrlBuilder
    {
        private const string BaseUrl = "https://api.github.com/";


        //Get URl for authenticated user's username via bearer token
        public static string GetAuthenticatedUserInfo()
        {
            return $"{BaseUrl}user";
        }

        // Gets all repositories of authorized user
        public static string GetAuthenticatedUserRepositories()
        {
            return $"{BaseUrl}user/repos";
        }

        // Gets public repositories of searched user
        public static string GetPublicUserRepositories(string username)
        {
            return $"{BaseUrl}users/{username}/repos";
        }

        // Get URls for repo of user 
        public static string GetRepoDetails(string owner, string repo)
        {
            return $"{BaseUrl}repos/{owner}/{repo}";
        }
        // Get URL for repo commits 
        public static string GetRepoCommits(string owner, string repo)
        {
            return $"{BaseUrl}repos/{owner}/{repo}/commits";
        }
    }
}
