using act_ms_consolidation.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace act_ms_consolidation.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConsolidationController : Controller
    {
        private readonly IConsolidationService _consolidationService;

        public ConsolidationController(IConsolidationService consolidationService)
        {
            _consolidationService = consolidationService;
        }
        [HttpGet("{date}")]
        public async Task<IActionResult> GetDailyConsolidation(string date)
        {
            if (!DateTime.TryParse(date, out _))
            {
                return BadRequest("Invalid date format. Use yyyy-MM-dd.");
            }

            var result = await _consolidationService.GetDailyConsolidationAsync(date);
            return Ok(result);
        }
    }
}
