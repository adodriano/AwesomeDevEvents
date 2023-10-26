using AwesomeDevEvents.API.Entities;
using AwesomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwesomeDevEvents.API.Controllers
{
    [Route("api/Dev-Events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventDbContext _context;

        public DevEventsController(DevEventDbContext context)
        {
            _context = context;
        }

        // api/dev-events/ GET
        [HttpGet]
        public IActionResult GetAll()
        {
            var DevEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();

            return Ok(DevEvents);
        }

        // api/dev-events/2312421 GET
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents.Include(de => de.Speakers).SingleOrDefault(d => d.Id == id);
            if (devEvent == null)
            {
                return NotFound();
            }
            return Ok(devEvent);
        }

        //api/dev-events/ POST
        [HttpPost]
        public IActionResult Post(DevEvent devEvent)
        {
            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);

        }

        //api/dev-events/12312421 PUT
        [HttpPut("{id}")]
        public IActionResult Update(Guid id, DevEvent input)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);
            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return NoContent();
        }

        //api/dev-events/12312421 DELETE
        [HttpDelete("{id}")]

        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id == id);
            if (devEvent == null)
            {
                return NotFound();
            }

            devEvent.Delete();

            _context.SaveChanges();

            return NoContent();
        }


        //api/dev-events/12312421 PUT
        [HttpPut("{id}/speakers")]
        public IActionResult PostSpearks(Guid id, DevEventSpeaker spearker)
        {   
            spearker.DevEventId = id;

            var devEvent = _context.DevEvents.Any(d => d.Id == id);

            if (!devEvent)
            {
                return NotFound();
            }

            _context.DevEventsSpeakers.Add(spearker);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
