using System;
using System.Threading.Tasks;
using ChuckNorrisAPI.Client;
using ChuckNorrisAPI.Model;

namespace ChuckNorrisAPI
{
    public class Program
    {
        ChuckNorrisClient client = new ChuckNorrisClient();
        public static async Task Main(string[] args)
        {
            try
            {
             Program p = new Program();
             await p.GetJoke();
            }
            catch(Exception ex)
            {
                Console.WriteLine("There was a problem getting the jokes, Internal server error");
            }
        }

        public async Task GetJoke()
        {
            
            var result = await client.GetJokes();
            Console.WriteLine($"Joke: {result.Value}");
            await GetChoice();

        }

        public async Task GetNextJoke()
        {

            var nextJoke = client.GetNextJoke();
            Console.WriteLine($"Joke: {nextJoke.Value}");
            await GetChoice();
        }

        public async Task GetPreviousJoke()
        {
         
            var PrevJoke = client.GetPreviousJoke();
            Console.WriteLine($"Joke: {PrevJoke.Value}");
            await GetChoice();

        }

        public async Task GetChoice()
        {
            string userInput = Console.ReadKey().Key.ToString();

            if (userInput.ToLowerInvariant() == "j")
                await GetJoke();

            else if (userInput.ToLowerInvariant() == "n")
                await GetNextJoke();

            else if (userInput.ToLowerInvariant() == "p")
                await GetPreviousJoke();

            else
                await GetJoke();



        }

    }
}
