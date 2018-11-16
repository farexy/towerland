using System;
using Towerland.GameServer.BusinessLogic.Helpers;
using Towerland.GameServer.Data.Entities;

namespace Towerland.GameServer.BusinessLogic.Infrastructure
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