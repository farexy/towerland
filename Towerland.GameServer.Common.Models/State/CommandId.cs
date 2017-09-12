using System;

namespace GameServer.Common.Models.State
{
  [Flags]
  public enum CommandId
  {
    AddUnit, AddTower
  }
}
