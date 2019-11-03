using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Towerland.Static.Api.Controllers
{
    [Route("")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IOptionsMonitor<ValuesConfig> _options;

        public ValuesController(IOptionsMonitor<ValuesConfig> options)
        {
            _options = options;
        }

        [HttpGet("gsurl")]
        public ActionResult<string> Get() => _options.CurrentValue.GsUrl;
    }
}