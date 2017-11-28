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
          var pos = f.State.Units.FirstOrDefault()?.Position;
          var pos21 = f.State.Towers.FirstOrDefault()?.Position;
          var unit = f.State.Units.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          var tower = f.State.Towers.FirstOrDefault(u => u.Position.X == i && u.Position.Y == j);
          if(unit == null)
            Console.Write((int)f.StaticData.Cells[i,j].Object);
          else if (tower != null)
            Console.Write("T");
          else
            Console.Write("*");
        }
        Console.WriteLine();
      }

      Console.WriteLine("monst : " + f.State.MonsterMoney);
      Console.WriteLine("tower : " + f.State.TowerMoney);
    }

    static void Main(string[] args)
    {
      var f = new FieldFactoryStub().ClassicField;

//      var pf = new PathFinder();
//      foreach (var p in pf.ResolvePath(f.StaticData.Cells, f.StaticData.Start, f.StaticData.Finish))
//      {
//        Console.WriteLine(p);
//      }

      var statsStub = new StatsLibrary();
      var uFactory = new UnitFactory(statsStub);
      var tFactory = new TowerFactory(statsStub);
      var pathOpt = new PathOptimisation(statsStub);
      var stateChang = new StateChangeRecalculator(pathOpt, statsStub, uFactory, tFactory);

      stateChang.AddNewUnit(f, GameObjectType.Unit_Skeleton);
      stateChang.AddNewTower(f, GameObjectType.Tower_Usual, new CreationOptions{Position = new Point(5, 5)});
      
      var calc = new StateCalculator(statsStub, f);

      var ticks = calc.CalculateActionsByTicks();
      var resolver = new FieldStateActionResolver(f);
      foreach (var actions in ticks)
      {
        foreach (var action in actions.Actions)
        {
          resolver.Resolve(action);
          Show(calc.Field);
          Thread.Sleep(700);

        }
        if (actions.Actions.Any() && actions.Actions.First().Position == new Point(2, 7))
        {
          break;
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
          Show(calc.Field);
          Thread.Sleep(700);
        }
      }
    }

  }
}
