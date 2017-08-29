using System;
using System.Linq;
using System.Threading;
using GameServer.Common.Models.GameActions;
using GameServer.Common.Models.GameField;
using GameServer.Common.Models.GameObjects;
using Towerland.GameServer.Common.Logic;
using Towerland.GameServer.Common.Logic.ActionResolver;
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
      for (int i = 0; i < f.Width; i++)
      {
        for (int j = 0; j < f.Height; j++)
        {
          var unit = f.Units.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          var tower = f.Towers.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          if(unit == null)
            Console.Write((int)f.Cells[i,j].Object);
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
      var f = FieldFactory.ClassicField;
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
        foreach (var action in actions)
        {
          resolver.Resolve(action);
          Show(f);
          Thread.Sleep(700);

        }
        if (actions.Any() && actions.First().Position == new Point(2, 7))
        {
          break;
        }
      }

      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      ticks = calc.CalculateActionsByTicks(f);
      foreach (var actions in ticks)
      {
        foreach (var action in actions)
        {
          resolver.Resolve(action);
          Show(f);
          Thread.Sleep(700);
        }
      }
    }

  }
}
