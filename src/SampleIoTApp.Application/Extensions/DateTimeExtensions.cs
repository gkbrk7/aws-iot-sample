namespace SampleIoTApp.Application.Extensions;

public static class DateTimeExtensions
{
    public static double? GetTimeDifferenceInSeconds(this DateTime startDate, DateTime? endDate)
    {
        if (endDate is null)
            return null;

        return Math.Abs((startDate - endDate).Value.TotalSeconds);
    }
}
