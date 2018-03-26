namespace Towerland.GameServer.Common.Models.GameObjects
{
  public enum GameObjectType
  {
    // ReSharper disable InconsistentNaming
    Undefined = 0,
    Reserved = 1,

    Castle = 10,
    Castle_Usual = 11,

    Whizzbang = 20,
    Whizzbang_Usual = 21,
    Whizzbang_Frost = 22,
    Whizzbang_Magic = 23,

    Tower = 100,
    Tower_Usual = 101,
    Tower_Frost = 102,
    Tower_Cannon = 103,
    Tower_FortressWatchtower = 104,
    Tower_Magic = 105,

    Unit = 1000,
    Unit_Skeleton = 1001,
    Unit_Impling = 1002,
    Unit_Orc= 1003,
    Unit_Goblin = 1004,
    Unit_Dragon = 1005,
    Unit_Golem = 1006,
    Unit_Necromancer = 1007,

    // ReSharper restore InconsistentNaming
  }
}
