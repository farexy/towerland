using System;
using Towerland.GameServer.Data.Interfaces;

namespace Towerland.GameServer.Data.Entities
{
  public class LiveBattle : DataEntity, IGuidEntity
  {
    public Guid Id { set; get; }
    public string SerializedState { set; get; }
    public string SerializedActions { set; get; }
  }
}
