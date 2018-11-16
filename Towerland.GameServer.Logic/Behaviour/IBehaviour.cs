namespace Towerland.GameServer.Logic.Behaviour
{
  public interface IBehaviour
  {
    bool CanDoAction();
    bool ApplyPreActionEffect();
    void DoAction();
    void ApplyPostActionEffect();
    void TickEndAction();
  }
}