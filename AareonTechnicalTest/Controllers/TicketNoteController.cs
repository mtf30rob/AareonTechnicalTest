using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AareonTechnicalTest.Models;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace AareonTechnicalTest.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("Ticket")]
    public class TicketNoteController : AareonControllerBase
    {
        private readonly ILogger<TicketNoteController> _logger;
        private readonly ApplicationContext _dbContext;

        public TicketNoteController(ILogger<TicketNoteController> logger, ApplicationContext dbContext)
            : base(logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("{ticketId}/Notes")]
        [ProducesResponseType(typeof(List<TicketNote>), 200)]
        public async Task<IEnumerable<TicketNote>> Get(int ticketId)
        {
            return await _dbContext.TicketNotes.Where(n => n.TicketId == ticketId && !n.IsRemoved).ToListAsync();
        }

        [HttpGet]
        [Route("{ticketId}/Notes/{noteId}")]
        [ProducesResponseType(typeof(TicketNote), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Get(int ticketId, int noteId)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Ticket Id must be greater than 0");
            }

            if (noteId <= 0)
            {
                return BadRequest("Note Id must be greater than 0");
            }

            var note = await _dbContext.TicketNotes.Where(n => n.TicketId == ticketId && n.Id == noteId && !n.IsRemoved).FirstOrDefaultAsync();
            if (note == null)
            {
                return NotFound();
            }

            return new OkObjectResult(note);
        }

        [HttpPost]
        [Route("{ticketId}/Notes")]
        [ProducesResponseType(typeof(TicketNote), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 409)]
        public async Task<IActionResult> Create(int ticketId, TicketNote note)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Ticket Id must be greater than 0");
            }

            // Ensure the note has the correct ticket id
            note.TicketId = ticketId;

            var entry = await _dbContext.TicketNotes.AddAsync(note);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                // Get inner most exception
                Exception ex = e;
                while (ex.InnerException != null) { ex = ex.InnerException; };

                _logger.LogWarning(e, "Creating ticket note failed for ticket id {} and person id {PersonId}. {ErrorMessage}", note.TicketId, note.PersonId, ex.Message);
                return Conflict(ex.Message);
            }

            return new CreatedResult(new Uri($"/Ticket/{ticketId}/Notes/{entry.Entity.Id}", UriKind.Relative), entry.Entity);
        }

        [HttpPut]
        [Route("{ticketId}/Notes/{noteId}")]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        [ProducesResponseType(typeof(string), 409)]
        public async Task<IActionResult> Update(int ticketId, int noteId, TicketNote note)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Ticket Id must be greater than 0");
            }

            if (noteId <= 0)
            {
                return BadRequest("Note Id must be greater than 0");
            }

            // Ensure the note has the correct ticket id
            note.TicketId = ticketId;

            // Ensure the tickets Id is populated correctly (required for update below)
            if (noteId != note.Id)
            {
                note.Id = noteId;
            }

            var exists = await _dbContext.TicketNotes.Where(n => n.TicketId == ticketId && n.Id == noteId && !n.IsRemoved).AnyAsync();
            if (!exists)
            {
                return NotFound();
            }

            _dbContext.Update(note);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                // Get inner most exception
                Exception ex = e;
                while (ex.InnerException != null) { ex = ex.InnerException; };

                _logger.LogWarning(e, "Updating ticket note failed for note id {NoteId} with ticket id and person id {PersonId}. {ErrorMessage}", note.Id, note.TicketId, note.PersonId, ex.Message);
                return Conflict(ex.Message);
            }

            return new NoContentResult();
        }

        /// <summary>
        /// Soft delete of note
        /// </summary>
        [HttpPatch]
        [Route("{ticketId}/Notes/{noteId}")]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Remove(int ticketId, int noteId)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Ticket Id must be greater than 0");
            }

            if (noteId <= 0)
            {
                return BadRequest("Note Id must be greater than 0");
            }

            var note = await _dbContext.TicketNotes.FirstOrDefaultAsync(n => n.TicketId == ticketId && n.Id == noteId && !n.IsRemoved);
            if (note == null)
            {
                return NotFound();
            }

            note.IsRemoved = true;
            await _dbContext.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// Very crude implementation of user authentication. Normally this would be done through an access token from and identity provier using the Authorize attribute (commented out below).
        /// </summary>
        //[Authorize("Administrator")]
        [HttpDelete]
        [Route("{ticketId}/Notes/{noteId}")]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Delete(int ticketId, int noteId, int personId)
        {
            if (ticketId <= 0)
            {
                return BadRequest("Ticket id must be greater than 0");
            }

            if (noteId <= 0)
            {
                return BadRequest("Note id must be greater than 0");
            }

            if (personId <= 0)
            {
                return BadRequest("Person id must be greater than 0");
            }

            // Very crude implementation of user authentication. 
            var isAdmin = await _dbContext.Persons.Where(p => p.Id == personId).Select(p => p.IsAdmin).SingleOrDefaultAsync();
            if (!isAdmin)
            {
                return Unauthorized($"Unable to delete note. Person id {personId} is not an admin.");
            }

            var note = await _dbContext.TicketNotes.FirstOrDefaultAsync(n => n.TicketId == ticketId && n.Id == noteId);
            if (note == null)
            {
                return NotFound();
            }

            _dbContext.TicketNotes.Remove(note);
            await _dbContext.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}