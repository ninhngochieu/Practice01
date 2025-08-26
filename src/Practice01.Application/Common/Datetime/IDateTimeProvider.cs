namespace Practice01.Application.Common.Datetime;

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTimeOffset OffsetNow { get; }
}