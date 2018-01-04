using System;

namespace Towerland.GameServer.Common.Models.Exceptions
{
  public class LogicException : Exception
  {
    public LogicException(string msg) : base(msg)
    {
    }
  }
}