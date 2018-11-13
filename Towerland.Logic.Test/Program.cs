using System;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Towerland.GameServer.Common.Models.GameActions;
using Towerland.GameServer.Common.Models.GameField;
using Towerland.GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic;
using Towerland.GameServer.Common.Logic.ActionResolver;
using Towerland.GameServer.Common.Logic.Calculators;
using Towerland.GameServer.Common.Logic.Factories;
using Towerland.GameServer.Common.Logic.Interfaces;
using Towerland.GameServer.Common.Logic.SpecialAI;

namespace Towerland.Logic.Test
{
  class TestViewResolver : IActionResolver
  {
    public void Resolve(GameAction action)
    {
      if(action.ActionId == ActionId.TowerAttacks)
        Console.WriteLine();
    }
  }

  class Program
  {
    private static Point FindPosForTower(Field f)
    {
      var rnd = new Random();
      var pos = f.StaticData.Cells
        .OfType<FieldCell>()
        .Where(c => c.Object == FieldObject.Ground)
        .Where(c => f.State.Towers.All(t => t.Position != c.Position))
        .Select(c => c.Position)
        .ToArray();

      return pos[rnd.Next(pos.Length)];
    }

    private static void Show(Field f)
    {
      Console.Clear();
      for (int i = 0; i < f.StaticData.Width; i++)
      {
        for (int j = 0; j < f.StaticData.Height; j++)
        {
          var unit = f.State.Units.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          var tower = f.State.Towers.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          if(unit != null)
            Console.Write("*");
          else if (tower != null)
            Console.Write("T");
          else
            Console.Write((int)f.StaticData.Cells[i,j].Object);
        }
        Console.WriteLine();
      }

      Console.WriteLine("monst : " + f.State.MonsterMoney);
      Console.WriteLine("tower : " + f.State.TowerMoney);
    }

    static void Main(string[] args)
    {
      var f = new FieldFactoryStub().ClassicField;
      var ss = JsonConvert.SerializeObject(f);

      var statsStub = new StatsLibrary(new StatsFactory());
      var uFactory = new UnitFactory(statsStub);
      var tFactory = new TowerFactory(statsStub);
      var pathOpt = new PathChooser(statsStub);
      var stateChang = new StateChangeRecalculator(pathOpt, statsStub, uFactory, tFactory);

      f.State.MonsterMoney = 100000;
      f.State.TowerMoney = 100000;

      stateChang.AddNewTower(f, GameObjectType.Tower_Usual, new CreationOptions{Position = new Point(4, 4)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Cannon, new CreationOptions{Position = new Point(5, 4)});
      stateChang.AddNewTower(f, GameObjectType.Tower_FortressWatchtower, new CreationOptions{Position = new Point(0, 5)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = new Point(5, 0)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = new Point(9, 9)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_FortressWatchtower, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});

      stateChang.AddNewUnit(f, GameObjectType.Unit_Goblin);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Dragon);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Goblin);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Golem);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Barbarian);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);

      //var f2 = (Field)f.Clone();
      var f2 = f;
      var calc = new StateCalculator(statsStub, f);

      var ticks = calc.CalculateActionsByTicks();
      var resolver = new FieldStateActionResolver(f2);
      
      foreach (var tick in ticks)
      {
        foreach (var action in tick.Actions)
        {
          resolver.Resolve(action);
          Show(f2);
          Thread.Sleep(20);
        }
        if (tick.Actions.Any() && tick.Actions.First().Position == new Point(2, 7))
        {
         // break;
        }
      }

      Console.WriteLine("end");
    }

  }
}
