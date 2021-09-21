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
    [Route(Constants.ApiStockRoute)]
    [Produces(Constants.ApplicationJson)]
    [Consumes(Constants.ApplicationJson)]
    [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(IAssetProblemDetailBase), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(IAssetProblemDetailBase), StatusCodes.Status401Unauthorized)]
    public class AssetStockController : ControllerBase
    {
        private readonly IAssetStockRepository _repo;

        public AssetStockController(IAssetStockRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Add Asset.Stock
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAsset), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAsync([FromBody, Required] AssetStockDto dto)
        {
            return Created(nameof(AssetStock), await _repo.AddAsync(dto));
        }

        /// <summary>
        /// Update Asset.Stock
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAssetSnapshot), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute, Required] int id,
            [FromBody, Required] AssetStockDto dto)
        {
            return Ok(await _repo.UpdateAsync(id, dto));
        }
    }
}
