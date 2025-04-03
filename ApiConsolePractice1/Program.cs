using System.ComponentModel.Design;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiConsolePractice1.Models;
using ApiConsolePractice1.UI;

namespace ApiConsolePractice1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Menu.ShowMenu();
        }
    }
}
