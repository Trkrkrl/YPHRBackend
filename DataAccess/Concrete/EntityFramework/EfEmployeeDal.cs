using Core.DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Context;
using DataAccess.Abstract;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfEmployeeDal : EfEntityBaseRepository<Employee, AppDbContext>, IEmployeeDal
    {
        private AppDbContext dbContext;

        public EfEmployeeDal(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}
