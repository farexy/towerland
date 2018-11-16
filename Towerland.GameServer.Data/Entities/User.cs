using System;
using Towerland.GameServer.Data.Interfaces;

namespace Towerland.GameServer.Data.Entities
{
  public class User : DataEntity, IGuidEntity
  {
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Nickname { get; set; }
    public string Password { get; set; }
    public int Experience { get; set; }
  }
}