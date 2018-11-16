using System;

namespace Towerland.GameServer.BusinessLogic.Models
{
  public class BusinessLogicException : Exception
  {
    public BusinessLogicException(string msg) : base(msg)
    {
    }
  }
}