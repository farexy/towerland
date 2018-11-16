namespace Towerland.GameServer.Models.Effects
{
  public enum AbilityId
  {
    None = 0,

    // Tower abilities
    Tower_FreezesUnit = 101,
    Tower_PoisonsUnit = 110,
    Tower_10xDamage_10PercentProbability = 201,

    //Unit abilities
    Unit_RevivesDeadUnit = 501,
    Unit_DestroysTowerOnDeath = 550,
  }
}