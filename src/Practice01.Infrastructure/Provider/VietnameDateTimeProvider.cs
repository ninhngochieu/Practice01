using Microsoft.Extensions.DependencyInjection;
using Practice01.Application.Common.Datetime;

namespace Practice01.Infrastructure.Provider;

public class VietnameDateTimeProvider : IDateTimeProvider
{
    private readonly TimeZoneInfo _vietnamTimeZoneInfo;

    public VietnameDateTimeProvider([FromKeyedServices("VietnamTimeZoneInfo")] TimeZoneInfo vietnamTimeZoneInfo)
    {
        _vietnamTimeZoneInfo = vietnamTimeZoneInfo;
    }
    public DateTime Now => TimeZoneInfo.ConvertTime(DateTime.UtcNow, _vietnamTimeZoneInfo);
    public DateTimeOffset OffsetNow => TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _vietnamTimeZoneInfo);
    
}