using System;
using Towerland.GameServer.Data.Entities;

namespace Towerland.GameServer.BusinessLogic.Helpers
{
  public static class BattleExtensions
  {
    public static bool IsWinner(this Battle b, Guid uid)
    {
      return b.WinnerId == uid;
    }
  }
}