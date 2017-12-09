using Newtonsoft.Json;

namespace GameServer.Common.Models.GameField
{
  public struct FieldCell
  {
    [JsonProperty("Object")] public FieldObject Object;
    [JsonProperty("Position")] public Point Position;
  }
}
