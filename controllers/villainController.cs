using System.Data;
using AutoMapper;
using Heroes.Database;
using Heroes.Models;
using Heroes.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Heroes.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class VillainController(AppDbContext database, IMapper mapper) : ControllerBase
    {
        private readonly AppDbContext _appDBContext = database;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<List<VillainDTO>>> GetAll()
        {
            var villains = await _appDBContext.Villains.Include(h => h.Habilities).ToListAsync();
            var villainList = _mapper.Map<List<VillainDTO>>(villains);
            return Ok(villainList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VillainDTO>> GetVillain(int id)
        {
            var retrievedVillain = await _appDBContext
                .Villains.Include(h => h.Habilities)
                .Include(v => v.Team)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (retrievedVillain == null)
            {
                return NotFound("El Villano no existe en la base de datos");
            }
            var villain = _mapper.Map<VillainDTO>(retrievedVillain);

            return Ok(villain);
        }

        [HttpPost]
        public async Task<ActionResult<VillainDTO>> PostVillain(CreateVillainDTO villain)
        {
            var existing_villain = await _appDBContext.Villains.FirstOrDefaultAsync(v =>
                v.Villain_Name == villain.Villain_Name
            );

            if (existing_villain != null)
            {
                return BadRequest("El nombre de villano especificado ya existe");
            }
            var newVillain = _mapper.Map<Villain>(villain);

            _appDBContext.Add(newVillain);
            await _appDBContext.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetVillain),
                new { villain_name = newVillain.Villain_Name },
                villain
            );
        }

        [HttpPost("bulkCreate")]
        public async Task<ActionResult<List<VillainDTO>>> CreateVillainsBulk(
            CreateVillainDTO[] villains
        )
        {
            List<Villain> villainsList = [];
            List<CreateVillainDTO> alreadyExist = [];
            foreach (CreateVillainDTO villainDTO in villains)
            {
                bool existingVillain = await _appDBContext.Villains.AnyAsync(v =>
                    v.Villain_Name == villainDTO.Villain_Name
                );
                if (existingVillain)
                {
                    alreadyExist.Add(villainDTO);
                }
                else
                {
                    Villain villain = _mapper.Map<Villain>(villainDTO);
                    villainsList.Add(villain);
                    await _appDBContext.AddAsync(villain);
                }
            }
            try
            {
                await _appDBContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    500,
                    "Error en la comunicacion del servidor con la base de datos"
                );
            }
            if (alreadyExist.Count > 0)
            {
                return Ok(
                    new
                    {
                        Message = "Algunos heroes fueron registrados, otros ya existian en la base de datos",
                        createdVillains = _mapper.Map<List<VillainDTO>>(villainsList),
                        alreadyExist,
                    }
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        Message = "Villanos creados con exito",
                        Villains = _mapper.Map<List<VillainDTO>>(villainsList),
                    }
                );
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<VillainDTO>> UpdateVillain(int id, VillainDTO villain)
        {
            var fetchedVillain = await _appDBContext
                .Villains.Include(h => h.Habilities)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (fetchedVillain == null)
            {
                return NotFound("El villano especificado no existe en la Base de datos");
            }

            if (fetchedVillain.Villain_Name != villain.Villain_Name)
            {
                var existing_villain = await _appDBContext.Villains.AnyAsync(v =>
                    v.Villain_Name == villain.Villain_Name
                );

                if (existing_villain)
                {
                    return BadRequest("El Nombre de Villano ya esta en uso en la base de datos");
                }
            }

            _appDBContext.RemoveRange(fetchedVillain.Habilities);
            _mapper.Map(villain, fetchedVillain);
            fetchedVillain.Habilities =
            [
                .. villain.Habilities.Select(h => new Hability { Name = h.Name }),
            ];
            try
            {
                await _appDBContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Error de comunicacion en el servidor con la base de datos");
            }
            return Ok(villain);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<VillainDTO>> DeleteVillain(int id)
        {
            var existing_villain = await _appDBContext
                .Villains.Include(v => v.Habilities)
                .FirstOrDefaultAsync(v => v.Id == id);
            if (existing_villain == null)
            {
                return NotFound("El villano solicitado para borrar no existe en la base de datos");
            }

            _appDBContext.Remove(existing_villain);

            await _appDBContext.SaveChangesAsync();

            return Ok("Villano eliminado con exito");
        }

        [HttpDelete("bulkDelete")]
        public async Task<ActionResult<List<VillainDTO>>> DeleteVillainBulk(
            List<VillainDTO> villains
        )
        {
            List<VillainDTO> deletedVillains = [];
            List<VillainDTO> notFoundVillains = [];
            foreach (VillainDTO villainDTO in villains)
            {
                var existingVillain = await _appDBContext.Villains.AnyAsync(v =>
                    v.Villain_Name == villainDTO.Villain_Name
                );
                if (existingVillain)
                {
                    deletedVillains.Add(villainDTO);
                    var villain = _mapper.Map<Villain>(villainDTO);
                    _appDBContext.Remove(villain);
                }
                else
                {
                    notFoundVillains.Add(villainDTO);
                }
            }
            try
            {
                await _appDBContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(
                    500,
                    "Error en la comunicacion del servidor con la base de datos"
                );
            }
            if (notFoundVillains.Count > 0)
            {
                return Ok(
                    new
                    {
                        Message = "Algunos villanos fueron eliminados, otros no fueron encontrados en la base de datos",
                        deletedVillains = villains,
                        notFoundVillains,
                    }
                );
            }
            else
            {
                return Ok(
                    new
                    {
                        Message = "Los villanos seleccionados fueron eliminados con exito",
                        deletedVillains = villains,
                    }
                );
            }
        }

        private bool VillainExists(int id)
        {
            return _appDBContext.Villains.Any(e => e.Id == id);
        }
    };
}
