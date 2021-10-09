using API.Manager.Core;
using API.Manager.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace API.Manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiManagerController : ControllerBase
    {
        private readonly IManagerService _managerService;

        public ApiManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpGet("channels")]
        [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetChannels(CancellationToken cancellationToken = default)
        {
            var result = await _managerService.GetChannelsAsync(cancellationToken);

            if (result is null && !result.Any())
                return NotFound();

            return Ok(result);
        }

        [HttpGet("projects")]
        [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProjects(CancellationToken cancellationToken = default)
        {
            var result = await _managerService.GetProjectsAsync(cancellationToken);

            if (result is null && !result.Any())
                return NotFound();

            return Ok(result);
        }

        [HttpGet("projects/controllers/{projectName}")]
        [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetControllers(string projectName, CancellationToken cancellationToken = default)
        {
            var result = await _managerService.GetControllersByProjectNameAsync(projectName, cancellationToken);

            if (result is null && !result.Any())
                return NotFound();

            return Ok(result);
        }

        [HttpGet("projects/controllers/methods/{controllerName}")]
        [ProducesResponseType(typeof(IList<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMethods(string controllerName, CancellationToken cancellationToken = default)
        {
            var result = await _managerService.GetMethodsByControllerNameAsync(controllerName, cancellationToken);

            if (result is null && !result.Any())
                return NotFound();

            return Ok(result);
        }

        [HttpPut("projects/controllers/methods/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put(int id, [FromQuery] bool isServiceable, CancellationToken cancellationToken = default)
        {
            await _managerService.UpdateServiceStatusByIdAsync(id, isServiceable, cancellationToken);
            return NoContent();
        }
    }
}
