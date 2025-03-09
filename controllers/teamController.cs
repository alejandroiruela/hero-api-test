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
    public class TeamController(AppDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _DbContext = context;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<List<TeamDTO>>> GetTeams()
        {
            var teams = await _DbContext.Teams.ToListAsync();
            var listTeams = _mapper.Map<List<TeamDTO>>(teams);
            return Ok(listTeams);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDTO>> GetTeam(int id)
        {
            var team = await _DbContext
                .Teams.Include(t => t.Heroes) // Incluye los héroes
                .ThenInclude(h => h.Habilities) // Incluye las habilidades de los héroes (si Heroes no es null)
                .Include(t => t.Villains) // Incluye los villanos
                .ThenInclude(v => v.Habilities) // Incluye las habilidades de los villanos (si Villains no es null)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (team == null)
            {
                return NotFound("El equipo solicitado no existe en la base de datos");
            }
            var resultedTeam = _mapper.Map<TeamDTO>(team);

            return Ok(resultedTeam);
        }

        [HttpPost]
        public async Task<ActionResult<TeamDTO>> CreateTeam(CreateTeamDTO team)
        {
            var newTeam = _mapper.Map<Team>(team);
            try
            {
                await _DbContext.AddAsync(newTeam);
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error de comunicacion del servidor con la base de datos");
            }

            var teamDTO = _mapper.Map<TeamDTO>(newTeam);
            return CreatedAtAction(nameof(GetTeam), new { teamName = teamDTO.TeamName }, teamDTO);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TeamDTO>> UpdateTeam(int id, TeamDTO team)
        {
            var retrievedTeam = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (retrievedTeam == null)
            {
                return NotFound("No existe un equipo con ese nombre asociado");
            }

            if (retrievedTeam.TeamName != team.TeamName)
            {
                bool existingTeamName = await _DbContext.Teams.AnyAsync(t =>
                    t.TeamName == team.TeamName
                );
                if (existingTeamName)
                {
                    return BadRequest("Ya existe ese nombre de equipo en otro equipo");
                }
            }
            _mapper.Map(team, retrievedTeam);
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

            return Ok(team);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TeamDTO>> DeleteTeam(int id)
        {
            var deletedTeam = await _DbContext.Teams.FirstOrDefaultAsync(t => t.Id == id);
            if (deletedTeam == null)
            {
                return NotFound("El equipo seleccionado no existe");
            }
            _DbContext.Teams.Remove(deletedTeam);
            try
            {
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error al intentar la operacion en la base de datos");
            }
            var returnedTeam = _mapper.Map<TeamDTO>(deletedTeam);
            return Ok(returnedTeam);
        }

        [HttpPut("{idTeam}/hero/{idHero}")]
        public async Task<ActionResult<HeroDTO>> AddHeroMember(int idTeam, int idHero)
        {
            var existingHero = await _DbContext
                .Heroes.Include(h => h.Habilities)
                .FirstOrDefaultAsync(h => h.Id == idHero);
            var existingTeam = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == idTeam);
            if (existingHero == null)
            {
                return NotFound("El heroe no esta registrado en la base de datos");
            }
            if (existingTeam == null)
            {
                return NotFound("El equipo no esta registrado en la base de datos");
            }
            else if (existingTeam.Villains?.Count != 0)
            {
                return BadRequest("El equipo especificado por parametros es de villanos");
            }
            existingHero.TeamId = existingTeam.Id;
            try
            {
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    500,
                    "Error al actualizar el equipo del heroe en la base de datos"
                );
            }
            var heroDTO = _mapper.Map<HeroDTO>(existingHero);

            return Ok(heroDTO);
        }

        [HttpPut("{idTeam}/hero/bulkUpdate")]
        public async Task<ActionResult<List<HeroDTO>>> addHeroMemberBulk(int idTeam, int[] idHeroes)
        {
            var existingTeam = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == idTeam);
            if (existingTeam == null)
            {
                return NotFound("El equipo especificado por ID no existe en la base de datos");
            }
            else if (existingTeam.Villains?.Count != 0)
            {
                return BadRequest("El equipo especificdo por ID es solamente de villanos");
            }
            List<Hero> heroes = [];
            List<int> ids = [];
            foreach (var id in idHeroes)
            {
                var existingHero = await _DbContext
                    .Heroes.Include(h => h.Habilities)
                    .FirstOrDefaultAsync(h => h.Id == id);
                if (existingHero == null)
                {
                    ids.Add(id);
                }
                else
                {
                    existingHero.TeamId = existingTeam.Id;
                    heroes.Add(existingHero);
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

            if (ids.Count > 0)
            {
                return Ok(
                    new
                    {
                        Message = "Algunos heroes fueron actualizados al nuevo equipo, otros no fueron encontrados en la base de datos",
                        Heroes = _mapper.Map<List<HeroDTO>>(heroes),
                        NotFound = ids,
                    }
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        Message = "Los heroes especificados fueron actualizados correctamente",
                        Heroes = _mapper.Map<List<HeroDTO>>(heroes),
                    }
                );
            }
        }

        [HttpDelete("{idTeam}/hero/bulkDelete")]
        public async Task<ActionResult<List<HeroDTO>>> DeleteHeroMemberBulk(
            int idTeam,
            int[] idHeroes
        )
        {
            var existingTeam = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == idTeam);
            if (existingTeam == null)
            {
                return NotFound("El equipo especificado por ID no existe en la base de datos");
            }
            else if (existingTeam.Villains?.Count != 0)
            {
                return BadRequest("El equipo especificdo por ID es solamente de villanos");
            }
            List<Hero> heroes = [];
            List<int> ids = [];
            foreach (var id in idHeroes)
            {
                var existingHero = await _DbContext
                    .Heroes.Include(h => h.Habilities)
                    .FirstOrDefaultAsync(h => h.Id == id);
                if (existingHero == null)
                {
                    ids.Add(id);
                }
                else
                {
                    existingHero.TeamId = null;
                    heroes.Add(existingHero);
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

            if (ids.Count > 0)
            {
                return Ok(
                    new
                    {
                        Message = "Algunos heroes fueron actualizados al nuevo equipo, otros no fueron encontrados en la base de datos",
                        Heroes = _mapper.Map<List<HeroDTO>>(heroes),
                        NotFound = ids,
                    }
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        Message = "Los heroes especificados fueron actualizados correctamente",
                        Heroes = _mapper.Map<List<HeroDTO>>(heroes),
                    }
                );
            }
        }

        [HttpDelete("{idTeam}/hero/{idHero}")]
        public async Task<ActionResult<HeroDTO>> RemoveHermoMember(int idTeam, int idHero)
        {
            var existingHero = await _DbContext
                .Heroes.Include(h => h.Habilities)
                .FirstOrDefaultAsync(h => h.Id == idHero);
            if (existingHero == null)
            {
                return NotFound(
                    "El heroe especificado por parametros no existe en la base de datos"
                );
            }

            var existingTeam = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == idTeam);
            if (existingTeam == null)
            {
                return NotFound(
                    "El equipo especificado por parametros no existe en la base de datos"
                );
            }
            else if (existingTeam.Villains?.Count != 0)
            {
                return BadRequest("El equipo especificado es de villanos");
            }

            existingHero.TeamId = null;
            try
            {
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error en el servidor al actualizar el equipo del heroe");
            }
            var heroDTO = _mapper.Map<HeroDTO>(existingHero);
            return Ok(heroDTO);
        }

        [HttpPut("{idTeam}/villain/add/{idVillain}")]
        public async Task<ActionResult<VillainDTO>> AddVillainMember(int idTeam, int idVillain)
        {
            var existingTeam = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == idTeam);
            if (existingTeam == null)
            {
                return NotFound("El equipo especificado por parametros no existe");
            }
            else if (existingTeam.Heroes?.Count != 0)
            {
                return BadRequest("El equipo especificado es de heroes, no de villanos");
            }
            var existingVillain = await _DbContext
                .Villains.Include(v => v.Habilities)
                .FirstOrDefaultAsync(v => v.Id == idVillain);
            if (existingVillain == null)
            {
                return NotFound(
                    "El villano especificado por parametros no existe en la base de datos"
                );
            }
            existingVillain.TeamId = existingTeam.Id;
            try
            {
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error de comunicacion en el servidor con la base de datos");
            }

            var villainDTO = _mapper.Map<VillainDTO>(existingVillain);
            return Ok(villainDTO);
        }

        [HttpDelete("{idTeam}/villain/del/{idVillain}")]
        public async Task<ActionResult<VillainDTO>> DelVillainMember(int idTeam, int idVillain)
        {
            var existingTeam = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == idTeam);
            if (existingTeam == null)
            {
                return NotFound(
                    "El equipo especificado por parametros no existe en la base de datos"
                );
            }
            else if (existingTeam.Heroes?.Count != 0)
            {
                return BadRequest("El equipo especificado por parametros es de heroes.");
            }

            var existingVillain = await _DbContext
                .Villains.Include(v => v.Habilities)
                .FirstOrDefaultAsync(v => v.Id == idVillain);
            if (existingVillain == null)
            {
                return NotFound(
                    "El villano especificado por parametros no existe en la base de datos"
                );
            }
            existingVillain.TeamId = null;
            try
            {
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error de comunicacion en el servidor con la base de datos");
            }
            var villainDTO = _mapper.Map<VillainDTO>(existingVillain);
            return Ok(villainDTO);
        }

        [HttpPut("{idTeam}/villain/bulkUpdate")]
        public async Task<ActionResult<List<VillainDTO>>> AddVillainsMemberBulk(
            int idTeam,
            int[] idVillains
        )
        {
            var team = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == idTeam);
            if (team == null)
            {
                return NotFound(
                    "No existe el equipo especificado por parametros en la base de datos"
                );
            }
            else if (team.Heroes?.Count != 0)
            {
                return BadRequest("El Equipo especificado es de heroes, no de villanos");
            }
            List<Villain> villains = [];
            List<int> notFounds = [];
            foreach (var id in idVillains)
            {
                var existingVillain = await _DbContext.Villains.FirstOrDefaultAsync(v =>
                    v.Id == id
                );
                if (existingVillain == null)
                {
                    notFounds.Add(id);
                }
                else
                {
                    existingVillain.TeamId = team.Id;
                    villains.Add(existingVillain);
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
            if (notFounds.Count > 0)
            {
                return Ok(
                    new
                    {
                        Message = "Algunos Villanos se actualizaron, pero otros no se encontraron",
                        updatedVillains = _mapper.Map<List<VillainDTO>>(villains),
                        notFounds,
                    }
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        Message = "Se actualizaron todos los miembros del equipo con exito",
                        team.TeamName,
                        UpdatedMembers = _mapper.Map<List<VillainDTO>>(villains),
                    }
                );
            }
        }

        [HttpDelete("{idTeam}/villain/bulkDelete")]
        public async Task<ActionResult<List<VillainDTO>>> DeleteVillainsMemberBulk(
            int idTeam,
            int[] idVillains
        )
        {
            var team = await _DbContext
                .Teams.Include(t => t.Heroes)
                .Include(t => t.Villains)
                .FirstOrDefaultAsync(t => t.Id == idTeam);
            if (team == null)
            {
                return NotFound(
                    "No existe el equipo especificado por parametros en la base de datos"
                );
            }
            else if (team.Heroes?.Count != 0)
            {
                return BadRequest("El Equipo especificado es de heroes, no de villanos");
            }
            List<Villain> villains = [];
            List<int> notFounds = [];
            foreach (var id in idVillains)
            {
                var existingVillain = await _DbContext.Villains.FirstOrDefaultAsync(v =>
                    v.Id == id
                );
                if (existingVillain == null)
                {
                    notFounds.Add(id);
                }
                else
                {
                    existingVillain.TeamId = null;
                    villains.Add(existingVillain);
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
            if (notFounds.Count > 0)
            {
                return Ok(
                    new
                    {
                        Message = "Algunos Villanos se actualizaron, pero otros no se encontraron",
                        updatedVillains = _mapper.Map<List<VillainDTO>>(villains),
                        notFounds,
                    }
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        Message = "Se actualizaron todos los miembros del equipo con exito",
                        team.TeamName,
                        UpdatedMembers = _mapper.Map<List<VillainDTO>>(villains),
                    }
                );
            }
        }
    }
}
