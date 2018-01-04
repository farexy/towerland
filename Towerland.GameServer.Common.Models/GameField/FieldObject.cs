using System;

namespace Towerland.GameServer.Common.Models.GameField
{
  [Flags]
  public enum FieldObject
  {
    Road = 0,
    Ground = 1,
    Entrance = 2,
    Castle = 3,

    Tree = 10
  }
}
