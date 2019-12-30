namespace Towerland.GameServer.Models.GameObjects
{
  public enum GameObjectType
  {
    // ReSharper disable InconsistentNaming
    Undefined = 0,
    Reserved = 1,

    Castle = 10,
    
    Whizzbang = 20,
    Whizzbang_Usual = 21,
    Whizzbang_Frost = 22,
    Whizzbang_Magic = 23,
    Whizzbang_Poison = 24,
    Whizzbang_Bomb = 25,
    Whizzbang_Shuriken = 26,
    
    Explosion = 30,

    Tower = 100,
    Tower_Usual = 101,
    Tower_Frost = 102,
    Tower_Cannon = 103,
    Tower_FortressWatchtower = 104,
    Tower_Magic = 105,
    Tower_Poisoning = 106,
    Tower_Orc = 107,

    Unit = 1000,
    Unit_Skeleton = 1001,
    Unit_Impling = 1002,
    Unit_Orc= 1003,
    Unit_Goblin = 1004,
    Unit_Dragon = 1005,
    Unit_Demon = 1006,
    Unit_Necromancer = 1007,
    Unit_BarbarianMage = 1008,
    Unit_Golem = 1009,

    // ReSharper restore InconsistentNaming
  }
}
