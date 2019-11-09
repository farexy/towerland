namespace Towerland.GameServer.Logic.Behaviour
{
  public interface IBehaviour
  {
    void ApplyAura();
    bool CanDoAction();
    bool ApplyPreActionEffect();
    void DoAction();
    void ApplyPostActionEffect();
    void TickEndAction();
  }
}