﻿namespace GameServer.Common.Models.GameObjects
{
  public enum GameObjectType
  {
    // ReSharper disable InconsistentNaming
    Undefined = 0,
    Reserved = 1,

    Castle = 10,
    Castle_Usual = 11,

    Tower = 100,
    Tower_Usual = 101,
    Tower_Frost = 102,
    Tower_Cannon = 103,
    Tower_FortressWatchtower = 104,
    Tower_Magic = 105,

    Unit = 1000,
    Unit_Skeleton = 1001,


    // ReSharper restore InconsistentNaming
  }
}
