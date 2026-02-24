using Bogus;
using CreditApp.Domain.Data;

namespace CreditApp.Api.Services;

public class CreditGenerator
{
    private const double CbRate = 16.0;

    public CreditApplication Generate(int id)
    {
        var statuses = new[] { "Новая", "В обработке", "Одобрена", "Отклонена" };
        var types = new[] { "Потребительский", "Ипотека", "Автокредит" };

        var faker = new Faker<CreditApplication>()
            .RuleFor(x => x.Id, id)
            .RuleFor(x => x.CreditType, f => f.PickRandom(types))
            .RuleFor(x => x.RequestedAmount, f => Math.Round(f.Random.Decimal(10000, 5_000_000), 2))
            .RuleFor(x => x.TermMonths, f => f.Random.Int(6, 360))
            .RuleFor(x => x.InterestRate, f => Math.Round(f.Random.Double(CbRate, CbRate + 5), 2))
            .RuleFor(x => x.ApplicationDate, f => DateOnly.FromDateTime(f.Date.Past(2)))
            .RuleFor(x => x.HasInsurance, f => f.Random.Bool())
            .RuleFor(x => x.Status, f => f.PickRandom(statuses))
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

        return faker.Generate();
    }
}