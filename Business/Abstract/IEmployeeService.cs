using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;

namespace Business.Abstract
{
    public interface IEmployeeService
    {
        Task<IDataResult<List<Employee>>> GetAllAsync(int page, int pageSize,string searchParam);

        Task<IDataResult<Employee>> GetByIdAsync(Guid id);

        Task<IResult> AddAsync(Employee entity);
        Task<IResult> UpdateAsync(Employee entity);
        Task<IResult> DeleteAsync(Employee entity);
        Task<IResult> UploadEmployeeImageAsync(IFormFile file, Guid employeeId);
    }
}
