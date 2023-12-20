namespace SampleIoTApp.Application.Utilities;
public static class DistanceUtilities
{
    private const double EARTH_RADIUS = 6371000;
    public static double CalculateHaversineDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
    {
        if (lon1 > 180 || lon1 < -180 || lon2 > 180 || lon1 < -180)
            return 0;

        if (lat1 > 90 || lat1 < -90 || lat2 > 90 || lat2 < -90)
            return 0;

        lat1 = DegreesToRadians(lat1); lon1 = DegreesToRadians(lon1);
        lat2 = DegreesToRadians(lat2); lon2 = DegreesToRadians(lon2);

        double dLat = lat2 - lat1;
        double dLon = lon2 - lon1;

        double a = Math.Pow(Math.Sin(dLat / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(dLon / 2), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EARTH_RADIUS * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return Math.PI * degrees / 180;
    }
}