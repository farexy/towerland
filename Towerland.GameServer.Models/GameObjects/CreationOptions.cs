using Newtonsoft.Json;
using Towerland.GameServer.Models.GameField;

namespace Towerland.GameServer.Models.GameObjects
{
  public struct CreationOptions
  {
    [JsonProperty("g")] public int? GameId;
    [JsonProperty("p")] public Point Position;
    [JsonProperty("i")] public int? PathId;
  }
}
