using System;

namespace GameServer.Api.Exceptions
{
  public class ApiException : ArgumentException
  {
    public ApiException(string msg):base(msg){}
  }
}