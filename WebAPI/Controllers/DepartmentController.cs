using Microsoft.AspNetCore.Mvc;
using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page , int pageSize,string? searchParam)
        {
            var result = await _departmentService.GetAllAsync(page,pageSize,searchParam);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _departmentService.GetByIdAsync(id);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Department entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _departmentService.AddAsync(entity);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Department entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _departmentService.UpdateAsync(entity);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] Department entity)
        {
            var result = await _departmentService.DeleteAsync(entity);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
