using System;
using Towerland.GameServer.Core.Entities;
using Towerland.GameServer.Domain.Helpers;

namespace Towerland.GameServer.Domain.Infrastructure
{
  public class UserExperienceCalculator
  {
    private const int UserLeftExp = 1;
    private const int UserLoosedExp = 3;
    private const int UserWonExp = 10;

    public int CalcUserExp(Battle b, Guid uid, Guid? left)
    {
      return b.IsWinner(uid)
        ? UserWonExp
        : left.HasValue && uid == left
          ? UserLeftExp
          : UserLoosedExp;
    }
  }
}