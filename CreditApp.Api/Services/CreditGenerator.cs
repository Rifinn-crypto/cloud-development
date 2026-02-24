using Bogus;
using CreditApp.Domain.Data;

namespace CreditApp.Api.Services;

/// <summary>
/// Генератор тестовых данных для кредитных заявок с использованием библиотеки Bogus.
/// </summary>
public class CreditGenerator : ICreditGenerator
{
    private const double CbRate = 16.0;
    private readonly string[] _statuses = { "Новая", "В обработке", "Одобрена", "Отклонена" };
    private readonly string[] _types = { "Потребительский", "Ипотека", "Автокредит" };

    /// <summary>
    /// Генерирует кредитную заявку со случайным seed.
    /// </summary>
    public CreditApplication Generate(int id)
    {
        return Generate(id, new Random().Next(1, 10000));
    }

    /// <summary>
    /// Генерирует кредитную заявку с указанным seed для воспроизводимости результатов.
    /// </summary>
    public CreditApplication Generate(int id, int seed)
    {
        var faker = new Faker<CreditApplication>()
            .RuleFor(x => x.Id, id)
            .RuleFor(x => x.CreditType, f => f.PickRandom(_types))
            .RuleFor(x => x.RequestedAmount, f => Math.Round(f.Random.Decimal(10000, 5_000_000), 2))
            .RuleFor(x => x.TermMonths, f => f.Random.Int(6, 360))
            .RuleFor(x => x.InterestRate, f => Math.Round(f.Random.Double(CbRate, CbRate + 5), 2))
            .RuleFor(x => x.ApplicationDate, f => DateOnly.FromDateTime(f.Date.Past(2)))
            .RuleFor(x => x.HasInsurance, f => f.Random.Bool())
            .RuleFor(x => x.Status, f => f.PickRandom(_statuses))
            .RuleFor(x => x.DecisionDate, (f, x) =>
                x.Status is "Одобрена" or "Отклонена"
                    ? DateOnly.FromDateTime(
                        f.Date.Between(
                            x.ApplicationDate.ToDateTime(TimeOnly.MinValue),
                            DateTime.Now))
                    : null)
            .RuleFor(x => x.ApprovedAmount, (f, x) =>
                x.Status == "Одобрена"
                    ? Math.Round(f.Random.Decimal(10000, x.RequestedAmount), 2)
                    : null);

        faker.UseSeed(seed);

        return faker.Generate();
    }
}