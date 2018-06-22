using System;

namespace Towerland.GameServer.Common.Logic
{
  public static class TimeHelper
  {
    public const int TickMs = 400;

    public static long TicksToMs(int ticks)
    {
      return ticks * TickMs;
    }

    public static int TimespanToTicks(TimeSpan ts)
    {
      return (int) (ts.TotalMilliseconds / TickMs);
    }

    public static int GetCurrentTickWithOffset(DateTime startTime, int ticksOffset)
    {
      var timeElapsed = DateTime.UtcNow - startTime;
      return TimespanToTicks(timeElapsed) - ticksOffset;
    }
  }
}