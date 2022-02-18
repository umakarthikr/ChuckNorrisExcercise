using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChuckNorrisAPI.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace ChuckNorrisAPI.Client
{
    public static class ChuckNorrisClient
    {
  
        static string key = "jokesCacheKey";
        static IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static async Task<ChuckNorrisModel> GetJokes()
        {
            
            if (!_cache.TryGetValue(key, out ChuckNorrisModel joke))
            {
              
                var services = new ServiceCollection();
                services.AddHttpClient();
                services.AddMemoryCache();
                var serviceProvider = services.BuildServiceProvider();

                
                var client = serviceProvider.GetService<HttpClient>();
                joke =  await client.GetFromJsonAsync<ChuckNorrisModel>("https://api.chucknorris.io/jokes/random");

                if (joke == null)
                {
                    var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent(string.Format("Jokes can't be retrived")),
                        ReasonPhrase = "Jokes not found"
                    };
                    throw new HttpResponseException(resp);
                }

                MemoryCacheEntryOptions cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddSeconds(50),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromSeconds(20)
                };
                
                _cache.Set(key, joke, cacheExpiryOptions);
            }
            return joke;

           }
    }
}
    

