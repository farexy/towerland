using Towerland.GameServer.Models.Stats;

namespace Towerland.GameServer.Models
{
    public class StatsResponseModel
    {
        public UnitStats[] UnitStats { get; set; }
        public TowerStats[] TowerStats { get; set; }
        public DefenceCoeff[] DefenceCoeffs { get; set; }
    }
}