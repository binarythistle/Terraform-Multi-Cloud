using System.Collections.Generic;
using AutoMapper;
using Client.Data;
using Client.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IClientRepo _repository;
        private readonly IMapper _mapper;

        public HomeController(IClientRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<LogReadDto>> GetAllLogs()
        {
            var logItems = _repository.GetAllLogs();

            return Ok(_mapper.Map<IEnumerable<LogReadDto>>(logItems));
        }
        
    }
}