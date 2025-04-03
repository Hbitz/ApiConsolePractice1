using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiConsolePractice1
{
    internal class Owner
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }
        //public string TotalRepositories { get; set; }
    }
}
