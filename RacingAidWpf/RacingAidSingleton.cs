using RacingAidData;

namespace RacingAidWpf;

public static class RacingAidSingleton
{
    private static RacingAid? racingAid;

    public static RacingAid Instance => racingAid ??= new RacingAid();
}