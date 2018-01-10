using System;
using Towerland.GameServer.Common.Models.State;
using Towerland.GameServer.Core.Entities;

namespace Towerland.GameServer.Domain.Helpers
{
  public static class BattleExtensions
  {
    public static bool IsWinner(this Battle b, Guid uid)
    {
      return b.Monsters_UserId == uid && b.Winner == (int) PlayerSide.Monsters ||
             b.Towers_UserId == uid && b.Winner == (int) PlayerSide.Towers;
    }
  }
}