using System;
using System.Collections;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChuckNorrisAPI.Model;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Reflection;

namespace ChuckNorrisAPI.Client
{
    public class ChuckNorrisClient
    {
  
        
        IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        CurrentIndex currentIndex = new CurrentIndex();

        public async Task<ChuckNorrisModel> GetJokes()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

                ChuckNorrisModel joke = new ChuckNorrisModel();
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
                AbsoluteExpiration = DateTime.Now.AddSeconds(60),
                Priority = CacheItemPriority.High,
                SlidingExpiration = TimeSpan.FromSeconds(60)
            };
                
                _cache.Set(joke.Id, joke, cacheExpiryOptions);
            
            return joke;

        }
        public class CurrentIndex
        {
            private int val;

            public CurrentIndex()
            {
               
            }
            public void setValue(int value)
            {
                this.val = value;
            }
            public int getValue()
            {
                return this.val;
            }
        }

        public ChuckNorrisModel GetNextJoke()
        {
         
            int index = currentIndex.getValue();
            List<Microsoft.Extensions.Caching.Memory.ICacheEntry> jokes = GetCacheItemsAsList();

            if (index == jokes.Count)
                index = 0;

            var nextIndex = jokes.IndexOf(jokes[index]) + 1;
            currentIndex.setValue(nextIndex);

            if (nextIndex == jokes.Count)
            {
                return (ChuckNorrisModel)jokes[0].Value;
            }
           

            return (ChuckNorrisModel)jokes[nextIndex].Value;

        }

        public ChuckNorrisModel GetPreviousJoke()
        {
            
            int index = currentIndex.getValue();
            List<Microsoft.Extensions.Caching.Memory.ICacheEntry> jokes = GetCacheItemsAsList();

            if (index == 0)
                index = jokes.Count -1;

            var prevIndex = jokes.IndexOf(jokes[index]) - 1;
            currentIndex.setValue(prevIndex);

            if (prevIndex == 0)
            {
                return (ChuckNorrisModel)jokes[jokes.Count-1].Value;
            }
            

            return (ChuckNorrisModel)jokes[prevIndex].Value;
            
        }

        private List<Microsoft.Extensions.Caching.Memory.ICacheEntry> GetCacheItemsAsList()
        {
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(_cache) as dynamic;

            List<Microsoft.Extensions.Caching.Memory.ICacheEntry> cacheCollectionValues = new List<Microsoft.Extensions.Caching.Memory.ICacheEntry>();

            foreach (var cacheItem in cacheEntriesCollection)
            { 
                Microsoft.Extensions.Caching.Memory.ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);

                cacheCollectionValues.Add(cacheItemValue);
            }
           
            return cacheCollectionValues;
        }

    }
}
    

