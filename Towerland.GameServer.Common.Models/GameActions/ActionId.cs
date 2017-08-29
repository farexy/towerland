namespace GameServer.Common.Models.GameActions
{
  public enum ActionId
  {
    Reserved = 0,

    Tower = 10,
    TowerAttacks = 11,
    TowerAttacksPosition = 12,
    TowerRecharges = 21,
    TowerSearches = 22,

    Unit = 100,
    UnitMoves = 101,
    UnitMovesFreezed = 102,

    UnitDies = 201,
    UnitRecievesDamage = 202,
    UnitFreezes = 210,

    UnitAttacksCastle = 301,

    Other = 1000,
    
  }
}
