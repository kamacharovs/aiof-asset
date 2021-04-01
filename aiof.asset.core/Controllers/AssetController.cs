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

namespace aiof.asset.core
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

        /// <summary>
        /// Get Asset by id
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IAsset), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(
            [FromRoute, Required] int id,
            [FromQuery] DateTime? snapshotsStartDate,
            [FromQuery] DateTime? snapshotsEndDate)
        {
            return Ok(await _repo.GetAsync(id, snapshotsStartDate, snapshotsEndDate));
        }

        /// <summary>
        /// Get Asset types
        /// </summary>
        [HttpGet]
        [Route("types")]
        [ProducesResponseType(typeof(IEnumerable<IAssetType>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssetTypesAsync()
        {
            return Ok(await _repo.GetTypesAsync());
        }
    }
}
