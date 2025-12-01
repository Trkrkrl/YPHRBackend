using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IUserService
    {
        Task<DataResult<User>> GetByMailAsync(string email);
        Task<DataResult<User>> GetByIdAsync(Guid userId);

        Task<Result> AddAsync(User user);
        Task<Result> UpdateAsync(User user);
        Task<Result> DeleteAsync(User user);

        Task<Result> UpdatePasswordAsync(UserPasswordUpdateDto userPasswordUpdateDto, Guid userId);

        Task<IDataResult<List<OperationClaim>>> GetClaimsAsync(User user);
    }
}
