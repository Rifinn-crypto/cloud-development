using CreditApp.Domain.Data;

namespace CreditApp.Api.Services;

/// <summary>
/// Интерфейс генератора кредитных заявок
/// </summary>
public interface ICreditGenerator
{
    /// <summary>
    /// Сгенерировать кредитную заявку
    /// </summary>
    public CreditApplication Generate(int id);


    /// <summary>
    /// Сгенерировать кредитную заявку с указанным seed
    /// </summary>
    public CreditApplication Generate(int id, int seed);

}