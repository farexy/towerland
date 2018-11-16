using Towerland.GameServer.Logic.Interfaces;
using Towerland.GameServer.Models.Exceptions;
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
    
    public void ResolveCommand(string command, Field field)
    {
      var args = command.Split(' ');
      switch (args[0])
      {
          case "addm":
            ResolveAddMoneyCommand(args, field);
            break;
          default:
            throw new LogicException("Unsupported command");
      }
    }

    private void ResolveAddMoneyCommand(string[] args, Field field)
    {
      if (args.Length == 1)
      {
        _stateRecalculator.AddMoney(field, 100, PlayerSide.Undefined);
      }

      if (args.Length == 2)
      {
        if (!int.TryParse(args[1], out var moneyVal))
        {
          throw new LogicException("Incorrect command signature");
        }
        _stateRecalculator.AddMoney(field, moneyVal, PlayerSide.Undefined);
      }

      if (args.Length == 3)
      {
        if (!int.TryParse(args[1], out var moneyVal) || !int.TryParse(args[2], out var side))
        {
          throw new LogicException("Incorrect command signature");
        }
        _stateRecalculator.AddMoney(field, moneyVal, (PlayerSide)side);
      }

      if (args.Length > 3)
      {
        throw new LogicException("Incorrect command signature");
      }
    }
  }
}