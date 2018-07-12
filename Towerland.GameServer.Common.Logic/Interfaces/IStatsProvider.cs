using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IStatsProvider
  {
    UnitStats[] Units { get; }
    TowerStats[] Towers { get; }
    DefenceCoeff[] DefenceCoeffs { get; }
  }
}