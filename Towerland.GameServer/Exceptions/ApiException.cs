using System;

namespace Towerland.GameServer.Exceptions
{
  public class ApiException : ArgumentException
  {
    public ApiException(string msg):base(msg){}
  }
}