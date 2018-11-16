using System;

namespace Towerland.GameServer.BusinessLogic.Models
{
  public class UserRating
  {
    public int Position { get; set; }
    public Guid UserId { get; set; }
    public string Nickname { get; set; }
    public int Expirience { get; set; }
    public int VictoryPercent { get; set; }
    public int RatingPoints { get; set; }
  }
}