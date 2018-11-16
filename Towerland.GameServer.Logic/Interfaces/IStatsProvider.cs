using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Logic.Interfaces
{
  public interface IStatsProvider
  {
    UnitStats[] GetUnitStats();
    TowerStats[] GetTowerStats();
    DefenceCoeff[] GetDefenceCoeffs();
  }
}