using System;
using System.Collections.Generic;
using System.Linq;
using Towerland.GameServer.Logic;
using Towerland.GameServer.Logic.ActionResolver;
using Towerland.GameServer.Logic.Calculators;
using Towerland.GameServer.Logic.Factories;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Logic.Selectors;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.GameObjects;

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

    private static void TestFieldFactory()
    {
      var field = new FieldFactory()
        .Create(new[,]
        {
          {1,2,1,1,1},
          {1,0,0,0,1},
          {1,0,1,0,1},
          {1,0,1,0,1},
          {1,3,0,0,1},
        }, FieldTheme.SunnyGlade);
      var pathFinder = new PathFinder(field.StaticData);
      pathFinder.AddPath(new List<Point>{field.StaticData.Start, field.StaticData.Finish});
      pathFinder.AddPath(new List<Point>{field.StaticData.Start, new Point(3, 3), field.StaticData.Finish});
      
    }

    static void Main(string[] args)
    {
      TestFieldFactory();
      
      var f = new FieldStorageStub().Get(0);

      var statsStub = new StatsLibrary(new StatsFactory());
      var uFactory = new UnitFactory(statsStub);
      var tFactory = new TowerFactory(statsStub);
      var pathOpt = new PathChooser(statsStub);
      var stateRecalc = new StateChangeRecalculator(pathOpt, statsStub, uFactory, tFactory);

      f.State.MonsterMoney = 100000;
      f.State.TowerMoney = 100000;

      stateRecalc.AddNewTower(f, GameObjectType.Tower_Usual, new CreationOptions{Position = new Point(4, 4)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Cannon, new CreationOptions{Position = new Point(5, 4)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_FortressWatchtower, new CreationOptions{Position = new Point(0, 5)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = new Point(5, 0)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = new Point(9, 9)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_FortressWatchtower, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Poisoning, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});
      stateRecalc.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = FindPosForTower(f)});

      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Goblin);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Dragon);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Goblin);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Goblin);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Goblin);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Goblin);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Demon);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Demon);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Demon);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_BarbarianMage);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateRecalc.AddNewUnit(f, GameObjectType.Unit_Necromancer);

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
          //Thread.Sleep(100);
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
