using Newtonsoft.Json;

namespace GameServer.Common.Models.GameField
{
  public struct FieldCell
  {
    [JsonProperty("o")] public FieldObject Object;
    [JsonProperty("p")] public Point Position;
  }
}
