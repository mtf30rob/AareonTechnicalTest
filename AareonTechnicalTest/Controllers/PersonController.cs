using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AareonTechnicalTest.Models;
using System;

namespace AareonTechnicalTest.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PersonController : AareonControllerBase
    {
        private readonly ILogger<PersonController> _logger;
        private readonly ApplicationContext _dbContext;

        public PersonController(ILogger<PersonController> logger, ApplicationContext dbContext)
            : base(logger)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<Person>), 200)]
        public async Task<IEnumerable<Person>> Get()
        {
            return await _dbContext.Persons.ToListAsync();
        }

        [HttpGet]
        [Route("{personId}")]
        [ProducesResponseType(typeof(Person), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Get(int personId)
        {
            if (personId <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            var person = await _dbContext.Persons.FirstOrDefaultAsync(t => t.Id == personId);
            if (person == null)
            {
                return NotFound();
            }

            return new OkObjectResult(person);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Person), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Create(Person person)
        {
            var personEntry = await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();

            return new CreatedResult(new Uri($"/Person/{personEntry.Entity.Id}", UriKind.Relative), personEntry.Entity);
        }

        [HttpPut]
        [Route("{personId}")]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Update(int personId, Person person)
        {
            if (personId <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            // Ensure the Id is populated correctly (required for update below)
            if (personId != person.Id)
            {
                person.Id = personId;
            }

            var exists = await _dbContext.Persons.AnyAsync(t => t.Id == personId);
            if (!exists)
            {
                return NotFound();
            }

            _dbContext.Update(person);
            await _dbContext.SaveChangesAsync();

            return new NoContentResult();
        }


        [HttpDelete]
        [Route("{personId}")]
        [ProducesResponseType(typeof(NoContentResult), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(NotFoundResult), 404)]
        public async Task<IActionResult> Delete(int personId)
        {
            if (personId <= 0)
            {
                return BadRequest("Id must be greater than 0");
            }

            var person = await _dbContext.Persons.FindAsync(personId);
            if (person == null)
            {
                return NotFound();
            }

            _dbContext.Persons.Remove(person);
            await _dbContext.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}