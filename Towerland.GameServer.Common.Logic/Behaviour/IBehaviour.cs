namespace Towerland.GameServer.Common.Logic.Behaviour
{
  public interface IBehaviour
  {
    bool CanDoAction();
    bool ApplyPreActionEffect();
    void DoAction();
    void ApplyPostActionEffect();
  }
}