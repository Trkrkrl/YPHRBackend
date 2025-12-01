using Core.DataAccess.Abstract;
using Core.Entities.Concrete;

namespace DataAccess.Abstract
{
    public interface IUserDal : IBaseRepository<User>
    {
        Task<List<OperationClaim>> GetClaimsAsync(User user);
    }
}
