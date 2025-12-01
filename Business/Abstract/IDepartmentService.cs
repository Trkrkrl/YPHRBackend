using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IDepartmentService
    {
        Task<IDataResult<List<Department>>> GetAllAsync(int page, int pageSize, string searchParam);

        Task<IDataResult<Department>> GetByIdAsync(Guid id);

        Task<IResult> AddAsync(Department entity);
        Task<IResult> UpdateAsync(Department entity);
        Task<IResult> DeleteAsync(Department entity);
    }
}
