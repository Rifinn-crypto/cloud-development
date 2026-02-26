using CreditApp.Domain.Data;

namespace CreditApp.Api.Services;

public interface ICreditService
{
    /// <summary>
    /// Получить кредитную заявку по идентификатору
    /// </summary>
    public Task<CreditApplication> GetAsync(
        int id,
        CancellationToken cancellationToken = default);
}