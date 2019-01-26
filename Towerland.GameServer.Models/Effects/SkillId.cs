namespace Towerland.GameServer.Models.Effects
{
  public enum SkillId
  {
    None = 0,

    // Tower abilities
    FreezesUnit = 101,
    PoisonsUnit = 110,
    ExtraDamageUnit = 201,
    ShurikenAttack = 251,
    BlocksUnitSkills = 301,
    BlocksUnitSkillsInRange = 302,

    //Unit abilities
    RevivesDeadUnit = 501,
    StealsTowerMoney = 510,
    DestroysTowerOnDeath = 550,
    BlocksTowerSkills = 601,
    BlocksTowerSkillsInRange = 602,
    AirUnit = 701,
  }
}