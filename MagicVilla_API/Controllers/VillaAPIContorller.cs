using MagicVilla_API.Data;
using MagicVilla_API.Model;
using MagicVilla_API.Model.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
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
        public VillaAPIContorller(ILogger<VillaAPIContorller> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            _logger.LogInformation("Get All villa");
            return Ok(VillaStore.villaList);
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(villa);
        }

        [HttpPost]
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            //model eke requird kiyla dammoth eka check wenne meken. 11 line eke thiyana [ApiController] ekenuth wenne mekamai.
            //eken wenne build in support eka labenwa Data annotation walin.
            //pe pala if condition ekai [ApiController] dekama thibbith valide da ndda kiyla check krnne [ApiController] eken.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // villa stor eke thiyna mamkma dmmoth eka invalid krnna mehema danawa
            if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }
            if (villaDTO == null)
            {
                return BadRequest(villaDTO);
            }

            if (villaDTO.Id > 0)
            {
                return BadRequest();
            }

            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.villaList.Add(villaDTO);
            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        //ActionResult eka dunnth return type eka define krnna wenwa mewidihata "ActionResult<VillaDTO>"
        //eth IActionResult dunnama ehema krnna one na

        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = VillaStore.villaList.FirstOrDefault(u =>u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.villaList.Remove(villa);
            return NoContent();
        }

        [HttpPut("{id:int}", Name ="UpdateVilla")]
        public IActionResult UpdateVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if ( villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }

            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;

            return NoContent();
        }

        //me patch eka add krnna kalin nuget package 2k install krnna one NewtonsoftJson ekai jsonPatch ekai.
        // itapsse program.cs eke AddNewtonsoftJson kiyla controllers wla reg krnna one
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            var vila = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            if (vila == null)
            {
                return NotFound();
            }
            patchDTO.ApplyTo(vila, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return NoContent();

        }
    }
}
