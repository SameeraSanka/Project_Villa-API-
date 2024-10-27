using MagicVilla_API.Data;
using MagicVilla_API.Model;
using MagicVilla_API.Model.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public class VillaAPIContorller : ControllerBase
    {

        //meken wenne app eke run krankota open wena CMD file ele log krna ena.
        //get all eke krala thiynne eka. me krala thiyna eka default ena widiha
        //serilog use krla custom widihata meka hadanna puluwan ekata Serilog.AspNetCore, Serilog.Sinks.File packege deka install krgnna one
        //me krana log eken api krna dewal wenama log file ekak hadila ele log wenwa. packages deka install krala program.cs eke configer krnna one
        private readonly ILogger<VillaAPIContorller> _logger;
        private readonly ApplicationDbContext _db;
        public VillaAPIContorller(ILogger<VillaAPIContorller> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.LogInformation("Get All villa");
            return Ok(await _db.Villas.ToListAsync());
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            else
            {
                var villa =await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                return Ok(villa);
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<VillaCreateDTO>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
        {
            //model eke requird kiyla dammoth eka check wenne meken. 11 line eke thiyana [ApiController] ekenuth wenne mekamai.
            //eken wenne build in support eka labenwa Data annotation walin.
            //me pala if condition ekai [ApiController] dekama thibbith valide da ndda kiyla check krnne [ApiController] eken.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // villa eke thiyna namkma dmmoth eka invalid krnna mehema danawa
            if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villaCreateDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }
            if (villaCreateDTO == null)
            {
                return BadRequest(villaCreateDTO);
            }

            //if (villaDTO.Id > 0)
            //{
            //    return BadRequest();
            //}
            Villa model = new()
            {
                //Id = villaDTO.Id,
                Name = villaCreateDTO.Name,
                Details = villaCreateDTO.Details,
                Rate = villaCreateDTO.Rate,
                Amenity = villaCreateDTO.Amenity,
                Sqft = villaCreateDTO.Sqft,
                ImageUrl = villaCreateDTO.ImageUrl,
                Occupancy = villaCreateDTO.Occupancy,
               
                
            };

            await _db.Villas.AddAsync(model);
            await _db.SaveChangesAsync();
            
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }

        //ActionResult eka dunnth return type eka define krnna wenwa mewidihata "ActionResult<VillaDTO>"
        //eth IActionResult dunnama ehema krnna one na

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}", Name ="UpdateVilla")]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO villaUpdateDTO)
        {
            if (villaUpdateDTO == null || id != villaUpdateDTO.Id)
            {
                return BadRequest();
            }

            Villa model = new Villa()
            {
                Amenity = villaUpdateDTO.Amenity,
                Details = villaUpdateDTO.Details,
                Id = villaUpdateDTO.Id,
                ImageUrl = villaUpdateDTO.ImageUrl,
                Name = villaUpdateDTO.Name,
                Occupancy = villaUpdateDTO.Occupancy,
                Rate = villaUpdateDTO.Rate,
                Sqft = villaUpdateDTO.Sqft,
            };
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        //me patch eka add krnna kalin nuget package 2k install krnna one NewtonsoftJson ekai jsonPatch ekai.
        // itapsse program.cs eke AddNewtonsoftJson kiyla controllers wla reg krnna one
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);
            VillaUpdateDTO villaUpdateDTO = new()
            {
                Amenity = villa.Amenity,
                Details = villa.Details,
                Id = villa.Id,
                ImageUrl = villa.ImageUrl,
                Name = villa.Name,
                Occupancy = villa.Occupancy,
                Rate = villa.Rate,
                Sqft = villa.Sqft,
            };

            if (villa == null)
            {
                return NotFound();
            }
            patchDTO.ApplyTo(villaUpdateDTO, ModelState);

            Villa model = new Villa()
            {
                Amenity = villaUpdateDTO.Amenity,
                Details = villaUpdateDTO.Details,
                Id = villaUpdateDTO.Id,
                ImageUrl = villaUpdateDTO.ImageUrl,
                Name = villaUpdateDTO.Name,
                Occupancy = villaUpdateDTO.Occupancy,
                Rate = villaUpdateDTO.Rate,
                Sqft = villaUpdateDTO.Sqft,
            };
            _db.Villas.Update(model);
            await _db.SaveChangesAsync();
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return NoContent();

        }
    }
}
