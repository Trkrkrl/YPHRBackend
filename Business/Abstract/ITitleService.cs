using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface ITitleService
    {
        Task<IDataResult<List<Title>>> GetAllAsync(int page, int pageSize,string searchParam);

        Task<IDataResult<Title>> GetByIdAsync(Guid id);

        Task<IResult> AddAsync(Title entity);
        Task<IResult> UpdateAsync(Title entity);
        Task<IResult> DeleteAsync(Title entity);
    }
}
