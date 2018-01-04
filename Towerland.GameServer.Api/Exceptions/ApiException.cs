using System;

namespace Towerland.GameServer.Api.Exceptions
{
  public class ApiException : ArgumentException
  {
    public ApiException(string msg):base(msg){}
  }
}