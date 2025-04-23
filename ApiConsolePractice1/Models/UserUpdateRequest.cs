using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiConsolePractice1.Models
{
    internal class UserUpdateRequest
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("blog")]
        public string? Blog { get; set; }
        [JsonPropertyName("bio")]
        public string? Bio {  get; set; }

    }
}
