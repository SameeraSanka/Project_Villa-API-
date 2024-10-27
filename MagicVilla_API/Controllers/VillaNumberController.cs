using AutoMapper;
using MagicVilla_API.Model;
using MagicVilla_API.Model.Dto;
using MagicVilla_API.Model.Dto.VillaNumber;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberController : ControllerBase
    {
        private readonly ILogger<VillaNumberController> _logger;
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _dbVillaNo;
        public APIResponce _responce;

        //DB eke nathi villa ID ekak enter kroth error message ekak pennanna one nisa VillaReop eka inject krnwa
        private readonly IVillaRepository _dbVilla;

        public VillaNumberController(ILogger<VillaNumberController> logger, IMapper mapper, IVillaNumberRepository dbVillaNo, IVillaRepository dbVilla)
        {
            _logger = logger;
            _mapper = mapper;
            _dbVillaNo = dbVillaNo;
            _responce = new APIResponce();
            _dbVilla = dbVilla;
        }

        [HttpGet]
        public async Task<ActionResult<APIResponce>> GetVillaNumbers()
        {
            _logger.LogInformation("Get All villaNumbers");
            IEnumerable<VillaNumber> villaNumbers = await _dbVillaNo.GetAllAsync();
            _responce.Result = villaNumbers;
            _responce.StatusCode = HttpStatusCode.OK;
            _responce.IsSuccess = true;
            return (_responce);
        }

        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        public async Task<ActionResult<APIResponce>> GetVillaNumber(int id)
        {
            if (id == 0)
            {
                _responce.StatusCode = HttpStatusCode.BadRequest;
                return (_responce);
            }
            var villaNo = await _dbVillaNo.GetAsync(u => u.VillaNo == id);
            if (villaNo != null)
            {
                _responce.Result = villaNo;
                _responce.StatusCode = HttpStatusCode.OK;
                _responce.IsSuccess = true;
                return (_responce);
            }
            _responce.StatusCode = HttpStatusCode.BadRequest;
            return (_responce);
        }

        [HttpPost]
        public async Task<ActionResult<APIResponce>> CreateVillaNumber([FromBody] VillaNumberCreateDTO villaNumberCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                _responce.StatusCode = HttpStatusCode.BadRequest;
                return (_responce);
            }
            if(await _dbVilla.GetAsync(u =>u.Id == villaNumberCreateDTO.VillaID) == null)
            {
                ModelState.AddModelError("CustomError", "Villa is not Exist");
                return BadRequest(ModelState);
            }
            if (await _dbVillaNo.GetAsync(u => u.VillaNo == villaNumberCreateDTO.VillaNo) != null)
            {
                ModelState.AddModelError("CustomError", "Villa Number Already Exist");
                return BadRequest(ModelState);
            }
            if (villaNumberCreateDTO != null)
            {
                VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberCreateDTO);
                await _dbVillaNo.CreateAsync(villaNumber);

                _responce.Result = villaNumber;
                _responce.StatusCode = HttpStatusCode.Created;
                _responce.IsSuccess = true;
                return (_responce);
            }
            _responce.StatusCode = HttpStatusCode.BadRequest;
            return (_responce);
        }

        [HttpDelete("{id:int}", Name = "VillaNumberDelete")]
        public async Task<ActionResult<APIResponce>> DeleteVillaNumber(int id)
        {
            if (id == 0)
            {
                _responce.StatusCode = HttpStatusCode.BadRequest;
                return (_responce);
            }
            var villaNumber = await _dbVillaNo.GetAsync(u => u.VillaNo == id);
            if (villaNumber != null)
            {
                await _dbVillaNo.RemoveAsync(villaNumber);
                _responce.Result = villaNumber;
                _responce.StatusCode=HttpStatusCode.OK;
                _responce.IsSuccess = true;
                return (_responce);
            }
            _responce.StatusCode = HttpStatusCode.BadRequest;
            return (_responce);
        }

        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        public async Task<ActionResult<APIResponce>> UpdateVillaNumber(int id, [FromBody]VillaNumberUpdateDTO villaNumberUpdateDTO)
        {
            if(villaNumberUpdateDTO == null || id != villaNumberUpdateDTO.VillaNo)
            {
                _responce.StatusCode = HttpStatusCode.BadRequest;
                return (_responce);
            }
            if (await _dbVilla.GetAsync(u => u.Id == villaNumberUpdateDTO.VillaID) == null)
            {
                ModelState.AddModelError("CustomError", "Villa is not Exist");
                return BadRequest(ModelState);
            }
            VillaNumber villaNumber = _mapper.Map<VillaNumber>(villaNumberUpdateDTO);

            if (villaNumber != null)
            {
                await _dbVillaNo.UpdateAsync(villaNumber);
                _responce.Result = villaNumber;
                _responce.StatusCode=HttpStatusCode.OK;
                _responce.IsSuccess = true;
                return (_responce);
            }
            _responce.StatusCode = HttpStatusCode.BadRequest;
            return (_responce);
        }
    }
}
