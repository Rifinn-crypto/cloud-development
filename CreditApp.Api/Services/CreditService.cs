using CreditApp.Domain.Data;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CreditApp.Api.Services;

/// <summary>
/// Сервис для работы с кредитными заявками, реализующий кэширование через IDistributedCache.
/// </summary>
public class CreditService : ICreditService
{
    private readonly IDistributedCache _cache;
    private readonly ICreditGenerator _generator;
    private readonly ILogger<CreditService> _logger;

    public CreditService(
        IDistributedCache cache,
        ICreditGenerator generator,
        ILogger<CreditService> logger)
    {
        _cache = cache;
        _generator = generator;
        _logger = logger;
    }

    /// <summary>
    /// Получает кредитную заявку по идентификатору. При отсутствии в кэше генерирует новую.
    /// </summary>
    public async Task<CreditApplication> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = $"credit:{id}";

        _logger.LogDebug("Attempting to get credit application {CreditId} from cache", id);

        var cached = await _cache.GetStringAsync(key, cancellationToken);

        if (cached != null)
        {
            _logger.LogInformation(
                "Cache HIT for credit application {CreditId}. Cache size: {SizeBytes} bytes",
                id,
                System.Text.Encoding.UTF8.GetByteCount(cached));

            return JsonSerializer.Deserialize<CreditApplication>(cached)!;
        }

        _logger.LogInformation(
            "Cache MISS for credit application {CreditId}. Generating new instance",
            id);

        var result = _generator.Generate(id);
        await CacheResultAsync(key, result, cancellationToken);

        return result;
    }

    /// <summary>
    /// Получает кредитную заявку по идентификатору с использованием seed для детерминированной генерации.
    /// </summary>
    public async Task<CreditApplication> GetAsync(int id, int seed, CancellationToken cancellationToken = default)
    {
        var key = $"credit:{id}:seed:{seed}";

        _logger.LogDebug("Attempting to get credit application {CreditId} with seed {Seed} from cache", id, seed);

        var cached = await _cache.GetStringAsync(key, cancellationToken);

        if (cached != null)
        {
            _logger.LogInformation(
                "Cache HIT for credit application {CreditId} with seed {Seed}",
                id,
                seed);

            return JsonSerializer.Deserialize<CreditApplication>(cached)!;
        }

        _logger.LogInformation(
            "Cache MISS for credit application {CreditId} with seed {Seed}. Generating new instance",
            id,
            seed);

        var result = _generator.Generate(id, seed);


        await CacheResultAsync(key, result, cancellationToken);

        return result;
    }

    /// <summary>
    /// Удаляет кредитную заявку из кэша.
    /// </summary>
    public async Task RemoveAsync(int id, CancellationToken cancellationToken = default)
    {
        var key = $"credit:{id}";
        await _cache.RemoveAsync(key, cancellationToken);

        _logger.LogWarning(
            "Credit application {CreditId} removed from cache",
            id);
    }

    /// <summary>
    /// Сохраняет сгенерированную кредитную заявку в кэш.
    /// </summary>
    private async Task CacheResultAsync(string key, CreditApplication result, CancellationToken cancellationToken)
    {
        var serialized = JsonSerializer.Serialize(result);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };

        await _cache.SetStringAsync(key, serialized, options, cancellationToken);

        _logger.LogInformation(
            "Generated new credit application {CreditId}. Type: {CreditType}, Amount: {Amount:C}, Status: {Status}",
            result.Id,
            result.CreditType,
            result.RequestedAmount,
            result.Status);
    }
}