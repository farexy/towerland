using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Towerland.GameServer.Common.Models.GameObjects;

namespace Towerland.GameServer.Common.Models.GameField
{
  public class FieldState
  {
    public FieldState()
    {
      Towers = new List<Tower>();
      Units = new List<Unit>();
    }

    public FieldState(IEnumerable<Tower> towers, IEnumerable<Unit> units, Castle castle)
    {
      Castle = (Castle)castle.Clone();
      Towers = towers.Select(t => (Tower)t.Clone()).ToList();
      Units = units.Select(u => (Unit)u.Clone()).ToList();
    }
    
    public Dictionary<int, GameObject> Objects { set; get; }
    public List<Tower> Towers { private set; get; }
    public List<Unit> Units { private set; get; }
    public Castle Castle { set; get; }

    public int MonsterMoney { set; get; }
    public int TowerMoney { set; get; }
  }
}