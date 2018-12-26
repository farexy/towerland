using Towerland.GameServer.Models.Effects;

namespace Towerland.GameServer.Models.GameObjects
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
        Effect = (SpecialEffect)Effect.Clone(),
      };
    }
  }
}
