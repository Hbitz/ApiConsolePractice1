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

        /// <summary>
        /// GET requests
        /// </summary>
        /// <returns></returns>
        
        // TODO - BuildUrl-method to validate argument and return error info?

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

        public static string GetStarredRepositories()
        {
            return $"{BaseUrl}user/starred";
        }

        public static string UnstarRepository(string owner, string repo)
        {
            return $"{BaseUrl}user/starred/{owner}/{repo}"; 
        }

        /// 
        // Authentication - Yes (Bearer token/PAT)
        // Request Body - None
        // Respones 
            // 204 No content - Success
            // 404 Not found - Repo doesn't exist or missing authentication
            // 401 Unauthorized - Missin/invalid token
        public static string StarRepository(string owner, string repo)
        {
            return $"{BaseUrl}user/starred/{owner}/{repo}";
        }

        public static string UpdateUserBio()
        {
            return $"{BaseUrl}user";
        }
    }
}
