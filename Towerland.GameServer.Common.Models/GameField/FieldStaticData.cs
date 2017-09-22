using GameServer.Common.Models.GameObjects;
using Newtonsoft.Json;

namespace GameServer.Common.Models.GameField
{
  public class FieldStaticData
  {
    public FieldStaticData(FieldCell[,] cells, Castle castle)
    {
      Cells = cells;
      Castle = castle;
    }
    
    [JsonProperty("p")] public Path[] Path { private set; get; }
    [JsonProperty("m")] public FieldCell[,] Cells { private set; get; }
    [JsonProperty("c")] public Castle Castle { private set; get; }
    
    public int Width
    {
      get { return Cells.GetLength(1); }
    }

    public int Height
    {
      get { return Cells.GetLength(0); }
    }

  }
}