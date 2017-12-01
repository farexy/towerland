using System;
using Towerland.GameServer.Core.Interfaces;

namespace Towerland.GameServer.Core.Entities
{
  public class Battle : DataEntity, IGuidEntity
  {
    public Guid Id { get; set; }
    public Guid MonstersUserId { get; set; }
    public Guid TowersUserId { get; set; }
    public int Winner { get; set; }
    public DateTime StarTime { get; set; }
    public DateTime EndTime { get; set; }
  }
}