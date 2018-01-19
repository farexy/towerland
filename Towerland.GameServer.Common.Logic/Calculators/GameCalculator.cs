using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Models.Stats;

namespace Towerland.GameServer.Common.Logic.Calculators
{
  public class GameCalculator
  {
    private readonly IStatsLibrary _statsLib;

    public GameCalculator(IStatsLibrary statsLibrary)
    {
      _statsLib = statsLibrary;
    }
    
    public int CalculateDamage(GameObjectType unit, GameObjectType tower)
    {
      var unitStats = _statsLib.GetUnitStats(unit);
      var towerStats = _statsLib.GetTowerStats(tower);
      return CalculateDamage(unitStats, towerStats);
    }
    
    public int CalculateDamage(GameObjectType unit, TowerStats towerStats)
    {
      var unitStats = _statsLib.GetUnitStats(unit);
      return CalculateDamage(unitStats, towerStats);
    }
    
    public int CalculateDamage(UnitStats unitStats, TowerStats towerStats)
    {
      if(towerStats.Attack == TowerStats.AttackType.Burst && unitStats.IsAir)
        return 0;

      return GameMath.Round(towerStats.Damage * _statsLib.GetDefenceCoeff(unitStats.Defence, towerStats.Attack));
    }

    public bool IsTowerCanAttack(GameObject tower, Point position)
    {
      return _statsLib.GetTowerStats(tower.Type).Range >= GameMath.Distance(tower.Position, position);
    }
  }
}