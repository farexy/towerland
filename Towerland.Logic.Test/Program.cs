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
    private static void Show(Field f)
    {
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.White;

      for (int i = 0; i < f.StaticData.Width; i++)
      {
        for (int j = 0; j < f.StaticData.Height; j++)
        {
          var unit = f.State.Units.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          var tower = f.State.Towers.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          var dead = f.State.DeadUnits.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          if (unit != null)
          {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(unit.Type == GameObjectType.Unit_Necromancer ? "N" : "*");
            Console.ForegroundColor = ConsoleColor.White;
          }
          else if (tower != null)
          {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("T");
            Console.ForegroundColor = ConsoleColor.White;
          }
          else if (dead != null)
          {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("D");
            Console.ForegroundColor = ConsoleColor.White;
          }
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
      var f2 = JsonConvert.DeserializeObject<Field>(ss);
      f = f2;
      
      var statsStub = new StatsLibrary();
      var uFactory = new UnitFactory(statsStub);
      var tFactory = new TowerFactory(statsStub);
      var pathOpt = new PathOptimisation(statsStub);
      var stateChang = new StateChangeRecalculator(pathOpt, statsStub, uFactory, tFactory);

      f.State.MonsterMoney = 10000;
      f.State.TowerMoney = 10000;

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
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Impling);
      
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Necromancer);

      stateChang.AddNewTower(f, GameObjectType.Tower_Usual, new CreationOptions{Position = new Point(4, 4)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Cannon, new CreationOptions{Position = new Point(5, 4)});
      stateChang.AddNewTower(f, GameObjectType.Tower_FortressWatchtower, new CreationOptions{Position = new Point(0, 5)});
      stateChang.AddNewTower(f, GameObjectType.Tower_FortressWatchtower, new CreationOptions{Position = new Point(0, 6)});
      stateChang.AddNewTower(f, GameObjectType.Tower_FortressWatchtower, new CreationOptions{Position = new Point(0, 7)});
      stateChang.AddNewTower(f, GameObjectType.Tower_FortressWatchtower, new CreationOptions{Position = new Point(0, 8)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Frost, new CreationOptions{Position = new Point(5, 0)});
      stateChang.AddNewTower(f, GameObjectType.Tower_Magic, new CreationOptions{Position = new Point(9, 9)});
      stateChang.AddNewUnit(f, GameObjectType.Unit_Dragon);

      //var pts = f.
      var calc = new StateCalculator(statsStub, f);

      var ticks = calc.CalculateActionsByTicks();
      var resolver = new FieldStateActionResolver(f);
      foreach (var actions in ticks)
      {
        foreach (var action in actions.Actions)
        {
          resolver.Resolve(action);
          Show(f);
          Thread.Sleep(200);
        }
        if (actions.Actions.Any() && actions.Actions.First().Position == new Point(2, 7))
        {
         // break;
        }
      }

      f.SetState(calc.Field.State);
      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      
      calc.SetState(f.State);
      ticks = calc.CalculateActionsByTicks();
      foreach (var actions in ticks)
      {
        foreach (var action in actions.Actions)
        {
          resolver.Resolve(action);
          Show(f);
          Thread.Sleep(100);
        }
      }
    }

  }
}
