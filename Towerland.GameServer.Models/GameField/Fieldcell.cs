using Newtonsoft.Json;

namespace Towerland.GameServer.Models.GameField
{
  public struct FieldCell
  {
    [JsonProperty("o")] public FieldObject Object;
    [JsonProperty("p")] public Point Position;
  }
}
