using Newtonsoft.Json;

namespace Towerland.GameServer.Common.Models.GameField
{
  public struct FieldCell
  {
    [JsonProperty("o")] public FieldObject Object;
    [JsonProperty("p")] public Point Position;
  }
}
