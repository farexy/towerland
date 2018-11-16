﻿using System.Collections.Generic;
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

    public FieldState(Dictionary<int, GameObject> objects, Castle castle)
    {
      Objects = objects;
      Castle = (Castle)castle.Clone();
      Towers = objects.Where(o => o.Value.IsTower).Select(o => o.Value).Cast<Tower>().ToList();
      Units = objects.Where(o => o.Value.IsUnit).Select(o => o.Value).Cast<Unit>().ToList();
    }

    public Dictionary<int, GameObject> Objects { private set; get; }
    public Castle Castle { set; get; }
    [JsonIgnore] public List<Tower> Towers { private set; get; }
    [JsonIgnore] public List<Unit> Units { private set; get; }

    public int MonsterMoney { set; get; }
    public int TowerMoney { set; get; }
  }
}