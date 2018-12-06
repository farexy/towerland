using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Models.GameField
{
  public class FieldState
  {
    public FieldState()
    {
      Towers = new List<Tower>();
      Units = new List<Unit>();
      RevivedUnits = (new HashSet<int>(), new HashSet<int>());
    }

    public FieldState(Dictionary<int, GameObject> objects, FieldState prevState)
    {
      Castle = (Castle)prevState.Castle.Clone();
      Towers = objects.Where(o => o.Value.IsTower).Select(o => o.Value).Cast<Tower>().ToList();
      Units = objects.Where(o => o.Value.IsUnit).Select(o => o.Value).Cast<Unit>().ToList();
      TowerMoney = prevState.TowerMoney;
      MonsterMoney = prevState.MonsterMoney;
      RevivedUnits = (new HashSet<int>(prevState.RevivedUnits.OldIds), new HashSet<int>(prevState.RevivedUnits.NewIds));
    }

    public Castle Castle { set; get; }
    public List<Tower> Towers { set; get; }
    public List<Unit> Units { set; get; }

    public int MonsterMoney { set; get; }
    public int TowerMoney { set; get; }

    [JsonIgnore] public (HashSet<int> OldIds, HashSet<int> NewIds) RevivedUnits { get; }

  }
}