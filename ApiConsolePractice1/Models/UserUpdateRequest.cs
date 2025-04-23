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
        [JsonPropertyName("bio")]
        public string? Bio {  get; set; }
    }
}
