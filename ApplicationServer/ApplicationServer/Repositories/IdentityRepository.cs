using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicationServer.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ApplicationServer.Repositories
{
    public class IdentityRepository
    {
        private readonly IMemoryCache _cache;

        private HttpClient _client = new HttpClient();
        
        private const string IdentityServerAddress = "identity";

        public IdentityRepository(IMemoryCache cache)
        {
            _cache = cache;
        }

        private readonly MemoryCacheEntryOptions _options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };
        
        public async Task<RSAPubKey> GetPublicKey(string taggedUsername)
        {
            if (_cache.TryGetValue(taggedUsername, out RSAPubKey pubKey)) return pubKey;
            var response =
                await _client.GetAsync(
                    $"http://{IdentityServerAddress}/Identity/GetIdentity?taggedUsername=\"{taggedUsername}\"");
            if (!response.IsSuccessStatusCode) return null;
            var stream = await response.Content.ReadAsStreamAsync();
            var key = await JsonSerializer.DeserializeAsync<RSAPubKey>(stream);
            _cache.Set(taggedUsername, pubKey, _options);
            return key;
        }
    }
}