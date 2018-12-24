namespace Towerland.GameServer.Models.GameActions
{
  public enum ActionId
  {
    Empty = 0,
    Reserved = 1,

    StateChanged = 2,
    
    Tower = 10,
    TowerAttacks = 11,
    TowerAttacksPosition = 12,
    TowerKills = 13,

    TowerRecharges = 21,
    TowerSearches = 22,

    TowerCollapses = 31,

    Unit = 100,
    UnitMoves = 101,

    UnitDisappears = 201, //dies or attacks castle
    UnitReceivesDamage = 202,
    UnitFreezes = 210,
    UnitPoisoned = 215,
    UnitEffectCanceled = 290,

    UnitAttacksCastle = 301,
    UnitDestroysTower = 310,

    UnitAppliesSkill = 502,

    UnitAppears = 801,
    UnitRevives = 810,

    Other = 1000,
    MonsterPlayerReceivesMoney = 1001,
    TowerPlayerReceivesMoney = 1002,
    PlayersReceivesMoney = 1003,
    MonsterPlayerLosesMoney = 1004,
    TowerPlayerLosesMoney = 1005,

    MonsterPlayerWins = 1010,
    TowerPlayerWins = 1020,
    
  }
}
