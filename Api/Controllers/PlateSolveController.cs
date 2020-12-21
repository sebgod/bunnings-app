using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Api.Models;
using BL;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlateSolveController : ControllerBase
    {
        private readonly IPlateSolverService _plateSolverService;
        private readonly ILogger<PlateSolveController> _logger;

        public PlateSolveController(ILogger<PlateSolveController> logger, IPlateSolverService plateSolverService)
        {
            _logger = logger;
            _plateSolverService = plateSolverService;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetAsync([FromRoute] int id)
        {
            var (success, jobs) = await _plateSolverService.GetJobsForSubmissionAsync(id);

            if (!success)
            {
                return BadRequest(new { error = $"Id {id} is not a valid submission"});
            }
            
            if (jobs?.Any() is not true)
            {
                return NotFound(new { error = $"No jobs found for submission {id}"});
            }

            return Ok(new PlateSolveJobsForSubmissionModel(id, jobs));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] PlateSolveModel model)
        {
            if (model is null)
            {
                return BadRequest(new { error = "body cannot be empty" });
            }

            if (model.ImageUri?.IsAbsoluteUri is not true)
            {
                return BadRequest(new { error = "Must provide an absolute image uri" });
            }

            var submission = await _plateSolverService.BlindSolveImageUriAsync(model.ImageUri, model.Session);

            return AcceptedAtAction("Get", new { id = submission.Id }, new PlateSolveSubmissionModel(model.ImageUri, submission.Session, submission.Id));
        }
    }
}
