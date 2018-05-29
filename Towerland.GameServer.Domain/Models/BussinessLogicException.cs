using System;

namespace Towerland.GameServer.Domain.Models
{
  public class BussinessLogicException : Exception
  {
    public BussinessLogicException(string msg) : base(msg)
    {
    }
  }
}