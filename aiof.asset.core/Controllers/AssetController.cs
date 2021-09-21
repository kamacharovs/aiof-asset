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
    [Route(Constants.ApiRoute)]
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
        /// Get Assets
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(IEnumerable<IAsset>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAsync(
            [FromQuery] DateTime? snapshotsStartDate,
            [FromQuery] DateTime? snapshotsEndDate,
            [FromQuery] string type)
        {
            return Ok(await _repo.GetAsync(snapshotsStartDate, snapshotsEndDate, type));
        }

        /// <summary>
        /// Get Asset types
        /// </summary>
        [HttpGet]
        [Route("types")]
        [ProducesResponseType(typeof(IEnumerable<IAssetType>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTypesAsync()
        {
            return Ok(await _repo.GetTypesAsync());
        }

        /// <summary>
        /// Add Asset
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAsset), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAsync([FromBody, Required] AssetDto dto)
        {
            return Created(nameof(Asset), await _repo.AddAsync(dto));
        }

        /// <summary>
        /// Add Assets
        /// </summary>
        [HttpPost]
        [Route("multiple")]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IEnumerable<IAsset>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAsync([FromBody, Required] IEnumerable<AssetDto> dtos)
        {
            return Created(nameof(Asset), await _repo.AddAsync(dtos));
        }

        /// <summary>
        /// Add Asset snapshot
        /// </summary>
        [HttpPost]
        [Route("snapshot")]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAssetSnapshot), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddSnapshotAsync([FromBody, Required] AssetSnapshotDto dto)
        {
            return Created(nameof(AssetSnapshot), await _repo.AddSnapshotAsync(dto));
        }

        /// <summary>
        /// Update Asset
        /// </summary>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(IAssetSnapshot), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute, Required] int id,
            [FromBody, Required] AssetDto dto)
        {
            return Ok(await _repo.UpdateAsync(id, dto));
        }

        /// <summary>
        /// Delete Asset
        /// </summary>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(typeof(IAssetProblemDetail), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAsync([FromRoute, Required] int id)
        {
            await _repo.DeleteAsync(id);

            return Ok();
        }
    }
}
