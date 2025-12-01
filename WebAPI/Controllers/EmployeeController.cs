using Microsoft.AspNetCore.Mvc;
using Business.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using Entities.DTOs;
using System.Globalization;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll(int page, int pageSize,string? searchParam)
        {
            var result = await _employeeService.GetAllAsync(page,pageSize,searchParam);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _employeeService.GetByIdAsync(id);
            if (result.Success)
                return Ok(result);
            return NotFound(result);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EmployeeCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!DateTime.TryParseExact(dto.HireDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var hireDate))
            {
                ModelState.AddModelError(nameof(dto.HireDate), "HireDate formatı geçersiz (yyyy-MM-dd olmalı).");
                return BadRequest(ModelState);
            }
            var entity = new Employee
            {
                Id = dto.Id.HasValue && dto.Id.Value != Guid.Empty
                    ? dto.Id.Value
                : Guid.NewGuid(),

                RegistryNumber = dto.RegistryNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,

                DepartmentId = dto.DepartmentId,
                TitleId = dto.TitleId,

                HireDate = hireDate,
                IsActive = dto.IsActive,

                PhotoPath = null,
                ImagePath = string.Empty
            };

            var result = await _employeeService.AddAsync(entity);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EmployeeCreateUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!dto.Id.HasValue || dto.Id.Value == Guid.Empty)
            {
                ModelState.AddModelError(nameof(dto.Id), "Id zorunludur.");
                return BadRequest(ModelState);
            }

            if (!DateTime.TryParseExact(dto.HireDate, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var hireDate))
            {
                ModelState.AddModelError(nameof(dto.HireDate), "HireDate formatı geçersiz (yyyy-MM-dd olmalı).");
                return BadRequest(ModelState);
            }

          
            var existingResult = await _employeeService.GetByIdAsync(dto.Id.Value);
            if (!existingResult.Success || existingResult.Data == null)
                return BadRequest(existingResult); 

            var entity = existingResult.Data;

            entity.RegistryNumber = dto.RegistryNumber;
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.DepartmentId = dto.DepartmentId;
            entity.TitleId = dto.TitleId;
            entity.HireDate = hireDate;
            entity.IsActive = dto.IsActive;

           

            var result = await _employeeService.UpdateAsync(entity);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingResult = await _employeeService.GetByIdAsync(id);
            if (!existingResult.Success || existingResult.Data == null)
                return BadRequest(existingResult); 

            var result = await _employeeService.DeleteAsync(existingResult.Data);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("uploadimage/{employeeId:guid}")]
        [Consumes("multipart/form-data")] 
        public async Task<IActionResult> UploadImage(
              Guid employeeId,          
              IFormFile Image)         
        {
            var employeeResult = await _employeeService.GetByIdAsync(employeeId);
            if (!employeeResult.Success)
                return NotFound(employeeResult);

            var uploadResult = await _employeeService.UploadEmployeeImageAsync(Image, employeeId);
            if (!uploadResult.Success)
                return BadRequest(uploadResult);

            return Ok(uploadResult);
        }

    }
}
