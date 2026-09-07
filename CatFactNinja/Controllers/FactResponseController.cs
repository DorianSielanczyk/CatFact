using CatFact.API.Client;
using CatFact.API.DTOs;
using CatFact.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatFact.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FactResponseController(IFactResponseService factResponseService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<FactResponse>> GetFactResponse(CancellationToken cancellationToken)
        {
            var factResponse = await factResponseService.SaveToFileFactResponseAsync(cancellationToken);
            return Ok(factResponse);
        }
    }
}