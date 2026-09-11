using CatFact.API.Clients;
using CatFact.API.Constants;
using CatFact.API.DTOs;
using CatFact.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CatFact.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FactResponseController(IFactResponseService factResponseService) : ControllerBase
    {

        [HttpPost]
        [EnableRateLimiting(RateLimitPolicies.PostFactPolicy)]
        public async Task<ActionResult<FactResponse>> SaveFactResponse(CancellationToken cancellationToken)
        {
            var factResponse = await factResponseService.SaveToFileFactResponseAsync(cancellationToken);
            return Ok(factResponse);
        }
    }
}

