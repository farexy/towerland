using System.Collections.Generic;
using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Exceptions;
using Towerland.GameServer.Models.GameActions;
using Towerland.GameServer.Models.GameField;
using Towerland.GameServer.Models.State;

namespace Towerland.GameServer.Logic
{
  public class CheatCommandManager : ICheatCommandManager
  {
    private readonly IStateChangeRecalculator _stateRecalculator;

    public CheatCommandManager(IStateChangeRecalculator stateRecalculator)
    {
      _stateRecalculator = stateRecalculator;
    }
    
    //Supported command signatures:
    //command 'addm' - increases money for players in battle
    //addm - gives 100 money to both players
    //addm <money> - gives specified number of <money> to both players
    //addm <money> <side> - gives specified number of <money> specified side players
    
    public List<GameAction> ResolveCommand(string command, Field field)
    {
      var args = command.Split(' ');
      switch (args[0])
      {
          case "addm":
            return ResolveAddMoneyCommand(args, field);
          default:
            throw new LogicException("Unsupported command");
      }
    }

    private List<GameAction> ResolveAddMoneyCommand(string[] args, Field field)
    {
      if (args.Length == 1)
      {
        return _stateRecalculator.AddMoney(field, 100, PlayerSide.Undefined);
      }

      if (args.Length == 2)
      {
        if (!int.TryParse(args[1], out var moneyVal))
        {
          throw new LogicException("Incorrect command signature");
        }
        return _stateRecalculator.AddMoney(field, moneyVal, PlayerSide.Undefined);
      }

      if (args.Length == 3)
      {
        if (!int.TryParse(args[1], out var moneyVal) || !int.TryParse(args[2], out var side))
        {
          throw new LogicException("Incorrect command signature");
        }
        return _stateRecalculator.AddMoney(field, moneyVal, (PlayerSide)side);
      }

      throw new LogicException("Incorrect command signature");
    }
  }
}