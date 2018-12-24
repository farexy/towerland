using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Towerland.GameServer.BusinessLogic.BackgroundServices
{
  public class AiService : BackgroundService
  {
    private readonly Dictionary<Guid, int> _processedRevisions;

    public AiService()
    {
      _processedRevisions = new Dictionary<Guid, int>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
    }
  }
}