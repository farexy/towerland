using Towerland.GameServer.Models.Effects;
using Towerland.GameServer.Models.GameObjects;

namespace Towerland.GameServer.Models.Stats
{
  public struct Skill
  {
    public SkillId Id;
    public GameObjectType GameObjectType;
    public int Duration;
    public int ProbabilityPercent;
    public EffectId EffectId;
    public double BuffValue;
    public double DebuffValue;
    public int WaitTicks;
    public int Range;
  }
}