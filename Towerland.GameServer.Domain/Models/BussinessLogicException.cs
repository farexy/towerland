using System;

namespace Towerland.GameServer.Domain.Models
{
  public class BusinessLogicException : Exception
  {
    public BusinessLogicException(string msg) : base(msg)
    {
    }
  }
}