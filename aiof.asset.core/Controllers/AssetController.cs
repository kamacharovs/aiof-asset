using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

using aiof.asset.data;
using aiof.asset.services;

namespace aiof.asset.core.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    [Produces(Constants.ApplicationJson)]
    [Consumes(Constants.ApplicationJson)]
    [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IAssetProblemDetailBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(IAssetProblemDetailBase), StatusCodes.Status401Unauthorized)]
    public class AssetController : ControllerBase
    {
        private readonly IAssetRepository _repo;

        public AssetController(IAssetRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAsync([FromRoute, Required] int id)
        {
            return Ok(await _repo.GetAsync(id));
        }
    }
}
