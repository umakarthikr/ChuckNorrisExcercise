using System;
using System.Threading.Tasks;
using ChuckNorrisAPI.Client;
using ChuckNorrisAPI.Model;

namespace ChuckNorrisAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await GetJoke();
        }

        public static async Task GetJoke()
        {
            var result = await ChuckNorrisClient.GetJokes();
            Console.WriteLine($"Joke: {result.Value}");
            string userInput = Console.ReadKey().Key.ToString();
            
            if (userInput.ToLowerInvariant() == "j")
                await GetJoke();
        }

      
    }
}
