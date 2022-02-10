using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AareonTechnicalTest.Models;
using System;
using AareonTechnicalTest.JsonConfiguration;

namespace AareonTechnicalTest.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TicketController : AareonControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        private readonly ApplicationContext _dbContext;

        public TicketController(ILogger<TicketController> logger, ApplicationContext dbContext)
            : base(logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Ticket>), 200)]
        public async Task<IEnumerable<Ticket>> Get()
        {
            AuditControllerAction();
            return await _dbContext.Tickets.ToListAsync();
        }

        [HttpGet]
        [Route("{ticketId}")]
        [ProducesResponseType(typeof(Ticket), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Get(int ticketId)
        {
            AuditControllerAction(paramters: ticketId);

            if (ticketId <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            return new OkObjectResult(ticket);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Ticket), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 409)]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            AuditControllerAction(paramters: ticket);

            var ticketEntry = await _dbContext.Tickets.AddAsync(ticket);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                // Get inner most exception
                Exception ex = e;
                while (ex.InnerException != null) { ex = ex.InnerException; };

                _logger.LogWarning(e, "Creating ticket failed for person id {PersonId}. {ErrorMessage}", ticket.PersonId, ex.Message);
                return Conflict(ex.Message);
            }

            return new CreatedResult(new Uri($"/Ticket/{ticketEntry.Entity.Id}", UriKind.Relative), ticketEntry.Entity);
        }

        [HttpPut]
        [Route("{ticketId}")]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        [ProducesResponseType(typeof(string), 409)]
        public async Task<IActionResult> Update(int ticketId, Ticket ticket)
        {
            AuditControllerAction(paramters: new object[] { ticketId, ticket });

            if (ticketId <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            // Ensure the tickets Id is populated correctly (required for update below)
            if (ticketId != ticket.Id)
            {
                ticket.Id = ticketId;
            }

            var exists = await _dbContext.Tickets.AnyAsync(t => t.Id == ticketId);
            if (!exists)
            {
                return NotFound();
            }

            _dbContext.Update(ticket);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                // Get inner most exception
                Exception ex = e;
                while (ex.InnerException != null) { ex = ex.InnerException; };

                _logger.LogWarning(e, "Updating ticket failed for ticket id {TicketId} with person id {PersonId}. {ErrorMessage}", ticket.Id, ticket.PersonId, ex.Message);
                return Conflict(ex.Message);
            }

            return new NoContentResult();
        }


        [HttpDelete]
        [Route("{ticketId}")]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Delete(int ticketId)
        {
            AuditControllerAction(paramters: ticketId);
            
            if (ticketId <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            var ticket = await _dbContext.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound();
            }

            _dbContext.Tickets.Remove(ticket);
            await _dbContext.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}