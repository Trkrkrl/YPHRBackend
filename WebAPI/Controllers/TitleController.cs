using Microsoft.AspNetCore.Mvc;
using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TitleController : ControllerBase
    {
        private readonly ITitleService _titleService;

        public TitleController(ITitleService titleService)
        {
            _titleService = titleService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page, int pageSize, string? searchParam)
        {
            var result = await _titleService.GetAllAsync(page,pageSize, searchParam);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _titleService.GetByIdAsync(id);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Title entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _titleService.AddAsync(entity);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Title entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _titleService.UpdateAsync(entity);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Title entity)
        {
            var result = await _titleService.DeleteAsync(entity);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
