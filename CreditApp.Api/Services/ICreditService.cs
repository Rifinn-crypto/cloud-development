using CreditApp.Domain.Data;

namespace CreditApp.Api.Services;

public interface ICreditService
{
    /// <summary>
    /// Получить кредитную заявку по идентификатору
    /// </summary>
    public Task<CreditApplication> GetAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить кредитную заявку с указанным seed для генерации
    /// </summary>
    public Task<CreditApplication> GetAsync(int id, int seed, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить заявку из кэша
    /// </summary>
    public Task RemoveAsync(int id, CancellationToken cancellationToken = default);
}