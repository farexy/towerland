using GameServer.Common.Models.Effects;

namespace GameServer.Common.Models.GameObjects
{
  public class Tower : GameObject
  {
    public Tower() : base()
    {
      Type = GameObjectType.Tower;
    }

    public override object Clone()
    {
      return new Tower
      {
        GameId = GameId,
        Position = Position,
        Type = Type,
        WaitTicks = WaitTicks,
        Effect = new SpecialEffect{Effect = Effect.Effect, Duration = Effect.Duration}
      };
    }
  }
}
