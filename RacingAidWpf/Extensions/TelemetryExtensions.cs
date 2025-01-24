namespace RacingAidWpf.Extensions;

public static class TelemetryExtensions
{
    private const float MetresPerSecondToKph = 3.6f;

    public static float ToKph(this float speedMetresPerSecond) =>
        Convert.ToInt32(speedMetresPerSecond * MetresPerSecondToKph);

    public static string ToGearString(this int gear)
    {
        return gear switch
        {
            -1 => "R",
            0 => "N",
            _ => gear.ToString()
        };
    }
}