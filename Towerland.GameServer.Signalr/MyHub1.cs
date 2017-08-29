using Microsoft.AspNet.SignalR;

namespace Towerland.GameServer.Signalr
{
  public class MyHub1 : Hub
  {
    public void Hello()
    {
      Clients.All.hello();
    }
  }
}