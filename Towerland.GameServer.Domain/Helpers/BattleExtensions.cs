using System;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Domain.Helpers
{
  public static class BattleExtensions
  {
    public static bool IsWinner(this Battle b, Guid uid)
    {
      return b.WinnerId == uid;
    }
  }
}