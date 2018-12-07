using System;

namespace Towerland.GameServer.Models
{
  public class StaticDataResponseModel
  {
    public StatsResponseModel Stats { get; set; }
    public DateTime ServerTime { get; set; }
  }
}