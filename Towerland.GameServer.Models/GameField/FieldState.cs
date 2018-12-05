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
    }

    public FieldState(Dictionary<int, GameObject> objects, Castle castle, int towerMoney, int monsterMoney)
    {
      Castle = (Castle)castle.Clone();
      Towers = objects.Where(o => o.Value.IsTower).Select(o => o.Value).Cast<Tower>().ToList();
      Units = objects.Where(o => o.Value.IsUnit).Select(o => o.Value).Cast<Unit>().ToList();
      TowerMoney = towerMoney;
      MonsterMoney = monsterMoney;
    }

    public Castle Castle { set; get; }
    public List<Tower> Towers { set; get; }
    public List<Unit> Units { set; get; }

    public int MonsterMoney { set; get; }
    public int TowerMoney { set; get; }
  }
}