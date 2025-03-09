using AutoMapper;
using Heroes.Database;
using Heroes.Models;
using Heroes.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Heroes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _DbContext = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet("findAll")]
        public async Task<ActionResult<IEnumerable<HeroDTO>>> GetHeroes()
        {
            var heroes = await _DbContext
                .Heroes.Include(h => h.Habilities)
                .Include(h => h.Team)
                .ToListAsync();
            var heroesList = _mapper.Map<List<HeroDTO>>(heroes);
            return Ok(heroesList);
        }

        [HttpGet("findById/{id}")]
        public async Task<ActionResult<HeroDTO>> GetHero(int id)
        {
            var hero = await _DbContext
                .Heroes.Include(h => h.Habilities)
                .Include(h => h.Team)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hero == null)
            {
                return NotFound();
            }
            var returnedHero = _mapper.Map<HeroDTO>(hero);
            return Ok(returnedHero);
        }

        [HttpPost("createHero")]
        public async Task<ActionResult<Hero>> NewHero(HeroCreateUpdateDTO hero)
        {
            var newHero = _mapper.Map<Hero>(hero);
            _DbContext.Heroes.Add(newHero);
            await _DbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetHero), new { id = newHero.Id }, hero);
        }

        [HttpPost("createHeroBulk")]
        public async Task<ActionResult<List<HeroDTO>>> CreateHeroBulk(
            List<HeroCreateUpdateDTO> heroesDTO
        )
        {
            List<Hero> createdHeroes = [];
            List<HeroCreateUpdateDTO> alreadyExists = [];
            foreach (HeroCreateUpdateDTO hero in heroesDTO)
            {
                bool existingHero = await _DbContext.Heroes.AnyAsync(h =>
                    h.Hero_name == hero.HeroName
                );
                if (existingHero)
                {
                    alreadyExists.Add(hero);
                }
                else
                {
                    Hero her = _mapper.Map<Hero>(hero);
                    createdHeroes.Add(her);
                    await _DbContext.AddAsync(her);
                }
            }
            try
            {
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    500,
                    "Error en la comunicacion del servidor con la base de datos"
                );
            }
            if (alreadyExists.Count > 0)
            {
                return Ok(
                    new
                    {
                        Message = "Algunos heroes fueron creados, otros ya existina en la base de datos",
                        createdHeroes = new
                        {
                            HeroDTOs = _mapper.Map<List<HeroDTO>>(createdHeroes),
                        },
                        AlreadyExistHeroes = new { alreadyExists },
                    }
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        Message = "Los heroes fueron registrados exitosamente en la base de datos",
                        createdHeroes = _mapper.Map<HeroDTO>(createdHeroes),
                    }
                );
            }
        }

        // PUT: api/hero/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutHero(int id, HeroCreateUpdateDTO hero)
        {
            if (id <= 0)
            {
                return BadRequest("El Id debe ser mayor a 0");
            }
            var existingHero = await _DbContext
                .Heroes.Include(h => h.Habilities)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (existingHero == null)
            {
                return NotFound("El heroe seleccionado no esta disponible en la base de datos");
            }

            _DbContext.Habilities.RemoveRange(existingHero.Habilities);

            _mapper.Map(hero, existingHero);
            existingHero.Habilities =
            [
                .. hero.Habilities.Select(h => new Hability { Name = h.Name }),
            ];

            try
            {
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            var heroDTO = _mapper.Map<HeroDTO>(existingHero);
            return Ok(heroDTO);
        }

        // DELETE: api/hero/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteHero(int id)
        {
            var hero = await _DbContext.Heroes.FindAsync(id);
            if (hero == null)
            {
                return NotFound();
            }

            _DbContext.Heroes.Remove(hero);
            await _DbContext.SaveChangesAsync();

            return Ok(hero);
        }

        [HttpDelete("deleteHeroBulk")]
        public async Task<ActionResult<List<HeroDTO>>> deleteHeroBulk(List<HeroDTO> heroes)
        {
            List<HeroDTO> deletedHeroes = [];
            List<HeroDTO> notFoundHeroes = [];

            foreach (HeroDTO heroDTO in heroes)
            {
                var existingHero = await _DbContext.Heroes.AnyAsync(h =>
                    h.Hero_name == heroDTO.HeroName
                );
                if (existingHero)
                {
                    deletedHeroes.Add(heroDTO);
                    Hero her = _mapper.Map<Hero>(heroDTO);
                    _DbContext.Remove(her);
                }
                else
                {
                    notFoundHeroes.Add(heroDTO);
                }
            }
            if (notFoundHeroes.Count > 0)
            {
                return Ok(
                    new
                    {
                        Message = "Algunos heroes fueron eliminados, otros no existen en la base de datos",
                        deletedHeroes = new { deletedHeroes },
                        notFoundHeroes = new { notFoundHeroes },
                    }
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        Message = "Se eliminaron exitosamente los registros de heroes seleccionados",
                        deletedHeroes = new { deletedHeroes },
                    }
                );
            }
        }

        private bool HeroExists(int id)
        {
            return _DbContext.Heroes.Any(e => e.Id == id);
        }
    }
}
