using System;
using Towerland.GameServer.Data.Interfaces;

namespace Towerland.GameServer.Data.Entities
{
  public class Battle : DataEntity, IGuidEntity
  {
    public Guid Id { get; set; }
    public Guid Monsters_UserId { get; set; }
    public Guid Towers_UserId { get; set; }
    public Guid WinnerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public GameMode Mode { get; set; }
    public MultiBattleInfo MultiBattleInfo { get; set; }
  }
}