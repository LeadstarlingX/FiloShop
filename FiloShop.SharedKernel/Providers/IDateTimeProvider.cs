namespace FiloShop.SharedKernel.Providers;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
    DateOnly Today { get; }
    DateOnly UtcToday { get; }
}