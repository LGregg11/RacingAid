using RacingAidData;

namespace RacingAidWpf.Core.Singleton;

public static class RacingAidSingleton
{
    private static RacingAid racingAid;

    public static RacingAid Instance => racingAid ??= new RacingAid();
}