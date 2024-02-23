using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NST.TestTask.Models;

namespace NST.TestTask.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PersonsController : ControllerBase
    {

        private readonly ILogger<PersonsController> _logger;
        private AppDbContext db;

        public PersonsController(ILogger<PersonsController> logger, AppDbContext ctx)
        {
            _logger = logger;
            db = ctx;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetAll()
        {
            return await db.Persons.Include(p => p.Skills).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<Results<Ok<Person>, NotFound>> GetById(long id)
        {
            var person = await db.Persons.Include(p => p.Skills).FirstOrDefaultAsync(p => p.Id == id);
            if (person == null) return TypedResults.NotFound();
            return TypedResults.Ok(person);
        }

        [HttpPost]
        public async Task<Person> Post(Person person)
        {
            var p = await db.Persons.AddAsync(person);
            await db.SaveChangesAsync();
            return p.Entity;
        }

        [HttpPut("{id}")]
        public async Task<Results<Ok<Person>, NotFound>> Put(long id, Person person)
        {
            var personForEdit = await db.Persons.Include(p => p.Skills).FirstOrDefaultAsync(p => p.Id == id);
            if (personForEdit == null) return TypedResults.NotFound();
            personForEdit.Name = person.Name;
            personForEdit.DisplayName = person.DisplayName;
            db.Skills.RemoveRange(personForEdit.Skills);
            personForEdit.Skills = person.Skills;
            await db.SaveChangesAsync();
            return TypedResults.Ok(await db.Persons.Include(p => p.Skills).FirstOrDefaultAsync(p => p.Id == id));
        }

        [HttpDelete("{id}")]
        public async Task<Results<Ok<Person>, NotFound>> Delete(long id)
        {
            var person = await db.Persons.Include(p => p.Skills).FirstOrDefaultAsync(p => p.Id == id);
            if (person == null) return TypedResults.NotFound();
            db.Skills.RemoveRange(person.Skills);
            db.Persons.Remove(person);
            await db.SaveChangesAsync();
            return TypedResults.Ok(person);
        }
    }
}
