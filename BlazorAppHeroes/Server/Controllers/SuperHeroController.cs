
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAppHeroes.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly DataContext _context;

        public SuperHeroController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetSuperHeroes()
        {
            var heroes = await _context.SuperHeroes.Include(sh => sh.Comic).ToListAsync();
            return Ok(heroes);
        }

        [HttpGet("comics")]
        public async Task<ActionResult<List<Comic>>> GetComics()
        {
            var comics = await _context.Comics.ToListAsync();
            return Ok(comics);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SuperHero>> GetSingleHero(int id)
        {
            var hero = await _context.SuperHeroes
                .Include(h => h.Comic)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (hero == null)
            {
                return NotFound("no hero here");
            }
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateSuperhero(SuperHero hero)
        {
            hero.Comic = null;
            _context.SuperHeroes.Add(hero);
            await _context.SaveChangesAsync();
            return Ok(await GetDbHeroes());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<List<SuperHero>>> UpdateSuperhero(SuperHero hero, int id)
        {
            var dbHero = await _context.SuperHeroes
                .Include(x => x.Comic)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dbHero == null)
                return NotFound("error");

            dbHero.FirstName = hero.FirstName;
            dbHero.LastName = hero.LastName;
            dbHero.HeroName = hero.HeroName;
            dbHero.ComicId = hero.ComicId;

            await _context.SaveChangesAsync();
            return Ok(await GetDbHeroes());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteSuperhero(int id)
        {
            var dbHero = await _context.SuperHeroes
                .Include(x => x.Comic)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (dbHero == null)
                return NotFound("error");
            _context.SuperHeroes.Remove(dbHero);
            await _context.SaveChangesAsync();
            return Ok(await GetDbHeroes());
        }

        private async Task<List<SuperHero>> GetDbHeroes()
        {
            return await _context.SuperHeroes.Include(x => x.Comic).ToListAsync();
        }
    }
}
