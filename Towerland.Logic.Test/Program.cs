using System;
using System.Linq;
using System.Threading;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic;
using Towerland.GameServer.Common.Logic.ActionResolver;
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
      for (int i = 0; i < f.StaticData.Width; i++)
      {
        for (int j = 0; j < f.StaticData.Height; j++)
        {
          var unit = f.Units.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          var tower = f.Towers.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          if(unit == null)
            Console.Write((int)f.StaticData.Cells[i,j].Object);
          else if (tower != null)
            Console.Write("T");
          else
            Console.Write("*");
        }
        Console.WriteLine();
      }
    }

    static void Main(string[] args)
    {
      var f = new FieldFactoryStub().ClassicField;

      var pf = new PathFinder();
      foreach (var p in pf.ResolvePath(f.Cells, f.Start, f.Finish))
      {
        Console.WriteLine(p);
      }

      var statsStub = new StatsStub();
      var calc = new StateCalculator(statsStub);
      var uFactory = new UnitFactory(statsStub);
      var tFactory = new TowerFactory(statsStub);
      var pathOpt = new PathOptimisation(statsStub);
      var stateChang = new StateChangeRecalculator(pathOpt, statsStub, uFactory, tFactory);

      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewTower(f, GameObjectType.Tower_Usual, new CreationOptions{Position = new Point(5, 5)});
      var ticks = calc.CalculateActionsByTicks(f);
      var resolver = new FieldStateActionResolver(f);
      foreach (var actions in ticks)
      {
        foreach (var action in actions.Actions)
        {
          resolver.Resolve(action);
          Show(f);
          Thread.Sleep(700);

        }
        if (actions.Actions.Any() && actions.Actions.First().Position == new Point(2, 7))
        {
          break;
        }
      }

      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      ticks = calc.CalculateActionsByTicks(f);
      foreach (var actions in ticks)
      {
        foreach (var action in actions.Actions)
        {
          resolver.Resolve(action);
          Show(f);
          Thread.Sleep(700);
        }
      }
    }

  }
}
