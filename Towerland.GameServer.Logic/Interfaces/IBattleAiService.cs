using System.Threading;
using System.Threading.Tasks;

namespace Towerland.GameServer.Logic.Interfaces
{
    public interface IBattleAiService
    {
        Task RunSessionAsync(CancellationToken stoppingToken);
    }
}