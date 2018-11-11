namespace Towerland.GameServer.Common.Models.GameActions
{
  public enum ActionId
  {
    Reserved = 0,

    StateChanged = 1,
    
    Tower = 10,
    TowerAttacks = 11,
    TowerAttacksPosition = 12,
    TowerKills = 13,

    TowerRecharges = 21,
    TowerSearches = 22,

    Unit = 100,
    UnitMoves = 101,

    UnitDies = 201,
    UnitRecievesDamage = 202,
    UnitFreezes = 210,

    UnitAttacksCastle = 301,

    UnitEffectCanseled = 401,
    
    UnitAppliesEffect_DarkMagic = 502,
    
    UnitAppears = 801,
    
    Other = 1000,
    MonsterPlayerRecievesMoney = 1001,
    TowerPlayerRecievesMoney = 1002,
    PlayersRecievesMoney = 1003,

    MonsterPlayerWins = 1010,
    TowerPlayerWins = 1020,
    
  }
}
