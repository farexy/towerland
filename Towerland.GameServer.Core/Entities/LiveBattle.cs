using System;
using Towerland.GameServer.Core.Interfaces;

namespace Towerland.GameServer.Core.Entities
{
  public class LiveBattle : DataEntity, IGuidEntity
  {
    public Guid Id { set; get; }
    public string SerializedState { set; get; }
    public string SerializedActions { set; get; }
  }
}
