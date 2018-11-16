using System;

namespace Towerland.GameServer.Models.Exceptions
{
  public class LogicException : Exception
  {
    public LogicException(string msg) : base(msg)
    {
    }
  }
}