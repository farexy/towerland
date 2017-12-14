using System;
using Towerland.GameServer.Core.Interfaces;

namespace Towerland.GameServer.Core.Entities
{
  public class Battle : DataEntity, IGuidEntity
  {
    public Guid Id { get; set; }
    public Guid Monsters_UserId { get; set; }
    public Guid Towers_UserId { get; set; }
    public int Winner { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
  }
}