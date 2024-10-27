using AutoMapper;
using MagicVilla_API.Data;
using MagicVilla_API.Model;
using MagicVilla_API.Model.Dto;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
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
    public class VillaContorller : ControllerBase
    {

        //meken wenne app eke run krankota open wena CMD file ele log krna ena.
        //get all eke krala thiynne eka. me krala thiyna eka default ena widiha
        //serilog use krla custom widihata meka hadanna puluwan ekata Serilog.AspNetCore, Serilog.Sinks.File packege deka install krgnna one
        //me krana log eken api krna dewal wenama log file ekak hadila ele log wenwa. packages deka install krala program.cs eke configer krnna one
        private readonly ILogger<VillaContorller> _logger;

        //Automapper user krnna ek inject krnwa
        private readonly IMapper _mapper;
        private readonly IVillaRepository _dbVilla;
        protected APIResponce _responce;
        public VillaContorller(ILogger<VillaContorller> logger, IVillaRepository dbVilla, IMapper mapper)
        {
            _logger = logger;
            _dbVilla = dbVilla;
            _mapper = mapper;
            _responce = new APIResponce();
        }

        [HttpGet]
        public async Task<ActionResult<APIResponce>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Get All villa");
                IEnumerable<Villa> villaLsit = await _dbVilla.GetAllAsync();
                var mappng = _mapper.Map<List<VillaDTO>>(villaLsit);
                _responce.Result = mappng;
                _responce.StatusCode = HttpStatusCode.OK;
                _responce.IsSuccess = true;
                return Ok(_responce);
            }
            catch (Exception ex)
            {
                _responce.IsSuccess = false;
                _responce.ErrorMessages = new List<string>() { ex.Message };
            }
            return _responce;

        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        public async Task<ActionResult<APIResponce>> GetVilla(int id)
        {
            if (id == 0)
            {
                _responce.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_responce);
            }
            else
            {
                var villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }
                var mapping = _mapper.Map<VillaDTO>(villa);
                _responce.Result = mapping;
                _responce.StatusCode = HttpStatusCode.OK;
                _responce.IsSuccess = true;
                return Ok(_responce);
            }

        }

        [HttpPost]
        public async Task<ActionResult<APIResponce>> CreateVilla([FromBody] VillaCreateDTO villaCreateDTO)
        {
            //model eke requird kiyla dammoth eka check wenne meken. 11 line eke thiyana [ApiController] ekenuth wenne mekamai.
            //eken wenne build in support eka labenwa Data annotation walin.
            //me pala if condition ekai [ApiController] dekama thibbith valide da ndda kiyla check krnne [ApiController] eken.
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // villa eke thiyna namkma dmmoth eka invalid krnna mehema danawa
            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == villaCreateDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }
            if (villaCreateDTO == null)
            {
                return BadRequest(villaCreateDTO);
            }
            //methana Villa model eken onject ekak hadnwa model kiyla. itpsse villaCreateDTO eken ena data tikaVilla ekata map krnwa
            Villa villa = _mapper.Map<Villa>(villaCreateDTO);

            await _dbVilla.CreateAsync(villa);

            _responce.Result = _mapper.Map<Villa>(villa);
            _responce.StatusCode = HttpStatusCode.Created;
            _responce.IsSuccess = true;
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _responce);
        }

        //ActionResult eka dunnth return type eka define krnna wenwa mewidihata "ActionResult<VillaDTO>"
        //eth IActionResult dunnama ehema krnna one na

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public async Task<ActionResult<APIResponce>> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            await _dbVilla.RemoveAsync(villa);
            _responce.Result = villa;
            _responce.StatusCode = HttpStatusCode.NoContent;
            _responce.IsSuccess = true;
            return Ok(_responce);
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        public async Task<ActionResult<APIResponce>> UpdateVilla(int id, [FromBody] VillaUpdateDTO villaUpdateDTO)
        {
            if (villaUpdateDTO == null || id != villaUpdateDTO.Id)
            {
                return BadRequest();
            }
            Villa villa = _mapper.Map<Villa>(villaUpdateDTO);

            await _dbVilla.UpdateAsync(villa);
            _responce.Result = villa;
            _responce.StatusCode = HttpStatusCode.NoContent;
            _responce.IsSuccess = true;
            return Ok(_responce);
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
            var villa = await _dbVilla.GetAsync(v => v.Id == id, tracked: false);
            VillaUpdateDTO villaUpdateDTO = _mapper.Map<VillaUpdateDTO>(villa);


            if (villa == null)
            {
                return NotFound();
            }
            patchDTO.ApplyTo(villaUpdateDTO, ModelState);

            Villa model = _mapper.Map<Villa>(patchDTO);

            await _dbVilla.UpdateAsync(model);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return NoContent();

        }
    }
}
