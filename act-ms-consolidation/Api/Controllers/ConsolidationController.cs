using act_ms_consolidation.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Retrieves the daily consolidation data for a specific date
        /// </summary>
        /// <param name="date">Date in the format yyyy-MM-dd</param>
        /// <returns>Daily consolidation data</returns>
        [HttpGet("{date}")]
        [SwaggerOperation(Summary = "Fetches daily consolidation data by date")]
        [SwaggerResponse(200, "Daily consolidation data retrieved successfully")]
        [SwaggerResponse(400, "Invalid date format")]
        [SwaggerResponse(404, "No data found for the provided date")]
        public async Task<IActionResult> GetDailyConsolidation([RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "Date must be in the format yyyy-MM-dd")] string date)
        {
            var result = await _consolidationService.GetDailyConsolidationAsync(date);
            return Ok(result);
        }
    }
}
