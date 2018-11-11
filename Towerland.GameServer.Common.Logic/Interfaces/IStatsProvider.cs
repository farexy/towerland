using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Interfaces
{
  public interface IStatsProvider
  {
    UnitStats[] GetUnitStats();
    TowerStats[] GetTowerStats();
    DefenceCoeff[] GetDefenceCoeffs();
  }
}