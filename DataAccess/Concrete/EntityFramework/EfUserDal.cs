using Core.DataAccess.Concrete.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityBaseRepository<User, AppDbContext>, IUserDal
    {
        public Task<List<OperationClaim>> GetClaimsAsync(User user)
        {
            using (var context = new AppDbContext())
            {
                var result = from oclaims in context.OperationClaims
                             join userOperationClaims in context.UserOperationClaims
                                on oclaims.Id equals userOperationClaims.OperationClaimId
                             where userOperationClaims.UserId == user.Id
                             select new OperationClaim
                             {
                                 Id = oclaims.Id,
                                 Name = oclaims.Name
                             };

                return result.ToListAsync();
            }
        }
    }
}
