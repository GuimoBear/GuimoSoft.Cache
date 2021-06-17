using GuimoSoft.Cache.InMemory;
using System;
using System.Threading.Tasks;

namespace GuimoSoft.Cache.Tests
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var cache = new Cache<string, TimeSpan>(config => 
                config.WithTTL(TimeSpan.FromMinutes(10))
                      .WithCleaner(TimeSpan.FromSeconds(1)));

            var value = cache.Get("key").OrAdd(() => TimeSpan.FromMinutes(5));

            var asyncValue = await cache.Get("async-key").OrAddAsync(() => Task.FromResult(TimeSpan.FromMinutes(5)));

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}
