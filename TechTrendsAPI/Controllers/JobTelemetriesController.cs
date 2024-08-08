using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechTrendsAPI.Models;

namespace TechTrendsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    //[Authorize(Roles = UserRoles.Admin)]
    public class JobTelemetriesController : ControllerBase
    {
        private readonly RemoteContext _context;

        public JobTelemetriesController(RemoteContext context)
        {
            _context = context;
        }

        // GET: api/JobTelemetries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobTelemetry>>> GetJobTelemetries()
        {
            return await _context.JobTelemetries.ToListAsync();
        }

        // GET: api/JobTelemetries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<JobTelemetry>> GetJobTelemetry(int id)
        {
            var jobTelemetry = await _context.JobTelemetries.FindAsync(id);

            if (jobTelemetry == null)
            {
                return NotFound();
            }

            return jobTelemetry;
        }

        // PUT: api/JobTelemetries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PutJobTelemetry(int id, JobTelemetry jobTelemetry)
        {
            if (id != jobTelemetry.Id)
            {
                return BadRequest();
            }

            _context.Entry(jobTelemetry).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobTelemetryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/JobTelemetries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<JobTelemetry>> PostJobTelemetry(JobTelemetry jobTelemetry)
        {
            _context.JobTelemetries.Add(jobTelemetry);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetJobTelemetry", new { id = jobTelemetry.Id }, jobTelemetry);
        }

        // DELETE: api/JobTelemetries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJobTelemetry(int id)
        {
            var jobTelemetry = await _context.JobTelemetries.FindAsync(id);
            if (jobTelemetry == null)
            {
                return NotFound();
            }

            _context.JobTelemetries.Remove(jobTelemetry);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool JobTelemetryExists(int id)
        {
            return _context.JobTelemetries.Any(e => e.Id == id);
        }

        [HttpGet("savings/project/{projectId}")]
        public IActionResult GetSavingsByProjectId(Guid projectId, DateTime startDate, DateTime endDate)
        {
            var telemetries = _context.JobTelemetries
                .Join(
                    _context.Processes,
                    jobTelemetry => jobTelemetry.ProccesId,
                    process => process.ProcessId,
                    (jobTelemetry, process) => new { jobTelemetry, process }
                )
                .Join(
                    _context.Projects,
                    jtp => jtp.process.ProjectId,
                    project => project.ProjectId,
                    (jtp, project) => new { jtp.jobTelemetry, jtp.process, project }
                )
                .Where(jtp => jtp.project.ProjectId == projectId
                            && jtp.jobTelemetry.EntryDate >= startDate
                            && jtp.jobTelemetry.EntryDate <= endDate)
                .ToList();

            // Check if ExcludeFromTimeSaving is set for any telemetry
            var filteredTelemetries = telemetries
                .Where(jtp => !jtp.jobTelemetry.ExcludeFromTimeSaving.HasValue || !jtp.jobTelemetry.ExcludeFromTimeSaving.Value);

            var totalTimeSaved = filteredTelemetries.Sum(jtp => jtp.jobTelemetry.HumanTime ?? 0); // Handle potential null HumanTime
            var totalCostSaved = 0; // Assuming no CostSaved property

            var savings = new
            {
                ProjectId = projectId,
                TotalTimeSaved = totalTimeSaved,
                TotalCostSaved = totalCostSaved
            };

            return Ok(savings);
        }



        [HttpGet("savings/client/{clientId}")]
        public IActionResult GetSavingsByClientId(Guid clientId, DateTime startDate, DateTime endDate)
        {
            var telemetries = _context.JobTelemetries
                .Join(
                    _context.Processes,
                    jobTelemetry => jobTelemetry.ProccesId,
                    process => process.ProcessId,
                    (jobTelemetry, process) => new { jobTelemetry, process }
                )
                .Join(
                    _context.Projects,
                    jtp => jtp.process.ProjectId,
                    project => project.ProjectId,
                    (jtp, project) => new { jtp.jobTelemetry, jtp.process, project }
                )
                .Where(jtp => jtp.project.ClientId == clientId && jtp.jobTelemetry.EntryDate >= startDate && jtp.jobTelemetry.EntryDate <= endDate)
                .ToList();

            // Check if ExcludeFromTimeSaving is set for any telemetry
            var filteredTelemetries = telemetries.Where(t => !t.jobTelemetry.ExcludeFromTimeSaving.HasValue || !t.jobTelemetry.ExcludeFromTimeSaving.Value);

            var totalTimeSaved = filteredTelemetries.Sum(t => t.jobTelemetry.HumanTime ?? 0); // Handle potential null HumanTime
            var totalCostSaved = 0; // Assuming no CostSaved property

            var savings = new
            {
                ClientId = clientId,
                TotalTimeSaved = totalTimeSaved,
                TotalCostSaved = totalCostSaved
            };

            return Ok(savings);
        }

    }
}
