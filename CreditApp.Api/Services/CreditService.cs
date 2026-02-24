using CreditApp.Domain.Data;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CreditApp.Api.Services;

public class CreditService
{
    private readonly IDistributedCache _cache;
    private readonly CreditGenerator _generator;
    private readonly ILogger<CreditService> _logger;

    public CreditService(
        IDistributedCache cache,
        CreditGenerator generator,
        ILogger<CreditService> logger)
    {
        _cache = cache;
        _generator = generator;
        _logger = logger;
    }

    public async Task<CreditApplication> GetAsync(int id)
    {
        var key = $"credit:{id}";
        var cached = await _cache.GetStringAsync(key);

        if (cached != null)
        {
            _logger.LogInformation("Cache HIT {Id}", id);
            return JsonSerializer.Deserialize<CreditApplication>(cached)!;
        }

        _logger.LogInformation("Cache MISS {Id}", id);

        var result = _generator.Generate(id);

        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return result;
    }
}