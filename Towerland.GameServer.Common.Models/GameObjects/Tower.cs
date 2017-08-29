using Newtonsoft.Json;

namespace GameServer.Common.Models.GameObjects
{
  public class Tower : GameObject
  {
    public Tower()
    {
      Type = GameObjectType.Tower;
    }
    
  }
}
