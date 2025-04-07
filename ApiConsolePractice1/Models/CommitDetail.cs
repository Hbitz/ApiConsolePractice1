using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Models
{
    internal class CommitDetail
    {
        [JsonPropertyName("author")]
        public CommitAuthor Author { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
