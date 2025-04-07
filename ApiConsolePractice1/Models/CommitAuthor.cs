using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Models
{
    internal class CommitAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }
    }
}
