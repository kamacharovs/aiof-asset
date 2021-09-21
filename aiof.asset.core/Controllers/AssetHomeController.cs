using System;
using System.Collections.Generic;
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
    [ApiVersion(Constants.ApiV1)]
    [Route(Constants.ApiHomeRoute)]
    [Produces(Constants.ApplicationJson)]
    [Consumes(Constants.ApplicationJson)]
    [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IAssetProblemDetailBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(IAssetProblemDetailBase), StatusCodes.Status401Unauthorized)]
    public class AssetHomeController : ControllerBase
    {
        private readonly IAssetHomeRepository _repo;

        public AssetHomeController(IAssetHomeRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Add Asset.Home
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAsset), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAsync([FromBody, Required] AssetHomeDto dto)
        {
            return Created(nameof(AssetHome), await _repo.AddAsync(dto));
        }
       
        /// <summary>
        /// Update Asset.Home
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAssetSnapshot), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute, Required] int id,
            [FromBody, Required] AssetHomeDto dto)
        {
            return Ok(await _repo.UpdateAsync(id, dto));
        }
    }
}
